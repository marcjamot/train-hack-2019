using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Newtonsoft.Json;
using UnityEngine;

public class TrafikverketClient
{
    private static HttpClient httpClient = new HttpClient();
    private string baseUrl = "https://api.trafikinfo.trafikverket.se/v2/data.json";

    public async Task<IEnumerable<TrainMessage>> GetTrainMessages() 
    {
        var request = new TrafikverketRequest {
            Login = new Login {
                Authenticationkey = "afa4cc0e8b9b43789d155b296a58ddca"
            },
            Query = new Query {
                Objecttype = "TrainMessage",
                Schemaversion = "1.4",
                Limit = "10"
            }
        };

        var response = await MakeRequest<TrainMessageResponse>(request);

        return response.TrainMessage;
    }

    public async Task<IEnumerable<TripInformation>> GetTripInformation(string bookingNumber) 
    {
        var request = new TrafikverketRequest 
        {
            Login = new Login 
            {
                Authenticationkey = "afa4cc0e8b9b43789d155b296a58ddca"
            },
            Query = new Query 
            {
                Objecttype = "TrainAnnouncement",
                Schemaversion = "1.5",
                LastModified = true,
                Filter = new Filter 
                {
                    Eq = new NameValue 
                    {
                        Name = "AdvertisedTrainIdent",
                        Value = bookingNumber
                    }
                }
            }
        };

        return await GetTripInformation(request);
    }

    public async Task<IEnumerable<TripInformation>> GetTripInformation(string bookingNumber, DateTime modifiedSince) 
    {
        var request = new TrafikverketRequest 
        {
            Login = new Login 
            {
                Authenticationkey = "afa4cc0e8b9b43789d155b296a58ddca"
            },
            Query = new Query 
            {
                Objecttype = "TrainAnnouncement",
                Schemaversion = "1.5",
                LastModified = true,
                Filter = new Filter 
                {
                    Eq = new NameValue 
                    {
                        Name = "AdvertisedTrainIdent",
                        Value = bookingNumber
                    },
                    Gt = new NameValue 
                    {
                        Name = "ModifiedTime",
                        Value = modifiedSince.ToString("yyyy-MM-ddTHH:mm:ssZ")
                    }
                }
            }
        };

        return await GetTripInformation(request);
    }

    public async Task<IEnumerable<TrainStation>> GetTrainStations(params string[] locationSignatures)
    {
        var request = new TrafikverketRequest 
        {
            Login = new Login 
            {
                Authenticationkey = "afa4cc0e8b9b43789d155b296a58ddca"
            },
            Query = new Query 
            {
                Objecttype = "TrainStation",
                Schemaversion = "1",
                Filter = new Filter 
                {
                    In = new NameValue 
                    {
                        Name = "LocationSignature",
                        Value = string.Join(", ", locationSignatures)
                    }
                }
            }
        };

        return (await MakeRequest<TrainStationResponse>(request)).TrainStation;
    }

    private async Task<IEnumerable<TripInformation>> GetTripInformation(TrafikverketRequest request) 
    {
        var trainAnnouncements = (await MakeRequest<TrainAnnouncementResponse>(request)).TrainAnnouncement;

        if(!trainAnnouncements.Any()) 
        {
            return new List<TripInformation>();
        }

        trainAnnouncements = trainAnnouncements.OrderBy(t => t.AdvertisedTimeAtLocation);

        var firstDeparture = trainAnnouncements.First();

        var viaLocations = firstDeparture.ViaToLocation.OrderBy(v => v.Order);

        var middleArrivals = viaLocations.Select(v => trainAnnouncements.First(t => t.ActivityType == "Ankomst" && t.LocationSignature == v.LocationName));
        
        var lastArrival = trainAnnouncements.First(t => t.LocationSignature == firstDeparture.ToLocation.First().LocationName);

        var tripInformations = new List<TripInformation>();

        Debug.Log("GetTrainStations");
        var trainStations = await GetTrainStations(trainAnnouncements.Select(t => t.LocationSignature).Distinct().ToArray());

        Debug.Log(string.Join(", ", trainStations.Select(t => t.AdvertisedLocationName)));
        tripInformations.Add(new TripInformation 
        {
            Index = 0,
            Name = trainStations.First(t => t.LocationSignature == firstDeparture.LocationSignature).AdvertisedLocationName,
            EstimatedDepartureTime = firstDeparture.AdvertisedTimeAtLocation,
            TypeOfTraffic = firstDeparture.TypeOfTraffic,
            ModifiedTime = firstDeparture.ModifiedTime

        });

        tripInformations.AddRange(middleArrivals.Select(m => new TripInformation
        {
            Index = viaLocations.First(v => v.LocationName == m.LocationSignature).Order + 1,
            Name = trainStations.First(t => t.LocationSignature == m.LocationSignature).AdvertisedLocationName,
            EstimatedArrivalTime = m.AdvertisedTimeAtLocation,
            TypeOfTraffic = m.TypeOfTraffic,
            ModifiedTime = m.ModifiedTime
        }));

        tripInformations.Add(new TripInformation
        {
            Index = tripInformations.Count,
            Name = trainStations.First(t => t.LocationSignature == lastArrival.LocationSignature).AdvertisedLocationName,
            EstimatedArrivalTime = lastArrival.AdvertisedTimeAtLocation,
            TypeOfTraffic = lastArrival.TypeOfTraffic,
            ModifiedTime = lastArrival.ModifiedTime
        });

        return tripInformations;
    }

    private static string Serialize<T>(T dataToSerialize)
    {
        var stringwriter = new System.IO.StringWriter();
        var serializer = new XmlSerializer(typeof(T));
        serializer.Serialize(stringwriter, dataToSerialize);
        return stringwriter.ToString();
    }

    private async Task<T> MakeRequest<T>(TrafikverketRequest request)
    {
        var serialized = Serialize(request);
        Debug.Log(serialized);
        var stringContent = new StringContent(serialized, Encoding.UTF8, "application/xml");

        var result = await httpClient.PostAsync(baseUrl, stringContent);
        var stringResponse = await result.Content.ReadAsStringAsync();

        Debug.Log(stringResponse);

        return JsonConvert.DeserializeObject<TrafikverketResponse<IEnumerable<T>>>(stringResponse)
            .Response.Result.First();
    }
}


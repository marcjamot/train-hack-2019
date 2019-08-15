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

        var serialized = Serialize(request);
        var stringContent = new StringContent(serialized, Encoding.UTF8, "application/xml");

        var result = await httpClient.PostAsync(baseUrl, stringContent);
        var response = await result.Content.ReadAsStringAsync();
    
        var trafikverketResponse = JsonConvert.DeserializeObject<TrafikverketResponse<IEnumerable<TrainMessageResponse>>>(response);

        return trafikverketResponse.Response.Result.First().TrainMessage;
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
                Filter = new Filter 
                {
                    Eq = new Eq 
                    {
                        Name = "AdvertisedTrainIdent",
                        Value = bookingNumber
                    }
                }
            }
        };

        var serialized = Serialize(request);
        var stringContent = new StringContent(serialized, Encoding.UTF8, "application/xml");

        var result = await httpClient.PostAsync(baseUrl, stringContent);
        var stringResponse = await result.Content.ReadAsStringAsync();

        var trainAnnouncements = JsonConvert.DeserializeObject<TrafikverketResponse<IEnumerable<TrainAnnouncement>>>(stringResponse)
            .Response.Result;

        trainAnnouncements = trainAnnouncements.OrderBy(t => t.AdvertisedTimeAtLocation);

        var firstDeparture = trainAnnouncements.First();

        var viaLocations = firstDeparture.ViaToLocation.OrderBy(v => v.Order);

        var middleArrivals = trainAnnouncements.Where(t => t.ActivityType == "Ankomst" && viaLocations.Any(v => v.LocationName == t.LocationSignature));
        
        var lastArrival = trainAnnouncements.First(t => t.LocationSignature == firstDeparture.ToLocation.First().LocationName);

        var tripInformations = new List<TripInformation>();

        tripInformations.Add(new TripInformation 
        {
            Index = 0,
            Name = firstDeparture.LocationSignature,
            EstimatedDepartureTime = firstDeparture.EstimatedTimeAtLocation,
            TypeOfTraffic = firstDeparture.TypeOfTraffic
        });

        tripInformations.AddRange(middleArrivals.Select(m => new TripInformation
        {
            Index = viaLocations.First(v => v.LocationName == m.LocationSignature).Order + 1,
            Name = m.LocationSignature,
            EstimatedArrivalTime = m.EstimatedTimeAtLocation,
            TypeOfTraffic = m.TypeOfTraffic
        }));

        tripInformations.Add(new TripInformation
        {
            Index = tripInformations.Count,
            Name = lastArrival.LocationSignature,
            EstimatedArrivalTime = lastArrival.EstimatedTimeAtLocation,
            TypeOfTraffic = lastArrival.TypeOfTraffic
        });

        return tripInformations;
    }

    private static string Serialize<T>(T dataToSerialize)
    {
        try
        {
            var stringwriter = new System.IO.StringWriter();
            var serializer = new XmlSerializer(typeof(T));
            serializer.Serialize(stringwriter, dataToSerialize);
            return stringwriter.ToString();
        }
        catch
        {
            throw;
        }
    }
}


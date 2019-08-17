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

    public async Task<IEnumerable<StationInformation>> GetStationInformations(string bookingNumber) 
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
                        Name = "AdvertisedTimeAtLocation",
                        Value = DateTime.Today.ToString("yyyy-MM-ddTHH:mm:ssZ")
                    }
                }
            }
        };

        return await GetTripInformation(request);
    }

    public async Task<IEnumerable<StationInformation>> GetTripInformation(string bookingNumber, DateTime modifiedSince) 
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

    private async Task<IEnumerable<StationInformation>> GetTripInformation(TrafikverketRequest request) 
    {
        var trainAnnouncements = (await MakeRequest<TrainAnnouncementResponse>(request)).TrainAnnouncement;

        if(!trainAnnouncements.Any()) 
        {
            return new List<StationInformation>();
        }

        trainAnnouncements = trainAnnouncements.OrderBy(t => t.AdvertisedTimeAtLocation);

        var firstDeparture = trainAnnouncements.First();

        var viaLocations = firstDeparture.ViaToLocation.OrderBy(v => v.Order);

        var middleArrivals = viaLocations.Select(v => trainAnnouncements.First(t => t.ActivityType == "Ankomst" && t.LocationSignature == v.LocationName));
        
        var lastArrival = trainAnnouncements.First(t => t.LocationSignature == firstDeparture.ToLocation.First().LocationName);

        var tripInformations = new List<StationInformation>();

        var trainStations = await GetTrainStations(trainAnnouncements.Select(t => t.LocationSignature).Distinct().ToArray());

        tripInformations.Add(new StationInformation 
        {
            Index = 0,
            Name = trainStations.First(t => t.LocationSignature == firstDeparture.LocationSignature).AdvertisedLocationName,
            EstimatedTime = firstDeparture.AdvertisedTimeAtLocation,
            TypeOfTraffic = firstDeparture.TypeOfTraffic,
            ModifiedTime = firstDeparture.ModifiedTime

        });

        tripInformations.AddRange(middleArrivals.Select(m => new StationInformation
        {
            Index = viaLocations.First(v => v.LocationName == m.LocationSignature).Order + 1,
            Name = trainStations.First(t => t.LocationSignature == m.LocationSignature).AdvertisedLocationName,
            EstimatedTime = m.AdvertisedTimeAtLocation,
            TypeOfTraffic = m.TypeOfTraffic,
            ModifiedTime = m.ModifiedTime
        }));

        tripInformations.Add(new StationInformation
        {
            Index = tripInformations.Count,
            Name = trainStations.First(t => t.LocationSignature == lastArrival.LocationSignature).AdvertisedLocationName,
            EstimatedTime = lastArrival.AdvertisedTimeAtLocation,
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
        string stringResponse;

        var result = await httpClient.PostAsync(baseUrl, stringContent);
        if(!result.IsSuccessStatusCode) {
            stringResponse = "{      'RESPONSE ': {          'RESULT ': [             {                  'TrainAnnouncement ': [                     {                          'ActivityId ':  '1500adde-0a5d-0422-08d7-160d69d9e84d ',                          'ActivityType ':  'Ankomst ',                          'Advertised ': true,                          'AdvertisedTimeAtLocation ':  '2019-08-17T18:10:00.000+02:00 ',                          'AdvertisedTrainIdent ':  '2016 ',                          'Canceled ': false,                          'EstimatedTimeIsPreliminary ': false,                          'FromLocation ': [                             {                                  'LocationName ':  'G ',                                  'Priority ': 1,                                  'Order ': 0                             }                         ],                          'InformationOwner ':  'MTRN ',                          'LocationSignature ':  'Sk ',                          'ModifiedTime ':  '2019-08-08T23:24:38.877Z ',                          'NewEquipment ': 0,                          'Operator ':  'MTRN ',                          'PlannedEstimatedTimeAtLocationIsValid ': false,                          'ProductInformation ': [                             {                                  'Code ':  'PNA070 ',                                  'Description ':  'MTRExpress '                             }                         ],                          'ScheduledDepartureDateTime ':  '2019-08-17T00:00:00.000+02:00 ',                          'TechnicalDateTime ':  '2019-08-17T18:10:00.000+02:00 ',                          'TechnicalTrainIdent ':  '2016 ',                          'ToLocation ': [                             {                                  'LocationName ':  'Sst ',                                  'Priority ': 1,                                  'Order ': 0                             }                         ],                          'TrackAtLocation ':  '1 ',                          'TrainOwner ':  'MTRN ',                          'TypeOfTraffic ':  'Tåg ',                          'ViaFromLocation ': [                             {                                  'LocationName ':  'A ',                                  'Priority ': 2,                                  'Order ': 0                             },                             {                                  'LocationName ':  'Hr ',                                  'Priority ': 1,                                  'Order ': 1                             }                         ],                          'WebLinkName ':  'MTRN '                     },                     {                          'ActivityId ':  '1500adde-0a5d-0422-08d7-160d69d9e850 ',                          'ActivityType ':  'Ankomst ',                          'Advertised ': true,                          'AdvertisedTimeAtLocation ':  '2019-08-17T20:17:00.000+02:00 ',                          'AdvertisedTrainIdent ':  '2016 ',                          'Canceled ': false,                          'EstimatedTimeIsPreliminary ': false,                          'FromLocation ': [                             {                                  'LocationName ':  'G ',                                  'Priority ': 1,                                  'Order ': 0                             }                         ],                          'InformationOwner ':  'MTRN ',                          'LocationSignature ':  'Sst ',                          'ModifiedTime ':  '2019-08-08T23:24:38.877Z ',                          'NewEquipment ': 0,                          'Operator ':  'MTRN ',                          'PlannedEstimatedTimeAtLocationIsValid ': false,                          'ProductInformation ': [                             {                                  'Code ':  'PNA070 ',                                  'Description ':  'MTRExpress '                             }                         ],                          'ScheduledDepartureDateTime ':  '2019-08-17T00:00:00.000+02:00 ',                          'TechnicalDateTime ':  '2019-08-17T20:17:00.000+02:00 ',                          'TechnicalTrainIdent ':  '2016 ',                          'ToLocation ': [                             {                                  'LocationName ':  'Sst ',                                  'Priority ': 1,                                  'Order ': 0                             }                         ],                          'TrackAtLocation ':  '4 ',                          'TrainOwner ':  'MTRN ',                          'TypeOfTraffic ':  'Tåg ',                          'ViaFromLocation ': [                             {                                  'LocationName ':  'A ',                                  'Priority ': 3,                                  'Order ': 0                             },                             {                                  'LocationName ':  'Hr ',                                  'Priority ': 2,                                  'Order ': 1                             },                             {                                  'LocationName ':  'Sk ',                                  'Priority ': 1,                                  'Order ': 2                             }                         ],                          'WebLinkName ':  'MTRN '                     },                     {                          'ActivityId ':  '1500adde-0a5d-0422-08d7-160d69d9e849 ',                          'ActivityType ':  'Avgang ',                          'Advertised ': true,                          'AdvertisedTimeAtLocation ':  '2019-08-17T16:50:00.000+02:00 ',                          'AdvertisedTrainIdent ':  '2016 ',                          'Canceled ': false,                          'EstimatedTimeIsPreliminary ': false,                          'FromLocation ': [                             {                                  'LocationName ':  'G ',                                  'Priority ': 1,                                  'Order ': 0                             }                         ],                          'InformationOwner ':  'MTRN ',                          'LocationSignature ':  'G ',                          'ModifiedTime ':  '2019-08-17T09:46:49.761Z ',                          'NewEquipment ': 0,                          'Operator ':  'MTRN ',                          'OtherInformation ': [                             {                                  'Code ':  'ONA179 ',                                  'Description ':  'Endast Endast MTR Express biljetter gäller. '                             }                         ],                          'PlannedEstimatedTimeAtLocationIsValid ': false,                          'ProductInformation ': [                             {                                  'Code ':  'PNA070 ',                                  'Description ':  'MTRExpress '                             }                         ],                          'ScheduledDepartureDateTime ':  '2019-08-17T00:00:00.000+02:00 ',                          'TechnicalDateTime ':  '2019-08-17T16:50:00.000+02:00 ',                          'TechnicalTrainIdent ':  '2016 ',                          'ToLocation ': [                             {                                  'LocationName ':  'Sst ',                                  'Priority ': 1,                                  'Order ': 0                             }                         ],                          'TrackAtLocation ':  '4 ',                          'TrainComposition ': [                             {                                  'Code ':  'TNA001 ',                                  'Description ':  'Vagnsordning E-D-C-B-A, Café i vagn B '                             }                         ],                          'TrainOwner ':  'MTRN ',                          'TypeOfTraffic ':  'Tåg ',                          'ViaToLocation ': [                             {                                  'LocationName ':  'Hr ',                                  'Priority ': 2,                                  'Order ': 0                             },                             {                                  'LocationName ':  'Sk ',                                  'Priority ': 1,                                  'Order ': 1                             },                             {                                  'LocationName ':  'Söö ',                                  'Priority ': 3,                                  'Order ': 2                             }                         ],                          'WebLinkName ':  'MTRN '                     },                     {                          'ActivityId ':  '1500adde-0a5d-0422-08d7-160d69d9e84c ',                          'ActivityType ':  'Avgang ',                          'Advertised ': true,                          'AdvertisedTimeAtLocation ':  '2019-08-17T17:37:00.000+02:00 ',                          'AdvertisedTrainIdent ':  '2016 ',                          'Canceled ': false,                          'EstimatedTimeIsPreliminary ': false,                          'FromLocation ': [                             {                                  'LocationName ':  'G ',                                  'Priority ': 1,                                  'Order ': 0                             }                         ],                          'InformationOwner ':  'MTRN ',                          'LocationSignature ':  'Hr ',                          'ModifiedTime ':  '2019-08-08T23:24:38.877Z ',                          'NewEquipment ': 0,                          'Operator ':  'MTRN ',                          'OtherInformation ': [                             {                                  'Code ':  'ONA179 ',                                  'Description ':  'Endast Endast MTR Express biljetter gäller. '                             }                         ],                          'PlannedEstimatedTimeAtLocationIsValid ': false,                          'ProductInformation ': [                             {                                  'Code ':  'PNA070 ',                                  'Description ':  'MTRExpress '                             }                         ],                          'ScheduledDepartureDateTime ':  '2019-08-17T00:00:00.000+02:00 ',                          'TechnicalDateTime ':  '2019-08-17T17:39:00.000+02:00 ',                          'TechnicalTrainIdent ':  '2016 ',                          'ToLocation ': [                             {                                  'LocationName ':  'Sst ',                                  'Priority ': 1,                                  'Order ': 0                             }                         ],                          'TrackAtLocation ':  '5 ',                          'TrainComposition ': [                             {                                  'Code ':  'TNA001 ',                                  'Description ':  'Vagnsordning E-D-C-B-A, Café i vagn B '                             }                         ],                          'TrainOwner ':  'MTRN ',                          'TypeOfTraffic ':  'Tåg ',                          'ViaToLocation ': [                             {                                  'LocationName ':  'Sk ',                                  'Priority ': 1,                                  'Order ': 0                             },                             {                                  'LocationName ':  'Söö ',                                  'Priority ': 2,                                  'Order ': 1                             }                         ],                          'WebLinkName ':  'MTRN '                     },                     {                          'ActivityId ':  '1500adde-0a5d-0422-08d7-160d69d9e84a ',                          'ActivityType ':  'Avgang ',                          'Advertised ': true,                          'AdvertisedTimeAtLocation ':  '2019-08-17T17:22:00.000+02:00 ',                          'AdvertisedTrainIdent ':  '2016 ',                          'Canceled ': false,                          'EstimatedTimeIsPreliminary ': false,                          'FromLocation ': [                             {                                  'LocationName ':  'G ',                                  'Priority ': 1,                                  'Order ': 0                             }                         ],                          'InformationOwner ':  'MTRN ',                          'LocationSignature ':  'A ',                          'ModifiedTime ':  '2019-08-08T23:24:38.877Z ',                          'NewEquipment ': 0,                          'Operator ':  'MTRN ',                          'OtherInformation ': [                             {                                  'Code ':  'ONA179 ',                                  'Description ':  'Endast Endast MTR Express biljetter gäller. '                             },                             {                                  'Code ':  'ONA121 ',                                  'Description ':  'Tåget gör uppehåll vid plattformens mitt. '                             }                         ],                          'PlannedEstimatedTimeAtLocationIsValid ': false,                          'ProductInformation ': [                             {                                  'Code ':  'PNA070 ',                                  'Description ':  'MTRExpress '                             }                         ],                          'ScheduledDepartureDateTime ':  '2019-08-17T00:00:00.000+02:00 ',                          'TechnicalDateTime ':  '2019-08-17T17:23:00.000+02:00 ',                          'TechnicalTrainIdent ':  '2016 ',                          'ToLocation ': [                             {                                  'LocationName ':  'Sst ',                                  'Priority ': 1,                                  'Order ': 0                             }                         ],                          'TrackAtLocation ':  '1 ',                          'TrainComposition ': [                             {                                  'Code ':  'TNA001 ',                                  'Description ':  'Vagnsordning E-D-C-B-A, Café i vagn B '                             }                         ],                          'TrainOwner ':  'MTRN ',                          'TypeOfTraffic ':  'Tåg ',                          'ViaToLocation ': [                             {                                  'LocationName ':  'Hr ',                                  'Priority ': 2,                                  'Order ': 0                             },                             {                                  'LocationName ':  'Sk ',                                  'Priority ': 1,                                  'Order ': 1                             },                             {                                  'LocationName ':  'Söö ',                                  'Priority ': 3,                                  'Order ': 2                             }                         ],                          'WebLinkName ':  'MTRN '                     },                     {                          'ActivityId ':  '1500adde-0a5d-0422-08d7-160d69d9e84f ',                          'ActivityType ':  'Ankomst ',                          'Advertised ': true,                          'AdvertisedTimeAtLocation ':  '2019-08-17T20:01:00.000+02:00 ',                          'AdvertisedTrainIdent ':  '2016 ',                          'Canceled ': false,                          'EstimatedTimeIsPreliminary ': false,                          'FromLocation ': [                             {                                  'LocationName ':  'G ',                                  'Priority ': 1,                                  'Order ': 0                             }                         ],                          'InformationOwner ':  'MTRN ',                          'LocationSignature ':  'Söö ',                          'ModifiedTime ':  '2019-08-08T23:24:38.877Z ',                          'NewEquipment ': 0,                          'Operator ':  'MTRN ',                          'PlannedEstimatedTimeAtLocationIsValid ': false,                          'ProductInformation ': [                             {                                  'Code ':  'PNA070 ',                                  'Description ':  'MTRExpress '                             }                         ],                          'ScheduledDepartureDateTime ':  '2019-08-17T00:00:00.000+02:00 ',                          'TechnicalDateTime ':  '2019-08-17T20:01:00.000+02:00 ',                          'TechnicalTrainIdent ':  '2016 ',                          'ToLocation ': [                             {                                  'LocationName ':  'Sst ',                                  'Priority ': 1,                                  'Order ': 0                             }                         ],                          'TrackAtLocation ':  '6 ',                          'TrainOwner ':  'MTRN ',                          'TypeOfTraffic ':  'Tåg ',                          'ViaFromLocation ': [                             {                                  'LocationName ':  'A ',                                  'Priority ': 3,                                  'Order ': 0                             },                             {                                  'LocationName ':  'Hr ',                                  'Priority ': 2,                                  'Order ': 1                             },                             {                                  'LocationName ':  'Sk ',                                  'Priority ': 1,                                  'Order ': 2                             }                         ],                          'WebLinkName ':  'MTRN '                     },                     {                          'ActivityId ':  '1500adde-0a5d-0422-08d7-160d69d9e84b ',                          'ActivityType ':  'Ankomst ',                          'Advertised ': true,                          'AdvertisedTimeAtLocation ':  '2019-08-17T17:37:00.000+02:00 ',                          'AdvertisedTrainIdent ':  '2016 ',                          'Canceled ': false,                          'EstimatedTimeIsPreliminary ': false,                          'FromLocation ': [                             {                                  'LocationName ':  'G ',                                  'Priority ': 1,                                  'Order ': 0                             }                         ],                          'InformationOwner ':  'MTRN ',                          'LocationSignature ':  'Hr ',                          'ModifiedTime ':  '2019-08-08T23:24:38.877Z ',                          'NewEquipment ': 0,                          'Operator ':  'MTRN ',                          'PlannedEstimatedTimeAtLocationIsValid ': false,                          'ProductInformation ': [                             {                                  'Code ':  'PNA070 ',                                  'Description ':  'MTRExpress '                             }                         ],                          'ScheduledDepartureDateTime ':  '2019-08-17T00:00:00.000+02:00 ',                          'TechnicalDateTime ':  '2019-08-17T17:37:00.000+02:00 ',                          'TechnicalTrainIdent ':  '2016 ',                          'ToLocation ': [                             {                                  'LocationName ':  'Sst ',                                  'Priority ': 1,                                  'Order ': 0                             }                         ],                          'TrackAtLocation ':  '5 ',                          'TrainOwner ':  'MTRN ',                          'TypeOfTraffic ':  'Tåg ',                          'ViaFromLocation ': [                             {                                  'LocationName ':  'A ',                                  'Priority ': 1,                                  'Order ': 0                             }                         ],                          'WebLinkName ':  'MTRN '                     },                     {                          'ActivityId ':  '1500adde-0a5d-0422-08d7-160d69d9e84e ',                          'ActivityType ':  'Avgang ',                          'Advertised ': true,                          'AdvertisedTimeAtLocation ':  '2019-08-17T18:10:00.000+02:00 ',                          'AdvertisedTrainIdent ':  '2016 ',                          'Canceled ': false,                          'EstimatedTimeIsPreliminary ': false,                          'FromLocation ': [                             {                                  'LocationName ':  'G ',                                  'Priority ': 1,                                  'Order ': 0                             }                         ],                          'InformationOwner ':  'MTRN ',                          'LocationSignature ':  'Sk ',                          'ModifiedTime ':  '2019-08-08T23:24:38.877Z ',                          'NewEquipment ': 0,                          'Operator ':  'MTRN ',                          'OtherInformation ': [                             {                                  'Code ':  'ONA179 ',                                  'Description ':  'Endast Endast MTR Express biljetter gäller. '                             }                         ],                          'PlannedEstimatedTimeAtLocationIsValid ': false,                          'ProductInformation ': [                             {                                  'Code ':  'PNA070 ',                                  'Description ':  'MTRExpress '                             }                         ],                          'ScheduledDepartureDateTime ':  '2019-08-17T00:00:00.000+02:00 ',                          'TechnicalDateTime ':  '2019-08-17T18:12:00.000+02:00 ',                          'TechnicalTrainIdent ':  '2016 ',                          'ToLocation ': [                             {                                  'LocationName ':  'Sst ',                                  'Priority ': 1,                                  'Order ': 0                             }                         ],                          'TrackAtLocation ':  '1 ',                          'TrainComposition ': [                             {                                  'Code ':  'TNA001 ',                                  'Description ':  'Vagnsordning E-D-C-B-A, Café i vagn B '                             }                         ],                          'TrainOwner ':  'MTRN ',                          'TypeOfTraffic ':  'Tåg ',                          'ViaToLocation ': [                             {                                  'LocationName ':  'Söö ',                                  'Priority ': 1,                                  'Order ': 0                             }                         ],                          'WebLinkName ':  'MTRN '                     }                 ],                  'INFO ': {                      'LASTMODIFIED ': {                          '_attr_datetime ':  '2019-08-17T09:46:49.761Z '                     }                 }             }         ]     } } ";
        } else {
            stringResponse = await result.Content.ReadAsStringAsync();
        }

        Debug.Log(stringResponse);

        return JsonConvert.DeserializeObject<TrafikverketResponse<IEnumerable<T>>>(stringResponse)
            .Response.Result.First();
    }
}


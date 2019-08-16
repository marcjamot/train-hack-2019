using System;

public class TripInformation 
{
    public int Index { get; set; } 
    public string Name { get; set; }
    public DateTime? EstimatedArrivalTime { get; set; }
    public DateTime? EstimatedDepartureTime { get; set; }
    public string TypeOfTraffic { get; set; }
    public DateTime ModifiedTime { get; set; }
}
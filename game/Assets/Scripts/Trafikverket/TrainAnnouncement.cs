using System;
using System.Collections.Generic;

public class TrainAnnouncement
{
    public string ActivityId { get; set; }
    public string ActivityType { get; set; }
    public bool Advertised { get; set; }
    public DateTime AdvertisedTimeAtLocation { get; set; }
    public string AdvertisedTrainIdent { get; set; }
    public IEnumerable<Booking> Booking { get; set; }
    public bool Canceled { get; set; }
    public bool Deleted { get; set; }
    public IEnumerable<Deviation> Deviation { get; set; }
    public DateTime EstimatedTimeAtLocation { get; set; }
    public bool EstimatedTimeIsPreliminary { get; set; }
    public IEnumerable<FromLocation> FromLocation { get; set; }
    public string InformationOwner { get; set; }
    public string LocationSignature { get; set; }
    public string MobileWebLink { get; set; }
    public DateTime ModifiedTime { get; set; }
    public int NewEquipment { get; set; }
    public string Operator { get; set; }
    public IEnumerable<OtherInformation> OtherInformation { get; set; }
    public DateTime PlannedEstimatedTimeAtLocation { get; set; }
    public bool PlannedEstimatedTimeAtLocationIsValid { get; set; }
    public IEnumerable<ProductInformation> ProductInformation { get; set; }
    public DateTime ScheduledDepartureDateTime { get; set; }
    public IEnumerable<Service> Service { get; set; }
    public DateTime TechnicalDateTime { get; set; }
    public string TechnicalTrainIdent { get; set; }
    public DateTime TimeAtLocation { get; set; }
    public DateTime TimeAtLocationWithSeconds { get; set; }
    public IEnumerable<ToLocation> ToLocation { get; set; }
    public string TrackAtLocation { get; set; }
    public IEnumerable<TrainComposition> TrainComposition { get; set; }
    public string TrainOwner { get; set; }
    public string TypeOfTraffic { get; set; }
    public string WebLink { get; set; }
    public string WebLinkName { get; set; }
    public IEnumerable<ViaFromLocation> ViaFromLocation { get; set; }
    public IEnumerable<ViaToLocation> ViaToLocation { get; set; }
}

public class FromLocation
{
    public string LocationName { get; set; }
    public int Order { get; set; }
    public int Priority { get; set; }
}

public class ToLocation 
{
    public string LocationName { get; set; }
    public int Order { get; set; }
    public int Priority { get; set; }
}

public class ViaFromLocation 
{
    public string LocationName { get; set; }
    public int Order { get; set; }
    public int Priority { get; set; }
}

public class ViaToLocation 
{
    public string LocationName { get; set; }
    public int Order { get; set; }
    public int Priority { get; set; }
}

public class Deviation {
    public string Code { get; set; }
    public string Description { get; set; }
}

public class Booking {
    public string Code { get; set; }
    public string Description { get; set; }
}

public class OtherInformation {
    public string Code { get; set; }
    public string Description { get; set; }
}

public class ProductInformation {
    public string Code { get; set; }
    public string Description { get; set; }
}

public class Service {
    public string Code { get; set; }
    public string Description { get; set; }
}

public class TrainComposition {
    public string Code { get; set; }
    public string Description { get; set; }
}
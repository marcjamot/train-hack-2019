using System;
using UnityEngine;

public class TrainMessage
{
    public string[] AffectedLocation { get; set; }//	Påverkade trafikplatser (stationssignatur)
    public int[] CountyNo { get; set; } //	Länsnummer
    public bool Deleted { get; set; }	// Anger att dataposten raderats
    public DateTime EndDateTime { get; set; } // Händelsens sluttid
    public string EventId { get; set; } //Fältet är nyckel för objektet.	Unikt id för händelsen
    public bool ExpectTrafficImpact	{ get; set; }	// Händelse kommer förmodligen att påverka trafiken, men trafikpåverkan och prognostiserad sluttidpunkt för trafikpåverkan är ännu inte specificerad.
    public string ExternalDescription { get; set; } // Informationstext
    // Geometry.SWEREF99TM // Fältet kan användas för geo-frågor.	WKT	Geometrisk punkt i koordinatsystem SWEREF99TM
    // Geometry.WGS84 // Fältet kan användas för geo-frågor.	WKT	Geometrisk punkt i koordinatsystem WGS84
    public string Header { get; set; } // Redaktörssatt rubrik för händelsen, kan i vissa fall vara samma som ReasonCodeText
    public DateTime LastUpdateDateTime { get; set; }	 //	Tidpunkt då händelsen uppdaterades
    public DateTime ModifiedTime { get; set; } // Tidpunkt då dataposten ändrades
    public DateTime PrognosticatedEndDateTimeTrafficImpact { get; set; } // Prognos för då händelsen inte längre väntas påverka trafiken
    public string ReasonCodeText { get; set; } // Händelsens eventuella orsak
    public DateTime StartDateTime { get; set; } // Händelsens starttid
    public TrafficImpact[] TrafficImpact { get; set; }
}

public class TrafficImpact 
{
    string[] AffectedLocation { get; set; }  // Påverkade stationer
    string[] FromLocation { get; set; } // Påverkad sträckas frånstation, för att avgöra om stationen är påverkad, se fältet AffectedLocation
    string[] ToLocation { get; set; } //Påverkad sträckas tillstation, för att avgöra om stationen är påverkad, se fältet AffectedLocation
}
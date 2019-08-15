using System;
using System.Xml.Serialization;
using UnityEngine;

[XmlType("REQUEST")]
public class TrafikVerketRequest
{
    [XmlElement(ElementName="LOGIN")]
    public Login Login { get; set; }
    [XmlElement(ElementName="QUERY")]
    public Query Query { get; set; }
}

[XmlRoot(ElementName="LOGIN")]
public class Login
{
    [XmlAttribute(AttributeName="authenticationkey")]
    public string Authenticationkey { get; set; }
}

[XmlRoot(ElementName="QUERY")]
public class Query
{
    [XmlAttribute(AttributeName="objecttype")]
    public string Objecttype { get; set; }
    [XmlAttribute(AttributeName="schemaversion")]
    public string Schemaversion { get; set; }
    [XmlAttribute(AttributeName="limit")]
    public string Limit { get; set; }
}

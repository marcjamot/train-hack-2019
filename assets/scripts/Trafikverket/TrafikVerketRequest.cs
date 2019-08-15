using System;
using System.Xml.Serialization;
using UnityEngine;

[XmlType("REQUEST")]
public class TrafikverketRequest
{
    [XmlElement(ElementName="LOGIN")]
    public Login Login { get; set; }
    [XmlElement(ElementName="QUERY")]
    public Query Query { get; set; }
}

public class Login
{
    [XmlAttribute(AttributeName="authenticationkey")]
    public string Authenticationkey { get; set; }
}

public class Query
{
    [XmlAttribute(AttributeName="objecttype")]
    public string Objecttype { get; set; }
    [XmlAttribute(AttributeName="schemaversion")]
    public string Schemaversion { get; set; }
    [XmlAttribute(AttributeName="limit")]
    public string Limit { get; set; }
    [XmlElement(ElementName="FILTER")]
    public Filter Filter { get; set; }
}

public class Filter 
{
    public Eq Eq { get; set; }
}

public class Eq 
{
    [XmlAttribute(AttributeName="name")]
    public string Name { get; set; }

    [XmlAttribute(AttributeName="value")]
    public string Value { get; set; }
}

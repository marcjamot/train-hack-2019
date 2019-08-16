using System.Xml.Serialization;

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
    [XmlAttribute(AttributeName="lastmodified")]
    public bool LastModified { get; set; }
    [XmlAttribute(AttributeName="limit")]
    public string Limit { get; set; }
    [XmlElement(ElementName="FILTER")]
    public Filter Filter { get; set; }
}

public class Filter 
{
    public NameValue Eq { get; set; }
    public NameValue In { get; set; }
    public NameValue Gt { get; set; }
}

public class NameValue 
{
    [XmlAttribute(AttributeName="name")]
    public string Name { get; set; }

    [XmlAttribute(AttributeName="value")]
    public string Value { get; set; }
}

using UnityEngine;

public class Stop
{
    public string Name = "stop ";
    public System.DateTime ArrivalTime;
    public Vector3 Position;
    public StopType Type;
    public int Order;
}

public enum StopType 
{
    City,
    Forest
}
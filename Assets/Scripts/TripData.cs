using System;

[Serializable]
public class TripData
{
    public string Name;
    public string Description;
    public string Date;

    public TripData(string name, string description, string date)
    {
        Name = name;
        Description = description;
        Date = date;
    }
}
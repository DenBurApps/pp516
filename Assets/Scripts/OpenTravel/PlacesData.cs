using System;
using System.Collections.Generic;

[Serializable]
public class PlacesData
{
    public string PlaceName;
    public string PlaceDescription;
    public List<string> ImagesPath;
    public string Date;

    public PlacesData(string placeName, string placeDescription, List<string> imagesPath, string date)
    {
        PlaceName = placeName;
        PlaceDescription = placeDescription;
        ImagesPath = imagesPath;
        Date = date;
    }
}

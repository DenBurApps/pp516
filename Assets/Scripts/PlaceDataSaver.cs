using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlaceDataSaver
{
    private const string JsonExtension = ".json";

    private string _tripKey;


    private List<PlacesData> _placesToSave = new List<PlacesData>();

    public PlaceDataSaver(string tripKey)
    {
        _tripKey = tripKey;
    }


    public string TripKey => _tripKey;
    public string SaveFilePath => Path.Combine(Application.persistentDataPath, _tripKey + JsonExtension);

    public void SetKey(string key)
    {
        _tripKey = key;
    }

    public void AddPlaceData(PlacesData placesData)
    {
        _placesToSave.Add(placesData);
        SavePlaceData();
    }

    public void RemovePlaceData(PlacesData placesData)
    {
        if (_placesToSave.Contains(placesData))
            _placesToSave.Remove(placesData);

        SavePlaceData();
    }

    private void SavePlaceData()
    {
        if (string.IsNullOrEmpty(_tripKey))
            throw new Exception("Key is null");

        PlaceDataList placeDataList = new PlaceDataList(_placesToSave);
        string json = JsonUtility.ToJson(placeDataList, true);

        try
        {
            File.WriteAllText(SaveFilePath, json);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to save place data: " + e.Message);
        }
    }

    public PlaceDataList LoadAllPlacesData()
    {
        if (File.Exists(SaveFilePath))
        {
            try
            {
                string json = File.ReadAllText(SaveFilePath);
                PlaceDataList loadedPlaceDataList = JsonUtility.FromJson<PlaceDataList>(json);
                return loadedPlaceDataList;
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to load place data: " + e.Message);
            }
        }

        return null;
    }

    public void DeleteAllPlacesData()
    {
        if (File.Exists(SaveFilePath))
        {
            File.Delete(SaveFilePath);
        }
    }
}

[Serializable]
public class PlaceDataList
{
    public List<PlacesData> Places;

    public PlaceDataList(List<PlacesData> places)
    {
        Places = places;
    }
}
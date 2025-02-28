using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FilledTripDataWindow : MonoBehaviour
{
    private const string DefaultValue = "";
    private const string PlacesText = " places";
    private const int DefaultPlacesValue = 0;

    [SerializeField] private Button _deleteButton;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _descriptionText;
    [SerializeField] private TMP_Text _dateText;
    [SerializeField] private TMP_Text _placesText;
    [SerializeField] private Button _editButton;

    private PlaceDataSaver _placeDataSaver;
    private string _name;
    private string _description;
    private string _date;
    [SerializeField] private int _places;


    private bool _isActive;

    private TripData _currentTripData;

    public event Action<FilledTripDataWindow> DeleteButtonClicked;
    public event Action<FilledTripDataWindow> EditButtonClicked;
    public event Action DataChanged;

    public int Places => _places;
    public bool IsActive => _isActive;
    public TripData TripData => _currentTripData;

    public List<PlacesData> UniquePlaces { get; private set; }

    private void Awake()
    {
        UniquePlaces = new List<PlacesData>();
    }

    private void OnEnable()
    {
        _deleteButton.onClick.AddListener(OnDeleteButtonClicked);
        _editButton.onClick.AddListener(OnEditButtonClicked);
        SetPlaces(DefaultPlacesValue);
    }

    private void OnDisable()
    {
        _deleteButton.onClick.AddListener(OnDeleteButtonClicked);
        _editButton.onClick.RemoveListener(OnEditButtonClicked);
    }

    public void AddPlace(PlacesData place)
    {
        UniquePlaces.Add(place);
        SetPlaces(UniquePlaces.Count);

        if (_placeDataSaver.TripKey == null)
        {
            _placeDataSaver.SetKey(_name);
        }

        _placeDataSaver.AddPlaceData(place);
    }

    public void RemovePlace(PlacesData placesData)
    {
        if (UniquePlaces.Contains(placesData))
            UniquePlaces.Remove(placesData);

        _placeDataSaver.RemovePlaceData(placesData);

        DataChanged?.Invoke();
        SetPlaces(UniquePlaces.Count);
    }

    public void LoadPlacesData()
    {
        var list = _placeDataSaver.LoadAllPlacesData();

        if (list == null)
            return;

        foreach (var data in list.Places)
        {
            UniquePlaces.Add(data);
            
        }
        
        SetPlaces(UniquePlaces.Count);
    }

    public void OnDeleteButtonClicked()
    {
        DeleteButtonClicked?.Invoke(this);

        _name = null;
        _description = null;
        _date = null;
        _places = 0;

        _nameText.text = DefaultValue;
        _descriptionText.text = DefaultValue;
        _dateText.text = DefaultValue;
        _placesText.text = DefaultPlacesValue.ToString();
        _placeDataSaver.DeleteAllPlacesData();
        _isActive = false;
        UniquePlaces.Clear();
    }

    public void SetBasicTripData(TripData tripData)
    {
        Enable();
        
        _currentTripData = tripData;

        SetTripName(tripData.Name);
        SetTripDescription(tripData.Description);
        SetTripDate(tripData.Date);
        _placeDataSaver = new PlaceDataSaver(tripData.Name);

        _isActive = true;

        DataChanged?.Invoke();
    }

    public void Enable()
    {
        gameObject.SetActive(true);
        _isActive = true;
    }

    public void Disable()
    {
        gameObject.SetActive(false);
        _isActive = false;
    }

    public void SetTripName(string name)
    {
        _name = name;
        _nameText.text = _name;
    }

    public void SetTripDescription(string description)
    {
        _description = description;
        _descriptionText.text = _description;
    }

    public void SetTripDate(string date)
    {
        _date = date;
        _dateText.text = _date;
    }

    public void SetPlaces(int places)
    {
        _places = places;
        _placesText.text = UniquePlaces.Count.ToString() + PlacesText;
    }

    private void OnEditButtonClicked() => EditButtonClicked?.Invoke(this);
}
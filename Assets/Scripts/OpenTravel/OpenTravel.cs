using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OpenTravel : MonoBehaviour
{
    [SerializeField] private EditTravel _editTravelScreen;
    [SerializeField] private OpenTravelView _view;
    [SerializeField] private MainScreenPresenter _mainScreen;
    [SerializeField] private List<FilledPlacesPlane> _places;
    [SerializeField] private ScreenStateManager _screenStateManager;
    [SerializeField] private AddPlaceScreen _addPlaceScreen;

    private FilledTripDataWindow _currentWindow;
    private TripData _tripData;

    public event Action<TripData> EditClicked;
    public event Action<TripData> AddPlaceClicked;
    public event Action BackButtonClicked;

    private List<int> _availableWindowIndexes = new List<int>();

    private void Start()
    {
        _view.Disable();
        _view.SetPlacesText(0.ToString());
    }

    private void OnEnable()
    {
        _mainScreen.OnOpenTripDataClicked += ProcessWindowOpen;
        _view.EditButtonClicked += OnEditButtonClicked;
        _view.BackButtonClicked += OnBackButtonClicked;
        _editTravelScreen.SaveButtonClicked += EditTravelData;
        _screenStateManager.OpenTravelClicked += _view.Enable;
        _view.AddPlaceButtonClicked += OnAddPlaceClicked;
        _addPlaceScreen.SaveButtonClicked += ActivateNewPlacePlane;
    }

    private void OnDisable()
    {
        _mainScreen.OnOpenTripDataClicked -= ProcessWindowOpen;
        _view.EditButtonClicked -= OnEditButtonClicked;
        _view.BackButtonClicked -= OnBackButtonClicked;
        _editTravelScreen.SaveButtonClicked -= EditTravelData;
        _screenStateManager.OpenTravelClicked -= _view.Enable;
        _view.AddPlaceButtonClicked -= OnAddPlaceClicked;
        _addPlaceScreen.SaveButtonClicked -= ActivateNewPlacePlane;
    }

    private void ProcessWindowOpen(FilledTripDataWindow window)
    {
        if (window == null)
            throw new ArgumentNullException(nameof(window));

        _currentWindow = window;
        _tripData = window.TripData;
        SetTripDataText();
        DisplayPlaces();
        _view.Enable();
    }

    private void DisplayPlaces()
    {
        DisableAllWindows();

        if (_currentWindow.UniquePlaces.Count <= 0)
        {
            _view.ToggleEmptyPlacesImage(true);
            return;
        }

        _view.ToggleEmptyPlacesImage(false);

        for (int i = 0; i < _currentWindow.UniquePlaces.Count; i++)
        {
            ActivateNewPlacePlane(_currentWindow.UniquePlaces[i]);
        }

        _view.SetPlacesText(_currentWindow.UniquePlaces.Count.ToString());
    }

    private void EditTravelData(TripData tripData)
    {
        if (tripData == null)
            throw new ArgumentNullException(nameof(tripData));

        _tripData = tripData;
        _currentWindow.SetBasicTripData(_tripData);
        SetTripDataText();
    }

    private void ActivateNewPlacePlane(PlacesData data)
    {
        if (data == null)
            throw new ArgumentNullException(nameof(data));

        foreach (var window in _places)
        {
            if (window.PlacesData == data)
                return;
        }

        if (string.IsNullOrEmpty(data.PlaceName) || string.IsNullOrEmpty(data.Date) ||
            string.IsNullOrEmpty(data.PlaceDescription))
            return;

        if (_availableWindowIndexes.Count > 0)
        {
            int availableIndex = _availableWindowIndexes[0];
            _availableWindowIndexes.RemoveAt(0);

            var currenPlacePlane = _places[availableIndex];

            if (!currenPlacePlane.IsActive)
            {
                currenPlacePlane.Enable();
                currenPlacePlane.FillData(data);

                if (!_currentWindow.UniquePlaces.Contains(data))
                    _currentWindow.AddPlace(data);
                
                currenPlacePlane.DeleteButtonClicked += ProcessPlacesPlaneDeletion;
            }

            _view.SetPlacesText(_currentWindow.UniquePlaces.Count.ToString());

            if (_availableWindowIndexes.Count < _places.Count)
            {
                _view.ToggleEmptyPlacesImage(false);
            }
        }

        SetTripDataText();
    }

    private void ProcessPlacesPlaneDeletion(FilledPlacesPlane placesPlane)
    {
        if (placesPlane == null)
            throw new ArgumentNullException(nameof(placesPlane));

        int windowIndex = _places.IndexOf(placesPlane);

        if (windowIndex >= 0 && !_availableWindowIndexes.Contains(windowIndex))
        {
            _availableWindowIndexes.Add(windowIndex);
        }

        placesPlane.DeleteButtonClicked -= ProcessPlacesPlaneDeletion;
        _currentWindow.RemovePlace(placesPlane.PlacesData);
        placesPlane.Disable();

        _view.SetPlacesText(_currentWindow.UniquePlaces.Count.ToString());

        if (_availableWindowIndexes.Count == _places.Count)
        {
            _view.ToggleEmptyPlacesImage(true);
        }

        SetTripDataText();
    }


    private void SetTripDataText()
    {
        _view.SetTripNameText(_tripData.Name);
        _view.SetDateText(_tripData.Date);
        _view.SetDescriptionText(_tripData.Description);
    }

    private void DisableAllWindows()
    {
        for (int i = 0; i < _places.Count; i++)
        {
            _places[i].Disable();
            _availableWindowIndexes.Add(i);
        }

        _view.ToggleEmptyPlacesImage(true);
    }

    private void OnBackButtonClicked()
    {
        BackButtonClicked?.Invoke();
        _view.SetPlacesText(0.ToString());
        _view.Disable();
    }

    private void OnEditButtonClicked()
    {
        EditClicked?.Invoke(_tripData);
        _view.Disable();
    }

    private void OnAddPlaceClicked()
    {
        AddPlaceClicked?.Invoke(_tripData);
        _view.Disable();
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;

public class AddPlaceScreen : MonoBehaviour
{
    private const string DefaultTextInputValue = "";

    [SerializeField] private AddPlaceScreenView _view;
    [SerializeField] private OpenTravel _openTravelScreen;
    [SerializeField] private List<AddPhotoImage> _images;

    private string _placeName;
    private string _placeDescription;
    private string _date;
    private List<int> _availableWindowIndexes = new List<int>();
    private AddPhotoImage _currentImage;

    public event Action BackButtonClicked;
    public event Action<PlacesData> SaveButtonClicked;

    private void Start()
    {
        _view.Disable();
        ReturnToDefaultValues();
    }

    private void OnEnable()
    {
        _view.PlaceNameChanged += OnPlaceNameChanged;
        _view.PlaceDescriptionChanged += OnPlaceDescriptionChanged;
        _view.DateChanged += OnDateChanged;
        _view.BackButtonClicked += OnBackButtonClicked;
        _view.AddPhotoClicked += TrySpawnPhoto;
        _openTravelScreen.AddPlaceClicked += OpenScreen;
        _view.SaveButtonClicked += OnSaveButtonClicked;
        
        foreach (var image in _images)
        {
            image.DeleteButtonClicked += OnPhotoDeleteClicked;
        }
    }

    private void OnDisable()
    {
        _view.PlaceNameChanged -= OnPlaceNameChanged;
        _view.PlaceDescriptionChanged -= OnPlaceDescriptionChanged;
        _view.DateChanged -= OnDateChanged;
        _view.BackButtonClicked -= OnBackButtonClicked;
        _view.AddPhotoClicked -= TrySpawnPhoto;
        _openTravelScreen.AddPlaceClicked -= OpenScreen;
        _view.SaveButtonClicked -= OnSaveButtonClicked;
        
        foreach (var image in _images)
        {
            image.DeleteButtonClicked -= OnPhotoDeleteClicked;
        }
    }
    
    private void DisableAllImages()
    {
        for (int i = 0; i < _images.Count; i++)
        {
            _images[i].Disable();
            _availableWindowIndexes.Add(i);
        }
    }

    private void TrySpawnPhoto()
    {
        if (_availableWindowIndexes.Count > 0)
        {
            int availableIndex = _availableWindowIndexes[0];
            _availableWindowIndexes.RemoveAt(0);

            _currentImage = _images[availableIndex];

            if (_currentImage != null)
            {
                _currentImage.Enable();
                GetImageFromGallery.PickImage(TakePhoto);
            }
        }
    }

    private void TakePhoto(string str)
    {
        if (!string.IsNullOrEmpty(str))
        {
            if (_currentImage != null)
                _currentImage.ImagePicker.Init(str);
        }
    }
    
    private void OnPhotoDeleteClicked(AddPhotoImage photoImage)
    {
        int index = _images.IndexOf(photoImage);

        if (index >= 0)
        {
            _availableWindowIndexes.Add(index);
            _images[index].Disable();
        }
    }

    private void OpenScreen(TripData tripData)
    {
        if (tripData == null)
            throw new ArgumentNullException(nameof(tripData));

        _view.SetTripNameText(tripData.Name);
        _view.Enable();
    }

    private void ValidateInputs()
    {
        bool allInputsValid = !string.IsNullOrEmpty(_placeName) && !string.IsNullOrEmpty(_placeDescription) &&
                              !string.IsNullOrEmpty(_date);

        _view.SetSaveButtonInteractable(allInputsValid);
    }

    private void OnPlaceNameChanged(string name)
    {
        _placeName = name;
        ValidateInputs();
    }

    private void OnPlaceDescriptionChanged(string description)
    {
        _placeDescription = description;
        ValidateInputs();
    }

    private void OnDateChanged(string date)
    {
        _date = date;
        ValidateInputs();
    }

    private void ReturnToDefaultValues()
    {
        _placeName = DefaultTextInputValue;
        _placeDescription = DefaultTextInputValue;
        _date = DefaultTextInputValue;

        _view.SetPlaceNameText(_placeName);
        _view.SetDescriptionText(_placeDescription);
        _view.SetCurrentDate();

        _view.CloseCalendar();

        DisableAllImages();
    }

    private void OnBackButtonClicked()
    {
        BackButtonClicked?.Invoke();
        ReturnToDefaultValues();
        _view.Disable();
    }

    private void OnSaveButtonClicked()
    {
        List<string> spritesToSave = new List<string>();

        foreach (var image in _images)
        {
            if (image.IsActive)
            {
                spritesToSave.Add(image.ImagePicker.CurrentPath);
            }
        }

        PlacesData placesData = new PlacesData(_placeName, _placeDescription, spritesToSave, _date);
        SaveButtonClicked?.Invoke(placesData);
        OnBackButtonClicked();
    }
}
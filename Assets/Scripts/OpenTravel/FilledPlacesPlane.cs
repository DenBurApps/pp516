using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FilledPlacesPlane : MonoBehaviour
{
    private const string DefaultValue = "";
    
    [SerializeField] private TMP_Text _placeNameText;
    [SerializeField] private TMP_Text _placeDescriptionText;
    [SerializeField] private TMP_Text _dateText;
    [SerializeField] private ImagePicker[] _images;
    [SerializeField] private Button _deleteButton;
    [SerializeField] private Button _editButton;
    [SerializeField] private Sprite _defaultImageSprite;

    private PlacesData _placesData;


    private string _placeName;
    private string _placeDescription;
    private string _date;
    private bool _isActive;

    public event Action<FilledPlacesPlane> DeleteButtonClicked;

    public PlacesData PlacesData => _placesData;
    public bool IsActive => _isActive;

    private void OnEnable()
    {
        _deleteButton.onClick.AddListener(OnDeleteButtonClicked);
    }

    private void OnDisable()
    {
        _deleteButton.onClick.RemoveListener(OnDeleteButtonClicked);
    }

    public void FillData(PlacesData data)
    {
        if (data == null)
            throw new ArgumentNullException(nameof(data));

        _placesData = data;

        SetPlaceNameText(data.PlaceName);
        SetDateText(data.Date);
        SetPlaceDescriptionText(data.PlaceDescription);
        SetImages(data.ImagesPath);
    }

    public void SetPlaceNameText(string text)
    {
        _placeName = text;
        _placeNameText.text = _placeName;
    }

    public void SetPlaceDescriptionText(string text)
    {
        _placeDescription = text;
        _placeDescriptionText.text = _placeDescription;
    }

    public void SetDateText(string text)
    {
        _date = text;
        _dateText.text = _date;
    }

    public void SetImages(List<string> images)
    {
        if (images == null || images.Count <= 0)
            return;
        
        foreach (var img in _images)
        {
            img.Image.sprite = _defaultImageSprite;
            img.gameObject.SetActive(false);
        }
        
        for (int i = 0; i < images.Count && i < _images.Length; i++)
        {
            _images[i].gameObject.SetActive(true);
            _images[i].Init(images[i]);
        }
    }
    
    public void Enable()
    {
        gameObject.SetActive(true);
        _isActive = true;
    }

    public void Disable()
    {
        _placesData = null;
        _placeName = null;
        _placeDescription = null;
        _date = null;
        _isActive = false;

        _placeNameText.text = DefaultValue;
        _placeDescriptionText.text = DefaultValue;
        _dateText.text = DefaultValue;

        foreach (var image in _images)
        {
            image.Image.sprite = _defaultImageSprite;
            image.gameObject.SetActive(false);
        }
        
        gameObject.SetActive(false);
        _isActive = false;
    }

    private void OnDeleteButtonClicked()
    {
        DeleteButtonClicked?.Invoke(this);
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScreenVisabilityHandler))]
public class OpenTravelView : MonoBehaviour
{
    private const string PlacesText = " places";

    [SerializeField] private Image _emptyPlacesImage;
    [SerializeField] private Button _editButton;
    [SerializeField] private TMP_Text _tripNameText;
    [SerializeField] private TMP_Text _tripNameSmallText;
    [SerializeField] private TMP_Text _descriptionText;
    [SerializeField] private TMP_Text _dateText;
    [SerializeField] private TMP_Text _placesText;
    [SerializeField] private Button _addPlaceButton;
    [SerializeField] private Button _backButton;
    
    private ScreenVisabilityHandler _screenVisabilityHandler;

    public event Action EditButtonClicked;
    public event Action AddPlaceButtonClicked;
    public event Action BackButtonClicked; 
    
    private void Awake()
    {
        _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
    }

    private void Start()
    {
        Disable();
    }

    private void OnEnable()
    {
        _backButton.onClick.AddListener(OnBackButtonClicked);
        _editButton.onClick.AddListener(OnEditButtonClicked);
        _addPlaceButton.onClick.AddListener(OnAddPlaceButtonClicked);
    }

    private void OnDisable()
    {
        _backButton.onClick.RemoveListener(OnBackButtonClicked);
        _editButton.onClick.RemoveListener(OnEditButtonClicked);
        _addPlaceButton.onClick.RemoveListener(OnAddPlaceButtonClicked);   
    }
    
    public void Enable()
    {
        _screenVisabilityHandler.EnableScreen();
    }

    public void Disable()
    {
        _screenVisabilityHandler.DisableScreen();
    }

    public void ToggleEmptyPlacesImage(bool status)
    {
        _emptyPlacesImage.enabled = status;
    }

    public void SetDateText(string text)
    {
        _dateText.text = text;
    }

    public void SetTripNameText(string text)
    {
        _tripNameText.text = text;
        _tripNameSmallText.text = text;
    }

    public void SetPlacesText(string text)
    {
        _placesText.text = text + PlacesText;
    }

    public void SetDescriptionText(string text)
    {
        _descriptionText.text = text;
    }

    private void OnBackButtonClicked() => BackButtonClicked?.Invoke();
    private void OnEditButtonClicked() => EditButtonClicked?.Invoke();
    private void OnAddPlaceButtonClicked() => AddPlaceButtonClicked?.Invoke();
}

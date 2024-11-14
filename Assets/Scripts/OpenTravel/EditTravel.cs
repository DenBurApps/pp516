using System;
using System.Collections;
using System.Collections.Generic;
using Bitsplash.DatePicker;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScreenVisabilityHandler))]
public class EditTravel : MonoBehaviour
{
    [SerializeField] private Sprite _calendarClosedSprite;
    [SerializeField] private Sprite _calendarOpenedSprite;

    [SerializeField] private TMP_Text _upperTripNameText;
    [SerializeField] private Button _backButton;
    [SerializeField] private TMP_InputField _nameInput;
    [SerializeField] private TMP_InputField _descriptionInput;
    [SerializeField] private TMP_Text _dateText;
    [SerializeField] private Button _saveButtonCalendarClosed;
    [SerializeField] private Button _saveButtonCalendarOpened;
    [SerializeField] private Button _dateButton;
    [SerializeField] private DatePickerSettings _datePicker;
    [SerializeField] private OpenTravel _openTravelScreen;

    private ScreenVisabilityHandler _screenVisabilityHandler;

    private string _newName;
    private string _newDescription;
    private string _newDate;
    
    public event Action BackButtonClicked;
    public event Action<TripData> SaveButtonClicked;
    
    private void Awake()
    {
        _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
    }
    
    private void Start()
    {
        _saveButtonCalendarOpened.gameObject.SetActive(false);
        _dateButton.image.sprite = _calendarClosedSprite;
        _datePicker.gameObject.SetActive(false);
        Disable();
    }

    private void OnEnable()
    {
        _nameInput.onValueChanged.AddListener(OnNameChanged);
        _descriptionInput.onValueChanged.AddListener(OnDescriptionChanged);
        _dateButton.onClick.AddListener(OpenCalendar);
        _saveButtonCalendarClosed.onClick.AddListener(OnSaveButtonClicked);
        _backButton.onClick.AddListener(OnBackButtonClicked);
        _openTravelScreen.EditClicked += OpenWindow;
    }

    private void OnDisable()
    {
        _nameInput.onValueChanged.RemoveListener(OnNameChanged);
        _descriptionInput.onValueChanged.RemoveListener(OnDescriptionChanged);
        _dateButton.onClick.RemoveListener(OpenCalendar);
        _saveButtonCalendarClosed.onClick.RemoveListener(OnSaveButtonClicked);
        _backButton.onClick.RemoveListener(OnBackButtonClicked);
        _openTravelScreen.EditClicked -= OpenWindow;
    }

    private void OpenWindow(TripData tripData)
    {
        if (tripData == null)
            throw new ArgumentNullException(nameof(tripData));
        
        SetNameText(tripData.Name);
        SetDescriptionText(tripData.Description);
        SetDateText(tripData.Date);
        Enable();
    }
    
    private void SetDate()
    {
        string text = "";
        var selection = _datePicker.Content.Selection;
        for (int i=0; i< selection.Count; i++)
        {
            var date = selection.GetItem(i);
            text += date.ToString(format:"dd.MM.yyyy");
        }
        _dateText.text = text;
        OnDateChanged(text);
    }
    
    private void OpenCalendar()
    {
        _dateButton.onClick.RemoveListener(OpenCalendar);
        _dateButton.image.sprite = _calendarOpenedSprite;
        
        _saveButtonCalendarClosed.onClick.RemoveListener(OnSaveButtonClicked);
        _saveButtonCalendarClosed.gameObject.SetActive(false);
        
        _saveButtonCalendarOpened.gameObject.SetActive(true);
        _saveButtonCalendarOpened.onClick.AddListener(OnSaveButtonClicked);
        
        _dateButton.onClick.AddListener(CloseCalendar);
        _datePicker.gameObject.SetActive(true);
        _datePicker.Content.OnSelectionChanged.AddListener(SetDate);
    }

    public void CloseCalendar()
    {
        _saveButtonCalendarOpened.onClick.RemoveListener(OnSaveButtonClicked);
        _saveButtonCalendarOpened.gameObject.SetActive(false);
        
        _dateButton.image.sprite = _calendarClosedSprite;
        
        _saveButtonCalendarClosed.gameObject.SetActive(true);
        _saveButtonCalendarClosed.onClick.AddListener(OnSaveButtonClicked);
        
        _datePicker.gameObject.SetActive(false);
        _dateButton.onClick.RemoveListener(CloseCalendar);
        _dateButton.onClick.AddListener(OpenCalendar);
    }
    
    private void SetNameText(string value)
    {
        _nameInput.text = value;
        _upperTripNameText.text = value;
    }

    private void SetDescriptionText(string value)
    {
        _descriptionInput.text = value;
    }

    private void SetDateText(string value)
    {
        _dateText.text = value;
    }
    
    private void OnDescriptionChanged(string description)
    {
        _newDescription = description;
        ValidateInputs();
    }

    private void OnNameChanged(string name)
    {
        _newName = name;
        ValidateInputs();
    }

    private void OnDateChanged(string date)
    {
        _newDate = date;
        ValidateInputs();
    }

    private void OnSaveButtonClicked()
    {
        TripData tripData = new TripData(_newName, _newDescription, _newDate);
        SaveButtonClicked?.Invoke(tripData);
        OnBackButtonClicked();
    }
    
    private void ValidateInputs()
    {
        bool allInputsValid = !string.IsNullOrEmpty(_newName) && !string.IsNullOrEmpty(_newDescription);

        SetSaveButtonInteractable(allInputsValid);
    }
    
    private void SetSaveButtonInteractable(bool isInteractable)
    {
        if(_saveButtonCalendarClosed.enabled)
            _saveButtonCalendarClosed.interactable = isInteractable;
        
        if(_saveButtonCalendarOpened.enabled)
            _saveButtonCalendarOpened.interactable = isInteractable;
    }
    
    private void OnBackButtonClicked()
    {
        BackButtonClicked?.Invoke();
        Disable();
    }
    
    private void Enable()
    {
        _screenVisabilityHandler.EnableScreen();
    }

    private void Disable()
    {
        _screenVisabilityHandler.DisableScreen();
    }
}

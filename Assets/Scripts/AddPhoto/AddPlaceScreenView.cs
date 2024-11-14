using System;
using System.Collections.Generic;
using Bitsplash.DatePicker;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScreenVisabilityHandler))]
public class AddPlaceScreenView : MonoBehaviour
{
    [SerializeField] private Sprite _calendarClosedSprite;
    [SerializeField] private Sprite _calendarOpenedSprite;

    [SerializeField] private TMP_Text _tripNameText;
    [SerializeField] private Button _editButton;
    [SerializeField] private Button _backButton;
    [SerializeField] private Button _addPhotoButton;
    [SerializeField] private TMP_InputField _placeNameInputField;
    [SerializeField] private TMP_InputField _placeDescriptionInputField;
    [SerializeField] private TMP_Text _dateText;
    [SerializeField] private DatePickerSettings _datePicker;
    [SerializeField] private Button _saveButtonCalendarClosed;
    [SerializeField] private Button _saveButtonCalendarOpened;
    [SerializeField] private Button _dateButton;

    private ScreenVisabilityHandler _screenVisabilityHandler;

    public event Action<string> PlaceNameChanged;
    public event Action<string> DateChanged;
    public event Action<string> PlaceDescriptionChanged;
    public event Action SaveButtonClicked;
    public event Action BackButtonClicked;
    public event Action AddPhotoClicked;

    private void Awake()
    {
        _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
    }

    private void Start()
    {
        _saveButtonCalendarOpened.gameObject.SetActive(false);
        _dateButton.image.sprite = _calendarClosedSprite;
        _datePicker.gameObject.SetActive(false);
        SetCurrentDate();
    }

    private void OnEnable()
    {
        _dateButton.onClick.AddListener(OpenCalendar);
        _saveButtonCalendarClosed.onClick.AddListener(OnSaveButtonClicked);
        _backButton.onClick.AddListener(OnBackButtonClicked);
        _placeNameInputField.onValueChanged.AddListener(OnPlaceNameChanged);
        _placeDescriptionInputField.onValueChanged.AddListener(OnPlaceDescriptionChanged);
        _saveButtonCalendarClosed.onClick.AddListener(OnSaveButtonClicked);
        _addPhotoButton.onClick.AddListener(OnAddPhotoClicked);
    }

    private void OnDisable()
    {
        _dateButton.onClick.RemoveListener(OpenCalendar);
        _saveButtonCalendarClosed.onClick.RemoveListener(OnSaveButtonClicked);
        _backButton.onClick.RemoveListener(OnBackButtonClicked);
        _placeNameInputField.onValueChanged.RemoveListener(OnPlaceNameChanged);
        _placeDescriptionInputField.onValueChanged.RemoveListener(OnPlaceDescriptionChanged);
        _saveButtonCalendarClosed.onClick.RemoveListener(OnSaveButtonClicked);
        _addPhotoButton.onClick.RemoveListener(OnAddPhotoClicked);
    }

    public void SetCurrentDate()
    {
        _dateText.text = DateTime.Now.ToString("dd.MM.yyyy");
        DateChanged?.Invoke(_dateText.text);
    }

    public void SetSaveButtonInteractable(bool isInteractable)
    {
        if (_saveButtonCalendarClosed.enabled)
            _saveButtonCalendarClosed.interactable = isInteractable;

        if (_saveButtonCalendarOpened.enabled)
            _saveButtonCalendarOpened.interactable = isInteractable;
    }

    public void Enable()
    {
        _screenVisabilityHandler.EnableScreen();
    }

    public void Disable()
    {
        _screenVisabilityHandler.DisableScreen();
    }

    public void SetPlaceNameText(string text)
    {
        _placeNameInputField.text = text;
    }

    public void SetDescriptionText(string text)
    {
        _placeDescriptionInputField.text = text;
    }

    public void SetTripNameText(string text)
    {
        _tripNameText.text = text;
    }

    private void SetDate()
    {
        string text = "";
        var selection = _datePicker.Content.Selection;
        for (int i = 0; i < selection.Count; i++)
        {
            var date = selection.GetItem(i);
            text += date.ToString(format: "dd.MM.yyyy");
        }

        _dateText.text = text;
        DateChanged?.Invoke(_dateText.text);
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

    private void OnSaveButtonClicked() => SaveButtonClicked?.Invoke();
    private void OnPlaceNameChanged(string value) => PlaceNameChanged?.Invoke(value);
    private void OnPlaceDescriptionChanged(string value) => PlaceDescriptionChanged?.Invoke(value);
    private void OnBackButtonClicked() => BackButtonClicked?.Invoke();
    private void OnAddPhotoClicked() => AddPhotoClicked?.Invoke();
}
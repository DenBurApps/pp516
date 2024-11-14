using System;
using Bitsplash.DatePicker;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScreenVisabilityHandler))]
public class CreateTravelScreenView : MonoBehaviour
{
    [SerializeField] private Sprite _calendarClosedSprite;
    [SerializeField] private Sprite _calendarOpenedSprite;
    
    [SerializeField] private Button _backButton;
    [SerializeField] private TMP_InputField _nameInput;
    [SerializeField] private TMP_InputField _descriptionInput;
    [SerializeField] private TMP_Text _dateText;
    [SerializeField] private Button _saveButtonCalendarClosed;
    [SerializeField] private Button _saveButtonCalendarOpened;
    [SerializeField] private Button _dateButton;
    [SerializeField] private DatePickerSettings _datePicker;

    private ScreenVisabilityHandler _screenVisabilityHandler;

    public event Action<string> NameChanged;
    public event Action<string> DescriptionChanged;
    public event Action<string> DateChanged;
    public event Action BackButtonClicked;
    public event Action SaveButtonClicked;
    
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
        _nameInput.onValueChanged.AddListener(OnNameInputChanged);
        _descriptionInput.onValueChanged.AddListener(OnDescriptionChanged);
        _dateButton.onClick.AddListener(OpenCalendar);
        _saveButtonCalendarClosed.onClick.AddListener(OnSaveButtonClicked);
        _backButton.onClick.AddListener(OnBackButtonClicked);
    }

    private void OnDisable()
    {
        _nameInput.onValueChanged.RemoveListener(OnNameInputChanged);
        _descriptionInput.onValueChanged.RemoveListener(OnDescriptionChanged);
        _dateButton.onClick.RemoveListener(OpenCalendar);
        _saveButtonCalendarClosed.onClick.RemoveListener(OnSaveButtonClicked);
        _backButton.onClick.RemoveListener(OnBackButtonClicked);
    }

    public void SetCurrentDate()
    {
        _dateText.text = DateTime.Now.ToString("dd.MM.yyyy");
        DateChanged?.Invoke(_dateText.text);
    }
    
    public void SetSaveButtonInteractable(bool isInteractable)
    {
        if(_saveButtonCalendarClosed.isActiveAndEnabled)
            _saveButtonCalendarClosed.interactable = isInteractable;
        
        if(_saveButtonCalendarOpened.isActiveAndEnabled)
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

    public void SetNameValue(string value)
    {
        _nameInput.text = value;
    }

    public void SetDescriptionValue(string value)
    {
        _descriptionInput.text = value;
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

    private void OnNameInputChanged(string value) => NameChanged?.Invoke(value);
    private void OnDescriptionChanged(string value) => DescriptionChanged?.Invoke(value);
    private void OnSaveButtonClicked() => SaveButtonClicked?.Invoke();
    private void OnBackButtonClicked() => BackButtonClicked?.Invoke();
}

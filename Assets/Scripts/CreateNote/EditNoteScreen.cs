using System;
using Bitsplash.DatePicker;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScreenVisabilityHandler))]
public class EditNoteScreen : MonoBehaviour
{
    private const string DefaultText = "";
    
    [SerializeField] private Sprite _calendarClosedSprite;
    [SerializeField] private Sprite _calendarOpenedSprite;
    
    [SerializeField] private Button _backButton;
    [SerializeField] private Button _editButton;
    [SerializeField] private Button _dateButton;
    [SerializeField] private TMP_InputField _noteInput;
    [SerializeField] private TMP_Text _dateText;
    [SerializeField] private DatePickerSettings _datePicker;
    [SerializeField] private Button _saveButtonCalendarClosed;
    [SerializeField] private Button _saveButtonCalendarOpened;

    [SerializeField] private OpenNotesScreen _openNotesScreen;
    
    private ScreenVisabilityHandler _screenVisabilityHandler;
    private FilledNoteInfo _filledNoteInfo;

    private string _newNote;
    private string _newDate;
    private bool _dateChanged;

    public event Action<NoteData> SaveButtonClicked;
    public event Action BackButtonClicked; 

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
        _noteInput.onValueChanged.AddListener(OnNoteChanged);
        _openNotesScreen.EditButtonClicked += ProcessWindowEnable;
        _dateButton.onClick.AddListener(OpenCalendar);
        _backButton.onClick.AddListener(OnBackButtonClicked);
    }

    private void OnDisable()
    {
        _noteInput.onValueChanged.RemoveListener(OnNoteChanged);
        _openNotesScreen.EditButtonClicked -= ProcessWindowEnable;
        _dateButton.onClick.RemoveListener(OpenCalendar);
        _backButton.onClick.RemoveListener(OnBackButtonClicked);
    }
    
    public void Enable()
    {
        _screenVisabilityHandler.EnableScreen();
    }

    public void Disable()
    {
        _screenVisabilityHandler.DisableScreen();
    }

    private void ProcessWindowEnable(FilledNoteInfo noteInfo)
    {
        if (noteInfo == null)
            throw new ArgumentNullException(nameof(noteInfo));

        _filledNoteInfo = noteInfo;

        _newNote = null;
        _newDate = null;
        
        _noteInput.text = noteInfo.Note;
        _dateText.text = noteInfo.Date;
        _newDate = noteInfo.Date;
        
        Enable();
        ValidateInputs();
        _saveButtonCalendarClosed.interactable = false;
    }
    

    private void OnNoteChanged(string note)
    {
        _newNote = note;
        ValidateInputs();
    }

    private void OnDateChanged(string date)
    {
        _newDate = date;
        _dateChanged = true;
        ValidateInputs();
    }
    
    private void ValidateInputs()
    {
        bool allInputsValid = !string.IsNullOrEmpty(_newNote);

        SetSaveButtonInteractable(allInputsValid);
    }
    
    private void OnSaveButtonClicked()
    {
        if(string.IsNullOrEmpty(_newNote) && !_dateChanged)
           return;
        
        NoteData noteData = new NoteData(_newNote, _newDate);
        _filledNoteInfo.SetNoteData(noteData);
        SaveButtonClicked?.Invoke(noteData);
        OnBackButtonClicked();
    }
    
    private void OnBackButtonClicked()
    {
        _noteInput.text = DefaultText;
        _dateText.text = DefaultText;
        _dateChanged = false;

        CloseCalendar();
        BackButtonClicked?.Invoke();
        Disable();
    }
    
    private void SetSaveButtonInteractable(bool isInteractable)
    {
        if (_saveButtonCalendarClosed.enabled)
            _saveButtonCalendarClosed.interactable = isInteractable;

        if (_saveButtonCalendarOpened.enabled)
            _saveButtonCalendarOpened.interactable = isInteractable;
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

    private void CloseCalendar()
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
}

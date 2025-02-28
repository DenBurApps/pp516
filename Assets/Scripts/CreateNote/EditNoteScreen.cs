using System;
using Bitsplash.DatePicker;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; // Added DoTween import

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
    
    [Header("Animation Settings")]
    [SerializeField] private float _animationDuration = 0.4f;
    [SerializeField] private Ease _showEaseType = Ease.OutBack;
    [SerializeField] private Ease _hideEaseType = Ease.InBack;
    [SerializeField] private RectTransform _contentPanel;
    
    private ScreenVisabilityHandler _screenVisabilityHandler;
    private FilledNoteInfo _filledNoteInfo;
    private Sequence _showSequence;
    private Sequence _hideSequence;
    private Sequence _calendarSequence;

    private string _newNote;
    private string _newDate;
    private bool _dateChanged;

    public event Action<NoteData> SaveButtonClicked;
    public event Action BackButtonClicked; 

    private void Awake()
    {
        _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
        InitializeAnimations();
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
        
        // Kill any active tweens
        if (_showSequence != null)
            _showSequence.Kill();
        if (_hideSequence != null)
            _hideSequence.Kill();
        if (_calendarSequence != null)
            _calendarSequence.Kill();
    }
    
    private void InitializeAnimations()
    {
        // Create show sequence
        _showSequence = DOTween.Sequence();
        _showSequence.SetAutoKill(false);
        _showSequence.Pause();
        
        // Create hide sequence
        _hideSequence = DOTween.Sequence();
        _hideSequence.SetAutoKill(false);
        _hideSequence.Pause();
        
        // Create calendar sequence
        _calendarSequence = DOTween.Sequence();
        _calendarSequence.SetAutoKill(false);
        _calendarSequence.Pause();
        
        if (_contentPanel != null)
        {
            // Animate entire content panel
            _contentPanel.localScale = Vector3.zero;
            _showSequence.Append(_contentPanel.DOScale(Vector3.one, _animationDuration).SetEase(_showEaseType));
            _hideSequence.Append(_contentPanel.DOScale(Vector3.zero, _animationDuration).SetEase(_hideEaseType));
            
            // Add individual element animations
            if (_backButton != null)
            {
                // Initial state
                _backButton.transform.localScale = Vector3.zero;
                
                // Add to sequences
                _showSequence.Join(_backButton.transform.DOScale(1, _animationDuration * 0.8f)
                    .SetEase(Ease.OutBounce).SetDelay(0.1f));
                _hideSequence.Join(_backButton.transform.DOScale(0, _animationDuration * 0.5f)
                    .SetEase(_hideEaseType));
            }
            
            if (_saveButtonCalendarClosed != null)
            {
                // Initial state
                _saveButtonCalendarClosed.transform.localScale = Vector3.zero;
                
                // Add to sequences
                _showSequence.Join(_saveButtonCalendarClosed.transform.DOScale(1, _animationDuration * 0.8f)
                    .SetEase(Ease.OutBounce).SetDelay(0.2f));
                _hideSequence.Join(_saveButtonCalendarClosed.transform.DOScale(0, _animationDuration * 0.5f)
                    .SetEase(_hideEaseType));
            }
            
            if (_dateButton != null)
            {
                // Initial state
                _dateButton.transform.localScale = Vector3.zero;
                
                // Add to sequences
                _showSequence.Join(_dateButton.transform.DOScale(1, _animationDuration * 0.8f)
                    .SetEase(Ease.OutBounce).SetDelay(0.3f));
                _hideSequence.Join(_dateButton.transform.DOScale(0, _animationDuration * 0.5f)
                    .SetEase(_hideEaseType));
            }
            
            if (_noteInput != null)
            {
                // Create a reference to the RectTransform
                RectTransform inputRect = _noteInput.GetComponent<RectTransform>();
                
                // Initial state
                Vector2 originalSize = inputRect.sizeDelta;
                inputRect.sizeDelta = new Vector2(0, originalSize.y);
                
                // Add to sequences
                _showSequence.Join(inputRect.DOSizeDelta(originalSize, _animationDuration)
                    .SetEase(Ease.OutExpo).SetDelay(0.2f));
                _hideSequence.Join(inputRect.DOSizeDelta(new Vector2(0, originalSize.y), _animationDuration * 0.7f)
                    .SetEase(Ease.InExpo));
            }
            
            if (_dateText != null)
            {
                // Initial state
                _dateText.alpha = 0;
                
                // Add to sequences
                _showSequence.Join(_dateText.DOFade(1, _animationDuration).SetEase(Ease.OutQuad));
                _hideSequence.Join(_dateText.DOFade(0, _animationDuration * 0.5f).SetEase(Ease.InQuad));
            }
        }
        
        // Set callback for hide sequence
        _hideSequence.OnComplete(OnHideComplete);
    }
    
    private void OnHideComplete()
    {
        _screenVisabilityHandler.DisableScreen();
    }
    
    public void Enable()
    {
        _screenVisabilityHandler.EnableScreen();
        
        // Reset and play animations
        if (_hideSequence != null)
            _hideSequence.Pause();
        if (_showSequence != null)
            _showSequence.Restart();
    }

    public void Disable()
    {
        // Play hide sequence - the screen will be disabled via OnComplete callback
        if (_showSequence != null)
            _showSequence.Pause();
        if (_hideSequence != null)
            _hideSequence.Restart();
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
        
        // Add a ripple effect to the note input field to draw attention
        if (_noteInput != null)
        {
            RectTransform inputRect = _noteInput.GetComponent<RectTransform>();
            inputRect.DOShakePosition(0.5f, 5f, 10, 90f, false, true)
                .SetDelay(0.5f);
        }
    }
    
    private void OnNoteChanged(string note)
    {
        _newNote = note;
        ValidateInputs();
        
        // Add subtle typing animation
        if (_noteInput != null)
        {
            _noteInput.transform.DOPunchScale(new Vector3(0.02f, 0.02f, 0.02f), 0.1f, 1, 0.5f);
        }
    }

    private void OnDateChanged(string date)
    {
        _newDate = date;
        _dateChanged = true;
        ValidateInputs();
        
        // Add date change animation
        if (_dateText != null)
        {
            Sequence dateChangeSeq = DOTween.Sequence();
            dateChangeSeq.Append(_dateText.transform.DOScale(1.1f, 0.15f).SetEase(Ease.OutQuad));
            dateChangeSeq.Append(_dateText.transform.DOScale(1f, 0.15f).SetEase(Ease.InQuad));
            dateChangeSeq.Play();
        }
    }
    
    private void ValidateInputs()
    {
        bool allInputsValid = !string.IsNullOrEmpty(_newNote);

        SetSaveButtonInteractable(allInputsValid);
        
        // Animate save button based on validity
        if (allInputsValid)
        {
            if (_saveButtonCalendarClosed.gameObject.activeSelf)
            {
                _saveButtonCalendarClosed.transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), 0.3f, 2, 0.5f);
            }
            else if (_saveButtonCalendarOpened.gameObject.activeSelf)
            {
                _saveButtonCalendarOpened.transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), 0.3f, 2, 0.5f);
            }
        }
    }
    
    private void OnSaveButtonClicked()
    {
        if(string.IsNullOrEmpty(_newNote) && !_dateChanged)
           return;
        
        // Animate save button
        Button activeButton = _saveButtonCalendarClosed.gameObject.activeSelf ? 
            _saveButtonCalendarClosed : _saveButtonCalendarOpened;
        
        activeButton.transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f), 0.3f, 5, 0.5f);
        
        NoteData noteData = new NoteData(_newNote, _newDate);
        _filledNoteInfo.SetNoteData(noteData);
        SaveButtonClicked?.Invoke(noteData);
        OnBackButtonClicked();
    }
    
    private void OnBackButtonClicked()
    {
        // Animate back button
        if (_backButton != null)
        {
            _backButton.transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f), 0.3f, 5, 0.5f);
        }
        
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
        
        // Animate the transition
        Sequence calendarOpenSeq = DOTween.Sequence();
        
        // Hide save button closed with animation
        calendarOpenSeq.Append(_saveButtonCalendarClosed.transform
            .DOScale(0, 0.2f).SetEase(Ease.InBack)
            .OnComplete(() => _saveButtonCalendarClosed.gameObject.SetActive(false)));
        
        // Show save button opened with animation
        calendarOpenSeq.AppendCallback(() => _saveButtonCalendarOpened.gameObject.SetActive(true));
        calendarOpenSeq.Append(_saveButtonCalendarOpened.transform
            .DOScale(1, 0.3f).SetEase(Ease.OutBack));
        
        _saveButtonCalendarOpened.onClick.AddListener(OnSaveButtonClicked);

        _dateButton.onClick.AddListener(CloseCalendar);
        
        // Animate calendar appearance
        _datePicker.gameObject.SetActive(true);
        RectTransform calendarRect = _datePicker.GetComponent<RectTransform>();
        calendarRect.localScale = Vector3.zero;
        calendarRect.DOScale(1, 0.4f).SetEase(Ease.OutBack);
        
        _datePicker.Content.OnSelectionChanged.AddListener(SetDate);
        
        // Animate date button
        _dateButton.transform.DOPunchRotation(new Vector3(0, 0, 10), 0.5f, 2, 0.5f);
    }

    private void CloseCalendar()
    {
        _saveButtonCalendarOpened.onClick.RemoveListener(OnSaveButtonClicked);
        
        // Animate the transition
        Sequence calendarCloseSeq = DOTween.Sequence();
        
        // Hide save button opened with animation
        calendarCloseSeq.Append(_saveButtonCalendarOpened.transform
            .DOScale(0, 0.2f).SetEase(Ease.InBack)
            .OnComplete(() => _saveButtonCalendarOpened.gameObject.SetActive(false)));
        
        // Show save button closed with animation
        calendarCloseSeq.AppendCallback(() => _saveButtonCalendarClosed.gameObject.SetActive(true));
        calendarCloseSeq.Append(_saveButtonCalendarClosed.transform
            .DOScale(1, 0.3f).SetEase(Ease.OutBack));

        _dateButton.image.sprite = _calendarClosedSprite;
        _saveButtonCalendarClosed.onClick.AddListener(OnSaveButtonClicked);

        // Animate calendar disappearance
        RectTransform calendarRect = _datePicker.GetComponent<RectTransform>();
        calendarRect.DOScale(0, 0.3f).SetEase(Ease.InBack)
            .OnComplete(() => _datePicker.gameObject.SetActive(false));
        
        _dateButton.onClick.RemoveListener(CloseCalendar);
        _dateButton.onClick.AddListener(OpenCalendar);
        
        // Animate date button
        _dateButton.transform.DOPunchRotation(new Vector3(0, 0, -10), 0.5f, 2, 0.5f);
    }
    
    private void OnDestroy()
    {
        // Clean up sequences when object is destroyed
        if (_showSequence != null)
            _showSequence.Kill();
        if (_hideSequence != null)
            _hideSequence.Kill();
        if (_calendarSequence != null)
            _calendarSequence.Kill();
    }
}
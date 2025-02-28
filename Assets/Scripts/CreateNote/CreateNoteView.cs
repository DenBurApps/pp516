using System;
using Bitsplash.DatePicker;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; // Added DOTween namespace
using System.Collections;

[RequireComponent(typeof(ScreenVisabilityHandler))]
public class CreateNoteView : MonoBehaviour
{
    [SerializeField] private Sprite _calendarClosedSprite;
    [SerializeField] private Sprite _calendarOpenedSprite;

    [SerializeField] private TMP_InputField _noteInput;
    [SerializeField] private Button _backButton;
    [SerializeField] private Button _dateButton;
    [SerializeField] private Button _saveButtonCalendarClosed;
    [SerializeField] private Button _saveButtonCalendarOpened;
    [SerializeField] private TMP_Text _dateText;
    [SerializeField] private DatePickerSettings _datePicker;
    
    [Header("Animation Settings")]
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private RectTransform _datePickerTransform;
    [SerializeField] private RectTransform _noteInputTransform;
    [SerializeField] private float _buttonPunchStrength = 0.2f;
    [SerializeField] private float _calendarExpandDuration = 0.3f;
    [SerializeField] private float _calendarCollapseDuration = 0.2f;
    [SerializeField] private Ease _calendarEase = Ease.OutBack;

    private ScreenVisabilityHandler _screenVisabilityHandler;
    private Vector3 _saveButtonOriginalScale;
    private Vector3 _datePickerOriginalPosition;
    private Vector3 _datePickerOriginalScale;

    public event Action<string> NoteChanged;
    public event Action<string> DateChanged;
    public event Action SaveButtonClicked;
    public event Action BackButtonClicked;
    
    private void Awake()
    {
        _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
        
        // Store original scales for animations
        if (_saveButtonCalendarClosed != null)
            _saveButtonOriginalScale = _saveButtonCalendarClosed.transform.localScale;
            
        if (_datePickerTransform != null)
        {
            _datePickerOriginalScale = _datePickerTransform.localScale;
            _datePickerOriginalPosition = _datePickerTransform.position;
        }
        
        // Ensure we have a CanvasGroup component for fading
        if (_canvasGroup == null)
            _canvasGroup = GetComponent<CanvasGroup>();
        if (_canvasGroup == null)
            _canvasGroup = gameObject.AddComponent<CanvasGroup>();
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
        // First remove any existing listeners to prevent duplicates
        _dateButton.onClick.RemoveListener(OpenCalendar);
        _saveButtonCalendarClosed.onClick.RemoveListener(OnSaveButtonClicked);
        _backButton.onClick.RemoveListener(OnBackButtonClicked);
        _noteInput.onValueChanged.RemoveListener(OnNoteChanged);
        
        // Then add listeners
        _dateButton.onClick.AddListener(OpenCalendar);
        _saveButtonCalendarClosed.onClick.AddListener(OnSaveButtonClicked);
        _backButton.onClick.AddListener(OnBackButtonClicked);
        _noteInput.onValueChanged.AddListener(OnNoteChanged);
    }

    private void OnDisable()
    {
        _dateButton.onClick.RemoveListener(OpenCalendar);
        _saveButtonCalendarClosed.onClick.RemoveListener(OnSaveButtonClicked);
        _backButton.onClick.RemoveListener(OnBackButtonClicked);
        _noteInput.onValueChanged.RemoveListener(OnNoteChanged);
    }

    // Animation methods
    public void AnimateOpen(float duration, Ease ease)
    {
        // Ensure everything is ready for animation
        _canvasGroup.alpha = 0f;
        
        // Animate fade in
        _canvasGroup.DOFade(1f, duration).SetEase(ease);
        
        // Animate input field appearance
        if (_noteInputTransform != null)
        {
            _noteInputTransform.localScale = Vector3.zero;
            _noteInputTransform.DOScale(Vector3.one, duration).SetEase(ease).SetDelay(0.1f);
        }
        
        // Animate buttons
        AnimateButtonsIn(duration, ease);
    }
    
    public void AnimateClose(float duration, Ease ease, Action onComplete = null)
    {
        // Animate fade out
        _canvasGroup.DOFade(0f, duration).SetEase(ease).OnComplete(() => {
            onComplete?.Invoke();
        });
    }
    
    private void AnimateButtonsIn(float duration, Ease ease)
    {
        float delay = 0f;
        
        // Stagger button animations
        _backButton.transform.localScale = Vector3.zero;
        _backButton.transform.DOScale(Vector3.one, duration).SetEase(ease).SetDelay(delay);
        delay += 0.1f;
        
        _dateButton.transform.localScale = Vector3.zero;
        _dateButton.transform.DOScale(Vector3.one, duration).SetEase(ease).SetDelay(delay);
        delay += 0.1f;
        
        if (_saveButtonCalendarClosed.gameObject.activeInHierarchy)
        {
            _saveButtonCalendarClosed.transform.localScale = Vector3.zero;
            _saveButtonCalendarClosed.transform.DOScale(_saveButtonOriginalScale, duration).SetEase(ease).SetDelay(delay);
        }
    }
    
    public void AnimateSaveButton(float duration, Ease ease)
    {
        Button activeButton = _saveButtonCalendarClosed.gameObject.activeInHierarchy ? 
            _saveButtonCalendarClosed : _saveButtonCalendarOpened;
        
        // Short pulse animation
        activeButton.transform.DOPunchScale(Vector3.one * _buttonPunchStrength, duration, 1, 0.5f).SetEase(ease);
    }
    
    public void AnimateSaveButtonPress(float duration, Ease ease, Action onComplete = null)
    {
        Button activeButton = _saveButtonCalendarClosed.gameObject.activeInHierarchy ? 
            _saveButtonCalendarClosed : _saveButtonCalendarOpened;
            
        Sequence sequence = DOTween.Sequence();
        sequence.Append(activeButton.transform.DOScale(Vector3.one * 0.9f, duration * 0.5f).SetEase(Ease.InQuad));
        sequence.Append(activeButton.transform.DOScale(_saveButtonOriginalScale, duration * 0.5f).SetEase(Ease.OutQuad));
        sequence.OnComplete(() => {
            onComplete?.Invoke();
        });
    }
    
    // Original methods with animation added
    public void SetCurrentDate()
    {
        _dateText.text = DateTime.Now.ToString("dd.MM.yyyy");
        DateChanged?.Invoke(_dateText.text);
        
        // Animate date text
        AnimateDateTextUpdate();
    }
    
    private void AnimateDateTextUpdate()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(_dateText.transform.DOScale(1.1f, 0.2f).SetEase(Ease.OutQuad));
        sequence.Append(_dateText.transform.DOScale(1f, 0.15f).SetEase(Ease.InOutQuad));
    }

    public void SetSaveButtonInteractable(bool isInteractable)
    {
        if (_saveButtonCalendarClosed.isActiveAndEnabled)
            _saveButtonCalendarClosed.interactable = isInteractable;

        if (_saveButtonCalendarOpened.isActiveAndEnabled)
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
        
        // Animate date text when changed
        AnimateDateTextUpdate();
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
        
        // Animate calendar opening
        AnimateCalendarOpen();
    }
    
    private void AnimateCalendarOpen()
    {
        if (_datePickerTransform != null)
        {
            // Start from scaled down
            _datePickerTransform.localScale = Vector3.zero;
            
            // Animate scale up with a nice bounce effect
            _datePickerTransform.DOScale(_datePickerOriginalScale, _calendarExpandDuration)
                .SetEase(_calendarEase);
        }
    }

    public void CloseCalendar()
    {
        // Animate calendar closing
        AnimateCalendarClose(() => {
            _saveButtonCalendarOpened.onClick.RemoveListener(OnSaveButtonClicked);
            _saveButtonCalendarOpened.gameObject.SetActive(false);

            _dateButton.image.sprite = _calendarClosedSprite;

            _saveButtonCalendarClosed.gameObject.SetActive(true);
            _saveButtonCalendarClosed.onClick.AddListener(OnSaveButtonClicked);

            _datePicker.gameObject.SetActive(false);
            _dateButton.onClick.RemoveListener(CloseCalendar);
            _dateButton.onClick.AddListener(OpenCalendar);
        });
    }
    
    private void AnimateCalendarClose(Action onComplete = null)
    {
        if (_datePickerTransform != null && _datePicker.gameObject.activeInHierarchy)
        {
            // Animate scale down
            _datePickerTransform.DOScale(Vector3.zero, _calendarCollapseDuration)
                .SetEase(Ease.InBack)
                .OnComplete(() => {
                    onComplete?.Invoke();
                });
        }
        else
        {
            onComplete?.Invoke();
        }
    }

    public void SetNoteText(string text)
    {
        _noteInput.text = text;
        
        // Small animation when text is cleared
        if (string.IsNullOrEmpty(text))
        {
            if (_noteInputTransform != null)
            {
                _noteInputTransform.DOShakePosition(0.3f, strength: 5, vibrato: 5, randomness: 90, snapping: false, fadeOut: true);
            }
        }
    }

    private void OnNoteChanged(string value) => NoteChanged?.Invoke(value);
    private void OnSaveButtonClicked() => SaveButtonClicked?.Invoke();
    private void OnBackButtonClicked() => BackButtonClicked?.Invoke();
}
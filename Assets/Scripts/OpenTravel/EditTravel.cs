using System;
using System.Collections;
using System.Collections.Generic;
using Bitsplash.DatePicker;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; // Added DOTween namespace

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

    // Animation parameters
    [Header("Animation Settings")]
    [SerializeField] private float _animationDuration = 0.5f;
    [SerializeField] private Ease _animationEase = Ease.OutBack;
    [SerializeField] private float _staggerDelay = 0.1f;
    [SerializeField] private Transform _formContainer; // Container for form elements to animate

    private ScreenVisabilityHandler _screenVisabilityHandler;
    private Sequence _animationSequence;
    private Vector3 _initialSaveButtonScale;
    private Vector3 _initialDateButtonScale;

    private string _newName;
    private string _newDescription;
    private string _newDate;
    
    public event Action BackButtonClicked;
    public event Action<TripData> SaveButtonClicked;
    
    private void Awake()
    {
        _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
        _initialSaveButtonScale = _saveButtonCalendarClosed.transform.localScale;
        _initialDateButtonScale = _dateButton.transform.localScale;
        
        // Initialize DOTween (if not initialized elsewhere)
        DOTween.SetTweensCapacity(500, 50);
    }
    
    private void Start()
    {
        _saveButtonCalendarOpened.gameObject.SetActive(false);
        _dateButton.image.sprite = _calendarClosedSprite;
        _datePicker.gameObject.SetActive(false);
        
        // Set initial animation states
        SetInitialAnimationStates();
        
        Disable();
    }

    private void SetInitialAnimationStates()
    {
        // Set initial states for animations if needed
        if (_formContainer != null)
        {
            foreach (Transform child in _formContainer)
            {
                child.localScale = Vector3.zero;
                CanvasGroup canvasGroup = child.GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 0;
                }
            }
        }
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
        
        // Kill all animations when disabled
        _animationSequence?.Kill();
    }

    private void OpenWindow(TripData tripData)
    {
        if (tripData == null)
            throw new ArgumentNullException(nameof(tripData));
        
        SetNameText(tripData.Name);
        SetDescriptionText(tripData.Description);
        SetDateText(tripData.Date);
        Enable();
        
        // Play entry animation
        PlayEnterAnimation();
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
        
        // Animate date text update
        _dateText.DOFade(0f, _animationDuration * 0.3f).OnComplete(() => {
            _dateText.text = text;
            _dateText.DOFade(1f, _animationDuration * 0.3f);
        });
        
        OnDateChanged(text);
    }
    
    private void OpenCalendar()
    {
        _dateButton.onClick.RemoveListener(OpenCalendar);
        
        // Animate button sprite change
        _dateButton.transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f), 0.3f, 1, 0.5f).OnComplete(() => {
            _dateButton.image.sprite = _calendarOpenedSprite;
        });
        
        _saveButtonCalendarClosed.onClick.RemoveListener(OnSaveButtonClicked);
        
        // Animate button swap
        _saveButtonCalendarClosed.transform.DOScale(0f, _animationDuration * 0.5f).SetEase(Ease.InBack).OnComplete(() => {
            _saveButtonCalendarClosed.gameObject.SetActive(false);
            
            _saveButtonCalendarOpened.gameObject.SetActive(true);
            _saveButtonCalendarOpened.transform.localScale = Vector3.zero;
            _saveButtonCalendarOpened.transform.DOScale(_initialSaveButtonScale, _animationDuration * 0.5f).SetEase(_animationEase);
            _saveButtonCalendarOpened.onClick.AddListener(OnSaveButtonClicked);
        });
        
        _dateButton.onClick.AddListener(CloseCalendar);
        
        // Animate calendar appearance
        _datePicker.gameObject.SetActive(true);
        _datePicker.transform.localScale = Vector3.zero;
        _datePicker.transform.DOScale(1f, _animationDuration).SetEase(_animationEase);
        _datePicker.Content.OnSelectionChanged.AddListener(SetDate);
    }

    public void CloseCalendar()
    {
        _saveButtonCalendarOpened.onClick.RemoveListener(OnSaveButtonClicked);
        
        // Animate button swap
        _saveButtonCalendarOpened.transform.DOScale(0f, _animationDuration * 0.5f).SetEase(Ease.InBack).OnComplete(() => {
            _saveButtonCalendarOpened.gameObject.SetActive(false);
            
            _saveButtonCalendarClosed.gameObject.SetActive(true);
            _saveButtonCalendarClosed.transform.localScale = Vector3.zero;
            _saveButtonCalendarClosed.transform.DOScale(_initialSaveButtonScale, _animationDuration * 0.5f).SetEase(_animationEase);
            _saveButtonCalendarClosed.onClick.AddListener(OnSaveButtonClicked);
        });
        
        // Animate button sprite change
        _dateButton.transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f), 0.3f, 1, 0.5f).OnComplete(() => {
            _dateButton.image.sprite = _calendarClosedSprite;
        });
        
        // Animate calendar disappearance
        _datePicker.transform.DOScale(0f, _animationDuration).SetEase(Ease.InBack).OnComplete(() => {
            _datePicker.gameObject.SetActive(false);
        });
        
        _dateButton.onClick.RemoveListener(CloseCalendar);
        _dateButton.onClick.AddListener(OpenCalendar);
    }
    
    private void SetNameText(string value)
    {
        // Animate text changes
        _nameInput.text = value;
        
        _upperTripNameText.DOFade(0f, _animationDuration * 0.3f).OnComplete(() => {
            _upperTripNameText.text = value;
            _upperTripNameText.DOFade(1f, _animationDuration * 0.3f);
        });
    }

    private void SetDescriptionText(string value)
    {
        _descriptionInput.text = value;
    }

    private void SetDateText(string value)
    {
        // Animate date text update if active
        if (gameObject.activeInHierarchy)
        {
            _dateText.DOFade(0f, _animationDuration * 0.3f).OnComplete(() => {
                _dateText.text = value;
                _dateText.DOFade(1f, _animationDuration * 0.3f);
            });
        }
        else
        {
            _dateText.text = value;
        }
    }
    
    private void OnDescriptionChanged(string description)
    {
        _newDescription = description;
        ValidateInputs();
    }

    private void OnNameChanged(string name)
    {
        _newName = name;
        _upperTripNameText.text = name;
        ValidateInputs();
    }

    private void OnDateChanged(string date)
    {
        _newDate = date;
        ValidateInputs();
    }

    private void OnSaveButtonClicked()
    {
        // Animate save button
        Button currentSaveButton = _saveButtonCalendarClosed.gameObject.activeInHierarchy ? 
            _saveButtonCalendarClosed : _saveButtonCalendarOpened;
            
        currentSaveButton.transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f), 0.3f, 1, 0.5f).OnComplete(() => {
            TripData tripData = new TripData(_newName, _newDescription, _newDate);
            SaveButtonClicked?.Invoke(tripData);
            PlayExitAnimation(() => OnBackButtonClicked());
        });
    }
    
    private void ValidateInputs()
    {
        bool allInputsValid = !string.IsNullOrEmpty(_newName) && !string.IsNullOrEmpty(_newDescription);
        SetSaveButtonInteractable(allInputsValid);
    }
    
    private void SetSaveButtonInteractable(bool isInteractable)
    {
        if(_saveButtonCalendarClosed.gameObject.activeInHierarchy)
        {
            _saveButtonCalendarClosed.interactable = isInteractable;
            
            // Subtle animation for button state change
            if (isInteractable)
            {
                _saveButtonCalendarClosed.transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), 0.2f, 1, 0.5f);
            }
        }
        
        if(_saveButtonCalendarOpened.gameObject.activeInHierarchy)
        {
            _saveButtonCalendarOpened.interactable = isInteractable;
            
            // Subtle animation for button state change
            if (isInteractable)
            {
                _saveButtonCalendarOpened.transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), 0.2f, 1, 0.5f);
            }
        }
    }
    
    private void OnBackButtonClicked()
    {
        // Animate back button
        _backButton.transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f), 0.3f, 1, 0.5f);
        BackButtonClicked?.Invoke();
        PlayExitAnimation(() => Disable());
    }
    
    private void Enable()
    {
        _screenVisabilityHandler.EnableScreen();
    }

    private void Disable()
    {
        _screenVisabilityHandler.DisableScreen();
    }
    
    private void PlayEnterAnimation()
    {
        // Kill previous animations if any
        _animationSequence?.Kill();
        
        // Create a new animation sequence
        _animationSequence = DOTween.Sequence();
        
        // First animate header elements
        _animationSequence.Append(_upperTripNameText.transform.DOScale(1f, _animationDuration).From(0.8f).SetEase(_animationEase));
        _animationSequence.Join(_upperTripNameText.DOFade(1f, _animationDuration).From(0f));
        _animationSequence.Join(_backButton.transform.DOScale(1f, _animationDuration).From(0f).SetEase(_animationEase));
        
        // Animate form elements with stagger
        if (_formContainer != null)
        {
            int childIndex = 0;
            foreach (Transform child in _formContainer)
            {
                float delay = childIndex * _staggerDelay;
                _animationSequence.Insert(delay, child.DOScale(1f, _animationDuration).SetEase(_animationEase));
                
                // If has CanvasGroup, fade in
                CanvasGroup canvasGroup = child.GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                {
                    _animationSequence.Insert(delay, canvasGroup.DOFade(1f, _animationDuration));
                }
                
                childIndex++;
            }
        }
        
        // Animate buttons
        _animationSequence.Append(_saveButtonCalendarClosed.transform.DOScale(_initialSaveButtonScale, _animationDuration).From(Vector3.zero).SetEase(_animationEase));
        _animationSequence.Join(_dateButton.transform.DOScale(_initialDateButtonScale, _animationDuration).From(Vector3.zero).SetEase(_animationEase));
        
        // Play the sequence
        _animationSequence.Play();
    }
    
    private void PlayExitAnimation(Action onComplete = null)
    {
        // Kill previous animations if any
        _animationSequence?.Kill();
        
        // Create a new animation sequence for exit
        _animationSequence = DOTween.Sequence();
        
        // Animate buttons first
        _animationSequence.Append(_saveButtonCalendarClosed.gameObject.activeInHierarchy ? 
            _saveButtonCalendarClosed.transform.DOScale(0f, _animationDuration * 0.5f).SetEase(Ease.InBack) : 
            _saveButtonCalendarOpened.transform.DOScale(0f, _animationDuration * 0.5f).SetEase(Ease.InBack));
        
        _animationSequence.Join(_dateButton.transform.DOScale(0f, _animationDuration * 0.5f).SetEase(Ease.InBack));
        
        // If calendar is open, close it with animation
        if (_datePicker.gameObject.activeInHierarchy)
        {
            _animationSequence.Join(_datePicker.transform.DOScale(0f, _animationDuration * 0.5f).SetEase(Ease.InBack));
        }
        
        // Animate form elements fading out
        if (_formContainer != null)
        {
            foreach (Transform child in _formContainer)
            {
                _animationSequence.Join(child.DOScale(0f, _animationDuration * 0.7f).SetEase(Ease.InBack));
                
                CanvasGroup canvasGroup = child.GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                {
                    _animationSequence.Join(canvasGroup.DOFade(0f, _animationDuration * 0.7f));
                }
            }
        }
        
        // Finally animate header elements
        _animationSequence.Append(_upperTripNameText.DOFade(0f, _animationDuration * 0.5f));
        _animationSequence.Join(_backButton.transform.DOScale(0f, _animationDuration * 0.5f).SetEase(Ease.InBack));
        
        // When all animations complete, call the callback
        _animationSequence.OnComplete(() => {
            onComplete?.Invoke();
            
            // Reset calendar state if it was open
            if (_datePicker.gameObject.activeInHierarchy)
            {
                _datePicker.gameObject.SetActive(false);
                _saveButtonCalendarOpened.gameObject.SetActive(false);
                _saveButtonCalendarClosed.gameObject.SetActive(true);
                _dateButton.image.sprite = _calendarClosedSprite;
            }
        });
        
        // Play the sequence
        _animationSequence.Play();
    }
}
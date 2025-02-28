using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; // Added DOTween namespace

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

    // Animation parameters
    [Header("Animation Settings")]
    [SerializeField] private float _animationDuration = 0.5f;
    [SerializeField] private float _staggerDelay = 0.1f;
    [SerializeField] private Ease _animationEase = Ease.OutBack;
    [SerializeField] private Transform _contentContainer; // Container for elements to animate
    
    private ScreenVisabilityHandler _screenVisabilityHandler;
    private Vector3 _initialAddButtonScale;
    private Sequence _animationSequence;

    public event Action EditButtonClicked;
    public event Action AddPlaceButtonClicked;
    public event Action BackButtonClicked; 
    
    private void Awake()
    {
        _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
        _initialAddButtonScale = _addPlaceButton.transform.localScale;
        
        // Initialize DOTween (if not initialized elsewhere)
        DOTween.SetTweensCapacity(500, 50);
    }

    private void Start()
    {
        // Set initial states for animations
        SetInitialAnimationStates();
        Disable();
    }

    private void SetInitialAnimationStates()
    {
        // Set initial animation states if needed
        if (_contentContainer != null)
        {
            foreach (Transform child in _contentContainer)
            {
                child.localScale = Vector3.zero;
                CanvasGroup canvasGroup = child.GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 0;
                }
            }
        }
        
        _emptyPlacesImage.transform.localScale = Vector3.zero;
        _addPlaceButton.transform.localScale = Vector3.zero;
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
        
        // Kill all animations when disabled
        _animationSequence?.Kill();
    }
    
    public void Enable()
    {
        _screenVisabilityHandler.EnableScreen();
        PlayEnterAnimation();
    }

    public void Disable()
    {
        // Play exit animation, then disable screen
        PlayExitAnimation(() => _screenVisabilityHandler.DisableScreen());
    }

    private void PlayEnterAnimation()
    {
        // Kill previous animations if any
        _animationSequence?.Kill();
        
        // Create a new animation sequence
        _animationSequence = DOTween.Sequence();
        
        // First animate header elements
        _animationSequence.Append(_tripNameText.transform.DOScale(1f, _animationDuration).From(0.8f).SetEase(_animationEase));
        _animationSequence.Join(_tripNameText.DOFade(1f, _animationDuration).From(0f));
        _animationSequence.Join(_dateText.DOFade(1f, _animationDuration).From(0f));
        
        // Then animate content elements with stagger
        if (_contentContainer != null)
        {
            foreach (Transform child in _contentContainer)
            {
                _animationSequence.Append(child.DOScale(1f, _animationDuration).SetEase(_animationEase));
                
                // If has CanvasGroup, fade in
                CanvasGroup canvasGroup = child.GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                {
                    _animationSequence.Join(canvasGroup.DOFade(1f, _animationDuration));
                }
                
                _animationSequence.AppendInterval(_staggerDelay);
            }
        }
        
        // Animate empty places image if visible
        if (_emptyPlacesImage.enabled)
        {
            _animationSequence.Append(_emptyPlacesImage.transform.DOScale(1f, _animationDuration).From(0f).SetEase(_animationEase));
            _animationSequence.Join(_emptyPlacesImage.DOFade(1f, _animationDuration).From(0f));
        }
        
        // Finally animate add place button with a bounce effect
        _animationSequence.Append(_addPlaceButton.transform.DOScale(_initialAddButtonScale, _animationDuration).From(Vector3.zero).SetEase(_animationEase));
        
        // Play the sequence
        _animationSequence.Play();
    }

    private void PlayExitAnimation(Action onComplete = null)
    {
        // Kill previous animations if any
        _animationSequence?.Kill();
        
        // Create a new animation sequence for exit
        _animationSequence = DOTween.Sequence();
        
        // Animate all elements fading out
        _animationSequence.Append(_tripNameText.DOFade(0f, _animationDuration * 0.7f));
        _animationSequence.Join(_tripNameSmallText.DOFade(0f, _animationDuration * 0.7f));
        _animationSequence.Join(_dateText.DOFade(0f, _animationDuration * 0.7f));
        _animationSequence.Join(_descriptionText.DOFade(0f, _animationDuration * 0.7f));
        _animationSequence.Join(_placesText.DOFade(0f, _animationDuration * 0.7f));
        _animationSequence.Join(_addPlaceButton.transform.DOScale(0f, _animationDuration * 0.7f).SetEase(Ease.InBack));
        
        if (_emptyPlacesImage.enabled)
        {
            _animationSequence.Join(_emptyPlacesImage.DOFade(0f, _animationDuration * 0.7f));
            _animationSequence.Join(_emptyPlacesImage.transform.DOScale(0f, _animationDuration * 0.7f).SetEase(Ease.InBack));
        }
        
        // When all animations complete, call the callback
        _animationSequence.OnComplete(() => onComplete?.Invoke());
        
        // Play the sequence
        _animationSequence.Play();
    }

    public void ToggleEmptyPlacesImage(bool status)
    {
        // If turning on, animate it
        if (status && !_emptyPlacesImage.enabled)
        {
            _emptyPlacesImage.enabled = true;
            _emptyPlacesImage.transform.localScale = Vector3.zero;
            _emptyPlacesImage.DOFade(0f, 0f);
            
            DOTween.Sequence()
                .Append(_emptyPlacesImage.transform.DOScale(1f, _animationDuration).SetEase(_animationEase))
                .Join(_emptyPlacesImage.DOFade(1f, _animationDuration));
        }
        // If turning off, animate out then disable
        else if (!status && _emptyPlacesImage.enabled)
        {
            DOTween.Sequence()
                .Append(_emptyPlacesImage.transform.DOScale(0f, _animationDuration).SetEase(Ease.InBack))
                .Join(_emptyPlacesImage.DOFade(0f, _animationDuration))
                .OnComplete(() => _emptyPlacesImage.enabled = false);
        }
        else
        {
            _emptyPlacesImage.enabled = status;
        }
    }

    public void SetDateText(string text)
    {
        // Animate text change
        _dateText.DOFade(0f, _animationDuration * 0.5f).OnComplete(() => {
            _dateText.text = text;
            _dateText.DOFade(1f, _animationDuration * 0.5f);
        });
    }

    public void SetTripNameText(string text)
    {
        // Animate both trip name texts
        _tripNameText.transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), _animationDuration, 1, 0.5f).OnComplete(() => {
            _tripNameText.text = text;
            _tripNameSmallText.text = text;
        });
    }

    public void SetPlacesText(string text)
    {
        // Animate counter
        _placesText.transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), _animationDuration, 1, 0.5f).OnComplete(() => {
            _placesText.text = text + PlacesText;
        });
    }

    public void SetDescriptionText(string text)
    {
        // Crossfade the description text
        _descriptionText.DOFade(0f, _animationDuration * 0.5f).OnComplete(() => {
            _descriptionText.text = text;
            _descriptionText.DOFade(1f, _animationDuration * 0.5f);
        });
    }

    // Animate button clicks
    private void OnBackButtonClicked()
    {
        _backButton.transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f), 0.3f, 1, 0.5f);
        BackButtonClicked?.Invoke();
    }

    private void OnEditButtonClicked()
    {
        _editButton.transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f), 0.3f, 1, 0.5f);
        EditButtonClicked?.Invoke();
    }

    private void OnAddPlaceButtonClicked()
    {
        _addPlaceButton.transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f), 0.3f, 1, 0.5f);
        AddPlaceButtonClicked?.Invoke();
    }

    // No need for extension methods as we're directly setting the alpha
}
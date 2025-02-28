using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(ScreenVisabilityHandler))]
public class OnboardingView : MonoBehaviour
{
    [SerializeField] private Button _iteractableButton;
    [SerializeField] private float _fadeInDuration = 0.5f;
    [SerializeField] private float _fadeOutDuration = 0.3f;
    [SerializeField] private float _buttonPunchScale = 0.2f;
    [SerializeField] private float _buttonPunchDuration = 0.3f;
    [SerializeField] private Ease _fadeInEase = Ease.OutQuad;
    [SerializeField] private Ease _fadeOutEase = Ease.InQuad;
    
    private ScreenVisabilityHandler _screenVisabilityHandler;
    private CanvasGroup _canvasGroup;
    private RectTransform _rectTransform;
    private Vector3 _originalButtonScale;

    public event Action InteractableButtonClicked;

    private void Awake()
    {
        _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
        _canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
        _rectTransform = GetComponent<RectTransform>();
        _originalButtonScale = _iteractableButton.transform.localScale;
    }

    private void Start()
    {
        _iteractableButton.onClick.AddListener(ProcessButtonClick);
        
        _iteractableButton.GetComponent<RectTransform>().localScale = _originalButtonScale;
        AddButtonHoverAnimation();
    }

    private void OnDisable()
    {
        _iteractableButton.onClick.RemoveListener(ProcessButtonClick);
        DOTween.Kill(_iteractableButton.transform);
    }

    public void DisableScreen()
    {
        DOTween.Kill(_canvasGroup);
        DOTween.Kill(_rectTransform);
        
        _screenVisabilityHandler.DisableScreen();
        
        _canvasGroup.alpha = 1f;
        
        _canvasGroup.DOFade(0f, _fadeOutDuration)
            .SetEase(_fadeOutEase);
        
        _rectTransform.DOScale(0.95f, _fadeOutDuration)
            .SetEase(_fadeOutEase);
    }

    public void EnableScreen()
    {
        _screenVisabilityHandler.EnableScreen();
        
        _canvasGroup.alpha = 0f;
        _rectTransform.localScale = new Vector3(0.95f, 0.95f, 0.95f);
        
        DOTween.Kill(_canvasGroup);
        DOTween.Kill(_rectTransform);
        
        _canvasGroup.DOFade(1f, _fadeInDuration)
            .SetEase(_fadeInEase);
        
        _rectTransform.DOScale(1f, _fadeInDuration)
            .SetEase(_fadeInEase);
    }

    private void ProcessButtonClick()
    {
        InteractableButtonClicked?.Invoke();
        
        _iteractableButton.transform.DOPunchScale(
            new Vector3(_buttonPunchScale, _buttonPunchScale, _buttonPunchScale), 
            _buttonPunchDuration, 
            10, 
            1)
            .SetEase(Ease.OutQuart);
    }
    
    private void AddButtonHoverAnimation()
    {
        var eventTrigger = _iteractableButton.gameObject.GetComponent<UnityEngine.EventSystems.EventTrigger>() 
            ?? _iteractableButton.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();
        
        var enterEntry = new UnityEngine.EventSystems.EventTrigger.Entry();
        enterEntry.eventID = UnityEngine.EventSystems.EventTriggerType.PointerEnter;
        enterEntry.callback.AddListener((data) => {
            DOTween.Kill(_iteractableButton.transform);
            _iteractableButton.transform.DOScale(_originalButtonScale * 1.1f, 0.2f).SetEase(Ease.OutQuad);
        });
        eventTrigger.triggers.Add(enterEntry);
        
        var exitEntry = new UnityEngine.EventSystems.EventTrigger.Entry();
        exitEntry.eventID = UnityEngine.EventSystems.EventTriggerType.PointerExit;
        exitEntry.callback.AddListener((data) => {
            DOTween.Kill(_iteractableButton.transform);
            _iteractableButton.transform.DOScale(_originalButtonScale, 0.2f).SetEase(Ease.OutQuad);
        });
        eventTrigger.triggers.Add(exitEntry);
    }
}
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(ScreenVisabilityHandler))]
public class MainScreenTravelsView : MonoBehaviour
{
    [SerializeField] private Image _emptyHistoryImage;
    [SerializeField] private Button _createTravelButton;
    [SerializeField] private Button _notesButton;
    [SerializeField] private Button _settingsButton;
    
    [Header("Animation Settings")]
    [SerializeField] private float _screenFadeDuration = 0.3f;
    [SerializeField] private float _buttonScaleDuration = 0.2f;
    [SerializeField] private float _emptyHistoryFadeDuration = 0.3f;

    private ScreenVisabilityHandler _screenVisabilityHandler;
    private Tweener _screenFadeTweener;
    private Tweener _emptyHistoryTweener;
    private CanvasGroup _canvasGroup;

    public event Action SettingsButtonClicked;
    public event Action CreateTravelClicked;
    public event Action NotesClicked;
    
    private void Awake()
    {
        _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
        
        _canvasGroup = GetComponent<CanvasGroup>();
        if (_canvasGroup == null)
        {
            _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        _canvasGroup.alpha = 0;
        _emptyHistoryImage.color = new Color(_emptyHistoryImage.color.r, _emptyHistoryImage.color.g, _emptyHistoryImage.color.b, 0);
    }

    private void OnEnable()
    {
        _createTravelButton.onClick.AddListener(ProcessCreateTravelClicked);
        _notesButton.onClick.AddListener(ProcessNotesClicked);
        _settingsButton.onClick.AddListener(ProcessSettingsButtonCLicked);
    }

    private void OnDisable()
    {
        _createTravelButton.onClick.RemoveListener(ProcessCreateTravelClicked);
        _notesButton.onClick.RemoveListener(ProcessNotesClicked);
        _settingsButton.onClick.RemoveListener(ProcessSettingsButtonCLicked);
    }

    public void Enable()
    {
        EnableWithAnimation(_screenFadeDuration, Ease.OutQuad);
    }

    public void EnableWithAnimation(float duration, Ease ease)
    {
        _screenFadeTweener?.Kill();

        _screenFadeTweener = _canvasGroup.DOFade(1f, duration)
            .SetEase(ease)
            .OnStart(() => {
                gameObject.SetActive(true);
                _screenVisabilityHandler.EnableScreen();
            });
    }

    public Tweener AnimateFadeOut(float duration, Ease ease)
    {
        _screenFadeTweener?.Kill();

        return _canvasGroup.DOFade(0f, duration)
            .SetEase(ease)
            .OnComplete(() => {
                _screenVisabilityHandler.DisableScreen();
                gameObject.SetActive(false);
            });
    }

    public Tweener AnimateFadeIn(float duration, Ease ease)
    {
        gameObject.SetActive(true);
        _screenVisabilityHandler.EnableScreen();

        _canvasGroup.alpha = 0;
        return _canvasGroup.DOFade(1f, duration)
            .SetEase(ease);
    }

    public void Disable()
    {
        AnimateFadeOut(_screenFadeDuration, Ease.OutQuad);
    }

    public void EnableEmptyHistoryWindowWithAnimation(float duration, Ease ease)
    {
        _emptyHistoryTweener?.Kill();

        _emptyHistoryTweener = _emptyHistoryImage.DOFade(1f, duration)
            .SetEase(ease);
        
        _emptyHistoryImage.enabled = true;
    }

    public void DisableEmptyHistoryWindowWithAnimation(float duration, Ease ease)
    {
        _emptyHistoryTweener?.Kill();

        _emptyHistoryTweener = _emptyHistoryImage.DOFade(0f, duration)
            .SetEase(ease)
            .OnComplete(() => {
                _emptyHistoryImage.enabled = false;
            });
    }

    private void ProcessSettingsButtonCLicked()
    {
        _settingsButton.transform.DOPunchScale(Vector3.one * 0.2f, _buttonScaleDuration);
        SettingsButtonClicked?.Invoke();
    }

    private void ProcessNotesClicked()
    {
        _notesButton.transform.DOPunchScale(Vector3.one * 0.2f, _buttonScaleDuration);
        NotesClicked?.Invoke();
    }

    private void ProcessCreateTravelClicked()
    {
        _createTravelButton.transform.DOPunchScale(Vector3.one * 0.2f, _buttonScaleDuration);
        CreateTravelClicked?.Invoke();
    }

    public void ToggleCreateTripButtonWithAnimation(bool status, float duration, Ease ease)
    {
        if (status)
        {
            _createTravelButton.interactable = true;
            _createTravelButton.transform.DOScale(1f, duration)
                .SetEase(ease);
        }
        else
        {
            _createTravelButton.transform.DOScale(0.7f, duration)
                .SetEase(ease)
                .OnComplete(() => {
                    _createTravelButton.interactable = false;
                });
        }
    }

    public void ToggleCreateTripButton(bool status)
    {
        _createTravelButton.interactable = status;
    }
}
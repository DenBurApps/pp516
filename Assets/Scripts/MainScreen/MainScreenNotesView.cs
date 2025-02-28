using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(ScreenVisabilityHandler))]
public class MainScreenNotesView : MonoBehaviour
{
    [SerializeField] private Image _emptyHistoryImage;
    [SerializeField] private Button _addNoteButton;
    [SerializeField] private Button _travelsButton;
    [SerializeField] private Button _settingsButton;
    
    [Header("Animation Settings")]
    [SerializeField] private float _screenFadeDuration = 0.3f;
    [SerializeField] private float _buttonScaleDuration = 0.2f;
    [SerializeField] private float _emptyHistoryFadeDuration = 0.3f;

    private ScreenVisabilityHandler _screenVisabilityHandler;
    private Tweener _screenFadeTweener;
    private Tweener _emptyHistoryTweener;
    private CanvasGroup _canvasGroup;

    // New field to track notes state
    private bool _hasNotes = false;

    public event Action SettingsButtonClicked;
    public event Action AddNoteClicked;
    public event Action TravelsClicked;
    
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
        _addNoteButton.onClick.AddListener(ProcessAddNoteClicked);
        _travelsButton.onClick.AddListener(ProcessTravelsClicked);
        _settingsButton.onClick.AddListener(ProcessSettingsButtonCLicked);
    }

    private void OnDisable()
    {
        _addNoteButton.onClick.RemoveListener(ProcessAddNoteClicked);
        _travelsButton.onClick.RemoveListener(ProcessTravelsClicked);
        _settingsButton.onClick.RemoveListener(ProcessSettingsButtonCLicked);
    }

    public void Enable()
    {
        _screenFadeTweener?.Kill();

        _screenFadeTweener = _canvasGroup.DOFade(1f, _screenFadeDuration)
            .OnStart(() => {
                gameObject.SetActive(true);
                _screenVisabilityHandler.EnableScreen();
                
                if (_hasNotes)
                {
                    DisableEmptyHistoryWindow();
                }
                else
                {
                    EnableEmptyHistoryWindow();
                }
            });
    }

    public void Disable()
    {
        _screenFadeTweener?.Kill();

        _screenFadeTweener = _canvasGroup.DOFade(0f, _screenFadeDuration)
            .OnComplete(() => {
                _screenVisabilityHandler.DisableScreen();
                gameObject.SetActive(false);
            });
    }
    
    public void EnableEmptyHistoryWindow()
    {
        _emptyHistoryTweener?.Kill();

        _emptyHistoryTweener = _emptyHistoryImage.DOFade(1f, _emptyHistoryFadeDuration)
            .SetEase(Ease.OutBounce);
        
        _emptyHistoryImage.enabled = true;
    }

    public void DisableEmptyHistoryWindow()
    {
        _emptyHistoryTweener?.Kill();

        _emptyHistoryTweener = _emptyHistoryImage.DOFade(0f, _emptyHistoryFadeDuration)
            .OnComplete(() => {
                _emptyHistoryImage.enabled = false;
            });
    }

    public void UpdateNotesState(bool hasNotes)
    {
        _hasNotes = hasNotes;

        if (hasNotes)
        {
            DisableEmptyHistoryWindow();
        }
        else
        {
            EnableEmptyHistoryWindow();
        }
    }

    private void ProcessSettingsButtonCLicked()
    {
        _settingsButton.transform.DOPunchScale(Vector3.one * 0.2f, _buttonScaleDuration);
        SettingsButtonClicked?.Invoke();
    }
    
    public void EnableEmptyHistoryWindowWithAnimation(float duration, Ease ease)
    {
        _emptyHistoryTweener?.Kill();

        _emptyHistoryTweener = _emptyHistoryImage.DOFade(1f, duration)
            .SetEase(ease);
        
        _emptyHistoryImage.enabled = true;
    }

    private void ProcessTravelsClicked()
    {
        _travelsButton.transform.DOPunchScale(Vector3.one * 0.2f, _buttonScaleDuration);
        TravelsClicked?.Invoke();
    }

    private void ProcessAddNoteClicked()
    {
        _addNoteButton.transform.DOPunchScale(Vector3.one * 0.2f, _buttonScaleDuration);
        AddNoteClicked?.Invoke();
    }
    
    public void ToggleAddNoteButton(bool status)
    {
        if (status)
        {
            _addNoteButton.interactable = true;
            _addNoteButton.transform.DOScale(1f, _buttonScaleDuration)
                .SetEase(Ease.OutBack);
        }
        else
        {
            _addNoteButton.transform.DOScale(0.7f, _buttonScaleDuration)
                .OnComplete(() => {
                    _addNoteButton.interactable = false;
                });
        }
    }
}
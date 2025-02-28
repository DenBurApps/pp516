using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScreenVisabilityHandler))]
[RequireComponent(typeof(CanvasGroup))]
public class VersionView : MonoBehaviour
{
    [SerializeField] private Button _backButton;
    [SerializeField] private TMP_Text _versionText;

    private string _version = "App version: ";
    private ScreenVisabilityHandler _screenVisabilityHandler;
    private CanvasGroup _canvasGroup;

    public event Action BackButtonClicked;
    
    private void Awake()
    {
        _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
        _canvasGroup = GetComponent<CanvasGroup>();

        ValidateComponents();
    }

    private void ValidateComponents()
    {
        if (_screenVisabilityHandler == null)
        {
            Debug.LogError("ScreenVisabilityHandler is missing on VersionView!");
        }

        if (_canvasGroup == null)
        {
            Debug.LogError("CanvasGroup is missing on VersionView!");
            _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        if (_backButton == null)
        {
            Debug.LogWarning("Back button is not assigned in VersionView!");
        }

        if (_versionText == null)
        {
            Debug.LogWarning("Version text is not assigned in VersionView!");
        }
    }

    private void Start()
    {
        Disable();
        UpdateVersionText();
    }

    private void UpdateVersionText()
    {
        if (_versionText != null)
        {
            _versionText.text = _version + Application.version;
        }
    }

    private void OnEnable()
    {
        SetupBackButton();
    }

    private void OnDisable()
    {
        if (_backButton != null)
        {
            _backButton.onClick.RemoveListener(ProcessBackButton);
        }
    }

    private void SetupBackButton()
    {
        if (_backButton != null)
        {
            _backButton.onClick.RemoveAllListeners();
            _backButton.onClick.AddListener(ProcessBackButton);
            
            _backButton.interactable = true;
        }
    }
    
    public Tweener AnimateFadeIn(float duration, Ease ease)
    {
        gameObject.SetActive(true);

        _canvasGroup.alpha = 0f;

        if (_backButton != null)
        {
            _backButton.interactable = true;
        }

        return _canvasGroup.DOFade(1f, duration)
            .SetEase(ease);
    }

    public Tweener AnimateFadeOut(float duration, Ease ease)
    {
        if (_backButton != null)
        {
            _backButton.interactable = false;
        }

        return _canvasGroup.DOFade(0f, duration)
            .SetEase(ease)
            .OnComplete(() => gameObject.SetActive(false));
    }
    
    public void Enable()
    {
        _screenVisabilityHandler.EnableScreen();

        if (_backButton != null)
        {
            _backButton.interactable = true;
        }
    }

    public void Disable()
    {
        _screenVisabilityHandler.DisableScreen();
    }

    private void ProcessBackButton()
    {
        BackButtonClicked?.Invoke();

        Disable();
    }
}
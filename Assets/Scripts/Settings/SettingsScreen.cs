using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
#if UNITY_IOS
using UnityEngine.iOS;
#endif

public class SettingsScreen : MonoBehaviour
{
    [SerializeField] private SettingsScreenView _view;
    [SerializeField] private VersionView _versionView;
    [SerializeField] private PrivacyPolicyView _privacyPolicyView;
    [SerializeField] private TermsOfUseView _termsOfUseView;
    [SerializeField] private ScreenStateManager _screenStateManager;
    
    [Header("Animation Settings")]
    [SerializeField] private float _fadeInDuration = 0.3f;
    [SerializeField] private float _fadeOutDuration = 0.2f;
    [SerializeField] private Ease _fadeInEase = Ease.OutQuad;
    [SerializeField] private Ease _fadeOutEase = Ease.InQuad;
    
    [Header("Contact Information")]
    [SerializeField] private string _contactEmail = "w63044073@gmail.com";
    
    private Sequence _currentScreenTransition;

    public event Action BackButtonClicked;

    private void Start()
    {
        DisableAllViews();
        SetupEventListeners();
    }

    private void DisableAllViews()
    {
        if (_view) _view.Disable();
        if (_versionView) _versionView.Disable();
        if (_privacyPolicyView) _privacyPolicyView.Disable();
        if (_termsOfUseView) _termsOfUseView.Disable();
    }

    private void SetupEventListeners()
    {
        if (_view == null)
        {
            Debug.LogError("SettingsScreenView is not assigned!");
            return;
        }

        RemoveEventListeners();

        _view.FeedbackButtonClicked += HandleFeedbackButtonClicked;
        _view.VersionButtonClicked += HandleVersionButtonClicked;
        _view.TermsOfUseButtonClicked += HandleTermsOfUseButtonClicked;
        _view.PrivacyPolicyButtonClicked += HandlePrivacyPolicyButtonClicked;
        _view.BackButtonClicked += HandleBackButtonClicked;
        _view.ContactUsButtonClicked += HandleContactUsButtonClicked;

        if (_privacyPolicyView)
            _privacyPolicyView.BackButtonClicked += ReturnToSettingsScreen;
        if (_versionView)
            _versionView.BackButtonClicked += ReturnToSettingsScreen;
        if (_termsOfUseView)
            _termsOfUseView.BackButtonClicked += ReturnToSettingsScreen;
        
        _screenStateManager.SettingScreenClicked += ShowSettingsScreen;
    }

    private void RemoveEventListeners()
    {
        if (_view == null) return;

        _view.FeedbackButtonClicked -= HandleFeedbackButtonClicked;
        _view.VersionButtonClicked -= HandleVersionButtonClicked;
        _view.TermsOfUseButtonClicked -= HandleTermsOfUseButtonClicked;
        _view.PrivacyPolicyButtonClicked -= HandlePrivacyPolicyButtonClicked;
        _view.BackButtonClicked -= HandleBackButtonClicked;
        _view.ContactUsButtonClicked -= HandleContactUsButtonClicked;

        if (_privacyPolicyView)
            _privacyPolicyView.BackButtonClicked -= ReturnToSettingsScreen;
        if (_versionView)
            _versionView.BackButtonClicked -= ReturnToSettingsScreen;
        if (_termsOfUseView)
            _termsOfUseView.BackButtonClicked -= ReturnToSettingsScreen;
        
        _screenStateManager.SettingScreenClicked -= ShowSettingsScreen;
    }

    private void OnEnable()
    {
        SetupEventListeners();
    }

    private void OnDisable()
    {
        RemoveEventListeners();
        DOTween.Kill(this);
    }

    private void HandleFeedbackButtonClicked()
    {
        #if UNITY_IOS
        Device.RequestStoreReview();
        #endif
    }

    private void HandleVersionButtonClicked()
    {
        if (_versionView == null)
        {
            Debug.LogError("Version View is not assigned!");
            return;
        }
        
        AnimateScreenTransition(_view.ScreenVisabilityHandler, 
                                _versionView.GetComponent<ScreenVisabilityHandler>());
    }

    private void HandleTermsOfUseButtonClicked()
    {
        if (_termsOfUseView == null)
        {
            Debug.LogError("Terms of Use View is not assigned!");
            return;
        }
        
        AnimateScreenTransition(_view.ScreenVisabilityHandler, 
                                _termsOfUseView.GetComponent<ScreenVisabilityHandler>());
    }

    private void HandlePrivacyPolicyButtonClicked()
    {
        if (_privacyPolicyView == null)
        {
            Debug.LogError("Privacy Policy View is not assigned!");
            return;
        }
        
        AnimateScreenTransition(_view.ScreenVisabilityHandler, 
                                _privacyPolicyView.GetComponent<ScreenVisabilityHandler>());
    }

    private void HandleBackButtonClicked()
    {
        BackButtonClicked?.Invoke();
        _view.Disable();
    }

    private void HandleContactUsButtonClicked()
    {
        Application.OpenURL($"mailto:{_contactEmail}?subject=App Feedback");
    }

    private void ShowSettingsScreen()
    {
        if (_view == null)
        {
            Debug.LogError("Cannot show Settings Screen - View is NULL!");
            return;
        }

        AnimateFadeIn(_view.ScreenVisabilityHandler);
        _view.Enable();
    }

    private void ReturnToSettingsScreen()
    {
        if (_view == null || _privacyPolicyView == null || _versionView == null || _termsOfUseView == null)
        {
            Debug.LogError("One of the views is not assigned!");
            return;
        }

        ScreenVisabilityHandler sourceView = null;

        if (_privacyPolicyView.gameObject.activeSelf)
        {
            sourceView = _privacyPolicyView.GetComponent<ScreenVisabilityHandler>();
        }
        else if (_versionView.gameObject.activeSelf)
        {
            sourceView = _versionView.GetComponent<ScreenVisabilityHandler>();
        }
        else if (_termsOfUseView.gameObject.activeSelf)
        {
            sourceView = _termsOfUseView.GetComponent<ScreenVisabilityHandler>();
        }

        if (sourceView != null)
        {
            AnimateScreenTransition(sourceView, _view.ScreenVisabilityHandler);
        }
    }

    private void AnimateScreenTransition(ScreenVisabilityHandler fadeOutScreen, ScreenVisabilityHandler fadeInScreen)
    {
        if (_currentScreenTransition != null && _currentScreenTransition.IsActive())
        {
            _currentScreenTransition.Kill();
        }

        _currentScreenTransition = DOTween.Sequence().SetId(this);

        _currentScreenTransition.Append(fadeOutScreen.GetComponent<CanvasGroup>()
            .DOFade(0f, _fadeOutDuration)
            .SetEase(_fadeOutEase)
            .OnComplete(() => fadeOutScreen.DisableScreen()));

        _currentScreenTransition.AppendCallback(() => 
        {
            fadeInScreen.EnableScreen();
        
            if (fadeInScreen.gameObject == _versionView?.gameObject)
                _versionView.Enable();
            else if (fadeInScreen.gameObject == _privacyPolicyView?.gameObject)
                _privacyPolicyView.Enable();
            else if (fadeInScreen.gameObject == _termsOfUseView?.gameObject)
                _termsOfUseView.Enable();
            
            fadeInScreen.GetComponent<CanvasGroup>().alpha = 0f;
        });

        _currentScreenTransition.Append(fadeInScreen.GetComponent<CanvasGroup>()
            .DOFade(1f, _fadeInDuration)
            .SetEase(_fadeInEase));

        _currentScreenTransition.Play();
    }

    private void AnimateFadeIn(ScreenVisabilityHandler screen)
    {
        if (_currentScreenTransition != null && _currentScreenTransition.IsActive())
        {
            _currentScreenTransition.Kill();
        }

        _currentScreenTransition = DOTween.Sequence().SetId(this);

        screen.EnableScreen();
        CanvasGroup canvasGroup = screen.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;

        _currentScreenTransition.Append(canvasGroup.DOFade(1f, _fadeInDuration).SetEase(_fadeInEase));
        _currentScreenTransition.Play();
    }
}
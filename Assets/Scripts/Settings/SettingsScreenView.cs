using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScreenVisabilityHandler))]
public class SettingsScreenView : MonoBehaviour
{
    [SerializeField] private Button _feedbackButton;
    [SerializeField] private Button _privacyPolicyButton;
    [SerializeField] private Button _termsOfUseButton;
    [SerializeField] private Button _versionButton;
    [SerializeField] private Button _backButton;
    [SerializeField] private Button _contactUsButton;
    
    private ScreenVisabilityHandler _screenVisabilityHandler;

    public event Action FeedbackButtonClicked;
    public event Action PrivacyPolicyButtonClicked;
    public event Action TermsOfUseButtonClicked;
    public event Action VersionButtonClicked;
    public event Action BackButtonClicked;
    public event Action ContactUsButtonClicked;
    
    private void OnEnable()
    {
        _feedbackButton.onClick.AddListener(OnProcessFeedbackButtonClicked);
        _privacyPolicyButton.onClick.AddListener(OnProcessPolicyButtonClicked);
        _termsOfUseButton.onClick.AddListener(OnTermsOfUseButtonClicked);
        _versionButton.onClick.AddListener(OnVersionButtonClicked);
        _backButton.onClick.AddListener(OnBackButtonClicked);
        _contactUsButton.onClick.AddListener(OnContactUsClicked);
    }

    private void OnDisable()
    {
        _feedbackButton.onClick.RemoveListener(OnProcessFeedbackButtonClicked);
        _privacyPolicyButton.onClick.RemoveListener(OnProcessPolicyButtonClicked);
        _termsOfUseButton.onClick.RemoveListener(OnTermsOfUseButtonClicked);
        _versionButton.onClick.RemoveListener(OnVersionButtonClicked);
        _backButton.onClick.RemoveListener(OnBackButtonClicked);
    }

    private void OnContactUsClicked()
    {
        ContactUsButtonClicked?.Invoke();
    }

    private void OnBackButtonClicked()
    {
        BackButtonClicked?.Invoke();
    }

    private void Awake()
    {
        _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
    }
    
    public void Enable()
    {
        _screenVisabilityHandler.EnableScreen();
    }

    public void Disable()
    {
        _screenVisabilityHandler.DisableScreen();
    }

    private void OnVersionButtonClicked()
    {
        VersionButtonClicked?.Invoke();
    }

    private void OnTermsOfUseButtonClicked()
    {
        TermsOfUseButtonClicked?.Invoke();
    }

    private void OnProcessPolicyButtonClicked()
    {
        PrivacyPolicyButtonClicked?.Invoke();
    }

    private void OnProcessFeedbackButtonClicked()
    {
        FeedbackButtonClicked?.Invoke();
    }
}

using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SettingsScreenView : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button _feedbackButton;
    [SerializeField] private Button _versionButton;
    [SerializeField] private Button _termsOfUseButton;
    [SerializeField] private Button _privacyPolicyButton;
    [SerializeField] private Button _backButton;
    [SerializeField] private Button _contactUsButton;

    public ScreenVisabilityHandler ScreenVisabilityHandler;

    private CanvasGroup _canvasGroup;

    public Button FeedbackButton => _feedbackButton;
    public Button VersionButton => _versionButton;
    public Button TermsOfUseButton => _termsOfUseButton;
    public Button PrivacyPolicyButton => _privacyPolicyButton;
    public Button BackButton => _backButton;
    public Button ContactUsButton => _contactUsButton;

    public event System.Action FeedbackButtonClicked;
    public event System.Action VersionButtonClicked;
    public event System.Action TermsOfUseButtonClicked;
    public event System.Action PrivacyPolicyButtonClicked;
    public event System.Action BackButtonClicked;
    public event System.Action ContactUsButtonClicked;

    private void Awake()
    {
        InitializeCanvasGroup();
        SetupButtonListeners();
    }

    private void InitializeCanvasGroup()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        if (_canvasGroup == null)
        {
            _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    private void SetupButtonListeners()
    {
        AddButtonListener(_feedbackButton, OnFeedbackButtonClicked);
        AddButtonListener(_versionButton, OnVersionButtonClicked);
        AddButtonListener(_termsOfUseButton, OnTermsOfUseButtonClicked);
        AddButtonListener(_privacyPolicyButton, OnPrivacyPolicyButtonClicked);
        AddButtonListener(_backButton, OnBackButtonClicked);
        AddButtonListener(_contactUsButton, OnContactUsButtonClicked);
    }

    private void AddButtonListener(Button button, UnityEngine.Events.UnityAction action)
    {
        if (button != null)
        {
            button.onClick.AddListener(action);
        }
        else
        {
            Debug.LogWarning($"Button is null: {action.Method.Name}");
        }
    }

    public void Enable()
    {
        ScreenVisabilityHandler.EnableScreen();
        SetAllButtonsInteractable(true);
    }

    public void Disable()
    {
        ScreenVisabilityHandler.DisableScreen();
    }

    private void SetAllButtonsInteractable(bool interactable)
    {
        SetButtonInteractable(_feedbackButton, interactable);
        SetButtonInteractable(_versionButton, interactable);
        SetButtonInteractable(_termsOfUseButton, interactable);
        SetButtonInteractable(_privacyPolicyButton, interactable);
        SetButtonInteractable(_backButton, interactable);
        SetButtonInteractable(_contactUsButton, interactable);
    }

    private void SetButtonInteractable(Button button, bool interactable)
    {
        if (button != null)
        {
            button.interactable = interactable;
        }
    }

    public Tweener AnimateFadeIn(float duration, Ease ease)
    {
        gameObject.SetActive(true);
        _canvasGroup.alpha = 0f;
        SetAllButtonsInteractable(true);
        return _canvasGroup.DOFade(1f, duration).SetEase(ease);
    }

    public Tweener AnimateFadeOut(float duration, Ease ease)
    {
        SetAllButtonsInteractable(false);
        return _canvasGroup.DOFade(0f, duration)
            .SetEase(ease)
            .OnComplete(() => gameObject.SetActive(false));
    }

    private void OnFeedbackButtonClicked()
    {
        FeedbackButtonClicked?.Invoke();
    }

    private void OnVersionButtonClicked()
    {
        VersionButtonClicked?.Invoke();
    }

    private void OnTermsOfUseButtonClicked()
    {
        TermsOfUseButtonClicked?.Invoke();
    }

    private void OnPrivacyPolicyButtonClicked()
    {
        PrivacyPolicyButtonClicked?.Invoke();
    }

    private void OnBackButtonClicked()
    {
        BackButtonClicked?.Invoke();
    }

    private void OnContactUsButtonClicked()
    {
        ContactUsButtonClicked?.Invoke();
    }
}
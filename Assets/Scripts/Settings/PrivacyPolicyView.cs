using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScreenVisabilityHandler))]
public class PrivacyPolicyView : MonoBehaviour
{
    [SerializeField] private Button _backButton;

    private ScreenVisabilityHandler _screenVisabilityHandler;

    public event Action BackButtonClicked;
    
    private void Awake()
    {
        _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
    }

    private void Start()
    {
        Disable();
    }

    private void OnEnable()
    {
        _backButton.onClick.AddListener(ProcessBackButton);
    }
    
    public Tweener AnimateFadeIn(float duration, Ease ease)
    {
        gameObject.SetActive(true);
    
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
    
        return canvasGroup.DOFade(1f, duration).SetEase(ease);
    }

    public Tweener AnimateFadeOut(float duration, Ease ease)
    {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
    
        return canvasGroup.DOFade(0f, duration).SetEase(ease);
    }
    
    public void Enable()
    {
        _screenVisabilityHandler.EnableScreen();
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

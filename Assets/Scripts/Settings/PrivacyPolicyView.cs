using System;
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

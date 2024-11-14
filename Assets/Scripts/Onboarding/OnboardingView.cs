using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScreenVisabilityHandler))]
public class OnboardingView : MonoBehaviour
{
    [SerializeField] private Button _iteractableButton;

    private ScreenVisabilityHandler _screenVisabilityHandler;

    public event Action InteractableButtonClicked;

    private void Awake()
    {
        _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
    }

    private void Start()
    {
        _iteractableButton.onClick.AddListener(ProcessButtonClick);
    }

    private void OnDisable()
    {
        _iteractableButton.onClick.RemoveListener(ProcessButtonClick);
    }

    public void DisableScreen()
    {
        _screenVisabilityHandler.DisableScreen();
    }

    public void EnableScreen()
    {
        _screenVisabilityHandler.EnableScreen();
    }

    private void ProcessButtonClick()
    {
        InteractableButtonClicked?.Invoke();
    }
}
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScreenVisabilityHandler))]
public class VersionView : MonoBehaviour
{
    [SerializeField] private Button _backButton;
    [SerializeField] private TMP_Text _versionText;

    private string _version = "App version: ";
    private ScreenVisabilityHandler _screenVisabilityHandler;

    public event Action BackButtonClicked;
    
    private void Awake()
    {
        _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
    }

    private void Start()
    {
        Disable();
        _versionText.text = _version + Application.version;
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

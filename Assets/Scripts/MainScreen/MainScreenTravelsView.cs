using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScreenVisabilityHandler))]
public class MainScreenTravelsView : MonoBehaviour
{
    [SerializeField] private Image _emptyHistoryImage;
    [SerializeField] private Button _createTravelButton;
    [SerializeField] private Button _notesButton;
    [SerializeField] private Button _settingsButton;
    
    private ScreenVisabilityHandler _screenVisabilityHandler;

    public event Action SettingsButtonClicked;
    public event Action CreateTravelClicked;
    public event Action NotesClicked;
    
    private void Awake()
    {
        _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
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
        _screenVisabilityHandler.EnableScreen();
    }

    public void Disable()
    {
        _screenVisabilityHandler.DisableScreen();
    }

    private void ProcessSettingsButtonCLicked()
    {
        SettingsButtonClicked?.Invoke();
    }

    private void ProcessNotesClicked()
    {
        NotesClicked?.Invoke();
    }

    private void ProcessCreateTravelClicked()
    {
        CreateTravelClicked?.Invoke();
    }

    public void EnableEmptyHistoryWindow()
    {
        _emptyHistoryImage.enabled = true;
    }

    public void DisableEmptyHistoryWindow()
    {
        _emptyHistoryImage.enabled = false;
    }

    public void ToggleCreateTripButton(bool status)
    {
        _createTravelButton.interactable = status;
    }
}

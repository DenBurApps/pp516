using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScreenVisabilityHandler))]
public class MainScreenNotesView : MonoBehaviour
{
    [SerializeField] private Image _emptyHistoryImage;
    [SerializeField] private Button _addNoteButton;
    [SerializeField] private Button _travelsButton;
    [SerializeField] private Button _settingsButton;
    
    private ScreenVisabilityHandler _screenVisabilityHandler;

    public event Action SettingsButtonClicked;
    public event Action AddNoteClicked;
    public event Action TravelsClicked;
    
    private void Awake()
    {
        _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
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
        _screenVisabilityHandler.EnableScreen();
    }

    public void Disable()
    {
        _screenVisabilityHandler.DisableScreen();
    }
    
    public void EnableEmptyHistoryWindow()
    {
        _emptyHistoryImage.enabled = true;
    }

    public void DisableEmptyHistoryWindow()
    {
        _emptyHistoryImage.enabled = false;
    }

    private void ProcessSettingsButtonCLicked()
    {
        SettingsButtonClicked?.Invoke();
    }

    private void ProcessTravelsClicked()
    {
        TravelsClicked?.Invoke();
    }

    private void ProcessAddNoteClicked()
    {
        AddNoteClicked?.Invoke();
    }
    
    public void ToggleAddNoteButton(bool status)
    {
        _addNoteButton.interactable = status;
    }
}

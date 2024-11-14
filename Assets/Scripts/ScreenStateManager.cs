using System;
using UnityEngine;

public class ScreenStateManager : MonoBehaviour
{
    [SerializeField] private MainScreenPresenter _mainScreen;
    [SerializeField] private CreateTravelScreen _createTravelScreen;
    [SerializeField] private CreateNote _createNoteScreen;
    [SerializeField] private OpenNotesScreen _openNotesScreen;
    [SerializeField] private EditNoteScreen _editNoteScreen;
    [SerializeField] private OpenTravel _openTravelScreen;
    [SerializeField] private EditTravel _editTravelScreen;
    [SerializeField] private AddPlaceScreen _addPlaceScreen;
    [SerializeField] private SettingsScreen _settingsScreen;

    public event Action MainScreenTravelsOpen;
    public event Action MainScreenNotesOpen;
    public event Action CreateTravelOpen;
    public event Action CreateNoteOpen;
    public event Action OpenNoteOpen;
    public event Action OpenTravelClicked;
    public event Action SettingScreenClicked;
    
    private void OnEnable()
    {
        _mainScreen.CreateTravelClicked += OnCreateTravelOpen;
        _mainScreen.AddNoteClicked += OnCreateNoteOpen;
        _mainScreen.SettingsClicked += OnSettingsOpen;
        _createTravelScreen.BackButtonClicked += OnMainScreenTravelsOpen;
        _createNoteScreen.BackButtonClicked += OnMainScreenNotesOpen;
        _openNotesScreen.BackButtonClicked += OnMainScreenNotesOpen;
        _editNoteScreen.BackButtonClicked += OnOpenNotesOpen;
        _openTravelScreen.BackButtonClicked += OnMainScreenTravelsOpen;
        _editTravelScreen.BackButtonClicked += OnOpenTravelOpen;
        _addPlaceScreen.BackButtonClicked += OnOpenTravelOpen;
        _settingsScreen.BackButtonClicked += OnMainScreenTravelsOpen;
    }

    private void OnDisable()
    {
        _mainScreen.CreateTravelClicked -= OnCreateTravelOpen;
        _mainScreen.AddNoteClicked -= OnCreateNoteOpen;
        _mainScreen.SettingsClicked -= OnSettingsOpen;
        _createTravelScreen.BackButtonClicked -= OnMainScreenTravelsOpen;
        _createNoteScreen.BackButtonClicked -= OnMainScreenNotesOpen;
        _openNotesScreen.BackButtonClicked -= OnMainScreenNotesOpen;
        _editNoteScreen.BackButtonClicked -= OnOpenNotesOpen;
        _openTravelScreen.BackButtonClicked -= OnMainScreenTravelsOpen;
        _editTravelScreen.BackButtonClicked -= OnOpenTravelOpen;
        _addPlaceScreen.BackButtonClicked -= OnOpenTravelOpen;
        _settingsScreen.BackButtonClicked -= OnMainScreenTravelsOpen;
    }

    private void OnMainScreenTravelsOpen() => MainScreenTravelsOpen?.Invoke();
    private void OnMainScreenNotesOpen() => MainScreenNotesOpen?.Invoke();
    private void OnCreateTravelOpen() => CreateTravelOpen?.Invoke();
    private void OnCreateNoteOpen() => CreateNoteOpen?.Invoke();
    private void OnOpenNotesOpen() => OpenNoteOpen?.Invoke();
    private void OnOpenTravelOpen() => OpenTravelClicked?.Invoke();
    private void OnSettingsOpen() => SettingScreenClicked?.Invoke();
}

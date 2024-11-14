using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateNote : MonoBehaviour
{
    private const string DefaultTextInputValue = "";
    
    [SerializeField] private CreateNoteView _view;
    [SerializeField] private ScreenStateManager _screenStateManager;

    private string _note;
    private string _date;
    
    public event Action BackButtonClicked;
    public event Action<NoteData> SaveButtonClicked;
    
    private void Start()
    {
        ReturnDefaultTripDataValues();
        _view.Disable();
    }

    private void OnEnable()
    {
        _screenStateManager.CreateNoteOpen += EnableScreen;
        _view.NoteChanged += OnNoteChanged;
        _view.DateChanged += OnDateChanged;
        _view.BackButtonClicked += OnBackButtonClicked;
        _view.SaveButtonClicked += OnSaveButtonClicked;
    }

    private void OnDisable()
    {
        _screenStateManager.CreateNoteOpen -= EnableScreen;
        _view.NoteChanged -= OnNoteChanged;
        _view.DateChanged -= OnDateChanged;
        _view.BackButtonClicked -= OnBackButtonClicked;
        _view.SaveButtonClicked -= OnSaveButtonClicked;
    }

    private void EnableScreen()
    {
        _view.Enable();
    }

    private void OnNoteChanged(string note)
    {
        _note = note;
        ValidateInputs();
    }

    private void OnDateChanged(string date)
    {
        _date = date;
        ValidateInputs();
    }
    
    private void ValidateInputs()
    {
        bool allInputsValid = !string.IsNullOrEmpty(_note) && !string.IsNullOrEmpty(_date);

        _view.SetSaveButtonInteractable(allInputsValid);
    }
    
    private void OnBackButtonClicked()
    {
        BackButtonClicked?.Invoke();
        ReturnDefaultTripDataValues();
        _view.Disable();
    }

    private void OnSaveButtonClicked()
    {
        NoteData noteData = new NoteData(_note, _date);
        SaveButtonClicked?.Invoke(noteData);
        OnBackButtonClicked();
    }

    private void ReturnDefaultTripDataValues()
    {
        _note = null;
        _date = null;
        
        _view.SetCurrentDate();
        _view.CloseCalendar();
        _view.SetNoteText(DefaultTextInputValue);
    }
}

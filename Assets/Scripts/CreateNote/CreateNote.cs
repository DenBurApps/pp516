using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CreateNote : MonoBehaviour
{
    private const string DefaultTextInputValue = "";
    
    [SerializeField] private CreateNoteView _view;
    [SerializeField] private ScreenStateManager _screenStateManager;
    
    // DOTween animation configuration
    [Header("Animation Settings")]
    [SerializeField] private float _fadeInDuration = 0.5f;
    [SerializeField] private float _fadeOutDuration = 0.3f;
    [SerializeField] private float _buttonScaleDuration = 0.2f;
    [SerializeField] private Ease _fadeEase = Ease.OutQuad;
    [SerializeField] private Ease _buttonEase = Ease.OutBack;

    private string _note;
    private string _date;
    
    // Add a flag to prevent multiple saves
    private bool _isSaving = false;
    
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
        
        // Unsubscribe first to prevent multiple subscriptions
        _view.SaveButtonClicked -= OnSaveButtonClicked;
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
        // Reset saving flag when screen is enabled
        _isSaving = false;
        
        _view.Enable();
        // Animate screen appear with DOTween
        _view.AnimateOpen(_fadeInDuration, _fadeEase);
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
    
    private bool _previousValidationState = false;
    
    private void ValidateInputs()
    {
        bool allInputsValid = !string.IsNullOrEmpty(_note) && !string.IsNullOrEmpty(_date);

        // Only animate when transitioning from invalid to valid state
        if (allInputsValid && !_previousValidationState)
        {
            _view.AnimateSaveButton(_buttonScaleDuration, _buttonEase);
        }
        
        _view.SetSaveButtonInteractable(allInputsValid && !_isSaving);
        _previousValidationState = allInputsValid;
    }
    
    private void OnBackButtonClicked()
    {
        // Prevent back action during saving
        if (_isSaving) return;

        // Animate screen close before disabling
        CloseScreen();
    }

    private void CloseScreen()
    {
        _view.AnimateClose(_fadeOutDuration, _fadeEase, () => {
            // Ensure back button event is invoked
            BackButtonClicked?.Invoke();
            
            // Reset data and disable view
            ReturnDefaultTripDataValues();
            _view.Disable();
        });
    }

    private void OnSaveButtonClicked()
    {
        // Prevent multiple save attempts
        if (_isSaving) return;

        // Mark as currently saving
        _isSaving = true;
        
        // Disable save button immediately
        _view.SetSaveButtonInteractable(false);

        // Animate save button press
        _view.AnimateSaveButtonPress(_buttonScaleDuration, _buttonEase, () => {
            // Validate inputs again before saving
            if (string.IsNullOrEmpty(_note) || string.IsNullOrEmpty(_date))
            {
                // If inputs are invalid, reset saving state
                _isSaving = false;
                ValidateInputs();
                return;
            }

            // Create note data
            NoteData noteData = new NoteData(_note, _date);
            
            // Invoke save event
            SaveButtonClicked?.Invoke(noteData);
            
            // Close the screen
            CloseScreen();
        });
    }

    private void ReturnDefaultTripDataValues()
    {
        _note = null;
        _date = null;
        _isSaving = false;
        
        _view.SetCurrentDate();
        _view.CloseCalendar();
        _view.SetNoteText(DefaultTextInputValue);
    }
}
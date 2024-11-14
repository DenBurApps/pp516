using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateTravelScreen : MonoBehaviour
{
    private const string DefaultTextInputValue = "";
    
    [SerializeField] private CreateTravelScreenView _view;
    [SerializeField] private ScreenStateManager _screenStateManager;

    private string _name;
    private string _description;
    private string _date;

    public event Action BackButtonClicked;
    public event Action<TripData> SaveButtonClicked;

    private void Start()
    {
        _view.Disable();
    }

    private void OnEnable()
    {
        _screenStateManager.CreateTravelOpen += EnableScreen;
        _view.NameChanged += OnNameChanged;
        _view.DescriptionChanged += OnDescriptionChanged;
        _view.DateChanged += OnDateChanged;
        _view.SaveButtonClicked += OnSaveButtonClicked;
        _view.BackButtonClicked += OnBackButtonClicked;
    }

    private void OnDisable()
    {
        _screenStateManager.CreateTravelOpen -= EnableScreen;
        _view.NameChanged -= OnNameChanged;
        _view.DescriptionChanged -= OnDescriptionChanged;
        _view.DateChanged -= OnDateChanged;
        _view.SaveButtonClicked -= OnSaveButtonClicked;
        _view.BackButtonClicked -= OnBackButtonClicked;
    }

    private void EnableScreen()
    {
        _view.Enable();
    }

    private void OnDescriptionChanged(string description)
    {
        _description = description;
        ValidateInputs();
    }

    private void OnNameChanged(string name)
    {
        _name = name;
        ValidateInputs();
    }

    private void OnDateChanged(string date)
    {
        _date = date;
        ValidateInputs();
    }

    private void OnSaveButtonClicked()
    {
        TripData tripData = new TripData(_name, _description, _date);
        SaveButtonClicked?.Invoke(tripData);
        OnBackButtonClicked();
    }
    
    private void ReturnDefaultTripDataValues()
    {
        _view.SetNameValue(DefaultTextInputValue);
        _view.SetDescriptionValue(DefaultTextInputValue);
        _view.SetCurrentDate();
        _view.CloseCalendar();
        _name = null;
        _description = null;
    }
    
    private void ValidateInputs()
    {
        bool allInputsValid = !string.IsNullOrEmpty(_name) && !string.IsNullOrEmpty(_description) && !string.IsNullOrEmpty(_date) ;

        _view.SetSaveButtonInteractable(allInputsValid);
    }

    private void OnBackButtonClicked()
    {
        BackButtonClicked?.Invoke();
        ReturnDefaultTripDataValues();
        _view.Disable();
    } 
}

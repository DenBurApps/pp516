using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class MainScreenPresenter : MonoBehaviour
{
    private const string TripsSaveFileName = "SavedTripsData.json";

    [SerializeField] private MainScreenTravelsView _travelsView;
    [SerializeField] private MainScreenNotesView _notesView;
    [SerializeField] private ScreenStateManager _screenStateManager;
    [SerializeField] private List<FilledTripDataWindow> _filledTripDataWindows;
    [SerializeField] private CreateTravelScreen _travelCreator;

    [Header("Animations")] [SerializeField]
    private float _fadeInDuration = 0.3f;

    [SerializeField] private float _fadeOutDuration = 0.2f;
    [SerializeField] private float _slideDistance = 50f;
    [SerializeField] private float _slideDuration = 0.4f;
    [SerializeField] private Ease _fadeInEase = Ease.OutQuad;
    [SerializeField] private Ease _fadeOutEase = Ease.InQuad;
    [SerializeField] private Ease _slideEase = Ease.OutBack;

    private Sequence _currentScreenTransition;

    public event Action CreateTravelClicked;
    public event Action AddNoteClicked;
    public event Action SettingsClicked;

    public event Action<FilledTripDataWindow> OnOpenTripDataClicked;
    public string SaveFilePath => Path.Combine(Application.persistentDataPath, TripsSaveFileName);

    private void Awake()
    {
        DOTween.SetTweensCapacity(500, 50);
    }

    private void Start()
    {
        _travelsView.EnableWithAnimation(_fadeInDuration, _fadeInEase);
        _notesView.GetComponent<ScreenVisabilityHandler>().DisableScreen();
        _notesView.gameObject.SetActive(false);

        DisableAllFilledWindows();
        LoadFilledWindowsData();
    }

    private void OnEnable()
    {
        _travelsView.NotesClicked += ProcessNotesClicked;
        _travelsView.CreateTravelClicked += OnCreateTravellClicked;
        _travelsView.SettingsButtonClicked += OnSettingClicked;

        _notesView.TravelsClicked += ProcessTravelsClicked;
        _notesView.AddNoteClicked += OnAddNoteClicked;
        _notesView.SettingsButtonClicked += OnSettingClicked;

        _screenStateManager.MainScreenTravelsOpen += EnableTravelsViewWithAnimation;
        _screenStateManager.MainScreenNotesOpen += EnableNotesViewWithAnimation;

        _travelCreator.SaveButtonClicked += FillTravelsData;
    }

    private void OnDisable()
    {
        _travelsView.NotesClicked -= ProcessNotesClicked;
        _travelsView.CreateTravelClicked -= OnCreateTravellClicked;
        _travelsView.SettingsButtonClicked -= OnSettingClicked;

        _notesView.TravelsClicked -= ProcessTravelsClicked;
        _notesView.AddNoteClicked -= OnAddNoteClicked;
        _notesView.SettingsButtonClicked -= OnSettingClicked;

        _screenStateManager.MainScreenTravelsOpen -= EnableTravelsViewWithAnimation;
        _screenStateManager.MainScreenNotesOpen -= EnableNotesViewWithAnimation;

        _travelCreator.SaveButtonClicked -= FillTravelsData;

        DOTween.Kill(this);
    }

    private void EnableTravelsViewWithAnimation()
    {
        _travelsView.EnableWithAnimation(_fadeInDuration, _fadeInEase);
    }

    private void EnableNotesViewWithAnimation()
    {
        _notesView.EnableWithAnimation(_fadeInDuration, _fadeInEase);
    }

    private void FillTravelsData(TripData tripData)
    {
        if (tripData == null)
            throw new ArgumentNullException(nameof(tripData));

        if (string.IsNullOrEmpty(tripData.Name) || string.IsNullOrEmpty(tripData.Date) ||
            string.IsNullOrEmpty(tripData.Description))
            return;

        // Find the first inactive window
        var currentFilledWindow = _filledTripDataWindows.FirstOrDefault(window => !window.IsActive);

        if (currentFilledWindow != null)
        {
            currentFilledWindow.EnableWithAnimation(_slideDuration, _slideEase, _slideDistance);
            currentFilledWindow.DeleteButtonClicked += ProcessFilledTravelsDataDeletion;
            currentFilledWindow.EditButtonClicked += OnOpenTripData;
            currentFilledWindow.DataChanged += SaveFilledWindowsData;
            currentFilledWindow.SetBasicTripData(tripData);

            // Check if we need to disable empty history or create trip button
            int activeWindowsCount = _filledTripDataWindows.Count(window => window.IsActive);
            if (activeWindowsCount > 0)
            {
                _travelsView.DisableEmptyHistoryWindowWithAnimation(_fadeOutDuration, _fadeOutEase);
            }

            if (activeWindowsCount >= _filledTripDataWindows.Count)
            {
                _travelsView.ToggleCreateTripButtonWithAnimation(false, _fadeOutDuration, _fadeOutEase);
            }
        }

        SaveFilledWindowsData();
    }

    private void ProcessFilledTravelsDataDeletion(FilledTripDataWindow window)
    {
        if (window == null)
            throw new ArgumentNullException(nameof(window));

        window.DeleteButtonClicked -= ProcessFilledTravelsDataDeletion;
        window.EditButtonClicked -= OnOpenTripData;
        window.DataChanged -= SaveFilledWindowsData;

        window.DisableWithAnimation(_fadeOutDuration, _fadeOutEase);

        _travelsView.ToggleCreateTripButtonWithAnimation(true, _fadeInDuration, _fadeInEase);

        int activeWindowsCount = _filledTripDataWindows.Count(w => w.IsActive);
        if (activeWindowsCount == 0)
        {
            _travelsView.EnableEmptyHistoryWindowWithAnimation(_fadeInDuration, _fadeInEase);
        }

        SaveFilledWindowsData();
    }

    private void SaveFilledWindowsData()
    {
        List<TripData> tripsToSave = _filledTripDataWindows
            .Where(window => window.IsActive)
            .Select(window => window.TripData)
            .ToList();

        TripDataList tripDataList = new TripDataList(tripsToSave);
        string json = JsonUtility.ToJson(tripDataList, true);

        try
        {
            File.WriteAllText(SaveFilePath, json);

            if (tripsToSave.Count > 0)
            {
                _travelsView.DisableEmptyHistoryWindowWithAnimation(_fadeOutDuration, _fadeOutEase);
            }
            else
            {
                _travelsView.EnableEmptyHistoryWindowWithAnimation(_fadeInDuration, _fadeInEase);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to save trip data: " + e.Message);
        }
    }
    
    private void UpdateNotesState()
    {
        // Implement this method based on how you track notes
        // For example, if you have a list of notes or a save file for notes
        bool hasNotes = CheckIfNotesExist();
        _notesView.UpdateNotesState(hasNotes);
    }
    
    private bool CheckIfNotesExist()
    {
        // Example implementation - replace with actual notes checking logic
        string notesSavePath = Path.Combine(Application.persistentDataPath, "NotesData.json");
        return File.Exists(notesSavePath) && new FileInfo(notesSavePath).Length > 0;
    }

      private void LoadFilledWindowsData()
    {
        if (File.Exists(SaveFilePath))
        {
            try
            {
                string json = File.ReadAllText(SaveFilePath);
                TripDataList loadedTripDataList = JsonUtility.FromJson<TripDataList>(json);

                Sequence loadSequence = DOTween.Sequence();

                int windowIndex = 0;
                foreach (TripData trip in loadedTripDataList.Trips)
                {
                    if (windowIndex < _filledTripDataWindows.Count)
                    {
                        var window = _filledTripDataWindows[windowIndex];
                        window.SetBasicTripData(trip);
                        window.LoadPlacesData();
                        window.DeleteButtonClicked += ProcessFilledTravelsDataDeletion;
                        window.EditButtonClicked += OnOpenTripData;
                        window.DataChanged += SaveFilledWindowsData;

                        int capturedIndex = windowIndex;
                        loadSequence.AppendCallback(() =>
                        {
                            window.EnableWithAnimation(_slideDuration, _slideEase, _slideDistance);
                        });
                        loadSequence.AppendInterval(0.1f);

                        windowIndex++;
                    }
                }

                loadSequence.OnComplete(() =>
                {
                    if (windowIndex > 0)
                    {
                        _travelsView.DisableEmptyHistoryWindowWithAnimation(_fadeOutDuration, _fadeOutEase);
                    }
                    else
                    {
                        _travelsView.EnableEmptyHistoryWindowWithAnimation(_fadeInDuration, _fadeInEase);
                    }
                });
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to load trip data: " + e.Message);
            }
        }
        else
        {
            _travelsView.EnableEmptyHistoryWindowWithAnimation(_fadeInDuration, _fadeInEase);
        }
    }

    private void DisableAllFilledWindows()
    {
        foreach (var window in _filledTripDataWindows)
        {
            window.Disable();
        }

        _travelsView.EnableEmptyHistoryWindowWithAnimation(_fadeInDuration, _fadeInEase);
    }

    private void ProcessNotesClicked()
    {
        TransitionBetweenScreens(_travelsView, _notesView);
    }

    private void ProcessTravelsClicked()
    {
        TransitionBetweenScreens(_notesView, _travelsView);
    }

    private void TransitionBetweenScreens(MonoBehaviour fadeOutScreen, MonoBehaviour fadeInScreen)
    {
        if (_currentScreenTransition != null && _currentScreenTransition.IsActive())
        {
            _currentScreenTransition.Kill();
        }

        _currentScreenTransition = DOTween.Sequence();

        if (fadeOutScreen is MainScreenTravelsView travelsView)
        {
            _currentScreenTransition.Append(travelsView.AnimateFadeOut(_fadeOutDuration, _fadeOutEase));
        }
        else if (fadeOutScreen is MainScreenNotesView notesView)
        {
            _currentScreenTransition.Append(notesView.AnimateFadeOut(_fadeOutDuration, _fadeOutEase));
        }

        if (fadeInScreen is MainScreenTravelsView inTravelsView)
        {
            _currentScreenTransition.Append(inTravelsView.AnimateFadeIn(_fadeInDuration, _fadeInEase));
        }
        else if (fadeInScreen is MainScreenNotesView inNotesView)
        {
            _currentScreenTransition.Append(inNotesView.AnimateFadeIn(_fadeInDuration, _fadeInEase));
        }

        _currentScreenTransition.Play();
    }

    private void OnCreateTravellClicked()
    {
        if (_travelsView.gameObject.activeSelf)
        {
            _travelsView.AnimateFadeOut(_fadeOutDuration, _fadeOutEase)
                .OnComplete(() => CreateTravelClicked?.Invoke());
        }
        else
        {
            CreateTravelClicked?.Invoke();
        }
    }

    private void OnAddNoteClicked()
    {
        if (_notesView.gameObject.activeSelf)
        {
            _notesView.AnimateFadeOut(_fadeOutDuration, _fadeOutEase)
                .OnComplete(() => AddNoteClicked?.Invoke());
        }
        else
        {
            AddNoteClicked?.Invoke();
        }
    }

    private void OnSettingClicked()
    {
        Sequence settingsTransition = DOTween.Sequence();

        if (_travelsView.gameObject.activeSelf)
        {
            settingsTransition.Append(_travelsView.AnimateFadeOut(_fadeOutDuration, _fadeOutEase));
        }
        else if (_notesView.gameObject.activeSelf)
        {
            settingsTransition.Append(_notesView.AnimateFadeOut(_fadeOutDuration, _fadeOutEase));
        }

        settingsTransition.OnComplete(() => SettingsClicked?.Invoke());
        settingsTransition.Play();
    }

    private void OnOpenTripData(FilledTripDataWindow window)
    {
        Sequence openTripTransition = DOTween.Sequence();

        if (_travelsView.gameObject.activeSelf)
        {
            openTripTransition.Append(_travelsView.AnimateFadeOut(_fadeOutDuration, _fadeOutEase));
        }

        openTripTransition.OnComplete(() => OnOpenTripDataClicked?.Invoke(window));
        openTripTransition.Play();
    }
}

[Serializable]
public class TripDataList
{
    public List<TripData> Trips;

    public TripDataList(List<TripData> trips)
    {
        Trips = trips;
    }
}
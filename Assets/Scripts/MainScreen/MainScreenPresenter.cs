using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MainScreenPresenter : MonoBehaviour
{
   private const string TripsSaveFileName = "SavedTripsData.json";
   
   [SerializeField] private MainScreenTravelsView _travelsView;
   [SerializeField] private MainScreenNotesView _notesView;
   [SerializeField] private ScreenStateManager _screenStateManager;
   [SerializeField] private List<FilledTripDataWindow> _filledTripDataWindows;
   [SerializeField] private CreateTravelScreen _travelCreator;

   private List<int> _availableWindowIndexes = new List<int>();

   public event Action CreateTravelClicked;
   public event Action AddNoteClicked;
   public event Action SettingsClicked;

   public event Action<FilledTripDataWindow> OnOpenTripDataClicked; 
   public string SaveFilePath => Path.Combine(Application.persistentDataPath, TripsSaveFileName);
   
   private void Start()
   {
      _travelsView.Enable();
      _notesView.Disable();
      
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

      _screenStateManager.MainScreenTravelsOpen += _travelsView.Enable;
      _screenStateManager.MainScreenNotesOpen += _notesView.Enable;

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
      
      _screenStateManager.MainScreenTravelsOpen -= _travelsView.Enable;
      _screenStateManager.MainScreenNotesOpen -= _notesView.Enable;
      
      _travelCreator.SaveButtonClicked -= FillTravelsData;
   }

   private void FillTravelsData(TripData tripData)
   {
      if (tripData == null)
         throw new ArgumentNullException(nameof(tripData));
      
      if(string.IsNullOrEmpty(tripData.Name) || string.IsNullOrEmpty(tripData.Date) || string.IsNullOrEmpty(tripData.Description))
         return;

      if (_availableWindowIndexes.Count > 0)
      {
         int availableIndex = _availableWindowIndexes[0];
         _availableWindowIndexes.RemoveAt(0);

         var currentFilleWindow = _filledTripDataWindows[availableIndex];

         if (!currentFilleWindow.IsActive)
         {
            currentFilleWindow.Enable();
            currentFilleWindow.DeleteButtonClicked += ProcessFilledTravelsDataDeletion;
            currentFilleWindow.EditButtonClicked += OnOpenTripData;
            currentFilleWindow.DataChanged += SaveFilledWindowsData;
            currentFilleWindow.SetBasicTripData(tripData);
         }
         
         if (_availableWindowIndexes.Count < _filledTripDataWindows.Count)
         {
            _travelsView.DisableEmptyHistoryWindow();
         }

         if (_availableWindowIndexes.Count == 0)
         {
            _travelsView.ToggleCreateTripButton(false);
         }
      }

      SaveFilledWindowsData();
   }

   private void ProcessFilledTravelsDataDeletion(FilledTripDataWindow window)
   {
      if (window == null)
         throw new ArgumentNullException(nameof(window));

      int windowIndex = _filledTripDataWindows.IndexOf(window);
      
      if (windowIndex >= 0 && !_availableWindowIndexes.Contains(windowIndex))
      {
         _availableWindowIndexes.Add(windowIndex);
      }
      
      window.DeleteButtonClicked -= ProcessFilledTravelsDataDeletion;
      window.EditButtonClicked -= OnOpenTripData;
      window.DataChanged -= SaveFilledWindowsData;
      window.Disable();
      
      _travelsView.ToggleCreateTripButton(true);
      
      if (_availableWindowIndexes.Count == _filledTripDataWindows.Count)
      {
         _travelsView.EnableEmptyHistoryWindow();
      }

      SaveFilledWindowsData();
   }
   
   private void SaveFilledWindowsData()
   {
      List<TripData> tripsToSave = new List<TripData>();

      foreach (var window in _filledTripDataWindows)
      {
         if (window.IsActive)
         {
            tripsToSave.Add(window.TripData);
         }
      }

      TripDataList tripDataList = new TripDataList(tripsToSave);
      string json = JsonUtility.ToJson(tripDataList, true);

      try
      {
         File.WriteAllText(SaveFilePath, json);
      }
      catch (Exception e)
      {
         Debug.LogError("Failed to save trip data: " + e.Message);
      }
   }
   
   private void LoadFilledWindowsData()
   {
      if (File.Exists(SaveFilePath))
      {
         try
         {
            string json = File.ReadAllText(SaveFilePath);
            TripDataList loadedTripDataList = JsonUtility.FromJson<TripDataList>(json);

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
                  window.Enable();
                  _availableWindowIndexes.Remove(windowIndex);
                  windowIndex++;
               }
            }
            
            _travelsView.DisableEmptyHistoryWindow();
            
            if (_availableWindowIndexes.Count >= _filledTripDataWindows.Count)
            {
               _travelsView.EnableEmptyHistoryWindow();
            }
         }
         catch (Exception e)
         {
            Debug.LogError("Failed to load trip data: " + e.Message);
         }
      }
   }

   private void DisableAllFilledWindows()
   {
      for (int i = 0; i < _filledTripDataWindows.Count; i++)
      {
         _filledTripDataWindows[i].Disable();
         _availableWindowIndexes.Add(i);
      }
      
      _travelsView.EnableEmptyHistoryWindow();
   }

   private void ProcessNotesClicked()
   {
      _travelsView.Disable();
      _notesView.Enable();
   }

   private void ProcessTravelsClicked()
   {
      _travelsView.Enable();
      _notesView.Disable();
   }

   private void OnCreateTravellClicked()
   {
      CreateTravelClicked?.Invoke();
      _travelsView.Disable();
      _notesView.Disable();
   }

   private void OnAddNoteClicked()
   {
      AddNoteClicked?.Invoke();
      _travelsView.Disable();
      _notesView.Disable();
   }

   private void OnSettingClicked()
   {
      SettingsClicked?.Invoke();
      _travelsView.Disable();
      _notesView.Disable();
   }

   private void OnOpenTripData(FilledTripDataWindow window)
   {
      _travelsView.Disable();
      _notesView.Disable();
      OnOpenTripDataClicked?.Invoke(window);
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
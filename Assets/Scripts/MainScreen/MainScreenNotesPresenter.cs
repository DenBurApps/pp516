using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MainScreenNotesPresenter : MonoBehaviour
{
    [SerializeField] private CreateNote _createNoteScreen;
    [SerializeField] private MainScreenNotesView _notesView;
    [SerializeField] private List<FilledNoteInfo> _filledNoteDataWindows;

    public event Action<FilledNoteInfo> NoteInfoClicked;

    private void Start()
    {
        DisableAllFilledWindows();
        LoadNotes();
    }

    private void OnEnable()
    {
        _createNoteScreen.SaveButtonClicked += FillNoteData;
    }

    private void OnDisable()
    {
        _createNoteScreen.SaveButtonClicked -= FillNoteData;
    }

    private void FillNoteData(NoteData data)
    {
        if (data == null)
            throw new ArgumentNullException(nameof(data));
      
        if(string.IsNullOrEmpty(data.Note) || string.IsNullOrEmpty(data.Date))
            return;
        
        // Find the first inactive window
        var currentFilleWindow = _filledNoteDataWindows.FirstOrDefault(window => !window.IsActive);
        
        if (currentFilleWindow != null)
        {
            currentFilleWindow.Enable();
            currentFilleWindow.DeleteButtonClicked += ProcessFilledTravelsDataDeletion;
            currentFilleWindow.OpenNoteInfoClicked += ProcessOpenNoteClicked;
            currentFilleWindow.DataChanged += SaveNotes;
            currentFilleWindow.SetNoteData(data);
         
            // Check if we have at least one active window
            if (_filledNoteDataWindows.Count(window => window.IsActive) > 0)
            {
                _notesView.DisableEmptyHistoryWindow();
                _notesView.UpdateNotesState(true);
            }

            // Check if all windows are now filled
            if (_filledNoteDataWindows.Count(window => !window.IsActive) == 0)
            {
                _notesView.ToggleAddNoteButton(false);
            }

            SaveNotes();
        }
    }
    
    private void ProcessFilledTravelsDataDeletion(FilledNoteInfo window)
    {
        if (window == null)
            throw new ArgumentNullException(nameof(window));

        window.DeleteButtonClicked -= ProcessFilledTravelsDataDeletion;
        window.OpenNoteInfoClicked -= ProcessOpenNoteClicked;
        window.DataChanged -= SaveNotes;
        window.Disable();
        _notesView.ToggleAddNoteButton(true);
      
        // Check if all windows are now inactive
        if (_filledNoteDataWindows.Count(w => w.IsActive) == 0)
        {
            _notesView.EnableEmptyHistoryWindow();
            _notesView.UpdateNotesState(false);
        }
        
        SaveNotes();
    }
    
    private void DisableAllFilledWindows()
    {
        foreach (var window in _filledNoteDataWindows)
        {
            window.Disable();
        }
      
        _notesView.EnableEmptyHistoryWindow();
        _notesView.UpdateNotesState(false);
    }
    
    private void SaveNotes()
    {
        // Select only active windows' data
        List<NoteData> noteDataList = _filledNoteDataWindows
            .Where(window => window.IsActive)
            .Select(window => window.NoteData)
            .ToList();

        string json = JsonUtility.ToJson(new NoteDataListWrapper(noteDataList), true);
        string path = System.IO.Path.Combine(Application.persistentDataPath, "notes.json");
        System.IO.File.WriteAllText(path, json);
    }
    
    private void LoadNotes()
    {
        string path = System.IO.Path.Combine(Application.persistentDataPath, "notes.json");
    
        if (System.IO.File.Exists(path))
        {
            string json = System.IO.File.ReadAllText(path);
            NoteDataListWrapper noteDataListWrapper = JsonUtility.FromJson<NoteDataListWrapper>(json);

            if (noteDataListWrapper?.NoteDataList != null)
            {
                DisableAllFilledWindows();
                
                // Check if there are any loaded notes
                bool hasLoadedNotes = noteDataListWrapper.NoteDataList.Count > 0;
                
                for (int i = 0; i < noteDataListWrapper.NoteDataList.Count; i++)
                {
                    if (i < _filledNoteDataWindows.Count)
                    {
                        var window = _filledNoteDataWindows[i];
                        window.Enable();
                        window.SetNoteData(noteDataListWrapper.NoteDataList[i]);
                        window.DeleteButtonClicked += ProcessFilledTravelsDataDeletion;
                        window.OpenNoteInfoClicked += ProcessOpenNoteClicked;
                        window.DataChanged += SaveNotes;
                    }
                }
                
                if (hasLoadedNotes)
                {
                    _notesView.DisableEmptyHistoryWindow();
                    _notesView.UpdateNotesState(true);
                }
                
                // Check if all windows are inactive
                if (_filledNoteDataWindows.Count(w => w.IsActive) == 0)
                {
                    _notesView.EnableEmptyHistoryWindow();
                    _notesView.UpdateNotesState(false);
                }
            }
        }
        else
        {
            // No notes file exists
            _notesView.EnableEmptyHistoryWindow();
            _notesView.UpdateNotesState(false);
        }
    }

    private void ProcessOpenNoteClicked(FilledNoteInfo filledNoteInfo) => NoteInfoClicked?.Invoke(filledNoteInfo);
}

[Serializable]
public class NoteDataListWrapper
{
    public List<NoteData> NoteDataList;

    public NoteDataListWrapper(List<NoteData> noteDataList)
    {
        NoteDataList = noteDataList;
    }
}
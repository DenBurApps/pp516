using System;
using System.Collections.Generic;
using UnityEngine;

public class MainScreenNotesPresenter : MonoBehaviour
{
    [SerializeField] private CreateNote _createNoteScreen;
    [SerializeField] private MainScreenNotesView _notesView;
    [SerializeField] private List<FilledNoteInfo> _filledNoteDataWindows;
    
    private List<int> _availableWindowIndexes = new List<int>();

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
        
        if (_availableWindowIndexes.Count > 0)
        {
            int availableIndex = _availableWindowIndexes[0];
            _availableWindowIndexes.RemoveAt(0);

            var currentFilleWindow = _filledNoteDataWindows[availableIndex];

            if (!currentFilleWindow.IsActive)
            {
                currentFilleWindow.Enable();
                currentFilleWindow.DeleteButtonClicked += ProcessFilledTravelsDataDeletion;
                currentFilleWindow.OpenNoteInfoClicked += ProcessOpenNoteClicked;
                currentFilleWindow.DataChanged += SaveNotes;
                currentFilleWindow.SetNoteData(data);
            }
         
            if (_availableWindowIndexes.Count < _filledNoteDataWindows.Count)
            {
                _notesView.DisableEmptyHistoryWindow();
            }

            if (_availableWindowIndexes.Count == 0)
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

        int windowIndex = _filledNoteDataWindows.IndexOf(window);
      
        if (windowIndex >= 0 && !_availableWindowIndexes.Contains(windowIndex))
        {
            _availableWindowIndexes.Add(windowIndex);
        }
      
        window.DeleteButtonClicked -= ProcessFilledTravelsDataDeletion;
        window.OpenNoteInfoClicked -= ProcessOpenNoteClicked;
        window.DataChanged -= SaveNotes;
        window.Disable();
        _notesView.ToggleAddNoteButton(true);
      
        if (_availableWindowIndexes.Count >= _filledNoteDataWindows.Count)
        {
            _notesView.EnableEmptyHistoryWindow();
        }
        
        SaveNotes();
    }
    
    private void DisableAllFilledWindows()
    {
        for (int i = 0; i < _filledNoteDataWindows.Count; i++)
        {
            _filledNoteDataWindows[i].Disable();
            _availableWindowIndexes.Add(i);
        }
      
        _notesView.EnableEmptyHistoryWindow();
    }
    
    private void SaveNotes()
    {
        List<NoteData> noteDataList = new List<NoteData>();

        foreach (var window in _filledNoteDataWindows)
        {
            if (window.IsActive)
            {
                noteDataList.Add(window.NoteData);
            }
        }

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

            if (noteDataListWrapper != null && noteDataListWrapper.NoteDataList != null)
            {
                DisableAllFilledWindows();
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
                        _availableWindowIndexes.Remove(i);
                    }
                }
                
                _notesView.DisableEmptyHistoryWindow();
            }
            
            if (_availableWindowIndexes.Count >= _filledNoteDataWindows.Count)
            {
                _notesView.EnableEmptyHistoryWindow();
            }
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

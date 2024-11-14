using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FilledNoteInfo : MonoBehaviour
{
    private const string DefaultValue = "";
    
    [SerializeField] private TMP_Text _noteText;
    [SerializeField] private TMP_Text _dateText;
    [SerializeField] private Button _deleteButton;
    [SerializeField] private Button _openNoteButton;
    
    private string _note;
    private string _date;

    public string Note => _note;

    public string Date => _date;

    private bool _isActive;

    public event Action<FilledNoteInfo> DeleteButtonClicked;
    public event Action<FilledNoteInfo> OpenNoteInfoClicked;
    public event Action DataChanged;
    
    public NoteData NoteData { get; private set; }
    
    public bool IsActive => _isActive;
    
    private void OnEnable()
    {
        _deleteButton.onClick.AddListener(ReturnToDefault);
        _openNoteButton.onClick.AddListener(ProcessOpenNoteClicked);
    }

    private void OnDisable()
    {
        _deleteButton.onClick.RemoveListener(ReturnToDefault);
        _openNoteButton.onClick.RemoveListener(ProcessOpenNoteClicked);
    }
    
    public void ReturnToDefault()
    {
        DeleteButtonClicked?.Invoke(this);
        
        _note = null;
        _date = null;
        NoteData = null;

        _noteText.text = DefaultValue;
        _dateText.text = DefaultValue;
        _isActive = false;
    }

    public void SetNoteData(NoteData noteData)
    {
        if (noteData == null)
            throw new ArgumentNullException(nameof(noteData));

        NoteData = noteData;
        SetNote(noteData.Note);
        SetDate(noteData.Date);
        DataChanged?.Invoke();
    }
    
    public void Enable()
    {
        gameObject.SetActive(true);
        _isActive = true;
    }

    public void Disable()
    {
        gameObject.SetActive(false);
        _isActive = false;
    }

    public void SetNote(string note)
    {
        _note = note;
        _noteText.text = _note;
    }

    public void SetDate(string date)
    {
        _date = date;
        _dateText.text = _date;
    }

    private void ProcessOpenNoteClicked() => OpenNoteInfoClicked?.Invoke(this);

}

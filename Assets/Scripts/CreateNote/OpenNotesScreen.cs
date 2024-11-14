using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScreenVisabilityHandler))]
public class OpenNotesScreen : MonoBehaviour
{
    [SerializeField] private Button _backButton;
    [SerializeField] private Button _editButton;
    [SerializeField] private TMP_Text _noteText;
    [SerializeField] private TMP_Text _dateText;
    [SerializeField] private MainScreenNotesPresenter _screenNotesPresenter;
    [SerializeField] private ScreenStateManager _screenStateManager;
    [SerializeField] private EditNoteScreen _editNoteScreen;
    
    private FilledNoteInfo _filledNoteInfo;
    
    private ScreenVisabilityHandler _screenVisabilityHandler;

    public event Action<FilledNoteInfo> EditButtonClicked;
    public event Action BackButtonClicked;
    
    private void Awake()
    {
        _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
    }

    private void Start()
    {
        Disable();
    }

    private void OnEnable()
    {
        _screenNotesPresenter.NoteInfoClicked += ProcessScreenOpen;
        _backButton.onClick.AddListener(ProcessBackButtonClicked);
        _screenStateManager.OpenNoteOpen += Enable;
        _editNoteScreen.SaveButtonClicked += EditNoteData;
    }

    private void OnDisable()
    {
        _screenNotesPresenter.NoteInfoClicked -= ProcessScreenOpen;
        _editButton.onClick.RemoveListener(ProcessEditClicked);
        _backButton.onClick.RemoveListener(ProcessBackButtonClicked);
        _screenStateManager.OpenNoteOpen -= Enable;
        _editNoteScreen.SaveButtonClicked -= EditNoteData;
    }

    public void Enable()
    {
        _screenVisabilityHandler.EnableScreen();
    }

    public void Disable()
    {
        _screenVisabilityHandler.DisableScreen();
    }

    private void ProcessScreenOpen(FilledNoteInfo filledNoteInfo)
    {
        if (filledNoteInfo == null)
            throw new ArgumentNullException(nameof(filledNoteInfo));

        _filledNoteInfo = filledNoteInfo;
        _noteText.text = _filledNoteInfo.Note;
        _dateText.text = _filledNoteInfo.Date;
        
        Enable();
        _editButton.onClick.AddListener(ProcessEditClicked);
    }

    private void EditNoteData(NoteData noteData)
    {
        if (noteData == null)
            throw new ArgumentNullException(nameof(noteData));
        
        _filledNoteInfo.SetNoteData(noteData);
        _noteText.text = noteData.Note;
        _dateText.text = noteData.Date;
    }

    private void ProcessBackButtonClicked()
    {
        BackButtonClicked?.Invoke();
        Disable();
    }

    private void ProcessEditClicked()
    {
        EditButtonClicked?.Invoke(_filledNoteInfo);
        Disable();
    }
}

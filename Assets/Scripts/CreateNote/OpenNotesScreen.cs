using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; // Added DoTween import

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
    
    [Header("Animation Settings")]
    [SerializeField] private float _animationDuration = 0.5f;
    [SerializeField] private float _elementDelay = 0.1f;
    [SerializeField] private Ease _easeType = Ease.OutBack;
    [SerializeField] private RectTransform _contentContainer; // Container for elements to animate
    
    private FilledNoteInfo _filledNoteInfo;
    private ScreenVisabilityHandler _screenVisabilityHandler;
    private Sequence _showSequence;
    private Sequence _hideSequence;

    public event Action<FilledNoteInfo> EditButtonClicked;
    public event Action BackButtonClicked;
    
    private void Awake()
    {
        _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
        InitializeAnimations();
    }

    private void Start()
    {
        _screenVisabilityHandler.DisableScreen();
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
        
        // Kill any active tweens if the component is disabled
        if (_showSequence != null)
            _showSequence.Kill();
        if (_hideSequence != null)
            _hideSequence.Kill();
    }

    private void InitializeAnimations()
    {
        // Create show sequence
        _showSequence = DOTween.Sequence();
        _showSequence.SetAutoKill(false);
        _showSequence.Pause();
        
        // Create hide sequence
        _hideSequence = DOTween.Sequence();
        _hideSequence.SetAutoKill(false);
        _hideSequence.Pause();
        
        if (_contentContainer != null)
        {
            // Set up initial state for all child elements
            foreach (RectTransform child in _contentContainer)
            {
                if (child != null)
                {
                    // Save original position
                    Vector2 originalPosition = child.anchoredPosition;
                    
                    // For show animation - start off-screen
                    child.anchoredPosition = new Vector2(Screen.width, originalPosition.y);
                    
                    // Add to show sequence
                    Tween showTween = child.DOAnchorPos(originalPosition, _animationDuration).SetEase(_easeType);
                    _showSequence.Append(showTween);
                    
                    // Add to hide sequence
                    Tween hideTween = child.DOAnchorPos(new Vector2(-Screen.width, originalPosition.y), _animationDuration).SetEase(_easeType);
                    _hideSequence.Append(hideTween);
                }
            }
            
            // Add fade animations for text elements
            if (_noteText != null)
            {
                _noteText.alpha = 0;
                _showSequence.Append(_noteText.DOFade(1, _animationDuration).SetEase(Ease.InQuad));
                _hideSequence.Join(_noteText.DOFade(0, _animationDuration).SetEase(Ease.OutQuad));
            }
            
            if (_dateText != null)
            {
                _dateText.alpha = 0;
                _showSequence.Join(_dateText.DOFade(1, _animationDuration).SetEase(Ease.InQuad));
                _hideSequence.Join(_dateText.DOFade(0, _animationDuration).SetEase(Ease.OutQuad));
            }
            
            // Add button animations
            if (_backButton != null)
            {
                _backButton.transform.localScale = Vector3.zero;
                _showSequence.Join(_backButton.transform.DOScale(1, _animationDuration).SetEase(Ease.OutBounce));
                _hideSequence.Join(_backButton.transform.DOScale(0, _animationDuration).SetEase(Ease.InBack));
            }
            
            if (_editButton != null)
            {
                _editButton.transform.localScale = Vector3.zero;
                _showSequence.Join(_editButton.transform.DOScale(1, _animationDuration).SetEase(Ease.OutBounce));
                _hideSequence.Join(_editButton.transform.DOScale(0, _animationDuration).SetEase(Ease.InBack));
            }
            
            // Set callback on hide sequence completion
            _hideSequence.OnComplete(OnHideComplete);
        }
    }

    private void OnHideComplete()
    {
        _screenVisabilityHandler.DisableScreen();
    }

    public void Enable()
    {
        _screenVisabilityHandler.EnableScreen();
        
        // Stop any hide sequence and play show sequence
        if (_hideSequence != null)
            _hideSequence.Pause();
        if (_showSequence != null)
            _showSequence.Restart();
    }

    public void Disable()
    {
        if (_showSequence != null)
            _showSequence.Pause();
        if (_hideSequence != null)
            _hideSequence.Restart();
        
        _screenVisabilityHandler.DisableScreen();
    }

    private void ProcessScreenOpen(FilledNoteInfo filledNoteInfo)
    {
        if (filledNoteInfo == null)
            throw new ArgumentNullException(nameof(filledNoteInfo));

        _filledNoteInfo = filledNoteInfo;
        _noteText.text = _filledNoteInfo.Note;
        _dateText.text = _filledNoteInfo.Date;
        
        // Add a small punch animation when content is loaded
        if (_noteText != null)
            _noteText.transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), 0.3f, 2, 0.5f);
        
        Enable();
        _editButton.onClick.AddListener(ProcessEditClicked);
    }

    private void EditNoteData(NoteData noteData)
    {
        if (noteData == null)
            throw new ArgumentNullException(nameof(noteData));
        
        _filledNoteInfo.SetNoteData(noteData);
        
        // Animate text changes
        if (_noteText != null)
        {
            Tween fadeOut = _noteText.DOFade(0, 0.2f);
            fadeOut.OnComplete(() => {
                _noteText.text = noteData.Note;
                _noteText.DOFade(1, 0.2f);
            });
        }
        
        if (_dateText != null)
        {
            Tween dateOut = _dateText.DOFade(0, 0.2f);
            dateOut.OnComplete(() => {
                _dateText.text = noteData.Date;
                _dateText.DOFade(1, 0.2f);
            });
        }
    }

    private void ProcessBackButtonClicked()
    {
        // Add click feedback animation
        if (_backButton != null)
            _backButton.transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f), 0.3f, 5, 0.5f);
        
        BackButtonClicked?.Invoke();
        Disable();
    }

    private void ProcessEditClicked()
    {
        // Add click feedback animation
        if (_editButton != null)
            _editButton.transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f), 0.3f, 5, 0.5f);
        
        EditButtonClicked?.Invoke(_filledNoteInfo);
        Disable();
    }
    
    private void OnDestroy()
    {
        // Clean up sequences when object is destroyed
        if (_showSequence != null)
            _showSequence.Kill();
        if (_hideSequence != null)
            _hideSequence.Kill();
    }
}
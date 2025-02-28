using System;
using Bitsplash.DatePicker;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScreenVisabilityHandler))]
public class CreateTravelScreenView : MonoBehaviour
{
    [Header("Sprites")] [SerializeField] private Sprite _calendarClosedSprite;
    [SerializeField] private Sprite _calendarOpenedSprite;

    [Header("UI Components")] [SerializeField]
    private Button _backButton;

    [SerializeField] private TMP_InputField _nameInput;
    [SerializeField] private TMP_InputField _descriptionInput;
    [SerializeField] private TMP_Text _dateText;
    [SerializeField] private Button _saveButtonCalendarClosed;
    [SerializeField] private Button _saveButtonCalendarOpened;
    [SerializeField] private Button _dateButton;
    [SerializeField] private DatePickerSettings _datePicker;

    [Header("Animation Settings")] [SerializeField]
    private float _animationDuration = 0.3f;

    [SerializeField] private Ease _animationEase = Ease.OutQuad;

    private ScreenVisabilityHandler _screenVisabilityHandler;
    private RectTransform _datePickerRectTransform;
    private Vector2 _datePickerInitialPosition;

    public event Action<string> NameChanged;
    public event Action<string> DescriptionChanged;
    public event Action<string> DateChanged;
    public event Action BackButtonClicked;
    public event Action SaveButtonClicked;

    private void Awake()
    {
        _screenVisabilityHandler = GetComponent<ScreenVisabilityHandler>();
        _datePickerRectTransform = _datePicker.GetComponent<RectTransform>();
        _datePickerInitialPosition = _datePickerRectTransform.anchoredPosition;
    }

    private void Start()
    {
        _saveButtonCalendarOpened.gameObject.SetActive(false);
        _dateButton.image.sprite = _calendarClosedSprite;
        _datePicker.gameObject.SetActive(false);
        SetCurrentDate();

        DOTween.Init();
        Disable();
    }

    private void OnEnable()
    {
        _nameInput.onValueChanged.AddListener(OnNameInputChanged);
        _descriptionInput.onValueChanged.AddListener(OnDescriptionChanged);
        _dateButton.onClick.AddListener(OpenCalendar);
        _saveButtonCalendarClosed.onClick.AddListener(OnSaveButtonClicked);
        _backButton.onClick.AddListener(OnBackButtonClicked);
    }

    private void OnDisable()
    {
        _nameInput.onValueChanged.RemoveListener(OnNameInputChanged);
        _descriptionInput.onValueChanged.RemoveListener(OnDescriptionChanged);
        _dateButton.onClick.RemoveListener(OpenCalendar);
        _saveButtonCalendarClosed.onClick.RemoveListener(OnSaveButtonClicked);
        _backButton.onClick.RemoveListener(OnBackButtonClicked);
    }

    public void SetCurrentDate()
    {
        _dateText.text = DateTime.Now.ToString("dd.MM.yyyy");
        DateChanged?.Invoke(_dateText.text);
    }

    public void SetSaveButtonInteractable(bool isInteractable)
    {
        if (_saveButtonCalendarClosed.isActiveAndEnabled)
        {
            _saveButtonCalendarClosed.interactable = isInteractable;
            AnimateSaveButton(_saveButtonCalendarClosed, isInteractable);
        }

        if (_saveButtonCalendarOpened.isActiveAndEnabled)
        {
            _saveButtonCalendarOpened.interactable = isInteractable;
            AnimateSaveButton(_saveButtonCalendarOpened, isInteractable);
        }
    }

    private void AnimateSaveButton(Button button, bool isInteractable)
    {
        button.transform
            .DOScale(isInteractable ? Vector3.one : new Vector3(0.9f, 0.9f, 1f), _animationDuration)
            .SetEase(_animationEase);
    }

    public void Enable()
    {
        _screenVisabilityHandler.EnableScreen();
        AnimateScreenEntrance();
    }

    private void AnimateScreenEntrance()
    {
        transform.DOLocalMoveX(0, _animationDuration)
            .From(new Vector3(Screen.width, 0, 0))
            .SetEase(_animationEase);

        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0;
            canvasGroup.DOFade(1, _animationDuration);
        }
    }

    public void Disable()
    {
        AnimateScreenExit(() => _screenVisabilityHandler.DisableScreen());
    }

    private void AnimateScreenExit(Action onComplete = null)
    {
        transform.DOLocalMoveX(-Screen.width, _animationDuration)
            .SetEase(_animationEase)
            .OnComplete(() => onComplete?.Invoke());

        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.DOFade(0, _animationDuration);
        }
    }

    public void SetNameValue(string value)
    {
        _nameInput.text = value;
        AnimateInputField(_nameInput);
    }

    public void SetDescriptionValue(string value)
    {
        _descriptionInput.text = value;
        AnimateInputField(_descriptionInput);
    }

    private void AnimateInputField(TMP_InputField inputField)
    {
        inputField.transform
            .DOShakePosition(0.5f, 5f, 10, 90, false, true)
            .SetEase(Ease.InOutQuad);
    }

    private void SetDate()
    {
        string text = "";
        var selection = _datePicker.Content.Selection;
        for (int i = 0; i < selection.Count; i++)
        {
            var date = selection.GetItem(i);
            text += date.ToString(format: "dd.MM.yyyy");
        }

        _dateText.text = text;
        DateChanged?.Invoke(_dateText.text);
    }

    private void OpenCalendar()
    {
        _dateButton.onClick.RemoveListener(OpenCalendar);
        _dateButton.image.sprite = _calendarOpenedSprite;

        _saveButtonCalendarClosed.onClick.RemoveListener(OnSaveButtonClicked);
        _saveButtonCalendarClosed.gameObject.SetActive(false);

        _saveButtonCalendarOpened.gameObject.SetActive(true);
        _saveButtonCalendarOpened.onClick.AddListener(OnSaveButtonClicked);

        _dateButton.onClick.AddListener(CloseCalendar);
        _datePicker.gameObject.SetActive(true);

        _datePickerRectTransform.anchoredPosition = _datePickerInitialPosition + new Vector2(0, -Screen.height);
        _datePickerRectTransform.DOAnchorPos(_datePickerInitialPosition, _animationDuration)
            .SetEase(_animationEase);

        _datePicker.Content.OnSelectionChanged.AddListener(SetDate);
    }

    public void CloseCalendar()
    {
        _saveButtonCalendarOpened.onClick.RemoveListener(OnSaveButtonClicked);
        _saveButtonCalendarOpened.gameObject.SetActive(false);

        _dateButton.image.sprite = _calendarClosedSprite;

        _saveButtonCalendarClosed.gameObject.SetActive(true);
        _saveButtonCalendarClosed.onClick.AddListener(OnSaveButtonClicked);

        _datePickerRectTransform
            .DOAnchorPos(_datePickerInitialPosition + new Vector2(0, -Screen.height), _animationDuration)
            .SetEase(_animationEase)
            .OnComplete(() => _datePicker.gameObject.SetActive(false));

        _dateButton.onClick.RemoveListener(CloseCalendar);
        _dateButton.onClick.AddListener(OpenCalendar);
    }

    private void OnNameInputChanged(string value) => NameChanged?.Invoke(value);
    private void OnDescriptionChanged(string value) => DescriptionChanged?.Invoke(value);
    private void OnSaveButtonClicked() => SaveButtonClicked?.Invoke();

    private void OnBackButtonClicked()
    {
        BackButtonClicked?.Invoke();
    }
}
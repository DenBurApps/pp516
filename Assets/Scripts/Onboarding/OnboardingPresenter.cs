using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;

public class OnboardingPresenter : MonoBehaviour
{
    [SerializeField] private OnboardingView _firstScreenView;
    [SerializeField] private OnboardingView _secondScreenView;
    [SerializeField] private float _transitionDelay = 0.3f;
    [SerializeField] private float _sceneTransitionDuration = 1f;
    
    private CanvasGroup _mainCanvasGroup;
    
    private void Awake()
    {
        _mainCanvasGroup = GetComponentInParent<Canvas>()?.GetComponent<CanvasGroup>();
        if (_mainCanvasGroup == null && GetComponentInParent<Canvas>() != null)
        {
            _mainCanvasGroup = GetComponentInParent<Canvas>().gameObject.AddComponent<CanvasGroup>();
        }
    }
    
    private void Start()
    {
        _secondScreenView.DisableScreen();
        
        _firstScreenView.EnableScreen();
        
        DOVirtual.DelayedCall(1.0f, () => {
            AnimateButtonAttention(_firstScreenView);
        });
    }

    private void OnEnable()
    {
        _firstScreenView.InteractableButtonClicked += ProcessFirstScreenButtonClick;
        _secondScreenView.InteractableButtonClicked += ProcessSecondScreenButtonClick;
    }

    private void OnDisable()
    {
        _firstScreenView.InteractableButtonClicked -= ProcessFirstScreenButtonClick;
        _secondScreenView.InteractableButtonClicked -= ProcessSecondScreenButtonClick;
    }

    private void ProcessFirstScreenButtonClick()
    {
        _firstScreenView.DisableScreen();
        
        _secondScreenView.EnableScreen();
        
        DOVirtual.DelayedCall(0.5f, () => {
            AnimateButtonAttention(_secondScreenView);
        });
    }

    private void ProcessSecondScreenButtonClick()
    {
        PlayerPrefs.SetInt("Onboarding", 1);
        PlayerPrefs.Save();
        
        if (_mainCanvasGroup != null)
        {
            _mainCanvasGroup.DOFade(0f, _sceneTransitionDuration)
                .SetEase(Ease.InOutQuad)
                .OnComplete(() => {
                    SceneManager.LoadScene("HomeScreenScene");
                });
        }
        else
        {
            SceneManager.LoadScene("HomeScreenScene");
        }
    }
    
    private void AnimateButtonAttention(OnboardingView view)
    {
        Button button = view.GetComponentInChildren<UnityEngine.UI.Button>();
        if (button != null)
        {
            DOVirtual.DelayedCall(1.0f, () => {
                Sequence sequence = DOTween.Sequence();
                sequence.Append(button.transform.DOScale(1.1f, 0.5f).SetEase(Ease.InOutQuad));
                sequence.Append(button.transform.DOScale(1f, 0.5f).SetEase(Ease.InOutQuad));
                sequence.SetLoops(2);
            });
        }
    }
    
    private void OnDestroy()
    {
        DOTween.KillAll();
    }
}
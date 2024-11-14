using UnityEngine;
using UnityEngine.SceneManagement;

public class OnboardingPresenter : MonoBehaviour
{
    [SerializeField] private OnboardingView _firstScreenView;
    [SerializeField] private OnboardingView _secondScreenView;
    
    private void Start()
    {
        _secondScreenView.DisableScreen();
        _firstScreenView.EnableScreen();
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
    }

    private void ProcessSecondScreenButtonClick()
    {
        PlayerPrefs.SetInt("Onboarding", 1);
        SceneManager.LoadScene("HomeScreenScene");
    }
}

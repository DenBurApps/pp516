using UnityEngine;
using UnityEngine.SceneManagement;

public class BootScript : MonoBehaviour
{
    private void Awake()
    {
        if (!PlayerPrefs.HasKey("Onboarding"))
        {
            SceneManager.LoadScene("OnboardingScene");
        }
        else
        {
            SceneManager.LoadScene("HomeScreenScene");
        }
    }
}

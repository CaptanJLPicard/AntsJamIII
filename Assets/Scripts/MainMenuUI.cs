using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [Header("UIPanels")]
    [SerializeField] private GameObject SettingMenu;
    [SerializeField] private GameObject CreditsMenu;
    private bool isSettingsActive;
    private bool isCreditsActive;

    [SerializeField] private MMF_Player buttonClickSound;

    public void StartButton()
    {
        buttonClickSound.PlayFeedbacks();
        SceneManager.LoadScene(1);
    }

    public void SettingsBtn()
    {
        buttonClickSound.PlayFeedbacks();
        if (isSettingsActive)
        {
            SettingMenu.SetActive(false);
            isSettingsActive = false;
        }
        else
        {
            SettingMenu.SetActive(true);
            isSettingsActive = true;
        }
    }

    public void CredistMenu()
    {
        buttonClickSound.PlayFeedbacks();
        if (isCreditsActive)
        {
            CreditsMenu.SetActive(false);
            isCreditsActive = false;
        }
        else
        {
            CreditsMenu.SetActive(true);
            isCreditsActive = true;
        }
    }

    public void QuitButton()
    {
        buttonClickSound.PlayFeedbacks();
        Application.Quit();
    }
}

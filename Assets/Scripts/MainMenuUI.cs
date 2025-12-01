using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [Header("UIPanels")]
    [SerializeField] private GameObject SettingMenu;
    [SerializeField] private GameObject CreditsMenu;
    private bool isSettingsActive;
    private bool isCreditsActive;

    public void StartButton()
    {
        SceneManager.LoadScene(1);
    }

    public void SettingsBtn()
    {
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
        Application.Quit();
    }
}

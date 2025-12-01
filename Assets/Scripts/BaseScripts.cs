using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.UI;

public class BaseScripts : MonoBehaviour
{
    [SerializeField] private float baseHealth = 100;
    [SerializeField] private MMF_Player deathEffects;
    [SerializeField] private GameObject LoseMenu;
    [SerializeField] private GameObject WinMenu;
    [SerializeField] private bool isFriend;

    [Header("UI")]
    [SerializeField] private Slider baseHealthSlider;
    private float maxBaseHealth;

    private void Start()
    {
        LoseMenu.SetActive(false);
        WinMenu.SetActive(false);

        maxBaseHealth = baseHealth;
        if (baseHealthSlider != null)
        {
            baseHealthSlider.maxValue = maxBaseHealth;
            baseHealthSlider.value = baseHealth;
        }
    }

    private void Update()
    {
        HealthCheck();
        UpdateHealthUI();
    }

    private void HealthCheck()
    {
        if (baseHealth <= 0f)
        {
            deathEffects?.PlayFeedbacks();
            gameObject.layer = 0;
            Destroy(gameObject, 3f);

            if (isFriend)
                LoseMenu.SetActive(true);
            else
                WinMenu.SetActive(true);
        }
    }

    public void BaseDamage(float damage)
    {
        baseHealth -= damage;
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        if (baseHealthSlider != null)
        {
            baseHealthSlider.value = baseHealth;
        }
    }
}

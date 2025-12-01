using MoreMountains.Feedbacks;
using UnityEngine;

public class BaseScripts : MonoBehaviour
{
    [SerializeField] private float baseHealth = 100;
    [SerializeField] private MMF_Player deathEffects;
    [SerializeField] private GameObject LoseMenu;
    [SerializeField] private GameObject WinMenu;
    [SerializeField] private bool isFriend;

    private void Start()
    {
        LoseMenu.SetActive(false);
        WinMenu.SetActive(false);
    }
    private void Update()
    {
        HealthCheck();
    }

    private void HealthCheck()
    {
        if (baseHealth <= 0f)
        {
            deathEffects?.PlayFeedbacks();
            gameObject.layer = 0;
            Destroy(gameObject, 3f);

            if(isFriend) LoseMenu.SetActive(true);
            else WinMenu.SetActive(true);
        }
    }

    public void BaseDamage(float damage)
    {
        baseHealth -= damage;
    }
}

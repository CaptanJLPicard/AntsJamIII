using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VInspector;

public class PAC_CraftSystem : MonoBehaviour, IDropHandler
{
    [Header("UI References")]
    public TextMeshProUGUI contentsTxt;
    public TextMeshProUGUI brewTimeTxt;
    public TextMeshProUGUI statusMessageTxt;
    public Image brewProgressBar;

    [Header("Status Message Settings")]
    [SerializeField] private float messageDuration = 2f;
    private float messageTimer = 0f;

    [Header("Chaos System")]
    [ReadOnly] public float chaos;
    [SerializeField] private float minChaos = 0f;
    [SerializeField] private float maxChaos = 100f;
    [SerializeField, Range(1, 5)] private float increaseChaosSpeed = 1f;

    [Header("Brew Time System")]
    [SerializeField] private float brewTime = 0f;
    [SerializeField] private float optimalBrewTimeMin = 2f;
    [SerializeField] private float optimalBrewTimeMax = 4f;
    [SerializeField] private float maxBrewTime = 6f;
    [SerializeField] private bool isBrewing = false;

    [Header("Bonus System")]
    [SerializeField] private float perfectBrewBonus = 1.5f;
    [SerializeField] private float goodBrewBonus = 1.2f;
    [SerializeField] private float poorBrewPenalty = 0.7f;
    [SerializeField] private int comboCount = 0;
    [SerializeField] private float comboMultiplier = 1f;
    [SerializeField] private float maxComboMultiplier = 2f;

    [System.Serializable]
    public class Combinations
    {
        public string spellName;
        public List<bool> combination;
        public bool isThisOk = false;
        public float howMuchChaos;
    }

    [SerializeField] private List<Combinations> combinations;
    public List<bool> currentCombination = new List<bool>();
    public List<GameObject> currentObjects = new List<GameObject>();
    private List<bool> previousCombination;
    [SerializeField] private int currentComboIndex = 0;
    [SerializeField] private Spawner spawner;

    // Events
    public System.Action<string, Color> OnStatusMessage;
    public System.Action<float> OnSpellSuccess;
    public System.Action OnSpellFail;

    private void Awake()
    {
        previousCombination = new List<bool>();
        minChaos = 0;
        chaos = maxChaos;
        ResetSpell();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            eventData.pointerDrag.GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
            eventData.pointerDrag.gameObject.GetComponent<CanvasGroup>().alpha = 1f;
        }
    }

    private void Update()
    {
        UpdateBrewTime();
        UpdateStatusMessage();

        if (!currentCombination.SequenceEqual(previousCombination))
        {
            CheckCombination();
            previousCombination = new List<bool>(currentCombination);
        }

        if (chaos < maxChaos)
            chaos += Time.deltaTime * increaseChaosSpeed;
        chaos = Mathf.Clamp(chaos, minChaos, maxChaos);
    }

    private void UpdateStatusMessage()
    {
        if (statusMessageTxt == null) return;

        if (messageTimer > 0)
        {
            messageTimer -= Time.deltaTime;

            if (messageTimer < 0.5f)
            {
                Color c = statusMessageTxt.color;
                c.a = messageTimer / 0.5f;
                statusMessageTxt.color = c;
            }
        }
        else
        {
            statusMessageTxt.text = "";
        }
    }

    private void UpdateBrewTime()
    {
        if (isBrewing && currentObjects.Count > 0)
        {
            brewTime += Time.deltaTime;

            if (brewTimeTxt != null)
            {
                string brewStatus = GetBrewStatus();
                brewTimeTxt.text = $"Brew: {brewTime:F1}s ({brewStatus})";
            }

            if (brewProgressBar != null)
            {
                float progress = Mathf.Clamp01(brewTime / maxBrewTime);
                brewProgressBar.fillAmount = progress;

                if (brewTime < optimalBrewTimeMin)
                    brewProgressBar.color = Color.yellow;
                else if (brewTime <= optimalBrewTimeMax)
                    brewProgressBar.color = Color.green;
                else
                    brewProgressBar.color = Color.red;
            }

            if (brewTime > maxBrewTime)
            {
                ShowStatusMessage("Spell ruined! Waited too long.", Color.red);
                ResetSpell();
            }
        }
    }

    private string GetBrewStatus()
    {
        if (brewTime < optimalBrewTimeMin) return "<color=#FFFF00>Too Early</color>";
        if (brewTime <= optimalBrewTimeMax) return "<color=#00FF00>PERFECT!</color>";
        if (brewTime <= maxBrewTime) return "<color=#FFA500>Late</color>";
        return "<color=#FF0000>RUINED</color>";
    }

    public void OnItemAdded(int itemIndex, string itemName)
    {
        if (!isBrewing && currentObjects.Count == 1)
        {
            isBrewing = true;
            brewTime = 0f;
        }
    }

    private void CheckCombination()
    {
        ResetCombinationState();

        for (int i = 0; i < combinations.Count; i++)
        {
            var combo = combinations[i];
            if (currentCombination.SequenceEqual(combo.combination))
            {
                combo.isThisOk = true;
                currentComboIndex = i;
                return;
            }
        }
    }

    private void ResetCombinationState()
    {
        foreach (var combo in combinations)
            combo.isThisOk = false;
        currentComboIndex = 0;
    }

    private void ShowStatusMessage(string message, Color color)
    {
        if (statusMessageTxt != null)
        {
            statusMessageTxt.text = message;
            statusMessageTxt.color = color;
            messageTimer = messageDuration;
        }

        OnStatusMessage?.Invoke(message, color);
        Debug.Log($"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{message}</color>");
    }

    #region Buttons

    public void ResetSpell()
    {
        foreach (var obj in currentObjects)
        {
            if (obj != null)
            {
                obj.GetComponent<CanvasGroup>().alpha = 0f;
                StopAllCoroutines();
                Destroy(obj);
            }
        }

        contentsTxt.text = "Contents: 0";
        currentCombination.Clear();
        currentObjects.Clear();
        currentCombination = new List<bool>(new bool[8]);
        ResetCombinationState();

        isBrewing = false;
        brewTime = 0f;

        if (brewProgressBar != null)
            brewProgressBar.fillAmount = 0f;

        if (brewTimeTxt != null)
            brewTimeTxt.text = "Brew: Ready";
    }

    public void Spawn()
    {
        if (currentObjects.Count == 0)
        {
            ShowStatusMessage("Cauldron is empty!", Color.yellow);
            return;
        }

        float requiredChaos = currentComboIndex == 0 ? 35f : combinations[currentComboIndex].howMuchChaos;

        if (chaos < requiredChaos)
        {
            ShowStatusMessage("Not enough energy!", Color.red);
            return;
        }

        chaos -= requiredChaos;

        float brewMultiplier = CalculateBrewMultiplier();
        float finalMultiplier = brewMultiplier * comboMultiplier;

        comboCount++;
        comboMultiplier = Mathf.Min(1f + (comboCount * 0.1f), maxComboMultiplier);

        spawner.SpawnWithMultiplier(currentComboIndex, finalMultiplier);

        string bonusText = finalMultiplier > 1f ? $" (x{finalMultiplier:F1} power!)" : "";
        ShowStatusMessage($"Spell success!{bonusText}", Color.green);
        OnSpellSuccess?.Invoke(finalMultiplier);

        ResetSpell();
        ResetCombinationState();
    }

    private float CalculateBrewMultiplier()
    {
        if (!isBrewing || brewTime <= 0f)
            return 1f;

        if (brewTime >= optimalBrewTimeMin && brewTime <= optimalBrewTimeMax)
        {
            ShowStatusMessage("PERFECT TIMING!", Color.cyan);
            return perfectBrewBonus;
        }
        else if (brewTime < optimalBrewTimeMin)
        {
            ShowStatusMessage("Too early! Weak spell.", Color.yellow);
            return poorBrewPenalty;
        }
        else
        {
            ShowStatusMessage("Too late! Spell weakened.", Color.yellow);
            return goodBrewBonus;
        }
    }

    public float GetBrewTime() => brewTime;
    public bool IsBrewing() => isBrewing;
    public int GetComboCount() => comboCount;
    public float GetComboMultiplier() => comboMultiplier;

    #endregion
}
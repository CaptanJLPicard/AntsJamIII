using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using VInspector;
using static PAC_CraftSystem;

public class PAC_CraftSystem : MonoBehaviour, IDropHandler
{
    [Header("Get Variables")]
    public TextMeshProUGUI contentsTxt;

    [Header("Set Variables")]
    [ReadOnly] public float chaos;
    [SerializeField] private float minChaos = 0f;
    [SerializeField] private float maxChaos = 100f;
    [SerializeField, Range(1, 5)] private float increaseChaosSpeed = 1f;

    [System.Serializable]
    public class Combinations
    {
        public string spellName;
        public List<bool> combination;
        public bool isThisOk = false;
        public float hwmuchChaos;
    }

    [SerializeField] private List<Combinations> combinations;
    public List<bool> currentCombination = new List<bool>();
    public List<GameObject> currentObjects = new List<GameObject>();

    private List<bool> previousCombination;

    [SerializeField] private int currentComboIndex = 0;
    [SerializeField] private Spawner spawner;

    private void Awake()
    {
        previousCombination = new List<bool>();
        minChaos = 0;
        chaos = maxChaos;
        ResetSpeel();
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
        // Sadece kombinasyon degistiginde kontrol et
        if (!currentCombination.SequenceEqual(previousCombination))
        {
            CheckCombination();
            previousCombination = new List<bool>(currentCombination);
        }

        if (chaos < maxChaos) chaos += Time.deltaTime * increaseChaosSpeed;
        chaos = Mathf.Clamp(chaos, minChaos, maxChaos);
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

    #region Buttons
    public void ResetSpeel()
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
    }

    public void Spawn()
    {
        if (currentComboIndex == 0 && chaos >= 35f) chaos -= 35f;
        else if (currentComboIndex != 0 && chaos >= combinations[currentComboIndex].hwmuchChaos) chaos -= combinations[currentComboIndex].hwmuchChaos;
        else return;
        spawner.ChangeSpawnIndex(currentComboIndex);
        ResetSpeel();
        ResetCombinationState();
    }
    #endregion
}
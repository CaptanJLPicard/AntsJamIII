using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject[] spawnerPrefabs;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnTime = 3f;
    [SerializeField] private float currentSpawnTime;

    [Header("Current State")]
    [SerializeField] private int currentSoldierIndex = 0;
    [SerializeField] private float pendingMultiplier = 1f;

    private void Start()
    {
        currentSpawnTime = spawnTime;
    }

    private void Update()
    {
        currentSpawnTime -= Time.deltaTime;

        if (currentSpawnTime <= 0)
        {
            currentSpawnTime = spawnTime;
            CharacterSpawn();
        }
    }

    public void ChangeSpawnIndex(int index)
    {
        currentSoldierIndex = Mathf.Clamp(index, 0, spawnerPrefabs.Length - 1);

        if (index != 0)
        {
            currentSpawnTime = 0;
        }
    }

    public void SpawnWithMultiplier(int index, float multiplier)
    {
        currentSoldierIndex = Mathf.Clamp(index, 0, spawnerPrefabs.Length - 1);
        pendingMultiplier = multiplier;
        currentSpawnTime = 0;
    }

    private void CharacterSpawn()
    {
        if (spawnerPrefabs == null || spawnerPrefabs.Length == 0)
        {
            Debug.LogWarning("Spawner prefabs empty!");
            return;
        }

        if (currentSoldierIndex >= spawnerPrefabs.Length)
        {
            Debug.LogWarning($"Invalid soldier index: {currentSoldierIndex}");
            currentSoldierIndex = 0;
        }

        GameObject spawnedSoldier = Instantiate(
            spawnerPrefabs[currentSoldierIndex],
            transform.position,
            Quaternion.identity
        );

        // Character script'ine multiplier uygula
        Character character = spawnedSoldier.GetComponent<Character>();
        if (character != null)
        {
            character.ApplyMultiplier(pendingMultiplier);
        }

        // Reset
        currentSoldierIndex = 0;
        pendingMultiplier = 1f;
    }

    public int GetPrefabCount() => spawnerPrefabs?.Length ?? 0;
}
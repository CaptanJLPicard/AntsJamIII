using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Refenrences")]
    [SerializeField] private GameObject[] spawnerPrefabs;
    // Default = 0 //Ates = 1 //Su = 2 // Bitki = 3 // Hava = 4

    [Header("Variables")]
    [SerializeField] private float currentSpawnTime;
    [SerializeField] private float spawnTime;

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

    private void CharacterSpawn()
    {
        Instantiate(spawnerPrefabs[Random.Range(0, spawnerPrefabs.Length)], transform.position, Quaternion.identity);
    }
}

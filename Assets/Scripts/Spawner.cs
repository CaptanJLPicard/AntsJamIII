using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Refenrences")]
    [SerializeField] private GameObject[] spawnerPrefabs;
    private int currentSoliderIndex;  // Default = 0 //Ates = 1 //Su = 2 // Bitki = 3 // Hava = 4
    
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

    public void ChangeSpawnIndex(int index)
    {
        currentSoliderIndex = index;
        if (index != 0) currentSpawnTime = 0;
    }

    private void CharacterSpawn()
    {
        Instantiate(spawnerPrefabs[currentSoliderIndex], transform.position, Quaternion.identity);
        currentSoliderIndex = 0;
    }
}

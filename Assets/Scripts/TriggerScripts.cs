using UnityEngine;

public class TriggerScripts : MonoBehaviour
{
    [SerializeField] private Spawner spawnerSc;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.W))
        {
            spawnerSc.ChangeSpawnIndex(3);
        }
    }
}

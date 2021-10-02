using System;
using UnityEngine;
using Random = UnityEngine.Random;


[Serializable]
class BoxSpawnWeight
{
    public PickableBox box;
    public float weight;
}

public class BoxesSpawner : MonoBehaviour
{
    [SerializeField] private BoxSpawnWeight[] boxSpawnWeights;

    private void Awake()
    {
        PickableBox box = GetBoxToSpawn();
        PickableBox boxSpawned = Instantiate(box);
        boxSpawned.transform.position = transform.position;
    }

    private PickableBox GetBoxToSpawn()
    {
        float totalWeight = 0.0f;
        foreach (BoxSpawnWeight boxSpawnWeight in boxSpawnWeights)
        {
            totalWeight += boxSpawnWeight.weight;
        }

        float RandomRange = Random.Range(0, totalWeight);
        foreach (BoxSpawnWeight boxSpawnWeight in boxSpawnWeights)
        {
            RandomRange -= boxSpawnWeight.weight;
            if (RandomRange <= 0.0f)
            {
                return boxSpawnWeight.box;
            }
        }

        return null;
    }
}

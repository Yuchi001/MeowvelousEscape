using UnityEngine;

public class SpawnCageMaybe : MonoBehaviour
{
    private static Vector2? randomOffset;
    
    [SerializeField]
    private float scale;

    [SerializeField]
    private float cageSpawnValue = 0.9f;
    [SerializeField]
    private GameObject cagePrefab;

    [SerializeField]
    private float guardSpawnValue = 0.6f;
    [SerializeField]
    private GameObject[] guardPrefabs;

    [SerializeField]
    private float inactiveRadius = 10;

    private void Awake()
    {
        if (randomOffset.HasValue == false)
        {
            randomOffset = new Vector2(
                Random.Range(-10000, 10000),
                Random.Range(-10000, 10000));
        }
    }


    private void Start()
    {
        if (transform.position.magnitude > inactiveRadius)
        {
            float value = Mathf.PerlinNoise(
                transform.position.x / scale + randomOffset.Value.x,
                transform.position.y / scale + randomOffset.Value.y);

            if (value > cageSpawnValue)
            {
                var cage = Instantiate(cagePrefab, transform.position, Quaternion.identity);
            }
            else if (value > guardSpawnValue)
            {
                var randomGuardPrefab = guardPrefabs[Random.Range(0, guardPrefabs.Length)];
                var guard = Instantiate(randomGuardPrefab, transform.position, Quaternion.identity);
            }
        }
    }
}

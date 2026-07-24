using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject enemyPrefab;

    [SerializeField] float spawnInterval = 2f;

    [SerializeField] float rangeX = 20f;
    [SerializeField] float rangeZ = 20f;

    float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        Vector3 pos = new Vector3(
        Random.Range(-rangeX, rangeX),
        50f,
        Random.Range(-rangeZ, rangeZ)
);

        if (Physics.Raycast(pos, Vector3.down, out RaycastHit hit, 100f))
        {
            Instantiate(enemyPrefab, hit.point, Quaternion.identity);
        }
    }
}
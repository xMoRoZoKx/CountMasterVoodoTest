using System.Collections;
using UnityEngine;
using TMPro;

public class EnemySpawner : MonoBehaviour
{
    [Header("Настройки спавна")]
    public GameObject enemyPrefab;
    public GameObject bossPrefab;
    public Transform spawnPoint;
    public float minSpawnInterval = 1f;
    public float maxSpawnInterval = 5f;
    public float spawnRangeX = 10f;

    public int spawnBossEach = 3;

    [Header("Здоровье спавнера")]
    public int maxLives = 99;
    private int currentLives;

    public TextMeshPro lifeText;
    public GameObject hitEffect;

    private bool spawning = true;
    private int spawnCount = 0; // счетчик спавнов

    private void Start()
    {
        currentLives = maxLives;
        UpdateLifeDisplay();
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        yield return new WaitForSeconds(2);
        while (spawning)
        {
            SpawnEnemy();
            float delay = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(delay);
        }
    }

    private void SpawnEnemy()
    {
        if (!spawning || spawnPoint == null) return;

        spawnCount++;

        GameObject prefabToSpawn;
        Vector3 spawnPos;

        if (spawnCount % spawnBossEach == 0 && bossPrefab != null)
        {
            // Спавним босса по центру
            prefabToSpawn = bossPrefab;
            spawnPos = spawnPoint.position;
        }
        else if (enemyPrefab != null)
        {
            // Спавним обычного врага со смещением
            prefabToSpawn = enemyPrefab;
            float offsetX = Random.Range(-spawnRangeX / 2f, spawnRangeX / 2f);
            spawnPos = spawnPoint.position + Vector3.right * offsetX;
        }
        else
        {
            return;
        }

        GameObject enemy = Instantiate(prefabToSpawn, spawnPos, Quaternion.Euler(0, 180, 0));

        if (prefabToSpawn == enemyPrefab)
        {
            // Только обычных врагов двигаем к позиции
            float targetX = spawnPoint.position.x + Random.Range(-spawnRangeX / 2f, spawnRangeX / 2f);
            StartCoroutine(MoveEnemyToPosition(enemy.transform, targetX, 2f, 1f));
        }
    }

    private IEnumerator MoveEnemyToPosition(Transform enemyTransform, float targetX, float delay, float duration)
    {
        yield return new WaitForSeconds(delay);

        float startX = enemyTransform.position.x;
        float elapsed = 0f;

        while (elapsed < duration && enemyTransform != null)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / duration);
            float newX = Mathf.Lerp(startX, targetX, t);

            Vector3 pos = enemyTransform.position;
            pos.x = newX;
            enemyTransform.position = pos;

            yield return null;
        }

        if (enemyTransform != null)
        {
            Vector3 finalPos = enemyTransform.position;
            finalPos.x = targetX;
            enemyTransform.position = finalPos;
        }
    }

    public void TakeHit()
    {
        currentLives--;

        if (hitEffect != null)
        {
            Instantiate(hitEffect, transform.position, Quaternion.identity);
        }

        UpdateLifeDisplay();

        if (currentLives <= 0)
        {
            spawning = false;
            Destroy(gameObject);
        }
    }

    private void UpdateLifeDisplay()
    {
        if (lifeText != null)
        {
            lifeText.text = currentLives.ToString();
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (spawnPoint == null) return;

        Gizmos.color = Color.red;
        Vector3 left = spawnPoint.position + Vector3.left * spawnRangeX / 2f;
        Vector3 right = spawnPoint.position + Vector3.right * spawnRangeX / 2f;

        float thickness = 0.1f;
        for (float offset = -thickness; offset <= thickness; offset += thickness / 2f)
        {
            Vector3 offsetY = Vector3.up * offset;
            Gizmos.DrawLine(left + offsetY, right + offsetY);
        }
    }
#endif
}

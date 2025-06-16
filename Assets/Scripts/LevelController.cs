using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class EnemySpawnInfo
{
    public GameObject enemyPrefab;
    public int count;
    public float delayBetweenSpawns = 1f;
    public int[] allowedLanes = { 0, 1 }; // индексы полос
    public float delayAfterWave = 2f;
}

public class LevelController : MonoBehaviour
{
    public Transform[] lanePositions; // индексы: 0 - левая, 1 - центр, 2 - правая
    public Transform player;
    public float spawnDistanceAhead = 30f;

    public List<EnemySpawnInfo> enemyWaves = new ();

    void Start()
    {
        StartCoroutine(SpawnWaves());
    }

    System.Collections.IEnumerator SpawnWaves()
    {
        foreach (var wave in enemyWaves)
        {
            float zOffset = 0f;

            for (int i = 0; i < wave.count; i++)
            {
                int laneIndex = wave.allowedLanes[Random.Range(0, wave.allowedLanes.Length)];
                Vector3 basePos = lanePositions[laneIndex].position;

                float spacingZ = 0.5f; // дистанция между врагами вдоль Z
                float offsetX = Random.Range(-0.5f, 0.5f); // разброс по X в пределах полосы

                Vector3 spawnPos = new Vector3(
                    basePos.x + offsetX,
                    basePos.y,
                    player.position.z + spawnDistanceAhead + zOffset
                );

                Instantiate(wave.enemyPrefab, spawnPos, Quaternion.identity);
                zOffset += spacingZ;

                yield return new WaitForSeconds(wave.delayBetweenSpawns);
            }

            yield return new WaitForSeconds(wave.delayAfterWave);
        }
    }
}
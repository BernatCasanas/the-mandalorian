using System;
using DiamondEngine;
using System.Collections.Generic;

public class SpawnManager : DiamondComponent
{
    public static SpawnManager instance = null;
    public List<GameObject> availableSpawnPoints = null;

    private int enemiesToSpawn = 0;
    public int maxEnemiesPerWave = 0;
    public int enemyIncreasePerWave = 0;
    public int wave = 0;
    public int maxWaves = 1;

    public float waveTimer = 0.0f;
    public float timeBetweenWaves = 1.0f;

    public void Awake()
    {
        instance = this;
        availableSpawnPoints = new List<GameObject>();

        enemiesToSpawn = maxEnemiesPerWave;

        if (waveTimer <= 0.0f)
            waveTimer = 0.01f;
    }

    public void Update()
    {
        if (waveTimer > 0.0f)
        {
            waveTimer -= Time.deltaTime;

            if(waveTimer <= 0.0f)
            {
                SpawnWave();

                if (enemiesToSpawn > 0)
                    waveTimer = timeBetweenWaves * 0.25f;
                else
                {
                    wave++;
                    enemiesToSpawn = maxEnemiesPerWave;

                    if (wave < maxWaves)
                        waveTimer = timeBetweenWaves;
                }
            }
        }
    }

    public void AddSpawnPoint(GameObject spawnPoint)
    {
        if (availableSpawnPoints == null)
            availableSpawnPoints = new List<GameObject>();

        availableSpawnPoints.Add(spawnPoint);
    }

    public void RemoveSpawnPoint(GameObject spawnPoint)
    {
        for (int i = 0; i < availableSpawnPoints.Count; ++i)
        {
            if (availableSpawnPoints[i].GetUid() == spawnPoint.GetUid())
            {
                availableSpawnPoints.RemoveAt(i);
                return;
            }
        }
    }

    public void ClearSpawnPointsList()
    {
        availableSpawnPoints.Clear();
    }

    private void SpawnWave()
    {
        //Debug.Log("Count: " + availableSpawnPoints.Count.ToString());
        List<GameObject> spawnPoints = new List<GameObject>();

        for (int i = 0; i < availableSpawnPoints.Count; ++i)
        {
            spawnPoints.Add(availableSpawnPoints[i]);
        }

        Random randomizer = new Random();

        int pendingEnemies = 0;
        if (enemiesToSpawn > availableSpawnPoints.Count)
        {
            //we should not maximize the amount of enemies to the maximum spawn points, it will be solved
            pendingEnemies = enemiesToSpawn - availableSpawnPoints.Count;
            enemiesToSpawn = availableSpawnPoints.Count;
        }

        for (int i = 0; i < enemiesToSpawn; ++i)
        {
            int index = randomizer.Next(0, spawnPoints.Count);

            //Debug.Log("Index: " + index.ToString() + " count: " + spawnPoints.Count.ToString());
            GameObject spawnPoint = spawnPoints[index];
            SpawnPoint spawnScript = spawnPoint.GetComponent<SpawnPoint>();

            if (spawnScript != null)
                spawnScript.SpawnEnemy();

            for (int j = 0; j < spawnPoints.Count; ++j)
            {
                if (spawnPoint.GetUid() == spawnPoints[j].GetUid())
                {
                    spawnPoints.RemoveAt(j);
                    break;
                }
            }
        }

        enemiesToSpawn = pendingEnemies;
    }
}
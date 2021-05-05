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

    public int enemiesLeftToNextWave = 2;

    public float waveTimer = 0.0f;
    public float timeBetweenWaves = 1.0f;

    public float timeDelayMult = 1.0f;

    private bool awaitingForEnemyCount = false;

    public void Awake()
    {
        instance = this;
        availableSpawnPoints = new List<GameObject>();

        enemiesToSpawn = maxEnemiesPerWave;

        if (waveTimer <= 0.0f)
            waveTimer = 0.01f;

        EnemyManager.ClearList();

        //Random randomizer = new Random();
        //enemiesLeftToNextWave = randomizer.Next(0, enemiesLeftToNextWave);
    }

    public void Update()
    {
        waveTimer -= Time.deltaTime;

        if (((waveTimer <= 0.0f && EnemyManager.EnemiesLeft() <= enemiesLeftToNextWave) || EnemyManager.EnemiesLeft() == 0) && EnemyManager.awaitingForEnemiesToSpawn == 0)
        {
            SpawnWave();
            waveTimer = 0f;
            awaitingForEnemyCount = true;

            //Random randomizer = new Random();
            //enemiesLeftToNextWave = randomizer.Next(0, enemiesLeftToNextWave);

            if (enemiesToSpawn > 0)
                waveTimer = timeBetweenWaves * 0.25f;
            else
            {
                ++wave;

                if (wave < maxWaves)
                {
                    waveTimer = timeBetweenWaves;
                    maxEnemiesPerWave += enemyIncreasePerWave;
                    enemiesToSpawn = maxEnemiesPerWave;
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
            {
                float delay = (float)(randomizer.NextDouble() * timeDelayMult);
                spawnScript.QueueSpawnEnemy(delay);
   
            }

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
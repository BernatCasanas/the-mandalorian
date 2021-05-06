using System;
using DiamondEngine;
using System.Collections.Generic;

enum SPAWN_STATE : int
{
    NONE = -1,

    PREV_DELAY,
    PARTICLE_DELAY
}


public class SpawnPoint : DiamondComponent
{
    private List<string> possibleEnemies = null;

    public bool spawnStormtrooper = false;
    public bool spawnBantha = false;
    public bool spawnSkytrooper = false;
    public bool spawnLaserTurret = false;
    public bool spawnDeathrooper = false;
    public bool spawnHeavyTrooper   = false;

    private string stormtrooperPath = "Library/Prefabs/489054570.prefab";
    private string banthaPath = "Library/Prefabs/978476012.prefab";
    private string skytrooperPath = "Library/Prefabs/355603284.prefab";
    private string laserTurretPath = "Library/Prefabs/1243866490.prefab";
    private string deathrooperPath = "Library/Prefabs/1439379622.prefab";
    private string heavytrooperPath  = "Library/Prefabs/21832274.prefab";

    public float spawnEnemyAt = 0f;

    private List<float> enemiesToSpawn = new List<float>();
    private List<float> spawningEnemies = new List<float>();

    public GameObject spawnParticleObj = null;
    private ParticleSystem spawnParticle = null;

    public void Awake()
    {
        if (SpawnManager.instance != null)
        {
            SpawnManager.instance.AddSpawnPoint(gameObject);
            Debug.Log("Spawn Point added");
        }

        possibleEnemies = new List<string>();

        if (spawnStormtrooper) possibleEnemies.Add(stormtrooperPath);
        if (spawnBantha) possibleEnemies.Add(banthaPath);
        if (spawnSkytrooper) possibleEnemies.Add(skytrooperPath);
        if (spawnLaserTurret) possibleEnemies.Add(laserTurretPath);
        if (spawnDeathrooper) possibleEnemies.Add(deathrooperPath);
        if (spawnHeavyTrooper)  possibleEnemies.Add(heavytrooperPath);

        enemiesToSpawn = new List<float>();
        spawningEnemies = new List<float>();

        if (spawnParticleObj != null)
            spawnParticle = spawnParticleObj.GetComponent<ParticleSystem>();

    }

    public void Update()
    {
        for (int i = 0; i < enemiesToSpawn.Count; i++)
        {
            enemiesToSpawn[i] -= Time.deltaTime;

            if (enemiesToSpawn[i] <= 0f)
            {
                if (spawnParticle != null)
                    spawnParticle.Play();

                Audio.PlayAudio(gameObject, "Play_Enemy_Respawn");

                spawningEnemies.Add(spawnEnemyAt);

                enemiesToSpawn.RemoveAt(i);
                --i;
            }
        }


        for (int i = 0; i < spawningEnemies.Count; i++)
        {
            spawningEnemies[i] -= Time.deltaTime;

            if (spawningEnemies[i] <= 0f)
            {
                InstanciateEnemy();
                spawningEnemies.RemoveAt(i);
                --i;
            }
        }

    }

    public bool QueueSpawnEnemy(float delayTime = 0f)
    {
        if (possibleEnemies.Count <= 0)
            return false;

        enemiesToSpawn.Add(delayTime);

        EnemyManager.awaitingForEnemiesToSpawn++;

        return true;
    }

    private GameObject InstanciateEnemy()
    {
        if (possibleEnemies.Count <= 0)
            return null;

        Random randomizer = new Random();

        int prefabIndex = randomizer.Next(possibleEnemies.Count);

        GameObject enemy = InternalCalls.CreatePrefab(possibleEnemies[prefabIndex], gameObject.transform.globalPosition, Quaternion.identity, new Vector3(1, 1, 1));

        EnemyManager.awaitingForEnemiesToSpawn--;

        return enemy;
    }


}
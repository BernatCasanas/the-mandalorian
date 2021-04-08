using System;
using DiamondEngine;
using System.Collections.Generic;
public class Spawn : DiamondComponent
{
    private string libraryPath = "Library/Prefabs/489054570.prefab";

    public int prefab_1_UID = 0;
    public int prefab_2_UID = 0;

    private bool doneSpawning = false;

    public GameObject spawnPoint0 = null;
    public GameObject spawnPoint1 = null;
    public GameObject spawnPoint2 = null;
    public GameObject spawnPoint3 = null;
    public GameObject spawnPoint4 = null;
    public GameObject spawnPoint5 = null;

    public int maxSpawnPoints = 0;
    public int maxEnemies = 0;
    public int enemyIncreasePerWave = 0;

    public int wave = 0;
    public int maxWaves = 1;

    public float timePassed = 0.0f;
    public float timeBetweenSpawns = 8.0f;

    private bool fightEndMusicPlayed = false;

    public bool turretSpawnPoint0 = false;
    public bool turretSpawnPoint1 = false;
    public bool turretSpawnPoint2 = false;
    public bool turretSpawnPoint3 = false;
    public bool turretSpawnPoint4 = false;
    public bool turretSpawnPoint5 = false;

    public List<GameObject> currentEnemies = null;


    bool start = true;

    public void Awake()
    {
    }

    public void Update()
    {
        if (start)
        {
            currentEnemies = new List<GameObject>();
            start = false;
        }


        timePassed += Time.deltaTime;

        if (!doneSpawning)
        {
            if (timePassed > timeBetweenSpawns)
            {
                SpawnWave();
                wave++;
                maxEnemies += enemyIncreasePerWave;
                timePassed = 0.0f;

                if (wave >= maxWaves)
                    doneSpawning = true;
            }

            //Debug.Log("Spawn enemies: " + spawnEnemies.ToString());

            if (Counter.roomEnemies <= 0 && !fightEndMusicPlayed && MusicSourceLocate.instance != null)
            {
                Audio.SetSwitch(MusicSourceLocate.instance.gameObject, "Player_Action", "Exploring");
            }
        }
    }

    GameObject SpawnPrefab(Vector3 position)
    {
       libraryPath = GenerateLibraryPath(RandomizePrefabs());

        GameObject enemy = InternalCalls.CreatePrefab(libraryPath, position, Quaternion.identity, Vector3.one);
        Counter.roomEnemies++;

        if (enemy != null)
        {
            currentEnemies.Add(enemy);

            Enemy enemyScript = enemy.GetComponent<StormTrooper2>();

            if (enemyScript != null)
                enemyScript.dieCallBack = this;
        }

        return enemy;
    }

    private GameObject IntToGameObject(int index)
    {
        GameObject spawnPoint = null;

        switch (index)
        {
            case 0:
                spawnPoint = spawnPoint0;
                break;
            case 1:
                spawnPoint = spawnPoint1;
                break;
            case 2:
                spawnPoint = spawnPoint2;
                break;
            case 3:
                spawnPoint = spawnPoint3;
                break;
            case 4:
                spawnPoint = spawnPoint4;
                break;
            case 5:
                spawnPoint = spawnPoint5;
                break;
            default:
                spawnPoint = spawnPoint0;
                break;
        }

        return spawnPoint;
    }

    private void SpawnWave()
    {
        if (wave == 0 && MusicSourceLocate.instance != null)
        {
            Audio.SetSwitch(MusicSourceLocate.instance.gameObject, "Player_Action", "Combat");
            Audio.SetSwitch(MusicSourceLocate.instance.gameObject, "Player_Health", "Healthy");
        }


        int[] spawnPoints = new int[maxSpawnPoints];
        int spawnEnemies = 0;

        for (int i = 0; i < maxSpawnPoints; i++)
        {
            spawnPoints[i] = 0;
        }

        for (int j = 0; j < maxEnemies; j++)
        {
            Random randomizer = new Random();

            for (int tries = 0; tries < 15; tries++)  //15 is max tries to not overwhelm the engine
            {
                int randomIndex = randomizer.Next(maxSpawnPoints);

                if (spawnPoints[randomIndex] == 0)
                {
                    GameObject spawnPoint = IntToGameObject(randomIndex);

                    if (spawnPoint == null)
                        Debug.Log("Null spawn");

                    GameObject enemy = SpawnPrefab(spawnPoint.transform.globalPosition);
                    spawnPoints[randomIndex] = 1;
                    spawnEnemies++;

                    if (enemy != null)
                    {
                        Enemy enemyScript = enemy.GetComponent<StormTrooper>();

                        if (enemyScript != null)
                            enemyScript.turretMode = TurretSpawnPoint(randomIndex);
                    }

                    break;
                }
            }
        }
    }

    private bool TurretSpawnPoint(int index)
    {
        bool turret = false;

        switch (index)
        {
            case 0:
                turret = turretSpawnPoint0;
                break;
            case 1:
                turret = turretSpawnPoint1;
                break;
            case 2:
                turret = turretSpawnPoint2;
                break;
            case 3:
                turret = turretSpawnPoint3;
                break;
            case 4:
                turret = turretSpawnPoint4;
                break;
            case 5:
                turret = turretSpawnPoint5;
                break;
            default:
                turret = false;
                break;
        }

        return turret;
    }

    private int RandomizePrefabs()
    {
        //First we check that we have both prefabs to randomize, if not we return the other if it has been set
        if (prefab_1_UID == 0)
        {
            if (prefab_2_UID != 0)
                return prefab_2_UID;
        }
        else if (prefab_2_UID == 0)
        {
            if (prefab_1_UID != 0)
                return prefab_1_UID;
        }
        else
        {
            Random randomizer = new Random();

            int index = randomizer.Next(2);

            if (index == 0)
                return prefab_1_UID;
            else
                return prefab_2_UID;
        }

        return 0;
    }

    private string GenerateLibraryPath(int uid)
    {
        if (uid == 0)
            return "Library/Prefabs/489054570.prefab";
        else
            return "Library/Prefabs/" + uid.ToString() + ".prefab";
    }
}
using System;
using DiamondEngine;
using System.Collections.Generic;

public class SpawnController : DiamondComponent
{
    //NPC Spawn Controller
    public int maxSpawnPoints = 0;
    public int maxNPCs = 0;

    public GameObject spawnPoint1 = null;
    public GameObject spawnPoint2 = null;
    public GameObject spawnPoint3 = null;
    public GameObject spawnPoint4 = null;
    public GameObject spawnPoint5 = null;
    public List<Vector3> spawnPoints = null; //maybe Vector3 array? We only need to store positions
    //public Vector3[] spawnPointsVec;
    public List<GameObject> currentEnemies = null;

    bool start = true;
    public void Update()
	{
        //TODO: Move to Start function
        if(start)
        {
            GenerateSpawnPointsList();
            maxNPCs = 5;
            maxSpawnPoints = 5;
            SpawnNPCs(maxNPCs);            
            start = false;
        }        
	}

    private void SpawnNPCs(int maxAmount)
    {
        //Get coordinates from a random spawnPoint
        //Generate a prefab on those coordinates
        for (int i = 0; i < maxAmount; i++)
        {
            Random randomizer = new Random();
            int randomIndex = randomizer.Next(maxSpawnPoints);
            Debug.Log("RandomNum:" + randomIndex);
            SpawnUnit("Library/Prefabs/2028292522.prefab", GetCoordinatesFromSpawnPoint(randomIndex));
        }        
    }
    private void SpawnUnit(string prefab, Vector3 pos)
    {
        InternalCalls.CreatePrefab(prefab, pos, Quaternion.identity, Vector3.one);
    }

    private Vector3 GetCoordinatesFromSpawnPoint(int index)
    {
        switch (index)
        {
            case 0:
                return spawnPoint1.transform.globalPosition;
            case 1:
                return spawnPoint2.transform.globalPosition;
            case 2:
                return spawnPoint3.transform.globalPosition;
            case 3:
                return spawnPoint4.transform.globalPosition;
            case 4:
                return spawnPoint5.transform.globalPosition;
            default:
                return spawnPoint1.transform.globalPosition;
        }
        /*if (spawnPoints[index] != null)
            return spawnPoints[index];
        
        Vector3 defaultVal = new Vector3(0,0,0);
        return defaultVal;*/
    }

    private void GenerateSpawnPointsList()
    {
        //Load spawnPoints position into a List or other data structure to handle information more efficiently
    }
}
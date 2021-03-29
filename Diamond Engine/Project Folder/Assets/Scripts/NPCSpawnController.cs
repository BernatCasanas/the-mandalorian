using System;
using DiamondEngine;
using System.Collections.Generic;

public class NPCSpawnController : DiamondComponent
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
        if (start)
        {
            GenerateSpawnPointsList();
            maxNPCs = 2;
            maxSpawnPoints = 5;
            //SpawnNPCs(maxNPCs);            
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

            //SpawnUnit("Library/Prefabs/489054570.prefab", GetCoordinatesFromSpawnPoint(randomIndex));
        }
    }
    private void SpawnUnit(string prefab, Vector3 pos)
    {
        InternalCalls.CreatePrefab(prefab, pos, Quaternion.identity, Vector3.one);
    }

    private Vector3 GetCoordinatesFromSpawnPoint(int index)
    {
        if (spawnPoints[index] != null)
            return spawnPoints[index];

        Vector3 defaultVal = new Vector3(0, 0, 0);
        return defaultVal;
    }

    private void GenerateSpawnPointsList()
    {
        /*Vector3 testPos = spawnPoint1;
        Debug.Log("PositionX:" + testPos.x);
        Debug.Log("PositionY:" + testPos.y);
        Debug.Log("PositionZ:" + testPos.z);
        spawnPointsVec = new Vector3[maxSpawnPoints];*/

        //spawnPoints[0] = spawnPoint1.transform.globalPosition;
        //if (testPos != null) spawnPoints.Add(testPos);
        //if (spawnPoint1 != null) spawnPoints.Add(spawnPoint1.transform.globalPosition);

        /*if (spawnPoint2 != null)spawnPoints.SetValue(spawnPoint2, index); index++;
        if (spawnPoint3 != null)spawnPoints.SetValue(spawnPoint3, index); index++;
        if (spawnPoint4 != null)spawnPoints.SetValue(spawnPoint4, index); index++;
        if (spawnPoint5 != null)spawnPoints.SetValue(spawnPoint5, index); index++;*/
    }
}
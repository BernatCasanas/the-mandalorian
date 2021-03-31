using System;
using DiamondEngine;
using System.Collections.Generic;

public class NPCSpawnController : DiamondComponent
{
    //NPC Spawn Controller
    private Dictionary<int, int> alreadyAppeared = new Dictionary<int, int>(); //<int,int> <number, timesAppeared>

    public bool Grogu = true;
    public bool BoKatan = true;
    public bool Ahsoka = true;
    private List<int> charactersUID = new List<int>();

    public GameObject spawnPoint1 = null;
    public GameObject spawnPoint2 = null;
    public GameObject spawnPoint3 = null;
    public GameObject spawnPoint4 = null;
    public GameObject spawnPoint5 = null;
    private List<Vector3> spawnPoints = new List<Vector3>();
 
    bool start = true;
    public void Update()
    {
        //TODO: Move to Start function
        if (start)
        {
            GenerateCharactersList();
            GenerateSpawnPointsList();
            SpawnNPCs();
            start = false;
        }
    }

    private void SpawnNPCs()
    {
        //Get coordinates from a random spawnPoint
        //Generate a prefab on those coordinates

        if(charactersUID.Count > spawnPoints.Count)
        {
            Debug.Log("Error. There are more NPCs than spawn points available");
            return;
        }

        for (int i = 0; i < spawnPoints.Count; i++)//Initialize map
        {
            alreadyAppeared[i] = 0;
        }

        for (int i = 0; i < charactersUID.Count; i++)
        {
            Random randomizer = new Random();
            int randomIndex = randomizer.Next(spawnPoints.Count);

            do
            {
                randomIndex = randomizer.Next(spawnPoints.Count);
            } while (alreadyAppeared[randomIndex] > 0);
            alreadyAppeared[randomIndex]++;

            //We update the coordinates of the already existing character
            string prefabPath = "Library/Prefabs/";
            prefabPath += charactersUID[i];
            prefabPath += ".prefab";
            SpawnUnit(prefabPath, GetCoordinatesFromSpawnPoint(randomIndex));
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
        
        Vector3 defaultVal = new Vector3(0,1,0);
        return defaultVal;
    }

    private void GenerateSpawnPointsList()
    {
        //Load spawnPoints position into a List or other data structure to handle information more efficiently
        if(spawnPoint1 != null) spawnPoints.Add(spawnPoint1.transform.globalPosition);
        if(spawnPoint2 != null) spawnPoints.Add(spawnPoint2.transform.globalPosition);
        if(spawnPoint3 != null) spawnPoints.Add(spawnPoint3.transform.globalPosition);
        if(spawnPoint4 != null) spawnPoints.Add(spawnPoint4.transform.globalPosition);
        if(spawnPoint5 != null) spawnPoints.Add(spawnPoint5.transform.globalPosition);
    }

    private void GenerateCharactersList()
    {
        if (Grogu) charactersUID.Add(1453817131); //Grogu's UID
        if (BoKatan) charactersUID.Add(1346158143); //BoKatan's UID
        if (Ahsoka) charactersUID.Add(2028292522); //Ahsoka's UID
    }
}
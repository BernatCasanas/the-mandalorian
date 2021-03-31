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
    private List<Tuple<Vector3, Quaternion, int>> spawnPoints = new List<Tuple<Vector3, Quaternion, int>>();
 
    enum State
    {
        SIT = 0,
        IDLE,
        BAR
    }

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
            SpawnUnit(prefabPath, GetCoordinatesFromSpawnPoint(randomIndex), GetRotationFromSpawnPoint(randomIndex));
        }
    }
    private void SpawnUnit(string prefab, Vector3 pos, Quaternion rot)
    {
        InternalCalls.CreatePrefab(prefab, pos, rot, Vector3.one);
    }

    private Vector3 GetCoordinatesFromSpawnPoint(int index)
    {
        if (spawnPoints[index] != null)
            return spawnPoints[index].Item1;
        
        Vector3 defaultVal = new Vector3(0,1,0);
        return defaultVal;
    }

    private Quaternion GetRotationFromSpawnPoint(int index)
    {
        if (spawnPoints[index] != null)
            return spawnPoints[index].Item2;

        Quaternion defaultVal = new Quaternion(0, 0, 0, 1);
        return defaultVal;
    }

    private void GenerateSpawnPointsList()
    {
        //Load spawnPoints position into a List or other data structure to handle information more efficiently
        Vector3 positionAux = new Vector3(1,1,1);
        Quaternion rotationAux = new Quaternion(0,0,0,1);
        int stateAux = (int)State.IDLE;
        Tuple<Vector3, Quaternion, int> TupleAux = new Tuple<Vector3, Quaternion, int>(positionAux, rotationAux, 1);

        if (spawnPoint1 != null)
        {
            positionAux = spawnPoint1.transform.globalPosition;
            rotationAux = spawnPoint1.transform.globalRotation;
            stateAux = (int)State.IDLE;
            TupleAux = Tuple.Create(positionAux, rotationAux, stateAux);
            spawnPoints.Add(TupleAux); 
        }
        if (spawnPoint2 != null)
        {
            positionAux = spawnPoint2.transform.globalPosition;
            rotationAux = spawnPoint2.transform.globalRotation;
            stateAux = (int)State.IDLE;
            TupleAux = Tuple.Create(positionAux, rotationAux, stateAux);
            spawnPoints.Add(TupleAux);
        }
        if (spawnPoint3 != null)
        {
            positionAux = spawnPoint3.transform.globalPosition;
            rotationAux = spawnPoint3.transform.globalRotation;
            stateAux = (int)State.IDLE;
            TupleAux = Tuple.Create(positionAux, rotationAux, stateAux);
            spawnPoints.Add(TupleAux);
        }
        if (spawnPoint4 != null)
        {
            positionAux = spawnPoint4.transform.globalPosition;
            rotationAux = spawnPoint4.transform.globalRotation;
            stateAux = (int)State.IDLE;
            TupleAux = Tuple.Create(positionAux, rotationAux, stateAux);
            spawnPoints.Add(TupleAux);
        }
        if (spawnPoint5 != null)
        {
            positionAux = spawnPoint5.transform.globalPosition;
            rotationAux = spawnPoint5.transform.globalRotation;
            stateAux = (int)State.IDLE;
            TupleAux = Tuple.Create(positionAux, rotationAux, stateAux);
            spawnPoints.Add(TupleAux);
        }
    }

    private void GenerateCharactersList()
    {
        if (Grogu) charactersUID.Add(1453817131); //Grogu's UID
        if (BoKatan) charactersUID.Add(1346158143); //BoKatan's UID
        if (Ahsoka) charactersUID.Add(2028292522); //Ahsoka's UID
    }
}
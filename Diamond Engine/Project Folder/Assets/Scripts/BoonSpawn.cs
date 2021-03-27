using System;
using DiamondEngine;
using System.Collections.Generic;




public class BoonSpawn : DiamondComponent
{
    struct BoonWeight
    {
        public string boonLibID;
        public float weight; //weight to take into account for the random pick
    }

    public GameObject boonSpawnPosGO;
    public int boonsToSpawn = 1;
    Vector3 boonScale = new Vector3(1.0f, 1.0f, 1.0f);
    Vector3 spawnPos = new Vector3(0.0f, 0.0f, 0.0f);
    Vector3 spawnDir = new Vector3(0.0f, 0.0f, 0.0f);
    public float separation = 2.0f;
    bool firstFrame = true;
    List<GameObject> instancedPrefabs = new List<GameObject>();
    List<BoonWeight> myRandomPool = new List<BoonWeight>();

    float totalweight = 0.0f;

    public void Update()
    {
        if (firstFrame)
        {
            firstFrame = false;
            totalweight = 0.0f;

            if (boonSpawnPosGO != null)
            {
                spawnPos = new Vector3(boonSpawnPosGO.transform.localPosition.x, boonSpawnPosGO.transform.localPosition.y, boonSpawnPosGO.transform.localPosition.z);
                spawnDir = boonSpawnPosGO.transform.GetRight();
                spawnDir = spawnDir * separation;
            }

            CreateAllBoonProbabilities();

           // SpawnBoons();//this method must be called from the place where the level is considered ended not from here
        }

    }

    //USE THIS WHEN THE LEVEL IS FINISHED
    //Spawns the number of boons requested with no repeating ones 
    public void SpawnBoons()
    {
        int newBoonsToSpawn = boonsToSpawn;

        if (newBoonsToSpawn > myRandomPool.Count) //this ensures that we do not end in an infinite bucle if we need to spawn more boons han random boons exist
        {
            newBoonsToSpawn = myRandomPool.Count;
        }

        List<int> requestedBoonList = new List<int>();

        for (int i = 0; i < newBoonsToSpawn; ++i)
        {
            int newBoonIndex = RequestRandomBoon();

            if (requestedBoonList.Contains(newBoonIndex))//This ensures that no index is repeated
            {
                --i;
            }
            else
            {
                requestedBoonList.Add(newBoonIndex);
            }

        }

        for (int i = 0; i < requestedBoonList.Count; ++i)
        {

            instancedPrefabs.Add(InternalCalls.CreatePrefab(PrefabPathFromID(myRandomPool[requestedBoonList[i]].boonLibID), spawnPos, boonSpawnPosGO.transform.globalRotation, boonScale));
            spawnPos += spawnDir;
        }

        requestedBoonList.Clear();

    }

    string PrefabPathFromID(string prefabID)
    {
        string ret = "Library/Prefabs/";
        ret = ret + prefabID + ".prefab";
        return ret;
    }

    public void DestroyAllCreatedBoons()
    {
        for (int i = instancedPrefabs.Count - 1; i >= 0; --i)
        {
            InternalCalls.Destroy(instancedPrefabs[i]);
            //instancedPrefabs.RemoveAt(i);
        }
        instancedPrefabs.Clear();

        //TODO add timer here ?????
        if (!Counter.isFinalScene)
            RoomSwitch.SwitchRooms();
        else
        {
            Counter.gameResult = Counter.GameResult.VICTORY;
            SceneManager.LoadScene(821370213);
        }
    }


    //returns the index of the boonWeight that has been chosen, returns -1 if the pool is empty
    public int RequestRandomBoon()
    {
        Random r = new Random();
        float randomPick = (float)(r.NextDouble() * totalweight);

        for (int i = 0; i < myRandomPool.Count; i++)
        {
            if (randomPick < myRandomPool[i].weight)
            {
                return i - 1;
            }
        }
        return (myRandomPool.Count - 1);
    }

    //Harcoded boons with their probabilities TODO consider exposing a dynamic list on the editor if we have any
    //TODO this has to be hardcoded for now (we do not have support for arrays of prefabs nor prefab drop on script editor)
    void CreateAllBoonProbabilities()
    {
        AddBoonToPool("1993886225", 1.0f);
        AddBoonToPool("1714074184", 1.0f);
    }
    
    void AddBoonToPool(string boonLibID, float weight)
    {
        BoonWeight newWeight;
        newWeight.boonLibID = boonLibID;
        newWeight.weight = totalweight;
        totalweight += weight;

        myRandomPool.Add(newWeight);
    }

}
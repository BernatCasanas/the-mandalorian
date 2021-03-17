using System;
using DiamondEngine;

public class BoonSpawn : DiamondComponent
{
    public GameObject boonSpawnPosGO;
    public Vector3 boonScale = new Vector3(1.0f, 1.0f, 1.0f);
    public float separation = 2.0f;
    bool firstFrame = true;
    public void Update()
    {
        if (firstFrame)
        {
            firstFrame = false;
            SpawnBoons();

        }
    }


    public void SpawnBoons()
    {
        //TODO this has to be hardcoded for now (we do not have support for arrays of prefabs nor prefab drop on script editor)

        if (boonSpawnPosGO != null)
        {
            Vector3 pos = boonSpawnPosGO.transform.globalPosition;
            Vector3 spawnDir = boonSpawnPosGO.transform.GetRight();
            spawnDir = spawnDir * separation;
            Vector3 spawnPos = pos - spawnDir;
            Debug.Log("Left SpawnPos:" + spawnPos.ToString());
            InternalCalls.CreatePrefab("Library/Prefabs/456478384.prefab", spawnPos, boonSpawnPosGO.transform.globalRotation, boonScale);
            Debug.Log("Center SpawnPos:" + pos.ToString());
            InternalCalls.CreatePrefab("Library/Prefabs/977900791.prefab", pos, boonSpawnPosGO.GetComponent<Transform>().globalRotation, boonScale);
            spawnPos = pos + spawnDir;
            Debug.Log("Right SpawnPos:" + spawnPos.ToString());
            InternalCalls.CreatePrefab("Library/Prefabs/1833518684.prefab", spawnPos, boonSpawnPosGO.GetComponent<Transform>().globalRotation, boonScale);
        }

    }

}
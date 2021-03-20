using System;
using DiamondEngine;

public class Spawn : DiamondComponent
{
	bool spawned = false;

	public GameObject spawnPoint0 = null;
	public GameObject spawnPoint1 = null;
	public GameObject spawnPoint2 = null;
	public GameObject spawnPoint3 = null;
	public GameObject spawnPoint4 = null;
	public GameObject spawnPoint5 = null;
	public GameObject spawnPoint6 = null;

	int spawnPointAmount = 0;
	//private int[] spawnedPoints;


	public void Update()
	{
		if(!spawned)
        {
			//spawnedPoints = new int[spawnPointAmount];
			if(spawnPoint0 != null)
            {
				SpawnPrefab(spawnPoint0.transform.globalPosition);
            }

			if (spawnPoint1 != null)
			{
				SpawnPrefab(spawnPoint1.transform.globalPosition);
			}

			if (spawnPoint2 != null)
			{
				SpawnPrefab(spawnPoint2.transform.globalPosition);
			}

			if (spawnPoint3 != null)
			{
				SpawnPrefab(spawnPoint3.transform.globalPosition);
			}

			if (spawnPoint4 != null)
			{
				SpawnPrefab(spawnPoint4.transform.globalPosition);
			}

			if (spawnPoint5 != null)
			{
				SpawnPrefab(spawnPoint5.transform.globalPosition);
			}

			if (spawnPoint6 != null)
			{
				SpawnPrefab(spawnPoint6.transform.globalPosition);
			}

			spawned = true;
        }
	}

	private void SpawnPrefab(Vector3 position)
	{
		InternalCalls.CreatePrefab("Library/Prefabs/44509991.prefab", position, Quaternion.identity, Vector3.one);
	}
}
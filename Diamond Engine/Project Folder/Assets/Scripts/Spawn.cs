using System;
using DiamondEngine;

public class Spawn : DiamondComponent
{
	bool doneSpawning = false;

	public GameObject spawnPoint0 = null;
	public GameObject spawnPoint1 = null;
	public GameObject spawnPoint2 = null;
	public GameObject spawnPoint3 = null;
	public GameObject spawnPoint4 = null;
	public GameObject spawnPoint5 = null;
	public GameObject spawnPoint6 = null;

	//int spawnPointAmount = 0;
	//private int[] spawnedPoints;
	int wave = 0;

	float timePassed = 0.0f;
	float timeBetweenSpawns = 5.0f;

	public void Update()
	{
		timePassed += Time.deltaTime;

		//please don't judge this code, I hope it disappears soon
		if(timePassed > timeBetweenSpawns)
        {
			if (!doneSpawning)
			{
				Debug.Log("Spawning wave: " + wave.ToString());
				if (wave == 0)
                {
					if (spawnPoint0 != null)
					{
						SpawnPrefab(spawnPoint0.transform.globalPosition);
						Counter.roomEnemies++;
					}

					if (spawnPoint1 != null)
					{
						SpawnPrefab(spawnPoint1.transform.globalPosition);
						Counter.roomEnemies++;
					}

					if (spawnPoint2 != null)
					{
						SpawnPrefab(spawnPoint2.transform.globalPosition);
						Counter.roomEnemies++;
					}
					timePassed = 0.0f;
				}
				else if(wave == 1)
                {
					if (spawnPoint3 != null)
					{
						SpawnPrefab(spawnPoint3.transform.globalPosition);
						Counter.roomEnemies++;
					}

					if (spawnPoint4 != null)
					{
						SpawnPrefab(spawnPoint4.transform.globalPosition);
						Counter.roomEnemies++;
					}

					timePassed = 0.0f;
				}
				else
                {
					if (spawnPoint5 != null)
					{
						SpawnPrefab(spawnPoint5.transform.globalPosition);
						Counter.roomEnemies++;
					}

					if (spawnPoint6 != null)
					{
						SpawnPrefab(spawnPoint6.transform.globalPosition);
						Counter.roomEnemies++;
					}

					timePassed = 0.0f;
					doneSpawning = true;
				}

				wave++;
			}
		}
	}

	private void SpawnPrefab(Vector3 position)
	{
		InternalCalls.CreatePrefab("Library/Prefabs/44509991.prefab", position, Quaternion.identity, Vector3.one);
	}
}
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

	public int maxSpawnPoints = 0;
	public int maxEnemies = 0;
	public int enemyIncreasePerWave = 0;

	public int wave = 0;
	public int maxWaves = 1;

	public float timePassed = 0.0f;
	public float timeBetweenSpawns = 8.0f;

	bool fightEndMusicPlayed = false;
	public void Update()
	{
		timePassed += Time.deltaTime;
		if(!doneSpawning)
        {
			if(timePassed > timeBetweenSpawns)
            {
				SpawnWave();
				wave++;
				maxEnemies += enemyIncreasePerWave;

				if (wave >= maxWaves)
					doneSpawning = true;
				
			}

			//Debug.Log("Spawn enemies: " + spawnEnemies.ToString());
		
			if(Counter.roomEnemies <= 0 && !fightEndMusicPlayed)
            {
				Audio.SetSwitch(MusicSourceLocate.instance.gameObject, "Player_Action", "Exploring");
			}
		}
	}

	private void SpawnPrefab(Vector3 position)
	{
		InternalCalls.CreatePrefab("Library/Prefabs/44509991.prefab", position, Quaternion.identity, Vector3.one);
	}

	private GameObject IntToGameObject(int index)
    {
		GameObject spawnPoint = null;

		switch(index)
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
				break;
        }

		return spawnPoint;
    }

	void SpawnWave()
    {
		if(wave == 0)
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
					SpawnPrefab(spawnPoint.transform.globalPosition);
					spawnPoints[randomIndex] = 1;
					spawnEnemies++;
					break;
				}
			}
		}
	}
}


/*
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
 */
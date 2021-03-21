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

	public bool turretSpawnPoint0 = false;
	public bool turretSpawnPoint1 = false;
	public bool turretSpawnPoint2 = false;
	public bool turretSpawnPoint3 = false;
	public bool turretSpawnPoint4 = false;
	public bool turretSpawnPoint5 = false;

	public void Update()
	{
		timePassed += Time.deltaTime;

		if(!doneSpawning)
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
		
			if(Counter.roomEnemies <= 0 && !fightEndMusicPlayed)
            {
				//Audio.SetSwitch(MusicSourceLocate.instance.gameObject, "Player_Action", "Exploring");
			}
		}
	}

	GameObject SpawnPrefab(Vector3 position)
	{
		GameObject enemy = InternalCalls.CreatePrefab("Library/Prefabs/489054570.prefab", position, Quaternion.identity, Vector3.one);
		Counter.roomEnemies++;
		return enemy;
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
				spawnPoint = spawnPoint0;
				break;
        }

		return spawnPoint;
    }

	private void SpawnWave()
    {
		
		if (wave == 0 && MusicSourceLocate.instance != null)
        {
			//Audio.SetSwitch(MusicSourceLocate.instance.gameObject, "Player_Action", "Combat");
			//Audio.SetSwitch(MusicSourceLocate.instance.gameObject, "Player_Health", "Healthy");
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

					if(enemy != null)
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
}
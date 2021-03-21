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
				Audio.SetSwitch(MusicSourceLocate.instance.gameObject, "Player_Action", "Exploring");
			}
		}
	}

	private void SpawnPrefab(Vector3 position)
	{
		InternalCalls.CreatePrefab("Library/Prefabs/489054570.prefab", position, Quaternion.identity, Vector3.one);
		Counter.roomEnemies++;
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

					if (spawnPoint == null)
						Debug.Log("Null spawn");

					SpawnPrefab(spawnPoint.transform.globalPosition);
					spawnPoints[randomIndex] = 1;
					spawnEnemies++;
					break;
				}
			}
		}
	}
}
using System;
using DiamondEngine;

public class TutoDestroyEnemies : DiamondComponent
{

	private bool cleared = false;
	public GameObject spawnPoint;
	public GameObject SpawnManager;
	public void Update()
	{
		
	}

	public void OnTriggerEnter(GameObject triggeredGameObject)
	{
		if (triggeredGameObject.CompareTag("Player") && !cleared)
		{
			SpawnManager.EnableNav(true);
			EnemyManager.ClearList();
			cleared = true;
			Core.instance.SetMySpawnPosition(spawnPoint.transform.globalPosition);
		}
	}
}
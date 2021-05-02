using System;
using DiamondEngine;

public class SpawnPoint : DiamondComponent
{
    public void Awake()
    {
		EnemyManager.AddSpawnPoint(gameObject);
    }
}
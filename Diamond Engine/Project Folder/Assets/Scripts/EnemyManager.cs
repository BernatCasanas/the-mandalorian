using System;
using DiamondEngine;
using System.Collections.Generic;

public class EnemyManager : DiamondComponent
{
	public static List<GameObject> currentEnemies = null;

	public static void AddEnemy(GameObject enemy)
    {
		if (currentEnemies == null)
			currentEnemies = new List<GameObject>();

		currentEnemies.Add(enemy);
	}

	public static bool RemoveEnemy(GameObject enemy)
    {
		if (currentEnemies == null)
			return false;

        foreach (GameObject enemyInList in currentEnemies)
        {
			if(enemyInList == enemy)
            {
				currentEnemies.Remove(enemy);
				return true;
            }
        }

		return false;
    }

	public static void ClearList()
    {
		currentEnemies.Clear();
		currentEnemies = null;
    }
}
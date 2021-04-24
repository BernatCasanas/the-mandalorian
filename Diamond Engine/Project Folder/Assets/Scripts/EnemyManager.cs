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
        if (currentEnemies == null || enemy == null)
        {
            Debug.Log("Current enemy list is null!!!");
            return false;
        }

        foreach (GameObject enemyInList in currentEnemies)
        {
            if (enemyInList.GetUid() == enemy.GetUid())
            {
                currentEnemies.Remove(enemy);
                return true;
            }
        }

        return false;
    }

    public static void ClearList()
    {
        if (currentEnemies != null)
        {
            currentEnemies.Clear();
            currentEnemies = null;
        }
    }
}
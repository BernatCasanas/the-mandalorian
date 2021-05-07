using System;
using DiamondEngine;
using System.Collections.Generic;

public class EnemyManager : DiamondComponent
{
    public static List<GameObject> currentEnemies = null;

    public static int awaitingForEnemiesToSpawn = 0;

    public static void AddEnemy(GameObject enemy)
    {
        if (currentEnemies == null)
            currentEnemies = new List<GameObject>();

        currentEnemies.Add(enemy);
    }

    public static bool RemoveEnemy(GameObject enemy)
    {
        bool ret = false;

        if (enemy == null)
            return false;

        if (currentEnemies == null)
        {
            Debug.Log("Current enemy list is null!!!");
            return false;
        }

        //Debug.Log("Removing enemy. Enemies left: " + currentEnemies.Count.ToString());

        for (int i = 0; i < currentEnemies.Count; ++i)
        {
            if (currentEnemies[i].GetUid() == enemy.GetUid())
            {
                currentEnemies.RemoveAt(i);
                ret = true;
            }
        }

        if (ret == false)
            Debug.Log("Enemy to remove not found in enmies list");

        //Debug.Log("Enemies left: " + currentEnemies.Count.ToString());

        bool enemiesLeftToSpawn = true;

        if(SpawnManager.instance != null)
        {
            enemiesLeftToSpawn = SpawnManager.instance.AreEnemiesLeftToSpawn();
        }

        if (currentEnemies.Count == 0  && awaitingForEnemiesToSpawn == 0 && enemiesLeftToSpawn == false && GameSceneManager.instance != null )
        {
            Debug.Log("Enemy Manager calling to Level End");
            GameSceneManager.instance.LevelEnd();
        }

        return ret;
    }

    public static void ClearList()
    {
        if (currentEnemies != null)
        {
            currentEnemies.Clear();
            currentEnemies = null;
        }
        awaitingForEnemiesToSpawn = 0;
    }

    public static int EnemiesLeft()
    {
        if (currentEnemies != null)
            return currentEnemies.Count;
        else
            return 0;
    }
}
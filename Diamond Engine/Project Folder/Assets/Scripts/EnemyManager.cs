using System;
using DiamondEngine;
using System.Collections.Generic;

public class EnemyManager : DiamondComponent
{
    public static List<GameObject> currentEnemies = null;

    public static List<GameObject> availableSpawnPoints = null;

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

        Debug.Log("Removing enemy. Enemies left: " + currentEnemies.Count.ToString());

        for(int i = 0; i < currentEnemies.Count; ++i)
        {
            if(currentEnemies[i].GetUid() == enemy.GetUid())
            {
                currentEnemies.RemoveAt(i);
                ret = true;
            }
        }

        if (ret == false)
            Debug.Log("Enemy not found");

        Debug.Log("Enemies left: " + currentEnemies.Count.ToString());

        if (currentEnemies.Count == 0 && GameSceneManager.instance != null)
            GameSceneManager.instance.LevelEnd();

        return ret;
    }

    public static void ClearList()
    {
        if (currentEnemies != null)
        {
            currentEnemies.Clear();
            currentEnemies = null;
        }
    }

    public static int EnemiesLeft()
    {
        if (currentEnemies != null)
            return currentEnemies.Count;
        else
            return 0;
    }

    public static void AddSpawnPoint(GameObject spawnPoint)
    {
        availableSpawnPoints.Add(spawnPoint);
    }

    public static void RemoveSpawnPoint(GameObject spawnPoint)
    {
        for (int i = 0; i < availableSpawnPoints.Count; i++)
        {
            if (spawnPoint.GetUid() == availableSpawnPoints[i].GetUid())
            {
                availableSpawnPoints.Remove(spawnPoint);
                return;
            }
        }
    }

    public static void ClearSpawnPointsList()
    {
        availableSpawnPoints.Clear();
    }
}
using System;
using DiamondEngine;

public class GameSceneManager : DiamondComponent
{

    bool enemiesHaveSpawned = false;
    EndLevelRewards rewardMenu = new EndLevelRewards();

    // THIS IS A VERY INCOMPLETE MODULE THAT IS CURRENTLY NOT MANAGING SCENE. HOWEVER, WE HAVE TO DO THAT IN A CLOSED PLACED. AND SOMEONE HAS TO START IT. YEET TIME :3
    public void Update()
    {

        if (Counter.roomEnemies > 0)    // This would, ideally, not be necessary, and enemies generating would have nothing to do with dialogue ocurring, since it would (presumably) stop the game
        {
            enemiesHaveSpawned = true;
        }

        else if (enemiesHaveSpawned && Counter.roomEnemies <= 0)    // Right now, when a room begins, enemy counter = 0
        {

        }

        // We should have a single reward prefab, and we only change its texture, which is the ID we store

    }

}
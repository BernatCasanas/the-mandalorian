using System;
using DiamondEngine;

public class GameSceneManager : DiamondComponent
{

    bool enemiesHaveSpawned = false;
    GameObject rewardObject = null;
    EndLevelRewardSpawn rewardSpawnComponent = null;
    EndLevelRewards rewardMenu = new EndLevelRewards();
    GameResources rewardData = null;
    Vector3 rewardInitialPos = new Vector3(0.0f, 0.0f, 0.0f);

    // THIS IS A VERY INCOMPLETE MODULE THAT IS CURRENTLY NOT MANAGING SCENE. HOWEVER, WE HAVE TO DO THAT IN A CLOSED PLACED. AND SOMEONE HAS TO START IT. YEET TIME :3
    // THIS SCRIPT SHOULD NOT BE DELETED AND RELOADED CONSTANTLY, CODE IS THOUGHT TO BE IN AN OBJECT THAT TAKES CARE OF ALL SCENE MANAGING DURING ALL GAME DURATION

    public void Update()
    {

        if (Counter.roomEnemies > 0)    // This would, ideally, not be necessary, and enemies generating would have nothing to do with dialogue ocurring, since it would (presumably) stop the game
        {
            enemiesHaveSpawned = true;
            Counter.roomEnemies = 0;    // KILL THIS LINE, IT'S ONLY FOR DEBUG PURPOSES
        }

        else if (enemiesHaveSpawned && Counter.roomEnemies <= 0 && rewardData == null)    // Right now, when a room begins, enemy counter = 0
        {
            Time.PauseGame();
            rewardData = rewardMenu.GenerateRewardPipeline();

            if (rewardData != null)
            {
                if (rewardObject != null)
                {
                    InternalCalls.Destroy(rewardObject);    // I don't like this, but if I try to give it a new position once created, it doesn't work; only does when assigning position at constructor...
                }

                rewardInitialPos = InternalCalls.FindObjectWithName("DinDjarin").transform.globalPosition;    // Not this position, but for now it's fine;
                rewardObject = InternalCalls.CreatePrefab("Library/Prefabs/1394471616.prefab", new Vector3(rewardInitialPos.x, rewardInitialPos.y, rewardInitialPos.z), new Quaternion(0.0f, 0.0f, 0.0f, 1.0f), new Vector3(1.0f, 1.0f, 1.0f));
                rewardObject.AssignLibraryTextureToMaterial(rewardData.libraryTextureID, "diffuseTexture");
                rewardObject.Enable(true);
                rewardSpawnComponent = rewardObject.GetComponent<EndLevelRewardSpawn>();

                Time.ResumeGame();
            }
        }

        else if (rewardData != null && rewardObject != null)
        {
            rewardSpawnComponent.AdvanceVerticalMovement(rewardInitialPos);
            rewardSpawnComponent.AdvanceRotation();

            if (rewardSpawnComponent.trigger == true)
            {
                rewardObject.Enable(false);
            }

        }

        // Then the player touches the thing, we add whatever they won to the player object, we disable rewardObject and we change scene :D
        // All bools that regulate stuff should be put at false when changing scene

    }

}
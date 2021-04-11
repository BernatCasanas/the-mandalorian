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

    public void Awake()
    {
        rewardObject = InternalCalls.CreatePrefab("Library/Prefabs/1394471616.prefab", new Vector3(rewardInitialPos.x, rewardInitialPos.y, rewardInitialPos.z), new Quaternion(0.0f, 0.0f, 0.0f, 1.0f), new Vector3(1.0f, 1.0f, 1.0f));
        rewardObject.SetParent(gameObject);
        rewardObject.Enable(false);
        rewardSpawnComponent = rewardObject.GetComponent<EndLevelRewardSpawn>();
    }

    public void Update()
    {
        if (Counter.roomEnemies > 0)    // This would, ideally, not be necessary, and enemies generating would have nothing to do with dialogue ocurring, since it would (presumably) stop the game
        {
            enemiesHaveSpawned = true;
        }

        else if (enemiesHaveSpawned && Counter.roomEnemies <= 0 && rewardData == null && Counter.gameResult != Counter.GameResult.DEFEAT)    // Right now, when a room begins, enemy counter = 0
        {
            rewardData = rewardMenu.GenerateRewardPipeline();

            if (rewardData != null && rewardObject != null)
            {
                rewardInitialPos = Core.instance.gameObject.transform.globalPosition + new Vector3(1.5f, 0.0f, 0.0f);    // Not this position, but for now it's fine;
                rewardObject.transform.localPosition = rewardInitialPos;
                rewardObject.AssignLibraryTextureToMaterial(rewardData.libraryTextureID, "diffuseTexture");
                rewardObject.Enable(true);
            }
        }

        else if (rewardData != null && rewardObject != null)
        {
            rewardSpawnComponent.AdvanceVerticalMovement(rewardInitialPos);
            rewardSpawnComponent.AdvanceRotation();

            if (rewardSpawnComponent.trigger == true)
            {
                rewardData.Use();
                rewardMenu.selectedReward = null;
                rewardObject.Enable(false);
                enemiesHaveSpawned = false;
                rewardSpawnComponent.trigger = false;
                rewardData = null;

                if (!Counter.isFinalScene)
                    RoomSwitch.SwitchRooms();
                else
                {
                    Counter.gameResult = Counter.GameResult.VICTORY;
                    SceneManager.LoadScene(821370213);
                }
            }

        }

        // We should clean boons when ending a run :3
    }

}
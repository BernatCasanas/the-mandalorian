using System;
using DiamondEngine;

public class GameSceneManager : DiamondComponent
{
    GameObject rewardObject = null;
    EndLevelRewardSpawn rewardSpawnComponent = null;
    EndLevelRewards rewardMenu = new EndLevelRewards();
    GameResources rewardData = null;
    Vector3 rewardInitialPos = new Vector3(0.0f, 0.0f, 0.0f);

    // THIS IS A VERY INCOMPLETE MODULE THAT IS CURRENTLY NOT MANAGING SCENE. HOWEVER, WE HAVE TO DO THAT IN A CLOSED PLACED. AND SOMEONE HAS TO START IT. YEET TIME :3
    // THIS SCRIPT SHOULD NOT BE DELETED AND RELOADED CONSTANTLY, CODE IS THOUGHT TO BE IN AN OBJECT THAT TAKES CARE OF ALL SCENE MANAGING DURING ALL GAME DURATION

    public void Awake()
    {
        if (rewardMenu == null)
            rewardMenu = new EndLevelRewards();

        if (rewardInitialPos == null)
            rewardInitialPos = new Vector3(0.0f, 0.0f, 0.0f);

        rewardObject = InternalCalls.CreatePrefab("Library/Prefabs/1394471616.prefab", new Vector3(rewardInitialPos.x, rewardInitialPos.y, rewardInitialPos.z), new Quaternion(0.0f, 0.0f, 0.0f, 1.0f), new Vector3(1.0f, 1.0f, 1.0f));
      
        if(rewardObject != null)
        {
            rewardObject.SetParent(gameObject);
            rewardObject.Enable(false);
            rewardSpawnComponent = rewardObject.GetComponent<EndLevelRewardSpawn>();
        }
    }

    public void Update()
    {
        if (Counter.allEnemiesDead && rewardData == null)    // We need a scene manager :')
        {
            if (PlayerResources.CheckBoon(BOONS.BOON_BOUNTYHUNTERSKILLS))
            {
                PlayerResources.AddRunCoins(2);
            }

            if(rewardMenu != null)
                rewardData = rewardMenu.GenerateRewardPipeline();

            //Debug.Log("Hi");
            if(rewardData != null)
                Debug.Log("Reward data texture id is " + rewardData.libraryTextureID + " and reward type is " + rewardData.resourceType);

            if (rewardData != null && rewardObject != null)
            {
                Counter.allEnemiesDead = false;
                Counter.roomEnemies = 0;    // Safety check that could be done in a more understandable place if we had a scene manager...
                rewardInitialPos = Core.instance.gameObject.transform.globalPosition + new Vector3(1.5f, 0.0f, 0.0f);    // Not this position, but for now it's fine;
                rewardObject.transform.localPosition = rewardInitialPos;
                rewardObject.AssignLibraryTextureToMaterial(rewardData.libraryTextureID, "diffuseTexture");
                rewardObject.Enable(true);
            }
        }
        else if (rewardData != null && rewardObject != null && rewardSpawnComponent != null)
        {
            rewardSpawnComponent.AdvanceVerticalMovement(rewardInitialPos);
            rewardSpawnComponent.AdvanceRotation();

            ChangeScene();
        }

        if (DebugOptionsHolder.goToNextRoom == true)
        {
            Debug.Log("Change scene");
            ChangeScene();
            DebugOptionsHolder.goToNextRoom = false;
        }

        // We should clean boons when ending a run :3
        //PlayerResources.ResetRunBoons();
    }


    private void ChangeScene()
    {
        if (rewardData != null)
        {
            rewardData.Use();
            rewardMenu.selectedReward = null;
            if(rewardObject != null)
                rewardObject.Enable(false);

            if(rewardSpawnComponent != null)
                rewardSpawnComponent.trigger = false;
            rewardData = null;
        }

        if (!Counter.isFinalScene)
            RoomSwitch.SwitchRooms();
        else
        {
            Counter.gameResult = Counter.GameResult.VICTORY;
            SceneManager.LoadScene(821370213);
        }
    }
}
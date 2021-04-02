using System;
using DiamondEngine;

public class GameSceneManager : DiamondComponent
{

    bool enemiesHaveSpawned = false;
    GameObject rewardObject = null;
    EndLevelRewards rewardMenu = new EndLevelRewards();
    DiamondEngineResources.GameResources rewardData = null;

    public float verticalMovementAmount = 1.0f;
    public float rotSpeedDegSec = 1.0f;
    float rotationAngle = 0.0f;
    public float verticalSpeedMultiplier = 0.5f;
    Vector3 initialPos = null; //lowest position of the obj
    bool goingUp = true;
    float animTime = 0.0f;

    // THIS IS A VERY INCOMPLETE MODULE THAT IS CURRENTLY NOT MANAGING SCENE. HOWEVER, WE HAVE TO DO THAT IN A CLOSED PLACED. AND SOMEONE HAS TO START IT. YEET TIME :3
    // THIS SCRIPT SHOULD NOT BE DELETED AND RELOADED CONSTANTLY, CODE IS THOUGHT TO BE IN AN OBJECT THAT TAKES CARE OF ALL SCENE MANAGING DURING ALL GAME DURATION
    public void Update()
    {

        if (Counter.roomEnemies > 0)    // This would, ideally, not be necessary, and enemies generating would have nothing to do with dialogue ocurring, since it would (presumably) stop the game
        {
            enemiesHaveSpawned = true;
        }

        else if (enemiesHaveSpawned && Counter.roomEnemies <= 0 && rewardData == null)    // Right now, when a room begins, enemy counter = 0
        {
            Time.PauseGame();
            rewardData = rewardMenu.GenerateRewardPipeline();

            if (rewardData != null)
            {
                // Update the reward spawn point near the player, in a walkable and reachable tile
                // Update the rewardObject texture
                Time.ResumeGame();
            }
        }

        else if (rewardData != null)
        {
            // We should have a single reward prefab, and we only change its texture, which is the ID we store
            rewardObject.Enable(true);
            AdvanceVerticalMovement();
            AdvanceRotation();
        }

        // Then the player touches the thing, we add whatever they won to the player object, and we change scene :D
        // All bools that regulate stuff should be put at false when changing scene

    }

    /* This regulated boon collected by player on old BoonInstance
    public void OnTriggerEnter(GameObject collidedGameObject)
    {
        if (collidedGameObject != null && collidedGameObject.CompareTag("Player"))//TODO sort this by UID
        {
            //Debug.Log("COLLISION DETECTED WITH: " + collidedGameObject.name);
            if (boonSpawner != null && myBoon != null)
            {
                myBoon.Use();
                boonSpawner.DestroyAllCreatedBoons();
            }
        }

    }*/

    public void AdvanceVerticalMovement()
    {
        if (goingUp)
        {
            animTime += Time.deltaTime * verticalSpeedMultiplier;
        }
        
        else
        {
            animTime -= Time.deltaTime * verticalSpeedMultiplier;
        }

        if (animTime > 1.0f)
        {
            goingUp = false;
            animTime = 1.0f;
        }
        
        else if (animTime < 0.0f)
        {
            goingUp = true;
            animTime = 0.0f;

        }

        float yPos = ParametricBlend(animTime);
        Vector3 newPos = new Vector3(initialPos.x, initialPos.y, initialPos.z);
        newPos.y += yPos * verticalMovementAmount;
        rewardObject.transform.localPosition = newPos;

    }

    public void AdvanceRotation()
    {
        rotationAngle += rotSpeedDegSec * Time.deltaTime;
        Vector3 axis = new Vector3(0.0f, 1.0f, 0.0f);
        float myRotAngle = 0.0174533f * rotationAngle;
        rewardObject.transform.localRotation = Quaternion.RotateAroundAxis(axis, myRotAngle);

    }

    public float ParametricBlend(float t) => (t * t) / (2.0f * ((t * t) - t) + 1.0f);

}
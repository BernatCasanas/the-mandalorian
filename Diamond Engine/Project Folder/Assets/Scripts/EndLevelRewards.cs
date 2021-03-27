using System;
using DiamondEngine;

public class EndLevelRewards : DiamondComponent
{
    public enum EndLevelRewardType
    {
        REWARD_BOON,
        REWARD_BESKAR,
        REWARD_MACARON,
        REWARD_SCRAP,
        REWARD_MILK
    }

    public struct EndLevelReward
    {
        public EndLevelReward(int index, EndLevelRewardType rewardType)
        {
            boonIndex = index;
            type = rewardType;
        }

        public int boonIndex;
        public EndLevelRewardType type;
    }

    bool enemiesHaveSpawned = false;
    int[] rewardChances = new int[5] { 80, 5, 5, 5, 5 }; // Reward chances, by order being boons, beskar, macarons, scraps and milk
    BoonSpawn boonSpawner = new BoonSpawn();
    EndLevelReward firstReward;
    EndLevelReward secondReward;
    EndLevelReward thirdReward;
    GameObject rewardsMenu;

    public void OnExecuteButton()
    {
        if (gameObject.name == "1st reward")
        {

        }

        if (gameObject.name == "2nd reward")
        {

        }

        if (gameObject.name == "3rd reward")
        {

        }

    }

    public void Update()
    {

        if (Counter.roomEnemies > 0)
        {
            enemiesHaveSpawned = true;
        }

        if (gameObject.name == "EndLevelRewardMenu" && Counter.roomEnemies <= 0 && enemiesHaveSpawned)  // This probably will be called from somewhere else
        {
            Time.PauseGame();
            firstReward = SelectRewards();
            secondReward = SelectRewards();
            thirdReward = SelectRewards();
            // If index = -1, there is no boon
            rewardsMenu = InternalCalls.CreatePrefab("Library/Prefabs/.prefab", new Vector3(0.0f, 0.0f, 0.0f), new Quaternion(0.0f, 0.0f, 0.0f, 1.0f), new Vector3(1.0f, 1.0f, 1.0f));
        }

    }

    public EndLevelReward SelectRewards()
    {

        // Do logic about dynamic percentage change

        EndLevelReward newReward;
        Random random = new Random();
        int randomNum = random.Next(100);
        int chanceAcumulation = 0;

        for (int i = 0; i < 5; i++)
        {
            chanceAcumulation += rewardChances[i];

            if (randomNum <= chanceAcumulation)
            {

                switch (i)
                {
                    case 0:
                        newReward = new EndLevelReward(boonSpawner.RequestRandomBoon(), EndLevelRewardType.REWARD_BOON);
                        break;

                    case 1:
                        newReward = new EndLevelReward(-1, EndLevelRewardType.REWARD_BESKAR);
                        break;

                    case 2:
                        newReward = new EndLevelReward(-1, EndLevelRewardType.REWARD_MACARON);
                        break;

                    case 3:
                        newReward = new EndLevelReward(-1, EndLevelRewardType.REWARD_SCRAP);
                        break;

                    case 4:
                        newReward = new EndLevelReward(-1, EndLevelRewardType.REWARD_MILK);
                        break;

                    default:
                        newReward = new EndLevelReward(boonSpawner.RequestRandomBoon(), EndLevelRewardType.REWARD_BOON); // Spawn boon, just in case
                        break;
                }

                return newReward;

            }

        }

        newReward = new EndLevelReward(boonSpawner.RequestRandomBoon(), EndLevelRewardType.REWARD_BOON); // Spawn boon, just in case
        return newReward;   // We should always quit from the previous return, ignore this

    }

}
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

    //bool enemiesHaveSpawned = false;
    int[] rewardChances = new int[5] { 80, 5, 5, 5, 5 }; // Reward chances, by order being boons, beskar, macarons, scraps and milk
    BoonSpawn boonSpawner = new BoonSpawn();
    EndLevelReward firstReward;
    EndLevelReward secondReward;
    EndLevelReward thirdReward;
    GameObject rewardsMenu;

    public void Update()
    {

        if (Counter.roomEnemies > 0)
        {
            //enemiesHaveSpawned = true;
        }

//        if (gameObject.name == "EndLevelRewardMenu" && Counter.roomEnemies <= 0 && enemiesHaveSpawned)  // This probably will be called from somewhere else
        {
            Time.PauseGame();
            firstReward = SelectRewards();
            secondReward = SelectRewards();
            thirdReward = SelectRewards();
            // If index = -1, there is no boon

            rewardsMenu = CreatePopUpGameObject();
            // This instantiation works
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

    GameObject CreatePopUpGameObject()
    {
        // We can't make a whole prefab for this, because we would need to access gameObject's child to set the generated rewards, and we don't have that functionality

        InternalCalls.CreateGameObject("FirstRewardText", new Vector3(0.0f, 0.0f, 0.0f));
        InternalCalls.CreateGameObject("SecondRewardText", new Vector3(0.0f, 0.0f, 0.0f));
        InternalCalls.CreateGameObject("ThirdRewardText", new Vector3(0.0f, 0.0f, 0.0f));

        GameObject firstText = InternalCalls.FindObjectWithName("FirstRewardText");
        GameObject secondText = InternalCalls.FindObjectWithName("SecondRewardText");
        GameObject thirdText = InternalCalls.FindObjectWithName("ThirdRewardText");
        GameObject rewardMenu = InternalCalls.CreatePrefab("Library/Prefabs/18131542.prefab", new Vector3(0.0f, 0.0f, 0.0f), new Quaternion(0.0f, 0.0f, 0.0f, 1.0f), new Vector3(1.0f, 1.0f, 1.0f));

        GameObject canvas = InternalCalls.FindObjectWithName("Canvas");

        if (canvas == null) {
            canvas = InternalCalls.CreatePrefab("Library/Prefabs/1965121116.prefab", new Vector3(0.0f, 0.0f, 0.0f), new Quaternion(0.0f, 0.0f, 0.0f, 1.0f), new Vector3(1.0f, 1.0f, 1.0f));
        }

        firstText.SetParent(InternalCalls.FindObjectWithName("FirstRewardButton"));
        secondText.SetParent(InternalCalls.FindObjectWithName("SecondRewardButton"));
        thirdText.SetParent(InternalCalls.FindObjectWithName("ThirdRewardButton"));
        rewardMenu.SetParent(canvas);

        firstText.AddComponent(9);
        Text firstTextComponent = firstText.GetComponent<Text>();
        firstTextComponent.text = RewardText(firstReward);

        secondText.AddComponent(9);
        Text secondTextComponent = secondText.GetComponent<Text>();
        secondTextComponent.text = RewardText(secondReward);
        
        thirdText.AddComponent(9);
        Text thirdTextComponent = thirdText.GetComponent<Text>();
        thirdTextComponent.text = RewardText(thirdReward);
        
        return rewardMenu;

    }

    string RewardText(EndLevelReward reward)
    {
        string text = null;

        switch (reward.type)
        {
            case EndLevelRewardType.REWARD_BOON:

                break;

            case EndLevelRewardType.REWARD_BESKAR:
                text = "The metal of the mandalorian people, second to none in the galaxy.";
                break;

            case EndLevelRewardType.REWARD_MACARON:
                text = "Just a macaron. Grogu does love them, though.";
                break;

            case EndLevelRewardType.REWARD_SCRAP:
                text = "Remains of powerful imperial technology.";
                break;

            case EndLevelRewardType.REWARD_MILK:
                text = "Sweet and tasty blue milk, a true delicacy.";
                break;

        }

        return text;
    }

}
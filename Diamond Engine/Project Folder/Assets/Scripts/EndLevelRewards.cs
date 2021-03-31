using System;
using DiamondEngine;

public enum EndLevelRewardType
{
    REWARD_BOON,
    REWARD_BESKAR,
    REWARD_MACARON,
    REWARD_SCRAP,
    REWARD_MILK
}

public class EndLevelRewards : DiamondComponent
{
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

    int[] rewardChances = new int[5] { 80, 5, 5, 5, 5 }; // Reward chances, by order being boons, beskar, macarons, scraps and milk
    BoonSpawn boonSpawner = new BoonSpawn();
    EndLevelReward firstReward;
    EndLevelReward secondReward;
    EndLevelReward thirdReward;
    GameObject rewardsMenu;

    public void Update()
    {

        // This probably will be called from somewhere else; keep the gameObject.name because this script will be in the buttons
        //        if (gameObject.name == "EndLevelRewardMenu" && Counter.roomEnemies <= 0 && enemiesHaveSpawned)
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

        // Do logic about dynamic percentage change (need the player stats to know)

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
        GameObject rewardMenu = InternalCalls.CreatePrefab("Library/Prefabs/18131542.prefab", new Vector3(0.0f, 0.0f, 0.0f), new Quaternion(0.0f, 0.0f, 0.0f, 1.0f), new Vector3(1.0f, 1.0f, 1.0f));
        GameObject canvas = InternalCalls.FindObjectWithName("Canvas");

        GameObject firstText = InternalCalls.FindObjectWithName("FirstRewardText");
        GameObject secondText = InternalCalls.FindObjectWithName("SecondRewardText");
        GameObject thirdText = InternalCalls.FindObjectWithName("ThirdRewardText");

        GameObject firstImage = InternalCalls.FindObjectWithName("FirstRewardImage");
        GameObject secondImage = InternalCalls.FindObjectWithName("SecondRewardImage");
        GameObject thirdImage = InternalCalls.FindObjectWithName("ThirdRewardImage");


        if (canvas == null)
        {
            canvas = InternalCalls.CreatePrefab("Library/Prefabs/1965121116.prefab", new Vector3(0.0f, 0.0f, 0.0f), new Quaternion(0.0f, 0.0f, 0.0f, 1.0f), new Vector3(1.0f, 1.0f, 1.0f));
        }

        rewardMenu.SetParent(canvas);

        //        firstImage.GetComponent<Image2D>().AssignLibrary2DTexture(GetRewardPath(firstReward));    // Have the function re-entered or something... --> THIS MAY BE POINTLESS, BECAUSE WE ALREADY SAVE THE INFO AT GAMERESOURCE.CS
        //       secondImage.GetComponent<Image2D>().AssignLibrary2DTexture(GetRewardPath(secondReward));
        //      thirdImage.GetComponent<Image2D>().AssignLibrary2DTexture(GetRewardPath(thirdReward));

        firstText.GetComponent<Text>().text = RewardText(firstReward);
        secondText.GetComponent<Text>().text = RewardText(secondReward);
        thirdText.GetComponent<Text>().text = RewardText(thirdReward);

        return rewardMenu;

    }

    string RewardText(EndLevelReward reward)
    {
        string text = null;

        switch (reward.type)
        {
            case EndLevelRewardType.REWARD_BOON:
                text = "Undefined.";    // Stablish a function for this
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

    public int GetRewardPath(EndLevelReward reward)
    {

        int textureId = 0;

        switch (reward.type)
        {
            case EndLevelRewardType.REWARD_BOON:
                // Stablish a function for this
                break;

            case EndLevelRewardType.REWARD_BESKAR:  // When I can compile, and texture IDs are generated, put them properly
                textureId = 1083690418;
                break;

            case EndLevelRewardType.REWARD_MACARON:
                textureId = 222418542;
                break;

            case EndLevelRewardType.REWARD_SCRAP:
                textureId = 594177043;
                break;

            case EndLevelRewardType.REWARD_MILK:
                textureId = 67621527;
                break;

        }

        return textureId;

    }

    public void OnExecuteButton()
    {
        if (gameObject.name == "FirstRewardButton")
        {
            Debug.Log("Tot va començar");
        }

        else if (gameObject.name == "SecondRewardButton")
        {
            Debug.Log("quan Caos va");
        }

        else if (gameObject.name == "ThirdRewardButton")
        {
            Debug.Log("crear a Gea");
        }

        // Spawn object

        Time.ResumeGame();

    }

}
using System;
using DiamondEngine;
using System.Collections.Generic;

public enum EndLevelRewardType
{
    REWARD_BOON,
    REWARD_BESKAR,
    REWARD_MACARON,
    REWARD_SCRAP,
    REWARD_MILK
}

public class EndLevelRewards : DiamondComponent // This can probably stop being a component
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
    EndLevelReward firstReward; // Could probably make an array for this... UwU
    EndLevelReward secondReward;
    EndLevelReward thirdReward;
    DiamondEngineResources.GameResources selectedReward = null;
    DiamondEngineResources.BoonDataHolder boonGenerator = new DiamondEngineResources.BoonDataHolder();
    float boonTotalWeights = 0.0f;

    public DiamondEngineResources.GameResources GenerateRewardPipeline()
    {
        // This probably needs to be done only once
        firstReward = SelectRewards();
        secondReward = SelectRewards();
        thirdReward = SelectRewards();

        CreatePopUpGameObject();

        // Do nazi things with buttons to assign a value to selectedReward; we may want to control the OnExecuteButton from the script instead of attaching this script as a component (it probably shouldn't a component, and instead, controlled by SceneManager

        return selectedReward;
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

        newReward = new EndLevelReward(boonSpawner.RequestRandomBoon(), EndLevelRewardType.REWARD_BOON); // Spawn boon, just in case, but we should always quit from the previous return
        return newReward;

    }

    public int RequestRandomBoon()
    {
        float randomPick = (float)(new Random().NextDouble() * boonTotalWeights);
        for (int i = 0; i < boonGenerator.boonType.Length; i++)
        {
            if (randomPick > boonGenerator.boonType[i].rngChanceWeight)
            {
                return i;
            }
        }
        return boonGenerator.boonType.Length - 1;
    }

    GameObject CreatePopUpGameObject()
    {
        // This maybe should be a find, and if not, then we instantiate; this will mean enabling and disabling the GO constantly from SceneManager
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

        //        firstImage.GetComponent<Image2D>().AssignLibrary2DTexture(GetRewardTexture(firstReward));    // Have the function re-entered or something...
        //       secondImage.GetComponent<Image2D>().AssignLibrary2DTexture(GetRewardTexture(secondReward));
        //      thirdImage.GetComponent<Image2D>().AssignLibrary2DTexture(GetRewardTexture(thirdReward));

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
                text = boonGenerator.boonType[reward.boonIndex].rewardDescription;
                break;

            case EndLevelRewardType.REWARD_BESKAR:
                text = new DiamondEngineResources.BeskarResource().rewardDescription;
                break;

            case EndLevelRewardType.REWARD_MACARON:
                text = new DiamondEngineResources.MacaronResource().rewardDescription;
                break;

            case EndLevelRewardType.REWARD_SCRAP:
                text = new DiamondEngineResources.ScrapResource().rewardDescription;
                break;

            case EndLevelRewardType.REWARD_MILK:
                text = new DiamondEngineResources.MilkResource().rewardDescription;
                break;

        }

        return text;
    }

    public int GetRewardTexture(EndLevelReward reward)
    {

        int textureId = 0;

        switch (reward.type)
        {
            case EndLevelRewardType.REWARD_BOON:
                textureId = boonGenerator.boonType[reward.boonIndex].libraryTextureID;
                break;

            case EndLevelRewardType.REWARD_BESKAR:
                textureId = new DiamondEngineResources.BeskarResource().libraryTextureID;
                break;

            case EndLevelRewardType.REWARD_MACARON:
                textureId = new DiamondEngineResources.MacaronResource().libraryTextureID;
                break;

            case EndLevelRewardType.REWARD_SCRAP:
                textureId = new DiamondEngineResources.ScrapResource().libraryTextureID;
                break;

            case EndLevelRewardType.REWARD_MILK:
                textureId = new DiamondEngineResources.MilkResource().libraryTextureID;
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

    }

}
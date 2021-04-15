using System;
using DiamondEngine;
using System.Collections.Generic;

public static class PlayerResources
{
    static int beskarCounter = 0;
    static int macaronCounter = 0;
    static int milkCounter = 0;
    static int scrapCounter = 0;
    static int runCoins = 0;
    static Dictionary<Type, int> boonCounter = new Dictionary<Type, int>();

    public static int GetResourceCount(RewardType type, Type boonType = null)
    {
        int auxCounter = 0;

        switch (type)
        {
            case RewardType.REWARD_BOON:
                if (boonCounter.ContainsKey(boonType)) {
                    auxCounter = boonCounter[boonType];
                }
                break;

            case RewardType.REWARD_BESKAR:
                auxCounter = beskarCounter;
                break;

            case RewardType.REWARD_MACARON:
                auxCounter = macaronCounter;
                break;

            case RewardType.REWARD_SCRAP:
                auxCounter = scrapCounter;
                break;

            case RewardType.REWARD_MILK:
                auxCounter = milkCounter;
                break;
        }

        return auxCounter;
    }

    public static int AddResourceBy1(RewardType type, Type boonType = null)
    {
        int resourceLeft = 0;

        switch (type)
        {
            case RewardType.REWARD_BOON:
                resourceLeft = AddBoonToMap(1, boonType);
                break;

            case RewardType.REWARD_BESKAR:
                resourceLeft = ++beskarCounter;
                break;

            case RewardType.REWARD_MACARON:
                resourceLeft = ++macaronCounter;
                break;

            case RewardType.REWARD_SCRAP:
                resourceLeft = ++scrapCounter;
                break;

            case RewardType.REWARD_MILK:
                resourceLeft = ++milkCounter;
                break;
        }

        return resourceLeft;
    }

    public static int SubstractResource(RewardType type, int total_to_substract, Type boonType = null)
    {
        int resourceLeft = 0;
        if (total_to_substract <= 0)
            return resourceLeft;

        switch (type)
        {
            case RewardType.REWARD_BOON:
                resourceLeft = AddBoonToMap(-total_to_substract, boonType);
                break;

            case RewardType.REWARD_BESKAR:
                if (beskarCounter > 0)
                {
                    resourceLeft = beskarCounter - total_to_substract ;
                }
                break;

            case RewardType.REWARD_MACARON:
                if (macaronCounter > 0)
                {
                    resourceLeft = macaronCounter - (total_to_substract);
                }
                break;

            case RewardType.REWARD_SCRAP:
                if (scrapCounter > 0)
                {
                    resourceLeft = scrapCounter - total_to_substract;
                }
                break;

            case RewardType.REWARD_MILK:
                if (milkCounter > 0)
                {
                    resourceLeft = milkCounter - total_to_substract;
                }
                break;
        }

        return resourceLeft;
    }

    static int AddBoonToMap(int amountToAdd, Type boonType)
    {
        int boonAmount = 0;

        if (boonCounter.ContainsKey(boonType) == false)
        {
            boonCounter.Add(boonType, amountToAdd);
        }
        else
        {
            boonCounter[boonType] += amountToAdd;
        }

        if (boonCounter[boonType] < 0) { boonCounter[boonType] = 0; }
        boonAmount = boonCounter[boonType];

        return boonAmount;
    }

    public static int GetRunCoins()
    {
        return runCoins;
    }

    public static void AddRunCoins(int val)
    {
        runCoins += val;
    }

    public static void SetRunCoins(int val)
    {
        runCoins = val;
    }

    public static void ResetRunBoons()
    {
        boonCounter.Clear();
    }

}
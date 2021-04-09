using System;
using DiamondEngine;
using System.Collections.Generic;

public static class PlayerResources
{
    static int beskarCounter = 0;
    static int macaronCounter = 0;
    static int milkCounter = 0;
    static int scrapCounter = 0;
    static Dictionary<Type, int> boonCounter = new Dictionary<Type, int>();

    public static int GetResourceCount(RewardType type, Type boonType = null)
    {
        int auxCounter = 0;

        switch (type)
        {
            case RewardType.REWARD_BOON:
                auxCounter = boonCounter[boonType];
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

    public static void AddResourceBy1(RewardType type, Type boonType = null)
    {
        switch (type)
        {
            case RewardType.REWARD_BOON:
                AddBoonToMap(1, boonType);
                break;

            case RewardType.REWARD_BESKAR:
                beskarCounter++;
                break;

            case RewardType.REWARD_MACARON:
                macaronCounter++;
                break;

            case RewardType.REWARD_SCRAP:
                scrapCounter++;
                break;

            case RewardType.REWARD_MILK:
                milkCounter++;
                break;
        }
    }

    public static void SubstractResourceBy1(RewardType type, Type boonType = null)
    {
        switch (type)
        {
            case RewardType.REWARD_BOON:
                AddBoonToMap(-1, boonType);
                break;

            case RewardType.REWARD_BESKAR:
                if (beskarCounter > 0)
                {
                    beskarCounter--;
                }
                break;

            case RewardType.REWARD_MACARON:
                if (macaronCounter > 0)
                {
                    macaronCounter--;
                }
                break;

            case RewardType.REWARD_SCRAP:
                if (scrapCounter > 0)
                {
                    scrapCounter--;
                }
                break;

            case RewardType.REWARD_MILK:
                if (milkCounter > 0)
                {
                    milkCounter--;
                }
                break;
        }
    }

    static void AddBoonToMap(int amountToAdd, Type boonType)
    {
        if (boonCounter.ContainsKey(boonType) == false)
        {
            boonCounter.Add(boonType, amountToAdd);
        }
        else
        {
            boonCounter[boonType] += amountToAdd;
        }

        if (boonCounter[boonType] < 0) { boonCounter[boonType] = 0; }
    }

    public static void ResetRunBoons()
    {
        boonCounter.Clear();
    }

}
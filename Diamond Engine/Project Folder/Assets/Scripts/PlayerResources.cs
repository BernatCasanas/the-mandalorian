using System;
using DiamondEngine;
using System.Collections.Generic;

public static class PlayerResources
{
    public static int beskarCounter = 0;
    public static int macaronCounter = 0;
    public static int milkCounter = 0;
    public static int scrapCounter = 0;
    public static Dictionary<Type, int> boonCounter;

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
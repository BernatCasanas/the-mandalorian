using System;
using DiamondEngine;
using System.Collections.Generic;

public enum BOONS
{
    BOON_WATTOSCOOLANT = 0,
    BOON_CADBANEROCKETBOOTS,
    BOON_IMPERIALREFINEDCOOLANT,
    BOON_WRECKERRESILIENCE,
    BOON_GREEDOQUICKSHOOTER,
    BOON_BOSSKSTRENGTH,
    BOON_MASTERYODAASSITANCE,
    BOON_GREEFPAYCHECK,
    BOON_BOUNTYHUNTERSKILLS,
    BOON_ANAKINKILLSTREAK,
    BOON_MAX
}

public static class PlayerResources
{
    static int beskarCounter = DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadInt("beskarCounter") : 0;
    static int macaronCounter = DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadInt("macaronCounter") : 0;
    static int milkCounter = DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadInt("milkCounter") : 0;
    static int scrapCounter = DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadInt("scrapCounter") : 0;
    static int runCoins = DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadInt("runCoins") : 0;
    static bool[] accquiredBoons = new bool[(int)BOONS.BOON_MAX];
    static Dictionary<Type, int> boonCounter = new Dictionary<Type, int>();


    public static int GetResourceCount(RewardType type, Type boonType = null)
    {
        int auxCounter = 0;

        switch (type)
        {
            case RewardType.REWARD_BOON:
                if (boonCounter.ContainsKey(boonType))
                {
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

    public static void ResetHoldingBoons()
    {
        for (int i = 0; i < accquiredBoons.Length; i++)
        {
            accquiredBoons[i] = false;
        }
    }

    public static void AddBoon(BOONS newBoon)
    {
        accquiredBoons[(int)newBoon] = true;
    }

    public static bool CheckBoon(BOONS newBoon)
    {
        if (accquiredBoons[(int)newBoon]) return true;
        else return false;
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
                DiamondPrefs.Write("beskarCounter", beskarCounter);
                break;

            case RewardType.REWARD_MACARON:
                resourceLeft = ++macaronCounter;
                DiamondPrefs.Write("macaronCounter", macaronCounter);
                break;

            case RewardType.REWARD_SCRAP:
                resourceLeft = ++scrapCounter;
                DiamondPrefs.Write("scrapCounter", scrapCounter);
                break;

            case RewardType.REWARD_MILK:
                resourceLeft = ++milkCounter;
                DiamondPrefs.Write("milkCounter", milkCounter);
                break;
        }

        return resourceLeft;
    }

    public static int AddResourceByValue(RewardType type, int num)
    {
        int resourceLeft = 0;

        switch (type)
        {
            case RewardType.REWARD_BESKAR:
                beskarCounter += num;
                resourceLeft = beskarCounter;
                DiamondPrefs.Write("beskarCounter", beskarCounter);
                break;

            case RewardType.REWARD_MACARON:
                macaronCounter += num;
                resourceLeft = macaronCounter;
                DiamondPrefs.Write("macaronCounter", macaronCounter);
                break;

            case RewardType.REWARD_SCRAP:
                scrapCounter += num;
                resourceLeft = scrapCounter;
                DiamondPrefs.Write("scrapCounter", scrapCounter);
                break;

            case RewardType.REWARD_MILK:
                milkCounter += num;
                resourceLeft = milkCounter;
                DiamondPrefs.Write("milkCounter", milkCounter);
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
                    beskarCounter = beskarCounter - total_to_substract;
                    resourceLeft = beskarCounter;
                    DiamondPrefs.Write("beskarCounter", beskarCounter);
                }
                break;

            case RewardType.REWARD_MACARON:
                if (macaronCounter > 0)
                {
                    macaronCounter = macaronCounter - (total_to_substract);
                    resourceLeft = macaronCounter;
                    DiamondPrefs.Write("macaronCounter", macaronCounter);
                }
                break;

            case RewardType.REWARD_SCRAP:
                if (scrapCounter > 0)
                {
                    scrapCounter = scrapCounter - total_to_substract;
                    resourceLeft = scrapCounter;
                    DiamondPrefs.Write("scrapCounter", scrapCounter);
                }
                break;

            case RewardType.REWARD_MILK:
                if (milkCounter > 0)
                {
                    milkCounter = milkCounter - total_to_substract;
                    resourceLeft = milkCounter;
                    DiamondPrefs.Write("milkCounter", milkCounter);
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

        for (int i = 0; i < val; i++)
        {
            Counter.SumToCounterType(Counter.CounterTypes.RUN_COINS);
        }

        DiamondPrefs.Write("runCoins", runCoins);
    }

    public static void SetRunCoins(int val)
    {
        runCoins = val;

        for (int i = 0; i < val; i++)
        {
            Counter.SumToCounterType(Counter.CounterTypes.RUN_COINS);
        }

        DiamondPrefs.Write("runCoins", runCoins);
    }

    public static void ResetRunBoons()
    {
        boonCounter.Clear();
        Debug.Log("Boons reseted");
    }

    public static void ResetResources()
    {
        beskarCounter = DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadInt("beskarCounter") : 0;
        macaronCounter = DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadInt("macaronCounter") : 0;
        milkCounter = DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadInt("milkCounter") : 0;
        scrapCounter = DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadInt("scrapCounter") : 0;
        runCoins = DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadInt("runCoins") : 0;
    }

}
using System;
using DiamondEngine;
using System.Collections.Generic;

public class GameResources
{
    public GameResources(int _id, RewardType _type, float _weight, string _description)
    {
        libraryTextureID = _id;
        resourceType = _type;
        rewardDescription = _description;
        rngChanceWeight = _weight;
    }

    public virtual void Use() { }

    public int libraryTextureID;
    public RewardType resourceType;
    public string rewardDescription;
    public float rngChanceWeight;

}

public class BeskarResource : GameResources
{
    public BeskarResource() : base(1068400135, RewardType.REWARD_BESKAR, -1.0f, "The metal of the mandalorian people, second to none in the galaxy.") { }

    public override void Use()
    {
        PlayerResources.AddResourceBy1(RewardType.REWARD_BESKAR);
    }
}

public class MacaronResource : GameResources
{
    public MacaronResource() : base(694717696, RewardType.REWARD_MACARON, -1.0f, "Just a macaron. Grogu does love them, though.") { }

    public override void Use()
    {
        PlayerResources.AddResourceBy1(RewardType.REWARD_MACARON);
    }
}

public class ScrapResource : GameResources
{
    public ScrapResource() : base(268510122, RewardType.REWARD_SCRAP, -1.0f, "Remains of powerful imperial technology.") { }

    public override void Use()
    {
        PlayerResources.AddResourceBy1(RewardType.REWARD_SCRAP);
    }
}

public class MilkResource : GameResources
{
    public MilkResource() : base(1314015482, RewardType.REWARD_MILK, -1.0f, "Sweet and tasty blue milk, a true delicacy.") { }

    public override void Use()
    {
        PlayerResources.AddResourceBy1(RewardType.REWARD_MILK);
    }
}

// WE SHOULD PROBABLY CHANGE THE NAMES TO THE BOON'S ACTUAL NAME

//Each time you kill an enemy heal +1 HP. - Bo Katan’s resilience
public class LifeStealBoon : GameResources
{
    public LifeStealBoon() : base(1240646973, RewardType.REWARD_BOON, 1.0f, "Each enemy kill heals 1 HP") { }

    public override void Use()
    {
        if (Core.instance.gameObject.GetComponent<PlayerHealth>() != null)
        {
            int currentLifeSteal = Core.instance.gameObject.GetComponent<PlayerHealth>().IncrementHealingWhenKillingEnemy(1);
            Debug.Log("LifeSteal increased to: " + currentLifeSteal);
            Counter.SumToCounterType(Counter.CounterTypes.BOKATAN_RES);
        }
        else
        {
            Debug.Log("ERROR!! Din Djarin has no player health component");

        }
    }
}

//+20% max HP. - Wrecker’s resilience
public class IncrementMaxHpBoon : GameResources
{
    public IncrementMaxHpBoon() : base(1143141246, RewardType.REWARD_BOON, 1.0f, "Increments +20 health") { }

    public override void Use()
    {

        if (Core.instance.gameObject.GetComponent<PlayerHealth>() != null)
        {
            int toIncrement = 20;
            int currentMaxHp = Core.instance.gameObject.GetComponent<PlayerHealth>().IncrementMaxHpValue(toIncrement);
            int currentHp = Core.instance.gameObject.GetComponent<PlayerHealth>().TakeDamage(-toIncrement);
            Debug.Log("Max HP increased to: " + currentMaxHp);
            Debug.Log("Curr HP increased to: " + currentHp);
           // Counter.SumToCounterType(Counter.CounterTypes.WRECKER_RES);
        }
        else
        {
            Debug.Log("ERROR!! Din Djarin has no player health component");
        }

    }
}

public class CadBaneSoH : GameResources
{
    public CadBaneSoH() : base(1240646973, RewardType.REWARD_BOON, 1.0f, "first attack with a different weapon from last attack deals +33% damage.") { }

    public override void Use()
    {
        if (Core.instance != null)
        {
            Core.instance.AddStatus(STATUS_TYPE.CAD_BANE_SOH, STATUS_APPLY_TYPE.ADDITIVE, 30f, 0, true);
            //Counter.SumToCounterType(Counter.CounterTypes.CAD_BANE_SOH);
            if (!Core.boons.Contains(STATUS_TYPE.CAD_BANE_SOH))
                Core.boons.Add(STATUS_TYPE.CAD_BANE_SOH);
        }
        //else
        //{
        //    Debug.Log("ERROR!! Din Djarin has no player health component");
        //}

    }
}


public class CadBaneBoots : GameResources
{
    public CadBaneBoots() : base(1240646973, RewardType.REWARD_BOON, 1.0f, "+5% permanent hasted state while grenade is on CD.") { }

    public override void Use()
    {
        if (Core.instance != null)
        {

            Core.instance.AddStatus(STATUS_TYPE.CAD_BANE_BOOTS, STATUS_APPLY_TYPE.ADDITIVE, 30f, 0, true);
            //Counter.SumToCounterType(Counter.CounterTypes.CAD_BANE_SOH);
            if (!Core.boons.Contains(STATUS_TYPE.CAD_BANE_BOOTS))
                Core.boons.Add(STATUS_TYPE.CAD_BANE_BOOTS);
        }
        //else
        //{
        //    Debug.Log("ERROR!! Din Djarin has no player health component");
        //}

    }
}
static class BoonDataHolder
{
    static BoonDataHolder()
    {
        for (int i = 0; i < boonType.Length; i++)
        {
            float auxWeight = boonType[i].rngChanceWeight;
            boonType[i].rngChanceWeight = boonTotalWeights;
            boonTotalWeights += auxWeight;
        }
    }

    public static GameResources[] boonType = new GameResources[] // There's probably a better way to organize this data, can't take care of it right now
        {
            new LifeStealBoon(),
            new IncrementMaxHpBoon(),
            new CadBaneSoH(),
            new CadBaneBoots(),
            //TODO Add boons here
        };

    public static float boonTotalWeights;

}

using System;
using DiamondEngine;
using System.Collections.Generic;
public class GameResources  // We make a struct of the GameResourceData, then we have a function with a getter to a dictionary that return the data. But wait... The Use function can be stored in the dictionary?
{
    public GameResources(int _id, EndLevelRewardType _type, float _weight, string _description)
    {
        libraryTextureID = _id;
        resourceType = _type;
        rewardDescription = _description;
    }
    public virtual void Use() { }

    public int libraryTextureID;
    public EndLevelRewardType resourceType;
    public string rewardDescription;
    public float rngChanceWeight;

}

public class BeskarResource : GameResources
{
    public BeskarResource() : base(968569395, EndLevelRewardType.REWARD_BESKAR, -1.0f, "The metal of the mandalorian people, second to none in the galaxy.") { }
}

public class MacaronResource : GameResources
{
    public MacaronResource() : base(2063474155, EndLevelRewardType.REWARD_MACARON, -1.0f, "Just a macaron. Grogu does love them, though.") { }
}

public class ScrapResource : GameResources
{
    public ScrapResource() : base(694204090, EndLevelRewardType.REWARD_SCRAP, -1.0f, "Remains of powerful imperial technology.") { }
}

public class MilkResource : GameResources
{
    public MilkResource() : base(1783333495, EndLevelRewardType.REWARD_MILK, -1.0f, "Sweet and tasty blue milk, a true delicacy.") { }
}

// WE SHOULD PROBABLY CHANGE THE NAMES TO THE BOON'S ACTUAL NAME

//Each time you kill an enemy heal +1 HP. - Bo Katan’s resilience
public class LifeStealBoon : GameResources
{
    public LifeStealBoon() : base(1240646973, EndLevelRewardType.REWARD_BOON, 1.0f, "Each enemy kill heals 1 HP") { }

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
    public IncrementMaxHpBoon() : base(1143141246, EndLevelRewardType.REWARD_BOON, 1.0f, "Increments +20 health") { }

    public override void Use()
    {
        if (Core.instance.gameObject.GetComponent<PlayerHealth>() != null)
        {
            int toIncrement = 20;
            int currentMaxHp = Core.instance.gameObject.GetComponent<PlayerHealth>().IncrementMaxHpValue(toIncrement);
            int currentHp = Core.instance.gameObject.GetComponent<PlayerHealth>().TakeDamage(-toIncrement);
            Debug.Log("Max HP increased to: " + currentMaxHp);
            Debug.Log("Curr HP increased to: " + currentHp);
            Counter.SumToCounterType(Counter.CounterTypes.WRECKER_RES);
        }
        else
        {
            Debug.Log("ERROR!! Din Djarin has no player health component");
        }

    }
}

public class BoonDataHolder
{

    public BoonDataHolder()
    {
        for (int i = 0; i < boonType.Length; i++)
        {
            float auxWeight = boonType[i].rngChanceWeight;
            boonType[i].rngChanceWeight = boonTotalWeights;
            boonTotalWeights += auxWeight;
        }
    }

    public GameResources[] boonType = new GameResources[] // There's probably a better way to organize this data, can't take care of it right now
        {
            new LifeStealBoon(),
            new IncrementMaxHpBoon(),
            //TODO Add boons here
        };

    public float boonTotalWeights = 0.0f;

}

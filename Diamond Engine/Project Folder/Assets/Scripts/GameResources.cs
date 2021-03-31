using System;
using DiamondEngine;
using System.Collections.Generic;

namespace DiamonEngineResources
{
    public class GameResources
    {
        public GameResources(int _id, EndLevelRewardType _type)
        {
            libraryTextureID = _id;
            resourceType = _type;
        }

        // Yes, we could have those only used in spawn functionality like we used to, but I think the cost memory-wise is worth to make adding boons way easier and more intuitive
        public int libraryTextureID;
        public EndLevelRewardType resourceType;
    }

    public class BeskarResource : GameResources
    {
        public BeskarResource() : base(1083690418, EndLevelRewardType.REWARD_BESKAR) { }
    }

    public class MacaronResource : GameResources
    {
        public MacaronResource() : base(222418542, EndLevelRewardType.REWARD_MACARON) { }
    }

    public class ScrapResource : GameResources
    {
        public ScrapResource() : base(594177043, EndLevelRewardType.REWARD_SCRAP) { }
    }

    public class MilkResource : GameResources
    {
        public MilkResource() : base(67621527, EndLevelRewardType.REWARD_MILK) { }
    }

    public class BoonResource : GameResources
    {
        public BoonResource(int _id, EndLevelRewardType _type, float _weight) : base(_id, _type)
        {
            rngChanceWeight = _weight;
        }

        public virtual void Use() { }

        public float rngChanceWeight;
    }

    // WE SHOULD PROBABLY CHANGE THE NAMES TO THE BOON'S ACTUAL NAME

    //Each time you kill an enemy heal +1 HP. - Bo Katan’s resilience
    public class LifeStealBoon : BoonResource
    {
        public LifeStealBoon() : base(1240646973, EndLevelRewardType.REWARD_BOON, 1.0f) { }

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
    public class IncrementMaxHpBoon : BoonResource
    {
        public IncrementMaxHpBoon() : base(1143141246, EndLevelRewardType.REWARD_BOON, 1.0f) { }

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
        public static Dictionary<string, Type> boonType = new Dictionary<string, Type>
        {
            {"LifeSteal", typeof(LifeStealBoon)},
            {"IncMaxHp", typeof(IncrementMaxHpBoon)},
            //TODO Add boons here
        };
    }

}
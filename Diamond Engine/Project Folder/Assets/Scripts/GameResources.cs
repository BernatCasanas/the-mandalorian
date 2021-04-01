using System;
using DiamondEngine;
using System.Collections.Generic;

namespace DiamondEngineResources
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
        // Probably add text here
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
        public BoonResource(int _id, EndLevelRewardType _type, float _weight, string _description) : base(_id, _type)
        {
            rngChanceWeight = _weight;
            boonDescription = _description;
        }

        public virtual void Use() { }

        public float rngChanceWeight;
        public string boonDescription;
    }

    // WE SHOULD PROBABLY CHANGE THE NAMES TO THE BOON'S ACTUAL NAME

    //Each time you kill an enemy heal +1 HP. - Bo Katan’s resilience
    public class LifeStealBoon : BoonResource
    {
        public LifeStealBoon() : base(1240646973, EndLevelRewardType.REWARD_BOON, 1.0f, "Health increases by each enemy kill") { }

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
                boonType[i].rngChanceWeight = boonTotalWeights;
                boonTotalWeights += boonType[i].rngChanceWeight;
            }
        }

        public BoonResource[] boonType = new BoonResource[] // There's probably a better way to organize this data, can't take care of it right now
            {
            new LifeStealBoon(),
            new IncrementMaxHpBoon(),
                //TODO Add boons here
            };

        public float boonTotalWeights = 0.0f;

    }

}
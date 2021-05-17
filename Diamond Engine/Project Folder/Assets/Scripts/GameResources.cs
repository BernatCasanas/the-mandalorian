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

#region RESOURCES
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
#endregion

// WE SHOULD PROBABLY CHANGE THE NAMES TO THE BOON'S ACTUAL NAME

#region BOONS
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
            PlayerResources.AddResourceBy1(RewardType.REWARD_BOON, BoonDataHolder.boonType[0].GetType());
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

            PlayerResources.AddResourceBy1(RewardType.REWARD_BOON);
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
            if (!Core.boons.Contains(STATUS_TYPE.CAD_BANE_SOH))
                Core.boons.Add(STATUS_TYPE.CAD_BANE_SOH);

            PlayerResources.AddResourceBy1(RewardType.REWARD_BOON);
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
            Core.instance.AddStatus(STATUS_TYPE.CAD_BANE_BOOTS, STATUS_APPLY_TYPE.ADDITIVE, 5f, 0, true);
            if (!Core.boons.Contains(STATUS_TYPE.CAD_BANE_BOOTS))
                Core.boons.Add(STATUS_TYPE.CAD_BANE_BOOTS);

            PlayerResources.AddResourceBy1(RewardType.REWARD_BOON);
        }
    }
}

public class MandoQuickDraw : GameResources
{
    public MandoQuickDraw() : base(1240646973, RewardType.REWARD_BOON, 1.0f, "first attack after a dash does 25% more damage") { }

    public override void Use()
    {
        if (Core.instance != null)
        {
            Core.instance.AddStatus(STATUS_TYPE.MANDO_QUICK_DRAW, STATUS_APPLY_TYPE.ADDITIVE, 25f, 0, true);
            if (!Core.boons.Contains(STATUS_TYPE.MANDO_QUICK_DRAW))
                Core.boons.Add(STATUS_TYPE.MANDO_QUICK_DRAW);

            PlayerResources.AddResourceBy1(RewardType.REWARD_BOON);
        }

    }
}

// TO DO
//public class BobbaAmmo : GameResources
//{
//    public BobbaAmmo() : base(1240646973, RewardType.REWARD_BOON, 1.0f, "first attack after a dash does 25% more damage") { }

//    public override void Use()
//    {
//        if (Core.instance != null)
//        {

//            Core.instance.AddStatus(STATUS_TYPE.BOBBA_AMMO, STATUS_APPLY_TYPE.ADDITIVE, 25F, 0, true);
//            if (!Core.boons.Contains(STATUS_TYPE.BOBBA_AMMO))
//                Core.boons.Add(STATUS_TYPE.BOBBA_AMMO);
//        }

//    }
//}

public class BosskStrength : GameResources
{
    public BosskStrength() : base(1240646973, RewardType.REWARD_BOON, 1.0f, "mitigate 10% of the damage received (rounded down).") { }

    public override void Use()
    {
        if (Core.instance != null)
        {
            Core.instance.AddStatus(STATUS_TYPE.BOSSK_STR, STATUS_APPLY_TYPE.ADDITIVE, 10f, 0, true);
            if (!Core.boons.Contains(STATUS_TYPE.BOSSK_STR))
                Core.boons.Add(STATUS_TYPE.BOSSK_STR);

            PlayerResources.AddResourceBy1(RewardType.REWARD_BOON);
        }
    }
}
public class RexSecBlaster : GameResources
{
    public RexSecBlaster() : base(1240646973, RewardType.REWARD_BOON, 1.0f, "when switching weapons, you get 5% additive haste for 5 seconds, stackable up to 20%.") { }

    public override void Use()
    {
        if (Core.instance != null)
        {
            Core.instance.AddStatus(STATUS_TYPE.REX_SEC_BLASTER, STATUS_APPLY_TYPE.ADDITIVE, 5f, 0, true);
            if (!Core.boons.Contains(STATUS_TYPE.REX_SEC_BLASTER))
                Core.boons.Add(STATUS_TYPE.REX_SEC_BLASTER);

            PlayerResources.AddResourceBy1(RewardType.REWARD_BOON);
        }
    }
}

public class GreedoShooter : GameResources
{
    public GreedoShooter() : base(1240646973, RewardType.REWARD_BOON, 1.0f, "+20% fire rate on the primary weapon") { }

    public override void Use()
    {
        if (Core.instance != null)
        {
            Core.instance.AddStatus(STATUS_TYPE.GREEDO_SHOOTER, STATUS_APPLY_TYPE.ADDITIVE, -20f, 0, true);
            if (!Core.boons.Contains(STATUS_TYPE.GREEDO_SHOOTER))
                Core.boons.Add(STATUS_TYPE.GREEDO_SHOOTER);

            PlayerResources.AddResourceBy1(RewardType.REWARD_BOON);
        }

    }
}

public class FennecSniper : GameResources
{
    public FennecSniper() : base(1240646973, RewardType.REWARD_BOON, 1.0f, "Double rifle sweet spot time window (non-stackable).") { }

    public override void Use()
    {
        if (Core.instance != null)
        {
            Core.instance.AddStatus(STATUS_TYPE.FENNEC_SR, STATUS_APPLY_TYPE.BIGGER_PERCENTAGE, 100f, 0, true);
            if (!Core.boons.Contains(STATUS_TYPE.FENNEC_SR))
                Core.boons.Add(STATUS_TYPE.FENNEC_SR);
        }

    }
}

public class AnakinKillstreak : GameResources
{
    public AnakinKillstreak() : base(1240646973, RewardType.REWARD_BOON, 1.0f, "While over a combo of 3, +5% additive Hasted") { }

    public override void Use()
    {
        if (Core.instance != null)
        {
            Core.instance.AddStatus(STATUS_TYPE.ANAKIN_KILLSTREAK, STATUS_APPLY_TYPE.ADDITIVE, 5f, 0, true);
            if (!Core.boons.Contains(STATUS_TYPE.ANAKIN_KILLSTREAK))
                Core.boons.Add(STATUS_TYPE.ANAKIN_KILLSTREAK);

            PlayerResources.AddResourceBy1(RewardType.REWARD_BOON);
        }

    }
}

public class EchoRecovery : GameResources
{
    public EchoRecovery() : base(1240646973, RewardType.REWARD_BOON, 1.0f, " +10% additive hasted state for 5 seconds after losing a combo.") { }

    public override void Use()
    {
        if (Core.instance != null)
        {
            Core.instance.AddStatus(STATUS_TYPE.ECHO_RECOVERY, STATUS_APPLY_TYPE.ADDITIVE, 10f, 0, true);
            if (!Core.boons.Contains(STATUS_TYPE.ECHO_RECOVERY))
                Core.boons.Add(STATUS_TYPE.ECHO_RECOVERY);

            PlayerResources.AddResourceBy1(RewardType.REWARD_BOON);
        }

    }
}

public class WattoCoolant : GameResources
{
    public WattoCoolant() : base(1240646973, RewardType.REWARD_BOON, 1.0f, "dash cooldown is reduced by 40%") { }

    public override void Use()
    {
        if (Core.instance != null)
        {
            Core.instance.AddStatus(STATUS_TYPE.WATTO_COOLANT, STATUS_APPLY_TYPE.ADDITIVE, 40f, 0, true);
            if (!Core.boons.Contains(STATUS_TYPE.WATTO_COOLANT))
                Core.boons.Add(STATUS_TYPE.WATTO_COOLANT);

            PlayerResources.AddResourceBy1(RewardType.REWARD_BOON);
        }
    }
}

public class LuminaraForce : GameResources
{
    public LuminaraForce() : base(1240646973, RewardType.REWARD_BOON, 1.0f, "Grogu’s dynamic covers lasts +5 seconds") { }

    public override void Use()
    {
        if (Core.instance != null)
        {
            Core.instance.AddStatus(STATUS_TYPE.LUMINARA_FORCE, STATUS_APPLY_TYPE.ADDITIVE, 4f, 0, true);
            if (!Core.boons.Contains(STATUS_TYPE.LUMINARA_FORCE))
                Core.boons.Add(STATUS_TYPE.LUMINARA_FORCE);

            PlayerResources.AddResourceBy1(RewardType.REWARD_BOON);
        }

    }
}
public class MandoCode : GameResources
{
    public MandoCode() : base(1240646973, RewardType.REWARD_BOON, 1.0f, "Deal +20% damage but have -20% less max health (stackable)") { }

    public override void Use()
    {
        if (Core.instance != null)
        {
            Core.instance.AddStatus(STATUS_TYPE.MANDO_CODE, STATUS_APPLY_TYPE.ADDITIVE, 20f, 0, true);
            if (!Core.boons.Contains(STATUS_TYPE.MANDO_CODE))
                Core.boons.Add(STATUS_TYPE.MANDO_CODE);

            PlayerResources.AddResourceBy1(RewardType.REWARD_BOON);
        }

    }
}


public class ItsaTrap : GameResources
{
    public ItsaTrap() : base(1240646973, RewardType.REWARD_BOON, 1.0f, "Deal +20% damage but have -20% less max health (stackable)") { }

    public override void Use()
    {
        if (Core.instance != null)
        {
            Core.instance.AddStatus(STATUS_TYPE.ITSA_TRAP, STATUS_APPLY_TYPE.ADDITIVE, -33f, 0, true);
            if (!Core.boons.Contains(STATUS_TYPE.ITSA_TRAP))
                Core.boons.Add(STATUS_TYPE.ITSA_TRAP);
        }

    }
}
public class WreckHeavyShot : GameResources
{
    public WreckHeavyShot() : base(1240646973, RewardType.REWARD_BOON, 1.0f, "Deal +20% damage but have -20% less max health (stackable)") { }

    public override void Use()
    {
        if (Core.instance != null)
        {
            Core.instance.AddStatus(STATUS_TYPE.WRECK_HEAVY_SHOT, STATUS_APPLY_TYPE.ADDITIVE, 25f, 0, true);
            if (!Core.boons.Contains(STATUS_TYPE.WRECK_HEAVY_SHOT))
                Core.boons.Add(STATUS_TYPE.WRECK_HEAVY_SHOT);
        }

    }
}

#endregion
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
            new MandoQuickDraw(),
            // not finnished new BobbaAmmo(),
            new BosskStrength(),
            new RexSecBlaster(),
            new GreedoShooter(),
            new AnakinKillstreak(),
            new EchoRecovery(),
            new WattoCoolant(),
            new LuminaraForce(),
            new MandoCode(),
            new FennecSniper(),
            new ItsaTrap(),
            new WreckHeavyShot(),
           // TODO Add boons here
        };

    public static float boonTotalWeights;

}

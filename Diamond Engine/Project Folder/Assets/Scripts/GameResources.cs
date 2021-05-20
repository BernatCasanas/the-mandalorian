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
    public string name = "No name Boon";
    public ShopPrice price = ShopPrice.SHOP_FREE;
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
public class BokatanResilence : GameResources
{
    public BokatanResilence() : base(1885100100, RewardType.REWARD_BOON, 1.0f, "Each enemy kill heals 1 HP")
    {
        name = "Bo-Katan's Resilence";
        price = ShopPrice.SHOP_EXPENSIVE;
    }

    public override void Use()
    {
        if (Core.instance.gameObject.GetComponent<PlayerHealth>() != null)
        {
            int currentLifeSteal = Core.instance.gameObject.GetComponent<PlayerHealth>().IncrementHealingWhenKillingEnemy(1);
            Debug.Log("LifeSteal increased to: " + currentLifeSteal);

            PlayerResources.AddResourceBy1(RewardType.REWARD_BOON, GetType());
        }
        else
        {
            Debug.Log("ERROR!! Din Djarin has no player health component");
        }
    }
}

//+20% max HP. - Wrecker’s resilience
public class WreckerResilence : GameResources
{
    public WreckerResilence() : base(1789396100, RewardType.REWARD_BOON, 1.0f, "Increments +20 health")
    {
        name = "Wrecker's Resilence";
        price = ShopPrice.SHOP_EXPENSIVE;
    }

    public override void Use()
    {
        if (Core.instance.gameObject.GetComponent<PlayerHealth>() != null)
        {
            int toIncrement = 20;
            int currentMaxHp = Core.instance.gameObject.GetComponent<PlayerHealth>().IncrementMaxHpValue(toIncrement);
            int currentHp = Core.instance.gameObject.GetComponent<PlayerHealth>().TakeDamage(-toIncrement);
            Debug.Log("Max HP increased to: " + currentMaxHp);
            Debug.Log("Curr HP increased to: " + currentHp);

            PlayerResources.AddResourceBy1(RewardType.REWARD_BOON, GetType());
        }
        else
        {
            Debug.Log("ERROR!! Din Djarin has no player health component");
        }
    }
}

public class CadBaneSoH : GameResources
{
    public CadBaneSoH() : base(405709743, RewardType.REWARD_BOON, 1.0f, "first attack with a different weapon from last attack deals +33% damage.")
    {
        name = "Cad Bane’s sleight of hand";
        price = ShopPrice.SHOP_CHEAP;
    }

    public override void Use()
    {
        if (Core.instance != null)
        {
            Core.instance.AddStatus(STATUS_TYPE.CAD_BANE_SOH, STATUS_APPLY_TYPE.ADDITIVE, 30f, 0, true);
            if (!Core.boons.Contains(STATUS_TYPE.CAD_BANE_SOH))
                Core.boons.Add(STATUS_TYPE.CAD_BANE_SOH);

            PlayerResources.AddResourceBy1(RewardType.REWARD_BOON, GetType());
        }
        //else
        //{
        //    Debug.Log("ERROR!! Din Djarin has no player health component");
        //}
    }
}


public class CadBaneBoots : GameResources
{
    public CadBaneBoots() : base(1747641289, RewardType.REWARD_BOON, 1.0f, "+5% permanent hasted state while grenade is on CD.")
    {
        name = "Cad Bane’s rocket boots";
        price = ShopPrice.SHOP_CHEAP;
    }

    public override void Use()
    {
        if (Core.instance != null)
        {
            Core.instance.AddStatus(STATUS_TYPE.CAD_BANE_BOOTS, STATUS_APPLY_TYPE.ADDITIVE, 5f, 0, true);
            if (!Core.boons.Contains(STATUS_TYPE.CAD_BANE_BOOTS))
                Core.boons.Add(STATUS_TYPE.CAD_BANE_BOOTS);

            PlayerResources.AddResourceBy1(RewardType.REWARD_BOON, GetType());
        }
    }
}

public class MandoQuickDraw : GameResources
{
    public MandoQuickDraw() : base(821239276, RewardType.REWARD_BOON, 1.0f, "first attack after a dash does 25% more damage")
    {
        name = "Mandalorian's Quick Draw";
        price = ShopPrice.SHOP_AVERAGE;
    }

    public override void Use()
    {
        if (Core.instance != null)
        {
            Core.instance.AddStatus(STATUS_TYPE.MANDO_QUICK_DRAW, STATUS_APPLY_TYPE.ADDITIVE, 25f, 0, true);
            if (!Core.boons.Contains(STATUS_TYPE.MANDO_QUICK_DRAW))
                Core.boons.Add(STATUS_TYPE.MANDO_QUICK_DRAW);

            PlayerResources.AddResourceBy1(RewardType.REWARD_BOON, GetType());
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
    public BosskStrength() : base(1309422332, RewardType.REWARD_BOON, 1.0f, "mitigate 10% of the damage received (rounded down).")
    {
        name = "Bossk's Strength";
        price = ShopPrice.SHOP_CHEAP;
    }

    public override void Use()
    {
        if (Core.instance != null)
        {
            Core.instance.AddStatus(STATUS_TYPE.BOSSK_STR, STATUS_APPLY_TYPE.ADDITIVE, 10f, 0, true);
            if (!Core.boons.Contains(STATUS_TYPE.BOSSK_STR))
                Core.boons.Add(STATUS_TYPE.BOSSK_STR);

            PlayerResources.AddResourceBy1(RewardType.REWARD_BOON, GetType());
        }
    }
}
public class RexSecBlaster : GameResources
{
    public RexSecBlaster() : base(1417560378, RewardType.REWARD_BOON, 1.0f, "when switching weapons, you get 5% additive haste for 5 seconds, stackable up to 20%.")
    {
        name = "Rex's second blaster";
        price = ShopPrice.SHOP_AVERAGE;
    }

    public override void Use()
    {
        if (Core.instance != null)
        {
            Core.instance.AddStatus(STATUS_TYPE.REX_SEC_BLASTER, STATUS_APPLY_TYPE.ADDITIVE, 5f, 0, true);
            if (!Core.boons.Contains(STATUS_TYPE.REX_SEC_BLASTER))
                Core.boons.Add(STATUS_TYPE.REX_SEC_BLASTER);

            PlayerResources.AddResourceBy1(RewardType.REWARD_BOON, GetType());
        }
    }
}

public class GreedoShooter : GameResources
{
    public GreedoShooter() : base(1721324939, RewardType.REWARD_BOON, 1.0f, "+20% fire rate on the primary weapon")
    {
        name = "Greedo's quick shooter";
        price = ShopPrice.SHOP_EXPENSIVE;
    }

    public override void Use()
    {
        if (Core.instance != null)
        {
            Core.instance.AddStatus(STATUS_TYPE.GREEDO_SHOOTER, STATUS_APPLY_TYPE.ADDITIVE, -20f, 0, true);
            if (!Core.boons.Contains(STATUS_TYPE.GREEDO_SHOOTER))
                Core.boons.Add(STATUS_TYPE.GREEDO_SHOOTER);

            PlayerResources.AddResourceBy1(RewardType.REWARD_BOON, GetType());
        }
    }
}

public class FennecSniper : GameResources
{
    public FennecSniper() : base(25534683, RewardType.REWARD_BOON, 1.0f, "Double rifle sweet spot time window (non-stackable).")
    {
        name = "Fennec's sniper rifle";
        price = ShopPrice.SHOP_EXPENSIVE;
    }

    public override void Use()
    {
        if (Core.instance != null)
        {
            Core.instance.AddStatus(STATUS_TYPE.FENNEC_SR, STATUS_APPLY_TYPE.BIGGER_PERCENTAGE, 100f, 0, true);
            if (!Core.boons.Contains(STATUS_TYPE.FENNEC_SR))
                Core.boons.Add(STATUS_TYPE.FENNEC_SR);

            PlayerResources.AddResourceBy1(RewardType.REWARD_BOON, GetType());
        }
    }
}

public class AnakinKillstreak : GameResources
{
    public AnakinKillstreak() : base(96086378, RewardType.REWARD_BOON, 1.0f, "While over a combo of 3, +5% additive Hasted")
    {
        name = "Anakin's Kill Streak";
        price = ShopPrice.SHOP_AVERAGE;
    }

    public override void Use()
    {
        if (Core.instance != null)
        {
            Core.instance.AddStatus(STATUS_TYPE.ANAKIN_KILLSTREAK, STATUS_APPLY_TYPE.ADDITIVE, 5f, 0, true);
            if (!Core.boons.Contains(STATUS_TYPE.ANAKIN_KILLSTREAK))
                Core.boons.Add(STATUS_TYPE.ANAKIN_KILLSTREAK);

            PlayerResources.AddResourceBy1(RewardType.REWARD_BOON, GetType());
        }
    }
}

public class EchoRecovery : GameResources
{
    public EchoRecovery() : base(65608412, RewardType.REWARD_BOON, 1.0f, " +10% additive hasted state for 5 seconds after losing a combo.")
    {
        name = "Echo's quick recovery";
        price = ShopPrice.SHOP_AVERAGE;
    }

    public override void Use()
    {
        if (Core.instance != null)
        {
            Core.instance.AddStatus(STATUS_TYPE.ECHO_RECOVERY, STATUS_APPLY_TYPE.ADDITIVE, 10f, 0, true);
            if (!Core.boons.Contains(STATUS_TYPE.ECHO_RECOVERY))
                Core.boons.Add(STATUS_TYPE.ECHO_RECOVERY);

            PlayerResources.AddResourceBy1(RewardType.REWARD_BOON, GetType());
        }
    }
}

public class WattoCoolant : GameResources
{
    public WattoCoolant() : base(1941353969, RewardType.REWARD_BOON, 1.0f, "dash cooldown is reduced by 40%")
    {
        name = "Watto's Coolant";
        price = ShopPrice.SHOP_CHEAP;
    }

    public override void Use()
    {
        if (Core.instance != null)
        {
            Core.instance.AddStatus(STATUS_TYPE.WATTO_COOLANT, STATUS_APPLY_TYPE.ADDITIVE, 40f, 0, true);
            if (!Core.boons.Contains(STATUS_TYPE.WATTO_COOLANT))
                Core.boons.Add(STATUS_TYPE.WATTO_COOLANT);

            PlayerResources.AddResourceBy1(RewardType.REWARD_BOON, GetType());
        }
    }
}

public class LuminaraForce : GameResources
{
    public LuminaraForce() : base(895016668, RewardType.REWARD_BOON, 1.0f, "Grogu’s dynamic covers lasts +5 seconds")
    {
        name = "Luminara's Force Control";
        price = ShopPrice.SHOP_AVERAGE;
    }

    public override void Use()
    {
        if (Core.instance != null)
        {
            Core.instance.AddStatus(STATUS_TYPE.LUMINARA_FORCE, STATUS_APPLY_TYPE.ADDITIVE, 4f, 0, true);
            if (!Core.boons.Contains(STATUS_TYPE.LUMINARA_FORCE))
                Core.boons.Add(STATUS_TYPE.LUMINARA_FORCE);

            PlayerResources.AddResourceBy1(RewardType.REWARD_BOON, GetType());
        }
    }
}
public class MandoCode : GameResources
{
    public MandoCode() : base(298014769, RewardType.REWARD_BOON, 1.0f, "Deal +20% damage but have -20% less max health (stackable)")
    {
        name = "The Mandalorian's Code";
        price = ShopPrice.SHOP_AVERAGE;
    }

    public override void Use()
    {
        if (Core.instance != null)
        {
            Core.instance.AddStatus(STATUS_TYPE.MANDO_CODE, STATUS_APPLY_TYPE.ADDITIVE, 20f, 0, true);
            if (!Core.boons.Contains(STATUS_TYPE.MANDO_CODE))
                Core.boons.Add(STATUS_TYPE.MANDO_CODE);

            PlayerResources.AddResourceBy1(RewardType.REWARD_BOON, GetType());
        }
    }
}


public class ItsATrap : GameResources
{
    public ItsATrap() : base(561759673, RewardType.REWARD_BOON, 1.0f, "traps deal -33% damage to Mando")
    {
        name = "It's a trap!";
        price = ShopPrice.SHOP_AVERAGE;
    }

    public override void Use()
    {
        if (Core.instance != null)
        {
            Core.instance.AddStatus(STATUS_TYPE.ITSA_TRAP, STATUS_APPLY_TYPE.ADDITIVE, -33f, 0, true);
            if (!Core.boons.Contains(STATUS_TYPE.ITSA_TRAP))
                Core.boons.Add(STATUS_TYPE.ITSA_TRAP);

            PlayerResources.AddResourceBy1(RewardType.REWARD_BOON, GetType());
        }
    }
}
public class WreckHeavyShot : GameResources
{
    public WreckHeavyShot() : base(678900863, RewardType.REWARD_BOON, 1.0f, "hitting an enemy with a slowed status applies +25% non-stackable extra slowed status for 3 seconds.")
    {
        name = "Wrecker's heavy shot";
        price = ShopPrice.SHOP_EXPENSIVE;
    }

    public override void Use()
    {
        if (Core.instance != null)
        {
            Core.instance.AddStatus(STATUS_TYPE.WRECK_HEAVY_SHOT, STATUS_APPLY_TYPE.ADDITIVE, 25f, 0, true);
            if (!Core.boons.Contains(STATUS_TYPE.WRECK_HEAVY_SHOT))
                Core.boons.Add(STATUS_TYPE.WRECK_HEAVY_SHOT);

            PlayerResources.AddResourceBy1(RewardType.REWARD_BOON, GetType());
        }
    }
}

public class MandoQuickCombo : GameResources
{
    public MandoQuickCombo() : base(1246718882, RewardType.REWARD_BOON, 1.0f, "hitting an enemy with the primary weapon makes them take +5 % more damage from the primary weapon for 5 seconds. % stacks up to 100%")
    {
        name = "Mandalorian Quick Combo";
        price = ShopPrice.SHOP_AVERAGE;
    }

    public override void Use()
    {
        if (Core.instance != null)
        {
            Core.instance.AddStatus(STATUS_TYPE.QUICK_COMBO, STATUS_APPLY_TYPE.ADDITIVE, 5f, 0, true);
            if (!Core.boons.Contains(STATUS_TYPE.QUICK_COMBO))
                Core.boons.Add(STATUS_TYPE.QUICK_COMBO);

            PlayerResources.AddResourceBy1(RewardType.REWARD_BOON, GetType());
        }
    }
}
public class BlastCannonMouthpiece : GameResources
{
    public BlastCannonMouthpiece() : base(1675434209, RewardType.REWARD_BOON, 1.0f, "Primary weapon does double damage while on 75% or more heat.")
    {
        name = "Blast Cannon Mouthpiece";
        price = ShopPrice.SHOP_CHEAP;
    }

    public override void Use()
    {
        if (Core.instance != null)
        {
            Core.instance.AddStatus(STATUS_TYPE.BLAST_CANNON, STATUS_APPLY_TYPE.ADDITIVE, 100f, 0, true);
            if (!Core.boons.Contains(STATUS_TYPE.BLAST_CANNON))
                Core.boons.Add(STATUS_TYPE.BLAST_CANNON);

            PlayerResources.AddResourceBy1(RewardType.REWARD_BOON, GetType());
        }
    }
}

public class BountyHunter : GameResources
{
    public BountyHunter() : base(463877375, RewardType.REWARD_BOON, 1.0f, "every time a room is cleared, +10 gold is received.")
    {
        name = "Cad Bane’s rocket boots";
        price = ShopPrice.SHOP_AVERAGE;
    }

    public override void Use()
    {
        if (Core.instance != null)
        {
            Core.instance.AddStatus(STATUS_TYPE.BOUNTY_HUNTER, STATUS_APPLY_TYPE.ADDITIVE, 10f, 0, true);
            if (!Core.boons.Contains(STATUS_TYPE.BOUNTY_HUNTER))
                Core.boons.Add(STATUS_TYPE.BOUNTY_HUNTER);

            PlayerResources.AddResourceBy1(RewardType.REWARD_BOON, GetType());
        }
    }
}
public class BosskSpecialAmmo : GameResources
{
    public BosskSpecialAmmo() : base(1238459342, RewardType.REWARD_BOON, 1.0f, "Grenade does double damage as long as more than one enemy is getting hit by it.")
    {
        name = "Cad Bane’s rocket boots";
        price = ShopPrice.SHOP_EXPENSIVE;
    }

    public override void Use()
    {
        if (Core.instance != null)
        {
            Core.instance.AddStatus(STATUS_TYPE.BOSSK_AMMO, STATUS_APPLY_TYPE.ADDITIVE, 100f, 0, true);
            if (!Core.boons.Contains(STATUS_TYPE.BOSSK_AMMO))
                Core.boons.Add(STATUS_TYPE.BOSSK_AMMO);

            PlayerResources.AddResourceBy1(RewardType.REWARD_BOON, GetType());
        }
    }
}

public class WinduForceControl : GameResources
{
    public WinduForceControl() : base(569941476, RewardType.REWARD_BOON, 1.0f, "when you kill an enemy, gain Force points equal to 5% of max Force")
    {
        name = "Cad Bane’s rocket boots";
        price = ShopPrice.SHOP_EXPENSIVE;
    }

    public override void Use()
    {
        if (Core.instance != null)
        {
            Core.instance.AddStatus(STATUS_TYPE.WINDU_FORCE, STATUS_APPLY_TYPE.ADDITIVE, 5f, 0, true);
            if (!Core.boons.Contains(STATUS_TYPE.WINDU_FORCE))
                Core.boons.Add(STATUS_TYPE.WINDU_FORCE);

            PlayerResources.AddResourceBy1(RewardType.REWARD_BOON, GetType());
        }
    }
}

public class YodaForceControl : GameResources
{
    public YodaForceControl() : base(840154977, RewardType.REWARD_BOON, 1.0f, "Grogu’s push pushes double the range.")
    {
        name = "Cad Bane’s rocket boots";
        price = ShopPrice.SHOP_EXPENSIVE;
    }

    public override void Use()
    {
        if (Core.instance != null)
        {
            Core.instance.AddStatus(STATUS_TYPE.YODA_FORCE, STATUS_APPLY_TYPE.ADDITIVE, 2f, 0, true);
            if (!Core.boons.Contains(STATUS_TYPE.YODA_FORCE))
                Core.boons.Add(STATUS_TYPE.YODA_FORCE);

            PlayerResources.AddResourceBy1(RewardType.REWARD_BOON, GetType());
        }
    }
}

public class CossHairLuckyShot : GameResources
{
    public CossHairLuckyShot() : base(1240646973, RewardType.REWARD_BOON, 1.0f, "After impacting a sniper shot, you have a 33% chance to recover ammo, and next shot deals +33% more damage.")
    {
        name = "Cad Bane’s rocket boots";
        price = ShopPrice.SHOP_EXPENSIVE;
    }

    public override void Use()
    {
        if (Core.instance != null)
        {
            Core.instance.AddStatus(STATUS_TYPE.CROSS_HAIR_LUCKY_SHOT, STATUS_APPLY_TYPE.ADDITIVE, 33f, 0, true);
            if (!Core.boons.Contains(STATUS_TYPE.CROSS_HAIR_LUCKY_SHOT))
                Core.boons.Add(STATUS_TYPE.CROSS_HAIR_LUCKY_SHOT);

            PlayerResources.AddResourceBy1(RewardType.REWARD_BOON, GetType());
        }
    }
}
#endregion
static class BoonDataHolder
{
    static BoonDataHolder()
    {
        boonType[(int)BOONS.BOON_ANAKIN_KILL_STREAK]        = new CossHairLuckyShot();
        boonType[(int)BOONS.BOON_BOKATAN_RESILENCE]         = new CossHairLuckyShot();
        boonType[(int)BOONS.BOON_BOSSK_STRENGTH]            = new CossHairLuckyShot();
        boonType[(int)BOONS.BOON_BOUNTY_HUNTER_SKILLS]      = new CossHairLuckyShot();
        boonType[(int)BOONS.BOON_CADBANE_ROCKET_BOOTS]      = new CossHairLuckyShot();
        boonType[(int)BOONS.BOON_CADBANE_SLEIGHT]           = new CossHairLuckyShot();
        boonType[(int)BOONS.BOON_ECHO_QUICK_RECOVERY]       = new CossHairLuckyShot();
        boonType[(int)BOONS.BOON_GREEF_PAYCHECK]            = new CossHairLuckyShot();
        boonType[(int)BOONS.BOON_ITS_A_TRAP]                = new CossHairLuckyShot();
        boonType[(int)BOONS.BOON_MANDALORIAN_QUICK_DRAW]    = new CossHairLuckyShot();
        boonType[(int)BOONS.BOON_REX_SECOND_BLASTER]        = new CossHairLuckyShot();
        boonType[(int)BOONS.BOON_SOLO_QUICK_DRAW]           = new CossHairLuckyShot();
        boonType[(int)BOONS.BOON_WATTOS_COOLANT]            = new CossHairLuckyShot();
        boonType[(int)BOONS.BOON_WRECKER_HEAVY_SHOT]        = new CossHairLuckyShot();
        boonType[(int)BOONS.BOON_WRECKER_RESILIENCE]        = new CossHairLuckyShot();
        boonType[(int)BOONS.BOON_BLAST_CANNON_MOUTHPIECE]   = new CossHairLuckyShot();
        boonType[(int)BOONS.BOON_GREEDO_QUICKSHOOTER]       = new CossHairLuckyShot();
        boonType[(int)BOONS.BOON_MANDALORIAN_QUICK_COMBO]   = new CossHairLuckyShot();
        boonType[(int)BOONS.BOON_AHSOKA_DETERMINATION]      = new CossHairLuckyShot();
        boonType[(int)BOONS.BOON_FENNEC_SNIPER_RIFLE]       = new CossHairLuckyShot();
        boonType[(int)BOONS.BOON_BOBBA_FETT_STUN_AMM]       = new CossHairLuckyShot();
        boonType[(int)BOONS.BOON_BOSSK_SPECIAL_AMMO]        = new CossHairLuckyShot();
        boonType[(int)BOONS.BOON_GEOTERMAL_MARKER]          = new CossHairLuckyShot();
        boonType[(int)BOONS.BOON_MASTER_LUMINARA_FORCE]     = new CossHairLuckyShot();
        boonType[(int)BOONS.BOON_MASTER_WINDU_FORCE]        = new CossHairLuckyShot();
        boonType[(int)BOONS.BOON_MASTER_YODA_FORCE]         = new CossHairLuckyShot();
        boonType[(int)BOONS.BOON_MANDALORIAN_CODE]          = new CossHairLuckyShot();
        boonType[(int)BOONS.CROSSHAIR_LUCKY_SHOT]           = new CossHairLuckyShot();
        for (int i = 0; i < boonType.Length; i++)
        {
            float auxWeight = boonType[i].rngChanceWeight;
            boonType[i].rngChanceWeight = boonTotalWeights;
            boonTotalWeights += auxWeight;
        }
    }

    public static GameResources[] boonType = new GameResources[(int)BOONS.BOON_MAX];

    public static float boonTotalWeights;

}

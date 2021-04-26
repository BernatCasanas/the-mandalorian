using System;
using System.Collections.Generic;
using DiamondEngine;

namespace DiamondEngine
{
    public class Skills
    {
        public String description = " ";
        public RewardType type_of_price = RewardType.REWARD_BESKAR;
        public int price = 0;
        public virtual void Use()
        {
        }

        public virtual void AssignCharacteristics()
        {
            description = "Sorry, but this skill is currently unavailable. Please wait for the next update.";
        }
    }

    #region Mando Skills

    #region Utility Skills
    public class UtilityKnockbackSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.MANDO, (int)Skill_Tree_Data.MandoSkillNames.UTILITY1);
        }

        //public override void AssignCharacteristics()
        //{
        //    description = "Knockback Skill. Press 'A' to buy it. Price: it's free!";
        //}
    }

    public class UtilityMovementSpeedSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.MANDO, (int)Skill_Tree_Data.MandoSkillNames.UTILITY2);
        }

    }

    public class UtilityIncreaseDamageSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.MANDO, (int)Skill_Tree_Data.MandoSkillNames.UTILITY_INCREASE_DAMAGE_WHEN_GROGU);
        }

        public override void AssignCharacteristics()
        {
            description = "Mando has 15% increased damage for 10 seconds after Grogu uses his Push skill";
            price = 0;
            type_of_price = RewardType.REWARD_BESKAR;
        }
    }

    public class UtilityDamageReductionSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.MANDO, (int)Skill_Tree_Data.MandoSkillNames.UTILITY_DAMAGE_REDUCTION_DASH);
        }

        public override void AssignCharacteristics()
        {
            description = "20% Damage reduction 2 seconds after dash (doesn't attack)";
            price = 0;
            type_of_price = RewardType.REWARD_BESKAR;
        }
    }

    public class UtilityOverheatSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.MANDO, (int)Skill_Tree_Data.MandoSkillNames.UTILITY5);
        }
    }


    public class UtilityReductionGroguCostSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.MANDO, (int)Skill_Tree_Data.MandoSkillNames.UTILITY6);
        }
    }

    public class UtilityHealSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.MANDO, (int)Skill_Tree_Data.MandoSkillNames.UTILITY_HEAL_WHEN_GROGU_SKILL);
        }

        public override void AssignCharacteristics()
        {
            description = "Mando heals 10 HP when Grogu uses a Skill";
            price = 0;
            type_of_price = RewardType.REWARD_BESKAR;
        }
    }


    public class UtilityDashSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.MANDO, (int)Skill_Tree_Data.MandoSkillNames.UTILITY_CONSECUTIVE_DASH);
            Core.instance.maxDashNumber = Skill_Tree_Data.GetMandoSkillTree().U8_consecutiveDashAmount;
        }

        public override void AssignCharacteristics()
        {
            description = "Dash can be used consecutively twice in a row (Normal CD upon double dashing)";
            price = 0;
            type_of_price = RewardType.REWARD_BESKAR;
        }
    }

    #endregion

    #region Aggression Skills
    public class AggressionDamageSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.MANDO, (int)Skill_Tree_Data.MandoSkillNames.AGGRESION1);
        }

        //public override void AssignCharacteristics()
        //{
        //    description = "Increase Damage Skill. Press 'A' to buy it. Price: it's free!";
        //}
    }

    public class AggressionFireRateSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.MANDO, (int)Skill_Tree_Data.MandoSkillNames.AGGRESION2);
        }
    }

    public class AggressionComboDamageSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.MANDO, (int)Skill_Tree_Data.MandoSkillNames.AGGRESION3);
        }
    }

    public class AggressionCriticalChanceSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.MANDO, (int)Skill_Tree_Data.MandoSkillNames.AGGRESION4);
        }
    }

    public class AggressionCriticalDamageSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.MANDO, (int)Skill_Tree_Data.MandoSkillNames.AGGRESION5);
        }
    }


    public class AggressionDamageBossesSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.MANDO, (int)Skill_Tree_Data.MandoSkillNames.AGGRESION_INCREASE_DAMAGE_TO_BOSS);
        }

        public override void AssignCharacteristics()
        {
            description = "Increase damage to Bosses and greater enemies by 20%";
            price = 0;
            type_of_price = RewardType.REWARD_BESKAR;
        }
    }

    public class AggressionHPMissingDamageSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.MANDO, (int)Skill_Tree_Data.MandoSkillNames.AGGRESION_EXTRA_DAMAGE_LOW_HEALTH);
        }

        public override void AssignCharacteristics()
        {
            description = "For each 1% of hp missing, gain 1% damage";
            price = 0;
            type_of_price = RewardType.REWARD_BESKAR;
        }
    }


    public class AggressionHPMissingCriticalChanceSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.MANDO, (int)Skill_Tree_Data.MandoSkillNames.AGGRESION8);
        }
    }

    #endregion

    #region Defense Skills
    public class DefenseMaxHPSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.MANDO, (int)Skill_Tree_Data.MandoSkillNames.DEFENSE1);
            if (Core.instance != null)
            {
                int newHP = Core.instance.gameObject.GetComponent<PlayerHealth>().IncrementMaxHpPercent(0.1f, true);
            }
        }

        public override void AssignCharacteristics()
        {
            description = "Increase Mando's Max HP by 10%. Press 'A' to buy it. Price: 1 Beskar Ingot";
            price = 0;
            type_of_price = RewardType.REWARD_BESKAR;
        }
    }

    public class DefenseDamageReductionSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.MANDO, (int)Skill_Tree_Data.MandoSkillNames.DEFENSE2);
        }

        public override void AssignCharacteristics()
        {
            description = "Decrease the Damage Reduction Skill. Press 'A' to buy it. Price: it's free!";
            price = 0;
            type_of_price = RewardType.REWARD_BESKAR;
        }
    }

    public class DefenseComboDamageReductionSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.MANDO, (int)Skill_Tree_Data.MandoSkillNames.DEFENSE3);
        }
    }

    public class DefenseComboHealSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.MANDO, (int)Skill_Tree_Data.MandoSkillNames.DEFENSE4);
        }
    }

    public class DefenseComboFinishHealSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.MANDO, (int)Skill_Tree_Data.MandoSkillNames.DEFENSE5);
        }
    }


    public class DefenseHealSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.MANDO, (int)Skill_Tree_Data.MandoSkillNames.DEFENSE6);
        }
    }

    public class DefenseHealEffectsSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.MANDO, (int)Skill_Tree_Data.MandoSkillNames.DEFENSE7);
        }
    }


    public class DefenseAvoidDamageSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.MANDO, (int)Skill_Tree_Data.MandoSkillNames.DEFENSE_CHANCE_AVOID_DAMAGE);
        }

        public override void AssignCharacteristics()
        {
            description = "10% chance to avoid damage";
            price = 0;
            type_of_price = RewardType.REWARD_BESKAR;
        }
    }
    #endregion

    #endregion

    #region Grogu Skills

    public class GroguForceRegenerationSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.GROGU, (int)Skill_Tree_Data.GroguSkillNames.Skill1);
        }
    }

    public class GroguPushRangeSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.GROGU, (int)Skill_Tree_Data.GroguSkillNames.Skill2);
        }
    }

    public class GroguForceDurationSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.GROGU, (int)Skill_Tree_Data.GroguSkillNames.Skill3);
        }
    }

    public class GroguThresholdComboForceSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.GROGU, (int)Skill_Tree_Data.GroguSkillNames.Skill4);
        }
    }

    public class GroguMaxForceSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.GROGU, (int)Skill_Tree_Data.GroguSkillNames.Skill5);
        }
    }


    public class GroguSingularComboForceSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.GROGU, (int)Skill_Tree_Data.GroguSkillNames.Skill6);
        }
    }

    public class GroguComboTimerSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.GROGU, (int)Skill_Tree_Data.GroguSkillNames.Skill7);
        }
    }


    public class GroguHPForceRegenerationSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.GROGU, (int)Skill_Tree_Data.GroguSkillNames.FORCE_REGENERATION);
        }

        public override void AssignCharacteristics()
        {
            description = "For each 10% of HP Mando is missing, gain 1 more passive Force Regeneration per second.";
            price = 0;
            type_of_price = RewardType.REWARD_MACARON;
        }
    }

    #endregion

    #region Weapons Skills

    #region Primary Weapon Skills

    public class PrimaryMovementSpeedSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.WEAPONS, (int)Skill_Tree_Data.WeaponsSkillNames.PRIMARY1);
        }
    }

    public class PrimaryProjectileRangeSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.WEAPONS, (int)Skill_Tree_Data.WeaponsSkillNames.PRIMARY2);
        }
    }

    public class PrimarySlowEnemiesSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.WEAPONS, (int)Skill_Tree_Data.WeaponsSkillNames.PRIMARY_SLOW_SPEED);
        }
        public override void AssignCharacteristics()
        {
            description = "Bullet impacts slow enemy speed by 20% for 3 seconds";
            price = 0;
            type_of_price = RewardType.REWARD_BESKAR;
        }
    }

    public class PrimaryCriticalChanceSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.WEAPONS, (int)Skill_Tree_Data.WeaponsSkillNames.PRIMARY4);
        }
    }

    public class PrimaryCriticalDamageSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.WEAPONS, (int)Skill_Tree_Data.WeaponsSkillNames.PRIMARY5);
        }
    }


    public class PrimaryIncreaseDamageSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.WEAPONS, (int)Skill_Tree_Data.WeaponsSkillNames.PRIMARY7);
            if(Core.instance != null && Skill_Tree_Data.GetWeaponsSkillTree() != null)
            {
                Core.instance.IncreaseNormalShootDamage(Skill_Tree_Data.GetWeaponsSkillTree().PW6_IncreaseDamageAmount);
            }
        }

        public override void AssignCharacteristics()
        {
            description = "Increase Mando's Primary Weapon damage by 25%. Press 'A' to buy it. Price: 1 Imperial Scrap";
            price = 0;
            type_of_price = RewardType.REWARD_SCRAP;
        }
    }

    public class PrimaryRateFireSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.WEAPONS, (int)Skill_Tree_Data.WeaponsSkillNames.PRIMARY7);
        }
    }


    #endregion

    #region Secondary Weapon Skills

    public class SecondaryStatusEffectSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.WEAPONS, (int)Skill_Tree_Data.WeaponsSkillNames.SECONDARY1);
        }
    }

    public class SecondaryCriticalChanceSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.WEAPONS, (int)Skill_Tree_Data.WeaponsSkillNames.SECONDARY2);
        }
    }

    public class SecondaryDelaySkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.WEAPONS, (int)Skill_Tree_Data.WeaponsSkillNames.SECONDARY_DELAY_BETWEEN_USES);
        }

        public override void AssignCharacteristics()
        {
            description = "[NOT IMPLEMENTED] Delay between uses reduced by 30%";
            price = 0;
            type_of_price = RewardType.REWARD_BESKAR;
        }
    }

    public class SecondaryUseRangeSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.WEAPONS, (int)Skill_Tree_Data.WeaponsSkillNames.SECONDARY4);
        }
    }

    public class SecondaryCriticalDamageSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.WEAPONS, (int)Skill_Tree_Data.WeaponsSkillNames.SECONDARY5);
        }
    }


    public class SecondaryDamageIncreaseSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.WEAPONS, (int)Skill_Tree_Data.WeaponsSkillNames.SECONDARY6);
        }
    }


    #endregion

    #region Special Weapon Skills

    public class SpecialCriticalChanceSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.WEAPONS, (int)Skill_Tree_Data.WeaponsSkillNames.SPECIAL1);
        }
    }

    public class SpecialChargeTimeSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.WEAPONS, (int)Skill_Tree_Data.WeaponsSkillNames.SPECIAL2);
        }
    }

    public class SpecialBulletsSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.WEAPONS, (int)Skill_Tree_Data.WeaponsSkillNames.SPECIAL3);
        }
    }

    public class SpecialRegenerateForceSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.WEAPONS, (int)Skill_Tree_Data.WeaponsSkillNames.SPECIAL4);
        }
    }

    public class SpecialCriticalDamageSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.WEAPONS, (int)Skill_Tree_Data.WeaponsSkillNames.SPECIAL5);
        }
    }


    public class SpecialMaximumDamageSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.WEAPONS, (int)Skill_Tree_Data.WeaponsSkillNames.SPECIAL6);
        }
    }


    #endregion

    #endregion


    public class SkillDictionary
    {
        public static Dictionary<string, Type> skill_type = new Dictionary<string, Type>
        {
            {"UKnockback", typeof(UtilityKnockbackSkill)},
            {"UMovSpd", typeof(UtilityMovementSpeedSkill)},
            {"UIncDmg", typeof(UtilityIncreaseDamageSkill)},
            {"UDmgRed", typeof(UtilityDamageReductionSkill)},
            {"UOverheat", typeof(UtilityOverheatSkill)},
            {"URedGroguCost", typeof(UtilityReductionGroguCostSkill)},
            {"UHeal", typeof(UtilityHealSkill)},
            {"UDash", typeof(UtilityDashSkill)},
            {"ADmg", typeof(AggressionDamageSkill)},
            {"AFireRate", typeof(AggressionFireRateSkill)},
            {"AComboDmg", typeof(AggressionComboDamageSkill)},
            {"ACritChance", typeof(AggressionCriticalChanceSkill)},
            {"ACritDmg", typeof(AggressionCriticalDamageSkill)},
            {"ADmgBos", typeof(AggressionDamageBossesSkill)},
            {"AHPMissDmg", typeof(AggressionHPMissingDamageSkill)},
            {"AHPMissCritChance", typeof(AggressionHPMissingCriticalChanceSkill)},
            {"DMaxHPS", typeof(DefenseMaxHPSkill)},
            {"DDmgRed", typeof(DefenseDamageReductionSkill)},
            {"DComboDmgRed", typeof(DefenseComboDamageReductionSkill)},
            {"DComboHeal", typeof(DefenseComboHealSkill)},
            {"DComboFnshHeal", typeof(DefenseComboFinishHealSkill)},
            {"DHeal", typeof(DefenseHealSkill)},
            {"DHealEfct", typeof(DefenseHealEffectsSkill)},
            {"DAvoidDmg", typeof(DefenseAvoidDamageSkill)},
            {"GForceReg", typeof(GroguForceRegenerationSkill)},
            {"GPushRange", typeof(GroguPushRangeSkill)},
            {"GForceDur", typeof(GroguForceDurationSkill)},
            {"GThrshComboForce", typeof(GroguThresholdComboForceSkill)},
            {"GMaxForce", typeof(GroguMaxForceSkill)},
            {"GSingComboForce", typeof(GroguSingularComboForceSkill)},
            {"GComboTimer", typeof(GroguComboTimerSkill)},
            {"GHPForceReg", typeof(GroguHPForceRegenerationSkill)},
            {"PMovSpd", typeof(PrimaryMovementSpeedSkill)},
            {"PProjRange", typeof(PrimaryProjectileRangeSkill)},
            {"PSlowEnem", typeof(PrimarySlowEnemiesSkill)},
            {"PCritChance", typeof(PrimaryCriticalChanceSkill)},
            {"PCritDmg", typeof(PrimaryCriticalDamageSkill)},
            {"PIncDmg", typeof(PrimaryIncreaseDamageSkill)},
            {"PRateFire", typeof(PrimaryRateFireSkill)},
            {"SeStatEfct", typeof(SecondaryStatusEffectSkill)},
            {"SeCritChance", typeof(SecondaryCriticalChanceSkill)},
            {"SeDelay", typeof(SecondaryDelaySkill)},
            {"SeUseRange", typeof(SecondaryUseRangeSkill)},
            {"SeCritDmg", typeof(SecondaryCriticalDamageSkill)},
            {"SeDmgInc", typeof(SecondaryDamageIncreaseSkill)},
            {"SpCritChance", typeof(SpecialCriticalChanceSkill)},
            {"SpChargeTime", typeof(SpecialChargeTimeSkill)},
            {"SpBullet", typeof(SpecialBulletsSkill)},
            {"SpRegForce", typeof(SpecialRegenerateForceSkill)},
            {"SpCritDmg", typeof(SpecialCriticalDamageSkill)},
            {"SpMaxDmg", typeof(SpecialMaximumDamageSkill)},
        };
    }
}
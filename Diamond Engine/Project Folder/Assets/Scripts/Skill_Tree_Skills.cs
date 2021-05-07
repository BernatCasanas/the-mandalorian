using System;
using System.Collections.Generic;
using DiamondEngine;

namespace DiamondEngine
{
    public class Skills
    {
        public String description = " ";
        public RewardType type_of_price = RewardType.REWARD_BESKAR;
        public int price = 1;
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
    public class UtilityDamagePerHeat : Skills
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

    public class UtilitySlowDamageSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.MANDO, (int)Skill_Tree_Data.MandoSkillNames.UTILITY_INCREASE_DAMAGE_WHEN_GROGU);
        }

        public override void AssignCharacteristics()
        {
            description = Skill_Tree_Data.GetMandoSkillTree().U3_description;
            type_of_price = RewardType.REWARD_BESKAR;
        }
    }

    public class UtilityFallDamageReductionSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.MANDO, (int)Skill_Tree_Data.MandoSkillNames.UTILITY_DAMAGE_REDUCTION_DASH);
        }

        public override void AssignCharacteristics()
        {
            description = Skill_Tree_Data.GetMandoSkillTree().U4_description;
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
            description = Skill_Tree_Data.GetMandoSkillTree().U7_description;
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
            description = Skill_Tree_Data.GetMandoSkillTree().U8_description;
            type_of_price = RewardType.REWARD_BESKAR;
        }
    }

    #endregion

    #region Aggression Skills
    public class AggressionBlasterDamageSkill : Skills
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

    public class AggressionGrenadeDamageSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.MANDO, (int)Skill_Tree_Data.MandoSkillNames.AGGRESION4);
        }
    }

    public class AggressionSniperDamageSkill : Skills
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
            description = Skill_Tree_Data.GetMandoSkillTree().A6_description;
            type_of_price = RewardType.REWARD_BESKAR;
        }
    }

    public class AggressionHPMissDamageBlasterSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.MANDO, (int)Skill_Tree_Data.MandoSkillNames.AGGRESION_EXTRA_DAMAGE_LOW_HEALTH);
        }

        public override void AssignCharacteristics()
        {
            description = Skill_Tree_Data.GetMandoSkillTree().A7_description;
            type_of_price = RewardType.REWARD_BESKAR;
        }
    }


    public class AggressionHPMissDamageSniperSkill : Skills
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
            //description = Skill_Tree_Data.GetMandoSkillTree().D1_description;
            description = "Increase Mando's Max HP by 10%. Press 'A' to buy it. Price: 1 Beskar Ingot";            
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
            description = Skill_Tree_Data.GetMandoSkillTree().D8_description;
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
            description = Skill_Tree_Data.GetGroguSkillTree().G8_description;         
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
            description = Skill_Tree_Data.GetWeaponsSkillTree().PrimaryW3_description;
            description = "Bullet impacts slow enemy speed by 20% for 3 seconds";            
            type_of_price = RewardType.REWARD_BESKAR;
        }
    }

    public class PrimaryOverheatSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.WEAPONS, (int)Skill_Tree_Data.WeaponsSkillNames.PRIMARY4);
        }
    }

    public class PrimaryChargedkill : Skills
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
            description = Skill_Tree_Data.GetWeaponsSkillTree().PrimaryW6_description;
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
            description = Skill_Tree_Data.GetWeaponsSkillTree().SecondaryW4_description;
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
            {"UDamageHeat", typeof(UtilityDamagePerHeat)},
            {"UMovSpd", typeof(UtilityMovementSpeedSkill)},
            {"USlowDamage", typeof(UtilitySlowDamageSkill)},
            {"UFallDmgRed", typeof(UtilityFallDamageReductionSkill)},
            {"UOverheat", typeof(UtilityOverheatSkill)},
            {"URedGroguCost", typeof(UtilityReductionGroguCostSkill)},
            {"UHeal", typeof(UtilityHealSkill)},
            {"UDash", typeof(UtilityDashSkill)},
            {"ABlasterDmg", typeof(AggressionBlasterDamageSkill)},
            {"AComboFireRate", typeof(AggressionFireRateSkill)},
            {"AComboDmg", typeof(AggressionComboDamageSkill)},
            {"AGrenadeDmg", typeof(AggressionGrenadeDamageSkill)},
            {"ASniperDmg", typeof(AggressionSniperDamageSkill)},
            {"ADmgBos", typeof(AggressionDamageBossesSkill)},
            {"AHPMissDmgBlaster", typeof(AggressionHPMissDamageBlasterSkill)},
            {"AHPMissDmgSniper", typeof(AggressionHPMissDamageSniperSkill)},
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
            {"PHeat", typeof(PrimaryOverheatSkill)},
            {"PChargedBullet", typeof(PrimaryChargedkill)},
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
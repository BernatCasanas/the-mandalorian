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

        public override void AssignCharacteristics()
        {
            description = Skill_Tree_Data.GetMandoSkillTree().U1_description;
             
        }
    }

    public class UtilityMovementSpeedSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.MANDO, (int)Skill_Tree_Data.MandoSkillNames.UTILITY2);
        }
        public override void AssignCharacteristics()
        {
            description = Skill_Tree_Data.GetMandoSkillTree().U2_description;
             
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
             
        }
    }

    public class UtilityOverheatSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.MANDO, (int)Skill_Tree_Data.MandoSkillNames.UTILITY5);
        }
        public override void AssignCharacteristics()
        {
            description = Skill_Tree_Data.GetMandoSkillTree().U5_description;
             
        }
    }


    public class UtilityReductionGroguCostSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.MANDO, (int)Skill_Tree_Data.MandoSkillNames.UTILITY6);
        }
        public override void AssignCharacteristics()
        {
            description = Skill_Tree_Data.GetMandoSkillTree().U6_description;
             
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
             
        }
    }


    public class UtilityDashSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.MANDO, (int)Skill_Tree_Data.MandoSkillNames.UTILITY_CONSECUTIVE_DASH);
        }

        public override void AssignCharacteristics()
        {
            description = Skill_Tree_Data.GetMandoSkillTree().U8_description;
             
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

        public override void AssignCharacteristics()
        {
            description = Skill_Tree_Data.GetMandoSkillTree().A1_description;
             
        }
    }

    public class AggressionFireRateSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.MANDO, (int)Skill_Tree_Data.MandoSkillNames.AGGRESION2);
        }
        public override void AssignCharacteristics()
        {
            description = Skill_Tree_Data.GetMandoSkillTree().A2_description;
             
        }
    }

    public class AggressionComboDamageSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.MANDO, (int)Skill_Tree_Data.MandoSkillNames.AGGRESION3);
        }
        public override void AssignCharacteristics()
        {
            description = Skill_Tree_Data.GetMandoSkillTree().A3_description;
             
        }
    }

    public class AggressionGrenadeDamageSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.MANDO, (int)Skill_Tree_Data.MandoSkillNames.AGGRESION4);
        }
        public override void AssignCharacteristics()
        {
            description = Skill_Tree_Data.GetMandoSkillTree().A4_description;
             
        }
    }

    public class AggressionSniperDamageSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.MANDO, (int)Skill_Tree_Data.MandoSkillNames.AGGRESION5);
        }
        public override void AssignCharacteristics()
        {
            description = Skill_Tree_Data.GetMandoSkillTree().A5_description;
             
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
             
        }
    }


    public class AggressionHPMissDamageSniperSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.MANDO, (int)Skill_Tree_Data.MandoSkillNames.AGGRESION8);
        }
        public override void AssignCharacteristics()
        {
            description = Skill_Tree_Data.GetMandoSkillTree().A8_description;
             
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
            description = Skill_Tree_Data.GetMandoSkillTree().D1_description;        
             
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
            description = Skill_Tree_Data.GetMandoSkillTree().D2_description;
             
        }
    }

    public class DefenseComboDamageReductionSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.MANDO, (int)Skill_Tree_Data.MandoSkillNames.DEFENSE3);
        }
        public override void AssignCharacteristics()
        {
            description = Skill_Tree_Data.GetMandoSkillTree().D3_description;
             
        }
    }

    public class DefenseComboHealSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.MANDO, (int)Skill_Tree_Data.MandoSkillNames.DEFENSE4);
        }
        public override void AssignCharacteristics()
        {
            description = Skill_Tree_Data.GetMandoSkillTree().D4_description;
             
        }
    }

    public class DefenseComboFinishHealSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.MANDO, (int)Skill_Tree_Data.MandoSkillNames.DEFENSE5);
        }
        public override void AssignCharacteristics()
        {
            description = Skill_Tree_Data.GetMandoSkillTree().D5_description;
             
        }
    }


    public class DefenseHealSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.MANDO, (int)Skill_Tree_Data.MandoSkillNames.DEFENSE6);
        }
        public override void AssignCharacteristics()
        {
            description = Skill_Tree_Data.GetMandoSkillTree().D6_description;
             
        }
    }

    public class DefenseHealEffectsSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.MANDO, (int)Skill_Tree_Data.MandoSkillNames.DEFENSE7);
        }
        public override void AssignCharacteristics()
        {
            description = Skill_Tree_Data.GetMandoSkillTree().D7_description;
             
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
        public override void AssignCharacteristics()
        {
            description = Skill_Tree_Data.GetGroguSkillTree().G1_description;
             
        }
    }

    public class GroguPushRangeSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.GROGU, (int)Skill_Tree_Data.GroguSkillNames.Skill2);
        }
        public override void AssignCharacteristics()
        {
            description = Skill_Tree_Data.GetGroguSkillTree().G2_description;
             
        }
    }

    public class GroguForceDurationSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.GROGU, (int)Skill_Tree_Data.GroguSkillNames.Skill3);
        }
        public override void AssignCharacteristics()
        {
            description = Skill_Tree_Data.GetGroguSkillTree().G3_description;
             
        }
    }

    public class GroguThresholdComboForceSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.GROGU, (int)Skill_Tree_Data.GroguSkillNames.Skill4);
        }
        public override void AssignCharacteristics()
        {
            description = Skill_Tree_Data.GetGroguSkillTree().G4_description;
             
        }
    }

    public class GroguMaxForceSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.GROGU, (int)Skill_Tree_Data.GroguSkillNames.Skill5);
        }
        public override void AssignCharacteristics()
        {
            description = Skill_Tree_Data.GetGroguSkillTree().G5_description;
             
        }
    }


    public class GroguSingularComboForceSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.GROGU, (int)Skill_Tree_Data.GroguSkillNames.Skill6);
        }
        public override void AssignCharacteristics()
        {
            description = Skill_Tree_Data.GetGroguSkillTree().G6_description;
             
        }
    }

    public class GroguComboGainSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.GROGU, (int)Skill_Tree_Data.GroguSkillNames.Skill7);
        }
        public override void AssignCharacteristics()
        {
            description = Skill_Tree_Data.GetGroguSkillTree().G7_description;
             
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
        public override void AssignCharacteristics()
        {
            description = Skill_Tree_Data.GetWeaponsSkillTree().PrimaryW1_description;
             
        }
    }

    public class PrimaryProjectileRangeSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.WEAPONS, (int)Skill_Tree_Data.WeaponsSkillNames.PRIMARY2);
        }
        public override void AssignCharacteristics()
        {
            description = Skill_Tree_Data.GetWeaponsSkillTree().PrimaryW2_description;
             
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
             
        }
    }

    public class PrimaryOverheatSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.WEAPONS, (int)Skill_Tree_Data.WeaponsSkillNames.PRIMARY4);
        }
        public override void AssignCharacteristics()
        {
            description = Skill_Tree_Data.GetWeaponsSkillTree().PrimaryW4_description;
             
        }
    }

    public class PrimaryChargedkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.WEAPONS, (int)Skill_Tree_Data.WeaponsSkillNames.PRIMARY5);
        }
        public override void AssignCharacteristics()
        {
            description = Skill_Tree_Data.GetWeaponsSkillTree().PrimaryW5_description;
             
        }
    }


    public class PrimaryIncreaseDamageSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.WEAPONS, (int)Skill_Tree_Data.WeaponsSkillNames.PRIMARY7);
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
        public override void AssignCharacteristics()
        {
            description = Skill_Tree_Data.GetWeaponsSkillTree().PrimaryW7_description;
             
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
        public override void AssignCharacteristics()
        {
            description = Skill_Tree_Data.GetWeaponsSkillTree().SecondaryW1_description;
             
        }
    }

    public class SecondarySlowSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.WEAPONS, (int)Skill_Tree_Data.WeaponsSkillNames.SECONDARY2);
        }
        public override void AssignCharacteristics()
        {
            description = Skill_Tree_Data.GetWeaponsSkillTree().SecondaryW2_description;
             
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
            description = Skill_Tree_Data.GetWeaponsSkillTree().SecondaryW3_description;
             
        }
    }

    public class SecondaryUseRangeSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.WEAPONS, (int)Skill_Tree_Data.WeaponsSkillNames.SECONDARY4);
        }
        public override void AssignCharacteristics()
        {
            description = Skill_Tree_Data.GetWeaponsSkillTree().SecondaryW4_description;
             
        }
    }

    public class SecondaryDurationSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.WEAPONS, (int)Skill_Tree_Data.WeaponsSkillNames.SECONDARY5);
        }
        public override void AssignCharacteristics()
        {
            description = Skill_Tree_Data.GetWeaponsSkillTree().SecondaryW5_description;
             
        }
    }


    public class SecondaryDamageIncreaseSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.WEAPONS, (int)Skill_Tree_Data.WeaponsSkillNames.SECONDARY6);
        }
        public override void AssignCharacteristics()
        {
            description = Skill_Tree_Data.GetWeaponsSkillTree().SecondaryW6_description;
             
        }
    }


    #endregion

    #region Special Weapon Skills

    public class SpecialNonChargedSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.WEAPONS, (int)Skill_Tree_Data.WeaponsSkillNames.SPECIAL1);
        }
        public override void AssignCharacteristics()
        {
            description = Skill_Tree_Data.GetWeaponsSkillTree().SpecialW1_description;
             
        }
    }

    public class SpecialChargeTimeSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.WEAPONS, (int)Skill_Tree_Data.WeaponsSkillNames.SPECIAL2);
        }
        public override void AssignCharacteristics()
        {
            description = Skill_Tree_Data.GetWeaponsSkillTree().SpecialW2_description;
             
        }
    }

    public class SpecialHealSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.WEAPONS, (int)Skill_Tree_Data.WeaponsSkillNames.SPECIAL3);
        }
        public override void AssignCharacteristics()
        {
            description = Skill_Tree_Data.GetWeaponsSkillTree().SpecialW3_description;
             
        }
    }

    public class SpecialRegenerateForceSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.WEAPONS, (int)Skill_Tree_Data.WeaponsSkillNames.SPECIAL4);
        }
        public override void AssignCharacteristics()
        {
            description = Skill_Tree_Data.GetWeaponsSkillTree().SpecialW4_description;
             
        }
    }

    public class SpecialMaxIntervalSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.WEAPONS, (int)Skill_Tree_Data.WeaponsSkillNames.SPECIAL5);
        }
        public override void AssignCharacteristics()
        {
            description = Skill_Tree_Data.GetWeaponsSkillTree().SpecialW5_description;
        }
    }


    public class SpecialMaximumDamageSkill : Skills
    {
        public override void Use()
        {
            Skill_Tree_Data.EnableSkill((int)Skill_Tree_Data.SkillTreesNames.WEAPONS, (int)Skill_Tree_Data.WeaponsSkillNames.SPECIAL6);
        }
        public override void AssignCharacteristics()
        {
            description = Skill_Tree_Data.GetWeaponsSkillTree().SpecialW6_description;
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
            {"GComboGain", typeof(GroguComboGainSkill)},
            {"GHPForceReg", typeof(GroguHPForceRegenerationSkill)},
            {"PMovSpd", typeof(PrimaryMovementSpeedSkill)},
            {"PProjRange", typeof(PrimaryProjectileRangeSkill)},
            {"PSlowEnem", typeof(PrimarySlowEnemiesSkill)},
            {"PHeat", typeof(PrimaryOverheatSkill)},
            {"PChargedBullet", typeof(PrimaryChargedkill)},
            {"PIncDmg", typeof(PrimaryIncreaseDamageSkill)},
            {"PRateFire", typeof(PrimaryRateFireSkill)},
            {"SeStatEfct", typeof(SecondaryStatusEffectSkill)},
            {"SeSlow", typeof(SecondarySlowSkill)},
            {"SeDelay", typeof(SecondaryDelaySkill)},
            {"SeUseRange", typeof(SecondaryUseRangeSkill)},
            {"SeDuration", typeof(SecondaryDurationSkill)},
            {"SeDmgInc", typeof(SecondaryDamageIncreaseSkill)},
            {"SpNonCharged", typeof(SpecialNonChargedSkill)},
            {"SpChargeTime", typeof(SpecialChargeTimeSkill)},
            {"SpHeal", typeof(SpecialHealSkill)},
            {"SpRegForce", typeof(SpecialRegenerateForceSkill)},
            {"SpMaxIntDmg", typeof(SpecialMaxIntervalSkill)},
            {"SpMaxDmg", typeof(SpecialMaximumDamageSkill)},
        };
    }
}
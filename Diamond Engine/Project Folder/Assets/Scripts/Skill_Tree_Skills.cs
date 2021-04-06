using System;
using System.Collections.Generic;
using DiamondEngine;

namespace DiamondEngine {
	public class Skills
    {
        bool active = false;
		public virtual void Use()
		{
            active = true;
		}
	}

    #region Mando Skills

    #region Utility Skills
    public class UtilityKnockbackSkill : Skills
    {
        public override void Use()
        {
        }
    }

    public class UtilityMovementSpeedSkill : Skills
    {
        public override void Use()
        {
        }
    }

    public class UtilityIncreaseDamageSkill : Skills
    {
        public override void Use()
        {
        }
    }

    public class UtilityDamageReductionSkill : Skills
    {
        public override void Use()
        {
        }
    }

    public class UtilityOverheatSkill : Skills
    {
        public override void Use()
        {
        }
    }


    public class UtilityReductionGroguCostSkill : Skills
    {
        public override void Use()
        {
        }
    }

    public class UtilityHealSkill : Skills
    {
        public override void Use()
        {
        }
    }


    public class UtilityDashSkill : Skills
    {
        public override void Use()
        {
        }
    }

    #endregion

    #region Aggression Skills
    public class AggressionDamageSkill : Skills
    {
        public override void Use()
        {
        }
    }

    public class AggressionFireRateSkill : Skills
    {
        public override void Use()
        {
        }
    }

    public class AggressionComboDamageSkill : Skills
    {
        public override void Use()
        {
        }
    }

    public class AggressionCriticalChanceSkill : Skills
    {
        public override void Use()
        {
        }
    }

    public class AggressionCriticalDamageSkill : Skills
    {
        public override void Use()
        {
        }
    }


    public class AggressionDamageBossesSkill : Skills
    {
        public override void Use()
        {
        }
    }

    public class AggressionHPMissingDamageSkill : Skills
    {
        public override void Use()
        {
        }
    }


    public class AggressionHPMissingCriticalChanceSkill : Skills
    {
        public override void Use()
        {
        }
    }

    #endregion

    #region Defense Skills
    public class DefenseMaxHPSkill : Skills
    {
        public override void Use()
        {
        }
    }

    public class DefenseDamageReductionSkill : Skills
    {
        public override void Use()
        {
        }
    }

    public class DefenseComboDamageReductionSkill : Skills
    {
        public override void Use()
        {
        }
    }

    public class DefenseComboHealSkill : Skills
    {
        public override void Use()
        {
        }
    }

    public class DefenseComboFinishHealSkill : Skills
    {
        public override void Use()
        {
        }
    }


    public class DefenseHealSkill : Skills
    {
        public override void Use()
        {
        }
    }

    public class DefenseHealEffectsSkill : Skills
    {
        public override void Use()
        {
        }
    }


    public class DefenseAvoidDamageSkill : Skills
    {
        public override void Use()
        {
        }
    }
    #endregion

    #endregion

    #region Grogu Skills

    public class GroguForceRegenerationSkill : Skills
    {
        public override void Use()
        {
        }
    }

    public class GroguPushRangeSkill : Skills
    {
        public override void Use()
        {
        }
    }

    public class GroguForceDurationSkill : Skills
    {
        public override void Use()
        {
        }
    }

    public class GroguThresholdComboForceSkill : Skills
    {
        public override void Use()
        {
        }
    }

    public class GroguMaxForceSkill : Skills
    {
        public override void Use()
        {
        }
    }


    public class GroguSingularComboForceSkill : Skills
    {
        public override void Use()
        {
        }
    }

    public class GroguComboTimerSkill : Skills
    {
        public override void Use()
        {
        }
    }


    public class GroguHPForceRegenerationSkill : Skills
    {
        public override void Use()
        {
        }
    }

    #endregion

    #region Weapons Skills

    #region Primary Weapon Skills

    public class PrimaryMovementSpeedSkill : Skills
    {
        public override void Use()
        {
        }
    }

    public class PrimaryProjectileRangeSkill : Skills
    {
        public override void Use()
        {
        }
    }

    public class PrimarySlowEnemiesSkill : Skills
    {
        public override void Use()
        {
        }
    }

    public class PrimaryCriticalChanceSkill : Skills
    {
        public override void Use()
        {
        }
    }

    public class PrimaryCriticalDamageSkill : Skills
    {
        public override void Use()
        {
        }
    }


    public class PrimaryIncreaseDamageSkill : Skills
    {
        public override void Use()
        {
        }
    }

    public class PrimaryRateFireSkill : Skills
    {
        public override void Use()
        {
        }
    }


    #endregion

    #region Secondary Weapon Skills

    public class SecondaryStatusEffectSkill : Skills
    {
        public override void Use()
        {
        }
    }

    public class SecondaryCriticalChanceSkill : Skills
    {
        public override void Use()
        {
        }
    }

    public class SecondaryDelaySkill : Skills
    {
        public override void Use()
        {
        }
    }

    public class SecondaryUseRangeSkill : Skills
    {
        public override void Use()
        {
        }
    }

    public class SecondaryCriticalDamageSkill : Skills
    {
        public override void Use()
        {
        }
    }


    public class SecondaryDamageIncreaseSkill : Skills
    {
        public override void Use()
        {
        }
    }


    #endregion

    #region Special Weapon Skills

    public class SpecialCriticalChanceSkill : Skills
    {
        public override void Use()
        {
        }
    }

    public class SpecialChargeTimeSkill : Skills
    {
        public override void Use()
        {
        }
    }

    public class SpecialBulletsSkill : Skills
    {
        public override void Use()
        {
        }
    }

    public class SpecialRegenerateForceSkill : Skills
    {
        public override void Use()
        {
        }
    }

    public class SpecialCriticalDamageSkill : Skills
    {
        public override void Use()
        {
        }
    }


    public class SpecialMaximumDamageSkill : Skills
    {
        public override void Use()
        {
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
using System;
using DiamondEngine;
using System.Collections.Generic;

public class Skill_Tree_Data : DiamondComponent
{
    public static Grogu_Skills_Data groguSkillTree = null;
    public static Weapons_Skills_Data weaponsSkillTree = null;
    public static Mando_Skills_Data mandoSkillTree = null;

    #region skillNames
    public enum SkillTreesNames
    {
        GROGU = 1,
        MANDO = 2,
        WEAPONS = 3,
    }
    public enum GroguSkillNames
    {
        Skill1 = 1,
        Skill2 = 2,
        Skill3 = 3,
        Skill4 = 4,
        Skill5 = 5,
        Skill6 = 6,
        Skill7 = 7,
        FORCE_REGENERATION = 8,
    }
    public enum WeaponsSkillNames
    {
        PRIMARY1 = 1,
        PRIMARY2 = 2,        
        PRIMARY_SLOW_SPEED = 3,
        PRIMARY4 = 4,
        PRIMARY5 = 5,
        PRIMARY_INCREASE_DAMAGE = 3,
        PRIMARY7 = 7,
        SECONDARY1 = 8,
        SECONDARY2 = 9,
        SECONDARY_DELAY_BETWEEN_USES = 10,
        SECONDARY4 = 11,
        SECONDARY5 = 12,
        SECONDARY6 = 13,
        SPECIAL1 = 14,
        SPECIAL2 = 15,
        SPECIAL3 = 16,
        SPECIAL4 = 17,
        SPECIAL5 = 18,
        SPECIAL6 = 19,
    }
    public enum MandoSkillNames
    {
        UTILITY1 = 1,
        UTILITY2 = 2,
        UTILITY_INCREASE_DAMAGE_WHEN_GROGU = 3,
        UTILITY_DAMAGE_REDUCTION_DASH = 4, //Damage reduction after dash
        UTILITY5 = 5,
        UTILITY6 = 6,
        UTILITY_HEAL_WHEN_GROGU_SKILL = 7,
        UTILITY_CONSECUTIVE_DASH = 8,
        AGGRESION1 = 9,
        AGGRESION2 = 10,
        AGGRESION3 = 11,
        AGGRESION4 = 12,
        AGGRESION5 = 13,
        AGGRESION_INCREASE_DAMAGE_TO_BOSS = 14,
        AGGRESION_EXTRA_DAMAGE_LOW_HEALTH = 15,
        AGGRESION8 = 16,
        DEFENSE1 = 17,
        DEFENSE2 = 18,
        DEFENSE3 = 19,
        DEFENSE4 = 20,
        DEFENSE5 = 21,
        DEFENSE6 = 22,
        DEFENSE7 = 23,
        DEFENSE_CHANCE_AVOID_DAMAGE = 24,
    }
    #endregion

    #region Skills are enabled or disabled
    //Grogu Skills   
    public static Dictionary<int, bool> groguSkillEnabled = new Dictionary<int, bool>
    {
        {1, false },
        {2, false },
        {3, false },
        {4, false },
        {5, false },
        {6, false },
        {7, false },
        {8, false },
    };
    //Mando
    public static Dictionary<int, bool> mandoSkillEnabled = new Dictionary<int, bool>
    {
        {1, false }, //Utility
        {2, false },
        {3, false },
        {4, false },
        {5, false },
        {6, false },
        {7, false },
        {8, false },
        {9, false }, //Aggression
        {10, false },
        {11, false },
        {12, false },
        {13, false },
        {14, false },
        {15, false },
        {16, false },
        {17, false }, //Defense
        {18, false },
        {19, false },
        {20, false },
        {21, false },
        {22, false },
        {23, false },
        {24, false },
    };
    //Weapons
    public static Dictionary<int, bool> weaponsSkillEnabled = new Dictionary<int, bool>
    {
        {1, false }, //Primary weapons
        {2, false },
        {3, false },
        {4, false },
        {5, false },
        {6, false },
        {7, false },
        {8, false },
        {9, false }, //Secondary weapons
        {10, false },
        {11, false },
        {12, false },
        {13, false },
        {14, false },
        {15, false },
        {16, false }, //Special weapons
        {17, false },
        {18, false },
        {19, false },
        {20, false },
        {21, false },
        {22, false },
    };
    #endregion

    #region Enable and Disable Skills
    public static void EnableSkill(int skillTree, int skill_Number)
    {
        switch (skillTree)
        {
            case 1: //Grogu Skill Tree
                groguSkillEnabled[skill_Number] = true;
                break;
            case 2: //Mando Skill Tree
                mandoSkillEnabled[skill_Number] = true;
                break;
            case 3: //Weapons Skill Tree
                weaponsSkillEnabled[skill_Number] = true;
                break;
            default:
                break;
        }
    }

    public static bool IsEnabled(int skillTree, int skill_Number)
    {
        switch (skillTree)
        {
            case 1:
                return groguSkillEnabled[skill_Number];
            case 2:
                return mandoSkillEnabled[skill_Number];
            case 3:
                return weaponsSkillEnabled[skill_Number];
            default:
                return false;
        }
    }
    #endregion

    #region Geters for SkillTrees
    public static Grogu_Skills_Data GetGroguSkillTree()
    {
        return groguSkillTree;
    }
    public static Weapons_Skills_Data GetWeaponsSkillTree()
    {
        return weaponsSkillTree;
    }

    public static Mando_Skills_Data GetMandoSkillTree()
    {
        return mandoSkillTree;
    }
    #endregion

    public class Grogu_Skills_Data
    {
        //Grogu Skills
        //Grogu Skill 1
        //Grogu Skill 2
        //Grogu Skill 3
        //Grogu Skill 4
        //Grogu Skill 5
        //Grogu Skill 6
        //Grogu Skill 7
        //Grogu Skill 8
        public float Grogu8_HPMissingPercentage = -1.0f;
        public float Grogu8_gainPassiveForceRegeneration = -1.0f;
    }
    public class Mando_Skills_Data
    {
        //Utility Skill 3
        public float U3_duration = -1.0f;
        public float U3_increasedDamagePercentage = -1.0f;
        //Utility Skill 4
        public float U4_seconds = -1.0f;
        public float U4_damageReduction = -1.0f;
        //Utility Skill 7
        public int U7_healAmount = -1;
        //Utility Skill 8
        public int U8_consecutiveDashAmount = -1;
        //Aggression Skill 6
        public float A6_increaseDamageToBossAmount = -1.0f;
        //Aggression Skill 7
        public float A7_extraDamageHPStep = -1.0f;
        public float A7_extraDamageAmount = -1.0f;
        //Defense Skill 8
        public int D8_changeToAvoidDamage = -1;
    }
    public class Weapons_Skills_Data
    {
        //Primary Weapon Skill 1
        //Primary Weapon Skill 2
        //Primary Weapon Skill 3
        //Primary Weapon Skill 4
        public float PW3_SlowDownAmount = -1.0f;
        public float PW3_SlowDownDuration = -1.0f;
        //Primary Weapon Skill 5
        //Primary Weapon Skill 6
        //Primary Weapon Skill 7
        public float PW6_IncreaseDamageAmount = -1.0f;
        //Primary Weapon Skill 8

        //Secondary Weapon Skill 1
        //Secondary Weapon Skill 2
        //Secondary Weapon Skill 3
        //Secondary Weapon Skill 4
        public float SW4_DelayReducedAmount = -1.0f;
        //Secondary Weapon Skill 5
        //Secondary Weapon Skill 6
        //Secondary Weapon Skill 7
    }
}
using System;
using DiamondEngine;
using System.Collections.Generic;

public class Skill_Tree_Data : DiamondComponent
{
    public static Skill_Tree_Data instance = null;
    private static Grogu_Skills_Data groguSkillTree = null;
    private static Weapons_Skills_Data weaponsSkillTree = null;
    private static Mando_Skills_Data mandoSkillTree = null;

    #region Variables showing in the inspector
    #region Grogu variables
    //Grogu Skill 1
    //Grogu Skill 2
    //Grogu Skill 3
    //Grogu Skill 4
    //Grogu Skill 5
    //Grogu Skill 6
    //Grogu Skill 7
    //Grogu Skill 8: For each 10% of HP Mando is missing, gain 1 more passive Force Regeneration per second.
    public float Grogu8_HPMissingPercentage = 10.0f;
    public float Grogu8_gainPassiveForceRegeneration = 1.0f;
    #endregion

    #region Weapon Variables

    #region Primary Weapon Variables
    //Primary Weapon Skill 1
    //Primary Weapon Skill 2
    //Primary Weapon Skill 3    
    public float PrimaryWeapon3_SlowDownAmount = 0.2f;
    public float PrimaryWeapon3_SlowDownDuration = 3.0f;
    //Primary Weapon Skill 4
    //Primary Weapon Skill 5
    //Primary Weapon Skill 6
    public float PrimaryWeapon6_IncreaseDamageAmount = 0.25f;
    //Primary Weapon Skill 7

    #endregion
    #region Secondary Weapon Variables
    //Secondary Weapon Skill 1
    //Secondary Weapon Skill 2
    //Secondary Weapon Skill 3
    //Secondary Weapon Skill 4
    public float SecondaryWeapon4_DelayReducedAmount = 0.3f;
    //Secondary Weapon Skill 5
    //Secondary Weapon Skill 6
    //Secondary Weapon Skill 7
    #endregion
    #region Special Weapon Variables


    #endregion
    #endregion

    #region Mando Variables
    #region Utility Variables
    //Utility Skill 1
    //Utility Skill 2
    //Utility Skill 3
    //Utility Skill 4
    public float Utility4_seconds = 2.0f;
    public float Utility4_damageReduction = 0.2f;
    //Utility Skill 5
    //Utility Skill 6
    //Utility Skill 7
    public int Utility7_healAmount = 10;
    //Utility Skill 8
    #endregion
    #region Aggression Variables
    //Aggression Skill 1
    //Aggression Skill 2
    //Aggression Skill 3
    //Aggression Skill 4
    //Aggression Skill 5
    //Aggression Skill 6
    public float Aggression6_increaseDamageToBossAmount = 0.2f;
    //Aggression Skill 7
    public float Aggression7_extraDamageHPStep = 1.0f;
    public float Aggression7_extraDamageAmount = 1.0f;
    //Aggression Skill 8
    #endregion
    #region Defense Variables
    //Defense Skill 1
    //Defense Skill 2
    //Defense Skill 3
    //Defense Skill 4
    //Defense Skill 5
    //Defense Skill 6
    //Defense Skill 7
    //Defense Skill 8
    public int Defense8_chanceToAvoidDamage = 10;
    #endregion
    #endregion
    #endregion

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
        PRIMARY3 = 3,
        PRIMARY_SLOW_SPEED = 4,
        PRIMARY5 = 5,
        PRIMARY6 = 6,
        PRIMARY_INCREASE_DAMAGE = 7,
        PRIMARY8 = 8,
        SECONDARY1 = 9,
        SECONDARY2 = 10,
        SECONDARY3 = 11,
        SECONDARY_DELAY_BETWEEN_USES = 12,
        SECONDARY5 = 13,
        SECONDARY6 = 14,
        SECONDARY7 = 15,
        SPECIAL1 = 16,
        SPECIAL2 = 17,
        SPECIAL3 = 18,
        SPECIAL4 = 19,
        SPECIAL5 = 20,
        SPECIAL6 = 21,
        SPECIAL7 = 22,
    }
    public enum MandoSkillNames
    {
        UTILITY1 = 1,
        UTILITY2 = 2,
        UTILITY3 = 3,
        UTILITY_DAMAGE_REDUCTION_DASH = 4, //Damage reduction after dash
        UTILITY5 = 5,
        UTILITY6 = 6,
        UTILITY_HEAL_WHEN_GROGU_SKILL = 7,
        UTILITY8 = 8,
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
    private static Dictionary<int, bool> groguSkillEnabled = new Dictionary<int, bool>
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
    private static Dictionary<int, bool> mandoSkillEnabled = new Dictionary<int, bool>
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
    private static Dictionary<int, bool> weaponsSkillEnabled = new Dictionary<int, bool>
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
    public void EnableSkill(int skillTree, int skill_Number)
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

    public bool IsEnabled(int skillTree, int skill_Number)
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
    public Grogu_Skills_Data GetGroguSkillTree()
    {
        return groguSkillTree;
    }
    public Weapons_Skills_Data GetWeaponsSkillTree()
    {
        return weaponsSkillTree;
    }

    public Mando_Skills_Data GetMandoSkillTree()
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
        //Utility Skill 4
        public float U4_seconds = -1.0f;
        public float U4_damageReduction = -1.0f;
        //Utility Skill 7
        public int U7_healAmount = -1;
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

    public void Awake()
    {
        if (instance != null) InternalCalls.Destroy(gameObject);
        else instance = this;

        //Generate the Skill Trees
        Grogu_Skills_Data groguSkillTreeInit = new Grogu_Skills_Data();
        groguSkillTree = groguSkillTreeInit;
        Weapons_Skills_Data weaponsSkillTreeInit = new Weapons_Skills_Data();
        weaponsSkillTree = weaponsSkillTreeInit;
        Mando_Skills_Data mandoSkillTreeInit = new Mando_Skills_Data();
        mandoSkillTree = mandoSkillTreeInit;

        //Assign the values from the inspector
        groguSkillTree.Grogu8_gainPassiveForceRegeneration = Grogu8_gainPassiveForceRegeneration;
        groguSkillTree.Grogu8_HPMissingPercentage = Grogu8_HPMissingPercentage;

        weaponsSkillTree.PW3_SlowDownAmount = PrimaryWeapon3_SlowDownAmount;
        weaponsSkillTree.PW3_SlowDownDuration = PrimaryWeapon3_SlowDownDuration;
        weaponsSkillTree.SW4_DelayReducedAmount = SecondaryWeapon4_DelayReducedAmount;
        weaponsSkillTree.PW6_IncreaseDamageAmount = PrimaryWeapon6_IncreaseDamageAmount;

        mandoSkillTree.U4_damageReduction = Utility4_damageReduction;
        mandoSkillTree.U4_seconds = Utility4_seconds;
        mandoSkillTree.U7_healAmount = Utility7_healAmount;
        mandoSkillTree.A6_increaseDamageToBossAmount = Aggression6_increaseDamageToBossAmount;
        mandoSkillTree.A7_extraDamageAmount = Aggression7_extraDamageAmount;
        mandoSkillTree.A7_extraDamageHPStep = Aggression7_extraDamageHPStep;
        mandoSkillTree.D8_changeToAvoidDamage = Defense8_chanceToAvoidDamage;
    }
}
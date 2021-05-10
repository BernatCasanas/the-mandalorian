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
        PRIMARY_INCREASE_DAMAGE = 6,
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
        {1, DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadBool("groguSkill" + 1.ToString()) : false },
        {2, DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadBool("groguSkill" + 2.ToString()) : false },
        {3, DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadBool("groguSkill" + 3.ToString()) : false },
        {4, DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadBool("groguSkill" + 4.ToString()) : false },
        {5, DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadBool("groguSkill" + 5.ToString()) : false },
        {6, DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadBool("groguSkill" + 6.ToString()) : false },
        {7, DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadBool("groguSkill" + 7.ToString()) : false },
        {8, DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadBool("groguSkill" + 8.ToString()) : false },
    };
    //Mando
    public static Dictionary<int, bool> mandoSkillEnabled = new Dictionary<int, bool>
    {
        {1, DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadBool("mandoSkill" + 1.ToString()) : false }, //Utility
        {2, DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadBool("mandoSkill" + 2.ToString()) : false },
        {3, DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadBool("mandoSkill" + 3.ToString()) : false },
        {4, DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadBool("mandoSkill" + 4.ToString()) : false },
        {5, DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadBool("mandoSkill" + 5.ToString()) : false },
        {6, DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadBool("mandoSkill" + 6.ToString()) : false },
        {7, DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadBool("mandoSkill" + 7.ToString()) : false },
        {8, DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadBool("mandoSkill" + 8.ToString()) : false },
        {9, DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadBool("mandoSkill" + 9.ToString()) : false }, //Aggression
        {10, DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadBool("mandoSkill" + 10.ToString()) : false },
        {11, DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadBool("mandoSkill" + 11.ToString()) : false },
        {12, DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadBool("mandoSkill" + 12.ToString()) : false },
        {13, DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadBool("mandoSkill" + 13.ToString()) : false },
        {14, DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadBool("mandoSkill" + 14.ToString()) : false },
        {15, DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadBool("mandoSkill" + 15.ToString()) : false },
        {16, DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadBool("mandoSkill" + 16.ToString()) : false },
        {17, DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadBool("mandoSkill" + 17.ToString()) : false }, //Defense
        {18, DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadBool("mandoSkill" + 18.ToString()) : false },
        {19, DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadBool("mandoSkill" + 19.ToString()) : false },
        {20, DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadBool("mandoSkill" + 20.ToString()) : false },
        {21, DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadBool("mandoSkill" + 21.ToString()) : false },
        {22, DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadBool("mandoSkill" + 22.ToString()) : false },
        {23, DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadBool("mandoSkill" + 23.ToString()) : false },
        {24, DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadBool("mandoSkill" + 24.ToString()) : false },
    };
    //Weapons
    public static Dictionary<int, bool> weaponsSkillEnabled = new Dictionary<int, bool>
    {
        {1, DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadBool("weaponSkill" + 1.ToString()) : false }, //Primary weapons
        {2, DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadBool("weaponSkill" + 2.ToString()) : false },
        {3, DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadBool("weaponSkill" + 3.ToString()) : false },
        {4, DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadBool("weaponSkill" + 4.ToString()) : false },
        {5, DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadBool("weaponSkill" + 5.ToString()) : false },
        {6, DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadBool("weaponSkill" + 6.ToString()) : false },
        {7, DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadBool("weaponSkill" + 7.ToString()) : false },
        {8, DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadBool("weaponSkill" + 8.ToString()) : false }, //Secondary weapons
        {9, DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadBool("weaponSkill" + 9.ToString()) : false },
        {10, DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadBool("weaponSkill" + 10.ToString()) : false },
        {11, DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadBool("weaponSkill" + 11.ToString()) : false },
        {12, DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadBool("weaponSkill" + 12.ToString()) : false },
        {13, DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadBool("weaponSkill" + 13.ToString()) : false },
        {14, DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadBool("weaponSkill" + 14.ToString()) : false }, //Special weapons
        {15, DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadBool("weaponSkill" + 15.ToString()) : false },
        {16, DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadBool("weaponSkill" + 16.ToString()) : false },
        {17, DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadBool("weaponSkill" + 17.ToString()) : false },
        {18, DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadBool("weaponSkill" + 18.ToString()) : false },
        {19, DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadBool("weaponSkill" + 19.ToString()) : false },
    };
    #endregion

    #region Enable and Disable Skills
    public static void EnableSkill(int skillTree, int skill_Number)
    {
        switch (skillTree)
        {
            case 1: //Grogu Skill Tree
                groguSkillEnabled[skill_Number] = true;
                DiamondPrefs.Write("groguSkill" + skill_Number.ToString(), groguSkillEnabled[skill_Number]);
                break;
            case 2: //Mando Skill Tree
                mandoSkillEnabled[skill_Number] = true;
                DiamondPrefs.Write("mandoSkill" + skill_Number.ToString(), mandoSkillEnabled[skill_Number]);
                break;
            case 3: //Weapons Skill Tree
                weaponsSkillEnabled[skill_Number] = true;
                DiamondPrefs.Write("weaponSkill" + skill_Number.ToString(), weaponsSkillEnabled[skill_Number]);
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
                Debug.Log(groguSkillEnabled[skill_Number].ToString());
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
        if(groguSkillTree == null)
            groguSkillTree = new Grogu_Skills_Data();

        return groguSkillTree;
    }
    public static Weapons_Skills_Data GetWeaponsSkillTree()
    {
        if (weaponsSkillTree == null)
            weaponsSkillTree = new Weapons_Skills_Data();

        return weaponsSkillTree;
    }

    public static Mando_Skills_Data GetMandoSkillTree()
    {
        if (mandoSkillTree == null)
            mandoSkillTree = new Mando_Skills_Data();

        return mandoSkillTree;
    }
    #endregion

    public class Grogu_Skills_Data
    {
        //Descriptions
        public string G1_description = "";
        public string G2_description = "";
        public string G3_description = "";
        public string G4_description = "";
        public string G5_description = "";
        public string G6_description = "";
        public string G7_description = "";
        public string G8_description = "";
    }
    public class Mando_Skills_Data
    {
        //Descriptions - Utility
        public string U1_description = "AAAAAA";
        public string U2_description = "";
        public string U3_description = "";
        public string U4_description = "";
        public string U5_description = "";
        public string U6_description = "";
        public string U7_description = "";
        public string U8_description = "";

        //Descriptions - Aggression
        public string A1_description = "";
        public string A2_description = "";
        public string A3_description = "";
        public string A4_description = "";
        public string A5_description = "";
        public string A6_description = "";
        public string A7_description = "";
        public string A8_description = "";

        //Descriptions - Defense
        public string D1_description = "";
        public string D2_description = "";
        public string D3_description = "";
        public string D4_description = "";
        public string D5_description = "";
        public string D6_description = "";
        public string D7_description = "";
        public string D8_description = "";
    }
    public class Weapons_Skills_Data
    {
        //Descriptions - Primary Weapon
        public string PrimaryW1_description = "";
        public string PrimaryW2_description = "";
        public string PrimaryW3_description = "";
        public string PrimaryW4_description = "";
        public string PrimaryW5_description = "";
        public string PrimaryW6_description = "";
        public string PrimaryW7_description = "";
        public string PrimaryW8_description = "";

        //Descriptions - Secondary Weapon
        public string SecondaryW1_description = "";
        public string SecondaryW2_description = "";
        public string SecondaryW3_description = "";
        public string SecondaryW4_description = "";
        public string SecondaryW5_description = "";
        public string SecondaryW6_description = "";

        //Descriptions - Special Weapon
        public string SpecialW1_description = "";
        public string SpecialW2_description = "";
        public string SpecialW3_description = "";
        public string SpecialW4_description = "";
        public string SpecialW5_description = "";
        public string SpecialW6_description = "";
    }

    public static void Reset()
    {
        for (int i = 0; i < groguSkillEnabled.Count; i++)
        {
            groguSkillEnabled[i + 1] = DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadBool("groguSkill" + (i + 1).ToString()) : false;
        }
        for (int i = 0; i < mandoSkillEnabled.Count; i++)
        {
            mandoSkillEnabled[i + 1] = DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadBool("mandoSkill" + (i + 1).ToString()) : false;
        }
        for (int i = 0; i < weaponsSkillEnabled.Count; i++)
        {
            weaponsSkillEnabled[i + 1] = DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadBool("weaponSkill" + (i + 1).ToString()) : false;
        }

        if(Core.instance!=null)
            Core.instance.ResetBuffs();
    }
}
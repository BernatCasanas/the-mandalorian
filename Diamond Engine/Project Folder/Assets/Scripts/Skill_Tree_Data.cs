using System;
using DiamondEngine;
using System.Collections.Generic;

public class Skill_Tree_Data : DiamondComponent
{
    public static Skill_Tree_Data instance = null;
    private static Grogu_Skills_Data groguSkillTree = null;
    private static Weapons_Skills_Data weaponsSkillTree = null;

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
    //Primary Weapon Skill 4
    public float PW4_SlowDownAmount = 0.2f;
    public float PW4_SlowDownDuration = 3.0f;
    //Primary Weapon Skill 5
    //Primary Weapon Skill 6
    //Primary Weapon Skill 7
    //Primary Weapon Skill 8
    #endregion
    #region Secondary Weapon Variables
    //Secondary Weapon Skill 1
    //Secondary Weapon Skill 2
    //Secondary Weapon Skill 3
    //Secondary Weapon Skill 4
    public float SW4_DelayReducedAmount = 0.3f;
    //Secondary Weapon Skill 5
    //Secondary Weapon Skill 6
    //Secondary Weapon Skill 7
    #endregion
    #region Special Weapon Variables


    #endregion
    #endregion

    #region Mando Variables
    #region Utility Variables
    //Primary Weapon Skill 1
    //Primary Weapon Skill 2
    //Primary Weapon Skill 3
    //Primary Weapon Skill 4
    //Primary Weapon Skill 5
    //Primary Weapon Skill 6
    //Primary Weapon Skill 7
    //Primary Weapon Skill 8
    #endregion
    #region Aggression Variables
    //Secondary Weapon Skill 1
    //Secondary Weapon Skill 2
    //Secondary Weapon Skill 3
    //Secondary Weapon Skill 4
    //Secondary Weapon Skill 5
    //Secondary Weapon Skill 6
    //Secondary Weapon Skill 7
    #endregion
    #region Defense Variables


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
        PRIMARY7 = 7,
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

    public void EnableSkill(int skillTree, int skill_Number)
    {
        switch (skillTree)
        {
            case 1: //Grogu Skill Tree
                groguSkillEnabled[skill_Number] = true;
                break;
            case 2: //Mando Skill Tree

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
                break;
            case 3:
                return weaponsSkillEnabled[skill_Number];
            default:
                return false;
        }

        return false; //TODO: Delete this when all the cases of the switch are filled;
    }

    public Grogu_Skills_Data GetGroguSkillTree()
    {
        return groguSkillTree;
    }
    public Weapons_Skills_Data GetWeaponsSkillTree()
    {
        return weaponsSkillTree;
    }

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
    public class Weapons_Skills_Data
    {
        //Primary Weapon Skill 1
        //Primary Weapon Skill 2
        //Primary Weapon Skill 3
        //Primary Weapon Skill 4
        public float PW4_SlowDownAmount = -1.0f;
        public float PW4_SlowDownDuration = -1.0f;
        //Primary Weapon Skill 5
        //Primary Weapon Skill 6
        //Primary Weapon Skill 7
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

        //Assign the values from the inspector
        groguSkillTree.Grogu8_gainPassiveForceRegeneration = Grogu8_gainPassiveForceRegeneration;
        groguSkillTree.Grogu8_HPMissingPercentage = Grogu8_HPMissingPercentage;

        weaponsSkillTree.PW4_SlowDownAmount = PW4_SlowDownAmount;
        weaponsSkillTree.PW4_SlowDownDuration = PW4_SlowDownDuration;
        weaponsSkillTree.SW4_DelayReducedAmount = SW4_DelayReducedAmount;
    }
}
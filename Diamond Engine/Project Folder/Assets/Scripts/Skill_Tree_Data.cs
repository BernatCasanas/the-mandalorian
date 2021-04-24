using System;
using DiamondEngine;
using System.Collections.Generic;

public class Skill_Tree_Data : DiamondComponent
{
    public static Skill_Tree_Data instance = null;
    private static Grogu_Skills_Data groguSkillTree = null;

    //----- VARIABLES SHOWING IN THE INSPECTOR -----
    //----- GROGU -----
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
                break;
            default:
                break;
        }
    }

    public bool IsActive(int skillTree, int skill_Number)
    {
        switch (skillTree)
        {
            case 1:
                return groguSkillEnabled[skill_Number];
            case 2:
                break;
            case 3:
                break;
            default:
                break;
        }

        return false;
    }

    public Grogu_Skills_Data GetGroguSkillTree()
    {
        return groguSkillTree;
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
        public float Grogu8_HPMissingPercentage = 10.0f;
        public float Grogu8_gainPassiveForceRegeneration = 1.0f;
    }

    public void Awake()
    {
        if (instance != null) InternalCalls.Destroy(gameObject);
        else instance = this;

        Grogu_Skills_Data groguSkillTreeInit = new Grogu_Skills_Data();
        groguSkillTree = groguSkillTreeInit;

        //Generate a Grogu Skill Tree
        groguSkillTree.Grogu8_gainPassiveForceRegeneration = Grogu8_gainPassiveForceRegeneration;
        groguSkillTree.Grogu8_HPMissingPercentage = Grogu8_HPMissingPercentage;
    }
}
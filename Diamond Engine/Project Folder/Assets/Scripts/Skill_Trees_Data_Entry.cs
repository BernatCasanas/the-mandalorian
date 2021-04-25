using System;
using DiamondEngine;

public class Skill_Trees_Data_Entry : DiamondComponent
{
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

    public void Awake()
    {
        //Generate the Skill Trees
        Skill_Tree_Data.groguSkillTree = new Skill_Tree_Data.Grogu_Skills_Data();
        Skill_Tree_Data.weaponsSkillTree = new Skill_Tree_Data.Weapons_Skills_Data();
        Skill_Tree_Data.mandoSkillTree = new Skill_Tree_Data.Mando_Skills_Data();

        //Assign the values from the inspector
        Skill_Tree_Data.groguSkillTree.Grogu8_gainPassiveForceRegeneration = Grogu8_gainPassiveForceRegeneration;
        Skill_Tree_Data.groguSkillTree.Grogu8_HPMissingPercentage = Grogu8_HPMissingPercentage;
        Skill_Tree_Data.weaponsSkillTree.PW3_SlowDownAmount = PrimaryWeapon3_SlowDownAmount;
        Skill_Tree_Data.weaponsSkillTree.PW3_SlowDownDuration = PrimaryWeapon3_SlowDownDuration;
        Skill_Tree_Data.weaponsSkillTree.SW4_DelayReducedAmount = SecondaryWeapon4_DelayReducedAmount;
        Skill_Tree_Data.weaponsSkillTree.PW6_IncreaseDamageAmount = PrimaryWeapon6_IncreaseDamageAmount;

        Skill_Tree_Data.mandoSkillTree.U4_damageReduction = Utility4_damageReduction;
        Skill_Tree_Data.mandoSkillTree.U4_seconds = Utility4_seconds;
        Skill_Tree_Data.mandoSkillTree.U7_healAmount = Utility7_healAmount;
        Skill_Tree_Data.mandoSkillTree.A6_increaseDamageToBossAmount = Aggression6_increaseDamageToBossAmount;
        Skill_Tree_Data.mandoSkillTree.A7_extraDamageAmount = Aggression7_extraDamageAmount;
        Skill_Tree_Data.mandoSkillTree.A7_extraDamageHPStep = Aggression7_extraDamageHPStep;
        Skill_Tree_Data.mandoSkillTree.D8_changeToAvoidDamage = Defense8_chanceToAvoidDamage;        
    }
}
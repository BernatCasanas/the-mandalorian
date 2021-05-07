using System;
using DiamondEngine;
using System.Collections.Generic;

public class LoadSkills : DiamondComponent
{
    public GameObject hubTextController = null;
    public GameObject skillNodeMandoUtility = null;
    public GameObject skillNodeMandoAgression = null;
    public GameObject skillNodeMandoDefense = null;
    public GameObject skillNodeMandoDefense2 = null;
    public GameObject skillNodeGrogu = null;
    public GameObject skillNodeWeaponPrimary = null;
    public GameObject skillNodeWeaponPrimary2 = null;
    public GameObject skillNodeWeaponSecondary = null;
    public GameObject skillNodeWeaponSpecial = null;
    private static bool start = true;

    public static Dictionary<string, string> skillDictionaryLoading = new Dictionary<string, string>
        {
            {"mandoSkill1","UDamageHeat"},
            {"mandoSkill2","UMovSpd"},
            {"mandoSkill3","USlowDamage"},
            {"mandoSkill4","UFallDmgRed"},
            {"mandoSkill5","UOverheat"},
            {"mandoSkill6","URedGroguCost"},
            {"mandoSkill7","UHeal"},
            {"mandoSkill8","UDash"},
            {"mandoSkill9","ABlasterDmg"},
            {"mandoSkill10","AComboFireRate"},
            {"mandoSkill11","AComboDmg"},
            {"mandoSkill12","AGrenadeDmg"},
            {"mandoSkill13","ASniperDmg"},
            {"mandoSkill14","ADmgBos"},
            {"mandoSkill15","AHPMissDmgBlaster"},
            {"mandoSkill16","AHPMissDmgSniper"},
            {"mandoSkill17","DMaxHPS"},
            {"mandoSkill18","DDmgRed"},
            {"mandoSkill19","DComboDmgRed"},
            {"mandoSkill20","DComboHeal"},
            {"mandoSkill21","DComboFnshHeal"},
            {"mandoSkill22","DHeal"},
            {"mandoSkill23","DHealEfct"},
            {"mandoSkill24","DAvoidDmg"},
            {"groguSkill1","GForceReg"},
            {"groguSkill2","GPushRange"},
            {"groguSkill3","GForceDur"},
            {"groguSkill4","GThrshComboForce"},
            {"groguSkill5","GMaxForce"},
            {"groguSkill6","GSingComboForce"},
            {"groguSkill7","GComboTimer"},
            {"groguSkill8","GHPForceReg"},
            {"weaponSkill1","PMovSpd"},
            {"weaponSkill2","PProjRange"},
            {"weaponSkill3","PSlowEnem"},
            {"weaponSkill4","PCritChance"},
            {"weaponSkill5","PCritDmg"},
            {"weaponSkill6","PIncDmg"},
            {"weaponSkill7","PRateFire"},
            {"weaponSkill8","SeStatEfct"},
            {"weaponSkill9","SeCritChance"},
            {"weaponSkill10","SeDelay"},
            {"weaponSkill11","SeUseRange"},
            {"weaponSkill12","SeCritDmg"},
            {"weaponSkill13","SeDmgInc"},
            {"weaponSkill14","SpCritChance"},
            {"weaponSkill15","SpChargeTime"},
            {"weaponSkill16","SpBullet"},
            {"weaponSkill17","SpRegForce"},
            {"weaponSkill18","SpCritDmg"},
            {"weaponSkill19","SpMaxDmg"},
        };
    public void Update()
    {
        if (DiamondPrefs.ReadBool("reset"))
        {
            DiamondPrefs.Delete("reset");
            Reset();
        }
        if (start)
        {
            Start();
        }
    }
    private void Start()
    {
        if (DiamondPrefs.ReadBool("loadData") == false)
            DiamondPrefs.Write("loadData", true);

        for (int i = 0; i < Skill_Tree_Data.groguSkillEnabled.Count; i++)
        {
            if (Skill_Tree_Data.groguSkillEnabled[i + 1] == true)
            {
                Skill_Tree_Node.addSkill(1, skillDictionaryLoading["groguSkill" + (i + 1).ToString()]);
            }
        }
        for (int i = 0; i < Skill_Tree_Data.mandoSkillEnabled.Count; i++)
        {
            if (Skill_Tree_Data.mandoSkillEnabled[i + 1] == true)
            {
                Skill_Tree_Node.addSkill(2, skillDictionaryLoading["mandoSkill" + (i + 1).ToString()]);
            }
        }
        for (int i = 0; i < Skill_Tree_Data.weaponsSkillEnabled.Count; i++)
        {
            if (Skill_Tree_Data.weaponsSkillEnabled[i + 1] == true)
            {
                Skill_Tree_Node.addSkill(3, skillDictionaryLoading["weaponSkill" + (i + 1).ToString()]);
            }
        }

        start = false;
    }

    private void Reset()
    {
        start = true;
        ///SKILLS
        Skill_Tree_Data.Reset();
        if (skillNodeMandoUtility != null)
            skillNodeMandoUtility.GetComponent<Skill_Tree_Node>().Reset();
        if (skillNodeMandoAgression != null)
            skillNodeMandoAgression.GetComponent<Skill_Tree_Node>().Reset();
        if (skillNodeMandoDefense != null)
            skillNodeMandoDefense.GetComponent<Skill_Tree_Node>().Reset();
        if (skillNodeMandoDefense2 != null)
            skillNodeMandoDefense2.GetComponent<Skill_Tree_Node>().Reset();
        if (skillNodeGrogu != null)
            skillNodeGrogu.GetComponent<Skill_Tree_Node>().Reset();
        if (skillNodeWeaponPrimary != null)
            skillNodeWeaponPrimary.GetComponent<Skill_Tree_Node>().Reset();
        if (skillNodeWeaponPrimary2 != null)
            skillNodeWeaponPrimary2.GetComponent<Skill_Tree_Node>().Reset();
        if (skillNodeWeaponSecondary != null)
            skillNodeWeaponSecondary.GetComponent<Skill_Tree_Node>().Reset();
        if (skillNodeWeaponSpecial != null)
            skillNodeWeaponSpecial.GetComponent<Skill_Tree_Node>().Reset();

        ///DIALOGS
        if (hubTextController != null)
        {
            hubTextController.GetComponent<HubTextController>().Reset();
        }
        ///RESOURCES
        PlayerResources.ResetResources();
    }
}
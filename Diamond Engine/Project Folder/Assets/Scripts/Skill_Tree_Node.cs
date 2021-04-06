using System;
using DiamondEngine;

public class Skill_Tree_Node : DiamondComponent
{
	public String skill_name = "";

	private Skills skill = null;
	private bool start = true;
	public void Update()
	{
        if (start)
        {
			start = false;
            if (SkillDictionary.skill_type.ContainsKey(skill_name))
            {
                Type t = SkillDictionary.skill_type[skill_name];
                skill = (Skills)Activator.CreateInstance(t);
            }
            else
            {
                Debug.Log("ERROR: Skill doesn't exist");
            }
        }
	}

}
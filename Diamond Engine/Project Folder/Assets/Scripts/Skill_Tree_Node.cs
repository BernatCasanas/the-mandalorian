using System;
using DiamondEngine;

public class Skill_Tree_Node : DiamondComponent
{
	public String skill_name = "";
    public GameObject children_1 = null;
    public GameObject children_2 = null;
    public GameObject button_skill_purchased = null;
    public GameObject button_skill_unavailable = null;

    public GameObject nav_left = null;
    public GameObject nav_right = null;
    public GameObject nav_up = null;
    public GameObject nav_down = null;

    public bool main_button = false;

    private Skills skill = null;
	private bool start = true;
	public void Update()
	{
        if (start && main_button)
        {
			start = false;
            if (button_skill_unavailable != null)
            {
                UpdateNavs(button_skill_unavailable);
                button_skill_unavailable.EnableNav(false);
            }
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
    public void OnExecuteButton()
    {
        skill.Use();
        if (children_1 != null)
        {
            children_1.EnableNav(true);
        }

        if (children_2 != null)
        {
            children_2.EnableNav(true);
        }
        if (button_skill_purchased != null)
        {
            button_skill_purchased.GetComponent<Skill_Tree_Node>().UpdateNavs(gameObject);
            button_skill_purchased.GetComponent<Navigation>().Select();
            button_skill_purchased.EnableNav(true);
        }
        gameObject.EnableNav(false);

    }


    public void UpdateNavs(GameObject previous_button_selected)
    {
        if (previous_button_selected.GetComponent<Skill_Tree_Node>().nav_left != null)
        {
            nav_left = previous_button_selected.GetComponent<Skill_Tree_Node>().nav_left;
            //Set this Navigation
            nav_left.GetComponent<Skill_Tree_Node>().nav_right = gameObject;
            //Set Navigation brother left;
        }

        if (previous_button_selected.GetComponent<Skill_Tree_Node>().nav_right != null)
        {
            nav_right = previous_button_selected.GetComponent<Skill_Tree_Node>().nav_right;
            //Set this Navigation
            nav_right.GetComponent<Skill_Tree_Node>().nav_left = gameObject;
            //Set Navigation brother right;
        }

        if (previous_button_selected.GetComponent<Skill_Tree_Node>().nav_up != null)
        {
            nav_up = previous_button_selected.GetComponent<Skill_Tree_Node>().nav_up;
            //Set this Navigation
            if (nav_up.GetComponent<Skill_Tree_Node>().nav_down == previous_button_selected)
            {
                nav_up.GetComponent<Skill_Tree_Node>().nav_down = gameObject;
                //Set Navigation brother left;
            }
        }

        if (previous_button_selected.GetComponent<Skill_Tree_Node>().nav_down != null)
        {
            nav_down = previous_button_selected.GetComponent<Skill_Tree_Node>().nav_down;
            //Set this Navigation
            if (nav_down.GetComponent<Skill_Tree_Node>().nav_up == previous_button_selected)
            {
                nav_down.GetComponent<Skill_Tree_Node>().nav_up = gameObject;
                //Set Navigation brother left;
            }
        }
    }
}
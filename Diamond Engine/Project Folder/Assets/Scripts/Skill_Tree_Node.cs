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

    public GameObject nav_aux = null;
    public GameObject nav_aux_2 = null;
    public bool has2children = false;

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
        if(has2children == false && nav_aux != null && (nav_up.GetComponent<Skill_Tree_Node>().main_button || nav_aux.GetComponent<Skill_Tree_Node>().main_button))
        {
            return;
        }

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
        has2children = previous_button_selected.GetComponent<Skill_Tree_Node>().has2children;

        if (previous_button_selected.GetComponent<Skill_Tree_Node>().nav_left != null)
        {
            nav_left = previous_button_selected.GetComponent<Skill_Tree_Node>().nav_left;
            gameObject.GetComponent<Navigation>().SetLeftNavButton(nav_left);
            nav_left.GetComponent<Skill_Tree_Node>().nav_right = gameObject;
            nav_left.GetComponent<Navigation>().SetRightNavButton(gameObject);
        }

        if (previous_button_selected.GetComponent<Skill_Tree_Node>().nav_right != null)
        {
            nav_right = previous_button_selected.GetComponent<Skill_Tree_Node>().nav_right;
            gameObject.GetComponent<Navigation>().SetRightNavButton(nav_right);
            nav_right.GetComponent<Skill_Tree_Node>().nav_left = gameObject;
            nav_right.GetComponent<Navigation>().SetLeftNavButton(gameObject);
        }

        if (previous_button_selected.GetComponent<Skill_Tree_Node>().nav_up != null)
        {
            nav_up = previous_button_selected.GetComponent<Skill_Tree_Node>().nav_up;
            gameObject.GetComponent<Navigation>().SetUpNavButton(nav_up);
            if (nav_up.GetComponent<Skill_Tree_Node>().nav_down.GetUid() == previous_button_selected.GetUid())
            {
                nav_up.GetComponent<Skill_Tree_Node>().nav_down = gameObject;
                nav_up.GetComponent<Navigation>().SetDownNavButton(gameObject);
            }
            else if (nav_up.GetComponent<Skill_Tree_Node>().nav_aux.GetUid() == previous_button_selected.GetUid())
            {
                nav_up.GetComponent<Skill_Tree_Node>().nav_aux = gameObject;
            }

            if (previous_button_selected.GetComponent<Skill_Tree_Node>().nav_aux != null && has2children == false)
            {
                nav_aux = previous_button_selected.GetComponent<Skill_Tree_Node>().nav_aux;
                nav_aux.GetComponent<Skill_Tree_Node>().nav_down = gameObject;
                nav_aux.GetComponent<Navigation>().SetDownNavButton(gameObject);
            }
            else if (previous_button_selected.GetComponent<Skill_Tree_Node>().nav_aux_2 != null)
            {
                nav_aux_2 = previous_button_selected.GetComponent<Skill_Tree_Node>().nav_aux_2;
                nav_aux_2.GetComponent<Skill_Tree_Node>().nav_down = gameObject;
                nav_aux_2.GetComponent<Navigation>().SetDownNavButton(gameObject);
            }
        }

        if (previous_button_selected.GetComponent<Skill_Tree_Node>().nav_down != null)
        {
            nav_down = previous_button_selected.GetComponent<Skill_Tree_Node>().nav_down;
            gameObject.GetComponent<Navigation>().SetDownNavButton(nav_down);
            if (nav_down.GetComponent<Skill_Tree_Node>().nav_up.GetUid() == previous_button_selected.GetUid())
            {
                nav_down.GetComponent<Skill_Tree_Node>().nav_up = gameObject;
                nav_down.GetComponent<Navigation>().SetUpNavButton(gameObject);
            }
            else if (nav_down.GetComponent<Skill_Tree_Node>().nav_aux.GetUid() == previous_button_selected.GetUid())
            {
                nav_down.GetComponent<Skill_Tree_Node>().nav_aux = gameObject;
            }
            else if (nav_down.GetComponent<Skill_Tree_Node>().nav_aux_2.GetUid() == previous_button_selected.GetUid())
            {
                nav_down.GetComponent<Skill_Tree_Node>().nav_aux_2 = gameObject;
            }

            if (previous_button_selected.GetComponent<Skill_Tree_Node>().nav_aux != null && has2children == true)
            {
                nav_aux = previous_button_selected.GetComponent<Skill_Tree_Node>().nav_aux;
                nav_aux.GetComponent<Skill_Tree_Node>().nav_up = gameObject;
                nav_aux.GetComponent<Navigation>().SetUpNavButton(gameObject);
            }
        }
    }
}
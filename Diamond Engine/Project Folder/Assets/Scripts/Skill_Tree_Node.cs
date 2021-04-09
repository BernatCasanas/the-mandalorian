using System;
using DiamondEngine;

public class Skill_Tree_Node : DiamondComponent
{

    public enum NODE_STATE
    {
        UNLOCKED,
        LOCKED,
        OWNED,
    };


    #region Style
    public int unlockedButtonPressed = 0;
    public int unlockedButtonHovered = 0;
    public int unlockedButtonUnhovered = 0;

    public int lockedButtonPressed = 0;
    public int lockedButtonHovered = 0;
    public int lockedButtonUnhovered = 0;

    public int ownedButtonPressed = 0;
    public int ownedButtonHovered = 0;
    public int ownedButtonUnhovered = 0;
    #endregion

    #region Logic
    public string skill_name = "";

    public GameObject children_1 = null;
    public GameObject children_2 = null;

    public GameObject oppositeNode = null;


    public GameObject text_description = null;
    public GameObject hub_skill_controller = null; 


    private Skills skill = null;

    public bool isRootNode = false;
    private NODE_STATE _state;


    public NODE_STATE state 
    {
        get { return _state; }
        set 
        {
            _state = value;
            switch (value)
            {
                case NODE_STATE.UNLOCKED:
                    gameObject.GetComponent<Button>().ChangeSprites(unlockedButtonPressed, unlockedButtonHovered, unlockedButtonUnhovered);
                    break;
                case NODE_STATE.LOCKED:
                    gameObject.GetComponent<Button>().ChangeSprites(lockedButtonHovered, lockedButtonHovered, lockedButtonUnhovered);
                    break;
                case NODE_STATE.OWNED:
                    gameObject.GetComponent<Button>().ChangeSprites(ownedButtonPressed, ownedButtonHovered, ownedButtonUnhovered);
                    break;
                default:
                    break;
            }
        }
    }
    #endregion



    public void Awake()
    {
        if (isRootNode)
            state = NODE_STATE.UNLOCKED;
        else
            state = NODE_STATE.LOCKED;
        if (SkillDictionary.skill_type.ContainsKey(skill_name))
        {
            Type t = SkillDictionary.skill_type[skill_name];
            skill = (Skills)Activator.CreateInstance(t);
            skill.AddDescription();
        }
        else
        {
            Debug.Log("ERROR: Skill doesn't exist");
        }
    }
	public void Update()
	{

        if (hub_skill_controller == null || hub_skill_controller.GetComponent<HubSkillTreeController>().skill_selected == skill || gameObject.GetComponent<Navigation>().is_selected == false)
            return;
        

        hub_skill_controller.GetComponent<HubSkillTreeController>().skill_selected = skill;
        if (text_description != null)
            text_description.GetComponent<Text>().text = skill.description;


    }
    public void OnExecuteButton()
    {
        if (state == NODE_STATE.LOCKED || state == NODE_STATE.OWNED)
            return;

        skill.Use();

        state = NODE_STATE.OWNED;

        if (children_1 != null)
            children_1.GetComponent<Skill_Tree_Node>().state = NODE_STATE.UNLOCKED;

        if (children_2 != null)
            children_2.GetComponent<Skill_Tree_Node>().state = NODE_STATE.UNLOCKED;

        if(oppositeNode != null)
            oppositeNode.GetComponent<Skill_Tree_Node>().state = NODE_STATE.LOCKED;
    }

}
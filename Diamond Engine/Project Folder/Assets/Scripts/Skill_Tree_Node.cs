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

    public enum NODE_TYPE
    {
        MANDO = 0,
        GROGU,
        WEAPON,
    }


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

    public int node_type = 0;
    private NODE_TYPE _type = NODE_TYPE.MANDO;


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

        if (node_type == 0)
            _type = NODE_TYPE.MANDO;
        else if (node_type == 1)
            _type = NODE_TYPE.GROGU;
        else
            _type = NODE_TYPE.WEAPON;
    }
	public void Update()
	{

        if (hub_skill_controller == null || hub_skill_controller.GetComponent<HubSkillTreeController>().skill_selected == skill || gameObject.GetComponent<Navigation>().is_selected == false)
            return;
        

        hub_skill_controller.GetComponent<HubSkillTreeController>().skill_selected = skill;
        if (text_description != null)
            text_description.GetComponent<Text>().text = skill.description;

        if (Input.GetGamepadButton(DEControllerButton.Y) == KeyState.KEY_DOWN)
        {
            Debug.Log("Beskar: ");
            PlayerResources.AddResourceBy1(RewardType.REWARD_BESKAR);
            Debug.Log("Beskar: " + PlayerResources.GetResourceCount(RewardType.REWARD_BESKAR));
        }



    }
    public void OnExecuteButton()
    {
        if (state == NODE_STATE.LOCKED || state == NODE_STATE.OWNED)
            return;

        if (_type == NODE_TYPE.MANDO && PlayerResources.GetResourceCount(RewardType.REWARD_BESKAR) == 0)
        {
            Debug.Log("You don't have enough Beskar!");
            return;
        }
        else
        {
            PlayerResources.SubstractResourceBy1(RewardType.REWARD_BESKAR);
            Debug.Log("Beskar: " + PlayerResources.GetResourceCount(RewardType.REWARD_BESKAR));
        }

        if (_type == NODE_TYPE.GROGU && PlayerResources.GetResourceCount(RewardType.REWARD_MACARON) == 0)
        {
            Debug.Log("You don't have enough Macarons!");
            return;
        }
        else
        {
            PlayerResources.SubstractResourceBy1(RewardType.REWARD_MACARON);
            Debug.Log("Macarons: " + PlayerResources.GetResourceCount(RewardType.REWARD_MACARON));
        }

        if (_type == NODE_TYPE.WEAPON && PlayerResources.GetResourceCount(RewardType.REWARD_SCRAP) == 0)
        {
            Debug.Log("You don't have enough Scrap!");
            return;
        }
        else
        {
            PlayerResources.SubstractResourceBy1(RewardType.REWARD_SCRAP);
            Debug.Log("Scrap: " + PlayerResources.GetResourceCount(RewardType.REWARD_SCRAP));
        }

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
using System;
using DiamondEngine;

public class HubSkillTreeController : DiamondComponent
{
	public GameObject mando = null;
	public GameObject mando_tree = null;
	public GameObject mando_tree_activation_point = null;
	public GameObject grogu_tree = null;
	public GameObject grogu_tree_activation_point = null;
	public GameObject weapon_tree = null;
	public GameObject weapon_tree_activation_point = null;

	public Skills skill_selected = null;

	public float maximum_distance_to_interact_squared_skill = 0.0f;

	public GameObject mandoText = null;
	public GameObject groguText = null;
	public GameObject weaponText = null;

	public GameObject beskarResourceText = null;
	public GameObject macaronResourceText = null;
	public GameObject scrapResourceText = null;

	public void Awake()
    {
		beskarResourceText.GetComponent<Text>().text = PlayerResources.GetResourceCount(RewardType.REWARD_BESKAR).ToString();
		macaronResourceText.GetComponent<Text>().text = PlayerResources.GetResourceCount(RewardType.REWARD_MACARON).ToString();
		scrapResourceText.GetComponent<Text>().text = PlayerResources.GetResourceCount(RewardType.REWARD_SCRAP).ToString();
    }

	public void Update()
	{
		if (Core.instance.gameObject != null)
			mando = Core.instance.gameObject;

		if (mando == null || (Input.GetGamepadButton(DEControllerButton.A) != KeyState.KEY_DOWN && Input.GetGamepadButton(DEControllerButton.B) != KeyState.KEY_DOWN))
			return;
		bool activate_tree = Input.GetGamepadButton(DEControllerButton.A) == KeyState.KEY_DOWN ? true : false;
		if (activate_tree && ((mando_tree != null && mando_tree.IsEnabled()) || (grogu_tree != null && grogu_tree.IsEnabled()) || (weapon_tree != null && weapon_tree.IsEnabled()))) 
			return;
        if (!activate_tree)
        {
            if (mando_tree !=null && mando_tree.IsEnabled())
            {
				mando_tree.EnableNav(false);
				if (mandoText != null)
					mandoText.Enable(true);
				return;
            }
			if (grogu_tree!=null && grogu_tree.IsEnabled())
			{
				grogu_tree.EnableNav(false);
				if (groguText != null)
					groguText.Enable(true);
				return;
			}
			if (weapon_tree !=null && weapon_tree.IsEnabled())
			{
				weapon_tree.EnableNav(false);
				if (weaponText != null)
					weaponText.Enable(true);
				return;
			}
		}
		if (mando_tree != null && mando_tree_activation_point != null)
		{
			if (mando.GetComponent<Transform>().globalPosition.DistanceNoSqrt(mando_tree_activation_point.GetComponent<Transform>().globalPosition) < maximum_distance_to_interact_squared_skill)
			{
				mando_tree.EnableNav(true);
				if (mandoText != null)
					mandoText.Enable(false);
			}
		}
		if (grogu_tree != null && grogu_tree_activation_point != null)
		{
			if (mando.GetComponent<Transform>().globalPosition.DistanceNoSqrt(grogu_tree_activation_point.GetComponent<Transform>().globalPosition) < maximum_distance_to_interact_squared_skill)
			{
				grogu_tree.EnableNav(true);
				if (groguText != null)
					groguText.Enable(false);
			}
		}
		if (weapon_tree != null && weapon_tree_activation_point != null)
		{
			if (mando.GetComponent<Transform>().globalPosition.DistanceNoSqrt(weapon_tree_activation_point.GetComponent<Transform>().globalPosition) < maximum_distance_to_interact_squared_skill)
			{
				weapon_tree.EnableNav(true);
				if (weaponText != null)
					weaponText.Enable(false);
			}
		}
	}

}
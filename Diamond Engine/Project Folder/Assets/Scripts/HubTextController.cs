using System;
using DiamondEngine;


public class HubTextController : DiamondComponent
{
	public enum Interaction
	{
		NONE,
		BO_KATAN,
		GREEF,
		ASHOKA,
		GROGU
	}

	public GameObject textController = null;

	public GameObject mando = null;
	public GameObject bo_katan = null;
	public GameObject greef = null;
	public GameObject ashoka = null;
	public GameObject grogu = null;

	public GameObject bo_katan_portrait = null;
	public GameObject greef_portrait = null;
	public GameObject ashoka_portrait = null;
	public GameObject grogu_portrait = null;



	public int total_interactions = 0;
	public int total_stages = 0;

	public float maximum_distance_to_interact_squared = 0.0f;

	private int bo_katan_stage = 1;
	private int greef_stage = 1;
	private int ashoka_stage = 1;
	private int grogu_stage = 1;

	private int bo_katan_interaction_num = 1;
	private int greef_interaction_num = 1;
	private int ashoka_interaction_num = 1;
	private int grogu_interaction_num = 1;

	private bool showtext = true;
	private int total_interactions_and_stages = 0;
	private bool start = false;

	

	Interaction interaction = Interaction.NONE;
	public void Update()
	{
		if (start)
        {
			total_interactions_and_stages = total_stages * total_interactions;
			start = false;
        }

		if (mando == null && Input.GetGamepadButton(DEControllerButton.A) != KeyState.KEY_DOWN && textController != null) 
			return;

		interaction = Interaction.NONE;

		if (bo_katan != null)
        {
			if (mando.GetComponent<Transform>().localPosition.DistanceNoSqrt(bo_katan.GetComponent<Transform>().localPosition) < maximum_distance_to_interact_squared)
				interaction = Interaction.BO_KATAN;
        }

		if (interaction == Interaction.NONE && greef != null)
		{
			if (mando.GetComponent<Transform>().localPosition.DistanceNoSqrt(greef.GetComponent<Transform>().localPosition) < maximum_distance_to_interact_squared)
				interaction = Interaction.GREEF;
		}

		if (interaction == Interaction.NONE && ashoka != null)
		{
			if(mando.GetComponent<Transform>().localPosition.DistanceNoSqrt(ashoka.GetComponent<Transform>().localPosition) < maximum_distance_to_interact_squared)
				interaction = Interaction.ASHOKA;
		}

		if (interaction == Interaction.NONE && grogu != null)
		{
			if(mando.GetComponent<Transform>().localPosition.DistanceNoSqrt(grogu.GetComponent<Transform>().localPosition) < maximum_distance_to_interact_squared)
				interaction = Interaction.GROGU;
		}

		if (interaction == Interaction.NONE)
			return;

		showtext = true;

        switch (interaction)
        {
			case Interaction.BO_KATAN:
				if (bo_katan_portrait != null)
					textController.GetComponent<TextController>().otherimage.GetComponent<Image2D>().SwapTwoImages(bo_katan_portrait); //ARNAU: WILL BE NEEDED TO BE CHANGED
				textController.GetComponent<TextController>().dialog_index = bo_katan_interaction_num;
				if (bo_katan_interaction_num % 3 != 0)
					bo_katan_interaction_num++;
				break;
			case Interaction.GREEF:
				if (greef_portrait != null)
					textController.GetComponent<TextController>().otherimage.GetComponent<Image2D>().SwapTwoImages(greef_portrait); //ARNAU: WILL BE NEEDED TO BE CHANGED
				textController.GetComponent<TextController>().dialog_index = (total_interactions_and_stages) + greef_interaction_num;
				if (greef_interaction_num % 3 != 0)
					greef_interaction_num++;
				break;
			case Interaction.ASHOKA:
				if (ashoka_portrait != null)
					textController.GetComponent<TextController>().otherimage.GetComponent<Image2D>().SwapTwoImages(ashoka_portrait); //ARNAU: WILL BE NEEDED TO BE CHANGED
				textController.GetComponent<TextController>().dialog_index = (total_interactions_and_stages * 2) + ashoka_interaction_num;
				if (ashoka_interaction_num % 3 != 0)
					ashoka_interaction_num++;
				break;
			case Interaction.GROGU:
				if (grogu_portrait != null)
					textController.GetComponent<TextController>().otherimage.GetComponent<Image2D>().SwapTwoImages(grogu_portrait); //ARNAU: WILL BE NEEDED TO BE CHANGED
				textController.GetComponent<TextController>().dialog_index = (total_interactions_and_stages * 3) + grogu_interaction_num;
				if (grogu_interaction_num % 2 != 0)
					grogu_interaction_num++;
				break;
		}

		if (showtext)
        {
			textController.Enable(true);
			showtext = false;
        }
	}

	public void IncreaseStage(Interaction interaction_to_increase_stage)
    {
        switch (interaction_to_increase_stage)
        {
			case Interaction.BO_KATAN:
				if(bo_katan_stage <= total_stages)
                {
					bo_katan_interaction_num = (bo_katan_stage * total_interactions) + 1;
					++bo_katan_stage;
                }
				break;
			case Interaction.GREEF:
				if (greef_stage <= total_stages)
				{
					greef_interaction_num = (greef_stage * total_interactions) + 1;
					++greef_stage;
				}
				break;
			case Interaction.ASHOKA:
				if (ashoka_stage <= total_stages)
				{
					ashoka_interaction_num = (ashoka_stage * total_interactions) + 1;
					++ashoka_stage;
				}
				break;
			case Interaction.GROGU:
				if (grogu_stage <= total_stages)
				{
					grogu_interaction_num = (grogu_stage * (total_interactions-1)) + 1;
					++grogu_stage;
				}
				break;
		}
    }
}
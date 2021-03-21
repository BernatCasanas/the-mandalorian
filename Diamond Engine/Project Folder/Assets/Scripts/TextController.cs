using System;
using System.Collections.Generic;
using DiamondEngine;

public class TextController : DiamondComponent
{
	public GameObject gui = null;
	public GameObject dialog = null;
	public GameObject text = null;
	public GameObject mandoimage = null;
	public GameObject groguimage = null;


	private int index = 0;
	private bool gui_not_enabled = true;

	private List<string> texts = new List<string>()
	{
		"Well kid, we should get going if we want to kill that     Rancor before nightfall. You know this desert can get  really dangerous at night, with all those Tusken           Raiders creeping around.",
		"(Looks at him, without any light of understandment      shining in his eyes)",
		"You don’t remember? We agreed with that woman thatif we defeated the Rancor that escaped from Jabba’s    palace she would help us find the Jedi.",
		"We need her if we want to finally find the Moff Gideonand put an end to this story, I don’t want you to be in   danger all the time.",
		"(Closes his eyes, as if just hearing that name scared    him)",
		"It’s ok kid, don’t worry, he’s not here now, he cannot     harm you. But we have to defeat him so he leaves us   alone, you know he won’t rest until we stop him.",
		"(Slowly calms and opens his eyes, still scared, and     looks at MANDO)",
		"Let’s go now, this zone of Tatooine has been controlledby the Empire since Jabba fell, and even now that       it’s gone I wouldn’t expect a pleasant ride.",

	};
	private List<string> images = new List<string>()
	{
		"Mando",
		"Grogu",
		"Mando",
		"Mando",
		"Grogu",
		"Mando",
		"Grogu",
		"Mando",
	};

	private bool startMenu = true;
	public void OnExecuteButton()
    {
		if (++index < texts.Count)
		{

			text.GetComponent<Text>().text = texts[index];
			if (images[index] == "Mando")
			{
				mandoimage.Enable(true);
				groguimage.Enable(false);
			}
			else
			{
				mandoimage.Enable(false);
				groguimage.Enable(true);
			}

		}

		if (index >= texts.Count && gui_not_enabled)
		{
			dialog.Enable(false);
			gui.Enable(true);
			gui.GetComponent<HUD>().UpdateCombo(0, 0, 0);
			gui_not_enabled = false;
			Time.ResumeGame();
		}
	}
	public void Update()
	{
		if (startMenu == true)
        {
			gui.Enable(false);
			startMenu = false;
			Time.PauseGame();
		}

		
	}

}
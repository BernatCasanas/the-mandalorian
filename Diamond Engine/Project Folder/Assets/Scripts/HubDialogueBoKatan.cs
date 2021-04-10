using System;
using System.Collections.Generic;
using DiamondEngine;

public class HubDialogueBoKatan : DiamondComponent
{
	public GameObject dialog = null;
	public GameObject text = null;
	public GameObject mandoimage = null;
	public GameObject boimage = null;

	private int index = 0;

	private List<string> texts = new List<string>()
	{
		"What a coincidence. Taking a break from your reclaiming of Mandalore?",
		"Do not speak as if you knew what it means to be a Mandalorian, bounty hunter. You are just a member of a cult of zealots who got lost in an ancient faith.",
		"It is nice to see you too.",
		"Whatever, you wouldn’t understand.",
		"What’s there to understand? There’s nothing on that planet worth fighting for. All the stories I’ve heard tell that Mandalore is cursed.",
		"These stories you have been fed are lies. It is true that nowadays little remains of what once was, but one day Mandalore will regain its splendour, and I will be there to see it.",

	};
	private List<string> images = new List<string>()
	{		
		"Mando",
		"Bo Katan",
		"Mando",
		"Bo Katan",
		"Mando",
		"Bo Katan",		
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
				boimage.Enable(false);
			}
			else
			{
				mandoimage.Enable(false);
				boimage.Enable(true);
			}
		}

		if (index >= texts.Count)
		{
			dialog.Enable(false);
			Time.ResumeGame();
		}
	}

	public void Update()
	{
		if (startMenu == true)
		{
			startMenu = false;
			Time.PauseGame();
		}
	}
}
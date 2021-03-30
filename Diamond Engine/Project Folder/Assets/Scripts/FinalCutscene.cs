using System;
using System.Collections.Generic;
using DiamondEngine;

public class FinalCutscene : DiamondComponent
{
	public GameObject gui = null;
	public GameObject dialog = null;
	public GameObject text = null;
	public GameObject mandoimage = null;
	public GameObject boimage = null;


	private int index = 0;
	private bool gui_not_enabled = true;

	private List<string> texts = new List<string>()
	{
		"Look who we got here. I honestly didn't know if you were going to make it.",
		"Wow, thanks for the trust you put in me. Also, pretty   bold to say that for the woman that was too scared to  do it herself. Anyways, I did it, so could you please     tell me...",
		"Don't you dare say I was scared of a simple beast       again! I am a true Mandalorian warrior, you'd know    what that means if you were really one of us, and not  one of those fools from your cult. If I asked you to do  it it's just because I wanted to test you.",
		"Test me? Test me for what?",
		"Well, as you already know, I'm trying to retake           Mandalore. I just wanted to see if you were a worthy   warrior, strong enough to help us bring our home      world back to its glory days. And you have proven      yourself indeed, so I want you to accompany us in thisimportant mission.",
		"Look, everyone knows Mandalore is cursed, i wouldn'tgo there if I were you. Besides that I already have a     mission, I have to protect this kid. And to do that I       need the information you promised me, so if you are  kind enough...",
		"I see... Let's make another deal. I'll give you the          information you need if, in exchange, you help me     retake Mandalore once you are done with your 'far     more important' mission. I think your abilities would    really help us, and it's your duty if you truly are a        Mandalorian.",
		"Wait, this doesn't seem fair. We already made a deal,remember?",
		"Come on, was this even an inconvenience to you,       killing that Rancor? Because if that's the case maybe I got the wrong person for the job. Maybe you are not atrue Mandalorian warrior after all...",
		"Alright, alright.. I'll help you retake Mandalore, but     only after I'm done with my mission. No matter how    long it takes.",
		"Ok Mando, as you wish. I'll be waiting for you to finishwhatever it is you're doing, gathering as many            warriors I can to help us. Now to the information you  wanted; our fellow friend Ahsoka Tano. Can I ask whyyou want to know where she is?",
		"No, you cannot. I need her help with the kid, that's     what you need to know.",
		"Fair enough, I guess it's none of my business. She is   at Dathomir, last thing I heard she was investigating    something about the Force in a Nightsisters temple.    But you know what they say about that planet; if you   think Mandalore is cursed you won't like Dathomir.", 
		"Stories of dead people walking, deadly creatures...     and not to mention the Nightbrothers. I still vividly       remember the only two I've known... and how they      overthrew Mandalore's government once.",
		"*Snaps out* Look I don0t want to bore you with old    stories. The important thing is that you should be        careful there.",
		"It's ok, I'll be fine. Thanks for everything, Bo-Katan.",
		"You're welcome. Can I ask for one last thing before    you go?",
		"I hope it's not another one of your deals.",
		"Nothing of that. When you find Ahsoka, could you askher if she'd like to help us with our mission?",
		"No problem. See you soon, Bo-Katan.",
		"May the force be with you, friend.",
	};
	private List<string> images = new List<string>()
	{
		"Bo Katan",
		"Mando",
		"Bo Katan",
		"Mando",
		"Bo Katan",
		"Mando",
		"Bo Katan",
		"Mando",
		"Bo Katan",
		"Mando",
		"Bo Katan",
		"Mando",
		"Bo Katan",
		"Bo Katan",
		"Bo Katan",
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

		if (index >= texts.Count && gui_not_enabled)
		{
			dialog.Enable(false);
			gui.Enable(true);
            gui.GetComponent<HUD>().ResetCombo();
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
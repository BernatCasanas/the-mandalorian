using System;
using DiamondEngine;
using System.Collections.Generic;


public class List_Of_Dialogs : DiamondComponent
{

	private List<string> Final_Cutscene = new List<string>()
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

	private List<bool> Final_Cutscene_bool = new List<bool>()
	{
		// Mando true, other false
		false,
		true,
		false,
		true,
		false,
		true,
		false,
		true,
		false,
		true,
		false,
		true,
		false,
		false,
		false,
		true,
		false,
		true,
		false,
		true,
		false,
	};

	private List<string> Initial_Cutscene = new List<string>()
	{
		"Well kid, we should get going if we want to kill that     Rancor before nightfall. You know this desert can get  really dangerous at night, with all those Tusken           Raiders creeping around.",
		"(Looks at him, without any light of understandment      shining in his eyes)",
		"You don't remember? We agreed with that woman     that if we defeated the Rancor that escaped from        Jabba's palace she would help us find the Jedi.",
		"We need her if we want to finally find the Moff Gideonand put an end to this story, I don't want you to be in  danger all the time.",
		"(Closes his eyes, as if just hearing that name scared    him)",
		"It's ok kid, don't worry, he's not here now, he cannot  harm you. But we have to defeat him so he leaves us   alone, you know he won't rest until we stop him.",
		"(Slowly calms and opens his eyes, still scared, and     looks at MANDO)",
		"Let's go now, this zone of Tatooine has been              controlled by the Empire since Jabba fell, and even     now that it's gone I wouldn't expect a pleasant ride.",
	};

	private List<bool> Initial_Cutscene_bool = new List<bool>()
	{
		// Mando true, other false
		true,
		false,
		true,
		true,
		false,
		true,
		false,
		true,
	};

	private List<string> Bo_Katan_0_1 = new List<string>()
	{
		"What a coincidence. Taking a break from your reclaiming of Mandalore?",
		"Do not speak as if you knew what it means to be a Mandalorian, bounty hunter. You are just a member of a cult of zealots who got lost in an ancient faith.",
		"It is nice to see you too.",
		"Whatever, you wouldn’t understand.",
		"What’s there to understand? There’s nothing on that planet worth fighting for. All the stories I’ve heard tell that Mandalore is cursed.",
		"These stories you have been fed are lies. It is true that nowadays little remains of what once was, but one day Mandalore will regain its splendour, and I will be there to see it.",
	};

	private List<bool> Bo_Katan_0_1_bool = new List<bool>()
	{
		// Mando true, other false
		true,
		false,
		true,
		false,
		true,
		false,
	};

	private List<string> Bo_Katan_0_2 = new List<string>()
	{
		"Why did you really come here? Was it to convince me to help you in your campaign?",
		"Maybe. You made a promise, remember? I helped you find Moff Gideon.",
		"You only pointed me in the direction of Ahsoka Tano. Helping in reclaiming a planet for telling where somebody is… not a very fair deal to be honest.",
		"A deal is a deal. Are you backing out from your word?",
		"No, a deal is a deal. I always honour my word.",
	};

	private List<bool> Bo_Katan_0_2_bool = new List<bool>()
	{
		// Mando true, other false
		true,
		false,
		true,
		false,
		true,
	};

	private List<string> Bo_Katan_0_3 = new List<string>()
	{
		"Isn’t there some other way I could help you?",
		"If you are not going to fight with us, you could give your beskar armour to a real Mandalorian who is willing to fight.",
		"You know that’s never going to happen.",
		"Then this means you will fight with us, right?",
		"Argh… I guess so. You are relentless. What other choice do I have?",
		"If you consider yourself to be a Mandalorian, none-other.",
	};

	private List<bool> Bo_Katan_0_3_bool = new List<bool>()
	{
		// Mando true, other false
		true,
		false,
		true,
		false,
		true,
		false,
	};

	public List<String> GetListOfDialog(uint index)
	{
        switch (index)
        {
			case 0:
				return Initial_Cutscene;
			case 1:
				return Bo_Katan_0_1;
			case 2:
				return Bo_Katan_0_2;
			case 3:
				return Bo_Katan_0_3;
			case 4:
				return Final_Cutscene;
		}
		return Initial_Cutscene;

	}

	public List<bool> GetListOfOrder(uint index)
	{
		switch (index)
		{
			case 0:
				return Initial_Cutscene_bool;
			case 1:
				return Bo_Katan_0_1_bool;
			case 2:
				return Bo_Katan_0_2_bool;
			case 3:
				return Bo_Katan_0_3_bool;
			case 4:
				return Final_Cutscene_bool;
		}
		return Initial_Cutscene_bool;
	}

}
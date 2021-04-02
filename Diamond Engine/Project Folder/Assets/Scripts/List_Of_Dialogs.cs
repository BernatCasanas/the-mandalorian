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
		"Whatever, you wouldn't understand.",
		"What's there to understand? There's nothing on that planet worth fighting for. All the stories I've heard tell that Mandalore is cursed.",
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
		"You only pointed me in the direction of Ahsoka Tano. Helping in reclaiming a planet for telling where somebody is... not a very fair deal to be honest.",
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
		"Isn't there some other way I could help you?",
		"If you are not going to fight with us, you could give your beskar armour to a real Mandalorian who is willing to fight.",
		"You know that's never going to happen.",
		"Then this means you will fight with us, right?",
		"Argh... I guess so. You are relentless. What other choice do I have?",
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

	private List<string> Bo_Katan_1_1 = new List<string>()
	{
		"I hope you didn't give me this thinking that I would drink it bounty hunter. It is unapologetically disgusting.",
		"Oh, excuse me your majesty. Is this not sophisticated enough to be worthy of your royal stomach?",
		"Very funny. Nobody would have guessed you were capable of having a sense of humour. But for your information, I am actually the last in the line of clan Kryze, a clan of rulers. I was once the Mand'alor, which is the title bestowed upon the sole leader of the Mandalorian people. Therefore, you can call me majesty if you wish.",
		"I'm definitely not doing that... If you were so great, what happened?",
		"Well, the great purge happened. And I lo... we lost everything. We fought with all our might, but nobody could have faced the full power of the Empire. That was not a battle, it was an execution for our people.",
		"I've heard about that from The Tribe. They say it was the worst that had ever happened to Mandalore, a complete genocide.",
		"Yes, it was... It really was...",

	};

	private List<bool> Bo_Katan_1_1_bool = new List<bool>()
	{
		// Mando true, other false
		false,
		true,
		false,
		true,
		false,
		true,
		false,
	};

	private List<string> Bo_Katan_1_2 = new List<string>()
	{
		"Say Bo-Katan, if you already were the leader of Mandalore, why don't you unite all the mandalorians once again and take the planet? Now that the Empire has been defeated it should be possible.",
		"And what do you exactly think I've been trying to do all this time boy? You do not know about our culture, it is not that easy. Besides, do you really think that the Empire has been defeated?",
		"No... I guess not. If it had been defeated, I would have had half the trouble protecting Grogu.",
		"Exactly. They are still lurking in the shadows, waiting for the right opportunity to strike. Moff Gideon was only one of many, and something much worse is still to come. An impending doom is looming in this galaxy, you should have realized it by now.",
	};

	private List<bool> Bo_Katan_1_2_bool = new List<bool>()
	{
		// Mando true, other false
		true,
		false,
		true,
		false,
	};

	private List<string> Bo_Katan_1_3 = new List<string>()
	{
		"Why did you say that it's not that easy to bring all the mandalorians together. Doesn't being the leader give you any kind of authority?",
		"I am not the leader anymore. The title of Mand'alor is not given, it is earned, and in the same way as it is earned, it can be lost. When we were defeated by the empire in the great purge and Moff Gideon took the Darksaber from me I lost the title.",
		"The Darksaber?",
		"It is a powerful ancient weapon that belongs to the mandalorians, a black lightsaber. The one who wields it will have a claim to the title of Mand'alor. I must get it back, and this time the right way.",
	};

	private List<bool> Bo_Katan_1_3_bool = new List<bool>()
	{
		// Mando true, other false
		true,
		false,
		true,
		false,
	};

	private List<string> Bo_Katan_2_1 = new List<string>()
	{
		"Another one? I already told you I cannot drink this, just looking at it is unpleasant.",
		"You are welcome. What happened the last time you acquired the Darksaber?",
		"I was blinded by the chance of becoming the leader of Mandalore. I had longed for that moment for a long time, and since everyone seemed to agree, I accepted the Darksaber from Sabine Wren, a fellow Mandalorian and rightful owner of the blade. That was naive of me.",
		"I see nothing wrong with that. If everyone agreed, you did the right thing.",
		"They surely agreed at that time. However, when the great purge occurred and I lost Mandalore, they changed their mind. Given that I had not really earned the weapon, they thought I had been an illegitimate ruler and we had lost because of my weakness.",
		"It was not your fault. The moment the Empire put its gaze on Mandalore, your fate was sealed.",
		"I do not need your consolation words bounty hunter! But thanks...",
	};

	private List<bool> Bo_Katan_2_1_bool = new List<bool>()
	{
		// Mando true, other false
		false,
		true,
		false,
		true,
		false,
		true,
		false,
	};

	private List<string> Bo_Katan_2_2 = new List<string>()
	{
		"How did you gain the people's approval to become their new leader? I'm guessing not thanks to your charming skills...",
		"Like you are one to talk. Stop trying to develop a sense of humour, thanks. That is a long story.",
		"I have time.",
		"*Sigh*, Then, long story short: my sister had been the leader of Mandalore for a long time and after her death, I was next in line. I helped defeat a dark lord of the Sith who had usurped the position. I had already been appointed regent for a small period of time. I also helped reclaim Mandalore from the hands of a clan that had bowed to the empire and ruled Mandalore under their supervision. And there were other things...",
		"You really didn't make that up right now did you? And here I thought I had lived an exciting life...",
		"I already told you; you know nothing of Mandalore.",
	};

	private List<bool> Bo_Katan_2_2_bool = new List<bool>()
	{
		// Mando true, other false
		true,
		false,
		true,
		false,
		true,
		false,
	};

	private List<string> Bo_Katan_2_3 = new List<string>()
	{
		"Hold on a second, to have lived through so much... How old are you?",
		"Well, that is rude. But coming from you, should I be surprised? I assume they do not teach manners in your cult.",
		"*Tsch* The Tribe is not a cult. If not for them I would be dead, you know?! I owe them everything. They are good people... But seriously, when were you born?",
		"You are hopeless. I will not tell you.",
		"Ohh pleaseee.",
		"Stop being annoying.",
		"Ok, I'll guess then. You must be... in your mid-forties... No, early fifties?",
		"You impertinent cheeky...",
		"Aha, so I did get it right. It's incredible what cosmetic surgery can do these days...",
		"If I ever meet you alone, you are dead.",
	};

	private List<bool> Bo_Katan_2_3_bool = new List<bool>()
	{
		// Mando true, other false
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
	};

	private List<string> Grogu_0_1 = new List<string>()
	{
		"Hey kid, how are you doing? We've been quite busy lately, haven't we? Almost no time to slow down and have a chat.",
		"(Little Grogu looks at Mando perplexed, though he is 50 years old and wiser than what most would expect, he has yet to entangle the mysteries of the spoken language.)",
		"Of course... For some reason I tend to forget you can't talk.",
		"(An innocent and pure smile is outlined in the thin, green, almost non-existent lips of the child.)",
		"For a second I almost imagined that you would start talking as if you had always known how, but chose not to do it.",
		"(Unable to understand what his beloved caretaker is telling him, Grogu cannot but look puzzled and utter a short array of unintelligible though unfairly cute baby noises...)",
		"Well, hit me up when you learn some words.",
	};

	private List<bool> Grogu_0_1_bool = new List<bool>()
	{
		// Mando true, other false
		true,
		false,
		true,
		false,
		true,
		false,
		true,
	};

	private List<string> Grogu_0_2 = new List<string>()
	{
		"Anything new you want to tell me about?",
		"(The adorable creature extends his arm while opening and closing his three short fingers, he definitely wants something, but his hand can only grasp thin air.)",
		"Sorry kid, I don’'t have your ball on me right now. It's in the ship. And before you ask me, the answer is no; I'm not going back there to get it for you.",
		"(The long and pointy ears of the child start lowering down, a small cry of sadness escapes his little mouse.)",
		"I told you I'm not going back for a stupid little ball. Next time, remind me about it before we leave the ship. If you are so sad, you can go and get it yourself.",
		"(The kid starts sitting up. It seems like, surprisingly he has understood what Din just said.)",
		"Argh... Ok you win.Here you go... Yes, I know I lied to you. I just didn't want you dropping it somewhere, because then it is me who has to go looking around everywhere for the darn ball. So, don't drop it, or next time there will be no ball.",
		"(Little Grogu, who appeared to be listening carefully to what Mando was saying, in reality has not understood a single thing. Therefore, he gives his big silver friend an adorable smile to whom no one in the galaxy could say no, and joyfully takes the ball in his hands.)",
	};

	private List<bool> Grogu_0_2_bool = new List<bool>()
	{
		// Mando true, other false
		true,
		false,
		true,
		false,
		true,
		false,
		true,
		false,
	};

	private List<string> Greef_0_1 = new List<string>()
	{
		"Ahh, if it isn't the great Mando himself. How is my best bounty hunter doing these days?",
		"I no longer work for you Greef. I'm doing fine, thanks.",
		"Isn't this nice? All of us, together again, here sharing old memories... I've been rather busy these past months, but now, I'm feeling much better.",
		"What have you been up to? Don't tell me you've got meddled up in business with another nasty client.",
		"On the contrary my friend. I've been trying to make Nevarro a better place. I find your lack of trust in me quite painful. Now that I'm the magistrate once again, and with Cara's help, we are going to turn that hole into something decent.",
		"Ah yes, I remember you were trying to do something like that. And since you turned your canteen into a school, now you have to go to other planets to get boozed-up.",
		"One can still enjoy a drink in Nevarro, but places like this one are often a den for ruffians. We've just made it safer for kids and stuff. Pay us a visit next time you fly close to the planet. You'll be surprised to see how it is changed.",
	};

	private List<bool> Greef_0_1_bool = new List<bool>()
	{
		// Mando true, other false
		false,
		true,
		false,
		true,
		false,
		true,
		false,
	};

	private List<string> Greef_0_2 = new List<string>()
	{
		"I bet you are enjoying your new position as magistrate Greef. You've always liked to order people around.",
		"You're right, I like this job, but I prefer to think I got back what was mine. You know I'd already been the magistrate for a long time before those nerf herders from the New Republic came to power and unfairly fired me.",
		"Sure, sure... Because you wouldn't happen to be running some underground business of questionable ethicality, which included illegal trafficking, assassination contracts...",
		"Hey, as far as I can remember, you and your Mandalorian friends greatly benefited from such contracts.",
		"I didn't kill anyone who didn't deserve it. And I usually brought my targets alive.",
		"Tell yourself that if it makes you sleep any better. We did what we had to do to survive, and if it hadn’t been me, someone else would have taken my place. It is just how it is. Under the rule of the Empire, this was the way. But well, as you can see, this has changed now.",
	};

	private List<bool> Greef_0_2_bool = new List<bool>()
	{
		// Mando true, other false
		true,
		false,
		true,
		false,
		true,
		false,
	};

	private List<string> Greef_0_3 = new List<string>()
	{
		"There's one thing that still doesn't convince me. What you are doing in Nevarro... do you really believe in it? Or are you just doing it to keep your new job?",
		"You genuinely think that low of me... Look, I could have just cleaned the place up a bit and buried all the bad stuff, because it doesn't matter. Nobody is going to look twice to a planet like Nevarro. But I'm honestly trying. I'm really making an effort to change things. I want people to know that they can safely grow their kids there, and live comfortable lives.",
		"Alright, I believe you. Now that Cara is helping you, I think you might actually be able to achieve what you're saying. It's not that I don't trust you, but I know you're only loyal to yourself.",
		"My days of messing around with wrongdoers are over. It is painful to admit it, but one has already a certain age here. What I want now is peace and tranquillity.",
		"It is quite amusing to imagine the infamous Greef Karga as an old gramps, in a park, seated on a bench while watching the children play.",
		"Very funny. Be careful with what you say. I might not be in my golden years, but I still have a lot in me. I'm still an agent of the Guild and he who underestimates me will find himself in a bad spot. But well, you just watch Mando, next time you come to Nevarro you won't be able to recognize it.",
	};

	private List<bool> Greef_0_3_bool = new List<bool>()
	{
		// Mando true, other false
		true,
		false,
		true,
		false,
		true,
		false,
	};

    private List<string> Ahsoka_0_1 = new List<string>()
    {
       "Ahsoka Tano… You are here too? That’s unexpected. This canteen is starting to get crowded.",
       "It is nice to meet you again, Mandalorian. I am glad to see that you and Grogu are fine. Since we parted ways in Dathomir, you two had me worried.",
       "There's no need for that, rest assured that I’m perfectly capable of fending both myself and the kid.",
       "I know, and it was not my intention to offend you, I apologize if I did so, you are quite competent. However, going against a Moff is a dangerous endeavour, even for somebody like you.",
       "Well, we survived. I appreciate your concern, but it was unnecessary. What brings you here Jedi?",
       "I have been wanting to discuss with Bo-Katan the whole Mandalore affair. I do not know if I could speak of her as a friend, but we go back a long time… I do not want to lose her too. Furthermore, I wanted to check on Grogu.",
    };

    private List<bool> Ahsoka_0_1_bool = new List<bool>()
    {
		// Mando true, other false
		true,
        false,
        true,
        false,
        true,
        false,
    };

    private List<string> Ahsoka_0_2 = new List<string>()
    {
       "So, have you changed your mind about training the kid? I still need to find him a Jedi master.",
       "I am sorry, my answer is still no. I am not a Jedi, I left the Order a long time ago. And I know that Grogu's place lies not with me.",
       "I don't know any other person who can teach him. Are there really any Jedi left nowadays? Because it doesn't look like it. I have the feeling I'm chasing ghosts.",
       "I am afraid these feelings you have are not without foundation. The Jedi Order disappeared at the end of the clone wars, and most of its survivors died shortly afterwards. I admit, yours is not an easy task Mandalorian.",
       "Then it's true. There are no longer any Jedi left...",
       "The Jedi Order might be no more, but for the past years, there has been balance in the force. There are some out there who are still following the old Jedi ways. The light side of the force is strong with them. If it is Grogu's fate to be trained, you will find them... or perchance... they will find you.",
    };

    private List<bool> Ahsoka_0_2_bool = new List<bool>()
    {
		// Mando true, other false
		true,
        false,
        true,
        false,
        true,
        false,
    };

    private List<string> Ahsoka_0_3 = new List<string>()
    {
       "Why haven't you tried to find any of these Jedi? The light side is the good side, right? They could prove valuable allies.",
       "As a matter of fact, I have spent these last years searching for one of them in particular. He was a friend of mine. Last time we met he saved my life, and before splitting up, I promised I would find him later. However, he vanished.",
       "Maybe I should help you. If we found him, could he be able to train Grogu?",
       "Thanks, but this is something I have to do by myself. At that time Ezra was good, though not a fully-fledged Jedi Knight. I do not know how much he has improved during this time, but he might not be ready to become a master yet.",
       "Great, another dead end, my odds are only getting better... Do you have any lead on where you could find him?",
       "Only one. If I want to find him, I must find another man. He was a grand admiral and the most brilliant strategist the old Galactic Empire ever possessed... He was best known as Thrawn.",
    };

    private List<bool> Ahsoka_0_3_bool = new List<bool>()
    {
		// Mando true, other false
		true,
        false,
        true,
        false,
    };

    private List<string> Ahsoka_1_1 = new List<string>()
    {
       "Blue milk... Why are you giving me this Mandalorian? It was one of the most popular beverages among the Jedi, did you know that?",
       "Sure, sure, that's why I'm giving it to you. Enjoy it.",
       "I can sense you are lying.",
       "Hey, don't play mind tricks on me Jedi...",
       "I am not a Jedi, and I do not need to use the force to see through you. You are not a particularly good liar.",
       "Ok, alright... Honestly, I didn't know what to do with it, so I figured maybe you would like it.",
       "It is fine. I appreciate it. In the future, should you offer me more, I will gladly accept it.",
    };

    private List<bool> Ahsoka_1_1_bool = new List<bool>()
    {
		// Mando true, other false
		false,
        true,
        false,
        true,
        false,
        true,
        false,
    };

    private List<string> Ahsoka_1_2 = new List<string>()
    {
        "You keep saying you're not a Jedi. Still, you fight with lightsabers, you use the force, and you're on the good guys' side. I have never met a Jedi, but from the stories I've heard, you tick all the boxes... Help me understand.",
        "Since I was a child, I was raised by Jedi in the Jedi ways... Eventually, I realized that things are not as black and white as they had taught me. While in The Order, I was so naively blinded by our just cause, that I was unable to see the dark shadows our bright light had casted. Then I understood; there lies darkness in light, and likewise, one can find light in darkness.",
        "Am I supposed to understand what all this means? Now I’m even more confused.",
        "There is no such thing as simply good and bad people, we are far more complex. I was not sure we were the 'good guys' anymore. What I had once considered my home, became unfamiliar, and where before I had seen friendly faces, I could only see strangers. It became clear to me that I no longer belonged in The Order.",
        "I'm going to pretend that this was more clarifying than before... You make it sound as if Jedi were bad people. Why should Grogu become one then? It doesn't seem like a good idea anymore.",
        "No, do not get me wrong. The Jedi were definitely trying to do good, it is just... those were hard times, and they had to make a lot of difficult decisions. I do not think it has ever been tougher for the members of the Jedi council. I even considered rejoining after a while, but then order 66 was issued and... Well, it was too late…",
    };

    private List<bool> Ahsoka_1_2_bool = new List<bool>()
    {
		// Mando true, other false
		true,
        false,
        true,
        false,
        true,
        false,
    };

    private List<string> Ahsoka_1_3 = new List<string>()
    {
        "",
        "",
        "",
        "",
        "",
        "",
    };

    private List<bool> Ahsoka_1_3_bool = new List<bool>()
    {
		// Mando true, other false
		true,
        false,
        true,
        false,
        true,
        false,
    };

    private List<string> Ahsoka_2_1 = new List<string>()
    {
        "",
        "",
        "",
        "",
        "",
        "",
    };

    private List<bool> Ahsoka_2_1_bool = new List<bool>()
    {
		// Mando true, other false
		true,
        false,
        true,
        false,
        true,
        false,
    };

    private List<string> Ahsoka_2_2 = new List<string>()
    {
        "",
        "",
        "",
        "",
        "",
        "",
    };

    private List<bool> Ahsoka_2_2_bool = new List<bool>()
    {
		// Mando true, other false
		true,
        false,
        true,
        false,
        true,
        false,
    };

    private List<string> Ahsoka_2_3 = new List<string>()
    {
        "",
        "",
        "",
        "",
        "",
        "",
    };

    private List<bool> Ahsoka_2_3_bool = new List<bool>()
    {
		// Mando true, other false
		true,
        false,
        true,
        false,
        true,
        false,
    };

    private List<string> Ahsoka_3_1 = new List<string>()
    {
        "",
        "",
        "",
        "",
        "",
        "",
    };

    private List<bool> Ahsoka_3_1_bool = new List<bool>()
    {
		// Mando true, other false
		true,
        false,
        true,
        false,
        true,
        false,
    };

    private List<string> Ahsoka_3_2 = new List<string>()
    {
        "",
        "",
        "",
        "",
        "",
        "",
    };

    private List<bool> Ahsoka_3_2_bool = new List<bool>()
    {
		// Mando true, other false
		true,
        false,
        true,
        false,
        true,
        false,
    };

    private List<string> Ahsoka_3_3 = new List<string>()
    {
        "",
        "",
        "",
        "",
        "",
        "",
    };

    private List<bool> Ahsoka_3_3_bool = new List<bool>()
    {
		// Mando true, other false
		true,
        false,
        true,
        false,
        true,
        false,
    };

    private List<string> Ahsoka_4_1 = new List<string>()
    {
        "",
        "",
        "",
        "",
        "",
        "",
    };

    private List<bool> Ahsoka_4_1_bool = new List<bool>()
    {
		// Mando true, other false
		true,
        false,
        true,
        false,
        true,
        false,
    };

    private List<string> Ahsoka_4_2 = new List<string>()
    {
        "",
        "",
        "",
        "",
        "",
        "",
    };

    private List<bool> Ahsoka_4_2_bool = new List<bool>()
    {
		// Mando true, other false
		true,
        false,
        true,
        false,
        true,
        false,
    };

    private List<string> Ahsoka_4_3 = new List<string>()
    {
        "",
        "",
        "",
        "",
        "",
        "",
    };

    private List<bool> Ahsoka_4_3_bool = new List<bool>()
    {
		// Mando true, other false
		true,
        false,
        true,
        false,
        true,
        false,
    };

    private List<string> Ahsoka_5_1 = new List<string>()
    {
        "",
        "",
        "",
        "",
        "",
        "",
    };

    private List<bool> Ahsoka_5_1_bool = new List<bool>()
    {
		// Mando true, other false
		true,
        false,
        true,
        false,
        true,
        false,
    };

    private List<string> Ahsoka_5_2 = new List<string>()
    {
        "",
        "",
        "",
        "",
        "",
        "",
    };

    private List<bool> Ahsoka_5_2_bool = new List<bool>()
    {
		// Mando true, other false
		true,
        false,
        true,
        false,
        true,
        false,
    };

    private List<string> Ahsoka_5_3 = new List<string>()
    {
        "",
        "",
        "",
        "",
        "",
        "",
    };

    private List<bool> Ahsoka_5_3_bool = new List<bool>()
    {
		// Mando true, other false
		true,
        false,
        true,
        false,
        true,
        false,
    };

    //////////////////////////////////////////////////////////////////////////////////////// CARA DUNE ////////////////////////////////////////////////////////////////////////////////////////

    private List<string> Cara_0_1 = new List<string>()
    {
        "How are you doing Cara?",
        "You know... here, chatting about old times. I feel like an old granny.",
        "What are you talking about? It hasn't even been that long since we met that day in Sorgan.",
        "I guess not, but seems like it was ages ago, doesn't it? We have been through enough trouble; a damn fine novel could be written out of us.",
        "I guess we've had our own fair share of action. You surprise me, I didn't take you for the reader type.",
        "Well, while hiding in Sorgan I had a lot of time to kill. And one needs to entertain herself in-between beating up storm troopers and curious mandalorians, you know.",
        "Sure, dreaming is free.",
    };

    private List<bool> Cara_0_1_bool = new List<bool>()
    {
		// Mando true, other false
		true,
        false,
        true,
        false,
        true,
        false,
        true,
    };

    private List<string> Cara_0_2 = new List<string>()
    {
        "Do you really think you can beat me?",
        "Why not? I already did once. The first time we met, we didn't have what you would call a very friendly introduction.",
        "Maybe your memory is a little bit rusty. As far as I can remember, that ended in a draw. And I was not even fighting seriously...",
        "That only ended in a draw because of the child. I wouldn't want to beat his daddy in front of him. Also, you were wearing a full suit of armour, that's cheating.",
        "You can make all the excuses you want. We can solve this right now, no blasters, what do you say?",
        "Are you going to take off your armour and fight fair and square?",
        "I can't remove my helmet.",
        "That's what I thought.",
    };

    private List<bool> Cara_0_2_bool = new List<bool>()
    {
		// Mando true, other false
		true,
        false,
        true,
        false,
        true,
        false,
        true,
        false,
    };

    private List<string> Cara_0_3 = new List<string>()
    {
        "Seriously, is it that bad to remove your helmet? I promise not to tell anyone.",
        "That's not how it works. The moment someone sees my face, I will stop being a member of the Tribe.",
        "But they don't need to know, it's not like they are watching every step you make right?",
        "I don't think so, but to be part of the Tribe is a choice, and an honour. I wouldn't have joined if I wasn't entirely sure that this is what I wanted. To always wear a helmet is a small price to pay.",
        "Do you realize this whole Tribe stuff sounds pretty obscure? It's not good to blindly follow rules without ever questioning them. That's why I left the Alliance.",
        "You are starting to sound like Bo-Katan… The Tribe is not like the New Republic, we look for ourselves and swear allegiance to no one. I only pick the contracts I like so I don't have to do anything I don't want to do.",
        "I too used to think like you.",
    };

    private List<bool> Cara_0_3_bool = new List<bool>()
    {
		// Mando true, other false
        false,
        true,
        false,
        true,
        false,
        true,
        false,
    };

    private List<string> Cara_1_1 = new List<string>()
    {
        "Wow, Bantha milk? Thanks! It's been a long time since I drank it. I used to like it a lot when I was younger. Then, in the military, we only got field rations and water. That basically killed my taste buds.",
        "Glad you like it. I’ll try and bring you more if I can. Was it tough there?",
        "At the beginning it was, when I was fighting in the Alliance to Restore the Republic. We had to fight against the full force of the empire's troops. After winning the war it became a completely different thing.",
        "I suppose it became much easier. Hunting the remnants of the empire doesn’t sound as dangerous.",
        "Well, they didn't use to just surrender without putting up a fight, but yes, your odds of coming out alive were much higher.",
        "I don't understand why you decided to resign at that point, the hard work had already been done. Were the new missions not exciting enough for you?",
        "They were exciting alright. But the New Republic Defence Force had more purposes than just hunting leftovers. We also had to escort important delegates and pacify riots. I had not signed for that. I felt like I wasn't fighting for a cause I believed in anymore.",
    };

    private List<bool> Cara_1_1_bool = new List<bool>()
    {
		// Mando true, other false
		false,
        true,
        false,
        true,
        false,
        true,
        false,
    };

    private List<string> Cara_1_2 = new List<string>()
    {
        "Now you are working for the New Republic again. Have you changed your mind on the nature of its cause?",
        "Hmm… It's not really the same thing. I'm just a Marshall now, it's true that I'm employed by the New Republic, but I don't have to do anything for them. I just make sure there's peace and order in Nevarro and report in case there's any trouble. I like it.",
        "Well, it's nice you've found something you like to do. Is Nevarro starting to feel like home?",
        "My home does not exist anymore, and nothing will ever be able to replace it… But Nevarro is… not too bad, I could get used to it. Life is fine and people are nice.",
        "I’ve lived most of my life in Nevarro, does this mean you think I’m nice?",
        "Hah… don’t push your luck Mandalorian.",
    };

    private List<bool> Cara_1_2_bool = new List<bool>()
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
				return Bo_Katan_1_1;
			case 5:
				return Bo_Katan_1_2;
			case 6:
				return Bo_Katan_1_3;
			case 7:
				return Bo_Katan_2_1;
			case 8:
				return Bo_Katan_2_2;
			case 9:
				return Bo_Katan_2_3;
			case 10:
				return Final_Cutscene;
			case 11:
				return Greef_0_1;
			case 12:
				return Greef_0_2;
			case 13:
				return Greef_0_3;
			case 14:
				return Ahsoka_0_1;
			case 15:
				return Ahsoka_0_2;
			case 16:
				return Ahsoka_0_3;
			case 17:
				return Grogu_0_1;
			case 18:
				return Grogu_0_1;
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
				return Bo_Katan_1_1_bool;
			case 5:
				return Bo_Katan_1_2_bool;
			case 6:
				return Bo_Katan_1_3_bool;
			case 7:
				return Bo_Katan_2_1_bool;
			case 8:
				return Bo_Katan_2_2_bool;
			case 9:
				return Bo_Katan_2_3_bool;
			case 10:
				return Final_Cutscene_bool;
			case 11:
				return Greef_0_1_bool;
			case 12:
				return Greef_0_2_bool;
			case 13:
				return Greef_0_3_bool;
			case 14:
				return Ahsoka_0_1_bool;
			case 15:
				return Ahsoka_0_2_bool;
			case 16:
				return Ahsoka_0_3_bool;
			case 17:
				return Grogu_0_1_bool;
			case 18:
				return Grogu_0_1_bool;
		}
		return Initial_Cutscene_bool;
	}

}
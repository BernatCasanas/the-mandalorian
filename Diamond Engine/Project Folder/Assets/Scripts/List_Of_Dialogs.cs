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
		"Sorry kid, I don't have your ball on me right now. It's in the ship. And before you ask me, the answer is no; I'm not going back there to get it for you.",
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
        "Tell yourself that if it makes you sleep any better. We did what we had to do to survive, and if it hadn't been me, someone else would have taken my place. It is just how it is. Under the rule of the Empire, this was the way. But well, as you can see, this has changed now.",
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
       "Ahsoka Tano... You are here too? That's unexpected. This canteen is starting to get crowded.",
       "It is nice to meet you again, Mandalorian. I am glad to see that you and Grogu are fine. Since we parted ways in Dathomir, you two had me worried.",
       "There's no need for that, rest assured that I'm perfectly capable of fending both myself and the kid.",
       "I know, and it was not my intention to offend you, I apologize if I did so, you are quite competent. However, going against a Moff is a dangerous endeavour, even for somebody like you.",
       "Well, we survived. I appreciate your concern, but it was unnecessary. What brings you here Jedi?",
       "I have been wanting to discuss with Bo-Katan the whole Mandalore affair. I do not know if I could speak of her as a friend, but we go back a long time... I do not want to lose her too. Furthermore, I wanted to check on Grogu.",
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
        "Am I supposed to understand what all this means? Now I'm even more confused.",
        "There is no such thing as simply good and bad people, we are far more complex. I was not sure we were the 'good guys' anymore. What I had once considered my home, became unfamiliar, and where before I had seen friendly faces, I could only see strangers. It became clear to me that I no longer belonged in The Order.",
        "I'm going to pretend that this was more clarifying than before... You make it sound as if Jedi were bad people. Why should Grogu become one then? It doesn't seem like a good idea anymore.",
        "No, do not get me wrong. The Jedi were definitely trying to do good, it is just... those were hard times, and they had to make a lot of difficult decisions. I do not think it has ever been tougher for the members of the Jedi council. I even considered rejoining after a while, but then order 66 was issued and... Well, it was too late...",
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
        "So, Grogu should become a Jedi. Are you sure this is the right way?",
        "I cannot tell you if it is the right way or not. That, only time can tell. But if he is to be trained in the force, he must have a true Jedi knight for a teacher. Someone who can help him embrace the light side of the force and get rid of all his negative emotions.",
        "Last time we met you mentioned the dark side is strong within him. Is this the reason why he can only be trained by a Jedi?",
        "Yes, he has to learn to let go of his fears. He is too attached to you, and this makes him vulnerable. I have witnessed such feelings consume one of the greatest Jedi knights... and this is something that must be avoided at all cost.",
        "I know I'm not one to talk, but to remove all attachments to other people doesn't sound right.",
        "I am not saying that he cannot have any friends, however the way of a Jedi is a lonely one. If he is willing to walk that path, it is his duty to accept certain responsibilities. It may look cruel or unfair, but he will eventually understand.",
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
        "Oh, I did not think you would actually take my petition seriously. Thank you anyway.",
        "I had one to spare... There is one thing that's been bugging me; how long does it take to train a Jedi? From what I understand, Grogu is around 50. I keep thinking of him as a kid, but in reality, could he be too old?",
        "He really is a remarkable being. I have only met one other like him in my life. Perhaps the greatest Jedi master who ever lived. He was more than 800 years old... You need not worry about these things, every species ages at its own rhythm. He may be 50, but he is still a youngling.",
        "More than 800 years?! By the time I die he'll probably still be too young to even get a speeder licence. I hope at least he'll have learned to talk...",
        "Do not underestimate Grogu. The fact that you cannot understand him does not mean he is unable to communicate with others. I can speak with him through the force for example.",
        "But the kid is so helpless. He'll have to rely on others for his protection for who knows how many years...",
        "I was 14 the first time I went into battle. And you would be surprised how capable force-sensitive kids can be. I used to think like you too, but one day a small group of younglings I was supposed to take care of proved me otherwise.",
    };

    private List<bool> Ahsoka_2_1_bool = new List<bool>()
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

    private List<string> Ahsoka_2_2 = new List<string>()
    {
        "Grogu will live more than 800 years... Do you think he'll remember any of us in a couple of centuries?",
        "I am not going to pretend to know how living for such a long time affects someone's mind. Nonetheless, what I can assure you is that Grogu is not just anyone. His kind are very special. The old master I told you about always had an answer for every question, and all the Jedi turned to him every time they needed counsel. He was the wisest one in the Jedi council and most of the other members had learned from him.",
        "That doesn't really answer my question. What would have been for him a few years when he was a kid compared to almost a millennium of experience studying the mysteries of the universe?",
        "Memories are powerful, the mind can decide to keep or to erase them based on their nature and the impact they had on you, but they always stay in your subconscious. The Jedi have a deeper connection between their mind and their subconscious, for we cannot allow repressed emotions to cloud our judgement and affect our actions. In the same way, we can access old memories that are buried deep in our subconscious.",
        "Does this mean he will remember us?",
        "To answer what is really troubling you... Yes, he will remember you.",
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
        "I wanted to ask you Mandalorian, why do you care so much for Grogu? I have encountered a fair amount of people like you in my life. Bounty hunters with good reputations who prefer to work alone and would stop at nothing to get their mission done, so that they can earn their desired credits at the end... None of them would have broken a deal to save the child.",
        "I suppose I'm not like the other bounty hunters you've met. I just... I felt it was the right thing to do.",
        "That might be what you tell yourself, but I sense there is something more. Something that makes you care for him and has created a strong bond between you two...",
        "I told you I don't like these Jedi tricks. Get out of my mind!",
        "My apologies, but this time I needed to be sure. Even if I do not accept to take him, I must know he is in good hands. I already told you that he is very special. ",
        "Alright, but don't do it again. I guess... Yes. That day, on my homeplanet. I was a child too... alone, under that hatch... I know. He reminds me of myself.",
        "Thank you for telling the truth.",
    };

    private List<bool> Ahsoka_2_3_bool = new List<bool>()
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

    private List<string> Ahsoka_3_1 = new List<string>()
    {
        "Another one to spare? Where do you get all this Bantha milk?",
        "You know... here and there. Doesn't matter. Have you spoken with Bo-Katan about the Mandalore business yet?",
        "I have indeed. I knew she would try to reclaim Mandalore once again, and I agree with her, the time has come. There are still vestiges of The Empire scattered around the galaxy, but they are the weakest they have ever been.",
        "What do you think is looming in there? I was always told that the planet was cursed, but nobody knew exactly what that meant.",
        "I am also clueless here. My guess is that either there is an old Empire stronghold, or it has become a den for fugitives, low-life mobsters, and criminals.",
        "I hope it's the later one, I'm more used to dealing with this type of scoundrels.",
        "Whatever it may be, I am certain that they are not going to just hand over the planet and leave. You should never underestimate the enemy. Bo-Katan has been planning this for a long time. This time, I hope she can return to stay once and for all.",
    };

    private List<bool> Ahsoka_3_1_bool = new List<bool>()
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

    private List<string> Ahsoka_3_2 = new List<string>()
    {
        "What went wrong the last time you two tried to claim Mandalore?",
        "Nothing went wrong. It was certainly difficult, and we had a lot of casualties, but we were ultimately successful. You can ask Bo-Katan for the details. The problem was that the Republic fell hours later, and since she had claimed Mandalore with their aid, the planet was targeted by The Empire. She could not keep her position for long.",
        "Well, you already did it once. I'm sure you can do it again.",
        "Last time I had an entire division of troopers from the Republic with me and Bo-Katan had a lot of Mandalorian warriors with her.",
        "I'm sure that we can do without the boys in white; their only purpose is to add bulk. One Mandalorian counts for twenty of them. We can find the numbers elsewhere.",
        "You do not know what you are talking about, you are too young. Those were not your usual storm troopers; those were clone troopers. They were created specifically to become soldiers in the Grand Army of the Republic. Fighting was in their blood. I have never met any army as brave and disciplined as them.",
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
        "I think you exaggerate a bit. If clones were so good, why did The Empire stop using them?",
        "There was no need for that amount of manpower anymore, they were expensive to produce, and could be programmed to do anything, such as betraying their commanding officers.",
        "That's what happened to the Jedi right?... They put too much trust in them.",
        "That is sadly true... However, I would not have fought with anyone else during the clone wars. When they turned against the Jedi... it was not their fault. They could not help it. Even if it was against their will.",
        "Seems like you were really fond of them.",
        "We went through so much together... They saved my life countless times. Some of them became dear friends. I was able to save one at least, my second in command. His name is Rex, the finest soldier I have ever known. He was beside me during our mission in Mandalore. He was beside me from the beginning until the very last moment...",
        "The way you talk about him, I'd have liked to meet this man. He would have been a great addition to our team now.",
        "Oh, you misunderstood me, he is still alive. That geezer, he last saw action during the battle of Endor, despite me advising him not to do it because of his age. I am sure if I told him, he would come without hesitation. I am not going to do it though. He has earned a well-deserved retirement. After a life full of war, I want him to spend his last years in peace..",
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
        true,
        false,
    };

    private List<string> Ahsoka_4_1 = new List<string>()
    {
        "You are spoiling me Mandalorian. I am sure other people would profit from this. Consider giving some to them.",
        "I'll give it to whom I want. I have quite a bit so don't worry. After the fall of the Republic, didn't you consider joining the Rebellion? They would have benefited from someone with your skills.",
        "Have you ever heard the word Fulcrum?",
        "Sounds familiar... Wasn't he supposed to be some kind of secret agent working for the Rebellion?",
        "Close, but it was not a he.",
        "It was you? So, you never stopped working for the Republic side.",
        "Several people became Fulcrum during the war, but I was the first one. I directly worked with senator Bail Organa to create what became the Alliance to Restore the Republic, and I was in charge of the Intelligence Service.",
        "Then, you could use your contacts there to get us some help from the New Republic. With their aid, reclaiming Mandalore is going to be no trouble.",
        "",
    };

    private List<bool> Ahsoka_4_1_bool = new List<bool>()
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
    };

    private List<string> Ahsoka_4_2 = new List<string>()
    {
        "It amazes me that you did so much for the Rebellion and still, your involvement has remained a secret. The Republic owes you part of their existence, but your name will be forgotten after you die.",
        "I did not do what I did to get a medal or expecting to become a hero at the end. I did what I felt was right, and to know I helped those who needed it the most, is the greatest reward I can get. I know this is not your way, bounty hunter, and yet I think that by now, you might be able to understand it.",
        "Yes, I think I can understand.",
        "I spent the first half of my life fighting for the Republic. During those years, I tasted victory and defeat, glory and condemnation, I was given respect and criticism... All those moments are lost in time, they are meaningless, for few now live who remember them. However, when I look in the eyes of the people who suffered under the Empire, who are living peacefully and with freedom today, and I see them sparkling with happiness and joy... I cannot ask for more.",
        "Maybe you aren't a Jedi, and it is too late for you to go back and become one. But from all the stories I've heard, if there's one thing I'm certain about, is that you embody all the great things a Jedi master is supposed to stand for. I'm sure that if they were alive now, they would be proud of you...",
        "Thank you Mandalorian. This means a lot to me...",
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
        "I have realized I owe you an apology Mandalorian. I read you wrong the first time we met.",
        "Well, uh... apology accepted. What do you mean?",
        "The first time, I decided to leave Grogu with you because I thought that, as he was your mission, you would protect him and keep him safe. But now that I know you better, there is no doubt in my mind that it was the right choice. He is more than your mission, and you are more than just a bounty hunter.",
        "I'll take that as a compliment, I guess. But hold on a second, are you saying that if you had considered me untrustworthy, you would have taken Grogu? You know I wouldn't have let you, right?",
        "There is no need to think about that, it did not happen. However, there is a chance I might have done it. But do not worry, I would not have harmed you. You treated Grogu well.",
        "You really think I wouldn't stand any chance against you... Your lightsabers can't cut through my beskar armour, and not to brag but I'm a good fighter. You wouldn't have it so easy...",
        "Fine. I remember our small fight the first time we met. If it makes you feel any better, I acknowledge you were surprisingly capable, and fairly resourceful. You reminded me of a certain bounty hunter, a blue skinned gunslinger who caused us a lot of trouble when I was still a Padaw--",
        "Alright, ok I get it... Thank you, that definitely made me feel better.",
    };

    private List<bool> Ahsoka_4_3_bool = new List<bool>()
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
    };

    private List<string> Ahsoka_5_1 = new List<string>()
    {
        "Please, let this be the last time you give me blue milk. I am grateful, but I cannot help but wonder whether others might benefit from it more than me. Now, instead, accept this as a gift from me.",
        "Thanks, and ok, I'll do as you ask. I've got to tell you, I know you've already refused many times, but now more than ever, I think you would make a great master. I wouldn't have any complaints if you decided to take the kid with you.",
        "And I must continue to refuse. I am glad that you feel that way about me. But the kid belongs with real Jedi, and I do not see myself as a master. In fact, I do not think I will ever be able to be a master.",
        "But why? I honestly think I'm right here. As a master--",
        "Please... Stop talking about being a master, this brings back painful memories.",
        "Sorry, I didn't know... But please try to see it from my point of view...",
        "I know you think it is right, but there are a lot of things you do not know about me. I am become who I am, as a result of how I was taught, and the person who taught me was the best master I could have ever asked for; still... he was not good enough...",
    };

    private List<bool> Ahsoka_5_1_bool = new List<bool>()
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

    private List<string> Ahsoka_5_2 = new List<string>()
    {
        "Can I ask you what happened with your master?",
        "I suppose I should be able to talk about him. What happened belongs to the past, and I need to move on. You see, when I was young, I was quite different from how I am now.",
        "This is true for everyone.",
        "What I mean is, I was not as calm and collected as I am today, and I was very inexperienced. I was eager to prove myself and I often acted hastily and carelessly, without thinking much or considering the consequences of my actions.",
        "Kids tend to be like this, it is not something to be ashamed of.",
        "But I was not just any kid, I was a Jedi Padawan. It was expected from us to be disciplined, focused and obedient to our masters. Any Jedi master would have deemed me a lost cause, any but Skyguy. He taught me a lot, and little by little, I changed into what I am today. Everything was going well, until one day everything started to change. I left the Order, and shortly after, he became someone else.",
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
        "What do you mean by that your master became someone else?",
        "Exactly that. I lost track of him, and for a few years I thought he had disappeared. Then, one day, I met him again. He was no longer my master, he was someone... or something else, something vile. Where once I had seen kindness, now there was just an overwhelming sense of hollowness and misery... He had taken me under his wing, had looked out and cared for me, and had taught me so much... and I, in the end... I was incapable of helping him...",
        "Now I understand why you said these were painful memories. I'm sorry I asked you about it. Most of us have a dark past, I guess overcoming it and being able to see the light at the end of the tunnel is what makes us be better persons.",
        "You know Mandalorian... I think there is a wise master hidden deep down inside you too.",
        "Oh please, I was being sincere for once... Don't make fun of me.",
        "I was being sincere too. I can rest easy knowing that Grogu is in really good hands, and this makes me happy.",
        "I appreciate that... So now, what's next?",
        "Well, I think now we have a planet to reclaim, don't you agree? It is time for a new episode; meet old friends, make some new ones, get betrayed by them, forgive... and tell the story to the next generation. And most important: may the force be with you.",
        "I couldn't agree more. And, whoever or whatever it is that we will find in Mandalore, they better be prepared, because we are coming for them.",
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
        true,
        false,
        true,
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
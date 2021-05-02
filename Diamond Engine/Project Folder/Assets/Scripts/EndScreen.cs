using System;
using DiamondEngine;

public class EndScreen : DiamondComponent
{
	public GameObject result = null;
	public GameObject banthaKills = null;
	public GameObject stormsKills = null;
	public GameObject leftImage = null;
	public GameObject leftMultiplier = null;
	public GameObject rightImage = null;
	public GameObject rightMultiplier = null;

	private bool firstFrame = true;

	public void Awake()
    {
		Counter.firstRun = false;
	}

	public void OnExecuteButton()
	{
		if (gameObject.Name == "Continue")
		{
			StaticVariablesInit.InitStaticVars();
			Time.ResumeGame();
			Audio.SetState("Game_State", "Run");
			if (MusicSourceLocate.instance != null)
			{
				Audio.SetSwitch(MusicSourceLocate.instance.gameObject, "Player_Action", "Exploring");
			}
			SceneManager.LoadScene(518261031);
		}
		else if (gameObject.Name == "Quit")
		{
			Time.ResumeGame();
			SceneManager.LoadScene(1726826608);
		}
	}
	public void Update()
	{
		if (firstFrame && gameObject.Name == "End Scene")
		{
			firstFrame = false;
			Counter.isFinalScene = false;
			DisplayResults();
		}
	}

	void DisplayResults()
	{
		int bo, wr, boPlace, wrPlace;
		Debug.Log("Intro display results");
		if (Counter.gameResult == Counter.GameResult.VICTORY)
			result.GetComponent<Text>().text = "            VICTORY!";
		else if (Counter.gameResult == Counter.GameResult.DEFEAT)
			result.GetComponent<Text>().text = "            DEFEAT!";
		else
			result.GetComponent<Text>().text = "            RETREAT!";

		if (Counter.GameCounters.ContainsKey(Counter.CounterTypes.ENEMY_BANTHA))
			banthaKills.GetComponent<Text>().text = Counter.GameCounters[Counter.CounterTypes.ENEMY_BANTHA].amount.ToString();
		if (Counter.GameCounters.ContainsKey(Counter.CounterTypes.ENEMY_STORMTROOPER))
			stormsKills.GetComponent<Text>().text = Counter.GameCounters[Counter.CounterTypes.ENEMY_STORMTROOPER].amount.ToString();

		/*
		This system will have to be eventually changed, because it's NOT feasible to have this for 40 boons. I (Ferran-Roger Basart i Bosch) have touched and made code that solves this in a very simple way.
		When this is redone, I suggest to contact me so that I can explain a quick method to have this done. If such decision is taken, I will probably be too retarded to remember how easy doing this
		actually is, so here's a short guide for future me, in case I'm contacted:
			- Get the values of all used boons using the GetResourceCount() from PlayerResources. The Type variable should be extracted from iterating the BoonDataHolder Dictionary
			- Store the results in a list, then have the prefab with the blanket textures (don't think we can load them directly every time we finish the game) replace them with the ones stablished in the
			  BoonDataHolder dictionary's libraryTextureID (this may bite us in the ass in terms of performance, but it's not like we should be calculating physics or anything at this point of the execution)...
			  The texture problem may require help from code, though (maybe change the libraryTextureID to a Rect, then have all textures into an atlas?)
			- Have a variable that iterates the position of the boon images in the menu based on how many we have, and how many we want per row

			- After everything works, it would be desirable to delete boon registration from GameCounters (that is a generic place where it shouldn't be stored; we have a specific class for that)
		*/

		bo = Counter.GameCounters.ContainsKey(Counter.CounterTypes.BOKATAN_RES) ? 1 : 0;
		wr = Counter.GameCounters.ContainsKey(Counter.CounterTypes.WRECKER_RES) ? 1 : 0;
		boPlace = Counter.GameCounters.ContainsKey(Counter.CounterTypes.BOKATAN_RES) ? Counter.GameCounters[Counter.CounterTypes.BOKATAN_RES].place : 0;
		wrPlace = Counter.GameCounters.ContainsKey(Counter.CounterTypes.WRECKER_RES) ? Counter.GameCounters[Counter.CounterTypes.WRECKER_RES].place : 0;

		if (bo * boPlace < wr * wrPlace)
		{
			if (Counter.GameCounters.ContainsKey(Counter.CounterTypes.BOKATAN_RES))
				leftMultiplier.GetComponent<Text>().text = Counter.GameCounters[Counter.CounterTypes.BOKATAN_RES].amount.ToString();
			if (Counter.GameCounters.ContainsKey(Counter.CounterTypes.WRECKER_RES))
				rightMultiplier.GetComponent<Text>().text = Counter.GameCounters[Counter.CounterTypes.WRECKER_RES].amount.ToString();
		}
		else
		{
			leftImage.GetComponent<Image2D>().SwapTwoImages(rightImage);

			if (Counter.GameCounters.ContainsKey(Counter.CounterTypes.BOKATAN_RES))
				rightMultiplier.GetComponent<Text>().text = Counter.GameCounters[Counter.CounterTypes.BOKATAN_RES].amount.ToString();
			if (Counter.GameCounters.ContainsKey(Counter.CounterTypes.WRECKER_RES))
				leftMultiplier.GetComponent<Text>().text = Counter.GameCounters[Counter.CounterTypes.WRECKER_RES].amount.ToString();
		}
		Debug.Log("End display results");
	}
}
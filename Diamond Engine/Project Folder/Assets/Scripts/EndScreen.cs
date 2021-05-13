using System;
using DiamondEngine;

public class EndScreen : DiamondComponent
{
	public GameObject result = null;
	public GameObject banthaPanel = null;
	public GameObject banthaKills = null;
	public GameObject stormsPanel = null;
	public GameObject stormsKills = null;
	public GameObject skytrooperPanel = null;
	public GameObject skytrooperKills = null;
	public GameObject laserturretPanel = null;
	public GameObject laserturretKills = null;
	public GameObject heavytrooperPanel = null;
	public GameObject heavytrooperKills = null;
	public GameObject deathtrooperPanel = null;
	public GameObject deathtrooperKills = null;
	public GameObject rancorPanel = null;
	public GameObject wampaPanel = null;
	public GameObject skelPanel = null;
	public GameObject wampaandskelPanel = null;
	public GameObject moffPanel = null;
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
			if (Core.instance != null)
				Core.instance.SaveBuffs();
			SceneManager.LoadScene(518261031);

		}
		else if (gameObject.Name == "Quit")
		{
			Time.ResumeGame();
			if (Core.instance != null)
				Core.instance.SaveBuffs();
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

		//Update Bantha PANEL
		if (Counter.GameCounters.ContainsKey(Counter.CounterTypes.ENEMY_BANTHA))
		{
			banthaKills.GetComponent<Text>().text = Counter.GameCounters[Counter.CounterTypes.ENEMY_BANTHA].amount.ToString();
		}
		else if (!Counter.GameCounters.ContainsKey(Counter.CounterTypes.ENEMY_BANTHA))
        {
			banthaPanel.Enable(false);
		}

		//Update Stormtrooper PANEL
		if (Counter.GameCounters.ContainsKey(Counter.CounterTypes.ENEMY_STORMTROOPER))
		{
			stormsKills.GetComponent<Text>().text = Counter.GameCounters[Counter.CounterTypes.ENEMY_STORMTROOPER].amount.ToString();
		}
		else if (!Counter.GameCounters.ContainsKey(Counter.CounterTypes.ENEMY_STORMTROOPER))
        {
			stormsPanel.Enable(false);
		}

		//Update Skytrooper PANEL
		if (Counter.GameCounters.ContainsKey(Counter.CounterTypes.ENEMY_SKYTROOPER))
		{
			skytrooperKills.GetComponent<Text>().text = Counter.GameCounters[Counter.CounterTypes.ENEMY_SKYTROOPER].amount.ToString();
		}
		else if (!Counter.GameCounters.ContainsKey(Counter.CounterTypes.ENEMY_SKYTROOPER))
        {
			skytrooperPanel.Enable(false);
		}

        //Update Laser Turret PANEL
        if (Counter.GameCounters.ContainsKey(Counter.CounterTypes.ENEMY_LASER_TURRET))
        {
			laserturretKills.GetComponent<Text>().text = Counter.GameCounters[Counter.CounterTypes.ENEMY_LASER_TURRET].amount.ToString();
		}
		else if (!Counter.GameCounters.ContainsKey(Counter.CounterTypes.ENEMY_LASER_TURRET))
        {
			laserturretPanel.Enable(false);
		}

		//Update Heavytrooper PANEL
		if (Counter.GameCounters.ContainsKey(Counter.CounterTypes.ENEMY_HEAVYTROOPER))
        {
			heavytrooperKills.GetComponent<Text>().text = Counter.GameCounters[Counter.CounterTypes.ENEMY_HEAVYTROOPER].amount.ToString();
		}
		else if (!Counter.GameCounters.ContainsKey(Counter.CounterTypes.ENEMY_HEAVYTROOPER))
        {
			heavytrooperPanel.Enable(false);
		}

        //Update Laser Turret PANEL
        if (Counter.GameCounters.ContainsKey(Counter.CounterTypes.ENEMY_DEATHTROOPER))
        {
			deathtrooperKills.GetComponent<Text>().text = Counter.GameCounters[Counter.CounterTypes.ENEMY_DEATHTROOPER].amount.ToString();
		}
		else if (!Counter.GameCounters.ContainsKey(Counter.CounterTypes.ENEMY_DEATHTROOPER))
        {
			deathtrooperPanel.Enable(false);
		}

		//Update Rancor PANEL
		if (Counter.GameCounters.ContainsKey(Counter.CounterTypes.RANCOR))
		{
			rancorPanel.Enable(true);
		}
		else if (!Counter.GameCounters.ContainsKey(Counter.CounterTypes.RANCOR))
		{
			rancorPanel.Enable(false);
		}

		//Update Wampa and Skel PANEL
		if (Counter.GameCounters.ContainsKey(Counter.CounterTypes.WAMPA) && Counter.GameCounters.ContainsKey(Counter.CounterTypes.SKEL))
		{
			wampaandskelPanel.Enable(true);
			skelPanel.Enable(false);
			wampaPanel.Enable(false);
		}
		else if (Counter.GameCounters.ContainsKey(Counter.CounterTypes.WAMPA) && !Counter.GameCounters.ContainsKey(Counter.CounterTypes.SKEL))
		{
			wampaPanel.Enable(true);
			skelPanel.Enable(false);
			wampaandskelPanel.Enable(false);
		}
		else if (!Counter.GameCounters.ContainsKey(Counter.CounterTypes.WAMPA) && Counter.GameCounters.ContainsKey(Counter.CounterTypes.SKEL))
		{
			skelPanel.Enable(true);
			wampaPanel.Enable(false);
			wampaandskelPanel.Enable(false);
		}
		else if (!Counter.GameCounters.ContainsKey(Counter.CounterTypes.WAMPA) && !Counter.GameCounters.ContainsKey(Counter.CounterTypes.SKEL))
        {
			skelPanel.Enable(false);
			wampaPanel.Enable(false);
			wampaandskelPanel.Enable(false);
		}

		//Update Moff Gideon PANEL
		if (Counter.GameCounters.ContainsKey(Counter.CounterTypes.MOFFGIDEON))
		{
			moffPanel.Enable(true);
		}
		else if (!Counter.GameCounters.ContainsKey(Counter.CounterTypes.MOFFGIDEON))
		{
			moffPanel.Enable(false);
		}

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
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
		if (Counter.GameCounters.ContainsKey(Counter.CounterTypes.ENEMY_STORMTROOP))
			stormsKills.GetComponent<Text>().text = Counter.GameCounters[Counter.CounterTypes.ENEMY_STORMTROOP].amount.ToString();

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
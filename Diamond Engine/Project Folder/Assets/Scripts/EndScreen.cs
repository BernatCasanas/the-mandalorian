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
			Counter.ResetCounters();
			Time.ResumeGame();
			SceneManager.LoadScene(1406013733);
		}
		else if (gameObject.Name == "Quit")
		{
			Counter.ResetCounters();
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

		if (Counter.GameCounters[Counter.CounterTypes.BOKATAN_RES].place < Counter.GameCounters[Counter.CounterTypes.WRECKER_RES].place)
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
using System;
using DiamondEngine;

public class Pause : DiamondComponent
{
	public GameObject pauseWindow = null;
	public GameObject settingsWindow = null;
	public GameObject displayWindow = null;
	public GameObject controlsWindow = null;
	public GameObject quitWindow = null;
	public GameObject background = null;
	public GameObject default_selected = null;

	public GameObject leftImage = null;
	public GameObject rightImage = null;
	public GameObject leftText = null;
	public GameObject rightText = null;
	public GameObject leftX = null;
	public GameObject rightX = null;

	public void OnExecuteButton()
	{
		if (gameObject.Name == "Continue")
		{
			Time.ResumeGame();
			background.Enable(false);
			pauseWindow.Enable(false);
		}
		if (gameObject.Name == "Settings")
		{
			settingsWindow.Enable(true);
			pauseWindow.Enable(false);
			if (default_selected != null)
			{
				default_selected.GetComponent<Navigation>().Select();
				default_selected.GetComponent<Settings>().FirstFrame();
			}
		}
		if (gameObject.Name == "Display")
		{
			displayWindow.Enable(true);
			pauseWindow.Enable(false);
			if (default_selected != null)
				default_selected.GetComponent<Navigation>().Select();
		}
		if (gameObject.Name == "Controls")
		{
			controlsWindow.Enable(true);
			pauseWindow.Enable(false);
			if (default_selected != null)
				default_selected.GetComponent<Navigation>().Select();
		}
		if (gameObject.Name == "Quit")
		{
			quitWindow.Enable(true);
			pauseWindow.Enable(false);
			if (default_selected != null)
				default_selected.GetComponent<Navigation>().Select();
		}
	}

	public void Update()
	{
		
	}

	public void DisplayBoons()
    {
		if (gameObject.Name == "PauseMenu")
		{
			int bo, wr, boPlace, wrPlace;
			leftImage.Enable(true);
			leftText.Enable(true);
			leftX.Enable(true);
			rightImage.Enable(true);
			rightText.Enable(true);
			rightX.Enable(true);

			bo = Counter.GameCounters.ContainsKey(Counter.CounterTypes.BOKATAN_RES) ? 1 : 0;
			wr = Counter.GameCounters.ContainsKey(Counter.CounterTypes.WRECKER_RES) ? 1 : 0;
			boPlace = Counter.GameCounters.ContainsKey(Counter.CounterTypes.BOKATAN_RES) ? Counter.GameCounters[Counter.CounterTypes.BOKATAN_RES].place : 0;
			wrPlace = Counter.GameCounters.ContainsKey(Counter.CounterTypes.WRECKER_RES) ? Counter.GameCounters[Counter.CounterTypes.WRECKER_RES].place : 0;

			if (Counter.GameCounters.ContainsKey(Counter.CounterTypes.BOKATAN_RES) || Counter.GameCounters.ContainsKey(Counter.CounterTypes.WRECKER_RES))
			{
				if (bo * boPlace < wr * wrPlace)
				{

					if (Counter.GameCounters.ContainsKey(Counter.CounterTypes.BOKATAN_RES))
					{
						rightText.GetComponent<Text>().text = Counter.GameCounters[Counter.CounterTypes.BOKATAN_RES].amount.ToString();
						Debug.Log(Counter.GameCounters[Counter.CounterTypes.BOKATAN_RES].amount.ToString());
					}
					else
					{
						leftImage.Enable(false);
						leftText.Enable(false);
						leftX.Enable(false);
					}

					if (Counter.GameCounters.ContainsKey(Counter.CounterTypes.WRECKER_RES))
					{
						leftText.GetComponent<Text>().text = Counter.GameCounters[Counter.CounterTypes.WRECKER_RES].amount.ToString();
						Debug.Log(Counter.GameCounters[Counter.CounterTypes.WRECKER_RES].amount.ToString());
					}
					else
					{
						rightImage.Enable(false);
						rightText.Enable(false);
						rightX.Enable(false);
					}
				}

				else
				{
					rightImage.GetComponent<Image2D>().SwapTwoImages(leftImage);
					if (Counter.GameCounters.ContainsKey(Counter.CounterTypes.BOKATAN_RES))
					{
						leftText.GetComponent<Text>().text = Counter.GameCounters[Counter.CounterTypes.BOKATAN_RES].amount.ToString();
						Debug.Log(Counter.GameCounters[Counter.CounterTypes.BOKATAN_RES].amount.ToString());
					}
					else
					{
						leftImage.Enable(false);
						leftText.Enable(false);
						leftX.Enable(false);
					}

					if (Counter.GameCounters.ContainsKey(Counter.CounterTypes.WRECKER_RES))
					{
						rightText.GetComponent<Text>().text = Counter.GameCounters[Counter.CounterTypes.WRECKER_RES].amount.ToString();
						Debug.Log(Counter.GameCounters[Counter.CounterTypes.WRECKER_RES].amount.ToString());
					}
					else
					{
						rightImage.Enable(false);
						rightText.Enable(false);
						rightX.Enable(false);
					}
				}
			}
			else
			{
				leftImage.Enable(false);
				leftText.Enable(false);
				leftX.Enable(false);
				rightImage.Enable(false);
				rightText.Enable(false);
				rightX.Enable(false);
			}
		}
	}
}

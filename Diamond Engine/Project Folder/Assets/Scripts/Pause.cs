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

	public GameObject shop = null;

	public void OnExecuteButton()
	{
		if (gameObject.Name == "Continue")
		{
			Time.ResumeGame();
			background.EnableNav(false);
			pauseWindow.EnableNav(false);
		}
		if (gameObject.Name == "Settings")
        {
			settingsWindow.EnableNav(true);
			pauseWindow.EnableNav(false);
			if (default_selected != null)
			{
				default_selected.GetComponent<Navigation>().Select();
				default_selected.GetComponent<Settings>().FirstFrame();
			}
		}
		if (gameObject.Name == "Display")
		{
			displayWindow.EnableNav(true);
			pauseWindow.EnableNav(false);
			if (default_selected != null)
				default_selected.GetComponent<Navigation>().Select();
		}
		if (gameObject.Name == "Controls")
		{
			controlsWindow.EnableNav(true);
			pauseWindow.EnableNav(false);
			if (default_selected != null)
				default_selected.GetComponent<Navigation>().Select();
		}
		if (gameObject.Name == "Quit")
		{
			quitWindow.EnableNav(true);
			pauseWindow.EnableNav(false);
			if (default_selected != null)
				default_selected.GetComponent<Navigation>().Select();
		}
	}

	public void Awake()
	{

	}

	public void Update()
	{
		
	}

	public void DisplayBoons()
    {
		if (gameObject.Name == "PauseMenu")
		{
			if (default_selected != null)
            {
				Navigation componentNavigation = default_selected.GetComponent<Navigation>();

				if (componentNavigation != null)
					componentNavigation.Select();
            }

			leftImage.Enable(true);
			leftText.Enable(true);
			leftX.Enable(true);
			rightImage.Enable(true);
			rightText.Enable(true);
			rightX.Enable(true);

			//All this must be redone

			//if (Counter.GameCounters.ContainsKey(Counter.CounterTypes.BOKATAN_RES) || Counter.GameCounters.ContainsKey(Counter.CounterTypes.WRECKER_RES))
			//{
			//	if (bo * boPlace < wr * wrPlace)
			//	{

			//		if (Counter.GameCounters.ContainsKey(Counter.CounterTypes.BOKATAN_RES))
			//		{
			//			rightText.GetComponent<Text>().text = Counter.GameCounters[Counter.CounterTypes.BOKATAN_RES].amount.ToString();
			//			Debug.Log(Counter.GameCounters[Counter.CounterTypes.BOKATAN_RES].amount.ToString());
			//		}
			//		else
			//		{
			//			rightImage.Enable(false);
			//			rightText.Enable(false);
			//			rightX.Enable(false);
			//		}

			//		if (Counter.GameCounters.ContainsKey(Counter.CounterTypes.WRECKER_RES))
			//		{
			//			leftText.GetComponent<Text>().text = Counter.GameCounters[Counter.CounterTypes.WRECKER_RES].amount.ToString();
			//			Debug.Log(Counter.GameCounters[Counter.CounterTypes.WRECKER_RES].amount.ToString());
			//		}
			//		else
			//		{
			//			leftImage.Enable(false);
			//			leftText.Enable(false);
			//			leftX.Enable(false);
			//		}
			//	}

			//	else
			//	{
			//		rightImage.GetComponent<Image2D>().SwapTwoImages(leftImage);
			//		if (Counter.GameCounters.ContainsKey(Counter.CounterTypes.BOKATAN_RES))
			//		{
			//			leftText.GetComponent<Text>().text = Counter.GameCounters[Counter.CounterTypes.BOKATAN_RES].amount.ToString();
			//			Debug.Log(Counter.GameCounters[Counter.CounterTypes.BOKATAN_RES].amount.ToString());
			//		}
			//		else
			//		{
			//			leftImage.Enable(false);
			//			leftText.Enable(false);
			//			leftX.Enable(false);
			//		}

			//		if (Counter.GameCounters.ContainsKey(Counter.CounterTypes.WRECKER_RES))
			//		{
			//			rightText.GetComponent<Text>().text = Counter.GameCounters[Counter.CounterTypes.WRECKER_RES].amount.ToString();
			//			Debug.Log(Counter.GameCounters[Counter.CounterTypes.WRECKER_RES].amount.ToString());
			//		}
			//		else
			//		{
			//			rightImage.Enable(false);
			//			rightText.Enable(false);
			//			rightX.Enable(false);
			//		}
			//	}
			//}
			//else
			//{
			//	leftImage.Enable(false);
			//	leftText.Enable(false);
			//	leftX.Enable(false);
			//	rightImage.Enable(false);
			//	rightText.Enable(false);
			//	rightX.Enable(false);
			//}
		}
	}

	public void HideShop()
    {
		if (shop != null && shop.IsEnabled())
		{
			shop.parent.GetComponent<SHOP>().CloseShop();
		}
    }
}

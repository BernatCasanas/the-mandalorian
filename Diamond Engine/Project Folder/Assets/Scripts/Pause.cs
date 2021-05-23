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
	public GameObject boonDisplayObject = null;
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

			if (boonDisplayObject != null)
			{
				BoonDisplay boonDisplay = boonDisplayObject.GetComponent<BoonDisplay>();

				if (boonDisplay != null)
				{
					boonDisplay.ShowBoons();
				}
				else
				{
					Debug.Log("Null boon display");
				}
			}
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

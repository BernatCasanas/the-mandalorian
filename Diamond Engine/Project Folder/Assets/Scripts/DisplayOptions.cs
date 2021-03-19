using System;
using DiamondEngine;

public class DisplayOptions : DiamondComponent
{
	public GameObject optionsWindow = null;
	public GameObject settings = null;
	public GameObject display = null;
	public GameObject controls = null;
	public GameObject bigBrother = null;
	public GameObject default_selected = null;

	public void onExecuteCheckbox()
    {
		Debug.Log("Vsync");
    }
	public void OnExecuteButton()
	{
		if (gameObject.Name == "Settings")
        {
			settings.Enable(true);
			optionsWindow.Enable(false);
			if (default_selected != null)
				default_selected.GetComponent<Navigation>().Select();
		}
		if (gameObject.Name == "Display")
		{
			display.Enable(true);
			optionsWindow.Enable(false);
			if (default_selected != null)
				default_selected.GetComponent<Navigation>().Select();
		}
		if (gameObject.Name == "Controls")
		{
			controls.Enable(true);
			optionsWindow.Enable(false);
			if (default_selected != null)
				default_selected.GetComponent<Navigation>().Select();
		}
		if (gameObject.Name == "Return")
		{
			bigBrother.Enable(true);
			optionsWindow.Enable(false);
		}
	}
	public void Update()
	{

	}

}
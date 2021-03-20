using System;
using DiamondEngine;

public class Confirmation : DiamondComponent
{
	public GameObject confirmationWindow = null;
	public GameObject mainMenu = null;
	public void OnExecuteButton()
	{
		if (gameObject.Name == "Yes")
			InternalCalls.CloseGame();
		if (gameObject.Name == "No")
        {
			mainMenu.Enable(true);
			confirmationWindow.Enable(false);
		}
	}
	public void Update()
	{

	}

}
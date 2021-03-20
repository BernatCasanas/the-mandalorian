using System;
using DiamondEngine;

public class QuitConfirmation : DiamondComponent
{
	public GameObject confirmScreen = null;
	public GameObject bigBrother = null;
	public void OnExecuteButton()
	{
		if (gameObject.Name == "Cancel")
		{
			bigBrother.Enable(true);
			bigBrother.Enable(true);
			if (bigBrother.Name == "PauseMenu")
			{
				bigBrother.GetComponent<Pause>().DisplayBoons();
			}
			confirmScreen.Enable(false);
		}
		if (gameObject.Name == "QuittoDesktop")
			InternalCalls.CloseGame();
		if (gameObject.Name == "QuittoMenu")
			SceneManager.LoadScene(1726826608);
	}
	public void Update()
	{

	}

}

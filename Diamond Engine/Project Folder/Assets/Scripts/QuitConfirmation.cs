using System;
using DiamondEngine;

public class QuitConfirmation : DiamondComponent
{
	public GameObject confirmScreen = null;
	public GameObject bigBrother = null;
	//private Pause aux = null;
	public void OnExecuteButton()
	{
		if (gameObject.Name == "Cancel")
		{
			bigBrother.EnableNav(true);
			confirmScreen.EnableNav(false);
		}
		if (gameObject.Name == "QuittoDesktop")
			InternalCalls.CloseGame();
		if (gameObject.Name == "QuittoMenu")
		{
			Time.ResumeGame();
			Audio.SetState("Game_State", "HUB");

			RoomSwitch.OnPlayerQuit();
		}
	}
	public void Update()
	{

	}

}

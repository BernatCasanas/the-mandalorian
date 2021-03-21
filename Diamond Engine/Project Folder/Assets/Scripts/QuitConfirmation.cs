using System;
using DiamondEngine;

public class QuitConfirmation : DiamondComponent
{
	public GameObject confirmScreen = null;
	public GameObject bigBrother = null;
	private Pause aux = null;
	public void OnExecuteButton()
	{
		if (gameObject.Name == "Cancel")
		{
			bigBrother.Enable(true);
			if (bigBrother.Name == "PauseMenu")
			{
				aux = bigBrother.GetComponent<Pause>();
				aux.DisplayBoons();
			}
			confirmScreen.Enable(false);
		}
		if (gameObject.Name == "QuittoDesktop")
			InternalCalls.CloseGame();
		if (gameObject.Name == "QuittoMenu")
		{
			Time.ResumeGame();
			Audio.SetState("Game_State", "HUB");
			Counter.gameResult = Counter.GameResult.NONE;
			SceneManager.LoadScene(821370213);
		}
	}
	public void Update()
	{

	}

}

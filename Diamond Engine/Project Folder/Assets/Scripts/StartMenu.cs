using System;
using DiamondEngine;

public class StartMenu : DiamondComponent
{
	public GameObject menuButtons = null;
	public GameObject options = null;
	public GameObject background = null;
	public GameObject default_selected = null;

	private bool musicplayed = false;

	public void OnExecuteButton()
	{
		if (gameObject.Name == "Play")
		{
			SceneManager.LoadScene(1406013733);
			//Audio.SetState("Player_State", "Alive");
			Audio.SetState("Game_State", "Run");
			Audio.SetSwitch(gameObject, "Player_Action", "Exploring");
			Debug.Log("Exploring");
		}
		else if (gameObject.Name == "Options")
		{
			menuButtons.Enable(false);
			options.Enable(true);
			background.Enable(true);
			if (default_selected != null)
				default_selected.GetComponent<Navigation>().Select();
		}
		else if (gameObject.Name == "Quit")
		{
			options.Enable(true);
			menuButtons.Enable(false);
		}
	}
	public void Update()
	{
		if (!musicplayed)
        {
			Audio.SetState("Player_State", "Alive");
			Audio.SetState("Game_State", "HUB");
			musicplayed = true;
		}
	}

}
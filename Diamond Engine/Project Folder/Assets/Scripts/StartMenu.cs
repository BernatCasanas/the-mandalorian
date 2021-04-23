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
			SceneManager.LoadScene(1564453141);
			
			if (MusicSourceLocate.instance != null)
			{
				Audio.SetSwitch(MusicSourceLocate.instance.gameObject, "Player_Action", "Exploring");
				Debug.Log("Exploring");
			}
			Audio.SetState("Game_State", "HUB");
			//Audio.PlayAudio(gameObject, "Play_Cantine_Voices");
		}
		else if (gameObject.Name == "Options")
		{
			menuButtons.EnableNav(false);
			options.EnableNav(true);
			background.EnableNav(true);
			if (default_selected != null)
				default_selected.GetComponent<Navigation>().Select();
		}
		else if (gameObject.Name == "Quit")
		{
			options.EnableNav(true);
			menuButtons.EnableNav(false);
		}
	}
	public void Update()
	{
		if (!musicplayed)
        {
			Audio.SetState("Player_State", "Alive");
			Audio.SetState("Game_State", "Menu");
			musicplayed = true;
		}
	}

}
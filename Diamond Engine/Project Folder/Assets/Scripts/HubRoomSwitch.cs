using System;
using System.Collections.Generic;
using DiamondEngine;

public class HubRoomSwitch : DiamondComponent
{
	public int nextRoomUID = -1;
	public bool isHubScene = false;

	public void OnExecuteButton()
	{
		if(isHubScene == true)
			RoomSwitch.ClearStaticData();

		//PlayerResources.ResetRunBoons(); //TODO: This has a bug when starting the run at the HUB
		if (Core.instance != null)
			Core.instance.SaveBuffs();
		if (nextRoomUID != -1)
			SceneManager.LoadScene(nextRoomUID);


		if (MusicSourceLocate.instance != null)
        {
			Audio.SetState("Player_State", "Alive");
			Audio.SetState("Game_State", "Run");
			Audio.SetSwitch(MusicSourceLocate.instance.gameObject, "Player_Action", "Exploring");
			Debug.Log("Exploring");
        }

		RoomSwitch.PlayLevelEnvironment();
		
	}
}
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

		SceneManager.LoadScene(nextRoomUID);
	}
	public void Update()
	{


	}

}
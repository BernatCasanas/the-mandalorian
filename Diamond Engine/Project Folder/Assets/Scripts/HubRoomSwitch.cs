using System;
using System.Collections.Generic;
using DiamondEngine;

public class HubRoomSwitch : DiamondComponent
{
	public int nextRoomUID = -1;

	public void OnExecuteButton()
	{
		if(RoomSwitch.currentLevelIndicator == RoomSwitch.LEVELS.ONE)
			RoomSwitch.ClearStaticData();

		SceneManager.LoadScene(nextRoomUID);
	}
	public void Update()
	{


	}

}
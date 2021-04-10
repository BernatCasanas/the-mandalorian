using System;
using System.Collections.Generic;
using DiamondEngine;

public class HubRoomSwitch : DiamondComponent
{
	public int nextRoomUID = -1;

	public void OnExecuteButton()
	{
		SceneManager.LoadScene(nextRoomUID);
	}
	public void Update()
	{


	}

}
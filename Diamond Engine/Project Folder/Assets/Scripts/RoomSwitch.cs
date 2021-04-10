using System;
using System.Collections.Generic;
using DiamondEngine;

public class RoomSwitch : DiamondComponent
{
	private static int roomID = 0;

	static int currentroom = 0;

	//public static int index = 0;
	static Random test = new Random();
	static List<int> visited = new List<int>()
	{
		1940308114,
		769097826,
		1796967585,
	};
	private static int finalScene = 308281119;

	public static void SwitchRooms()
	{
		if (visited.Count > 0)
		{
			int roomnumber = test.Next(0, visited.Count);
			roomID = visited[roomnumber];
			currentroom = roomID;
			visited.Remove(currentroom);
			SceneManager.LoadScene(roomID);

		}
		else
		{
			visited = new List<int>()
				{
					1940308114,
					769097826,
					1796967585,

				};
			currentroom = 0;
			//IMPORTANT: isFinalScene is needed to know when to change from a room to the end scene to show statistics
			Counter.isFinalScene = true;
			SceneManager.LoadScene(finalScene);
		}
	}
}
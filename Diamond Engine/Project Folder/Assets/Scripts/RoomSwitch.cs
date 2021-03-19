using System;
using System.Collections.Generic;
using DiamondEngine;

public class RoomSwitch : DiamondComponent
{
	private int roomID = 0;

	static int currentroom = 1076838722;

	//public static int index = 0;
	static Random test = new Random();
	private bool start = true;
	static List<int> visited = new List<int>() 
	{
							1076838722,
						   1934547592,
						   1406013733,
						   
	};
	private int finalScene = 1482507639;

	public void OnTriggerEnter(GameObject triggeringObject)
    {
		if (!triggeringObject.CompareTag("Player"))
			return;

		if (visited.Count > 0)
		{
			int roomnumber = test.Next(0, visited.Count);
			roomID = visited[roomnumber];
			currentroom = roomID;

			SceneManager.LoadScene(roomID);

		}
		else
		{
			Debug.Log("No more room in hell");
			visited = new List<int>()
				{
							1076838722,
						   1934547592,
						   1406013733,

				};
			currentroom = 1076838722;
			SceneManager.LoadScene(finalScene);
		}
	}

    public void Update()
	{
		if (start)
        {
			start = false;
			visited.Remove(currentroom);

		}

		if (Input.GetKey(DEKeyCode.I) == KeyState.KEY_DOWN)
		{
				if (visited.Count > 0)
				{
					int roomnumber = test.Next(0, visited.Count);
					roomID = visited[roomnumber];
					currentroom = roomID;

					SceneManager.LoadScene(roomID);

				}
				else
				{
					Debug.Log("No more room in hell");
					visited = new List<int>()
				{
							1076838722,
						   1934547592,
						   1406013733,

				};
					currentroom = 1076838722;
					SceneManager.LoadScene(finalScene);
				}

		}

		//if (searchroom)
  //      {
			
			
		//		roomID = rooms[test.Next(0, rooms.Length)];
		//		if (roomID != currentroom)
		//		{
		//			currentroom = roomID;
		//			visited.Add(roomID);
		//			SceneManager.LoadScene(roomID);
		//		}
			
		//}
	}

}

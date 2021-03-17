using System;
using DiamondEngine;

public class RoomSwitch : DiamondComponent
{
	private int roomID = 0;
	static int currentroom = 1076838722;
	static int[] rooms = { 1076838722,
						   1934547592,
						   1406013733,
						   1482507639};
	private bool searchroom = false;
	//public static int index = 0;
	static Random test = new Random();
	private bool start = true;

    public void OnCollisionEnter()
    {
	//	searchroom = true;

	}

    public void Update()
	{
		if (start)
        {
			start = false;
			Debug.Log("CURRENT ROOM = " + currentroom.ToString());

		}

		if (Input.GetKey(DEKeyCode.I) == KeyState.KEY_DOWN)
		{
			roomID = rooms[test.Next(0, rooms.Length)];
			if (roomID != currentroom)
			{
				currentroom = roomID;
				SceneManager.LoadScene(currentroom);
			}
		}

		if (searchroom)
        {
			roomID = rooms[test.Next(0, rooms.Length)];
			if (roomID != currentroom)
			{
				currentroom = roomID;
				SceneManager.LoadScene(rooms[currentroom]);
			}
		}
	}

}

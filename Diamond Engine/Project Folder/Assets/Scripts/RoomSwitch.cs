using System;
using System.Collections.Generic;
using DiamondEngine;

public class RoomDataHolder
{
	public List<int> visited = new List<int>(); //I think this is a fucking pointer-type shit? i don't know, i don't think this is working, im out
	public int finalScene = 0;

    public RoomDataHolder(List<int> initList, int final)
	{
		//visited.Clear();
		visited = new List<int>(initList);
		finalScene = final;
    }

	public void ClearData(List<int> initList)
    {
		//visited.Clear();
		visited = new List<int>(initList);
	}

	//public bool isFinalScene = false;
}

public static class RoomSwitch
{

    public enum LEVELS
    {
		ONE,
		TWO,
		MAX,
    }

	public readonly static List<List<int>> originalLevelPools = new List<List<int>>()
	{
		new List<int>()
		{
			1940308114,
			769097826,
			1796967585,
		},

		new List<int>()
		{
			614298780,
			2043151660,
			1984495527,
			1172505392,
			352974858,
		},

	};

	public readonly static List<RoomDataHolder> levelLists = new List<RoomDataHolder>() 
	{
		new RoomDataHolder(originalLevelPools[0], 308281119),
		new RoomDataHolder(originalLevelPools[1], 880545183),
	};

	private readonly static int postBossRoom = 466646284;

	private static int roomID = 0;
	public static int currentroom = 0;

	//public static int index = 0;
	static Random randomGenerator = new Random();


	public static LEVELS currentLevelIndicator = LEVELS.ONE;

	public static LEVELS GetLevelIndicator()
    {
		return currentLevelIndicator;
    }

	public static void SwitchRooms()
	{

		int index = (int)currentLevelIndicator;

		if(index > 0 && currentLevelIndicator == LEVELS.TWO && levelLists[index-1].finalScene == currentroom  /*&& currentLevelIndicator == LEVELS.TWO*/)
        {
			Debug.Log("PostBoss loaded");
			SceneManager.LoadScene(postBossRoom);
			currentroom = postBossRoom;
			return;
        }

		if (levelLists[index].visited.Count > 0)
		{
			int roomnumber = randomGenerator.Next(0, levelLists[index].visited.Count);
			roomID = levelLists[index].visited[roomnumber];
			currentroom = roomID;
			levelLists[index].visited.Remove(currentroom);
			SceneManager.LoadScene(roomID);

		}
		else
		{
			//levelLists[index].visited.Clear();
			levelLists[index].visited = new List<int>(originalLevelPools[index]);

			currentLevelIndicator++;
			//IMPORTANT: isFinalScene is needed to know when to change from a room to the end scene to show statistics

			if(currentLevelIndicator == LEVELS.MAX)
				Counter.isFinalScene = true;

			currentroom = levelLists[index].finalScene;
			SceneManager.LoadScene(levelLists[index].finalScene);
		}
	}

	public static void ClearStaticData()
    {
		currentLevelIndicator = LEVELS.ONE;
		currentroom = 0;

        for (int i = 0; i < 2; i++)
        {
			levelLists[i].ClearData(originalLevelPools[i]);
        }

		Counter.gameResult = Counter.GameResult.NONE;   // This is morally wrong. But we don't have a scene manager
	}

	//public void OnApplicationQuit()
 //   {
	//	RoomSwitch.ClearStaticData();
	//}

}
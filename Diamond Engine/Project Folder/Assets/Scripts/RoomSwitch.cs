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
        THREE,
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

        new List<int>()
        {
            494045661,  //Room 1
            573625925,  //Room 2
            1529493583, //Room 3
        },

    };

    public readonly static List<RoomDataHolder> levelLists = new List<RoomDataHolder>()
    {
        new RoomDataHolder(originalLevelPools[0], 308281119),
        new RoomDataHolder(originalLevelPools[1], 880545183),
        new RoomDataHolder(originalLevelPools[2], 2069575406),
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

    public static LEVELS GetNextLevel()
    {
        switch (currentLevelIndicator)
        {
            case LEVELS.ONE:
                return LEVELS.TWO;
            case LEVELS.TWO:
                return LEVELS.THREE;
            case LEVELS.THREE:
                return LEVELS.MAX;
            default:
                return LEVELS.MAX;
        }
    }

    public static void SwitchToLevel2()
    {
        currentLevelIndicator = LEVELS.TWO;
        Debug.Log("Level 2 loaded");
        currentroom = levelLists[(int)currentLevelIndicator - 1].finalScene;
        levelLists[0].ClearData(originalLevelPools[0]);
        SwitchRooms();
    }

    public static void SwitchRooms()
    {

        EnemyManager.ClearList();
        int index = (int)currentLevelIndicator;
        if (Core.instance != null)
            Core.instance.SaveBuffs();

        //Load Post Boss Room
        if (index > 0 && currentLevelIndicator == LEVELS.TWO && levelLists[index - 1].finalScene == currentroom  /*&& currentLevelIndicator == LEVELS.TWO*/)
        {
            Debug.Log("PostBoss loaded");
            SceneManager.LoadScene(postBossRoom);
            currentroom = postBossRoom;
            return;
        }

        //Switch Rooms
        if (levelLists[index].visited.Count > 0)
        {
            int roomnumber = randomGenerator.Next(0, levelLists[index].visited.Count);
            roomID = levelLists[index].visited[roomnumber];
            currentroom = roomID;
            levelLists[index].visited.Remove(currentroom);
            SceneManager.LoadScene(roomID);

            PlayLevelEnvironment();
        }
        //Switch Level
        else
        {
            //levelLists[index].visited.Clear();
            levelLists[index].visited = new List<int>(originalLevelPools[index]);

            currentLevelIndicator++;
            //IMPORTANT: isFinalScene is needed to know when to change from a room to the end scene to show statistics

            if (currentLevelIndicator == LEVELS.MAX)
                Counter.isFinalScene = true;

            currentroom = levelLists[index].finalScene;
            SceneManager.LoadScene(levelLists[index].finalScene);

            PlayLevelEnvironment();
        }

    }

    public static void ClearStaticData()
    {
        currentLevelIndicator = LEVELS.ONE;
        currentroom = 0;

        for (int i = 0; i < (int)LEVELS.MAX; i++)
        {
            levelLists[i].ClearData(originalLevelPools[i]);
        }

        Counter.gameResult = Counter.GameResult.NONE;   // This is morally wrong. But we don't have a scene manager
    }

    public static void PlayLevelEnvironment()
    {
        switch(currentLevelIndicator)
        {
            case LEVELS.ONE:
                break;
            case LEVELS.TWO:
                break;
            case LEVELS.THREE:
                Audio.PlayAudio(EnvironmentSourceLocate.instance.gameObject, "Play_Spaceship_Interior_Ambience");
                break;
        }
    }

    public static void OnPlayerDeath()
    {
        Counter.gameResult = Counter.GameResult.DEFEAT;

        ToWinLoseScene();
    }

    public static void OnPlayerWin()
    {
        Counter.gameResult = Counter.GameResult.VICTORY;
        DebugOptionsHolder.goToNextLevel = false;

        ToWinLoseScene();
    }

    public static void OnPlayerQuit()
    {
        Counter.gameResult = Counter.GameResult.NONE;

        //TODo: Maybe this should go to HUB?
        //if (Core.instance != null)
        //    Core.instance.SaveBuffs();

        //EnemyManager.ClearList();

        //ToHub();
        ToWinLoseScene();
    }

    private static void ToWinLoseScene()
    {
        if (Core.instance != null)
            Core.instance.SaveBuffs();

        EnemyManager.ClearList();

        SceneManager.LoadScene(821370213);
    }

}
using System;
using System.Collections.Generic;
using DiamondEngine;

public class RoomDataHolder
{
    public List<int> visited = new List<int>(); //I think this is a fucking pointer-type shit? i don't know, i don't think this is working, im out
    public int bossScene = 0;
    public int preBossScene = 0;

    public RoomDataHolder(List<int> initList, int final, int preBoss = 0)
    {
        //visited.Clear();
        visited = new List<int>(initList);
        bossScene = final;

        preBossScene = preBoss;
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
            1529493583, //Should remove one of these
        },

    };

    public readonly static List<RoomDataHolder> levelLists = new List<RoomDataHolder>()
    {
        new RoomDataHolder(originalLevelPools[0], 308281119),
        new RoomDataHolder(originalLevelPools[1], 880545183),
        new RoomDataHolder(originalLevelPools[2], 2069575406, 518439386),
    };

    private readonly static int shopRoom = 466646284;

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

    public static void SwitchRooms()
    {

        EnemyManager.ClearList();
        int index = (int)currentLevelIndicator;
        if (Core.instance != null)
            Core.instance.SaveBuffs();

        //Load Post Boss Room
        if (index > 0 && currentLevelIndicator <= LEVELS.MAX && levelLists[index - 1].bossScene == currentroom  /*&& currentLevelIndicator == LEVELS.TWO*/)
        {
            Debug.Log("PostBoss loaded");
            SceneManager.LoadScene(shopRoom);
            currentroom = shopRoom;
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

            if(levelLists[index].preBossScene != 0 && currentroom != levelLists[index].preBossScene)
            {
                currentroom = levelLists[index].preBossScene;
                SceneManager.LoadScene(levelLists[index].preBossScene);
                PlayLevelEnvironment();
            }
            else
            {
                Debug.Log("Loading boss scene");
                //levelLists[index].visited.Clear();
                levelLists[index].visited = new List<int>(originalLevelPools[index]);

                currentLevelIndicator++;
                //IMPORTANT: isFinalScene is needed to know when to change from a room to the end scene to show statistics

                if (currentLevelIndicator == LEVELS.MAX)
                    Counter.isFinalScene = true;

                currentroom = levelLists[index].bossScene;
                SceneManager.LoadScene(levelLists[index].bossScene);
                PlayMusicBoss((LEVELS)index);
            }

            
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
                Audio.StopAudio(EnvironmentSourceLocate.instance.gameObject);
                break;
            case LEVELS.TWO:
                Audio.StopAudio(EnvironmentSourceLocate.instance.gameObject);
                break;
            case LEVELS.THREE:
                Audio.PlayAudio(EnvironmentSourceLocate.instance.gameObject, "Play_Spaceship_Interior_Ambience");
                Audio.SetState("Player_State", "Alive");
                Audio.SetState("Game_State", "Cruiser_Explore");
                break;
        }
    }
    public static void PlayMusicBoss(LEVELS index)
    {
        Debug.Log(index.ToString());
        switch (index)
        {
            case LEVELS.ONE:
                break;
            case LEVELS.TWO:
                break;
            case LEVELS.THREE:
                Audio.SetState("Player_State", "Alive");
                Audio.SetState("Game_State", "Moff_Guideon_Room");
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
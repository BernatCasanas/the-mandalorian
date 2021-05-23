using System;
using DiamondEngine;

public class TriggerSwitch : DiamondComponent
{

    public int roomUidToLoad = -1;
    public bool start = true;
    public bool isTutorialScene = false;

    private void Start()
    {
        Audio.SetState("Game_State", "Run");
        Audio.SetState("Player_State", "Alive");
        Debug.Log("Before music instance");
        if (MusicSourceLocate.instance != null)
        {
            Audio.SetSwitch(MusicSourceLocate.instance.gameObject,"Player_Health", "Healthy");
            Audio.SetSwitch(MusicSourceLocate.instance.gameObject,"Player_Action", "Exploring");
        }
        Debug.Log("After music instance");
    }
    public void Update()
    {
        if (start)
        {
            Start();
            start = false;
        }
    }

    public void OnTriggerEnter(GameObject triggeredGameObject)
    {
        if (roomUidToLoad != -1 && triggeredGameObject.CompareTag("Player"))
        {
            RoomDataHolder.isTutorial = isTutorialScene;
            StaticVariablesInit.InitStaticVars();
            SceneManager.LoadScene(roomUidToLoad);
        }

    }

}
using System;
using DiamondEngine;

public class StartMenu : DiamondComponent
{
    public GameObject popUpNewGame = null;
    public GameObject menuButtons = null;
    public GameObject options = null;
    public GameObject background = null;
    public GameObject default_selected = null;

    private bool musicplayed = false;

    public void OnExecuteButton()
    {
        if (gameObject.Name == "New Game" || gameObject.Name == "Load Game")
        {
            if (gameObject.Name == "Load Game")
            {
                DiamondPrefs.LoadData();
                if (DiamondPrefs.ReadBool("loadData") == false)
                    return;
            }
            else
            {
                DiamondPrefs.LoadData();
                if (DiamondPrefs.ReadBool("loadData") == true && popUpNewGame != null && menuButtons != null)
                {
                    popUpNewGame.EnableNav(true);
                    menuButtons.EnableNav(false);
                    if (default_selected != null)
                        default_selected.GetComponent<Navigation>().Select();
                    DiamondPrefs.Clear();
                    return;
                }
                DiamondPrefs.Clear();

            }
            StartGame();
            
        }
        else if (gameObject.Name == "Options")
        {
            menuButtons.EnableNav(false);
            options.EnableNav(true);
            background.EnableNav(true);
            if (default_selected != null)
                default_selected.GetComponent<Navigation>().Select();
        }
        else if (gameObject.Name == "Quit")
        {
            options.EnableNav(true);
            menuButtons.EnableNav(false);
            if (default_selected != null)
                default_selected.GetComponent<Navigation>().Select();
        }
    }
    public void Update()
    {
        if (!musicplayed)
        {
            Audio.SetState("Player_State", "Alive");
            Audio.SetState("Game_State", "Menu");
            musicplayed = true;
        }
    }

    public static void StartGame()
    {
        DiamondPrefs.Write("reset", true);
        if (Core.instance != null)
            Core.instance.SaveBuffs();
        SceneManager.LoadScene(1564453141);


        if (MusicSourceLocate.instance != null)
        {
            Audio.SetSwitch(MusicSourceLocate.instance.gameObject, "Player_Action", "Exploring");
            Debug.Log("Exploring");
        }
        Audio.SetState("Game_State", "HUB");
    }

}
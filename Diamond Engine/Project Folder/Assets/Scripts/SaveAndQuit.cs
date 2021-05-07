using System;
using DiamondEngine;

public class SaveAndQuit : DiamondComponent
{
    public GameObject pauseWindow = null;
    public GameObject quitWindow = null;
    public GameObject default_selected = null;
    public void OnExecuteButton()
    {
        if (pauseWindow == null || quitWindow == null || default_selected == null)
            return;
        DiamondPrefs.SaveData();
        quitWindow.EnableNav(true);
        pauseWindow.EnableNav(false);
        default_selected.GetComponent<Navigation>().Select();
    }

}
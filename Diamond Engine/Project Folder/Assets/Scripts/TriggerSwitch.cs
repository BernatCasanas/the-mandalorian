using System;
using DiamondEngine;

public class TriggerSwitch : DiamondComponent
{

    public int roomUidToLoad = -1;

    public void OnTriggerEnter(GameObject triggeredGameObject)
    {
        if (roomUidToLoad != -1 && triggeredGameObject.CompareTag("Player"))
        {
            EnemyManager.ClearList();
            SceneManager.LoadScene(roomUidToLoad);
        }

    }

}
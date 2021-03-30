using System;
using DiamondEngine;

public class HubRoomSwitch : DiamondComponent
{
    public int nextRoomUID;
    public void OnCollisionEnter(GameObject collidedGameObject)
    {
        if (collidedGameObject.CompareTag("Player"))
        {
            SceneManager.LoadScene(nextRoomUID);
        }
    }
}
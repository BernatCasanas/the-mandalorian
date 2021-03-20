using System;
using DiamondEngine;

public class Respawn : DiamondComponent
{
	//public GameObject spawnpoint = null;
	//public GameObject player = null;
    public int room;
    bool haveToRespawn = false; //I don't know why but it doesn't edit mando's pos on collision enter we have to have a dirty bool and do it from the update
    public void OnCollisionEnter(GameObject collidedGameObject)
    {
        if (collidedGameObject.CompareTag("Player"))
        {
            haveToRespawn = true;
        }

    }

    public void Update()
	{
        if(haveToRespawn)
        {
            Core.instance.gameObject.GetComponent<Core>().RespawnOnFall();
            haveToRespawn = false;
        }
    }

}
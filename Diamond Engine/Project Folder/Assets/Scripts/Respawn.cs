using System;
using DiamondEngine;

public class Respawn : DiamondComponent
{
	//public GameObject spawnpoint = null;
	//public GameObject player = null;
    public int room;
    public void OnCollisionEnter(GameObject collidedGameObject)
    {
          Debug.Log("Respawn");
        SceneManager.LoadScene(room);

    }

    public void Update()
	{

	}

}
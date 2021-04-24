using System;
using DiamondEngine;

public class PostBossEnvironment : DiamondComponent
{
	public void Awake()
    {
		Audio.PlayAudio(this.gameObject, "Play_Post_Boss_Room_1_Ambience");
		Debug.Log("LOOOP");
    }
	public void Update()
	{

	}

}
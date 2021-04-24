using System;
using DiamondEngine;

public class PostBossEnvironment : DiamondComponent
{
	public void Awake()
    {
		Audio.PlayAudio(this.gameObject, "Play_Post_Boss_Room_1_Ambience");
		Audio.SetState("Player_State", "Alive");
		Audio.SetState("Game_State", "Run");

		Audio.SetSwitch(MusicSourceLocate.instance.gameObject, "Player_Action", "Exploring");
		Debug.Log("LOOOP");
    }
	public void Update()
	{

	}

}
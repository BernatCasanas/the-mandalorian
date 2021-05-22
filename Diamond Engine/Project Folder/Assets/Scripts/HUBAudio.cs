using System;
using DiamondEngine;

public class HUBAudio : DiamondComponent
{
	private bool start = false;
	private bool once = true;
	public GameObject cinematicGO = null;
	private IntroCinematic cinematic = null;
	private void Start()
    {
		if (!Counter.firstRun)
		{
			Audio.PlayAudio(gameObject, "Play_Cantine_Ambience");
			Audio.SetState("Game_State", "HUB");
			if (MusicSourceLocate.instance != null)
				Audio.SetSwitch(MusicSourceLocate.instance.gameObject, "Player_Health", "Healthy");
		}
		start = true;
		once = true;
		if (cinematicGO != null)
        {
			cinematic = cinematicGO.GetComponent<IntroCinematic>();
        }
    }
	public void Update()
	{
        if (!start)
        {
			Start();
        }
		if (Counter.firstRun && cinematic.beyondDark && once)
        {
			Audio.PlayAudio(gameObject, "Play_Cantine_Ambience");
			Audio.SetState("Game_State", "HUB");
			if (MusicSourceLocate.instance != null)
				Audio.SetSwitch(MusicSourceLocate.instance.gameObject, "Player_Health", "Healthy");
			once = false;
		}
	}

}
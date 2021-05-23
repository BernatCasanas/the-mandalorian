using System;
using DiamondEngine;

public class TutoManager : DiamondComponent
{
	public void Awake()
    {
        BlackFade.onFadeInCompleted = Core.instance.gameObject.GetComponent<PlayerHealth>().TutorialDie;
    }
}
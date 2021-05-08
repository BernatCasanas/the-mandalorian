using System;
using DiamondEngine;

public class HeavyTrooperParticles : DiamondComponent
{
    public GameObject spearObj = null;
    public ParticleSystem spear = null;

    public void Awake()
    {
        if (spearObj != null)
        {
            spear = spearObj.GetComponent<ParticleSystem>();
        }
    }
}

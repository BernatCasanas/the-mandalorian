using System;
using DiamondEngine;

public class PlayerParticles : DiamondComponent
{
	public GameObject dustObj = null;
	public GameObject muzzleObj = null;
	public GameObject jetpackObj = null;
	public GameObject impactObj = null;
    public GameObject grenade_not = null;
    public GameObject sniper_not = null;

	public ParticleSystem dust = null;
	public ParticleSystem muzzle = null;
	public ParticleSystem jetpack = null;
	public ParticleSystem impact = null;
    public ParticleSystem grenade = null;
    public ParticleSystem sniper = null;

    public void Awake()
    {
        if (dustObj != null)
        {
            dust = dustObj.GetComponent<ParticleSystem>();
        }
        if (muzzleObj != null)
        {
            muzzle = muzzleObj.GetComponent<ParticleSystem>();
        }
        if (jetpackObj != null)
        {
            jetpack = jetpackObj.GetComponent<ParticleSystem>();
        }

        if (impactObj != null)
        {
            impact = impactObj.GetComponent<ParticleSystem>();
        }
        
        if (grenade_not != null)
        {
            grenade = grenade_not.GetComponent<ParticleSystem>();
        }
        if (sniper_not != null)
        {
            sniper = sniper_not.GetComponent<ParticleSystem>();
        }
    }
    public void Update()
	{

	}

}
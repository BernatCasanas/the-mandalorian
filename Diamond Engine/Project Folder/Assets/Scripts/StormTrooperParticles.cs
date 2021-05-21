using System;
using DiamondEngine;

public class StormTrooperParticles : DiamondComponent
{
    
    public GameObject spawnObj = null;
    public GameObject deadObj = null;
    public GameObject waveObj = null;
    public GameObject soulsObj = null;
    public GameObject alertObj = null;
    public GameObject hitExplosion = null;
    public GameObject sniperHitObj = null;

    public ParticleSystem spawn = null;
    public ParticleSystem dead = null;
    public ParticleSystem wave = null;
    public ParticleSystem souls = null;
    public ParticleSystem alert = null;
    public ParticleSystem hit = null;
    public ParticleSystem sniperHit = null;

    public void Awake()
    {
       
        if (spawnObj != null)
        {
            spawn = spawnObj.GetComponent<ParticleSystem>();
        }

        if (deadObj != null)
        {
            dead = deadObj.GetComponent<ParticleSystem>();
        }
        if (waveObj != null)
        {
            wave = waveObj.GetComponent<ParticleSystem>();
        }
        if (soulsObj != null)
        {
            souls = soulsObj.GetComponent<ParticleSystem>();
        }
        if (alertObj != null)
        {
            alert = alertObj.GetComponent<ParticleSystem>();
        }
        if (hitExplosion != null)
        {
            hit = hitExplosion.GetComponent<ParticleSystem>();
        }
        if (sniperHitObj != null)
        {
            sniperHit = sniperHitObj.GetComponent<ParticleSystem>();
        }
    }
    public void Update()
    {
      
    }

}
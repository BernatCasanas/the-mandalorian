using System;
using DiamondEngine;

public class StormTrooperParticles : DiamondComponent
{
    
    public GameObject spawnObj = null;
    public GameObject deadObj = null;
    public GameObject waveObj = null;
    public GameObject soulsObj = null;

    public ParticleSystem spawn = null;
    public ParticleSystem dead = null;
    public ParticleSystem wave = null;
    public ParticleSystem souls = null;

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


      
        
    }
    public void Update()
    {
      
    }

}
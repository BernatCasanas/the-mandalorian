using System;
using DiamondEngine;

public class StormTrooperParticles : DiamondComponent
{
    public GameObject circlesObj = null;
    public GameObject circlesUPObj = null;
    public GameObject stormTrooperObj = null;
    public GameObject deadObj = null;
    public GameObject waveObj = null;
    public GameObject soulsObj = null;
    public ParticleSystem circles = null;
    public ParticleSystem circlesUp = null;
    public ParticleSystem stormTrooper = null;
    public ParticleSystem dead = null;
    public ParticleSystem wave = null;
    public ParticleSystem souls = null;

    public void Awake()
    {
        if (circlesObj != null) 
        {
            circles = circlesObj.GetComponent<ParticleSystem>();
        }
        if (circlesUPObj != null)
        {
            circlesUp = circlesUPObj.GetComponent<ParticleSystem>();
        }
        if (stormTrooperObj != null)
        {
            stormTrooper = stormTrooperObj.GetComponent<ParticleSystem>();
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


        if (circles != null) 
        {
            circles.Play();
        }
        if (circlesUp != null)
        {
            circlesUp.Play();
        }
        if (stormTrooper != null)
        {
            stormTrooper.Play();
        }
        
    }
    public void Update()
    {
      
    }

}
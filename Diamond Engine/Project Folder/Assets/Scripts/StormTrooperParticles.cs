using System;
using DiamondEngine;

public class StormTrooperParticles : DiamondComponent
{
    public GameObject circles;
    public GameObject circlesUp;
    public GameObject stormTrooper;

    public bool start = true;
    public void Update()
    {
        if (start)
        {
            ParticleSystem circlesP = null;
            ParticleSystem circlesUpP = null;
            ParticleSystem stormTrooperP = null;

            if (circles != null && circlesUp != null && stormTrooper != null)
            {

                circlesP = circles.GetComponent<ParticleSystem>();
                circlesUpP = circlesUp.GetComponent<ParticleSystem>();
                stormTrooperP = stormTrooper.GetComponent<ParticleSystem>();
            }


            if (circlesP != null && circlesUpP != null && stormTrooperP != null)
            {
                circlesP.Play();
                circlesUpP.Play();
                stormTrooperP.Play();
            }
            start = false;
        }
    }

}
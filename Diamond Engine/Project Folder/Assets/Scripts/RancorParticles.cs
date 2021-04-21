using DiamondEngine;
using System;
using System.Collections.Generic;
public class RancorParticles : DiamondComponent
{
    public GameObject trailLeftObj = null;
    public GameObject trailRightObj = null;
    public GameObject impactObj = null;
    public GameObject rushObj = null;
    public GameObject swingObj = null;
    public GameObject handSlamObj = null;
    public ParticleSystem trailLeft = null;
    public ParticleSystem trailRight = null;
    public ParticleSystem impact = null;
    public ParticleSystem rush = null;
    public ParticleSystem swing = null;
    public ParticleSystem handSlam = null;
    public void Awake()
    {
        if (trailLeftObj != null)
        {
            trailLeft = trailLeftObj.GetComponent<ParticleSystem>();
        }
        if (trailRightObj != null)
        {
            trailRight = trailRightObj.GetComponent<ParticleSystem>();
        }
        if (impactObj != null)
        {
            impact = impactObj.GetComponent<ParticleSystem>();
        }
        if (rushObj != null)
        {
            rush = rushObj.GetComponent<ParticleSystem>();
        }
        if (swingObj != null)
        {
            swing = swingObj.GetComponent<ParticleSystem>();
        }
        if (handSlamObj != null)
        {
            handSlam = handSlamObj.GetComponent<ParticleSystem>();
        }
    }
    public void Update()
    {
    }
}
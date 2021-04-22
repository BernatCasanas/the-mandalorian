﻿using DiamondEngine;
using System;
using System.Collections.Generic;
public class RancorParticles : DiamondComponent
{
    public GameObject trailLeftObj = null;
    public GameObject trailRightObj = null;
    public GameObject impactObj = null;
    public GameObject rushObj = null;
    public GameObject swingLeftObj = null;
    public GameObject swingRightObj = null;
    public GameObject handSlamObj = null;
    public GameObject roarObj = null;
    public ParticleSystem trailLeft = null;
    public ParticleSystem trailRight = null;
    public ParticleSystem impact = null;
    public ParticleSystem rush = null;
    public ParticleSystem swingLeft = null;
    public ParticleSystem swingRight = null;
    public ParticleSystem handSlam = null;
    public ParticleSystem roar = null;
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
        if (swingRightObj != null)
        {
            swingRight = swingRightObj.GetComponent<ParticleSystem>();
        }
        if (swingLeftObj != null)
        {
            swingLeft = swingLeftObj.GetComponent<ParticleSystem>();
        }
        if (handSlamObj != null)
        {
            handSlam = handSlamObj.GetComponent<ParticleSystem>();
        }
        if (roarObj != null)
        {
            roar = roarObj.GetComponent<ParticleSystem>();
        }
    }
    public void Update()
    {
    }
}
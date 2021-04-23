using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Collections.Generic;

using DiamondEngine;
public class Skel : Bosseslv2
{
    private bool start = false;
    private void Start()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
        if (agent == null)
            Debug.Log("Null agent, add a NavMeshAgent Component");

        Animator.Play(gameObject, "");
        Audio.PlayAudio(gameObject, "");
        Counter.roomEnemies++;  // Just in case
        EnemyManager.AddEnemy(gameObject);
    }

    public void Update()
	{
        if (!start) Start();

		Debug.Log("ghola");
	}

}
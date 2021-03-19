using System;
using DiamondEngine;

public class smallGrenade : DiamondComponent
{
    public GameObject thisReference = null; //This is needed until i make all this be part of a component base class


    public float detonationTime = 2.0f;

    private float Timer = 0.0f;

    private bool move = true;


    public void OnCollisionEnter(GameObject collidedGameObject)
    {

        move = false;
        Debug.Log("Collision");

    }

    public void Update()
    {
        if (!move)
        {
            Timer += Time.deltaTime;

        }

        if (Timer > detonationTime)
        {
            InternalCalls.Destroy(thisReference);
           

            


        }

    }

}
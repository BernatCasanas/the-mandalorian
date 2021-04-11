using System;
using DiamondEngine;

public class MoveMeleeCollForward : DiamondComponent
{
    public float colliderSpeed = 1.0f;

    public bool startState = false;

    private bool colliderState = false;
    private bool start = false;
    private Vector3 startingPos;


    public void Update()
    {
        // This is intentional, since we con't change the position in the Awake() function
        if (start == false)
        {
            startingPos = gameObject.transform.localPosition;
            colliderState = !startState;
            EnableHit(startState);
            start = true;
        }

        if (colliderState == true)
            gameObject.transform.localPosition += Vector3.forward * colliderSpeed * Time.deltaTime;

    }


    public void EnableHit(bool newState)
    {
        if (colliderState == newState)
            return;

        colliderState = !colliderState;

        if (newState == true)
        {
            ToTrueState();
        }
        else
        {
            ToFalseState();
        }

    }


    void ToTrueState()
    {
        gameObject.transform.localPosition = startingPos;
    }


    void ToFalseState()
    {
        gameObject.transform.localPosition = new Vector3(0, 10000f, 0);
    }


}
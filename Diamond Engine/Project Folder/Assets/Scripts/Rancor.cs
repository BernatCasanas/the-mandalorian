using System;
using DiamondEngine;

using System.Collections.Generic;

public class Rancor : DiamondComponent
{

	enum RANCOR_STATE : int
    {
		NONE = -1,
		SEARCH_STATE,
		WANDER,
		RUSH,
		HAND_SLAM,
		PROJECTILE,
		MELEE_COMBO_HIT1,
        MELEE_COMBO_HIT2,
        MELEE_COMBO_HIT3,
        DEAD
    }

	enum RANCOR_INPUT : int
    {
		NONE = -1,
		IN_WANDER_SHORT,
        IN_WANDER_LONG,
        IN_WANDER_END,
		IN_RUSH,
		IN_RUSH_END,
		IN_HAND_SLAM,
		IN_HAND_SLAM_END,
		IN_PROJECTILE,
		IN_PROJECTILE_END,
		IN_MELEE_COMBO_1HIT,
        IN_MELEE_COMBO_2HIT,
        IN_MELEE_COMBO_3HIT,
		IN_MELEE_COMBO_END,
        IN_DEAD
	}

	//State
	private RANCOR_STATE currentState = RANCOR_STATE.WANDER;   //NEVER SET THIS VARIABLE DIRECTLLY, ALLWAYS USE INPUTS
															   //Setting states directlly will break the behaviour  -Jose
	private List<RANCOR_INPUT> inputsList = new List<RANCOR_INPUT>();
    Random randomNum = new Random();

    public float shortWanderTime = 2.0f;
    public float longWanderTime = 4.0f;
    private float wanderTimer = 0.0f;

    public float meleeComboHit1Time = 2.0f;
    public float meleeComboHit2Time = 2.0f;
    public float meleeComboHit3Time = 4.0f;

    private float meleeCH1Timer = 0.0f;
    private float meleeCH2Timer = 0.0f;
    private float meleeCH3Timer = 0.0f;

    private bool start = false;

    private void Start()
    {
        wanderTimer = shortWanderTime;
    }

    public void Update()
	{
        if (start == false)
        {
            Start();
            start = true;
        }

		ProcessInternalInput();
		ProcessExternalInput();
		ProcessState();

        UpdateState();
	}

	//Timers go here
	private void ProcessInternalInput()
    {
        if (wanderTimer > 0)
        {
            wanderTimer -= Time.deltaTime;

            if (wanderTimer <= 0)
                inputsList.Add(RANCOR_INPUT.IN_WANDER_END);
        }

        if (meleeCH1Timer > 0)
        {
            meleeCH1Timer -= Time.deltaTime;

            if (meleeCH1Timer <= 0)
                inputsList.Add(RANCOR_INPUT.IN_MELEE_COMBO_2HIT);
        }

        if (meleeCH2Timer > 0)
        {
            meleeCH2Timer -= Time.deltaTime;

            if (meleeCH2Timer <= 0)
                inputsList.Add(RANCOR_INPUT.IN_MELEE_COMBO_3HIT);
        }

        if (meleeCH3Timer > 0)
        {
            meleeCH3Timer -= Time.deltaTime;

            if (meleeCH3Timer <= 0)
                inputsList.Add(RANCOR_INPUT.IN_MELEE_COMBO_END);
        }
    }

	private void ProcessExternalInput()
    {
        

    }

	private void ProcessState()
    {
		while (inputsList.Count > 0)
		{
			RANCOR_INPUT input = inputsList[0];

            switch (currentState)
            {
                case RANCOR_STATE.NONE:
					Debug.Log("CORE ERROR STATE");
					break;

                case RANCOR_STATE.SEARCH_STATE:
                    switch (input)
                    {
                        case RANCOR_INPUT.IN_WANDER_SHORT:
                            StartWander();
                            currentState = RANCOR_STATE.WANDER;
                            break;

                        case RANCOR_INPUT.IN_WANDER_LONG:
                            StartWander();
                            currentState = RANCOR_STATE.WANDER;
                            break;

                        case RANCOR_INPUT.IN_RUSH:
                            break;
                        
                        case RANCOR_INPUT.IN_HAND_SLAM:
                            break;
                        
                        case RANCOR_INPUT.IN_PROJECTILE:
                            break;
                       
                        case RANCOR_INPUT.IN_MELEE_COMBO_1HIT:
                            currentState = RANCOR_STATE.MELEE_COMBO_HIT1;
                            StartMCHit1();
                            break;
                        
                        case RANCOR_INPUT.IN_DEAD:
                            break;
                    }
                    break;

                case RANCOR_STATE.WANDER:
                    switch (input)
                    {
                        case RANCOR_INPUT.IN_WANDER_END:
                            currentState = RANCOR_STATE.SEARCH_STATE;
                            break;

                        case RANCOR_INPUT.IN_DEAD:
                            break;
                    }
                    break;

                case RANCOR_STATE.RUSH:
                    break;

                case RANCOR_STATE.HAND_SLAM:
                    break;

                case RANCOR_STATE.PROJECTILE:
                    break;

                case RANCOR_STATE.MELEE_COMBO_HIT1:
                    switch (input)
                    {
                        case RANCOR_INPUT.IN_MELEE_COMBO_2HIT:
                            currentState = RANCOR_STATE.MELEE_COMBO_HIT2;
                            EndMCHit1();
                            StartMCHit2();
                            break;
                      
                        case RANCOR_INPUT.IN_DEAD:
                            break;
                    }
                    break;

                case RANCOR_STATE.MELEE_COMBO_HIT2:
                    switch (input)
                    {
                        case RANCOR_INPUT.IN_MELEE_COMBO_3HIT:
                            currentState = RANCOR_STATE.MELEE_COMBO_HIT3;
                            EndMCHit2();
                            StartMCHit3();
                            break;

                        case RANCOR_INPUT.IN_DEAD:
                            break;
                    }
                    break;

                case RANCOR_STATE.MELEE_COMBO_HIT3:
                    switch (input)
                    {
                        case RANCOR_INPUT.IN_MELEE_COMBO_END:
                            currentState = RANCOR_STATE.SEARCH_STATE;
                            EndMCHit3();
                            break;

                        case RANCOR_INPUT.IN_DEAD:
                            break;
                    }
                    break;

                default:
					Debug.Log("NEED TO ADD STATE TO RANCOR");
                    break;
            }
            inputsList.RemoveAt(0);
        }
	}

    private void UpdateState()
    {
        switch (currentState)
        {
            case RANCOR_STATE.NONE:
                Debug.Log("CORE ERROR STATE");
                break;

            case RANCOR_STATE.SEARCH_STATE:
                SelectAction();
                break;

            case RANCOR_STATE.WANDER:
                UpdateWander();
                break;

            case RANCOR_STATE.RUSH:
                break;

            case RANCOR_STATE.HAND_SLAM:
                break;

            case RANCOR_STATE.PROJECTILE:
                break;

            case RANCOR_STATE.MELEE_COMBO_HIT1:
                UpdateMCHit1();
                break;

            case RANCOR_STATE.MELEE_COMBO_HIT2:
                UpdateMCHit2();
                break;

            case RANCOR_STATE.MELEE_COMBO_HIT3:
                UpdateMCHit3();
                break;

            case RANCOR_STATE.DEAD:
                break;
            default:
                break;
        }
    }


    private void SelectAction()
    {
        int decision = randomNum.Next(1, 100);

        Debug.Log("Decision value: " + decision.ToString());

        if (decision <= 60)
        {
            //Do all distance checks
            float distance = Mathf.Distance(Core.instance.gameObject.transform.localPosition, gameObject.transform.localPosition);

            if (distance < 20)
            {
                inputsList.Add(RANCOR_INPUT.IN_MELEE_COMBO_1HIT);
            }
        }
        else if (decision > 60 && decision <= 90)
            inputsList.Add(RANCOR_INPUT.IN_WANDER_SHORT);

        else if (decision > 90)
            inputsList.Add(RANCOR_INPUT.IN_WANDER_LONG);

    }


    #region MELEE_COMBO
    private void StartMCHit1()
    {
        meleeCH1Timer = meleeComboHit1Time;
        //TODO: Add animation
    }

    private void UpdateMCHit1()
    {
        Debug.Log("Combo hit 1");
        //TODO: Activate collider
    }

    private void EndMCHit1()
    {
        //TODO: Deactivate collider
    }


    private void StartMCHit2()
    {
        meleeCH2Timer = meleeComboHit2Time;
        //TODO: Add animation
    }

    private void UpdateMCHit2()
    {
        Debug.Log("Combo hit 2");
        //TODO: Activate collider
    }

    private void EndMCHit2()
    {
        //TODO: Deactivate collider
    }


    private void StartMCHit3()
    {
        meleeCH3Timer = meleeComboHit3Time;
        //TODO: Add animation
    }

    private void UpdateMCHit3()
    {
        Debug.Log("Combo hit 3");
        //TODO: Activate collider
    }

    private void EndMCHit3()
    {
        //TODO: Deactivate collider
    }

    #endregion


    #region WANDER

    private void StartWander()
    {
        //Start walk animation
        //Search point

        wanderTimer = shortWanderTime;
    }


    private void UpdateWander()
    {
        //Move character
        Debug.Log("Wandering");
    }


    private void EndWander()
    {
        
    }

    #endregion
}
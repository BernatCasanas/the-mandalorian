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

    private NavMeshAgent agent = null;

	//State
	private RANCOR_STATE currentState = RANCOR_STATE.WANDER;   //NEVER SET THIS VARIABLE DIRECTLLY, ALLWAYS USE INPUTS
															   //Setting states directlly will break the behaviour  -Jose
	private List<RANCOR_INPUT> inputsList = new List<RANCOR_INPUT>();
    Random randomNum = new Random();

    //Stats
    public int attackProbability = 66;  //FROM 1 TO A 100
    public int shortWanderProbability = 90; //FROM THE PREVIOS VALUE TO HERE

    public float meleeRange = 14.0f;
    public float longRange = 21.0f;

    //Wander
    public float shortWanderTime = 2.0f;
    public float longWanderTime = 4.0f;
    private float wanderTimer = 0.0f;
    public float wanderSpeed = 2.0f;

    //Melee Combo
    private float meleeComboHit1Time = 0.0f;
    private float meleeComboHit2Time = 0.0f;
    private float meleeComboHit3Time = 0.0f;

    private float meleeCH1Timer = 0.0f;
    private float meleeCH2Timer = 0.0f;
    private float meleeCH3Timer = 0.0f;

    //Projectile
    private float projectileTime = 0.0f;
    private float projectileTimer = 0.0f;


    //Hand slam
    public float handSlamTime = 0.0f;
    private float handSlamTimer = 0.0f;


    //rush
    public float rushTime = 0.0f;
    private float rushTimer = 0.0f;

    private bool start = false;

    private void Start()
    {
        wanderTimer = shortWanderTime;

        meleeComboHit1Time = Animator.GetAnimationDuration(gameObject, "RN_MeleeComboP1") - 0.016f;
        meleeComboHit2Time = Animator.GetAnimationDuration(gameObject, "RN_MeleeComboP2") - 0.016f;
        meleeComboHit3Time = Animator.GetAnimationDuration(gameObject, "RN_MeleeComboP3") - 0.016f;

        projectileTime = Animator.GetAnimationDuration(gameObject, "RN_ProjectileThrow") - 0.016f;

        handSlamTime = Animator.GetAnimationDuration(gameObject, "RN_HandSlam") - 0.016f;

        rushTime = Animator.GetAnimationDuration(gameObject, "RN_Rush") - 0.016f;
    }

    public void Awake()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();

        if (agent == null)
            Debug.Log("Null agent, add a NavMeshAgent Component");

        Animator.Play(gameObject, "RN_Idle");
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
            {
                inputsList.Add(RANCOR_INPUT.IN_WANDER_END);
            }
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

        if (projectileTimer > 0)
        {
            projectileTimer -= Time.deltaTime;

            if (projectileTimer <= 0)
                inputsList.Add(RANCOR_INPUT.IN_PROJECTILE_END);
        }


        if (handSlamTimer > 0)
        {
            handSlamTimer -= Time.deltaTime;

            if (handSlamTimer <= 0)
                inputsList.Add(RANCOR_INPUT.IN_HAND_SLAM_END);
        }


        if (rushTimer > 0)
        {
            rushTimer -= Time.deltaTime;

            if (rushTimer <= 0)
                inputsList.Add(RANCOR_INPUT.IN_RUSH_END);
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
                            currentState = RANCOR_STATE.WANDER;
                            StartShortWander();
                            break;

                        case RANCOR_INPUT.IN_WANDER_LONG:
                            currentState = RANCOR_STATE.WANDER;
                            StartLongWander();
                            break;

                        case RANCOR_INPUT.IN_RUSH:
                            currentState = RANCOR_STATE.RUSH;
                            StartRush();
                            break;
                        
                        case RANCOR_INPUT.IN_HAND_SLAM:
                            currentState = RANCOR_STATE.HAND_SLAM;
                            StartHandSlam();
                            break;
                        
                        case RANCOR_INPUT.IN_PROJECTILE:
                            currentState = RANCOR_STATE.PROJECTILE;
                            StartProjectile();
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
                            EndWander();
                            break;

                        case RANCOR_INPUT.IN_DEAD:
                            break;
                    }
                    break;

                case RANCOR_STATE.RUSH:
                    switch (input)
                    {
                        case RANCOR_INPUT.IN_RUSH_END:
                            currentState = RANCOR_STATE.SEARCH_STATE;
                            EndRush();
                            break;
                        
                        case RANCOR_INPUT.IN_DEAD:
                            break;
                    }
                    break;

                case RANCOR_STATE.HAND_SLAM:
                    switch (input)
                    {
                        case RANCOR_INPUT.IN_HAND_SLAM_END:
                            currentState = RANCOR_STATE.SEARCH_STATE;
                            EndHandSlam();
                            break;
                        
                        case RANCOR_INPUT.IN_DEAD:
                            break;
                    }
                    break;

                case RANCOR_STATE.PROJECTILE:
                    switch (input)
                    {
                        case RANCOR_INPUT.IN_PROJECTILE_END:
                            currentState = RANCOR_STATE.SEARCH_STATE;
                            EndProjectile();
                            break;
                        
                        case RANCOR_INPUT.IN_DEAD:
                            break;
                    }
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
                UpdateRush();
                break;

            case RANCOR_STATE.HAND_SLAM:
                UpdateHandSlam();
                break;

            case RANCOR_STATE.PROJECTILE:
                UpdateProjectile();
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

        if (decision <= attackProbability)
        {
            //Do all distance checks
            float distance = Mathf.Distance(Core.instance.gameObject.transform.localPosition, gameObject.transform.localPosition);

            if (distance <= meleeRange)
            {
                decision = randomNum.Next(1, 100);

                if (decision <= 50)
                    inputsList.Add(RANCOR_INPUT.IN_MELEE_COMBO_1HIT);

                else
                    inputsList.Add(RANCOR_INPUT.IN_HAND_SLAM);
            }

            else if (distance > meleeRange && distance <= longRange)
            {
                decision = randomNum.Next(1, 100);

                if (decision <= 50)
                    inputsList.Add(RANCOR_INPUT.IN_PROJECTILE);

                else
                    inputsList.Add(RANCOR_INPUT.IN_RUSH); 
            }
            else
            {
                //Projectile
                inputsList.Add(RANCOR_INPUT.IN_PROJECTILE);
            }
        }
        else if (decision > attackProbability && decision <= shortWanderProbability)
            inputsList.Add(RANCOR_INPUT.IN_WANDER_SHORT);

        else if (decision > shortWanderProbability)
            inputsList.Add(RANCOR_INPUT.IN_WANDER_LONG);
    }


    #region MELEE_COMBO
    private void StartMCHit1()
    {
        meleeCH1Timer = meleeComboHit1Time;

        Animator.Play(gameObject, "RN_MeleeComboP1");
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
        Animator.Play(gameObject, "RN_MeleeComboP2");
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
        Animator.Play(gameObject, "RN_MeleeComboP3");
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

    private void StartShortWander()
    {
        //Start walk animation
        Animator.Play(gameObject, "RN_Walk");

        //Search point
        wanderTimer = shortWanderTime;

        if (agent != null)
            agent.CalculatePath(gameObject.transform.globalPosition, Core.instance.gameObject.transform.globalPosition);
    }


    private void StartLongWander()
    {
        //Start walk animation
        //Search point

        Animator.Play(gameObject, "RN_Walk");

        wanderTimer = longWanderTime;

        if (agent != null)
            agent.CalculatePath(gameObject.transform.globalPosition, Core.instance.gameObject.transform.globalPosition);
    }


    private void UpdateWander()
    {
        //Move character

        if (agent != null && Mathf.Distance(gameObject.transform.globalPosition, agent.GetDestination()) > agent.stoppingDistance)
            agent.MoveToCalculatedPos(wanderSpeed);

        Debug.Log("Wandering");
    }


    private void EndWander()
    {
        
    }

    #endregion


    #region PROJECTILE

    private void StartProjectile()
    {
        projectileTimer = projectileTime;

        Animator.Play(gameObject, "RN_ProjectileThrow");
        //add timer to spawn projectiles
    }

    private void UpdateProjectile()
    {
        Debug.Log("Projectile");
    }

    private void EndProjectile()
    {

    }

    #endregion


    #region HAND_SLAM

    private void StartHandSlam()
    {
        handSlamTimer = handSlamTime;
        Animator.Play(gameObject, "RN_HandSlam");
    }


    private void UpdateHandSlam()
    {
        Debug.Log("Hand slam");
    }


    private void EndHandSlam()
    {

    }

    #endregion

    #region rush

    private void StartRush()
    {
        rushTimer = rushTime;
        Animator.Play(gameObject, "RN_Rush");
    }


    private void UpdateRush()
    {
        Debug.Log("Rush");
    }


    private void EndRush()
    {

    }

    #endregion


}
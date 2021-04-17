using System;
using DiamondEngine;

using System.Collections.Generic;


public class Wampa : DiamondComponent
{
    enum WAMPA_STATE : int
    {
        NONE = -1,
        SEARCH_STATE,
        FOLLOW,
        WANDER,
        RUSH,
        RUSH_STUN,
        PROJECTILE,
        DEAD
    }

    enum WAMPA_INPUT : int
    {
        NONE = -1,
        IN_FOLLOW,
        IN_FOLLOW_END,
        IN_WANDER,
        IN_WANDER_END,
        IN_FAST_RUSH,
        IN_FAST_RUSH_END,
        IN_SLOW_RUSH,
        IN_SLOW_RUSH_END,
        IN_RUSH_STUN,
        IN_RUSH_STUN_END,
        IN_PROJECTILE,
        IN_PROJECTILE_END,
        IN_DEAD
    }

    private NavMeshAgent agent = null;

    private WAMPA_STATE currentState = WAMPA_STATE.FOLLOW;
    private List<WAMPA_INPUT> inputsList = new List<WAMPA_INPUT>();

    public float slerpSpeed = 5.0f;

    //Public Variables
    public float probFollow = 60.0f;
    public float probWander = 40.0f;
    public GameObject projectilePoint = null;


    //Private Variables
    private float multiplier = 1.0f;

    //Stats
    public float healthPoints = 1920.0f;
    public float speed = 3.5f;

    //Timers
    private float walkingTime = 4.0f;
    private float walkingTimer = 0.0f;
    private float fastChasingTime = 0.5f;
    private float fastChasingTimer = 0.0f;
    private float slowChasingTime = 3.5f;
    private float slowChasingTimer = 0.0f;
    private float shootingTime = 2.0f;
    private float shootingTimer = 0.0f;
    private float dieTime = 2.0f;
    private float dieTimer = 0.0f; 

    //Atacks
    public float projectileAngle = 30.0f;
    public float projectileRange = 6.0f;
    public float projectileDamage = 10.0f;
    public float rushDamage = 15.0f;

    public void Awake()
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
        ProcessInternalInput();
        ProcessExternalInput();
        ProcessState();

        UpdateState();
    }

    private void ProcessInternalInput()
    {
        if (walkingTimer > 0)
        {
            walkingTimer -= Time.deltaTime;

            if (walkingTimer <= 0)
            {
                if(currentState == WAMPA_STATE.FOLLOW)
                    inputsList.Add(WAMPA_INPUT.IN_FOLLOW_END);
                else if(currentState == WAMPA_STATE.WANDER)
                    inputsList.Add(WAMPA_INPUT.IN_WANDER_END);
            }
        }

        if (fastChasingTimer > 0)
        {
            fastChasingTimer -= Time.deltaTime;

            if (fastChasingTimer <= 0)
            {
                inputsList.Add(WAMPA_INPUT.IN_FAST_RUSH_END);
            }
        }

        if (slowChasingTimer > 0)
        {
            slowChasingTimer -= Time.deltaTime;

            if (slowChasingTimer <= 0)
            {
                inputsList.Add(WAMPA_INPUT.IN_SLOW_RUSH_END);
            }
        }
    }

    private void ProcessExternalInput()
    {
        
    }

    private void ProcessState()
    {
        while (inputsList.Count > 0)
        {
            WAMPA_INPUT input = inputsList[0];

            switch (currentState)
            {
                case WAMPA_STATE.NONE:
                    Debug.Log("WAMPA ERROR STATE");
                    break;

                case WAMPA_STATE.SEARCH_STATE:
                    switch (input)
                    {
                        case WAMPA_INPUT.IN_PROJECTILE:
                            currentState = WAMPA_STATE.PROJECTILE;
                            StartProjectile();
                            break;

                        case WAMPA_INPUT.IN_FAST_RUSH:
                            currentState = WAMPA_STATE.RUSH;
                            StartRush();
                            break;

                        case WAMPA_INPUT.IN_FOLLOW:
                            currentState = WAMPA_STATE.FOLLOW;
                            StartFollowing();
                            break;
                    }
                    break;

                case WAMPA_STATE.FOLLOW:
                    switch (input)
                    {
                        case WAMPA_INPUT.IN_FOLLOW_END:
                            currentState = WAMPA_STATE.SEARCH_STATE;
                            EndFollowing();
                            break;
                        case WAMPA_INPUT.IN_DEAD:
                            currentState = WAMPA_STATE.SEARCH_STATE;
                            StartDie();
                            break;
                    }
                    break;
                case WAMPA_STATE.PROJECTILE:
                    break;
                case WAMPA_STATE.RUSH:
                    break;
                case WAMPA_STATE.RUSH_STUN:
                    break;
                case WAMPA_STATE.WANDER:
                    break;

                default:
                    Debug.Log("NEED TO ADD STATE TO RANCOR");
                    break;
            }
            inputsList.RemoveAt(0);
        }
    }

    #region PROJECTILE
    private void StartProjectile()
    {

    }
    private void UpdateProjectile()
    {

    }

    private void EndProjectile()
    {

    }
    #endregion

    #region RUSH
    private void StartRush()
    {

    }
    private void UpdateRush()
    {

    }

    private void EndRush()
    {

    }
    #endregion

    #region FOLLOW
    private void StartFollowing()
    {

    }
    private void UpdateFollowing()
    {

    }

    private void EndFollowing()
    {

    }
    #endregion

    #region DIE
    private void StartDie()
    {

    }
    private void UpdateDie()
    {

    }

    private void EndDie()
    {

    }
    #endregion

    private void UpdateState()
    {

    }

}
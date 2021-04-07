using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Collections.Generic;

using DiamondEngine;

public class StormTrooper2 : Enemy
{
    enum STATE : int
    {
        NONE = -1,
        IDLE,
        WANDER,
        RUN,
        PUSHED,
        SHOOT,
        HIT,
        DIE
    }

    enum INPUT : int
    {
        IN_IDLE,
        IN_IDLE_END,
        IN_RUN,
        IN_RUN_END,
        IN_WANDER,
        IN_PUSHED,
        IN_SHOOT,
        IN_SEQUENCE_END,
        IN_HIT,
        IN_DIE,
        IN_DIE_END,
        IN_PLAYER_IN_RANGE
    }

    //State
    private STATE currentState = STATE.NONE; //NEVER SET THIS VARIABLE DIRECTLLY, ALLWAYS USE INPUTS
                                             //Setting states directlly will break the behaviour  -Jose
    private List<INPUT> inputsList = new List<INPUT>();

    private bool started = false;

    public GameObject shootPoint = null;
    public GameObject hitParticles = null;

    //Action times
    public float idleTime = 5.0f;
    public float dieTime = 3.0f;
    public float timeBewteenShots = 0.5f;
    public float timeBewteenSequences = 1.0f;
    public float timeBewteenStates = 1.5f;

    //Speeds
    public float wanderSpeed = 3.5f;
    public float runningSpeed = 7.5f;
    public float bulletSpeed = 10.0f;
    //private float pushSkillSpeed = 0.2f;

    //Ranges
    public float wanderRange = 7.5f;
    public float runningRange = 12.5f;

    //Timers
    private float idleTimer = 0.0f;
    private float shotTimer = 0.0f;
    private float sequenceTimer = 0.0f;
    private float dieTimer = 0.0f;
    private float statesTimer = 0.0f;
    //private float pushSkillTimer = 0.15f;

    //Action variables
    int shotTimes = 0;
    public int maxShots = 2;
    private int shotSequences = 0;
    public int maxSequences = 2;

    //private bool rightTriggerPressed = false;

    private void Start()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
        targetPosition = null;

        currentState = STATE.IDLE;
        Animator.Play(gameObject, "ST_Idle");

        shotTimes = 0;
        shotSequences = 0;
        //stormTrooperDamage = 1.0f;
    }

    public void Update()
    {
        // Placeholder for Start() function
        if (started == false)
        {
            Start();
            started = true;
        }

        if (player == null)
        {
            Debug.Log("Null player");
            player = Core.instance.gameObject;
        }

        #region STATE MACHINE

        ProcessInternalInput();
        ProcessExternalInput();
        ProcessState();

        UpdateState();

        #endregion
    }


    //Timers go here
    private void ProcessInternalInput()
    {
        if (idleTimer > 0.0f)
        {
            idleTimer -= Time.deltaTime;

            if (idleTimer <= 0.0f)
            {
                inputsList.Add(INPUT.IN_WANDER);
            }
        }

        if (currentState == STATE.RUN || currentState == STATE.WANDER)
        {
            Debug.Log("Stopping distance: " + agent.stoppingDistance);
            if (Mathf.Distance(gameObject.transform.localPosition, agent.GetDestination()) <= agent.stoppingDistance)
            {
                inputsList.Add(INPUT.IN_IDLE);
                Debug.Log("Stop running man");
            }
        }

        //if (statesTimer > 0.0f)
        //{
        //    statesTimer -= Time.deltaTime;

        //    if (shotSequences >= maxSequences)
        //    {
        //        shotSequences = 0;
        //        inputsList.Add(INPUT.IN_RUN);
        //    }
        //}

        if (dieTimer > 0.0f)
        {
            dieTimer -= Time.deltaTime;

            if (dieTimer <= 0.0f)
            {
                inputsList.Add(INPUT.IN_DIE_END);
            }
        }
    }

    //All events from outside the stormtrooper
    private void ProcessExternalInput()
    {
        if (currentState != STATE.DIE && currentState != STATE.RUN)
        {
            if (InRange(player.transform.globalPosition, detectionRange))
            {
                inputsList.Add(INPUT.IN_PLAYER_IN_RANGE);

                if (player != null)
                    LookAt(player.transform.globalPosition);
                //Debug.Log("In range");
            }
        }
    }

    //Manages state changes throught inputs
    private void ProcessState()
    {
        //Debug.Log("State: " + currentState.ToString());

        while (inputsList.Count > 0)
        {
            INPUT input = inputsList[0];

            switch (currentState)
            {
                case STATE.NONE:
                    Debug.Log("CORE ERROR STATE");
                    break;

                case STATE.IDLE:
                    switch (input)
                    {
                        case INPUT.IN_WANDER:
                            currentState = STATE.WANDER;
                            StartWander();
                            break;

                        case INPUT.IN_PLAYER_IN_RANGE:
                            currentState = STATE.SHOOT;
                            StartShoot();
                            break;

                        case INPUT.IN_DIE:
                            currentState = STATE.DIE;
                            StartDie();
                            break;
                    }
                    break;

                case STATE.WANDER:
                    switch (input)
                    {
                        case INPUT.IN_IDLE:
                            currentState = STATE.IDLE;
                            StartIdle();
                            break;

                        case INPUT.IN_PLAYER_IN_RANGE:
                            currentState = STATE.SHOOT;
                            StartShoot();
                            break;

                        case INPUT.IN_DIE:
                            currentState = STATE.DIE;
                            StartDie();
                            break;
                    }
                    break;

                case STATE.RUN:
                    switch (input)
                    {
                        case INPUT.IN_IDLE:
                            currentState = STATE.IDLE;
                            StartIdle();
                            break;

                        case INPUT.IN_WANDER:
                            currentState = STATE.WANDER;
                            StartWander();
                            break;

                        case INPUT.IN_DIE:
                            currentState = STATE.DIE;
                            StartDie();
                            break;
                    }
                    break;

                case STATE.SHOOT:
                    switch (input)
                    {
                        case INPUT.IN_RUN:
                            currentState = STATE.RUN;
                            StartRun();
                            break;

                        case INPUT.IN_DIE:
                            currentState = STATE.DIE;
                            StartDie();
                            break;
                    }
                    break;

                default:
                    Debug.Log("NEED TO ADD STATE TO CORE SWITCH");
                    break;
            }
            inputsList.RemoveAt(0);
        }
    }


    private void UpdateState()
    {
        switch (currentState)
        {
            case STATE.NONE:
                break;
            case STATE.IDLE:
                break;
            case STATE.RUN:
                UpdateRun();
                break;
            case STATE.WANDER:
                UpdateWander();
                break;
            case STATE.SHOOT:
                UpdateShoot();
                break;
            case STATE.DIE:
                UpdateDie();
                break;
            default:
                Debug.Log("NEED TO ADD STATE TO CORE");
                break;
        }
    }

    #region IDLE
    private void StartIdle()
    {
        idleTimer = idleTime;
        Animator.Play(gameObject, "ST_Idle");
    }

    #endregion

    #region WANDER
    private void StartWander()
    {
        //targetPosition = CalculateNewPosition(wanderRange);

        if (agent != null)
            agent.CalculateRandomPath(gameObject.transform.globalPosition, wanderRange);
        else
            Debug.Log("Null Nav Mesh Agent");

        //ANIMATIONS OR FX AT WANDER START
        Animator.Play(gameObject, "ST_Run");
    }
    private void UpdateWander()
    {
        //LookAt(agent.GetDestination());

        //if (agent != null)
        //    agent.MoveToCalculatedPos(wanderSpeed);
        //else
        //    Debug.Log("Null agent");

        //MoveToPosition(targetPosition, wanderSpeed);
        //ANIMATIONS OR FX WHILE WANDERING
    }
    #endregion

    #region RUN
    private void StartRun()
    {
        //targetPosition = CalculateNewPosition(runningRange);

        agent.CalculateRandomPath(gameObject.transform.globalPosition, wanderRange);

        //ANIMATIONS OR FX AT RUN START
        Animator.Play(gameObject, "ST_Run");
    }
    private void UpdateRun()
    {
        LookAt(agent.GetDestination());
        agent.MoveToCalculatedPos(runningSpeed);

        //MoveToPosition(targetPosition, runningSpeed);

        //ANIMATIONS OR FX WHILE RUNNNG
    }
    #endregion

    #region SHOOT
    private void StartShoot()
    {
        //SFX LIKE AIMING OR SIMILAR + STOP RUNNING
        statesTimer = timeBewteenStates;
        Debug.Log("States timer: " + statesTimer.ToString());

        Animator.Play(gameObject, "ST_Shoot");

        Debug.Log("Shoot started");
    }

    private void UpdateShoot()
    {
        if (statesTimer > 0.0f)
        {
            statesTimer -= Time.deltaTime;

            if (statesTimer <= 0.0f)
            {
                //First Timer
                if (shotSequences == 0)
                {
                    //First Shot
                    Shoot();
                    shotTimer = timeBewteenShots;
                }
                //Second Timer
                else
                {
                    //Reboot times
                    shotTimes = 0;
                    shotSequences = 0;
                    inputsList.Add(INPUT.IN_RUN);
                    //Debug.Log("Run for your life man");
                }
            }
        }

        if (shotTimer > 0.0f)
        {
            shotTimer -= Time.deltaTime;

            if (shotTimer <= 0.0f)
            {
                Shoot();

                if (shotTimes >= maxShots)
                {
                    shotSequences++;

                    if (shotSequences < maxSequences)
                    {
                        sequenceTimer = timeBewteenSequences;
                        shotTimes = 0;
                    }
                    else
                    {
                        statesTimer = timeBewteenStates;
                    }
                }
            }
        }

        if (sequenceTimer > 0.0f)
        {
            sequenceTimer -= Time.deltaTime;

            if (sequenceTimer <= 0.0f)
            {
                Shoot();
                shotTimer = timeBewteenShots;
            }
        }
    }

    private void Shoot()
    {
        Debug.Log("Shoot");

        GameObject bullet = InternalCalls.CreatePrefab("Library/Prefabs/373530213.prefab", shootPoint.transform.globalPosition, shootPoint.transform.globalRotation, shootPoint.transform.globalScale);
        bullet.GetComponent<BH_Bullet>().damage = damage;

        Animator.Play(gameObject, "ST_Shoot");
        Audio.PlayAudio(gameObject, "PLay_Blaster_Stormtrooper");
        shotTimes++;
    }

    #endregion

    #region DIE
    private void StartDie()
    {
        dieTimer = dieTime;

        Animator.Play(gameObject, "ST_Die", 1.0f);

        Audio.PlayAudio(gameObject, "Play_Stormtrooper_Death");
        Audio.PlayAudio(gameObject, "Play_Mando_Voice");

        if (hitParticles != null)
            hitParticles.GetComponent<ParticleSystem>().Play();

        RemoveFromSpawner();


    }
    private void UpdateDie()
    {
        Counter.SumToCounterType(Counter.CounterTypes.ENEMY_STORMTROOP);
        Counter.roomEnemies--;
        Debug.Log("Enemies: " + Counter.roomEnemies.ToString());
        if (Counter.roomEnemies <= 0)
        {
            Core.instance.gameObject.GetComponent<BoonSpawn>().SpawnBoons();
        }
        player.GetComponent<PlayerHealth>().TakeDamage(-PlayerHealth.healWhenKillingAnEnemy);
        InternalCalls.Destroy(gameObject);
    }
    #endregion

    public void OnCollisionEnter(GameObject collidedGameObject)
    {
        //Debug.Log("CS: Collided object: " + gameObject.tag + ", Collider: " + collidedGameObject.tag);
        //Debug.Log("Collided by tag: " + collidedGameObject.tag);

        if (collidedGameObject.CompareTag("Bullet"))
        {
            //Debug.Log("Collision bullet");
            healthPoints -= collidedGameObject.GetComponent<BH_Bullet>().damage;

            if (Core.instance.hud != null)
            {
                Core.instance.hud.GetComponent<HUD>().AddToCombo(20, 1.0f);
            }

            if (currentState != STATE.DIE && healthPoints <= 0.0f)  //quitar STATE
            {
                inputsList.Add(INPUT.IN_DIE);
            }
        }
        else if (collidedGameObject.CompareTag("Grenade"))
        {
            Debug.Log("Collision Grenade");

            healthPoints -= collidedGameObject.GetComponent<BH_Bullet>().damage;

            if (Core.instance.hud != null)
            {
                Core.instance.hud.GetComponent<HUD>().AddToCombo(20, 0.5f);
            }

            if (currentState != STATE.DIE && healthPoints <= 0.0f)  //quitar STATE
            {
                inputsList.Add(INPUT.IN_DIE);
            }
        }
        else if (collidedGameObject.CompareTag("WorldLimit"))
        {
            Debug.Log("Collision w/ The End");

            if (currentState != STATE.DIE)  //quitar STATE
            {
                inputsList.Add(INPUT.IN_DIE);
            }
        }
    }

    public void OnTriggerEnter(GameObject triggeredGameObject)
    {
        //Debug.Log("CS: Collided object: " + gameObject.tag + ", Collider: " + triggeredGameObject.tag);
        if (triggeredGameObject.CompareTag("PushSkill") && currentState != STATE.PUSHED && currentState != STATE.DIE)
        {
            inputsList.Add(INPUT.IN_PUSHED);
        }

        //Debug.Log("Triggered by tag: " + triggeredGameObject.tag);
    }

}
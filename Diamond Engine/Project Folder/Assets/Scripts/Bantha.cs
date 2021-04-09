using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Collections.Generic;

using DiamondEngine;

public class Bantha : Enemy
{
    enum STATE: int
    {
        NONE = -1,
        IDLE,
        RUN,
        WANDER,
        LOADING_ATTACK,
        CHARGE,
        TIRED,
        PUSHED,
        HIT,
        DIE
    }

    enum INPUT : int
    {
        IN_IDLE,
        IN_RUN,
        IN_WANDER,
        IN_PUSHED,
        IN_LOADING,
        IN_CHARGE,
        IN_CHARGE_END,
        IN_HIT,
        IN_DIE,
        IN_DIE_END,
        IN_CHARGE_RANGE,
        IN_PLAYER_IN_RANGE,
        IN_RUN_END
    }

    //State
    private STATE currentState = STATE.NONE;
                                             
    private List<INPUT> inputsList = new List<INPUT>();

    public GameObject hitParticles = null;


    //Action times
    public float idleTime = 5.0f;
    private float dieTime = 3.0f;
    public float tiredTime = 2.0f;
    public float loadingTime = 2.0f;
    public float timeBewteenStates = 1.5f;

    //Speeds
    public float wanderSpeed = 3.5f;
    public float runningSpeed = 7.5f;
    public float chargeSpeed = 60.0f;

    //Ranges
    public float wanderRange = 7.5f;
    //public float runningRange = 12.5f;
    public float chargeRange = 5.0f;
    public float chargeLenght = 20.0f;

    //Timers
    private float idleTimer = 0.0f;
    private float dieTimer = 0.0f;
    private float tiredTimer = 0.0f;
    private float loadingTimer = 0.0f;
    private float chargeTimer = 0.0f;
    //private float chargeDuration = 1.0f;

    public void Awake()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
        targetPosition = null;
        loadingTime = Animator.GetAnimationDuration(gameObject, "BT_Charge");

        currentState = STATE.IDLE;
        StartIdle();
        
        dieTime = Animator.GetAnimationDuration(gameObject, "BT_Die");
    }

    public void Update()
    {
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

            if (idleTimer < 0.0f)
            {
                inputsList.Add(INPUT.IN_WANDER);
            }
        }

        if (currentState == STATE.WANDER)
        {
            if (Mathf.Distance(gameObject.transform.localPosition, agent.GetDestination()) <= agent.stoppingDistance)
            {
                inputsList.Add(INPUT.IN_IDLE);
            }
        }

        if (loadingTimer > 0.0f)
        {
            loadingTimer -= Time.deltaTime;

            if (loadingTimer < 0.0f)
            {
                inputsList.Add(INPUT.IN_CHARGE);
            }
        }

        if (currentState == STATE.CHARGE && chargeTimer > 0.0f)
        {
            chargeTimer -= Time.deltaTime;
            if (Mathf.Distance(gameObject.transform.localPosition, targetPosition) <= agent.stoppingDistance || chargeTimer < 0.0f)
            {
                inputsList.Add(INPUT.IN_CHARGE_END);
            }
        }

        if (tiredTimer > 0.0f)
        {
            tiredTimer -= Time.deltaTime;

            if (tiredTimer < 0.0f)
            {
                inputsList.Add(INPUT.IN_RUN);
            }
        }
    }

    //All events from outside the stormtrooper
    private void ProcessExternalInput()
    {
        if (currentState != STATE.DIE)
        {
            if (InRange(player.transform.globalPosition, detectionRange))
            {
                inputsList.Add(INPUT.IN_PLAYER_IN_RANGE);
              //  Debug.Log("In run range");
            }
            if (InRange(player.transform.globalPosition, chargeRange))
            {
                inputsList.Add(INPUT.IN_CHARGE_RANGE);
              //  Debug.Log("In charge range");
            }
        }
        if(currentState == STATE.RUN)
        {
            if(!InRange(player.transform.globalPosition, detectionRange))
            {
                inputsList.Add(INPUT.IN_WANDER);
            //    Debug.Log("In wander");
            }
        }

    }

    private void ProcessState()
    {
      //  Debug.Log("State: " + currentState.ToString());

        while (inputsList.Count > 0)
        {
            INPUT input = inputsList[0];

            switch (currentState)
            {
                case STATE.NONE:
                    Debug.Log("BANTHA ERROR STATE");
                    break;

                case STATE.IDLE:
                    switch (input)
                    {
                        case INPUT.IN_WANDER:
                            currentState = STATE.WANDER;
                            StartWander();
                            break;

                        case INPUT.IN_PLAYER_IN_RANGE:
                            currentState = STATE.RUN;
                            StartRun();
                            break;
                        case INPUT.IN_CHARGE_RANGE:
                            currentState = STATE.LOADING_ATTACK;
                            StartLoading();
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
                            currentState = STATE.RUN;
                            StartRun();
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

                        case INPUT.IN_CHARGE_RANGE:
                            currentState = STATE.LOADING_ATTACK;
                            StartLoading();
                            break;

                        case INPUT.IN_DIE:
                            currentState = STATE.DIE;
                            StartDie();
                            break;
                    }
                    break;

                case STATE.LOADING_ATTACK:
                    switch (input)
                    {
                        case INPUT.IN_CHARGE:
                            currentState = STATE.CHARGE;
                            StartCharge();
                            break;

                        case INPUT.IN_DIE:
                            currentState = STATE.DIE;
                            StartDie();
                            break;
                    }
                    break;

                case STATE.CHARGE:
                    switch (input)
                    {
                        case INPUT.IN_WANDER:
                            currentState = STATE.WANDER;
                            StartWander();
                            break;

                        case INPUT.IN_RUN:
                            currentState = STATE.RUN;
                            StartRun();
                            break;

                        case INPUT.IN_CHARGE_END:
                            currentState = STATE.TIRED;
                            StartTired();
                            break;

                        case INPUT.IN_DIE:
                            currentState = STATE.DIE;
                            StartDie();
                            break;
                    }
                    break;

                case STATE.TIRED:
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
                    Debug.Log("NEED TO ADD STATE TO BANTHA SWITCH");
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
            case STATE.LOADING_ATTACK:
                UpdateLoading();
                break;
            case STATE.CHARGE:
                UpdateCharge();
                break;
            case STATE.TIRED:
                break;
            case STATE.DIE:
                UpdateDie();
                break;
            default:
                Debug.Log("NEED TO ADD STATE TO BANTHA");
                break;
        }
    }

    #region IDLE
    private void StartIdle()
    {
        idleTimer = idleTime;
        Animator.Play(gameObject, "BT_Idle");
    }
    #endregion

    #region TIRED
    private void StartTired()
    {
        tiredTimer = tiredTime;
        Animator.Play(gameObject, "BT_Idle");
    }
    #endregion

    #region RUN
    private void StartRun()
    {
        Animator.Play(gameObject, "BT_Walk");
    }
    private void UpdateRun()
    {
        agent.CalculatePath(gameObject.transform.localPosition, player.transform.localPosition);
        LookAt(agent.GetDestination());
        agent.MoveToCalculatedPos(runningSpeed);
    }
    #endregion

    #region WANDER
    private void StartWander()
    {
        agent.CalculateRandomPath(gameObject.transform.globalPosition, wanderRange);
        Animator.Play(gameObject, "BT_Walk", 1.4f);

        Audio.PlayAudio(gameObject, "Play_Footsteps_Bantha");
    }
    private void UpdateWander()
    {
        LookAt(agent.GetDestination());
        agent.MoveToCalculatedPos(wanderSpeed);
    }
    #endregion

    #region LOADING_ATTACK
    private void StartLoading()
    {
        loadingTimer = loadingTime;
        Animator.Play(gameObject, "BT_Charge");
        //targetPosition = player.transform.localPosition;
        
    }
    private void UpdateLoading()
    {
        LookAt(player.transform.localPosition);
    }
    #endregion

    #region CHARGE
    private void StartCharge()
    {
        chargeTimer = chargeLenght/chargeSpeed;
        Animator.Play(gameObject, "BT_Run");

        Vector3 direction = player.transform.globalPosition - gameObject.transform.globalPosition;
        targetPosition = direction.normalized * chargeLenght + gameObject.transform.globalPosition;
        
    }
    private void UpdateCharge()
    {
        agent.CalculatePath(gameObject.transform.localPosition, targetPosition);
        LookAt(agent.GetDestination());
        
        agent.MoveToCalculatedPos(chargeSpeed);
        
    }
    #endregion

    #region DIE
    private void StartDie()
    {
        dieTimer = dieTime;

        Animator.Play(gameObject, "BT_Die", 1.0f);

        Audio.PlayAudio(gameObject, "Play_Growl_Bantha_Death");
        //Audio.PlayAudio(gameObject, "Play_Mando_Voice");

        if (hitParticles != null)
            hitParticles.GetComponent<ParticleSystem>().Play();

        RemoveFromSpawner();
    }
    private void UpdateDie()
    {
        if (dieTimer > 0.0f)
        {
            dieTimer -= Time.deltaTime;

            if (dieTimer <= 0.0f)
            {
                Die();
            }
        }
    }
    private void Die()
    {
        Counter.SumToCounterType(Counter.CounterTypes.ENEMY_BANTHA);
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

        if (collidedGameObject.CompareTag("Bullet"))
        {
            //Debug.Log("Collision bullet");
            healthPoints -= collidedGameObject.GetComponent<BH_Bullet>().damage;

            if (currentState != STATE.DIE && healthPoints <= 0.0f)  //quitar STATE
            {
                inputsList.Add(INPUT.IN_DIE);
            }
        }
        else if (collidedGameObject.CompareTag("Grenade"))
        {
            Debug.Log("Collision Grenade");

            healthPoints -= collidedGameObject.GetComponent<BH_Bullet>().damage;

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
        else if (collidedGameObject.CompareTag("Player"))
        {
            Debug.Log("Collision with player");

            if(currentState == STATE.CHARGE)
            {
                inputsList.Add(INPUT.IN_CHARGE_END);
                PlayerHealth playerHealth = collidedGameObject.GetComponent<PlayerHealth>();
                if(playerHealth!= null)
                {
                    playerHealth.TakeDamage((int)damage);
                }
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

    }
}
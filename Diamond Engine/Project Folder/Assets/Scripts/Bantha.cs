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
        IN_CHARGE,
        IN_CHARGE_END,
        IN_HIT,
        IN_DIE,
        IN_DIE_END,
        IN_CHARGE_RANGE,
        IN_PLAYER_IN_RANGE
    }

    private bool started = false;

    //State
    private STATE currentState = STATE.NONE;
                                             
    private List<INPUT> inputsList = new List<INPUT>();


    public GameObject hitParticles = null;


    //Action times
    public float idleTime = 5.0f;
    public float dieTime = 3.0f;
    public float tiredTime = 2.0f;
    public float timeBewteenStates = 1.5f;

    //Speeds
    public float wanderSpeed = 3.5f;
    public float runningSpeed = 7.5f;
    public float chargeSpeed = 60.0f;

    //Ranges
    public float wanderRange = 7.5f;
    //public float runningRange = 12.5f;
    public float chargeRange = 5.0f;

    //Timers
    private float idleTimer = 0.0f;
    private float dieTimer = 0.0f;
    private float tiredTimer = 0.0f;
    //private float chargeDuration = 1.0f;


    public void Start()
    {
        currentState = STATE.IDLE;
        Animator.Play(gameObject, "BT_Idle");
        targetPosition = CalculateNewPosition(wanderRange);
    }
    public void Update()
    {
        if (player == null)
        {
            return;
        }
        if(!started)
        {
            Start();
            started = true;
        }

        #region STATE MACHINE

        ProcessInternalInput();
        ProcessExternalInput();
        ProcessState();

        UpdateState();

        #endregion

        #region CODE


        //switch (currentState)
        //{
        //    case STATE.IDLE:
        //        Debug.Log("Idle");

        //        timePassed += Time.deltaTime;

        //        if (InRange(player.transform.globalPosition, range))
        //        {
        //            LookAt(player.transform.globalPosition);

        //            if (timePassed > idleTime)
        //            {
        //                currentState = STATE.SHOOT;
        //                Animator.Play(gameObject, "BT_Dash", 1.0f);
        //                //timePassed = timeBewteenShots;
        //            }
        //        }
        //        else
        //        {
        //            if (timePassed > idleTime)
        //            {
        //                currentState = STATE.WANDER;
        //                timePassed = 0.0f;
        //                targetPosition = CalculateNewPosition(wanderRange);
        //            }
        //        }
        //        break;

        //    case STATE.RUN:
        //        Debug.Log("Running");

        //        LookAt(player.transform.globalPosition);
        //        MoveToPosition(player.transform.localPosition, runningSpeed);

        //        // If the player is in range attack him
        //        if (InRange(player.transform.globalPosition, chargeRange))
        //        {
        //            currentState = STATE.SHOOT;
        //            //timePassed = timeBewteenShots;
        //        }

        //        if (Mathf.Distance(gameObject.transform.localPosition, player.transform.localPosition) < stoppingDistance)
        //        {
        //            currentState = STATE.IDLE;
        //            timePassed = 0.0f;
        //            Animator.Play(gameObject, "BT_Idle", 1.0f);
        //        }
        //        break;

        //    case STATE.WANDER:
        //        Debug.Log("Wander");

        //        // If the player is in range run to him
        //        if (InRange(player.transform.globalPosition, range))
        //        {
        //            currentState = STATE.RUN;
        //            Animator.Play(gameObject, "BT_Run", 1.0f);
        //            //timePassed = timeBewteenShots;
        //        }
        //        else  //if not, keep wandering
        //        {
        //            if (targetPosition == null)
        //                targetPosition = CalculateNewPosition(wanderRange);

        //            LookAt(targetPosition);
        //            MoveToPosition(targetPosition, wanderSpeed);

        //            if (Mathf.Distance(gameObject.transform.localPosition, targetPosition) < stoppingDistance)
        //            {
        //                //targetPosition = CalculateNewPosition(wanderRange);
        //                currentState = STATE.IDLE;
        //                Animator.Play(gameObject, "BT_Idle", 1.0f);
        //                timePassed = 0.0f;
        //            }
        //        }
        //        break;

        //    case STATE.SHOOT:
        //        Debug.Log("Charging");

        //        timePassed += Time.deltaTime;

        //        //LookAt(player.transform.globalPosition);

        //        //Debug.Log(player.transform.localPosition.ToString());
        //        //Debug.Log(gameObject.transform.localPosition.ToString());

        //        if (timePassed < chargeTime)
        //        {
        //            LookAt(player.transform.globalPosition);
        //        }
        //        else
        //        {
        //            if (chargeCounter < chargeDuration)
        //            {
        //                chargeCounter += Time.deltaTime;
        //                gameObject.SetVelocity(gameObject.transform.GetForward().normalized * chargeSpeed);



        //            }
        //            else
        //            {


        //                chargeCounter = 0.0f;
        //                currentState = STATE.RUN;
        //                Animator.Play(gameObject, "BT_Run", 1.0f);
        //                timePassed = 0.0f;
        //            }
        //        }

        //        if (Mathf.Distance(gameObject.transform.localPosition, player.transform.localPosition) < stoppingDistance)
        //        {
        //            chargeCounter = 0.0f;
        //            currentState = STATE.RUN;
        //            timePassed = 0.0f;
        //        }

        //        break;

        //    case STATE.HIT:
        //        Debug.Log("Being Hit");

        //        break;

        //    case STATE.DIE:
        //        Debug.Log("Dying");
        //        Counter.SumToCounterType(Counter.CounterTypes.ENEMY_BANTHA);
        //        Counter.roomEnemies--;
        //        if (Counter.roomEnemies <= 0 && Core.instance != null)
        //        {
        //            Core.instance.gameObject.GetComponent<BoonSpawn>().SpawnBoons();
        //        }
        //        InternalCalls.Destroy(gameObject);
        //        break;

        //}
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

        if (currentState == STATE.CHARGE)
        {
            if (Mathf.Distance(gameObject.transform.localPosition, player.transform.localPosition) <= stoppingDistance)
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

        if (dieTimer > 0.0f)
        {
            dieTimer -= Time.deltaTime;

            if (dieTimer <= 0.0f)
            {
                inputsList.Add(INPUT.IN_DIE);
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
                Debug.Log("In run range");
            }
            if (InRange(player.transform.globalPosition, chargeRange))
            {
                inputsList.Add(INPUT.IN_CHARGE_RANGE);
                Debug.Log("In charge range");
            }
        }

    }

    private void ProcessState()
    {
        Debug.Log("State: " + currentState.ToString());

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
                            currentState = STATE.CHARGE;
                            StartCharge();
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
                            currentState = STATE.CHARGE;
                            StartCharge();
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
        Animator.Play(gameObject, "BT_Run");
    }
    private void UpdateRun()
    {
        LookAt(player.transform.globalPosition);
        MoveToPosition(player.transform.localPosition, runningSpeed);
    }
    #endregion

    #region WANDER
    private void StartWander()
    {
        agent.CalculateRandomPath(gameObject.transform.globalPosition, wanderRange);

        Animator.Play(gameObject, "BT_Run");
    }
    private void UpdateWander()
    {
        LookAt(agent.GetDestination());
        agent.MoveToCalculatedPos(wanderSpeed);
    }
    #endregion

    #region CHARGE
    private void StartCharge()
    {
        throw new NotImplementedException();
    }
    private void UpdateCharge()
    {
        throw new NotImplementedException();
    }
    #endregion

    #region DIE
    private void StartDie()
    {
        dieTimer = dieTime;

        Animator.Play(gameObject, "BT_Die", 1.0f);

        Audio.PlayAudio(gameObject, "Play_Stormtrooper_Death");
        Audio.PlayAudio(gameObject, "Play_Mando_Voice");

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
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

    public GameObject chargePoint = null;
    public GameObject hitParticles = null;
    private GameObject visualFeedback = null;


    //Action times
    public float idleTime = 5.0f;
    private float dieTime = 3.0f;
    public float tiredTime = 2.0f;
    public float loadingTime = 2.0f;
    public float timeBewteenStates = 1.5f;
    public float directionDecisionTime = 0.8f;

    //Speeds
    public float wanderSpeed = 3.5f;
    public float runningSpeed = 7.5f;
    public float chargeSpeed = 60.0f;
    private bool skill_slowDownActive = false;

    //Ranges
    public float wanderRange = 7.5f;
    public float chargeRange = 5.0f;
    public float chargeLength = 20.0f;

    //Timers
    private float idleTimer = 0.0f;
    private float dieTimer = 0.0f;
    private float tiredTimer = 0.0f;
    private float loadingTimer = 0.0f;
    private float chargeTimer = 0.0f;
    private float directionDecisionTimer = 0.0f;
    private float skill_slowDownTimer = 0.0f;

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

        if (skill_slowDownActive)
        {
            skill_slowDownTimer += Time.deltaTime;
            if(skill_slowDownTimer >= skill_slowDownDuration)
            {
                skill_slowDownTimer = 0.0f;
                skill_slowDownActive = false;
            }
        }

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
            if (Mathf.Distance(gameObject.transform.globalPosition, agent.GetDestination()) <= agent.stoppingDistance)
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
            if (Mathf.Distance(gameObject.transform.globalPosition, targetPosition) <= agent.stoppingDistance || chargeTimer < 0.0f)
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
            }
            if (InRange(player.transform.globalPosition, chargeRange))
            {
                inputsList.Add(INPUT.IN_CHARGE_RANGE);
            }
        }
        if(currentState == STATE.RUN)
        {
            if(!InRange(player.transform.globalPosition, detectionRange))
            {
                inputsList.Add(INPUT.IN_WANDER);
            }
        }

    }

    private void ProcessState()
    {
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
                            WanderEnd();
                            StartIdle();
                            break;

                        case INPUT.IN_PLAYER_IN_RANGE:
                            currentState = STATE.RUN;
                            WanderEnd();
                            StartRun();
                            break;

                        case INPUT.IN_DIE:
                            currentState = STATE.DIE;
                            WanderEnd();
                            StartDie();
                            break;
                    }
                    break;

                case STATE.RUN:
                    switch (input)
                    {
                        case INPUT.IN_IDLE:
                            currentState = STATE.IDLE;
                            RunEnd();
                            StartIdle();
                            break;

                        case INPUT.IN_WANDER:
                            currentState = STATE.WANDER;
                            RunEnd();
                            StartWander();
                            break;

                        case INPUT.IN_CHARGE_RANGE:
                            currentState = STATE.LOADING_ATTACK;
                            RunEnd();
                            StartLoading();
                            break;

                        case INPUT.IN_DIE:
                            currentState = STATE.DIE;
                            RunEnd();
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
        Audio.StopAudio(gameObject);

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
        agent.CalculatePath(gameObject.transform.globalPosition, player.transform.globalPosition);
        LookAt(agent.GetDestination());
        if (skill_slowDownActive) agent.MoveToCalculatedPos(runningSpeed * (1 - skill_slowDownAmount));
        else agent.MoveToCalculatedPos(runningSpeed);
    }
    private void RunEnd()
    {
        Audio.StopAudio(gameObject);
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
        if (skill_slowDownActive) agent.MoveToCalculatedPos(wanderSpeed * (1 - skill_slowDownAmount));
        else agent.MoveToCalculatedPos(wanderSpeed);
    }
    private void WanderEnd()
    {
        Audio.StopAudio(gameObject);
    }
    #endregion

    #region LOADING_ATTACK
    private void StartLoading()
    {
        loadingTimer = loadingTime;
        directionDecisionTimer = directionDecisionTime;

        visualFeedback = InternalCalls.CreatePrefab("Library/Prefabs/1137197426.prefab", chargePoint.transform.globalPosition, chargePoint.transform.globalRotation, new Vector3(1.0f, 1.0f, 0.01f));
        Animator.Play(gameObject, "BT_Charge");
    }
    private void UpdateLoading()
    {
        if (directionDecisionTimer > 0.0f)
        {
            directionDecisionTimer -= Time.deltaTime;
            LookAt(player.transform.globalPosition);

            if(directionDecisionTimer < 0.1f)
            {
                Vector3 direction = player.transform.globalPosition - gameObject.transform.globalPosition;
                targetPosition = direction.normalized * chargeLength + gameObject.transform.globalPosition;
                agent.CalculatePath(gameObject.transform.globalPosition, targetPosition);
            }
        }
        if(visualFeedback.transform.globalScale.z < 1.0)
        {
            visualFeedback.transform.localScale = new Vector3(1.0f, 1.0f, Mathf.Lerp(visualFeedback.transform.localScale.z, 1.0f, Time.deltaTime * (loadingTime / loadingTimer)));
            visualFeedback.transform.localRotation = gameObject.transform.globalRotation;
        }
    }
    #endregion

    #region CHARGE
    private void StartCharge()
    {
        if (skill_slowDownActive) chargeTimer = chargeLength / (chargeSpeed * (1 - skill_slowDownAmount));
        else chargeTimer = chargeLength / chargeSpeed;

        Animator.Play(gameObject, "BT_Run");

        InternalCalls.Destroy(visualFeedback);
        Audio.PlayAudio(gameObject, "Play_Bantha_Attack");
        Audio.PlayAudio(gameObject, "Play_Bantha_Ramming");
        Audio.PlayAudio(gameObject, "Play_Footsteps_Bantha");

       
    }
    private void UpdateCharge()
    {
        LookAt(agent.GetDestination());
        
        if (skill_slowDownActive) agent.MoveToCalculatedPos(chargeSpeed * (1 - skill_slowDownAmount));
        else agent.MoveToCalculatedPos(chargeSpeed);
    }
    #endregion

    #region DIE
    private void StartDie()
    {
        //Audio.StopAudio(gameObject);

        dieTimer = dieTime;

        Animator.Play(gameObject, "BT_Die", 1.0f);

        Audio.PlayAudio(gameObject, "Play_Growl_Bantha_Death");
        //Audio.PlayAudio(gameObject, "Play_Mando_Voice");

        if (hitParticles != null)
            hitParticles.GetComponent<ParticleSystem>().Play();

        //Combo
        if (PlayerResources.CheckBoon(BOONS.BOON_MASTERYODAASSITANCE))
        {
            Core.instance.hud.GetComponent<HUD>().AddToCombo(300, 1.0f);
        }

        RemoveFromEnemyList();
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
        if(Counter.roomEnemies <= 0)
        {
            Counter.allEnemiesDead = true;
        }
        
        //Created dropped coins
        var rand = new Random();
        int droppedCoins = rand.Next(1,4);
        for (int i = 0;i < droppedCoins;i++)
        {
            Vector3 pos = gameObject.transform.globalPosition;
            pos.x += rand.Next(-200, 201) / 100;
            pos.z += rand.Next(-200, 201) / 100;
            InternalCalls.CreatePrefab(coinDropPath, pos, Quaternion.identity, new Vector3(0.07f, 0.07f, 0.07f));
        }

        player.GetComponent<PlayerHealth>().TakeDamage(-PlayerHealth.healWhenKillingAnEnemy);
        InternalCalls.Destroy(gameObject);
    }
    #endregion

    public void OnCollisionEnter(GameObject collidedGameObject)
    {

        if (collidedGameObject.CompareTag("Bullet"))
        {
            healthPoints -= collidedGameObject.GetComponent<BH_Bullet>().damage;

            Audio.PlayAudio(gameObject, "Play_Growl_Bantha_Hit");

            if (Core.instance.hud != null)
            {
                Core.instance.hud.GetComponent<HUD>().AddToCombo(20, 1.0f);
            }

            if (currentState != STATE.DIE && healthPoints <= 0.0f)
            {
                inputsList.Add(INPUT.IN_DIE);
            }

            if (skill_slowDownEnabled)
            {
                skill_slowDownActive = true;
                skill_slowDownTimer = 0.0f;
            }
        }
        else if (collidedGameObject.CompareTag("Grenade"))
        {
            smallGrenade smallGrenade = collidedGameObject.GetComponent<smallGrenade>();
          //  bigGrenade bigGrenade = collidedGameObject.GetComponent<bigGrenade>();

            if (smallGrenade != null)
                healthPoints -= smallGrenade.damage;

            //if (bigGrenade != null)
            //    healthPoints -= bigGrenade.damage;

            if (Core.instance.hud != null)
            {
                Core.instance.hud.GetComponent<HUD>().AddToCombo(20, 0.5f);
            }

            if (currentState != STATE.DIE && healthPoints <= 0.0f)
            {
                inputsList.Add(INPUT.IN_DIE);
            }

            if (skill_slowDownEnabled)
            {
                skill_slowDownActive = true;
                skill_slowDownTimer = 0.0f;
            }                           
        }
        else if (collidedGameObject.CompareTag("WorldLimit"))
        {
            if (currentState != STATE.DIE)
            {
                inputsList.Add(INPUT.IN_DIE);
            }
        }
        else if (collidedGameObject.CompareTag("Player"))
        {
            if (currentState == STATE.CHARGE)
            {
                inputsList.Add(INPUT.IN_CHARGE_END);
                PlayerHealth playerHealth = collidedGameObject.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage((int)damage);
                }
            }
        }
    }

    public void OnTriggerEnter(GameObject triggeredGameObject)
    {
        if (triggeredGameObject.CompareTag("PushSkill") && currentState != STATE.PUSHED && currentState != STATE.DIE)
        {
            inputsList.Add(INPUT.IN_PUSHED);
        }
    }
}
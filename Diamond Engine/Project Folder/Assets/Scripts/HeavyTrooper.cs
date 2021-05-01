using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Collections.Generic;

using DiamondEngine;

public class HeavyTrooper : Enemy
{
    enum STATE : int
    {
        NONE = -1,
        IDLE,
        WANDER,
        RUN,
        DASH,
        SWEEP,
        TIRED,
        PUSHED,
        HIT,
        DIE
    }

    enum INPUT : int
    {
        IN_IDLE,
        IN_RUN,
        IN_DASH,
        IN_DASH_END,
        IN_WANDER,
        IN_PUSHED,
        IN_SWEEP,
        IN_SWEEP_END,
        IN_HIT,
        IN_DIE,
        IN_DIE_END,
        IN_DASH_RANGE,
        IN_PLAYER_IN_RANGE
    }

    //State
    private STATE currentState = STATE.NONE;

    private List<INPUT> inputsList = new List<INPUT>();

    public GameObject chargePoint = null;
    public GameObject hitParticles = null;
    private GameObject visualFeedback = null;

    private bool straightPath = false;

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
    public float dashSpeed = 60.0f;
    public float dashSpeedReduction = 0.3f;
    private bool skill_slowDownActive = false;

    //Ranges
    public float wanderRange = 7.5f;
    public float attackRange = 10.0f;
    public float dashRange = 5.0f;
    public float dashLength = 20.0f;

    //Timers
    private float idleTimer = 0.0f;
    private float dieTimer = 0.0f;
    private float tiredTimer = 0.0f;
    private float directionDecisionTimer = 0.0f;
    private float skill_slowDownTimer = 0.0f;
    private float pushTimer = 0.0f;

    //force
    public float forcePushMod = 1;

    //Particles
    public GameObject stunParticle = null;
    private ParticleSystem stun = null;

    public void Awake()
    {
        InitEntity(ENTITY_TYPE.HEAVYTROOPER);

        StartIdle();
        agent = gameObject.GetComponent<NavMeshAgent>();
        targetPosition = null;

        currentState = STATE.IDLE;

        loadingTime = Animator.GetAnimationDuration(gameObject, "BT_Charge");
        dieTime = Animator.GetAnimationDuration(gameObject, "BT_Die");

        if (stunParticle != null)
            stun = stunParticle.GetComponent<ParticleSystem>();
    }

    public void Update()
    {
        if (player == null)
        {
            Debug.Log("Null player");
            player = Core.instance.gameObject;
        }

        myDeltaTime = Time.deltaTime * speedMult;
        UpdateStatuses();

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
            if (Mathf.Distance(gameObject.transform.globalPosition, agent.GetDestination()) <= agent.stoppingDistance)
            {
                inputsList.Add(INPUT.IN_IDLE);
            }
        }

        

        if (skill_slowDownActive)
        {
            skill_slowDownTimer += Time.deltaTime;

            if (skill_slowDownTimer >= Skill_Tree_Data.GetWeaponsSkillTree().PW3_SlowDownDuration) //Get duration from Primary Weapon Skill 4
            {
                skill_slowDownTimer = 0.0f;
                skill_slowDownActive = false;
            }
        }
    }

    //All events from outside the stormtrooper
    private void ProcessExternalInput()
    {
       
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

                        case INPUT.IN_DIE:
                            currentState = STATE.DIE;
                            StartDie();
                            break;

                        case INPUT.IN_PUSHED:
                            currentState = STATE.PUSHED;
                            StartPush();
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

                        case INPUT.IN_PUSHED:
                            currentState = STATE.PUSHED;
                            StartPush();
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

                        case INPUT.IN_DASH_RANGE:
                            currentState = STATE.DASH;
                            RunEnd();
                            StartDash();
                            break;

                        case INPUT.IN_DIE:
                            currentState = STATE.DIE;
                            RunEnd();
                            StartDie();
                            break;

                        case INPUT.IN_PUSHED:
                            currentState = STATE.PUSHED;
                            StartPush();
                            break;
                    }
                    break;

                case STATE.DASH:
                    switch (input)
                    {
                        case INPUT.IN_WANDER:
                            currentState = STATE.WANDER;
                            StartWander();
                            break;

                        case INPUT.IN_DASH_END:
                            currentState = STATE.RUN;
                            StartRun();
                            break;

                        case INPUT.IN_SWEEP:
                            currentState = STATE.SWEEP;
                            StartSweep();
                            break;

                        case INPUT.IN_DIE:
                            currentState = STATE.DIE;
                            StartDie();
                            break;

                        case INPUT.IN_PUSHED:
                            currentState = STATE.PUSHED;
                            StartPush();
                            break;
                    }
                    break;

                case STATE.SWEEP:
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

                        case INPUT.IN_SWEEP_END:
                            currentState = STATE.RUN;
                            StartRun();
                            break;

                        case INPUT.IN_DIE:
                            currentState = STATE.DIE;
                            StartDie();
                            break;

                        case INPUT.IN_PUSHED:
                            currentState = STATE.PUSHED;
                            StartPush();
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

                        case INPUT.IN_PUSHED:
                            currentState = STATE.PUSHED;
                            StartPush();
                            break;
                    }
                    break;

                case STATE.PUSHED:
                    switch (input)
                    {
                        case INPUT.IN_DIE:
                            currentState = STATE.DIE;
                            StartDie();
                            break;

                        case INPUT.IN_IDLE:
                            currentState = STATE.IDLE;
                            StartIdle();
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
            case STATE.DASH:
                UpdateDash();
                break;
            case STATE.SWEEP:
                UpdateSweep();
                break;
            case STATE.TIRED:
                break;
            case STATE.DIE:
                UpdateDie();
                break;
            case STATE.PUSHED:
                UpdatePush();
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

    #region RUN
    private void StartRun()
    {
        
    }
    private void UpdateRun()
    {
       
    }
    private void RunEnd()
    {
       
    }
    #endregion

    #region WANDER
    private void StartWander()
    {
        
    }
    private void UpdateWander()
    {
       
    }
    private void WanderEnd()
    {
       
    }
    #endregion

    #region DASH
    private void StartDash()
    {
       
    }
    private void UpdateDash()
    {
        
    }
    #endregion

    #region SWEEP
    private void StartSweep()
    {
        
    }
    private void UpdateSweep()
    {
        
    }
    #endregion

    #region TIRED
    private void StartTired()
    {
        Audio.StopAudio(gameObject);

        tiredTimer = tiredTime;
        Animator.Play(gameObject, "BT_Idle");
        Audio.PlayAudio(gameObject, "Play_Bantha_Breath");
        stun.Play();
    }
    #endregion

    #region DIE
    private void StartDie()
    {
        
    }
    private void UpdateDie()
    {
       
    }
    private void Die()
    {
        
    }
    #endregion

    #region PUSH
    private void StartPush()
    {
        Vector3 force = gameObject.transform.globalPosition - player.transform.globalPosition;
        if (BabyYoda.instance != null)
        {
            force.y = BabyYoda.instance.pushVerticalForce * forcePushMod;
            gameObject.AddForce(force * BabyYoda.instance.pushHorizontalForce * forcePushMod);
            pushTimer = 0.0f;
        }

    }
    private void UpdatePush()
    {
        pushTimer += Time.deltaTime;
        if (BabyYoda.instance != null)
        {
            if (pushTimer >= BabyYoda.instance.PushStun)
                inputsList.Add(INPUT.IN_IDLE);
        }
        else
        {
            inputsList.Add(INPUT.IN_IDLE);
        }

    }
    #endregion

    public void OnCollisionEnter(GameObject collidedGameObject)
    {

        
    }

    public void OnTriggerEnter(GameObject triggeredGameObject)
    {
        
    }

    public override void TakeDamage(float damage)
    {
        healthPoints -= damage;

        if (currentState != STATE.DIE)
        {
            if (healthPoints <= 0.0f)
                inputsList.Add(INPUT.IN_DIE);
        }

    }

    public void StraightPath()
    {
        if (Vector2.Dot(agent.GetLastVector().ToVector2(), (agent.GetDestination() - gameObject.transform.localPosition).ToVector2()) > 0.9f)
        {
            straightPath = true;
        }
        else
        {
            straightPath = false;
        }
        Debug.Log("StraightPath: " + straightPath);
    }
}
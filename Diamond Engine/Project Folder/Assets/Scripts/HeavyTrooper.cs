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
        LOADING_DASH,
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
        IN_LOADING_DASH,
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

    public  GameObject  spear = null;
    private BoxCollider spearCollider = null;
    public  GameObject  hitParticles = null;
    private GameObject  visualFeedback = null;

    private bool straightPath = false;

    //Action times
    public  float  idleTime = 5.0f;
    private float  dieTime = 3.0f;
    private float  sweepTime = 0.0f;
    public  float  tiredTime = 2.0f;
    public  float  loadingTime = 2.0f;
    public  float  directionDecisionTime = 0.8f;
    public  float  directionSweepDecisionTime = 0.8f;

    //Speeds
    public  float wanderSpeed = 3.5f;
    public  float runningSpeed = 7.5f;
    public  float dashSpeed = 60.0f;
    public  float dashSpeedReduction = 0.3f;
    private bool  skill_slowDownActive = false;

    //Ranges
    public float wanderRange = 7.5f;
    public float sweepRange = 3.0f;
    public float dashRange = 5.0f;
    public float dashLength = 20.0f;

    //Timers
    private float idleTimer = 0.0f;
    private float dashTimer = 0.0f;
    private float sweepTimer = 0.0f;
    private float dieTimer = 0.0f;
    private float tiredTimer = 0.0f;
    private float loadingTimer = 0.0f;
    private float directionDecisionTimer = 0.0f;
    private float directionSweepDecisionTimer = 0.0f;
    private float skill_slowDownTimer = 0.0f;
    private float pushTimer = 0.0f;
    private float currAnimationPlaySpd = 1f;

    int doneDashes = 0;

    //force
    public float forcePushMod = 1;

    //Particles
    public GameObject stunParticle = null;
    private ParticleSystem stun = null;

    public void Awake()
    {
        InitEntity(ENTITY_TYPE.HEAVYTROOPER);
        EnemyManager.AddEnemy(gameObject);

        StartIdle();
        agent = gameObject.GetComponent<NavMeshAgent>();
        targetPosition = null;

        currentState = STATE.IDLE;

        if(spear != null)
        {
            HeavyTrooperSpear heavyTrooperSpear = spear.GetComponent<HeavyTrooperSpear>();

            heavyTrooperSpear.damage = (int)damage;

            spearCollider = spear.GetComponent<BoxCollider>();
        }

        sweepTime = Animator.GetAnimationDuration(gameObject, "HVY_Sweep");
        dieTime = Animator.GetAnimationDuration(gameObject, "HVY_Die");

        if (stunParticle != null)
            stun = stunParticle.GetComponent<ParticleSystem>();
    }

    public void Update()
    {
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
            idleTimer -= myDeltaTime;

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

        if (currentState == STATE.DASH && dashTimer > 0.0f)
        {
            dashTimer -= myDeltaTime;
            if (Mathf.Distance(gameObject.transform.globalPosition, targetPosition) <= agent.stoppingDistance || dashTimer < 0.0f)
            {
                doneDashes++;
                //Debug.Log("Dashes++");

                if(Mathf.Distance(Core.instance.gameObject.transform.globalPosition, gameObject.transform.globalPosition) > dashRange 
                   || Mathf.Distance(Core.instance.gameObject.transform.globalPosition, gameObject.transform.globalPosition) > sweepRange && doneDashes < 2)
                {
                    inputsList.Add(INPUT.IN_LOADING_DASH);
                }
                else
                {
                    inputsList.Add(INPUT.IN_DASH_END);
                }
            }
        }

        if (currentState == STATE.SWEEP && sweepTimer > 0.0f)
        {
            sweepTimer -= myDeltaTime;
            if (sweepTimer < 0.0f)
            {
                inputsList.Add(INPUT.IN_SWEEP_END);
            }
        }

        if (loadingTimer > 0.0f)
        {
            loadingTimer -= myDeltaTime;

            if (loadingTimer < 0.0f)
            {
                inputsList.Add(INPUT.IN_DASH);
            }
        }

        if (tiredTimer > 0.0f)
        {
            tiredTimer -= myDeltaTime;

            LookAt(Core.instance.gameObject.transform.globalPosition);

            if (tiredTimer < 0.0f)
            {
                inputsList.Add(INPUT.IN_RUN);
            }
        }

        if (skill_slowDownActive)
        {
            skill_slowDownTimer += myDeltaTime;

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
        if (currentState != STATE.DIE && currentState != STATE.DASH)
        {
            if (InRange(Core.instance.gameObject.transform.globalPosition, detectionRange) && agent.CalculatePath(gameObject.transform.globalPosition, Core.instance.gameObject.transform.globalPosition))
            {
                inputsList.Add(INPUT.IN_PLAYER_IN_RANGE);
            }
            if (InRange(Core.instance.gameObject.transform.globalPosition, dashRange) && straightPath)
            {
                inputsList.Add(INPUT.IN_DASH_RANGE);
            }
        }

        if (currentState == STATE.RUN)
        {
            if (agent == null)
            {
                inputsList.Add(INPUT.IN_IDLE);
                Debug.Log("My agent is null :)");
            }

            if (!InRange(Core.instance.gameObject.transform.globalPosition, detectionRange))
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
                            currentState = STATE.LOADING_DASH;
                            RunEnd();
                            StartLoading();
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

                case STATE.LOADING_DASH:
                    switch (input)
                    {
                        case INPUT.IN_DASH:
                            currentState = STATE.DASH;
                            StartDash();
                            break;

                        case INPUT.IN_PUSHED:
                            currentState = STATE.PUSHED;
                            StartPush();
                            break;

                        case INPUT.IN_DIE:
                            currentState = STATE.DIE;
                            StartDie();
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

                        case INPUT.IN_LOADING_DASH:
                            currentState = STATE.LOADING_DASH;
                            StartLoading();
                            break;

                        case INPUT.IN_DASH_END:
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
                            currentState = STATE.TIRED;
                            StartTired();
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
                UpdateIdle();
                break;
            case STATE.RUN:
                UpdateRun();
                break;
            case STATE.WANDER:
                UpdateWander();
                break;
            case STATE.LOADING_DASH:
                UpdateLoading();
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
        //Debug.Log("HEAVYTROOPER IDLE");
        idleTimer = idleTime;
        Animator.Play(gameObject, "HVY_Idle", speedMult);
        Animator.Play(spear, "HVY_Idle", speedMult);
        UpdateAnimationSpd(speedMult);
    }

    private void UpdateIdle()
    {
        UpdateAnimationSpd(speedMult);
    }

    #endregion

    #region RUN
    private void StartRun()
    {
        //Debug.Log("HEAVYTROOPER RUN");
        Animator.Play(gameObject, "HVY_Run", speedMult);
        Animator.Play(spear, "HVY_Run", speedMult);
        UpdateAnimationSpd(speedMult);
        Audio.PlayAudio(gameObject, "Play_Heavytrooper_Run");
        Audio.PlayAudio(gameObject, "Play_Heavytrooper_Enemy_Detection");
    }
    private void UpdateRun()
    {
        if (Core.instance != null)
        {
            if (agent.CalculatePath(gameObject.transform.globalPosition, Core.instance.gameObject.transform.globalPosition) == false)
            {
                inputsList.Add(INPUT.IN_IDLE);
                return;
            }
        }

        LookAt(agent.GetDestination());
        //if (skill_slowDownActive)
        //    agent.MoveToCalculatedPos(runningSpeed * (1 - Skill_Tree_Data.GetWeaponsSkillTree().PW3_SlowDownAmount));

        agent.MoveToCalculatedPos(runningSpeed * speedMult);

        StraightPath();

        UpdateAnimationSpd(speedMult);
    }
    private void RunEnd()
    {
        Audio.StopAudio(gameObject);
    }
    #endregion

    #region WANDER
    private void StartWander()
    {
        //Debug.Log("HEAVYTROOPER WANDER");
        agent.CalculateRandomPath(gameObject.transform.globalPosition, wanderRange);

        Animator.Play(gameObject, "HVY_Wander", speedMult);
        Animator.Play(spear, "HVY_Wander", speedMult);

        UpdateAnimationSpd(speedMult);
        Audio.PlayAudio(gameObject, "Play_Heavytrooper_Wander");
    }
    private void UpdateWander()
    {
        LookAt(agent.GetDestination());
        //if (skill_slowDownActive) 
        //    agent.MoveToCalculatedPos(wanderSpeed * (1 - Skill_Tree_Data.GetWeaponsSkillTree().PW3_SlowDownAmount));

        agent.MoveToCalculatedPos(wanderSpeed * speedMult);

        UpdateAnimationSpd(speedMult);
    }
    private void WanderEnd()
    {
        Audio.StopAudio(gameObject);
    }
    #endregion

    #region LOADING_ATTACK
    private void StartLoading()
    {
        Debug.Log("HEAVYTROOPER LOADING");
        loadingTimer = loadingTime * (doneDashes > 0 ? 0.1f : 1.0f);
        Debug.Log("Loading timer: " + loadingTimer.ToString());
        directionDecisionTimer = directionDecisionTime;

        //visualFeedback = InternalCalls.CreatePrefab("Library/Prefabs/1137197426.prefab", chargePoint.transform.globalPosition, chargePoint.transform.globalRotation, new Vector3(1.0f, 1.0f, 0.01f));
        //Animator.Play(gameObject, "BT_Charge", speedMult);
        UpdateAnimationSpd(speedMult);
    }
    private void UpdateLoading()
    {
        if (directionDecisionTimer > 0.0f)
        {
            //Debug.Log("HEAVYTROOPER LOADING ENTRY 1");
            directionDecisionTimer -= myDeltaTime;
            LookAt(Core.instance.gameObject.transform.globalPosition);

            if (directionDecisionTimer < 0.1f)
            {
                //Debug.Log("HEAVYTROOPER LOADING ENTRY 2");
                Vector3 direction = Core.instance.gameObject.transform.globalPosition - gameObject.transform.globalPosition;
                targetPosition = direction.normalized * dashLength + gameObject.transform.globalPosition;
                agent.CalculatePath(gameObject.transform.globalPosition, targetPosition);
            }
        }
        //if (visualFeedback.transform.globalScale.z < 1.0)
        //{
        //    visualFeedback.transform.localScale = new Vector3(1.0f, 1.0f, Mathf.Lerp(visualFeedback.transform.localScale.z, 1.0f, myDeltaTime * (loadingTime / loadingTimer)));
        //    visualFeedback.transform.localRotation = gameObject.transform.globalRotation;
        //}

        UpdateAnimationSpd(speedMult);
    }
    #endregion

    #region DASH
    private void StartDash()
    {
        Debug.Log("HEAVYTROOPER DASH");
        //Animator.Play(gameObject, "BT_Walk", speedMult);
        if (!straightPath)
            dashTimer = (dashLength * 0.5f / (dashSpeed * dashSpeedReduction)) * speedMult;
        else 
            dashTimer = (dashLength * 0.5f / dashSpeed) * speedMult;

        StraightPath();

        UpdateAnimationSpd(speedMult);

        Audio.PlayAudio(gameObject, "Play_Heavytrooper_Dash");

        Debug.Log("Done dashes: " + doneDashes.ToString());
    }
    private void UpdateDash()
    {
        if (!straightPath)
            agent.MoveToCalculatedPos(dashSpeed * dashSpeedReduction * speedMult);
        else
            agent.MoveToCalculatedPos(dashSpeed * speedMult);



        UpdateAnimationSpd(speedMult);
    }
    #endregion

    #region SWEEP
    private void StartSweep()
    {
        Debug.Log("HEAVYTROOPER SWEEP");
        sweepTimer = sweepTime;

        Animator.Play(gameObject, "HVY_Sweep", speedMult);
        
        if(spear != null)
            Animator.Play(spear, "HVY_Sweep", speedMult);

        Audio.PlayAudio(gameObject, "Play_Heavytrooper_Attack");
    }
    private void UpdateSweep()
    {
        if (directionSweepDecisionTimer > 0.0f)
        {
            directionSweepDecisionTimer -= myDeltaTime;
            LookAt(Core.instance.gameObject.transform.globalPosition);
        }

        if(spearCollider != null)
        {
            if(sweepTimer < sweepTime * 0.33f && spearCollider.active) // Stop doing damage at the last third of the animation
            {
                spearCollider.active = false;
            }
            else if(sweepTimer < sweepTime * 0.75f && !spearCollider.active) //Start doing damage at the first fourth of the animation
            {
                spearCollider.active = true;
            }
        }

        UpdateAnimationSpd(speedMult);
    }

    #endregion

    #region TIRED
    private void StartTired()
    {
        Debug.Log("HEAVYTROOPER TIRED");
        tiredTimer = tiredTime;

        //Audio.StopAudio(gameObject);

        LookAt(Core.instance.gameObject.transform.globalPosition);
        UpdateAnimationSpd(speedMult);
        Animator.Play(gameObject, "ST_Idle",speedMult);
        //Audio.PlayAudio(gameObject, "Play_Bantha_Breath");
        //stun.Play();
    }
    #endregion

    #region DIE
    private void StartDie()
    {
        //Audio.StopAudio(gameObject);

        if (visualFeedback != null)
            InternalCalls.Destroy(visualFeedback);

        dieTimer = dieTime;

        Animator.Play(gameObject, "HVY_Die", speedMult);
        Animator.Play(spear, "HVY_Die", speedMult);
        UpdateAnimationSpd(speedMult);

        Audio.PlayAudio(gameObject, "Play_Heavytrooper_Death");

        if (hitParticles != null)
            hitParticles.GetComponent<ParticleSystem>().Play();

        //Combo
        if (PlayerResources.CheckBoon(BOONS.BOON_MASTERYODAASSITANCE))
        {
            Core.instance.hud.GetComponent<HUD>().AddToCombo(300, 1.0f);
        }
    }
    private void UpdateDie()
    {
        if (dieTimer > 0.0f)
        {
            dieTimer -= myDeltaTime;

            if (dieTimer <= 0.0f)
            {
                Die();
            }
        }

        UpdateAnimationSpd(speedMult);
    }
    private void Die()
    {
        Counter.SumToCounterType(Counter.CounterTypes.ENEMY_HEAVYTROOPER);
        EnemyManager.RemoveEnemy(gameObject);

        DropCoins();

        Core.instance.gameObject.GetComponent<PlayerHealth>().TakeDamage(-PlayerHealth.healWhenKillingAnEnemy);

        InternalCalls.Destroy(gameObject);
    }
    #endregion

    #region PUSH
    private void StartPush()
    {
        Vector3 force = gameObject.transform.globalPosition - Core.instance.gameObject.transform.globalPosition;
        if (BabyYoda.instance != null)
        {
            force.y = BabyYoda.instance.pushVerticalForce * Time.deltaTime;
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
        if (collidedGameObject.CompareTag("Bullet"))
        {
            BH_Bullet bullet = collidedGameObject.GetComponent<BH_Bullet>();

            if (bullet != null)
            {
                TakeDamage(bullet.damage);

                Audio.PlayAudio(gameObject, "Play_Stormtrooper_Hit");

                if (Core.instance.hud != null)
                {
                    HUD hudComponent = Core.instance.hud.GetComponent<HUD>();

                    if (hudComponent != null)
                        hudComponent.AddToCombo(25, 0.95f);
                }

                if (Skill_Tree_Data.IsEnabled((int)Skill_Tree_Data.SkillTreesNames.WEAPONS, (int)Skill_Tree_Data.WeaponsSkillNames.PRIMARY_SLOW_SPEED))
                {
                    skill_slowDownActive = true;
                    skill_slowDownTimer = 0.0f;
                }
            }
        }
        else if (collidedGameObject.CompareTag("ChargeBullet"))
        {
            ChargedBullet bullet = collidedGameObject.GetComponent<ChargedBullet>();

            if (bullet != null)
            {
                healthPoints -= bullet.damage;
                this.AddStatus(STATUS_TYPE.DAMAGE_DOWN, STATUS_APPLY_TYPE.BIGGER_PERCENTAGE, 0.5f, 3.5f);

                TakeDamage(bullet.damage);

                Audio.PlayAudio(gameObject, "Play_Stormtrooper_Hit");

                if (Core.instance.hud != null)
                {
                    Core.instance.hud.GetComponent<HUD>().AddToCombo(55, 0.25f);
                }

                if (currentState != STATE.DIE && healthPoints <= 0.0f)
                {
                    inputsList.Add(INPUT.IN_DIE);
                }
            }
        }
        else if (collidedGameObject.CompareTag("ExplosiveBarrel") && collidedGameObject.GetComponent<SphereCollider>().active)
        {
            //if (myParticles != null && myParticles.hit != null)
            //    myParticles.hit.Play();
            BH_DestructBox explosion = collidedGameObject.GetComponent<BH_DestructBox>();

            if (explosion != null)
            {
                healthPoints -= explosion.explosion_damage * 2;
                if (currentState != STATE.DIE && healthPoints <= 0.0f)
                    inputsList.Add(INPUT.IN_DIE);
            }

        }
    }

    public void OnTriggerEnter(GameObject triggeredGameObject)
    {
        if (triggeredGameObject.CompareTag("PushSkill") && currentState != STATE.PUSHED && currentState != STATE.DIE)
        {
            if (Core.instance.gameObject != null)
            {
                inputsList.Add(INPUT.IN_PUSHED);
            }
        }
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
        //Debug.Log("StraightPath: " + straightPath);
    }

    private void UpdateAnimationSpd(float newSpd)
    {
        if (currAnimationPlaySpd != newSpd)
        {
            Animator.SetSpeed(gameObject, newSpd);
            Animator.SetSpeed(spear, newSpd);
            currAnimationPlaySpd = newSpd;
        }
    }
}
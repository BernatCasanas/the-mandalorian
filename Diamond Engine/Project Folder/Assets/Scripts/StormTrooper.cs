using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Collections.Generic;

using DiamondEngine;

public class StormTrooper : Enemy
{
    enum STATE : int
    {
        NONE = -1,
        IDLE,
        WANDER,
        RUN,
        PUSHED,
        FIND_AIM,
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
        IN_FIND_AIM,
        IN_SHOOT,
        IN_HIT,
        IN_DIE,
        IN_PLAYER_IN_RANGE,
        IN_PLAYER_SHOOTABLE
    }

    //State
    private STATE currentState = STATE.NONE;

    private List<INPUT> inputsList = new List<INPUT>();

    public GameObject shootPoint = null;
    public GameObject blaster = null;

    //Action times
    public float idleTime = 5.0f;
    private float dieTime = 3.0f;
    public float timeBetweenShots = 0.5f;
    public float timeBetweenSequences = 0.5f;
    public float timeBetweenStates = 1.5f;
    //private float reAimTime = 0.5f;

    //Speeds
    public float wanderSpeed = 3.5f;
    public float runningSpeed = 7.5f;
    public float bulletSpeed = 10.0f;
    private bool skill_slowDownActive = false;
    private float currAnimationPlaySpd = 1f;

    //Ranges
    public float wanderRange = 7.5f;
    public float runningRange = 12.5f;
    public float avoidRange = 15f;

    //Timers
    private float idleTimer = 0.0f;
    private float shotTimer = 0.0f;
    private float sequenceTimer = 0.0f;
    private float dieTimer = 0.0f;
    private float statesTimer = 0.0f;
    private float pushTimer = 0.0f;
    //private float reAimTimer = 0.0f;
    private float skill_slowDownTimer = 0.0f;

    //Action variables
    int shotTimes = 0;
    public int maxShots = 2;
    private int shotSequences = 0;
    public int maxSequences = 2;
    private bool shooting = false;
    private bool needFindAim = false;

    //force
    public float forcePushMod = 1;

    //Death point
    public GameObject deathPoint = null;

    private StormTrooperParticles myParticles = null;
    public void Awake()
    {
        Debug.Log("Stormtrooper Awake");

        InitEntity(ENTITY_TYPE.STROMTROOPER);
        EnemyManager.AddEnemy(gameObject);

        agent = gameObject.GetComponent<NavMeshAgent>();
        targetPosition = null;

        currentState = STATE.IDLE;

        Animator.Play(gameObject, "ST_Idle", 1.0f);

        if (blaster != null)
            Animator.Play(blaster, "ST_Idle", 1.0f);

        UpdateAnimationSpd(1f);

        shotTimes = 0;
        shotSequences = 0;

        idleTimer = idleTime;
        dieTime = Animator.GetAnimationDuration(gameObject, "ST_Die");

        myParticles = gameObject.GetComponent<StormTrooperParticles>();
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

            if (idleTimer <= 0.0f)
            {
                inputsList.Add(INPUT.IN_WANDER);
            }
        }

        if (currentState == STATE.RUN || currentState == STATE.WANDER)
        {
            if (Mathf.Distance(gameObject.transform.globalPosition, agent.GetDestination()) <= agent.stoppingDistance)
            {
                inputsList.Add(INPUT.IN_IDLE);
            }
        }

        if (currentState == STATE.FIND_AIM)
        {
            if (Mathf.Distance(gameObject.transform.globalPosition, agent.GetDestination()) <= agent.stoppingDistance)
            {
                inputsList.Add(INPUT.IN_SHOOT);
            }
        }

        /*if (skill_slowDownActive)
        {
            skill_slowDownTimer += myDeltaTime;
            if (skill_slowDownTimer >= Skill_Tree_Data.GetWeaponsSkillTree().PW3_SlowDownDuration)
            {
                skill_slowDownTimer = 0.0f;
                skill_slowDownActive = false;
            }
        }*/
    }

    //All events from outside the stormtrooper
    private void ProcessExternalInput()
    {
        if (currentState != STATE.DIE && currentState != STATE.RUN)
        {
            if (InRange(Core.instance.gameObject.transform.globalPosition, detectionRange))
            {
                inputsList.Add(INPUT.IN_PLAYER_IN_RANGE);

                //if (Core.instance != null && currentState != STATE.SHOOT)
                //    LookAt(Core.instance.gameObject.transform.globalPosition);
            }
        }

        if(currentState == STATE.RUN && needFindAim)
        {
            inputsList.Add(INPUT.IN_FIND_AIM);
        }
    }

    //Manages state changes throught inputs
    private void ProcessState()
    {
        while (inputsList.Count > 0)
        {
            INPUT input = inputsList[0];

            switch (currentState)
            {
                case STATE.NONE:
                    Debug.Log("STORMTROOPER ERROR STATE");
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
                            PlayerDetected();
                            StartShoot();
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
                            currentState = STATE.SHOOT;
                            WanderEnd();
                            PlayerDetected();
                            StartShoot();
                            break;

                        case INPUT.IN_DIE:
                            currentState = STATE.DIE;
                            WanderEnd();
                            StartDie();
                            break;

                        case INPUT.IN_PUSHED:
                            currentState = STATE.PUSHED;
                            WanderEnd();
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

                        case INPUT.IN_FIND_AIM:
                            currentState = STATE.FIND_AIM;
                            RunEnd();
                            StartFindAim();
                            break;

                        case INPUT.IN_DIE:
                            currentState = STATE.DIE;
                            RunEnd();
                            StartDie();
                            break;

                        case INPUT.IN_PUSHED:
                            currentState = STATE.PUSHED;
                            RunEnd();
                            StartPush();
                            break;
                    }
                    break;

                case STATE.FIND_AIM:
                    switch (input)
                    {
                        case INPUT.IN_SHOOT:
                            currentState = STATE.SHOOT;
                            FindAimEnd();
                            StartShoot();
                            break;

                        case INPUT.IN_PUSHED:
                            currentState = STATE.PUSHED;
                            FindAimEnd();
                            StartPush();
                            break;

                        case INPUT.IN_DIE:
                            currentState = STATE.DIE;
                            FindAimEnd();
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
                    Debug.Log("NEED TO ADD STATE TO STORMTROOPER SWITCH");
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
            case STATE.FIND_AIM:
                UpdateFindAim();
                break;
            case STATE.SHOOT:
                UpdateShoot();
                break;
            case STATE.DIE:
                UpdateDie();
                break;
            case STATE.PUSHED:
                UpdatePush();
                break;
            default:
                Debug.Log("NEED TO ADD STATE TO STORMTROOPER");
                break;
        }
    }

    #region IDLE
    private void StartIdle()
    {
        idleTimer = idleTime;
        Animator.Play(gameObject, "ST_Idle", speedMult);
        if (blaster != null)
            Animator.Play(blaster, "ST_Idle", speedMult);

        UpdateAnimationSpd(speedMult);

    }

    private void UpdateIdle()
    {
        UpdateAnimationSpd(speedMult);
    }

    #endregion

    #region WANDER
    private void StartWander()
    {
        agent.CalculateRandomPath(gameObject.transform.globalPosition, wanderRange);

        Animator.Play(gameObject, "ST_Run", speedMult);
        if (blaster != null)
            Animator.Play(blaster, "ST_Idle", speedMult);
        UpdateAnimationSpd(speedMult);
        Audio.PlayAudio(gameObject, "Play_Footsteps_Stormtrooper");
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

    #region RUN
    private void StartRun()
    {
        //Debug.Log("STORMTROOPER RUN");
        agent.CalculateRandomPath(gameObject.transform.globalPosition, runningRange);

        Animator.Play(gameObject, "ST_Run", speedMult);
        if (blaster != null)
            Animator.Play(blaster, "ST_Run", speedMult);
        UpdateAnimationSpd(speedMult);
        Audio.PlayAudio(gameObject, "Play_Footsteps_Stormtrooper");
    }

    private void UpdateRun()
    {
        LookAt(agent.GetDestination());

        agent.MoveToCalculatedPos(runningSpeed * speedMult);

        UpdateAnimationSpd(speedMult);
    }
    private void RunEnd()
    {
        Audio.StopAudio(gameObject);
        
        PlayerIsShootable();
    }
    #endregion

    #region FIND_AIM

    private void StartFindAim()
    {
        //Debug.Log("STORMTROOPER FIND AIM");
        Animator.Play(gameObject, "ST_Run", speedMult);
        if (blaster != null)
            Animator.Play(blaster, "ST_Run", speedMult);
        UpdateAnimationSpd(speedMult);
        Audio.PlayAudio(gameObject, "Play_Footsteps_Stormtrooper");

        if(agent != null && Core.instance != null)
        {
            agent.CalculatePath(gameObject.transform.globalPosition, Core.instance.gameObject.transform.globalPosition);
            Vector3 directionFindAim = new Vector3(agent.GetPointAt(1) - gameObject.transform.globalPosition);
            agent.ClearPath();
            targetPosition = directionFindAim.normalized * runningRange + gameObject.transform.globalPosition;
            agent.CalculatePath(gameObject.transform.globalPosition, targetPosition);
        }  
    }

    private void UpdateFindAim()
    {
        LookAt(agent.GetDestination());

        agent.MoveToCalculatedPos(runningSpeed * speedMult);

        UpdateAnimationSpd(speedMult);
    }
    private void FindAimEnd()
    {
        Audio.StopAudio(gameObject);

        PlayerIsShootable();
    }
    #endregion

    #region SHOOT
    private void StartShoot()
    {
        statesTimer = timeBetweenStates;
        Animator.Play(gameObject, "ST_Idle", speedMult);
        if (blaster != null)
            Animator.Play(blaster, "ST_Idle", speedMult);
        UpdateAnimationSpd(speedMult);
    }

    private void UpdateShoot()
    {
        if (statesTimer > 0.0f)
        {
            statesTimer -= myDeltaTime;

            if (statesTimer <= 0.0f)
            {
                //First Timer
                if (shotSequences == 0)
                {
                    //First Shot
                    Shoot();
                    shotTimer = timeBetweenShots;
                }
                //Second Timer
                else
                {
                    //Reboot times
                    shooting = false;
                    shotTimes = 0;
                    shotSequences = 0;
                    inputsList.Add(INPUT.IN_RUN);
                }
            }
        }

        if (shotTimer > 0.0f)
        {

            shotTimer -= myDeltaTime;

            if (shotTimer <= 0.0f)
            {
                Shoot();
                //shooting = true;

                if (shotTimes >= maxShots)
                {
                    shotSequences++;

                    Animator.Play(gameObject, "ST_Idle", speedMult);
                    if (blaster != null)
                        Animator.Play(blaster, "ST_Idle", speedMult);
                    UpdateAnimationSpd(speedMult);

                    //End of second shot of the first sequence
                    if (shotSequences < maxSequences)
                    {
                        //Start of pause between sequences
                        sequenceTimer = timeBetweenSequences;
                        shotTimes = 0;
                        shooting = false;
                    }
                    //End of second shot of the second sequence
                    else
                    {
                        statesTimer = timeBetweenStates;
                        shooting = false;
                        //Debug.Log("Ending 2 time shot");
                    }
                }
            }
        }

        if (sequenceTimer > 0.0f)
        {
            sequenceTimer -= myDeltaTime;

            if (sequenceTimer <= 0.0f)
            {
                Shoot();
                shotTimer = timeBetweenShots;
                //shooting = true;
            }
        }

        if(!shooting)
            LookAt(Core.instance.gameObject.transform.globalPosition);

        UpdateAnimationSpd(speedMult);
    }

    private void Shoot()
    {
        shooting = true;

        GameObject bullet = InternalCalls.CreatePrefab("Library/Prefabs/1635392825.prefab", shootPoint.transform.globalPosition, shootPoint.transform.globalRotation, shootPoint.transform.globalScale);
        bullet.GetComponent<BH_Bullet>().damage = damage;

        Animator.Play(gameObject, "ST_Shoot", speedMult);
        if (blaster != null)
            Animator.Play(blaster, "ST_Shoot", speedMult);

        Audio.PlayAudio(gameObject, "PLay_Blaster_Stormtrooper");
        UpdateAnimationSpd(speedMult);
        shotTimes++;
    }
    private void PlayerDetected()
    {
        Audio.PlayAudio(gameObject, "Play_Enemy_Detection");
        if (myParticles != null && myParticles.alert != null)
            myParticles.alert.Play();
    }
    #endregion

    #region DIE
    private void StartDie()
    {
        dieTimer = dieTime;
        //Audio.StopAudio(gameObject);

        Animator.Play(gameObject, "ST_Die", speedMult);
        if (blaster != null)
            Animator.Play(blaster, "ST_Die", speedMult);
        UpdateAnimationSpd(speedMult);

        Audio.PlayAudio(gameObject, "Play_Stormtrooper_Death");
        Audio.PlayAudio(gameObject, "Play_Mando_Kill_Voice");

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
            dieTimer -= Time.deltaTime;

            if (dieTimer <= 0.0f)
            {
                Die();
            }
        }

        UpdateAnimationSpd(speedMult);
    }

    private void Die()
    {
        Counter.SumToCounterType(Counter.CounterTypes.ENEMY_STORMTROOPER);
        EnemyManager.RemoveEnemy(gameObject);

        float dist = (deathPoint.transform.globalPosition - gameObject.transform.globalPosition).magnitude;
        Vector3 forward = gameObject.transform.GetForward();
        forward = forward.normalized * (-dist);

        Core.instance.gameObject.GetComponent<PlayerHealth>().TakeDamage(-PlayerHealth.healWhenKillingAnEnemy);
        InternalCalls.CreatePrefab("Library/Prefabs/230945350.prefab", new Vector3(gameObject.transform.globalPosition.x + forward.x, gameObject.transform.globalPosition.y, gameObject.transform.globalPosition.z + forward.z), Quaternion.identity, new Vector3(1, 1, 1));
        DropCoins();

        InternalCalls.Destroy(gameObject);
    }

    #endregion

    #region PUSH
    private void StartPush()
    {
        Vector3 force = gameObject.transform.globalPosition - Core.instance.gameObject.transform.globalPosition;
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

    private bool PlayerIsShootable()
    {
        float distance = detectionRange;
        float hitDistance = detectionRange;
        GameObject raycastHit = InternalCalls.RayCast(shootPoint.transform.globalPosition, (Core.instance.gameObject.transform.globalPosition - shootPoint.transform.globalPosition).normalized, distance, ref hitDistance);

        if (raycastHit != null)
        {
            if (raycastHit.CompareTag("Player"))
            {
                //Debug.Log("RayCast Player True with tag");
                needFindAim = false;
                return true;
            }

        }

        //Debug.Log("RayCast Player False");
        needFindAim = true;
        return false;
    }

    public void OnCollisionEnter(GameObject collidedGameObject)
    {
        if (collidedGameObject.CompareTag("Bullet"))
        {
            if (Core.instance != null)
                if (Core.instance.HasStatus(STATUS_TYPE.PRIM_SLOW))
                    AddStatus(STATUS_TYPE.SLOWED, STATUS_APPLY_TYPE.BIGGER_PERCENTAGE, Core.instance.GetStatusData(STATUS_TYPE.PRIM_SLOW).severity / 100, 2, false);
            if (Core.instance != null)
                if (Core.instance.HasStatus(STATUS_TYPE.PRIM_MOV_SPEED))
                    AddStatus(STATUS_TYPE.ACCELERATED, STATUS_APPLY_TYPE.BIGGER_PERCENTAGE, Core.instance.GetStatusData(STATUS_TYPE.PRIM_MOV_SPEED).severity / 100, 5, false);
            BH_Bullet bullet = collidedGameObject.GetComponent<BH_Bullet>();

            if (bullet != null)
                TakeDamage(bullet.GetDamage());

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
        else if (collidedGameObject.CompareTag("ChargeBullet"))
        {
            ChargedBullet bullet = collidedGameObject.GetComponent<ChargedBullet>();

            if (bullet != null)
            {
                this.AddStatus(STATUS_TYPE.ENEMY_DAMAGE_DOWN, STATUS_APPLY_TYPE.BIGGER_PERCENTAGE, 0.5f, 3.5f);
                TakeDamage(bullet.GetDamage());

            }

            Audio.PlayAudio(gameObject, "Play_Stormtrooper_Hit");

            if (Core.instance.hud != null)
            {
                HUD hudComponent = Core.instance.hud.GetComponent<HUD>();

                if (hudComponent != null)
                    hudComponent.AddToCombo(55, 0.25f);
            }
            if (currentState != STATE.DIE && healthPoints <= 0.0f)
            {
                inputsList.Add(INPUT.IN_DIE);
                if (Core.instance != null)
                {
                    if (Core.instance.HasStatus(STATUS_TYPE.SP_HEAL))
                    {
                        if (Core.instance.gameObject != null && Core.instance.gameObject.GetComponent<PlayerHealth>() != null)
                        {
                            float healing = Core.instance.GetStatusData(STATUS_TYPE.SP_HEAL).severity;
                            Core.instance.gameObject.GetComponent<PlayerHealth>().SetCurrentHP(PlayerHealth.currHealth + (int)(healing));
                        }
                    }
                    if (Core.instance.HasStatus(STATUS_TYPE.SP_FORCE_REGEN))
                    {
                        if (Core.instance.gameObject != null && BabyYoda.instance != null)
                        {
                            float force = Core.instance.GetStatusData(STATUS_TYPE.SP_FORCE_REGEN).severity;
                            BabyYoda.instance.SetCurrentForce((int)(BabyYoda.instance.GetCurrentForce() + force));
                        }
                    }
                }

            }

        }
        else if (collidedGameObject.CompareTag("WorldLimit"))
        {
            if (currentState != STATE.DIE)
            {
                inputsList.Add(INPUT.IN_DIE);
            }
        }
        else if (collidedGameObject.CompareTag("ExplosiveBarrel") && collidedGameObject.GetComponent<SphereCollider>().active)
        {
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
            if (Core.instance != null)
            {
                inputsList.Add(INPUT.IN_PUSHED);
            }
        }
    }

    public override void TakeDamage(float damage)
    {

        if (currentState != STATE.DIE)
        {
            if (myParticles != null && myParticles.hit != null)
                myParticles.hit.Play();
            
            healthPoints -= damage;

            if (Core.instance != null)
            {
                if (Core.instance.HasStatus(STATUS_TYPE.LIFESTEAL))
                {
                    Random rand = new Random();
                    float result = rand.Next(1, 101);
                    if (result <= 11)
                        if (Core.instance.gameObject != null && Core.instance.gameObject.GetComponent<PlayerHealth>() != null)
                    {
                        float healing = Core.instance.GetStatusData(STATUS_TYPE.LIFESTEAL).severity * damage / 100;
                        if (healing < 1) healing = 1;
                        Core.instance.gameObject.GetComponent<PlayerHealth>().SetCurrentHP(PlayerHealth.currHealth + (int)(healing));
                    }
                }
            }

            if (healthPoints <= 0.0f)
            {
                inputsList.Add(INPUT.IN_DIE);
            }
        }


    }
    private void UpdateAnimationSpd(float newSpd)
    {
        if (currAnimationPlaySpd != newSpd)
        {
            Animator.SetSpeed(gameObject, newSpd);
            if (blaster != null)
                Animator.SetSpeed(blaster, newSpd);
            currAnimationPlaySpd = newSpd;
        }
    }

    public void OnDestroy()
    {
        EnemyManager.RemoveEnemy(this.gameObject);
    }
}
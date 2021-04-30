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
        //FIND_AIM,
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
        //IN_FIND_AIM,
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

    //Action times
    public float idleTime = 5.0f;
    private float dieTime = 3.0f;
    public float timeBewteenShots = 0.5f;
    public float timeBewteenSequences = 0.5f;
    public float timeBewteenStates = 1.5f;
    private float reAimTime = 0.5f;

    //Speeds
    public float wanderSpeed = 3.5f;
    public float runningSpeed = 7.5f;
    public float bulletSpeed = 10.0f;
    private bool skill_slowDownActive = false;

    //Ranges
    public float wanderRange = 7.5f;
    public float runningRange = 12.5f;

    //Timers
    private float idleTimer = 0.0f;
    private float shotTimer = 0.0f;
    private float sequenceTimer = 0.0f;
    private float dieTimer = 0.0f;
    private float statesTimer = 0.0f;
    private float pushTimer = 0.0f;
    private float reAimTimer = 0.0f;
    private float skill_slowDownTimer = 0.0f;

    //Action variables
    int shotTimes = 0;
    public int maxShots = 2;
    private int shotSequences = 0;
    public int maxSequences = 2;
    private bool shooting = false;
    private bool unableToShoot = false;

    //force
    public float forcePushMod = 1;

    //Death point
    public GameObject deathPoint = null;

    private StormTrooperParticles myParticles = null;
    public void Awake()
    {
        Debug.Log("Stormtrooper Awake");


        agent = gameObject.GetComponent<NavMeshAgent>();
        targetPosition = null;

        currentState = STATE.IDLE;
        Animator.Play(gameObject, "ST_Idle");

        shotTimes = 0;
        shotSequences = 0;

        idleTimer = idleTime;
        dieTime = Animator.GetAnimationDuration(gameObject, "ST_Die");

        ParticleSystem spawnparticles = null;

        myParticles = gameObject.GetComponent<StormTrooperParticles>();
        if (myParticles != null)
        {
            spawnparticles = myParticles.spawn;
        }

        if (spawnparticles != null)
        {
            //Debug.Log("PLAY SPAWN!!!");
            spawnparticles.Play();
        }
        else
        {
            //Debug.Log("CAN'T PLAY SPAWN!!!"); 
        }
    }

    public void Update()
    {
        if (player == null)
        {
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
            if (Mathf.Distance(gameObject.transform.globalPosition, agent.GetDestination()) <= agent.stoppingDistance)
            {
                inputsList.Add(INPUT.IN_IDLE);
            }
        }

        if (skill_slowDownActive)
        {
            skill_slowDownTimer += Time.deltaTime;
            if (skill_slowDownTimer >= Skill_Tree_Data.GetWeaponsSkillTree().PW3_SlowDownDuration)
            {
                skill_slowDownTimer = 0.0f;
                skill_slowDownActive = false;
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

                if (player != null && currentState != STATE.SHOOT)
                    LookAt(player.transform.globalPosition);
            }
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

                //case STATE.FIND_AIM:
                //    switch(input)
                //    {
                //        case INPUT.IN_SHOOT:
                //            currentState = STATE.SHOOT;
                //            RunEnd();
                //            StartShoot();
                //            break;

                //        case INPUT.IN_PUSHED:
                //            currentState = STATE.PUSHED;
                //            StartPush();
                //            break;

                //        case INPUT.IN_DIE:
                //            currentState = STATE.DIE;
                //            StartDie();
                //            break;
                //    }
                //    break;

                case STATE.SHOOT:
                    switch (input)
                    {
                        case INPUT.IN_RUN:
                            currentState = STATE.RUN;
                            StartRun();
                            break;

                        //case INPUT.IN_FIND_AIM:
                        //    currentState = STATE.FIND_AIM;
                        //    StartFindAim();
                        //    break;

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
            //case STATE.FIND_AIM:
            //    UpdateFindAim();
            //    break;
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
        agent.CalculateRandomPath(gameObject.transform.globalPosition, wanderRange);

        Animator.Play(gameObject, "ST_Run");
        Audio.PlayAudio(gameObject, "Play_Footsteps_Stormtrooper");
    }
    private void UpdateWander()
    {
        LookAt(agent.GetDestination());
        if (skill_slowDownActive) 
            agent.MoveToCalculatedPos(wanderSpeed * (1 - Skill_Tree_Data.GetWeaponsSkillTree().PW3_SlowDownAmount));
        else agent.MoveToCalculatedPos(wanderSpeed);

    }
    private void WanderEnd()
    {
        Audio.StopAudio(gameObject);
    }
    #endregion

    #region RUN
    private void StartRun()
    {
        agent.CalculateRandomPath(gameObject.transform.globalPosition, runningRange);

        Animator.Play(gameObject, "ST_Run");
        Audio.PlayAudio(gameObject, "Play_Footsteps_Stormtrooper");
    }

    private void UpdateRun()
    {
        LookAt(agent.GetDestination());

        if (skill_slowDownActive)
            agent.MoveToCalculatedPos(runningSpeed * (1 - Skill_Tree_Data.GetWeaponsSkillTree().PW3_SlowDownAmount));
        else
            agent.MoveToCalculatedPos(runningSpeed);

    }
    private void RunEnd()
    {
        Audio.StopAudio(gameObject);
    }
    #endregion

    #region FIND_AIM

    private void StartFindAim()
    {
        if (agent != null && player != null)
        {
            if (!agent.CalculatePath(gameObject.transform.globalPosition, player.transform.globalPosition))
            {
                inputsList.Add(INPUT.IN_SHOOT);
                return;
            }

            targetPosition = agent.GetPointAt(agent.GetPathSize() - 1);

            if (targetPosition == null)
            {
                inputsList.Add(INPUT.IN_SHOOT);
                return;
            }

            targetPosition = (targetPosition - gameObject.transform.globalPosition).normalized * wanderRange;

            Animator.Play(gameObject, "ST_Run");
            reAimTimer = reAimTime;
        }

    }

    private void UpdateFindAim()
    {
        if (targetPosition != null)
        {
            if (reAimTimer > 0.0f)
            {
                reAimTimer -= Time.deltaTime;

                LookAt(targetPosition);
                MoveToPosition(targetPosition, wanderSpeed);

                if (reAimTime <= 0.0f)
                {
                    if (PlayerIsShootable())
                    {
                        inputsList.Add(INPUT.IN_SHOOT);
                    }
                    else
                    {
                        reAimTimer = reAimTime;
                    }
                }
            }
        }

    }

    #endregion

    #region SHOOT
    private void StartShoot()
    {
        statesTimer = timeBewteenStates;
        Animator.Play(gameObject, "ST_Idle");
    }

    private void UpdateShoot()
    {
        if (unableToShoot)
        {
            agent.CalculatePath(gameObject.transform.globalPosition, Core.instance.gameObject.transform.globalPosition);
            agent.MoveToCalculatedPos(runningSpeed);
            LookAt(agent.GetDestination());
            if (PlayerIsShootable())
            {
                unableToShoot = false;
                statesTimer = timeBewteenStates;
            }
            else return;
        }
        else
        {
            Animator.Play(gameObject, "ST_Idle");
        }


        if (statesTimer > 0.0f)
        {
            statesTimer -= Time.deltaTime;

            if (statesTimer <= 0.0f)
            {


                //First Timer
                if (shotSequences == 0)
                {
                    //First Shot
                    if (!Shoot()) return;
                    shotTimer = timeBewteenShots;
                    shooting = true;
                }
                //Second Timer
                else
                {
                    //Reboot times
                    shotTimes = 0;
                    shotSequences = 0;
                    shooting = false;
                    inputsList.Add(INPUT.IN_RUN);
                }
            }
        }

        if (shotTimer > 0.0f)
        {

            shotTimer -= Time.deltaTime;

            if (shotTimer <= 0.0f)
            {
                shooting = true;

                if (!Shoot()) return;

                if (shotTimes >= maxShots)
                {
                    shotSequences++;

                    Animator.Play(gameObject, "ST_Idle");

                    //End of second shot of the first sequence
                    if (shotSequences < maxSequences)
                    {
                        sequenceTimer = timeBewteenSequences;
                        shotTimes = 0;
                        shooting = false;
                        //Start of pause between sequences
                    }
                    //End of second shot of the second sequence
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
                if (!Shoot()) return;
                shotTimer = timeBewteenShots;
                shooting = true;
            }
        }

        if (!shooting)
            LookAt(Core.instance.gameObject.transform.globalPosition);

    }

    private bool Shoot()
    {
        if (!PlayerIsShootable())
        {
            unableToShoot = true;
            shotTimes = 0;
            shotTimer = 0.0f;
            shotSequences = 0;
            sequenceTimer = 0.0f;
            Animator.Play(gameObject, "ST_Run");
            return false;
        }

        GameObject bullet = InternalCalls.CreatePrefab("Library/Prefabs/1635392825.prefab", shootPoint.transform.globalPosition, shootPoint.transform.globalRotation, shootPoint.transform.globalScale);
        bullet.GetComponent<BH_Bullet>().damage = damage;

        Animator.Play(gameObject, "ST_Shoot");
        Audio.PlayAudio(gameObject, "PLay_Blaster_Stormtrooper");
        shotTimes++;
        return true;
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

        Animator.Play(gameObject, "ST_Die", 1.0f);

        Audio.PlayAudio(gameObject, "Play_Stormtrooper_Death");
        Audio.PlayAudio(gameObject, "Play_Mando_Kill_Voice");

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
        float dist = (deathPoint.transform.globalPosition - gameObject.transform.globalPosition).magnitude;
        Vector3 forward = gameObject.transform.GetForward();
        forward = forward.normalized * (-dist);
        Counter.SumToCounterType(Counter.CounterTypes.ENEMY_STORMTROOP);
        Counter.roomEnemies--;
        Debug.Log("Enemies: " + Counter.roomEnemies.ToString());
        if (Counter.roomEnemies <= 0)
        {
            Counter.allEnemiesDead = true;
        }

        //Created dropped coins
        var rand = new Random();
        int droppedCoins = rand.Next(1, 4);
        for (int i = 0; i < droppedCoins; i++)
        {
            Vector3 pos = gameObject.transform.globalPosition;
            pos.x += rand.Next(-200, 201) / 100;
            pos.z += rand.Next(-200, 201) / 100;
            InternalCalls.CreatePrefab(coinDropPath, pos, Quaternion.identity, new Vector3(0.07f, 0.07f, 0.07f));
        }
        player.GetComponent<PlayerHealth>().TakeDamage(-PlayerHealth.healWhenKillingAnEnemy);
        InternalCalls.CreatePrefab("Library/Prefabs/230945350.prefab", new Vector3(gameObject.transform.globalPosition.x + forward.x, gameObject.transform.globalPosition.y, gameObject.transform.globalPosition.z + forward.z), Quaternion.identity, new Vector3(1, 1, 1));
        InternalCalls.Destroy(gameObject);
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

    private bool PlayerIsShootable()
    {
        float distance = detectionRange;
        float hitDistance = detectionRange;
        GameObject raycastHit = InternalCalls.RayCast(shootPoint.transform.globalPosition, (Core.instance.gameObject.transform.globalPosition-shootPoint.transform.globalPosition).normalized, distance, ref hitDistance);

        if (raycastHit != null)
        {
            if (raycastHit.CompareTag("Player"))
            {
                Debug.Log("RayCast Player True with tag");
                return true;
            }

        }
        
        Debug.Log("RayCast Player False");
        return false;
    }

    public void OnCollisionEnter(GameObject collidedGameObject)
    {
        if (collidedGameObject.CompareTag("Bullet"))
        {
            if (myParticles != null && myParticles.hit != null)
                myParticles.hit.Play();
            BH_Bullet bullet = collidedGameObject.GetComponent<BH_Bullet>();

            if (bullet != null)
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
        else if (collidedGameObject.CompareTag("ChargeBullet"))
        {
            if (myParticles != null && myParticles.hit != null)
                myParticles.hit.Play();
            ChargedBullet bullet = collidedGameObject.GetComponent<ChargedBullet>();

            if (bullet != null)
                TakeDamage(bullet.damage);

            Audio.PlayAudio(gameObject, "Play_Stormtrooper_Hit");

            if (Core.instance.hud != null)
            {
                HUD hudComponent = Core.instance.hud.GetComponent<HUD>();

                if (hudComponent != null)
                    hudComponent.AddToCombo(55, 0.25f);
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
            if (myParticles != null && myParticles.hit != null)
                myParticles.hit.Play();
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
            if (player != null)
            {
                inputsList.Add(INPUT.IN_PUSHED);

            }
        }
    }

    public override void TakeDamage(float damage)
    {

        if (currentState != STATE.DIE)
        {
            healthPoints -= damage;

            if (healthPoints <= 0.0f)
            {
                inputsList.Add(INPUT.IN_DIE);
            }
        }


    }
}
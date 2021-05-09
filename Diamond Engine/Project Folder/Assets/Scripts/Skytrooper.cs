using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Collections.Generic;

using DiamondEngine;

public class Skytrooper : Enemy
{
    enum STATE : int
    {
        NONE = -1,
        IDLE,
        WANDER,
        DASH,
        PUSHED,
        SHOOT,
        HIT,
        DIE
    }

    enum INPUT : int
    {
        IN_IDLE,
        IN_IDLE_END,
        IN_DASH,
        IN_DASH_END,
        IN_WANDER,
        IN_PUSHED,
        IN_SHOOT,
        IN_HIT,
        IN_DIE,
        IN_PLAYER_IN_RANGE,

    }

    //State
    private STATE currentState = STATE.NONE;

    private List<INPUT> inputsList = new List<INPUT>();

    public GameObject shootPoint = null;
    public GameObject blaster = null;
    private float initialHeight = 0.0f;

    //Action times
    public float idleTime = 5.0f;
    public float wanderTime = 0.0f;
    private float dashTime = 0.0f;
    private float dieTime = 0.75f;
    public float timeBewteenShots = 0.5f;
    public float timeBewteenShootingStates = 1.5f;

    //Speeds
    public float wanderSpeed = 3.5f;
    public float dashSpeed = 7.5f;
    //public float bulletSpeed = 10.0f;
    private bool skill_slowDownActive = false;

    //Ranges
    public float wanderRange = 7.5f;
    public float dashRange = 12.5f;

    //Timers
    private float idleTimer = 0.0f;
    private float wanderTimer = 0.0f;
    private float dashTimer = 0.0f;
    //private float shotTimer = 0.0f;
    private float dieTimer = 0.0f;
    private float shootTimer = 0.0f;
    private float pushTimer = 0.0f;
    private float skill_slowDownTimer = 0.0f;
    private float currAnimationPlaySpd = 1f;

    //Action variables
    private int shotsShooted = 0;
    public int maxShots = 2;
    public float explosionDistance = 2.0f;

    //push
    public float pushHorizontalForce = 100;
    public float pushVerticalForce = 10;
    public float PushStun = 2;

    //hit particles
    public GameObject hitParticlesObj = null;
    private ParticleSystem hitParticles = null;

    public void Awake()
    {
        InitEntity(ENTITY_TYPE.SKYTROOPER);
        EnemyManager.AddEnemy(gameObject);

        agent = gameObject.GetComponent<NavMeshAgent>();
        targetPosition = null;

        currentState = STATE.IDLE;
        Animator.Play(gameObject, "SK_Idle", speedMult);
        Animator.Play(blaster, "SK_Idle", speedMult);
        UpdateAnimationSpd(speedMult);

        idleTimer = idleTime;
        dashTime = Animator.GetAnimationDuration(gameObject, "SK_Dash");

        initialHeight = gameObject.transform.globalPosition.y;
        if (hitParticlesObj != null)
            hitParticles = hitParticlesObj.GetComponent<ParticleSystem>();
        else
            Debug.Log("Hit particles gameobject not found!");
    }

    public void Update()
    {
        myDeltaTime = Time.deltaTime * speedMult;
        UpdateStatuses();

        if (Input.GetKey(DEKeyCode.T) == KeyState.KEY_DOWN)
            inputsList.Add(INPUT.IN_DIE);

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

        if (currentState == STATE.WANDER && wanderTimer > 0.0f)
        {
            wanderTimer -= myDeltaTime;
            if (wanderTimer < 0.0f)
            {
                inputsList.Add(INPUT.IN_IDLE);
            }
        }

        if (currentState == STATE.DASH && dashTimer > 0.0f)
        {
            dashTimer -= myDeltaTime;
            if (dashTimer < 0.0f)
            {
                inputsList.Add(INPUT.IN_DASH_END);
            }
        }

        if (skill_slowDownActive)
        {
            skill_slowDownTimer += myDeltaTime;
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
        if (currentState != STATE.DIE && currentState != STATE.DASH)
        {
            if (Core.instance.gameObject == null)
                return;

            if (InRange(Core.instance.gameObject.transform.globalPosition, detectionRange))
            {
                inputsList.Add(INPUT.IN_PLAYER_IN_RANGE);
                LookAt(Core.instance.gameObject.transform.globalPosition);
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
                            IdleEnd();
                            StartWander();
                            break;

                        case INPUT.IN_PLAYER_IN_RANGE:
                            currentState = STATE.SHOOT;
                            IdleEnd();
                            PlayerDetected();
                            StartShoot();
                            break;

                        case INPUT.IN_DIE:
                            currentState = STATE.DIE;
                            IdleEnd();
                            StartDie();
                            break;

                        case INPUT.IN_PUSHED:
                            currentState = STATE.PUSHED;
                            IdleEnd();
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
                            StartPush();
                            break;
                    }
                    break;

                case STATE.DASH:
                    switch (input)
                    {
                        case INPUT.IN_DASH_END:
                            currentState = STATE.IDLE;
                            DashEnd();
                            StartIdle();
                            break;

                        case INPUT.IN_WANDER:
                            currentState = STATE.WANDER;
                            DashEnd();
                            StartWander();
                            break;

                        case INPUT.IN_DIE:
                            currentState = STATE.DIE;
                            DashEnd();
                            StartDie();
                            break;

                        case INPUT.IN_PUSHED:
                            currentState = STATE.PUSHED;
                            DashEnd();
                            StartPush();
                            break;
                    }
                    break;

                case STATE.SHOOT:
                    switch (input)
                    {
                        case INPUT.IN_DASH:
                            currentState = STATE.DASH;
                            StartDash();
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
                            DashEnd();
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
                UpdateIdle();
                break;
            case STATE.DASH:
                UpdateDash();
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
        //Debug.Log("SKYTROOPER IDLE");
        idleTimer = idleTime;
        Animator.Play(gameObject, "SK_Idle", speedMult);
        Animator.Play(blaster, "SK_Idle", speedMult);
        UpdateAnimationSpd(speedMult);



        Audio.PlayAudio(gameObject, "Play_Skytrooper_Jetpack_Loop");
    }

    private void UpdateIdle()
    {
        UpdateAnimationSpd(speedMult);

        if (gameObject.transform.globalPosition.y > initialHeight)
            gameObject.transform.localPosition.y--;
    }

    private void IdleEnd()
    {
        Audio.StopAudio(gameObject);
    }
    #endregion

    #region WANDER
    private void StartWander()
    {
        wanderTimer = wanderTime;
        //Debug.Log("SKYTROOPER WANDER");

        Animator.Play(gameObject, "SK_Wander", speedMult);
        Animator.Play(blaster, "SK_Wander", speedMult);
        UpdateAnimationSpd(speedMult);
        Audio.PlayAudio(gameObject, "Play_Skytrooper_Jetpack_Loop");

        targetPosition = CalculateNewPosition(wanderRange);

    }
    private void UpdateWander()
    {
        LookAt(targetPosition);

        //if (skill_slowDownActive)
        //    MoveToPosition(targetPosition, wanderSpeed * (1 - Skill_Tree_Data.GetWeaponsSkillTree().PW3_SlowDownAmount));
        //else 
        MoveToPosition(targetPosition, wanderSpeed * speedMult);

        UpdateAnimationSpd(speedMult);
    }
    private void WanderEnd()
    {
        Audio.StopAudio(gameObject);
    }
    #endregion

    #region DASH
    private void StartDash()
    {
        dashTimer = dashTime;
        //Debug.Log("SKYTROOPER DASH");

        Animator.Play(gameObject, "SK_Dash", speedMult);
        Animator.Play(blaster, "SK_Dash", speedMult);
        Audio.PlayAudio(gameObject, "Play_Skytrooper_Dash");

        //agent.CalculateRandomPath(gameObject.transform.globalPosition, dashRange);
        targetPosition = CalculateRandomInRangePosition();

        if (targetPosition == null)
            inputsList.Add(INPUT.IN_WANDER);
        //Debug.Log(targetPosition.ToString());

        UpdateAnimationSpd(speedMult);
    }
    private void UpdateDash()
    {
        LookAt(targetPosition);

        //if (skill_slowDownActive)
        //    MoveToPosition(targetPosition, dashSpeed * (1 - Skill_Tree_Data.GetWeaponsSkillTree().PW3_SlowDownAmount));
        //else 
        MoveToPosition(targetPosition, dashSpeed * speedMult);

        UpdateAnimationSpd(speedMult);
    }
    private void DashEnd()
    {
        Audio.StopAudio(gameObject);
    }
    #endregion

    #region SHOOT
    private void StartShoot()
    {
        //Debug.Log("SKYTROOPER SHOOT");
        shootTimer = timeBewteenShootingStates;
        shotsShooted = 0;
        Animator.Play(gameObject, "SK_Idle", speedMult);
        Animator.Play(blaster, "SK_Idle", speedMult);
        UpdateAnimationSpd(speedMult);
        Audio.PlayAudio(gameObject, "Play_Skytrooper_Jetpack_Loop");
    }

    private void UpdateShoot()
    {
        shootTimer -= myDeltaTime;

        if (shootTimer <= 0.0f)
        {
            if (shotsShooted == maxShots)
            {
                inputsList.Add(INPUT.IN_DASH);
            }
            else
            {
                Shoot();
            }
        }
        UpdateAnimationSpd(speedMult);
    }

    private void Shoot()
    {
        GameObject bullet = InternalCalls.CreatePrefab("Library/Prefabs/1662408971.prefab", shootPoint.transform.globalPosition, shootPoint.transform.globalRotation, new Vector3(0.5f, 0.5f, 0.5f));

        Vector2 player2DPosition = new Vector2(Core.instance.gameObject.transform.globalPosition.x, Core.instance.gameObject.transform.globalPosition.z);
        Vector2 randomPosition = Mathf.RandomPointAround(player2DPosition, 1);

        Random randomizer = new Random();
        int sign = randomizer.Next(3);
        sign--;

        //Debug.Log("Sign: " + sign.ToString());

        Vector3 projectileEndPosition = null;

        projectileEndPosition = new Vector3(randomPosition.x, Core.instance.gameObject.transform.globalPosition.y, randomPosition.y);
        
        if (Core.instance.GetSate() == Core.STATE.DASH)
        {
           // Debug.Log("Skytrooper shot while dashing");
            projectileEndPosition += Core.instance.gameObject.transform.GetForward().normalized * Core.instance.dashDistance;
        }


        bullet.GetComponent<SkyTrooperShot>().SetTarget(projectileEndPosition, false);
        Animator.Play(gameObject, "SK_Shoot", speedMult);
        Animator.Play(blaster, "SK_Shoot", speedMult);
        UpdateAnimationSpd(speedMult);
        Audio.PlayAudio(gameObject, "PLay_Skytrooper_Grenade_Launch");

        shotsShooted++;
        if (shotsShooted < maxShots)
            shootTimer = timeBewteenShots;
        else
        {
            shootTimer = timeBewteenShootingStates;
            Animator.Play(gameObject, "SK_Idle", speedMult);
            Animator.Play(blaster, "SK_Idle", speedMult);
            UpdateAnimationSpd(speedMult);
        }

    }
    private void PlayerDetected()
    {
        //Debug.Log("SKYTROOPER PLAYER DETECTED");
        Audio.PlayAudio(gameObject, "Play_Enemy_Detection");
    }
    #endregion

    #region DIE
    private void StartDie()
    {
        //Debug.Log("SKYTROOPER DIE");
        dieTimer = dieTime;
        //Audio.StopAudio(gameObject);

        //Animator.Play(gameObject, "ST_Die", 1.0f);

        Audio.PlayAudio(gameObject, "Play_Stormtrooper_Death");
        Audio.PlayAudio(gameObject, "Play_Mando_Kill_Voice");

        //Combo
        //UNCOMMENT
        if (PlayerResources.CheckBoon(BOONS.BOON_MASTERYODAASSITANCE))
        {
            Debug.Log("Start die ended");
            HUD hud = Core.instance.hud.GetComponent<HUD>();

            if (hud != null)
                hud.AddToCombo(300, 1.0f);
        }
        //UNCOMMENT
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
    }

    private void Die()
    {
        //float dist = (deathPoint.transform.globalPosition - gameObject.transform.globalPosition).magnitude;
        //forward = forward.normalized * (-dist);
        Counter.SumToCounterType(Counter.CounterTypes.ENEMY_SKYTROOPER);
        EnemyManager.RemoveEnemy(gameObject);

        DropCoins();

        Core.instance.gameObject.GetComponent<PlayerHealth>().TakeDamage(-PlayerHealth.healWhenKillingAnEnemy);

        //Explosion
        Explode();
        InternalCalls.Destroy(gameObject);
    }

    private void Explode()
    {
        Vector3 forward = gameObject.transform.GetForward();
        InternalCalls.CreatePrefab("Library/Prefabs/828188331.prefab", new Vector3(gameObject.transform.globalPosition.x + forward.x, gameObject.transform.globalPosition.y, gameObject.transform.globalPosition.z + forward.z), Quaternion.identity, new Vector3(1, 1, 1));

        if (Mathf.Distance(Core.instance.gameObject.transform.globalPosition, gameObject.transform.globalPosition) <= explosionDistance)
        {
            PlayerHealth playerHealth = Core.instance.gameObject.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                //Debug.Log("Player hurt by skytrooper explosion");
                playerHealth.TakeDamage((int)damage);
            }
        }
    }

    #endregion

    #region PUSH

    private void StartPush()
    {
        Vector3 force = gameObject.transform.globalPosition - Core.instance.gameObject.transform.globalPosition;
        force.y = pushVerticalForce;
        gameObject.AddForce(force * pushHorizontalForce);
        pushTimer = 0.0f;
    }
    private void UpdatePush()
    {
        pushTimer += myDeltaTime;
        if (pushTimer >= PushStun)
            inputsList.Add(INPUT.IN_IDLE);

    }
    #endregion

    public void OnCollisionEnter(GameObject collidedGameObject)
    {
        if (collidedGameObject.CompareTag("Bullet"))
        {
            BH_Bullet bullet = collidedGameObject.GetComponent<BH_Bullet>();

            if (bullet != null)
            {
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
        }
        else if (collidedGameObject.CompareTag("ChargeBullet"))
        {
            ChargedBullet bullet = collidedGameObject.GetComponent<ChargedBullet>();

            if (bullet != null)
            {
                this.AddStatus(STATUS_TYPE.ENEMY_DAMAGE_DOWN, STATUS_APPLY_TYPE.BIGGER_PERCENTAGE, 0.5f, 3.5f);
               // healthPoints -= bullet.damage;

                TakeDamage(bullet.GetDamage());

                Audio.PlayAudio(gameObject, "Play_Stormtrooper_Hit");

                if (Core.instance.hud != null)
                {
                    Core.instance.hud.GetComponent<HUD>().AddToCombo(55, 0.25f);
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
        hitParticles.Play();
        if (currentState != STATE.DIE)
        {
            if (healthPoints <= 0.0f)
                inputsList.Add(INPUT.IN_DIE);
        }
    }

    private Vector3 CalculateRandomInRangePosition()
    {
        Random randomAngle = new Random();

        if (Core.instance == null)
            return null;

        if (Mathf.Distance(Core.instance.gameObject.transform.globalPosition, gameObject.transform.globalPosition) < detectionRange * 0.5f)
        {
            float angle = randomAngle.Next(-60, 60);

            //Debug.Log("Un pasito patrás: " + angle.ToString());

            Vector3 direction = gameObject.transform.globalPosition - Core.instance.gameObject.transform.globalPosition;

            Vector3 randomPosition = new Vector3((float)(Math.Cos(angle * Mathf.Deg2RRad) * direction.normalized.x - Math.Sin(angle * Mathf.Deg2RRad) * direction.normalized.z),
                                                 0.0f,
                                                 (float)(Math.Sin(angle * Mathf.Deg2RRad) * direction.normalized.x + Math.Cos(angle * Mathf.Deg2RRad) * direction.normalized.z));

            return gameObject.transform.localPosition + randomPosition * dashRange;
        }
        else if (Mathf.Distance(Core.instance.gameObject.transform.globalPosition, gameObject.transform.globalPosition) < detectionRange)
        {
            float angle = randomAngle.Next(-90, 90);

            //Debug.Log("Un pasito palante, María: " + angle.ToString());

            Vector3 direction = Core.instance.gameObject.transform.globalPosition - gameObject.transform.globalPosition;

            Vector3 randomPosition = new Vector3((float)(Math.Cos(angle * Mathf.Deg2RRad) * direction.normalized.x - Math.Sin(angle * Mathf.Deg2RRad) * direction.normalized.z),
                                                 0.0f,
                                                 (float)(Math.Sin(angle * Mathf.Deg2RRad) * direction.normalized.x + Math.Cos(angle * Mathf.Deg2RRad) * direction.normalized.z));

            return gameObject.transform.localPosition + randomPosition * dashRange;
        }

        return null;
    }

    private void UpdateAnimationSpd(float newSpd)
    {
        if (currAnimationPlaySpd != newSpd)
        {
            Animator.SetSpeed(gameObject, newSpd);
            currAnimationPlaySpd = newSpd;
        }
    }

}
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
    //public GameObject hitParticles = null;

    //Action times
    public float idleTime = 5.0f;
    public float wanderTime = 0.0f;
    private float dashTime = 0.0f;
    private float dieTime = 3.0f;
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

    //Action variables
    private int shotsShooted = 0;
    public int maxShots = 2;


    //push
    public float pushHorizontalForce = 100;
    public float pushVerticalForce = 10;
    public float PushStun = 2;

    public void Awake()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
        targetPosition = null;

        currentState = STATE.IDLE;
        Animator.Play(gameObject, "SK_Idle");

        idleTimer = idleTime;
        dashTime = Animator.GetAnimationDuration(gameObject, "SK_Dash");
    }

    public void Update()
    {
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

        if (currentState == STATE.WANDER && wanderTimer > 0.0f)
        {
            wanderTimer -= Time.deltaTime;
            if (wanderTimer < 0.0f)
            {
                inputsList.Add(INPUT.IN_IDLE);
            }
        }

        if (currentState == STATE.DASH && dashTimer > 0.0f)
        {
            dashTimer -= Time.deltaTime;
            if (dashTimer < 0.0f)
            {
                inputsList.Add(INPUT.IN_DASH_END);
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
        Animator.Play(gameObject, "SK_Idle");
        Audio.PlayAudio(gameObject, "Play_Skytrooper_Jetpack_Loop");
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

        Animator.Play(gameObject, "SK_Wander");
        Audio.PlayAudio(gameObject, "Play_Skytrooper_Jetpack_Loop");

        targetPosition = CalculateNewPosition(wanderRange);
    }
    private void UpdateWander()
    {
        LookAt(targetPosition);

        if (skill_slowDownActive)
            MoveToPosition(targetPosition, wanderSpeed * (1 - Skill_Tree_Data.GetWeaponsSkillTree().PW3_SlowDownAmount));
        else MoveToPosition(targetPosition, wanderSpeed);
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

        Animator.Play(gameObject, "SK_Dash");
        Audio.PlayAudio(gameObject, "Play_Skytrooper_Dash");

        //agent.CalculateRandomPath(gameObject.transform.globalPosition, dashRange);
        targetPosition = CalculateNewPosition(dashRange);
        //Debug.Log(targetPosition.ToString());
    }
    private void UpdateDash()
    {
        LookAt(targetPosition);

        if (skill_slowDownActive)
            MoveToPosition(targetPosition, dashSpeed * (1 - Skill_Tree_Data.GetWeaponsSkillTree().PW3_SlowDownAmount));
        else MoveToPosition(targetPosition, dashSpeed);
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
        Animator.Play(gameObject, "SK_Idle");
        Audio.PlayAudio(gameObject, "Play_Skytrooper_Jetpack_Loop");
    }

    private void UpdateShoot()
    {
        shootTimer -= Time.deltaTime;

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
    }

    private void Shoot()
    {
        //GameObject bullet = InternalCalls.CreatePrefab("Library/Prefabs/1635392825.prefab", shootPoint.transform.globalPosition, shootPoint.transform.globalRotation, shootPoint.transform.globalScale);
        //bullet.GetComponent<BH_Bullet>().damage = damage;
        GameObject bullet = InternalCalls.CreatePrefab("Library/Prefabs/1662408971.prefab", shootPoint.transform.globalPosition, shootPoint.transform.globalRotation, new Vector3(0.5f, 0.5f, 0.5f));
        bullet.GetComponent<SkyTrooperShot>().SetTarget(Core.instance.gameObject.transform.globalPosition, false);
        Animator.Play(gameObject, "SK_Shoot");
        Audio.PlayAudio(gameObject, "PLay_Skytrooper_Grenade_Launch");

        shotsShooted++;
        if (shotsShooted < maxShots)
            shootTimer = timeBewteenShots;
        else
        {
            shootTimer = timeBewteenShootingStates;
            Animator.Play(gameObject, "SK_Idle");
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
            Quaternion dieRotation = gameObject.transform.localRotation;
            Quaternion rotation = new Quaternion(0,45,0);
            dieRotation = rotation * dieRotation;
            Vector3 dieTransform = gameObject.transform.localPosition;
            dieTransform.y += 0.15f;
            gameObject.transform.localPosition = dieTransform;
            gameObject.transform.localRotation = dieRotation;


            if (dieTimer <= 0.0f)
            {
                Die();
            }
        }
    }

    private void Die()
    {
        //float dist = (deathPoint.transform.globalPosition - gameObject.transform.globalPosition).magnitude;
        Vector3 forward = gameObject.transform.GetForward();
        //forward = forward.normalized * (-dist);
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
        Core.instance.gameObject.GetComponent<PlayerHealth>().TakeDamage(-PlayerHealth.healWhenKillingAnEnemy);
        InternalCalls.CreatePrefab("Library/Prefabs/230945350.prefab", new Vector3(gameObject.transform.globalPosition.x + forward.x, gameObject.transform.globalPosition.y, gameObject.transform.globalPosition.z + forward.z), Quaternion.identity, new Vector3(1, 1, 1));
        InternalCalls.Destroy(gameObject);
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
        pushTimer += Time.deltaTime;
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
}
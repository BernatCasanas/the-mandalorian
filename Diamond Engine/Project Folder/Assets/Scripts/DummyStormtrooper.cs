using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Collections.Generic;

using DiamondEngine;

public class DummyStormtrooper : Enemy
{
    enum STATE : int
    {
        NONE = -1,
        IDLE,
        PUSHED,
        SHOOT,
        HIT,
        DIE
    }

    enum INPUT : int
    {
        IN_IDLE,
        IN_IDLE_END,
        IN_PUSHED,
        IN_SHOOT,
        IN_HIT,
        IN_DIE,
        IN_PLAYER_IN_RANGE
    }

    //State
    private STATE currentState = STATE.NONE;

    private List<INPUT> inputsList = new List<INPUT>();

    public GameObject shootPoint = null;

    public bool canShoot = true;

    //Action times
    public float idleTime = 5.0f;
    private float dieTime = 3.0f;
    public float timeBewteenShots = 0.5f;
    public float timeBewteenSequences = 0.5f;
    public float timeBewteenStates = 1.5f;

    //Speeds
    public float bulletSpeed = 10.0f;
    private bool skill_slowDownActive = false;
    private float currAnimationPlaySpd = 1.0f;

    //Timers
    public float idleTimer = 0.0f;
    private float shotTimer = 0.0f;
    private float sequenceTimer = 0.0f;
    private float dieTimer = 0.0f;
    private float statesTimer = 0.0f;
    private float pushTimer = 0.0f;
    private float skill_slowDownTimer = 0.0f;

    //Action variables
    int shotTimes = 0;
    public int maxShots = 2;
    private int shotSequences = 0;
    public int maxSequences = 2;

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

        currentState = STATE.IDLE;
        Animator.Play(gameObject, "ST_Idle", 1.0f);
        UpdateAnimationSpd(1.0f);

        targetPosition = null;

        shotTimes = 0;
        shotSequences = 0;

        dieTime = Animator.GetAnimationDuration(gameObject, "ST_Die");

        ParticleSystem spawnparticles = null;

        myParticles = gameObject.GetComponent<StormTrooperParticles>();
        if (myParticles != null)
            spawnparticles = myParticles.spawn;

        if (spawnparticles != null)
            spawnparticles.Play();

    }

    public void Update()
    {
        //Debug.Log("Current State: " + currentState.ToString());


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
        if (currentState != STATE.DIE && canShoot)
        {
            if (InRange(Core.instance.gameObject.transform.globalPosition, detectionRange) && idleTimer <= 0.0f)
            {
                if (Core.instance != null && currentState != STATE.SHOOT)
                {
                    inputsList.Add(INPUT.IN_PLAYER_IN_RANGE);
                    Debug.Log("Player in range");
                    LookAt(Core.instance.gameObject.transform.globalPosition);
                }
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

                case STATE.SHOOT:
                    switch (input)
                    {
                        case INPUT.IN_IDLE:
                            currentState = STATE.IDLE;
                            StartIdle();
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
                    Debug.Log("NEED TO ADD STATE TO DUMMY STORMTROOPER SWITCH");
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
        Animator.Play(gameObject, "ST_Idle", speedMult);
        UpdateAnimationSpd(speedMult);
    }

    private void UpdateIdle()
    {
        if (idleTimer > 0.0f)
            idleTimer -= Time.deltaTime;

        UpdateAnimationSpd(speedMult);
    }
    #endregion

    #region SHOOT
    private void StartShoot()
    {
        //Debug.Log("Start Shoot");
        statesTimer = timeBewteenStates;
        Animator.Play(gameObject, "ST_Idle", speedMult);
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
                    shotTimer = timeBewteenShots;
                }
                //Second Timer
                else
                {
                    //Reboot times
                    shotTimes = 0;
                    shotSequences = 0;
                    inputsList.Add(INPUT.IN_IDLE);
                }
            }
        }

        if (shotTimer > 0.0f)
        {
            shotTimer -= myDeltaTime;

            if (shotTimer <= 0.0f)
            {
                Shoot();

                if (shotTimes >= maxShots)
                {
                    shotSequences++;

                    Animator.Play(gameObject, "ST_Idle", speedMult);
                    UpdateAnimationSpd(speedMult);

                    //End of second shot of the first sequence
                    if (shotSequences < maxSequences)
                    {
                        sequenceTimer = timeBewteenSequences;
                        shotTimes = 0;
                        //Start of pause between sequences
                    }
                    //End of second shot of the second sequence
                    else
                    {
                        statesTimer = timeBewteenStates;
                        //Debug.Log("Ending 2 time shot");
                        inputsList.Add(INPUT.IN_IDLE);
                        idleTimer = idleTime;
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
                shotTimer = timeBewteenShots;
            }
        }

        LookAt(Core.instance.gameObject.transform.globalPosition);
        UpdateAnimationSpd(speedMult);
    }

    private void Shoot()
    {
        GameObject bullet = InternalCalls.CreatePrefab("Library/Prefabs/1635392825.prefab", shootPoint.transform.globalPosition, shootPoint.transform.globalRotation, shootPoint.transform.globalScale);
        bullet.GetComponent<BH_Bullet>().damage = damage;

        Animator.Play(gameObject, "ST_Shoot", speedMult);
        UpdateAnimationSpd(speedMult);
        Audio.PlayAudio(gameObject, "PLay_Blaster_Stormtrooper");
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

        Animator.Play(gameObject, "ST_Die", speedMult);
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
                Die();
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
            {
                this.AddStatus(STATUS_TYPE.ENEMY_DAMAGE_DOWN, STATUS_APPLY_TYPE.BIGGER_PERCENTAGE, 0.5f, 3.5f);
                TakeDamage(bullet.damage);

            }

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
            if (Core.instance != null)
                inputsList.Add(INPUT.IN_PUSHED);
        }
    }

    public override void TakeDamage(float damage)
    {
        if (currentState != STATE.DIE)
        {
            healthPoints -= damage;

            if (healthPoints <= 0.0f)
                inputsList.Add(INPUT.IN_DIE);

        }
    }

    private void UpdateAnimationSpd(float newSpd)
    {
        if (currAnimationPlaySpd != newSpd)
        {
            Animator.SetSpeed(gameObject, newSpd);
            currAnimationPlaySpd = newSpd;
        }
    }

    public void OnDestroy()
    {
        EnemyManager.RemoveEnemy(this.gameObject);
    }
}
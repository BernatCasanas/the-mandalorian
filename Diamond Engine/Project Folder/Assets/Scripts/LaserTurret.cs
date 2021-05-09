using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Collections.Generic;

using DiamondEngine;

public class LaserTurret : Enemy
{
    enum STATE : int
    {
        NONE = -1,
        IDLE,
        LOAD,
        SHOOT,
        DIE
    }

    enum INPUT : int
    {
        IN_IDLE,
        IN_IDLE_END,
        IN_LOAD,
        IN_SHOOT,
        IN_DIE
    }

    //State
    private STATE currentState = STATE.NONE;

    private List<INPUT> inputsList = new List<INPUT>();

    //Action times
    public float idleTime = 0.0f;
    public float loadTime = 0.0f;
    public float shotTime = 0.0f;
    public float dieTime = 0.0f;

    public float damageMaxTimer = 0.0f;

    public float feedbackTime = 0.0f;

    //Speeds
    private float angle = 0.0f;
    private float rotationSpeed = 0.0f;

    //Ranges
    public float laserRange = 0.0f;
    public float shotTotalAngle = 0.0f;

    //Timers
    private float idleTimer = 0.0f;
    private float loadTimer = 0.0f;
    private float shotTimer = 0.0f;
    private float dieTimer = 0.0f;
    private float damageCurrentTimer = 0.0f;

    private Vector3[] laserDirections;
    public int lasersNumber = 4;
    public float laserOffser;

    //Explosion effect
    public GameObject explosion = null;
    public GameObject wave = null;
    public GameObject mesh = null;
    public GameObject hit = null;

    private ParticleSystem partExp = null;
    private ParticleSystem partWave = null;
    private ParticleSystem hitParticle = null;

    public void Awake()
    {
        InitEntity(ENTITY_TYPE.TURRET);
        EnemyManager.AddEnemy(gameObject);

        laserDirections = new Vector3[lasersNumber];

        agent = gameObject.GetComponent<NavMeshAgent>();
        targetPosition = null;

        currentState = STATE.IDLE;

        idleTimer = idleTime;

        ParticleSystem spawnparticles = null;

        StormTrooperParticles myParticles = gameObject.GetComponent<StormTrooperParticles>();
        if (myParticles != null)
        {
            spawnparticles = myParticles.spawn;
        }

        if (spawnparticles != null)
        {
           // Debug.Log("PLAY SPAWN!!!");
            spawnparticles.Play();
        }
        else
        {
            //Debug.Log("CAN'T PLAY SPAWN!!!"); 
        }
        if (hit != null)
        {
            hitParticle = hit.GetComponent<ParticleSystem>();
        }

        rotationSpeed = shotTotalAngle * Mathf.Deg2RRad / shotTime;
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
                inputsList.Add(INPUT.IN_LOAD);
            }
        }

        if (loadTimer > 0.0f)
        {
            loadTimer -= myDeltaTime;

            if (loadTimer <= 0.0f)
            {
                inputsList.Add(INPUT.IN_SHOOT);
            }
        }

        //if (currentState == STATE.SHOOT)
        //{
        //    if (Mathf.Distance(gameObject.transform.globalPosition, agent.GetDestination()) <= agent.stoppingDistance) //USE THIS IF WE WANT TO TAKE INTO ACCOUNT THE FINAL ANGLE LIKE STOPPPING DISTANCE
        //    {
        //        inputsList.Add(INPUT.IN_IDLE);
        //    }
        //}

        if (damageCurrentTimer < damageMaxTimer)
            damageCurrentTimer += myDeltaTime;

        if (shotTimer > 0.0f)
        {
            shotTimer -= myDeltaTime;

            if (shotTimer <= 0.0f)
            {
                inputsList.Add(INPUT.IN_IDLE);
            }
        }
    }

    //All events from outside the stormtrooper
    private void ProcessExternalInput()
    {
    
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
                        case INPUT.IN_LOAD:
                            currentState = STATE.LOAD;
                            StartLoad();
                            break;

                        case INPUT.IN_DIE:
                            currentState = STATE.DIE;
                            StartDie();
                            break;
                    }
                    break;

                case STATE.LOAD:
                    switch (input)
                    {
                        case INPUT.IN_SHOOT:
                            currentState = STATE.SHOOT;
                            LoadEnd();
                            StartShoot();
                            break;

                        case INPUT.IN_DIE:
                            currentState = STATE.DIE;
                            LoadEnd();
                            StartDie();
                            break;
                    }
                    break;

                case STATE.SHOOT:
                    switch (input)
                    {
                        case INPUT.IN_IDLE:
                            currentState = STATE.IDLE;
                            ShootEnd();
                            StartIdle();
                            break;

                        case INPUT.IN_DIE:
                            currentState = STATE.DIE;
                            ShootEnd();
                            StartDie();
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
            case STATE.LOAD:
                UpdateLoad();
                break;
            case STATE.SHOOT:
                UpdateShoot();
                break;
            case STATE.DIE:
                UpdateDie();
                break;
            default:
                Debug.Log("NEED TO ADD STATE TO CORE");
                break;
        }
    }

    #region IDLE
    private void StartIdle()
    {
        //Debug.Log("TURRET IDLE");
        idleTimer = idleTime;
    }
    #endregion

    #region LOAD
    private void StartLoad()
    {
        //Debug.Log("TURRET LOAD");
        loadTimer = loadTime;
        Audio.PlayAudio(gameObject, "Play_Turret_Shot_Charge");
    }
    private void UpdateLoad()
    {

    }
    private void LoadEnd()
    {
        Audio.StopAudio(gameObject);
    }
    #endregion

    #region SHOOT
    private void StartShoot()
    {
        //Debug.Log("TURRET SHOOT");
        shotTimer = shotTime;
        Audio.PlayAudio(gameObject, "Play_Turret_Shot");
        Audio.PlayAudio(gameObject, "Play_Turret_Charge"); 
    }

    private void UpdateShoot()
    {
        angle += rotationSpeed * myDeltaTime;
        gameObject.transform.localRotation = Quaternion.RotateAroundAxis(Vector3.up, angle);

        //LASER ROTATION
        CalculateLaserRotation();
    }
    private void ShootEnd()
    {
        Audio.StopAudio(gameObject);
    }

    private void CalculateLaserRotation()
    {

        float angleIncrement = 360 / lasersNumber;
        for (int i = 0; i < lasersNumber; i++)
        {
            ////Quaternion rotation = 
            Quaternion q = Quaternion.RotateAroundAxis(new Vector3(0, 1, 0), (angleIncrement * i) * 0.0174532925f);
            Vector3 v = gameObject.transform.GetForward() /** laserRange*/;

            // Do the math
            laserDirections[i] = Vector3.RotateAroundQuaternion(q, v);
        }


        for (int i = 0; i < laserDirections.Length; i++)
        {
            float hitDistance = 0;
            GameObject hit = InternalCalls.RayCast(gameObject.transform.globalPosition + Vector3.up + (laserDirections[i] * laserOffser), laserDirections[i], laserRange, ref hitDistance);
            if (hit != null)
            {
                PlayerHealth health = hit.GetComponent<PlayerHealth>();
                if (health != null && hit.CompareTag("Player") && damageCurrentTimer >= damageMaxTimer)
                {
                    //Debug.Log("Hit player");
                    health.TakeDamage((int)this.damage);
                    damageCurrentTimer = 0.0f;
                }
            }

            InternalCalls.DrawRay(gameObject.transform.globalPosition + Vector3.up /*+ (laserDirections[i] * laserOffser)*/, gameObject.transform.globalPosition + Vector3.up + (laserDirections[i] * (hitDistance != 0 ? (hitDistance + laserOffser) : laserRange)), new Vector3(1, 0, 0));
            //Debug.Log(laserPoints[i].ToString());
        }
    }
    #endregion

    #region DIE
    private void StartDie()
    {
        //Debug.Log("TURRET DIE");
        dieTimer = dieTime;

        if (explosion != null && wave != null)
        {
            //Debug.Log("Want to play particles");
            partExp = explosion.GetComponent<ParticleSystem>();
            partWave = wave.GetComponent<ParticleSystem>();
        }

        if (partExp != null)
            partExp.Play();

        if (partWave != null)
            partWave.Play();

        if (mesh != null)
            InternalCalls.Destroy(mesh);

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
            dieTimer -= myDeltaTime;

            if (dieTimer <= 0.0f)
            {
                Die();
            }
        }
    }

    private void Die()
    {
        Counter.SumToCounterType(Counter.CounterTypes.ENEMY_LASER_TURRET);
        EnemyManager.RemoveEnemy(gameObject);

        DropCoins();

        Core.instance.gameObject.GetComponent<PlayerHealth>().TakeDamage(-PlayerHealth.healWhenKillingAnEnemy);
        InternalCalls.Destroy(gameObject);
    }

    #endregion

    public void OnCollisionEnter(GameObject collidedGameObject)
    {
        if (collidedGameObject.CompareTag("Bullet"))
        {
            if (Core.instance != null)
                if (Core.instance.HasStatus(STATUS_TYPE.PRIM_SLOW))
                    AddStatus(STATUS_TYPE.SLOWED, STATUS_APPLY_TYPE.BIGGER_PERCENTAGE, Core.instance.GetStatusData(STATUS_TYPE.PRIM_SLOW).severity / 100, 1, false);
            BH_Bullet bullet = collidedGameObject.GetComponent<BH_Bullet>();
            if (bullet != null)
            {

                TakeDamage(bullet.GetDamage());
                // healthPoints -= collidedGameObject.GetComponent<BH_Bullet>().damage;
            }
            Audio.PlayAudio(gameObject, "Play_Stormtrooper_Hit");

            if (Core.instance.hud != null)
            {
                Core.instance.hud.GetComponent<HUD>().AddToCombo(25, 0.95f);
            }

            if (currentState != STATE.DIE && healthPoints <= 0.0f)
            {
                inputsList.Add(INPUT.IN_DIE);
            }

        }
        else if (collidedGameObject.CompareTag("ChargeBullet"))
        {
            ChargedBullet bullet = collidedGameObject.GetComponent<ChargedBullet>();

            if (bullet != null)
            {
                TakeDamage(bullet.GetDamage());
                //healthPoints -= bullet.damage;
                this.AddStatus(STATUS_TYPE.ENEMY_DAMAGE_DOWN, STATUS_APPLY_TYPE.BIGGER_PERCENTAGE, 0.5f, 3.5f);
            }

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

    public override void TakeDamage(float damage)
    {
        Debug.Log("Turret Takes damage");
        healthPoints -= damage;
        hitParticle.Play();
        if (currentState != STATE.DIE)
        {
            if (healthPoints <= 0.0f)
                inputsList.Add(INPUT.IN_DIE);
        }

    }

    public void OnDestroy()
    {
        EnemyManager.RemoveEnemy(this.gameObject);
    }
}
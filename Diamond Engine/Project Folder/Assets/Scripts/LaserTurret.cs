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

    public GameObject hitParticles = null;

    //Action times
    public float idleTime = 0.0f;
    public float loadTime = 0.0f;
    public float shotTime = 0.0f;
    private float dieTime = 0.0f;
    public float feedbackTime = 0.0f;

    //Speeds
    private float angle = 0.0f;
    public float rotationSpeed = 0.0f;

    //Ranges
    public float laserRange = 0.0f;

    //Timers
    private float idleTimer = 0.0f;
    private float loadTimer = 0.0f;
    private float shotTimer = 0.0f;
    private float dieTimer = 0.0f;
    private float feedbackTimer = 0.0f;

    private Vector3[] laserPoints;
    public int lasersNumber = 4;

    public void Awake()
    {

        laserPoints = new Vector3[lasersNumber];
        for (int i = 0; i < lasersNumber; i++)
        {
            laserPoints[i] = gameObject.transform.globalPosition + gameObject.transform.GetForward();
        }

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
                inputsList.Add(INPUT.IN_LOAD);
            }
        }

        if (loadTimer > 0.0f)
        {
            loadTimer -= Time.deltaTime;

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

        if (shotTimer > 0.0f)
        {
            shotTimer -= Time.deltaTime;

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
        Debug.Log("TURRET IDLE");
        idleTimer = idleTime;
        Animator.Play(gameObject, "SK_Idle");
    }
    #endregion

    #region LOAD
    private void StartLoad()
    {
        Debug.Log("TURRET LOAD");
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
        Debug.Log("TURRET SHOOT");
        shotTimer = shotTime;
        Audio.PlayAudio(gameObject, "Play_Turret_Shot");
        Audio.PlayAudio(gameObject, "Play_Turret_Charge"); 
    }

    private void UpdateShoot()
    {
        angle += rotationSpeed;
        gameObject.transform.localRotation = Quaternion.RotateAroundAxis(Vector3.up, angle);

        //LASER ROTATION
        CalculateLaserRotation();
    }
    private void ShootEnd()
    {
        Audio.StopAudio(gameObject);
    }
    private void Shoot()
    {
        //GameObject bullet = InternalCalls.CreatePrefab("Library/Prefabs/1635392825.prefab", shootPoint.transform.globalPosition, shootPoint.transform.globalRotation, shootPoint.transform.globalScale);
        //bullet.GetComponent<BH_Bullet>().damage = damage;
        //visualFeedback = InternalCalls.CreatePrefab("Library/Prefabs/203996773.prefab", player.transform.globalPosition, gameObject.transform.globalRotation, new Vector3(1.0f, 1.0f, 1.0f));
        //Animator.Play(gameObject, "SK_Shoot");
        Audio.PlayAudio(gameObject, "PLay_Blaster_Stormtrooper");
        //shotTimes++;
    }
    private void CalculateLaserRotation()
    {

        for (int i = 0; i < laserPoints.Length; i++)
        {
            InternalCalls.DrawRay(this.gameObject.transform.globalPosition, laserPoints[i], new Vector3(1, 0, 0));
            //Debug.Log(laserPoints[i].ToString());
        }

        //laser1.transform.localRotation = gameObject.transform.localRotation;
        //laser3.transform.localRotation = gameObject.transform.localRotation + Quaternion.RotateAroundAxis(Vector3.up, 1.57f);
        //laser3.transform.localRotation = gameObject.transform.localRotation + Quaternion.RotateAroundAxis(Vector3.up, 3.14f);
        //laser3.transform.localRotation = gameObject.transform.localRotation + Quaternion.RotateAroundAxis(Vector3.up, 4.71f);
    }
    #endregion

    #region DIE
    private void StartDie()
    {
        Debug.Log("TURRET DIE");
        dieTimer = dieTime;
        //Audio.StopAudio(gameObject);

        //Animator.Play(gameObject, "ST_Die", 1.0f);

        Audio.PlayAudio(gameObject, "Play_Stormtrooper_Death");
        Audio.PlayAudio(gameObject, "Play_Mando_Kill_Voice");

        ParticleSystem dead = null;
        ParticleSystem wave = null;
        ParticleSystem souls = null;

        StormTrooperParticles myParticles = gameObject.GetComponent<StormTrooperParticles>();
        if (myParticles != null)
        {
            dead = myParticles.dead;
            wave = myParticles.wave;
            souls = myParticles.souls;
        }

        if (dead != null)
        {
            dead.Play();
        }
        if (wave != null)
        {
            wave.Play();
        }
        if (souls != null)
        {
            souls.Play();
        }

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
        InternalCalls.Destroy(gameObject);
    }

    #endregion

    public void OnCollisionEnter(GameObject collidedGameObject)
    {
        if (collidedGameObject.CompareTag("Bullet"))
        {
            healthPoints -= collidedGameObject.GetComponent<BH_Bullet>().damage;

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
        else if (collidedGameObject.CompareTag("Grenade"))
        {
            //healthPoints -= collidedGameObject.GetComponent<smallGrenade>().damage;
            healthPoints -= 5; //TODO: Hardcoded value, talk with adria

            if (Core.instance.hud != null)
            {
                Core.instance.hud.GetComponent<HUD>().AddToCombo(8, 1.5f);
            }

            if (currentState != STATE.DIE && healthPoints <= 0.0f)
            {
                inputsList.Add(INPUT.IN_DIE);
            }

        }
    }

    public void OnTriggerEnter(GameObject triggeredGameObject)
    {
    }
}
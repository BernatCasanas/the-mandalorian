using System;
using DiamondEngine;

using System.Collections.Generic;


public class Wampa : DiamondComponent
{
    enum WAMPA_STATE : int
    {
        NONE = -1,
        SEARCH_STATE,
        FOLLOW,
        WANDER,
        FAST_RUSH,
        SLOW_RUSH,
        RUSH_STUN,
        PROJECTILE,
        DEAD
    }

    enum WAMPA_INPUT : int
    {
        NONE = -1,
        IN_FOLLOW,
        IN_FOLLOW_END,
        IN_WANDER,
        IN_WANDER_END,
        IN_FAST_RUSH,
        IN_FAST_RUSH_END,
        IN_SLOW_RUSH,
        IN_SLOW_RUSH_END,
        IN_RUSH_STUN,
        IN_RUSH_STUN_END,
        IN_PROJECTILE,
        IN_PROJECTILE_END,
        IN_DEAD
    }

    private NavMeshAgent agent = null;
    Random randomNum = new Random();

    private WAMPA_STATE currentState = WAMPA_STATE.FOLLOW;
    private List<WAMPA_INPUT> inputsList = new List<WAMPA_INPUT>();

    public float slerpSpeed = 5.0f;

    //Public Variables
    public float probFollow = 60.0f;
    public float probWander = 40.0f;
    public GameObject projectilePoint = null;
    public float distanceProjectile = 15.0f;
    public float wanderRange = 7.5f;


    //Private Variables
    private bool resting = false;
    private bool firstShot = false;

    //Stats
    public float healthPoints = 1920.0f;
    public float speed = 3.5f;
    public float fastRushSpeed = 14.0f;
    public float slowRushSpeed = 7.0f;

    //Timers
    public float walkingTime = 4.0f;
    private float walkingTimer = 0.0f;
    public float fastChasingTime = 0.5f;
    private float fastChasingTimer = 0.0f;
    public float slowChasingTime = 3.5f;
    private float slowChasingTimer = 0.0f;
    public float shootingTime = 1.0f;
    private float shootingTimer = 0.0f;
    public float dieTime = 2.0f;
    private float dieTimer = 0.0f;
    public float restingTime = 4.0f;
    private float restingTimer = 0.0f;

    //Atacks
    public float projectileAngle = 30.0f;
    public float projectileRange = 6.0f;
    public float projectileDamage = 10.0f;
    public float rushDamage = 15.0f;

    public void Awake()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
        if (agent == null)
            Debug.Log("Null agent, add a NavMeshAgent Component");

        Animator.Play(gameObject, "");
        Audio.PlayAudio(gameObject, "");
        Counter.roomEnemies++;  // Just in case
        EnemyManager.AddEnemy(gameObject);
    }

    public void Update()
    {
        ProcessInternalInput();
        ProcessExternalInput();
        ProcessState();

        UpdateState();
    }

    private void ProcessInternalInput()
    {
        if (walkingTimer > 0)
        {
            walkingTimer -= Time.deltaTime;

            if (walkingTimer <= 0)
            {
                if (currentState == WAMPA_STATE.FOLLOW)
                    inputsList.Add(WAMPA_INPUT.IN_FOLLOW_END);
                else if (currentState == WAMPA_STATE.WANDER)
                    inputsList.Add(WAMPA_INPUT.IN_WANDER_END);
            }
        }

        if (fastChasingTimer > 0)
        {
            fastChasingTimer -= Time.deltaTime;

            if (fastChasingTimer <= 0)
            {
                inputsList.Add(WAMPA_INPUT.IN_FAST_RUSH_END);
            }
        }

        if (slowChasingTimer > 0)
        {
            slowChasingTimer -= Time.deltaTime;

            if (slowChasingTimer <= 0)
            {
                inputsList.Add(WAMPA_INPUT.IN_SLOW_RUSH_END);
            }
        }

        if (restingTimer > 0)
        {
            restingTimer -= Time.deltaTime;

            if (restingTimer <= 0)
            {
                resting = false;
            }
        }
    }

    private void ProcessExternalInput()
    {
        if (currentState == WAMPA_STATE.WANDER && Mathf.Distance(gameObject.transform.globalPosition, agent.GetDestination()) <= agent.stoppingDistance)
        {
            agent.CalculateRandomPath(gameObject.transform.globalPosition, wanderRange);
        }
    }

    private void ProcessState()
    {
        while (inputsList.Count > 0)
        {
            WAMPA_INPUT input = inputsList[0];

            switch (currentState)
            {
                case WAMPA_STATE.NONE:
                    Debug.Log("WAMPA ERROR STATE");
                    break;

                case WAMPA_STATE.SEARCH_STATE:
                    switch (input)
                    {
                        case WAMPA_INPUT.IN_PROJECTILE:
                            currentState = WAMPA_STATE.PROJECTILE;
                            StartProjectile();
                            break;

                        case WAMPA_INPUT.IN_FOLLOW:
                            currentState = WAMPA_STATE.FOLLOW;
                            StartFollowing();
                            break;
                        case WAMPA_INPUT.IN_WANDER:
                            currentState = WAMPA_STATE.WANDER;
                            StartWander();
                            break;
                    }
                    break;

                case WAMPA_STATE.FOLLOW:
                    switch (input)
                    {
                        case WAMPA_INPUT.IN_FOLLOW_END:
                            currentState = WAMPA_STATE.SEARCH_STATE;
                            EndFollowing();
                            break;
                        case WAMPA_INPUT.IN_DEAD:
                            currentState = WAMPA_STATE.SEARCH_STATE;
                            StartDie();
                            break;
                    }
                    break;
                case WAMPA_STATE.PROJECTILE:
                    switch (input)
                    {
                        case WAMPA_INPUT.IN_PROJECTILE_END:
                            currentState = WAMPA_STATE.SEARCH_STATE;
                            EndProjectile();
                            break;
                        case WAMPA_INPUT.IN_DEAD:
                            currentState = WAMPA_STATE.SEARCH_STATE;
                            StartDie();
                            break;
                    }
                    break;
                case WAMPA_STATE.FAST_RUSH:
                    switch (input)
                    {
                        case WAMPA_INPUT.IN_FAST_RUSH_END:
                            currentState = WAMPA_STATE.SLOW_RUSH;
                            StartSlowRush();
                            break;
                        case WAMPA_INPUT.IN_DEAD:
                            currentState = WAMPA_STATE.SEARCH_STATE;
                            StartDie();
                            break;
                    }
                    break;
                case WAMPA_STATE.RUSH_STUN:
                    switch (input)
                    {
                        case WAMPA_INPUT.IN_RUSH_STUN_END:
                            currentState = WAMPA_STATE.SEARCH_STATE;
                            EndRushStun();
                            break;
                        case WAMPA_INPUT.IN_DEAD:
                            currentState = WAMPA_STATE.SEARCH_STATE;
                            StartDie();
                            break;
                    }
                    break;
                case WAMPA_STATE.WANDER:
                    switch (input)
                    {
                        case WAMPA_INPUT.IN_WANDER_END:
                            currentState = WAMPA_STATE.SEARCH_STATE;
                            EndWander();
                            break;
                        case WAMPA_INPUT.IN_DEAD:
                            currentState = WAMPA_STATE.SEARCH_STATE;
                            StartDie();
                            break;
                    }
                    break;

                default:
                    Debug.Log("NEED TO ADD STATE TO WAMPA");
                    break;
            }
            inputsList.RemoveAt(0);
        }
    }
    private void UpdateState()
    {
        switch (currentState)
        {
            case WAMPA_STATE.NONE:
                Debug.Log("Error Wampa State");
                break;
            case WAMPA_STATE.DEAD:
                UpdateDie();
                break;
            case WAMPA_STATE.FOLLOW:
                UpdateFollowing();
                break;
            case WAMPA_STATE.PROJECTILE:
                UpdateProjectile();
                break;
            case WAMPA_STATE.FAST_RUSH:
                UpdateFastRush();
                break;
            case WAMPA_STATE.RUSH_STUN:
                UpdateRushStun();
                break;
            case WAMPA_STATE.WANDER:
                UpdateWander();
                break;
            case WAMPA_STATE.SEARCH_STATE:
                SelectAction();
                break;
        }
    }

    private void SelectAction()
    {
        if (resting)
        {
            int decision = randomNum.Next(1, 100);
            if (decision <= 60)
                inputsList.Add(WAMPA_INPUT.IN_FOLLOW);
            else
                inputsList.Add(WAMPA_INPUT.IN_WANDER);
        }
        else
        {
            if (Mathf.Distance(gameObject.transform.globalPosition, Core.instance.gameObject.transform.globalPosition) >= distanceProjectile)
                inputsList.Add(WAMPA_INPUT.IN_PROJECTILE);
            else
                inputsList.Add(WAMPA_INPUT.IN_FAST_RUSH);
        }
    }


    #region PROJECTILE
    private void StartProjectile()
    {
        Debug.Log("Starting Throwing Projectile");
        shootingTimer = shootingTime;
        firstShot = true;
    }
    private void UpdateProjectile()
    {
        if (shootingTimer > 0.0f)
        {
            shootingTimer -= Time.deltaTime;
            if (shootingTimer < 0.0f)
            {
                if (projectilePoint != null)
                {
                    Vector3 pos = projectilePoint.transform.globalPosition;
                    Quaternion rot = projectilePoint.transform.globalRotation;
                    Vector3 scale = new Vector3(1, 1, 1);

                    GameObject projectile = InternalCalls.CreatePrefab("Library/Prefabs/tocreate.prefab", pos, rot, scale);
                    projectile.GetComponent<RancorProjectile>().targetPos = Core.instance.gameObject.transform.globalPosition;
                    if (firstShot)
                    {
                        shootingTimer = shootingTime;
                        firstShot = false;
                    }
                }
            }
        }
    }

    private void EndProjectile()
    {

    }
    #endregion

    #region RUSH
    private void StartFastRush()
    {
        fastChasingTimer = fastChasingTime;
        Debug.Log("Fast Rush");
    }
    private void UpdateFastRush()
    {
        agent.CalculatePath(gameObject.transform.globalPosition, Core.instance.gameObject.transform.globalPosition);
        LookAt(agent.GetDestination());
        agent.MoveToCalculatedPos(fastRushSpeed);
    }

    private void EndFastRush()
    {

    }

    private void StartSlowRush()
    {
        slowChasingTimer = slowChasingTime;
        Debug.Log("Slow Rush");
    }
    private void UpdateSlowRush()
    {
        agent.CalculatePath(gameObject.transform.globalPosition, Core.instance.gameObject.transform.globalPosition);
        LookAt(agent.GetDestination());
        agent.MoveToCalculatedPos(slowRushSpeed);
    }

    private void EndSlowRush()
    {

    }
    #endregion

    #region RUSH STUN
    private void StartRushStun()
    {

    }
    private void UpdateRushStun()
    {

    }

    private void EndRushStun()
    {

    }
    #endregion

    #region FOLLOW
    private void StartFollowing()
    {
        walkingTimer = walkingTime;
    }
    private void UpdateFollowing()
    {
        agent.CalculatePath(gameObject.transform.globalPosition, Core.instance.gameObject.transform.globalPosition);
        LookAt(agent.GetDestination());
        agent.MoveToCalculatedPos(speed);
        Debug.Log("Following player");
        Debug.Log(walkingTimer.ToString());
    }

    private void EndFollowing()
    {

    }
    #endregion

    #region WANDER
    private void StartWander()
    {
        walkingTimer = walkingTime;
        agent.CalculateRandomPath(gameObject.transform.globalPosition, wanderRange);
        Debug.Log("Wander");
    }
    private void UpdateWander()
    {
        LookAt(agent.GetDestination());
        agent.MoveToCalculatedPos(speed);
        Debug.Log("Following player");
    }

    private void EndWander()
    {

    }
    #endregion

    #region DIE
    private void StartDie()
    {
        dieTimer = dieTime;
        Debug.Log("Dying");
    }
    private void UpdateDie()
    {
        if (dieTimer > 0.0f)
        {
            dieTimer -= Time.deltaTime;

            if (dieTimer <= 0.0f)
            {
                EndDie();
            }
        }
    }

    private void EndDie()
    {
        Debug.Log("WAMPA DEAD");

        Counter.roomEnemies--;

        if (Counter.roomEnemies <= 0)
            Counter.allEnemiesDead = true;

        EnemyManager.RemoveEnemy(gameObject);
        InternalCalls.Destroy(gameObject);
    }
    #endregion

    public void LookAt(Vector3 pointToLook)
    {
        Vector3 direction = pointToLook - gameObject.transform.globalPosition;
        direction = direction.normalized;
        float angle = (float)Math.Atan2(direction.x, direction.z);

        if (Math.Abs(angle * Mathf.Rad2Deg) < 1.0f)
            return;

        Quaternion dir = Quaternion.RotateAroundAxis(Vector3.up, angle);

        float rotationSpeed = Time.deltaTime * slerpSpeed;

        Quaternion desiredRotation = Quaternion.Slerp(gameObject.transform.localRotation, dir, rotationSpeed);

        gameObject.transform.localRotation = desiredRotation;
    }

    public void OnCollisionEnter(GameObject collidedGameObject)
    {
        if (collidedGameObject.CompareTag("Bullet"))
        {
            healthPoints -= collidedGameObject.GetComponent<BH_Bullet>().damage;
            Debug.Log("Rancor HP: " + healthPoints.ToString());
            //damaged = 1.0f; this is HUD things

            if (Core.instance.hud != null)
            {
                Core.instance.hud.GetComponent<HUD>().AddToCombo(20, 1.0f);
            }

            if (currentState != WAMPA_STATE.DEAD && healthPoints <= 0.0f)
            {
                inputsList.Add(WAMPA_INPUT.IN_DEAD);
            }
        }
        else if (collidedGameObject.CompareTag("Grenade"))
        {
            bigGrenade bGrenade = collidedGameObject.GetComponent<bigGrenade>();
            smallGrenade sGrenade = collidedGameObject.GetComponent<smallGrenade>();

            if (bGrenade != null)
                healthPoints -= bGrenade.GetDamage();

            if (sGrenade != null)
                healthPoints -= sGrenade.damage;

            if (Core.instance.hud != null)
            {
                Core.instance.hud.GetComponent<HUD>().AddToCombo(20, 0.5f);
            }

            if (currentState != WAMPA_STATE.DEAD && healthPoints <= 0.0f)
            {
                inputsList.Add(WAMPA_INPUT.IN_DEAD);
            }
        }
        else if (collidedGameObject.CompareTag("WorldLimit"))
        {
            if (currentState != WAMPA_STATE.DEAD)
            {
                inputsList.Add(WAMPA_INPUT.IN_DEAD);
            }
        }
        else if (collidedGameObject.CompareTag("Wall"))
        {
            if (currentState == WAMPA_STATE.FAST_RUSH || currentState == WAMPA_STATE.SLOW_RUSH)
                inputsList.Add(WAMPA_INPUT.IN_RUSH_STUN);
        }
        else if (collidedGameObject.CompareTag("Player"))
        {
            if (currentState == WAMPA_STATE.FAST_RUSH || currentState == WAMPA_STATE.SLOW_RUSH)
            {
                inputsList.Add(WAMPA_INPUT.IN_SLOW_RUSH_END);
                PlayerHealth playerHealth = collidedGameObject.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage((int)rushDamage);
                }
            }
        }
    }
}
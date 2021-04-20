using System;
using DiamondEngine;

using System.Collections.Generic;

public class Rancor : DiamondComponent
{
    enum RANCOR_STATE : int
    {
        NONE = -1,
        SEARCH_STATE,
        FOLLOW,
        LOADING_RUSH,
        RUSH,
        RUSH_STUN,
        HAND_SLAM,
        PROJECTILE,
        MELEE_COMBO_HIT1,
        MELEE_COMBO_HIT2,
        MELEE_COMBO_HIT3,
        DEAD
    }

    enum RANCOR_INPUT : int
    {
        NONE = -1,
        IN_FOLLOW_SHORT,
        IN_FOLLOW_LONG,
        IN_FOLLOW_END,
        IN_LOADING_RUSH,
        IN_RUSH,
        IN_RUSH_END,
        IN_RUSH_STUN,
        IN_RUSH_STUN_END,
        IN_HAND_SLAM,
        IN_HAND_SLAM_END,
        IN_PROJECTILE,
        IN_PROJECTILE_END,
        IN_MELEE_COMBO_1HIT,
        IN_MELEE_COMBO_2HIT,
        IN_MELEE_COMBO_3HIT,
        IN_MELEE_COMBO_END,
        IN_DEAD
    }

    private NavMeshAgent agent = null;

    //private Vector3 targetPosition = null;
    private Vector3 targetDirection = null;

    //State
    private RANCOR_STATE currentState = RANCOR_STATE.FOLLOW;   //NEVER SET THIS VARIABLE DIRECTLLY, ALLWAYS USE INPUTS
                                                               //Setting states directlly will break the behaviour  -Jose
    private List<RANCOR_INPUT> inputsList = new List<RANCOR_INPUT>();
    Random randomNum = new Random();

    public GameObject hitParticles = null;

    public float slerpSpeed = 5.0f;

    //Stats
    public float healthPoints = 60.0f;          //IF INITAL HEALTH IS CHANGED, CHANGE MAX HEALTH AS WELL!
    public float maxHealthPoints = 60.0f;

    public int attackProbability = 66;  //FROM 1 TO A 100
    public int shortFollowProbability = 90; //FROM THE PREVIOS VALUE TO HERE

    public float meleeRange = 14.0f;
    public float longRange = 21.0f;

    //Follow
    public float shortFollowTime = 2.0f;
    public float longFollowTime = 4.0f;
    private float followTimer = 0.0f;
    public float followSpeed = 2.0f;

    public float meleeComboMovSpeed = 0.0f;

    public float jumpAttackSpeed = 2.0f;
    private Vector3 jumpAttackTarget;

    public float jumpDelayDuration = 0.0f;
    private float jumpDelayTimer = 0.0f;

    private bool startedJumping = false;

    private float meleeComboHit1Time = 0.0f;
    private float meleeComboHit2Time = 0.0f;
    private float meleeComboHit3Time = 0.0f;

    private float meleeCH1Timer = 0.0f;
    private float meleeCH2Timer = 0.0f;
    private float meleeCH3Timer = 0.0f;

    public float meleeCH1ColliderDuration = 0.0f;
    public float meleeCH2ColliderDuration = 0.0f;
    public float meleeComboHit3CollisionTimeToActivate = 0.0f;
    public float meleeComboHit3CollisionDuration= 0.0f;


    private float meleeCH1ColliderTimer = 0.0f;
    private float meleeCH2ColliderTimer = 0.0f;
    private float meleeCH3ColliderTimer = 0.0f;
    private float meleeCH3ColliderActiveTimer = 0.0f;

    private bool meleeHit3Haptic = false;


    //Projectile
    private float projectileTime = 0.0f;
    private float projectileTimer = 0.0f;

    public float prepareShotDuration = 0.5f;
    private float prepareShotTimer = 0.0f;

    public GameObject projectilePoint = null;
    private Vector3 target = new Vector3(0, 0, 0);

    //Hand slam
    private float handSlamTime = 0.0f;
    private float handSlamTimer = 0.0f;
    public GameObject handSlamHitBox = null;
    public GameObject handSlamWave = null;
    private float handSlamTimerToActivate = 0.0f;
    private float handSlamTimeToActivate = 1.0f;
    private bool activateWave = false;
    public float handSlamHapticStrenght = 10f;

    //Rush
    public float rushDamage = 10.0f;
    public float loadRushTime = 0.4f;
    private float loadRushTimer = 0.0f;

    public float rushSpeed = 15.0f;
    private float rushTime = 0.0f;
    private float rushTimer = 0.0f;

    private float rushStunDuration = 1.2f;
    private float rushStunTimer = 0.0f;

    private bool startRush = false;


    //Die
    private float dieTime = 0.0f;
    private float dieTimer = 0.0f;

    //Boss bar updating
    public GameObject boss_bar = null;
    public GameObject rancor_mesh = null;
    private float damaged = 0.0f;
    private float limbo_health = 0.0f;

    private bool start = false;

    private void Start()
    {
        followTimer = shortFollowTime;

        meleeComboHit1Time = Animator.GetAnimationDuration(gameObject, "RN_MeleeComboP1") - 0.016f;
        meleeComboHit2Time = Animator.GetAnimationDuration(gameObject, "RN_MeleeComboP2") - 0.016f;
        meleeComboHit3Time = Animator.GetAnimationDuration(gameObject, "RN_MeleeComboP3") - 0.016f;

        projectileTime = Animator.GetAnimationDuration(gameObject, "RN_ProjectileThrow") - 0.016f;

        handSlamTime = Animator.GetAnimationDuration(gameObject, "RN_HandSlam") - 0.016f;

        rushTime = Animator.GetAnimationDuration(gameObject, "RN_Rush") - 0.016f;
        Debug.Log("RUSH TiME: "+ rushTime.ToString());

        //rushStunDuration = Animator.GetAnimationDuration(gameObject, "RN_RushRecover") - 0.016f;

        dieTime = Animator.GetAnimationDuration(gameObject, "RN_Die") - 0.016f;

        Counter.SumToCounterType(Counter.CounterTypes.RANCOR);
        limbo_health = maxHealthPoints = healthPoints;
        damaged = 0.0f;
    }

    public void Awake()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();

        if (agent == null)
            Debug.Log("Null agent, add a NavMeshAgent Component");

        Animator.Play(gameObject, "RN_Idle");
        Audio.PlayAudio(gameObject, "Play_Rancor_Breath");
        Counter.roomEnemies++;  // If we had a manager...
        EnemyManager.AddEnemy(gameObject);
    }

    public void Update()
    {
        if (start == false)
        {
            Start();
            start = true;
        }

        ProcessInternalInput();
        ProcessExternalInput();
        ProcessState();

        UpdateState();
    }

    //Timers go here
    private void ProcessInternalInput()
    {
        if (followTimer > 0)
        {
            followTimer -= Time.deltaTime;

            if (followTimer <= 0)
            {
                inputsList.Add(RANCOR_INPUT.IN_FOLLOW_END);
            }
        }

        if (meleeCH1Timer > 0)
        {
            meleeCH1Timer -= Time.deltaTime;

            if (meleeCH1Timer <= 0)
                inputsList.Add(RANCOR_INPUT.IN_MELEE_COMBO_2HIT);
        }

        if (meleeCH2Timer > 0)
        {
            meleeCH2Timer -= Time.deltaTime;

            if (meleeCH2Timer <= 0)
                inputsList.Add(RANCOR_INPUT.IN_MELEE_COMBO_3HIT);
        }

        if (meleeCH3Timer > 0)
        {
            meleeCH3Timer -= Time.deltaTime;

            if (meleeCH3Timer <= 0)
                inputsList.Add(RANCOR_INPUT.IN_MELEE_COMBO_END);
        }

        if (projectileTimer > 0)
        {
            projectileTimer -= Time.deltaTime;

            if (projectileTimer <= 0)
                inputsList.Add(RANCOR_INPUT.IN_PROJECTILE_END);
        }


        if (handSlamTimer > 0)
        {
            handSlamTimer -= Time.deltaTime;

            if (handSlamTimer <= 0)
                inputsList.Add(RANCOR_INPUT.IN_HAND_SLAM_END);
        }

        if (loadRushTimer > 0)
        {
            loadRushTimer -= Time.deltaTime;

            if (loadRushTimer <= 0)
                inputsList.Add(RANCOR_INPUT.IN_RUSH);
        }

        if (rushTimer > 0)
        {
            rushTimer -= Time.deltaTime;

            if (rushTimer <= 0)
                inputsList.Add(RANCOR_INPUT.IN_RUSH_END);
        }


        if (rushStunTimer > 0)
        {
            rushStunTimer -= Time.deltaTime;

            if (rushStunTimer <= 0)
                inputsList.Add(RANCOR_INPUT.IN_RUSH_STUN_END);
        }
    }

    private void ProcessExternalInput()
    {

    }

    private void ProcessState()
    {
        while (inputsList.Count > 0)
        {
            RANCOR_INPUT input = inputsList[0];

            switch (currentState)
            {
                case RANCOR_STATE.NONE:
                    Debug.Log("CORE ERROR STATE");
                    break;

                case RANCOR_STATE.SEARCH_STATE:
                    switch (input)
                    {
                        case RANCOR_INPUT.IN_FOLLOW_SHORT:
                            currentState = RANCOR_STATE.FOLLOW;
                            StartShortFollow();
                            break;

                        case RANCOR_INPUT.IN_FOLLOW_LONG:
                            currentState = RANCOR_STATE.FOLLOW;
                            StartLongFollow();
                            break;

                        case RANCOR_INPUT.IN_LOADING_RUSH:
                            currentState = RANCOR_STATE.LOADING_RUSH;
                            StartLoadingRush();
                            break;

                        case RANCOR_INPUT.IN_HAND_SLAM:
                            currentState = RANCOR_STATE.HAND_SLAM;
                            StartHandSlam();
                            break;

                        case RANCOR_INPUT.IN_PROJECTILE:
                            currentState = RANCOR_STATE.PROJECTILE;
                            StartProjectile();
                            break;

                        case RANCOR_INPUT.IN_MELEE_COMBO_1HIT:
                            currentState = RANCOR_STATE.MELEE_COMBO_HIT1;
                            StartMCHit1();
                            break;

                        case RANCOR_INPUT.IN_DEAD:
                            currentState = RANCOR_STATE.DEAD;
                            StartDie();
                            break;
                    }
                    break;

                case RANCOR_STATE.FOLLOW:
                    switch (input)
                    {
                        case RANCOR_INPUT.IN_FOLLOW_END:
                            currentState = RANCOR_STATE.SEARCH_STATE;
                            EndFollow();
                            break;

                        case RANCOR_INPUT.IN_DEAD:
                            currentState = RANCOR_STATE.DEAD;
                            StartDie();
                            break;
                    }
                    break;

                case RANCOR_STATE.LOADING_RUSH:
                    switch (input)
                    {
                        case RANCOR_INPUT.IN_RUSH:
                            currentState = RANCOR_STATE.RUSH;
                            StartRush();
                            break;

                        case RANCOR_INPUT.IN_DEAD:
                            currentState = RANCOR_STATE.DEAD;
                            StartDie();
                            break;
                    }
                    break;

                case RANCOR_STATE.RUSH:
                    switch (input)
                    {
                        case RANCOR_INPUT.IN_RUSH_END:
                            currentState = RANCOR_STATE.SEARCH_STATE;
                            EndRush();
                            break;

                        case RANCOR_INPUT.IN_RUSH_STUN:
                            currentState = RANCOR_STATE.RUSH_STUN;
                            EndRush();
                            StartRushStun();
                            break;

                        case RANCOR_INPUT.IN_DEAD:
                            currentState = RANCOR_STATE.DEAD;
                            StartDie();
                            break;
                    }
                    break;

                case RANCOR_STATE.RUSH_STUN:
                    switch (input)
                    {
                        case RANCOR_INPUT.IN_RUSH_STUN_END:
                            currentState = RANCOR_STATE.SEARCH_STATE;
                            EndRushStun();
                            break;

                        case RANCOR_INPUT.IN_DEAD:
                            currentState = RANCOR_STATE.DEAD;
                            StartDie();
                            break;
                    }
                    break;

                case RANCOR_STATE.HAND_SLAM:
                    switch (input)
                    {
                        case RANCOR_INPUT.IN_HAND_SLAM_END:
                            currentState = RANCOR_STATE.SEARCH_STATE;
                            EndHandSlam();
                            break;

                        case RANCOR_INPUT.IN_DEAD:
                            currentState = RANCOR_STATE.DEAD;
                            StartDie();
                            break;
                    }
                    break;

                case RANCOR_STATE.PROJECTILE:
                    switch (input)
                    {
                        case RANCOR_INPUT.IN_PROJECTILE_END:
                            currentState = RANCOR_STATE.SEARCH_STATE;
                            EndProjectile();
                            break;

                        case RANCOR_INPUT.IN_DEAD:
                            currentState = RANCOR_STATE.DEAD;
                            StartDie();
                            break;
                    }
                    break;

                case RANCOR_STATE.MELEE_COMBO_HIT1:
                    switch (input)
                    {
                        case RANCOR_INPUT.IN_MELEE_COMBO_2HIT:
                            currentState = RANCOR_STATE.MELEE_COMBO_HIT2;
                            EndMCHit1();
                            StartMCHit2();
                            break;

                        case RANCOR_INPUT.IN_DEAD:
                            currentState = RANCOR_STATE.DEAD;
                            StartDie();
                            break;
                    }
                    break;

                case RANCOR_STATE.MELEE_COMBO_HIT2:
                    switch (input)
                    {
                        case RANCOR_INPUT.IN_MELEE_COMBO_3HIT:
                            currentState = RANCOR_STATE.MELEE_COMBO_HIT3;
                            EndMCHit2();
                            StartMCHit3();
                            break;

                        case RANCOR_INPUT.IN_DEAD:
                            currentState = RANCOR_STATE.DEAD;
                            StartDie();
                            break;
                    }
                    break;

                case RANCOR_STATE.MELEE_COMBO_HIT3:
                    switch (input)
                    {
                        case RANCOR_INPUT.IN_MELEE_COMBO_END:
                            currentState = RANCOR_STATE.SEARCH_STATE;
                            EndMCHit3();
                            break;

                        case RANCOR_INPUT.IN_DEAD:
                            currentState = RANCOR_STATE.DEAD;
                            StartDie();
                            break;
                    }
                    break;

                default:
                    Debug.Log("NEED TO ADD STATE TO RANCOR");
                    break;
            }
            inputsList.RemoveAt(0);
        }
    }

    private void UpdateState()
    {
        switch (currentState)
        {
            case RANCOR_STATE.NONE:
                Debug.Log("CORE ERROR STATE");
                break;

            case RANCOR_STATE.SEARCH_STATE:
                SelectAction();
                break;

            case RANCOR_STATE.FOLLOW:
                UpdateFollow();
                break;

            case RANCOR_STATE.LOADING_RUSH:
                UpdateLoadingRush();
                break;

            case RANCOR_STATE.RUSH:
                UpdateRush();
                break;

            case RANCOR_STATE.RUSH_STUN:
                UpdateRushStun();
                break;

            case RANCOR_STATE.HAND_SLAM:
                UpdateHandSlam();
                break;

            case RANCOR_STATE.PROJECTILE:
                UpdateProjectile();
                break;

            case RANCOR_STATE.MELEE_COMBO_HIT1:
                UpdateMCHit1();
                break;

            case RANCOR_STATE.MELEE_COMBO_HIT2:
                UpdateMCHit2();
                break;

            case RANCOR_STATE.MELEE_COMBO_HIT3:
                UpdateMCHit3();
                break;

            case RANCOR_STATE.DEAD:
                UpdateDie();
                break;
        }
        limbo_health = Mathf.Lerp(limbo_health, healthPoints, 0.01f);
        if (boss_bar != null)
        {
            boss_bar.GetComponent<Material>().SetFloatUniform("length_used", healthPoints / maxHealthPoints);
            boss_bar.GetComponent<Material>().SetFloatUniform("limbo", limbo_health / maxHealthPoints);
        }
        if (damaged > 0.01f)
        {
            damaged = Mathf.Lerp(damaged, 0.0f, 0.1f);
        }
        else
        {
            damaged = 0.0f;
        }
        if (rancor_mesh != null)
        {
            rancor_mesh.GetComponent<Material>().SetFloatUniform("damaged", damaged);
        }
    }

    private void SelectAction()
    {
        int decision = randomNum.Next(1, 100);

        Debug.Log("Decision value: " + decision.ToString());

        if (decision <= attackProbability)
        {
            //Do all distance checks
            float distance = Mathf.Distance(Core.instance.gameObject.transform.localPosition, gameObject.transform.localPosition);

            if (distance <= meleeRange)
            {
                decision = randomNum.Next(1, 100);

                if (decision <= 50)
                    inputsList.Add(RANCOR_INPUT.IN_MELEE_COMBO_1HIT);

                else
                    inputsList.Add(RANCOR_INPUT.IN_HAND_SLAM);
            }

            else if (distance > meleeRange && distance <= longRange)
            {
                decision = randomNum.Next(1, 100);

                if (decision <= 50)
                    inputsList.Add(RANCOR_INPUT.IN_PROJECTILE);

                else
                    inputsList.Add(RANCOR_INPUT.IN_LOADING_RUSH);
            }
            else
            {
                //Projectile
                inputsList.Add(RANCOR_INPUT.IN_PROJECTILE);
            }
        }
        else if (decision > attackProbability && decision <= shortFollowProbability)
            inputsList.Add(RANCOR_INPUT.IN_FOLLOW_SHORT);

        else if (decision > shortFollowProbability)
            inputsList.Add(RANCOR_INPUT.IN_FOLLOW_LONG);
    }


    #region MELEE_COMBO
    private void StartMCHit1()
    {
        meleeCH1Timer = meleeComboHit1Time;
        meleeCH1ColliderTimer = meleeCH1ColliderDuration;

        Animator.Play(gameObject, "RN_MeleeComboP1");
        Audio.PlayAudio(gameObject, "Play_Rancor_Melee_1rst_Punch");
        LookAt(Core.instance.gameObject.transform.globalPosition);
    }

    private void UpdateMCHit1()
    {
        Debug.Log("Combo hit 1");
        //TODO: Activate collider

        if (meleeCH1ColliderTimer > 0.0f)
        {
            meleeCH1ColliderTimer -= Time.deltaTime;

            if (meleeCH1ColliderTimer <= 0.0f)
            {
                InternalCalls.CreatePrefab("Library/Prefabs/1846472793.prefab", gameObject.transform.localPosition, gameObject.transform.localRotation, gameObject.transform.localScale);
            }
        }

        LookAt(Core.instance.gameObject.transform.globalPosition);
        gameObject.transform.localPosition += gameObject.transform.GetForward().normalized * meleeComboMovSpeed * Time.deltaTime;
    }

    private void EndMCHit1()
    {
    }


    private void StartMCHit2()
    {
        meleeCH2Timer = meleeComboHit2Time;
        meleeCH2ColliderTimer = meleeCH2ColliderDuration;

        Animator.Play(gameObject, "RN_MeleeComboP2");
        Audio.PlayAudio(gameObject, "Play_Rancor_Melee_2nd_Punch");

        LookAt(Core.instance.gameObject.transform.globalPosition);
    }

    private void UpdateMCHit2()
    {
        Debug.Log("Combo hit 2");
        if (meleeCH2ColliderTimer > 0.0f)
        {
            meleeCH2ColliderTimer -= Time.deltaTime;

            if (meleeCH2ColliderTimer <= 0.0f)
            {
                InternalCalls.CreatePrefab("Library/Prefabs/1846472793.prefab", gameObject.transform.localPosition, gameObject.transform.localRotation, gameObject.transform.localScale);
            }
        }

        LookAt(Core.instance.gameObject.transform.globalPosition);
        gameObject.transform.localPosition += gameObject.transform.GetForward().normalized * meleeComboMovSpeed * Time.deltaTime;
    }

    private void EndMCHit2()
    {
    }


    private void StartMCHit3()
    {
        meleeCH3Timer = meleeComboHit3Time;
        meleeCH3ColliderTimer = meleeComboHit3CollisionTimeToActivate;
        meleeCH3ColliderActiveTimer = meleeComboHit3CollisionDuration;

        Animator.Play(gameObject, "RN_MeleeComboP3");

        jumpAttackTarget = Core.instance.gameObject.transform.globalPosition;
        LookAt(jumpAttackTarget);

        jumpDelayTimer = jumpDelayDuration;

        meleeHit3Haptic = true;
    }

    private void UpdateMCHit3()
    {
        Debug.Log("Combo hit 3");

        if (meleeCH3ColliderTimer > 0.0f)
        {
            meleeCH3ColliderTimer -= Time.deltaTime;

            if (meleeCH3ColliderTimer <= 0.0f)
            {
                Audio.PlayAudio(gameObject, "Play_Rancor_Area_Impact");

                Vector3 pos = gameObject.transform.localPosition;
                pos.y += 3;

                InternalCalls.CreatePrefab("Library/Prefabs/376114835.prefab", pos, gameObject.transform.localRotation, gameObject.transform.localScale);
            }
           
        }

        if(meleeCH3Timer < 1.5f)
        {
            if (meleeHit3Haptic)
            {
                meleeHit3Haptic = false;
                Input.PlayHaptic(0.8f, 500);
                Debug.Log("Hpatic Jump");
            }
        }

        if (jumpDelayTimer > 0.0f)
        {
            jumpDelayTimer -= Time.deltaTime;

            if (jumpDelayTimer <= 0.0f)
                startedJumping = true;
        }

        if (startedJumping)
        {
            float x = Mathf.Lerp(gameObject.transform.localPosition.x, jumpAttackTarget.x, jumpAttackSpeed * Time.deltaTime);
            float z = Mathf.Lerp(gameObject.transform.localPosition.z, jumpAttackTarget.z, jumpAttackSpeed * Time.deltaTime);
            gameObject.transform.localPosition = new Vector3(x, gameObject.transform.localPosition.y, z);
            //TODO: Disable collider component
        }
        LookAt(jumpAttackTarget);
    }

    private void EndMCHit3()
    {
        startedJumping = false;
    }

    #endregion

    #region FOLLOW

    private void StartShortFollow()
    {
        //Start walk animation
        Animator.Play(gameObject, "RN_Walk");
        Audio.PlayAudio(gameObject, "PLay_Rancor_Footsteps");
        //Search point
        followTimer = shortFollowTime;


    }


    private void StartLongFollow()
    {
        //Start walk animation
        //Search point

        Animator.Play(gameObject, "RN_Walk");
        Audio.PlayAudio(gameObject, "PLay_Rancor_Footsteps");
        followTimer = longFollowTime;

    }


    private void UpdateFollow()
    {
        if (agent != null)
            agent.CalculatePath(gameObject.transform.globalPosition, Core.instance.gameObject.transform.globalPosition);

        //Move character
        if (agent != null && Mathf.Distance(gameObject.transform.globalPosition, agent.GetDestination()) <= agent.stoppingDistance)
        {
            LookAt(agent.GetDestination());
            agent.MoveToCalculatedPos(followSpeed);
        }

        Debug.Log("Following");
    }


    private void EndFollow()
    {
        Audio.StopAudio(gameObject);
        Animator.Play(gameObject, "RN_Idle");
        Debug.Log("End Following");
    }

    #endregion

    #region PROJECTILE

    private void StartProjectile()
    {
        projectileTimer = projectileTime;

        Animator.Play(gameObject, "RN_ProjectileThrow");
        Audio.PlayAudio(gameObject, "Play_Rancor_Throw");
        //add timer to spawn projectiles

        prepareShotTimer = prepareShotDuration;
    }

    private void UpdateProjectile()
    {
        if (prepareShotTimer > 0.0f)
        {
            prepareShotTimer -= Time.deltaTime;

            LookAt(Core.instance.gameObject.transform.globalPosition);

            if (prepareShotTimer <= 0.0f)
            {
                if (projectilePoint != null)
                {
                    Vector3 pos = projectilePoint.transform.globalPosition;
                    Vector3 scale = new Vector3(1f, 1f, 1f);

                    GameObject projectile = InternalCalls.CreatePrefab("Library/Prefabs/1225675544.prefab", pos, Quaternion.identity, scale);
                    projectile.GetComponent<RancorProjectile>().targetPos = Core.instance.gameObject.transform.globalPosition - new Vector3(-3f,-1f,0f);
                    GameObject projectile1 = InternalCalls.CreatePrefab("Library/Prefabs/1225675544.prefab", pos, Quaternion.identity, scale);
                    projectile1.GetComponent<RancorProjectile>().targetPos = Core.instance.gameObject.transform.globalPosition - new Vector3(3f,-1f,0f);
                    GameObject projectile2 = InternalCalls.CreatePrefab("Library/Prefabs/1225675544.prefab", pos, Quaternion.identity, scale);
                    projectile2.GetComponent<RancorProjectile>().targetPos = Core.instance.gameObject.transform.globalPosition - new Vector3(0f,-1f,0f);
                }
            }
        }

        Debug.Log("Projectile");
    }

    private void EndProjectile()
    {

    }

    #endregion

    #region HAND_SLAM

    private void StartHandSlam()
    {
        handSlamTimer = handSlamTime;
        Animator.Play(gameObject, "RN_HandSlam");
        Audio.PlayAudio(gameObject, "Play_Rancor_Hand_Slam");

        handSlamTimerToActivate = 0.0f;
        handSlamHitBox.GetComponent<RancorHandSlamHitCollider>().RestartCollider();
        activateWave = true;
       
    }


    private void UpdateHandSlam()
    {
      
        handSlamTimerToActivate += Time.deltaTime;
        if(handSlamTimerToActivate < 0.5f)
        {
            LookAt(Core.instance.gameObject.transform.globalPosition);
        }
        if(handSlamTimerToActivate > handSlamTimeToActivate)
        {
            if (handSlamHitBox == null)
                return;
            //Actiavate collider
            handSlamHitBox.Enable(true); //Enable collider not gameobject
            //Temporal while colliders can't set active
            //handSlamHitBox.transform.localPosition = handSlamHitBoxPos;
            //handSlamHitBox.transform.localRotation = Quaternion.RotateAroundAxis(Vector3.up, 3.14159f);
            //----

        }

        if (handSlamTimer < 0.6f)
        {
            if (handSlamHitBox == null)
                return;
            handSlamHitBox.Enable(false);


        }

        if (handSlamTimer < 0.6f)
        {
            if (activateWave)
            {
                InternalCalls.CreatePrefab("Library/Prefabs/1923485827.prefab", gameObject.transform.localPosition, gameObject.transform.localRotation, new Vector3(0.767f, 0.225f, 1.152f));
                Input.PlayHaptic(.8f, 350);
                activateWave = false;
            }
        }


        Debug.Log("Hand slam");
    }


    private void EndHandSlam()
    {


    }

    #endregion

    #region LOADING_RUSH
    private void StartLoadingRush()
    {
        loadRushTimer = loadRushTime;

        //Animator.Play(gameObject, "RN_LoadingRush"); //THIS ANIMATION DOESNT EXIST, BUT ITS NEEDED

    }
    private void UpdateLoadingRush()
    {
        Debug.Log("Loading Rush");

        LookAt(Core.instance.gameObject.transform.globalPosition); //RANCOR POINTING PLAYER WHILE LOADING
    }
    #endregion

    #region RUSH

    private void StartRush()
    {
        rushTimer = rushTime;
        Animator.Play(gameObject, "RN_Rush");
        Audio.PlayAudio(gameObject, "Play_Rancor_Hand_Slam");
        targetDirection = Core.instance.gameObject.transform.globalPosition - gameObject.transform.globalPosition; //RANCOR CALCULATES RUSH DIRECTION
        startRush = true;

    }


    private void UpdateRush()
    {
        Debug.Log("Rush");

        if (rushTimer < 2.9f)
        {
            if (startRush)
            {
                Audio.PlayAudio(gameObject, "Play_Rancor_Rush_Steps");
                startRush = false;
            }
            gameObject.transform.localPosition += targetDirection.normalized * rushSpeed * Time.deltaTime; //ADD SPEED IN RUSH DIRECTION
        }

    }


    private void EndRush()
    {
        Audio.StopAudio(gameObject);
    }


    private void StartRushStun()
    {
        rushStunTimer = rushStunDuration;
        Animator.Play(gameObject, "RN_RushRecover");
        Audio.PlayAudio(gameObject, "Play_Rancor_Recovery");
    }

    private void UpdateRushStun()
    {
        Debug.Log("Rush Stun");
        Debug.Log(rushStunTimer.ToString());
    }


    private void EndRushStun()
    {

    }

    #endregion

    #region DIE
    private void StartDie()
    {
        Audio.StopAudio(gameObject);

        dieTimer = dieTime;

        Animator.Play(gameObject, "RN_Die", 1.0f);

        Audio.PlayAudio(gameObject, "Play_Rancor_Death");
        //Audio.PlayAudio(gameObject, "Play_Mando_Voice");

        if (hitParticles != null)
            hitParticles.GetComponent<ParticleSystem>().Play();

        //RemoveFromEnemyList();
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

    public void Die()
    {
        Debug.Log("RANCOR DEAD");

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
            damaged = 1.0f;
            //CHANGE FOR APPROPIATE RANCOR HIT
            Audio.PlayAudio(gameObject, "Play_Rancor_Hit");

            if (Core.instance.hud != null)
            {
                Core.instance.hud.GetComponent<HUD>().AddToCombo(20, 1.0f);
            }

            if (currentState != RANCOR_STATE.DEAD && healthPoints <= 0.0f)
            {
                inputsList.Add(RANCOR_INPUT.IN_DEAD);
            }
        }
        else if (collidedGameObject.CompareTag("Grenade"))
        {
            bigGrenade bGrenade = collidedGameObject.GetComponent<bigGrenade>();
            smallGrenade sGrenade = collidedGameObject.GetComponent<smallGrenade>();

            if (bGrenade != null)
                healthPoints -= bGrenade.GetDamage() ;

            if (sGrenade != null)
                healthPoints -= sGrenade.damage;

            //CHANGE FOR APPROPIATE RANCOR HIT
            Audio.PlayAudio(gameObject, "Play_Rancor_Hit");

            if (Core.instance.hud != null)
            {
                Core.instance.hud.GetComponent<HUD>().AddToCombo(20, 0.5f);
            }

            if (currentState != RANCOR_STATE.DEAD && healthPoints <= 0.0f)
            {
                inputsList.Add(RANCOR_INPUT.IN_DEAD);
            }
        }
        else if (collidedGameObject.CompareTag("WorldLimit"))
        {
            if (currentState != RANCOR_STATE.DEAD)
            {
                inputsList.Add(RANCOR_INPUT.IN_DEAD);
            }
        }
        else if (collidedGameObject.CompareTag("Wall"))
        {
            if (currentState == RANCOR_STATE.RUSH)
                inputsList.Add(RANCOR_INPUT.IN_RUSH_STUN);
        }
        else if (collidedGameObject.CompareTag("Player"))
        {
            if (currentState == RANCOR_STATE.RUSH)
            {
                inputsList.Add(RANCOR_INPUT.IN_RUSH_END);
                PlayerHealth playerHealth = collidedGameObject.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage((int)rushDamage);
                }
            }
        }
    }
}
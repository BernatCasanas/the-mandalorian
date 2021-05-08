using System;
using DiamondEngine;

using System.Collections.Generic;

public class MoffGideon : Entity
{
    enum MOFFGIDEON_PHASE : int
    {
        NONE = -1,
        PHASE1,
        PHASE2
    }

    enum MOFFGIDEON_STATE : int
    {
        NONE = -1,
        SEARCH_STATE,
        CHARGE_THROW,
        THROW_SABER,
        RETRIEVE_SABER,
        DASH_FORWARD,
        MELEE_COMBO,
        DASH_BACKWARDS,
        PROJECTILE,
        SPAWN_ENEMIES,
        NEUTRAL,
        DEAD
    }

    enum MOFFGIDEON_INPUT : int
    {
        NONE = -1,
        IN_THROW_SABER,
        IN_THROW_SABER_END,
        IN_CHARGE_THROW,
        IN_CHARGE_THROW_END,
        IN_RETRIEVE_SABER,
        IN_RETRIEVE_SABER_END,
        IN_DASH_FORWARD,
        IN_DASH_FORWARD_END,
        IN_DASH_BACKWARDS,
        IN_DASH_BACKWARDS_END,
        IN_MELEE_COMBO,
        IN_MELEE_COMBO_END,
        IN_SPAWN_ENEMIES,
        IN_SPAWN_ENEMIES_END,
        IN_PROJECTILE,
        IN_PROJECTILE_END,
        IN_NEUTRAL,
        IN_NEUTRAL_END,
        IN_DEAD
    }

    private NavMeshAgent agent = null;
    public GameObject camera = null;

    //State
    private MOFFGIDEON_STATE currentState = MOFFGIDEON_STATE.NEUTRAL;
    private List<MOFFGIDEON_INPUT> inputsList = new List<MOFFGIDEON_INPUT>();
    private MOFFGIDEON_PHASE currentPhase = MOFFGIDEON_PHASE.PHASE1;

    Random randomNum = new Random();

    public GameObject hitParticles = null;

    public float slerpSpeed = 5.0f;

    private float damageMult = 1.0f;

    private bool start = false;

    //Stats
    public float healthPoints = 8500.0f;
    public float maxHealthPoints_fase1 = 4500.0f;
    public float maxHealthPoints_fase2 = 4000.0f;

    //Public Variables
    public float followSpeed = 3f;
    public float touchDamage = 10f;
    public float distance2Melee = 10f;
    public float probWanderP1 = 60f;
    public float probWanderP2 = 40f;
    public float probWander = 0f;
    public float radiusWander = 5f;
    public float distanceProjectile = 17f;
    public float dashSpeed = 10f;
    public float dashDistance = 10f;
    public float closerDistance = 5f;
    public float farDistance = 10f;
    public float furtherDistance = 15f;
    public GameObject spawner1 = null;
    public GameObject spawner2 = null;
    public GameObject spawner3 = null;
    public GameObject spawner4 = null;
    public GameObject sword = null;
    public GameObject shootPoint = null;

    //Private Variables
    private float damaged = 0.0f;
    private float currAnimationPlaySpd = 1f;
    private bool invencible = false;
    private bool wander = false;
    private bool ready2Spawn = false;
    private Vector3 targetDash = null;
    private bool justDashing = false;


    //Timers
    private float dieTimer = 0f;
    public float dieTime = 0.1f;
    private float neutralTimer = 0f;
    public float neutralTime = 5f;
    private float enemiesTimer = 0f;
    public float enemiesTime = 20f;
    private float comboTime = 0f;
    private float comboTimer = 0f;

    private void Start()
    {
        StartNeutral();
        //sword.DisableCollider();

        //comboTime = Animator.GetAnimationDuration(gameObject, "") - 0.016f;
    }
    
    public void Awake()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();

        InitEntity(ENTITY_TYPE.MOFF);

        damageMult = 1f;

        damaged = 0f;

        probWander = probWanderP1;

        if (EnemyManager.EnemiesLeft() > 0)
            EnemyManager.ClearList();

        EnemyManager.AddEnemy(gameObject);

        if (agent == null)
            Debug.Log("Null agent, add a NavMeshAgent Component");


    }

    public void Update()
    {
        if (!start)
        {
            Start();
            start = true;
        }

        myDeltaTime = Time.deltaTime * speedMult;

        UpdateStatuses();

        ProcessInternalInput();
        ProcessExternalInput();
        ProcessState();

        UpdateState();
    }

    //Timers go here
    private void ProcessInternalInput()
    {
        if (neutralTimer > 0)
        {
            neutralTimer -= myDeltaTime;

            if (neutralTimer <= 0)
                inputsList.Add(MOFFGIDEON_INPUT.IN_NEUTRAL_END);

        }

        if (enemiesTimer > 0)
        {
            enemiesTimer -= myDeltaTime;

            if (enemiesTimer <= 0)
                ready2Spawn = true;

        }

        if (comboTimer > 0)
        {
            comboTimer -= myDeltaTime;

            if (comboTimer <= 0)
                inputsList.Add(MOFFGIDEON_INPUT.IN_DASH_BACKWARDS);

        }
    }

    private void ProcessExternalInput()
    {
        if (currentState == MOFFGIDEON_STATE.SPAWN_ENEMIES && CheckDeathtroopers())
            inputsList.Add(MOFFGIDEON_INPUT.IN_NEUTRAL);

        if (currentState == MOFFGIDEON_STATE.NEUTRAL && Mathf.Distance(gameObject.transform.globalPosition, agent.GetDestination()) <= agent.stoppingDistance && wander)
            agent.CalculateRandomPath(gameObject.transform.globalPosition, radiusWander);

        if (currentState == MOFFGIDEON_STATE.NEUTRAL && Mathf.Distance(gameObject.transform.globalPosition, Core.instance.gameObject.transform.globalPosition) <= 1.0f && !wander)
            wander = true;

        if (currentState == MOFFGIDEON_STATE.DASH_FORWARD && Mathf.Distance(gameObject.transform.globalPosition, targetDash) <= 1f)
            inputsList.Add(MOFFGIDEON_INPUT.IN_MELEE_COMBO);

        if (currentState == MOFFGIDEON_STATE.DASH_FORWARD && Mathf.Distance(gameObject.transform.globalPosition, targetDash) >= dashDistance && justDashing)
        {
            inputsList.Add(MOFFGIDEON_INPUT.IN_NEUTRAL);
            justDashing = false;
        }

    }

    private void ProcessState()
    {
        while (inputsList.Count > 0)
        {
            MOFFGIDEON_INPUT input = inputsList[0];

            if (currentPhase == MOFFGIDEON_PHASE.PHASE1)
            {
                switch (currentState)
                {
                    case MOFFGIDEON_STATE.NONE:
                        Debug.Log("CORE ERROR STATE");
                        break;

                    case MOFFGIDEON_STATE.NEUTRAL:
                        switch (input)
                        {
                            case MOFFGIDEON_INPUT.IN_NEUTRAL_END:
                                currentState = MOFFGIDEON_STATE.SEARCH_STATE;
                                EndNeutral();
                                break;

                            case MOFFGIDEON_INPUT.IN_DEAD:
                                EndNeutral();
                                StartDie();
                                break;
                        }
                        break;


                    case MOFFGIDEON_STATE.SEARCH_STATE:
                        switch (input)
                        {
                            case MOFFGIDEON_INPUT.IN_SPAWN_ENEMIES:
                                currentState = MOFFGIDEON_STATE.SPAWN_ENEMIES;
                                EndNeutral();
                                StartSpawnEnemies();
                                break;

                            case MOFFGIDEON_INPUT.IN_PROJECTILE:
                                currentState = MOFFGIDEON_STATE.PROJECTILE;
                                EndNeutral();
                                StartProjectile();
                                break;

                            case MOFFGIDEON_INPUT.IN_DASH_FORWARD:
                                currentState = MOFFGIDEON_STATE.DASH_FORWARD;
                                EndNeutral();
                                StartDashForward();
                                break;
                        }
                        break;

                    case MOFFGIDEON_STATE.SPAWN_ENEMIES:
                        switch (input)
                        {
                            case MOFFGIDEON_INPUT.IN_SPAWN_ENEMIES_END:
                                currentState = MOFFGIDEON_STATE.SEARCH_STATE;
                                EndSpawnEnemies();
                                break;

                            case MOFFGIDEON_INPUT.IN_NEUTRAL:
                                currentState = MOFFGIDEON_STATE.NEUTRAL;
                                EndSpawnEnemies();
                                StartNeutral();
                                break;
                        }
                        break;

                    case MOFFGIDEON_STATE.PROJECTILE:
                        switch (input)
                        {
                            case MOFFGIDEON_INPUT.IN_PROJECTILE_END:
                                currentState = MOFFGIDEON_STATE.SEARCH_STATE;
                                EndProjectile();
                                break;

                            case MOFFGIDEON_INPUT.IN_NEUTRAL:
                                currentState = MOFFGIDEON_STATE.NEUTRAL;
                                EndProjectile();
                                StartNeutral();
                                break;

                            case MOFFGIDEON_INPUT.IN_DEAD:
                                EndProjectile();
                                StartDie();
                                break;
                        }
                        break;

                    case MOFFGIDEON_STATE.DASH_FORWARD:
                        switch (input)
                        {
                            case MOFFGIDEON_INPUT.IN_DASH_BACKWARDS_END:
                                currentState = MOFFGIDEON_STATE.MELEE_COMBO;
                                EndDashForward();
                                StartMeleeCombo();
                                break;

                            case MOFFGIDEON_INPUT.IN_DEAD:
                                EndProjectile();
                                StartDie();
                                break;
                        }
                        break;

                    case MOFFGIDEON_STATE.MELEE_COMBO:
                        switch (input)
                        {
                            case MOFFGIDEON_INPUT.IN_MELEE_COMBO_END:
                                currentState = MOFFGIDEON_STATE.DASH_BACKWARDS;
                                EndDashBackward();
                                StartDashBackward();
                                break;

                            case MOFFGIDEON_INPUT.IN_DEAD:
                                EndProjectile();
                                StartDie();
                                break;
                        }
                        break;

                    case MOFFGIDEON_STATE.DASH_BACKWARDS:
                        switch (input)
                        {
                            case MOFFGIDEON_INPUT.IN_DASH_BACKWARDS_END:
                                currentState = MOFFGIDEON_STATE.SEARCH_STATE;
                                EndDashBackward();
                                break;

                            case MOFFGIDEON_INPUT.IN_DEAD:
                                EndProjectile();
                                StartDie();
                                break;
                        }
                        break;

                    default:
                        Debug.Log("NEED TO ADD STATE TO MOFF GIDEON");
                        break;
                }
            }
            else if (currentPhase == MOFFGIDEON_PHASE.PHASE2)
            {
                switch (currentState)
                {
                    case MOFFGIDEON_STATE.NONE:
                        Debug.Log("CORE ERROR STATE");
                        break;

                    case MOFFGIDEON_STATE.NEUTRAL:
                        switch (input)
                        {
                            case MOFFGIDEON_INPUT.IN_NEUTRAL_END:
                                currentState = MOFFGIDEON_STATE.SEARCH_STATE;
                                EndNeutral();
                                break;

                            case MOFFGIDEON_INPUT.IN_DEAD:
                                EndNeutral();
                                StartDie();
                                break;
                        }
                        break;


                    case MOFFGIDEON_STATE.SEARCH_STATE:
                        switch (input)
                        {
                            case MOFFGIDEON_INPUT.IN_SPAWN_ENEMIES:
                                currentState = MOFFGIDEON_STATE.SPAWN_ENEMIES;
                                EndNeutral();
                                StartSpawnEnemies();
                                break;

                            case MOFFGIDEON_INPUT.IN_PROJECTILE:
                                currentState = MOFFGIDEON_STATE.PROJECTILE;
                                EndNeutral();
                                StartProjectile();
                                break;

                            case MOFFGIDEON_INPUT.IN_DASH_FORWARD:
                                currentState = MOFFGIDEON_STATE.DASH_FORWARD;
                                EndNeutral();
                                StartDashForward();
                                break;
                        }
                        break;

                    case MOFFGIDEON_STATE.SPAWN_ENEMIES:
                        switch (input)
                        {
                            case MOFFGIDEON_INPUT.IN_SPAWN_ENEMIES_END:
                                currentState = MOFFGIDEON_STATE.SEARCH_STATE;
                                EndSpawnEnemies();
                                break;

                            case MOFFGIDEON_INPUT.IN_NEUTRAL:
                                currentState = MOFFGIDEON_STATE.NEUTRAL;
                                EndSpawnEnemies();
                                StartNeutral();
                                break;

                            case MOFFGIDEON_INPUT.IN_DEAD:
                                EndSpawnEnemies();
                                StartDie();
                                break;
                        }
                        break;

                    case MOFFGIDEON_STATE.CHARGE_THROW:
                        switch (input)
                        {
                            case MOFFGIDEON_INPUT.IN_THROW_SABER:
                                currentState = MOFFGIDEON_STATE.THROW_SABER;
                                EndChargeThrow();
                                StartThrowSaber();
                                break;

                            case MOFFGIDEON_INPUT.IN_DEAD:
                                EndProjectile();
                                StartDie();
                                break;
                        }
                        break;

                    case MOFFGIDEON_STATE.THROW_SABER:
                        switch (input)
                        {
                            case MOFFGIDEON_INPUT.IN_RETRIEVE_SABER:
                                currentState = MOFFGIDEON_STATE.RETRIEVE_SABER;
                                EndThrowSaber();
                                StartRetrieveSaber();
                                break;

                            case MOFFGIDEON_INPUT.IN_DEAD:
                                EndThrowSaber();
                                StartDie();
                                break;
                        }
                        break;

                    case MOFFGIDEON_STATE.RETRIEVE_SABER:
                        switch (input)
                        {
                            case MOFFGIDEON_INPUT.IN_RETRIEVE_SABER_END:
                                currentState = MOFFGIDEON_STATE.NEUTRAL;
                                EndRetrieveSaber();
                                StartNeutral();
                                break;

                            case MOFFGIDEON_INPUT.IN_DEAD:
                                EndRetrieveSaber();
                                StartDie();
                                break;
                        }
                        break;

                    case MOFFGIDEON_STATE.DASH_FORWARD:
                        switch (input)
                        {
                            case MOFFGIDEON_INPUT.IN_DASH_BACKWARDS_END:
                                currentState = MOFFGIDEON_STATE.MELEE_COMBO;
                                EndDashForward();
                                StartMeleeCombo();
                                break;

                            case MOFFGIDEON_INPUT.IN_NEUTRAL:
                                currentState = MOFFGIDEON_STATE.NEUTRAL;
                                EndDashForward();
                                StartNeutral();
                                break;

                            case MOFFGIDEON_INPUT.IN_DEAD:
                                EndProjectile();
                                StartDie();
                                break;
                        }
                        break;

                    case MOFFGIDEON_STATE.MELEE_COMBO:
                        switch (input)
                        {
                            case MOFFGIDEON_INPUT.IN_MELEE_COMBO_END:
                                currentState = MOFFGIDEON_STATE.DASH_BACKWARDS;
                                EndDashBackward();
                                StartDashBackward();
                                break;

                            case MOFFGIDEON_INPUT.IN_DEAD:
                                EndProjectile();
                                StartDie();
                                break;
                        }
                        break;

                    case MOFFGIDEON_STATE.DASH_BACKWARDS:
                        switch (input)
                        {
                            case MOFFGIDEON_INPUT.IN_DASH_BACKWARDS_END:
                                currentState = MOFFGIDEON_STATE.SEARCH_STATE;
                                EndDashBackward();
                                break;

                            case MOFFGIDEON_INPUT.IN_DEAD:
                                EndProjectile();
                                StartDie();
                                break;
                        }
                        break;

                    default:
                        Debug.Log("NEED TO ADD STATE TO MOFF GIDEON");
                        break;
                }
            }
            inputsList.RemoveAt(0);
        }
    }

    private void UpdateState()
    {
        switch (currentState)
        {
            case MOFFGIDEON_STATE.NONE:
                Debug.Log("GIDEON ERROR STATE");
                break;

            case MOFFGIDEON_STATE.NEUTRAL:
                UpdateNeutral();
                break;

            case MOFFGIDEON_STATE.SEARCH_STATE:
                SelectAction();
                break;

            case MOFFGIDEON_STATE.SPAWN_ENEMIES:
                UpdateSpawnEnemies();
                break;

            case MOFFGIDEON_STATE.MELEE_COMBO:
                UpdateMeleeCombo();
                break;

            case MOFFGIDEON_STATE.DASH_BACKWARDS:
                UpdateDashBackward();
                break;

            case MOFFGIDEON_STATE.DASH_FORWARD:
                UpdateDashForward();
                break;

            case MOFFGIDEON_STATE.PROJECTILE:
                UpdateProjectile();
                break;

            case MOFFGIDEON_STATE.DEAD:
                UpdateDie();
                break;

            case MOFFGIDEON_STATE.CHARGE_THROW:
                UpdateChargeThrow();
                break;

            case MOFFGIDEON_STATE.THROW_SABER:
                UpdateThrowSaber();
                break;

            case MOFFGIDEON_STATE.RETRIEVE_SABER:
                UpdateRetrieveSaber();
                break;
        }
    }

    private void SelectAction()
    {
        if (currentPhase == MOFFGIDEON_PHASE.PHASE1)
        {
            if (ready2Spawn)
                inputsList.Add(MOFFGIDEON_INPUT.IN_SPAWN_ENEMIES);

            else if (Mathf.Distance(gameObject.transform.globalPosition, Core.instance.gameObject.transform.globalPosition) <= distanceProjectile)
                inputsList.Add(MOFFGIDEON_INPUT.IN_DASH_FORWARD);

            else
                inputsList.Add(MOFFGIDEON_INPUT.IN_PROJECTILE);
        }
        else
        {
            if (Mathf.Distance(gameObject.transform.globalPosition, Core.instance.gameObject.transform.globalPosition) <= closerDistance)
            {
                justDashing = true;
                inputsList.Add(MOFFGIDEON_INPUT.IN_DASH_FORWARD);
            }

            else if (Mathf.Distance(gameObject.transform.globalPosition, Core.instance.gameObject.transform.globalPosition) <= farDistance)
                inputsList.Add(MOFFGIDEON_INPUT.IN_CHARGE_THROW);

            else if(Mathf.Distance(gameObject.transform.globalPosition, Core.instance.gameObject.transform.globalPosition) <= furtherDistance)
                inputsList.Add(MOFFGIDEON_INPUT.IN_DASH_FORWARD);

        }
        Debug.Log("Selecting Action");
    }


    #region NEUTRAL

    private void StartNeutral()
    {
        Animator.Play(gameObject, "MG_Run", speedMult);
        UpdateAnimationSpd(speedMult);

        neutralTimer = neutralTime;

        if (randomNum.Next(1, 100) <= probWander)
        {
            wander = true;
            agent.CalculateRandomPath(gameObject.transform.globalPosition, radiusWander);
        }
    }


    private void UpdateNeutral()
    {
        if (agent != null && !wander)
        {
            agent.CalculatePath(gameObject.transform.globalPosition, Core.instance.gameObject.transform.globalPosition);
        }

        //Move character
        LookAt(agent.GetDestination());
        agent.MoveToCalculatedPos(followSpeed * speedMult);

        Debug.Log("Neutral");
    }


    private void EndNeutral()
    {

    }

    #endregion

    #region DASH_FORWARD
    private void StartDashForward()
    {
        targetDash = Core.instance.gameObject.transform.globalPosition;
    }

    private void UpdateDashForward()
    {
        LookAt(targetDash);
        MoveToPosition(targetDash, dashSpeed * speedMult);
        Debug.Log("Dash Forward");
    }

    private void EndDashForward()
    {

    }

    #endregion

    #region MELEE_COMBO
    private void StartMeleeCombo()
    {
        sword.EnableCollider();
    }

    private void UpdateMeleeCombo()
    {
        LookAt(Core.instance.gameObject.transform.globalPosition);
        Debug.Log("Combo");
    }

    private void EndMeleeCombo()
    {
        sword.DisableCollider();
    }

    #endregion
    
    #region DASH_BACKWARD
    private void StartDashBackward()
    {

    }

    private void UpdateDashBackward()
    {
        gameObject.transform.localPosition += dashSpeed * myDeltaTime * speedMult;
        Debug.Log("Dash Backward");
    }

    private void EndDashBackward()
    {

    }

    #endregion

    #region PROJECTILE

    private void StartProjectile()
    {
        GameObject bullet = InternalCalls.CreatePrefab("Library/Prefabs/1606118587.prefab", shootPoint.transform.globalPosition, shootPoint.transform.globalRotation, shootPoint.transform.globalScale);
    }

    private void UpdateProjectile()
    {
        Debug.Log("Projectile");
    }

    private void EndProjectile()
    {
        
    }

    #endregion

    #region SPAWN_ENEMIES

    private void StartSpawnEnemies()
    {
        invencible = true;
        ready2Spawn = false;
        SpawnEnemies();
    }

    private void UpdateSpawnEnemies()
    {
        Debug.Log("Spawning Enemies");

    }

    private void EndSpawnEnemies()
    {
        enemiesTimer = enemiesTime;
        invencible = false;
    }

    #endregion

    #region THROW SABER

    private void StartChargeThrow()
    {

    }


    private void UpdateChargeThrow()
    {
        Debug.Log("Charge Throw");

    }


    private void EndChargeThrow()
    {

    }

    private void StartThrowSaber()
    {

    }


    private void UpdateThrowSaber()
    {
        Debug.Log("Throw Saber");
    }


    private void EndThrowSaber()
    {

    }

    private void StartRetrieveSaber()
    {

    }


    private void UpdateRetrieveSaber()
    {
        Debug.Log("Retrieve Saber");
    }


    private void EndRetrieveSaber()
    {

    }

    #endregion

    #region DIE
    private void StartDie()
    {
        dieTimer = dieTime;

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

    public void Die()
    {
        Debug.Log("MOFF'S DEAD");

        EnemyManager.RemoveEnemy(gameObject);

        Animator.Pause(gameObject);
        Audio.StopAudio(gameObject);
        Input.PlayHaptic(0.3f, 3);
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

        float rotationSpeed = myDeltaTime * slerpSpeed;

        Quaternion desiredRotation = Quaternion.Slerp(gameObject.transform.localRotation, dir, rotationSpeed);

        gameObject.transform.localRotation = desiredRotation;
    }

    public void OnCollisionEnter(GameObject collidedGameObject)
    {
        if (collidedGameObject.CompareTag("Bullet"))
        {
            float damageToBoss = 0f;

            BH_Bullet bulletScript = collidedGameObject.GetComponent<BH_Bullet>();

            if (bulletScript != null)
            {
                damageToBoss += bulletScript.damage;
            }
            else
            {
                Debug.Log("The collider with tag Bullet didn't have a bullet Script!!");
            }

            if (Skill_Tree_Data.IsEnabled((int)Skill_Tree_Data.SkillTreesNames.MANDO, (int)Skill_Tree_Data.MandoSkillNames.AGGRESION_INCREASE_DAMAGE_TO_BOSS))
            {
                damageToBoss *= (1.0f + Skill_Tree_Data.GetMandoSkillTree().A6_increaseDamageToBossAmount);
            }

            TakeDamage(damageToBoss);
            Debug.Log("GIDEON HP: " + healthPoints.ToString());
            damaged = 1.0f;
            //CHANGE FOR APPROPIATE RANCOR HIT
            Audio.PlayAudio(gameObject, "Play_Rancor_Hit");

            if (Core.instance.hud != null)
            {
                HUD hudComponent = Core.instance.hud.GetComponent<HUD>();

                if (hudComponent != null)
                {
                    hudComponent.AddToCombo(25, 0.95f);
                }
            }

            if (currentState != MOFFGIDEON_STATE.DEAD && healthPoints <= 0.0f)
            {
                inputsList.Add(MOFFGIDEON_INPUT.IN_DEAD);
            }
        }
        else if (collidedGameObject.CompareTag("ChargeBullet"))
        {
            float damageToBoss = 0f;

            ChargedBullet bulletScript = collidedGameObject.GetComponent<ChargedBullet>();

            if (bulletScript != null)
            {
                this.AddStatus(STATUS_TYPE.DAMAGE_DOWN, STATUS_APPLY_TYPE.BIGGER_PERCENTAGE, 0.5f, 3.5f);
                damageToBoss += bulletScript.damage;
            }
            else
            {
                Debug.Log("The collider with tag Bullet didn't have a bullet Script!!");
            }

            if (Skill_Tree_Data.IsEnabled((int)Skill_Tree_Data.SkillTreesNames.MANDO, (int)Skill_Tree_Data.MandoSkillNames.AGGRESION_INCREASE_DAMAGE_TO_BOSS))
            {
                damageToBoss *= (1.0f + Skill_Tree_Data.GetMandoSkillTree().A6_increaseDamageToBossAmount);
            }

            TakeDamage(damageToBoss);
            Debug.Log("Rancor HP: " + healthPoints.ToString());
            damaged = 1.0f;
            //CHANGE FOR APPROPIATE RANCOR HIT
            Audio.PlayAudio(gameObject, "Play_Rancor_Hit");

            if (Core.instance.hud != null)
            {
                HUD hudComponent = Core.instance.hud.GetComponent<HUD>();

                if (hudComponent != null)
                {
                    hudComponent.AddToCombo(55, 0.25f);
                }
            }
        }
        else if (collidedGameObject.CompareTag("WorldLimit"))
        {
            if (currentState != MOFFGIDEON_STATE.DEAD)
            {
                inputsList.Add(MOFFGIDEON_INPUT.IN_DEAD);
            }
        }
        else if (collidedGameObject.CompareTag("Player"))
        {
            if (currentState == MOFFGIDEON_STATE.DEAD) return;


            float damageToPlayer = touchDamage;

            PlayerHealth playerHealth = collidedGameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {

            }
                //playerHealth.TakeDamage((int)(damageToPlayer * damageMult));
            

        }
        else if (collidedGameObject.CompareTag("WallSkill"))
        {
            //if (currentState == RANCOR_STATE.RUSH)
            //    inputsList.Add(RANCOR_INPUT.IN_RUSH_STUN);
        }
    }

    public void TakeDamage(float damage)
    {
        if (!DebugOptionsHolder.bossDmg)
        {
            Debug.Log("Moff damage" + damage.ToString());
            if (currentState != MOFFGIDEON_STATE.DEAD)
            {
                healthPoints -= damage * Core.instance.DamageToBosses;

                if (healthPoints <= 0.0f)
                {
                    if(currentPhase==MOFFGIDEON_PHASE.PHASE1)
                    {
                        currentPhase = MOFFGIDEON_PHASE.PHASE2;
                        currentState = MOFFGIDEON_STATE.NEUTRAL;
                    }
                    else
                        inputsList.Add(MOFFGIDEON_INPUT.IN_DEAD);
                }
            }
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

    #region STATUS_SYSTEM

    protected override void OnInitStatus(ref StatusData statusToInit)
    {
        switch (statusToInit.statusType)
        {
            case STATUS_TYPE.SLOWED:
                {
                    this.speedMult -= statusToInit.severity;

                    if (speedMult < 0.1f)
                    {
                        statusToInit.severity = statusToInit.severity - (Math.Abs(this.speedMult) + 0.1f);

                        speedMult = 0.1f;
                    }

                    this.myDeltaTime = Time.deltaTime * speedMult;

                }
                break;
            case STATUS_TYPE.ACCELERATED:
                {
                    this.speedMult += statusToInit.severity;

                    this.myDeltaTime = Time.deltaTime * speedMult;
                }
                break;
            case STATUS_TYPE.DAMAGE_DOWN:
                {
                    this.damageMult -= statusToInit.severity;
                }
                break;
            default:
                break;
        }
    }

    protected override void OnDeleteStatus(StatusData statusToDelete)
    {
        switch (statusToDelete.statusType)
        {
            case STATUS_TYPE.SLOWED:
                {
                    this.speedMult += statusToDelete.severity;

                    this.myDeltaTime = Time.deltaTime * speedMult;
                }
                break;
            case STATUS_TYPE.ACCELERATED:
                {
                    this.speedMult -= statusToDelete.severity;

                    this.myDeltaTime = Time.deltaTime * speedMult;
                }
                break;
            case STATUS_TYPE.DAMAGE_DOWN:
                {
                    this.damageMult += statusToDelete.severity;
                }
                break;
            default:
                break;
        }
    }



    #endregion

    private bool CheckDeathtroopers()
    {
        return (spawner1.GetComponent<Deathtrooper>().GetCurrentSate() == Deathtrooper.STATE.DIE && spawner2.GetComponent<Deathtrooper>().GetCurrentSate() == Deathtrooper.STATE.DIE && spawner3.GetComponent<Deathtrooper>().GetCurrentSate() == Deathtrooper.STATE.DIE && spawner4.GetComponent<Deathtrooper>().GetCurrentSate() == Deathtrooper.STATE.DIE);
    }

    private void SpawnEnemies()
    {
        InternalCalls.CreatePrefab("Library/Prefabs/1439379622.prefab", spawner1.transform.globalPosition, gameObject.transform.localRotation, null);
        InternalCalls.CreatePrefab("Library/Prefabs/1439379622.prefab", spawner2.transform.globalPosition, gameObject.transform.localRotation, null);
        InternalCalls.CreatePrefab("Library/Prefabs/1439379622.prefab", spawner3.transform.globalPosition, gameObject.transform.localRotation, null);
        InternalCalls.CreatePrefab("Library/Prefabs/1439379622.prefab", spawner4.transform.globalPosition, gameObject.transform.localRotation, null);
    }

    public void MoveToPosition(Vector3 positionToReach, float speed)
    {
        Vector3 direction = positionToReach - gameObject.transform.localPosition;

        gameObject.transform.localPosition += direction.normalized * speed * myDeltaTime;
    }

}
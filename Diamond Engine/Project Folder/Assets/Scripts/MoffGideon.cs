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
        THROW_SABER,
        DASH_FORWARD,
        MELEE_COMBO,
        DASH_BACKWARDS,
        PROJECTILE,
        SPAWN_ENEMIES,
        FOLLOW,
        DEAD
    }

    enum MOFFGIDEON_INPUT : int
    {
        NONE = -1,
        IN_THROW_SABER,
        IN_THROW_SABER_END,
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
        IN_FOLLOW,
        IN_FOLLOW_END,
        IN_DEAD
    }

    private NavMeshAgent agent = null;
    public GameObject camera = null;

    //State
    private MOFFGIDEON_STATE currentState = MOFFGIDEON_STATE.FOLLOW;
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
    public float followSpeed = 15f;
    public float touchDamage = 10f;
    public GameObject spawner1 = null;
    public GameObject spawner2 = null;
    public GameObject spawner3 = null;
    public GameObject spawner4 = null;

    //Private Variables
    private float damaged = 0.0f;
    private float currAnimationPlaySpd = 1f;
    private bool invencible = false;
    private List<GameObject> spawners = null;

    //Timers
    private float dieTimer = 0f;
    public float dieTime = 0.1f;
    private float followTimer = 0f;
    public float followTime = 5f;
    private float deadEnemiesTimer = 0f;
    public float deadEnemiesTime = 5f;

    private void Start()
    {
        StartFollow();
        spawners.Add(spawner1);
        spawners.Add(spawner2);
        spawners.Add(spawner3);
        spawners.Add(spawner4);
    }

    public void Awake()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();

        InitEntity(ENTITY_TYPE.MOFF);

        damageMult = 1f;

        damaged = 0f;

        if (EnemyManager.EnemiesLeft() > 0)
            EnemyManager.ClearList();

        EnemyManager.AddEnemy(gameObject);

        if (agent == null)
            Debug.Log("Null agent, add a NavMeshAgent Component");

        Audio.SetState("Game_State", "Rancor_Room");
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
        if (followTimer > 0)
        {
            followTimer -= myDeltaTime;

            if (followTimer <= 0)
                inputsList.Add(MOFFGIDEON_INPUT.IN_SPAWN_ENEMIES);

        }
    }

    private void ProcessExternalInput()
    {
        if (currentState == MOFFGIDEON_STATE.SPAWN_ENEMIES && CheckDeathtroopers())
            inputsList.Add(MOFFGIDEON_INPUT.IN_FOLLOW);

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

                    case MOFFGIDEON_STATE.FOLLOW:
                        switch (input)
                        {
                            case MOFFGIDEON_INPUT.IN_FOLLOW_END:
                                currentState = MOFFGIDEON_STATE.SEARCH_STATE;
                                EndFollow();
                                break;

                            case MOFFGIDEON_INPUT.IN_SPAWN_ENEMIES:
                                currentState = MOFFGIDEON_STATE.SPAWN_ENEMIES;
                                EndFollow();
                                StartSpawnEnemies();
                                break;

                            case MOFFGIDEON_INPUT.IN_PROJECTILE:
                                currentState = MOFFGIDEON_STATE.PROJECTILE;
                                EndFollow();
                                StartProjectile();
                                break;

                            case MOFFGIDEON_INPUT.IN_DASH_FORWARD:
                                currentState = MOFFGIDEON_STATE.DASH_FORWARD;
                                EndFollow();
                                StartDashForward();
                                break;

                            case MOFFGIDEON_INPUT.IN_DEAD:
                                EndFollow();
                                StartDie();
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

                            case MOFFGIDEON_INPUT.IN_PROJECTILE:
                                currentState = MOFFGIDEON_STATE.PROJECTILE;
                                EndSpawnEnemies();
                                StartProjectile();
                                break;

                            case MOFFGIDEON_INPUT.IN_DEAD:
                                EndSpawnEnemies();
                                StartDie();
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

                            case MOFFGIDEON_INPUT.IN_FOLLOW:
                                currentState = MOFFGIDEON_STATE.FOLLOW;
                                EndProjectile();
                                StartFollow();
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

            case MOFFGIDEON_STATE.FOLLOW:
                UpdateFollow();
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
        }
    }

    private void SelectAction()
    {
        
    }


    #region FOLLOW

    private void StartFollow()
    {
        followTimer = followTime;
    }


    private void UpdateFollow()
    {
        if (agent != null)
            agent.CalculatePath(gameObject.transform.globalPosition, Core.instance.gameObject.transform.globalPosition);

        //Move character
        if (agent != null && Mathf.Distance(gameObject.transform.globalPosition, agent.GetDestination()) <= agent.stoppingDistance)
        {
            agent.CalculatePath(gameObject.transform.globalPosition, Core.instance.gameObject.transform.globalPosition);
            LookAt(agent.GetDestination());
            agent.MoveToCalculatedPos(followSpeed * speedMult);
        }

        Debug.Log("Following");
    }


    private void EndFollow()
    {

    }

    #endregion

    #region DASH_FORWARD
    private void StartDashForward()
    {

    }

    private void UpdateDashForward()
    {

    }

    private void EndDashForward()
    {

    }

    #endregion

    #region MELEE_COMBO
    private void StartMeleeCombo()
    {

    }

    private void UpdateMeleeCombo()
    {
        
    }

    private void EndMeleeCombo()
    {
        
    }

    #endregion
    
    #region DASH_BACKWARD
    private void StartDashBackward()
    {

    }

    private void UpdateDashBackward()
    {

    }

    private void EndDashBackward()
    {

    }

    #endregion

    #region PROJECTILE

    private void StartProjectile()
    {

    }

    private void UpdateProjectile()
    {
        
    }

    private void EndProjectile()
    {
        
    }

    #endregion

    #region SPAWN_ENEMIES

    private void StartSpawnEnemies()
    {
        invencible = true;
        deadEnemiesTimer = deadEnemiesTime;
        SpawnEnemies();
    }

    private void UpdateSpawnEnemies()
    {
        if (deadEnemiesTimer > 0)
        {
            deadEnemiesTimer -= myDeltaTime;

            if (deadEnemiesTimer <= 0)
                SpawnEnemies();

        }
    }

    private void EndSpawnEnemies()
    {
        invencible = false;
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
        Debug.Log("RANCOR'S DEAD");

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

            if (currentState != MOFFGIDEON_STATE.DEAD && healthPoints <= 0.0f)
            {
                inputsList.Add(MOFFGIDEON_INPUT.IN_DEAD);
            }
        }
        else if (collidedGameObject.CompareTag("WorldLimit"))
        {
            if (currentState != MOFFGIDEON_STATE.DEAD)
            {
                inputsList.Add(MOFFGIDEON_INPUT.IN_DEAD);
            }
        }
        else if (collidedGameObject.CompareTag("Wall"))
        {
            //if (currentState == MOFFGIDEON_STATE.RUSH)
            //    inputsList.Add(RANCOR_INPUT.IN_RUSH_STUN);
        }
        else if (collidedGameObject.CompareTag("Player"))
        {
            if (currentState == MOFFGIDEON_STATE.DEAD) return;

            float damageToPlayer = touchDamage;

            PlayerHealth playerHealth = collidedGameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
                playerHealth.TakeDamage((int)(damageToPlayer * damageMult));

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
            Debug.Log("Rancor damage" + damage.ToString());
            if (currentState != MOFFGIDEON_STATE.DEAD)
            {
                healthPoints -= damage * Core.instance.DamageToBosses;

                if (healthPoints <= 0.0f)
                {
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

}
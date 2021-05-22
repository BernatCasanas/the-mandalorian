using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Collections.Generic;

using DiamondEngine;
public class Skel : Bosseslv2
{
    enum STATE : int
    {
        NONE = -1,
        SEARCH_STATE,
        FOLLOW,
        WANDER,
        FAST_RUSH,
        SLOW_RUSH,
        RUSH_STUN,
        PROJECTILE,
        JUMP_SLAM,
        BOUNCE_RUSH,
        PRESENTATION,
        DEAD
    }

    enum INPUT : int
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
        IN_JUMPSLAM,
        IN_JUMPSLAM_END,
        IN_BOUNCERUSH,
        IN_BOUNCERUSH_END,
        IN_PRESENTATION,
        IN_PRESENTATION_END,
        IN_DEAD
    }

    private STATE currentState = STATE.PRESENTATION;
    public bool firstSorrowRoar = false;
    private List<INPUT> inputsList = new List<INPUT>();
    private bool firstFrame = true;
    private bool angry = false;
    float roarTime = 0.0f;

    public override void Awake()
    {
        base.Awake();
        Debug.Log("Skel Awake");

        InitEntity(ENTITY_TYPE.SKEL);
        EnemyManager.AddEnemy(gameObject);
        //roarTime = Animator.GetAnimationDuration(gameObject, "RN_Roar") - 0.016f;

        agent = gameObject.GetComponent<NavMeshAgent>();
        if (agent == null)
            Debug.Log("Null agent, add a NavMeshAgent Component");
        else
            Debug.Log("Agent is located");

        //Animator.Play(gameObject, "");
        //Audio.PlayAudio(gameObject, "");
    }

    public void Update()
    {
        if (firstFrame)
        {
            companion = InternalCalls.FindObjectWithName("WampaBoss");
            firstFrame = false;
            StartPresentation();
        }
        myDeltaTime = Time.deltaTime * speedMult;
        UpdateStatuses();

        ProcessInternalInput();
        ProcessExternalInput();
        ProcessState();


        UpdateState();
        //Debug.Log(healthPoints.ToString());

        if (firstSorrowRoar)
        {
            firstSorrowRoar = false;
            Audio.PlayAudio(gameObject, "Play_Skel_When_Wampa_Dies");
        }
    }

    private void ProcessInternalInput()
    {
        if (walkingTimer > 0)
        {
            walkingTimer -= myDeltaTime;

            if (walkingTimer <= 0)
            {
                if (currentState == STATE.FOLLOW)
                    inputsList.Add(INPUT.IN_FOLLOW_END);
                else if (currentState == STATE.WANDER)
                    inputsList.Add(INPUT.IN_WANDER_END);
            }
        }

        if (presentationTimer > 0.0f)
        {
            presentationTimer -= myDeltaTime;
            healthPoints = (1 - (presentationTimer / roarTime)) * maxHealthPoints;

            if (presentationTimer <= 0.0f)
            {
                inputsList.Add(INPUT.IN_PRESENTATION_END);
                healthPoints = maxHealthPoints;
                limboHealth = healthPoints;
            }
        }

        if (restingTimer > 0)
        {
            restingTimer -= myDeltaTime;

            if (restingTimer <= 0)
            {
                resting = false;
            }
        }

        if (totalJumpSlamTimer > 0)
        {
            totalJumpSlamTimer -= myDeltaTime;

            if (totalJumpSlamTimer <= 0)
            {
                inputsList.Add(INPUT.IN_JUMPSLAM_END);
            }
        }
        if (bounceRushTimer > 0)
        {
            bounceRushTimer -= myDeltaTime;

            if (bounceRushTimer <= 0)
            {
                inputsList.Add(INPUT.IN_BOUNCERUSH_END);
            }
        }

    }

    private void ProcessExternalInput()
    {
        if (currentState == STATE.WANDER && Mathf.Distance(gameObject.transform.globalPosition, agent.GetDestination()) <= 1.0f)
        {
            agent.CalculateRandomPath(gameObject.transform.globalPosition, wanderRange);
        }

        if (!companion.IsEnabled() && !angry)
        {
            Debug.Log("skel angry");
            inputsList.Add(INPUT.IN_PRESENTATION);
            presentationTimer = presentationTime;
            SkelAngry();
            angry = true;
        }
    }

    private void ProcessState()
    {
        while (inputsList.Count > 0)
        {
            INPUT input = inputsList[0];
            switch (currentState)
            {
                case STATE.NONE:
                    Debug.Log("SKEL ERROR STATE");
                    break;

                case STATE.SEARCH_STATE:
                    switch (input)
                    {
                        case INPUT.IN_JUMPSLAM:
                            currentState = STATE.JUMP_SLAM;
                            StartJumpSlam();
                            break;

                        case INPUT.IN_BOUNCERUSH:
                            currentState = STATE.BOUNCE_RUSH;
                            StartBounceRush();
                            break;

                        case INPUT.IN_FOLLOW:
                            currentState = STATE.FOLLOW;
                            StartFollowing();
                            break;
                        case INPUT.IN_WANDER:
                            currentState = STATE.WANDER;
                            StartWander();
                            break;
                        case INPUT.IN_PRESENTATION:
                            currentState = STATE.PRESENTATION;
                            StartPresentation();
                            break;
                    }
                    break;

                case STATE.PRESENTATION:
                    switch (input)
                    {
                        case INPUT.IN_PRESENTATION_END:
                            currentState = STATE.SEARCH_STATE;
                            EndPresentation();
                            break;
                    }
                    break;

                case STATE.FOLLOW:
                    switch (input)
                    {
                        case INPUT.IN_FOLLOW_END:
                            currentState = STATE.SEARCH_STATE;
                            EndFollowing();
                            break;
                        case INPUT.IN_DEAD:
                            currentState = STATE.DEAD;
                            StartDie();
                            break;
                        case INPUT.IN_PRESENTATION:
                            currentState = STATE.PRESENTATION;
                            EndFollowing();
                            StartPresentation();
                            break;
                    }
                    break;
                case STATE.BOUNCE_RUSH:
                    switch (input)
                    {
                        case INPUT.IN_BOUNCERUSH_END:
                            currentState = STATE.SEARCH_STATE;
                            EndBounceRush();
                            break;
                        case INPUT.IN_DEAD:
                            currentState = STATE.DEAD;
                            StartDie();
                            break;
                        case INPUT.IN_PRESENTATION:
                            currentState = STATE.PRESENTATION;
                            EndBounceRush();
                            StartPresentation();
                            break;
                    }
                    break;
                case STATE.JUMP_SLAM:
                    switch (input)
                    {
                        case INPUT.IN_JUMPSLAM_END:
                            currentState = STATE.SEARCH_STATE;
                            EndJumpSlam();
                            break;
                        case INPUT.IN_DEAD:
                            currentState = STATE.DEAD;
                            StartDie();
                            break;
                        case INPUT.IN_PRESENTATION:
                            currentState = STATE.PRESENTATION;
                            EndJumpSlam();
                            StartPresentation();
                            break;
                    }
                    break;

                case STATE.WANDER:
                    switch (input)
                    {
                        case INPUT.IN_WANDER_END:
                            currentState = STATE.SEARCH_STATE;
                            EndWander();
                            break;
                        case INPUT.IN_DEAD:
                            currentState = STATE.DEAD;
                            StartDie();
                            break;
                        case INPUT.IN_PRESENTATION:
                            currentState = STATE.PRESENTATION;
                            EndWander();
                            StartPresentation();
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
            case STATE.NONE:
                Debug.Log("Error Wampa State");
                break;
            case STATE.DEAD:
                UpdateDie();
                break;
            case STATE.FOLLOW:
                UpdateFollowing();
                break;
            case STATE.JUMP_SLAM:
                UpdateJumpSlam();
                break;
            case STATE.BOUNCE_RUSH:
                UpdateBounceRush();
                break;
            case STATE.WANDER:
                UpdateWander();
                break;
            case STATE.PRESENTATION:
                UpdatePresentation();
                break;
            case STATE.SEARCH_STATE:
                SelectAction();
                break;
        }

        limboHealth = Mathf.Lerp(limboHealth, healthPoints, 0.01f);
        if (bossHealth != null)
        {
            Material bossBarMat = bossHealth.GetComponent<Material>();

            if (bossBarMat != null)
            {
                bossBarMat.SetFloatUniform("length_used", healthPoints / maxHealthPoints);
                bossBarMat.SetFloatUniform("limbo", limboHealth / maxHealthPoints);
            }
            else
                Debug.Log("Boss Bar component was null!!");

        }
    }

    private void SelectAction()
    {
        if (resting)
        {
            int decision = randomNum.Next(1, 100);
            if (decision <= 60)
                inputsList.Add(INPUT.IN_FOLLOW);
            else
                inputsList.Add(INPUT.IN_WANDER);
        }
        //else
        //{
        //    if (Mathf.Distance(gameObject.transform.globalPosition, Core.instance.gameObject.transform.globalPosition) >= distanceProjectile)
        //        inputsList.Add(INPUT.IN_PROJECTILE);
        //    else
        //        inputsList.Add(INPUT.IN_FAST_RUSH);
        //    Debug.Log("Selecting Action");
        //}
        else
        {
            int decision = randomNum.Next(1, 100);
            if (decision <= 75)
                inputsList.Add(INPUT.IN_JUMPSLAM);
            else
                inputsList.Add(INPUT.IN_BOUNCERUSH);
        }
    }


    public void OnCollisionEnter(GameObject collidedGameObject)
    {
        if (collidedGameObject.CompareTag("Bullet"))
        {
            if (Core.instance != null)
            {
                if (Core.instance.HasStatus(STATUS_TYPE.PRIM_MOV_SPEED))
                    Core.instance.AddStatus(STATUS_TYPE.ACCELERATED, STATUS_APPLY_TYPE.BIGGER_PERCENTAGE, Core.instance.GetStatusData(STATUS_TYPE.PRIM_MOV_SPEED).severity / 100, 5, false);
                if (Core.instance.HasStatus(STATUS_TYPE.MANDO_QUICK_DRAW))
                    AddStatus(STATUS_TYPE.BLASTER_VULN, STATUS_APPLY_TYPE.ADDITIVE, Core.instance.GetStatusData(STATUS_TYPE.MANDO_QUICK_DRAW).severity / 100, 5);

            }
            BH_Bullet bulletComp = collidedGameObject.GetComponent<BH_Bullet>();


            if (bulletComp != null)
            {
                float damageToBoss = bulletComp.GetDamage();
                if (Core.instance != null)
                    damageToBoss *= Core.instance.DamageToBosses * BlasterVulnerability;

                //if (Skill_Tree_Data.IsEnabled((int)Skill_Tree_Data.SkillTreesNames.MANDO, (int)Skill_Tree_Data.MandoSkillNames.AGGRESION_INCREASE_DAMAGE_TO_BOSS))
                //{
                //    damageToBoss *= (1.0f + Skill_Tree_Data.GetMandoSkillTree().A6_increaseDamageToBossAmount);
                //}

                TakeDamage(damageToBoss * damageRecieveMult);
            }

            //damaged = 1.0f; this is HUD things

            if (Core.instance.hud != null)
            {
                Core.instance.hud.GetComponent<HUD>().AddToCombo(25, 0.95f);
            }

        }
        else if (collidedGameObject.CompareTag("ChargeBullet"))
        {

            ChargedBullet bulletComp = collidedGameObject.GetComponent<ChargedBullet>();

            if (bulletComp != null)
            {
                float vulerableSev = 0.2f;
                float vulerableTime = 4.5f;
                STATUS_APPLY_TYPE applyType = STATUS_APPLY_TYPE.BIGGER_PERCENTAGE;
                float damageMult = 1f;

                if (Core.instance != null)
                {
                    if (Core.instance.HasStatus(STATUS_TYPE.SNIPER_STACK_DMG_UP))
                    {
                        vulerableSev += Core.instance.GetStatusData(STATUS_TYPE.SNIPER_STACK_DMG_UP).severity;
                    }
                    if (Core.instance.HasStatus(STATUS_TYPE.SNIPER_STACK_ENABLE))
                    {
                        vulerableTime += Core.instance.GetStatusData(STATUS_TYPE.SNIPER_STACK_ENABLE).severity;
                        applyType = STATUS_APPLY_TYPE.ADD_SEV;
                    }
                    if (Core.instance.HasStatus(STATUS_TYPE.SNIPER_STACK_WORK_SNIPER))
                    {
                        vulerableSev += Core.instance.GetStatusData(STATUS_TYPE.SNIPER_STACK_WORK_SNIPER).severity;
                        damageMult = damageRecieveMult;
                    }
                    if (Core.instance.HasStatus(STATUS_TYPE.SNIPER_STACK_BLEED))
                    {
                        StatusData bleedData = Core.instance.GetStatusData(STATUS_TYPE.SNIPER_STACK_BLEED);
                        float chargedBulletMaxDamage = Core.instance.GetSniperMaxDamage();

                        damageMult *= bleedData.remainingTime;
                        this.AddStatus(STATUS_TYPE.ENEMY_BLEED, STATUS_APPLY_TYPE.ADD_SEV, (chargedBulletMaxDamage * bleedData.severity) / vulerableTime, vulerableTime);
                    }
                }
                this.AddStatus(STATUS_TYPE.ENEMY_VULNERABLE, applyType, vulerableSev, vulerableTime);

                float damageToBoss = bulletComp.GetDamage() * damageMult;
                if (Core.instance != null)
                    damageToBoss *= Core.instance.DamageToBosses;
                if (Core.instance.HasStatus(STATUS_TYPE.CROSS_HAIR_LUCKY_SHOT))
                {
                    float mod = Core.instance.GetStatusData(STATUS_TYPE.CROSS_HAIR_LUCKY_SHOT).severity;
                    Random rand = new Random();
                    float result = rand.Next(1, 101);
                    if (result <= mod)
                        Core.instance.RefillSniper();

                    Core.instance.luckyMod = 1 + mod / 100;
                }
                TakeDamage(damageToBoss);
            }

            //damaged = 1.0f; this is HUD things

            if (Core.instance.hud != null)
            {
                Core.instance.hud.GetComponent<HUD>().AddToCombo(55, 0.25f);
            }
        }
        else if (collidedGameObject.CompareTag("WorldLimit"))
        {
            if (currentState != STATE.DEAD)
            {
                inputsList.Add(INPUT.IN_DEAD);
            }
        }
        else if (collidedGameObject.CompareTag("Wall"))
        {
            if (currentState == STATE.FAST_RUSH || currentState == STATE.SLOW_RUSH)
                inputsList.Add(INPUT.IN_SLOW_RUSH_END);
        }
        else if (collidedGameObject.CompareTag("Player"))
        {
            if (currentState == STATE.FAST_RUSH || currentState == STATE.SLOW_RUSH)
            {
                inputsList.Add(INPUT.IN_SLOW_RUSH_END);
            }

            else if (currentState == STATE.BOUNCE_RUSH)
            {
                inputsList.Add(INPUT.IN_BOUNCERUSH_END);
            }
        }
        else if (collidedGameObject.CompareTag("PushSkill"))
        {
            if (currentState == STATE.FAST_RUSH || currentState == STATE.SLOW_RUSH)
            {
                inputsList.Add(INPUT.IN_SLOW_RUSH_END);
            }
            else if (currentState == STATE.BOUNCE_RUSH)
            {
                inputsList.Add(INPUT.IN_BOUNCERUSH_END);
            }
        }
    }

    public override void TakeDamage(float damage)
    {
        if (!DebugOptionsHolder.bossDmg)
        {
            

            if (currentState != STATE.DEAD)
            {
                Audio.PlayAudio(gameObject, "Play_Skel_Hit");
                float mod = 1;
                if (Core.instance != null && Core.instance.HasStatus(STATUS_TYPE.GEOTERMAL_MARKER))
                {
                    if (HasNegativeStatus())
                    {
                        mod = 1 + GetStatusData(STATUS_TYPE.GEOTERMAL_MARKER).severity / 100;
                    }
                }
                healthPoints -= damage * mod; 
                if (Core.instance != null)
                {
                    if (Core.instance.HasStatus(STATUS_TYPE.WRECK_HEAVY_SHOT) && HasStatus(STATUS_TYPE.SLOWED))
                        AddStatus(STATUS_TYPE.SLOWED, STATUS_APPLY_TYPE.ADDITIVE, Core.instance.GetStatusData(STATUS_TYPE.WRECK_HEAVY_SHOT).severity / 100, 5);

                    if (Core.instance.HasStatus(STATUS_TYPE.LIFESTEAL))
                    {
                        Random rand = new Random();
                        float result = rand.Next(1, 101);
                        if (result <= 10)
                            if (Core.instance.gameObject != null && Core.instance.gameObject.GetComponent<PlayerHealth>() != null)
                            {
                                float healing = Core.instance.GetStatusData(STATUS_TYPE.LIFESTEAL).severity * damage / 100;
                                if (healing < 1) healing = 1;
                                Core.instance.gameObject.GetComponent<PlayerHealth>().SetCurrentHP(PlayerHealth.currHealth + (int)(healing));
                            }
                    }
                    if (Core.instance.HasStatus(STATUS_TYPE.SOLO_HEAL))
                    {
                        Core.instance.gameObject.GetComponent<PlayerHealth>().SetCurrentHP(PlayerHealth.currHealth + (int)Core.instance.skill_SoloHeal);
                        Core.instance.skill_SoloHeal = 0;
                    }
                }
                Debug.Log("Skel HP: " + healthPoints.ToString());

                if (healthPoints <= 0.0f)
                {
                    inputsList.Add(INPUT.IN_DEAD);
                }
            }
        }
    }

    private void SkelAngry()
    {
        
    }

}
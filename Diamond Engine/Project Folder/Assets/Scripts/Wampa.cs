using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Collections.Generic;

using DiamondEngine;


public class Wampa : Bosseslv2
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
        IN_DEAD
    }

    private STATE currentState = STATE.SEARCH_STATE;
    private List<INPUT> inputsList = new List<INPUT>();

    public void Awake()
    {
        Debug.Log("Wampa Awake");

        agent = gameObject.GetComponent<NavMeshAgent>();
        if (agent == null)
            Debug.Log("Null agent, add a NavMeshAgent Component");
        else
            Debug.Log("Agent is located");

        //Animator.Play(gameObject, "");
        //Audio.PlayAudio(gameObject, "");
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
                if (currentState == STATE.FOLLOW)
                    inputsList.Add(INPUT.IN_FOLLOW_END);
                else if (currentState == STATE.WANDER)
                    inputsList.Add(INPUT.IN_WANDER_END);
            }
        }

        if (fastChasingTimer > 0)
        {
            fastChasingTimer -= Time.deltaTime;

            if (fastChasingTimer <= 0)
            {
                inputsList.Add(INPUT.IN_FAST_RUSH_END);
            }
        }

        if (slowChasingTimer > 0)
        {
            slowChasingTimer -= Time.deltaTime;

            if (slowChasingTimer <= 0)
            {
                inputsList.Add(INPUT.IN_SLOW_RUSH_END);
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
        if (currentState == STATE.WANDER && Mathf.Distance(gameObject.transform.globalPosition, agent.GetDestination()) <= 1.0f)
        {
            agent.CalculateRandomPath(gameObject.transform.globalPosition, wanderRange);
        }
        if (currentState == STATE.PROJECTILE && secondShot)
        {
            inputsList.Add(INPUT.IN_PROJECTILE_END);
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
                    Debug.Log("WAMPA ERROR STATE");
                    break;

                case STATE.SEARCH_STATE:
                    switch (input)
                    {
                        case INPUT.IN_PROJECTILE:
                            currentState = STATE.PROJECTILE;
                            StartProjectile();
                            break;

                        case INPUT.IN_FAST_RUSH:
                            currentState = STATE.FAST_RUSH;
                            StartFastRush();
                            break;

                        case INPUT.IN_FOLLOW:
                            currentState = STATE.FOLLOW;
                            StartFollowing();
                            break;
                        case INPUT.IN_WANDER:
                            currentState = STATE.WANDER;
                            StartWander();
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
                    }
                    break;

                case STATE.PROJECTILE:
                    switch (input)
                    {
                        case INPUT.IN_PROJECTILE_END:
                            currentState = STATE.SEARCH_STATE;
                            EndProjectile();
                            break;
                        case INPUT.IN_DEAD:
                            currentState = STATE.DEAD;
                            StartDie();
                            break;
                    }
                    break;
                case STATE.FAST_RUSH:
                    switch (input)
                    {
                        case INPUT.IN_FAST_RUSH_END:
                            currentState = STATE.SLOW_RUSH;
                            StartSlowRush();
                            break;
                        case INPUT.IN_DEAD:
                            currentState = STATE.DEAD;
                            StartDie();
                            break;
                    }
                    break;

                case STATE.SLOW_RUSH:
                    switch (input)
                    {
                        case INPUT.IN_SLOW_RUSH_END:
                            currentState = STATE.SEARCH_STATE;
                            EndSlowRush();
                            break;
                        case INPUT.IN_DEAD:
                            currentState = STATE.DEAD;
                            StartDie();
                            break;
                    }
                    break;

                case STATE.RUSH_STUN:
                    switch (input)
                    {
                        case INPUT.IN_RUSH_STUN_END:
                            currentState = STATE.SEARCH_STATE;
                            EndRushStun();
                            break;
                        case INPUT.IN_DEAD:
                            currentState = STATE.DEAD;
                            StartDie();
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
            case STATE.PROJECTILE:
                UpdateProjectile();
                break;
            case STATE.FAST_RUSH:
                UpdateFastRush();
                break;
            case STATE.SLOW_RUSH:
                UpdateSlowRush();
                break;
            case STATE.RUSH_STUN:
                UpdateRushStun();
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
            case STATE.SEARCH_STATE:
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
                inputsList.Add(INPUT.IN_FOLLOW);
            else
                inputsList.Add(INPUT.IN_WANDER);
        }
        else
        {
            if (Mathf.Distance(gameObject.transform.globalPosition, Core.instance.gameObject.transform.globalPosition) >= distanceProjectile)
                inputsList.Add(INPUT.IN_PROJECTILE);
            else
                inputsList.Add(INPUT.IN_FAST_RUSH);
        }
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
                Core.instance.hud.GetComponent<HUD>().AddToCombo(25, 0.95f);
            }

            if (currentState != STATE.DEAD && healthPoints <= 0.0f)
            {
                inputsList.Add(INPUT.IN_DEAD);
            }
        }
        else if (collidedGameObject.CompareTag("ChargeBullet"))
        {
            healthPoints -= collidedGameObject.GetComponent<ChargedBullet>().damage;
            Debug.Log("Wampa HP: " + healthPoints.ToString());
            //damaged = 1.0f; this is HUD things

            if (Core.instance.hud != null)
            {
                Core.instance.hud.GetComponent<HUD>().AddToCombo(55, 0.25f);
            }

            if (currentState != STATE.DEAD && healthPoints <= 0.0f)
            {
                inputsList.Add(INPUT.IN_DEAD);
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
                Core.instance.hud.GetComponent<HUD>().AddToCombo(8, 1.5f);
            }

            if (currentState != STATE.DEAD && healthPoints <= 0.0f)
            {
                inputsList.Add(INPUT.IN_DEAD);
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
                inputsList.Add(INPUT.IN_RUSH_STUN);
        }
        else if (collidedGameObject.CompareTag("Player"))
        {
            if (currentState == STATE.FAST_RUSH || currentState == STATE.SLOW_RUSH)
            {
                inputsList.Add(INPUT.IN_SLOW_RUSH_END);
                PlayerHealth playerHealth = collidedGameObject.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage((int)rushDamage);
                }
            }
        }
    }
}
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Collections.Generic;

using DiamondEngine;


public class Wampa : Bosseslv2
{
    private bool start = false;
    private void Start()
    {
        agent = gameObject.GetComponent<NavMeshAgent>();
        if (agent == null)
            Debug.Log("Null agent, add a NavMeshAgent Component");

        Animator.Play(gameObject, "");
        Audio.PlayAudio(gameObject, "");
        Counter.roomEnemies++;  // Just in case
        EnemyManager.AddEnemy(gameObject);
        Debug.Log("Null agent, add a NavMeshAgent Component");
        Debug.Log("Null agent, add a NavMeshAgent Component");
        Debug.Log("Null agent, add a NavMeshAgent Component");
        Debug.Log("Null agent, add a NavMeshAgent Component");
        Debug.Log("Null agent, add a NavMeshAgent Component");
        Debug.Log("Null agent, add a NavMeshAgent Component");
    
    }

    public void Update()
    {
        if (!start) Start();

        ProcessInternalInput();
        ProcessExternalInput();
        ProcessState();

        UpdateState();
        Debug.Log("is working");
    }

    private void ProcessInternalInput()
    {
        if (walkingTimer > 0)
        {
            walkingTimer -= Time.deltaTime;

            if (walkingTimer <= 0)
            {
                if (currentState == BOSS_STATE.FOLLOW)
                    inputsList.Add(BOSS_INPUT.IN_FOLLOW_END);
                else if (currentState == BOSS_STATE.WANDER)
                    inputsList.Add(BOSS_INPUT.IN_WANDER_END);
            }
        }

        if (fastChasingTimer > 0)
        {
            fastChasingTimer -= Time.deltaTime;

            if (fastChasingTimer <= 0)
            {
                inputsList.Add(BOSS_INPUT.IN_FAST_RUSH_END);
            }
        }

        if (slowChasingTimer > 0)
        {
            slowChasingTimer -= Time.deltaTime;

            if (slowChasingTimer <= 0)
            {
                inputsList.Add(BOSS_INPUT.IN_SLOW_RUSH_END);
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
        if (currentState == BOSS_STATE.WANDER && Mathf.Distance(gameObject.transform.globalPosition, agent.GetDestination()) <= agent.stoppingDistance)
        {
            agent.CalculateRandomPath(gameObject.transform.globalPosition, wanderRange);
        }
    }

    private void ProcessState()
    {
        while (inputsList.Count > 0)
        {
            BOSS_INPUT input = inputsList[0];

            switch (currentState)
            {
                case BOSS_STATE.NONE:
                    Debug.Log("WAMPA ERROR STATE");
                    break;

                case BOSS_STATE.SEARCH_STATE:
                    switch (input)
                    {
                        case BOSS_INPUT.IN_PROJECTILE:
                            currentState = BOSS_STATE.PROJECTILE;
                            StartProjectile();
                            break;

                        case BOSS_INPUT.IN_FOLLOW:
                            currentState = BOSS_STATE.FOLLOW;
                            StartFollowing();
                            break;
                        case BOSS_INPUT.IN_WANDER:
                            currentState = BOSS_STATE.WANDER;
                            StartWander();
                            break;
                    }
                    break;

                case BOSS_STATE.FOLLOW:
                    switch (input)
                    {
                        case BOSS_INPUT.IN_FOLLOW_END:
                            currentState = BOSS_STATE.SEARCH_STATE;
                            EndFollowing();
                            break;
                        case BOSS_INPUT.IN_DEAD:
                            currentState = BOSS_STATE.SEARCH_STATE;
                            StartDie();
                            break;
                    }
                    break;
                case BOSS_STATE.PROJECTILE:
                    switch (input)
                    {
                        case BOSS_INPUT.IN_PROJECTILE_END:
                            currentState = BOSS_STATE.SEARCH_STATE;
                            EndProjectile();
                            break;
                        case BOSS_INPUT.IN_DEAD:
                            currentState = BOSS_STATE.SEARCH_STATE;
                            StartDie();
                            break;
                    }
                    break;
                case BOSS_STATE.FAST_RUSH:
                    switch (input)
                    {
                        case BOSS_INPUT.IN_FAST_RUSH_END:
                            currentState = BOSS_STATE.SLOW_RUSH;
                            StartSlowRush();
                            break;
                        case BOSS_INPUT.IN_DEAD:
                            currentState = BOSS_STATE.SEARCH_STATE;
                            StartDie();
                            break;
                    }
                    break;
                case BOSS_STATE.RUSH_STUN:
                    switch (input)
                    {
                        case BOSS_INPUT.IN_RUSH_STUN_END:
                            currentState = BOSS_STATE.SEARCH_STATE;
                            EndRushStun();
                            break;
                        case BOSS_INPUT.IN_DEAD:
                            currentState = BOSS_STATE.SEARCH_STATE;
                            StartDie();
                            break;
                    }
                    break;
                case BOSS_STATE.WANDER:
                    switch (input)
                    {
                        case BOSS_INPUT.IN_WANDER_END:
                            currentState = BOSS_STATE.SEARCH_STATE;
                            EndWander();
                            break;
                        case BOSS_INPUT.IN_DEAD:
                            currentState = BOSS_STATE.SEARCH_STATE;
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
            case BOSS_STATE.NONE:
                Debug.Log("Error Wampa State");
                break;
            case BOSS_STATE.DEAD:
                UpdateDie();
                break;
            case BOSS_STATE.FOLLOW:
                UpdateFollowing();
                break;
            case BOSS_STATE.PROJECTILE:
                UpdateProjectile();
                break;
            case BOSS_STATE.FAST_RUSH:
                UpdateFastRush();
                break;
            case BOSS_STATE.RUSH_STUN:
                UpdateRushStun();
                break;
            case BOSS_STATE.WANDER:
                UpdateWander();
                break;
            case BOSS_STATE.SEARCH_STATE:
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
                inputsList.Add(BOSS_INPUT.IN_FOLLOW);
            else
                inputsList.Add(BOSS_INPUT.IN_WANDER);
        }
        else
        {
            if (Mathf.Distance(gameObject.transform.globalPosition, Core.instance.gameObject.transform.globalPosition) >= distanceProjectile)
                inputsList.Add(BOSS_INPUT.IN_PROJECTILE);
            else
                inputsList.Add(BOSS_INPUT.IN_FAST_RUSH);
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

            if (currentState != BOSS_STATE.DEAD && healthPoints <= 0.0f)
            {
                inputsList.Add(BOSS_INPUT.IN_DEAD);
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

            if (currentState != BOSS_STATE.DEAD && healthPoints <= 0.0f)
            {
                inputsList.Add(BOSS_INPUT.IN_DEAD);
            }
        }
        else if (collidedGameObject.CompareTag("WorldLimit"))
        {
            if (currentState != BOSS_STATE.DEAD)
            {
                inputsList.Add(BOSS_INPUT.IN_DEAD);
            }
        }
        else if (collidedGameObject.CompareTag("Wall"))
        {
            if (currentState == BOSS_STATE.FAST_RUSH || currentState == BOSS_STATE.SLOW_RUSH)
                inputsList.Add(BOSS_INPUT.IN_RUSH_STUN);
        }
        else if (collidedGameObject.CompareTag("Player"))
        {
            if (currentState == BOSS_STATE.FAST_RUSH || currentState == BOSS_STATE.SLOW_RUSH)
            {
                inputsList.Add(BOSS_INPUT.IN_SLOW_RUSH_END);
                PlayerHealth playerHealth = collidedGameObject.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage((int)rushDamage);
                }
            }
        }
    }
}
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Collections.Generic;

using DiamondEngine;

public class StormTrooper2 : Enemy
{
    //public static Enemy instance;

    enum STATE : int
    {
        NONE = -1,
        IDLE,
        RUN,
        WANDER,
        //PUSHED,
        SHOOT,
        HIT,
        DIE
    }

    enum INPUT : int
    {
        IN_IDLE,
        IN_RUN,
        IN_RUN_END,
        IN_WANDER,
        IN_PUSHED,
        IN_SHOOT,
        IN_SHOOT_END,
        IN_SEQUENCE_END,
        IN_HIT,
        IN_DIE
    }

    //VARIABLES DEL STORMTROOPER ORIGINAL
    public GameObject hitParticles = null;

    private int shotSequences = 0;
    public int maxShots = 2;
    public int maxSequences = 2;
    //private float timeBetweenSequences = 0.75f;
    private float shotTimer = 0.0f;

    private float pushSkillTimer = 0.15f;
    //private float pushSkillSpeed = 0.2f;
    public float stormTrooperDamage = 5.0f;


    //VARIABLES DE CORE PASTED
    //public GameObject shootPoint = null;
    public GameObject hud = null;

    private bool started = false;

    //State
    private STATE currentState = STATE.NONE;   //NEVER SET THIS VARIABLE DIRECTLLY, ALLWAYS USE INPUTS
                                                //Setting states directlly will break the behaviour  -Jose
    private List<INPUT> inputsList = new List<INPUT>();

    //private bool rightTriggerPressed = false;


    private void Start()
    {
        //currentState = STATES.IDLE; //esto se tiene que quitar, no se deberia iniciar el state
        targetPosition = CalculateNewPosition(wanderRange);
        shotTimes = 0;
        stormTrooperDamage = 1.0f;
    }

    public void Update()
    {
        // Placeholder for Start() function
        if (started == false)
        {
            Start();
            started = true;
        }

        if (player == null)
        {
            Debug.Log("Null player");
            player = Core.instance.gameObject;
        }

        timePassed += Time.deltaTime;

        #region UPDATE STUFF

        //if (Input.GetKey(DEKeyCode.C) == KeyState.KEY_DOWN)
        //{
        //    Audio.SetState("Game_State", "Run");
        //    if (MusicSourceLocate.instance != null)
        //    {
        //        Audio.SetSwitch(MusicSourceLocate.instance.gameObject, "Player_Action", "Combat");
        //        Audio.SetSwitch(MusicSourceLocate.instance.gameObject, "Player_Health", "Healthy");
        //    }
        //}

        //UpdateControllerInputs();

        #endregion

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
        //AQUI DEBERIAN ESTAR TODOS LOS TIMERS

        if (shotTimer > 0.0f)
        {
            shotTimer -= Time.deltaTime;

            if (shotTimes < maxShots)
            {
                inputsList.Add(INPUT.IN_SHOOT);
            }
        }


        if (shotSequences == maxSequences && !turretMode)
        {
            inputsList.Add(INPUT.IN_SHOOT);
        }

        if (shotTimes == maxShots)
        {
            inputsList.Add(INPUT.IN_SHOOT_END);
        }

        if (timePassed > idleTime && !turretMode)
        {
            inputsList.Add(INPUT.IN_WANDER);
        }


        if (timePassed >= pushSkillTimer)
        {
            inputsList.Add(INPUT.IN_IDLE);
        }

        if (currentState != STATE.DIE && healthPoints <= 0.0f)
        {
            inputsList.Add(INPUT.IN_DIE);
        }

    }


    //Controler inputs go here
    private void ProcessExternalInput()
    {
        //AQUI DEBERIAN ESTAR LAS COSAS QUE HACEN CAMBIAR DE ESTADO (RANGOS DE ACCION)

        if (InRange(player.transform.globalPosition, range))
        {
            inputsList.Add(INPUT.IN_SHOOT);
        }

        if (InRange(player.transform.globalPosition, wanderRange))
        {
            inputsList.Add(INPUT.IN_WANDER);
        }

        if (Mathf.Distance(gameObject.transform.localPosition, targetPosition) < stoppingDistance)
        {
            inputsList.Add(INPUT.IN_IDLE);
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
                        case INPUT.IN_RUN:
                            currentState = STATE.RUN;
                            StartRun();
                            break;

                        case INPUT.IN_WANDER:
                            currentState = STATE.WANDER;
                            StartWander();
                            break;

                        case INPUT.IN_SHOOT:
                            currentState = STATE.SHOOT;
                            StartShoot();
                            break;

                        case INPUT.IN_HIT:
                            currentState = STATE.HIT;
                            //StartHit();
                            break;

                        case INPUT.IN_DIE:
                            currentState = STATE.DIE;
                            StartDie();
                            break;
                    }
                    break;


                case STATE.RUN:
                    switch (input)
                    {
                        case INPUT.IN_IDLE:
                            currentState = STATE.IDLE;
                            StartIdle();
                            break;

                        case INPUT.IN_WANDER:
                            currentState = STATE.WANDER;
                            StartWander();
                            break;

                        case INPUT.IN_SHOOT:
                            currentState = STATE.SHOOT;
                            StartShoot();
                            break;

                        case INPUT.IN_DIE:
                            currentState = STATE.DIE;
                            StartDie();
                            break;
                    }
                    break;


                case STATE.WANDER:
                    switch (input)
                    {
                        case INPUT.IN_SHOOT:
                            currentState = STATE.IDLE;
                            StartShoot();
                            break;

                        case INPUT.IN_DIE:
                            currentState = STATE.DIE;
                            StartDie();
                            break;
                    }
                    break;


                case STATE.SHOOT:
                    switch (input)
                    {
                        case INPUT.IN_WANDER:
                            currentState = STATE.WANDER;
                            StartWander();
                            break;

                        case INPUT.IN_RUN:
                            currentState = STATE.IDLE;
                            StartRun();
                            break;

                        case INPUT.IN_SHOOT:
                            currentState = STATE.SHOOT;
                            StartShoot();
                            break;

                        case INPUT.IN_DIE:
                            currentState = STATE.DIE;
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
            case STATE.RUN:
                UpdateRun();
                break;
            case STATE.WANDER:
                UpdateWander();
                break;
            case STATE.SHOOT:
                //UpdateShoot();
                break;
            case STATE.DIE:
                break;
            default:
                Debug.Log("NEED TO ADD STATE TO CORE");
                break;
        }
    }


    #region SHOOT
    private void StartShoot()
    {
        //SFX LIKE AIMING OR SIMILAR + STOP RUNNING
    }

    private void UpdateShoot()
    {
        //HERE SHOULD BE THE BEHAVIOUR WHILE SHOOTING

        //2 SECONDS STOPPED IN IDLE ANIMATION

        //SHOT 2 BULLETS

        //2 SECONDS STOPPED IN IDLE ANIMATION

        //SHOT 2 BULLETS

        //1 SECOND STOPPED IN IDLE ANIMATION

        //AFTER THAT SHOTTIMES VARIABLE WOULD BE 2 AND START RUNNING
    }
    #endregion

    #region RUN
    private void StartRun()
    {
        LookAt(targetPosition);
        //MoveToPosition(targetPosition, runningSpeed);

        //ANIMATIONS OR FX AT RUN START
    }
    private void UpdateRun()
    {
        LookAt(targetPosition);
        MoveToPosition(targetPosition, runningSpeed);

        //ANIMATIONS OR FX WHILE RUNNNG
    }
    #endregion

    #region WANDER
    private void StartWander()
    {
        targetPosition = CalculateNewPosition(wanderRange);
        LookAt(targetPosition);
        //MoveToPosition(targetPosition, wanderSpeed);

        //ANIMATIONS OR FX AT WANDER START
    }
    private void UpdateWander()
    {
        LookAt(targetPosition);
        MoveToPosition(targetPosition, wanderSpeed);

        //ANIMATIONS OR FX WHILE WANDERING
    }
    #endregion

    #region DIE
    private void StartDie()
    {
        timePassed = 0.0f;
        Animator.Play(gameObject, "ST_Die", 1.0f);
        //Play Sound("Die")
        Audio.PlayAudio(gameObject, "Play_Stormtrooper_Death");
        Audio.PlayAudio(gameObject, "Play_Mando_Voice");

        if (hitParticles != null)
            hitParticles.GetComponent<ParticleSystem>().Play();

        RemoveFromSpawner();

        if (Core.instance.hud != null)
        {
            Core.instance.hud.GetComponent<HUD>().AddToCombo(20, 1.0f);
        }

    }
    private void UpdateDie()
    {
        timePassed += Time.deltaTime;

        if (timePassed > 1.2f)
        {
            Counter.SumToCounterType(Counter.CounterTypes.ENEMY_STORMTROOP);
            Counter.roomEnemies--;
            Debug.Log("Enemies: " + Counter.roomEnemies.ToString());
            if (Counter.roomEnemies <= 0)
            {
                Core.instance.gameObject.GetComponent<BoonSpawn>().SpawnBoons();
            }
            player.GetComponent<PlayerHealth>().TakeDamage(-PlayerHealth.healWhenKillingAnEnemy);
            InternalCalls.Destroy(gameObject);
        }
    }
    #endregion

    #region DASH
    //THIS WILL BE HERE CAUSE BANTHA WILL USE IT, I DONT WANT TO ERASE IT YET
    private void StartDash()
    {
        /* Audio.StopAudio(gameObject);
         Audio.PlayAudio(gameObject, "Play_Dash");
         Animator.Play(gameObject, "Dash");

         dashTimer = dashDuration;
         dashStartYPos = gameObject.transform.localPosition.y;*/
    }

    private void UpdateDash()
    {
        // StopPlayer();
        // gameObject.AddForce(gameObject.transform.GetForward().normalized * dashforce);
    }

    private void EndDash()
    {
        //StopPlayer();
        // gameObject.transform.localPosition.y = dashStartYPos;
    }

    #endregion


    private void StartIdle()
    {
        Animator.Play(gameObject, "Idle");
    }

    public void OnCollisionEnter(GameObject collidedGameObject)
    {
        //Debug.Log("CS: Collided object: " + gameObject.tag + ", Collider: " + collidedGameObject.tag);
        //Debug.Log("Collided by tag: " + collidedGameObject.tag);

        /* if (collidedGameObject.CompareTag("Bullet"))
         {
             Debug.Log("Collision bullet");
             healthPoints -= collidedGameObject.GetComponent<BH_Bullet>().damage;
             if (currentState != STATE.DIE && healthPoints <= 0.0f)  //quitar STATE
             {
                 currentState = STATE.DIE;   //quitar STATE
                 timePassed = 0.0f;
                 Animator.Play(gameObject, "ST_Die", 1.0f);
                 //Play Sound("Die")
                 Audio.PlayAudio(gameObject, "Play_Stormtrooper_Death");
                 Audio.PlayAudio(gameObject, "Play_Mando_Voice");

                 if (hitParticles != null)
                     hitParticles.GetComponent<ParticleSystem>().Play();

                 RemoveFromSpawner();

                 if (Core.instance.hud != null)
                 {
                     Core.instance.hud.GetComponent<HUD>().AddToCombo(20, 1.0f);
                 }
             }
         }
         else if (collidedGameObject.CompareTag("Grenade"))
         {
             Debug.Log("Collision Grenade");

             if (currentState != STATE.DIE)  //quitar STATE
             {
                 currentState = STATE.DIE;   //quitar STATE
                 timePassed = 0.0f;
                 Animator.Play(gameObject, "ST_Die", 1.0f);
                 //Play Sound("Die")
                 Audio.PlayAudio(gameObject, "Play_Stormtrooper_Death");
                 Audio.PlayAudio(gameObject, "Play_Mando_Voice");

                 RemoveFromSpawner();


                 if (Core.instance.hud != null)
                 {
                     Core.instance.hud.GetComponent<HUD>().AddToCombo(20, 0.5f);
                 }
             }
         }
         else if (collidedGameObject.CompareTag("WorldLimit"))
         {
             Debug.Log("Collision w/ The End");

             if (currentState != STATE.DIE)  //quitar STATE
             {
                 currentState = STATE.DIE;   //quitar STATE
                 timePassed = 0.0f;
                 Animator.Play(gameObject, "ST_Die", 1.0f);
                 Audio.PlayAudio(gameObject, "Play_Stormtrooper_Death");
                 RemoveFromSpawner();
             }
         }*/

    }


    public void OnTriggerEnter(GameObject triggeredGameObject)
    {
        //Debug.Log("CS: Collided object: " + gameObject.tag + ", Collider: " + triggeredGameObject.tag);
        /* if (triggeredGameObject.CompareTag("Bullet"))
         {
             // InternalCalls.Destroy(gameObject);
             gameObject.GetComponent<PlayerHealth>().TakeDamage(5);
         }*/

        //Debug.Log("Triggered by tag: " + triggeredGameObject.tag);
    }

}
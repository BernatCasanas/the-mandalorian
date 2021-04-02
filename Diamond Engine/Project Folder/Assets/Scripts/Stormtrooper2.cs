using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Collections.Generic;

using DiamondEngine;

public class StormTrooper2 : Enemy
{
    //public static Enemy instance;

    //enum STATE : int
    //{
    //    NONE = -1,
    //    IDLE,
    //    RUN,
    //    WANDER,
    //    PUSHED,
    //    SHOOT,
    //    HIT,
    //    DIE
    //}

    enum INPUT : int
    {
        IN_IDLE,
        IN_RUN,
        IN_WANDER,
        IN_PUSHED,
        IN_SHOOT,
        IN_HIT,
        IN_DIE
    }

    //VARIABLES DEL STORMTROOPER ORIGINAL
    public GameObject hitParticles;

    private int shotSequences = 0;
    public int maxShots = 2;
    public int maxSequences = 2;

    private float pushSkillTimer = 0.15f;
    private float pushSkillSpeed = 0.2f;
    public float stormTrooperDamage = 5.0f;


    //VARIABLES DE CORE PASTED
    //public GameObject shootPoint = null;
    public GameObject hud = null;

    private bool scriptStart = true;

    //State
    //private STATES currentState = STATES.NONE;   //NEVER SET THIS VARIABLE DIRECTLLY, ALLWAYS USE INPUTS
                                                 //Setting states directlly will break the behaviour  -Jose
    private List<INPUT> inputsList = new List<INPUT>();

    private bool rightTriggerPressed = false;


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
        if (scriptStart == true)
        {
            Start();
            scriptStart = false;
        }

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

        //if (dashTimer > 0)
        //{
        //    dashTimer -= Time.deltaTime;

        //    if (dashTimer <= 0)
        //        //inputsList.Add(INPUT.IN_DASH_END);
        //}

        //if (shootingTimer > 0)
        //{
        //    shootingTimer -= Time.deltaTime;

        //    if (shootingTimer <= 0)
        //    {
        //        //inputsList.Add(INPUT.IN_SHOOT);
        //        Debug.Log("In shoot");
        //    }
        //}
    }


    //Controler inputs go here
    private void ProcessExternalInput()
    {
        //AQUI DEBERIAN ESTAR LAS COSAS QUE HACEN CAMBIAR DE ESTADO (RANGOS DE ACCION)

        //if (Input.GetGamepadButton(DEControllerButton.X) == KeyState.KEY_DOWN || Input.GetGamepadButton(DEControllerButton.X) == KeyState.KEY_REPEAT)
        //    //inputsList.Add(INPUT.IN_SHOOTING);

        //else if ((Input.GetGamepadButton(DEControllerButton.X) == KeyState.KEY_UP || Input.GetGamepadButton(DEControllerButton.X) == KeyState.KEY_IDLE) && hasShot == true && CanStopShooting() == true)
        //{
        //    //inputsList.Add(INPUT.IN_SHOOTING_END);
        //    hasShot = false;
        //}

        //if (IsJoystickMoving() == true)
        //    inputsList.Add(INPUT.IN_MOVE);

        //else if (currentState == STATE.MOVE && IsJoystickMoving() == false)
        //    inputsList.Add(INPUT.IN_IDLE);

        //if (Input.GetRightTrigger() > 0 && rightTriggerPressed == false)
        //{
        //    inputsList.Add(INPUT.IN_DASH);
        //    rightTriggerPressed = true;
        //}
        //else if (Input.GetRightTrigger() == 0 && rightTriggerPressed == true)
        //    rightTriggerPressed = false;

        //if (Input.GetGamepadButton(DEControllerButton.Y) == KeyState.KEY_DOWN && smallGrenades.Count == 0 && BigGrenades.Count == 0)
        //    inputsList.Add(INPUT.IN_SEC_SHOOT);
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
                            StartMove();
                            break;

                        case INPUT.IN_WANDER:
                            currentState = STATE.WANDER;
                            StartDash();
                            break;

                        case INPUT.IN_SHOOT:
                            currentState = STATE.SHOOT;
                            StartShooting();
                            break;

                        case INPUT.IN_HIT:
                            currentState = STATE.HIT;
                            break;

                        case INPUT.IN_DIE:
                            currentState = STATE.DIE;
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
                            StartDash();
                            break;

                        case INPUT.IN_SHOOT:
                            currentState = STATE.SHOOT;
                            StartShooting();
                            break;

                        case INPUT.IN_DIE:
                            currentState = STATE.DIE;
                            break;
                    }
                    break;


                case STATE.WANDER:
                    switch (input)
                    {
                        case INPUT.IN_SHOOT:
                            currentState = STATE.IDLE;
                            EndDash();
                            break;

                        case INPUT.IN_DIE:
                            currentState = STATE.DIE;
                            break;
                    }
                    break;


                case STATE.SHOOT:
                    switch (input)
                    {
                        case INPUT.IN_WANDER:
                            currentState = STATE.WANDER;
                            StartDash();
                            break;

                        case INPUT.IN_RUN:
                            currentState = STATE.IDLE;
                            EndShooting();
                            StartIdle();
                            break;

                        case INPUT.IN_SHOOT:
                            currentState = STATE.SHOOT;
                            StartShoot();
                            break;

                        case INPUT.IN_DIE:
                            currentState = STATE.DIE;
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
                UpdateMove();
                break;
            case STATE.WANDER:
                UpdateDash();
                break;
            case STATE.SHOOT:
                break;
            case STATE.DIE:
                break;
            default:
                Debug.Log("NEED TO ADD STATE TO CORE");
                break;
        }
    }


    #region SHOOT

    private void StartShooting()
    {
        fireRate = GetCurrentFireRate();
        Animator.Play(gameObject, "Shoot", normalShootSpeed);

        shootingTimer = fireRate;

        Debug.Log(fireRate.ToString());

        if (myAimbot != null)
        {
            myAimbot.isShooting = true;
            myAimbot.SearchForNewObjective();
        }
    }


    private void UpdateShooting()
    {
        if (myAimbot != null && myAimbot.HasObjective())
            myAimbot.RotateToObjective();
        else if (IsJoystickMoving() == true)
            RotatePlayer();
    }

    private void EndShooting()
    {
        if (myAimbot != null)
            myAimbot.isShooting = false;
    }

    private bool CanStopShooting()
    {
        return shootingTimer > fireRate * 0.5 ? true : false;
    }

    private void StartShoot()
    {
        Audio.StopAudio(gameObject);
        Audio.PlayAudio(shootPoint, "Play_Blaster_Shoot_Mando");
        Input.PlayHaptic(.3f, 10);

        InternalCalls.CreatePrefab("Library/Prefabs/346087333.prefab", shootPoint.transform.globalPosition, shootPoint.transform.globalRotation, shootPoint.transform.globalScale);

        inputsList.Add(INPUT.IN_SHOOT_END);
        hasShot = true;
    }
    #endregion

    #region RUN
    private void StartDash()
    {
        Audio.StopAudio(gameObject);
        Audio.PlayAudio(gameObject, "Play_Dash");
        Animator.Play(gameObject, "Dash");

        dashTimer = dashDuration;
        dashStartYPos = gameObject.transform.localPosition.y;
    }

    private void UpdateDash()
    {
        StopPlayer();
        gameObject.AddForce(gameObject.transform.GetForward().normalized * dashforce);
    }

    private void EndDash()
    {
        StopPlayer();
        gameObject.transform.localPosition.y = dashStartYPos;
    }

    #endregion

    #region MOVE AND ROTATE ENEMY
    private void StartMove()
    {
        Animator.Play(gameObject, "Run");
        Audio.PlayAudio(this.gameObject, "Play_Footsteps_Mando");
    }

    private void UpdateMove()
    {
        RotatePlayer();
        gameObject.SetVelocity(gameObject.transform.GetForward() * movementSpeed);
    }

    private void StopPlayer()
    {
        Debug.Log("Stoping");
        gameObject.SetVelocity(new Vector3(0, 0, 0));
    }

    private void RotatePlayer()
    {
        //Calculate player rotation
        Vector3 aX = new Vector3(gamepadInput.x, 0, -gamepadInput.y - 1);
        Vector3 aY = new Vector3(0, 0, 1);
        aX = Vector3.Normalize(aX);

        if (aX.x >= 0)
        {
            angle = Math.Acos(Vector3.Dot(aX, aY) - 1);
        }
        else if (aX.x < 0)
        {
            angle = -Math.Acos(Vector3.Dot(aX, aY) - 1);
        }

        //Convert angle from world view to orthogonal view
        angle += 0.785398f; //Rotate 45 degrees to the right

        gameObject.transform.localRotation = Quaternion.RotateAroundAxis(Vector3.up, (float)-angle);
    }

    private void StartIdle()
    {
        Animator.Play(gameObject, "Idle");
    }

    #endregion

 
    public void OnCollisionEnter(GameObject collidedGameObject)
    {
        //Debug.Log("CS: Collided object: " + gameObject.tag + ", Collider: " + collidedGameObject.tag);
        //Debug.Log("Collided by tag: " + collidedGameObject.tag);

        if (collidedGameObject.CompareTag("Bullet"))
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
        }

    }


    public void OnTriggerEnter(GameObject triggeredGameObject)
    {
        //Debug.Log("CS: Collided object: " + gameObject.tag + ", Collider: " + triggeredGameObject.tag);
        if (triggeredGameObject.CompareTag("Bullet"))
        {
            // InternalCalls.Destroy(gameObject);
            gameObject.GetComponent<PlayerHealth>().TakeDamage(5);
        }

        //Debug.Log("Triggered by tag: " + triggeredGameObject.tag);
    }

}
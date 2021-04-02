using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Collections.Generic;

using DiamondEngine;

public class Core : DiamondComponent
{
    public static Core instance;

    enum STATE : int
    {
        NONE = -1,
        IDLE,
        MOVE,
        DASH,
        SHOOTING,
        SHOOT,
        SECONDARY_SHOOT,
        DEAD
    }

    enum INPUT : int
    {
        IN_IDLE,
        IN_MOVE,
        IN_DASH,
        IN_DASH_END,
        IN_SHOOTING,
        IN_SHOOTING_END,
        IN_SHOOT,
        IN_SHOOT_END,
        IN_SEC_SHOOT,
        IN_DEAD
    }

    public GameObject shootPoint = null;
    public GameObject hud = null;

    private bool scriptStart = true;

    //State
    private STATE currentState = STATE.NONE;   //NEVER SET THIS VARIABLE DIRECTLLY, ALLWAYS USE INPUTS
                                               //Setting states directlly will break the behaviour  -Jose
    private List<INPUT> inputsList = new List<INPUT>();

    private bool rightTriggerPressed = false;

    // Movement
    public float rotationSpeed = 2.0f;
    public float movementSpeed = 35.0f;
    public float mouseSens = 1.0f;
    private double angle = 0.0f;

    // Dash
    private float timeSinceLastDash = 0.0f;
    public float dashCD = 0.33f;
    public float dashDuration = 0.25f;
    public float dashDistance = 1.0f;
    public float dashforce = 1000f;
    private float dashSpeed = 0.0f;
    private float dashTimer = 0.0f;
    private float dashStartYPos = 0.0f;

    // Shooting
    public float fireRate = 0.2f;
    private float shootingTimer = 0.0f;
    public float secondaryRate = 0.2f;

    private bool hasShot = false;

    public float fireRateAfterDashRecoverRatio = 2.0f;
    private float secondaryRateRecoverCap = 0.0f;

    public float fireRateMultCap = 0.0f;
    private int deathZone = 15000;
    public float normalShootSpeed = 0.0f;

    //Grenades
    public float grenadesFireRate = 0.0f;
    //private float grenadesTimer = 0.0f;

    //Animations
    private float shootAnimationTotalTime = 0.0f;
    public string currentStateString = "";

    //Controller Variables
    int verticalInput = 0;
    int horizontalInput = 0;
    Vector3 gamepadInput;

    public List<smallGrenade> smallGrenades = new List<smallGrenade>();
    public List<bigGrenade> BigGrenades = new List<bigGrenade>();

    //For Pause
    public GameObject background = null;
    public GameObject pause = null;

    private Vector3 mySpawnPos = new Vector3(0.0f, 0.0f, 0.0f);
    // private Pause aux = null;

    AimBot myAimbot = null;

    private void Start()
    {
        #region VARIABLES WITH DEPENDENCIES

        // INIT VARIABLES WITH DEPENDENCIES //

        //Shoot
        secondaryRate = 0.2f;

        //Animation
        shootAnimationTotalTime = 0.288f;

        //Dash - if scene doesnt have its values
        //dashDuration = 0.2f;
        //dashDistance = 4.5f;

        // END INIT VARIABLES WITH DEPENDENCIES //
        smallGrenades = new List<smallGrenade>();
        BigGrenades = new List<bigGrenade>();
        #endregion

        #region SHOOT

        normalShootSpeed = shootAnimationTotalTime / fireRate;
        //fireRateAfterDashRecoverRatio = 2f;
        //fireRateMultCap = 2.5f;
        secondaryRateRecoverCap = 3.0f / secondaryRate;
        myAimbot = gameObject.GetComponent<AimBot>();
        #endregion

        #region DASH

        // Dash
        dashTimer = 0f;
        dashSpeed = dashDistance / dashDuration;
        //dashAvaliable = true;

        #endregion

        #region OTHERS

        //player instance
        instance = this;

        //Controller
        deathZone = 15000;

        currentState = STATE.IDLE;

        Debug.Log("Start!");
        mySpawnPos = new Vector3(gameObject.transform.globalPosition.x, gameObject.transform.globalPosition.y, gameObject.transform.globalPosition.z);
        #endregion
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

        if (Input.GetKey(DEKeyCode.C) == KeyState.KEY_DOWN)
        {
            Audio.SetState("Game_State", "Run");
            if (MusicSourceLocate.instance != null)
            {
                Audio.SetSwitch(MusicSourceLocate.instance.gameObject, "Player_Action", "Combat");
                Audio.SetSwitch(MusicSourceLocate.instance.gameObject, "Player_Health", "Healthy");
            }
        }

        UpdateControllerInputs();

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
        if (dashTimer > 0)
        {
            dashTimer -= Time.deltaTime;

            if (dashTimer <= 0)
                inputsList.Add(INPUT.IN_DASH_END);
        }

        if (shootingTimer > 0)
        {
            shootingTimer -= Time.deltaTime;

            if (shootingTimer <= 0)
            {
                inputsList.Add(INPUT.IN_SHOOT);
                Debug.Log("In shoot");
            }
        }
    }


    //Controler inputs go here
    private void ProcessExternalInput()
    {
        if (Input.GetGamepadButton(DEControllerButton.X) == KeyState.KEY_DOWN || Input.GetGamepadButton(DEControllerButton.X) == KeyState.KEY_REPEAT)
            inputsList.Add(INPUT.IN_SHOOTING);

        else if ((Input.GetGamepadButton(DEControllerButton.X) == KeyState.KEY_UP || Input.GetGamepadButton(DEControllerButton.X) == KeyState.KEY_IDLE) && hasShot == true && CanStopShooting() == true)
        {
            inputsList.Add(INPUT.IN_SHOOTING_END);
            hasShot = false;
        }
    }

        if (IsJoystickMoving() == true)
            inputsList.Add(INPUT.IN_MOVE);

        else if (currentState == STATE.MOVE && IsJoystickMoving() == false)
            inputsList.Add(INPUT.IN_IDLE);

        if (Input.GetRightTrigger() > 0 && rightTriggerPressed == false)
        {
            inputsList.Add(INPUT.IN_DASH);
            rightTriggerPressed = true;
        }
        else if (Input.GetRightTrigger() == 0 && rightTriggerPressed == true)
            rightTriggerPressed = false;

        if (Input.GetGamepadButton(DEControllerButton.Y) == KeyState.KEY_DOWN && smallGrenades.Count == 0 && BigGrenades.Count == 0)
            inputsList.Add(INPUT.IN_SEC_SHOOT);
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
                        case INPUT.IN_MOVE:
                            currentState = STATE.MOVE;
                            StartMove();
                            break;

                        case INPUT.IN_DASH:
                            currentState = STATE.DASH;
                            StartDash();
                            break;

                        case INPUT.IN_SHOOTING:
                            currentState = STATE.SHOOTING;
                            StartShooting();
                            break;

                        case INPUT.IN_SEC_SHOOT:
                            currentState = STATE.SECONDARY_SHOOT;
                            break;

                        case INPUT.IN_DEAD:
                            break;
                    }
                    break;


                case STATE.MOVE:
                    switch (input)
                    {
                        case INPUT.IN_IDLE:
                            currentState = STATE.IDLE;
                            StartIdle();
                            break;

                        case INPUT.IN_DASH:
                            currentState = STATE.DASH;
                            StartDash();
                            break;

                        case INPUT.IN_SHOOTING:
                            currentState = STATE.SHOOTING;
                            StartShooting();
                            break;

                        case INPUT.IN_DEAD:
                            break;
                    }
                    break;


                case STATE.DASH:
                    switch (input)
                    {
                        case INPUT.IN_DASH_END:
                            currentState = STATE.IDLE;
                            EndDash();
                            break;

                        case INPUT.IN_DEAD:
                            break;
                    }
                    break;


                case STATE.SHOOTING:
                    switch (input)
                    {
                        case INPUT.IN_DASH:
                            currentState = STATE.DASH;
                            StartDash();
                            break;

                        case INPUT.IN_SHOOTING_END:
                            currentState = STATE.IDLE;
                            EndShooting();
                            StartIdle();
                            break;

                        case INPUT.IN_SHOOT:
                            currentState = STATE.SHOOT;
                            StartShoot();
                            break;

                        case INPUT.IN_DEAD:
                            break;
                    }
                    break;

                case STATE.SHOOT:
                    switch (input)
                    {
                        case INPUT.IN_SHOOT_END:
                            currentState = STATE.SHOOTING;
                            StartShooting();
                            break;

                        case INPUT.IN_DEAD:
                            break;
                    }
                    break;

                case STATE.SECONDARY_SHOOT:
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
            case STATE.MOVE:
                UpdateMove();
                break;
            case STATE.DASH:
                UpdateDash();
                break;
            case STATE.SHOOTING:
                UpdateShooting();
                break;
            case STATE.SHOOT:
                break;
            case STATE.SECONDARY_SHOOT:
                break;
            case STATE.DEAD:
                break;
            default:
                Debug.Log("NEED TO ADD STATE TO CORE");
                break;
        }
    }


    #region NORMAL SHOOT

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
        if (IsJoystickMoving() == true)
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

    /* #region SPECIAL SHOOT
     private void SecondaryShootInput()
     {



     }

     private void HandleSecondaryShoot()
     {
         Audio.StopAudio(gameObject);
         Vector3 scale = new Vector3(0.2f, 0.2f, 0.2f);
         Vector3 rot = new Vector3(0f, 1f, 0f);
         Quaternion rotation = Quaternion.RotateAroundAxis(rot, 0.383972f);

         Audio.PlayAudio(shootPoint, "Play_Weapon_Shoot_Mando");
         InternalCalls.CreatePrefab("Library/Prefabs/142833782.prefab", shootPoint.transform.globalPosition, shootPoint.transform.globalRotation * rotation, scale);
         rotation = Quaternion.RotateAroundAxis(rot, -0.383972f);
         InternalCalls.CreatePrefab("Library/Prefabs/142833782.prefab", shootPoint.transform.globalPosition, shootPoint.transform.globalRotation * rotation, scale);

         grenadesTimer = 0.0f;

         Input.PlayHaptic(2f, 30);

         if (fireButtonPressed == true)
         {
             ChangeState(State.Shoot);
         }
         else
         {
             if (IsJoystickMoving())
             {
                 ChangeState(State.Run);
             }
             else
             {
                 ChangeState(State.Idle);

             }
         }

     }

     #endregion*/

    #region DASH
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

    #region MOVE AND ROTATE PLAYER
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

    #region UTILITIES

    private void UpdateControllerInputs()
    {
        //Check if user is moving joystick
        //if (IsJoystickMoving() == true)
        //{
        verticalInput = Input.GetLeftAxisX();
        horizontalInput = Input.GetLeftAxisY();

        gamepadInput = new Vector3(horizontalInput, -verticalInput, 0f);
        //}

        //else
        //gamepadInput = new Vector3(0f, 0f, 0f);

        /*if (Input.GetGamepadButton(DEControllerButton.START) == KeyState.KEY_DOWN)
        {
            Audio.StopAudio(gameObject);
            pause.Enable(true);
            aux = pause.GetComponent<Pause>();
            aux.DisplayBoons();
            background.Enable(true);
            Time.PauseGame();
        }*/
    }

    private float GetCurrentFireRate()
    {

        return fireRate;
    }

    private float GetCurrentSecondaryRate()
    {
        float ret = secondaryRate;

        ret = (float)(Math.Log(timeSinceLastDash * fireRateAfterDashRecoverRatio) - Math.Log(0.01)) / secondaryRateRecoverCap;

        ret = Math.Min(ret, secondaryRate * fireRateMultCap);
        //Debug.Log("New fire rate: " + ret.ToString());

        return ret;

    }

    private bool IsJoystickMoving()
    {
        return gamepadInput.magnitude > deathZone;
    }

    #endregion


    public void OnCollisionEnter(GameObject collidedGameObject)
    {
        //Debug.Log("CS: Collided object: " + gameObject.tag + ", Collider: " + collidedGameObject.tag);
        //Debug.Log("Collided by tag: " + collidedGameObject.tag);

        if (collidedGameObject.CompareTag("StormTrooperBullet"))
        {
            //InternalCalls.Destroy(gameObject);
            BH_Bullet bulletScript = collidedGameObject.GetComponent<BH_Bullet>();

            if (bulletScript != null)
                gameObject.GetComponent<PlayerHealth>().TakeDamage((int)bulletScript.damage);
        }
        if (collidedGameObject.CompareTag("Bantha"))
        {
            //InternalCalls.Destroy(gameObject);
            float damage = collidedGameObject.GetComponent<Enemy>().damage;
            Debug.Log("Me cago en diamond engine");


            if (damage != 0)
                gameObject.GetComponent<PlayerHealth>().TakeDamage((int)damage);
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

    #region UTILITIES

    private void UpdateControllerInputs()
    {
        //Check if user is moving joystick
        //if (IsJoystickMoving() == true)
        //{
        verticalInput = Input.GetLeftAxisX();
        horizontalInput = Input.GetLeftAxisY();

        gamepadInput = new Vector3(horizontalInput, -verticalInput, 0f);
        //}

        //else
        //gamepadInput = new Vector3(0f, 0f, 0f);

        /*if (Input.GetGamepadButton(DEControllerButton.START) == KeyState.KEY_DOWN)
        {
            Audio.StopAudio(gameObject);
            pause.Enable(true);
            aux = pause.GetComponent<Pause>();
            aux.DisplayBoons();
            background.Enable(true);
            Time.PauseGame();
        }*/
    }

    /*
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
    */

    public void RespawnOnFall()
    {

        gameObject.transform.localPosition = mySpawnPos;
        PlayerHealth myHealth = gameObject.GetComponent<PlayerHealth>();
        if (myHealth != null)
        {
            int finalHealthSubstract = (int)(PlayerHealth.currMaxHealth * 0.25f);

            if (PlayerHealth.currHealth - finalHealthSubstract <= 0)
            {
                finalHealthSubstract = PlayerHealth.currHealth - 1;
            }

            myHealth.TakeDamage(finalHealthSubstract); //TODO whats the right amount we have to substract?

        }
    }

}
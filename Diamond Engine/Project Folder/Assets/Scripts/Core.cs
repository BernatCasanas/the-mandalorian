using DiamondEngine;
using System;
using System.Collections.Generic;

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
        GADGET_SHOOT,
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
        IN_GADGET_SHOOT,
        IN_GADGET_SHOOT_END,
        IN_SEC_SHOOT,
        IN_SEC_SHOOT_END,
        IN_DEAD
    }

    enum PARTICLES : int
    {
        NONE = -1,
        DUST,
        MUZZLE,
        JETPACK,
        IMPACT
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
    private float runTime = 0.0f;
    private float dustTime = 0.0f;


    // Dash
    //private float timeSinceLastDash = 0.0f;
    public float dashCD = 0.33f;
    public float dashDuration = 0.2f;
    public float dashSpeed = 30.0f;
    private float dashTimer = 0.0f;
    private float dashCDTimer = 0.0f;
    private float dashStartYPos = 0.0f;
    private bool dashAvaliable = true;

    // Shooting
    public float fireRate = 0.2f;
    public float gadgetFireRate = 0.2f;
    private float shootingTimer = 0.0f;
    private float gadgetShootTimer = 0.0f;

    private bool hasShot = false;

    public float fireRateAfterDashRecoverRatio = 2.0f;
    //private float secondaryRateRecoverCap = 0.0f;

    public float fireRateMultCap = 0.0f;
    private int deathZone = 15000;
    private float normalShootSpeed = 0.0f;
    private float gadgetShootSkill = 0.0f;

    //Grenades
    public static float grenadesFireRate;
    private float grenadesFireRateTimer = 0.0f;
    //private float grenadesTimer = 0.0f;

    //Animations
    private float shootAnimationTotalTime = 0.0f;

    //Controller Variables
    int verticalInput = 0;
    int horizontalInput = 0;
    Vector3 gamepadInput;
    public bool lockInputs = false;

    //For Pause
    public GameObject background = null;
    public GameObject pause = null;

    private Vector3 mySpawnPos = new Vector3(0.0f, 0.0f, 0.0f);

    AimBot myAimbot = null;

    private static float bulletDamage = 9f;
    private float bulletDamageDefault = 9f;

    private void Start()
    {
        #region VARIABLES WITH DEPENDENCIES

        // INIT VARIABLES WITH DEPENDENCIES //
        //Animation
        shootAnimationTotalTime = 0.288f;
        lockInputs = false;
        //Dash - if scene doesnt have its values
        //dashDuration = 0.2f;
        //dashDistance = 4.5f;
        #endregion

        #region SHOOT

        normalShootSpeed = shootAnimationTotalTime / fireRate;
        gadgetShootSkill = shootAnimationTotalTime / gadgetFireRate;

        myAimbot = gameObject.GetComponent<AimBot>();

        grenadesFireRate = 4.0f;
        #endregion

        #region DASH

        // Dash
        dashTimer = 0.0f;
        //dashSpeed = dashDistance / dashDuration;
        
        #endregion

        #region OTHERS

        //player instance
        instance = this;

        //Controller
        deathZone = 15000;

        currentState = STATE.IDLE;

        Debug.Log("Start!");
        mySpawnPos = new Vector3(gameObject.transform.globalPosition.x, gameObject.transform.globalPosition.y, gameObject.transform.globalPosition.z);
        runTime = Animator.GetAnimationDuration(gameObject, "Run") / 2;
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

        if (dashCDTimer > 0 && dashAvaliable == false)
        {
            dashCDTimer -= Time.deltaTime;

            if (dashCDTimer <= 0)
                dashAvaliable = true;
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

        if (gadgetShootTimer > 0)
        {
            gadgetShootTimer -= Time.deltaTime;

            if (gadgetShootTimer <= 0)
                inputsList.Add(INPUT.IN_GADGET_SHOOT_END);
        }
    }


    //Controler inputs go here
    private void ProcessExternalInput()
    {
        if (!lockInputs)
        {
            if (Input.GetGamepadButton(DEControllerButton.X) == KeyState.KEY_DOWN || Input.GetGamepadButton(DEControllerButton.X) == KeyState.KEY_REPEAT)
                inputsList.Add(INPUT.IN_SHOOTING);

            else if ((Input.GetGamepadButton(DEControllerButton.X) == KeyState.KEY_UP || Input.GetGamepadButton(DEControllerButton.X) == KeyState.KEY_IDLE) && hasShot == true && CanStopShooting() == true)
            {
                inputsList.Add(INPUT.IN_SHOOTING_END);
                hasShot = false;
            }

            if (IsJoystickMoving() == true)
                inputsList.Add(INPUT.IN_MOVE);

            else if (currentState == STATE.MOVE && IsJoystickMoving() == false)
                inputsList.Add(INPUT.IN_IDLE);

            if (Input.GetRightTrigger() > 0 && rightTriggerPressed == false && dashAvaliable == true)
            {
                inputsList.Add(INPUT.IN_DASH);
                rightTriggerPressed = true;
            }
            else if (Input.GetRightTrigger() == 0 && rightTriggerPressed == true)
                rightTriggerPressed = false;


            if (Input.GetKey(DEKeyCode.LSHIFT) == KeyState.KEY_REPEAT && Input.GetKey(DEKeyCode.LALT) == KeyState.KEY_REPEAT && 
                Input.GetKey(DEKeyCode.D) == KeyState.KEY_DOWN && Time.deltaTime != 0.0f)
            {
                InternalCalls.CreateUIPrefab("Library/Prefabs/1871660106.prefab", new Vector3(0, 0, 0), new Quaternion(0, 0, 0), new Vector3(1, 1, 1));
            }


            if (Input.GetGamepadButton(DEControllerButton.Y) == KeyState.KEY_DOWN && grenadesFireRateTimer <= 0.0f)
            {
                inputsList.Add(INPUT.IN_GADGET_SHOOT);
                grenadesFireRateTimer = grenadesFireRate;
            }
        }
        grenadesFireRateTimer -= Time.deltaTime;
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

                        case INPUT.IN_GADGET_SHOOT:
                            currentState = STATE.GADGET_SHOOT;
                            StartGadgetShoot();
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
                            MoveEnd();
                            StartIdle();
                            break;

                        case INPUT.IN_DASH:
                            currentState = STATE.DASH;
                            MoveEnd();
                            StartDash();
                            break;

                        case INPUT.IN_SHOOTING:
                            currentState = STATE.SHOOTING;
                            MoveEnd();
                            StartShooting();
                            break;

                        case INPUT.IN_GADGET_SHOOT:
                            currentState = STATE.GADGET_SHOOT;
                            MoveEnd();
                            StartGadgetShoot();
                            break;

                        case INPUT.IN_DEAD:
                            MoveEnd();
                            break;
                    }
                    break;


                case STATE.DASH:
                    switch (input)
                    {
                        case INPUT.IN_DASH_END:
                            currentState = STATE.IDLE;
                            EndDash();
                            StartIdle();
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

                case STATE.GADGET_SHOOT:
                    switch (input)
                    {
                        case INPUT.IN_GADGET_SHOOT_END:
                            currentState = STATE.IDLE;
                            EndGadgetShoot();
                            StartIdle();
                            break;

                        case INPUT.IN_DEAD:
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

    #region IDLE
    private void StartIdle()
    {
        Animator.Play(gameObject, "Idle");
    }
    #endregion

    #region NORMAL SHOOT

    private void StartShooting()
    {
        Animator.Play(gameObject, "Shoot", normalShootSpeed);

        shootingTimer = fireRate;
        PlayParticles(PARTICLES.MUZZLE);
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
        if (hud != null)
            hud.GetComponent<HUD>().ShootSwapImage(true);
        GameObject bullet = InternalCalls.CreatePrefab("Library/Prefabs/1821505626.prefab", shootPoint.transform.globalPosition, shootPoint.transform.globalRotation, shootPoint.transform.globalScale);
        if(bullet!=null)
            bullet.GetComponent<BH_Bullet>().damage = bulletDamage;
        inputsList.Add(INPUT.IN_SHOOT_END);
        hasShot = true;
    }


    private void StartGadgetShoot()
    {
        Animator.Play(gameObject, "Shoot", gadgetShootSkill);
        gadgetShootTimer = gadgetFireRate;
    }


    private void EndGadgetShoot()
    {
        Audio.StopAudio(gameObject);
        Audio.PlayAudio(shootPoint, "Play_Weapon_Shoot_Mando");

        Input.PlayHaptic(2f, 30);

        Vector3 scale = new Vector3(0.2f, 0.2f, 0.2f);
        Quaternion rotation = Quaternion.RotateAroundAxis(Vector3.up, 0.383972f);

        //TODO: Some of this is hardcoded, will change it once I have all the Mando's new numbers. Besis, Alex <3

        bigGrenade grenadeComp = InternalCalls.CreatePrefab("Library/Prefabs/142833782.prefab", shootPoint.transform.globalPosition - 0.5f, shootPoint.transform.globalRotation * rotation, scale).GetComponent<bigGrenade>();
        
        if(grenadeComp != null)
        {
            Vector3 targetPos = grenadeComp.gameObject.transform.globalPosition + grenadeComp.gameObject.transform.GetForward() * 1.9f;
            grenadeComp.InitGrenade(targetPos, 0.204f, 10);
        }
        
        rotation = Quaternion.RotateAroundAxis(Vector3.up, -0.383972f);
        grenadeComp = InternalCalls.CreatePrefab("Library/Prefabs/142833782.prefab", shootPoint.transform.globalPosition - 0.5f, shootPoint.transform.globalRotation * rotation, scale).GetComponent<bigGrenade>();

        if (grenadeComp != null)
        {
            Vector3 targetPos = grenadeComp.gameObject.transform.globalPosition + grenadeComp.gameObject.transform.GetForward() * 1.9f;
            grenadeComp.InitGrenade(targetPos, 0.204f, 10);
        }

    }

    public void IncreaseNormalShootDamage(float percent)
    {
        bulletDamage += (bulletDamage * percent);
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

        PlayParticles(PARTICLES.JETPACK);
    }

    private void UpdateDash()
    {
        StopPlayer();
        //gameObject.AddForce(gameObject.transform.GetForward().normalized * dashSpeed);
        gameObject.transform.localPosition = gameObject.transform.localPosition + gameObject.transform.GetForward().normalized * dashSpeed * Time.deltaTime;
    }

    private void EndDash()
    {
        dashCDTimer = dashCD;
        dashAvaliable = false;
        //Debug.Log(dashCDTimer.ToString());
        //Debug.Log(dashAvaliable.ToString());

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
        dustTime += Time.deltaTime;
        if (dustTime >= runTime)
        {

            PlayParticles(PARTICLES.DUST);
            dustTime = 0;
        }


        //gameObject.SetVelocity(gameObject.transform.GetForward() * movementSpeed);
        gameObject.transform.localPosition = gameObject.transform.localPosition + gameObject.transform.GetForward().normalized * movementSpeed * Time.deltaTime;
    }
    private void MoveEnd()
    {
        Audio.StopAudio(gameObject);
    }

    private void StopPlayer()
    {
        // Debug.Log("Stoping");
        gameObject.SetVelocity(new Vector3(0, 0, 0));
    }

    private void RotatePlayer()
    {
        //Calculate player rotation
        Vector3 aX = new Vector3(gamepadInput.x, 0, (gamepadInput.y + 1) * -1);
        Vector3 aY = new Vector3(0, 0, 1);
        aX = Vector3.Normalize(aX);
        if (aX == Vector3.zero)
            return;

        if (aX.x >= 0)
        {
            angle = Math.Acos(Vector3.Dot(aX, aY));
        }
        else if (aX.x < 0)
        {
            angle = -Math.Acos(Vector3.Dot(aX, aY));
        }

        //Convert angle from world view to orthogonal view
        angle += 0.785398f; //Rotate 45 degrees to the right

        gameObject.transform.localRotation = Quaternion.RotateAroundAxis(Vector3.up, (float)-angle);
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

        if (Input.GetGamepadButton(DEControllerButton.START) == KeyState.KEY_DOWN)
        {
            Audio.StopAudio(gameObject);

            if (pause != null)
            {
                pause.EnableNav(true);
                pause.GetComponent<Pause>().DisplayBoons();
                background.Enable(true);
                Time.PauseGame();
            }

            else
                Debug.Log("Need to add pause GO");
        }
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
            PlayParticles(PARTICLES.IMPACT);
            BH_Bullet bulletScript = collidedGameObject.GetComponent<BH_Bullet>();
            Audio.PlayAudio(gameObject, "Play_Mando_Hit");

            if (bulletScript != null)
                gameObject.GetComponent<PlayerHealth>().TakeDamage((int)bulletScript.damage);
        }
        if (collidedGameObject.CompareTag("Bantha"))
        {
            //InternalCalls.Destroy(gameObject);
            Audio.PlayAudio(gameObject, "Play_Mando_Hit");
            float damage = collidedGameObject.GetComponent<Enemy>().damage;

            if (damage != 0)
                gameObject.GetComponent<PlayerHealth>().TakeDamage((int)damage);
        }
    }

    
    public void OnTriggerEnter(GameObject triggeredGameObject)
    {
        if (triggeredGameObject.CompareTag("Coin"))
        {
            PlayerResources.AddRunCoins(1);
            hud.GetComponent<HUD>().UpdateCurrency(PlayerResources.GetRunCoins());
            InternalCalls.Destroy(triggeredGameObject);
        }
    }
    

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

    private void PlayParticles(PARTICLES particletype)
    {
        PlayerParticles myParticles = gameObject.GetComponent<PlayerParticles>();
        ParticleSystem particle = null;

        switch (particletype)
        {
            case PARTICLES.NONE:
                break;
            case PARTICLES.DUST:
                if (myParticles != null)
                {
                    particle = myParticles.dust;
                    if (particle != null)
                        particle.Play();
                    else
                        Debug.Log("Jetpack particle not found");
                }
                else
                    Debug.Log("Component Particles not found");
                break;

            case PARTICLES.IMPACT:
                if (myParticles != null)
                {
                    particle = myParticles.impact;
                    if (particle != null)
                        particle.Play();
                    else
                        Debug.Log("Jetpack particle not found");
                }
                else
                    Debug.Log("Component Particles not found");
                break;

            case PARTICLES.JETPACK:
                if (myParticles != null)
                {
                    particle = myParticles.jetpack;
                    if (particle != null)
                        particle.Play();
                    else
                        Debug.Log("Jetpack particle not found");
                }
                else
                    Debug.Log("Component Particles not found");
                break;

            case PARTICLES.MUZZLE:
                if (myParticles != null)
                {
                    particle = myParticles.muzzle;
                    if (particle != null)
                        particle.Play();
                    else
                        Debug.Log("Jetpack particle not found");
                }
                else
                    Debug.Log("Component Particles not found");
                break;

        }
    }

    public void SetSkill(string skillName, float value = 0.0f)
    {
        if (skillName == "SeDelay") //Secondary Delay Skill
        {
            grenadesFireRate -= grenadesFireRate * value;
        }
    }

    public void OnApplicationQuit()
    {
        bulletDamage = bulletDamageDefault;
    }
}
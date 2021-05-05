using DiamondEngine;
using System;
using System.Collections.Generic;

public class Core : Entity
{
    public static Core instance;
    private static Dictionary<STATUS_TYPE, StatusData> PlayerStatuses = new Dictionary<STATUS_TYPE, StatusData>();

    public enum STATE : int
    {
        NONE = -1,
        IDLE,
        MOVE,
        DASH,
        SHOOTING,
        SHOOT,
        GADGET_SHOOT,
        CHARGING_SEC_SHOOT,
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
        IN_CHARGE_SEC_SHOOT,
        IN_CHARGE_SEC_SHOOT_END,
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
        IMPACT,
        SNIPER,
        GRENADE
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
    public float afterShootDelay = 2.5f;
    public float afterShootMult = 1.5f;
    private double angle = 0.0f;
    private float runTime = 0.0f;
    private float dustTime = 0.0f;
    private float currAnimationPlaySpd = 0f;
    private float footStepTimer = 0f;

    //Skill
    private bool skill_damageReductionDashActive = false;
    private float skill_damageReductionDashTimer = 0.0f;
    private bool skill_groguIncreaseDamageActive = false;
    private float skill_groguIncreaseDamageTimer = 0.0f;

    // Dash
    public float dashCD = 0.33f;
    public float dashDuration = 0.2f;
    public float dashDistance = 6f;
    private float dashSpeed = 30.0f;
    public int maxDashNumber = 1;
    public float betweeMultiplenDashesCD = 0.12f;
    private float dashTimer = 0.0f;
    private float dashCDTimer = 0.0f;
    private int currentDashes = 1;
    private float betweenDashesCDTimer = 0.0f;
    private float dashStartYPos = 0.0f;
    private bool dashAvailable = true;
    private float timeSinceLastDash = 0.0f;
    public GameObject RayCastObj = null;
    private Vector3 dashDirection = new Vector3(0, 0, 0);
    private Quaternion preDashRotation = new Quaternion(0, 0, 0, 1);

    // Shooting
    public float baseFireRate = 0.2f;
    private float currFireRate = 0f;
    public float gadgetFireRate = 0.2f;
    public float heatReductionSpeed = 22.5f;
    public float dashHeatReductionMult = 0.75f;
    public float onGrenadeHeatReduction = 5f;
    public float onSniperHeatReduction = 10f;
    private float shootingTimer = 0.0f;
    private float gadgetShootTimer = 0.0f;
    private bool primaryWeaponOverHeat = false;

    private bool hasShot = false;

    public float fireRateAfterDashRecoverRatio = 2.0f;
    //private float secondaryRateRecoverCap = 0.0f;

    public float fireRateMultCap = 0.0f;
    private int deathZone = 15000;
    private float normalShootSpeed = 0.0f;
    private float gadgetShootSkill = 0.0f;
    private float stopShootingTime = 0f;

    //Grenades
    private static float grenadesFireRate;
    private float grenadesFireRateTimer = 0.0f;
    private bool grenade_reloading = false;

    private Material grenadeCooldownIcon = null;
    private Material sniperBullet1 = null;
    private Material sniperBullet2 = null;

    // Secondary Shoot (sniper)
    public float timeToPerfectCharge = 0.424f;
    public int framesForPerfectCharge = 4;
    private float timeToPerfectChargeEnd = 0f;
    public float timeToAutomaticallyShootCharge = 2.296f;
    public float bulletRechargeTime = 0f;
    public int numberOfBullets = 0;
    private int currentBullets = 0;
    private float totalRechargeTime = 0f;
    private float sniperRechargeTimer = 0f;
    private float chargeTimer = 0.0f;
    public float chargedBulletDmg = 0f;
    public float perfectShotDmgMult = 1.4f;
    public float overShotDmgMult = 1.05f;
    public Vector3 defaultSniperLaserColor = new Vector3(1, 0, 0);
    private Vector3 currSniperLaserColor = new Vector3(1, 0, 0);
    public float overheatTimeBeeping = 0.08f;
    private float changeColorSniperTimer = 0f;

    //Animations
    private float shootAnimationTotalTime = 0.0f;

    //Controller Variables
    int verticalInput = 0;
    int horizontalInput = 0;
    Vector3 gamepadInput = Vector3.zero;
    public bool lockInputs = false;
    private bool lockAttacks = false;

    //For Pause
    public GameObject background = null;
    public GameObject pause = null;

    private Vector3 mySpawnPos = new Vector3(0.0f, 0.0f, 0.0f);

    AimBot myAimbot = null;

    private static float bulletDamage = 9f;
    private float bulletDamageDefault = 9f;

    //Audio
    public GameObject secSound = null;

    //Telemetry
    int numberOfShots = 0;
    int damageTaken = 0;
    int timesFellOfMap = 0;
    float distanceMoved = 0f;
    float timeOfRoom = 0f;


    public void Awake()
    {
        #region VARIABLES WITH DEPENDENCIES

        //bulletDamage = 1000.0f;  //Please do not delete this, it's for quick debugging purposes

        // INIT VARIABLES WITH DEPENDENCIES //
        //Animation
        shootAnimationTotalTime = 0.288f;

        //Dash - if scene doesnt have its values
        //dashDuration = 0.2f;
        //dashDistance = 4.5f;

        #endregion

        #region SHOOT

        baseFireRate = Math.Max(baseFireRate, 0.1f);

        normalShootSpeed = shootAnimationTotalTime / baseFireRate;
        gadgetShootSkill = shootAnimationTotalTime / gadgetFireRate;

        myAimbot = gameObject.GetComponent<AimBot>();

        currFireRate = baseFireRate;

        timeToPerfectChargeEnd = timeToPerfectCharge + (0.016f * framesForPerfectCharge);

        currSniperLaserColor = defaultSniperLaserColor;

        totalRechargeTime = numberOfBullets * bulletRechargeTime;
        currentBullets = numberOfBullets;

        grenadesFireRate = 4.0f;
        #endregion

        #region DASH

        // Dash
        dashTimer = 0.0f;
        maxDashNumber = Math.Max(maxDashNumber, 1);
        currentDashes = maxDashNumber;
        dashSpeed = dashDistance / dashDuration;

        #endregion

        #region OTHERS

        //player instance
        instance = this;

        //Controller
        deathZone = 15000;

        currentState = STATE.IDLE;
        Animator.Play(gameObject, "Idle");
        UpdateAnimationSpd(1f);

        lockInputs = false;

        Debug.Log("Start!");
        mySpawnPos = new Vector3(gameObject.transform.globalPosition.x, gameObject.transform.globalPosition.y, gameObject.transform.globalPosition.z);
        runTime = Animator.GetAnimationDuration(gameObject, "Run") / 2;
        #endregion

        #region STATUS_SYSTEM

        InitEntity(ENTITY_TYPE.PLAYER);

        #endregion

    }

    public void Update()
    {
        // Placeholder for Start() function
        if (scriptStart == true)
        {
            LoadBuffs();
            hud = InternalCalls.FindObjectWithName("HUD");

            if (hud == null)
                Debug.Log("Core: HUD not found");

            pause = InternalCalls.FindObjectWithName("PauseMenu");

            if (pause == null)
                Debug.Log("Core: Pause menu not found");

            background = InternalCalls.FindObjectWithName("Background");

            if (pause == null)
                Debug.Log("Core: Background not found");

            GameObject grenadeCooldown = InternalCalls.FindObjectWithName("GrenadeCooldownIcon");
            if (grenadeCooldown != null)
                grenadeCooldownIcon = grenadeCooldown.GetComponent<Material>();


            GameObject sniper1Cooldown = InternalCalls.FindObjectWithName("SniperCooldown1");
            if (sniper1Cooldown != null)
                sniperBullet1 = sniper1Cooldown.GetComponent<Material>();

            GameObject sniper2Cooldown = InternalCalls.FindObjectWithName("SniperCooldown2");
            if (sniper2Cooldown != null)
                sniperBullet2 = sniper2Cooldown.GetComponent<Material>();

            if(secSound == null)
            {
                secSound = InternalCalls.FindObjectWithName("SecSound");

                if (secSound == null)
                    Debug.Log("Core: Sec Sound GO not found");
            }



            GameObject lockInputsScene = InternalCalls.FindObjectWithName("LockInputsBool");

            if (lockInputsScene != null)
                lockAttacks = true;

            //SkillDataTree -> LoadStaticBuffs();

            //Start();
            scriptStart = false;
        }

        #region UPDATE STUFF

        myDeltaTime = Time.deltaTime * speedMult;

        if (Input.GetKey(DEKeyCode.C) == KeyState.KEY_DOWN)
        {
            Audio.SetState("Game_State", "Run");
            if (MusicSourceLocate.instance != null)
            {
                Audio.SetSwitch(MusicSourceLocate.instance.gameObject, "Player_Action", "Combat");
                Audio.SetSwitch(MusicSourceLocate.instance.gameObject, "Player_Health", "Healthy");
            }
        }

        if (Input.GetKey(DEKeyCode.Z) == KeyState.KEY_DOWN)
        {
            this.AddStatus(STATUS_TYPE.SLOWED, STATUS_APPLY_TYPE.BIGGER_PERCENTAGE, 0.25f, 60f);
        }

        if (Input.GetKey(DEKeyCode.X) == KeyState.KEY_DOWN)
        {
            this.AddStatus(STATUS_TYPE.ACCELERATED, STATUS_APPLY_TYPE.BIGGER_PERCENTAGE, 0.75f, 60f);
        }

        if (Input.GetKey(DEKeyCode.D) == KeyState.KEY_DOWN)
        {
            this.ClearStatuses();
        }

        UpdateControllerInputs();

        UpdateStatuses();

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
            dashTimer -= myDeltaTime;

            if (dashTimer <= 0)
            {
                // Debug.Log(dashTimer.ToString());

                GameObject hit = null;
                if (RayCastObj != null)
                {
                    float hitDistance = 0;
                    hit = InternalCalls.RayCast(RayCastObj.transform.globalPosition, RayCastObj.transform.GetForward(), 100, ref hitDistance);

                }
                else
                    Debug.Log("Raycast obj == null");

                if (hit != null)
                {
                    //Debug.Log(hit.tag);

                    if (hit.CompareTag("Gap"))
                    {
                        dashTimer = 0.1f;
                    }
                    else
                        inputsList.Add(INPUT.IN_DASH_END);

                }
                else
                {
                    inputsList.Add(INPUT.IN_DASH_END);
                    Debug.Log("Problem with raycast");
                }
            }
        }

        if (dashCDTimer > 0)
        {
            dashCDTimer -= myDeltaTime;
            betweenDashesCDTimer -= myDeltaTime;

            if (dashCDTimer <= 0f)
                currentDashes = maxDashNumber;

            if (currentDashes > 0 && betweenDashesCDTimer <= 0f)
                dashAvailable = true;

        }

        if (shootingTimer > 0 || stopShootingTime <= 0f)
        {
            shootingTimer -= myDeltaTime;

            if (shootingTimer <= 0 && stopShootingTime <= 0f)
            {
                inputsList.Add(INPUT.IN_SHOOT);
                //Debug.Log("In shoot");
            }
        }

        if (gadgetShootTimer > 0)
        {
            gadgetShootTimer -= myDeltaTime;

            if (gadgetShootTimer <= 0)
                inputsList.Add(INPUT.IN_GADGET_SHOOT_END);
        }

        if (skill_damageReductionDashActive && skill_damageReductionDashTimer > 0)
        {
            skill_damageReductionDashTimer -= myDeltaTime;

            if (skill_damageReductionDashTimer <= 0)
                skill_damageReductionDashActive = false;
        }

        if (skill_groguIncreaseDamageActive && skill_groguIncreaseDamageTimer > 0)
        {
            skill_groguIncreaseDamageTimer -= myDeltaTime;

            if (skill_groguIncreaseDamageTimer <= 0)
                skill_groguIncreaseDamageActive = false;
        }

        grenadesFireRateTimer -= myDeltaTime;
        timeOfRoom += myDeltaTime;
        timeSinceLastDash += myDeltaTime;
    }


    //Controler inputs go here
    private void ProcessExternalInput()
    {

        bool isPrimaryOverHeat = false;
        if (hud != null)
        {
            isPrimaryOverHeat = hud.GetComponent<HUD>().IsPrimaryOverheated();

            if (isPrimaryOverHeat == true && primaryWeaponOverHeat == false)
                Audio.PlayAudio(secSound, "Play_Mando_Blaster_Overcharged");

            primaryWeaponOverHeat = isPrimaryOverHeat;
        }

        if (!lockInputs)
        {
            if ((Input.GetGamepadButton(DEControllerButton.X) == KeyState.KEY_DOWN || Input.GetGamepadButton(DEControllerButton.X) == KeyState.KEY_REPEAT) && isPrimaryOverHeat == false && lockAttacks == false)
            {
                stopShootingTime = 0f;
                inputsList.Add(INPUT.IN_SHOOTING);
            }
            else if ((Input.GetGamepadButton(DEControllerButton.X) == KeyState.KEY_UP || Input.GetGamepadButton(DEControllerButton.X) == KeyState.KEY_IDLE || isPrimaryOverHeat == true) && hasShot == true && lockAttacks == false)
            {
                if (CanStopShooting() == true || isPrimaryOverHeat == true || (this.currentState != STATE.SHOOT && this.currentState != STATE.SHOOTING))
                {
                    inputsList.Add(INPUT.IN_SHOOTING_END);
                    hasShot = false;
                }
                else if (CanStopShooting() == false)
                {
                    stopShootingTime += myDeltaTime;
                    Animator.Play(gameObject, "Shoot", 0.01f);
                    UpdateAnimationSpd(0.01f);
                }
            }


            if ((Input.GetGamepadButton(DEControllerButton.B) == KeyState.KEY_DOWN) && currentBullets > 0 && lockAttacks == false)
            {
                inputsList.Add(INPUT.IN_CHARGE_SEC_SHOOT);
            }
            else if (Input.GetGamepadButton(DEControllerButton.B) == KeyState.KEY_UP && lockAttacks == false)
            {
                inputsList.Add(INPUT.IN_CHARGE_SEC_SHOOT_END);
            }

            if (Input.GetRightTrigger() > 0 && rightTriggerPressed == false && dashAvailable == true && lockAttacks == false)
            {
                inputsList.Add(INPUT.IN_DASH);
                rightTriggerPressed = true;
            }
            else if (Input.GetRightTrigger() == 0 && rightTriggerPressed == true && lockAttacks == false)
                rightTriggerPressed = false;


            if ((Input.GetKey(DEKeyCode.LSHIFT) == KeyState.KEY_REPEAT && Input.GetKey(DEKeyCode.LALT) == KeyState.KEY_REPEAT &&
                Input.GetKey(DEKeyCode.D) == KeyState.KEY_DOWN && myDeltaTime != 0.0f) || Input.GetGamepadButton(DEControllerButton.BACK) == KeyState.KEY_DOWN)
            {
                InternalCalls.CreateUIPrefab("Library/Prefabs/1342862578.prefab", new Vector3(0, 0, 0), new Quaternion(0, 0, 0), new Vector3(1, 1, 1));
            }

            if (Input.GetKey(DEKeyCode.T) == KeyState.KEY_DOWN)
            {
                Debug.Log("Amount of bullets fired: " + numberOfShots.ToString());
                Debug.Log("Time to complete room: " + timeOfRoom.ToString() + " seconds");
                Debug.Log("Times fallen of the map: " + timesFellOfMap.ToString());
                Debug.Log("Total distance moved: " + distanceMoved.ToString() + " mandos");
            }

            if (Input.GetGamepadButton(DEControllerButton.Y) == KeyState.KEY_DOWN && grenadesFireRateTimer <= 0.0f && lockAttacks == false)
            {
                inputsList.Add(INPUT.IN_GADGET_SHOOT);
                grenadesFireRateTimer = grenadesFireRate;
            }


            if (IsJoystickMoving() == true)
                inputsList.Add(INPUT.IN_MOVE);

            else if (currentState == STATE.MOVE && IsJoystickMoving() == false)
                inputsList.Add(INPUT.IN_IDLE);
        }


        if (grenadesFireRateTimer > 0.0f && grenadeCooldownIcon != null)
        {
            grenadeCooldownIcon.SetFloatUniform("currentGrenadeCooldown", grenadesFireRate - grenadesFireRateTimer);
            grenadeCooldownIcon.SetFloatUniform("maxGrenadeCooldown", grenadesFireRate);
        }
        else if (grenade_reloading && grenadesFireRateTimer < 0.0f)
        {
            PlayParticles(PARTICLES.GRENADE);
            grenade_reloading = false;
            Audio.PlayAudio(secSound, "Play_Grenade_Cooldown_Finish");
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

                        case INPUT.IN_CHARGE_SEC_SHOOT:
                            currentState = STATE.CHARGING_SEC_SHOOT;
                            StartSecCharge();
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


                        case INPUT.IN_CHARGE_SEC_SHOOT:
                            currentState = STATE.CHARGING_SEC_SHOOT;
                            MoveEnd();
                            StartSecCharge();
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

                        case INPUT.IN_GADGET_SHOOT:
                            currentState = STATE.GADGET_SHOOT;
                            EndShooting();
                            StartGadgetShoot();
                            break;

                        case INPUT.IN_CHARGE_SEC_SHOOT:
                            currentState = STATE.CHARGING_SEC_SHOOT;
                            EndShooting();
                            StartSecCharge();
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

                case STATE.CHARGING_SEC_SHOOT:
                    switch (input)
                    {
                        case INPUT.IN_CHARGE_SEC_SHOOT_END:
                            currentState = STATE.SECONDARY_SHOOT;
                            StartSecondaryShoot();
                            EndShootCharge();
                            break;

                        case INPUT.IN_DASH:
                            currentState = STATE.DASH;
                            EndShootCharge();
                            StartDash();
                            break;

                        case INPUT.IN_DEAD:
                            break;
                    }
                    break;

                case STATE.SECONDARY_SHOOT:
                    switch (input)
                    {
                        case INPUT.IN_SEC_SHOOT_END:
                            currentState = STATE.IDLE;
                            StartIdle();
                            //EndSecondaryShoot();
                            break;

                        case INPUT.IN_DEAD:
                            break;
                    }
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
                ReducePrimaryWeaponHeat();
                UpdateSecondaryShotAmmo();
                UpdateIdle();
                break;
            case STATE.MOVE:
                UpdateMove();
                ReducePrimaryWeaponHeat();
                UpdateSecondaryShotAmmo();
                break;
            case STATE.DASH:
                UpdateDash();
                ReducePrimaryWeaponHeat(dashHeatReductionMult);
                UpdateSecondaryShotAmmo();
                break;
            case STATE.SHOOTING:
                UpdateShooting();
                UpdateSecondaryShotAmmo();
                break;
            case STATE.SHOOT:
                UpdateSecondaryShotAmmo();
                break;
            case STATE.CHARGING_SEC_SHOOT:
                UpdateSecondaryShootCharge();
                break;
            case STATE.SECONDARY_SHOOT:
                ReducePrimaryWeaponHeat();
                break;
            case STATE.GADGET_SHOOT:
                ReducePrimaryWeaponHeat();
                UpdateSecondaryShotAmmo();
                UpdateGadgetShoot();
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
        Animator.Play(gameObject, "Idle", speedMult);
        UpdateAnimationSpd(speedMult);
    }

    private void UpdateIdle()
    {
        UpdateAnimationSpd(speedMult);
    }
    #endregion

    #region NORMAL SHOOT

    private void StartShooting()
    {
        shootingTimer = GetCurrentFireRate();

        if (currFireRate != shootingTimer)
        {
            normalShootSpeed = shootAnimationTotalTime / currFireRate;
            currFireRate = shootingTimer;
            numberOfShots += 1;
        }
        Animator.Play(gameObject, "Shoot", normalShootSpeed * speedMult);
        UpdateAnimationSpd(normalShootSpeed * speedMult);

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

        UpdateAnimationSpd(normalShootSpeed * speedMult);
    }

    private void EndShooting()
    {
        if (myAimbot != null)
            myAimbot.isShooting = false;
    }

    private bool CanStopShooting()
    {
        //bool ret = false;
        //if (currFireRate <= baseFireRate * afterShootMult)
        //{
        //    ret = stopShootingTime > afterShootDelay;
        //}
        //else
        //{
        //    ret = stopShootingTime > afterShootDelay * (currFireRate / baseFireRate) * afterShootMult;
        //}

        return stopShootingTime > afterShootDelay;
    }

    private void StartShoot()
    {
        inputsList.Add(INPUT.IN_SHOOT_END);
        hasShot = true;

        if (shootPoint == null)
        {
            Debug.Log("Shootpoint reference is null!");
            return;
        }

        Audio.StopAudio(gameObject);
        Audio.PlayAudio(shootPoint, "Play_Blaster_Shoot_Mando");
        Input.PlayHaptic(.3f, 10);
        if (hud != null)
        {
            hud.GetComponent<HUD>().ShootSwapImage(true);
        }
        GameObject bullet = InternalCalls.CreatePrefab("Library/Prefabs/1606118587.prefab", shootPoint.transform.globalPosition, shootPoint.transform.globalRotation, shootPoint.transform.globalScale);
        if (bullet != null)
        {
            AddPrimaryHeat();
            //Debug.Log("Bullet Shot!");

            bullet.GetComponent<BH_Bullet>().damage = GetDamage();
        }
    }


    private void StartGadgetShoot()
    {
        Animator.Play(gameObject, "Shoot", gadgetShootSkill * speedMult);
        UpdateAnimationSpd(gadgetShootSkill * speedMult);
        gadgetShootTimer = gadgetFireRate;
        grenade_reloading = true;

        ReducePrimaryWeaponHeat(onGrenadeHeatReduction);
    }

    private void UpdateGadgetShoot()
    {
        UpdateAnimationSpd(gadgetShootSkill * speedMult);
    }

    private void EndGadgetShoot()
    {
        if (shootPoint == null)
        {
            Debug.Log("Shootpoint reference is null!");
            return;
        }

        Audio.StopAudio(gameObject);
        Audio.PlayAudio(shootPoint, "Play_Weapon_Shoot_Mando");

        Input.PlayHaptic(2f, 30);

        //Vector3 scale = new Vector3(0.2f, 0.2f, 0.2f);
        //Quaternion rotation = Quaternion.RotateAroundAxis(Vector3.up, 0.383972f);

        //TODO: Some of this is hardcoded, will change it once I have all the Mando's new numbers. Besis, Alex <3

        // bigGrenade grenadeComp = InternalCalls.CreatePrefab("Library/Prefabs/142833782.prefab", shootPoint.transform.globalPosition - 0.5f, shootPoint.transform.globalRotation * rotation, scale).GetComponent<bigGrenade>();

        //if(grenadeComp != null)
        //{
        //    Vector3 targetPos = grenadeComp.gameObject.transform.globalPosition + grenadeComp.gameObject.transform.GetForward() * 1.9f;
        //    grenadeComp.InitGrenade(targetPos, 0.204f, 10);
        //}

        //rotation = Quaternion.RotateAroundAxis(Vector3.up, -0.383972f);
        //grenadeComp = InternalCalls.CreatePrefab("Library/Prefabs/142833782.prefab", shootPoint.transform.globalPosition - 0.5f, shootPoint.transform.globalRotation * rotation, scale).GetComponent<bigGrenade>();

        //if (grenadeComp != null)
        //{
        //    Vector3 targetPos = grenadeComp.gameObject.transform.globalPosition + grenadeComp.gameObject.transform.GetForward() * 1.9f;
        //    grenadeComp.InitGrenade(targetPos, 0.204f, 10);
        //}

        Vector3 scale = new Vector3(0.2f, 0.2f, 0.2f);
        InternalCalls.CreatePrefab("Library/Prefabs/660835192.prefab", shootPoint.transform.globalPosition - 0.5f, shootPoint.transform.globalRotation, scale);

    }

    public void IncreaseNormalShootDamage(float percent)
    {
        bulletDamage += (bulletDamage * percent);
    }

    #endregion

    #region SPECIAL SHOOT
    private void StartSecCharge()
    {
        chargeTimer = 0f;
        changeColorSniperTimer = 0f;
        //Animation play :O
    }

    private void EndShootCharge()
    {
        chargeTimer = 0f;
        changeColorSniperTimer = 0f;
    }

    private void UpdateSecondaryShootCharge()
    {
        if (IsJoystickMoving() == true)
            RotatePlayer(0.25f);

        if (chargeTimer < timeToPerfectCharge)
        {
            currSniperLaserColor = defaultSniperLaserColor;

        }
        else if (chargeTimer > timeToPerfectCharge && chargeTimer < timeToPerfectChargeEnd)
        {
            currSniperLaserColor = new Vector3(1, 1, 0);
        }
        else if (chargeTimer > timeToPerfectChargeEnd)
        {
            changeColorSniperTimer += myDeltaTime;

            if (changeColorSniperTimer > 0f && changeColorSniperTimer < overheatTimeBeeping)
            {
                currSniperLaserColor = defaultSniperLaserColor;
            }
            else
            {
                currSniperLaserColor = new Vector3(1, 1, 1);

                if (changeColorSniperTimer > overheatTimeBeeping * 2)
                    changeColorSniperTimer = 0f;
            }

        }

        if (shootPoint != null && myAimbot != null)
        {
            float hitDistance = myAimbot.maxRange;
            GameObject hit = InternalCalls.RayCast(shootPoint.transform.globalPosition + (shootPoint.transform.GetForward() * 1.5f), shootPoint.transform.GetForward(), myAimbot.maxRange, ref hitDistance);
            if (hit != null)
            {
                Debug.Log(hitDistance.ToString());
            }

            hitDistance = Math.Min(hitDistance, Mathf.Lerp(0, myAimbot.maxRange, chargeTimer / timeToPerfectCharge));

            InternalCalls.DrawRay(shootPoint.transform.globalPosition, shootPoint.transform.globalPosition + (shootPoint.transform.GetForward() * hitDistance), currSniperLaserColor);
        }

        chargeTimer += myDeltaTime;
        //TODO: This needs to go away once the sniper animations are done
        Animator.Play(gameObject, "Shoot", 0.01f);
        UpdateAnimationSpd(0.01f);

        if (chargeTimer >= timeToAutomaticallyShootCharge)
        {
            inputsList.Add(INPUT.IN_CHARGE_SEC_SHOOT_END);
        }

    }

    private void StartSecondaryShoot()
    {
        bool perfectShot = false;

        Animator.Play(gameObject, "Shoot", normalShootSpeed * speedMult);
        UpdateAnimationSpd(normalShootSpeed * speedMult);

        if (chargeTimer > timeToPerfectCharge && chargeTimer < timeToPerfectChargeEnd)
        {
            perfectShot = true;
            Debug.Log("Frame Perfect Charge!");
            //TODO: Change color beam (yellow)
        }

        ShootChargeShot(perfectShot, chargeTimer);
    }

    private void ShootChargeShot(bool perfectShot, float chargeTimer)
    {

        inputsList.Add(INPUT.IN_SEC_SHOOT_END);
        --currentBullets;
        sniperRechargeTimer += bulletRechargeTime;

        if (shootPoint == null)
        {
            Debug.Log("Shootpoint reference is null!");
            return;
        }

        Audio.StopAudio(gameObject);
        Audio.PlayAudio(shootPoint, "Play_Sniper_Shoot_Mando");
        Input.PlayHaptic(.5f, 10);

        GameObject aimHelpTarget = myAimbot.SearchForNewObjRaw(15, myAimbot.maxRange);

        GameObject bullet = InternalCalls.CreatePrefab("Library/Prefabs/739906161.prefab", shootPoint.transform.globalPosition, shootPoint.transform.globalRotation, null);
        if (bullet != null)
        {
            //Debug.Log("Charged Bullet Shot!");
            ReducePrimaryWeaponHeat(onSniperHeatReduction);

            ChargedBullet chrBulletComp = bullet.GetComponent<ChargedBullet>();

            if (chrBulletComp != null)
            {
                float bulletDamage = chargedBulletDmg;

                if (perfectShot == true)
                {
                    bulletDamage *= perfectShotDmgMult;
                    bullet.transform.localScale *= perfectShotDmgMult;
                }
                else if (chargeTimer > timeToPerfectChargeEnd)
                {
                    bulletDamage = (chargedBulletDmg * overShotDmgMult);
                }
                else
                {
                    float dmgMult = Mathf.InvLerp(0.0f, timeToPerfectCharge, chargeTimer);

                    dmgMult = Math.Max(dmgMult, 0.15f);
                    bulletDamage = (chargedBulletDmg * dmgMult);

                    bulletDamage = Math.Max(bulletDamage, 1f);
                }
                //Debug.Log("Charge Bullet dmg: " + bulletDamage.ToString());

                //    if (Skill_Tree_Data.IsEnabled((int)Skill_Tree_Data.SkillTreesNames.MANDO, (int)Skill_Tree_Data.MandoSkillNames.AGGRESION_EXTRA_DAMAGE_LOW_HEALTH))
                //    {
                //        //If the skill is active, we override the damage amount
                //        bullet.GetComponent<ChargedBullet>().damage = GetExtraDamageWithSkill();
                //    }  

                chrBulletComp.InitChargedBullet(bulletDamage);

                if (aimHelpTarget != null)
                {
                    chrBulletComp.SetTarget(aimHelpTarget.transform.globalPosition);
                }

            }


        }
    }

    #endregion

    #region DASH
    private void StartDash()
    {
        Audio.StopAudio(gameObject);
        Audio.PlayAudio(gameObject, "Play_Dash");
        Animator.Play(gameObject, "Dash", speedMult);
        UpdateAnimationSpd(speedMult);

        dashTimer = dashDuration;
        dashStartYPos = gameObject.transform.localPosition.y;

        preDashRotation = this.gameObject.transform.localRotation;

        if (IsJoystickMoving() == true)
        {
            dashDirection = WorldDirFromGamepadInput().normalized;
            RotatePlayer();
        }
        else
        {
            dashDirection = this.gameObject.transform.GetForward();
        }

        PlayParticles(PARTICLES.JETPACK);
    }

    private void UpdateDash()
    {
        StopPlayer();
        //gameObject.AddForce(gameObject.transform.GetForward().normalized * dashSpeed);
        PlayParticles(PARTICLES.JETPACK);
        UpdateAnimationSpd(speedMult);

        gameObject.transform.localPosition = gameObject.transform.localPosition + dashDirection * dashSpeed * myDeltaTime;
        distanceMoved += dashSpeed * myDeltaTime;
    }

    private void EndDash()
    {
        dashCDTimer = dashCD;
        betweenDashesCDTimer = betweeMultiplenDashesCD;
        dashAvailable = false;
        --currentDashes;
        timeSinceLastDash = 0.01f;
        //Debug.Log(dashCDTimer.ToString());
        //Debug.Log(dashAvaliable.ToString());

        StopPlayer();
        gameObject.transform.localPosition.y = dashStartYPos;
        this.gameObject.transform.localRotation = preDashRotation;

        //if (Skill_Tree_Data.IsEnabled((int)Skill_Tree_Data.SkillTreesNames.MANDO, (int)Skill_Tree_Data.MandoSkillNames.UTILITY_DAMAGE_REDUCTION_DASH))
        //{
        //    skill_damageReductionDashActive = true;
        //    skill_damageReductionDashTimer = Skill_Tree_Data.GetMandoSkillTree().U4_seconds;
        //}

        PlayParticles(PARTICLES.JETPACK, true);
    }

    #endregion

    #region MOVE AND ROTATE PLAYER
    private void StartMove()
    {
        Animator.Play(gameObject, "Run", speedMult);
        UpdateAnimationSpd(speedMult);

        if (RoomSwitch.currentLevelIndicator == RoomSwitch.LEVELS.ONE)
        {
            Audio.PlayAudio(this.gameObject, "Play_Footsteps_Sand_Mando");
        }
        else if (RoomSwitch.currentLevelIndicator == RoomSwitch.LEVELS.TWO)
        {
            Audio.PlayAudio(this.gameObject, "Play_Footsteps_Snow_Mando");
        }
    }

    private void UpdateMove()
    {
        UpdateAnimationSpd(speedMult);

        footStepTimer += myDeltaTime;

        if (footStepTimer > 0.33f)
        {
            Audio.StopAudio(gameObject);

            if (RoomSwitch.currentLevelIndicator == RoomSwitch.LEVELS.ONE)
            {
                Audio.PlayAudio(this.gameObject, "Play_Footsteps_Sand_Mando");
            }
            else if (RoomSwitch.currentLevelIndicator == RoomSwitch.LEVELS.TWO)
            {
                Audio.PlayAudio(this.gameObject, "Play_Footsteps_Snow_Mando");
            }

            footStepTimer = 0f;
        }


        RotatePlayer();
        dustTime += myDeltaTime;
        if (dustTime >= runTime)
        {
            PlayParticles(PARTICLES.DUST);
            dustTime = 0;
        }

        //gameObject.SetVelocity(gameObject.transform.GetForward() * movementSpeed);
        gameObject.transform.localPosition = gameObject.transform.localPosition + gameObject.transform.GetForward().normalized * movementSpeed * MovspeedMult * myDeltaTime;
        distanceMoved += movementSpeed * myDeltaTime;
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

    private void RotatePlayer(float slerpValue = 1f)
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

        gameObject.transform.localRotation = Quaternion.Slerp(gameObject.transform.localRotation, Quaternion.RotateAroundAxis(Vector3.up, (float)-angle), slerpValue);

    }
    #endregion

    #region UTILITIES
    //returns vector direction in world space from the gamepad input, or Vector3.zero if gamepad joystick is idle
    public Vector3 WorldDirFromGamepadInput()
    {
        //Calculate player rotation
        Vector3 aX = new Vector3(gamepadInput.x, 0, (gamepadInput.y + 1) * -1);
        Vector3 aY = new Vector3(0, 0, 1);
        aX = Vector3.Normalize(aX);
        if (aX == Vector3.zero)
            return Vector3.zero;

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

        Quaternion rotation = Quaternion.RotateAroundAxis(Vector3.up, (float)-angle);

        return (rotation * aY);

    }

    private void UpdateAnimationSpd(float newSpd)
    {
        if (currAnimationPlaySpd != newSpd)
            Animator.SetSpeed(gameObject, newSpd);
    }
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
                pause.GetComponent<Pause>().HideShop();
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

    private float GetCurrentFireRate()
    {
        float ret = baseFireRate;

        float currentHeat = hud.GetComponent<HUD>().GetPrimaryHeat();

        if (currentHeat <= 0f)
            currentHeat = 0.01f;

        ret = (currentHeat * (baseFireRate * fireRateMultCap)) / hud.GetComponent<HUD>().GetPrimaryMaxHeat();


        //Debug.Log("Firerate: " + ret.ToString());

        ret = Math.Min(ret, baseFireRate * fireRateMultCap * 0.45f);
        ret = Math.Max(ret, baseFireRate * 0.75f);

        if (Core.instance.HasStatus(STATUS_TYPE.COMBO_FIRE_RATE))
        {
            Debug.Log("FireRate: " + FireRateMult.ToString());
        }
        if (Core.instance.HasStatus(STATUS_TYPE.COMBO_DAMAGE))
        {
            Debug.Log("Damage: " + RawDamageMult.ToString());
        }

        return ret * FireRateMult;
    }

    private void AddPrimaryHeat()
    {
        if (hud != null)
            hud.GetComponent<HUD>().AddPrimaryHeatShot();
    }

    private void ReducePrimaryWeaponHeat(float stateMult = 0f)
    {
        float newValue = heatReductionSpeed * myDeltaTime;
        float refreshMult = 1.0f + stateMult;

        if (hud != null)
            hud.GetComponent<HUD>().ReducePrimaryHeat(newValue * refreshMult);

    }

    private void UpdateSecondaryShotAmmo()
    {
        if (sniperRechargeTimer > 0f)
        {
            sniperRechargeTimer -= myDeltaTime;

            //TODO: This works with just 2 bullets
            if (sniperBullet2 != null)
            {
                float numberToUpdate = sniperRechargeTimer < bulletRechargeTime ? bulletRechargeTime : 1 / Math.Abs(bulletRechargeTime - sniperRechargeTimer);

                sniperBullet2.SetFloatUniform("currentBulletCooldown", numberToUpdate);
                sniperBullet2.SetFloatUniform("maxBulletCooldown", bulletRechargeTime);
            }
            if (sniperBullet1 != null)
            {

                float numberToUpdate = sniperRechargeTimer <= 0 ? bulletRechargeTime : bulletRechargeTime - sniperRechargeTimer;

                if (sniperRechargeTimer > bulletRechargeTime)
                    numberToUpdate = 0f;

                sniperBullet1.SetFloatUniform("currentBulletCooldown", numberToUpdate);
                sniperBullet1.SetFloatUniform("maxBulletCooldown", bulletRechargeTime);
            }

            if (sniperRechargeTimer <= ((numberOfBullets - currentBullets - 1) * bulletRechargeTime))
            {
                currentBullets++;
                PlayParticles(PARTICLES.SNIPER);

                Audio.PlayAudio(secSound, "Play_Sniper_Cooldown_Finish");
            }
        }

    }

    public bool IsDashing()
    {
        return currentState == STATE.DASH;
    }

    public void ReduceComboOnHit(int hitDamage, float comboSubstractionMult = 1f)
    {
        if (hud != null)
        {
            HUD myHud = hud.GetComponent<HUD>();

            if (myHud != null)
                myHud.SubstractToCombo(hitDamage * comboSubstractionMult);

        }
    }

    #endregion


    public void OnCollisionEnter(GameObject collidedGameObject)
    {
        //Debug.Log("CS: Collided object: " + gameObject.tag + ", Collider: " + collidedGameObject.tag);
        //Debug.Log("Collided by tag: " + collidedGameObject.tag);

        if (currentState != STATE.DASH)
        {
            if (collidedGameObject.CompareTag("StormTrooperBullet"))
            {
                //InternalCalls.Destroy(gameObject);
                PlayParticles(PARTICLES.IMPACT);
                BH_Bullet bulletScript = collidedGameObject.GetComponent<BH_Bullet>();
                Audio.PlayAudio(gameObject, "Play_Mando_Hit");

                if (bulletScript != null)
                {
                    int damageFromBullet = 0;

                    if (skill_damageReductionDashActive)
                        damageFromBullet = (int)(bulletScript.damage * (1.0f/* - Skill_Tree_Data.GetMandoSkillTree().U4_damageReduction*/));
                    else
                        damageFromBullet = (int)bulletScript.damage;

                    PlayerHealth healthScript = gameObject.GetComponent<PlayerHealth>();

                    if (healthScript != null)
                        healthScript.TakeDamage(damageFromBullet);

                    damageTaken += damageFromBullet;
                }
            }
            if (collidedGameObject.CompareTag("Bantha"))
            {
                //InternalCalls.Destroy(gameObject);
                Audio.PlayAudio(gameObject, "Play_Mando_Hit");

                Enemy enemy = collidedGameObject.GetComponent<Enemy>();

                if (enemy != null)
                {
                    float damage = enemy.damage;

                    if (damage != 0)
                    {
                        int damageFromEnemy = 0;
                        if (skill_damageReductionDashActive)
                            damageFromEnemy = (int)(damage * (1.0f /*- Skill_Tree_Data.GetMandoSkillTree().U4_damageReduction*/));
                        else
                            damageFromEnemy = (int)damage;

                        PlayerHealth playerHealth = gameObject.GetComponent<PlayerHealth>();
                        if (playerHealth != null)
                            playerHealth.TakeDamage(damageFromEnemy);

                        damageTaken += damageFromEnemy;
                    }
                }
            }
            else if (collidedGameObject.CompareTag("ExplosiveBarrel"))
            {
                SphereCollider sphereColl = collidedGameObject.GetComponent<SphereCollider>();

                if (sphereColl != null)
                {
                    if (sphereColl.active)
                        gameObject.GetComponent<PlayerHealth>().TakeDamage((int)(collidedGameObject.GetComponent<BH_DestructBox>().explosion_damage * 0.5f));
                }
            }
        }
    }


    public void OnTriggerEnter(GameObject triggeredGameObject)
    {
        if (triggeredGameObject.CompareTag("Coin"))
        {
            PlayerResources.AddRunCoins(1);

            if (hud != null)
            {
                HUD hudComponent = hud.GetComponent<HUD>();

                if (hudComponent != null)
                    hudComponent.UpdateCurrency(PlayerResources.GetRunCoins());
            }

            InternalCalls.Destroy(triggeredGameObject);
        }
    }


    public void RespawnOnFall()
    {

        gameObject.transform.localPosition = mySpawnPos;
        PlayerHealth myHealth = gameObject.GetComponent<PlayerHealth>();
        if (myHealth != null)
        {
            int finalHealthSubstract = 0;
            if (HasStatus(STATUS_TYPE.FALL))
            {
                finalHealthSubstract = (int)(PlayerHealth.currMaxHealth * 0.25f / 2);
            }
            else
                finalHealthSubstract = (int)(PlayerHealth.currMaxHealth * 0.25f);

            if (PlayerHealth.currHealth - finalHealthSubstract <= 0)
            {
                finalHealthSubstract = PlayerHealth.currHealth - 1;
            }

            myHealth.TakeDamage(finalHealthSubstract); //TODO whats the right amount we have to substract?
            damageTaken += finalHealthSubstract;
            timesFellOfMap += 1;

        }
    }

    private void PlayParticles(PARTICLES particletype, bool stopParticle = false)
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
                    else if (particle != null && stopParticle == true)
                        particle.Stop();
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
                    else if (particle != null && stopParticle == true)
                        particle.Stop();
                    //else
                    //    Debug.Log("Jetpack particle not found");
                }
                else
                    Debug.Log("Component Particles not found");
                break;

            case PARTICLES.JETPACK:
                if (myParticles != null)
                {
                    particle = myParticles.jetpack;
                    if (particle != null && !particle.playing)
                        particle.Play();
                    else if (particle != null && stopParticle == true)
                        particle.Stop();
                    else
                        Debug.Log("Jetpack Rush particle not found!");

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
                    else if (particle != null && stopParticle == true)
                        particle.Stop();
                    else
                        Debug.Log("Jetpack particle not found");
                }
                else
                    Debug.Log("Component Particles not found");
                break;
            case PARTICLES.GRENADE:
                if (myParticles != null)
                {
                    particle = myParticles.grenade;
                    if (particle != null)
                        particle.Play();
                    else if (particle != null && stopParticle == true)
                        particle.Stop();
                    else
                        Debug.Log("Grenade particle not found");
                }
                else
                    Debug.Log("Component Particles not found");
                break;
            case PARTICLES.SNIPER:
                if (myParticles != null)
                {
                    particle = myParticles.sniper;
                    if (particle != null)
                        particle.Play();
                    else if (particle != null && stopParticle == true)
                        particle.Stop();
                    else
                        Debug.Log("Sniper particle not found");
                }
                else
                    Debug.Log("Component Particles not found");
                break;
        }
    }

    public void SetSkill(int skillTree, int SkillNumber)
    {
        if (skillTree == (int)Skill_Tree_Data.SkillTreesNames.MANDO)
        {
            if (SkillNumber == (int)Skill_Tree_Data.MandoSkillNames.UTILITY_INCREASE_DAMAGE_WHEN_GROGU)
            {
                skill_groguIncreaseDamageActive = true;
                skill_groguIncreaseDamageTimer = Skill_Tree_Data.GetMandoSkillTree().U3_duration;
            }
        }
    }

    private float GetDamage()
    {
        //We apply modifications to the damage based on the skill actives in the talent tree
        float damageWithSkills = bulletDamage * BlasterDamageMult * DamagePerHpMult * DamagePerHeatMult * RawDamageMult;

        //if (skill_groguIncreaseDamageActive)
        //{
        //    damageWithSkills *= (1.0f /*+ Skill_Tree_Data.GetMandoSkillTree().U3_increasedDamagePercentage*/);
        //}

        //if (Skill_Tree_Data.IsEnabled((int)Skill_Tree_Data.SkillTreesNames.MANDO, (int)Skill_Tree_Data.MandoSkillNames.AGGRESION_EXTRA_DAMAGE_LOW_HEALTH))
        //{
        //    float HPMissing = 1.0f - ((float)PlayerHealth.currHealth / (float)PlayerHealth.currMaxHealth);

        //    float local_damageAmount = Skill_Tree_Data.GetMandoSkillTree().A7_extraDamageAmount;
        //    float local_HPStep = Skill_Tree_Data.GetMandoSkillTree().A7_extraDamageHPStep;
        //    damageWithSkills += (damageWithSkills * local_damageAmount * (HPMissing / local_HPStep));
        //}
        return damageWithSkills;
    }

    public void LockInputs(bool locked)
    {
        if (locked)
        {
            lockInputs = true;
            inputsList.Add(INPUT.IN_IDLE);
        }
        else
        {
            lockInputs = false;
        }
    }

    public void OnApplicationQuit()
    {
        bulletDamage = bulletDamageDefault;
    }

    public STATE GetSate()
    {
        return currentState;
    }

    public void SaveBuffs()
    {
        Debug.Log("SAVE STATUSES");
        copyBuffs(ref PlayerStatuses);
    }

    public void LoadBuffs()
    {
        Debug.Log("LOAD STATUSES");
        Debug.Log(PlayerStatuses.Count.ToString());
        var mapKeys = PlayerStatuses.Keys;
        foreach (STATUS_TYPE statusType in mapKeys)
        {
            if (PlayerStatuses.ContainsKey(statusType) == false)
                continue;
            Debug.Log(PlayerStatuses[statusType].statusType.ToString());
            Debug.Log(PlayerStatuses[statusType].applyType.ToString());
            Debug.Log(PlayerStatuses[statusType].severity.ToString());
            Debug.Log(PlayerStatuses[statusType].remainingTime.ToString());
            Debug.Log(PlayerStatuses[statusType].isPermanent.ToString());

            AddStatus(PlayerStatuses[statusType].statusType, PlayerStatuses[statusType].applyType, PlayerStatuses[statusType].severity, 1, PlayerStatuses[statusType].isPermanent);

        }
    }
}
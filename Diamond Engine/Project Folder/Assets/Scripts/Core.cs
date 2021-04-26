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
    public float afterShootDelay = 2.5f;
    public float afterShootMult = 1.5f;
    private double angle = 0.0f;
    private float runTime = 0.0f;
    private float dustTime = 0.0f;

    //Skill
    private float skill_damageReductionDashTimer = 0.0f;
    private bool skill_damageReductionDashActive = false;

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
    private float shootingTimer = 0.0f;
    private float gadgetShootTimer = 0.0f;

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
    public Vector3 defaultSniperLaserColor = new Vector3(1, 0, 0);
    private Vector3 currSniperLaserColor = new Vector3(1, 0, 0);

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

    //Telemetry
    int numberOfShots = 0;
    int damageTaken = 0;
    int timesFellOfMap = 0;
    float distanceMoved = 0f;
    float timeOfRoom = 0f;


    public void Awake()
    {
        #region VARIABLES WITH DEPENDENCIES

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

        lockInputs = false;

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


            GameObject lockInputsScene = InternalCalls.FindObjectWithName("LockInputsBool");

            if (lockInputsScene != null)
                lockAttacks = true;

            //Start();
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
                    Debug.Log(hit.tag);

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
            dashCDTimer -= Time.deltaTime;
            betweenDashesCDTimer -= Time.deltaTime;

            if (dashCDTimer <= 0f)
                currentDashes = maxDashNumber;

            if (currentDashes > 0 && betweenDashesCDTimer <= 0f)
                dashAvailable = true;

        }

        if (shootingTimer > 0 || stopShootingTime <= 0f)
        {
            shootingTimer -= Time.deltaTime;

            if (shootingTimer <= 0 && stopShootingTime <= 0f)
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

        if (Skill_Tree_Data.IsEnabled((int)Skill_Tree_Data.SkillTreesNames.MANDO, (int)Skill_Tree_Data.MandoSkillNames.UTILITY_DAMAGE_REDUCTION_DASH)
            && skill_damageReductionDashTimer > 0)
        {
            skill_damageReductionDashTimer -= Time.deltaTime;

            if (skill_damageReductionDashTimer <= 0)
                skill_damageReductionDashActive = false;
        }


        grenadesFireRateTimer -= Time.deltaTime;
        timeOfRoom += Time.deltaTime;
        timeSinceLastDash += Time.deltaTime;
    }


    //Controler inputs go here
    private void ProcessExternalInput()
    {

        bool isPrimaryOverHeat = false;
        if (hud != null)
        {
            isPrimaryOverHeat = hud.GetComponent<HUD>().IsPrimaryOverheated();
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
                    stopShootingTime += Time.deltaTime;
                    Animator.Play(gameObject, "Shoot", 0.01f);
                }
            }


            if ((Input.GetGamepadButton(DEControllerButton.B) == KeyState.KEY_DOWN || Input.GetGamepadButton(DEControllerButton.B) == KeyState.KEY_REPEAT) && currentBullets > 0 && lockAttacks == false)
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
                Input.GetKey(DEKeyCode.D) == KeyState.KEY_DOWN && Time.deltaTime != 0.0f) || Input.GetGamepadButton(DEControllerButton.BACK) == KeyState.KEY_DOWN)
            {
                InternalCalls.CreateUIPrefab("Library/Prefabs/1871660106.prefab", new Vector3(0, 0, 0), new Quaternion(0, 0, 0), new Vector3(1, 1, 1));
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
                break;
            case STATE.MOVE:
                UpdateMove();
                ReducePrimaryWeaponHeat();
                UpdateSecondaryShotAmmo();
                break;
            case STATE.DASH:
                UpdateDash();
                ReducePrimaryWeaponHeat(0.75f);
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
        shootingTimer = GetCurrentFireRate();

        if (currFireRate != shootingTimer)
        {
            normalShootSpeed = shootAnimationTotalTime / currFireRate;
            currFireRate = shootingTimer;
            numberOfShots += 1;
        }
        Animator.Play(gameObject, "Shoot", normalShootSpeed);


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
            Debug.Log("Bullet Shot!");

            bullet.GetComponent<BH_Bullet>().damage = bulletDamage;

            if (Skill_Tree_Data.IsEnabled((int)Skill_Tree_Data.SkillTreesNames.MANDO, (int)Skill_Tree_Data.MandoSkillNames.AGGRESION_EXTRA_DAMAGE_LOW_HEALTH))
            {
                //If the skill is active, we override the damage amount
                bullet.GetComponent<BH_Bullet>().damage = GetExtraDamageWithSkill();
            }
        }
    }


    private void StartGadgetShoot()
    {
        Animator.Play(gameObject, "Shoot", gadgetShootSkill);
        gadgetShootTimer = gadgetFireRate;

        ReducePrimaryWeaponHeat(5);
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
        //Animation play :O
    }

    private void EndShootCharge()
    {
        chargeTimer = 0f;
    }

    private void UpdateSecondaryShootCharge()
    {
        if (IsJoystickMoving() == true)
            RotatePlayer(0.25f);

        if (chargeTimer < timeToPerfectCharge)
        {
            //Change color beam (red)
            currSniperLaserColor = defaultSniperLaserColor;

        }
        else if (chargeTimer > timeToPerfectCharge && chargeTimer < timeToPerfectChargeEnd)
        {
            //Change color beam (yellow)
            currSniperLaserColor = new Vector3(1, 1, 0);
        }
        else if (chargeTimer > timeToPerfectChargeEnd)
        {
            //TODO: Change color beam (beeping red-white)
            currSniperLaserColor = new Vector3(1, 1, 1);
        }


        if (shootPoint != null && myAimbot != null)
        {
            float hitDistance = myAimbot.maxRange;
            GameObject hit = InternalCalls.RayCast(shootPoint.transform.globalPosition + (shootPoint.transform.GetForward() * 2), shootPoint.transform.GetForward(), myAimbot.maxRange, ref hitDistance);
            if (hit != null)
            {
                Debug.Log(hitDistance.ToString());
            }

            hitDistance = Math.Min(hitDistance, Mathf.Lerp(0, myAimbot.maxRange, chargeTimer / timeToPerfectCharge));

            InternalCalls.DrawRay(shootPoint.transform.globalPosition, shootPoint.transform.globalPosition + (shootPoint.transform.GetForward() * hitDistance), currSniperLaserColor);
        }

        chargeTimer += Time.deltaTime;
        Animator.Play(gameObject, "Shoot", 0.01f);

        if (chargeTimer >= timeToAutomaticallyShootCharge)
        {
            inputsList.Add(INPUT.IN_CHARGE_SEC_SHOOT_END);
        }

    }

    private void StartSecondaryShoot()
    {
        bool perfectShot = false;

        Animator.Play(gameObject, "Shoot", normalShootSpeed);

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
            Debug.Log("Charged Bullet Shot!");
            ReducePrimaryWeaponHeat(10);

            ChargedBullet chrBulletComp = bullet.GetComponent<ChargedBullet>();

            if (chrBulletComp != null)
            {
                float bulletDamage = chargedBulletDmg;

                if (perfectShot == true)
                {
                    bulletDamage *= 1.4f;
                    bullet.transform.localScale *= 1.2f;
                }
                else if (chargeTimer > timeToPerfectChargeEnd)
                {
                    bulletDamage = (chargedBulletDmg * 1.05f);
                }
                else
                {
                    float dmgMult = Mathf.InvLerp(0.0f, timeToPerfectCharge, chargeTimer);

                    dmgMult = Math.Max(dmgMult, 0.15f);
                    bulletDamage = (chargedBulletDmg * dmgMult);

                    bulletDamage = Math.Max(bulletDamage, 1f);
                }
                Debug.Log("Charge Bullet dmg: " + bulletDamage.ToString());

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
        Animator.Play(gameObject, "Dash");

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

        gameObject.transform.localPosition = gameObject.transform.localPosition + dashDirection * dashSpeed * Time.deltaTime;
        distanceMoved += dashSpeed * Time.deltaTime;
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

        if (Skill_Tree_Data.IsEnabled((int)Skill_Tree_Data.SkillTreesNames.MANDO, (int)Skill_Tree_Data.MandoSkillNames.UTILITY_DAMAGE_REDUCTION_DASH))
        {
            skill_damageReductionDashActive = true;
            skill_damageReductionDashTimer = Skill_Tree_Data.GetMandoSkillTree().U4_seconds;
        }
    }

    #endregion

    #region MOVE AND ROTATE PLAYER
    private void StartMove()
    {
        Animator.Play(gameObject, "Run");

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
        RotatePlayer();
        dustTime += Time.deltaTime;
        if (dustTime >= runTime)
        {

            PlayParticles(PARTICLES.DUST);
            dustTime = 0;
        }


        //gameObject.SetVelocity(gameObject.transform.GetForward() * movementSpeed);
        gameObject.transform.localPosition = gameObject.transform.localPosition + gameObject.transform.GetForward().normalized * movementSpeed * Time.deltaTime;
        distanceMoved += movementSpeed * Time.deltaTime;
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

    private float GetCurrentFireRate()
    {
        float ret = baseFireRate;

        float currentHeat = hud.GetComponent<HUD>().GetPrimaryHeat();

        if (currentHeat <= 0f)
            currentHeat = 0.01f;

        ret = (currentHeat * (baseFireRate * fireRateMultCap)) / hud.GetComponent<HUD>().GetPrimaryMaxHeat();


        Debug.Log("Firerate: " + ret.ToString());

        ret = Math.Min(ret, baseFireRate * fireRateMultCap * 0.45f);
        ret = Math.Max(ret, baseFireRate * 0.75f);

        return ret;
    }

    private void AddPrimaryHeat()
    {
        if (hud != null)
            hud.GetComponent<HUD>().AddPrimaryHeatShot();
    }

    private void ReducePrimaryWeaponHeat(float stateMult = 0f)
    {
        float newValue = 22.5f * Time.deltaTime;
        float refreshMult = 1.0f + stateMult;

        if (hud != null)
            hud.GetComponent<HUD>().ReducePrimaryHeat(newValue * refreshMult);

    }

    private void UpdateSecondaryShotAmmo()
    {
        if (sniperRechargeTimer > 0f)
        {
            sniperRechargeTimer -= Time.deltaTime;

            //TODO: This works with just 2 bullets
            if (sniperBullet2 != null)
            {
                float numberToUpdate = sniperRechargeTimer < bulletRechargeTime ? bulletRechargeTime : 1 / Math.Abs(bulletRechargeTime - sniperRechargeTimer);

                sniperBullet2.SetFloatUniform("currentBulletCooldown", numberToUpdate);
                sniperBullet2.SetFloatUniform("maxBulletCooldown", bulletRechargeTime);
            }
            if (sniperBullet1 != null)
            {
                // 2.5 - 2.5


                float numberToUpdate = sniperRechargeTimer <= 0 ? bulletRechargeTime : bulletRechargeTime - sniperRechargeTimer;

                if (sniperRechargeTimer > bulletRechargeTime)
                    numberToUpdate = 0f;

                sniperBullet1.SetFloatUniform("currentBulletCooldown", numberToUpdate);
                sniperBullet1.SetFloatUniform("maxBulletCooldown", bulletRechargeTime);
            }

            if (sniperRechargeTimer <= ((numberOfBullets - currentBullets - 1) * bulletRechargeTime))
            {
                currentBullets++;
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
                        damageFromBullet = (int)(bulletScript.damage * (1.0f - Skill_Tree_Data.GetMandoSkillTree().U4_damageReduction));
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
                            damageFromEnemy = (int)(damage * (1.0f - Skill_Tree_Data.GetMandoSkillTree().U4_damageReduction));
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
                        gameObject.GetComponent<PlayerHealth>().TakeDamage(collidedGameObject.GetComponent<BH_DestructBox>().explosion_damage / 2);
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
            int finalHealthSubstract = (int)(PlayerHealth.currMaxHealth * 0.25f);

            if (PlayerHealth.currHealth - finalHealthSubstract <= 0)
            {
                finalHealthSubstract = PlayerHealth.currHealth - 1;
            }

            myHealth.TakeDamage(finalHealthSubstract); //TODO whats the right amount we have to substract?
            damageTaken += finalHealthSubstract;
            timesFellOfMap += 1;

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

    private static float GetExtraDamageWithSkill()
    {
        float HPMissing = 1.0f - ((float)PlayerHealth.currHealth / (float)PlayerHealth.currMaxHealth);

        float local_damageAmount = Skill_Tree_Data.GetMandoSkillTree().A7_extraDamageAmount;
        float local_HPStep = Skill_Tree_Data.GetMandoSkillTree().A7_extraDamageHPStep;
        return bulletDamage + (bulletDamage * local_damageAmount * (HPMissing / local_HPStep));
    }

    public void OnApplicationQuit()
    {
        bulletDamage = bulletDamageDefault;
    }
}
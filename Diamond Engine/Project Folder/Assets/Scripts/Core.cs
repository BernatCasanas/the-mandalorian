using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Collections.Generic;

using DiamondEngine;

public class Core : DiamondComponent
{
    public static Core instance;

    enum State
    {
        None,
        Idle,
        Run,
        Dash,
        Shoot,
        SecShoot
    }

    public GameObject shootPoint = null;

    private bool scriptStart = true;

    //State
    private State _currentState = State.None;
    private State _previousState = State.None;

    // Movement
    public float rotationSpeed = 2.0f;
    public float movementSpeed = 35.0f;
    public float mouseSens = 1.0f;
    private double angle = 0.0f;

    // Dash
    private float timeSinceLastDash = 0.0f;
    private bool dashAvaliable = true;
    private float dashingCounter = 0.0f;
    public float dashCD = 0.33f;
    public float dashDuration = 0.25f;
    public float dashDistance = 1.0f;
    private float dashSpeed = 0.0f;
    private float dashTimer = 0.0f;
    private float timeBetweenDashes = .5f;

    // Shooting
    public float fireRate = 0.2f;
    public float baseFireRate = 0.0f;
    private float currFireRate = 0.0f;
    private float timeSinceLastNormalShoot = 0.0f;
    public float secondaryRate = 0.2f;
    private float currSecondaryRate = 0.0f;
    private float timeSinceLastSpecialShoot = 0.0f;

    private bool shooting = false;
    private bool fireButtonPressed = false;
    public float fireRateAfterDashRecoverRatio = 2.0f;
    private float fireRateRecoverCap = 0.0f;
    private float secondaryRateRecoverCap = 0.0f;

    public float fireRateMultCap = 0.0f;
    private bool rightTriggerPressed = false;
    private bool leftTriggerPressed = false;
    private float rightTriggerTimer = 0.0f;
    private float leftTriggerTimer = 0.0f;
    private int deathZone = 15000;
    public float normalShootSpeed = 0.0f;

    //Grenades
    public float grenadesFireRate = 0.0f;
    private float grenadesTimer = 0.0f;

    //Animations
    private float shootAnimationTotalTime = 0.0f;
    public string currentStateString = "";

    //Controller Variables
    int verticalInput = 0;
    int horizontalInput = 0;
    Vector3 gamepadInput;

    public List<smallGrenade> smallGrenades = new List<smallGrenade>();
    public List<bigGrenade> BigGrenades = new List<bigGrenade>();

    public void Update()
    {

        #region START
        // Placeholder for Start() function
        if (scriptStart == true)
        {
            Audio.SetState("Player_State", "Alive");
            Audio.SetSwitch(gameObject, "Player_Action", "Combat");

            #region VARIABLES WITH DEPENDENCIES

            // INIT VARIABLES WITH DEPENDENCIES //

            //Shoot
            fireRate = 0.15f;
            secondaryRate = 0.2f;
            baseFireRate = 0.15f;

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

            //Shooting Triggers
            leftTriggerTimer = 0f;
            rightTriggerTimer = 0f;
            leftTriggerPressed = false;
            rightTriggerPressed = false;

            //Shooting bools
            shooting = false;

            //Shooting timers
            timeSinceLastNormalShoot = 1f;  //Can shoot in first instance
            timeSinceLastSpecialShoot = 1f; //Can shoot in first instance

            //Shooting Rates
            currFireRate = fireRate;
            normalShootSpeed = shootAnimationTotalTime / fireRate;
            //fireRateAfterDashRecoverRatio = 2f;
            fireRateRecoverCap = 3.0f / baseFireRate;
            //fireRateMultCap = 2.5f;
            currSecondaryRate = secondaryRate;
            secondaryRateRecoverCap = 3.0f / secondaryRate;
            #endregion

            #region DASH

            // Dash
            dashTimer = 0f;
            dashSpeed = dashDistance / dashDuration;
            dashAvaliable = true;
            timeBetweenDashes = dashCD;

            #endregion

            #region OTHERS

            //player instance
            instance = this;

            //Controller
            deathZone = 15000;

            scriptStart = false;

            _previousState = State.None;
            _currentState = State.Idle;

            Debug.Log("Start!");

            #endregion
        }

        #endregion

        #region UPDATE STUFF

        if(Input.GetKey(DEKeyCode.SPACE) == KeyState.KEY_DOWN)
        {
            Audio.SetState("Player_State", "Alive");

        }
        if(Input.GetKey(DEKeyCode.K) == KeyState.KEY_DOWN)
        {
            Audio.SetState("Player_State", "Dead");
        }   
        
        if(Input.GetKey(DEKeyCode.L) == KeyState.KEY_DOWN)
        {
            Audio.SetSwitch(gameObject, "Player_Action", "Exploring");
        }


        UpdateTimers();

        UpdateControllerInputs();

        #endregion

        #region STATE MACHINE


        switch (_currentState)
        {
            case State.None:
                ChangeState(State.Idle);
                break;

            case State.Idle:
                if (IsJoystickMoving())
                {
                    ChangeState(State.Run);
                }
                ShootInput();
                SecondaryShootInput();
                InputDash();

                break;

            case State.Run:

                if (IsJoystickMoving())
                {
                    RotatePlayer(gamepadInput);
                    MovePlayer();
                    if (_currentState != State.Run)
                    {
                        Audio.StopAudio(gameObject);
                    }
                }
                else
                {
                    Audio.StopAudio(this.gameObject);
                    ChangeState(State.Idle);
                }

                ShootInput();
                SecondaryShootInput();
                InputDash();


                break;

            case State.Dash:
                HandleDash();
                break;
            case State.Shoot:
                if (IsJoystickMoving())
                {
                    RotatePlayer(gamepadInput);
                }

                ShootInput();
                HandleShoot();
                SecondaryShootInput();
                InputDash();
                break;

            case State.SecShoot:
                Debug.Log("Grenade Fire!");
                if (IsJoystickMoving())
                {
                    RotatePlayer(gamepadInput);
                }

                grenadesTimer += Time.deltaTime;

                if (grenadesTimer > grenadesFireRate)
                    HandleSecondaryShoot();
                break;
        }

        currentStateString = _currentState.ToString();
        #endregion
    }


    #region NORMAL SHOOT

    private void ShootInput()
    {
        KeyState fireButton = Input.GetGamepadButton(DEControllerButton.A);

        if ((fireButton == KeyState.KEY_REPEAT || fireButton == KeyState.KEY_DOWN) && fireButtonPressed == false)
        {
            fireRate = GetCurrentFireRate();
            shooting = true;
            fireButtonPressed = true;
            ChangeState(State.Shoot);
        }
        else if (fireButton == KeyState.KEY_UP || fireButton == KeyState.KEY_IDLE)
        {
            fireButtonPressed = false;
        }
    }

    private void HandleShoot()
    {
        if (fireButtonPressed || shooting)
        {
            if (timeSinceLastNormalShoot > fireRate)
            {
                Audio.StopAudio(gameObject);
                Audio.PlayAudio(shootPoint, "Play_Weapon_Shoot_Mando");
                timeSinceLastNormalShoot = 0f;
                Input.PlayHaptic(.3f, 10);
                fireRate = GetCurrentFireRate();
                float newShootSpeed = shootAnimationTotalTime / fireRate;

                if (newShootSpeed != normalShootSpeed)
                {
                    normalShootSpeed = newShootSpeed;
                }

                Animator.Play(gameObject, "Shoot", normalShootSpeed);
                InternalCalls.CreatePrefab("Library/Prefabs/346087333.prefab", shootPoint.transform.globalPosition, shootPoint.transform.globalRotation, shootPoint.transform.globalScale);
                shooting = false;
            }
        }
        else if (!shooting) // Time to cancel animation
        {
            if (timeSinceLastNormalShoot > fireRate * 0.5f)
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
    }

    #endregion

    #region SPECIAL SHOOT
    private void SecondaryShootInput()
    {
        if (BigGrenades.Count == 0)
            Debug.Log(BigGrenades.Count.ToString());

        if (Input.GetGamepadButton(DEControllerButton.Y) == KeyState.KEY_DOWN && smallGrenades.Count == 0 && BigGrenades.Count == 0)
        {
            ChangeState(State.SecShoot);
        }

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

    #endregion

    #region DASH
    private void InputDash()
    {
        if (Input.GetRightTrigger() > 0 && rightTriggerPressed == false && dashAvaliable == true)
        {
            rightTriggerPressed = true;
            rightTriggerTimer = 0f;

            Audio.StopAudio(this.gameObject);
            Audio.PlayAudio(this.gameObject, "Play_Dash");

            dashAvaliable = false;
            dashingCounter = 0.0f;
            dashTimer = 0f;
            ChangeState(State.Dash);
        }
        else if (Input.GetRightTrigger() == 0)
        {
            rightTriggerPressed = false;
            rightTriggerTimer += Time.deltaTime;
        }

        if (dashTimer < timeBetweenDashes)
        {
            dashTimer += Time.deltaTime;
        }
        else
        {
            dashAvaliable = true;
        }
    }

    private void HandleDash()
    {
        if (dashingCounter < dashDuration)
        {
            dashingCounter += Time.deltaTime;
            gameObject.transform.localPosition += gameObject.transform.GetForward().normalized * dashSpeed * Time.deltaTime;

        }
        else
        {

            timeSinceLastDash = 0.0f;
            dashingCounter = 0.0f;

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
    }

    #endregion

    #region MOVE AND ROTATE PLAYER
    private void MovePlayer()
    {
        gameObject.transform.localPosition += gameObject.transform.GetForward() * movementSpeed * Time.deltaTime;
    }

    private void RotatePlayer(Vector3 gamepadInput)
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

    #endregion

    #region UTILITIES

    private void ChangeState(State newState)
    {
        if (_currentState == newState) return;

        _previousState = _currentState;
        _currentState = newState;

        HandleAnimation();
    }

    private void HandleAnimation()
    {
        if (_previousState == _currentState) return;

        switch (_currentState)
        {
            case State.None:
                Animator.Play(gameObject, "Idle");
                break;
            case State.Idle:
                Animator.Play(gameObject, "Idle");
                break;
            case State.Run:
                Animator.Play(gameObject, "Run");
                Audio.PlayAudio(this.gameObject, "Play_Footsteps_Mando");
                break;
            case State.Dash:
                Animator.Play(gameObject, "Dash");
                break;
            case State.Shoot:
                Animator.Play(gameObject, "Shoot", normalShootSpeed);
                break;
            case State.SecShoot:
                Animator.Play(gameObject, "Shoot", normalShootSpeed);
                break;
        }
    }
    private void UpdateTimers()
    {
        //Timers
        timeSinceLastNormalShoot += Time.deltaTime;
        timeSinceLastSpecialShoot += Time.deltaTime;
        timeSinceLastDash += Time.deltaTime;
    }

    private void UpdateControllerInputs()
    {
        //Check if user is moving joystick
        verticalInput = Input.GetLeftAxisX();
        horizontalInput = Input.GetLeftAxisY();
        gamepadInput = new Vector3(horizontalInput, -verticalInput, 0f);
    }

    private float GetCurrentFireRate()
    {
        float ret = baseFireRate;

        ret = (float)(Math.Log(timeSinceLastDash * fireRateAfterDashRecoverRatio) - Math.Log(0.01)) / fireRateRecoverCap;

        ret = Math.Min(ret, baseFireRate * fireRateMultCap);
        //Debug.Log("New fire rate: " + ret.ToString());

        return ret;
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

        if (collidedGameObject.CompareTag("Bullet"))
        {
            //InternalCalls.Destroy(gameObject);
            float damage = collidedGameObject.GetComponent<BH_Bullet>().damage;
            gameObject.GetComponent<PlayerHealth>().TakeDamage((int)damage);
        }
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
}

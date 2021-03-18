using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using DiamondEngine;

public class Core : DiamondComponent
{
    public static Core instance;
    enum State
    {
        Idle,
        Run,
        Dash,
        Shoot,
        SecShoot
    }

    public GameObject shootPoint = null;

    private bool scriptStart = true;

    //State
    private State _state;

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

    //Animations
    private float shootAnimationTotalTime = 0.0f;
    public string currentAnimation = "";

    //Controller Variables
    int verticalInput = 0;
    int horizontalInput = 0;
    Vector3 gamepadInput;

    public void Update()
    {

        #region START
        // Placeholder for Start() function
        if (scriptStart == true)
        {

            #region VARIABLES WITH DEPENDENCIES

            // INIT VARIABLES WITH DEPENDENCIES //

            //Shoot
            fireRate = 0.15f;
            secondaryRate = 0.2f;
            baseFireRate = 0.15f;

            //Animation
            shootAnimationTotalTime = 0.288f;

            //Dash - if scene doesnt have its values
            dashDuration = 0.2f;
            dashDistance = 4.5f;

            // END INIT VARIABLES WITH DEPENDENCIES //

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
            fireRateAfterDashRecoverRatio = 2f;
            fireRateRecoverCap = 3.0f / baseFireRate;
            fireRateMultCap = 2.5f;
            currSecondaryRate = secondaryRate;
            secondaryRateRecoverCap = 3.0f / secondaryRate;
            #endregion

            #region DASH

            // Dash
            dashTimer = 0f;
            dashSpeed = dashDistance / dashDuration;
            dashAvaliable = true;
            timeBetweenDashes = .5f;

            #endregion

            #region OTHERS

            //player instance
            instance = this;

            //Controller
            deathZone = 15000;

            scriptStart = false;

            _state = State.Idle;

            Debug.Log("Start!");

            #endregion
        }

        #endregion

        #region UPDATE STUFF
        UpdateTimers();

        UpdateControllerInputs();

        #endregion

        #region STATE MACHINE

        switch (_state)
        {
            case State.Idle:
                if (currentAnimation != "Idle") Animator.Play(gameObject, "Idle");

                if (IsJoystickMoving())
                {
                    _state = State.Run;
                    Debug.Log("Change to Run state");
                }
                ShootInput();
                SecondaryShootInput();
                InputDash();

                break;
            case State.Run:
                if (currentAnimation != "Run")
                {
                    Animator.Play(gameObject, "Run");
                    Audio.PlayAudio(this.gameObject, "Play_Footsteps_Mando");
                };

                if (IsJoystickMoving())
                {
                    RotatePlayer(gamepadInput);
                    MovePlayer();
                    if (_state != State.Run)
                    {
                        Audio.StopAudio(gameObject);
                    }
                }
                else
                {
                    Audio.StopAudio(this.gameObject);
                    _state = State.Idle;
                }

                ShootInput();
                SecondaryShootInput();
                InputDash();

                break;
            case State.Dash:
                if (currentAnimation != "Dash") Animator.Play(gameObject, "Dash");
                HandleDash();
                break;
            case State.Shoot:
                if (currentAnimation != "Shoot") Animator.Play(gameObject, "Shoot", normalShootSpeed);
                if (IsJoystickMoving())
                {
                    RotatePlayer(gamepadInput);
                }

                ShootInput();
                HandleShoot();
                InputDash();
                break;

            case State.SecShoot:
                if (currentAnimation != "Shoot") Animator.Play(gameObject, "Shoot", normalShootSpeed);
                Debug.Log("fire");
                if (IsJoystickMoving())
                {
                    RotatePlayer(gamepadInput);
                }
                SecondaryShootInput();
                HandleSecondaryShoot();
                InputDash();
                break;
        }

        currentAnimation = Animator.GetCurrentAnimation(gameObject);
        #endregion
    }


    #region NORMAL SHOOT

    private void ShootInput()
    {
        if (Input.GetRightTrigger() > 0 && !rightTriggerPressed)
        {
            fireRate = GetCurrentFireRate();
            rightTriggerPressed = true;
            shooting = true;
            rightTriggerTimer = 0f;
            _state = State.Shoot;
        }
        else if (Input.GetRightTrigger() == 0)
        {
            rightTriggerPressed = false;
            rightTriggerTimer += Time.deltaTime;
        }
    }

    private void HandleShoot()
    {
        if (rightTriggerPressed || shooting)
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
                InternalCalls.CreateBullet(shootPoint.transform.globalPosition, shootPoint.transform.globalRotation, shootPoint.transform.globalScale);
                shooting = false;
            }
        }
        else if (!shooting) // Time to cancel animation
        {
            if (timeSinceLastNormalShoot > fireRate * 0.5f)
            {
                if (IsJoystickMoving())
                {
                    _state = State.Run;
                }
                else
                {
                    _state = State.Idle;

                }
            }
        }
    }

    #endregion

    #region SPECIAL SHOOT
    private void SecondaryShootInput()
    {
        if (Input.GetLeftTrigger() > 0 && !leftTriggerPressed)
        {
            fireRate = GetCurrentSecondaryRate();
            shooting = true;
            leftTriggerPressed = true;
            leftTriggerTimer = 0f;
            _state = State.SecShoot;
        }
        else if (Input.GetLeftTrigger() == 0)
        {
            leftTriggerPressed = false;
            leftTriggerTimer += Time.deltaTime;
        }
    }

    private void HandleSecondaryShoot()
    {
        if (leftTriggerPressed || shooting)
        {
            currSecondaryRate = GetCurrentSecondaryRate();

            if (timeSinceLastSpecialShoot > currSecondaryRate)
            {
                Audio.StopAudio(gameObject);
                Vector3 scale = new Vector3(0.2f, 0.2f, 0.2f);
                Audio.PlayAudio(shootPoint, "Play_Weapon_Shoot_Mando");
                InternalCalls.CreatePrefab("Library/Prefabs/142833782.prefab", shootPoint.transform.globalPosition, shootPoint.transform.globalRotation, scale);
                Input.PlayHaptic(1f, 30);
                timeSinceLastSpecialShoot = 0.0f;
                shooting = false;
            }
        }
        else if (!shooting) // Time to cancel animation
        {
            if (timeSinceLastSpecialShoot > currSecondaryRate * 0.5f)
            {
                if (IsJoystickMoving())
                {
                    _state = State.Run;
                }
                else
                {
                    _state = State.Idle;

                }
            }
        }


    }

    #endregion

    #region DASH
    private void InputDash()
    {
        if (Input.GetGamepadButton(DEControllerButton.A) == KeyState.KEY_DOWN && dashAvaliable == true)
        {
            Audio.StopAudio(this.gameObject);
            Audio.PlayAudio(this.gameObject, "Play_Dash");
            dashAvaliable = false;
            dashingCounter = 0.0f;
            dashTimer = 0f;
            _state = State.Dash;
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

            if (rightTriggerPressed)
            {
                _state = State.Shoot;

            }
            else if (leftTriggerPressed)
            {
                _state = State.SecShoot;
            }
            else
            {

                if (IsJoystickMoving())
                {
                    _state = State.Run;
                }
                else
                {
                    _state = State.Idle;

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
        Debug.Log("New fire rate: " + ret.ToString());

        return ret;
    }

    private float GetCurrentSecondaryRate()
    {
        float ret = secondaryRate;

        ret = (float)(Math.Log(timeSinceLastDash * fireRateAfterDashRecoverRatio) - Math.Log(0.01)) / secondaryRateRecoverCap;

        ret = Math.Min(ret, secondaryRate * fireRateMultCap);
        Debug.Log("New fire rate: " + ret.ToString());

        return ret;

    }

    private bool IsJoystickMoving()
    {
        return gamepadInput.magnitude > deathZone;
    }

    #endregion
}

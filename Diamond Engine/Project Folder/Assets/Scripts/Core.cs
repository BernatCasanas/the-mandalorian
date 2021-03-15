using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using DiamondEngine;


public class Core : DiamondComponent
{
    enum State
    {
        Idle,
        Run,
        Dash,
        Shoot
    }


    public static Action newAction;
    public GameObject reference = null;
    public GameObject shootPoint = null;

    private bool scriptStart = true;

    //State
    private State _prevState;
    private State _state;

    // Movement
    public float rotationSpeed = 2.0f;
    public float movementSpeed = 35.0f;
    public float mouseSens = 1.0f;
    private double angle = 0.0f;
    private bool walking = false;

    // Dash
    private bool dashing = false;
    private float timeSinceLastDash = 0.0f;
    public bool dashAvaliable = true;
    private float dashingCounter = 0.0f;
    public float dashCD = 0.33f;
    private float dashTimer;
    private float timeBetweenDashes = .5f;

    //private float dashCDCounter = 0.0f;
    public float dashDuration = 0.25f;
    public float dashDistance = 1.0f;
    private float dashSpeed = 0.0f;

    // Shooting
    public float fireRate;
    public float baseFireRate;
    private float currFireRate = 0.0f;
    public float timeSinceLastBullet = 0.0f;
    private bool shooting = false;
    public float fireRateAfterDashRecoverRatio = 2.0f;
    private float fireRateRecoverCap = 0.0f;
    private float boostTime;
    public bool triggerPressed;
    private float triggerTimer;
    private bool acceptShoot;
    int deathZone = 15000;
    private float recoverFireRateTime;
    private float fireRateTimer;
    public string currentAnimation;
    // Animation
    //public string idle_animation = "Idle";
    //public string run_animation = "Run";
    //public string shoot_animation = "Shoot";


    //private Vector3 faceDirection;



    //NEW VARIABLES
    int verticalInput = 0;
    int horizontalInput = 0;
    Vector3 gamepadInput;
    bool canRun = false;
    bool isIdle;
    public float normalShootSpeed;
    private float shootAnimationTotalTime;

    public void Update(/*int x*/)
    {
        if (this.reference == null)
            return;

        // Placeholder for Start() function
        if (scriptStart == true)
        {
            triggerPressed = false;
            triggerTimer = 0f;
            fireRate = 0.15f;
            baseFireRate = 0.15f;
            fireRateTimer = 0.0f;
            recoverFireRateTime = 0.5f;
            shootAnimationTotalTime = 0.2f;
            normalShootSpeed = shootAnimationTotalTime / fireRate;
            fireRateAfterDashRecoverRatio = 2f;
            dashTimer = 0f;
            _state = State.Idle;
            newAction += DebugSomeLog;
            dashSpeed = dashDistance / dashDuration;
            currFireRate = fireRate;
            scriptStart = false;
            dashAvaliable = true;
            canRun = true;
            timeBetweenDashes = .5f;
            acceptShoot = false;
            shooting = false;
            deathZone = 15000;
            boostTime = 1.0f;
            fireRateRecoverCap = 3.0f / baseFireRate;
            timeSinceLastBullet = 1f; //Can shoot in first instance
            Debug.Log("Start!");
        }

        //--------------NEW CODE------------------//

        timeSinceLastBullet += Time.deltaTime;
        timeSinceLastDash += Time.deltaTime;

        //Check if user is moving joystick
        verticalInput = Input.GetLeftAxisX();
        horizontalInput = Input.GetLeftAxisY();

        gamepadInput = new Vector3(horizontalInput, -verticalInput, 0f);

        switch (_state)
        {
            case State.Idle:
                if (currentAnimation != "Idle") Animator.Play(reference, "Idle");

                if (IsJoystickMoving())
                {
                    _state = State.Run;
                    Debug.Log("Change to Run state");
                }
                ShootInput();
                InputDash();

                break;
            case State.Run:
                if (!canRun) return;
                if (currentAnimation != "Run")
                {
                    Animator.Play(reference, "Run");
                    Audio.PlayAudio(this.reference, "Play_Footstep");
                };

                if (IsJoystickMoving())
                {
                    RotatePlayer(gamepadInput);
                    MovePlayer();
                    if (_state != State.Run)
                    {
                        Audio.StopAudio(reference);
                    }
                }
                else
                {
                    Audio.StopAudio(this.reference);
                    _state = State.Idle;
                }

                ShootInput();
                InputDash();

                break;
            case State.Dash:
                if (currentAnimation != "Dash") Animator.Play(reference, "Dash");
                HandleDash();
                break;
            case State.Shoot:
                if (currentAnimation != "Shoot") Animator.Play(reference, "Shoot", normalShootSpeed);
                if (IsJoystickMoving())
                {
                    RotatePlayer(gamepadInput);
                }

                ShootInput();
                HandleShoot();
                InputDash();
                break;
        }

        //Debug.Log(_state.ToString());
        currentAnimation = Animator.GetCurrentAnimation(reference);


        if (Input.GetLeftTrigger() > 0)
        {
            newAction?.Invoke();
        }


        {
            ////--------------OLD CODE-----------------//
            //faceDirection = Vector3.zero;
            //Vector3 joyRot = new Vector3(Input.GetLeftAxisY(), -Input.GetLeftAxisX(), 0);
            //int joyStickSensibility = 15000;

            //if (joyRot.magnitude > joyStickSensibility && dashing == false)
            //{
            //    RotatePlayer(joyRot);

            //    if (Input.GetMouseX() != 0 && reference != null)
            //    {
            //        reference.localRotation = Quaternion.RotateAroundAxis(Vector3.up, -Input.GetMouseX() * mouseSens * Time.deltaTime) * reference.localRotation;
            //    }

            //    faceDirection = reference.GetForward();
            //}

            //if (dashAvaliable == false && dashing == false)
            //{
            //    dashCDCounter += Time.deltaTime;
            //    if (dashCDCounter > dashCD)
            //    {
            //        dashCDCounter = 0.0f;
            //        dashAvaliable = true;
            //    }

            //}


            //timeSinceLastBullet += Time.deltaTime; //Moved here to keep shoot cd counting while dashing

            //if (dashing == true)
            //{
            //    Dash();
            //}
            //else
            //{
            //    if (shooting == false && timeSinceLastBullet > 0.3f)
            //        reference.localPosition += faceDirection.normalized * movementSpeed * Time.deltaTime;

            //    if (faceDirection != Vector3.zero && shooting == false)
            //    {
            //        if (walking == false)
            //        {
            //            Audio.PlayAudio(this.reference, "Play_Footstep");
            //            Animator.Play(reference, "Run");
            //        }
            //        walking = true;
            //    }
            //    else
            //    {
            //        if (walking == true)
            //        {
            //            Audio.StopAudio(this.reference);
            //            //Animator.Play(reference, idle_animation);
            //            Animator.Play(reference, "Idle");
            //        }
            //        walking = false;
            //    }


            //    timeSinceLastDash += Time.deltaTime;


            //    //Shooting
            //    if ((Input.GetMouseClick(MouseButton.LEFT) == KeyState.KEY_DOWN || Input.GetRightTrigger() > 0) && shooting == false)
            //    {
            //        shooting = true;
            //        currFireRate = fireRate;
            //    }
            //    else if ((Input.GetMouseClick(MouseButton.LEFT) == KeyState.KEY_UP || Input.GetRightTrigger() == 0) && shooting == true)
            //    {
            //        shooting = false;
            //        Animator.Play(reference, "Idle");
            //    }

            //    if (shooting == true)
            //    {
            //        Shoot();
            //    }

            //}
        }

    }

    private void InputDash()
    {
        if (Input.GetGamepadButton(DEControllerButton.A) == KeyState.KEY_DOWN && dashAvaliable == true)
        {
            Debug.Log("DAsh");
            Audio.StopAudio(this.reference);
            Audio.PlayAudio(this.reference, "Play_Dash");
            dashing = true;
            _state = State.Dash;
            dashAvaliable = false;
            dashingCounter = 0.0f;
            dashTimer = 0f;
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

    private void ShootInput()
    {
        if (Input.GetRightTrigger() > 0 && !triggerPressed)
        {
            triggerPressed = true;
            shooting = true;
            fireRate = GetCurrentFireRate();
            _state = State.Shoot;
            Debug.Log(triggerTimer.ToString());
            triggerTimer = 0f;
            //Debug.Log("Trigger pressed");
        }
        else if (Input.GetRightTrigger() == 0)
        {
            triggerPressed = false;
            triggerTimer += Time.deltaTime;
            //Debug.Log("Trigger released");
        }


    }
    private void HandleDash()
    {

        Dash();

    }
    private void HandleShoot()
    {
        if (triggerPressed || shooting)
        {
            if (timeSinceLastBullet > fireRate)
            {
                Audio.StopAudio(reference);
                Audio.PlayAudio(shootPoint, "Play_Weapon_Shoot");
                InternalCalls.CreateBullet(shootPoint.globalPosition, shootPoint.globalRotation, shootPoint.globalScale);
                timeSinceLastBullet = 0f;

                fireRate = GetCurrentFireRate();
                float newShootSpeed = shootAnimationTotalTime / fireRate;
                Debug.Log("New shoot speed : " + newShootSpeed.ToString());

                if (newShootSpeed != normalShootSpeed)
                {
                    normalShootSpeed = newShootSpeed;
                    Animator.Play(reference, "Shoot", normalShootSpeed);
                }

                shooting = false;
            }
        }
        else if (!shooting) // Time to cancel animation
        {
            if (timeSinceLastBullet > fireRate * 0.5f)
            {
                //Debug.Log(timeSinceLastBullet.ToString());
                Debug.Log("Shoot Finished");
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

    private void DebugSomeLog()
    {
        Debug.Log("Im called from an action");
    }

    private void MovePlayer()
    {
        reference.localPosition += reference.GetForward() * movementSpeed * Time.deltaTime;
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

        reference.localRotation = Quaternion.RotateAroundAxis(Vector3.up, (float)-angle);
    }

    //private void Shoot()
    //{
    //    currFireRate = GetCurrentFireRate();

    //    if (timeSinceLastBullet < currFireRate)
    //    {
    //        return;
    //    }

    //    Audio.PlayAudio(shootPoint, "Play_Weapon_Shoot");
    //    InternalCalls.CreateBullet(shootPoint.globalPosition, shootPoint.globalRotation, shootPoint.globalScale);
    //    timeSinceLastBullet = 0.0f;
    //    //Animator.Play(reference, shoot_animation);
    //    Animator.Play(reference, "Shoot");

    //}

    private void Dash()
    {
        if (dashingCounter < dashDuration)
        {
            dashingCounter += Time.deltaTime;
            reference.localPosition += reference.GetForward().normalized * dashSpeed * Time.deltaTime;

        }
        else
        {
            Debug.Log("Finished Dashing!");
            timeSinceLastDash = 0.0f;

            if (triggerPressed)
            {
                _state = State.Shoot;

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

    private float GetCurrentFireRate()
    {
        float ret = baseFireRate;

        ret = (float)(Math.Log(timeSinceLastDash * fireRateAfterDashRecoverRatio) - Math.Log(0.01)) / fireRateRecoverCap;

        ret = Math.Min(ret, baseFireRate * 2.5f);
        Debug.Log("New fire rate: " + ret.ToString());

        return ret;
    }

    private void SetFireRate(float rate)
    {
        fireRate = rate;
        fireRateTimer = 0.0f;
    }

    private bool IsJoystickMoving()
    {
        return gamepadInput.magnitude > deathZone;
    }
}

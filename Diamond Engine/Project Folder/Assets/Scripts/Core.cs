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

    public GameObject reference = null;
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
    public bool dashAvaliable = true;
    private float dashingCounter = 0.0f;
    public float dashCD = 0.33f;
    private float dashTimer;
    private float timeBetweenDashes = .5f;

    public float dashDuration = 0.25f;
    public float dashDistance = 1.0f;
    private float dashSpeed = 0.0f;

    // Shooting
    public float fireRate;
    public float baseFireRate;
    public float timeSinceLastBullet = 0.0f;
    private bool shooting = false;
    public float fireRateAfterDashRecoverRatio = 2.0f;
    private float fireRateRecoverCap = 0.0f;
    private bool rightTriggerPressed;
    private float rightTriggerTimer;
    private int deathZone;
    public float normalShootSpeed;

    //Animations
    private float shootAnimationTotalTime;
    public string currentAnimation;

    //Controller Variables
    int verticalInput = 0;
    int horizontalInput = 0;
    Vector3 gamepadInput;

    public void Update(/*int x*/)
    {
        if (this.reference == null)
            return;

        // Placeholder for Start() function
        if (scriptStart == true)
        {
            rightTriggerPressed = false;
            rightTriggerTimer = 0f;
            fireRate = 0.15f;
            baseFireRate = 0.15f;
            shootAnimationTotalTime = 0.2f;
            normalShootSpeed = shootAnimationTotalTime / fireRate;
            fireRateAfterDashRecoverRatio = 2f;
            dashTimer = 0f;
            dashSpeed = dashDistance / dashDuration;
            scriptStart = false;
            dashAvaliable = true;
            timeBetweenDashes = .5f;
            shooting = false;
            deathZone = 15000;
            fireRateRecoverCap = 3.0f / baseFireRate;
            timeSinceLastBullet = 1f; //Can shoot in first instance
            _state = State.Idle;
            Debug.Log("Start!");
        }

        //Timers
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

        currentAnimation = Animator.GetCurrentAnimation(reference);

    }

    private void InputDash()
    {
        if (Input.GetGamepadButton(DEControllerButton.A) == KeyState.KEY_DOWN && dashAvaliable == true)
        {
            Audio.StopAudio(this.reference);
            Audio.PlayAudio(this.reference, "Play_Dash");
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
    private void HandleDash()
    {
        if (dashingCounter < dashDuration)
        {
            dashingCounter += Time.deltaTime;
            reference.localPosition += reference.GetForward().normalized * dashSpeed * Time.deltaTime;

        }
        else
        {
            timeSinceLastDash = 0.0f;

            if (rightTriggerPressed)
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
    private void HandleShoot()
    {
        if (rightTriggerPressed || shooting)
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


    private float GetCurrentFireRate()
    {
        float ret = baseFireRate;

        ret = (float)(Math.Log(timeSinceLastDash * fireRateAfterDashRecoverRatio) - Math.Log(0.01)) / fireRateRecoverCap;

        ret = Math.Min(ret, baseFireRate * 2.5f);
        Debug.Log("New fire rate: " + ret.ToString());

        return ret;
    }

    private bool IsJoystickMoving()
    {
        return gamepadInput.magnitude > deathZone;
    }
}

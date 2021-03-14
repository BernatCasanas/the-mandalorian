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
    //private bool dashAvaliable = true;
    private float dashingCounter = 0.0f;
    public float dashCD = 0.33f;
    //private float dashCDCounter = 0.0f;
    public float dashDuration = 0.25f;
    public float dashDistance = 1.0f;
    private float dashSpeed = 0.0f;

    // Shooting
    public const float fireRate = 0.2f;
    private float currFireRate = 0.0f;
    public float timeSinceLastBullet = 0.0f;
    private bool shooting = false;
    public float fireRateAfterDashRecoverRatio = 2.0f;
    private float fireRateRecoverCap = 0.0f;

    int deathZone = 15000;

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

    public void Update(/*int x*/)
    {
        if (this.reference == null)
            return;

        // Placeholder for Start() function
        if (scriptStart == true)
        {
            _state = State.Idle;
            newAction += DebugSomeLog;
            dashSpeed = dashDistance / dashDuration;
            currFireRate = fireRate;
            scriptStart = false;
            canRun = true;
            shooting = false;
            deathZone = 15000;
            fireRateRecoverCap = 3.0f / fireRate;
            timeSinceLastBullet = 1f; //Can shoot in first instance
            Debug.Log("Start!");
        }

        //--------------NEW CODE------------------//

        timeSinceLastBullet += Time.deltaTime;

        //Check if user is moving joystick
        verticalInput = Input.GetLeftAxisX();
        horizontalInput = Input.GetLeftAxisY();

        gamepadInput = new Vector3(horizontalInput, -verticalInput, 0f);





        switch (_state)
        {
            case State.Idle:
                if(currentAnimation != "Idle") Animator.Play(reference, "Idle");

                if (gamepadInput.magnitude > deathZone)
                {
                    _state = State.Run;
                    Debug.Log("Change to Run state");
                }
                CanShoot();
                break;
            case State.Run:
                if (!canRun) return;
                if (currentAnimation != "Run") Animator.Play(reference, "Run");
                
                CanShoot();
                if (gamepadInput.magnitude > deathZone)
                {
                    RotatePlayer(gamepadInput);
                    MovePlayer();
                }
                else
                {
                    _state = State.Idle;
                }
                break;
            case State.Dash:
                if (currentAnimation != "Dash") Animator.Play(reference, "Dash");
                HandleDash();
                break;
            case State.Shoot:
                if (currentAnimation != "Shoot") Animator.Play(reference, "Shoot");
                Debug.Log("State shoot");
                HandleShoot();
                break;
        }

        Debug.Log(_state.ToString());
        currentAnimation = Animator.GetCurrentAnimation(reference);


        if(Input.GetLeftTrigger() > 0)
        {
            newAction?.Invoke();
        }




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

        if ((Input.GetKey(DEKeyCode.SPACE) == KeyState.KEY_DOWN || Input.GetGamepadButton(DEControllerButton.A) == KeyState.KEY_DOWN)/* && dashAvaliable == true*/)
        {
            Audio.StopAudio(this.reference);
            Audio.PlayAudio(this.reference, "Play_Dash");
            dashing = true;
            _state = State.Dash;
            //dashAvaliable = false;
            dashingCounter = 0.0f;
            //Animator.Play(reference, "Dash");
        }

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

    private void CanShoot()
    {
        if(Input.GetRightTrigger() > 0 && !shooting && timeSinceLastBullet > GetCurrentFireRate())
        {
            currFireRate = fireRate;
            timeSinceLastBullet = 0f;
            InternalCalls.CreateBullet(shootPoint.globalPosition, shootPoint.globalRotation, shootPoint.globalScale);
            _state = State.Shoot;
        }
    }
    private void HandleDash()
    {
        if (dashing)
        {
            Dash();
        }
    }
    private void HandleShoot()
    {
        if(timeSinceLastBullet > 0.2f) {
            _state = State.Idle;
        }
    }

    private void DebugSomeLog() {
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
            dashing = false;
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

    private float GetCurrentFireRate()
    {
        float ret = fireRate;

        ret = (float)(Math.Log(timeSinceLastDash * fireRateAfterDashRecoverRatio) - Math.Log(0.01)) / fireRateRecoverCap;

        ret = Math.Min(ret, fireRate * 2.5f);

        return ret;
    }

    private bool IsJoystickMoving()
    {
        return gamepadInput.magnitude > deathZone;
    }
}

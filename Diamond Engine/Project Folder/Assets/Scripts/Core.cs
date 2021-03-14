using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using DiamondEngine;

public class Core : DiamondComponent
{
    public GameObject shootPoint = null;

    private bool scriptStart = true;

    // Movement
    public float rotationSpeed = 2.0f;
    public float movementSpeed = 35.0f;
    public float mouseSens = 1.0f;
    private double angle = 0.0f;
    private bool walking = false;

    // Dash
    private bool dashing = false;
    private float timeSinceLastDash = 0.0f;
    private bool dashAvaliable = true;
    private float dashingCounter = 0.0f;
    public float dashCD = 0.33f;
    private float dashCDCounter = 0.0f;
    public float dashDuration = 0.25f;
    public float dashDistance = 1.0f;
    private float dashSpeed = 0.0f;

    // Shooting
    public const float fireRate = 0.2f;
    private float currFireRate = 0.0f;
    private float timeSinceLastBullet = 0.0f;
    private bool shooting = false;
    public float fireRateAfterDashRecoverRatio = 2.0f;
    private float fireRateRecoverCap = 0.0f;


    // Animation
    //public string idle_animation = "Idle";
    //public string run_animation = "Run";
    //public string shoot_animation = "Shoot";

    public void Update(/*int x*/)
    {
        // Placeholder for Start() function
        if (scriptStart == true)
        {
            dashSpeed = dashDistance / dashDuration;
            currFireRate = fireRate;
            scriptStart = false;
            fireRateRecoverCap = 3.0f / fireRate;
            Debug.Log("Start!");
        }

        Vector3 move = Vector3.zero;
        Vector3 joyRot = new Vector3(Input.GetLeftAxisY(), -Input.GetLeftAxisX(), 0);
        int joyStickSensibility = 15000;

        if (joyRot.magnitude > joyStickSensibility && dashing == false)
        {
            //Calculate player rotation
            Vector3 aX = new Vector3(joyRot.x, 0, -joyRot.y - 1);
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

            if (Input.GetMouseX() != 0)
            {
                gameObject.transform.localRotation = Quaternion.RotateAroundAxis(Vector3.up, -Input.GetMouseX() * mouseSens * Time.deltaTime) * gameObject.transform.localRotation;
            }

            move = gameObject.transform.GetForward();
        }

        if (dashAvaliable == false && dashing == false)
        {
            dashCDCounter += Time.deltaTime;
            if (dashCDCounter > dashCD)
            {
                dashCDCounter = 0.0f;
                dashAvaliable = true;
            }

        }

        if ((Input.GetKey(DEKeyCode.SPACE) == KeyState.KEY_DOWN || Input.GetGamepadButton(DEControllerButton.A) == KeyState.KEY_DOWN) && dashAvaliable == true)
        {
            Audio.StopAudio(gameObject);
            Audio.PlayAudio(gameObject, "Play_Dash");
            dashing = true;
            dashAvaliable = false;
            dashingCounter = 0.0f;
            Animator.Play(gameObject, "Dash");
        }

        timeSinceLastBullet += Time.deltaTime; //Moved here to keep shoot cd counting while dashing

        if (dashing == true)
        {
            Dash();
        }
        else
        {
            if (shooting == false)
                gameObject.transform.localPosition += move.normalized * movementSpeed * Time.deltaTime;

            if (move != Vector3.zero && shooting == false)
            {
                if (walking == false)
                {
                    Audio.PlayAudio(gameObject, "Play_Footstep");
                    Animator.Play(gameObject, "Run");
                }
                walking = true;
            }
            else
            {
                if (walking == true)
                {
                    Audio.StopAudio(gameObject);
                    //Animator.Play(reference, idle_animation);
                    Animator.Play(gameObject, "Idle");
                }
                walking = false;
            }


            timeSinceLastDash += Time.deltaTime;


            //Shooting
            if ((Input.GetMouseClick(MouseButton.LEFT) == KeyState.KEY_DOWN || Input.GetRightTrigger() > 0) && shooting == false)
            {
                shooting = true;
                currFireRate = fireRate;
            }
            else if ((Input.GetMouseClick(MouseButton.LEFT) == KeyState.KEY_UP || Input.GetRightTrigger() == 0) && shooting == true)
            {
                shooting = false;
                Animator.Play(gameObject, "Idle");
            }

            if (shooting == true)
            {
                Shoot();
            }

        }
    }

    private void Shoot()
    {
        currFireRate = GetCurrentFireRate();

        if (timeSinceLastBullet < currFireRate)
        {
            return;
        }

        Audio.PlayAudio(shootPoint, "Play_Weapon_Shoot_Mando");
        InternalCalls.CreateBullet(shootPoint.transform.globalPosition, shootPoint.transform.globalRotation, shootPoint.transform.globalScale);
        Input.PlayHaptic(1f,30);

        timeSinceLastBullet = 0.0f;
        //Animator.Play(reference, shoot_animation);
        Animator.Play(gameObject, "Shoot");

    }

    private void Dash()
    {
        if (dashingCounter < dashDuration)
        {
            dashingCounter += Time.deltaTime;
            gameObject.transform.localPosition += gameObject.transform.GetForward().normalized * dashSpeed * Time.deltaTime;

        }
        else
        {
            Debug.Log("Finished Dashing!");
            timeSinceLastDash = 0.0f;
            dashing = false;
            if(walking)
                Animator.Play(gameObject, "Run");
            else
                Animator.Play(gameObject, "Idle");
        }
    }

    private float GetCurrentFireRate()
    {
        float ret = fireRate;

        ret = (float)(Math.Log(timeSinceLastDash * fireRateAfterDashRecoverRatio) - Math.Log(0.01)) / fireRateRecoverCap;

        ret = Math.Min(ret, fireRate * 2.5f);

        return ret;
    }

}

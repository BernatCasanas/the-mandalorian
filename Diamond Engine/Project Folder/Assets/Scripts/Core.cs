using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using DiamondEngine;

public class Core : DiamondComponent
{

    public GameObject reference = null;
    public GameObject shootPoint = null;

    public float rotationSpeed = 2.0f;
    public float movementSpeed = 35.0f;
    public float mouseSens = 1.0f;
    public float delayTime = 0.2f;
    private float timePassed = 0.0f;

    //public Vector3 testOtherClass; //Should find a way to tell if the class is a gameobject or not

    public void Update(/*int x*/)
    {
        if (this.reference == null)
            return;

        if (Input.GetKey(DEKeyCode.W) == KeyState.KEY_REPEAT || Input.GetLeftAxisY() < -30000)
        {
            Vector3 vectorUp = new Vector3(-0.5f, 0, 0.5f);
            reference.localPosition += vectorUp * movementSpeed * Time.deltaTime;
        }
        if (Input.GetKey(DEKeyCode.S) == KeyState.KEY_REPEAT || Input.GetLeftAxisY() > 30000)
        {
            Vector3 vectorDown = new Vector3(0.5f, 0, -0.5f);
            reference.localPosition += vectorDown * movementSpeed * Time.deltaTime;
        }
        if (Input.GetKey(DEKeyCode.A) == KeyState.KEY_REPEAT || Input.GetLeftAxisX() < -30000)
        {
            Vector3 vectorLeft = new Vector3(0.5f, 0, 0.5f);
            reference.localPosition += vectorLeft * movementSpeed * Time.deltaTime;
        }
        if (Input.GetKey(DEKeyCode.D) == KeyState.KEY_REPEAT || Input.GetLeftAxisX() > 30000)
        {
            Vector3 vectorRight = new Vector3(-0.5f, 0, -0.5f);
            reference.localPosition += vectorRight * movementSpeed * Time.deltaTime;
        }
        else
        {
            if (shooting == false)
                reference.localPosition += move.normalized * movementSpeed * Time.deltaTime;

            if (move != Vector3.zero && shooting == false)
            {
                if (walking == false)
                {
                    Audio.PlayAudio(this.reference, "Play_Footstep");
                }
                walking = true;
            }
            else
            {
                if (walking == true)
                {
                    Audio.StopAudio(this.reference);
                    Animator.Play(reference, idle_animation);
                }
                walking = false;
            }


            timeSinceLastDash += Time.deltaTime;

        timePassed += Time.deltaTime;

        if ((Input.GetMouseClick(MouseButton.LEFT) == KeyState.KEY_REPEAT || Input.GetRightTrigger() > 0) && timePassed >= delayTime)
        {
            InternalCalls.CreateBullet(shootPoint.globalPosition, shootPoint.globalRotation, shootPoint.globalScale);
            timePassed = 0.0f;
        }

        Audio.PlayAudio(shootPoint, "Play_Weapon_Shoot");
        InternalCalls.CreateBullet(shootPoint.globalPosition, shootPoint.globalRotation, shootPoint.globalScale);
        timeSinceLastBullet = 0.0f;
        Animator.Play(reference, shoot_animation);

    }

    private void Dash()
    {
        if (dashingCounter < dashDuration)
        {
            dashingCounter += Time.deltaTime;
            reference.localPosition += lastDir.normalized * dashSpeed * Time.deltaTime;

        }
        else
        {
            Debug.Log("Finished Dashing!");
            timeSinceLastDash = 0.0f;
            dashing = false;
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

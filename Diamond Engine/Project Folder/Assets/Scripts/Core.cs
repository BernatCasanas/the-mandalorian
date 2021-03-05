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
    double angle = 0.0f;
    private bool walking = false;
    private bool shooting = false;

    public bool dashUp = true;
    public float dashSpeed = 70.0f;
    Vector3 lastDir = Vector3.zero;
    bool dashing = false;
    float dashCD = 0.33f;
    float dashCD_counter = 0.0f;
    float dashDuration = 0.25f;
    float dashingCounter = 0.0f;

    public void Start()
    {       
    }

    //public Vector3 testOtherClass; //Should find a way to tell if the class is a gameobject or not

    public void Update(/*int x*/)
    {
        if (this.reference == null)
            return;

        Vector3 move = Vector3.zero;
        Vector3 joyRot = new Vector3(Input.GetLeftAxisX(), Input.GetLeftAxisY(), 0);
        int joyStickSensibility = 15000;

        if (joyRot.magnitude > joyStickSensibility && !shooting)
        {
            //Calculate player rotation
            Vector3 aX = new Vector3(joyRot.x, 0, -joyRot.y - 1);
            Vector3 aY = new Vector3(0, 0, 1);
            aX = Vector3.Normalize(aX);

            if (aX.x >= 0)
                angle = Math.Acos(Vector3.Dot(aX, aY) - 1);
            else if (aX.x < 0)
                angle = -Math.Acos(Vector3.Dot(aX, aY) - 1);

            //Convert angle from world view to orthogonal view
            angle += 0.785398f; //Rotate 45 degrees to the right

            reference.localRotation = Quaternion.RotateAroundAxis(Vector3.up, (float)-angle);

            if (Input.GetMouseX() != 0 && reference != null)
                reference.localRotation = Quaternion.RotateAroundAxis(Vector3.up, -Input.GetMouseX() * mouseSens * Time.deltaTime) * reference.localRotation;

            lastDir = move += reference.GetForward();
        }

        if (dashCD_counter < dashCD && !dashUp && !dashing) dashCD_counter += Time.deltaTime;
        else
        {
            dashCD_counter = 0.0f;
            dashUp = true;
        }

        if ((Input.GetKey(DEKeyCode.SPACE) == KeyState.KEY_DOWN || Input.GetGamepadButton(DEControllerButton.A) == KeyState.KEY_DOWN) && dashUp && !dashing)
        {
            dashing = true;
            dashUp = false;
            dashingCounter = 0.0f;
        }

        timePassed += Time.deltaTime; //Moved here to keep shoot cd counting while dashing

        if (dashing) Dash();
        else
        {
            reference.localPosition += move.normalized * movementSpeed * Time.deltaTime;

            if (move != Vector3.zero)
            {
                if (!walking)
                {
                    Audio.PlayAudio(this.reference, "Play_Footstep");
                }
                walking = true;
            }
            else
            {
                if (walking)
                {
                    Audio.StopAudio(this.reference);
                }
                walking = false;
            }

            //if (Input.GetMouseY() != 0 && turret != null)
            //    turret.localRotation = turret.localRotation * Quaternion.RotateAroundAxis(Vector3.right, -Input.GetMouseY() * Time.deltaTime);         

            //Shooting
            if ((Input.GetMouseClick(MouseButton.LEFT) == KeyState.KEY_REPEAT || Input.GetRightTrigger() > 0) && timePassed >= delayTime)
            {
                shooting = true;
                Audio.PlayAudio(shootPoint, "Play_Weapon_Shoot");
                InternalCalls.CreateBullet(shootPoint.globalPosition, shootPoint.globalRotation, shootPoint.globalScale);
                timePassed = 0.0f;
            }
            else if(timePassed >= delayTime) shooting = false; //Wait delayTime before moving again
        }
    }
    private void Dash()
    {
        if (dashingCounter < dashDuration)
        {
            dashingCounter += Time.deltaTime;
            reference.localPosition += lastDir.normalized * dashSpeed * Time.deltaTime;
        }
        else dashing = false;       
    }
}

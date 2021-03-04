﻿using System;
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

    //public Vector3 testOtherClass; //Should find a way to tell if the class is a gameobject or not

    public void Update(/*int x*/)
    {
        if (this.reference == null)
            return;

        Vector3 move = Vector3.zero;
        Vector3 joyRot = Vector3(Input.GetLeftAxisX(), Input.GetLeftAxisY(), 0);
        int joyStickSensibility = 30000;

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

        //if (Input.GetKey(DEKeyCode.W) == KeyState.KEY_REPEAT || Input.GetLeftAxisY() < -30000)
        //{
        //    move += reference.GetForward();
        //    //reference.localRotation = Quaternion.RotateAroundAxis(Vector3.up, -1.0472f);
        //}
        //if (Input.GetKey(DEKeyCode.A) == KeyState.KEY_REPEAT || Input.GetLeftAxisX() < -30000)
        //{
        //    move += reference.GetForward();
        //    //reference.localRotation = Quaternion.RotateAroundAxis(Vector3.up, 1.0472f);
        //}
        //if (Input.GetKey(DEKeyCode.S) == KeyState.KEY_REPEAT || Input.GetLeftAxisY() > 30000)
        //{
        //    move += reference.GetForward();
        //    //reference.localRotation = Quaternion.RotateAroundAxis(Vector3.up, 2.61799f);
        //}        
        //if (Input.GetKey(DEKeyCode.D) == KeyState.KEY_REPEAT || Input.GetLeftAxisX() > 30000)
        //{
        //    move += reference.GetForward();
        //    //reference.localRotation = Quaternion.RotateAroundAxis(Vector3.up, -2.61799f);
        //}

        if (joyRot.magnitude > joyStickSensibility)
        {
            move += reference.GetForward();
        }

        reference.localPosition += move.normalized * movementSpeed * Time.deltaTime;
        
        if (move != Vector3.zero)
        { 
            if(!walking)
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

        timePassed += Time.deltaTime;

        if ((Input.GetMouseClick(MouseButton.LEFT) == KeyState.KEY_REPEAT || Input.GetRightTrigger() > 0) && timePassed >= delayTime)
        {
            InternalCalls.CreateBullet(shootPoint.globalPosition, shootPoint.globalRotation, shootPoint.globalScale);
            timePassed = 0.0f;
        }
    }
}
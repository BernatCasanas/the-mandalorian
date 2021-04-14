using System;
using DiamondEngine;

public class BoonInstance : DiamondComponent
{
    // OBSOLETE SCRIPT. Now boon functionality is managed from somewhere else. I didn't delete it because engine began exploding for some reason

    /*public string myType = "";
    Boon myBoon = null;
    BoonSpawn boonSpawner = null;
    public float verticalMovementAmount = 1.0f;
    public float rotSpeedDegSec = 1.0f;
    float rotationAngle = 0.0f;
    public float verticalSpeedMultiplier = 0.5f;

    Vector3 initialPos; //lowest position of the obj
    bool goingUp = true;
    float animTime = 0.0f;

    bool firstFrame = true;

    public void Update()
    {
        if (firstFrame == true)
        {
            firstFrame = false;
            initialPos = new Vector3(0.0f, 0.0f, 0.0f);
            if (Boon_Data_Holder.boonType.ContainsKey(myType))
            {
                Type t = Boon_Data_Holder.boonType[myType];
                myBoon = (Boon)Activator.CreateInstance(t);
                boonSpawner = Core.instance.gameObject.GetComponent<BoonSpawn>();
            }
            else
            {
                Debug.Log("ERROR: Boon type doesn't exist");
            }

            //Setting positions
            initialPos = new Vector3(gameObject.transform.localPosition.x, gameObject.transform.localPosition.y, gameObject.transform.localPosition.z);
        }

        //TODO animate boon here (UP-DOWN + rotation movement)
        AdvanceVerticalMovement();
        AdvanceRotation();
    }


    public void OnTriggerEnter(GameObject collidedGameObject)
    {
        if (collidedGameObject != null && collidedGameObject.CompareTag("Player"))//TODO sort this by UID
        {
            //Debug.Log("COLLISION DETECTED WITH: " + collidedGameObject.name);
            if (boonSpawner != null && myBoon != null)
            {
                myBoon.Use();
                boonSpawner.DestroyAllCreatedBoons();
            }
        }

    }

    public void AdvanceVerticalMovement()
    {
        if (goingUp)
        {
            animTime += Time.deltaTime * verticalSpeedMultiplier;
        }
        else
        {
            animTime -= Time.deltaTime * verticalSpeedMultiplier;
        }

        if (animTime > 1.0f)
        {
            goingUp = false;
            animTime = 1.0f;
        }
        else if (animTime < 0.0f)
        {
            goingUp = true;
            animTime = 0.0f;

        }
        float yPos = ParametricBlend(animTime);



        Vector3 newPos = new Vector3(initialPos.x, initialPos.y, initialPos.z);
        newPos.y += yPos * verticalMovementAmount;
        gameObject.transform.localPosition = newPos;

    }

    public void AdvanceRotation()
    {
        rotationAngle += rotSpeedDegSec * Time.deltaTime;
        Vector3 axis = new Vector3(0.0f, 1.0f, 0.0f);
        float myRotAngle = 0.0174533f * rotationAngle;
        gameObject.transform.localRotation = Quaternion.RotateAroundAxis(axis, myRotAngle);

    }

    public float ParametricBlend(float t)
    {
        float sqt = t * t;
        return sqt / (2.0f * (sqt - t) + 1.0f);
    }*/

}
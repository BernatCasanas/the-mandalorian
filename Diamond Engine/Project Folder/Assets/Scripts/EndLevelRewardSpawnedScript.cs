using System;
using DiamondEngine;

public class EndLevelRewardSpawn : DiamondComponent
{
    public bool trigger = false;
    public float verticalMovementAmount = 1.0f;
    public float rotSpeedDegSec = 1.0f;
    float rotationAngle = 0.0f;
    public float verticalSpeedMultiplier = 0.5f;
    bool goingUp = true;
    float animTime = 0.0f;

    public void OnTriggerEnter(GameObject collidedGameObject)
    {
        if (collidedGameObject != null && collidedGameObject.CompareTag("Player"))
        {
            trigger = true;
        }
    }

    public void AdvanceVerticalMovement(Vector3 initialPos)
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
        gameObject.transform.localPosition.Set(newPos.x, newPos.y, newPos.z);

    }

    public void AdvanceRotation()
    {
        rotationAngle += rotSpeedDegSec * Time.deltaTime;
        Vector3 axis = new Vector3(0.0f, 1.0f, 0.0f);
        Quaternion newQuat = Quaternion.RotateAroundAxis(axis, rotationAngle);
        gameObject.transform.globalRotation.Set(newQuat.x, newQuat.y, newQuat.z, newQuat.w);
    }

    public float ParametricBlend(float t) => ((t * t) / (2.0f * ((t * t) - t) + 1.0f));

}
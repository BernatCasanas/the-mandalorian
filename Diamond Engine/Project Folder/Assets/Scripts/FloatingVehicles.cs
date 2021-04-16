using System;
using DiamondEngine;

public class FloatingVehicles : DiamondComponent
{
	public float verticalMovementAmount = 1.0f;
	public float rotSpeedDegSec = 1.0f;
	public float verticalSpeedMultiplier = 0.5f;
	bool goingUp = true;
	float animTime = 0.0f;
    private Vector3 initialPos;

    public void Awake()
    {
        initialPos = gameObject.transform.localPosition;
    }

    public void Update()
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

    public float ParametricBlend(float t) => ((t * t) / (2.0f * ((t * t) - t) + 1.0f));
}
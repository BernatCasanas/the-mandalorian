using System;
using DiamondEngine;

public class FloatingVehicles : DiamondComponent
{
    //Vehicle Movement
	private float maxHeight = 1.0f;
	private float verticalSpeed = 0.5f;
	private bool goingUp = true;
	private float currPercentageOfAnimation = 0.0f;    
    private Vector3 initialPos;

    //Vehicle Rotation
    private float currRotation = 0.0f;
    private float maxRotation = 0.15f; 

    public void Awake()
    {
        initialPos = gameObject.transform.localPosition;
        SetVehicleProperties();
        currPercentageOfAnimation = GetRandomStartHeight();
    }

    public void Update()
    {   
        if (goingUp) currPercentageOfAnimation += Time.deltaTime * verticalSpeed;        
        else currPercentageOfAnimation -= Time.deltaTime * verticalSpeed;
        
        VehicleRotation();

        if (currPercentageOfAnimation > 1.0f) goingUp = false;
        else if (currPercentageOfAnimation < 0.0f) goingUp = true;

        //Vehicle Position
        float yPos = ParametricBlend(currPercentageOfAnimation);
        Vector3 newPos = new Vector3(initialPos.x, initialPos.y, initialPos.z);
        newPos.y += yPos * maxHeight;
        gameObject.transform.localPosition = newPos;

        //Vehicle Rotation
        gameObject.transform.localRotation *= Quaternion.RotateAroundAxis(Vector3.right, currRotation * Mathf.Deg2RRad);
    }

    public float ParametricBlend(float t) => ((t * t) / (2.0f * ((t * t) - t) + 1.0f));

    private float GetRandomStartHeight()
    {
        float min = 0.0f;
        float max = 1.0f;
        System.Random random = new System.Random();
        double val = (random.NextDouble() * (max - min) + min);
        return (float)val;
    }

    private void VehicleRotation()
    {
        if(currPercentageOfAnimation <= 0.5f)
        {
            currRotation = currPercentageOfAnimation * 2.0f * maxRotation;
        }
        else
        {
            float localPercentage = (currPercentageOfAnimation - 0.5f) * 2.0f;
            currRotation = -1.0f * (1.0f - localPercentage) * maxRotation;
        }
    }

    private void SetVehicleProperties()
    {
        switch (gameObject.Name)
        {
            case "LandSpeeder": 
                maxHeight = 2.0f;
                verticalSpeed = 0.3f;
                break;
            case "SpeederBike": 
                maxHeight = 0.8f;
                verticalSpeed = 0.5f;
                break;
            case "SnowSpeeder": 
                maxHeight = 1.5f;
                verticalSpeed = 0.15f;
                break;
            default:
                Debug.Log("Vehicle properties have not been setup correctly");
                break;            
        }
    }
}
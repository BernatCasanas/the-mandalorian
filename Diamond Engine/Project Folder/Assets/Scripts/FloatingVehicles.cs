using System;
using DiamondEngine;

public class FloatingVehicles : DiamondComponent
{
	private float maxHeight = 1.0f;
	private float verticalSpeed = 0.5f;
	private bool goingUp = true;
	private float currPercentageOfAnimation = 0.0f;
    private Vector3 initialPos;

    public void Awake()
    {
        initialPos = gameObject.transform.localPosition;
        SetVehicleProperties();
    }

    public void Update()
    {
        if (goingUp) currPercentageOfAnimation += Time.deltaTime * verticalSpeed;        
        else currPercentageOfAnimation -= Time.deltaTime * verticalSpeed;        

        if (currPercentageOfAnimation > 1.0f) goingUp = false;
        else if (currPercentageOfAnimation < 0.0f) goingUp = true;        

        float yPos = ParametricBlend(currPercentageOfAnimation);
        Vector3 newPos = new Vector3(initialPos.x, initialPos.y, initialPos.z);
        newPos.y += yPos * maxHeight;
        gameObject.transform.localPosition = newPos;
    }

    public float ParametricBlend(float t) => ((t * t) / (2.0f * ((t * t) - t) + 1.0f));

    /*private float GenerateRandomHeight()
    {
        float min = 1.0f - randomPercentage;
        float max = 1.0f + randomPercentage;
        System.Random random = new System.Random();
        double val = (random.NextDouble() * (max - min) + min);
        return (float)val;
    }*/

    private void SetVehicleProperties()
    {
        Debug.Log("-------------------GO Name:" + gameObject.Name);
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
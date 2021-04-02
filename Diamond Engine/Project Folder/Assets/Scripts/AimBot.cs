using System;
using DiamondEngine;
using System.Collections.Generic;

public class AimBot : DiamondComponent
{
    public float startAimConeAngle = 90.0f;//this will only be used on the inspector, to change the actual angle use the method
    public float maxRange = 50.0f; //maximum distance at wich it will
    public float angleWeight = 0.5f;
    public float distanceWeight = 0.5f;
    public GameObject spawnGameObject = null;
    float dotMin = 0;
    Spawn spawnComponent = null;
    bool start = true;

    int lastFrameEnemyCount = 0;
    GameObject myCurrentObjective = null;
    public bool isShooting = false;

    public void Update()
    {

        if (start)
        {
            if (spawnGameObject != null)
                spawnComponent = spawnGameObject.GetComponent<Spawn>();

            ChangeConeAngle(startAimConeAngle);
            start = false;
        }

        if (!isShooting && myCurrentObjective != null)
            myCurrentObjective = null;

        if (spawnComponent != null)
        {
            if (isShooting)
            {
                if (myCurrentObjective == null)
                {
                    SearchForNewObjective();
                }
                else if (lastFrameEnemyCount != spawnComponent.currentEnemies.Count)//change in enemies! if targeting an enemy make sure it hasn't died
                {
                    if (myCurrentObjective != null && !spawnComponent.currentEnemies.Contains(myCurrentObjective))//if the target is not in the list anymore search for a new objective
                    {
                        SearchForNewObjective();
                    }
                }
            }

            lastFrameEnemyCount = spawnComponent.currentEnemies.Count;
        }

    }

    //takes the overall cone angle as an input (an angle of 90 means 45 degrees to each side of the player forward)
    public void ChangeConeAngle(float newAngle)
    {
        float newAngleRad = newAngle * (1 / Mathf.Rad2Deg);
        dotMin = (float)Math.Cos(newAngleRad * 0.5f);
        //Debug.Log("New Dot Min set to: " + dotMin + " From Angle: " + newAngle);
    }

    //assigns a new objective if there is any objective in range, otherwise myCurrentObjective is null
    public void SearchForNewObjective()
    {
        if (spawnComponent == null)
        {
            myCurrentObjective = null;
            return;
        }


        //Test
        //if (spawnComponent.currentEnemies.Count > 0)
        //    myCurrentObjective = spawnComponent.currentEnemies[0];
        //end test

        //Debug.Log("Searching for a new objective!");
        KeyValuePair<float, GameObject> weightedObj = new KeyValuePair<float, GameObject>(float.NegativeInfinity, null);

        Debug.Log("Enemies searching num: " + spawnComponent.currentEnemies.Count.ToString());
        for (int i = 0; i < spawnComponent.currentEnemies.Count; ++i)
        {
            float targetWeight = GetTargetWeight(spawnComponent.currentEnemies[i]);


            if (targetWeight > weightedObj.Key)
            {
                weightedObj = new KeyValuePair<float, GameObject>(targetWeight, spawnComponent.currentEnemies[i]);
            }
        }

        if (weightedObj.Key != float.NegativeInfinity)
        {
            myCurrentObjective = weightedObj.Value;
            //Debug.Log("Objective found: " + myCurrentObjective.name);
            //Debug.Log("New Objecte! Position: " + weightedObj.Value.Name.ToString());
        }
        else
        {
            Debug.Log("No Objective found!!");
            myCurrentObjective = null;
            //Debug.Log("No suitable objective found!");
        }
    }


    float GetTargetWeight(GameObject target)
    {
        float newDistanceWeight = 0.0f;
        float newAngleWeight = 0.0f;

        Vector3 myPos = new Vector3(gameObject.transform.globalPosition.x, 0.0f, gameObject.transform.globalPosition.z);
        Vector3 myForward = new Vector3(gameObject.transform.GetForward().x, 0.0f, gameObject.transform.GetForward().z).normalized;
        Vector3 targetPos = new Vector3(target.transform.globalPosition.x, 0.0f, target.transform.globalPosition.z);
        Vector3 targetVector = targetPos - myPos;//note it is not normalized
        float distance = targetVector.magnitude;
        Vector3 targetDirection = targetVector.normalized;
        float targetDot = Vector3.Dot(myForward, targetDirection);


        if (distance <= maxRange)
        {
            newDistanceWeight = ((maxRange - distance) / maxRange) * distanceWeight;//number between 0 & 1 being 1 when the distance is the smallest *distance weight
            //newDistanceWeight = (1 / distance) * distanceWeight;//TODO simplification
        }

        if (targetDot >= dotMin)
        {
            newAngleWeight = Mathf.InvLerp(dotMin, 1.0f, targetDot) * angleWeight;
            //newAngleWeight = targetDot * angleWeight;//TODO simplification
        }

        if (newDistanceWeight != 0.0f && newAngleWeight != 0.0f)
            return newDistanceWeight + newAngleWeight;
        else
            return float.NegativeInfinity;
    }

    public bool HasObjective()
    {
        return (myCurrentObjective != null);
    }

    public void RotateToObjective()
    {
        if (!HasObjective())
            return;

        //Debug.Log("Rotating to the objective!");

        Vector3 targetDire = (myCurrentObjective.transform.globalPosition - gameObject.transform.globalPosition);
        targetDire.y = 0.0f;
        targetDire = targetDire.normalized;
        float angle = (float)Math.Atan2(targetDire.x, targetDire.z);
        Quaternion dir = Quaternion.RotateAroundAxis(Vector3.up, angle);
        gameObject.transform.localRotation = dir;
        //Debug.Log("ROTATE ANGLE: " + angle.ToString());

    }
}
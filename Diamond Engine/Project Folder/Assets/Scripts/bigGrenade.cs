using System;
using DiamondEngine;

public class bigGrenade : DiamondComponent
{
    public GameObject thisReference = null; //This is needed until i make all this be part of a component base class
   

    public float speed = 25.0f;

    public float yVel = 0.0f;

    public float detonationTime = 1.5f;

    private float Timer = 0.0f;

    private bool move = true;

    private bool detonate = false;

    public void OnCollisionEnter(GameObject collidedGameObject)
    {
        if (collidedGameObject.CompareTag("Enemy"))
        {
            detonate = true;
        }

    }

    public void Update()
    {
        if (thisReference.transform.globalPosition.y < Core.instance.gameObject.transform.globalPosition.y + 0.5)
        {
            move = false;
        }

        if (move)
        {
            gameObject.transform.localPosition += gameObject.transform.GetForward() * (speed * Time.deltaTime);

            yVel -= Time.deltaTime * 0.102f;
            gameObject.transform.localPosition += (Vector3.up * yVel);
        }
        else
        {
            Timer += Time.deltaTime;

        }

        if (Timer > detonationTime || detonate)
        {
            InternalCalls.Destroy(thisReference);
            Vector3 scale = new Vector3(0.07f, 0.07f, 0.07f);
               
            Vector3 position = new Vector3(0.0f, 0.0f, 0.0f); ;
            position.y = 0.25f;

            position.x = 0.45f;
            position.z = 0f;
            //  InternalCalls.CreatePrefab("Library/Prefabs/1968664915.prefab", position, thisReference.transform.globalRotation, scale);
            InternalCalls.CreatePrefab("Library/Prefabs/1968664915.prefab",  thisReference.transform.globalPosition + position, thisReference.transform.globalRotation, scale);
            
            position.x = 0f;
            position.z = -0.45f;
            InternalCalls.CreatePrefab("Library/Prefabs/1968664915.prefab",  thisReference.transform.globalPosition + position, thisReference.transform.globalRotation, scale);

            position.x = -0.45f;
            position.z = 0f;
            InternalCalls.CreatePrefab("Library/Prefabs/1968664915.prefab",  thisReference.transform.globalPosition + position, thisReference.transform.globalRotation, scale);

            position.x  = 0f;
            position.z  = 0.45f;
            InternalCalls.CreatePrefab("Library/Prefabs/1968664915.prefab",  thisReference.transform.globalPosition + position, thisReference.transform.globalRotation, scale);

            position.x = 0f;
            position.z = 0.9f;
            InternalCalls.CreatePrefab("Library/Prefabs/1968664915.prefab",  thisReference.transform.globalPosition + position, thisReference.transform.globalRotation, scale); 
            
            position.x = 0.675f;
            position.z = 0.675f;
            InternalCalls.CreatePrefab("Library/Prefabs/1968664915.prefab", thisReference.transform.globalPosition +  position, thisReference.transform.globalRotation, scale);

            position.x = 0.9f;
            position.z = 0f;
            InternalCalls.CreatePrefab("Library/Prefabs/1968664915.prefab",  thisReference.transform.globalPosition + position, thisReference.transform.globalRotation, scale);

            position.x = 0.675f;
            position.z = -0.675f;
            InternalCalls.CreatePrefab("Library/Prefabs/1968664915.prefab",  thisReference.transform.globalPosition + position, thisReference.transform.globalRotation, scale);

            position.x = 0f;
            position.z = -0.9f;
            InternalCalls.CreatePrefab("Library/Prefabs/1968664915.prefab",  thisReference.transform.globalPosition + position, thisReference.transform.globalRotation, scale);

            position.x = -0.675f;
            position.z = -0.675f;
            InternalCalls.CreatePrefab("Library/Prefabs/1968664915.prefab",  thisReference.transform.globalPosition + position, thisReference.transform.globalRotation, scale);

            position.x = -0.9f;
            position.z = 0f;
            InternalCalls.CreatePrefab("Library/Prefabs/1968664915.prefab",  thisReference.transform.globalPosition + position, thisReference.transform.globalRotation, scale);

            position.x = -0.675f;
            position.z = 0.675f;
            InternalCalls.CreatePrefab("Library/Prefabs/1968664915.prefab",  thisReference.transform.globalPosition + position, thisReference.transform.globalRotation, scale);
        }

    }

}
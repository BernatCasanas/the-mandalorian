using System;
using DiamondEngine;

public class bigGrenade : DiamondComponent
{
    public GameObject thisReference = null; //This is needed until i make all this be part of a component base class

    public float speed = 30.0f;

    public float yVel = 0.0f;

    public float detonationTime = 2.0f;

    private float Timer = 0.0f;

    private bool move = true;


    public void OnCollisionEnter(GameObject collidedGameObject)
    {

        move = false;

    }

    public void Update()
    {
        if (move)
        {
            gameObject.transform.localPosition += gameObject.transform.GetForward() * (speed * Time.deltaTime);

            yVel -= Time.deltaTime / 15.0f;
            gameObject.transform.localPosition += (Vector3.up * yVel);
        }
        else
        {
            Timer += Time.deltaTime;

        }

        if (Timer > detonationTime)
        {
            InternalCalls.Destroy(thisReference);
            Vector3 scale = new Vector3(0.07f, 0.07f, 0.07f);
          
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    Vector3 position = thisReference.transform.globalPosition;
                    position.x += i * 0.1f;
                    position.z += j * 0.1f;
                    InternalCalls.CreatePrefab("Library/Prefabs/1968664915.prefab", position, thisReference.transform.globalRotation, scale);
                }

                
            }


        }

    }

}
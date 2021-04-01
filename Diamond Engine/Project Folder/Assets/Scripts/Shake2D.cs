using System;
using DiamondEngine;

public class Shake2D : DiamondComponent
{
    public GameObject elementShake = null; //if this is left as null, shake will take its game object as the shake element
    public float shakeXMax = 0.0f;
    public float shakeYMax = 0.0f;
    public float maxShakeTime = 0.0f;
    float currShakeTime = 0.0f;
    PerlinNoise2D shakeNoiseX = new PerlinNoise2D();
    PerlinNoise2D shakeNoiseY = new PerlinNoise2D();
    Vector3 initialPos = new Vector3(0.0f, 0.0f, 0.0f);

    //float shake = 1.0f;//value between 0 and 1 that will control the amount of shake performed in the future

    bool start = true;
    public void Update()
    {
        if (start)
        {
            //TODO need to have Transform2D as a c# component to continue
            //Transform2D trans2D = null;
            if (elementShake == null)
            {
                //trans2D = elementShake.GetComponent<Transform2D>();
                //if(trans2D!=null)
                elementShake = gameObject;
            }
            else
            {
                //trans2D = elementShake.GetComponent<Transform2D>();
                //if(trans2D==null)
                //elementShake = null;
            }

            if (elementShake != null)
            {

                //initialPos = new Vector3(trans2D.position.x, trans2D.position.y, trans2D.position.z);
            }


            start = false;
        }

        if (elementShake == null)
            return;

        if (currShakeTime < maxShakeTime)
        {
            PerformShake();
        }


        //Test CODE
        if (Input.GetKey(DEKeyCode.J) == KeyState.KEY_DOWN) //test key
        {
            StartShake(1.0f);
        }

    }

    public void StartShake(float duration, int seed = -1)
    {
        maxShakeTime = Math.Abs(duration);
        currShakeTime = 0.0f;

        if (seed != -1)
        {
            shakeNoiseX.GenerateNoise(seed, 4, 10.0f);
            shakeNoiseY.GenerateNoise(seed + 1, 4, 10.0f);
        }
        else
        {
            shakeNoiseX.GenerateNoise(4, 10.0f);
            shakeNoiseY.GenerateNoise(4, 10.0f);
        }

    }
    public void StopShake()
    {
        maxShakeTime = 0.0f;
        currShakeTime = 0.0f;
    }

    void PerformShake()
    {
        //TODO need to have Transform2D as a c# component to continue

        //Transform2D trans2D = elementShake.GetComponent<Transform2D>();
        //trans2D.position.x = initialPos.x + shakeXMax * shake * shakeNoiseX.GetNoiseAt(0, currShakeTime);
        //trans2D.position.y = initialPos.y + shakeYMax * shake * shakeNoiseY.GetNoiseAt(0, currShakeTime);

        Debug.Log("NOISE X: " + shakeNoiseX.GetNoiseAt(0, currShakeTime).ToString() + " =================================");
        Debug.Log("NOISE Y: " + shakeNoiseY.GetNoiseAt(0, currShakeTime).ToString() + " =================================");

        currShakeTime += Time.deltaTime;

        if (currShakeTime > maxShakeTime)
            StopShake();
    }

}
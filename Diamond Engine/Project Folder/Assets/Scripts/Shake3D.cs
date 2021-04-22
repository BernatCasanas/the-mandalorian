using System;
using DiamondEngine;

public class Shake3D : DiamondComponent
{

    private float shakeTimer = 0f;
    private float shakeMaxDistance = 0.7f;
    private float decreaseFactor = 1.0f;

    private bool enable = false;
    private Random random;

	public void Awake()
    {
        if (gameObject.transform == null)
        {
			Debug.Log("Component Transform is NULL");
        }
        random = new Random();
    }

    public void StartShaking(float duration, float maxDistance)
    {
        shakeMaxDistance = maxDistance;
        enable = true;
        shakeTimer = duration;
    }

	public void Update()
	{
        if (!enable) return;

        if (shakeTimer > 0)
        {
            int intShake = (int)(shakeMaxDistance * 100);
            Debug.Log(random.Next(-intShake, intShake).ToString());
            Vector3 randomPos = new Vector3((float)random.Next(-intShake, intShake) / 100, (float)random.Next(-intShake, intShake) / 100, (float)random.Next(-intShake, intShake) / 100);
            Debug.Log(randomPos.ToString());
            gameObject.transform.localPosition += randomPos;

            shakeTimer -= Time.deltaTime;
        }
        else
        {
            enable = false;
        }
	}

}
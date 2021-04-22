using System;
using DiamondEngine;

public class Shake3D : DiamondComponent
{

    private float shakeTimer = 0f;
    private float shakeMaxDistance = 0.7f;
    private float decreaseFactor = 1.0f;

    private bool enable = false;
    private Vector3 originPos;
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
        originPos = gameObject.transform.localPosition;
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
            int x = random.Next(-intShake, intShake);
            int y = random.Next(-intShake, intShake);
            int z = random.Next(-intShake, intShake);
            Debug.Log(random.Next(-intShake, intShake).ToString());
            Vector3 randomPos = new Vector3((float)x/100, (float)z / 100, (float)z / 100);
            Debug.Log(randomPos.ToString());
            gameObject.transform.localPosition = originPos + randomPos;

            shakeTimer -= Time.deltaTime;
        }
        else
        {
            gameObject.transform.localPosition = originPos;
            enable = false;
        }
	}

}
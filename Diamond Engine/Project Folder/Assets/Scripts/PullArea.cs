using System;
using DiamondEngine;

public class PullArea : DiamondComponent
{
	public float areaRange = 10f;
    public float initSpeed = 2f;
    public float finalSpeed = 10f;
    float timer = 0.0f;
    public float gettingDistance = 2f;
	public void Update()
	{
      
        if (InRange())
        {
            timer += Time.deltaTime;
            Vector3 direction = Core.instance.gameObject.transform.globalPosition - gameObject.transform.globalPosition;
            direction.y = 0;
            float speed = Mathf.Lerp(initSpeed, finalSpeed, Ease.OutCubic(timer));
            gameObject.transform.localPosition += direction.normalized * speed * Time.deltaTime;

            if (gameObject.transform.globalPosition.DistanceNoSqrt(Core.instance.gameObject.transform.globalPosition) < gettingDistance)
            {
                Core.instance.GetCoin();
                InternalCalls.Destroy(gameObject);
            }
        }
	}
    public bool InRange()
    {
        if (Core.instance == null)
            return false;

        Vector3 playerPos = Core.instance.gameObject.transform.globalPosition;
        double distance = gameObject.transform.globalPosition.DistanceNoSqrt(playerPos);

        if (distance >= -areaRange && distance <= areaRange)
            return true;
        else
            return false;
    }
}
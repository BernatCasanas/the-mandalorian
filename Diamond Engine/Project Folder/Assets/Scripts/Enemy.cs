using System;
using DiamondEngine;


public class Enemy : Entity
{
	public GameObject player = null;

	public float detectionRange = 12.5f;
	public float damage = 5.0f;
    public float healthPoints = 60.0f;

	protected Vector3 targetPosition = null;
	protected float stoppingDistance = 1.0f;
	protected string coinDropPath = "Library/Prefabs/1907169014.prefab";

	public float slerpSpeed = 5.0f;

	//protected STATES currentState = STATES.WANDER;

	protected NavMeshAgent agent;

	public virtual void TakeDamage(float damage)
	{}

	public virtual Vector3 CalculateNewPosition(float maxPos)
	{
        Vector3 newPosition = new Vector3(0, 0, 0);
        Random random = new Random();

        newPosition.x = random.Next(-(int)maxPos, (int)maxPos);
        newPosition.y = gameObject.transform.localPosition.y;
        newPosition.z = random.Next(-(int)maxPos, (int)maxPos);

        return newPosition;

        //return InternalCalls.GetWalkablePointAround(gameObject.transform.globalPosition, maxPos);
    }

	public void MoveToPosition(Vector3 positionToReach, float speed)
	{
		Vector3 direction = positionToReach - gameObject.transform.localPosition;

		gameObject.transform.localPosition += direction.normalized * speed * myDeltaTime;
	}
	public void InterpolatePosition(Vector3 positionToReach, float speed)
	{
		gameObject.transform.localPosition = Vector3.Lerp(gameObject.transform.localPosition, positionToReach, speed * myDeltaTime);
	}

	public void LookAt(Vector3 pointToLook)
	{
		Vector3 direction = pointToLook - gameObject.transform.globalPosition;
		direction = direction.normalized;
		float angle = (float)Math.Atan2(direction.x, direction.z);

		//Debug.Log("Desired angle: " + (angle * Mathf.Rad2Deg).ToString());

		if (Math.Abs(angle * Mathf.Rad2Deg) < 1.0f)
			return;

		Quaternion dir = Quaternion.RotateAroundAxis(Vector3.up, angle);

		float rotationSpeed = myDeltaTime * slerpSpeed;

		Quaternion desiredRotation = Quaternion.Slerp(gameObject.transform.localRotation, dir, rotationSpeed);

		gameObject.transform.localRotation = desiredRotation;
	}

	public bool InRange(Vector3 point, float givenRange)
    {
		return Mathf.Distance(gameObject.transform.globalPosition, point) < givenRange;
	}

	public void DropCoins()
    {
		//Created dropped coins
		var rand = new Random();
		int droppedCoins = rand.Next(1, 4);
		for (int i = 0; i < droppedCoins; i++)
		{
			Vector3 pos = gameObject.transform.globalPosition;
			pos.x += rand.Next(-200, 201) / 100;
			pos.z += rand.Next(-200, 201) / 100;
			InternalCalls.CreatePrefab(coinDropPath, pos, Quaternion.identity, new Vector3(0.07f, 0.07f, 0.07f));
		}
	}
}

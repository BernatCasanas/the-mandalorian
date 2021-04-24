using System;
using DiamondEngine;

public class SkyTrooperShot : DiamondComponent
{
	public int damage = 14;
	ParticleSystem particles = null;
	public float speed= 0.0f;
	public float gravity = 0.0f;

	Vector3 initialPos = null;
	Vector3 velocity = new Vector3(0,0,0);

	public float lifeTime = 6.0f;

	Vector3 targetPosition = null;
	float time = 0.0f;
	float deleteTimer = 0.0f;
	bool hasImpacted = false;


	public void Awake()
    {
		if (gravity > 0)
			gravity = -gravity;
    }
	public void Update()
	{
        if (time < lifeTime && !hasImpacted)
        {
			time += Time.deltaTime;
			Vector3 pos = new Vector3(initialPos.x,initialPos.y,initialPos.z);
			pos.x += velocity.x * time;
			pos.z += velocity.z * time;
			pos.y += (velocity.y * time) + (0.5f * gravity * time * time);
			gameObject.transform.localPosition = new Vector3(pos.x,pos.y,pos.z);
        }
        else if(!hasImpacted)
        {
			InternalCalls.Destroy(gameObject);
		}

		if(deleteTimer > 0.0f)
        {
			deleteTimer -= Time.deltaTime;

			if(deleteTimer <= 0.0f)
            {
				InternalCalls.Destroy(gameObject);
			}
        }
	}

	public void SetTarget(Vector3 target, bool low_angle)
    {
		initialPos = gameObject.transform.localPosition;
		targetPosition = target;

		float distanceX;
		float distanceZ;
		float distanceHorizontal;

		float distanceY = targetPosition.y - initialPos.y;

		if (initialPos.x == targetPosition.x)
        {
			distanceX = 0;
			distanceZ = targetPosition.z - initialPos.z;
			distanceHorizontal = distanceZ;
        }
		else if (initialPos.z == targetPosition.z)
        {
			distanceX = targetPosition.x - initialPos.x;
			distanceZ = 0;
			distanceHorizontal = distanceX;
		}
        else
        {
			distanceX = targetPosition.x - initialPos.x;
			distanceZ = targetPosition.z - initialPos.z;

			distanceHorizontal = (float)Math.Sqrt(distanceX*distanceX+distanceZ*distanceZ);
		}

		float calcAngleSubFunction = (speed * speed) / (-gravity * distanceHorizontal);
		float angleSpeed;
		if (low_angle)
			angleSpeed = (float)Math.Atan(calcAngleSubFunction - (float)Math.Sqrt(calcAngleSubFunction * calcAngleSubFunction - 1 - 2 * calcAngleSubFunction * distanceY / distanceHorizontal));
		else 
			angleSpeed = (float)Math.Atan(calcAngleSubFunction + (float)Math.Sqrt(calcAngleSubFunction * calcAngleSubFunction - 1 - 2 * calcAngleSubFunction * distanceY / distanceHorizontal));


		float velocityHorizontal = speed * (float)Math.Cos(angleSpeed);
		
		velocity.y = speed *(float)Math.Sin(angleSpeed);

		if (distanceX == 0)
        {
			velocity.x = 0;
			velocity.z = velocityHorizontal;
        }
		else if (distanceZ == 0)
        {
			velocity.x = velocityHorizontal;
			velocity.z = 0;
		}
		else
        {
			float angle = (float)Math.Atan(distanceZ / distanceX); 
			velocity.x = velocityHorizontal * (float)Math.Cos(angle);
			velocity.z = velocityHorizontal * (float)Math.Sin(angle);
		}
		
		if (distanceX < 0 && velocity.x > 0)
			velocity.x = -velocity.x;


		if ((distanceZ < 0 && velocity.z > 0) || (distanceZ > 0 && velocity.z < 0))
			velocity.z = -velocity.z;

	}

	public void OnCollisionEnter(GameObject collidedGameObject)
	{
		hasImpacted = true;
		if(particles != null)
			particles.Play();

		deleteTimer = 5.0f;

		if(collidedGameObject.CompareTag("Player"))
        {
			PlayerHealth playerHealth = collidedGameObject.GetComponent<PlayerHealth>();

			if(playerHealth != null)
            {
				playerHealth.TakeDamage(damage);
            }
        }
		InternalCalls.Destroy(gameObject);
	}
}
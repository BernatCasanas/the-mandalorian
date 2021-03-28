using System;
using DiamondEngine;

public class ColliderTest : DiamondComponent
{
	public void Update()
	{

	}

	public void OnCollisionEnter(GameObject collidedGameObject)
	{
		Debug.Log("Collision detected with: " + collidedGameObject.tag);
	}

	public void OnCollisionStay(GameObject collidedGameObject)
	{ 
		Debug.Log("Touching with: " + collidedGameObject.tag);
	}
	public void OnCollisionExit(GameObject collidedGameObject)
	{
		Debug.Log("Exiting collision with: " + collidedGameObject.tag);
	}

	public void OnTriggerEnter(GameObject triggeredGameObject)
	{
		Debug.Log("Trigger detected with: " + triggeredGameObject.tag);
	}

	public void OnTriggerExit(GameObject triggeredGameObject)
	{
		Debug.Log("Exiting trigger with: " + triggeredGameObject.tag);
	}
}
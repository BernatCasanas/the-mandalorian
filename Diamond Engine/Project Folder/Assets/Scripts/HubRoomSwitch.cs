using System;
using DiamondEngine;

public class HubRoomSwitch : DiamondComponent
{
    public int nextRoomUID;
    public GameObject colliderPosition;
    public GameObject player;
    public float maxDistance = 5;
    public bool insideCollider = false;
    
    public bool IsInside()
    {         
        Vector3 playerPos = player.transform.globalPosition;
        Vector3 colliderPos = colliderPosition.transform.globalPosition;
        double insideNum = Math.Pow(playerPos.x - colliderPos.x, 2) + Math.Pow(playerPos.y - colliderPos.y, 2) + Math.Pow(playerPos.z - colliderPos.z, 2);
        double distance = Math.Sqrt(insideNum);
        Debug.Log("distance:" + distance);
        Debug.Log("maxdistance:" + maxDistance);
        if (distance >= -maxDistance && distance <= maxDistance)
        {
            Debug.Log("true");
            return true;
        }

        else {
            Debug.Log("false");
            return false; }
    }

    public void Update()
    {
        //Debug.Log("Update");
        if (IsInside() == true)
        {
            //Show text
            Debug.Log("Inside Collider");
            if (Input.GetGamepadButton(DEControllerButton.A) == KeyState.KEY_DOWN || Input.GetGamepadButton(DEControllerButton.A) == KeyState.KEY_REPEAT)
            {
                Debug.Log("Key pressed");
                SceneManager.LoadScene(nextRoomUID);
            }
        }
        //else
        //{
        //    Debug.Log("Outside");
        //}
    }
}
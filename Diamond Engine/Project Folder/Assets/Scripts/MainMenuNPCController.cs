using System;
using DiamondEngine;

public class MainMenuNPCController : DiamondComponent
{
	public GameObject point1 = null;
	public GameObject point2 = null;
	public GameObject point3 = null;
	public GameObject point4 = null;
	public GameObject point5 = null;
	public GameObject point6 = null;
	public GameObject point7 = null;
	public GameObject point8 = null;
	public GameObject point9 = null;
	public GameObject point10 = null;
	public GameObject point11 = null;

	public GameObject DinDjarin = null;
	public GameObject Grogu = null;
	public GameObject Bantha = null;
	public GameObject Skytrooper = null;

	public float speed = 0.25f;
	public float timer = 0;
	public int pointCount = 0;
	public int WhichNPC = 0;

	Vector3 toGoVector = null;
	Quaternion toRotateQuaternion = null;
	private GameObject[] pointArray;
	private GameObject[] NPCS;

	public void Awake()
	{
		toGoVector = point1.transform.localPosition;
		toRotateQuaternion = point1.transform.localRotation;

		pointArray = new GameObject[] { point1, point2, point3, point4, point5, point6, point7, point8, point9, point10, point11 };
		NPCS = new GameObject[] { DinDjarin, Grogu, Bantha, Skytrooper };
	}

	public void Update()
	{
		if (Mathf.Distance(NPCS[WhichNPC].transform.localPosition, toGoVector) < 0.2f)
		{
			//timer = 0;
			pointCount++;
			if (pointCount >= pointArray.Length)
            {
				pointCount = 0;
				SpawnOtherNPC();
			}

			toGoVector = pointArray[pointCount].transform.localPosition;
			toRotateQuaternion = pointArray[pointCount].transform.globalRotation;

			//speed = Mathf.Distance(NPCS[WhichNPC].transform.localPosition, toGoVector) / 10;

		}

		if (toGoVector != null)
		{
			NPCS[WhichNPC].transform.localPosition += (toGoVector - NPCS[WhichNPC].transform.localPosition).normalized * Time.deltaTime * speed;
			//NPCS[WhichNPC].transform.localRotation = Quaternion.Slerp(NPCS[WhichNPC].transform.localRotation, toRotateQuaternion, Time.deltaTime);
		}
		//timer += Time.deltaTime;
	}

	public void PutInPlaceNPCs()
    {
		DinDjarin.transform.localPosition = Vector3.zero;
		Bantha.transform.localPosition = Vector3.zero;
		Skytrooper.transform.localPosition = Vector3.zero;
		Grogu.transform.localPosition = Vector3.zero;
	}

	public void SpawnOtherNPC()
    {
		WhichNPC++;
		if(WhichNPC >= NPCS.Length)
        {
			WhichNPC = 0;
        }
		PutInPlaceNPCs();
		/*
		switch (WhichNPC)
		{
			case (0):
				DinDjarin.Enable(true);
				Grogu.Enable(false);
				Bantha.Enable(false);
				Skytrooper.Enable(false);
				break;
			case (1):
				DinDjarin.Enable(false);
				Grogu.Enable(true);
				Bantha.Enable(false);
				Skytrooper.Enable(false);
				break;
			case (2):
				DinDjarin.Enable(false);
				Grogu.Enable(false);
				Bantha.Enable(true);
				Skytrooper.Enable(false);
				break;
			case (3):
				DinDjarin.Enable(false);
				Grogu.Enable(false);
				Bantha.Enable(false);
				Skytrooper.Enable(true);
				break;
			default:
				DinDjarin.Enable(true);
				Grogu.Enable(false);
				Bantha.Enable(false);
				Skytrooper.Enable(false);
				break;
		}
		*/
	}
}
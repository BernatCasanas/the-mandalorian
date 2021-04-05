using System;
using DiamondEngine;
using System.Collections.Generic;

public class NPCSpawnController : DiamondComponent
{
    //NPC Spawn Controller
    private Dictionary<int, int> alreadyAppeared = new Dictionary<int, int>(); //<int,int> <number, timesAppeared>

    public GameObject HubTextController = null;

    public bool Grogu = true;
    public bool BoKatan = true;
    public bool Ahsoka = true;
    public bool CaraDune = true;
    private const int GroguUID = 1075538485;
    private const int BoKatanUID = 653387112;
    private const int AhsokaUID = 1082641369;
    private const int CaraDuneUID = 2028292522;
    private List<int> charactersUID = new List<int>();

    public GameObject spawnPoint1 = null;
    public int animation1 = 0;
    public GameObject spawnPoint2 = null;
    public int animation2 = 0;
    public GameObject spawnPoint3 = null;
    public int animation3 = 0;
    public GameObject spawnPoint4 = null;
    public int animation4 = 0;
    public GameObject spawnPoint5 = null;
    public int animation5 = 0;
    public GameObject spawnPoint6 = null;
    public int animation6 = 0;
    public GameObject spawnPoint7 = null;
    public int animation7 = 0;
    public GameObject spawnPoint8 = null;
    public int animation8 = 0;

    private List<Tuple<Vector3, Quaternion, int>> spawnPoints = new List<Tuple<Vector3, Quaternion, int>>();
 
    enum State
    {
        IDLE = 0,
        SIT,
        LEAN
    }

    bool start = true;
    public void Update()
    {
        //TODO: Move to Start function
        if (start)
        {
            GenerateCharactersList();
            GenerateSpawnPointsList();
            SpawnNPCs();
            start = false;
        }
    }

    private void SpawnNPCs()
    {
        //Get coordinates from a random spawnPoint
        //Generate a prefab on those coordinates
        if(charactersUID.Count > spawnPoints.Count)
        {
            Debug.Log("Error. There are more NPCs than spawn points available");
            return;
        }

        for (int i = 0; i < spawnPoints.Count; i++)//Initialize map
        {
            alreadyAppeared[i] = 0;
        }

        for (int i = 0; i < charactersUID.Count; i++)
        {
            Random randomizer = new Random();
            int randomIndex = randomizer.Next(spawnPoints.Count);

            do
            {
                randomIndex = randomizer.Next(spawnPoints.Count);
            } while (alreadyAppeared[randomIndex] > 0);
            alreadyAppeared[randomIndex]++;

            //We update the coordinates of the already existing character
            Debug.Log(spawnPoints[randomIndex].Item3.ToString());
            SpawnUnit(charactersUID[i], GetCoordinatesFromSpawnPoint(randomIndex), GetRotationFromSpawnPoint(randomIndex), spawnPoints[randomIndex].Item3);
        }
    }
    private void SpawnUnit(int prefabUID, Vector3 pos, Quaternion rot, int animation)
    {
        string prefabPath = "Library/Prefabs/";
        prefabPath += prefabUID;
        prefabPath += ".prefab";

        GameObject unit = InternalCalls.CreatePrefab(prefabPath, pos, rot, Vector3.one);
        //Animator animatorComponent = unit.GetComponent<Animator>();
        Debug.Log("Character: " + prefabUID + " , Animation: " + animation);
        switch(prefabUID)
        {
            case GroguUID:
                Animator.Play(unit, "Grogu_Idle");

                if(HubTextController != null) {
                    HubTextController.GetComponent<HubTextController>().grogu = unit;
                }

                break;
            case BoKatanUID:
                if (animation == 0) Animator.Play(unit, "BoKatan_Idle");
                else if (animation == 1) Animator.Play(unit, "BoKatan_Sit");
                else if (animation == 2) Animator.Play(unit, "BoKatan_Lean");

                if (HubTextController != null) { 
                    HubTextController.GetComponent<HubTextController>().bo_katan = unit;
                }

                break;
            case AhsokaUID:
                if (animation == 0) Animator.Play(unit, "Ahsoka_Idle");
                else if (animation == 1) Animator.Play(unit, "Ahsoka_Sit");
                else if (animation == 2) Animator.Play(unit, "Ahsoka_Lean");

                if (HubTextController != null)
                {
                    HubTextController.GetComponent<HubTextController>().ashoka = unit;
                }

                break;
            case CaraDuneUID:
                if (animation == 0) Animator.Play(unit, "Idle");
                else if (animation == 1) Animator.Play(unit, "Sit");
                else if (animation == 2) Animator.Play(unit, "Bar");
                break;
            default:
                break;
        }
    }

    private Vector3 GetCoordinatesFromSpawnPoint(int index)
    {
        if (spawnPoints[index] != null)
            return spawnPoints[index].Item1;
        Debug.Log("texto");
        Vector3 defaultVal = new Vector3(0,1,0);
        return defaultVal;
    }

    private Quaternion GetRotationFromSpawnPoint(int index)
    {
        if (spawnPoints[index] != null)
            return spawnPoints[index].Item2;

        Quaternion defaultVal = new Quaternion(0, 0, 0, 1);
        return defaultVal;
    }

    private void GenerateSpawnPointsList()
    {
        //Load spawnPoints position into a List or other data structure to handle information more efficiently
        Vector3 positionAux = new Vector3(1,1,1);
        Quaternion rotationAux = new Quaternion(0,0,0,1);
        Tuple<Vector3, Quaternion, int> TupleAux = new Tuple<Vector3, Quaternion, int>(positionAux, rotationAux, 1);

        if (spawnPoint1 != null)
        {
            positionAux = spawnPoint1.transform.globalPosition;
            rotationAux = spawnPoint1.transform.globalRotation;
            TupleAux = Tuple.Create(positionAux, rotationAux, animation1);
            spawnPoints.Add(TupleAux); 
        }
        if (spawnPoint2 != null)
        {
            positionAux = spawnPoint2.transform.globalPosition;
            rotationAux = spawnPoint2.transform.globalRotation;
            TupleAux = Tuple.Create(positionAux, rotationAux, animation2);
            spawnPoints.Add(TupleAux);
        }
        if (spawnPoint3 != null)
        {
            positionAux = spawnPoint3.transform.globalPosition;
            rotationAux = spawnPoint3.transform.globalRotation;
            TupleAux = Tuple.Create(positionAux, rotationAux, animation3);
            spawnPoints.Add(TupleAux);
        }
        if (spawnPoint4 != null)
        {
            positionAux = spawnPoint4.transform.globalPosition;
            rotationAux = spawnPoint4.transform.globalRotation;
            TupleAux = Tuple.Create(positionAux, rotationAux, animation4);
            spawnPoints.Add(TupleAux);
        }
        if (spawnPoint5 != null)
        {
            positionAux = spawnPoint5.transform.globalPosition;
            rotationAux = spawnPoint5.transform.globalRotation;
            TupleAux = Tuple.Create(positionAux, rotationAux, animation5);
            spawnPoints.Add(TupleAux);
        }
        if (spawnPoint6 != null)
        {
            positionAux = spawnPoint6.transform.globalPosition;
            rotationAux = spawnPoint6.transform.globalRotation;
            TupleAux = Tuple.Create(positionAux, rotationAux, animation6);
            spawnPoints.Add(TupleAux);
        }
        if (spawnPoint7 != null)
        {
            positionAux = spawnPoint7.transform.globalPosition;
            rotationAux = spawnPoint7.transform.globalRotation;
            TupleAux = Tuple.Create(positionAux, rotationAux, animation7);
            spawnPoints.Add(TupleAux);
        }
        if (spawnPoint8 != null)
        {
            positionAux = spawnPoint8.transform.globalPosition;
            rotationAux = spawnPoint8.transform.globalRotation;
            TupleAux = Tuple.Create(positionAux, rotationAux, animation8);
            spawnPoints.Add(TupleAux);
        }
    }

    private void GenerateCharactersList()
    {
        if (Grogu) charactersUID.Add(GroguUID); //Grogu's UID
        if (BoKatan) charactersUID.Add(BoKatanUID); //BoKatan's UID
        if (Ahsoka) charactersUID.Add(AhsokaUID); //Ahsoka's UID
        if (CaraDune) charactersUID.Add(CaraDuneUID); //CaraDune's UID
    }
}
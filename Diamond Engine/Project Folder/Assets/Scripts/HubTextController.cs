using System;
using DiamondEngine;


public class HubTextController : DiamondComponent
{
    public enum Interaction
    {
        NONE,
        BO_KATAN,
        GREEF,
        ASHOKA,
        GROGU
    }

    public GameObject textController = null;
    public GameObject dialog = null;
    public GameObject mando = null;
    public GameObject bo_katan = null;
    public GameObject greef = null;
    public GameObject ashoka = null;
    public GameObject grogu = null;

    public int bo_katan_portrait_uid = 0;
    public int greef_portrait_uid = 0;
    public int ashoka_portrait_uid = 0;
    public int grogu_portrait_uid = 0;

    public float bo_katan_portrait_pos_x = 0;
    public float bo_katan_portrait_pos_y = 0;
    public float bo_katan_portrait_size_x = 0;
    public float bo_katan_portrait_size_y = 0;

    public float greef_portrait_pos_x = 0;
    public float greef_portrait_pos_y = 0;
    public float greef_portrait_size_x = 0;
    public float greef_portrait_size_y = 0;

    public float ashoka_portrait_pos_x = 0;
    public float ashoka_portrait_pos_y = 0;
    public float ashoka_portrait_size_x = 0;
    public float ashoka_portrait_size_y = 0;

    public float grogu_portrait_pos_x = 0;
    public float grogu_portrait_pos_y = 0;
    public float grogu_portrait_size_x = 0;
    public float grogu_portrait_size_y = 0;


    public int total_interactions = 0;
    public int total_stages = 0;

    public float maximum_distance_to_interact_squared = 0.0f;

    private static int boKatanStage = DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadInt("boKatanStage") : 1;
    private static int greefStage = DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadInt("greefStage") : 1;
    private static int ashokaStage = DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadInt("ashokaStage") : 1;
    private static int groguStage = DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadInt("groguStage") : 1;

    private static int boKatanInteractionNum = DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadInt("boKatanInteractionNum") : 1;
    private static int greefInteractionNum = DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadInt("greefInteractionNum") : 1;
    private static int ashokaInteractionNum = DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadInt("ashokaInteractionNum") : 1;
    private static int groguInteractionNum = DiamondPrefs.ReadBool("loadData") ? DiamondPrefs.ReadInt("groguInteractionNum") : 1;

    private int total_interactions_and_stages = 0;
    private bool dialog_finished = false;

    private bool start = true;


    Interaction interaction = Interaction.NONE;

    public void Awake()
    {
        total_interactions_and_stages = total_stages * total_interactions;
        DiamondPrefs.Write("boKatanStage", boKatanStage);
        DiamondPrefs.Write("greefStage", greefStage);
        DiamondPrefs.Write("ashokaStage", ashokaStage);
        DiamondPrefs.Write("groguStage", groguStage);
        DiamondPrefs.Write("boKatanInteractionNum", boKatanInteractionNum);
        DiamondPrefs.Write("greefInteractionNum", greefInteractionNum);
        DiamondPrefs.Write("ashokaInteractionNum", ashokaInteractionNum);
        DiamondPrefs.Write("groguInteractionNum", groguInteractionNum);
    }
    public void Update()
    {
        if (start)
        {
            if (DiamondPrefs.ReadBool("loadData") == false)
                DiamondPrefs.Write("loadData", true);
            start = false;
        }
        if (mando == null || Input.GetGamepadButton(DEControllerButton.A) != KeyState.KEY_DOWN || textController == null || textController.GetComponent<TextController>().otherimage == null || dialog == null ||
            textController.IsEnabled() == false)
        {
            return;
        }

        if (dialog_finished)
        {
            dialog_finished = false;
            return;
        }
        interaction = Interaction.NONE;

        if (bo_katan != null)
        {
            if (mando.GetComponent<Transform>().globalPosition.DistanceNoSqrt(bo_katan.GetComponent<Transform>().globalPosition) < maximum_distance_to_interact_squared)
            {
                interaction = Interaction.BO_KATAN;
            }
        }

        if (interaction == Interaction.NONE && greef != null)
        {
            if (mando.GetComponent<Transform>().globalPosition.DistanceNoSqrt(greef.GetComponent<Transform>().globalPosition) < maximum_distance_to_interact_squared)
            {
                interaction = Interaction.GREEF;
            }
        }

        if (interaction == Interaction.NONE && ashoka != null)
        {
            if (mando.GetComponent<Transform>().globalPosition.DistanceNoSqrt(ashoka.GetComponent<Transform>().globalPosition) < maximum_distance_to_interact_squared)
            {
                interaction = Interaction.ASHOKA;
            }
        }

        if (interaction == Interaction.NONE && grogu != null)
        {
            if (mando.GetComponent<Transform>().globalPosition.DistanceNoSqrt(grogu.GetComponent<Transform>().globalPosition) < maximum_distance_to_interact_squared)
            {
                interaction = Interaction.GROGU;
            }
        }

        if (interaction == Interaction.NONE)
        {
            return;
        }


        switch (interaction)
        {
            case Interaction.BO_KATAN:
                if (bo_katan_portrait_uid != 0)
                {
                    textController.GetComponent<TextController>().otherimage.GetComponent<Image2D>().AssignLibrary2DTexture(bo_katan_portrait_uid);
                    textController.GetComponent<TextController>().otherimage.GetComponent<Transform2D>().lPos = new Vector3(bo_katan_portrait_pos_x, bo_katan_portrait_pos_y, 0);
                    textController.GetComponent<TextController>().otherimage.GetComponent<Transform2D>().size = new Vector3(bo_katan_portrait_size_x, bo_katan_portrait_size_y, 0);
                }
                textController.GetComponent<TextController>().dialog_index = boKatanInteractionNum;
                if (boKatanInteractionNum % 3 != 0)
                {
                    boKatanInteractionNum++;
                    DiamondPrefs.Write("boKatanInteractionNum", boKatanInteractionNum);
                }
                break;
            case Interaction.GREEF:
                if (greef_portrait_uid != 0)
                {
                    textController.GetComponent<TextController>().otherimage.GetComponent<Image2D>().AssignLibrary2DTexture(greef_portrait_uid);
                    textController.GetComponent<TextController>().otherimage.GetComponent<Transform2D>().lPos = new Vector3(greef_portrait_pos_x, greef_portrait_pos_y, 0);
                    textController.GetComponent<TextController>().otherimage.GetComponent<Transform2D>().size = new Vector3(greef_portrait_size_x, greef_portrait_size_y, 0);
                }
                textController.GetComponent<TextController>().dialog_index = (total_interactions_and_stages) + greefInteractionNum;
                if (greefInteractionNum % 3 != 0)
                {
                    greefInteractionNum++;
                    DiamondPrefs.Write("greefInteractionNum", greefInteractionNum);
                }
                break;
            case Interaction.ASHOKA:
                if (ashoka_portrait_uid != 0)
                {
                    textController.GetComponent<TextController>().otherimage.GetComponent<Image2D>().AssignLibrary2DTexture(ashoka_portrait_uid);
                    textController.GetComponent<TextController>().otherimage.GetComponent<Transform2D>().lPos = new Vector3(ashoka_portrait_pos_x, ashoka_portrait_pos_y, 0);
                    textController.GetComponent<TextController>().otherimage.GetComponent<Transform2D>().size = new Vector3(ashoka_portrait_size_x, ashoka_portrait_size_y, 0);
                }
                textController.GetComponent<TextController>().dialog_index = (total_interactions_and_stages * 2) + ashokaInteractionNum;
                if (ashokaInteractionNum % 3 != 0)
                {
                    ashokaInteractionNum++;
                    DiamondPrefs.Write("ashokaInteractionNum", ashokaInteractionNum);
                }
                break;
            case Interaction.GROGU:
                if (grogu_portrait_uid != 0)
                {
                    textController.GetComponent<TextController>().otherimage.GetComponent<Image2D>().AssignLibrary2DTexture(grogu_portrait_uid);
                    textController.GetComponent<TextController>().otherimage.GetComponent<Transform2D>().lPos = new Vector3(grogu_portrait_pos_x, grogu_portrait_pos_y, 0);
                    textController.GetComponent<TextController>().otherimage.GetComponent<Transform2D>().size = new Vector3(grogu_portrait_size_x, grogu_portrait_size_y, 0);
                }
                textController.GetComponent<TextController>().dialog_index = (total_interactions_and_stages * 4) + groguInteractionNum;
                if (groguInteractionNum % 2 != 0)
                {
                    groguInteractionNum++;
                    DiamondPrefs.Write("groguInteractionNum", groguInteractionNum);
                }
                break;
        }
        dialog_finished = true;
        textController.GetComponent<Navigation>().Select();
        dialog.Enable(true);
    }

    public void IncreaseStage(Interaction interaction_to_increase_stage)
    {
        switch (interaction_to_increase_stage)
        {
            case Interaction.BO_KATAN:
                if (boKatanStage <= total_stages)
                {
                    boKatanInteractionNum = (boKatanStage * total_interactions) + 1;
                    DiamondPrefs.Write("boKatanInteractionNum", boKatanInteractionNum);
                    ++boKatanStage;
                    DiamondPrefs.Write("boKatanStage", boKatanStage);
                }
                break;
            case Interaction.GREEF:
                if (greefStage <= total_stages)
                {
                    greefInteractionNum = (greefStage * total_interactions) + 1;
                    DiamondPrefs.Write("greefInteractionNum", greefInteractionNum);
                    ++greefStage;
                    DiamondPrefs.Write("greefStage", greefStage);
                }
                break;
            case Interaction.ASHOKA:
                if (ashokaStage <= total_stages)
                {
                    ashokaInteractionNum = (ashokaStage * total_interactions) + 1;
                    DiamondPrefs.Write("ashokaInteractionNum", ashokaInteractionNum);
                    ++ashokaStage;
                    DiamondPrefs.Write("ashokaStage", ashokaStage);
                }
                break;
            case Interaction.GROGU:
                if (groguStage <= total_stages)
                {
                    groguInteractionNum = (groguStage * (total_interactions - 1)) + 1;
                    DiamondPrefs.Write("groguInteractionNum", groguInteractionNum);
                    ++groguStage;
                    DiamondPrefs.Write("groguStage", groguStage);
                }
                break;
        }
    }
}
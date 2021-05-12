using System;
using System.Collections.Generic;
using DiamondEngine;

public class TextController : DiamondComponent
{
	public GameObject gui = null;
	public GameObject dialog = null;
	public GameObject text = null;
	public GameObject mandoimage = null;
	public GameObject otherimage = null;
	public GameObject list_of_dialogs = null;
	public int dialog_index = -1;


	private int index = -1;
	private bool gui_not_enabled = true;

	private List<String> texts;
	private List<bool> images;



	private bool startMenu = true;
	private bool finished = false;

	public void Awake()
    {
		if (gui == null)
			gui = InternalCalls.FindObjectWithName("HUD");
    }

	public void OnExecuteButton()
	{
		if (startMenu==true)
			return;
        if (++index < texts.Count)
        {
			if(text!=null)
				text.GetComponent<Text>().text = texts[index];
            if (images[index] == true)
            {
				if(mandoimage!=null)
					mandoimage.Enable(true);
				if(otherimage!=null)
					otherimage.Enable(false);
            }
            else
            {
				if(mandoimage!=null)
					mandoimage.Enable(false);
				if(otherimage!=null)
					otherimage.Enable(true);
            }

        }



        if (index >= texts.Count && gui_not_enabled)
        {
            index = -1;
			if(mandoimage!=null)
				mandoimage.Enable(true);
			if (otherimage != null)
				otherimage.Enable(true);
			if (text != null)
				text.GetComponent<Text>().text = " ";
			if (dialog != null)
				dialog.Enable(false);
            finished = true;
            if (gui != null)
            {
                gui.Enable(true);
                gui.GetComponent<HUD>().ResetCombo();
                gui_not_enabled = false;
            }
            Time.ResumeGame();
            //InternalCalls.Destroy(dialog);
        }
    }
    public void Update()
	{
		if (gui == null)
			gui = Core.instance.hud;

		if (startMenu == true && dialog != null && list_of_dialogs != null && dialog_index >= 0)
		{
			Debug.Log(dialog_index.ToString());

			if (gui != null)
			{
				gui.Enable(false);
			}
			startMenu = false;
			Time.PauseGame();
			texts = list_of_dialogs.GetComponent<List_Of_Dialogs>().GetListOfDialog((uint)dialog_index);
			images = list_of_dialogs.GetComponent<List_Of_Dialogs>().GetListOfOrder((uint)dialog_index);
		}
		else if (finished == true)
		{
			Debug.Log(finished.ToString());

			startMenu = true;
			finished = false;

		}


	}

}
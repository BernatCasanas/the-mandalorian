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
	public void OnExecuteButton()
    {
		if (++index < texts.Count)
		{

			text.GetComponent<Text>().text = texts[index];
			if (images[index] == true)
			{
				mandoimage.Enable(true);
				otherimage.Enable(false);
			}
			else
			{
				mandoimage.Enable(false);
				otherimage.Enable(true);
			}

		}

		if (index >= texts.Count && gui_not_enabled)
		{
			index = -1;
			mandoimage.Enable(true);
			otherimage.Enable(true);
			text.GetComponent<Text>().text = " ";
			dialog.Enable(false);
			finished = true;
            if (gui != null)
            {
				gui.Enable(true);
				gui.GetComponent<HUD>().ResetCombo();
				gui_not_enabled = false;
			}
			Time.ResumeGame();
		}
	}
	public void Update()
	{
		if (startMenu == true && dialog != null && list_of_dialogs != null && dialog_index >= 0)
        {
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
			startMenu = true;
			finished = false;

		}


	}

}
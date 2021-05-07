﻿using System;
using DiamondEngine;
using System.Collections.Generic;

public static class ConfigFunctionality
{
    private static GameObject displayText = null;

    public static void UpdateDisplayText()
    {
        Debug.Log("Inside UpdateDisplayText");
        Text txt = null;
        displayText = InternalCalls.FindObjectWithName("Window Mode Value");

        if (displayText != null)
            txt = displayText.GetComponent<Text>();

        if (txt == null)
        {
            displayText = null;
            Debug.Log("Couldn't get the Window Mode Value text component");
            return;
        }
        else
        {
            if (Config.GetWindowMode() == 1)
                txt.text = "Windowed";
            else if (Config.GetWindowMode() == 2)
                txt.text = "Full Screen";

            Debug.Log(txt.text);
        }
    }
}

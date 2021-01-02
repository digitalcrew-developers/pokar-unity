﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TipsScreenUIManagerTeenPatti : MonoBehaviour
{
    public void OnClickOnButton(string eventName)
    {
        SoundManager.instance.PlaySound(SoundType.Click);

        switch (eventName)
        {
            case "back":
                {

                    InGameUiManagerTeenPatti.instance.DestroyScreen(InGameScreensTeenPatti.Tips);
                   
                }
                break;

            default:
                {
                    Debug.LogError("Unhandled eventName found in MissionsUiManager = " + eventName);
                }
                break;
        }
    }
}

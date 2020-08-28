using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TipsScreenUIManager : MonoBehaviour
{
    public void OnClickOnButton(string eventName)
    {
        SoundManager.instance.PlaySound(SoundType.Click);

        switch (eventName)
        {
            case "back":
                {
                    
                        InGameUiManager.instance.DestroyScreen(InGameScreens.Tips);
                   
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

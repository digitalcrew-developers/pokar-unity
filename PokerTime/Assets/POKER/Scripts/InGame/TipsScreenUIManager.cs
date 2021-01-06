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
                    if (InGameUiManager.instance != null)
                        InGameUiManager.instance.DestroyScreen(InGameScreens.Tips);
                    else if (ClubInGameUIManager.instance != null)
                        ClubInGameUIManager.instance.DestroyScreen(InGameScreens.Tips);                   
                }
                break;

            //Case to close EV Chop Tips 
            case "closeEVChopTips":
                {
                    if(ClubInGameUIManager.instance != null)
                    {
                        ClubInGameUIManager.instance.DestroyScreen(InGameScreens.EVChopRules);
                    }
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

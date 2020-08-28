using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealTimeResultUiManager : MonoBehaviour
{

    public void OnClickOnButton(string eventName)
    {
        SoundManager.instance.PlaySound(SoundType.Click);

        switch (eventName)
        {
            case "back":
                {
                    InGameUiManager.instance.DestroyScreen(InGameScreens.RealTimeResult);
                }
                break;
          
            default:
                {
                    Debug.LogError("Unhandled eventName found in RealTimeResultUiManager = " + eventName);
                }
                break;
        }
    }
}

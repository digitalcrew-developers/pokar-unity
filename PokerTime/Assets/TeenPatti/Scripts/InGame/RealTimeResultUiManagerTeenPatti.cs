using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealTimeResultUiManagerTeenPatti : MonoBehaviour
{

    public void OnClickOnButton(string eventName)
    {
        SoundManager.instance.PlaySound(SoundType.Click);

        switch (eventName)
        {
            case "back":
                {
                    InGameUiManagerTeenPatti.instance.DestroyScreen(InGameScreensTeenPatti.RealTimeResult);
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

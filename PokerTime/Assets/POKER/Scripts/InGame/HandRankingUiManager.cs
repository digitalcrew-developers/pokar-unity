using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandRankingUiManager : MonoBehaviour
{
    public void OnClickOnButton(string eventName)
    {
        SoundManager.instance.PlaySound(SoundType.Click);

        switch (eventName)
        {
            case "back":
                {
                    InGameUiManager.instance.DestroyScreen(InGameScreens.HandRanking);
                }
                break;

            default:
                {
                    Debug.LogError("Unhandled eventName found in HandRankingUiManager = " + eventName);
                }
                break;
        }
    }
}

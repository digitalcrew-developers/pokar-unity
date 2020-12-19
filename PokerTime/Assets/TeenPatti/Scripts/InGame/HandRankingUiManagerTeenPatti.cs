using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandRankingUiManagerTeenPatti : MonoBehaviour
{
    public void OnClickOnButton(string eventName)
    {
        SoundManager.instance.PlaySound(SoundType.Click);

        switch (eventName)
        {
            case "back":
                {
                    InGameUiManagerTeenPatti.instance.DestroyScreen(InGameScreens.HandRanking);
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

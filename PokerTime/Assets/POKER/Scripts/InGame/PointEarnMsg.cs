using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointEarnMsg : MonoBehaviour
{
    public Text EarnedPointMsg;

    public void onClickCloseMSG()
    {
        if (InGameUiManager.instance != null)
            InGameUiManager.instance.DestroyScreen(InGameScreens.PointEarnMsg);
        else if (ClubInGameUIManager.instance != null)
            ClubInGameUIManager.instance.DestroyScreen(InGameScreens.PointEarnMsg);
    }

    public void OnOpen()
    {
        EarnedPointMsg.text = "Earned " + InGameManager.instance.PointEarnedCounter.ToString() + " Point(s)";
    }
}

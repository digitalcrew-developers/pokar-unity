using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointEarnMsg : MonoBehaviour
{
    public void onClickCloseMSG()
    {
        InGameUiManager.instance.DestroyScreen(InGameScreens.PointEarnMsg);
    }
}

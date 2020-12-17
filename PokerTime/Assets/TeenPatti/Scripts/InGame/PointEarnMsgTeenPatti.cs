using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointEarnMsgTeenPatti : MonoBehaviour
{
    public void onClickCloseMSG()
    {
        InGameUiManagerTeenPatti.instance.DestroyScreen(InGameScreens.PointEarnMsg);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CongratulationUIManager : MonoBehaviour
{
    public void OnPannelClick()
    {
        MainMenuController.instance.DestroyScreen(MainMenuScreens.Congratulation);
    }
}

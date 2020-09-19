using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CareerDefintionScreenManager : MonoBehaviour
{
    public void OnCloseBtnClick()
    {

        MainMenuController.instance.DestroyScreen(MainMenuScreens.CareerDefinationScreen);
    }

}

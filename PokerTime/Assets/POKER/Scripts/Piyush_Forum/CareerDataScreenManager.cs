using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CareerDataScreenManager : MonoBehaviour
{
    public void OnCloseBtnClick()
    {

        MainMenuController.instance.DestroyScreen(MainMenuScreens.CareerDataScreen);
    }

    public void OnShowCareerDefinationScreen()
    {

        MainMenuController.instance.ShowScreen(MainMenuScreens.CareerDefinationScreen);
    }
}

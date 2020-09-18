using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CareerManager : MonoBehaviour
{
    public void OnMenuBtnClick()
    {
        MainMenuController.instance.ShowScreen(MainMenuScreens.CareerMenuScreen);
    }
}

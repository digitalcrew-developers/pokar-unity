using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CareerMnuScreenManager : MonoBehaviour
{
    public void OnCloseBtnClick() {

        MainMenuController.instance.DestroyScreen(MainMenuScreens.CareerMenuScreen);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FairGaming : MonoBehaviour
{
   public void OnCloseFairGame()
    {
        MainMenuController.instance.DestroyScreen(MainMenuScreens.FairGaming);
    }
}

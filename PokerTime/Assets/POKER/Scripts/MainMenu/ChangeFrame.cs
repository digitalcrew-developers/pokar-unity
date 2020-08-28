using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeFrame : MonoBehaviour
{

    public void CloseBtnChanegFrame()
    {
        MainMenuController.instance.DestroyScreen(MainMenuScreens.ChangeFrame);
        //Destroy(this.gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContactManager : MonoBehaviour
{
    public void onClickContact(string name)
    {
        switch (name)
        {
            case "close":
                {
                    MainMenuController.instance.DestroyScreen(MainMenuScreens.Contact);
                }
                break;
            case "email":
                {
                    Application.OpenURL("https://www.lipsum.com/");
                }
                break;
            case "facebook":
                {
                    Application.OpenURL("https://www.lipsum.com/");
                }
                break;
            case "twitter":
                {
                    Application.OpenURL("https://www.lipsum.com/");
                }
                break;
            case "instagram":
                {
                    Application.OpenURL("https://www.lipsum.com/");
                }
                break;
        }
    }
}

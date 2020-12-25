using Org.BouncyCastle.Crypto.Engines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeenDashboardManager : MonoBehaviour
{
    public static TeenDashboardManager instance;

    private void Awake()
    {
        instance = this;   
    }

    public void OnClickOnButton(string eventName)
    {
        switch (eventName)
        {
            case "lobbyTeen":
                {
                    //MainMenuControllerTeen.instance.ShowScreen(MainMenuScreensTeen.LobbyTeenPatti);
                    MainMenuController.instance.ShowScreen(MainMenuScreens.LobbyTeenPatti);
                }
                break;

            case "mainMenuTeen":
                {
                    //MainMenuControllerTeen.instance.ShowScreen(MainMenuScreensTeen.MainMenuTeenPatti);
                    MainMenuController.instance.ShowScreen(MainMenuScreens.MainMenuTeenPatti);
                }
                break;
            case "playPoker":
                {
                    //GlobalGameManager.instance.LoadScene(Scenes.MainMenu);
                    MainMenuController.instance.OnClickPlayPoker();
                }
                break;
        }
    }
}
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
                    MainMenuController.instance.ShowScreen(MainMenuScreens.LobbyTeenPatti);
                }
                break;

            case "mainMenuTeen":
                {
                    MainMenuController.instance.ShowScreen(MainMenuScreens.MainMenuTeenPatti);
                }
                break;
        }
    }
}
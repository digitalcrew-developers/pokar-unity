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
                    MainMenuControllerTeen.instance.ShowScreen(MainMenuScreensTeen.Lobby);
                }
                break;

            case "mainMenuTeen":
                {
                    //MainMenuControllerTeen.instance.ShowScreen(MainMenuScreensTeen.MainMenuTeenPatti);
                    //Debug.Log("Showing Screen MainMenuTeen...");
                    MainMenuControllerTeen.instance.ShowScreen(MainMenuScreensTeen.MainMenu);
                }
                break;
            case "playPoker":
                {
                    GameConstants.poker = true;
                    GlobalGameManager.instance.LoadScene(Scenes.MainMenu);
                    //MainMenuControllerTeen.instance.OnClickPlayPoker();
                }
                break;
        }
    }
}
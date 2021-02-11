using Org.BouncyCastle.Crypto.Engines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeenDashboardManager : MonoBehaviour
{
    public static TeenDashboardManager instance;
    public Text coinsText ,diamondsText ,pointsText;
    private void Awake()
    {
        instance = this;   
    }
    private void Start()
    {
        UpdateAllText();
    }
    public void UpdateAllText()
    {
        PlayerGameDetails playerData = PlayerManager.instance.GetPlayerGameData();
        Debug.LogError("Data =====" + playerData.coins);
        coinsText.text = Utility.GetTrimmedAmount("" + playerData.coins);
        diamondsText.text = Utility.GetTrimmedAmount("" + playerData.diamonds);
        pointsText.text = Utility.GetTrimmedAmount("" + playerData.points);
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
                    Debug.LogError("Showing Screen MainMenuTeen...");
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
using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TournamentWinnerManager : MonoBehaviour
{
    public static TournamentWinnerManager instance;

    public Text rankingText, tournamentName;

    [Header("Player Data")]
    public Text playerName;
    public Text playerId;
    //public Text

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void Initialize(string serverResponse)
    {
        Debug.Log("Inside Winner Screen: " + serverResponse);
        JsonData data = JsonMapper.ToObject(serverResponse);

        rankingText.text = "Ranking " + data[0]["players"][0]["rank"].ToString() + "/170";
        playerName.text = data[0]["players"][0]["nickName"].ToString();
        playerId.text = "ID: " + data[0]["players"][0]["userId"].ToString();
    }

    public void OnExit()
    {
        if (TournamentInGameUiManager.instance != null)
        {
            TournamentInGameManager.instance.LoadMainMenu();
        }
    }
}

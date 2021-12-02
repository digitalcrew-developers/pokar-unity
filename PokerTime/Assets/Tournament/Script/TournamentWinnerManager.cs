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

    public void Initialize(/*string serverResponse*/)
    {
        //Debug.Log("Inside Winner Screen: " + serverResponse);

        playerName.text = PlayerManager.instance.GetPlayerGameData().userName;
        playerId.text = "ID: " + PlayerManager.instance.GetPlayerGameData().userId;

        //if (serverResponse.Length > 0)
        //{
        //    JsonData data = JsonMapper.ToObject(serverResponse);

        //    //rankingText.text = "Ranking " + data[0]["players"][0]["rank"].ToString() + "/170";
        //    playerName.text = data[0]["players"][0]["nickName"].ToString();
        //    playerId.text = "ID: " + data[0]["players"][0]["userId"].ToString();
        //}

        string requestData = "{\"tourneyId\":\"" + TournamentSocketController.instance.TOURNEY_ID + "\"," +
                             "\"userId\":\"" + PlayerManager.instance.GetPlayerGameData().userId + "\"}";
        StartCoroutine(WebServices.instance.POSTRequestData("http://3.109.177.149:3335/tournament/details", requestData, OnGetTourneyDetailsComplete));
    }

    private void OnGetTourneyDetailsComplete(string serverResponse, bool isErrorMessage, string errorMessage)
    {
        if (isErrorMessage)
        {
            Debug.Log("error=" + errorMessage);
        }
        else
        {
            Debug.Log("Response => OnGetStreamDetails: " + serverResponse);

            JsonData jsonData = JsonMapper.ToObject(serverResponse);

            IDictionary dictionary = jsonData;      //DEV_CODE Converting to dictionary for checking key availability

            if (jsonData["status"].Equals(true))
            {
                rankingText.text = "Ranking " + jsonData["data"]["myRank"].ToString() + "/" + jsonData["data"]["ranking"].Count.ToString();
            }
        }
    }
    public void OnExit()
    {
        if (TournamentInGameUiManager.instance != null)
        {
            TournamentInGameManager.instance.LoadMainMenu();
        }
    }
}

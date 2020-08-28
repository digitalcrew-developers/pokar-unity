using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopPlayerUIManager : MonoBehaviour
{

    public GameObject TopPlayerContentPrefab;
    public Transform container;

    public Text myRankTxt;
    public Text usernameTxt;
    public Text scoreLabelTxt;

    public Image[] onfocusImageAry;
    public Texture2D loadingSpr, errorSpr;

    string playerTypeVal;

    void Start()
    {
        playerTypeVal = "LOBBY";
        GetLobbyTopPlayerList(true);
    }

    public void GetLobbyTopPlayerList(bool isShowLoading)
    {
        playerTypeVal = "LOBBY";
        scoreLabelTxt.text = "Winning";
        ChangeBtnFocus(0);
        string requestData = "{\"userId\":\"" + PlayerManager.instance.GetPlayerGameData().userId + "\"," +
                            "\"PlayersType\":\"" + playerTypeVal + "\"," +
                             "\"pageSize\":\"" + 100 + "\"}";

        if (isShowLoading)
        {
            MainMenuController.instance.ShowScreen(MainMenuScreens.Loading);
        }

        WebServices.instance.SendRequest(RequestType.GetTopPlayers, requestData, true, OnServerResponseFound);
    }


    public void GetMTTTopPlayerList(bool isShowLoading)
    {
        playerTypeVal = "MTT";
        scoreLabelTxt.text = "Score";
        ChangeBtnFocus(1);
        string requestData = "{\"userId\":\"" + PlayerManager.instance.GetPlayerGameData().userId + "\"," +
                            "\"PlayersType\":\"" + playerTypeVal + "\"," +
                             "\"pageSize\":\"" + 100 + "\"}";

        if (isShowLoading)
        {
            MainMenuController.instance.ShowScreen(MainMenuScreens.Loading);
        }

        WebServices.instance.SendRequest(RequestType.GetTopPlayers, requestData, true, OnServerResponseFound);
    }


    public void GetClubTopPlayerList(bool isShowLoading)
    {
        playerTypeVal = "CLUB";
        scoreLabelTxt.text = "Score";
        ChangeBtnFocus(2);
        string requestData = "{\"userId\":\"" + PlayerManager.instance.GetPlayerGameData().userId + "\"," +
                            "\"PlayersType\":\"" + playerTypeVal + "\"," +
                             "\"pageSize\":\"" + 100 + "\"}";

        if (isShowLoading)
        {
            MainMenuController.instance.ShowScreen(MainMenuScreens.Loading);
        }

        WebServices.instance.SendRequest(RequestType.GetTopPlayers, requestData, true, OnServerResponseFound);
    }


    public void GetBankRollTopPlayerList(bool isShowLoading)
    {
        playerTypeVal = "BANKROLL";
        scoreLabelTxt.text = "Coins";
        ChangeBtnFocus(3);
        string requestData = "{\"userId\":\"" + PlayerManager.instance.GetPlayerGameData().userId + "\"," +
                            "\"PlayersType\":\"" + playerTypeVal + "\"," +
                             "\"pageSize\":\"" + 100 + "\"}";

        if (isShowLoading)
        {
            MainMenuController.instance.ShowScreen(MainMenuScreens.Loading);
        }

        WebServices.instance.SendRequest(RequestType.GetTopPlayers, requestData, true, OnServerResponseFound);
    }
    void ChangeBtnFocus(int focusVal)
    {
        for (int i = 0; i < onfocusImageAry.Length; i++)
        {
            if (i != focusVal)
            {
                Color temp = onfocusImageAry[i].color;
                temp.a = 0.01f;
                onfocusImageAry[i].color = temp;
            }
            else
            {
                Color temp = onfocusImageAry[i].color;
                temp.a = 1f;
                onfocusImageAry[i].color = temp;
            }
        }

    }

    public void OnServerResponseFound(RequestType requestType, string serverResponse, bool isShowErrorMessage, string errorMessage)
    {
        MainMenuController.instance.DestroyScreen(MainMenuScreens.Loading);

        if (errorMessage.Length > 0)
        {
            if (isShowErrorMessage)
            {
                MainMenuController.instance.ShowMessage(errorMessage);
            }

            return;
        }

        if (requestType == RequestType.GetTopPlayers)
        {
            JsonData data = JsonMapper.ToObject(serverResponse);

            if (data["success"].ToString() == "1")
            {
                ShowForumList(data);
            }
            else
            {
                MainMenuController.instance.ShowMessage(data["message"].ToString());
                for (int i = 0; i < container.childCount; i++)
                {
                    Destroy(container.GetChild(i).gameObject);
                }
            }
        }
    }

    private void ShowForumList(JsonData data)
    {
        for (int i = 0; i < container.childCount; i++)
        {
            Destroy(container.GetChild(i).gameObject);
        }

        for (int i = 0; i < data["getData"][0]["TopPlayers"].Count; i++)
        {

            GameObject gm1 = Instantiate(TopPlayerContentPrefab, container) as GameObject;


            switch (i)
            {
                case 0:
                    gm1.GetComponent<TopPlayerUIContent>().rankTxt.text = (i + 1).ToString();
                    GameObject g = Instantiate(gm1.GetComponent<TopPlayerUIContent>().topRankImagePrefabs[0], gm1.GetComponent<TopPlayerUIContent>().rankTxt.transform) as GameObject;
                    break;
                case 1:
                    gm1.GetComponent<TopPlayerUIContent>().rankTxt.text = (i + 1).ToString();
                    GameObject g1 = Instantiate(gm1.GetComponent<TopPlayerUIContent>().topRankImagePrefabs[1], gm1.GetComponent<TopPlayerUIContent>().rankTxt.transform) as GameObject;

                    break;
                case 2:
                    gm1.GetComponent<TopPlayerUIContent>().rankTxt.text = (i + 1).ToString();
                    GameObject g2 = Instantiate(gm1.GetComponent<TopPlayerUIContent>().topRankImagePrefabs[2], gm1.GetComponent<TopPlayerUIContent>().rankTxt.transform) as GameObject;

                    break;
                default:
                    gm1.GetComponent<TopPlayerUIContent>().rankTxt.text = (i + 1).ToString();
                    break;
            }
            gm1.GetComponent<TopPlayerUIContent>().nameTxt.text = data["getData"][0]["TopPlayers"][i]["userName"].ToString();
           
                
            if (data["getData"][0]["TopPlayers"][i]["frameURL"] != null)
            {
                Debug.Log("Here IMAGE is COME   " + data["getData"][0]["TopPlayers"][i]["frameURL"].ToString());
                Davinci.get()
            .load(data["getData"][0]["TopPlayers"][i]["frameURL"].ToString())
            .setLoadingPlaceholder(loadingSpr)
            .setErrorPlaceholder(errorSpr)
            .setCached(false)
            .into(gm1.GetComponent<TopPlayerUIContent>().nameImage)
            .start();
            }

            switch (playerTypeVal)
            {
                case "LOBBY":
                    gm1.GetComponent<TopPlayerUIContent>().goldTxt.text = data["getData"][0]["TopPlayers"][i]["TotalWinnings"].ToString();

                    break;
                case "MTT":
                    gm1.GetComponent<TopPlayerUIContent>().goldTxt.text = data["getData"][0]["TopPlayers"][i]["TotalScore"].ToString();

                    break;
                case "CLUB":
                    gm1.GetComponent<TopPlayerUIContent>().goldTxt.text = data["getData"][0]["TopPlayers"][i]["TotalWinnings"].ToString();

                    break;
                case "BANKROLL":
                    gm1.GetComponent<TopPlayerUIContent>().goldTxt.text = data["getData"][0]["TopPlayers"][i]["TotalCoins"].ToString();

                    break;

            }
            Debug.Log("Here the Count of value  ----   " + data["getData"][1]["MyRank"].ToString());

            myRankTxt.text = "My Rank " + data["getData"][1]["MyRank"].ToString();
            usernameTxt.text = PlayerManager.instance.GetPlayerGameData().userName;
        }
    }
    public void OnClickOnButton(string eventName)
    {
        SoundManager.instance.PlaySound(SoundType.Click);

        switch (eventName)
        {
            case "back":
                {
                    MainMenuController.instance.ShowScreen(MainMenuScreens.Lobby);
                }
                break;

            default:
                {
                    Debug.LogError("Unhandled eventName found in MissionsUiManager = " + eventName);
                }
                break;
        }
    }


   // public Texture2D loadingSpr, errorSpr;

    



}

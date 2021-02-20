﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using LitJson;
using System;
using UnityEngine.Networking;

public class ExchangeChipsTeen : MonoBehaviour
{
    public TextMeshProUGUI TotalDiamonds;
    public /*TMP_InputField*/InputField Diamonds;
    public /*TMP_InputField*/InputField PTChips;

    public Button ConfirmButton;

    public static ExchangeChipsTeen instance;

    public Button PTChipsTabButton, DiamondTabButton;
    public GameObject DiamondPanel, PTPanel;

    private float diamondsToConvert = 0;

    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
        }
    }

    private void OnEnable()
    {
        Init();
    }

    public void Init()
    {
        TotalDiamonds.text = PlayerManager.instance.GetPlayerGameData().diamonds.ToString();
        
        DiamondTabButton.onClick.RemoveAllListeners();
        PTChipsTabButton.onClick.RemoveAllListeners();
        PTChipsTabButton.onClick.AddListener(() => OpenScreen("PTChips"));
        DiamondTabButton.onClick.AddListener(() => OpenScreen("Diamond"));

        OpenScreen("PTChips");

        ConfirmButton.onClick.RemoveAllListeners();
        ConfirmButton.onClick.AddListener(AddPTChips);
    }

    private void AddPTChips()
    {
        float.TryParse(Diamonds.text, out diamondsToConvert);

        if (diamondsToConvert > PlayerManager.instance.GetPlayerGameData().diamonds)
        {
            //StartCoroutine(Showp)
            Debug.Log("Insufficient Diamonds...");
            Diamonds.text = "";
        }
        else
        {
            float chips = diamondsToConvert * 3;
            PTChips.text = chips.ToString();
            Debug.Log("Ready to convert..." + PTChips + " Chips");

            string userID = PlayerManager.instance.GetPlayerGameData().userId;
            string clubID = ClubDetailsUIManagerTeen.instance.GetClubId();
            
            string request = "{\"userId\":\"" + userID + "\"," +
                            "\"clubId\":\"" + clubID + "\"," +
                            "\"chips\":\"" + PTChips.text + "\"}";

            WebServices.instance.SendRequestTP(RequestTypeTP.AddPTChips, request, true, OnServerResponseFound);
        }
    }

    private void OpenScreen(string screenName)
    {
        Color c = new Color(1, 1, 1, 1);
        Color c1 = new Color(1, 1, 1, 0);

        switch (screenName)
        {
            case "PTChips":
                PTChipsTabButton.GetComponent<Image>().color = c;
                DiamondTabButton.GetComponent<Image>().color = c1;

                PTPanel.SetActive(true);
                DiamondPanel.SetActive(false);
                break;
            case "Diamond":
                PTChipsTabButton.GetComponent<Image>().color = c1;
                DiamondTabButton.GetComponent<Image>().color = c;

                DiamondPanel.SetActive(true);
                PTPanel.SetActive(false);
                break;
            default:
                break;
        }
    }

    private void UpdatePlayerDiamonds(PlayerGameDetails updatedData)
    {
        Debug.Log("Available Diamonds: " + PlayerManager.instance.GetPlayerGameData().diamonds);


        string requestData = "{\"userId\":\"" + PlayerManager.instance.GetPlayerGameData().userId + "\"," +
            "\"silver\":\"0\"," +
            "\"coins\":\"" + (int)updatedData.coins + "\"," +
            "\"points\":\"" + (int)updatedData.points + "\"," +
            "\"diamond\":\"" + (int)updatedData.diamonds+ "\"," +

            "\"rabbit\":\"0\"," +
            "\"emoji\":\"0\"," +
            "\"time\":\"0\"," +
            "\"day\":\"0\"," +
            "\"playerProgress\":\"\"}";

        WebServices.instance.SendRequest(RequestType.UpdateUserBalance, requestData, true, (requestType, serverResponse, isShowErrorMessage, errorMessage) =>
        {
            //MainMenuController.instance.DestroyScreen(MainMenuScreens.Loading);

            if (errorMessage.Length > 0)
            {
                MainMenuControllerTeen.instance.ShowMessage(errorMessage);
            }
            else
            {
                JsonData data = JsonMapper.ToObject(serverResponse);
                if (data["status"].Equals(true))
                {
                    PlayerManager.instance.SetPlayerGameData(updatedData);
                    
                    if (MenuHandllerTeen.instance != null)
                    {
                        MenuHandllerTeen.instance.UpdateAllText();
                    }
                }
                else
                {
                    MainMenuControllerTeen.instance.ShowMessage(data["message"].ToString());
                    if (MenuHandllerTeen.instance != null)
                    {
                        MenuHandllerTeen.instance.UpdateAllText();
                    }
                }
            }
        });

        //int id = 1;
        ////string userId = MemberListUIManager.instance.GetClubOwnerObject().userId;
        //string userId = PlayerManager.instance.GetPlayerGameData().userId;
        //int userIdInt = 0;

        //int.TryParse(userId, out userIdInt);

        //string clubID = ClubDetailsUIManager.instance.GetClubId();
        //int clubIdInt = 0;

        //int.TryParse(clubID, out clubIdInt);

        //string request = "{\"userId\":\"" + userIdInt + "\"," +
        //                "\"clubId\":\"" + clubIdInt + "\"," +
        //                "\"uniqueClubId\":\"" + ClubDetailsUIManager.instance.GetClubUniqueId() + "\"," +
        //                "\"clubStatus\":\"" + id + "\"}";

        //WebServices.instance.SendRequest(RequestType.GetClubDetails, request, true, OnServerResponseFound);
    }    

    public void OnServerResponseFound(RequestTypeTP requestType, string serverResponse, bool isShowErrorMessage, string errorMessage)
    {
        //Debug.Log(serverResponse);
        //MainMenuController.instance.DestroyScreen(MainMenuScreens.Loading);

        if (errorMessage.Length > 0)
        {
            if (isShowErrorMessage)
            {
                MainMenuControllerTeen.instance.ShowMessage(errorMessage);
            }

            return;
        }

        switch (requestType)
        {
            case RequestTypeTP.GetClubDetails:
                {
                    Debug.Log("Response => GetClubDetails(Exchange Chips) : " + serverResponse);
                    JsonData data = JsonMapper.ToObject(serverResponse);
                    string chipsText = data["data"][0]["ptChips"].ToString();
                    TotalDiamonds.text = chipsText;
                }
                break;
            case RequestTypeTP.AddPTChips:
                {
                    Debug.Log("Response => AddPTChips: " + serverResponse);
                    PlayerGameDetails playerData = PlayerManager.instance.GetPlayerGameData();
                    playerData.diamonds -= diamondsToConvert;
                    UpdatePlayerDiamonds(playerData);
                    Diamonds.text = "";
                    PTChips.text = "";
                    gameObject.SetActive(false);
                }
                break;
            default:
#if ERROR_LOG
			Debug.LogError("Unhandled requestType found in  MenuHandller = "+requestType);
#endif
                break;
        }
    }
}

[Serializable]
public class ClubDetailsAPITeen
{
    public int userId;
    public int clubId;
    public string uniqueClubId;
    public int clubStatus;
}

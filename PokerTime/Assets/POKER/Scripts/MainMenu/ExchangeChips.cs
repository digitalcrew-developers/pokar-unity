using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using LitJson;
using System;
using UnityEngine.Networking;

public class ExchangeChips : MonoBehaviour
{
    public TextMeshProUGUI ChipsCount;
    public TMP_InputField Diamonds;
    public TMP_InputField PPChips;

    public Button ConfirmButton;

    public static ExchangeChips instance;

    public Button PTChipsTabButton, DiamondTabButton;
    public GameObject DiamondPanel, PTPanel;

    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
        }
    }

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        Diamonds.text = PlayerManager.instance.GetPlayerGameData().diamonds.ToString();
        Diamonds.enabled = false;
        
        GetChips();
        DiamondTabButton.onClick.RemoveAllListeners();
        PTChipsTabButton.onClick.RemoveAllListeners();
        PTChipsTabButton.onClick.AddListener(() => OpenScreen("PTChips"));
        DiamondTabButton.onClick.AddListener(() => OpenScreen("Diamond"));

        ConfirmButton.onClick.RemoveAllListeners();
        ConfirmButton.onClick.AddListener(AddPTChips);
    }

    private void AddPTChips()
    {
        string userID = PlayerManager.instance.GetPlayerGameData().userId;
        string clubID = ClubDetailsUIManager.instance.GetClubId();
        string chipsCount = PPChips.text;

        string request = "{\"userId\":\"" + userID + "\"," +
                        "\"clubId\":\"" + clubID + "\"," +
                        "\"chips\":\"" + chipsCount + "\"}";

        WebServices.instance.SendRequest(RequestType.AddPTChips, request, true, OnServerResponseFound);

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

    private void GetChips()
    {
        int id = 1;
        //string userId = MemberListUIManager.instance.GetClubOwnerObject().userId;
        string userId = PlayerManager.instance.GetPlayerGameData().userId;
        int userIdInt = 0;

        int.TryParse(userId, out userIdInt);

        string clubID = ClubDetailsUIManager.instance.GetClubId();
        int clubIdInt = 0;

        int.TryParse(clubID, out clubIdInt);

        string request = "{\"userId\":\"" + userIdInt + "\"," +
                        "\"clubId\":\"" + clubIdInt + "\"," +
                        "\"uniqueClubId\":\"" + ClubDetailsUIManager.instance.GetClubUniqueId() + "\"," +
                        "\"clubStatus\":\"" + id + "\"}";

        WebServices.instance.SendRequest(RequestType.GetClubDetails, request, true, OnServerResponseFound);
    }    

    public void OnServerResponseFound(RequestType requestType, string serverResponse, bool isShowErrorMessage, string errorMessage)
    {
        Debug.Log(serverResponse);
        MainMenuController.instance.DestroyScreen(MainMenuScreens.Loading);

        if (errorMessage.Length > 0)
        {
            if (isShowErrorMessage)
            {
                MainMenuController.instance.ShowMessage(errorMessage);
            }

            return;
        }

        switch (requestType)
        {
            case RequestType.GetClubDetails:
                {
                    JsonData data = JsonMapper.ToObject(serverResponse);
                    string chipsText = data["data"][0]["ptChips"].ToString();
                    ChipsCount.text = chipsText;
                }
                break;
            case RequestType.AddPTChips:
                {
                    GetChips();
                    ClubDetailsUIManager.instance.GetChips();
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
public class ClubDetailsAPI
{
    public int userId;
    public int clubId;
    public string uniqueClubId;
    public int clubStatus;
}

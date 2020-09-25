using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClubCounter : MonoBehaviour
{
    public static ClubCounter instance;

    public Button StatsBtn;
    public Button TabTradeBtn, TabSleepModeBtn, TabVIPBtn, TabTicketBtn;
    public GameObject TradePanel, SleepPanel, VIPPanel, TicketPanel;

    public Button SendOutBtn, ClaimBackBtn, SetLimitBtn, RemoveLimitBtn, SendVIPBtn, AddTicketBtn;

    public GameObject TradeListItemPrefab, SleepListItemPrefab, SendVIPListItemPrefab, TicketItemListPrefab;
    public Transform TradeListContent, SleepListContent, SendVIPContent, TicketContent;

    public GameObject SendOutPanel;
    public Button ConfirmChipsSendButton;

    public List<TMPro.TextMeshProUGUI> MemberCountTexts;
    public TextMeshProUGUI ClubChipsCount, ClubOwnerChipCount, RestMemberChipCount;

    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
        }
    }

    private void Start()
    {
        Initialise();
    }

    private void Initialise()
    {
        TabTradeBtn.onClick.RemoveAllListeners();
        TabSleepModeBtn.onClick.RemoveAllListeners();
        TabVIPBtn.onClick.RemoveAllListeners();
        TabTicketBtn.onClick.RemoveAllListeners();

        TabTradeBtn.onClick.AddListener(() => OpenScreen("Trade"));
        TabSleepModeBtn.onClick.AddListener(() => OpenScreen("SleepMode"));
        TabVIPBtn.onClick.AddListener(() => OpenScreen("VIP"));
        TabTicketBtn.onClick.AddListener(() => OpenScreen("Ticket"));

        GetMembersListFromServer();

        SendOutBtn.onClick.RemoveAllListeners();
        SendOutBtn.onClick.AddListener(OpenSendOutPanel);

        ConfirmChipsSendButton.onClick.RemoveAllListeners();
        ConfirmChipsSendButton.onClick.AddListener(SendChipsAPIRequest);
    }

    

    private void GetMembersListFromServer()
    {
        string requestData = "{\"clubId\":\"" + ClubDetailsUIManager.instance.GetClubId() + "\"}";
        //old member list
        WebServices.instance.SendRequest(RequestType.GetClubMemberList, requestData, true, OnServerResponseFound);
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

        if(requestType == RequestType.SendChipsOut)
        {
            JsonData data = JsonMapper.ToObject(serverResponse);
            if (data["success"].Equals(1))
            {
                CloseSendOutPanel();
            }
            else
            {
                MainMenuController.instance.ShowMessage(data["message"].ToString());
            }
        }

        if (requestType == RequestType.GetClubMemberList)
        {
            JsonData data = JsonMapper.ToObject(serverResponse);

            if (data["status"].Equals(true))
            {
                AddToAllLists(data);
            }
            else
            {
                //MainMenuController.instance.ShowMessage(data["message"].ToString());
            }
        }
        else
        {

#if ERROR_LOG
            Debug.LogError("Unhandled server requestType found  " + requestType);
#endif

        }
    }

    private List<ClubMemberDetails> SelectedTradeMembers = new List<ClubMemberDetails>();
    private List<ClubMemberDetails> SelectedSleepMembers = new List<ClubMemberDetails>();
    private ClubMemberDetails LastSelectedVIPMember;

    private List<GameObject> ScrollItems = new List<GameObject>();

    private void CleanAll()
    {
        if (ScrollItems.Count > 0)
        {
            foreach(GameObject g in ScrollItems)
            {
                Destroy(g);
            }
        }
    }

    private void AddToAllLists(JsonData data)
    {
        CleanAll();

        foreach(TMPro.TextMeshProUGUI t in MemberCountTexts)
        {
            t.text = "Member: " + data["data"].Count.ToString();
        }

        for (int i = 0; i < data["data"].Count; i++)
        {
            ClubMemberDetails clubMemberDetails = new ClubMemberDetails();
            clubMemberDetails.userId = data["data"][i]["requestUserId"].ToString();
            clubMemberDetails.userName = data["data"][i]["requestUserName"].ToString();
            clubMemberDetails.clubRequestId = data["data"][i]["clubRequestId"].ToString();
            clubMemberDetails.ptChips = data["data"][i]["ptChips"].ToString();

            string initial = clubMemberDetails.userName.ToUpper();
            initial = initial.Substring(0, 2);

            GameObject tardeItem = Instantiate(TradeListItemPrefab, TradeListContent) as GameObject;
            tardeItem.SetActive(true);

            tardeItem.transform.Find("TextName").GetComponent<TMPro.TextMeshProUGUI>().text = clubMemberDetails.userName;
            tardeItem.transform.Find("TextId").GetComponent<TMPro.TextMeshProUGUI>().text = "ID : " + clubMemberDetails.userId;
            tardeItem.transform.Find("TextNickname").GetComponent<TMPro.TextMeshProUGUI>().text = "Nickname : " + clubMemberDetails.nickName;
            tardeItem.transform.Find("Image/Coins").GetComponent<TMPro.TextMeshProUGUI>().text = clubMemberDetails.ptChips;  
            tardeItem.transform.Find("Image/Text (TMP)").GetComponent<TMPro.TextMeshProUGUI>().text = initial;

            tardeItem.transform.Find("Toggle").GetComponent<Toggle>().onValueChanged.AddListener(delegate {
                ToggleValueChangedTradeList(tardeItem.transform.Find("Toggle").GetComponent<Toggle>(), clubMemberDetails);
            });
            ScrollItems.Add(tardeItem);

            GameObject sleepItem = Instantiate(SleepListItemPrefab, SleepListContent) as GameObject;
            sleepItem.SetActive(true);

            sleepItem.transform.Find("TextName").GetComponent<TMPro.TextMeshProUGUI>().text = clubMemberDetails.userName;
            sleepItem.transform.Find("TextId").GetComponent<TMPro.TextMeshProUGUI>().text = "ID : " + clubMemberDetails.userId;
            sleepItem.transform.Find("TextNickname").GetComponent<TMPro.TextMeshProUGUI>().text = "Nickname : " + clubMemberDetails.nickName;
            sleepItem.transform.Find("Image/Coins").GetComponent<TMPro.TextMeshProUGUI>().text = clubMemberDetails.ptChips;
            sleepItem.transform.Find("Image/Text (TMP)").GetComponent<TMPro.TextMeshProUGUI>().text = initial;

            sleepItem.transform.Find("Toggle").GetComponent<Toggle>().onValueChanged.AddListener(delegate {
                ToggleValueChangedSleep(sleepItem.transform.Find("Toggle").GetComponent<Toggle>(), clubMemberDetails);
            });
            ScrollItems.Add(sleepItem);

            GameObject vipItem = Instantiate(SendVIPListItemPrefab, SendVIPContent) as GameObject;
            vipItem.SetActive(true);

            vipItem.transform.Find("TextName").GetComponent<TMPro.TextMeshProUGUI>().text = clubMemberDetails.userName;
            vipItem.transform.Find("TextId").GetComponent<TMPro.TextMeshProUGUI>().text = "ID : " + clubMemberDetails.userId;
            vipItem.transform.Find("TextNickname").GetComponent<TMPro.TextMeshProUGUI>().text = "Nickname : " + clubMemberDetails.nickName;
            vipItem.transform.Find("Image/Text (TMP)").GetComponent<TMPro.TextMeshProUGUI>().text = initial;
            vipItem.transform.Find("Toggle").GetComponent<Toggle>().group = SendVIPContent.GetComponent<ToggleGroup>();

            vipItem.transform.Find("Toggle").GetComponent<Toggle>().onValueChanged.AddListener(delegate {
                ToggleValueChangedVIP(vipItem.transform.Find("Toggle").GetComponent<Toggle>(), clubMemberDetails);
            });
            ScrollItems.Add(vipItem);

        }
    }

    private void ToggleValueChangedTradeList(Toggle tardeItem, ClubMemberDetails clubMemberDetails)
    {
        if (tardeItem.isOn)
        {
            if (!SelectedTradeMembers.Contains(clubMemberDetails))
            {
                SelectedTradeMembers.Add(clubMemberDetails);
            }
        }
        else
        {
            if (SelectedTradeMembers.Contains(clubMemberDetails))
            {
                SelectedTradeMembers.Remove(clubMemberDetails);
            }
        }
    }

    private void ToggleValueChangedSleep(Toggle sleepItem, ClubMemberDetails clubMemberDetails)
    {
        if (sleepItem.isOn)
        {
            if (!SelectedSleepMembers.Contains(clubMemberDetails))
            {
                SelectedSleepMembers.Add(clubMemberDetails);
            }
        }
        else
        {
            if (SelectedSleepMembers.Contains(clubMemberDetails))
            {
                SelectedSleepMembers.Remove(clubMemberDetails);
            }
        }
    }

    private void ToggleValueChangedVIP(Toggle vipItem, ClubMemberDetails clubMemberDetails)
    {
        if (vipItem.isOn)
        {
            LastSelectedVIPMember = clubMemberDetails;
        }

    }

    private void OpenSendOutPanel()
    {
        SendOutPanel.SetActive(true);
        OpenSendOut();
    }

    public Transform SendOutListContent;

    public void CloseSendOutPanel()
    {
        for (int i = 0; i < SendOutListContent.childCount; i++)
        {
            Destroy(SendOutListContent.GetChild(i).gameObject);        
        }
        selectedMembers = string.Empty;
        selectedSendPlayerCount = 0;
        AmountToSendInputField.text = string.Empty;
        PlayerSelected.text = string.Empty;
        TotalAmountSendText.text = "Total:";
        SendOutPanel.SetActive(false);
    }

    public TMPro.TMP_InputField AmountToSendInputField;
    public TextMeshProUGUI PlayerSelected, TotalAmountSendText;
    private int selectedSendPlayerCount = 0;

    private void OpenSendOut()
    {
        AmountToSendInputField.onValueChanged.RemoveAllListeners();
        AmountToSendInputField.onValueChanged.AddListener(UpdateUIForSendAmount);

        //to-d0
        //1. fill club chips text
        for (int i = 0; i < SelectedTradeMembers.Count; i++)
        {
            ClubMemberDetails clubMemberDetails = SelectedTradeMembers[i];
            string initial = clubMemberDetails.userName.ToUpper();
            initial = initial.Substring(0, 2);

            GameObject tardeItem = Instantiate(TradeListItemPrefab, SendOutListContent) as GameObject;
            tardeItem.SetActive(true);

            tardeItem.transform.Find("TextName").GetComponent<TMPro.TextMeshProUGUI>().text = clubMemberDetails.userName;
            tardeItem.transform.Find("TextId").GetComponent<TMPro.TextMeshProUGUI>().text = "ID : " + clubMemberDetails.userId;
            tardeItem.transform.Find("TextNickname").GetComponent<TMPro.TextMeshProUGUI>().text = "Nickname : " + clubMemberDetails.nickName;
            tardeItem.transform.Find("Image/Coins").GetComponent<TMPro.TextMeshProUGUI>().text = clubMemberDetails.ptChips;
            tardeItem.transform.Find("Image/Text (TMP)").GetComponent<TMPro.TextMeshProUGUI>().text = initial;
            tardeItem.transform.Find("Toggle").GetComponent<Toggle>().SetIsOnWithoutNotify(true);
            selectedSendPlayerCount++;

            tardeItem.transform.Find("Toggle").GetComponent<Toggle>().onValueChanged.AddListener(delegate {
                AddToSelectedUsersForSendOut(tardeItem.transform.Find("Toggle").GetComponent<Toggle>(), clubMemberDetails);
            });

        }
    }

    private void UpdateUIForSendAmount(string arg0)
    {
        float amount = CalculateSenAmountBasedOnPercentage();

        PlayerSelected.text = "x" + selectedSendPlayerCount;
        TotalAmountSendText.text = "Total: " + amount.ToString();
    }

    private float CalculateSenAmountBasedOnPercentage()
    {
        int amountEntered = 0;
        Int32.TryParse(AmountToSendInputField.text, out amountEntered);
        float totalAmount = amountEntered * selectedSendPlayerCount;
        float percentageFee = 0.05f * totalAmount;
        totalAmount = totalAmount + percentageFee;
        return totalAmount;
    }

    string selectedMembers = string.Empty;

    private void AddToSelectedUsersForSendOut(Toggle t, ClubMemberDetails clubMemberDetails)
    {
        if (t.isOn)
        {
            selectedSendPlayerCount++;
        }
        else
        {
            selectedSendPlayerCount--;
        }

        if (selectedSendPlayerCount < 0) { selectedSendPlayerCount = 0; }

        PlayerSelected.text = "x" + selectedSendPlayerCount;
        selectedMembers += "{\"userId\":\"" + clubMemberDetails.userId + "\"},";
    }

    private void SendChipsAPIRequest()
    {
        float amount = CalculateSenAmountBasedOnPercentage();

        string request = "{\"userId\":\"" + MemberListUIManager.instance.GetClubOwnerObject().userId + "\"," +
            "\"clubId\":\"" + ClubDetailsUIManager.instance.GetClubId() + "\"," +
            "\"amount\":\"" + amount.ToString() + "\"," +
            "\"membersArray\":\"" + "[" + selectedMembers + "]" + "\"}";

        Debug.Log("request is - " + request);
        WebServices.instance.SendRequest(RequestType.SendChipsOut, request, true, OnServerResponseFound);
    }

    private void OpenScreen(string screenName)
    {
        Color c = new Color(1, 1, 1, 1);
        Color c1 = new Color(1, 1, 1, 0);

        switch (screenName)
        {
            case "Trade":
                TabTradeBtn.GetComponent<Image>().color = c;
                TabSleepModeBtn.GetComponent<Image>().color = c1;
                TabVIPBtn.GetComponent<Image>().color = c1;
                TabTicketBtn.GetComponent<Image>().color = c1;

                TradePanel.SetActive(true);
                SleepPanel.SetActive(false);
                VIPPanel.SetActive(false);
                TicketPanel.SetActive(false);
                break;
            case "SleepMode":
                TabTradeBtn.GetComponent<Image>().color = c1;
                TabSleepModeBtn.GetComponent<Image>().color = c;
                TabVIPBtn.GetComponent<Image>().color = c1;
                TabTicketBtn.GetComponent<Image>().color = c1;

                TradePanel.SetActive(false);
                SleepPanel.SetActive(true);
                VIPPanel.SetActive(false);
                TicketPanel.SetActive(false);
                break;
            case "VIP":
                TabTradeBtn.GetComponent<Image>().color = c1;
                TabSleepModeBtn.GetComponent<Image>().color = c1;
                TabVIPBtn.GetComponent<Image>().color = c;
                TabTicketBtn.GetComponent<Image>().color = c1;

                TradePanel.SetActive(false);
                SleepPanel.SetActive(false);
                VIPPanel.SetActive(true);
                TicketPanel.SetActive(false);
                break;
            case "Ticket":
                TabTradeBtn.GetComponent<Image>().color = c1;
                TabSleepModeBtn.GetComponent<Image>().color = c1;
                TabVIPBtn.GetComponent<Image>().color = c1;
                TabTicketBtn.GetComponent<Image>().color = c;

                TradePanel.SetActive(false);
                SleepPanel.SetActive(false);
                VIPPanel.SetActive(false);
                TicketPanel.SetActive(true);
                break;
            default:
                break;
        }
    }

}

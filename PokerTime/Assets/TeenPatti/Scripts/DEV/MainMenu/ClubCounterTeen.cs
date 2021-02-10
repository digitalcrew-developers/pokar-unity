using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClubCounterTeen : MonoBehaviour
{
    public static ClubCounterTeen instance;

    private int allPTChips;
    private float sendOutAmount = 0.0f;
    private float totalAmount = 0.0f;
    private float claimBackAmount = 0.0f;
    private float totalClaimBackAmount = 0.0f;
    private float clubChips;

    public GameObject popUpText;

    public Button StatsBtn;
    public Button TabTradeBtn, /*TabSleepModeBtn,*/ TabVIPBtn, TabTicketBtn;
    public GameObject TradePanel, /*SleepPanel,*/ VIPPanel, TicketPanel, TradeRecordPanel;

    public GameObject exchangePanel;

    public Button SendOutBtn, ClaimBackBtn, SetLimitBtn, RemoveLimitBtn, SendVIPBtn, AddTicketBtn, OpenTradeRecordBtn;

    public GameObject TradeListItemPrefab, /*SleepListItemPrefab,*/ SendVIPListItemPrefab, TicketItemListPrefab, TradeRecordPrefab;
    public Transform TradeListContent,/* SleepListContent,*/ SendVIPContent, TicketContent, TradeRecordListContent;

    public GameObject SendOutPanel;
    public TMP_Text SendOutChipsCount;
    public Button ConfirmChipsSendButton;
    public Button ConfirmChipsClaimBackButton;

    public List<TMPro.TextMeshProUGUI> MemberCountTexts;
    public TextMeshProUGUI ClubChipsCount, ClubOwnerChipCount, RestMemberChipCount;

    [Header("TRADE FILTERS")]
    public List<FilterButtonState> TradeFilterButtons = new List<FilterButtonState>();
    public Text TradeListFilterName;
    public Image TradeListFilterImage;
    public Button TradeFilterBtn;
    public GameObject TradeFilterPanel;

    [Header("SLEEP FILTERS")]
    public List<FilterButtonState> SleepButtons = new List<FilterButtonState>();
    public Text SleepFilterName;
    public Image SleepFilterImage;
    public Button SleepFilterBtn;
    public GameObject SleepFilterPanel;

    [Header("VIP FILTERS")]
    public List<FilterButtonState> VIPButtons = new List<FilterButtonState>();
    public Text VIPFilterName;
    public Image VIPFilterImage;
    public Button VIPFilterBtn;
    public GameObject VIPFilterPanel;
    public GameObject SelectVIPCardPanel;
    private string selectedUserIDForVIP;
    public Button OpenVIPButton;

    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
        }

        Initialise();

        popUpText.SetActive(false);        
    }
    private void Initialise()
    {
        TabTradeBtn.onClick.RemoveAllListeners();
        //TabSleepModeBtn.onClick.RemoveAllListeners();
        TabVIPBtn.onClick.RemoveAllListeners();
        TabTicketBtn.onClick.RemoveAllListeners();
        OpenTradeRecordBtn.onClick.RemoveAllListeners();

        TabTradeBtn.onClick.AddListener(() => OpenScreen("Trade"));
        //TabSleepModeBtn.onClick.AddListener(() => OpenScreen("SleepMode"));
        TabVIPBtn.onClick.AddListener(() => OpenScreen("VIP"));
        TabTicketBtn.onClick.AddListener(() => OpenScreen("Ticket"));
        OpenTradeRecordBtn.onClick.AddListener(() => OpenScreen("OpenTradeRecord"));

        GetMembersListFromServer();

        SendOutBtn.onClick.RemoveAllListeners();
        SendOutBtn.onClick.AddListener(() => OpenSendOutPanel(false));

        ClaimBackBtn.onClick.RemoveAllListeners();
        ClaimBackBtn.onClick.AddListener(() => OpenSendOutPanel(true));
        
        TradeFilterBtn.onClick.RemoveAllListeners();
        TradeFilterBtn.onClick.AddListener(ToggleOpenTradeListFilter);

        for (int i = 0; i < TradeFilterButtons.Count; i++)
        {
            TradeFilterButtons[i].OnStateChange += Filter_OnStateChange;
        }

        SleepFilterBtn.onClick.RemoveAllListeners();
        SleepFilterBtn.onClick.AddListener(ToggleOpenSleepFilter);

        for (int i = 0; i < SleepButtons.Count; i++)
        {
            SleepButtons[i].OnStateChange += Filter_OnStateChange;
        }


        VIPFilterBtn.onClick.RemoveAllListeners();
        VIPFilterBtn.onClick.AddListener(ToggleOpenVIPFIlter);

        for (int i = 0; i < VIPButtons.Count; i++)
        {
            VIPButtons[i].OnStateChange += Filter_OnStateChange;
        }

        float.TryParse(ClubDetailsUIManagerTeen.instance.CLubChips.text,out clubChips);
        ClubChipsCount.text = ClubDetailsUIManagerTeen.instance.CLubChips.text;
        SendOutChipsCount.text = ClubDetailsUIManagerTeen.instance.CLubChips.text;
        OpenVIPButton.onClick.RemoveAllListeners();
        OpenVIPButton.onClick.AddListener(OpenVIPPanel);
    }

    private void ToggleOpenTradeListFilter()
    {
        if (TradeFilterPanel.activeInHierarchy)
        {
            TradeFilterPanel.SetActive(false);
        }
        else
        {
            TradeFilterPanel.SetActive(true);
        }
    }

    private void ToggleOpenSleepFilter()
    {
        if (SleepFilterPanel.activeInHierarchy)
        {
            SleepFilterPanel.SetActive(false);
        }
        else
        {
            SleepFilterPanel.SetActive(true);
        }
    }

    private void ToggleOpenVIPFIlter()
    {
        if (VIPFilterPanel.activeInHierarchy)
        {
            VIPFilterPanel.SetActive(false);
        }
        else
        {
            VIPFilterPanel.SetActive(true);
        }
    }

    private void Filter_OnStateChange(FilterState stateType, string stateName,string PanelName)
    {
        //to-do. do filtering based on PanelName.
        switch (PanelName)
        {
            case "Trade":
                TradeListFilterName.text = stateName;
                if (stateType == FilterState.Ascending)
                {
                    TradeListFilterImage.transform.localScale = new Vector3(1, 1, 1);
                }
                else
                {
                    TradeListFilterImage.transform.localScale = new Vector3(1, -1, 1);
                }

                for (int i = 0; i < TradeFilterButtons.Count; i++)
                {
                    string name = TradeFilterButtons[i].GetStateName();
                    if (stateName != name)
                    {
                        TradeFilterButtons[i].UpdateState(FilterState.None);
                    }
                    else
                    {
                        TradeFilterButtons[i].UpdateState(stateType);
                    }
                }
                TradeFilterPanel.SetActive(false);
                break;
            case "Sleep":
                SleepFilterName.text = stateName;
                if (stateType == FilterState.Ascending)
                {
                    SleepFilterImage.transform.localScale = new Vector3(1, 1, 1);
                }
                else
                {
                    SleepFilterImage.transform.localScale = new Vector3(1, -1, 1);
                }

                for (int i = 0; i < SleepButtons.Count; i++)
                {
                    string name = SleepButtons[i].GetStateName();
                    if (stateName != name)
                    {
                        SleepButtons[i].UpdateState(FilterState.None);
                    }
                    else
                    {
                        SleepButtons[i].UpdateState(stateType);
                    }
                }
                SleepFilterPanel.SetActive(false);
                break;
            case "VIP":
                VIPFilterName.text = stateName;
                if (stateType == FilterState.Ascending)
                {
                    VIPFilterImage.transform.localScale = new Vector3(1, 1, 1);
                }
                else
                {
                    VIPFilterImage.transform.localScale = new Vector3(1, -1, 1);
                }

                for (int i = 0; i < VIPButtons.Count; i++)
                {
                    string name = VIPButtons[i].GetStateName();
                    if (stateName != name)
                    {
                        VIPButtons[i].UpdateState(FilterState.None);
                    }
                    else
                    {
                        VIPButtons[i].UpdateState(stateType);
                    }
                }
                VIPFilterPanel.SetActive(false);
                break;
            default:
                break;
        }

        //sort based on statename and type
        switch (stateName)
        {
            case "Time Joined":
                {

                }
                break;

            case "PP Chip Balance":
                {

                }
                break;

            case "Players With Prizes":
                {

                }
                break;

            case "Prize Expiry Date":
                {

                }
                break;

            case "Fee":
                {

                }
                break;

            case "SpinUp Buy-In":
                {

                }
                break;

            case "Winnings":
                {

                }
                break;

            case "Hand":
                {

                }
                break;

            case "LastLogin":
                {

                }
                break;

            case "LastPlayed":
                {

                }
                break;

            case "OldMember":
                {

                }
                break;

            case "NewMember":
                {

                }
                break;

            case "ActiveMember":
                {

                }
                break;

            default:
                break;
        }
        TradeFilterPanel.SetActive(false);
        VIPFilterPanel.SetActive(false);
    }

    public void CheckForAvailableMembers()
    {
        if (TradeListContent.childCount < 10)
            StartCoroutine(ShowPopUp("Cannot exchange with less than 10 members.", 1.29f));
        else
        {
            ClubAdminManagerTeen.instance.ShowScreen(ClubScreensTeen.Exchange);
        }
    }

    IEnumerator ShowPopUp(string msg, float delay)
    {
        popUpText.SetActive(true);
        popUpText.transform.GetChild(0).GetComponent<Text>().text = msg;
        yield return new WaitForSeconds(delay);
        popUpText.SetActive(false);
    }

    public void GetMembersListFromServer()
    {
        string requestData = "{\"clubId\":\"" + ClubDetailsUIManagerTeen.instance.GetClubId() + "\"}";
        //old member list
        WebServices.instance.SendRequestTP(RequestTypeTP.GetClubMemberList, requestData, true, OnServerResponseFound);
    }

    public void GetTradeRecordList()
    {
        string requestData = "{\"clubId\":\"" + ClubDetailsUIManagerTeen.instance.GetClubId() + "\"," +
                             "\"OrderBy\":\"" + "created" + "\"," +
                             "\"Sequence\":\"" + "DESC" + "\"}";
        WebServices.instance.SendRequestTP(RequestTypeTP.GetTradeHistory, requestData, true, OnServerResponseFound);
    }

    public void OnServerResponseFound(RequestTypeTP requestType, string serverResponse, bool isShowErrorMessage, string errorMessage)
    {
        MainMenuControllerTeen.instance.DestroyScreen(MainMenuScreensTeen.Loading);

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
            case RequestTypeTP.SendChipsOut:
                {
                    Debug.Log("Response => SendChipsOut : " + serverResponse);
                    JsonData data1 = JsonMapper.ToObject(serverResponse);
                    if (data1["success"].Equals(1))
                    {
                        MainMenuControllerTeen.instance.ShowMessage(data1["message"].ToString(), () =>
                        {
                            GetMembersListFromServer();
                            CloseSendOutPanel();
                        });

                        clubChips -= totalAmount;

                        ClubDetailsUIManagerTeen.instance.CLubChips.text = clubChips.ToString();
                        ClubChipsCount.text = ClubDetailsUIManagerTeen.instance.CLubChips.text;
                        SendOutChipsCount.text = ClubDetailsUIManagerTeen.instance.CLubChips.text;

                        for (int i = 0; i < SelectedTradeMembers.Count; i++)
                        {
                            SelectedTradeMembers.RemoveAt(i);
                        }
                    }
                    else
                    {
                        MainMenuControllerTeen.instance.ShowMessage(data1["message"].ToString());
                    }
                }
                break;

            case RequestTypeTP.ClaimBackChips:
                Debug.Log("Response => ClaimBackChips : " + serverResponse);
                JsonData data2 = JsonMapper.ToObject(serverResponse);
                if (data2["success"].Equals(1))
                {
                    MainMenuControllerTeen.instance.ShowMessage(data2["message"].ToString(), () =>
                    {
                        GetMembersListFromServer();
                        CloseSendOutPanel();
                    });

                    //clubChips += claimBackAmount;
                    clubChips += totalAmount;

                    ClubDetailsUIManagerTeen.instance.CLubChips.text = clubChips.ToString();
                    ClubChipsCount.text = ClubDetailsUIManagerTeen.instance.CLubChips.text;
                    SendOutChipsCount.text = ClubDetailsUIManagerTeen.instance.CLubChips.text;

                    for (int i = 0; i < SelectedTradeMembers.Count; i++)
                    {
                        SelectedTradeMembers.RemoveAt(i);
                    }
                }
                else
                {
                    MainMenuControllerTeen.instance.ShowMessage(data2["message"].ToString());
                }
                break;

            case RequestTypeTP.GetClubMemberList:
                {
                    Debug.Log("Response => GetClubMemberListByClubId: " + serverResponse);
                    JsonData data3 = JsonMapper.ToObject(serverResponse);
                    Debug.LogWarning("Club memeber list counter :" + serverResponse);
                    if (data3["status"].Equals(true))
                    {
                        AddToAllLists(data3);
                    }
                    else
                    {
                        //MainMenuController.instance.ShowMessage(data["message"].ToString());
                    }
                }
                break;
            //case RequestType.SendVIP:
            //    {
            //        JsonData data4 = JsonMapper.ToObject(serverResponse);
            //        Debug.LogWarning("Club memeber SendVIP :" + serverResponse);
            //        if (data4["status"].Equals(true))
            //        {
            //            MainMenuController.instance.ShowMessage("Successfully sent VIP to" + LastSelectedVIPMember.userName + " with hardcoded value of shopId 13", () =>
            //            {
            //                LastSelectedVIPMember = null;
            //                selectedUserIDForVIP = string.Empty;
            //                SelectVIPCardPanel.SetActive(false);
            //            });
            //        }
            //    }
            //    break;

            case RequestTypeTP.GetTradeHistory:
                {
                    JsonData data = JsonMapper.ToObject(serverResponse);
                    Debug.Log("Response GetTradeHistory: " + serverResponse);
                    if (data["success"].Equals(1))
                    {
                        for (int i = 0; i < TradeRecordListContent.childCount; i++)
                        {
                            Destroy(TradeRecordListContent.GetChild(i).gameObject);
                        }

                        //Debug.Log("Total Data: " + data["response"][0]["nickName"].ToString());
                        for (int i = 0; i < data["response"].Count; i++)
                        {
                            GameObject gm = Instantiate(TradeRecordPrefab, TradeRecordListContent);

                            if (data["response"][i]["nickName"] != null)
                            {
                                gm.name = data["response"][i]["nickName"].ToString();
                                gm.transform.Find("Name").GetComponent<TMP_Text>().text = data["response"][i]["nickName"].ToString();
                            }
                            else
                                gm.transform.Find("Name").GetComponent<TMP_Text>().text = data["response"][i]["userId"].ToString();

                            gm.transform.Find("Time").GetComponent<TMP_Text>().text = data["response"][i]["created"].ToString().Substring(0, 10) + " " + data["response"][i]["created"].ToString().Substring(11, 5);

                            if (data["response"][i]["tradeType"].ToString().Equals("Dr"))
                            {
                                gm.transform.Find("Image").GetChild(0).GetComponent<TMP_Text>().color = Color.red;
                                gm.transform.Find("Image").GetChild(0).GetComponent<TMP_Text>().text = "-" + data["response"][i]["amount"].ToString();

                                gm.transform.Find("Data").GetComponent<TMP_Text>().text = "Sent out to " + data["response"][i]["toUserId"].ToString();
                            }
                            else
                            {
                                gm.transform.Find("Image").GetChild(0).GetComponent<TMP_Text>().color = Color.green;
                                gm.transform.Find("Image").GetChild(0).GetComponent<TMP_Text>().text = "+" + data["response"][i]["amount"].ToString();

                                gm.transform.Find("Data").GetComponent<TMP_Text>().text = "Claimed back from " + data["response"][i]["userId"].ToString();
                            }
                        }
                    }
                }
                break;

            default:
                Debug.LogError("Unhandled server requestType found  " + requestType);
                break;
        }                
    }

    private List<ClubMemberDetailsTeen> SelectedTradeMembers = new List<ClubMemberDetailsTeen>();
    private List<ClubMemberDetailsTeen> SelectedSleepMembers = new List<ClubMemberDetailsTeen>();
    private ClubMemberDetailsTeen LastSelectedVIPMember;

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
        allPTChips = 0;

        foreach(TMPro.TextMeshProUGUI t in MemberCountTexts)
        {
            t.text = "Member : " + data["data"].Count.ToString();
        }

        for (int i = 0; i < data["data"].Count; i++)
        {
            int x = i;
            ClubMemberDetailsTeen clubMemberDetails = new ClubMemberDetailsTeen();
            clubMemberDetails.userId = data["data"][x]["requestUserId"].ToString();
            clubMemberDetails.userName = data["data"][x]["requestUserName"].ToString();
            clubMemberDetails.clubRequestId = data["data"][x]["clubRequestId"].ToString();
            clubMemberDetails.ptChips = data["data"][x]["ptChips"].ToString();

            Debug.Log("For Player: " + clubMemberDetails.userId + " - PT Chips : " + clubMemberDetails.ptChips);

            //DEV_CODE
            allPTChips += int.Parse(clubMemberDetails.ptChips);

            if (clubMemberDetails.userId.Equals(PlayerManager.instance.GetPlayerGameData().userId))
                ClubOwnerChipCount.text = clubMemberDetails.ptChips;


            string initial = clubMemberDetails.userName.ToUpper();
            initial = initial.Substring(0, 2);

            GameObject tardeItem = Instantiate(TradeListItemPrefab, TradeListContent) as GameObject;
            tardeItem.SetActive(true);

            //Set Instantiated Object name to user name
            tardeItem.name = clubMemberDetails.userName;

            tardeItem.transform.Find("TextName").GetComponent<TMPro.TextMeshProUGUI>().text = clubMemberDetails.userName;
            tardeItem.transform.Find("TextId").GetComponent<TMPro.TextMeshProUGUI>().text = "ID : " + clubMemberDetails.userId;
            tardeItem.transform.Find("TextNickname").GetComponent<TMPro.TextMeshProUGUI>().text = "Nickname : " + clubMemberDetails.nickName;
            tardeItem.transform.Find("Image/Coins").GetComponent<TMPro.TextMeshProUGUI>().text = clubMemberDetails.ptChips;  
            tardeItem.transform.Find("Image/Text (TMP)").GetComponent<TMPro.TextMeshProUGUI>().text = initial;

            tardeItem.transform.Find("Toggle").GetComponent<Toggle>().onValueChanged.AddListener(delegate {
                ToggleValueChangedTradeList(tardeItem.transform.Find("Toggle").GetComponent<Toggle>(), clubMemberDetails);
            });
            ScrollItems.Add(tardeItem);

            //GameObject sleepItem = Instantiate(SleepListItemPrefab, SleepListContent) as GameObject;
            //sleepItem.SetActive(true);

            //sleepItem.transform.Find("TextName").GetComponent<TMPro.TextMeshProUGUI>().text = clubMemberDetails.userName;
            //sleepItem.transform.Find("TextId").GetComponent<TMPro.TextMeshProUGUI>().text = "ID : " + clubMemberDetails.userId;
            //sleepItem.transform.Find("TextNickname").GetComponent<TMPro.TextMeshProUGUI>().text = "Nickname : " + clubMemberDetails.nickName;
            //sleepItem.transform.Find("Image/Coins").GetComponent<TMPro.TextMeshProUGUI>().text = clubMemberDetails.ptChips;
            //sleepItem.transform.Find("Image/Text (TMP)").GetComponent<TMPro.TextMeshProUGUI>().text = initial;

            //sleepItem.transform.Find("Toggle").GetComponent<Toggle>().onValueChanged.AddListener(delegate {
            //    ToggleValueChangedSleep(sleepItem.transform.Find("Toggle").GetComponent<Toggle>(), clubMemberDetails);
            //});
            //ScrollItems.Add(sleepItem);

            GameObject vipItem = Instantiate(SendVIPListItemPrefab, SendVIPContent) as GameObject;
            vipItem.SetActive(true);

            //Set Instantiated Object name to user name
            vipItem.name = clubMemberDetails.userName;

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


        RestMemberChipCount.text = allPTChips.ToString();
    }

    private void ToggleValueChangedTradeList(Toggle tardeItem, ClubMemberDetailsTeen clubMemberDetails)
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

    //private void ToggleValueChangedSleep(Toggle sleepItem, ClubMemberDetails clubMemberDetails)
    //{
    //    if (sleepItem.isOn)
    //    {
    //        if (!SelectedSleepMembers.Contains(clubMemberDetails))
    //        {
    //            SelectedSleepMembers.Add(clubMemberDetails);
    //        }
    //    }
    //    else
    //    {
    //        if (SelectedSleepMembers.Contains(clubMemberDetails))
    //        {
    //            SelectedSleepMembers.Remove(clubMemberDetails);
    //        }
    //    }
    //}

    private void OpenVIPPanel()
    {
        if (string.IsNullOrEmpty(selectedUserIDForVIP))
        {
            MainMenuControllerTeen.instance.ShowMessage("Please select a member");
        }
        SelectVIPCardPanel.SetActive(true);
    }

    public void SendVIPCardRequest()
    {
        string requestData = "{\"userId\":\"" + PlayerManager.instance.GetPlayerGameData().userId + "\"," +
                   "\"clubId\":\"" + ClubDetailsUIManagerTeen.instance.GetClubId() + "\"," +
                   "\"shopId\":\"" + "13" + "\"," +
                   "\"toUserId\":\"" + selectedUserIDForVIP + "\"}";

        Debug.Log("sending vip for user id :" + requestData + " username :" + LastSelectedVIPMember);

        //WebServices.instance.SendRequest(RequestType.SendVIP, requestData, true, OnServerResponseFound);

    }

    private void ToggleValueChangedVIP(Toggle vipItem, ClubMemberDetailsTeen clubMemberDetails)
    {
        if (vipItem.isOn)
        {
            LastSelectedVIPMember = clubMemberDetails;
            selectedUserIDForVIP = clubMemberDetails.userId;
        }

    }

    private void OpenSendOutPanel(bool claim = false)
    {
        bool isAnyMemberSelected = false;

        for (int i = 0; i < TradeListContent.childCount; i++)
        {
            if (TradeListContent.GetChild(i).Find("Toggle").GetComponent<Toggle>().isOn)
                isAnyMemberSelected = true;
        }


        if (!isAnyMemberSelected)
            StartCoroutine(ShowPopUp("Please select a member", 1.29f));
        else
        {
            ConfirmChipsSendButton.onClick.RemoveAllListeners();
            if (!claim)
            {
                SendOutPanel.transform.Find("BG1/Heading/Text").GetComponent<Text>().text = "Send Out";
                SendOutPanel.transform.Find("BG1/BG2/Trade/Label").gameObject.SetActive(false);
                SendOutPanel.transform.Find("BG1/BG2/Trade/Chip1").gameObject.SetActive(true);
                SendOutPanel.transform.Find("BG1/BG2/Trade/Panel/CommisionText").gameObject.SetActive(true);
                SendOutPanel.transform.Find("BG1/BG2/Trade/Panel/AllClaimBackToggle").gameObject.SetActive(false);
                ConfirmChipsSendButton.onClick.AddListener(SendChipsAPIRequest);
            }
            else
            {
                SendOutPanel.transform.Find("BG1/Heading/Text").GetComponent<Text>().text = "Claim Back";
                SendOutPanel.transform.Find("BG1/BG2/Trade/Label").gameObject.SetActive(true);
                SendOutPanel.transform.Find("BG1/BG2/Trade/Chip1").gameObject.SetActive(false);
                SendOutPanel.transform.Find("BG1/BG2/Trade/Panel/CommisionText").gameObject.SetActive(false);
                SendOutPanel.transform.Find("BG1/BG2/Trade/Panel/AllClaimBackToggle").gameObject.SetActive(true);
                //ConfirmChipsClaimBackButton.onClick.AddListener(ClaimChipsAPIRequest);
                ConfirmChipsSendButton.onClick.AddListener(ClaimChipsAPIRequest);
            }

            SendOutPanel.SetActive(true);
            OpenSendOut(claim);
        }
    }

    public Transform SendOutListContent;

    public void CloseSendOutPanel()
    {
        for (int i = 0; i < SendOutListContent.childCount; i++)
        {
            Destroy(SendOutListContent.GetChild(i).gameObject);        
        }

        for (int i = 0; i < SelectedTradeMembers.Count; i++)
        {
            SelectedTradeMembers.RemoveAt(i);
        }

        for (int i = 0; i < TradeListContent.childCount; i++)
        {
            TradeListContent.GetChild(i).Find("Toggle").GetComponent<Toggle>().isOn = false;
        }

        selectedMembers = string.Empty;
        selectedSendPlayerCount = 0;
        AmountToSendInputField.text = string.Empty;
        PlayerSelected.text = string.Empty;
        TotalAmountSendText.text = "Total:";
        claimBackAmount = 0;
        SendOutPanel.SetActive(false);
    }

    public TMPro.TMP_InputField AmountToSendInputField;
    public TextMeshProUGUI PlayerSelected, TotalAmountSendText;
    private int selectedSendPlayerCount = 0;

    private void OpenSendOut(bool isClaimBack)
    {
        selectedSendPlayerCount = 0;

        PlayerSelected.text = string.Empty;

        AmountToSendInputField.onValueChanged.RemoveAllListeners();
        AmountToSendInputField.onValueChanged.AddListener(UpdateUIForSendAmount);

        //to-do
        //1. fill club chips text
        for (int i = 0; i < SendOutListContent.childCount; i++)
        {
            Destroy(SendOutListContent.GetChild(i).gameObject);
        }

        for (int i = 0; i < SelectedTradeMembers.Count; i++)
        {
            ClubMemberDetailsTeen clubMemberDetails = SelectedTradeMembers[i];
            string initial = clubMemberDetails.userName.ToUpper();
            initial = initial.Substring(0, 2);

            GameObject tardeItem = Instantiate(TradeListItemPrefab, SendOutListContent) as GameObject;
            tardeItem.SetActive(true);

            //Set Instantiated Object name to user name
            tardeItem.name = clubMemberDetails.userName;

            tardeItem.transform.Find("TextName").GetComponent<TMPro.TextMeshProUGUI>().text = clubMemberDetails.userName;
            tardeItem.transform.Find("TextId").GetComponent<TMPro.TextMeshProUGUI>().text = "ID : " + clubMemberDetails.userId;
            tardeItem.transform.Find("TextNickname").GetComponent<TMPro.TextMeshProUGUI>().text = "Nickname : " + clubMemberDetails.nickName;
            tardeItem.transform.Find("Image/Coins").GetComponent<TMPro.TextMeshProUGUI>().text = clubMemberDetails.ptChips;
            tardeItem.transform.Find("Image/Text (TMP)").GetComponent<TMPro.TextMeshProUGUI>().text = initial;
            tardeItem.transform.Find("Toggle").GetComponent<Toggle>().SetIsOnWithoutNotify(true);

            tardeItem.transform.Find("Toggle").GetComponent<Toggle>().onValueChanged.AddListener(delegate
            {
                AddToSelectedUsersForSendOut(tardeItem.transform.Find("Toggle").GetComponent<Toggle>(), clubMemberDetails);
            });

            selectedSendPlayerCount++;

            PlayerSelected.text = "x" + selectedSendPlayerCount;
            selectedMembers += "{\"userId\":" + clubMemberDetails.userId + ",\"role\":\"" + "Member" + "\"},";

            if (isClaimBack)
            {
                if (TotalAmountSendText.gameObject.activeSelf)
                    TotalAmountSendText.gameObject.SetActive(false);

                if (claimBackAmount == 0)
                    float.TryParse(tardeItem.transform.Find("Image/Coins").GetComponent<TMPro.TextMeshProUGUI>().text, out claimBackAmount);

                float amount;
                float.TryParse(tardeItem.transform.Find("Image/Coins").GetComponent<TMPro.TextMeshProUGUI>().text, out amount);
                if (claimBackAmount > amount)
                    claimBackAmount = amount;

                SendOutPanel.transform.Find("BG1/BG2/Trade/Label/ChipsToClaimBack").GetComponent<TMP_Text>().text = claimBackAmount.ToString();
            }
            else
            {
                if (!TotalAmountSendText.gameObject.activeSelf)
                    TotalAmountSendText.gameObject.SetActive(true);
            }
        }
    }

    private void UpdateUIForSendAmount(string arg0)
    {
        if (SendOutPanel.transform.Find("BG1/BG2/Trade/Label").gameObject.activeSelf)
        {
            int amountEntered = 0;
            Int32.TryParse(AmountToSendInputField.text, out amountEntered);
            if (amountEntered > claimBackAmount)
            {
                SendOutPanel.transform.Find("BG1/BG2/Trade/Label").GetComponent<TMP_Text>().color = Color.red;
                SendOutPanel.transform.Find("BG1/BG2/Trade/Label/ChipsToClaimBack").GetComponent<TMP_Text>().color = Color.red;

                ConfirmChipsSendButton.onClick.RemoveAllListeners();
                ConfirmChipsSendButton.onClick.AddListener(PopUpToShow);
            }
            else
            {
                SendOutPanel.transform.Find("BG1/BG2/Trade/Label").GetComponent<TMP_Text>().color = Color.white;
                SendOutPanel.transform.Find("BG1/BG2/Trade/Label/ChipsToClaimBack").GetComponent<TMP_Text>().color = Color.white;

                ConfirmChipsSendButton.onClick.RemoveAllListeners();
                ConfirmChipsSendButton.onClick.AddListener(ClaimChipsAPIRequest);

                totalAmount = CalculateSendAmountBasedOnPercentage(true);
            }
        }
        else
        {
            totalAmount = CalculateSendAmountBasedOnPercentage(false);

            PlayerSelected.text = "x" + selectedSendPlayerCount;
            TotalAmountSendText.text = "Total: " + totalAmount.ToString();
        }
    }

    private void PopUpToShow()
    {
        StartCoroutine(ShowPopUp("Insufficient member balance", 1.29f));
    }

    private float CalculateSendAmountBasedOnPercentage(bool isClaimBack)
    {
        int amountEntered = 0;
        Int32.TryParse(AmountToSendInputField.text, out amountEntered);
        float totalAmount = amountEntered * selectedSendPlayerCount;
        
        if (!isClaimBack)
        {
            float percentageFee = 0.05f * totalAmount;
            totalAmount = totalAmount + percentageFee;
        }

        return totalAmount;
    }

    string selectedMembers = string.Empty;

    private void AddToSelectedUsersForSendOut(Toggle t, ClubMemberDetailsTeen clubMemberDetails)
    {
        if (t.isOn)
        {
            selectedSendPlayerCount++;
            //selectedMembers += "{\"userId\":\"" + clubMemberDetails.userId + "\"," +
            //                   "\"role\":\"" + "Member" + "\"},";
        }
        else
        {
            selectedSendPlayerCount--;
        }

        if (selectedSendPlayerCount < 0) { selectedSendPlayerCount = 0; }

        PlayerSelected.text = "x" + selectedSendPlayerCount;

        if (SendOutPanel.transform.Find("BG1/BG2/Trade/Label").gameObject.activeSelf)
        {
            claimBackAmount = 0;
            selectedMembers = string.Empty;

            for (int i = 0; i < SendOutListContent.childCount; i++)
            {
                if (SendOutListContent.GetChild(i).Find("Toggle").GetComponent<Toggle>().isOn)
                {
                    selectedMembers += "{\"userId\":\"" + SendOutListContent.GetChild(i).Find("TextId").GetComponent<TMPro.TextMeshProUGUI>().text.Substring(5) + "\"," +
                                       "\"role\":\"" + "Member" + "\"},";

                    if (claimBackAmount == 0)
                        float.TryParse(SendOutListContent.GetChild(i).Find("Image/Coins").GetComponent<TMP_Text>().text, out claimBackAmount);

                    float amount;
                    float.TryParse(SendOutListContent.GetChild(i).Find("Image/Coins").GetComponent<TMP_Text>().text, out amount);

                    Debug.Log("ClaimBackAmount : " + claimBackAmount);
                    Debug.Log("Amount : " + amount);
                    if (claimBackAmount > amount)
                        claimBackAmount = amount;
                }
            }
            SendOutPanel.transform.Find("BG1/BG2/Trade/Label/ChipsToClaimBack").GetComponent<TMP_Text>().text = claimBackAmount.ToString();
        }
        else
        {
            for (int i = 0; i < SendOutListContent.childCount; i++)
            {
                if (SendOutListContent.GetChild(i).Find("Toggle").GetComponent<Toggle>().isOn)
                {
                    selectedMembers += "{\"userId\":\"" + SendOutListContent.GetChild(i).Find("TextId").GetComponent<TMPro.TextMeshProUGUI>().text + "\"," +
                                       "\"role\":\"" + "Member" + "\"},";
                }
            }
        }
    }

    private void ClaimChipsAPIRequest()
    {
        //int amount = 0;
        //int.TryParse(AmountToSendInputField.text, out amount);

        float.TryParse(AmountToSendInputField.text, out claimBackAmount);

        if (claimBackAmount > 0)
        {
            selectedMembers = selectedMembers.Remove(selectedMembers.Length - 1, 1);

            string role = "";
            if (MemberListUIManagerTeen.instance.GetClubOwnerObject().memberRole == ClubMemberRole.Owner)
            {
                role = "Creater";
            }
            else
            {
                role = MemberListUIManagerTeen.instance.GetClubOwnerObject().memberRole.ToString();
            }

            string request = "{\"userId\":\"" + PlayerManager.instance.GetPlayerGameData().userId + "\"," +
                "\"role\":\"" + (role.Equals("Creater") ? "Creater" : "") + "\"," +
                "\"clubId\":" + ClubDetailsUIManagerTeen.instance.GetClubId() + "," +
                "\"amount\":" + claimBackAmount.ToString() + "," +
                "\"membersArray\":[" + selectedMembers + "]}";

            //Debug.Log("request is - " + request);
            WebServices.instance.SendRequestTP(RequestTypeTP.ClaimBackChips, request, true, OnServerResponseFound);
        }
        else
        {
            StartCoroutine(ShowPopUp("Please enter the amount", 1.29f));
        }
    }

    private void SendChipsAPIRequest()
    {
        //Debug.Log("User ID:" + PlayerManager.instance.GetPlayerGameData().userId);

        //int amount = 0;
        //int.TryParse(AmountToSendInputField.text, out amount);

        float.TryParse(AmountToSendInputField.text, out sendOutAmount);

        if (sendOutAmount > 0)
        {
            selectedMembers = selectedMembers.Remove(selectedMembers.Length - 1, 1);

            string request = "{\"userId\":\"" + PlayerManager.instance.GetPlayerGameData().userId + "\"," +
                "\"clubId\":\"" + ClubDetailsUIManagerTeen.instance.GetClubId() + "\"," +
                "\"amount\":\"" + sendOutAmount.ToString() + "\"," +
                "\"membersArray\":[" + selectedMembers + "]}";

            //Debug.Log("request is - " + request);
            WebServices.instance.SendRequestTP(RequestTypeTP.SendChipsOut, request, true, OnServerResponseFound);
        }
        else
        {
            StartCoroutine(ShowPopUp("Please enter the amount", 1.29f));
        }
    }

    private void OpenScreen(string screenName)
    {
        Color c = new Color(1, 1, 1, 1);
        Color c1 = new Color(1, 1, 1, 0);

        switch (screenName)
        {
            case "Trade":
                TabTradeBtn.GetComponent<Image>().color = c;
                //TabSleepModeBtn.GetComponent<Image>().color = c1;
                TabVIPBtn.GetComponent<Image>().color = c1;
                TabTicketBtn.GetComponent<Image>().color = c1;

                TradePanel.SetActive(true);
                //SleepPanel.SetActive(false);
                VIPPanel.SetActive(false);
                TicketPanel.SetActive(false);
                break;
            case "SleepMode":
                TabTradeBtn.GetComponent<Image>().color = c1;
                //TabSleepModeBtn.GetComponent<Image>().color = c;
                TabVIPBtn.GetComponent<Image>().color = c1;
                TabTicketBtn.GetComponent<Image>().color = c1;

                TradePanel.SetActive(false);
                //SleepPanel.SetActive(true);
                VIPPanel.SetActive(false);
                TicketPanel.SetActive(false);
                break;
            case "VIP":
                TabTradeBtn.GetComponent<Image>().color = c1;
                //TabSleepModeBtn.GetComponent<Image>().color = c1;
                TabVIPBtn.GetComponent<Image>().color = c;
                TabTicketBtn.GetComponent<Image>().color = c1;

                TradePanel.SetActive(false);
                //SleepPanel.SetActive(false);
                VIPPanel.SetActive(true);
                TicketPanel.SetActive(false);
                break;
            case "Ticket":
                TabTradeBtn.GetComponent<Image>().color = c1;
                //TabSleepModeBtn.GetComponent<Image>().color = c1;
                TabVIPBtn.GetComponent<Image>().color = c1;
                TabTicketBtn.GetComponent<Image>().color = c;

                TradePanel.SetActive(false);
                //SleepPanel.SetActive(false);
                VIPPanel.SetActive(false);
                TicketPanel.SetActive(true);
                break;

            case "OpenTradeRecord":
                GetTradeRecordList();
                TradeRecordPanel.SetActive(true);
                break;

            default:
                TradeRecordPanel.SetActive(false);
                break;
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using LitJson;
using System.Net;
using UnityEngine.Networking;

public class MemberDetails : MonoBehaviour
{
    public static MemberDetails instance;

    public GameObject popUpText;

    public TextMeshProUGUI UserID, UserName, Remark, LastLogin;
    public Image ProfileImage;
    public Button EditProfileBtn;

    public List<Button> TabButtons;

    public TextMeshProUGUI Winnings, Hands, BB, MTT, SpinBuy, Fee;
    public Button TrackPlayerBtn, DeleteMemberBtn;
    public Toggle Manager, Agent, Member;
    public List<GameObject> AdditionalObjects;

    //DEV_CODE
    [Space(10)]
    public GameObject TipsObj;

    public TMP_InputField editMemberName, editMemberNote;

    private ClubMemberDetails clubMemberDetails;

    private float fullHeight = 979f;
    private float halfHeight = 690f;
    private float fullWidth = 650f;

    private GameObject ListObject;

    #region AGENT_DATA
    [Space(18)]
    [Header("AGENT DATA")]
    public GameObject AgentCareerDetails;
    public GameObject AgentPanel;
    public GameObject AgentPrefabForDownline;
    public GameObject AgentPrefabForGrantCredit;

    [Space(9)]
    public List<Button> AgentPanelTabButtons;

    [Space(9)]
    public Transform AgentSelectDownlineContainer;
    public Transform AgentGrantCreditContainer;
    public Transform AgentCurrentDownlineContainer;

    [Space(9)]
    public GameObject AgentSelectDownlineObj;
    public GameObject AgentGrantCreditObj;
    public GameObject AgentCurrentDownlineObj;
    #endregion

    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
        }

        if (TipsObj != null)
            TipsObj.SetActive(false);
    }

    /// <summary>
    /// Initialise needs to be called for each time member
    /// screen is opened
    /// </summary>
    public void Initialise(ClubMemberDetails _clubMemberDetails, GameObject ListObject)
    {
        Debug.Log("After Changing new role");
        //DEV_CODE
        editMemberName.text = _clubMemberDetails.userName;

        //fill general details from previously present data 
        clubMemberDetails = _clubMemberDetails;
        UserName.text = _clubMemberDetails.userName;
        UserID.text = "ID: " + clubMemberDetails.userId + " | " + "Nickname " + clubMemberDetails.nickName;

        //Assign tab listeners
        TabButtons[0].onClick.RemoveAllListeners();
        TabButtons[1].onClick.RemoveAllListeners();
        TabButtons[2].onClick.RemoveAllListeners();

        TabButtons[0].onClick.AddListener(() => EnableTab("overall"));
        TabButtons[1].onClick.AddListener(() => EnableTab("7days"));
        TabButtons[2].onClick.AddListener(() => EnableTab("select"));

        EnableTab("overall");
        //call api and fill rest details

        //Setting Current Player role
        Debug.Log("Current Player Role: " + clubMemberDetails.memberRole);
        switch (clubMemberDetails.memberRole)
        {
            case ClubMemberRole.Agent:
                Agent.isOn = true;
                Agent.transform.Find("Image").GetComponent<Button>().interactable = true;
                if (!transform.Find("BG1/Heading/Career").gameObject.activeSelf)
                    transform.Find("BG1/Heading/Career").gameObject.SetActive(true);

                string requestData = "{\"clubId\":\"" + ClubDetailsUIManager.instance.GetClubId() + "\"," +
                                 "\"userId\":\"" + clubMemberDetails.userId + "\"}";

                WebServices.instance.SendRequest(RequestType.GetAgentDetails, requestData, true, OnServerResponseFound);
                break;

            case ClubMemberRole.Manager:
                Agent.transform.Find("Image").GetComponent<Button>().interactable = false;
                Manager.isOn = true;
                if (transform.Find("BG1/Heading/Career").gameObject.activeSelf)
                    transform.Find("BG1/Heading/Career").gameObject.SetActive(false);
                break;

            case ClubMemberRole.Member:
                Agent.transform.Find("Image").GetComponent<Button>().interactable = false;
                Member.isOn = true;
                if (transform.Find("BG1/Heading/Career").gameObject.activeSelf)
                    transform.Find("BG1/Heading/Career").gameObject.SetActive(false);
                break;
        }

        Manager.onValueChanged.RemoveAllListeners();
        Agent.onValueChanged.RemoveAllListeners();
        Member.onValueChanged.RemoveAllListeners();

        Manager.onValueChanged.AddListener(delegate
        {
            ManagerToogleValueChanged(true);
        });
        Agent.onValueChanged.AddListener(delegate
        {   
            AgentToogleValueChanged(true);
        });
        Member.onValueChanged.AddListener(delegate
        {
            MemberToogleValueChanged(true);
        });

        if (clubMemberDetails.memberRole == ClubMemberRole.Owner)
        {
            Debug.Log("Owner Details" + clubMemberDetails.userId);
            transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(fullWidth, halfHeight);
            foreach (GameObject g in AdditionalObjects)
            {
                g.SetActive(false);
            }
        }
        else
        {
            Debug.Log("Other Details" + clubMemberDetails.userId);
            transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(fullWidth, fullHeight);
            foreach (GameObject g in AdditionalObjects)
            {
                g.SetActive(true);
            }

            DeleteMemberBtn.onClick.RemoveAllListeners();
            DeleteMemberBtn.onClick.AddListener(DeleteMember);
        }
    }

    private void ManagerToogleValueChanged(bool isToUpdateRole = false)
    {
        if (Manager.isOn && isToUpdateRole)
        {
            Debug.Log("Manager Toggle Changed..");
            CallUpdatePlayer(ClubMemberRole.Manager);
        }
    }

    private void AgentToogleValueChanged(bool isToUpdateRole = false)
    {
        if (Agent.isOn && isToUpdateRole)
        {
            Debug.Log("Agent Toggle Changed..");
            CallUpdatePlayer(ClubMemberRole.Agent);
        }
    }

    private void MemberToogleValueChanged(bool isToUpdateRole = false)
    {
        if(Member.isOn && isToUpdateRole)
        {
            Debug.Log("Member Toggle Changed..");
            CallUpdatePlayer(ClubMemberRole.Member);
        }
    }

    public void CallUpdatePlayer(ClubMemberRole clubMemberRole)
    {
        Debug.Log("Current Role: " + clubMemberRole);
        Debug.Log("Previous Role: " + clubMemberDetails.memberRole);

        if (clubMemberDetails.memberRole == ClubMemberRole.Agent)
        {
            TipsObj.transform.Find("BG1/BG2/Text").GetComponent<Text>().text = "Current downlines' data will be removed if you chagne this Agent's rights. Are you sure?";
        }
        else
        {
            TipsObj.transform.Find("BG1/BG2/Text").GetComponent<Text>().text = "Confirm to change user rights?";
        }

        TipsObj.SetActive(true);
        TipsObj.transform.Find("BG1/BG2/BottomBtns/ConfirmBtn").GetComponent<Button>().onClick.AddListener(() =>
            OnConfirmChangeMemberRole(clubMemberRole));
        TipsObj.transform.Find("BG1/BG2/BottomBtns/CancelBtn").GetComponent<Button>().onClick.AddListener(() =>
            OnCancelChangeMemberRole(clubMemberDetails.memberRole));
    }

    private void OnConfirmChangeMemberRole(ClubMemberRole memberRole)
    {
        Debug.Log("Confirm Toggle: " + memberRole);

        TipsObj.transform.Find("BG1/Heading/Text").GetComponent<Text>().text = "Tips";
        TipsObj.transform.Find("BG1/Heading/Close").gameObject.SetActive(false);
        TipsObj.transform.Find("BG1/BG2/BottomBtns/CancelBtn").gameObject.SetActive(true);

        if (memberRole == ClubMemberRole.Agent)
        {
            string requestData = "{\"clubId\":\"" + ClubDetailsUIManager.instance.GetClubId() + "\"," +
                                 "\"userId\":\"" + clubMemberDetails.userId + "\"}";

            WebServices.instance.SendRequest(RequestType.GetAgentDetails, requestData, true, OnServerResponseFound);

            Agent.transform.Find("Image").GetComponent<Button>().interactable = true;
            transform.Find("BG1/Heading/Career").gameObject.SetActive(true);
            TipsObj.SetActive(false);
            MemberListUIManager.instance.ChangeUserRole(ListObject, false, clubMemberDetails, memberRole);
        }
        else if(memberRole == ClubMemberRole.Manager)
        {
            Agent.transform.Find("Image").GetComponent<Button>().interactable = false;
            transform.Find("BG1/Heading/Career").gameObject.SetActive(false);
            TipsObj.SetActive(false);
            MemberListUIManager.instance.ChangeUserRole(ListObject, false, clubMemberDetails, memberRole);
        }
        else if (memberRole == ClubMemberRole.Member)
        {
            Agent.transform.Find("Image").GetComponent<Button>().interactable = false;
            transform.Find("BG1/Heading/Career").gameObject.SetActive(false);
            TipsObj.SetActive(false);
            MemberListUIManager.instance.ChangeUserRole(ListObject, false, clubMemberDetails, memberRole);
        }        
    }

    private void OnCancelChangeMemberRole(ClubMemberRole memberRole)
    {
        switch (memberRole)
        {
            case ClubMemberRole.Agent:
                Agent.isOn = true;
                TipsObj.SetActive(false);
                break;
            case ClubMemberRole.Member:
                Member.isOn = true;
                TipsObj.SetActive(false);
                break;
            case ClubMemberRole.Manager:
                Manager.isOn = true;
                TipsObj.SetActive(false);
                break;
        }
    }

    private void DeleteMember()
    {
        TipsObj.transform.Find("BG1/Heading/Text").GetComponent<Text>().text = "Notice";
        TipsObj.transform.Find("BG1/Heading/Close").gameObject.SetActive(true);
        TipsObj.transform.Find("BG1/BG2/Text").GetComponent<Text>().text = "Wish to delete member?";
        TipsObj.transform.Find("BG1/BG2/BottomBtns/CancelBtn").gameObject.SetActive(false);
        TipsObj.transform.Find("BG1/BG2/BottomBtns/ConfirmBtn").GetComponent<Button>().onClick.AddListener(() =>
            MemberListUIManager.instance.ChangeUserRole(ListObject, true, clubMemberDetails, clubMemberDetails.memberRole));

        TipsObj.SetActive(true);
    }

    private void EnableTab(string tab)
    {
        Color c = new Color(1, 1, 1, 1);
        Color c1 = new Color(1, 1, 1, 0);

        switch (tab)
        {
            case "overall":
                TabButtons[0].GetComponent<Image>().color = c;
                TabButtons[1].GetComponent<Image>().color = c1;
                TabButtons[2].GetComponent<Image>().color = c1;
                break;
            case "7days":
                TabButtons[1].GetComponent<Image>().color = c;
                TabButtons[0].GetComponent<Image>().color = c1;
                TabButtons[2].GetComponent<Image>().color = c1;
                break;
            case "select":
                break;

            //FOR AGENT PANEL TABS
            case "selectDownline":
                AgentPanelTabButtons[0].GetComponent<Image>().color = c;
                AgentPanelTabButtons[1].GetComponent<Image>().color = c1;
                AgentPanelTabButtons[2].GetComponent<Image>().color = c1;

                AgentSelectDownlineObj.SetActive(true);
                AgentGrantCreditObj.SetActive(false);
                AgentCurrentDownlineObj.SetActive(false);
                break;
            case "grantCredit":
                AgentPanelTabButtons[0].GetComponent<Image>().color = c1;
                AgentPanelTabButtons[1].GetComponent<Image>().color = c;
                AgentPanelTabButtons[2].GetComponent<Image>().color = c1;

                AgentSelectDownlineObj.SetActive(false);
                AgentGrantCreditObj.SetActive(true);
                AgentCurrentDownlineObj.SetActive(false);
                break;
            case "currentDownline":
                AgentPanelTabButtons[0].GetComponent<Image>().color = c1;
                AgentPanelTabButtons[1].GetComponent<Image>().color = c1;
                AgentPanelTabButtons[2].GetComponent<Image>().color = c;

                AgentSelectDownlineObj.SetActive(false);
                AgentGrantCreditObj.SetActive(false);
                AgentCurrentDownlineObj.SetActive(true);
                break;

            default:
                break;
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

        if (requestType == RequestType.EditClubMemberDetails)
        {
            Debug.Log("Response => EditClubMemberDetails: " + serverResponse);

            JsonData data = JsonMapper.ToObject(serverResponse);

            if (data["success"].ToString() == "1")
            {

            }
            else
            {
                MainMenuController.instance.ShowMessage(data["message"].ToString());
            }
        }
        else if (requestType == RequestType.GetAgentDetails)
        {
            Debug.Log("Response => GetAgentDetails: " + serverResponse);

            JsonData data = JsonMapper.ToObject(serverResponse);

            if (data["success"].ToString() == "1")
            {
                //Assign tab listeners
                AgentPanelTabButtons[0].onClick.RemoveAllListeners();
                AgentPanelTabButtons[1].onClick.RemoveAllListeners();
                AgentPanelTabButtons[2].onClick.RemoveAllListeners();

                AgentPanelTabButtons[0].onClick.AddListener(() => EnableTab("selectDownline"));
                AgentPanelTabButtons[1].onClick.AddListener(() => EnableTab("grantCredit"));
                AgentPanelTabButtons[2].onClick.AddListener(() => EnableTab("currentDownline"));

                EnableTab("selectDownline");

                AgentCareerDetails.transform.Find("BG1/BG2/CenterArea/Data1/TotalFee/Data").GetComponent<Text>().text = data["response"][0]["TotalFee"].ToString();
                AgentCareerDetails.transform.Find("BG1/BG2/CenterArea/Data2/SpinUpBuyIn/Data").GetComponent<Text>().text = data["response"][0]["SpinUpBuyIn"].ToString();
                AgentCareerDetails.transform.Find("BG1/BG2/CenterArea/Data3/TotalWinnings/Data").GetComponent<Text>().text = data["response"][0]["TotalWinnings"].ToString();
                AgentCareerDetails.transform.Find("BG1/BG2/BottomPanel/MemberCount").GetComponent<Text>().text = data["response"][0]["MemberCount"].ToString();

                RequestAllDownlinerList();
                RequestCurrentDownlinerList();
            }
            else
            {
                MainMenuController.instance.ShowMessage(data["message"].ToString());
            }
        }
        else if (requestType == RequestType.AddDownliners)
        {
            Debug.Log("Response => AddDownliners: " + serverResponse);

            JsonData data = JsonMapper.ToObject(serverResponse);

            if (data["success"].ToString() == "1")
            {
                for (int i = 0; i < data["response"].Count; i++)
                {
                    RequestCurrentDownlinerList();
                }
            }           
        }
    }

    public void EditClubMemberDetails()
    {
        string requestData = "{\"uniqueClubId\":\"" + ClubDetailsUIManager.instance.GetClubUniqueId() + "\"," +
                    "\"requestUserId\":\"" + clubMemberDetails.userId + "\"," +
                    "\"userAlias\":\"" + editMemberName.text + "\"," +
                    "\"note\":\"" + editMemberNote.text + "\"}";

        WebServices.instance.SendRequest(RequestType.EditClubMemberDetails, requestData, true, OnServerResponseFound);
    }

    public void AddDownliners()
    {
        int selectedPlayersCounter = 0;
        string playerIds = "";

        for (int i = 0; i < AgentSelectDownlineContainer.childCount; i++)
        {
            if (AgentSelectDownlineContainer.GetChild(i).Find("SelectedToggle").GetComponent<Toggle>().isOn)
                selectedPlayersCounter++;
        }

        if (selectedPlayersCounter > 0)
        {
            for (int i = 0; i < AgentSelectDownlineContainer.childCount; i++)
            {
                if (AgentSelectDownlineContainer.GetChild(i).Find("SelectedToggle").GetComponent<Toggle>().isOn)
                {
                    if (playerIds == "")
                        playerIds += AgentSelectDownlineContainer.GetChild(i).Find("TextId").GetComponent<TMP_Text>().text;
                    else
                        playerIds += "," + AgentSelectDownlineContainer.GetChild(i).Find("TextId").GetComponent<TMP_Text>().text;
                }
            }

            string requestData = "{\"clubId\":\"" + ClubDetailsUIManager.instance.GetClubId() + "\"," +
                                    "\"agentId\":\"" + clubMemberDetails.userId + "\"," +
                                    "\"playerIds\":[\"" + playerIds + "\"]}";

            WebServices.instance.SendRequest(RequestType.AddDownliners, requestData, true, OnServerResponseFound);
        }
        else
        {
            Debug.Log("No Player is selected to add");
            StartCoroutine(ShowPopUp("Please select players first", 1.29f));
        }
    }

    private void RequestCurrentDownlinerList()
    {
        string requestData = "{\"clubId\":\"" + ClubDetailsUIManager.instance.GetClubId() + "\"," +
                             "\"agentId\":\"" + clubMemberDetails.userId + "\"}";

        WebServices.instance.SendRequest(RequestType.GetDownlineList, requestData, true, (requestType, serverResponse, isShowErrorMessage, errorMessage) =>
        {
            if (errorMessage.Length > 0)
            {
                MainMenuController.instance.ShowMessage(errorMessage);
            }
            else
            {
                JsonData data = JsonMapper.ToObject(serverResponse);
                if (data["success"].ToString() == "1")
                {
                    LoadCurrentDownliners(data);
                }
                else
                {
                    //MainMenuController.instance.ShowMessage(data["message"].ToString());
                }
            }
        });
    }

    private void LoadCurrentDownliners(JsonData data)
    {
        for (int i = 0; i < AgentSelectDownlineContainer.childCount; i++)
        {
            Destroy(AgentSelectDownlineContainer.GetChild(i).gameObject);
        }

        for (int i = 0; i < data["response"].Count; i++)
        {
            GameObject obj = Instantiate(AgentPrefabForDownline, AgentCurrentDownlineContainer) as GameObject;

            obj.transform.Find("TextName").GetComponent<TMP_Text>().text = data["response"][i]["requestUserName"].ToString();
            obj.transform.Find("TextId").GetComponent<TMP_Text>().text = data["response"][i]["requestUserId"].ToString();
            obj.transform.Find("TextNickname").GetComponent<TMP_Text>().text = data["response"][i]["nickName"].ToString();
            obj.transform.Find("Coins").GetComponent<TMP_Text>().text = data["response"][i]["ptChips"].ToString();
            obj.transform.Find("Coins").gameObject.SetActive(true);
            obj.transform.Find("SelectedToggle").gameObject.SetActive(false);

            StartCoroutine(LoadSpriteImageFromUrl(data["response"][i]["profileImage"].ToString(), obj.transform.Find("Image").GetComponent<Image>()));
        }
    }

    private void RequestAllDownlinerList()
    {
        string requestData = "{\"clubId\":\"" + ClubDetailsUIManager.instance.GetClubId() + "\"," +
                             "\"agentId\":\"\"}";

        WebServices.instance.SendRequest(RequestType.GetDownlineList, requestData, true, (requestType, serverResponse, isShowErrorMessage, errorMessage) =>
        {
            if (errorMessage.Length > 0)
            {
                MainMenuController.instance.ShowMessage(errorMessage);
            }
            else
            {
                JsonData data = JsonMapper.ToObject(serverResponse);
                if (data["success"].ToString() == "1")
                {
                    LoadAllDownlinerList(data);
                }
                else
                {
                    //MainMenuController.instance.ShowMessage(data["message"].ToString());
                }
            }
        });
    }

    private void LoadAllDownlinerList(JsonData data)
    {
        for (int i = 0; i < AgentSelectDownlineContainer.childCount; i++)
        {
            Destroy(AgentSelectDownlineContainer.GetChild(i).gameObject);
        }

        for (int i = 0; i < data["response"].Count; i++)
        {
            GameObject obj = Instantiate(AgentPrefabForDownline, AgentSelectDownlineContainer) as GameObject;

            obj.transform.Find("TextName").GetComponent<TMP_Text>().text = data["response"][i]["requestUserName"].ToString();
            obj.transform.Find("TextId").GetComponent<TMP_Text>().text = data["response"][i]["requestUserId"].ToString();
            obj.transform.Find("TextNickname").GetComponent<TMP_Text>().text = data["response"][i]["nickName"].ToString();
            //obj.transform.Find("Coins").GetComponent<TMP_Text>().text = data["response"][i]["ptChips"].ToString();
            obj.transform.Find("Coins").gameObject.SetActive(false);
            obj.transform.Find("SelectedToggle").gameObject.SetActive(true);

            StartCoroutine(LoadSpriteImageFromUrl(data["response"][i]["profileImage"].ToString(), obj.transform.Find("Image").GetComponent<Image>()));
        }


    }

    IEnumerator LoadSpriteImageFromUrl(string URL, Image image)
    {
        UnityWebRequest unityWebRequest = UnityWebRequestTexture.GetTexture(URL);
        yield return unityWebRequest.SendWebRequest();

        if (unityWebRequest.isNetworkError || unityWebRequest.isHttpError)
        {
            Debug.LogError("Download failed");
        }
        else
        {
            var Text = DownloadHandlerTexture.GetContent(unityWebRequest);
            Sprite sprite = Sprite.Create(Text, new Rect(0, 0, Text.width, Text.height), Vector2.zero);
            image.sprite = sprite;

            //Debug.Log("Successfully Set Player Profile");
        }
    }

    IEnumerator ShowPopUp(string msg, float delay)
    {
        popUpText.SetActive(true);
        popUpText.transform.GetChild(0).GetComponent<Text>().text = msg;
        yield return new WaitForSeconds(delay);
        popUpText.SetActive(false);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using System;
using UnityEngine.Networking;

public class MemberListUIManager : MonoBehaviour
{
    public static MemberListUIManager instance;
    public LayoutManager layoutManager;
    public GameObject oldMemberPrefab, newMemberPrefab,memberListScreen;
    public Transform container;
    public Button newMemberButton, oldMemberButton;
    public GameObject memeberPanel;
    public TMPro.TextMeshProUGUI MemberCountText;
    public GameObject MemberDetailsPanel;

    public List<ClubMemberDetails> newMembersList = new List<ClubMemberDetails>();
    public List<ClubMemberDetails> oldMembersList = new List<ClubMemberDetails>();

    public List<FilterButtonState> ClubMemberFilterButtons = new List<FilterButtonState>();
    public Text CurrentMemberListFilterName;
    public Image CurrentMemberListFilterImage;

    public Button MemberFilter;
    public GameObject MemberFilterPanel;

    //private void Awake()
    //{
    //    instance = this;
    //}

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }

        Debug.Log("MemberListUIManager Start Called...");
        if(ClubDetailsUIManager.instance.playerTypeForClub.Equals("Creater"))
        {
            //Debug.Log("I'm the Owner..");
        }
        else
        {
            memberListScreen.transform.Find("BG1/BG2/Menu").gameObject.SetActive(false);
            memeberPanel.transform.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0, 0, 0);
            memeberPanel.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(610, 660);
            //Debug.Log("Not Owner..");
        }

        FetchMembersList();
        OnClickOnButton("oldMember");

        MemberFilter.onClick.RemoveAllListeners();
        MemberFilter.onClick.AddListener(ToggleOpenMemberListFilter);

        for (int i = 0; i < ClubMemberFilterButtons.Count; i++)
        {
            ClubMemberFilterButtons[i].OnStateChange += MemberListUIManager_OnStateChange;
        }
    }

    //private void OnEnable()
    //{
        
    //}

    public void FetchMembersList()
    {
        string requestData = "{\"clubId\":\"" + ClubDetailsUIManager.instance.GetClubId() + "\"}";
        int limit = 0;
        string pendingUserRequestData = "{\"limit\":\"" +  limit.ToString() + "\"," + "\"clubId\":\"" + ClubDetailsUIManager.instance.GetClubId() + "\"}";

        //Debug.LogWarning("CLUB DETAILS CLUB ID-" + ClubDetailsUIManager.instance.GetClubId());
        //Debug.LogWarning("CLUB DETAILS UNIQUE CLUB ID-" + ClubDetailsUIManager.instance.GetClubUniqueId());


        //MainMenuController.instance.ShowScreen(MainMenuScreens.Loading);
        //old member list
        WebServices.instance.SendRequest(RequestType.GetClubMemberList, requestData, true, OnServerResponseFound);
        //new member list
        WebServices.instance.SendRequest(RequestType.GetPendingClubJoinRequest, pendingUserRequestData, true, OnServerResponseFound);
    }

    public void ToggleScreen(bool isShow)
    {
        if(isShow)
        {
            //Removing Old Members from list
            for (int i = 0; i < oldMembersList.Count; i++)
            {
                oldMembersList.RemoveAt(i);
            }
            //Removing New Members from List
            for (int i = 0; i < newMembersList.Count; i++)
            {
                newMembersList.RemoveAt(i);
            }
            for (int i = 0; i < oldMembersList.Count; i++)
            {
                oldMembersList.RemoveAt(i);
            }
            FetchMembersList();
        }
        //memberListScreen.SetActive(isShow);
    }


    public void OnClickOnButton(string eventName)
    {
        SoundManager.instance.PlaySound(SoundType.Click);


        switch (eventName)
        {
            case "back":
                {
                    ToggleScreen(false);
                }
            break;

            case "newMember":
                {
                    Color c = new Color(1, 1, 1, 1);
                    newMemberButton.GetComponent<Image>().color = c;

                    Color c1 = new Color(1, 1, 1, 0);
                    oldMemberButton.GetComponent<Image>().color = c1;

                    ShowMemberDetails(true);
                }
                break;

            case "oldMember":
                {
                    Color c = new Color(1, 1, 1, 1);
                    oldMemberButton.GetComponent<Image>().color = c;

                    Color c1 = new Color(1, 1, 1, 0);
                    newMemberButton.GetComponent<Image>().color = c1;

                    ShowMemberDetails(false);
                }
                break;

            default:
#if ERROR_LOG
            Debug.LogError("Unhandled event name found in MemberListUiManager = "+eventName);
#endif
            break;
        }

    }

    private void ShowMemberDetails(bool isShowNewMembers)
    {
        memeberPanel.SetActive(true);

        for (int i = 0; i < container.childCount; i++)
        {
            Destroy(container.GetChild(i).gameObject);
        }

        if (isShowNewMembers)
        {
            for (int i = 0; i < newMembersList.Count; i++)
            {
                ClubMemberDetails memberDetails = newMembersList[i];

                GameObject gm = Instantiate(newMemberPrefab,container) as GameObject;

                //Set Instantiated Object name to user name
                gm.name = memberDetails.userAlias;

                gm.transform.Find("TextName").GetComponent<TMPro.TextMeshProUGUI>().text = memberDetails.userAlias + " (" + "ID: " + memberDetails.userId + ")";
                gm.transform.Find("TextId").GetComponent<TMPro.TextMeshProUGUI>().text = "Referral ID : None ";
                gm.transform.Find("TextNickname").GetComponent<TMPro.TextMeshProUGUI>().text = "Nickname : " + memberDetails.userName;
                //string initial = memberDetails.userName.ToUpper();
                //initial = initial.Substring(0, 2);
                //gm.transform.Find("Image/Text (TMP)").GetComponent<TMPro.TextMeshProUGUI>().text = initial;

                if (memberDetails.profileImagePath.Length > 0)
                {
                    if(transform.gameObject.activeSelf)
                        StartCoroutine(LoadSpriteImageFromUrl(memberDetails.profileImagePath, gm.transform.Find("Image").GetComponent<Image>()));
                }
                gm.transform.Find("Reject").GetComponent<Button>().onClick.AddListener(() => newMemberButton.transform.Find("Notification").gameObject.SetActive(false));
                gm.transform.Find("Reject").GetComponent<Button>().onClick.AddListener(()=> ChangeUserRole(gm,true, memberDetails));
                gm.transform.Find("Approve").GetComponent<Button>().onClick.AddListener(() => newMemberButton.transform.Find("Notification").gameObject.SetActive(false));
                gm.transform.Find("Approve").GetComponent<Button>().onClick.AddListener(() => ChangeUserRole(gm,false, memberDetails));
            }
            MemberCountText.text = "Members : " + newMembersList.Count;
        }
        else
        {
            for (int i = 0; i < oldMembersList.Count; i++)
            {
                int x = i;

                GameObject gm = Instantiate(oldMemberPrefab, container) as GameObject;
                gm.SetActive(true);

                //Set Instantiated Object name to user name
                gm.name = oldMembersList[i].userAlias;

                gm.transform.Find("TextName").GetComponent<TMPro.TextMeshProUGUI>().text = oldMembersList[i].userAlias;
                gm.transform.Find("TextId").GetComponent<TMPro.TextMeshProUGUI>().text = "ID : " + oldMembersList[i].userId;
                gm.transform.Find("TextNickname").GetComponent<TMPro.TextMeshProUGUI>().text = "Nickname : " + oldMembersList[i].nickName;
                gm.transform.Find("Coins").GetComponent<TMPro.TextMeshProUGUI>().text = /*oldMembersList[i].creditChips*/"0";
                //string initial = oldMembersList[i].userName.ToUpper();
                //initial = initial.Substring(0, 2);
                //gm.transform.Find("Image/Text (TMP)").GetComponent<TMPro.TextMeshProUGUI>().text = initial;


                if (oldMembersList[i].profileImagePath.Length > 0)
                {
                    if (transform.gameObject.activeSelf)
                        StartCoroutine(LoadSpriteImageFromUrl(oldMembersList[i].profileImagePath, gm.transform.Find("Image").GetComponent<Image>()));
                }

                gm.GetComponent<Button>().onClick.RemoveAllListeners();
                //Debug.Log("Debug i value :" + i);

                if(ClubDetailsUIManager.instance.playerTypeForClub.Equals("Creater"))
                    gm.GetComponent<Button>().onClick.AddListener(() => OpenMemberDetailsPanel(x, gm));
            }
            MemberCountText.text = "Members : " + oldMembersList.Count;
        }

        //layoutManager.UpdateLayout();
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
            
            if(image!=null)
                image.sprite = sprite;

            //Debug.Log("Successfully Set Player Profile");
        }
    }

    private void OpenMemberDetailsPanel(int i, GameObject gm)
    {
        MemberDetailsPanel.SetActive(true);
        Debug.Log(oldMembersList.Count);
        Debug.Log("i is"  + i);
        MemberDetails.instance.Initialise(oldMembersList[i], gm);
    }

    public void ChangeUserRole(GameObject gm,bool isDeleteRequest, ClubMemberDetails memberDetails,ClubMemberRole roleToAssign = ClubMemberRole.Member)
    {
        if (isDeleteRequest)
        {
            string requestData = "{\"requestUserId\":\"" + memberDetails.userId + "\"," +
                    "\"assignRole\":\"" + roleToAssign + "\"," +
                    "\"requestStatus\":\"Deleted\"," +
                    "\"uniqueClubId\":\"" + ClubDetailsUIManager.instance.GetClubUniqueId() + "\"}";


            //string requestData = "{\"clubRequestId\":\"" + memberDetails.clubRequestId + "\"}";

            //MainMenuController.instance.ShowScreen(MainMenuScreens.Loading);
            //WebServices.instance.SendRequest(RequestType.DeleteUserJoinRequest, requestData, true, (requestType, serverResponse, isShowErrorMessage, errorMessage) =>

            WebServices.instance.SendRequest(RequestType.ChangePlayerRoleInClub, requestData, true, (requestType, serverResponse, isShowErrorMessage, errorMessage) =>
            {
                //MainMenuController.instance.DestroyScreen(MainMenuScreens.Loading);

                if (errorMessage.Length > 0)
                {
                    if (isShowErrorMessage)
                    {
                        MainMenuController.instance.ShowMessage(errorMessage);
                    }

                    return;
                }

                Debug.Log("Response => DeleteMemberRequest : " + serverResponse);
                JsonData data = JsonMapper.ToObject(serverResponse);

                if (data["success"].ToString() == "1")
                {
                    Destroy(gm);
                    newMembersList.Remove(memberDetails);
                                        
                    MemberCountText.text = "Members : " + newMembersList.Count;
                }
                else
                {
                    MainMenuController.instance.ShowMessage(data["message"].ToString());
                }
            });
        }
        else
        {
            string requestData = "{\"requestUserId\":\"" + memberDetails.userId + "\"," +
                "\"assignRole\":\"" + roleToAssign + "\"," +
                "\"requestStatus\":\"Approved\"," +
                "\"uniqueClubId\":\"" + ClubDetailsUIManager.instance.GetClubUniqueId() + "\"}";
            
            //MainMenuController.instance.ShowScreen(MainMenuScreens.Loading);
            WebServices.instance.SendRequest(RequestType.ChangePlayerRoleInClub, requestData, true, (requestType, serverResponse, isShowErrorMessage, errorMessage) =>
            {
                //MainMenuController.instance.DestroyScreen(MainMenuScreens.Loading);

                if (errorMessage.Length > 0)
                {
                    if (isShowErrorMessage)
                    {
                        MainMenuController.instance.ShowMessage(errorMessage);
                    }

                    return;
                }

                Debug.Log("Response => UserRoleChange: " + serverResponse);

                JsonData data = JsonMapper.ToObject(serverResponse);

                if (data["success"].ToString() == "1")
                {
                    Destroy(gm);
                    newMembersList.Remove(memberDetails);
                    MemberCountText.text = "Members : " + newMembersList.Count;

                    memberDetails.memberRole = roleToAssign;
                    //MemberDetails.instance.isRoleAssigned = true;
                    oldMembersList.Add(memberDetails);
                }
                else
                {
                    MainMenuController.instance.ShowMessage(data["message"].ToString());
                }
            });
        }        
    }

    private ClubMemberDetails clubOwner;

    public ClubMemberDetails GetClubOwnerObject()
    {
        return clubOwner;
    }

    public void ShowMemberDetails(JsonData data, bool newMembers = false)
    {
        for (int i = 0; i < container.childCount; i++)
        {
            Destroy(container.GetChild(i).gameObject);
        }        

        for (int i = 0; i < data["data"].Count; i++)
        {
            ClubMemberDetails clubMemberDetails = new ClubMemberDetails();
            clubMemberDetails.userId = data["data"][i]["requestUserId"].ToString();
            clubMemberDetails.userName = data["data"][i]["requestUserName"].ToString();

            if (data["data"][i]["userAlias"] == null || data["data"][i]["userAlias"].Equals(""))
            {
                clubMemberDetails.userAlias = clubMemberDetails.userName;
            }
            else
            {
                clubMemberDetails.userAlias = data["data"][i]["userAlias"].ToString();
            }

            if (data["data"][i]["note"] == null)
            {
                clubMemberDetails.userNote = "";
            }
            else
            {
                clubMemberDetails.userNote = data["data"][i]["note"].ToString();
            }

            clubMemberDetails.clubRequestId = data["data"][i]["clubRequestId"].ToString();
            clubMemberDetails.ptChips = data["data"][i]["ptChips"].ToString();
            clubMemberDetails.creditChips = data["data"][i]["creditChips"].ToString();
            clubMemberDetails.profileImagePath = data["data"][i]["profileImage"].ToString();
            

            if (!newMembers)
            {
                clubMemberDetails.nickName = data["data"][i]["nickName"].ToString();
            }
            else
            {

            }

            switch (data["data"][i]["assignRole"].ToString())
            {
                case "Creater":
                    {
                        clubMemberDetails.memberRole = ClubMemberRole.Owner;
                        clubOwner = clubMemberDetails;
                    }
                    break;

                case "Manager":
                    {
                        clubMemberDetails.memberRole = ClubMemberRole.Manager;
                    }
                break;

                case "Agent":
                    {
                        clubMemberDetails.memberRole = ClubMemberRole.Agent;
                    }
                    break;

                case "Member":
                    {
                        clubMemberDetails.memberRole = ClubMemberRole.Member;
                    }
                    break;

                default:
                    {
                        clubMemberDetails.memberRole = ClubMemberRole.NotAssigned;
                    }
                break;
            }

            if (clubMemberDetails.memberRole == ClubMemberRole.NotAssigned)
            {
                newMembersList.Add(clubMemberDetails);
            }
            else
            {
                oldMembersList.Add(clubMemberDetails);
            }

            OnClickOnButton("oldMember");
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

        //if(requestType == RequestType.ChangePlayerRoleInClub)
        //{
        //    UnityEngine.Debug.Log(serverResponse);
        //}

        if(requestType == RequestType.GetPendingClubJoinRequest)
        {
            Debug.Log("Response => GetPendingClubJoinRequest: " + serverResponse);
            JsonData data = JsonMapper.ToObject(serverResponse);
            if (data["status"].Equals(true))
            {
                newMemberButton.transform.Find("Notification").gameObject.SetActive(true);
                ShowMemberDetails(data,true);
            }
            else
            {
                newMemberButton.transform.Find("Notification").gameObject.SetActive(false);
                //MainMenuController.instance.ShowMessage(data["message"].ToString());
            }
        }
        else if (requestType == RequestType.GetClubMemberList)
        {
            //Debug.Log("Response GetClubDetailsByUserId: " + serverResponse);
            Debug.Log("Response => GetClubMemberListByClubId: " + serverResponse);
            JsonData data = JsonMapper.ToObject(serverResponse);

            if (data["status"].Equals(true))
            {
                for (int i = 0; i < container.childCount; i++)
                {
                    Destroy(container.GetChild(i).gameObject);
                }

                ShowMemberDetails(data);
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

    private void ToggleOpenMemberListFilter()
    {
        if (MemberFilterPanel.activeInHierarchy)
        {
            MemberFilterPanel.SetActive(false);
        }
        else
        {
            MemberFilterPanel.SetActive(true);
        }
    }

    private void MemberListUIManager_OnStateChange(FilterState stateType, string stateName, string PanelName)
    {
        CurrentMemberListFilterName.text = stateName;
        if(stateType == FilterState.Ascending)
        {
            CurrentMemberListFilterImage.transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            CurrentMemberListFilterImage.transform.localScale = new Vector3(1, -1, 1);
        }

        for (int i = 0; i < ClubMemberFilterButtons.Count; i++)
        {
            string name = ClubMemberFilterButtons[i].GetStateName();
            if(stateName != name)
            {
                ClubMemberFilterButtons[i].UpdateState(FilterState.None);
            }
            else
            {
                ClubMemberFilterButtons[i].UpdateState(stateType);            
            }
        }

        //sort based on statename and type
        switch (stateName)
        {
            case "Fee":
                break;
            case "SpinUp Buy-In":
                break;
            case "Winnings":
                break;
            case "Hand":
                break;
            case "LastLogin":
                break;
            case "LastPlayed":
                break;
            case "OldMember":
                break;
            case "NewMember":
                break;
            case "ActiveMember":
            default:
                break;
        }
        MemberFilterPanel.SetActive(false);
    }

}

[System.Serializable]
public class ClubMemberDetails
{
    public string profileImagePath;
    public string userId;
    public string clubRequestId;
    public string userName;
    public string nickName;
    public string ptChips;
    public string userAlias;
    public string userNote;
    public string creditChips;

    public ClubMemberRole memberRole;
}


public enum ClubMemberRole
{
    Owner,
    Manager,
    Agent,
    Member,
    NotAssigned
}

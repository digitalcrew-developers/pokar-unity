using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using System;

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

    private List<ClubMemberDetails> newMembersList = new List<ClubMemberDetails>();
    private List<ClubMemberDetails> oldMembersList = new List<ClubMemberDetails>();

    public List<FilterButtonState> ClubMemberFilterButtons = new List<FilterButtonState>();
    public Text CurrentMemberListFilterName;
    public Image CurrentMemberListFilterImage;

    public Button MemberFilter;
    public GameObject MemberFilterPanel;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        FetchMembersList();

        MemberFilter.onClick.RemoveAllListeners();
        MemberFilter.onClick.AddListener(ToggleOpenMemberListFilter);

        for (int i = 0; i < ClubMemberFilterButtons.Count; i++)
        {
            ClubMemberFilterButtons[i].OnStateChange += MemberListUIManager_OnStateChange;
        }
    }

    public void FetchMembersList()
    {
        string requestData = "{\"clubId\":\"" + ClubDetailsUIManager.instance.GetClubId() + "\"}";
        int limit = 0;
        string pendingUserRequestData = "{\"limit\":\"" +  limit.ToString() + "\"," + "\"clubId\":\"" + ClubDetailsUIManager.instance.GetClubId() + "\"}";

        Debug.LogWarning("CLUB DETAILS CLUB ID-" + ClubDetailsUIManager.instance.GetClubId());
        Debug.LogWarning("CLUB DETAILS UNIQUE CLUB ID-" + ClubDetailsUIManager.instance.GetClubUniqueId());


        MainMenuController.instance.ShowScreen(MainMenuScreens.Loading);
        //old member list
        WebServices.instance.SendRequest(RequestType.GetClubMemberList, requestData, true, OnServerResponseFound);
        //new member list
        WebServices.instance.SendRequest(RequestType.GetPendingClubJoinRequest, pendingUserRequestData, true, OnServerResponseFound);
    }

    public void ToggleScreen(bool isShow)
    {
        memberListScreen.SetActive(isShow);
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
                gm.transform.Find("TextName").GetComponent<TMPro.TextMeshProUGUI>().text = memberDetails.userName + " (" + "ID: " + memberDetails.userId + ")";
                gm.transform.Find("TextId").GetComponent<TMPro.TextMeshProUGUI>().text = "Referral ID : None ";
                gm.transform.Find("TextNickname").GetComponent<TMPro.TextMeshProUGUI>().text = "Nickname : " + memberDetails.nickName;
                string initial = memberDetails.userName.ToUpper();
                initial = initial.Substring(0, 2);
                gm.transform.Find("Image/Text (TMP)").GetComponent<TMPro.TextMeshProUGUI>().text = initial;

                gm.transform.Find("Reject").GetComponent<Button>().onClick.AddListener(()=> ChangeUserRole(gm,true, memberDetails));
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

                gm.transform.Find("TextName").GetComponent<TMPro.TextMeshProUGUI>().text = oldMembersList[i].userName;
                gm.transform.Find("TextId").GetComponent<TMPro.TextMeshProUGUI>().text = "ID : " + oldMembersList[i].userId;
                gm.transform.Find("TextNickname").GetComponent<TMPro.TextMeshProUGUI>().text = "Nickname : " + oldMembersList[i].nickName;
                gm.transform.Find("Coins").GetComponent<TMPro.TextMeshProUGUI>().text = oldMembersList[i].ptChips;
                string initial = oldMembersList[i].userName.ToUpper();
                initial = initial.Substring(0, 2);
                gm.transform.Find("Image/Text (TMP)").GetComponent<TMPro.TextMeshProUGUI>().text = initial;

                gm.GetComponent<Button>().onClick.RemoveAllListeners();
                Debug.Log("Debug i value :" + i);
                gm.GetComponent<Button>().onClick.AddListener(() => OpenMemberDetailsPanel(x, gm));
            }
            MemberCountText.text = "Members : " + oldMembersList.Count;
        }

        //layoutManager.UpdateLayout();
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


            WebServices.instance.SendRequest(RequestType.ChangePlayerRoleInClub, requestData, true, OnServerResponseFound);

            //string requestData = "{\"clubRequestId\":\"" + memberDetails.clubRequestId + "\"}";

            //MainMenuController.instance.ShowScreen(MainMenuScreens.Loading);
            //WebServices.instance.SendRequest(RequestType.DeleteUserJoinRequest, requestData, true, (requestType, serverResponse, isShowErrorMessage, errorMessage) =>
            //{
            //    MainMenuController.instance.DestroyScreen(MainMenuScreens.Loading);

            //    if (errorMessage.Length > 0)
            //    {
            //        if (isShowErrorMessage)
            //        {
            //            MainMenuController.instance.ShowMessage(errorMessage);
            //        }

            //        return;
            //    }

            //    JsonData data = JsonMapper.ToObject(serverResponse);

            //    if (data["success"].ToString() == "1")
            //    {
            //        Destroy(gm);
            //        newMembersList.Remove(memberDetails);
            //        MemberCountText.text = "Members : " + newMembersList.Count;
            //    }
            //    else
            //    {
            //        MainMenuController.instance.ShowMessage(data["message"].ToString());
            //    }
            //});
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

                Debug.Log("Response User Role Change: " + serverResponse);

                JsonData data = JsonMapper.ToObject(serverResponse);

                if (data["success"].ToString() == "1")
                {
                    Destroy(gm);
                    newMembersList.Remove(memberDetails);
                    MemberCountText.text = "Members : " + newMembersList.Count;

                    memberDetails.memberRole = roleToAssign;
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

    private void ShowMemberDetails(JsonData data, bool newMembers = false)
    {
        for (int i = 0; i < data["data"].Count; i++)
        {
            ClubMemberDetails clubMemberDetails = new ClubMemberDetails();
            clubMemberDetails.userId = data["data"][i]["requestUserId"].ToString();
            clubMemberDetails.userName = data["data"][i]["requestUserName"].ToString();
            clubMemberDetails.clubRequestId = data["data"][i]["clubRequestId"].ToString();
            clubMemberDetails.ptChips = data["data"][i]["ptChips"].ToString();

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
            JsonData data = JsonMapper.ToObject(serverResponse);
            if (data["status"].Equals(true))
            {
                ShowMemberDetails(data,true);
            }
            else
            {
                //MainMenuController.instance.ShowMessage(data["message"].ToString());
            }
        }
        else
        if (requestType == RequestType.GetClubMemberList)
        {
            Debug.Log("Response ClubMemberList: " + serverResponse);
            JsonData data = JsonMapper.ToObject(serverResponse);

            if (data["status"].Equals(true))
            {
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
    public string userId;
    public string clubRequestId;
    public string userName;
    public string nickName;
    public string ptChips;

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

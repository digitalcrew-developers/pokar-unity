using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;



public class MemberListUIManager : MonoBehaviour
{
    public static MemberListUIManager instance;
    public LayoutManager layoutManager;
    public GameObject oldMemberPrefab, newMemberPrefab,memberListScreen;
    public Transform container;
    public Button newMemberButton, oldMemberButton;

    [SerializeField]
    private List<ClubMemberDetails> newMembersList = new List<ClubMemberDetails>();

    [SerializeField]
    private List<ClubMemberDetails> oldMembersList = new List<ClubMemberDetails>();



    private void Awake()
    {
        instance = this;
    }


    private void Start()
    {
        FetchMembersList();
    }



    private void FetchMembersList()
    {
        string requestData = "{\"clubId\":\"" + ClubDetailsUIManager.instance.GetClubId() + "\"}";

        MainMenuController.instance.ShowScreen(MainMenuScreens.Loading);
        WebServices.instance.SendRequest(RequestType.GetClubMemberList, requestData, true, OnServerResponseFound);
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
                    newMemberButton.interactable = false;
                    oldMemberButton.interactable = true;

                    ShowMemberDetails(true);
                }
                break;

            case "oldMember":
                {
                    newMemberButton.interactable = true;
                    oldMemberButton.interactable = false;

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
                gm.transform.Find("Name").GetComponent<Text>().text = memberDetails.userName;
                gm.transform.Find("Id").GetComponent<Text>().text = "ID : " + memberDetails.userId;
                gm.transform.Find("Reject").GetComponent<Button>().onClick.AddListener(()=> ChangeUserRole(gm,true, memberDetails));
                gm.transform.Find("Approve").GetComponent<Button>().onClick.AddListener(() => ChangeUserRole(gm,false, memberDetails));
            }
        }
        else
        {
            for (int i = 0; i < oldMembersList.Count; i++)
            {
                GameObject gm = Instantiate(oldMemberPrefab, container) as GameObject;
                gm.transform.Find("Name").GetComponent<Text>().text = oldMembersList[i].userName;
                gm.transform.Find("Id").GetComponent<Text>().text = "ID : " + oldMembersList[i].userId;
            }
        }

        layoutManager.UpdateLayout();
    }


    public void ChangeUserRole(GameObject gm,bool isDeleteRequest, ClubMemberDetails memberDetails,ClubMemberRole roleToAssign = ClubMemberRole.Member)
    {
        if (isDeleteRequest)
        {
            string requestData = "{\"clubRequestId\":\"" + memberDetails.clubRequestId + "\"}";

            MainMenuController.instance.ShowScreen(MainMenuScreens.Loading);
            WebServices.instance.SendRequest(RequestType.DeleteUserJoinRequest, requestData, true, (requestType, serverResponse, isShowErrorMessage, errorMessage) =>
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

                JsonData data = JsonMapper.ToObject(serverResponse);

                if (data["success"].ToString() == "1")
                {
                    Destroy(gm);
                    newMembersList.Remove(memberDetails);
                }
                else
                {
                    MainMenuController.instance.ShowMessage(data["message"].ToString());
                }
            });
        }
        else
        {
            string requestData = "{\"requestUserId\":\"" + memberDetails.userId + "\","+
            "\"assignRole\":\"" + roleToAssign + "\"," +
            "\"requestStatus\":\"Approved\","+
            "\"uniqueClubId\":\"" + ClubDetailsUIManager.instance.GetClubUniqueId() + "\"}";

            

            MainMenuController.instance.ShowScreen(MainMenuScreens.Loading);
            WebServices.instance.SendRequest(RequestType.ChangePlayerRoleInClub, requestData, true, (requestType, serverResponse, isShowErrorMessage, errorMessage) =>
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

                JsonData data = JsonMapper.ToObject(serverResponse);

                if (data["success"].ToString() == "1")
                {
                    Destroy(gm);
                    newMembersList.Remove(memberDetails);

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




    private void ShowMemberDetails(JsonData data)
    {
        for (int i = 0; i < data["data"].Count; i++)
        {
            ClubMemberDetails clubMemberDetails = new ClubMemberDetails();
            clubMemberDetails.userId = data["data"][i]["requestUserId"].ToString();
            clubMemberDetails.userName = data["data"][i]["userName"].ToString();
            clubMemberDetails.clubRequestId = data["data"][i]["clubRequestId"].ToString();


            switch (data["data"][i]["assignRole"].ToString())
            {
                case "Creater":
                    {
                        clubMemberDetails.memberRole = ClubMemberRole.Owner;
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

        if (requestType == RequestType.GetClubMemberList)
        {
            JsonData data = JsonMapper.ToObject(serverResponse);

            if (data["status"].Equals(true))
            {
                ShowMemberDetails(data);
            }
            else
            {
                MainMenuController.instance.ShowMessage(data["message"].ToString());
            }
        }
        else
        {

#if ERROR_LOG
            Debug.LogError("Unhandled server requestType found  " + requestType);
#endif

        }

    }
}

[System.Serializable]
public class ClubMemberDetails
{
    public string userId;
    public string clubRequestId;
    public string userName;

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
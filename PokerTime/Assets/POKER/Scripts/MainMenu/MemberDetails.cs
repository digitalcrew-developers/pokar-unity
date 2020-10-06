using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class MemberDetails : MonoBehaviour
{
    public static MemberDetails instance;

    public TextMeshProUGUI UserID, UserName, Remark, LastLogin;
    public Image ProfileImage;
    public Button EditProfileBtn;

    public List<Button> TabButtons;

    public TextMeshProUGUI Winnings, Hands, BB, MTT, SpinBuy, Fee;
    public Button TrackPlayerBtn, DeleteMemberBtn;
    public Toggle Manager, Agent, Member;
    public List<GameObject> AdditionalObjects;

    private ClubMemberDetails clubMemberDetails;

    private float fullHeight = 978.8948f;
    private float halfHeight = 678f;
    private float fullWidth = 637.2f;

    private GameObject ListObject;

    private void Awake()
    {
        if(null== instance)
        {
            instance = this;
        }
    }

    /// <summary>
    /// Initialise needs to be called for each time member
    /// screen is opened
    /// </summary>
    public void Initialise(ClubMemberDetails _clubMemberDetails, GameObject ListObject)
    {
        //fill general details from previously present data 
        clubMemberDetails = _clubMemberDetails;
        UserName.text = _clubMemberDetails.userName;
        UserID.text = "ID: " + clubMemberDetails.userId + " | " + "Nickname " + clubMemberDetails.nickName;

        //call api and fill rest details

        Manager.onValueChanged.RemoveAllListeners();
        Agent.onValueChanged.RemoveAllListeners();
        Member.onValueChanged.RemoveAllListeners();

        Manager.onValueChanged.AddListener(delegate {
            ManagerToogleValueChanged(Manager);
        });
        Agent.onValueChanged.AddListener(delegate {
            AgentToogleValueChanged(Agent);
        });
        Member.onValueChanged.AddListener(delegate {
            MemberToogleValueChanged(Member);
        });

        /*
        if(clubMemberDetails.memberRole == ClubMemberRole.Owner)
        {
            transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(fullWidth, halfHeight);
            foreach (GameObject g in AdditionalObjects)
            {
                g.SetActive(false);
            }
        }
        else
        {
            transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(fullWidth, fullHeight);
            foreach (GameObject g in AdditionalObjects)
            {
                g.SetActive(true);
            }
        }
        */

        DeleteMemberBtn.onClick.RemoveAllListeners();
        DeleteMemberBtn.onClick.AddListener(DeleteMember);
    }

    private void ManagerToogleValueChanged(object classicToogle)
    {
        CallUpdatePlayer(ClubMemberRole.Manager);
    }

    private void AgentToogleValueChanged(object classicToogle)
    {
        CallUpdatePlayer(ClubMemberRole.Agent);
    }

    private void MemberToogleValueChanged(object listedToogle)
    {
        CallUpdatePlayer(ClubMemberRole.Member);
    }

    private void CallUpdatePlayer(ClubMemberRole clubMemberRole)
    {
        MemberListUIManager.instance.ChangeUserRole(ListObject,false, clubMemberDetails, clubMemberRole);
    }

    private void DeleteMember()
    {
        MemberListUIManager.instance.ChangeUserRole(ListObject, true, clubMemberDetails, ClubMemberRole.Member);
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
            default:
                break;
        }
    }

}
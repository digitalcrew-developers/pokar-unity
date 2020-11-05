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

    //DEV_CODE
    [Space(10)]
    public GameObject TipsObj;

    public TMP_InputField editMemberName, editMemberNote;

    private ClubMemberDetails clubMemberDetails;

    private float fullHeight = 979f;
    private float halfHeight = 690f;
    private float fullWidth = 650f;

    private GameObject ListObject;

    private void Awake()
    {
        if(null== instance)
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
                Agent.transform.Find("Image").GetComponent<Button>().interactable = true;
                Agent.isOn = true;
                if (!transform.Find("BG1/Heading/Career").gameObject.activeSelf)
                    transform.Find("BG1/Heading/Career").gameObject.SetActive(true);
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
            ManagerToogleValueChanged(Manager);
        });
        Agent.onValueChanged.AddListener(delegate
        {
            AgentToogleValueChanged(Agent);
        });
        Member.onValueChanged.AddListener(delegate
        {
            MemberToogleValueChanged(Member);
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

    private void Update()
    {
        //if(clubMemberDetails.memberRole == ClubMemberRole.Agent)
        //{
        //    if(!transform.Find("BG1/Heading/Career").gameObject.activeSelf)
        //        transform.Find("BG1/Heading/Career").gameObject.SetActive(true);

        //    if()
        //}
        //else
        //{
        //    if(transform.Find("BG1/Heading/Career").gameObject.activeSelf)
        //        transform.Find("BG1/Heading/Career").gameObject.SetActive(false);
        //}
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
        switch (clubMemberDetails.memberRole)
        {
            case ClubMemberRole.Agent:
                TipsObj.transform.Find("BG1/BG2/Text").GetComponent<Text>().text = "Current downlines' data will be removed if you chagne this Agent's rights. Are you sure?";
                TipsObj.transform.Find("BG1/BG2/BottomBtns/CancelBtn").GetComponent<Button>().onClick.AddListener(() =>
                    Agent.isOn = true);
                TipsObj.transform.Find("BG1/BG2/BottomBtns/CancelBtn").GetComponent<Button>().onClick.AddListener(() =>
                   TipsObj.SetActive(false));                
                break;

            case ClubMemberRole.Manager:
                TipsObj.transform.Find("BG1/BG2/Text").GetComponent<Text>().text = "Confirm to change user rights?";
                TipsObj.transform.Find("BG1/BG2/BottomBtns/CancelBtn").GetComponent<Button>().onClick.AddListener(() =>
                    Manager.isOn = true);
                TipsObj.transform.Find("BG1/BG2/BottomBtns/CancelBtn").GetComponent<Button>().onClick.AddListener(() =>
                   TipsObj.SetActive(false));
                break;

            case ClubMemberRole.Member:
                TipsObj.transform.Find("BG1/BG2/Text").GetComponent<Text>().text = "Confirm to change user rights?";
                TipsObj.transform.Find("BG1/BG2/BottomBtns/CancelBtn").GetComponent<Button>().onClick.AddListener(() =>
                    Member.isOn = true);
                TipsObj.transform.Find("BG1/BG2/BottomBtns/CancelBtn").GetComponent<Button>().onClick.AddListener(() =>
                   TipsObj.SetActive(false));
                break;
        }
        
        TipsObj.transform.Find("BG1/Heading/Text").GetComponent<Text>().text = "Tips";
        TipsObj.transform.Find("BG1/Heading/Close").gameObject.SetActive(false);
        TipsObj.transform.Find("BG1/BG2/BottomBtns/CancelBtn").gameObject.SetActive(true);
        TipsObj.transform.Find("BG1/BG2/BottomBtns/ConfirmBtn").GetComponent<Button>().onClick.AddListener(() =>
            MemberListUIManager.instance.ChangeUserRole(ListObject, false, clubMemberDetails, clubMemberRole));
        TipsObj.transform.Find("BG1/BG2/BottomBtns/ConfirmBtn").GetComponent<Button>().onClick.AddListener(() =>
            TipsObj.SetActive(false));

        if (clubMemberRole == ClubMemberRole.Agent)
        {
            TipsObj.transform.Find("BG1/BG2/BottomBtns/ConfirmBtn").GetComponent<Button>().onClick.AddListener(() =>
                Agent.transform.Find("Image").GetComponent<Button>().interactable = true);
            TipsObj.transform.Find("BG1/BG2/BottomBtns/ConfirmBtn").GetComponent<Button>().onClick.AddListener(() =>
                transform.Find("BG1/Heading/Career").gameObject.SetActive(true));
        }
        else
        {
            TipsObj.transform.Find("BG1/BG2/BottomBtns/ConfirmBtn").GetComponent<Button>().onClick.AddListener(() =>
                Agent.transform.Find("Image").GetComponent<Button>().interactable = false);
            TipsObj.transform.Find("BG1/BG2/BottomBtns/ConfirmBtn").GetComponent<Button>().onClick.AddListener(() =>
                transform.Find("BG1/Heading/Career").gameObject.SetActive(false));
        }

        TipsObj.SetActive(true);

        //TipsObj.SetActive(false);
        //MemberListUIManager.instance.ChangeUserRole(ListObject, false, clubMemberDetails, clubMemberRole);
    }

    private void DeleteMember()
    {
        TipsObj.transform.Find("BG1/Heading/Text").GetComponent<Text>().text = "Notice";
        TipsObj.transform.Find("BG1/Heading/Close").gameObject.SetActive(true);
        TipsObj.transform.Find("BG1/BG2/Text").GetComponent<Text>().text = "Wish to delete member?";
        TipsObj.transform.Find("BG1/BG2/BottomBtns/CancelBtn").gameObject.SetActive(false);
        TipsObj.transform.Find("BG1/BG2/BottomBtns/ConfirmBtn").GetComponent<Button>().onClick.AddListener(() =>
            MemberListUIManager.instance.ChangeUserRole(ListObject, true, clubMemberDetails, clubMemberDetails.memberRole)
            );

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
            default:
                break;
        }
    }

}
using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClubSleepMode : MonoBehaviour
{
    public static ClubSleepMode instance;

    public GameObject SleepModeListItemPrefab;
    public Transform SleepModeListContainer;

    [Header("SLEEP FILTERS")]
    public List<FilterButtonState> SleepButtons = new List<FilterButtonState>();
    public Text SleepFilterName;
    public Image SleepFilterImage;
    public Button SleepFilterBtn;
    public GameObject SleepFilterPanel;

    private List<ClubMemberDetails> SelectedSleepMembers = new List<ClubMemberDetails>();

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void OnEnable()
    {
        GetMembersListFromServer();
    }
    private void GetMembersListFromServer()
    {
        string requestData = "{\"clubId\":\"" + ClubDetailsUIManager.instance.GetClubId() + "\"}";
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

        switch (requestType)
        {
            case RequestType.GetClubMemberList:
                {
                    Debug.Log("Response GetClubMemberList: " + serverResponse);
                    JsonData data = JsonMapper.ToObject(serverResponse);
                    Debug.LogWarning("Club memeber list counter :" + serverResponse);
                    if (data["status"].Equals(true))
                    {
                        AddMembersToList(data);
                    }
                    else
                    {
                        //MainMenuController.instance.ShowMessage(data["message"].ToString());
                    }
                }
                break;

            default:
                Debug.LogError("Unhandled server requestType found  " + requestType);
                break;
        }
    }

    private void AddMembersToList(JsonData data)
    {
        for (int i = 0; i < data["data"].Count; i++)
        {
            int x = i;
            ClubMemberDetails clubMemberDetails = new ClubMemberDetails();
            clubMemberDetails.userId = data["data"][x]["requestUserId"].ToString();
            clubMemberDetails.userName = data["data"][x]["requestUserName"].ToString();
            clubMemberDetails.clubRequestId = data["data"][x]["clubRequestId"].ToString();
            clubMemberDetails.ptChips = data["data"][x]["ptChips"].ToString();

            Debug.Log("For Player: " + clubMemberDetails.userId + " - PT Chips : " + clubMemberDetails.ptChips);

            string initial = clubMemberDetails.userName.ToUpper();
            initial = initial.Substring(0, 2);

            GameObject sleepItem = Instantiate(SleepModeListItemPrefab, SleepModeListContainer) as GameObject;
            sleepItem.SetActive(true);

            sleepItem.transform.Find("TextName").GetComponent<TMPro.TextMeshProUGUI>().text = clubMemberDetails.userName;
            sleepItem.transform.Find("TextId").GetComponent<TMPro.TextMeshProUGUI>().text = "ID : " + clubMemberDetails.userId;
            sleepItem.transform.Find("TextNickname").GetComponent<TMPro.TextMeshProUGUI>().text = "Nickname : " + clubMemberDetails.nickName;
            sleepItem.transform.Find("Image/Coins").GetComponent<TMPro.TextMeshProUGUI>().text = clubMemberDetails.ptChips;
            sleepItem.transform.Find("Image/Text (TMP)").GetComponent<TMPro.TextMeshProUGUI>().text = initial;

            sleepItem.transform.Find("Toggle").GetComponent<Toggle>().onValueChanged.AddListener(delegate
            {
                ToggleValueChangedSleep(sleepItem.transform.Find("Toggle").GetComponent<Toggle>(), clubMemberDetails);
            });
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
}
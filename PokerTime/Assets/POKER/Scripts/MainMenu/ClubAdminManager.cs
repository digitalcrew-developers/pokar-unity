using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClubAdminManager : MonoBehaviour
{
    public static ClubAdminManager instance = null;
    public GameObject bottomPanel;
    public GameObject popUpText;
    public List<GameObject> AllScreens = new List<GameObject>();
    public Transform clubScreenLayer;
    
    private List<ClubActiveScreens> clubActiveScreens = new List<ClubActiveScreens>();


    #region JackpotSettings variables
    //[Header("JACKPOT")]
    //public string JackpotAmount = "";
    //public static bool isJackpotActivated = false;
    //public GameObject TipsObj;
    //public GameObject TopUpJackpotPrefab;
    //public Transform TopUpJackpotContainer;
    //public TMP_Text TotalJackpotAmountText;
    //public TMPro.TextMeshProUGUI ChipsAvailableText;
    //public TMPro.TMP_InputField JackpotAmountInputField;
    //public Button JackpotTopUpConfimButton;
    //public ToggleController JackpotToggleController;
    //public GameObject JackpotTopUpPopup, JackpotExplanationPanel, JackpotPayoutPanel, TopUpPanel, TopRecordPanel;
    //public Button JackpotExplanationButton, JackpotPayoutButton, TopUpButton,TopupTabButton,TopupRecordTabButton;
    //private bool isExplanationScreenOpen = true;
    //public bool IsJackpotEnabled { get => IsJackpotEnabled; }
    //private bool isJackpotEnabled { get; set; }
    //public GameObject JackpotPanel, TurnOffJackpotPanel;
    #endregion

    [Space(10)]

    #region ClubRating variables
    [Header("CLUB RATING")]
    public GameObject ClubRatingItemPrefab;
    public GameObject ClubRatingScrollParent;
    public List<GameObject> Stars;
    public Text DiamondsCountText;
    public TextMeshProUGUI LevelText;
    #endregion

    [Space(10)]

    #region Notifications variables
    [Header("NOTIFICATION")]
    public Image NotificationClubImage;
    public Text NotificationClubName;
    public GameObject NotificationPanel, NotTextPanel, NotImagePanel;
    public Button NotificationEditButton, NotificationHelpButton;
    public Button NotificationTextTabButton, NotificationImageTabButton;
    public TMP_InputField NotificationText;
    public TextMeshProUGUI OldNotificationText;
    public RawImage NotificationImage;
    public Button SendNotificationButton;
    #endregion

    [Space(10)]

    #region DisbandClub
    [Header("DISBAND CLUB")]
    public TMP_InputField ClubIdInputField;
    public Button DisbandClubConfirmButton;
    public Text DisbandClubIdText;
    public GameObject DisbandClub;
    #endregion

    [Space(10)]

    #region Preferences
    [Header("PREFERENCES")]
    public Toggle ListedToogle;
    public Toggle ClassicToogle;
    public GameObject FilterOnPosition, FilterOffPosition, FilterPanel, TogglePanel;
    public ToggleController PreferencesFilterToggle;
    public GameObject ClubDashboardFilterPanel;
    public GameObject PreferencesPanel;
    #endregion

    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
        }        
    }

    public void RequestJackpotAndMemberData()
    {
        ShowScreen(ClubScreens.Jackpot);
        ShowScreen(ClubScreens.Members);

        for (int i = 0; i < clubActiveScreens.Count; i++)
        {
            clubActiveScreens[i].screenObject.SetActive(false);
        }
    }

    /// <summary>
    /// temp. implementation. change later
    /// </summary>
    /// <param name="eventName"></param>
    public void OnClickOnButton(string eventName)
    {
        SoundManager.instance.PlaySound(SoundType.Click);

        CloseAllScreens();

        //to-do add null checks

        switch (eventName)
        {
            case "Notifications":
                AllScreens[0].SetActive(true);
                InitialiseNotifications();
                break;
            case "ClubRating":
                AllScreens[1].SetActive(true);
                InitialiseClubRating();
                break;
            case "Backpack":
                AllScreens[2].SetActive(true);
                break;
            case "Jackpot":
                //AllScreens[3].SetActive(true);
                //InitialiseJackpotScreen();
                ShowScreen(ClubScreens.Jackpot);
                break;
            case "Promotion":
                AllScreens[4].SetActive(true);
                break;
            case "Preferences":
                AllScreens[5].SetActive(true);
                InitialisePreferences();
                break;
            case "DisbandTheClub":
                AllScreens[6].SetActive(true);
                InitialiseDisbandClubScreen();
                break;
            case "Data":
                AllScreens[7].SetActive(true);
                ShowScreen(ClubScreens.Data);
                break;
            case "Counter":
                ShowScreen(ClubScreens.Counter);
                break;
            case "Exchange":
                ShowScreen(ClubScreens.Exchange);
                break;
            case "CreateTable":
                ShowScreen(ClubScreens.CreateTable);
                break;
            case "Members":
                ShowScreen(ClubScreens.Members);
                MemberListUIManager.instance.ToggleScreen(true);
                break;

            default:
                break;
        }
    }
    
    private void CloseAllScreens()
    {
        foreach(GameObject g in AllScreens)
        {
            if(null!=g)
                g.SetActive(false);
        }
    }

    #region Promotion
    private void GetPromotionImage()
    {
        //to-do. get promotion image from server
    }
    #endregion

    #region DisbandTheClub

    private void InitialiseDisbandClubScreen()
    {
        DisbandClubIdText.text = "ID: " + ClubDetailsUIManager.instance.GetClubUniqueId();
        DisbandClubConfirmButton.onClick.RemoveAllListeners();
        DisbandClubConfirmButton.onClick.AddListener(DisbandTheClub);
    }

    private void DisbandTheClub()
    {
        if (ClubIdInputField.text == ClubDetailsUIManager.instance.GetClubUniqueId())
        {
            MainMenuController.instance.ShowMessage("Are you sure you want to disband the club?", () => {
                bool j = ClubDetailsUIManager.instance.GetJackpotStatus();
                string jStatus = string.Empty;
                if (j) { jStatus = "1"; } else { jStatus = "0"; };

                string requestData = "{\"uniqueClubId\":\"" + ClubDetailsUIManager.instance.GetClubUniqueId() + "\"," +
                            "\"clubName\":\"" + ClubDetailsUIManager.instance.GetClubName() + "\"," +
                            "\"clubStatus\":\"" + "0" + "\"," +
                            "\"jackpotToggle\":\"" + jStatus + "\"," +
                            "\"layout\":\"" + ClubDetailsUIManager.instance.GetLayout()
                            + "\"}";

                WebServices.instance.SendRequest(RequestType.UpdateClub, requestData, true, OnServerResponseFound);

            }, "Confirm");
        }
        else
        {
            MainMenuController.instance.ShowMessage("Id does not match");
        }

    }
    #endregion

    #region Jackpot
    //private void InitialiseJackpotScreen()
    //{
    //    //To-Do.
    //    //1. set on click listeners
    //    //2.call api and check if jackpot is enabled, based on that update ui, get jackpot amount
    //    //3. get current chips amount, update in topup panel
    //    //4. update jackpot explanation and payout structure

    //    JackpotExplanationButton.onClick.RemoveAllListeners();
    //    JackpotPayoutButton.onClick.RemoveAllListeners();
    //    TopUpButton.onClick.RemoveAllListeners();
    //    TopupTabButton.onClick.RemoveAllListeners();
    //    TopupRecordTabButton.onClick.RemoveAllListeners();

    //    TopUpButton.onClick.AddListener(OpenTopupPopup);

    //    JackpotExplanationButton.onClick.AddListener(OpenJackpotExplanationPanel);
    //    JackpotPayoutButton.onClick.AddListener(OpenJackpotPayoutPanel);

    //    TopupTabButton.onClick.AddListener(OpenTopUpPanel);
    //    TopupRecordTabButton.onClick.AddListener(OpenTopRecordPanel);

    //    //Default Open TopupPanel
    //    OpenTopUpPanel();
    //    RequestJackpotDetails();

    //    JackpotToggleController.ToggleValueChanged += JackpotToggleController_ToggleValueChanged;

    //    JackpotTopUpConfimButton.onClick.RemoveAllListeners();
    //    JackpotTopUpConfimButton.onClick.AddListener(SendTopUpJackpotRequest);

    //    ChipsAvailableText.text = ClubDetailsUIManager.instance.CLubChips.text;
    //}

    //public void RequestJackpotDetails()
    //{
    //    string requestData = "{\"clubId\":\"" + ClubDetailsUIManager.instance.GetClubId() + "\"}"; 

    //    WebServices.instance.SendRequest(RequestType.GetJackpotDetailByClubId, requestData, true, OnServerResponseFound);
    //}

    //private void SendTopUpJackpotRequest()
    //{
    //    float amount = 0;
    //    float availableAmount = 0;
    //    float.TryParse(JackpotAmountInputField.text, out amount);
    //    float.TryParse(ChipsAvailableText.text, out availableAmount);
    //    //Debug.Log("Jackpot Amount: " + amount);
    //    //Debug.Log("Available Amount: " + availableAmount);
        
    //    if(amount > availableAmount)
    //    {
    //        //Debug.Log("Insufficient Amount...");
    //        StartCoroutine(ShowPopUp("Insufficient amount", 1.29f));
    //    }
    //    else if(amount == 0)
    //    {
    //        StartCoroutine(ShowPopUp("Please enter valid amount", 1.29f));
    //    }
    //    else
    //    {
    //        //Debug.Log("Available to topUp");
    //        string requestData = "{\"clubId\":\"" + ClubDetailsUIManager.instance.GetClubId() + "\"," +
    //                                 "\"userId\":\"" + MemberListUIManager.instance.GetClubOwnerObject().userId + "\"," +
    //                                 "\"jackpotAmount\":\"" + amount + "\"}";

    //        WebServices.instance.SendRequest(RequestType.TopUpJackpot, requestData, true, OnServerResponseFound);
    //    }
    //}

    //private void JackpotToggleController_ToggleValueChanged(bool val)
    //{
    //    if (isJackpotActivated)
    //    {
    //        ClubDetailsUIManager.instance.SetJackpotStatus(val);
    //        //Debug.Log("jackpot status :" + val.ToString());
    //        string b = string.Empty;
    //        if (val) { b = "1"; } else { b = "0"; }
    //        //Debug.Log(ClubDetailsUIManager.instance.GetClubUniqueId());

    //        string requestData = "{\"uniqueClubId\":\"" + ClubDetailsUIManager.instance.GetClubUniqueId() + "\"," +
    //                            "\"clubName\":\"" + ClubDetailsUIManager.instance.GetClubName() + "\"," +
    //                            "\"clubStatus\":\"" + "1" + "\"," +
    //                            "\"jackpotToggle\":\"" + b + "\"," +
    //                            "\"layout\":\"" + ClubDetailsUIManager.instance.GetLayout() //to-do. get layout from club details ui manager
    //                            + "\"}";

    //        WebServices.instance.SendRequest(RequestType.UpdateClub, requestData, true, OnServerResponseFound);

    //        if(!val)
    //        {
    //            TurnOffJackpotPanel.SetActive(true);
    //            TurnOffJackpotPanel.transform.Find("BG1/Heading/Close").GetComponent<Button>().onClick.RemoveAllListeners();
    //            TurnOffJackpotPanel.transform.Find("BG1/Heading/Close").GetComponent<Button>().onClick.AddListener(() => OnCloseTurnOffJackpotPanel());
    //            TurnOffJackpotPanel.transform.Find("BG1/BG2/CenterArea/Text (TMP)").GetComponent<TMP_Text>().text = "ID: " + ClubDetailsUIManager.instance.GetClubId();
    //            TurnOffJackpotPanel.transform.Find("BG1/BG2/CenterArea/BtnConfirm").GetComponent<Button>().onClick.AddListener(() => CheckToConfirm(val));
    //        }
    //        else if(val)
    //        {
    //            TurnOffJackpotPanel.SetActive(false);
    //            string req = "{\"clubId\":\"" + ClubDetailsUIManager.instance.GetClubId() + "\"," +
    //                         "\"status\":\"" + "Active" + "\"}";

    //            WebServices.instance.SendRequest(RequestType.OnOffJackpot, req, true, OnServerResponseFound);
    //        }            
    //    }
    //    else
    //    {
    //        if(val)
    //        {
    //            JackpotTopUpPopup.SetActive(true);
    //            OpenTopUpPanel();
    //        }            
    //    }
    //}
    
    //private void OnCloseTurnOffJackpotPanel()
    //{
    //    //JackpotToggleController.isOn = true;
    //    //JackpotToggleController.DoYourStaff();
    //    JackpotToggleController.isOn = true;
    //    JackpotToggleController.Toggle(true);
    //    TurnOffJackpotPanel.SetActive(false);
    //}

    //private void CheckToConfirm(bool val)
    //{
    //    if(TurnOffJackpotPanel.transform.Find("BG1/BG2/CenterArea/InputField (TMP)").GetComponent<TMP_InputField>().text.Length <= 0)
    //    {
    //        StartCoroutine(ShowPopUp("Enter Club ID", 1.29f));
    //    }
    //    else if(!TurnOffJackpotPanel.transform.Find("BG1/BG2/CenterArea/InputField (TMP)").GetComponent<TMP_InputField>().text.Equals(ClubDetailsUIManager.instance.GetClubId()))
    //    {
    //        StartCoroutine(ShowPopUp("ID incorrect", 1.29f));
    //    }
    //    else
    //    {
    //        TipsObj.SetActive(true);
    //        TipsObj.transform.Find("BG1/BG2/ConfirmDeleteMember").GetComponent<Button>().onClick.RemoveAllListeners();
    //        TipsObj.transform.Find("BG1/BG2/ConfirmDeleteMember").GetComponent<Button>().onClick.AddListener(() => OnConfirmDisableJackpot());
    //    }
    //}

    //private void OnConfirmDisableJackpot()
    //{
    //    string req = "{\"clubId\":\"" + ClubDetailsUIManager.instance.GetClubId() + "\"," +
    //                 "\"status\":\"" + "Inactive" + "\"}";

    //    WebServices.instance.SendRequest(RequestType.OnOffJackpot, req, true, OnServerResponseFound);
    //}


    //private void OpenTopupPopup()
    //{
    //    JackpotTopUpPopup.SetActive(true);
    //    OpenTopUpPanel();
    //}

    //private void OpenJackpotExplanationPanel()
    //{
    //    isExplanationScreenOpen = true;
    //    Color c = new Color(1, 1, 1, 1);
    //    JackpotExplanationButton.GetComponent<Image>().color = c;

    //    Color c1 = new Color(1, 1, 1, 0);
    //    JackpotPayoutButton.GetComponent<Image>().color = c1;

    //    JackpotExplanationPanel.SetActive(true);
    //    JackpotPayoutPanel.SetActive(false);

    //}

    //private void OpenJackpotPayoutPanel()
    //{
    //    isExplanationScreenOpen = false;
    //    Color c = new Color(1, 1, 1, 0);
    //    JackpotExplanationButton.GetComponent<Image>().color = c;

    //    Color c1 = new Color(1, 1, 1, 1);
    //    JackpotPayoutButton.GetComponent<Image>().color = c1;

    //    JackpotExplanationPanel.SetActive(false);
    //    JackpotPayoutPanel.SetActive(true);
    //}

    //private void OpenTopUpPanel()
    //{   
    //    //to-do.. get current chips

    //    Color c = new Color(1, 1, 1, 1);
    //    TopupTabButton.GetComponent<Image>().color = c;

    //    Color c1 = new Color(1, 1, 1, 0);
    //    TopupRecordTabButton.GetComponent<Image>().color = c1;

    //    TopUpPanel.SetActive(true);
    //    TopRecordPanel.SetActive(false);
    //}

    //public void OnCloseTopUpPanel()
    //{
    //    if(!isJackpotActivated)
    //    {
    //        JackpotToggleController.isOn = false;
    //        //JackpotToggleController.Toggle(false);
    //        JackpotToggleController.Toggle(false);
    //        JackpotTopUpPopup.SetActive(false);
    //    }
    //    else
    //    {
    //        JackpotToggleController.isOn = true;
    //        //JackpotToggleController.Toggle(false);
    //        JackpotToggleController.Toggle(true);
    //        JackpotTopUpPopup.SetActive(false);
    //    }
    //}

    //private void OpenTopRecordPanel()
    //{
    //    Color c = new Color(1, 1, 1, 0);
    //    TopupTabButton.GetComponent<Image>().color = c;

    //    Color c1 = new Color(1, 1, 1, 1);
    //    TopupRecordTabButton.GetComponent<Image>().color = c1;

    //    TopUpPanel.SetActive(false);
    //    TopRecordPanel.SetActive(true);

    //    WebServices.instance.SendRequest(RequestType.GetTopUpDetailsByClubId, "{\"clubId\":" + ClubDetailsUIManager.instance.GetClubId() + "}", true, OnServerResponseFound);
    //}

    #endregion

    #region ClubRating
    private void InitialiseClubRating()
    {
        //T0-Do. get ratings level information from server, total diamonds and club rating.
        //and instantiate in a loop
        //until then manually instantiate each item.

        string clubRatingRequest = "{\"userId\":\"" + PlayerManager.instance.GetPlayerGameData().userId + "\"," +
            "\"clubId\":\"" + ClubDetailsUIManager.instance.GetClubId() + "\"," +
            "\"uniqueClubId\":\"" + ClubDetailsUIManager.instance.GetClubUniqueId() + "\"," +
            "\"clubStatus\":\"" + "1"
            + "\"}";

        WebServices.instance.SendRequest(RequestType.GetClubDetails, clubRatingRequest, true, OnServerResponseFound);        

        //Fill Diamonds available
        DiamondsCountText.text = PlayerManager.instance.GetPlayerGameData().diamonds.ToString();

        if (ClubRatingScrollParent.transform.childCount > 0)
        {
            for (int i = 0; i < ClubRatingScrollParent.transform.childCount; i++)
            {
                Destroy(ClubRatingScrollParent.transform.GetChild(i).gameObject);
            }
        }

        GameObject gm = Instantiate(ClubRatingItemPrefab, ClubRatingScrollParent.transform) as GameObject;
        gm.GetComponent<ClubRatingItem>().Initialise("One-Star Club", "(valid for 30 days)", "Manager:3", "Member:60", "1,500","Club/LV 1");
        gm.SetActive(true);

        GameObject gm1 = Instantiate(ClubRatingItemPrefab, ClubRatingScrollParent.transform) as GameObject;
        gm1.GetComponent<ClubRatingItem>().Initialise("Two-Star Club", "(valid for 30 days)", "Manager:4", "Member:100", "2,500", "Club/LV 2");
        gm1.SetActive(true);

        GameObject gm2 = Instantiate(ClubRatingItemPrefab, ClubRatingScrollParent.transform) as GameObject;
        gm2.GetComponent<ClubRatingItem>().Initialise("Three-Star Club", "(valid for 30 days)", "Manager:5", "Member:150", "4,000", "Club/LV 3");
        gm2.SetActive(true);

        GameObject gm3 = Instantiate(ClubRatingItemPrefab, ClubRatingScrollParent.transform) as GameObject;
        gm3.GetComponent<ClubRatingItem>().Initialise("Four-Star Club", "(valid for 30 days)", "Manager:6", "Member:250", "8,000","Club/LV 4");
        gm3.SetActive(true);

        GameObject gm4 = Instantiate(ClubRatingItemPrefab, ClubRatingScrollParent.transform) as GameObject;
        gm4.GetComponent<ClubRatingItem>().Initialise("Five-Star Club", "(valid for 30 days)", "Manager:10", "Member:600", "20,000", "Club/LV 5");
        gm4.SetActive(true);

        //GameObject gm5 = Instantiate(ClubRatingItemPrefab, ClubRatingScrollParent.transform) as GameObject;
        //gm5.GetComponent<ClubRatingItem>().Initialise("Six-Star Club", "(valid for 30 days)", "Manager:12", "Member:800", "30,000");
        //gm5.SetActive(true);

        //GameObject gm6 = Instantiate(ClubRatingItemPrefab, ClubRatingScrollParent.transform) as GameObject;
        //gm6.GetComponent<ClubRatingItem>().Initialise("Seven-Star Club", "(valid for 30 days)", "Manager:15", "Member:1,200", "45,000");
        //gm6.SetActive(true);

        //GameObject gm7 = Instantiate(ClubRatingItemPrefab, ClubRatingScrollParent.transform) as GameObject;
        //gm7.GetComponent<ClubRatingItem>().Initialise("Eight-Star Club", "(valid for 30 days)", "Manager:20", "Member:1,500", "60,000");
        //gm7.SetActive(true);

        //GameObject gm8 = Instantiate(ClubRatingItemPrefab, ClubRatingScrollParent.transform) as GameObject;
        //gm8.GetComponent<ClubRatingItem>().Initialise("Nine-Star Club", "(valid for 30 days)", "Manager:25", "Member:1,800", "80,000");
        //gm8.SetActive(true);

        //GameObject gm9 = Instantiate(ClubRatingItemPrefab, ClubRatingScrollParent.transform) as GameObject;
        //gm9.GetComponent<ClubRatingItem>().Initialise("Ten-Star Club", "(valid for 30 days)", "Manager:35", "Member:2,500", "110,000");
        //gm9.SetActive(true);
    }
    #endregion

    #region Notification
    private void InitialiseNotifications()
    {
        //To-Do.. when opening notification screen. get data from server for existing notification sent
        //allow edit of existing text, image notification
        
        //OldNotificationText.text = ""; 
        
        //NotificationClubImage.sprite = ClubDetailsUIManager.instance.GetClubImage();
        //NotificationClubName.text = ClubDetailsUIManager.instance.GetClubName();

        NotificationEditButton.onClick.RemoveAllListeners();
        //NotificationHelpButton.onClick.RemoveAllListeners();

        NotificationTextTabButton.onClick.RemoveAllListeners();
        NotificationImageTabButton.onClick.RemoveAllListeners();

        SendNotificationButton.onClick.RemoveAllListeners();
        SendNotificationButton.onClick.AddListener(SendNotification);

        NotificationEditButton.onClick.AddListener(OpenNotificationPanel);
        //NotificationHelpButton.onClick.AddListener(OpenHelpPopup); 

        NotificationTextTabButton.onClick.AddListener(OpenNotificationTextPanel);
        NotificationImageTabButton.onClick.AddListener(OpenNotificationImagePanel);
    }

    private void SendNotification()
    {
        //to-do get text, and image and send to notification api
        string text = NotificationText.text;
        if (string.IsNullOrEmpty(text))
        {
            Debug.Log("text is empty");
            MainMenuController.instance.ShowMessage("Please make sure text or image is not empty", () => {
                Debug.Log("ok pressed");
            });
        }

        //send notification

        MainMenuController.instance.ShowMessage("Wish to send to all memebers?", () => {

            string requestData = "{\"clubId\":\"" + ClubDetailsUIManager.instance.GetClubId() + "\"," +
                            "\"title\":\"" + "Notification" + "\"," + "\"message\":\"" + text + "\"}";

            MainMenuController.instance.ShowScreen(MainMenuScreens.Loading);
            WebServices.instance.SendRequest(RequestType.PostNotification, requestData, true, OnServerResponseFound);
        },"Confirm");
    }

    private void LoadImageFromGallery()
    {

    }

    private void OpenNotificationPanel()
    {   
        NotificationPanel.SetActive(true);
    }

    private void OpenNotificationTextPanel()
    {
        //to-do.. get current chips

        Color c = new Color(1, 1, 1, 1);
        NotificationTextTabButton.GetComponent<Image>().color = c;

        Color c1 = new Color(1, 1, 1, 0);
        NotificationImageTabButton.GetComponent<Image>().color = c1;

        NotTextPanel.SetActive(true);
        NotImagePanel.SetActive(false);

    }

    private void OpenNotificationImagePanel()
    {
        Color c = new Color(1, 1, 1, 0);
        NotificationTextTabButton.GetComponent<Image>().color = c;

        Color c1 = new Color(1, 1, 1, 1);
        NotificationImageTabButton.GetComponent<Image>().color = c1;

        NotTextPanel.SetActive(false);
        NotImagePanel.SetActive(true);
    }

    IEnumerator ShowPopUp(string msg, float delay)
    {
        popUpText.SetActive(true);
        popUpText.transform.GetChild(0).GetComponent<Text>().text = msg;
        yield return new WaitForSeconds(delay);
        popUpText.SetActive(false);
    }
    #endregion

    #region Preferences

    private void InitialisePreferences()
    {
        ListedToogle.onValueChanged.RemoveAllListeners();
        ClassicToogle.onValueChanged.RemoveAllListeners();

        ListedToogle.onValueChanged.AddListener(delegate {
            ListedToogleValueChanged(ListedToogle);
        });
        ClassicToogle.onValueChanged.AddListener(delegate {
            ClassicToogleValueChanged(ClassicToogle);
        });

        PreferencesFilterToggle.ToggleValueChanged += PreferencesFilterToggle_ToggleValueChanged;
    }

    private void PreferencesFilterToggle_ToggleValueChanged(bool val)
    {
        if (val) {
            FilterPanel.SetActive(true);
            ClubDashboardFilterPanel.SetActive(true);
            TogglePanel.transform.position = FilterOnPosition.transform.position;
        }
        else
        {
            FilterPanel.SetActive(false);
            ClubDashboardFilterPanel.SetActive(false);
            TogglePanel.transform.position = FilterOffPosition.transform.position;
        }
    }

    private void ListedToogleValueChanged(Toggle change)
    {
        if (change.isOn)
        {
            SetLayout("Listed");
        }
    }

    private void ClassicToogleValueChanged(Toggle change)
    {
        if (change.isOn)
        {
            SetLayout("Classic");
        }
    }

    public void SetLayout(string layout)
    {
        switch (layout)
        {
            case "Listed":
                ClubDetailsUIManager.instance.SetLayout(ClubTableLayout.Listed);
                break;

            case "Classic":
                ClubDetailsUIManager.instance.SetLayout(ClubTableLayout.Classic);
                break;

            default:
                break;
        }

        bool j = ClubDetailsUIManager.instance.GetJackpotStatus();
        string jStatus = string.Empty;
        if (j) { jStatus = "1"; } else { jStatus = "0"; };

        string requestData = "{\"uniqueClubId\":\"" + ClubDetailsUIManager.instance.GetClubUniqueId() + "\"," +
                    "\"clubName\":\"" + ClubDetailsUIManager.instance.GetClubName() + "\"," +
                    "\"clubStatus\":\"" + "1" + "\"," +
                    "\"jackpotToggle\":\"" + jStatus + "\"," +
                    "\"layout\":\"" + ClubDetailsUIManager.instance.GetLayout() 
                    + "\"}";

        WebServices.instance.SendRequest(RequestType.UpdateClub, requestData, true, OnServerResponseFound);
    }

    #endregion

    #region ServerResponse

    public void OnServerResponseFound(RequestType requestType, string serverResponse, bool isShowErrorMessage, string errorMessage)
    {
        //Debug.Log("server response club admin : " + serverResponse);
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

            case RequestType.PostNotification:
                {
                    NotificationText.text = string.Empty;
                    NotificationPanel.SetActive(false);
                    MainMenuController.instance.ShowMessage("Notification sent to all members");
                }
                break;

            case RequestType.UpdateClub:
                {
                    JsonData data = JsonMapper.ToObject(serverResponse);
                    if(data["status"].Equals(true))
                    {
                        if (DisbandClub.activeInHierarchy)//api response is for disband club
                        {
                            MainMenuController.instance.ShowMessage("Club has been disbanded", () => {
                                ClubDetailsUIManager.instance.OnClickOnButton("back");
                                for (int i = 0; i < ClubListUiManager.instance.container.childCount; i++)
                                {
                                    Destroy(ClubListUiManager.instance.container.GetChild(i).gameObject);
                                }
                                ClubListUiManager.instance.FetchList(true);
                            });
                        }
                        if (PreferencesPanel.activeInHierarchy)//api response is for preferences
                        {

                        }
                        //if (JackpotPanel.activeInHierarchy)//api response is for jackpot
                        //{
                            
                        //}

                    }
                    else
                    {
                        MainMenuController.instance.ShowMessage(data["message"].ToString());
                    }
                }
                break;

            case RequestType.GetClubDetails:
                {
                    Debug.Log("Response => GetClubDetails: " + serverResponse.ToString());

                    JsonData data = JsonMapper.ToObject(serverResponse);
                    if (data["status"].Equals(true))
                    {
                        int rating = 2;//temp 

                        string stars = data["data"][0]["rating"].ToString();
                        Debug.Log("stars on club :" + stars);
                        int.TryParse(stars, out rating);

                        //Fill Stars        
                        foreach (GameObject g in Stars) { g.SetActive(true); };
                        if (rating > 0)
                        {
                            for (int i = 0; i < rating; i++)
                            {
                                Stars[i].SetActive(false);
                            }
                        }
                        //Fill Level
                        LevelText.text = "Lv." + rating.ToString();
                    }                    
                }
                break;

            //case RequestType.GetJackpotDetailByClubId:
            //    {
            //        Debug.Log("Response => GetJackpotDetailsByClubId : " + serverResponse);
            //        JsonData data = JsonMapper.ToObject(serverResponse);
            //        if (data["status"].Equals(true))
            //        {
            //            if (!isJackpotActivated)
            //                isJackpotActivated = true;

            //            int a = data["data"][0]["jackpotAmount"].ToString().Length;
            //            int b = TotalJackpotAmountText.text.Length;

            //            string str = "";
            //            for (int i = 0; i < (b-a); i++)
            //            {
            //                if (i == 1)
            //                {
            //                    str += ",";
            //                    continue;
            //                }
            //                else if (i == 5)
            //                {
            //                    str += ",";
            //                    continue;
            //                }
            //                str += "0";
            //            }

            //            str += data["data"][0]["jackpotAmount"].ToString();

            //            JackpotAmount = str;
            //            Debug.Log("Jackpot Amount: " + JackpotAmount);
            //            TotalJackpotAmountText.text = JackpotAmount;

            //            Debug.Log("Jackpot is active");
            //            if(data["data"][0]["jackpotStatus"].ToString().Equals("Active"))
            //            {
            //                JackpotToggleController.isOn = true;
            //                //JackpotToggleController.DoYourStaff();
            //                JackpotToggleController.Toggle(true); 
            //            }
            //            else
            //            {
            //                JackpotToggleController.isOn = false;
            //                //JackpotToggleController.DoYourStaff();
            //                JackpotToggleController.Toggle(false);
            //            }
            //        }
            //        else
            //        {
            //            Debug.Log("No Jackpot is available...");
            //            JackpotToggleController.isOn = false;
            //            //JackpotToggleController.DoYourStaff();
            //            JackpotToggleController.Toggle(false);
            //        }
            //    }
            //    break;

            //case RequestType.TopUpJackpot:
            //    {
            //        Debug.Log("Response => TopUpJackpot : " + serverResponse);
            //        JsonData data = JsonMapper.ToObject(serverResponse);
            //        if (data["status"].Equals(true)/*["message"].ToString() == "Jackpot topup successfully"*/)
            //        {
            //            JackpotToggleController.isOn = true;
            //            JackpotToggleController.Toggle(true);
            //            JackpotTopUpPopup.SetActive(false);
            //            JackpotAmountInputField.text = "";

            //            StartCoroutine(ShowPopUp("Topped Up Successfully", 1.29f));
            //        }
            //    }
            //    break;

            //case RequestType.OnOffJackpot:
            //    {
            //        Debug.Log("Response => OnOffJackpot : " + serverResponse);
            //        JsonData data = JsonMapper.ToObject(serverResponse);
            //        if (data["status"].Equals(true))
            //        {
            //            if (TipsObj.activeSelf)
            //                TipsObj.SetActive(false);

            //            if (TurnOffJackpotPanel.activeSelf)
            //            {
            //                TurnOffJackpotPanel.SetActive(false);
            //                TurnOffJackpotPanel.transform.Find("BG1/BG2/CenterArea/InputField (TMP)").GetComponent<TMP_InputField>().text = "";
            //                ClubDetailsUIManager.instance.jackpotData.SetActive(false);
            //            }

            //            ClubDetailsUIManager.instance.FetchJackpotDetails();
            //        }
            //    }
            //    break;


            //case RequestType.GetTopUpDetailsByClubId:
            //    {
            //        Debug.Log("Response => GetTopUpDetailsByClubId : " + serverResponse);
            //        JsonData data = JsonMapper.ToObject(serverResponse);
            //        if (data["success"].Equals(1))
            //        {
            //            for (int i = 0; i < TopUpJackpotContainer.childCount; i++)
            //            {
            //                Destroy(TopUpJackpotContainer.GetChild(0).gameObject);
            //            }

            //            for (int i = 0; i < data["data"].Count; i++)
            //            {
            //                GameObject gm = Instantiate(TopUpJackpotPrefab, TopUpJackpotContainer) as GameObject;
            //                gm.transform.Find("Data").GetComponent<TMP_Text>().gameObject.SetActive(false);
            //                gm.transform.Find("Name").GetComponent<TMP_Text>().text = MemberListUIManager.instance.GetClubOwnerObject().userAlias + " topped up";
            //                //gm.transform.Find("Name").GetComponent<RectTransform>().position = new Vector3(18, 12, 0);
            //                gm.transform.Find("Time").GetComponent<TMP_Text>().text = data["data"][i]["created"].ToString().Substring(0, 10) + " " + data["data"][i]["created"].ToString().Substring(11, 5);
            //                gm.transform.Find("Image/Coins").GetComponent<TMP_Text>().text = "-" + data["data"][i]["jackpotAmount"].ToString();
            //            }
            //        }
            //    }
            //    break;

            default:
#if ERROR_LOG
                Debug.LogError("Unhandled requestType found in  MenuHandller = " + requestType);
#endif
                break;
        }
    }
    #endregion

    #region SHOW_SCREEN
    public void ShowScreen(ClubScreens screenName, object[] parameter = null)
    {
        //if (screenName == MainMenuScreens.MainMenu)
        //{
        //    SwitchToMainMenu(true);
        //    return;
        //}

        //int layer = (int)GetScreenLayer(screenName);
        //for (int i = layer + 1; i < screenLayers.Length; i++)
        //{
        //    DestroyScreen();
        //}

        if (!IsScreenActive(screenName))
        {
            //DestroyScreen(GetScreenLayer(screenName));

            ClubActiveScreens clubScreen = new ClubActiveScreens();
            clubScreen.screenName = screenName;
            //clubScreen.screenLayer = clubScreenLayer;

            GameObject gm = Instantiate(AllScreens[(int)screenName], clubScreenLayer) as GameObject;
            clubScreen.screenObject = gm;
            clubActiveScreens.Add(clubScreen);
            gm.SetActive(true);
            switch (screenName)
            {
                //case MainMenuScreens.ClubDetails:
                //    gm.GetComponent<ClubDetailsUIManager>().Initialize((string)parameter[0], (string)parameter[1], (string)parameter[2], (string)parameter[3], (string)parameter[4], (string)parameter[5]);
                //    break;


                //case MainMenuScreens.GlobalTournament:
                //    {
                //        if (parameter != null)
                //        {
                //            gm.GetComponent<GlobalTournamentListUiManager>().ShowScreen((string)parameter[0]);
                //        }
                //        else
                //        {
                //            gm.GetComponent<GlobalTournamentListUiManager>().ShowScreen();
                //        }
                //    }
                //    break;


                //case MainMenuScreens.Shop:
                //    {
                //        if (parameter != null)
                //        {
                //            gm.GetComponent<ShopUiManager>().ShowScreen((string)parameter[0]);
                //        }
                //        else
                //        {
                //            gm.GetComponent<ShopUiManager>().ShowScreen();
                //        }
                //    }
                //    break;

                //case ClubScreens.Data:
                //    {

                //    }
                //    break;

                default:
                    break;
            }
        }
        else
        {
            for (int i = 0; i < clubActiveScreens.Count; i++)
            {
                if (clubActiveScreens[i].screenName == screenName)
                {
                    clubActiveScreens[i].screenObject.SetActive(true);
                }
                else
                {
                    clubActiveScreens[i].screenObject.SetActive(false);
                }
            }
        }
    }

    private bool IsScreenActive(ClubScreens screenName)
    {
        for (int i = 0; i < clubActiveScreens.Count; i++)
        {
            if (clubActiveScreens[i].screenName == screenName)
            {
                return true;
            }
        }

        return false;
    }
    #endregion
}

public enum AdminSettins
{
    Notifications,
    ClubRating,
    Backpack,
    Jackpot,
    Promotion,
    Preferences,
    DisbandTheClub
}

public class ClubActiveScreens
{
    public GameObject screenObject;
    public ClubScreens screenName;
    //public ScreenLayer screenLayer;
}

public enum ClubScreens
{
    Notifications,
    ClubRating,
    Backpack,
    Jackpot,
    Promotion,
    Preferences,
    DisbandClub,
    Data,
    Counter,
    Exchange,
    CreateTable,
    Members,
    GameData,
    ExpertData
}
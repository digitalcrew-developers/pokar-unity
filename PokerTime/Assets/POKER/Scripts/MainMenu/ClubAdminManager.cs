using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClubAdminManager : MonoBehaviour
{
    public static ClubAdminManager instance = null;
    public List<GameObject> AllScreens = new List<GameObject>();

    #region JackpotSettings variables
    [Header("JACKPOT")]
    public ToggleController JackpotToggleController;
    public GameObject JackpotTopUpPopup, JackpotExplanationPanel, JackpotPayoutPanel, TopUpPanel, TopRecordPanel;
    public Button JackpotExplanationButton, JackpotPayoutButton, TopUpButton,TopupTabButton,TopupRecordTabButton;
    private bool isExplanationScreenOpen = true;
    public bool IsJackpotEnabled { get => IsJackpotEnabled; }
    private bool isJackpotEnabled { get; set; }
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
    public RawImage NotificationImage;
    public Button SendNotificationButton;
    #endregion

    [Space(10)]

    #region DisbandClub
    [Header("DISBAND CLUB")]
    public TMP_InputField ClubIdInputField;
    public Button DisbandClubConfirmButton;
    public Text DisbandClubIdText;
    #endregion

    [Space(10)]

    #region Preferences
    [Header("PREFERENCES")]
    public Toggle ListedToogle;
    public Toggle ClassicToogle;
    #endregion

    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
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
                AllScreens[3].SetActive(true);
                InitialiseJackpotScreen();
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
    private void InitialiseJackpotScreen()
    {
        //To-Do.
        //1. set on click listeners
        //2.call api and check if jackpot is enabled, based on that update ui, get jackpot amount
        //3. get current chips amount, update in topup panel
        //4. update jackpot explanation and payout structure

        JackpotExplanationButton.onClick.RemoveAllListeners();
        JackpotPayoutButton.onClick.RemoveAllListeners();
        TopUpButton.onClick.RemoveAllListeners();
        TopupTabButton.onClick.RemoveAllListeners();
        TopupRecordTabButton.onClick.RemoveAllListeners();

        TopUpButton.onClick.AddListener(OpenTopupPopup);

        JackpotExplanationButton.onClick.AddListener(OpenJackpotExplanationPanel);
        JackpotPayoutButton.onClick.AddListener(OpenJackpotPayoutPanel);

        TopupTabButton.onClick.AddListener(OpenTopUpPanel);
        TopupRecordTabButton.onClick.AddListener(OpenTopRecordPanel);

        JackpotToggleController.ToggleValueChanged += JackpotToggleController_ToggleValueChanged;
    }

    private void JackpotToggleController_ToggleValueChanged(bool val)
    {
        ClubDetailsUIManager.instance.SetJackpotStatus(val);
        Debug.Log("jackpot status :" + val.ToString());
        string b = string.Empty;
        if (val) { b = "1"; } else { b = "0"; }
        Debug.Log(ClubDetailsUIManager.instance.GetClubUniqueId());

        string requestData = "{\"uniqueClubId\":\"" + ClubDetailsUIManager.instance.GetClubUniqueId() + "\"," +
                            "\"clubName\":\"" + ClubDetailsUIManager.instance.GetClubName() + "\"," +
                            "\"clubStatus\":\"" + "1" + "\"," +
                            "\"jackpotToggle\":\"" + b + "\"," +
                            "\"layout\":\"" + ClubDetailsUIManager.instance.GetLayout() //to-do. get layout from club details ui manager
                            + "\"}";

        WebServices.instance.SendRequest(RequestType.UpdateClub, requestData, true, OnServerResponseFound);
    }

    private void OpenTopupPopup()
    {
        JackpotTopUpPopup.SetActive(true);
        OpenTopUpPanel();
    }

    private void OpenJackpotExplanationPanel()
    {
        isExplanationScreenOpen = true;
        Color c = new Color(1, 1, 1, 1);
        JackpotExplanationButton.GetComponent<Image>().color = c;

        Color c1 = new Color(1, 1, 1, 0);
        JackpotPayoutButton.GetComponent<Image>().color = c1;

        JackpotExplanationPanel.SetActive(true);
        JackpotPayoutPanel.SetActive(false);

    }

    private void OpenJackpotPayoutPanel()
    {
        isExplanationScreenOpen = false;
        Color c = new Color(1, 1, 1, 0);
        JackpotExplanationButton.GetComponent<Image>().color = c;

        Color c1 = new Color(1, 1, 1, 1);
        JackpotPayoutButton.GetComponent<Image>().color = c1;

        JackpotExplanationPanel.SetActive(false);
        JackpotPayoutPanel.SetActive(true);
    }

    private void OpenTopUpPanel()
    {   
        //to-do.. get current chips

        Color c = new Color(1, 1, 1, 1);
        TopupTabButton.GetComponent<Image>().color = c;

        Color c1 = new Color(1, 1, 1, 0);
        TopupRecordTabButton.GetComponent<Image>().color = c1;

        TopUpPanel.SetActive(true);
        TopRecordPanel.SetActive(false);

    }

    private void OpenTopRecordPanel()
    {
        Color c = new Color(1, 1, 1, 0);
        TopupTabButton.GetComponent<Image>().color = c;

        Color c1 = new Color(1, 1, 1, 1);
        TopupRecordTabButton.GetComponent<Image>().color = c1;

        TopUpPanel.SetActive(false);
        TopRecordPanel.SetActive(true);
    }

    #endregion

    #region ClubRating
    private void InitialiseClubRating()
    {
        //T0-Do. get ratings level information from server, total diamonds and club rating.
        //and instantiate in a loop
        //until then manually instantiate each item.

        //Fill Stars        
        int rating = 5;//temp 
        foreach (GameObject g in Stars) { g.SetActive(true); };
        if (rating > 0)
        {
            for(int i = 0; i < rating; i++)
            {
                Stars[i].SetActive(false);
            }
        }
        //Fill Level
        LevelText.text = "Lv." + rating.ToString();

        //Fill Diamonds available
        DiamondsCountText.text = "0"; //temp value

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
       
        //NotificationClubImage.sprite = ClubDetailsUIManager.instance.GetClubImage();
        //NotificationClubName.text = ClubDetailsUIManager.instance.GetClubName();

        NotificationEditButton.onClick.RemoveAllListeners();
        //NotificationHelpButton.onClick.RemoveAllListeners();

        NotificationTextTabButton.onClick.RemoveAllListeners();
        NotificationImageTabButton.onClick.RemoveAllListeners();

        SendNotificationButton.onClick.RemoveAllListeners();

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
    {   //to-do
        //if existing notification data is available. fill it.
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
                    MainMenuController.instance.ShowMessage("Notification sent to all members");
                }
                break;

            case RequestType.UpdateClub:
                {

                }
                break;

            default:
#if ERROR_LOG
                Debug.LogError("Unhandled requestType found in  MenuHandller = " + requestType);
#endif
                break;
        }
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

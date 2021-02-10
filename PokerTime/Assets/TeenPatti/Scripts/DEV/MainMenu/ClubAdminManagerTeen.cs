using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClubAdminManagerTeen : MonoBehaviour
{
    public static ClubAdminManagerTeen instance = null;
    public GameObject bottomPanel;
    public GameObject popUpText;
    public List<GameObject> AllScreens = new List<GameObject>();
    public Transform clubScreenLayer;
    
    private List<ClubActiveScreensTeen> clubActiveScreens = new List<ClubActiveScreensTeen>();

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
        ShowScreen(ClubScreensTeen.Jackpot);
        ShowScreen(ClubScreensTeen.Members);

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
                ShowScreen(ClubScreensTeen.Jackpot);
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
                ShowScreen(ClubScreensTeen.Data);
                break;
            case "Counter":
                ShowScreen(ClubScreensTeen.Counter);
                break;
            case "Exchange":
                ShowScreen(ClubScreensTeen.Exchange);
                break;
            case "CreateTable":
                ShowScreen(ClubScreensTeen.CreateTable);
                break;
            case "Members":
                Debug.Log("Showing Member Screen");
                ShowScreen(ClubScreensTeen.Members);
                MemberListUIManagerTeen.instance.ToggleScreen(true);
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
        DisbandClubIdText.text = "ID: " + ClubDetailsUIManagerTeen.instance.GetClubUniqueId();
        DisbandClubConfirmButton.onClick.RemoveAllListeners();
        DisbandClubConfirmButton.onClick.AddListener(DisbandTheClub);
    }

    private void DisbandTheClub()
    {
        if (ClubIdInputField.text == ClubDetailsUIManagerTeen.instance.GetClubUniqueId())
        {
            MainMenuControllerTeen.instance.ShowMessage("Are you sure you want to disband the club?", () => {
                bool j = ClubDetailsUIManagerTeen.instance.GetJackpotStatus();
                string jStatus = string.Empty;
                if (j) { jStatus = "1"; } else { jStatus = "0"; };

                string requestData = "{\"uniqueClubId\":\"" + ClubDetailsUIManagerTeen.instance.GetClubUniqueId() + "\"," +
                            "\"clubName\":\"" + ClubDetailsUIManagerTeen.instance.GetClubName() + "\"," +
                            "\"clubStatus\":\"" + "0" + "\"," +
                            "\"jackpotToggle\":\"" + jStatus + "\"," +
                            "\"layout\":\"" + ClubDetailsUIManagerTeen.instance.GetLayout()
                            + "\"}";

                WebServices.instance.SendRequestTP(RequestTypeTP.UpdateClub, requestData, true, OnServerResponseFound);
                //WebServices.instance.SendRequestTP(RequestTypeTP.UpdateClub, requestData, true, (requestType, serverResponse, isShowErrorMessage, errorMessage) =>
                //{
                //    Debug.Log("Response => UpdateClub : " + serverResponse);
                //    JsonData data = JsonMapper.ToObject(serverResponse);
                //    if (data["status"].Equals(true))
                //    {
                //        if (DisbandClub.activeInHierarchy)//api response is for disband club
                //        {
                //            MainMenuControllerTeen.instance.ShowMessage("Club has been disbanded", () => {
                //                ClubDetailsUIManagerTeen.instance.OnClickOnButton("back");
                //                for (int i = 0; i < ClubListUiManagerTeen.instance.container.childCount; i++)
                //                {
                //                    Destroy(ClubListUiManagerTeen.instance.container.GetChild(i).gameObject);
                //                }
                //                ClubListUiManagerTeen.instance.FetchList(true);
                //            });
                //        }
                //        if (PreferencesPanel.activeInHierarchy)//api response is for preferences
                //        {

                //        }
                //    }
                //    else
                //    {
                //        MainMenuControllerTeen.instance.ShowMessage(data["message"].ToString());
                //    }
                //});

            }, "Confirm");
        }
        else
        {
            MainMenuControllerTeen.instance.ShowMessage("Id does not match");
        }

    }
    #endregion

    #region ClubRating
    private void InitialiseClubRating()
    {
        //T0-Do. get ratings level information from server, total diamonds and club rating.
        //and instantiate in a loop
        //until then manually instantiate each item.

        string clubRatingRequest = "{\"userId\":\"" + PlayerManager.instance.GetPlayerGameData().userId + "\"," +
            "\"clubId\":\"" + ClubDetailsUIManagerTeen.instance.GetClubId() + "\"," +
            "\"uniqueClubId\":\"" + ClubDetailsUIManagerTeen.instance.GetClubUniqueId() + "\"," +
            "\"clubStatus\":\"" + "1"
            + "\"}";

        WebServices.instance.SendRequestTP(RequestTypeTP.GetClubDetails, clubRatingRequest, true, OnServerResponseFound);        

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
        gm.GetComponent<ClubRatingItemTeen>().Initialise("One-Star Club", "(valid for 30 days)", "Manager:3", "Member:60", "1,500","Club/LV 1");
        gm.SetActive(true);

        GameObject gm1 = Instantiate(ClubRatingItemPrefab, ClubRatingScrollParent.transform) as GameObject;
        gm1.GetComponent<ClubRatingItemTeen>().Initialise("Two-Star Club", "(valid for 30 days)", "Manager:4", "Member:100", "2,500", "Club/LV 2");
        gm1.SetActive(true);

        GameObject gm2 = Instantiate(ClubRatingItemPrefab, ClubRatingScrollParent.transform) as GameObject;
        gm2.GetComponent<ClubRatingItemTeen>().Initialise("Three-Star Club", "(valid for 30 days)", "Manager:5", "Member:150", "4,000", "Club/LV 3");
        gm2.SetActive(true);

        GameObject gm3 = Instantiate(ClubRatingItemPrefab, ClubRatingScrollParent.transform) as GameObject;
        gm3.GetComponent<ClubRatingItemTeen>().Initialise("Four-Star Club", "(valid for 30 days)", "Manager:6", "Member:250", "8,000","Club/LV 4");
        gm3.SetActive(true);

        GameObject gm4 = Instantiate(ClubRatingItemPrefab, ClubRatingScrollParent.transform) as GameObject;
        gm4.GetComponent<ClubRatingItemTeen>().Initialise("Five-Star Club", "(valid for 30 days)", "Manager:10", "Member:600", "20,000", "Club/LV 5");
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
        
        //NotificationClubImage.sprite = ClubDetailsUIManagerTeen.instance.GetClubImage();
        //NotificationClubName.text = ClubDetailsUIManagerTeen.instance.GetClubName();

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
            MainMenuControllerTeen.instance.ShowMessage("Please make sure text or image is not empty", () => {
                Debug.Log("ok pressed");
            });
        }

        //send notification

        MainMenuControllerTeen.instance.ShowMessage("Wish to send to all memebers?", () => {

            string requestData = "{\"clubId\":\"" + ClubDetailsUIManagerTeen.instance.GetClubId() + "\"," +
                                 "\"unionId\":\"" + "" + "\"," +
                            "\"title\":\"" + "Notification" + "\"," + "\"message\":\"" + text + "\"}";

            MainMenuControllerTeen.instance.ShowScreen(MainMenuScreensTeen.Loading);
            WebServices.instance.SendRequestTP(RequestTypeTP.PostNotification, requestData, true, OnServerResponseFound);
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
                ClubDetailsUIManagerTeen.instance.SetLayout(ClubTableLayout.Listed);
                break;

            case "Classic":
                ClubDetailsUIManagerTeen.instance.SetLayout(ClubTableLayout.Classic);
                break;

            default:
                break;
        }

        bool j = ClubDetailsUIManagerTeen.instance.GetJackpotStatus();
        string jStatus = string.Empty;
        if (j) { jStatus = "1"; } else { jStatus = "0"; };

        string requestData = "{\"uniqueClubId\":\"" + ClubDetailsUIManagerTeen.instance.GetClubUniqueId() + "\"," +
                    "\"clubName\":\"" + ClubDetailsUIManagerTeen.instance.GetClubName() + "\"," +
                    "\"clubStatus\":\"" + "1" + "\"," +
                    "\"jackpotToggle\":\"" + jStatus + "\"," +
                    "\"layout\":\"" + ClubDetailsUIManagerTeen.instance.GetLayout() 
                    + "\"}";

        WebServices.instance.SendRequestTP(RequestTypeTP.UpdateClub, requestData, true, OnServerResponseFound);
        //WebServices.instance.SendRequestTP(RequestTypeTP.UpdateClub, requestData, true, (requestType, serverResponse, isShowErrorMessage, errorMessage) =>
        //{
        //    Debug.Log("Response => UpdateClub : " + serverResponse);
        //    JsonData data = JsonMapper.ToObject(serverResponse);
        //    if (data["status"].Equals(true))
        //    {
        //        if (DisbandClub.activeInHierarchy)//api response is for disband club
        //        {
        //            MainMenuControllerTeen.instance.ShowMessage("Club has been disbanded", () => {
        //                ClubDetailsUIManagerTeen.instance.OnClickOnButton("back");
        //                for (int i = 0; i < ClubListUiManagerTeen.instance.container.childCount; i++)
        //                {
        //                    Destroy(ClubListUiManagerTeen.instance.container.GetChild(i).gameObject);
        //                }
        //                ClubListUiManagerTeen.instance.FetchList(true);
        //            });
        //        }
        //        if (PreferencesPanel.activeInHierarchy)//api response is for preferences
        //        {

        //        }
        //    }
        //    else
        //    {
        //        MainMenuControllerTeen.instance.ShowMessage(data["message"].ToString());
        //    }
        //});
    }

    #endregion

    #region ServerResponse

    public void OnServerResponseFound(RequestTypeTP requestType, string serverResponse, bool isShowErrorMessage, string errorMessage)
    {
        //Debug.Log("server response club admin : " + serverResponse);
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
                case RequestTypeTP.PostNotification:
                {
                    Debug.Log("Response => PostNotification: " + serverResponse);
                    //NotificationText.text = string.Empty;
                    //NotificationPanel.SetActive(false);
                    MainMenuControllerTeen.instance.ShowMessage("Notification sent to all members");
                }
                break;

            case RequestTypeTP.UpdateClub:
                {
                    JsonData data = JsonMapper.ToObject(serverResponse);
                    if(data["status"].Equals(true))
                    {
                        if (DisbandClub.activeInHierarchy)//api response is for disband club
                        {
                            MainMenuControllerTeen.instance.ShowMessage("Club has been disbanded", () => {
                                ClubDetailsUIManagerTeen.instance.OnClickOnButton("back");
                                for (int i = 0; i < ClubListUiManagerTeen.instance.container.childCount; i++)
                                {
                                    Destroy(ClubListUiManagerTeen.instance.container.GetChild(i).gameObject);
                                }
                                ClubListUiManagerTeen.instance.FetchList(true);
                            });
                        }
                        //if (PreferencesPanel.activeInHierarchy)//api response is for preferences
                        //{

                        //}
                        //if (JackpotPanel.activeInHierarchy)//api response is for jackpot
                        //{
                            
                        //}
                    }
                    else
                    {
                        MainMenuControllerTeen.instance.ShowMessage(data["message"].ToString());
                    }
                }
                break;

            case RequestTypeTP.GetClubDetails:
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

            default:
#if ERROR_LOG
                Debug.LogError("Unhandled requestType found in  MenuHandller = " + requestType);
#endif
                break;
        }
    }
    #endregion

    #region SHOW_SCREEN
    public void ShowScreen(ClubScreensTeen screenName, object[] parameter = null)
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

            ClubActiveScreensTeen clubScreen = new ClubActiveScreensTeen();
            clubScreen.screenName = screenName;
            //clubScreen.screenLayer = clubScreenLayer;

            GameObject gm = Instantiate(AllScreens[(int)screenName], clubScreenLayer) as GameObject;
            clubScreen.screenObject = gm;
            clubActiveScreens.Add(clubScreen);
            gm.SetActive(true);
            switch (screenName)
            {
                //case MainMenuScreens.ClubDetails:
                //    gm.GetComponent<ClubDetailsUIManagerTeen>().Initialize((string)parameter[0], (string)parameter[1], (string)parameter[2], (string)parameter[3], (string)parameter[4], (string)parameter[5]);
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

                case ClubScreensTeen.Data:
                    {

                    }
                    break;

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

    private bool IsScreenActive(ClubScreensTeen screenName)
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

public enum AdminSettingsTeen
{
    Notifications,
    ClubRating,
    Backpack,
    Jackpot,
    Promotion,
    Preferences,
    DisbandTheClub
}

public class ClubActiveScreensTeen
{
    public GameObject screenObject;
    public ClubScreensTeen screenName;
    //public ScreenLayer screenLayer;
}

public enum ClubScreensTeen
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
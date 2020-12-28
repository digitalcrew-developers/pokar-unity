﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.IO;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class MainMenuControllerTeen : MonoBehaviour
{
    public static MainMenuControllerTeen instance;
    //public GameObject mainMenuPoker, mainMenuFlash;

    public GameObject bottomPanel/*, bottomPanelTeen*/;

    public GameObject[] screens; // All screens prefab
    public Transform[] screenLayers; // screen spawn parent

    //DEV_CODE
    public GameObject[] bottomMenus;

    public bool isVIPFromShop = false;
    public bool isVIPFromProfile = false;

    private List<MainMenuActiveScreenTeen> mainMenuActiveScreens = new List<MainMenuActiveScreenTeen>();
    private NotificationDetails notificationDetails = new NotificationDetails();


    private void Awake()
    {
        instance = this;
    }


    private void Start()
    {
        GlobalGameManager.IsJoiningPreviousGame = false;

        if (PlayerManager.instance.IsLogedIn())
        {
            SwitchToMainMenu();
        }
        else
        {            
            ShowScreen(MainMenuScreensTeen.Registration);
            bottomPanel.GetComponent<PaginationManager>().DisableScreens();
        }
        LoadVideoAndScreenshot();
        bottomPanel.GetComponent<PaginationManager>().PageToggleClickEvent += MainMenuController_PageToggleClickEvent;
    }

    public void OnClickPlayFlash()
    {
        GameConstants.poker = false;
        //mainMenuFlash.SetActive(true);
        //mainMenuPoker.SetActive(false);
        GlobalGameManager.instance.LoadScene(Scenes.MainMenuTeenPatti);
    }

    public void OnClickPlayPoker()
    {
        GameConstants.poker = true;
        //mainMenuPoker.SetActive(true);
        //mainMenuFlash.SetActive(false);
        GlobalGameManager.instance.LoadScene(Scenes.MainMenu);
    }


    private void LoadVideoAndScreenshot()
    {
        DirectoryInfo dir;
        FileInfo[] info;

        //Create directories to store videos and screenshots
        if (!Directory.Exists(Path.Combine(Application.persistentDataPath, "Videos")))
            Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "Videos"));

        if (!Directory.Exists(Path.Combine(Application.persistentDataPath, "Screenshots")))
            Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "Screenshots"));

        //Remove Currepted Videos
        dir = new DirectoryInfo(Path.Combine(Application.persistentDataPath, "Videos"));
        info = dir.GetFiles("*.mp4");

        for (int j = 0; j < info.Length; j++)
        {
            string[] x = info[j].Name.Split('_');

            //Removing currepted files.
            if (x.Length == 7)
            {
                File.Delete(info[j].FullName);
            }
        }

        //Remove Currepted Screenshots
        dir = new DirectoryInfo(Path.Combine(Application.persistentDataPath, "Screenshots"));
        info = dir.GetFiles("*.png");

        for (int j = 0; j < info.Length; j++)
        {
            string[] x = info[j].Name.Split('_');

            //Removing currepted files.
            if (x.Length == 7)
            {
                File.Delete(info[j].FullName);
            }
        }
    }

    public List<GameObject> AnimatedElements = new List<GameObject>();

    public void PlayMainMenuAnimations()
    {
        if (AnimatedElements.Count > 0)
        {
            foreach(GameObject g in AnimatedElements)
            {
                Animator anim = g.GetComponent<Animator>();
                anim.SetTrigger("op");
            }
        }
    }

    public void OpenShopPage(string shopPanel)
    {
        int layer = (int)GetScreenLayer(MainMenuScreensTeen.MainMenu);
        for (int i = layer + 1; i < screenLayers.Length; i++)
        {
            DestroyScreen((ScreenLayerTeen)i);
        }
        bottomPanel.SetActive(true);
        bottomPanel.GetComponent<PaginationManager>().GoToScreen(0);
        ShopUiManagerTeen.instance.ShowScreen(shopPanel);
    }

    public void SwitchToMainMenu(bool playAnimation = false, int PageNo = -1)
    {
        if(PageNo == -1) { PageNo = 2; }
        bottomPanel.SetActive(true);
        bottomPanel.GetComponent<PaginationManager>().GetScreen(0).SetActive(false);
        bottomPanel.GetComponent<PaginationManager>().GoToScreen(PageNo);

        if (mainMenuActiveScreens.Count > 0)
        {
            int layer = (int)GetScreenLayer(MainMenuScreensTeen.MainMenu);
            for (int i = layer + 1; i < screenLayers.Length; i++)
            {
                DestroyScreen((ScreenLayerTeen)i);
            }
        }

        //if registration screen is acive, destory
        if (null != RegistrationManager.instance)
        {
            Destroy(RegistrationManager.instance.gameObject);
        }
        bottomPanel.GetComponent<PaginationManager>().EnableScreens();
        bottomPanel.SetActive(true);
        bottomPanel.GetComponent<PaginationManager>().GetScreen(0).SetActive(false);
        bottomPanel.GetComponent<PaginationManager>().GoToScreen(PageNo);

        //make sure all screens are initialised.
        //shop
        ShopUiManagerTeen.instance.ShowScreen();
        ////career
        //CareerManager.instance.GetRequestList();
        ////profile
        //FetchUserData();
        //FetchUserLogs();
        ////forum
        //ForumListUIManager.instance.InitialiseForum();
        ////club list
        //ClubListUiManager.instance.FetchList(false);
        ////ProfileScreen
        //ProfileScreenUiManager.instance.InitialiseProfileScreen();

        bottomPanel.transform.GetChild(PageNo).GetComponent<Toggle>().isOn = true;
        if (playAnimation) { PlayMainMenuAnimations(); }
    }

    private void MainMenuController_PageToggleClickEvent(int pageNo)
    {
        switch (pageNo)
        {
            case 0:
                //shop
                ShopUiManagerTeen.instance.ShowScreen();

                if (GameObject.Find("RegistrationManager(Clone)")) { } else
                {
                    FetchUserData();
                    FetchUserLogs();
                    //ProfileScreen
                    ProfileScreenUiManagerTeen.instance.InitialiseProfileScreen();
                }
                break;
            case 1:
                //forum
                ForumListUIManager.instance.InitialiseForum();
                break;
            case 2:
                //club list
                //ClubListUiManagerTeen.instance.FetchList(false);

                FetchUserData();
                FetchUserLogs();
                //ProfileScreen
                //ProfileScreenUiManagerTeen.instance.InitialiseProfileScreen();
                break;
            case 3:
                //career
                CareerManagerTeen.instance.GetRequestList();
                break;
            case 4:
                //profile
                FetchUserData();
                FetchUserLogs();
                //ProfileScreen
                ProfileScreenUiManagerTeen.instance.InitialiseProfileScreen();
                break;
            default:
                break;
        }
    }

    public void OnClickOnButton(string eventName)
    {
        SoundManager.instance.PlaySound(SoundType.Click);


        switch (eventName)
        {

            case "menu":
                {
                    //MenuSelection(4);
                    //ShowScreen(MainMenuScreens.MainMenu);
                }
                break;


            case "profile":
                {
                    //MenuSelection(3);
                    //ShowScreen(MainMenuScreens.Profile);
                }
                break;

            case "shop":
                {
                    //MenuSelection(0);
                    //ShowScreen(MainMenuScreens.Shop);
                }
                break;
            case "Forum":
                {
                    //MenuSelection(1);
                    //ShowScreen(MainMenuScreens.Forum);
                }
                break;
            case "career":
                {
                    //MenuSelection(2);
                    //ShowScreen(MainMenuScreens.Career);
                }
                break;

            default:
#if ERROR_LOG
                Debug.LogError("unhdnled eventName found in MainMenuController = " + eventName);
#endif
                break;
        }

    }


    private void MenuSelection(int index)
    {
        for (int i = 0; i < bottomMenus.Length; i++)
        {
            if (i == index)
            {
                bottomMenus[i].GetComponent<Image>().enabled = true;
                bottomMenus[i].transform.Find("Buttons").gameObject.SetActive(true);
            }
            else
            {
                bottomMenus[i].GetComponent<Image>().enabled = false;
                bottomMenus[i].transform.Find("Buttons").gameObject.SetActive(false);
            }
        }
    }

    public void GetUserDetails(string playerid)
    {
        if (GameObject.Find("RegistrationManager(Clone)")) { return; }
        WebServices.instance.SendRequest(RequestType.GetUserDetails, "{\"userId\":\"" + playerid + "\"}", true, OnServerResponseFound);
    }

    private void FetchUserData()
    {
        //string requestData = "{\"userName\":\"" + PlayerManager.instance.GetPlayerGameData().userName + "\"," +
        //    "\"userPassword\":\"" + PlayerManager.instance.GetPlayerGameData().password + "\"," +
        //      "\"registrationType\":\"" + "Custom" + "\"," +
        //      "\"socialId\":\"" + PlayerManager.instance.GetPlayerGameData().password + "\"}";


        ////ShowScreen(MainMenuScreens.Loading);
        //WebServices.instance.SendRequest(RequestType.Login, requestData, true, OnServerResponseFound);

        GetUserDetails(PlayerManager.instance.GetPlayerGameData().userId);
        DownloadNotificationMessage();
    }
    private void FetchUserLogs()
    {
        string requestData = "{\"userId\":\"" + PlayerManager.instance.GetPlayerGameData().userId + "\"}";
        //{ userId: 2}

        //WebServices.instance.SendRequest(RequestType.userLoginLogs, requestData, true, OnServerResponseFound);
    }
    public void DownloadNotificationMessage()
    {
        string notificationRequestData = "{\"userId\":\"" + PlayerManager.instance.GetPlayerGameData().userId + "\"}";
        WebServices.instance.SendRequest(RequestType.GetNotificationMessage, notificationRequestData, true, OnServerResponseFound);
    }

    public void UpdateReadMessage(string notificationid)
    {
        for (int i = 0; i < notificationDetails.notifications.Count; i++)
        {
            if (notificationDetails.notifications[i].id == notificationid)
            {
                notificationDetails.notifications[i].isRead = true;

                ++notificationDetails.readMessageCount;
                --notificationDetails.unreadMessageCount;

                break;
            }
        }
    }


    private void PraseNotificationMessage(JsonData data)
    {
        notificationDetails.notifications.Clear();

        for (int i = 0; i < data["data"].Count; i++)
        {
            Notification notification = new Notification();
            notification.id = data["data"][i]["firebaseNotificationId"].ToString();

            notification.title = data["data"][i]["title"].ToString();
            notification.desc = data["data"][i]["body"].ToString();
            notification.isRead = data["data"][i]["isRead"].ToString() == "Yes";

            if (notification.isRead)
            {
                ++notificationDetails.readMessageCount;
            }
            else
            {
                ++notificationDetails.unreadMessageCount;
            }

            notificationDetails.notifications.Add(notification);
        }

        if (MenuHandllerTeen.instance != null)
        {
            MenuHandllerTeen.instance.UpdateNotificationData(notificationDetails.unreadMessageCount);
        }

    }

    public NotificationDetails GetNotificationDetails()
    {
        return notificationDetails;
    }

    public void ShowScreen(MainMenuScreensTeen screenName, object[] parameter = null)
    {
        //if(screenName == MainMenuScreensTeen.MainMenu)
        //{
        //    SwitchToMainMenu(true);
        //    return;
        //}

        int layer = (int)GetScreenLayer(screenName);
        for (int i = layer + 1; i < screenLayers.Length; i++)
        {
            DestroyScreen((ScreenLayerTeen)i);
        }

        if (!IsScreenActive(screenName))
        {
            DestroyScreen(GetScreenLayer(screenName));

            MainMenuActiveScreenTeen mainMenuScreen = new MainMenuActiveScreenTeen();
            mainMenuScreen.screenName = screenName;
            mainMenuScreen.screenLayer = GetScreenLayer(screenName);

            GameObject gm = Instantiate(screens[(int)screenName], screenLayers[(int)mainMenuScreen.screenLayer]) as GameObject;
            mainMenuScreen.screenObject = gm;
            mainMenuActiveScreens.Add(mainMenuScreen);
            gm.SetActive(true);
            switch (screenName)
            {
                case MainMenuScreensTeen.ClubDetails:
                    gm.GetComponent<ClubDetailsUIManager>().Initialize((string)parameter[0], (string)parameter[1], (string)parameter[2], (string)parameter[3], (string)parameter[4], (string)parameter[5]);
                    break;


                case MainMenuScreensTeen.GlobalTournament:
                    {
                        if (parameter != null)
                        {
                            gm.GetComponent<GlobalTournamentListUiManager>().ShowScreen((string)parameter[0]);
                        }
                        else
                        {
                            gm.GetComponent<GlobalTournamentListUiManager>().ShowScreen();
                        }
                    }
                    break;


                case MainMenuScreensTeen.Shop:
                    {
                        if (parameter != null)
                        {
                            gm.GetComponent<ShopUiManagerTeen>().ShowScreen((string)parameter[0]);
                        }
                        else
                        {
                            gm.GetComponent<ShopUiManagerTeen>().ShowScreen();
                        }
                    }
                    break;

                default:
                    break;
            }

        }
    }


    public void ShowMessage(string messageToShow, Action callBackMethod = null, string okButtonText = "Ok")
    {
        if (!IsScreenActive(MainMenuScreensTeen.Message))
        {
            MainMenuActiveScreenTeen mainMenuScreen = new MainMenuActiveScreenTeen();
            mainMenuScreen.screenName = MainMenuScreensTeen.Message;
            mainMenuScreen.screenLayer = GetScreenLayer(MainMenuScreensTeen.Message);

            GameObject gm = Instantiate(screens[(int)MainMenuScreensTeen.Message], screenLayers[(int)mainMenuScreen.screenLayer]) as GameObject;
            mainMenuScreen.screenObject = gm;

            mainMenuActiveScreens.Add(mainMenuScreen);

            gm.GetComponent<MessageScript>().ShowSingleButtonPopUp(messageToShow, callBackMethod, okButtonText);
        }
    }


    public void ShowMessage(string messageToShow, Action yesButtonCallBack, Action noButtonCallBack, string yesButtonText = "Yes", string noButtonText = "No")
    {
        if (!IsScreenActive(MainMenuScreensTeen.Message))
        {
            MainMenuActiveScreenTeen mainMenuScreen = new MainMenuActiveScreenTeen();
            mainMenuScreen.screenName = MainMenuScreensTeen.Message;
            mainMenuScreen.screenLayer = GetScreenLayer(MainMenuScreensTeen.Message);

            GameObject gm = Instantiate(screens[(int)MainMenuScreensTeen.Message], screenLayers[(int)mainMenuScreen.screenLayer]) as GameObject;
            mainMenuScreen.screenObject = gm;

            mainMenuActiveScreens.Add(mainMenuScreen);
            gm.GetComponent<MessageScript>().ShowDoubleButtonPopUp(messageToShow, yesButtonCallBack, noButtonCallBack, yesButtonText, noButtonText);
        }
    }



    public void DestroyScreen(MainMenuScreensTeen screenName)
    {
        for (int i = 0; i < mainMenuActiveScreens.Count; i++)
        {
            if (mainMenuActiveScreens[i].screenName == screenName)
            {
                Destroy(mainMenuActiveScreens[i].screenObject);
                mainMenuActiveScreens.RemoveAt(i);
            }
        }
    }

    public void DestroyScreen(ScreenLayerTeen layerName)
    {
        for (int i = 0; i < mainMenuActiveScreens.Count; i++)
        {
            if (mainMenuActiveScreens[i].screenLayer == layerName)
            {
                Destroy(mainMenuActiveScreens[i].screenObject);
                mainMenuActiveScreens.RemoveAt(i);
            }
        }
    }

    private bool IsScreenActive(MainMenuScreensTeen screenName)
    {
        for (int i = 0; i < mainMenuActiveScreens.Count; i++)
        {
            if (mainMenuActiveScreens[i].screenName == screenName)
            {
                return true;
            }
        }

        return false;
    }


    private ScreenLayerTeen GetScreenLayer(MainMenuScreensTeen screenName)
    {
        switch (screenName)
        {
            case MainMenuScreensTeen.MainMenu:
            case MainMenuScreensTeen.Shop:
            case MainMenuScreensTeen.Profile:
            case MainMenuScreensTeen.Forum:

                return ScreenLayerTeen.LAYER1;

            case MainMenuScreensTeen.Message:
            case MainMenuScreensTeen.SelectFrom:
            case MainMenuScreensTeen.FairGaming:
            case MainMenuScreensTeen.Compliance:
            case MainMenuScreensTeen.Contact:
            case MainMenuScreensTeen.FriendList:
            case MainMenuScreensTeen.Language:
            case MainMenuScreensTeen.LinkYourEmail:
            case MainMenuScreensTeen.LinkingSucessfull:
            case MainMenuScreensTeen.UnlinkYourEmail:
            case MainMenuScreensTeen.ChangePassword:
            case MainMenuScreensTeen.RedeemCode:
            case MainMenuScreensTeen.InGameShop:
            case MainMenuScreensTeen.Missions:
            case MainMenuScreensTeen.ConsecutiveLoginReward:
            case MainMenuScreensTeen.Congratulation:
            case MainMenuScreensTeen.BackPack:
            case MainMenuScreensTeen.CareerMenuScreen:
            case MainMenuScreensTeen.CareerDataScreen:
            case MainMenuScreensTeen.CareerDefinationScreen:

                return ScreenLayerTeen.LAYER3;

            /*case MainMenuScreens.Loading:*/
            case MainMenuScreensTeen.ChangeFrame:
            case MainMenuScreensTeen.SelectRegion:
            case MainMenuScreensTeen.ChangeProfileIcon:

                return ScreenLayerTeen.LAYER4;

            case MainMenuScreensTeen.Loading:
                return ScreenLayerTeen.LAYER5;

            default:
                return ScreenLayerTeen.LAYER2;
        }
    }


    public void OnServerResponseFound(RequestType requestType, string serverResponse, bool isShowErrorMessage, string errorMessage)
    {
        if (errorMessage.Length > 0)
        {
            if (isShowErrorMessage)
            {
                if (requestType == RequestType.Login)
                {

                }
                else
                {
                    ShowMessage(errorMessage);
                }
            }

            return;
        }

        if (requestType == RequestType.GetUserDetails)
        {
            JsonData data = JsonMapper.ToObject(serverResponse);

            if (data["success"].ToString() == "1")
            {
                PlayerGameDetails playerData = Utility.ParseUserDetails(data);
                playerData.password = PlayerManager.instance.GetPlayerGameData().password;
                playerData.userName = PlayerManager.instance.GetPlayerGameData().userName;
                PlayerManager.instance.SetPlayerGameData(playerData);
            }
            else
            {
                Debug.LogError(data["message"].ToString());
            }
        }

        if (requestType == RequestType.Login)
        {
            JsonData data = JsonMapper.ToObject(serverResponse);

            if (data["success"].ToString() == "1")
            {
                PlayerGameDetails playerData = Utility.ParsePlayerGameData(data);
                playerData.password = PlayerManager.instance.GetPlayerGameData().password;
                playerData.userName = PlayerManager.instance.GetPlayerGameData().userName;
                PlayerManager.instance.SetPlayerGameData(playerData);
                //FetchUserData();
                //SwitchToMainMenu();
            }
            else
            {   
                ShowMessage(data["message"].ToString());

                ShowScreen(MainMenuScreensTeen.Registration);
            }
        }
        else if (requestType == RequestType.GetNotificationMessage)
        {
            JsonData data = JsonMapper.ToObject(serverResponse);

            if (data["success"].ToString() == "1")
            {
                PraseNotificationMessage(data);
            }
            else
            {
                //Debug.LogError(data["message"].ToString());
                //ShowMessage(data["message"].ToString());
            }
        }
        else if (requestType == RequestType.userLoginLogs)
        {
            Debug.Log("Response => UserLoginLogs" + serverResponse);
            JsonData data = JsonMapper.ToObject(serverResponse);

            if (data["success"].ToString() == "1")
            {
                //PraseNotificationMessage(data);
            }
            else
            {
                ShowMessage(data["message"].ToString());
            }
        }
        DestroyScreen(MainMenuScreensTeen.Loading);
    }
}


public class MainMenuActiveScreenTeen
{
    public GameObject screenObject;
    public MainMenuScreensTeen screenName;
    public ScreenLayerTeen screenLayer;
}

public enum MainMenuScreensTeen
{
    Registration,
    MainMenu,
    ClubDetails,
    Message,
    Loading,
    ClubList,
    Profile,
    Lobby,
    GlobalTournament,
    Shop,
    VIP_Privilege,
    Notification,
    Missions,
    Forum,
    ProfileModification,
    ChangeFrame,
    SelectRegion,
    SelectFrom,
    ChangeProfileIcon,
    ProfileSetting,
    AboutUs,
    FairGaming,
    Compliance,
    Contact,
    Language,
    TopPlayer,
    FriendList,
    LinkYourEmail,
    LinkingSucessfull,
    UnlinkYourEmail,
    ChangePassword,
    RedeemCode,
    InGameShop,
    ConsecutiveLoginReward,
    Congratulation,
    BackPack,
    Career,
    CareerMenuScreen,
    CareerDataScreen,
    CareerDefinationScreen,
    HandScreen,
    //LobbyTeenPatti,
    //MainMenuTeenPatti,
}


public enum ScreenLayerTeen
{
    LAYER1,
    LAYER2,
    LAYER3,
    LAYER4,
    LAYER5
}
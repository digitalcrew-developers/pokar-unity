﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using TMPro;
using UnityEngine.Networking;
using System.Runtime.InteropServices;

public class TournamentLobbyUiManager : MonoBehaviour
{
    public static TournamentLobbyUiManager instance;
    [SerializeField]
    private LayoutManager layoutManager;

    [SerializeField]
    private GameObject roomPrefab;

    [SerializeField]
    private Transform container;

    [SerializeField]
    private Button[] gameModeButtons;

    [SerializeField]
    private List<List<RoomData>> allRoomData = new List<List<RoomData>>();

    public Text coinsText;
    public Button missionBtn;
    public Button topPlayerBtn;
    public Button shopBtn;
    public Button BagPackBtn;

    private void OnEnable()
    {
        //Deactivate Bottom Panel
        //if (MainMenuController.instance.bottomPanel.activeSelf /*&& GameConstants.poker*/)
            //MainMenuController.instance.bottomPanel.SetActive(false);
        //else if(MainMenuController.instance.bottomPanel.activeSelf && !GameConstants.poker)
        //    MainMenuController.instance.bottomPanelTeen.SetActive(false);

        //missionBtn.onClick.AddListener(() => ShowMissonScreen());
        //topPlayerBtn.onClick.AddListener(() => ShowTopPlayerScreen());
        //shopBtn.onClick.AddListener(() => ShowShopScreen());
        //BagPackBtn.onClick.AddListener(() => ShowBackPackScreen());
        //ChangeTextColor(0);
    }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        for (int i = 0; i < 3; i++)
        {
            List<RoomData> dummyList = new List<RoomData>();
            allRoomData.Add(dummyList);
        }

        for (int i = 0; i < container.childCount; i++)
        {
            //Destroy(container.GetChild(i).gameObject);
        }

        coinsText.text = Utility.GetTrimmedAmount(""+PlayerManager.instance.GetPlayerGameData().coins);

        //TournamentInGameUiManager.instance.ShowScreen(TournamentInGameScreens.Loading);
        //WebServices.instance.SendRequest(RequestType.GetLobbyRooms, "{}", true, OnServerResponseFound);
    }

    public void ShowMissonScreen()
    {
        Debug.Log("^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^");
        MainMenuController.instance.ShowScreen(MainMenuScreens.Missions);
    }
    void ShowTopPlayerScreen()
    {
        MainMenuController.instance.ShowScreen(MainMenuScreens.TopPlayer);
    }
    void ShowBackPackScreen()
    {
        MainMenuController.instance.ShowScreen(MainMenuScreens.BackPack);
    }
    void ShowShopScreen()
    {
        MainMenuController.instance.ShowScreen(MainMenuScreens.Shop);
    }
    public void OnClickOnButton(string eventName)
    {
        SoundManager.instance.PlaySound(SoundType.Click);

        switch (eventName)
        {
            case "back":
                {
                    //MainMenuController.instance.SwitchToMainMenu(true);
                    TournamentInGameUiManager.instance.DestroyScreen(TournamentInGameScreens.Loading);
                    GlobalGameManager.instance.LoadScene(Scenes.MainMenu);
                }
                break;

            case "coinsShop":
                {
                    MainMenuController.instance.ShowScreen(MainMenuScreens.InGameShop);
                }
                break;


            case "nlh":
                {
                    ShowScreen(GameMode.NLH);
                    ChangeTextColor(0);
                    
                }
                break;

            case "plo":
                {
                    ShowScreen(GameMode.PLO);
                    ChangeTextColor(1);
                }
                break;

            case "ofc":
                {
                    ShowScreen(GameMode.OFC);
                    ChangeTextColor(2);
                }
                break;
            case "FriendList":
                {
                    MainMenuController.instance.ShowScreen(MainMenuScreens.FriendList);
                }
                break;

            default:
#if ERROR_LOG
			Debug.LogError("unhdnled eventName found in TournamentLobbyUiManager = " + eventName);
#endif
            break;
        }
    }

    void ChangeTextColor (int val){
        for (int i = 0; i < gameModeButtons.Length; i++)
        {
            if (i == val)
            {
                //gameModeButtons[i].transform.GetChild(0).GetComponent<Text>().color = new Color32(0, 0, 35, 255);
            }
            else {
                //gameModeButtons[i].transform.GetChild(0).GetComponent<Text>().color = new Color32(200, 200, 200, 255);
            }
        }
    }
    
    private void ShowScreen(GameMode gameMode)
    {

        for (int i = 0; i < container.childCount; i++)
        {
            Destroy(container.GetChild(i).gameObject);
        }

        for (int i = 0; i < gameModeButtons.Length; i++)
        {
            gameModeButtons[i].interactable = true;
        }

        gameModeButtons[(int)gameMode].interactable = false;

        int index = (int)gameMode;

        for (int i = 0; i < allRoomData[index].Count; i++)
        {
            RoomData data = allRoomData[index][i];

            GameObject gm = Instantiate(roomPrefab, container) as GameObject;

            loadRoomImage(data.roomIconUrl, gm);
            LoadRoomBG(data.roomBG, gm);

            gm.transform.Find("Name").GetComponent<Text>().text = data.title;
            gm.transform.Find("Blinds").GetComponent<Text>().text = "" + Utility.GetTrimmedAmount("" + data.smallBlind) + "/" + Utility.GetTrimmedAmount("" + data.bigBlind);
            gm.transform.Find("BuyIn").transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "" + Utility.GetTrimmedAmount("" + data.minBuyIn);

            gm.transform.Find("LivePlayer").GetComponent<Text>().text = data.totalActivePlayers.ToString();

            gm.GetComponent<Button>().onClick.AddListener(() => OnClickOnPlayButton(data,index));
            gm.GetComponent<Button>().onClick.AddListener(() => gm.GetComponent<LobbyRoomManager>().CallInsufficientCoin(data));
        }

        layoutManager.UpdateLayout();

    }

    private void LoadRoomBG(string url, GameObject obj)
    {
        StartCoroutine(loadRoomBGSpriteFromUrl(url, obj));
    }
    IEnumerator loadRoomBGSpriteFromUrl(string URL, GameObject obj)
    {
        UnityWebRequest unityWebRequest = UnityWebRequestTexture.GetTexture(URL);
        yield return unityWebRequest.SendWebRequest();

        if (unityWebRequest.isNetworkError || unityWebRequest.isHttpError)
        {
            Debug.LogError("Download failed");
        }
        else
        {
            //  image.sprite = null;
            var Text = DownloadHandlerTexture.GetContent(unityWebRequest);
            Sprite sprite = Sprite.Create(Text, new Rect(0, 0, Text.width, Text.height), Vector2.zero);

            obj.transform.Find("BG").GetComponent<Image>().sprite = sprite;            
        }
    }

    public void loadRoomImage(string url, GameObject obj)
    {
        //   Debug.Log("Success data send");
        StartCoroutine(loadRoomSpriteImageFromUrl(url, obj));
    }
    
    IEnumerator loadRoomSpriteImageFromUrl(string URL, GameObject obj)
    {
        UnityWebRequest unityWebRequest = UnityWebRequestTexture.GetTexture(URL);
        yield return unityWebRequest.SendWebRequest();

        if (unityWebRequest.isNetworkError || unityWebRequest.isHttpError)
        {
            //Debug.LogError("Download failed");
        }
        else
        {
            //  image.sprite = null;
            var Text = DownloadHandlerTexture.GetContent(unityWebRequest);
            Sprite sprite = Sprite.Create(Text, new Rect(0, 0, Text.width, Text.height), Vector2.zero);

            obj.transform.Find("PhotoBg").GetComponent<Image>().sprite = sprite;
            obj.transform.Find("PhotoBg").GetChild(0).GetComponent<Image>().sprite = sprite;
        }
    }


    private void OnClickOnPlayButton(RoomData data, int gameMode = -1)
    {
        SoundManager.instance.PlaySound(SoundType.Click);

        if (PlayerManager.instance.GetPlayerGameData().coins < data.minBuyIn)
        {
           
            //MainMenuController.instance.ShowMessage("You dont have sufficient coins to play, please purchase coins to continue",()=>{
            //    MainMenuController.instance.ShowScreen(MainMenuScreens.Shop);
            //},()=> {
            //},"Shop","Cancel");

            return;
           // InsufficientCoinLobbyRoomManager.in
        }



        data.isLobbyRoom = true;

        GlobalGameManager.instance.SetRoomData(data);
        GameConstants.TURN_TIME = data.callTimer;

        //Debug.Log("GameConstants.TURN_TIME " + GameConstants.TURN_TIME);
        //Debug.Log("data.callTimer " + data.callTimer);
        if (data.gameMode != GameMode.OFC)
        {
            if (GameConstants.poker)
            {
                GlobalGameManager.instance.LoadScene(Scenes.InGame);
            }
            else
            {
                GlobalGameManager.instance.LoadScene(Scenes.InGameTeenPatti);
            }
            //GlobalGameManager.instance.LoadScene(Scenes.InGame);
        }
        else
        {
            MainMenuController.instance.ShowMessage("Coming soon");
        }

        //GlobalGameManager.instance.StoreLastLobbyData(allRoomData, gameMode);
    }



    public void ReadRoomData(string serverResponse)
    {
        JsonData data = JsonMapper.ToObject(serverResponse);

        Debug.Log(data[0]["data"][0]["name"].ToString());
        int totalData = data[0]["data"].Count;

        for (int i = 0; i < container.childCount; i++)
        {
            Destroy(container.GetChild(i).gameObject);
        }

        Debug.Log(totalData);
        for (int i = 0; i < totalData; i++)
        {
            Debug.Log(data[0]["data"][i]["name"].ToString());
            GameObject gm = Instantiate(roomPrefab, container) as GameObject;

            //loadRoomImage(data.roomIconUrl, gm);
            //LoadRoomBG(data.roomBG, gm);

            gm.transform.GetChild(1).GetComponent<Text>().text = data[0]["data"][i]["name"].ToString();
            gm.transform.GetChild(5).GetComponent<Text>().text = data[0]["data"][i]["min_player"] + "/" + data[0]["data"][i]["max_player"];

            //gm.GetComponent<Button>().onClick.AddListener(() => OnClickOnPlayButton(data, index));
            //gm.GetComponent<Button>().onClick.AddListener(() => gm.GetComponent<LobbyRoomManager>().CallInsufficientCoin(data));
        }

        //layoutManager.UpdateLayout();
        TournamentInGameUiManager.instance.DestroyScreen(TournamentInGameScreens.Loading);
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



        if (requestType == RequestType.GetLobbyRooms)
        {
            Debug.Log("Response => GetLobbyRooms: " + serverResponse);
            JsonData data = JsonMapper.ToObject(serverResponse);

            if (data["status"].Equals(true))
            {
                //ReadRoomData(data);
            }
            else
            {
                MainMenuController.instance.ShowMessage("Unable to get room data");
            }

        }
        else
        {
#if ERROR_LOG
			Debug.LogError("Unhadnled response found in TournamentLobbyUiManager = " + requestType);
#endif
        }

    }
}
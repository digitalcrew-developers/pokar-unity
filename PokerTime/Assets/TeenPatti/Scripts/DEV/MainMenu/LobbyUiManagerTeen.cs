using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using TMPro;
using UnityEngine.Networking;
using System.Runtime.InteropServices;

public class LobbyUiManagerTeen : MonoBehaviour
{
    public static LobbyUiManagerTeen instance;
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
        //if (MainMenuController.instance.bottomPanel.activeSelf && GameConstants.poker)
        //    MainMenuController.instance.bottomPanel.SetActive(false);
        //else if(MainMenuController.instance.bottomPanel.activeSelf && !GameConstants.poker)
        //    MainMenuController.instance.bottomPanelTeen.SetActive(false);

        if (MainMenuControllerTeen.instance.bottomPanel.activeSelf)
            MainMenuControllerTeen.instance.bottomPanel.SetActive(false);

        missionBtn.onClick.AddListener(() => ShowMissonScreen());
        topPlayerBtn.onClick.AddListener(() => ShowTopPlayerScreen());
        shopBtn.onClick.AddListener(() => ShowShopScreen());
        BagPackBtn.onClick.AddListener(() => ShowBackPackScreen());
        //ChangeTextColor(0);
        OnClickOnButton("classic");
        gameModeButtons[0].interactable = false;
    }

    private void Awake()
    {
        instance = this;
        //ChangeTextColor(0);
    }

    private void Start()
    {
        //OnClickOnButton("classic");

        for (int i = 0; i < 3; i++)
        {
            List<RoomData> dummyList = new List<RoomData>();
            allRoomData.Add(dummyList);
        }

        for (int i = 0; i < container.childCount; i++)
        {
            Destroy(container.GetChild(i).gameObject);
        }

        coinsText.text = Utility.GetTrimmedAmount(""+PlayerManager.instance.GetPlayerGameData().coins);

        MainMenuControllerTeen.instance.ShowScreen(MainMenuScreensTeen.Loading);
        //GenerateRequest("http://3.17.201.78:6000/tp_getRooms");
        //string req = "{\"Content-Type\":\"" + "application/Json" + "\"}";
        WebServices.instance.SendRequest(RequestType.LobbyRoomsTeenPatti, "{}", true, OnServerResponseFound);
    }

    

    public void GenerateRequest(string URL)
    {
        StartCoroutine(ProcessRequest(URL));
    }

    private IEnumerator ProcessRequest(string uri)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(uri))
        {
            yield return request.SendWebRequest();

            if (request.isNetworkError)
            {
                Debug.Log(request.error);
            }
            else
            {
                MainMenuControllerTeen.instance.DestroyScreen(MainMenuScreensTeen.Loading);
               
                 Debug.LogError("Response => GetLobbyRooms: " + request.downloadHandler.text);
                JsonData data = JsonMapper.ToObject(request.downloadHandler.text);

                if (data["status"].Equals(true))
                {
                    ReadRoomData(data);
                }
                else
                {
                    MainMenuControllerTeen.instance.ShowMessage("Unable to get room data");
                }
            }
        }
    }


public void ShowMissonScreen()
    {
        Debug.Log("^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^");
        MainMenuControllerTeen.instance.ShowScreen(MainMenuScreensTeen.Missions);
    }
    void ShowTopPlayerScreen()
    {
        MainMenuControllerTeen.instance.ShowScreen(MainMenuScreensTeen.TopPlayer);
    }
    void ShowBackPackScreen()
    {
        MainMenuControllerTeen.instance.ShowScreen(MainMenuScreensTeen.BackPack);
    }
    void ShowShopScreen()
    {
        MainMenuControllerTeen.instance.ShowScreen(MainMenuScreensTeen.Shop);
    }
    public void OnClickOnButton(string eventName)
    {
        SoundManager.instance.PlaySound(SoundType.Click);

        switch (eventName)
        {
            case "back":
                {
                    MainMenuControllerTeen.instance.SwitchToMainMenu(true);
                }
                break;

            case "coinsShop":
                {
                    MainMenuControllerTeen.instance.ShowScreen(MainMenuScreensTeen.InGameShop);
                }
                break;


            case "classic":
                {
                    //ShowScreen(GameMode.NLH);
                    ChangeTextColor(0);
                }
                break;

            case "muflis":
                {
                    //ShowScreen(GameMode.PLO);
                    ChangeTextColor(1);
                }
                break;

            case "joker":
                {
                    //ShowScreen(GameMode.OFC);
                    ChangeTextColor(2);
                }
                break;

            case "999":
                {
                    //ShowScreen(GameMode.NLH);
                    ChangeTextColor(3);
                }
                break;

            case "FriendList":
                {
                    MainMenuControllerTeen.instance.ShowScreen(MainMenuScreensTeen.FriendList);
                }
                break;

            default:
#if ERROR_LOG
			Debug.LogError("unhdnled eventName found in LobbyUiManager = " + eventName);
#endif
            break;
        }
    }

    void ChangeTextColor (int val)
    {
        if (!gameModeButtons[0].interactable)
            gameModeButtons[0].interactable = true;
        for (int i = 0; i < gameModeButtons.Length; i++)
        {
            if (i == val)
            {
                //gameModeButtons[i].interactable = true;
                //gameModeButtons[i].transform.GetChild(0).GetComponent<Text>().color = new Color32(0, 0, 35, 255);
            }
            else 
            {
                //gameModeButtons[i].interactable = false;
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

        //for (int i = 0; i < gameModeButtons.Length; i++)
        //{
        //    gameModeButtons[i].interactable = true;
        //}

        //gameModeButtons[(int)gameMode].interactable = false;

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
            gm.GetComponent<Button>().onClick.AddListener(() => gm.GetComponent<LobbyRoomManagerTeen>().CallInsufficientCoin(data));
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

        GlobalGameManager.instance.StoreLastLobbyData(allRoomData, gameMode);
    }



    private void ReadRoomData(JsonData data)
    {
        for (int i = 0; i < data["data"].Count; i++)
        {
           /* if (data["data"][i]["roomTitle"].ToString() == "Club_Room")
            {
                // TODO remove this later after adding club room
                continue;
            }*/

            RoomData roomData = new RoomData();

            roomData.roomId = data["data"][i]["roomId"].ToString();
            roomData.title = data["data"][i]["roomTitle"].ToString();
            roomData.players = int.Parse(data["data"][i]["players"].ToString());
            roomData.callTimer = int.Parse(data["data"][i]["callTimmer"].ToString());

           // roomData.commision = float.Parse(data["data"][i]["commission"].ToString());
            roomData.smallBlind = float.Parse(data["data"][i]["smallBlind"].ToString());
            roomData.bigBlind = float.Parse(data["data"][i]["bigBlind"].ToString());
            roomData.minBuyIn = float.Parse(data["data"][i]["minBet"].ToString());
            roomData.maxBuyIn = float.Parse(data["data"][i]["maxBet"].ToString());

            //DEV_CODE
            //roomData.roomBG = data["data"][i]["backgroundImg"].ToString();
            //roomData.roomIconUrl = data["data"][i]["iconBaseUrl"].ToString();
            roomData.totalActivePlayers = int.Parse(data["data"][i]["totalActivePlayer"].ToString());            

            switch (data["data"][i]["gameType"].ToString())
            {
                case "PLO":
                roomData.gameMode = GameMode.PLO;
                break;

                case "OFC":
                roomData.gameMode = GameMode.OFC;
                break;

                default:
                roomData.gameMode = GameMode.NLH;
                break;
            }

            allRoomData[(int)roomData.gameMode].Add(roomData);
        }


        //ShowScreen(GameModeTeen.CLASSIC);
        ShowScreen(GameMode.NLH);
    }
    
    public void OnServerResponseFound(RequestType requestType, string serverResponse, bool isShowErrorMessage, string errorMessage)
    {
        MainMenuControllerTeen.instance.DestroyScreen(MainMenuScreensTeen.Loading);
        Debug.LogError("Response Get Room :" + requestType);
        Debug.LogError("Response => GetLobbyRooms: " + serverResponse);
        if (errorMessage.Length > 0)
        {
            if (isShowErrorMessage)
            {
                MainMenuControllerTeen.instance.ShowMessage(errorMessage);
            }

            return;
        }



        if (requestType == RequestType.LobbyRoomsTeenPatti)
        {
            Debug.LogError("Response => GetLobbyRooms: " + serverResponse);
            JsonData data = JsonMapper.ToObject(serverResponse);

            if (data["status"].Equals(true))
            {
                ReadRoomData(data);
            }
            else
            {
                MainMenuControllerTeen.instance.ShowMessage("Unable to get room data");
            }

        }
        else
        {
#if ERROR_LOG
			Debug.LogError("Unhadnled response found in LobbyUiManager = "+requestType);
#endif
        }

    }
}
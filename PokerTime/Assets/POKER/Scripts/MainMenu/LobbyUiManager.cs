using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using TMPro;

public class LobbyUiManager: MonoBehaviour
{

    [SerializeField]
    private LayoutManager layoutManager;

    [SerializeField]
    private GameObject roomPrefab;

    [SerializeField]
    private Transform container;

    [SerializeField]
    private Button[] gameModeButtons;

    [SerializeField]
    private Text coinsText;

    private List<List<RoomData>> allRoomData = new List<List<RoomData>>();

    public Button missionBtn;
    public Button topPlayerBtn;
    public Button shopBtn;


    private void Start()
    {
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

        MainMenuController.instance.ShowScreen(MainMenuScreens.Loading);
        WebServices.instance.SendRequest(RequestType.GetLobbyRooms, "{}", true, OnServerResponseFound);
    }

    private void OnEnable()
    {
        missionBtn.onClick.AddListener(() => ShowMissonScreen());
        topPlayerBtn.onClick.AddListener(() => ShowTopPlayerScreen());
        shopBtn.onClick.AddListener(() => ShowShopScreen());
        ChangeTextColor(0);
    }
    void ShowMissonScreen()
    {
        Debug.Log("^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^");
        MainMenuController.instance.ShowScreen(MainMenuScreens.Missions);
    }
    void ShowTopPlayerScreen()
    {
        MainMenuController.instance.ShowScreen(MainMenuScreens.TopPlayer);
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
                    MainMenuController.instance.ShowScreen(MainMenuScreens.MainMenu);
                }
                break;

            case "coinsShop":
                {
                    MainMenuController.instance.ShowScreen(MainMenuScreens.Shop);
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
			Debug.LogError("unhdnled eventName found in LobbyUiManager = " + eventName);
#endif
            break;
        }
    }

    void ChangeTextColor (int val){
        for (int i = 0; i < gameModeButtons.Length; i++)
        {
            if (i == val)
            {
                gameModeButtons[i].transform.GetChild(0).GetComponent<Text>().color = new Color32(0, 0, 35, 255);
            }
            else {
                gameModeButtons[i].transform.GetChild(0).GetComponent<Text>().color = new Color32(200, 200, 200, 255);
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
            gm.transform.Find("Name").GetComponent<Text>().text = data.title;
            gm.transform.Find("Blinds").GetComponent<Text>().text = "" + Utility.GetTrimmedAmount("" + data.smallBlind) + "/" + Utility.GetTrimmedAmount("" + data.bigBlind);
            gm.transform.Find("BuyIn").transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "" + Utility.GetTrimmedAmount("" + data.minBuyIn);

            gm.GetComponent<Button>().onClick.AddListener(() => OnClickOnPlayButton(data));
        }

        layoutManager.UpdateLayout();

    }


    private void OnClickOnPlayButton(RoomData data)
    {
        SoundManager.instance.PlaySound(SoundType.Click);

        if (PlayerManager.instance.GetPlayerGameData().coins < data.minBuyIn)
        {
            MainMenuController.instance.ShowMessage("You dont have sufficient coins to play, please purchase coins to continue",()=>{
                MainMenuController.instance.ShowScreen(MainMenuScreens.Shop);
            },()=> {
            },"Shop","Cancel");

            return;
        }



        data.isLobbyRoom = true;

        GlobalGameManager.instance.SetRoomData(data);
        GameConstants.TURN_TIME = data.callTimer;

        if (data.gameMode != GameMode.OFC)
        {
            GlobalGameManager.instance.LoadScene(Scenes.InGame);
        }
        else
        {
            MainMenuController.instance.ShowMessage("Coming soon");
        }
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

            roomData.commision = float.Parse(data["data"][i]["commission"].ToString());
            roomData.smallBlind = float.Parse(data["data"][i]["smallBlind"].ToString());
            roomData.bigBlind = float.Parse(data["data"][i]["bigBlind"].ToString());
            roomData.minBuyIn = float.Parse(data["data"][i]["minBet"].ToString());
            roomData.maxBuyIn = float.Parse(data["data"][i]["maxBet"].ToString());


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


        ShowScreen(GameMode.NLH);
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
            JsonData data = JsonMapper.ToObject(serverResponse);

            if (data["status"].Equals(true))
            {
                ReadRoomData(data);
            }
            else
            {
                MainMenuController.instance.ShowMessage("Unable to get room data");
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





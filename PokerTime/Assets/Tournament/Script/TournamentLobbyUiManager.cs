using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using TMPro;
using UnityEngine.Networking;
using System.Runtime.InteropServices;
using System;

public class TournamentLobbyUiManager : MonoBehaviour
{
    public static TournamentLobbyUiManager instance;
    [SerializeField]
    private LayoutManager layoutManager;

    public Text popUpText;

    [SerializeField]
    private GameObject roomPrefab;

    [SerializeField]
    private ScrollRect scrollRect;

    [SerializeField]
    //private List<RectTransform> containers = new List<RectTransform>();
    private Transform container;
    //private Transform container_Recommended;
    //private Transform container_SpinUp;
    //private Transform container_MMT;

    [SerializeField]
    private Button[] tournamentTypeButtons;

    [SerializeField]
    private List<List<TournamentRoomData>> allRoomData = new List<List<TournamentRoomData>>();

    [Space(9)]
    public Sprite regSprite;
    public Sprite lateRegSprite, observeSprite;

    public Text coinsText;
    public Button missionBtn;
    public Button topPlayerBtn;
    public Button shopBtn;
    public Button BagPackBtn;

    public GameObject tournamentDetailsPanel;

    public Sprite timerOnSprite, timerOffSprite;

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

        ResetContainers();
        ResetRoomData();
    }

    private void Start()
    {
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


            //case "nlh":
            //    {
            //        ShowScreen(GameMode.NLH);
            //        ChangeTextColor(0);
                    
            //    }
            //    break;

            //case "plo":
            //    {
            //        ShowScreen(GameMode.PLO);
            //        ChangeTextColor(1);
            //    }
            //    break;

            //case "ofc":
            //    {
            //        ShowScreen(GameMode.OFC);
            //        ChangeTextColor(2);
            //    }
            //    break;

            case "FriendList":
                {
                    MainMenuController.instance.ShowScreen(MainMenuScreens.FriendList);
                }
                break;

            case "Recommended":
            {
                ShowScreen(TournamentType.Recommended);
                //ChangeTextColor((int)TournamentType.Recommended);
            }
            break;

            case "SpinUp":
            {
                ShowScreen(TournamentType.SpinUp);
                //ChangeTextColor((int)TournamentType.SpinUp);
            }
            break;

            case "MTT":
            {
                ShowScreen(TournamentType.MTT);
                //ChangeTextColor((int)TournamentType.MTT);
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
        for (int i = 0; i < tournamentTypeButtons.Length; i++)
        {
            if (i == val)
            {
                //tournamentTypeButtons[i].transform.GetChild(0).GetComponent<Text>().color = new Color32(0, 0, 35, 255);
            }
            else 
            {
                //tournamentTypeButtons[i].transform.GetChild(0).GetComponent<Text>().color = new Color32(255, 255, 255, 255);
            }
        }
    }

    private void ShowScreen(/*GameMode gameMode*/TournamentType tournamentType)
    {
        ResetContainers();

        for (int i = 0; i < tournamentTypeButtons.Length; i++)
        {
            tournamentTypeButtons[i].interactable = true;
            tournamentTypeButtons[i].transform.GetComponent<Image>().enabled = false;
        }

        tournamentTypeButtons[(int)tournamentType].interactable = false;
        tournamentTypeButtons[(int)tournamentType].transform.GetComponent<Image>().enabled = true;

        int index = (int)tournamentType;

        for (int i = 0; i < allRoomData[index].Count; i++)
        {
            TournamentRoomData data = allRoomData[index][i];

            GameObject gm = Instantiate(roomPrefab, container) as GameObject;

            gm.transform.GetChild(1).GetComponent<Text>().text = data.name;

            gm.transform.GetChild(3).GetComponent<Text>().text = data.entryChipAmt.ToString();

            gm.transform.GetChild(5).GetComponent<Text>().text = data.registeredUsers + "/" + data.maxPlayer;

            int tournyId = int.Parse(data.id);
            //print("Tournay ID:  " + tournyId.ToString() + " ------ User Status: " + data.isRegistered);

            /*if (data.isRegistered)
            {
                //print("User Registered..");
                gm.transform.GetChild(11).gameObject.SetActive(false);
                gm.transform.GetChild(12).gameObject.SetActive(true);
            }
            else
            {
                //print("User not Registered...");
                gm.transform.GetChild(11).gameObject.SetActive(true);
                gm.transform.GetChild(12).gameObject.SetActive(false);
            }*/

            DateTime gameStartDate = Convert.ToDateTime(data.gameStart).ToLocalTime();
            gm.transform.GetChild(9).GetComponent<Text>().text = gameStartDate.ToString(@"MM'/'dd"); //previously: (@"MM'/'yy");
            gm.transform.GetChild(10).GetChild(0).GetComponent<Text>().text = gameStartDate.ToString(@"HH':'mm");


            //START: for timer, timer starts before 1 hour
            GameObject dateTextParent = gm.transform.GetChild(9).gameObject;
            GameObject timerTextParent = gm.transform.GetChild(10).gameObject;
            GameObject registerBtn = gm.transform.GetChild(11).gameObject;
            GameObject joinBtn = gm.transform.GetChild(12).gameObject;
            GameObject lateRegisterBtn = gm.transform.GetChild(13).gameObject;
            GameObject observeBtn = gm.transform.GetChild(14).gameObject;
            GameObject enrolledBtn = gm.transform.GetChild(15).gameObject;

            DateTime today = DateTime.Now;

            TimeSpan differenceGameStart = today.Subtract(gameStartDate);
            int totalSecondsGameStart = (int)differenceGameStart.TotalSeconds;

            DateTime lateRegStart = gameStartDate;
            DateTime lateRegEnd = lateRegStart.AddMinutes(30); //30min, late registration starts after gameStart time & its end upto this min
            TimeSpan differenceLateReg = lateRegStart.Subtract(lateRegEnd);
            int totalSecondsLateReg = (int)differenceLateReg.TotalSeconds;

            TimeSpan diffLateRegWithNow = today.Subtract(lateRegEnd);
            int totSecondLateRegWithNow = (int)diffLateRegWithNow.TotalSeconds;
            //Debug.Log("totalSeconds: " + totalSecondsGameStart + " totalSecondsLateReg: " + totalSecondsLateReg + " totSecondLateRegWithNow: " + totSecondLateRegWithNow);

            /*
            button cases:
            timer1 running && time before gameStart: register ? "Enrolled" : "Register" 
            timer1 complete immediately
                timer2 (late reg. timer) starts
                    register ? "Join" : "Late Reg."
                timer2 end
                    register ? "Join": "Observe"
            time after gameStart
                if late reg. time remaining
                    timer2 (late reg. timer) starts
                        register ? "Join" : "Late Reg."
                else
                    register ? "Join": "Observe"
            */

            if (totalSecondsGameStart < 0)
            {
                if (data.isRegistered)
                {
                    // enrolled
                    enrolledBtn.SetActive(true);
                    joinBtn.SetActive(false);
                    observeBtn.SetActive(false);
                    registerBtn.SetActive(false);
                    lateRegisterBtn.SetActive(false);
                }
                else
                {
                    // register
                    enrolledBtn.SetActive(false);
                    joinBtn.SetActive(false);
                    observeBtn.SetActive(false);
                    registerBtn.SetActive(true);
                    lateRegisterBtn.SetActive(false);
                }
            }
            else
            {
                if (totSecondLateRegWithNow < 0)
                {
                    if (data.isRegistered)
                    {
                        // join
                        enrolledBtn.SetActive(false);
                        joinBtn.SetActive(true);
                        observeBtn.SetActive(false);
                        registerBtn.SetActive(false);
                        lateRegisterBtn.SetActive(false);
                    }
                    else
                    {
                        // late reg.
                        enrolledBtn.SetActive(false);
                        joinBtn.SetActive(false);
                        observeBtn.SetActive(false);
                        registerBtn.SetActive(false);
                        lateRegisterBtn.SetActive(true);
                    }
                }
                else
                {
                    if (data.isRegistered)
                    {
                        // join
                        enrolledBtn.SetActive(false);
                        joinBtn.SetActive(true);
                        observeBtn.SetActive(false);
                        registerBtn.SetActive(false);
                        lateRegisterBtn.SetActive(false);
                    }
                    else
                    {
                        // observe
                        enrolledBtn.SetActive(false);
                        joinBtn.SetActive(false);
                        observeBtn.SetActive(true);
                        registerBtn.SetActive(false);
                        lateRegisterBtn.SetActive(false);
                    }
                }
            }


            if ((totalSecondsGameStart < 0) && (differenceGameStart.Days == 0) && (differenceGameStart.Hours == 0))
            {
                dateTextParent.GetComponent<Text>().text = "Starts in";

                StartCoroutine(TournamentStartTimer(gameStartDate, totalSecondsGameStart, timerTextParent, (myReturnValue) => {
                    if (myReturnValue)
                    {
                        if (data.isRegistered)
                        {
                            // join
                            enrolledBtn.SetActive(false);
                            joinBtn.SetActive(true);
                            observeBtn.SetActive(false);
                            registerBtn.SetActive(false);
                            lateRegisterBtn.SetActive(false);
                        }
                        else
                        {
                            // observe
                            enrolledBtn.SetActive(false);
                            joinBtn.SetActive(false);
                            observeBtn.SetActive(true);
                            registerBtn.SetActive(false);
                            lateRegisterBtn.SetActive(false);
                        }

                        dateTextParent.GetComponent<Text>().text = "Reg. ends in";

                        if ((totalSecondsLateReg < 0) && (differenceLateReg.Days == 0) && (differenceLateReg.Hours == 0))
                        {
                            if (data.isRegistered)
                            {
                                // join
                                enrolledBtn.SetActive(false);
                                joinBtn.SetActive(true);
                                observeBtn.SetActive(false);
                                registerBtn.SetActive(false);
                                lateRegisterBtn.SetActive(false);
                            }
                            else
                            {
                                // late reg.
                                enrolledBtn.SetActive(false);
                                joinBtn.SetActive(false);
                                observeBtn.SetActive(false);
                                registerBtn.SetActive(false);
                                lateRegisterBtn.SetActive(true);
                            }

                            StartCoroutine(TournamentStartTimer(lateRegEnd, totalSecondsLateReg, timerTextParent, (myReturnValue) =>
                            {
                                if (myReturnValue)
                                {
                                    if (data.isRegistered)
                                    {
                                        // join
                                        enrolledBtn.SetActive(false);
                                        joinBtn.SetActive(true);
                                        observeBtn.SetActive(false);
                                        registerBtn.SetActive(false);
                                        lateRegisterBtn.SetActive(false);
                                    }
                                    else
                                    {
                                        // observe
                                        enrolledBtn.SetActive(false);
                                        joinBtn.SetActive(false);
                                        observeBtn.SetActive(true);
                                        registerBtn.SetActive(false);
                                        lateRegisterBtn.SetActive(false);
                                    }
                                    dateTextParent.GetComponent<Text>().text = gameStartDate.ToString(@"MM'/'dd");
                                    timerTextParent.GetComponent<Text>().text = gameStartDate.ToString(@"HH':'mm");
                                }
                            }));
                        }
                    }
                }));
            }
            else if ((totSecondLateRegWithNow < 0) && (diffLateRegWithNow.Days == 0) && (diffLateRegWithNow.Hours == 0))
            {
                dateTextParent.GetComponent<Text>().text = "Reg. ends in";

                StartCoroutine(TournamentStartTimer(lateRegEnd, totSecondLateRegWithNow, timerTextParent, (myReturnValue) =>
                {
                    if (myReturnValue)
                    {
                        if (data.isRegistered)
                        {
                            // join
                            enrolledBtn.SetActive(false);
                            joinBtn.SetActive(true);
                            observeBtn.SetActive(false);
                            registerBtn.SetActive(false);
                            lateRegisterBtn.SetActive(false);
                        }
                        else
                        {
                            // observe
                            enrolledBtn.SetActive(false);
                            joinBtn.SetActive(false);
                            observeBtn.SetActive(true);
                            registerBtn.SetActive(false);
                            lateRegisterBtn.SetActive(false);
                        }
                        dateTextParent.GetComponent<Text>().text = gameStartDate.ToString(@"MM'/'dd");
                        timerTextParent.GetComponent<Text>().text = gameStartDate.ToString(@"HH':'mm");
                    }
                }));
            }

            lateRegisterBtn.GetComponent<Button>().onClick.AddListener(() => OnClickOnRegisterForTournament(tournyId));
            enrolledBtn.GetComponent<Button>().onClick.AddListener(() => OnClickTournamentDetails());

            //END: for timer

            gm.transform.GetChild(11).GetComponent<Button>().onClick.AddListener(() => OnClickOnRegisterForTournament(tournyId));
            gm.transform.GetChild(12).GetComponent<Button>().onClick.AddListener(() => OnClickTournamentJoinRoom(tournyId));

            gm.transform.GetComponent<Button>().onClick.AddListener(() => OnClickTournamentDetails());
            //loadRoomImage(data.roomIconUrl, gm);
            //LoadRoomBG(data.roomBG, gm);            
        }

        //layoutManager.UpdateLayout();

    }


    //Coroutine for timer of gameStart
    IEnumerator TournamentStartTimer(DateTime tournDateTime, int totalSeconds, GameObject timerTextParent, System.Action<bool> callback)
    {
        timerTextParent.GetComponent<Image>().sprite = timerOnSprite;
        Text timerText = timerTextParent.transform.GetChild(0).GetComponent<Text>();
        timerText.color = Color.black;
        //Debug.Log("#### coroutine tournDateTime: " + tournDateTime + ", totalSeconds: " + totalSeconds);

        for (int i = totalSeconds; i <= 0; i++) //for (int i = (totalSeconds * -1); i >= 0; i--)
        {
            TimeSpan difference1 = DateTime.Now.Subtract(tournDateTime);
            //Debug.Log("### Coroutine: difference1: " + difference1.Days + " days, " + difference1.Hours + " hours, " + difference1.Minutes + " minutes, " + difference1.Seconds + " seconds, " + difference1.TotalSeconds + " totalsecond");
            if (i == 0)
            {
                timerTextParent.GetComponent<Image>().sprite = timerOffSprite;
                timerText.color = Color.white;
                timerText.text = (tournDateTime.ToLocalTime()).ToString(@"HH':'mm");
                yield return null;
                callback(true);
                break;
            }
            else
            {
                timerText.text = (difference1.Minutes * -1).ToString("D2") + ":" + (difference1.Seconds * -1).ToString("D2");
                yield return new WaitForSecondsRealtime(1f);
            }
        }
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

        //IDictionary tdictionary = data[0] as IDictionary;

        //if (tdictionary.Contains("data"))
        //{
        //    int totalData = data[0]["data"].Count;

        //    print("Total Data: " + totalData);
        //    if (totalData > 0)
        //    {
        //        ResetContainers();
        //        ResetRoomData();

        //        for (int i = 0; i < totalData; i++)
        //        {
        //            TournamentRoomData roomData = new TournamentRoomData();

        //            roomData.id = data[0]["data"][i]["id"].ToString();
        //            roomData.name = data[0]["data"][i]["name"].ToString();
        //            roomData.modifiedAt = data[0]["data"][i]["modified_at"].ToString();
        //            roomData.isRebuy = !data[0]["data"][i]["is_rebuy"].ToString().Equals("0");
        //            roomData.isAddOn = !data[0]["data"][i]["is_addon"].ToString().Equals("0");
        //            roomData.isFreezOut = !data[0]["data"][i]["is_freez_out"].ToString().Equals("0");
        //            roomData.isLaterRegister = !data[0]["data"][i]["is_later_register"].ToString().Equals("0");

        //            if (data[0]["data"][i]["entry_chip_type"] != null)
        //                roomData.entryChipType = data[0]["data"][i]["entry_chip_type"].ToString();

        //            if (data[0]["data"][i]["entry_chip_amt"] != null)
        //                roomData.entryChipAmt = int.Parse(data[0]["data"][i]["entry_chip_amt"].ToString());

        //            if (data[0]["data"][i]["rebuy_amt"] != null)
        //                roomData.rebuyAmt = int.Parse(data[0]["data"][i]["rebuy_amt"].ToString());

        //            if (data[0]["data"][i]["addon_amt"] != null)
        //                roomData.addonAmt = int.Parse(data[0]["data"][i]["addon_amt"].ToString());

        //            roomData.defaultStack = int.Parse(data[0]["data"][i]["default_stack"].ToString());
        //            roomData.sb = int.Parse(data[0]["data"][i]["sb"].ToString());
        //            roomData.bb = int.Parse(data[0]["data"][i]["bb"].ToString());
        //            roomData.prizeType = !data[0]["data"][i]["prize_type"].ToString().Equals("0");
        //            roomData.minPlayer = int.Parse(data[0]["data"][i]["min_player"].ToString());
        //            roomData.maxPlayer = int.Parse(data[0]["data"][i]["max_player"].ToString());
        //            roomData.regStart = data[0]["data"][i]["reg_start"].ToString();
        //            roomData.gameStart = data[0]["data"][i]["game_start"].ToString();
        //            roomData.lateRegStart = data[0]["data"][i]["lat_reg_start"].ToString();
        //            roomData.status = int.Parse(data[0]["data"][i]["status"].ToString());
        //            roomData.isRegistered = !data[0]["data"][i]["is_registered"].ToString().Equals("0");
        //            roomData.registeredUsers = int.Parse(data[0]["data"][i]["registered_users"].ToString());

        //            switch (data[0]["data"][i]["type"].ToString())
        //            {
        //                case "SpinUp":
        //                    roomData.type = TournamentType.SpinUp;
        //                    break;

        //                case "MTT":
        //                    roomData.type = TournamentType.MTT;
        //                    break;

        //                default:
        //                    roomData.type = TournamentType.Recommended;
        //                    break;
        //            }

        //            allRoomData[(int)roomData.type].Add(roomData);
        //            allRoomData[0].Add(roomData);
        //        }
        //        ShowScreen(TournamentType.MTT);
        //    }
        //}
        //else
        //{
            int totalData = data[0].Count;
            print("Total Data: " + totalData);
            if (totalData > 0)
            {
                ResetContainers();
                ResetRoomData();

                for (int i = 0; i < totalData; i++)
                {
                    TournamentRoomData roomData = new TournamentRoomData();

                    roomData.id = data[0]/*["data"]*/[i]["id"].ToString();
                    roomData.name = data[0]/*["data"]*/[i]["name"].ToString();
                    roomData.modifiedAt = data[0]/*["data"]*/[i]["modified_at"].ToString();
                    roomData.isRebuy = !data[0]/*["data"]*/[i]["is_rebuy"].ToString().Equals("0");
                    roomData.isAddOn = !data[0]/*["data"]*/[i]["is_addon"].ToString().Equals("0");
                    roomData.isFreezOut = !data[0]/*["data"]*/[i]["is_freez_out"].ToString().Equals("0");
                    roomData.isLaterRegister = !data[0]/*["data"]*/[i]["is_later_register"].ToString().Equals("0");

                    if (data[0]/*["data"]*/[i]["entry_chip_type"] != null)
                        roomData.entryChipType = data[0]/*["data"]*/[i]["entry_chip_type"].ToString();

                    if (data[0]/*["data"]*/[i]["entry_chip_amt"] != null)
                        roomData.entryChipAmt = int.Parse(data[0]/*["data"]*/[i]["entry_chip_amt"].ToString());

                    if (data[0]/*["data"]*/[i]["rebuy_amt"] != null)
                        roomData.rebuyAmt = int.Parse(data[0]/*["data"]*/[i]["rebuy_amt"].ToString());

                    if (data[0]/*["data"]*/[i]["addon_amt"] != null)
                        roomData.addonAmt = int.Parse(data[0]/*["data"]*/[i]["addon_amt"].ToString());

                    roomData.defaultStack = int.Parse(data[0]/*["data"]*/[i]["default_stack"].ToString());
                    roomData.sb = int.Parse(data[0]/*["data"]*/[i]["sb"].ToString());
                    roomData.bb = int.Parse(data[0]/*["data"]*/[i]["bb"].ToString());
                    roomData.prizeType = !data[0]/*["data"]*/[i]["prize_type"].ToString().Equals("0");
                    roomData.minPlayer = int.Parse(data[0]/*["data"]*/[i]["min_player"].ToString());
                    roomData.maxPlayer = int.Parse(data[0]/*["data"]*/[i]["max_player"].ToString());
                    roomData.regStart = data[0]/*["data"]*/[i]["reg_start"].ToString();
                    roomData.gameStart = data[0]/*["data"]*/[i]["game_start"].ToString();

                    if(data[0]/*["data"]*/[i]["lat_reg_start"] != null)
                        roomData.lateRegStart = data[0]/*["data"]*/[i]["lat_reg_start"].ToString();
                    roomData.status = int.Parse(data[0]/*["data"]*/[i]["status"].ToString());
                    roomData.isRegistered = !data[0]/*["data"]*/[i]["is_registered"].ToString().Equals("0");
                    roomData.registeredUsers = int.Parse(data[0]/*["data"]*/[i]["registered_users"].ToString());

                    switch (data[0]/*["data"]*/[i]["type"].ToString())
                    {
                        case "SpinUp":
                            roomData.type = TournamentType.SpinUp;
                            break;

                        case "MTT":
                            roomData.type = TournamentType.MTT;
                            break;

                        default:
                            roomData.type = TournamentType.Recommended;
                            break;
                    }

                    allRoomData[(int)roomData.type].Add(roomData);
                    allRoomData[0].Add(roomData);
                }
                //print("Showing Screen");
                ShowScreen(TournamentType.MTT);
            }
        //}

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

    //DEV_CODE
    private void OnClickOnRegisterForTournament(int id)
    {
        SoundManager.instance.PlaySound(SoundType.Click);

        TournamentSocketController.instance.RequestRegisterForTournament(id);
    }

    private void OnClickTournamentJoinRoom(double id)
    {
        SoundManager.instance.PlaySound(SoundType.Click);

        TournamentSocketController.instance.RequestTournamentJoinRoom(id);
    }

    private void OnClickTournamentDetails()
    {
        TournamentInGameUiManager.instance.ShowScreen(TournamentInGameScreens.TournamentDetails);
        //tournamentDetailsPanel.SetActive(true);
    }

    private void ResetRoomData()
    {
        allRoomData.Clear();
        for (int i = 0; i < 3; i++)
        {
            List<TournamentRoomData> dummyList = new List<TournamentRoomData>();
            allRoomData.Add(dummyList);
            //print("Adding Dummy Data");
        }
    }

    private void ResetContainers()
    {
        //Reseting Container
        for (int i = 0; i < container.childCount; i++)
        {
            Destroy(container.GetChild(i).gameObject);
        }
    }

    public IEnumerator ShowPopUp(string msg, float delay)
    {
        popUpText.gameObject.SetActive(true);
        popUpText.text = msg;
        yield return new WaitForSeconds(delay);
        popUpText.gameObject.SetActive(false);
    }
}

public enum TournamentType
{
    Recommended,
    SpinUp,
    MTT
}

public class TournamentRoomData
{
    public string id;
    public string name;
    public TournamentType type;
    public string modifiedAt;
    public bool isRebuy;
    public bool isAddOn;
    public bool isFreezOut;
    public bool isLaterRegister;
    public string entryChipType;
    public int entryChipAmt;
    public int rebuyAmt;
    public int addonAmt;
    public int defaultStack;
    public int sb;
    public int bb;
    public bool prizeType;
    public int minPlayer;
    public int maxPlayer;
    public string regStart;
    public string gameStart;
    public string lateRegStart;
    public int status;
    public bool isRegistered;
    public int registeredUsers;
}
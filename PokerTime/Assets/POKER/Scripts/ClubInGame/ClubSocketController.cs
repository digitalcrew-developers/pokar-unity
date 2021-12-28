using System.Collections.Generic;
using UnityEngine;
using System;
using BestHTTP.SocketIO;
using System.Collections;
using BestHTTP.JSON;
using LitJson;
using DG.Tweening;
using UnityEngine.UI;

public class ClubSocketController : MonoBehaviour
{
    public static ClubSocketController instance;

    private const float RESPONSE_READ_DELAY = 0.2f, REQUEST_SEND_DELAY = 0.2f;
    private SocketManager socketManager;

    private List<SocketResponse> socketResponse = new List<SocketResponse>();
    private List<SocketRequest> socketRequest = new List<SocketRequest>();

    [SerializeField]
    private SocketState socketState;

    [SerializeField]
    private string TABLE_ID = "";
    [SerializeField]
    GameObject clubDetail, clubDetailLayer;
    public GameObject buttonCanvas;
    public GameObject[] tableButton;
    public Sprite[] tableButtonSprite;


    void Awake()
    {
        instance = this;
        SetSocketState(SocketState.NULL);

        Canvas btnCanvas = buttonCanvas.GetComponent<Canvas>();
        btnCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        btnCanvas.worldCamera = Camera.main;
    }

    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 30;
        if (GlobalGameManager.IsJoiningPreviousGame)
        {
            TABLE_ID = GlobalGameManager.instance.GetRoomData().socketTableId;
        }

        Connect();
        StartCoroutine(WaitAndCheckInternetConnection());
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            RequestForMatchHistory();
        }
        
        if(SwipeManager.swipeLeft)
        {
            if (GlobalGameManager.instance.AllTables.Count == 1)
                return;
            GlobalGameManager.currentTableInd--;
            if (GlobalGameManager.currentTableInd < 0)
                GlobalGameManager.currentTableInd = GlobalGameManager.instance.AllTables.Count - 1;
            Debug.Log("Left Swap " + GlobalGameManager.currentTableInd);
            MoveTable(GlobalGameManager.currentTableInd);
        }
        else if(SwipeManager.swipeRight)
        {
            int index = 0;
            for (int i = 0; i < GlobalGameManager.instance.AllTables.Count; i++)
            {
                if (GlobalGameManager.instance.table[i].transform.GetChild(0).GetChild(0).localPosition.x == 0)
                {
                    index = i;
                    break;  // According to question, you are after the key 
                }
            }
            Debug.Log(index + " - " + GlobalGameManager.instance.AllTables.Count);
            //if (GlobalGameManager.currentTableInd >= 2 || GlobalGameManager.instance.AllTables.Count == 1 || (GlobalGameManager.instance.AllTables.Count == (index + 1)))
            if (GlobalGameManager.instance.AllTables.Count == 1)
                return;

            GlobalGameManager.currentTableInd++;
            if (GlobalGameManager.currentTableInd > 2 || (GlobalGameManager.instance.AllTables.Count == (index + 1)))
                GlobalGameManager.currentTableInd = 0;
            Debug.Log("Left Swap " + GlobalGameManager.currentTableInd);
            MoveTable(GlobalGameManager.currentTableInd);
        }
    }


    private void OnDestroy()
    {
        //    SendLeaveMatchRequest();
    }


    public void Connect(bool isReconnecting = false)
    {
        ResetConnection(isReconnecting);
        SetSocketState(SocketState.Connecting);


        if (!IsInvoking("HandleSocketResponse"))
        {
            InvokeRepeating("HandleSocketResponse", RESPONSE_READ_DELAY, RESPONSE_READ_DELAY);
        }

        if (!IsInvoking("SendSocketRequest"))
        {
            InvokeRepeating("SendSocketRequest", REQUEST_SEND_DELAY, REQUEST_SEND_DELAY);
        }

        ReConnect();
    }


    private void ReConnect()
    {
        socketManager = null;
        SocketOptions socketOptions = new SocketOptions();
        socketOptions.Timeout = new TimeSpan(0, 0, 4);
        socketOptions.Reconnection = false;
        socketOptions.AutoConnect = false;
        socketOptions.ReconnectionDelayMax = new TimeSpan(0, 0, 4);

        socketManager = new SocketManager(new Uri(GameConstants.CLUB_SOCKET_URL + "/socket.io/"), socketOptions);
        //        Debug.LogError("URL IS " + GameConstants.SOCKET_URL + "/socket.io/");
        socketManager.Socket.On(SocketIOEventTypes.Connect, OnServerConnect);
        socketManager.Socket.On(SocketIOEventTypes.Disconnect, OnServerDisconnect);
        socketManager.Socket.On(SocketIOEventTypes.Error, OnError);

        //Default Events
        socketManager.Socket.On("reconnect", OnReconnect);
        socketManager.Socket.On("reconnecting", OnReconnecting);
        socketManager.Socket.On("reconnect_attempt", OnReconnectAttempt);
        socketManager.Socket.On("reconnect_failed", OnReconnectFailed);


        //Custom Events
        socketManager.Socket.On("playerObject", OnPlayerObjectFound);
        socketManager.Socket.On("openCards", OnOpenCardDataFound);
        //socketManager.Socket.On("openCardTimmer", OnOpenCardTimerFound);
        socketManager.Socket.On("nextRoundTimmer", OnNextRoundTimerFound);
        socketManager.Socket.On("gameOverTimmer", OnGameOverTimerFound);
        socketManager.Socket.On("callTimmer", OnCallTimerFound);
        socketManager.Socket.On("startGameTimmer", OnStartGameTimerFound);
        socketManager.Socket.On("currentRound", OnRoundNoFound);

        //socketManager.Socket.On("potUpdate", OnPotDataFound);
        socketManager.Socket.On("tableResults", OnResultDataFound);
        //socketManager.Socket.On("roundWinnerTimmer", OnResultTimerFound);
        socketManager.Socket.On("betData", OnBetDataFound);
        //socketManager.Socket.On("cardDistributeTimmer", OnCardDistributeTimerFound);
        socketManager.Socket.On("sendMessage", OnMessageFound);
        socketManager.Socket.On("userReconnect", OnReconnected);
        socketManager.Socket.On("matchPlayLogs", MatchHistory);
        socketManager.Socket.On("sendEmoji", OnSentEmoji);
        socketManager.Socket.On("predictionReward", OnSendWinningBooster);
        socketManager.Socket.On("getRandomCard", OnGetRandomCard);

        socketManager.Socket.On("playerStatndOut", OnPlayerStandUp);
        socketManager.Socket.On("allTipData", OnAllTipData);
        socketManager.Socket.On("pointUpdate", OnPointUpdate);
        socketManager.Socket.On("minMaxAppEmit", MinimizeAppServer);
        socketManager.Socket.On("seatObject", SeatObjectsReceived);
        socketManager.Socket.On("rabbitOpenCards", RabbitCardDataReceived);
        //socketManager.Socket.On("evChopData", EVChopDataReceived);  //DEV_CODE Added Event for EVChopDataReceived called
        socketManager.Socket.On("playerExit", PlayerExit);          //DEV_CODE Added Event for PlayerExit called
        socketManager.Socket.On("askEvChop", EVChopDataReceived);
        socketManager.Socket.On("closePopUp", EVChopCloseDataReceived);
        socketManager.Socket.On("tableExit", TableExit);

        //DEV_CODE
        socketManager.Socket.On("askMultiRunAction", OnAskMultiRunAction);
        socketManager.Socket.On("confirmMultiRunAction", OnConfirmMultiRunAction);

        socketManager.Socket.On("askMultiRun", OnAskMultiRun);
        socketManager.Socket.On("confirmMultiRun", OnConfirmMultiRun);

        //socketManager.Socket.On("closePopUp", OnClosePopUp);
        socketManager.Socket.On("comCard1", OnComCards1);
        socketManager.Socket.On("comCard2", OnComCards2);
        socketManager.Socket.On("matchRunItTimes", MatchRunItTimes);
        socketManager.Socket.On("checkClubCoins", CheckClubCoins);
        socketManager.Socket.On("match", Match);

        socketManager.Open();
    }

    bool isPaused = false;

    void OnGUI()
    {
        if (isPaused)
        {
            //MinimizeAppEvent();
            GUI.Label(new Rect(100, 100, 50, 30), "Game paused");
        }
    }

    void OnApplicationFocus(bool hasFocus)
    {
#if !UNITY_EDITOR
        isPaused = !hasFocus;
        if (!isPaused)
        {
            MaximizeAppEvent();
        }
        if (isPaused)
        {
            MinimizeAppEvent();
        }
#endif
    }

    void OnApplicationPause(bool pauseStatus)
    {
        isPaused = pauseStatus;
        if (isPaused)
        {
            MinimizeAppEvent();
        }
    }

    private void MinimizeAppServer(Socket socket, Packet packet, object[] args)
    {
        string responseText = JsonMapper.ToJson(args);
        Debug.Log(responseText);
    }

    //DEV_CODE 
    private void Match(Socket socket, Packet packet, object[] args)
    {
        //ClubInGameManager.instance.DontShowCommunityCardAnimation = true;
        string responseText = JsonMapper.ToJson(args);
        Debug.Log("<color=magenta>Match :</color> " + responseText);
    }

    private void CheckClubCoins(Socket socket, Packet packet, object[] args)
    {
        //ClubInGameManager.instance.DontShowCommunityCardAnimation = true;
        string responseText = JsonMapper.ToJson(args);
        Debug.Log("<color=magenta>CheckClubCoins :</color> " + responseText);
    }
    private void OnAskMultiRunAction(Socket socket, Packet packet, object[] args)
    {
        string responseText = JsonMapper.ToJson(args);
        Debug.Log("OnAskMultiRunReceived :" + responseText);
    }

    private void OnConfirmMultiRunAction(Socket socket, Packet packet, object[] args)
    {
        string responseText = JsonMapper.ToJson(args);
        Debug.Log("ConfirmMultiRunActionReceived :" + responseText);
        //ClubInGameManager.instance.MultiRunActionPanel.SetActive(true);
    }

    private void OnAskMultiRun(Socket socket, Packet packet, object[] args)
    {
        string responseText = JsonMapper.ToJson(args);
        Debug.Log("OnAskMultiRun :" + responseText);
        currentTable.transform.GetChild(1).GetComponent<ClubInGameManager>().MultiRunPanel.SetActive(true);
        //ClubInGameManager.instance.MultiRunPanel.SetActive(true);
    }

    private void OnConfirmMultiRun(Socket socket, Packet packet, object[] args)
    {
        string responseText = JsonMapper.ToJson(args);
        Debug.Log("OnConfirmMultiRun :" + responseText);
        currentTable.transform.GetChild(1).GetComponent<ClubInGameManager>().MultiRunActionPanel.SetActive(true);
        //ClubInGameManager.instance.MultiRunActionPanel.SetActive(true);
    }


    /*private void OnClosePopUp(Socket socket, Packet packet, object[] args)
    {
        string responseText = JsonMapper.ToJson(args);
        Debug.Log("OnClosePopUp :" + responseText);
    }*/
    int runItTimes = 0;
    private void MatchRunItTimes(Socket socket, Packet packet, object[] args)
    {
        //JsonData data = JsonMapper.ToObject(args);

        //int remainingTime = (int)float.Parse(data[0].ToString());

        string responseText = JsonMapper.ToJson(args);
        Debug.Log("<color=magenta>MatchRunItTimes :</color> " + responseText + ", " + args[0] + ", " + runItTimes);
        runItTimes = int.Parse(args[0].ToString());
        Debug.Log("<color=magenta>MatchRunItTimes :</color> " + responseText + ", " + runItTimes);
    }

    private void OnComCards1(Socket socket, Packet packet, object[] args)
    {
        //ClubInGameManager.instance.DontShowCommunityCardAnimation = true;
        string responseText = JsonMapper.ToJson(args);
        Debug.Log("<color=magenta>OnComCards1 :</color> " + responseText);
        if (runItTimes == 2)
            StartCoroutine(currentTable.transform.GetChild(1).GetComponent<ClubInGameManager>().ShowMultiRunCards(responseText, ClubInGameManager.instance.communityCardLayer2.position));
        else if (runItTimes == 1)
            StartCoroutine(currentTable.transform.GetChild(1).GetComponent<ClubInGameManager>().ShowMultiRunCards(responseText, ClubInGameManager.instance.communityCardLayer1.position));
    }

    private void OnComCards2(Socket socket, Packet packet, object[] args)
    {
        //ClubInGameManager.instance.DontShowCommunityCardAnimation = true;
        string responseText = JsonMapper.ToJson(args);
        Debug.Log("<color=magenta>OnComCards2 :</color> " + responseText);
        StartCoroutine(currentTable.transform.GetChild(1).GetComponent<ClubInGameManager>().ShowMultiRunCards(responseText, ClubInGameManager.instance.communityCardLayer1.position));
    }

    //This method to be called when EVChopDataReceived emited
    private void EVChopDataReceived(Socket socket, Packet packet, object[] args)
    {
        //Debug.Log("<color=magenta>EVChopDataReceived :</color>" + args);
        string responseText = JsonMapper.ToJson(args);

        //Debug.LogError("EVChopDataReceived :" + responseText);

#if DEBUG

#if UNITY_EDITOR
        if (GlobalGameManager.instance.CanDebugThis(SocketEvetns.EVCHOP))
        {
            Debug.Log("EVChopDataReceived = " + responseText + "  Time = " + System.DateTime.Now);
        }
#else
        Debug.Log("OnNextRoundTimerFound = " + responseText + "  Time = " + System.DateTime.Now);
#endif
#endif

        SocketResponse response = new SocketResponse();
        response.eventType = SocketEvetns.EVCHOP;
        response.data = responseText;
        socketResponse.Add(response);
    }

    private void EVChopCloseDataReceived(Socket socket, Packet packet, object[] args)
    {
        string responseText = JsonMapper.ToJson(args);
        Debug.LogError("EVChopCloseDataReceived :" + responseText);
        ClubInGameManager.instance.HideEVChopButtons();
    }

    private void RabbitCardDataReceived(Socket socket, Packet packet, object[] args)
    {
        string responseText = JsonMapper.ToJson(args);
        Debug.LogError("RabbitCardDataReceived :" + responseText);

#if DEBUG

#if UNITY_EDITOR
        if (GlobalGameManager.instance.CanDebugThis(SocketEvetns.RABBIT_CARDS))
        {
            Debug.Log("RABBIT_CARDS = " + responseText + "  Time = " + System.DateTime.Now);
        }
#else
        Debug.Log("OnNextRoundTimerFound = " + responseText + "  Time = " + System.DateTime.Now);
#endif
#endif

        SocketResponse response = new SocketResponse();
        response.eventType = SocketEvetns.RABBIT_CARDS;
        response.data = responseText;
        socketResponse.Add(response);
    }

    private void SeatObjectsReceived(Socket socket, Packet packet, object[] args)
    {
        string responseText = JsonMapper.ToJson(args);
        JsonData data = JsonMapper.ToObject(responseText);
        Debug.LogError(responseText);
        JsonData commanData = JsonMapper.ToObject(responseText);
        Debug.LogError(commanData[0]["tableId"].ToString());
        GameObject ct = GlobalGameManager.instance.AllTables[commanData[0]["tableId"].ToString()];
        ct.transform.GetChild(1).GetComponent<ClubInGameManager>().SetClubGameState(ClubGameState.WaitingForOpponent);
        //Debug.LogError(data["data"]);
        //InGameManager.instance.gameExitCalled = true;
        //ResetConnection();
    }

    private void TableExit(Socket socket, Packet packet, object[] args)
    {
        string responseText = JsonMapper.ToJson(args);
        Debug.LogError("Response => TableExit: " + responseText);
        JsonData commanData = JsonMapper.ToObject(responseText);
        Debug.LogError(commanData[0]["tableId"].ToString());

        GlobalGameManager.instance.AllTables.Remove(commanData[0]["tableId"].ToString());
        for (int i = 0; i < GlobalGameManager.instance.table.Count; i++)
        {
            if (GlobalGameManager.instance.table[i].name == commanData[0]["tableId"].ToString())
            {
                GlobalGameManager.instance.table.RemoveAt(i);
            }
        }
        ResetTablesAfterClose("");
        Destroy(GameObject.Find(commanData[0]["tableId"].ToString()));
    }

    //DEV_CODE Added this method to be called when PlayerExit event emited 
    private void PlayerExit(Socket socket, Packet packet, object[] args)
    {
        string responseText = JsonMapper.ToJson(args);
        Debug.LogError("Response => PlayerExit: " + responseText);
        currentTable.transform.GetChild(1).GetComponent<ClubInGameManager>().gameExitCalled = true;
        //ClubInGameManager.instance.gameExitCalled = true;
        ResetConnection();
    }
    public GameObject currentTable = null;
    private void HandleSocketResponse()
    {
        if (socketResponse.Count > 0)
        {
            SocketResponse responseObject = socketResponse[0];
            socketResponse.RemoveAt(0);

#if DEBUG

#if UNITY_EDITOR
            //if (GlobalGameManager.instance.CanDebugThis(responseObject.eventType))
            //{
            //    Debug.Log("---------------Handling ServerResponse = " + responseObject.eventType + "  socketState = " + socketState + "   data = " + responseObject.data);
            //}
#else
            Debug.LogError("Handling ServerResponse = " + responseObject.eventType + "  socketState = " + socketState + "   data = " + responseObject.data);
#endif

#endif
            Debug.LogWarning(ClubInGameManager.instance.userWinner + " <color=yellow>Event " + responseObject.eventType + ",</color> " + responseObject.data);
            //GameObject table = null;
            if (responseObject.data != null && responseObject.data.Contains("tableId"))
            {
                JsonData commanData = JsonMapper.ToObject(responseObject.data);
                Debug.Log(GlobalGameManager.instance.AllTables.ContainsKey(commanData[0]["tableId"].ToString()) + " - " + commanData[0]["tableId"].ToString());
                if (GlobalGameManager.instance.AllTables.ContainsKey(commanData[0]["tableId"].ToString()))
                    currentTable = GlobalGameManager.instance.AllTables[commanData[0]["tableId"].ToString()];
                //currentTable = GameObject.Find(commanData[0]["tableId"].ToString());
            }
            switch (responseObject.eventType)
            {
                case SocketEvetns.CONNECT:
                    {
                        switch (GetSocketState())
                        {
                            case SocketState.Connecting:
                                SetSocketState(SocketState.WaitingForOpponent);

                                Debug.Log("<color=yellow>IsJoiningPreviousGame " + GlobalGameManager.IsJoiningPreviousGame + "</color>");

                                if (GlobalGameManager.IsJoiningPreviousGame)
                                {
                                    RequestForMatchStatus();
                                }
                                else
                                {
                                    Debug.Log("Going into club..." + GlobalGameManager.instance.GetRoomData().exclusiveTable);
                                    if (!GlobalGameManager.instance.GetRoomData().exclusiveTable.Equals("On") || GlobalGameManager.instance.GetRoomData().assignRole.Equals("Creater"))
                                    {
                                        SendClubGameJoinRequest();
                                    }
                                    //SendGameJoinRequest();
                                }
                                break;

                            case SocketState.ReConnecting:
                                RequestForMatchStatus();
                                break;

                            default:
                                break;

                        }
                    }



                    break;

                case SocketEvetns.DISCONNECT:
                    StartReconnectProcedure();
                    break;


                case SocketEvetns.RECONNECT_ATTEMPT:
                    break;


                case SocketEvetns.PLAYER_OBJECT:
                    //if (ClubInGameManager.instance != null)
                    {
                        Debug.LogError("Current Socket State: " + GetSocketState());

                        if (GetSocketState() == SocketState.ReConnecting)
                        {
                            SetSocketState(SocketState.Game_Running);
                        }
                        Debug.LogError("responseObject.data: " + responseObject.data);
                        JsonData po = JsonMapper.ToObject(responseObject.data);
                        Debug.LogError("TableId: " + po[0]["tableId"].ToString());
                        //GameObject table = GameObject.Find(po[0]["tableId"].ToString());
                        currentTable = GlobalGameManager.instance.AllTables[po[0]["tableId"].ToString()];
                        currentTable.transform.GetChild(1).GetComponent<ClubInGameManager>().OnPlayerObjectFound(responseObject.data);
                        currentTable.transform.GetChild(0).GetComponent<ClubInGameUIManager>().DestroyScreen(InGameScreens.Reconnecting);
                        currentTable.transform.GetChild(0).GetComponent<ClubInGameUIManager>().ShowTableMessage("");
                        /*ClubInGameManager.instance.OnPlayerObjectFound(responseObject.data);
                        ClubInGameUIManager.instance.DestroyScreen(InGameScreens.Reconnecting);
                        ClubInGameUIManager.instance.ShowTableMessage("");*/
                    }
                    //else
                    //{
                    //    Debug.LogError("Null reference exception found ClubInGameManager is null... ");
                    //}
                    break;
                case SocketEvetns.ON_OPEN_CARD_DATA_FOUND:
                    JsonData oc = JsonMapper.ToObject(responseObject.data);
                    //GameObject table = GameObject.Find(oc[0]["tableId"].ToString());
                    currentTable.transform.GetChild(1).GetComponent<ClubInGameManager>().OnOpenCardsDataFound(oc[0]["data"].ToJson());
                    //ClubInGameManager.instance.OnOpenCardsDataFound(oc[0]["data"].ToJson());
                    break;
                case SocketEvetns.RABBIT_CARDS:
                    JsonData rc = JsonMapper.ToObject(responseObject.data);
                    //GameObject table = GameObject.Find(rc[0]["tableId"].ToString());
                    currentTable.transform.GetChild(1).GetComponent<ClubInGameManager>().OnRabbitDataFound(rc[0]["data"].ToJson());
                    //ClubInGameManager.instance.OnRabbitDataFound(rc[0]["data"].ToJson());
                    break;
                case SocketEvetns.EVCHOP:
                    JsonData e = JsonMapper.ToObject(responseObject.data);
                    currentTable.transform.GetChild(1).GetComponent<ClubInGameManager>().OnEVChopDataFound(e[0]["data"].ToJson());
                    //ClubInGameManager.instance.OnEVChopDataFound(e[0]["data"].ToJson());
                    break;

                //case SocketEvetns.ON_OPEN_CARD_TIMER_FOUND:
                //ClubInGameManagerScript.instance.OnOpenCardTimerFound(responseObject.data);
                //break;

                case SocketEvetns.ON_NEXT_ROUND_TIMER_FOUND:
                    JsonData n = JsonMapper.ToObject(responseObject.data);
                    currentTable.transform.GetChild(1).GetComponent<ClubInGameManager>().OnNextMatchCountDownFound(n[0]["data"].ToJson());
                    //ClubInGameManager.instance.OnNextMatchCountDownFound(n[0]["data"].ToJson());
                    break;

                case SocketEvetns.ON_GAME_OVER_TIMER_FOUND:
                    Debug.LogError("Game Over - " + responseObject.data);
                    JsonData o = JsonMapper.ToObject(responseObject.data);
                    currentTable.transform.GetChild(1).GetComponent<ClubInGameManager>().OnGameOverCountDownFound(o[0]["data"].ToJson());
                    //ClubInGameManager.instance.OnGameOverCountDownFound(o[0]["data"].ToJson());
                    break;

                case SocketEvetns.ON_CALL_TIMER_FOUND:
                    Debug.LogError("ON_CALL_TIMER_FOUND - " + responseObject.data);
                    JsonData c = JsonMapper.ToObject(responseObject.data);
                    currentTable.transform.GetChild(1).GetComponent<ClubInGameManager>().OnTurnCountDownFound(c[0]["data"].ToJson());
                    //ClubInGameManager.instance.OnTurnCountDownFound(c[0]["data"].ToJson());
                    break;


                case SocketEvetns.ON_GAME_START_TIMER_FOUND:
                    JsonData s = JsonMapper.ToObject(responseObject.data);
                    currentTable.transform.GetChild(1).GetComponent<ClubInGameManager>().OnGameStartTimeFound(s[0]["data"].ToJson());
                    //ClubInGameManager.instance.OnGameStartTimeFound(s[0]["data"].ToJson());
                    break;

                case SocketEvetns.ON_ROUND_NO_FOUND:
                    // ClubInGameUIManager.instance.LoadingImage.SetActive(false);
                    JsonData d = JsonMapper.ToObject(responseObject.data);
                    currentTable.transform.GetChild(1).GetComponent<ClubInGameManager>().OnRoundDataFound(d[0]["data"].ToJson());
                    //ClubInGameManager.instance.OnRoundDataFound(d[0]["data"].ToJson());
                    break;

                //case SocketEvetns.ON_POT_DATA_FOUND:
                //ClubInGameManagerScript.instance.OnPotDataFound(responseObject.data);
                //break;

                case SocketEvetns.ON_RESULT_FOUND:
                    Debug.LogError("Result - " + responseObject.data);
                    JsonData r = JsonMapper.ToObject(responseObject.data);
                    currentTable.transform.GetChild(1).GetComponent<ClubInGameManager>().OnResultResponseFound(r[0]["data"].ToJson());
                    //ClubInGameManager.instance.OnResultResponseFound(responseObject.data);
                    break;

                //case SocketEvetns.ON_RESULT_TIMER_FOUND:
                //break;

                case SocketEvetns.ON_BET_DATA_FOUND:
                    //ClubInGameManager.instance.PlayerTimerReset();
                    currentTable.transform.GetChild(1).GetComponent<ClubInGameManager>().PlayerTimerReset();
                    JsonData b = JsonMapper.ToObject(responseObject.data);
                    currentTable.transform.GetChild(1).GetComponent<ClubInGameManager>().OnBetDataFound(b[0]["data"].ToJson());
                    //ClubInGameManager.instance.OnBetDataFound(b[0]["data"].ToJson());
                    break;

                //case SocketEvetns.ON_CARD_DISTRIBUTE_TIMER_FOUND:
                //ClubInGameManagerScript.instance.OnCardDistributeTimerFound(responseObject.data);
                //break;

                case SocketEvetns.ON_MESSAGE_FOUND:
                    ChatManager.instance.OnChatMessageReceived(responseObject.data);
                    break;


                case SocketEvetns.ON_RECONNECTED:
                    if (!responseObject.data.Contains("Reconnect Success"))
                    {
                        ClubInGameUIManager.instance.DestroyScreen(InGameScreens.Reconnecting);

                        ClubInGameUIManager.instance.ShowMessage("Match Data not found", () =>
                        {
                            ClubInGameManager.instance.LoadMainMenu();
                        });
                    }

                    break;

                case SocketEvetns.ON_MATCH_HISTORY_FOUND:

                    //if (HandHistoryUiManager.instance != null)
                    //{
                    //    HandHistoryUiManager.instance.OnMatchHistoryFound(responseObject.data);
                    //}
                    break;

                case SocketEvetns.ON_PlayerStandUp:
                    ClubInGameManager.instance.StandUpPlayer(responseObject.data);
                    break;
                case SocketEvetns.ON_SendEmoji:
                    JsonData se = JsonMapper.ToObject(responseObject.data);
                    currentTable.transform.GetChild(1).GetComponent<ClubInGameManager>().SendEmoji(se[0]["data"].ToJson());
                    //ClubInGameManager.instance.SendEmoji(responseObject.data);
                    break;
                case SocketEvetns.ON_ALL_TIP_DATA:
                    JsonData t = JsonMapper.ToObject(responseObject.data);
                    currentTable.transform.GetChild(1).GetComponent<ClubInGameManager>().TipToDealer(t[0]["data"].ToJson());
                    //ClubInGameManager.instance.TipToDealer(responseObject.data);
                    break;
                case SocketEvetns.ON_POINT_UPDATE:
                    JsonData pu = JsonMapper.ToObject(responseObject.data);
                    currentTable.transform.GetChild(1).GetComponent<ClubInGameManager>().PointUpdated(pu[0]["data"].ToJson());
                    //ClubInGameManager.instance.PointUpdated(responseObject.data);
                    break;
                case SocketEvetns.ON_GET_RANDOM_CARD:
                    //ClubInGameUIManager.instance.DeductCoinPostServer(ClubInGameUIManager.instance.winnigBoosterAmount, responseObject.data.Substring(2, 2));
                    string cardName = responseObject.data.Substring(2, 2);
                    //Debug.Log("Card Name FUll: " + cardName);

                    CardData cardDt = CardsManager.instance.GetCardData(cardName);
                    ClubInGameUIManager.instance.winningBoosterCardName = cardDt.cardNumber.ToString();
                    ClubInGameUIManager.instance.arrowPopUpText.text += ClubInGameUIManager.instance.winningBoosterCardName;

                    //Debug.Log("Name: " + cardDt.cardNumber);
                    break;

                default:
                    Debug.LogError("UnHandlled EventType Found in response eventType = " + responseObject.eventType + "   responseStructure = " + responseObject.data);
                    break;
            }
            //     Debug.Log("responseObject.data***********" + responseObject.data);
        }
    }


    private void SendSocketRequest()
    {
        if (socketRequest.Count > 0)
        {
            SocketRequest request = socketRequest[0];
            socketRequest.RemoveAt(0);

            if (request.plainDataToBeSend != null)
            {
                socketManager.Socket.Emit(request.emitEvent, request.plainDataToBeSend);

#if DEBUG
                Debug.Log("sending Plain request " + request.requestDataStructure + "         event = " + request.emitEvent + "   Time = " + System.DateTime.Now);
#endif
            }
            else if (request.jsonDataToBeSend != null)
            {
                socketManager.Socket.Emit(request.emitEvent, request.jsonDataToBeSend);

#if DEBUG
                Debug.Log("Send Socket Request " + request.requestDataStructure + "          emitEvent   ==" + request.emitEvent + "  Time = " + System.DateTime.Now);
#endif
            }
        }
    }




    // LISTNER_METHODS ---------------------------------------------------------------------------------------------------------------------------------------------------------




    #region LISTNER_METHODS

    void OnPlayerObjectFound(Socket socket, Packet packet, params object[] args)
    {
        string responseText = JsonMapper.ToJson(args);

#if DEBUG

#if UNITY_EDITOR
        if (GlobalGameManager.instance.CanDebugThis(SocketEvetns.PLAYER_OBJECT))
        {
            //Debug.Log("OnPlayerObjectFound = " + responseText + "  Time = " + System.DateTime.Now);

        }
#else
        Debug.Log("OnPlayerObjectFound = " + responseText + "  Time = " + System.DateTime.Now);
#endif
#endif
        SocketResponse response = new SocketResponse();
        response.eventType = SocketEvetns.PLAYER_OBJECT;
        response.data = responseText;
        socketResponse.Add(response);
    }
    void OnOpenCardDataFound(Socket socket, Packet packet, params object[] args)
    {
        string responseText = JsonMapper.ToJson(args);
#if DEBUG

#if UNITY_EDITOR
        if (GlobalGameManager.instance.CanDebugThis(SocketEvetns.ON_OPEN_CARD_DATA_FOUND))
        {
            //Debug.Log("OnOpenCardDataFound = " + responseText + "  Time = " + System.DateTime.Now);

        }
#else
        Debug.Log("OnOpenCardDataFound = " + responseText + "  Time = " + System.DateTime.Now);
#endif
#endif

        SocketResponse response = new SocketResponse();
        response.eventType = SocketEvetns.ON_OPEN_CARD_DATA_FOUND;
        response.data = responseText;
        socketResponse.Add(response);
    }




    void OnOpenCardTimerFound(Socket socket, Packet packet, params object[] args)
    {
        string responseText = JsonMapper.ToJson(args);

#if DEBUG

#if UNITY_EDITOR
        if (GlobalGameManager.instance.CanDebugThis(SocketEvetns.ON_OPEN_CARD_TIMER_FOUND))
        {
            Debug.Log("OnOpenCardTimerFound = " + responseText + "  Time = " + System.DateTime.Now);
        }
#else
        Debug.Log("OnOpenCardTimerFound = " + responseText + "  Time = " + System.DateTime.Now);
#endif
#endif

        SocketResponse response = new SocketResponse();
        response.eventType = SocketEvetns.ON_OPEN_CARD_TIMER_FOUND;
        response.data = responseText;
        socketResponse.Add(response);
    }



    void OnNextRoundTimerFound(Socket socket, Packet packet, params object[] args)
    {
        string responseText = JsonMapper.ToJson(args);

#if DEBUG

#if UNITY_EDITOR
        if (GlobalGameManager.instance.CanDebugThis(SocketEvetns.ON_NEXT_ROUND_TIMER_FOUND))
        {
            Debug.Log("OnNextRoundTimerFound = " + responseText + "  Time = " + System.DateTime.Now);
        }
#else
        Debug.Log("OnNextRoundTimerFound = " + responseText + "  Time = " + System.DateTime.Now);
#endif
#endif

        //DEV_CODE To disable Action Buttons and Suggestion Buttons on round end and going to next round
        if (ClubInGameUIManager.instance.actionButtonParent.activeSelf)
            ClubInGameUIManager.instance.actionButtonParent.SetActive(false);

        if (ClubInGameUIManager.instance.suggestionButtonParent.activeSelf)
            ClubInGameUIManager.instance.suggestionButtonParent.SetActive(false);
        //***********************************************


        SocketResponse response = new SocketResponse();
        response.eventType = SocketEvetns.ON_NEXT_ROUND_TIMER_FOUND;
        response.data = responseText;
        socketResponse.Add(response);
    }




    //socketManager.Socket.On("callTimmer", PlayGame);
    //socketManager.Socket.On("startGameTimmer", PlayGame);

    void OnGameOverTimerFound(Socket socket, Packet packet, params object[] args)
    {
        string responseText = JsonMapper.ToJson(args);

#if DEBUG

#if UNITY_EDITOR
        if (GlobalGameManager.instance.CanDebugThis(SocketEvetns.ON_GAME_OVER_TIMER_FOUND))
        {
            Debug.Log("OnGameOverTimerFound = " + responseText + "  Time = " + System.DateTime.Now);
        }
#else
        Debug.Log("OnGameOverTimerFound = " + responseText + "  Time = " + System.DateTime.Now);
#endif
#endif

        SocketResponse response = new SocketResponse();
        response.eventType = SocketEvetns.ON_GAME_OVER_TIMER_FOUND;
        response.data = responseText;
        socketResponse.Add(response);
    }


    void OnCallTimerFound(Socket socket, Packet packet, params object[] args)
    {
        string responseText = JsonMapper.ToJson(args);

#if DEBUG

#if UNITY_EDITOR
        if (GlobalGameManager.instance.CanDebugThis(SocketEvetns.ON_CALL_TIMER_FOUND))
        {
            //       Debug.Log("OnCallTimerFound = " + responseText + "  Time = " + System.DateTime.Now);
        }
#else
        Debug.Log("OnCallTimerFound = " + responseText + "  Time = " + System.DateTime.Now);
#endif
#endif

        SocketResponse response = new SocketResponse();
        response.eventType = SocketEvetns.ON_CALL_TIMER_FOUND;
        response.data = responseText;
        socketResponse.Add(response);
    }



    void OnStartGameTimerFound(Socket socket, Packet packet, params object[] args)
    {
        string responseText = JsonMapper.ToJson(args);

#if DEBUG

#if UNITY_EDITOR
        if (GlobalGameManager.instance.CanDebugThis(SocketEvetns.ON_GAME_START_TIMER_FOUND))
        {
            //Debug.Log("OnStartGameTimerFound = " + responseText + "  Time = " + System.DateTime.Now);
        }
#else
        Debug.Log("OnStartGameTimerFound = " + responseText + "  Time = " + System.DateTime.Now);
#endif
#endif

        SocketResponse response = new SocketResponse();
        response.eventType = SocketEvetns.ON_GAME_START_TIMER_FOUND;
        response.data = responseText;
        socketResponse.Add(response);
    }


    void OnRoundNoFound(Socket socket, Packet packet, params object[] args)
    {
        string responseText = JsonMapper.ToJson(args);

#if DEBUG

#if UNITY_EDITOR
        if (GlobalGameManager.instance.CanDebugThis(SocketEvetns.ON_ROUND_NO_FOUND))
        {
            //Debug.Log("OnRoundNoFound = " + responseText + "  Time = " + System.DateTime.Now);
        }
#else
        Debug.Log("OnRoundNoFound = " + responseText + "  Time = " + System.DateTime.Now);
#endif
#endif

        SocketResponse response = new SocketResponse();
        response.eventType = SocketEvetns.ON_ROUND_NO_FOUND;
        response.data = responseText;
        socketResponse.Add(response);
    }

    void OnPotDataFound(Socket socket, Packet packet, params object[] args)
    {
        string responseText = JsonMapper.ToJson(args);

#if DEBUG

#if UNITY_EDITOR
        if (GlobalGameManager.instance.CanDebugThis(SocketEvetns.ON_POT_DATA_FOUND))
        {
            Debug.Log("OnPotDataFound = " + responseText + "  Time = " + System.DateTime.Now);
        }
#else
        Debug.Log("OnPotDataFound = " + responseText + "  Time = " + System.DateTime.Now);
#endif
#endif

        SocketResponse response = new SocketResponse();
        response.eventType = SocketEvetns.ON_POT_DATA_FOUND;
        response.data = responseText;
        socketResponse.Add(response);
    }

    void OnPointUpdate(Socket socket, Packet packet, params object[] args)
    {
        string responseText = JsonMapper.ToJson(args);
        ClubInGameManager.instance.Pot.SetActive(false);
        ClubInGameManager.instance.DeactivateAllPots();
#if DEBUG

#if UNITY_EDITOR
        if (GlobalGameManager.instance.CanDebugThis(SocketEvetns.ON_POINT_UPDATE))
        {
            Debug.Log("ON_POINT_UPDATE = " + responseText + "  Time = " + System.DateTime.Now);
        }
#else
        Debug.Log("OnResultDataFound = " + responseText + "  Time = " + System.DateTime.Now);
#endif
#endif

        SocketResponse response = new SocketResponse();
        response.eventType = SocketEvetns.ON_POINT_UPDATE;
        response.data = responseText;
        socketResponse.Add(response);
    }

    void OnAllTipData(Socket socket, Packet packet, params object[] args)
    {
        string responseText = JsonMapper.ToJson(args);
        ClubInGameManager.instance.Pot.SetActive(false);
        ClubInGameManager.instance.DeactivateAllPots();
#if DEBUG

#if UNITY_EDITOR
        if (GlobalGameManager.instance.CanDebugThis(SocketEvetns.ON_ALL_TIP_DATA))
        {
            Debug.Log("OnAllTipData = " + responseText + "  Time = " + System.DateTime.Now);
        }
#else
        Debug.Log("OnResultDataFound = " + responseText + "  Time = " + System.DateTime.Now);
#endif
#endif

        SocketResponse response = new SocketResponse();
        response.eventType = SocketEvetns.ON_ALL_TIP_DATA;
        response.data = responseText;
        socketResponse.Add(response);
    }

    void OnResultDataFound(Socket socket, Packet packet, params object[] args)
    {
        string responseText = JsonMapper.ToJson(args);
        ClubInGameManager.instance.Pot.SetActive(false);
        ClubInGameManager.instance.DeactivateAllPots();
#if DEBUG

#if UNITY_EDITOR
        if (GlobalGameManager.instance.CanDebugThis(SocketEvetns.ON_RESULT_FOUND))
        {
            Debug.Log("OnResultDataFound = " + responseText + "  Time = " + System.DateTime.Now);
        }
#else
        Debug.Log("OnResultDataFound = " + responseText + "  Time = " + System.DateTime.Now);
#endif
#endif

        SocketResponse response = new SocketResponse();
        response.eventType = SocketEvetns.ON_RESULT_FOUND;
        response.data = responseText;
        socketResponse.Add(response);
    }


    void OnResultTimerFound(Socket socket, Packet packet, params object[] args)
    {
        string responseText = JsonMapper.ToJson(args);

#if DEBUG

#if UNITY_EDITOR
        if (GlobalGameManager.instance.CanDebugThis(SocketEvetns.ON_RESULT_TIMER_FOUND))
        {
            Debug.Log("OnResultTimerFound = " + responseText + "  Time = " + System.DateTime.Now);
        }
#else
        Debug.Log("OnResultTimerFound = " + responseText + "  Time = " + System.DateTime.Now);
#endif
#endif

        SocketResponse response = new SocketResponse();
        response.eventType = SocketEvetns.ON_RESULT_TIMER_FOUND;
        response.data = responseText;
        socketResponse.Add(response);
    }


    void OnBetDataFound(Socket socket, Packet packet, params object[] args)
    {
        string responseText = JsonMapper.ToJson(args);

#if DEBUG

#if UNITY_EDITOR
        if (GlobalGameManager.instance.CanDebugThis(SocketEvetns.ON_BET_DATA_FOUND))
        {
            //Debug.Log("OnBetDataFound = " + responseText + "  Time = " + System.DateTime.Now);
        }
#else
        Debug.Log("OnBetDataFound = " + responseText + "  Time = " + System.DateTime.Now);
#endif
#endif

        SocketResponse response = new SocketResponse();
        response.eventType = SocketEvetns.ON_BET_DATA_FOUND;
        response.data = responseText;
        socketResponse.Add(response);
    }


    void OnCardDistributeTimerFound(Socket socket, Packet packet, params object[] args)
    {
        string responseText = JsonMapper.ToJson(args);

#if DEBUG

#if UNITY_EDITOR
        if (GlobalGameManager.instance.CanDebugThis(SocketEvetns.ON_CARD_DISTRIBUTE_TIMER_FOUND))
        {
            Debug.Log("OnCardDistributeTimerFound = " + responseText + "  Time = " + System.DateTime.Now);
        }
#else
        Debug.Log("OnCardDistributeTimerFound = " + responseText + "  Time = " + System.DateTime.Now);
#endif
#endif

        SocketResponse response = new SocketResponse();
        response.eventType = SocketEvetns.ON_CARD_DISTRIBUTE_TIMER_FOUND;
        response.data = responseText;
        socketResponse.Add(response);
    }


    void OnMessageFound(Socket socket, Packet packet, params object[] args)
    {
        string responseText = JsonMapper.ToJson(args);

#if DEBUG

#if UNITY_EDITOR
        if (GlobalGameManager.instance.CanDebugThis(SocketEvetns.ON_MESSAGE_FOUND))
        {
            Debug.Log("OnMessageFound = " + responseText + "  Time = " + System.DateTime.Now);
        }
#else
        Debug.Log("OnMessageFound = " + responseText + "  Time = " + System.DateTime.Now);
#endif
#endif

        SocketResponse response = new SocketResponse();
        response.eventType = SocketEvetns.ON_MESSAGE_FOUND;
        response.data = responseText;
        socketResponse.Add(response);
    }


    void OnReconnected(Socket socket, Packet packet, params object[] args)
    {
        string responseText = JsonMapper.ToJson(args);

#if DEBUG

#if UNITY_EDITOR
        if (GlobalGameManager.instance.CanDebugThis(SocketEvetns.ON_RECONNECTED))
        {
            Debug.Log("OnReconnected = " + responseText + "  Time = " + System.DateTime.Now);
        }
#else
        Debug.Log("OnReconnected = " + responseText + "  Time = " + System.DateTime.Now);
#endif
#endif

        SocketResponse response = new SocketResponse();
        response.eventType = SocketEvetns.ON_RECONNECTED;
        response.data = responseText;
        socketResponse.Add(response);
    }

    void MatchHistory(Socket socket, Packet packet, params object[] args)
    {
        string responseText = JsonMapper.ToJson(args);

#if DEBUG

#if UNITY_EDITOR
        if (GlobalGameManager.instance.CanDebugThis(SocketEvetns.ON_MATCH_HISTORY_FOUND))
        {
            Debug.Log("MatchHistory = " + responseText + "  Time = " + System.DateTime.Now);
        }
#else
        Debug.Log("MatchHistory = " + responseText + "  Time = " + System.DateTime.Now);
#endif
#endif

        SocketResponse response = new SocketResponse();
        response.eventType = SocketEvetns.ON_MATCH_HISTORY_FOUND;
        response.data = responseText;
        socketResponse.Add(response);
    }



    void OnServerConnect(Socket socket, Packet packet, params object[] args)
    {

#if DEBUG

#if UNITY_EDITOR
        if (GlobalGameManager.instance.CanDebugThis(SocketEvetns.CONNECT))
        {
            Debug.Log("Enter in OnServerConnect Time = " + System.DateTime.Now);
        }
#else
        Debug.Log("Enter in OnServerConnect Time = " + System.DateTime.Now);
#endif

#endif

        SocketResponse response = new SocketResponse();
        response.eventType = SocketEvetns.CONNECT;
        socketResponse.Add(response);
    }


    void OnServerDisconnect(Socket socket, Packet packet, params object[] args)
    {
        //string responseText = JsonMapper.ToJson(args);

#if DEBUG

#if UNITY_EDITOR
        if (GlobalGameManager.instance.CanDebugThis(SocketEvetns.DISCONNECT))
        {
            //Debug.Log("Enter in OnServerDisconnect Time = " + System.DateTime.Now);
        }
#else
        Debug.Log("Enter in OnServerDisconnect Time = " + System.DateTime.Now);
#endif
#endif
        SocketResponse response = new SocketResponse();
        response.eventType = SocketEvetns.DISCONNECT;
        socketResponse.Add(response);
    }



    void OnError(Socket socket, Packet packet, params object[] args)
    {
        if (string.IsNullOrEmpty(TABLE_ID)) //first time connecting
        {
            ReConnect();
        }

#if DEBUG

        Error error = args[0] as Error;
        switch (error.Code)
        {
            case SocketIOErrors.User:
                Debug.LogError("Exception in an event handler! Time = " + System.DateTime.Now);
                break;
            case SocketIOErrors.Internal:
                Debug.LogError("Internal error! Time = " + System.DateTime.Now);
                break;
            default:
                Debug.LogError("server error! Time = " + System.DateTime.Now);
                break;
        }
#endif

    }


    void OnReconnect(Socket socket, Packet packet, params object[] args)
    {
#if DEBUG
        Debug.Log("Reconnected Time = " + System.DateTime.Now);
#endif
    }


    void OnReconnecting(Socket socket, Packet packet, params object[] args)
    {

#if DEBUG
        Debug.Log("Reconnecting Time = " + System.DateTime.Now);
#endif
    }

    void OnReconnectAttempt(Socket socket, Packet packet, params object[] args)
    {
#if DEBUG

#if UNITY_EDITOR
        if (GlobalGameManager.instance.CanDebugThis(SocketEvetns.RECONNECT_ATTEMPT))
        {
            Debug.Log("Enter in OnReconnectAttempt Time = " + System.DateTime.Now);
        }
#else
        Debug.Log("Enter in OnReconnectAttempt Time = " + System.DateTime.Now);
#endif

#endif

        SocketResponse response = new SocketResponse();
        response.eventType = SocketEvetns.RECONNECT_ATTEMPT;
        socketResponse.Add(response);
    }

    void OnReconnectFailed(Socket socket, Packet packet, params object[] args)
    {
#if DEBUG
        Debug.Log("ReconnectFailed Time = " + System.DateTime.Now);
#endif
    }


    void OnSentEmoji(Socket socket, Packet packet, params object[] args)
    {
#if DEBUG
        Debug.Log("OnOtherSeeEmoji -CALL    = " + System.DateTime.Now);
#endif

        string responseText = JsonMapper.ToJson(args);

#if DEBUG

#if UNITY_EDITOR
        if (GlobalGameManager.instance.CanDebugThis(SocketEvetns.ON_SendEmoji))
        {
            Debug.Log("OnOtherSeeEmoji -CALL  = " + responseText + "  Time = " + System.DateTime.Now);
        }
#else
        Debug.Log("OnReconnected = " + responseText + "  Time = " + System.DateTime.Now);
#endif
#endif
        SocketResponse response = new SocketResponse();
        response.eventType = SocketEvetns.ON_SendEmoji;
        response.data = responseText;
        socketResponse.Add(response);
    }
    void OnPlayerStandUp(Socket socket, Packet packet, params object[] args)
    {
        string responseText = JsonMapper.ToJson(args);

#if DEBUG

#if UNITY_EDITOR
        if (GlobalGameManager.instance.CanDebugThis(SocketEvetns.ON_PlayerStandUp))
        {
            //Debug.Log("OnStartGameTimerFound = " + responseText + "  Time = " + System.DateTime.Now);
        }
#else
        Debug.Log("OnStartGameTimerFound = " + responseText + "  Time = " + System.DateTime.Now);
#endif
#endif

        SocketResponse response = new SocketResponse();
        response.eventType = SocketEvetns.ON_PlayerStandUp;
        response.data = responseText;
        socketResponse.Add(response);
    }


    void OnSendWinningBooster(Socket socket, Packet packet, params object[] args)
    {
        string responseText = JsonMapper.ToJson(args);

#if DEBUG

#if UNITY_EDITOR
        if (GlobalGameManager.instance.CanDebugThis(SocketEvetns.ON_SEND_WINNING_BOOSTER))
        {
            Debug.Log("OnWinningBoosterFound = " + responseText + "  Time = " + System.DateTime.Now);
        }
#else
        Debug.Log("OnWinningBoosterFound = " + responseText + "  Time = " + System.DateTime.Now);
#endif
#endif

        SocketResponse response = new SocketResponse();
        response.eventType = SocketEvetns.ON_SEND_WINNING_BOOSTER;
        response.data = responseText;
        socketResponse.Add(response);
    }

    void OnGetRandomCard(Socket socket, Packet packet, params object[] args)
    {
        string responseText = JsonMapper.ToJson(args);

#if DEBUG

#if UNITY_EDITOR
        if (GlobalGameManager.instance.CanDebugThis(SocketEvetns.ON_GET_RANDOM_CARD))
        {
            Debug.Log("OnRandomCardFound = " + responseText + "  Time = " + System.DateTime.Now);
        }
#else
        Debug.Log("OnRandomCardFound = " + responseText + "  Time = " + System.DateTime.Now);
#endif
#endif

        SocketResponse response = new SocketResponse();
        response.eventType = SocketEvetns.ON_GET_RANDOM_CARD;
        response.data = responseText;
        socketResponse.Add(response);
    }

    #endregion





    // EMIT_METHODS ---------------------------------------------------------------------------------------------------------------------------------------------------------


    #region EMIT_METHODS
    //DEV_CODE
    public void RequestAskMultiRunAction(int value)
    {
        ClubInGameManager.instance.MultiRunPanel.SetActive(false);
        AskMultiRunAction requestData = new AskMultiRunAction();
        requestData.userId = "" + PlayerManager.instance.GetPlayerGameData().userId;
        requestData.tableId = TABLE_ID;
        if (value == 0)
            requestData.action = false;
        else
            requestData.action = true;
        requestData.runIt = value;

        string requestStringData = JsonMapper.ToJson(requestData);
        object requestObjectData = Json.Decode(requestStringData);

        SocketRequest request = new SocketRequest();
        request.emitEvent = "askMultiRunAction"; //rabbitCardData //rabbitOpenCards
        request.plainDataToBeSend = null;
        request.jsonDataToBeSend = requestObjectData;
        request.requestDataStructure = requestStringData;
        socketRequest.Add(request);
    }

    public void RequestConfirmMultiRunAction(bool action)
    {
        ClubInGameManager.instance.MultiRunActionPanel.SetActive(false);
        ConfirmMultiRunAction requestData = new ConfirmMultiRunAction();
        requestData.userId = "" + PlayerManager.instance.GetPlayerGameData().userId;
        requestData.tableId = TABLE_ID;
        requestData.action = action;

        string requestStringData = JsonMapper.ToJson(requestData);
        object requestObjectData = Json.Decode(requestStringData);

        SocketRequest request = new SocketRequest();
        request.emitEvent = "confirmMultiRunAction"; //rabbitCardData //rabbitOpenCards
        request.plainDataToBeSend = null;
        request.jsonDataToBeSend = requestObjectData;
        request.requestDataStructure = requestStringData;
        socketRequest.Add(request);
    }

    //public void RequestEVCHOP()
    //{
    //    EVChopData requestData = new EVChopData();
    //    requestData.tableId = TABLE_ID;
    //    requestData.userId = "" + PlayerManager.instance.GetPlayerGameData().userId;

    //    string requestStringData = JsonMapper.ToJson(requestData);
    //    object requestObjectData = Json.Decode(requestStringData);

    //    SocketRequest request = new SocketRequest();
    //    request.emitEvent = "calculateEvChop"; //rabbitCardData //rabbitOpenCards
    //    request.plainDataToBeSend = null;
    //    request.jsonDataToBeSend = requestObjectData;
    //    request.requestDataStructure = requestStringData;
    //    socketRequest.Add(request);
    //}

    public void RequestRabbitCard()
    {
        RabitData requestData = new RabitData();
        requestData.tableId = TABLE_ID;
        requestData.userId = "" + PlayerManager.instance.GetPlayerGameData().userId;

        string requestStringData = JsonMapper.ToJson(requestData);
        object requestObjectData = Json.Decode(requestStringData);

        SocketRequest request = new SocketRequest();
        request.emitEvent = "rabbitCardData"; //rabbitCardData //rabbitOpenCards
        request.plainDataToBeSend = null;
        request.jsonDataToBeSend = requestObjectData;
        request.requestDataStructure = requestStringData;
        socketRequest.Add(request);
    }

    public void SendStandUpdata()
    {
        StandUpdata requestData = new StandUpdata();
        requestData.tableId = TABLE_ID;
        requestData.userId = "" + PlayerManager.instance.GetPlayerGameData().userId;
        requestData.isStatndOut = 1;

        //Debug.LogError(requestData.userId + "  <Stand up User ID table id >" + TABLE_ID);

        string requestStringData = JsonMapper.ToJson(requestData);
        object requestObjectData = Json.Decode(requestStringData);

        SocketRequest request = new SocketRequest();
        request.emitEvent = "playerStatndOut";
        request.plainDataToBeSend = null;
        request.jsonDataToBeSend = requestObjectData;
        request.requestDataStructure = requestStringData;
        socketRequest.Add(request);

    }

    public void SendTopUpRequest(int coinsToAdd)
    {
        string roomId = GlobalGameManager.instance.GetRoomData().roomId;

        string requestStringData = "{\"userId\":\"" + PlayerManager.instance.GetPlayerGameData().userId + "\"," +
     "\"tableId\":\"" + TABLE_ID + "\"," +
     "\"roomId\":\"" + roomId + "\"," +
     "\"playerType\":\"" + "Real" + "\"," +
     "\"coins\":\"" + coinsToAdd + "\"}";


        object requestObjectData = Json.Decode(requestStringData);

        SocketRequest request = new SocketRequest();
        request.emitEvent = "topUp";

        request.plainDataToBeSend = null;
        request.jsonDataToBeSend = requestObjectData;
        request.requestDataStructure = requestStringData;
        socketRequest.Add(request);
    }

    public void SendClubTopUpRequest(int coinsToAdd)
    {
        string roomId = GlobalGameManager.instance.GetRoomData().roomId;

        string requestStringData = "{\"userId\":\"" + PlayerManager.instance.GetPlayerGameData().userId + "\"," +
     "\"tableId\":\"" + TABLE_ID + "\"," +
     "\"clubId\":\"" + ClubDetailsUIManager.instance.GetClubId() + "\"," +
     "\"roomId\":\"" + roomId + "\"," +
     "\"playerType\":\"" + "Real" + "\"," +
     "\"ptChips\":\"" + coinsToAdd + "\"}";


        object requestObjectData = Json.Decode(requestStringData);

        SocketRequest request = new SocketRequest();
        request.emitEvent = "clubTopUp";

        request.plainDataToBeSend = null;
        request.jsonDataToBeSend = requestObjectData;
        request.requestDataStructure = requestStringData;
        socketRequest.Add(request);
    }

    public void TipToDealer()
    {
        string requestStringData = "{\"userId\":\"" + PlayerManager.instance.GetPlayerGameData().userId + "\"," +
                                    "\"tableId\":\"" + TABLE_ID + "\"}";

        Debug.Log("i am herejhdsf   " + requestStringData);
        object requestObjectData = Json.Decode(requestStringData);

        SocketRequest request = new SocketRequest();
        request.emitEvent = "tipToDealer";

        request.plainDataToBeSend = null;
        request.jsonDataToBeSend = requestObjectData;
        request.requestDataStructure = requestStringData;
        socketRequest.Add(request);
    }

    public void SentEmoji(int otherUserId, int emojiIndex)
    {

        string requestStringData = "{\"sentBy\":\"" + ((int.Parse(PlayerManager.instance.GetPlayerGameData().userId)).ToString() + "\"," +
            "\"sentTo\":\"" + otherUserId + "\"," +
            "\"deductionValue\":\"" + 2 + "\"," +
            "\"emojiIndex\":\"" + emojiIndex + "\"," +
            "\"tableId\":\"" + int.Parse(TABLE_ID)).ToString() + "\"}";

        //Debug.LogError("i am SentEmoji   " + requestStringData);
        object requestObjectData = Json.Decode(requestStringData);


        SocketRequest request = new SocketRequest();
        request.emitEvent = "sendEmoji";
        request.plainDataToBeSend = null;
        request.jsonDataToBeSend = requestObjectData;
        request.requestDataStructure = requestStringData;
        socketRequest.Add(request);
    }

    public void SendGameJoinRequest()
    {
        string requestStringData = "{\"userId\":\"" + PlayerManager.instance.GetPlayerGameData().userId + "\"," +
             "\"players\":\"" + GlobalGameManager.instance.GetRoomData().players + "\"," +
             "\"roomId\":\"" + GlobalGameManager.instance.GetRoomData().roomId + "\"," +
             "\"playerType\":\"Real\"," +
             "\"isPrivate\":\"No\"," +
             "\"isFree\":\"No\"}";

        object requestObjectData = Json.Decode(requestStringData);

        SocketRequest request = new SocketRequest();
        request.emitEvent = "joinRoom";

        Debug.LogError("joinRoom: " + requestStringData);

        request.plainDataToBeSend = null;
        request.jsonDataToBeSend = requestObjectData;
        request.requestDataStructure = requestStringData;
        socketRequest.Add(request);
    }

    public void SendGameJoinRequestWithSeat(string seatNo)
    {
        string requestStringData = "{\"userId\":\"" + PlayerManager.instance.GetPlayerGameData().userId + "\"," +
             "\"players\":\"" + GlobalGameManager.instance.GetRoomData().players + "\"," +
             "\"roomId\":\"" + GlobalGameManager.instance.GetRoomData().roomId + "\"," +
             "\"seatNo\":\"" + seatNo + "\"," +
             "\"playerType\":\"Real\"," +
             "\"isPrivate\":\"No\"," +
             "\"isFree\":\"No\"}";

        object requestObjectData = Json.Decode(requestStringData);

        SocketRequest request = new SocketRequest();
        request.emitEvent = "joinRoom";

        Debug.LogError("joinRoom: " + requestStringData);

        request.plainDataToBeSend = null;
        request.jsonDataToBeSend = requestObjectData;
        request.requestDataStructure = requestStringData;
        socketRequest.Add(request);
    }

    //clubId
    //
    public void SendClubGameJoinRequest()
    {
        string requestStringData = "{\"userId\":\"" + PlayerManager.instance.GetPlayerGameData().userId + "\"," +
            "\"players\":\"" + GlobalGameManager.instance.GetRoomData().players + "\"," +
            "\"roomId\":\"" + GlobalGameManager.instance.GetRoomData().roomId + "\"," +
            "\"clubId\":\"" + ClubDetailsUIManager.instance.GetClubId() + "\"," +
            "\"seatNo\":\"" + "" + "\"," +
            "\"gameType\":\"" + "club" + "\"," +
            "\"playerType\":\"Real\"," +
            "\"isPrivate\":\"No\"," +
            "\"isFree\":\"No\"}";

        //    string requestStringData = "{\"userId\":\"" + PlayerManager.instance.GetPlayerGameData().userId + "\"," +
        //"\"players\":\"" + GlobalGameManager.instance.GetRoomData().players + "\"," +
        //"\"roomId\":\"" + "9" + "\"," +
        //"\"seatNo\":\"" + "" + "\"," +
        //"\"gameType\":\"" + "club" + "\"," +
        //"\"playerType\":\"Real\"," +
        //"\"isPrivate\":\"No\"," +
        //"\"isFree\":\"No\"}";

        object requestObjectData = Json.Decode(requestStringData);

        SocketRequest request = new SocketRequest();
        request.emitEvent = "joinRoom";

        Debug.LogError("joinRoom: " + requestStringData);

        request.plainDataToBeSend = null;
        request.jsonDataToBeSend = requestObjectData;
        request.requestDataStructure = requestStringData;
        socketRequest.Add(request);
    }


    public void SendFoldRequest(int totalBetInRound)
    {
        FoldData requestData = new FoldData();
        requestData.userData = new UserBetData();

        requestData.userData.betData = totalBetInRound;
        requestData.userData.playerAction = ClubInGameManager.instance.GetLastPlayerAction();
        requestData.userData.roundNo = ClubInGameManager.instance.GetMatchRound();

        requestData.userId = "" + PlayerManager.instance.GetPlayerGameData().userId;
        requestData.tableId = TABLE_ID;

        string requestStringData = JsonMapper.ToJson(requestData);
        object requestObjectData = Json.Decode(requestStringData);
        Debug.Log("<color=magenta>Fold Request </color>" + requestStringData);
        SocketRequest request = new SocketRequest();
        request.emitEvent = "foldCards";
        request.plainDataToBeSend = null;
        request.jsonDataToBeSend = requestObjectData;
        request.requestDataStructure = requestStringData;
        socketRequest.Add(request);
    }

    public void MinimizeAppEvent()
    {
        MinEvent requestData = new MinEvent();
        requestData.appStatus = "minimize";//maximize

        string requestStringData = JsonMapper.ToJson(requestData);
        object requestObjectData = Json.Decode(requestStringData);

        SocketRequest req = new SocketRequest();
        req.emitEvent = "minMaxApp";

        req.plainDataToBeSend = null;
        req.jsonDataToBeSend = requestObjectData;
        req.requestDataStructure = requestStringData;
        socketRequest.Add(req);
    }

    public void MaximizeAppEvent()
    {
        MinEvent requestData = new MinEvent();
        requestData.appStatus = "maximize";//maximize

        string requestStringData = JsonMapper.ToJson(requestData);
        object requestObjectData = Json.Decode(requestStringData);

        SocketRequest req = new SocketRequest();
        req.emitEvent = "minMaxApp";

        req.plainDataToBeSend = null;
        req.jsonDataToBeSend = requestObjectData;
        req.requestDataStructure = requestStringData;
        //socketRequest.Add(req);

        //DEV_CODE  To send emit event immediately
        socketManager.Socket.Emit(req.emitEvent, req.jsonDataToBeSend);
        Debug.LogError("Maximize :" + requestStringData);
    }


    public void SendBetData(int betAmount, int totalBetInRound, string userAction, int roundNo, string localTableId = null)
    {
        Debug.Log("Local Table Id " + localTableId);
        BetData requestData = new BetData();
        requestData.userData = new UserBetData();
        requestData.userAction = userAction;
        requestData.userData.betData = totalBetInRound;

        requestData.userData.playerAction = userAction;
        requestData.userData.roundNo = roundNo;

        requestData.userId = "" + PlayerManager.instance.GetPlayerGameData().userId;
        requestData.tableId = localTableId;// TABLE_ID;
        requestData.bet = "" + betAmount;

        string requestStringData = JsonMapper.ToJson(requestData);
        object requestObjectData = Json.Decode(requestStringData);

        SocketRequest request = new SocketRequest();
        request.emitEvent = "submitBet";
        request.plainDataToBeSend = null;
        request.jsonDataToBeSend = requestObjectData;
        request.requestDataStructure = requestStringData;
        socketRequest.Add(request);
    }


    public void RequestForMatchHistory()
    {
        FoldData requestData = new FoldData();

        requestData.userId = "" + PlayerManager.instance.GetPlayerGameData().userId;
        requestData.tableId = TABLE_ID;

        string requestStringData = JsonMapper.ToJson(requestData);
        object requestObjectData = Json.Decode(requestStringData);

        SocketRequest request = new SocketRequest();
        request.emitEvent = "matchPlayLogs";
        request.plainDataToBeSend = null;
        request.jsonDataToBeSend = requestObjectData;
        request.requestDataStructure = requestStringData;
        socketRequest.Add(request);
    }


    public void SendReMatchRequest(string isJoin, string coinsToAssign = "0")
    {
        RematchData requestData = new RematchData();

        requestData.userId = "" + PlayerManager.instance.GetPlayerGameData().userId;
        requestData.tableId = TABLE_ID;
        requestData.isBuyIn = isJoin;
        requestData.coins = coinsToAssign;


        string requestStringData = JsonMapper.ToJson(requestData);
        object requestObjectData = Json.Decode(requestStringData);

        SocketRequest request = new SocketRequest();
        request.emitEvent = "isBuyIn";
        request.plainDataToBeSend = null;
        request.jsonDataToBeSend = requestObjectData;
        request.requestDataStructure = requestStringData;
        socketRequest.Add(request);
    }



    public bool SendLeaveMatchRequest()
    {
        if (string.IsNullOrEmpty(TABLE_ID))
        {
            return false;
        }

        FoldData requestData = new FoldData();

        requestData.userId = "" + PlayerManager.instance.GetPlayerGameData().userId;
        requestData.tableId = TABLE_ID;

        string requestStringData = JsonMapper.ToJson(requestData);
        Debug.LogError("[SOCKET EVENT] - leaveMatch" + "[Params]  " + requestStringData);
        object requestObjectData = Json.Decode(requestStringData);

        SocketRequest request = new SocketRequest();
        request.emitEvent = "leaveMatch";
        request.plainDataToBeSend = null;
        request.jsonDataToBeSend = requestObjectData;
        request.requestDataStructure = requestStringData;
        //socketRequest.Add(request);

        //DEV_CODE Sending event directly, no need to add socketRequest to list
        socketManager.Socket.Emit(request.emitEvent, request.jsonDataToBeSend);

        return true;
    }

    public bool SendLeaveTableRequest(string tableId)
    {
        FoldData requestData = new FoldData();

        requestData.userId = "" + PlayerManager.instance.GetPlayerGameData().userId;
        requestData.tableId = tableId;

        string requestStringData = JsonMapper.ToJson(requestData);
        Debug.LogError("[SOCKET EVENT] - leaveTable" + "[Params]  " + requestStringData);
        object requestObjectData = Json.Decode(requestStringData);

        SocketRequest request = new SocketRequest();
        request.emitEvent = "leaveTable";
        request.plainDataToBeSend = null;
        request.jsonDataToBeSend = requestObjectData;
        request.requestDataStructure = requestStringData;
        //socketRequest.Add(request);

        //DEV_CODE Sending event directly, no need to add socketRequest to list
        socketManager.Socket.Emit(request.emitEvent, request.jsonDataToBeSend);

        return true;
    }


    public void SendChatMessage(string title, string desc)
    {
        SendMessageData messageData = new SendMessageData();
        messageData.from = "" + PlayerManager.instance.GetPlayerGameData().userId;
        messageData.to = messageData.from;
        messageData.title = title;
        messageData.desc = desc;
        messageData.tableId = TABLE_ID;
        messageData.userId = messageData.from;

        string requestData = JsonMapper.ToJson(messageData);

        object requestObject = Json.Decode(requestData);
        SocketRequest request = new SocketRequest();

        request.emitEvent = "sendMessage";

        request.plainDataToBeSend = null;
        request.jsonDataToBeSend = requestObject;
        request.requestDataStructure = requestData;
        socketRequest.Add(request);
    }

    //DEV_CODE
    public void SendWinningBooster(int tableId, int rewardAmount, string cardValue)
    {
        string requestStringData = "{\"tableId\":" + tableId + "," +
                     "\"userId\":" + int.Parse(PlayerManager.instance.GetPlayerGameData().userId) + "," +
                     "\"card\":\"" + cardValue + "\"," +
                     "\"reward\":" + rewardAmount + "}";

        object requestObjectData = Json.Decode(requestStringData);

        SocketRequest request = new SocketRequest();
        request.emitEvent = "predictionReward";

        request.plainDataToBeSend = null;
        request.jsonDataToBeSend = requestObjectData;
        request.requestDataStructure = requestStringData;
        socketRequest.Add(request);
    }

    public void GetRandomCard()
    {
        string requestStringData = "";

        object requestObjectData = Json.Decode(requestStringData);

        SocketRequest request = new SocketRequest();
        request.emitEvent = "getRandomCard";

        request.plainDataToBeSend = "";
        //request.jsonDataToBeSend = requestObjectData;
        //request.requestDataStructure = requestStringData;
        socketRequest.Add(request);
    }

    public void ConfrimEvChop(string action, string index)
    {
        EvChopData requestData = new EvChopData();

        requestData.userId = "" + PlayerManager.instance.GetPlayerGameData().userId;
        requestData.tableId = TABLE_ID;
        requestData.action = action;
        requestData.index = index;
        string requestStringData = JsonMapper.ToJson(requestData);
        object requestObjectData = Json.Decode(requestStringData);
        Debug.LogError("evChopAction " + requestStringData);
        SocketRequest request = new SocketRequest();
        request.emitEvent = "evChopAction";
        request.plainDataToBeSend = null;
        request.jsonDataToBeSend = requestObjectData;
        request.requestDataStructure = requestStringData;
        socketRequest.Add(request);
        //SendSocketRequest();
    }

    #endregion


    // OTHER_METHODS ---------------------------------------------------------------------------------------------------------------------------------------------------------

    public SocketState GetSocketState()
    {
        return socketState;
    }

    public void SetSocketState(SocketState state)
    {
        //Debug.Log("Set Socket State " + state);
        socketState = state;
    }

    public void SetTableId(string tableIdToAssign)
    {
        GlobalGameManager.instance.GetRoomData().socketTableId = tableIdToAssign;
        TABLE_ID = tableIdToAssign;

        PrefsManager.SetData(PrefsKey.RoomData, JsonUtility.ToJson(GlobalGameManager.instance.GetRoomData()));
        //Debug.Log("Table ID Is :" + TABLE_ID);
    }

    public string GetTableID()
    {
        return TABLE_ID;
    }

    public void ResetConnection(bool isReconnecting = false)
    {
        if (socketManager != null && socketManager.Socket.IsOpen)
        {
            socketManager.Close();
        }

        socketRequest.Clear();
        socketResponse.Clear();

        if (!isReconnecting)
        {
            if (IsInvoking("HandleSocketResponse"))
            {
                CancelInvoke("HandleSocketResponse");
            }

            if (IsInvoking("SendSocketRequest"))
            {
                CancelInvoke("SendSocketRequest");
            }

            SetSocketState(SocketState.NULL);
        }
    }
    

    #region ReconnectProtocols

    private void StartReconnectProcedure()
    {
        if (!isPreocedureRunning)
        {
            StartCoroutine(WaitForReconnect());
        }
    }

    private bool isPreocedureRunning = false;
    private IEnumerator WaitForReconnect()
    {
        isPreocedureRunning = true;
        ClubInGameUIManager.instance.ShowScreen(InGameScreens.Reconnecting);
        SetSocketState(SocketState.ReConnecting);
        socketManager = null;
        ReConnect();

        int counter = 0, maxCount = 30, reInitialisationCount = 3;

        while (counter < maxCount)
        {
            yield return new WaitForSeconds(1f);

            if (socketManager.Socket.IsOpen)
            {
                counter = maxCount;
            }
            else
            {
                if (counter % reInitialisationCount == 0)
                {
                    ReConnect();
                }
            }

            ++counter;
        }

        if (!socketManager.Socket.IsOpen)
        {
            ClubInGameUIManager.instance.DestroyScreen(InGameScreens.Reconnecting);
            ClubInGameUIManager.instance.ShowMessage("ReConnect Attempt failed, please check your network connection", () =>
            {
                StartCoroutine(WaitForReconnect());
            },

             () =>
             {
                 ClubInGameManager.instance.LoadMainMenu();
             }, "Retry", "Cancel"
            );
        }

        isPreocedureRunning = false;
    }

    private void RequestForMatchStatus()
    {
        //ClubInGameManagerScript.instance.isUpdateTableCards = true;
        //ClubInGameManagerScript.instance.MATCH_ROUND = -1;


        ReconnectData data = new ReconnectData();
        data.userId = "" + PlayerManager.instance.GetPlayerGameData().userId;
        data.tableId = TABLE_ID;
        data.isYesOrNo = "Yes";

        string requestdeta = JsonMapper.ToJson(data);
        object requestfordeta = Json.Decode(requestdeta);

        SocketRequest request = new SocketRequest();
        request.emitEvent = "userReconnect";
        request.plainDataToBeSend = null;
        request.jsonDataToBeSend = requestfordeta;
        request.requestDataStructure = requestdeta;
        socketRequest.Add(request);
    }


    private IEnumerator WaitAndCheckInternetConnection()
    {
        while (true)
        {
            yield return new WaitForSeconds(GameConstants.NETWORK_CHECK_DELAY);

            if (GetSocketState() == SocketState.Game_Running)
            {
                if (!WebServices.instance.IsInternetAvailable())
                {
                    if (socketManager != null)
                    {
                        socketManager.Close();
                    }
                }
            }
        }
    }

    #endregion

    #region Creating multiple table

    public void CreateNewTable()
    {
        GlobalGameManager.instance.creatingNewTable = true;
        GameObject gm = Instantiate(clubDetail, clubDetailLayer.transform) as GameObject;
        gm.GetComponent<ClubDetailsUIManager>().Initialize(GlobalGameManager.instance.currentClubName, GlobalGameManager.instance.currentUniqueClubId, GlobalGameManager.instance.currentClubId, GlobalGameManager.instance.currentClubProfileImagePath, GlobalGameManager.instance.currentPlayerType, GlobalGameManager.instance.currentPlayerRole);
        //buttonCanvas.SetActive(false);
        Debug.Log("Table " + GlobalGameManager.instance.table.Count);
        if (GlobalGameManager.instance.table.Count == 2)
            GlobalGameManager.instance.table[1].transform.GetChild(0).GetComponent<Canvas>().sortingOrder = -1;
        //GlobalGameManager.instance.LoadScene(Scenes.MainMenu);
    }

    public void DestroyClubDetailMenu()
    {
        //Debug.Log("DestroyClubDetailMenu " + clubDetailLayer.transform.GetChild(0).gameObject);
        //Destroy(clubDetailLayer.transform.GetChild(0).gameObject);
        Destroy(GameObject.Find("ClubDetails_New(Clone)"));
        //Destroy(GameObject.Find("MainMenuScene(Clone)"));
    }

    public void MoveTable(int tableNum)
    {
        Debug.Log(tableNum + " Total Tables " + GlobalGameManager.instance.table.Count);
        if (GlobalGameManager.instance.table.Count == 1)
            return;
        for (int i = 0; i < 3; i++)
        {
            tableButton[i].transform.GetChild(0).GetComponent<Image>().sprite = tableButtonSprite[1];
        }
        tableButton[tableNum].transform.GetChild(0).GetComponent<Image>().sprite = tableButtonSprite[0];
        
        switch (tableNum)
        {
            case 0:
                GlobalGameManager.instance.table[0].transform.GetChild(0).GetChild(0).DOLocalMoveX(0f, 0.5f);
                GlobalGameManager.instance.table[1].transform.GetChild(0).GetChild(0).DOLocalMoveX(1000f, 0.5f);
                if (GlobalGameManager.instance.table.Count == 3)
                    GlobalGameManager.instance.table[2].transform.GetChild(0).GetChild(0).DOLocalMoveX(2000f, 0.5f);
                break;
            case 1:
                GlobalGameManager.instance.table[0].transform.GetChild(0).GetChild(0).DOLocalMoveX(-1000f, 0.5f);
                GlobalGameManager.instance.table[1].transform.GetChild(0).GetChild(0).DOLocalMoveX(0f, 0.5f);
                if (GlobalGameManager.instance.table.Count == 3)
                    GlobalGameManager.instance.table[2].transform.GetChild(0).GetChild(0).DOLocalMoveX(1000f, 0.5f);
                break;
            case 2:
                GlobalGameManager.instance.table[0].transform.GetChild(0).GetChild(0).DOLocalMoveX(-2000f, 0.5f);
                GlobalGameManager.instance.table[1].transform.GetChild(0).GetChild(0).DOLocalMoveX(-1000f, 0.5f);
                if (GlobalGameManager.instance.table.Count == 3)
                    GlobalGameManager.instance.table[2].transform.GetChild(0).GetChild(0).DOLocalMoveX(0f, 0.5f);
                break;
        }
        GlobalGameManager.currentTableInd = tableNum;
        StartCoroutine(ResetTopTimer(tableNum));
    }

    IEnumerator ResetTopTimer(int ind)
    {
        yield return new WaitForSeconds(1f);
        tableButton[ind].GetComponent<Image>().fillAmount = 0f;
    }

    public void ResetTablesAfterClose(string cTableId)
    {
        /*int index = 0;
        foreach (var kvp in GlobalGameManager.instance.AllTables)
        {
            Debug.Log(kvp.Key + " - " + cTableId);
            if (kvp.Key == cTableId)
            {
                break;  // According to question, you are after the key 
            }
            index++;
        }
        Debug.Log(index + " - " + GlobalGameManager.instance.AllTables.Count);*/
        GlobalGameManager.instance.creatingNewTable = false;
        //GlobalGameManager.instance.currentClubName = null;
        for (int i = 0; i < tableButton.Length - 1; i++)
        {
            tableButton[i].GetComponent<Image>().fillAmount = 0f;
            tableButton[i].SetActive(false);
        }
        
        switch (GlobalGameManager.instance.AllTables.Count)
        {
            case 1:
                tableButton[0].SetActive(true);
                tableButton[0].transform.GetChild(0).GetComponent<Image>().sprite = tableButtonSprite[0];
                GlobalGameManager.instance.table[0].transform.GetChild(0).GetChild(0).localPosition = new Vector2(0f, 0f);
                break;
            case 2:
                tableButton[0].SetActive(true);
                tableButton[1].SetActive(true);
                tableButton[0].transform.GetChild(0).GetComponent<Image>().sprite = tableButtonSprite[0];
                tableButton[1].transform.GetChild(0).GetComponent<Image>().sprite = tableButtonSprite[1];
                GlobalGameManager.instance.table[0].transform.GetChild(0).GetChild(0).localPosition = new Vector2(0f, 0f);
                GlobalGameManager.instance.table[1].transform.GetChild(0).GetChild(0).localPosition = new Vector2(1000f, 0f);
                break;
        }
    }

    public void RemoveAllTables()
    {
        //buttonCanvas.SetActive(false);
        for (int i = 0; i < GlobalGameManager.instance.table.Count; i++)
        {
            Destroy(GlobalGameManager.instance.table[i]);
        }
        GlobalGameManager.instance.table.Clear();
        GlobalGameManager.instance.AllTables.Clear();
        //GlobalGameManager.instance.creatingNewTable = false;
    }

    #endregion
}


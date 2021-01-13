using System.Collections.Generic;
using UnityEngine;
using System;
using BestHTTP.SocketIO;
using System.Collections;
using BestHTTP.JSON;
using LitJson;


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


    void Awake()
    {
        instance = this;
        SetSocketState(SocketState.NULL);
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

        socketManager = new SocketManager(new Uri(GameConstants.SOCKET_URL + "/socket.io/"), socketOptions);
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
        socketManager.Socket.On("evChopData", EVChopDataReceived);  //DEV_CODE Added Event for EVChopDataReceived called
        socketManager.Socket.On("playerExit", PlayerExit);          //DEV_CODE Added Event for PlayerExit called
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

    //DEV_CODE Added this method to be called when EVChopDataReceived emited
    private void EVChopDataReceived(Socket socket, Packet packet, object[] args)
    {
        string responseText = JsonMapper.ToJson(args);
        Debug.LogError("EVChopDataReceived :" + responseText);

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
        Debug.LogError(responseText);
        //InGameManager.instance.gameExitCalled = true;
        //ResetConnection();
    }

    //DEV_CODE Added this method to be called when PlayerExit event emited 
    private void PlayerExit(Socket socket, Packet packet, object[] args)
    {
        string responseText = JsonMapper.ToJson(args);
        Debug.LogError("Response => PlayerExit: " + responseText);

        ClubInGameManager.instance.gameExitCalled = true;
        ResetConnection();
    }

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
            Debug.Log(ClubInGameManager.instance.userWinner + " <color=yellow>Event " + responseObject.eventType + ",</color> " + responseObject.data);
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
                                    SendClubGameJoinRequest();
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
                    if (ClubInGameManager.instance != null)
                    {
                        Debug.LogError("Current Socket State: " + GetSocketState());

                        if (GetSocketState() == SocketState.ReConnecting)
                        {
                            SetSocketState(SocketState.Game_Running);
                        }

                        ClubInGameManager.instance.OnPlayerObjectFound(responseObject.data);
                        ClubInGameUIManager.instance.DestroyScreen(InGameScreens.Reconnecting);
                        ClubInGameUIManager.instance.ShowTableMessage("");
                    }
                    else
                    {
                        Debug.LogError("Null reference exception found ClubInGameManager is null... ");
                    }
                    break;

                case SocketEvetns.ON_OPEN_CARD_DATA_FOUND:
                    ClubInGameManager.instance.OnOpenCardsDataFound(responseObject.data);
                    break;
                case SocketEvetns.RABBIT_CARDS:
                    ClubInGameManager.instance.OnRabbitDataFound(responseObject.data);
                    break;
                case SocketEvetns.EVCHOP:
                    ClubInGameManager.instance.OnEVChopDataFound(responseObject.data);
                    break;

                //case SocketEvetns.ON_OPEN_CARD_TIMER_FOUND:
                //ClubInGameManagerScript.instance.OnOpenCardTimerFound(responseObject.data);
                //break;

                case SocketEvetns.ON_NEXT_ROUND_TIMER_FOUND:
                    ClubInGameManager.instance.OnNextMatchCountDownFound(responseObject.data);
                    break;

                case SocketEvetns.ON_GAME_OVER_TIMER_FOUND:
                    Debug.LogError("Game Over - " + responseObject.data);
                    ClubInGameManager.instance.OnGameOverCountDownFound(responseObject.data);
                    break;

                case SocketEvetns.ON_CALL_TIMER_FOUND:
                    ClubInGameManager.instance.OnTurnCountDownFound(responseObject.data);
                    break;


                case SocketEvetns.ON_GAME_START_TIMER_FOUND:
                    ClubInGameManager.instance.OnGameStartTimeFound(responseObject.data);
                    break;

                case SocketEvetns.ON_ROUND_NO_FOUND:
                    // ClubInGameUIManager.instance.LoadingImage.SetActive(false);
                    ClubInGameManager.instance.OnRoundDataFound(responseObject.data);
                    break;

                //case SocketEvetns.ON_POT_DATA_FOUND:
                //ClubInGameManagerScript.instance.OnPotDataFound(responseObject.data);
                //break;

                case SocketEvetns.ON_RESULT_FOUND:
                    ClubInGameManager.instance.OnResultResponseFound(responseObject.data);
                    break;

                //case SocketEvetns.ON_RESULT_TIMER_FOUND:
                //break;

                case SocketEvetns.ON_BET_DATA_FOUND:
                    ClubInGameManager.instance.PlayerTimerReset();
                    ClubInGameManager.instance.OnBetDataFound(responseObject.data);
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
                    ClubInGameManager.instance.SendEmoji(responseObject.data);
                    break;
                case SocketEvetns.ON_ALL_TIP_DATA:
                    ClubInGameManager.instance.TipToDealer(responseObject.data);
                    break;
                case SocketEvetns.ON_POINT_UPDATE:
                    ClubInGameManager.instance.PointUpdated(responseObject.data);
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
            Debug.Log("OnPlayerObjectFound = " + responseText + "  Time = " + System.DateTime.Now);

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

    public void RequestEVCHOP()
    {
        EVChopData requestData = new EVChopData();
        requestData.tableId = TABLE_ID;
        requestData.userId = "" + PlayerManager.instance.GetPlayerGameData().userId;

        string requestStringData = JsonMapper.ToJson(requestData);
        object requestObjectData = Json.Decode(requestStringData);

        SocketRequest request = new SocketRequest();
        request.emitEvent = "calculateEvChop"; //rabbitCardData //rabbitOpenCards
        request.plainDataToBeSend = null;
        request.jsonDataToBeSend = requestObjectData;
        request.requestDataStructure = requestStringData;
        socketRequest.Add(request);
    }

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


    public void SendBetData(int betAmount, int totalBetInRound, string userAction, int roundNo)
    {
        BetData requestData = new BetData();
        requestData.userData = new UserBetData();
        requestData.userAction = userAction;
        requestData.userData.betData = totalBetInRound;

        requestData.userData.playerAction = userAction;
        requestData.userData.roundNo = roundNo;

        requestData.userId = "" + PlayerManager.instance.GetPlayerGameData().userId;
        requestData.tableId = TABLE_ID;
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


}


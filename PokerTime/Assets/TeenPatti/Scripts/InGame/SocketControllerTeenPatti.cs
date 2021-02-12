using System.Collections.Generic;
using UnityEngine;
using System;
using BestHTTP.SocketIO;
using System.Collections;
using BestHTTP.JSON;
using LitJson;


public class SocketControllerTeenPatti : MonoBehaviour
{
    public static SocketControllerTeenPatti instance;

    private const float RESPONSE_READ_DELAY = 0.2f, REQUEST_SEND_DELAY = 0.2f;
    private SocketManager socketManager;

    private List<SocketResponseTeenPatti> socketResponse = new List<SocketResponseTeenPatti>();
    private List<SocketRequestTeenPatti> socketRequest = new List<SocketRequestTeenPatti>();

    [SerializeField]
    private SocketStateTeenPatti socketState;

    [SerializeField]
    private string TABLE_ID = "";


    void Awake()
    {
        instance = this;
        SetSocketState(SocketStateTeenPatti.NULL);
    }

    void Start()
    {
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
        InGameUiManagerTeenPatti.instance.ShowTableMessage("Connecting...");
        ResetConnection(isReconnecting);
        SetSocketState(SocketStateTeenPatti.Connecting);


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

    public void OnPlayerSit()
    {
        SendGameJoinRequest();

    }


    private void ReConnect()
    {
        socketManager = null;
        SocketOptions socketOptions = new SocketOptions();
        socketOptions.Timeout = new TimeSpan(0, 0, 4);
        socketOptions.Reconnection = false;
        socketOptions.AutoConnect = false;
        socketOptions.ReconnectionDelayMax = new TimeSpan(0, 0, 4);

        socketManager = new SocketManager(new Uri(GameConstants.SOCKET_URL_FLASH + "/socket.io/"), socketOptions);
//        Debug.LogError("URL IS " + GameConstants.SOCKET_URL + "/socket.io/");
        socketManager.Socket.On(SocketIOEventTypes.Connect, OnServerConnect);
        socketManager.Socket.On(SocketIOEventTypes.Disconnect, OnServerDisconnect);
        socketManager.Socket.On(SocketIOEventTypes.Error, OnError);

        //Default Events
        socketManager.Socket.On
            ("reconnect", OnReconnect);
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
        socketManager.Socket.On("tipToDealer", OntipToDealer);
        socketManager.Socket.On("sendEmoji", OnSentEmoji);


        socketManager.Socket.On("playerStatndOut", OnPlayerStandUp);
        socketManager.Socket.On("seenData", OnCardSeen);
        socketManager.Socket.On("seatObject", OnSeatObject);
        //socketManager.Socket.On("showData", OnShowMatch);

        socketManager.Socket.On("showTest", OnShowMatch);


        socketManager.Socket.On("sideShowRequest", OnSideShow);

        socketManager.Socket.On("sideShowWinner", OnSideShowWinnerCalled);
        socketManager.Socket.On("playerReSeat", OnPlayerReseat);
        socketManager.Socket.On("rejectRequest", OnSideShowRequestReject);
        socketManager.Socket.On("getAllCards", OnGetCardsOnPotLimit);
        socketManager.Socket.On("chalNotification", OnChaalNotification);
        socketManager.Socket.On("showNotification", OnShowNotification);
        socketManager.Socket.On("packNotification", OnFoldNotification);
        socketManager.Open();
    }


    private void HandleSocketResponse()
    {
        if (socketResponse.Count > 0)
        {
            SocketResponseTeenPatti responseObject = socketResponse[0];
            socketResponse.RemoveAt(0);

            //Debug.LogError("Response is :" + responseObject.eventType);

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

            switch (responseObject.eventType)
            {
                case SocketEvetnsTeenPatti.CONNECT:
                    {
                        switch (GetSocketState())
                        {
                            case SocketStateTeenPatti.Connecting:
                                SetSocketState(SocketStateTeenPatti.WaitingForOpponent);

                                if (GlobalGameManager.IsJoiningPreviousGame)
                                {
                                    RequestForMatchStatus();
                                }
                                else
                                {
                                    Debug.Log("How many Times");
                                    SendGameJoinRequest();
                                }
                                break;

                            case SocketStateTeenPatti.ReConnecting:
                                RequestForMatchStatus();
                                break;

                            default:
                                break;

                        }
                    }



                    break;

                case SocketEvetnsTeenPatti.DISCONNECT:
                    StartReconnectProcedure();
                    break;


                case SocketEvetnsTeenPatti.RECONNECT_ATTEMPT:
                    break;


                case SocketEvetnsTeenPatti.PLAYER_OBJECT:


                    Debug.Log("Error of player object----------");
                    if (InGameManagerTeenPatti.instance != null)
                    {
                        Debug.Log("Error of player object---000000000-------");

                        if (GetSocketState() == SocketStateTeenPatti.ReConnecting)
                        {
                            SetSocketState(SocketStateTeenPatti.Game_Running);
                        }

                        InGameManagerTeenPatti.instance.OnPlayerObjectFound(responseObject.data);
                        InGameUiManagerTeenPatti.instance.DestroyScreen(InGameScreensTeenPatti.Reconnecting);
                        InGameUiManagerTeenPatti.instance.ShowTableMessage("");
                    }
                    else
                    {
                        Debug.LogError("Null reference exception found IngameManager is null... ");
                    }
                    break;

                case SocketEvetnsTeenPatti.ON_OPEN_CARD_DATA_FOUND:
                   // InGameManagerTeenPatti.instance.OnOpenCardsDataFound(responseObject.data);
                    break;

                //case SocketEvetns.ON_OPEN_CARD_TIMER_FOUND:
                //InGameManagerScript.instance.OnOpenCardTimerFound(responseObject.data);
                //break;

                case SocketEvetnsTeenPatti.ON_NEXT_ROUND_TIMER_FOUND:
                    InGameManagerTeenPatti.instance.OnNextMatchCountDownFound(responseObject.data);
                    break;

                case SocketEvetnsTeenPatti.ON_GAME_OVER_TIMER_FOUND:
                    //Debug.LogError("Game Over - " + responseObject.data);
                    //InGameManager.instance.OnGameOverCountDownFound(responseObject.data);
                    break;

                case SocketEvetnsTeenPatti.ON_CALL_TIMER_FOUND:
                    InGameManagerTeenPatti.instance.OnTurnCountDownFound(responseObject.data);
                    break;


                case SocketEvetnsTeenPatti.ON_GAME_START_TIMER_FOUND:
                    InGameManagerTeenPatti.instance.OnGameStartTimeFound(responseObject.data);
                    break;

                case SocketEvetnsTeenPatti.ON_ROUND_NO_FOUND:
                    // InGameUiManager.instance.LoadingImage.SetActive(false);
                    InGameManagerTeenPatti.instance.OnRoundDataFound(responseObject.data);
                    break;

                //case SocketEvetns.ON_POT_DATA_FOUND:
                //InGameManagerScript.instance.OnPotDataFound(responseObject.data);
                //break;

                case SocketEvetnsTeenPatti.ON_RESULT_FOUND:
                    InGameManagerTeenPatti.instance.OnResultResponseFound(responseObject.data);
                    break;

                //case SocketEvetns.ON_RESULT_TIMER_FOUND:
                //break;

                case SocketEvetnsTeenPatti.ON_BET_DATA_FOUND:
                    InGameManagerTeenPatti.instance.PlayerTimerReset();
                    InGameManagerTeenPatti.instance.OnBetDataFound(responseObject.data);
                    break;

                //case SocketEvetns.ON_CARD_DISTRIBUTE_TIMER_FOUND:
                //InGameManagerScript.instance.OnCardDistributeTimerFound(responseObject.data);
                //break;

                case SocketEvetnsTeenPatti.ON_MESSAGE_FOUND:
                    ChatManagerTeenPatti.instance.OnChatMessageReceived(responseObject.data);
                    break;


                case SocketEvetnsTeenPatti.ON_RECONNECTED:
                    if (!responseObject.data.Contains("Reconnect Success"))
                    {
                        InGameUiManagerTeenPatti.instance.DestroyScreen(InGameScreensTeenPatti.Reconnecting);

                        InGameUiManagerTeenPatti.instance.ShowMessage("Match Data not found", () =>
                        {
                            InGameManagerTeenPatti.instance.LoadMainMenu();
                        });
                    }

                    break;

                case SocketEvetnsTeenPatti.ON_MATCH_HISTORY_FOUND:

                    if (HandHistoryUiManagerTeenPatti.instance != null)
                    {
                        HandHistoryUiManagerTeenPatti.instance.OnMatchHistoryFound(responseObject.data);
                    }
                    break;

                case SocketEvetnsTeenPatti.ON_PlayerStandUp:
                    InGameManagerTeenPatti.instance.StandUpPlayer(responseObject.data);
                    break;
                case SocketEvetnsTeenPatti.ON_SendEmoji:
                    InGameManagerTeenPatti.instance.SendEmoji(responseObject.data);
                    break;
                case SocketEvetnsTeenPatti.ON_CARD_SEEN:
                    InGameManagerTeenPatti.instance.CardSeen(responseObject.data);
                    break;
                case SocketEvetnsTeenPatti.ON_PLAYER_SEAT_AGAIN:
                    InGameManagerTeenPatti.instance.PlayerReseat(responseObject.data);
                    break;
                case SocketEvetnsTeenPatti.ON_SHOW_MATCH:
                    InGameManagerTeenPatti.instance.ShowMatch(responseObject.data);
                    break;
                case SocketEvetnsTeenPatti.ON_SIDE_SHOW:
                    InGameManagerTeenPatti.instance.SideShow(responseObject.data);
                    break;
                case SocketEvetnsTeenPatti.ON_SIDE_SHOW_REJECT:
                    InGameManagerTeenPatti.instance.SideShowReject(responseObject.data);
                    break;

                case SocketEvetnsTeenPatti.ON_CHAAL_NOTIFICATION:
                    InGameManagerTeenPatti.instance.OnChaalNotify(responseObject.data);
                    break;
                case SocketEvetnsTeenPatti.ON_SHOW_NOTIFICATION:
                    InGameManagerTeenPatti.instance.OnShowNotify(responseObject.data);
                    break;
                case SocketEvetnsTeenPatti.ON_FOLD_NOTIFICATION:
                    InGameManagerTeenPatti.instance.OnFoldNotify(responseObject.data);
                    break;

                case SocketEvetnsTeenPatti.ON_GET_CARDS_ON_POT_LIMIT:
                    InGameManagerTeenPatti.instance.PotLimiReached(responseObject.data);
                    break;
                case SocketEvetnsTeenPatti.ON_SIDE_SHOW_WINNER:
                    InGameManagerTeenPatti.instance.OnSideShowWinner(responseObject.data);
                    break;
                case SocketEvetnsTeenPatti.ON_SEAT_OBJECT:
                    InGameManagerTeenPatti.instance.OnSeatObject(responseObject.data);
                    break;
                case SocketEvetnsTeenPatti.ON_TipToDealer:
                    InGameManagerTeenPatti.instance.TipToDealer(responseObject.data);
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
            SocketRequestTeenPatti request = socketRequest[0];
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
        //Debug.LogError("Objecyt is :" + responseText);

#if DEBUG

#if UNITY_EDITOR
        if (GlobalGameManager.instance.CanDebugThis(SocketEvetns.PLAYER_OBJECT))
        {
           // Debug.Log("OnPlayerObjectFound = " + responseText + "  Time = " + System.DateTime.Now);
           
        }
#else
        Debug.Log("OnPlayerObjectFound = " + responseText + "  Time = " + System.DateTime.Now);
#endif
#endif
        SocketResponseTeenPatti response = new SocketResponseTeenPatti();
        response.eventType = SocketEvetnsTeenPatti.PLAYER_OBJECT;
        response.data = responseText;
        socketResponse.Add(response);
    }
    void OnOpenCardDataFound(Socket socket, Packet packet, params object[] args)
    {
        string responseText = JsonMapper.ToJson(args);
        InGameManagerTeenPatti.instance.Pot.SetActive(true);
#if DEBUG

#if UNITY_EDITOR
        if (GlobalGameManager.instance.CanDebugThis(SocketEvetns.ON_OPEN_CARD_DATA_FOUND))
        {
            Debug.Log("OnOpenCardDataFound = " + responseText + "  Time = " + System.DateTime.Now);
           
        }
#else
        Debug.Log("OnOpenCardDataFound = " + responseText + "  Time = " + System.DateTime.Now);
#endif
#endif

        SocketResponseTeenPatti response = new SocketResponseTeenPatti();
        response.eventType = SocketEvetnsTeenPatti.ON_OPEN_CARD_DATA_FOUND;
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

        SocketResponseTeenPatti response = new SocketResponseTeenPatti();
        response.eventType = SocketEvetnsTeenPatti.ON_OPEN_CARD_TIMER_FOUND;
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

        SocketResponseTeenPatti response = new SocketResponseTeenPatti();
        response.eventType = SocketEvetnsTeenPatti.ON_NEXT_ROUND_TIMER_FOUND;
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
           // Debug.Log("OnGameOverTimerFound = " + responseText + "  Time = " + System.DateTime.Now);
        }
#else
        Debug.Log("OnGameOverTimerFound = " + responseText + "  Time = " + System.DateTime.Now);
#endif
#endif

        SocketResponseTeenPatti response = new SocketResponseTeenPatti();
        response.eventType = SocketEvetnsTeenPatti.ON_GAME_OVER_TIMER_FOUND;
        response.data = responseText;
        socketResponse.Add(response);
    }


    void OnCallTimerFound(Socket socket, Packet packet, params object[] args)
    {
        string responseText = JsonMapper.ToJson(args);
       // Debug.LogError("CheckTimer");
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

        SocketResponseTeenPatti response = new SocketResponseTeenPatti();
        response.eventType = SocketEvetnsTeenPatti.ON_CALL_TIMER_FOUND;
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

        SocketResponseTeenPatti response = new SocketResponseTeenPatti();
        response.eventType = SocketEvetnsTeenPatti.ON_GAME_START_TIMER_FOUND;
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
            Debug.Log("OnRoundNoFound = " + responseText + "  Time = " + System.DateTime.Now);
        }
#else
        Debug.Log("OnRoundNoFound = " + responseText + "  Time = " + System.DateTime.Now);
#endif
#endif

        SocketResponseTeenPatti response = new SocketResponseTeenPatti();
        response.eventType = SocketEvetnsTeenPatti.ON_ROUND_NO_FOUND;
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

        SocketResponseTeenPatti response = new SocketResponseTeenPatti();
        response.eventType = SocketEvetnsTeenPatti.ON_POT_DATA_FOUND;
        response.data = responseText;
        socketResponse.Add(response);
    }


    void OnResultDataFound(Socket socket, Packet packet, params object[] args)
    {
        string responseText = JsonMapper.ToJson(args);
        InGameManagerTeenPatti.instance.Pot.SetActive(false);
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

        SocketResponseTeenPatti response = new SocketResponseTeenPatti();
        response.eventType = SocketEvetnsTeenPatti.ON_RESULT_FOUND;
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

        SocketResponseTeenPatti response = new SocketResponseTeenPatti();
        response.eventType = SocketEvetnsTeenPatti.ON_RESULT_TIMER_FOUND;
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
            Debug.Log("OnBetDataFound = " + responseText + "  Time = " + System.DateTime.Now);
        }
#else
        Debug.Log("OnBetDataFound = " + responseText + "  Time = " + System.DateTime.Now);
#endif
#endif

        SocketResponseTeenPatti response = new SocketResponseTeenPatti();
        response.eventType = SocketEvetnsTeenPatti.ON_BET_DATA_FOUND;
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

        SocketResponseTeenPatti response = new SocketResponseTeenPatti();
        response.eventType = SocketEvetnsTeenPatti.ON_CARD_DISTRIBUTE_TIMER_FOUND;
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

        SocketResponseTeenPatti response = new SocketResponseTeenPatti();
        response.eventType = SocketEvetnsTeenPatti.ON_MESSAGE_FOUND;
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

        SocketResponseTeenPatti response = new SocketResponseTeenPatti();
        response.eventType = SocketEvetnsTeenPatti.ON_RECONNECTED;
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

        SocketResponseTeenPatti response = new SocketResponseTeenPatti();
        response.eventType = SocketEvetnsTeenPatti.ON_MATCH_HISTORY_FOUND;
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

        SocketResponseTeenPatti response = new SocketResponseTeenPatti();
        response.eventType = SocketEvetnsTeenPatti.CONNECT;
        socketResponse.Add(response);
    }


    void OnServerDisconnect(Socket socket, Packet packet, params object[] args)
    {

#if DEBUG

#if UNITY_EDITOR
        if (GlobalGameManager.instance.CanDebugThis(SocketEvetns.DISCONNECT))
        {
            Debug.Log("Enter in OnServerDisconnect Time = " + System.DateTime.Now);
        }
#else
        Debug.Log("Enter in OnServerDisconnect Time = " + System.DateTime.Now);
#endif
#endif
        SocketResponseTeenPatti response = new SocketResponseTeenPatti();
        response.eventType = SocketEvetnsTeenPatti.DISCONNECT;
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
                Debug.Log("Internal error! Time = " + System.DateTime.Now);
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
        //if (GlobalGameManager.instance.CanDebugThis(SocketEvetnsTeenPatti.RECONNECT_ATTEMPT))
        //{
        //    Debug.Log("Enter in OnReconnectAttempt Time = " + System.DateTime.Now);
        //}
#else
        Debug.Log("Enter in OnReconnectAttempt Time = " + System.DateTime.Now);
#endif

#endif

        SocketResponseTeenPatti response = new SocketResponseTeenPatti();
        response.eventType = SocketEvetnsTeenPatti.RECONNECT_ATTEMPT;
        socketResponse.Add(response);
    }

    void OnReconnectFailed(Socket socket, Packet packet, params object[] args)
    {
#if DEBUG
        Debug.Log("ReconnectFailed Time = " + System.DateTime.Now);
#endif
    }



    void OntipToDealer(Socket socket, Packet packet, params object[] args)
    {
#if DEBUG
        Debug.Log("OnOtherSeeTip CALL     = " + System.DateTime.Now);
#endif

        string responseText = JsonMapper.ToJson(args);

#if DEBUG

#if UNITY_EDITOR
        //if (GlobalGameManager.instance.CanDebugThis(SocketEvetnsTeenPatti.ON_TipToDealer))
        //{
        //    Debug.Log("OnOtherSeeTip CALL = " + responseText + "  Time = " + System.DateTime.Now);
        //}
#else
        Debug.Log("OnReconnected = " + responseText + "  Time = " + System.DateTime.Now);
#endif
#endif

        SocketResponseTeenPatti response = new SocketResponseTeenPatti();
        response.eventType = SocketEvetnsTeenPatti.ON_TipToDealer;
        response.data = responseText;
        socketResponse.Add(response);
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
        SocketResponseTeenPatti response = new SocketResponseTeenPatti();
        response.eventType = SocketEvetnsTeenPatti.ON_SendEmoji;
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

        SocketResponseTeenPatti response = new SocketResponseTeenPatti();
        response.eventType = SocketEvetnsTeenPatti.ON_PlayerStandUp;
        response.data = responseText;
        socketResponse.Add(response);
    }


    void OnCardSeen(Socket socket, Packet packet, params object[] args)
    {
        string responseText = JsonMapper.ToJson(args);

#if DEBUG

#if UNITY_EDITOR
        //if (GlobalGameManager.instance.CanDebugThis(SocketEvetns.ON_PlayerStandUp))
        //{
        //    //Debug.Log("OnStartGameTimerFound = " + responseText + "  Time = " + System.DateTime.Now);
        //}
#else
        Debug.Log("OnStartGameTimerFound = " + responseText + "  Time = " + System.DateTime.Now);
#endif
#endif

        SocketResponseTeenPatti response = new SocketResponseTeenPatti();
        response.eventType = SocketEvetnsTeenPatti.ON_CARD_SEEN;
        response.data = responseText;
        socketResponse.Add(response);
    }

    void OnPlayerReseat(Socket socket, Packet packet, params object[] args)
    {
        string responseText = JsonMapper.ToJson(args);

#if DEBUG

#if UNITY_EDITOR
        //if (GlobalGameManager.instance.CanDebugThis(SocketEvetns.ON_PlayerStandUp))
        //{
        //    //Debug.Log("OnStartGameTimerFound = " + responseText + "  Time = " + System.DateTime.Now);
        //}
#else
        Debug.Log("OnStartGameTimerFound = " + responseText + "  Time = " + System.DateTime.Now);
#endif
#endif

        SocketResponseTeenPatti response = new SocketResponseTeenPatti();
        response.eventType = SocketEvetnsTeenPatti.ON_PLAYER_SEAT_AGAIN;
        response.data = responseText;
        socketResponse.Add(response);
    }

    void OnSeatObject(Socket socket, Packet packet, params object[] args)
    {
        string responseText = JsonMapper.ToJson(args);

#if DEBUG

#if UNITY_EDITOR
        //if (GlobalGameManager.instance.CanDebugThis(SocketEvetns.ON_PlayerStandUp))
        //{
        //    //Debug.Log("OnStartGameTimerFound = " + responseText + "  Time = " + System.DateTime.Now);
        //}
#else
        Debug.Log("OnStartGameTimerFound = " + responseText + "  Time = " + System.DateTime.Now);
#endif
#endif

        SocketResponseTeenPatti response = new SocketResponseTeenPatti();
        response.eventType = SocketEvetnsTeenPatti.ON_SEAT_OBJECT;
        response.data = responseText;
        socketResponse.Add(response);
    }

    void OnShowMatch(Socket socket, Packet packet, params object[] args)
    {
        string responseText = JsonMapper.ToJson(args);

#if DEBUG

#if UNITY_EDITOR
        //if (GlobalGameManager.instance.CanDebugThis(SocketEvetns.ON_PlayerStandUp))
        //{
        //    //Debug.Log("OnStartGameTimerFound = " + responseText + "  Time = " + System.DateTime.Now);
        //}
#else
        Debug.Log("OnStartGameTimerFound = " + responseText + "  Time = " + System.DateTime.Now);
#endif
#endif

        SocketResponseTeenPatti response = new SocketResponseTeenPatti();
        response.eventType = SocketEvetnsTeenPatti.ON_SHOW_MATCH;
        response.data = responseText;
        socketResponse.Add(response);
    }


    void OnSideShow(Socket socket, Packet packet, params object[] args)
    {
        string responseText = JsonMapper.ToJson(args);

#if DEBUG

#if UNITY_EDITOR
        //if (GlobalGameManager.instance.CanDebugThis(SocketEvetns.ON_PlayerStandUp))
        //{
        //    //Debug.Log("OnStartGameTimerFound = " + responseText + "  Time = " + System.DateTime.Now);
        //}
#else
        Debug.Log("OnStartGameTimerFound = " + responseText + "  Time = " + System.DateTime.Now);
#endif
#endif

        SocketResponseTeenPatti response = new SocketResponseTeenPatti();
        response.eventType = SocketEvetnsTeenPatti.ON_SIDE_SHOW;
        response.data = responseText;
        socketResponse.Add(response);
    }

    void OnSideShowWinnerCalled(Socket socket, Packet packet, params object[] args)
    {
        string responseText = JsonMapper.ToJson(args);

#if DEBUG

#if UNITY_EDITOR
        //if (GlobalGameManager.instance.CanDebugThis(SocketEvetns.ON_PlayerStandUp))
        //{
        //    //Debug.Log("OnStartGameTimerFound = " + responseText + "  Time = " + System.DateTime.Now);
        //}
#else
        Debug.Log("OnStartGameTimerFound = " + responseText + "  Time = " + System.DateTime.Now);
#endif
#endif

        SocketResponseTeenPatti response = new SocketResponseTeenPatti();
        response.eventType = SocketEvetnsTeenPatti.ON_SIDE_SHOW_WINNER;
        response.data = responseText;
        socketResponse.Add(response);
    }


    void OnSideShowRequestReject(Socket socket, Packet packet, params object[] args)
    {
        string responseText = JsonMapper.ToJson(args);

#if DEBUG

#if UNITY_EDITOR
        //if (GlobalGameManager.instance.CanDebugThis(SocketEvetns.ON_PlayerStandUp))
        //{
        //    //Debug.Log("OnStartGameTimerFound = " + responseText + "  Time = " + System.DateTime.Now);
        //}
#else
        Debug.Log("OnStartGameTimerFound = " + responseText + "  Time = " + System.DateTime.Now);
#endif
#endif

        SocketResponseTeenPatti response = new SocketResponseTeenPatti();
        response.eventType = SocketEvetnsTeenPatti.ON_SIDE_SHOW_REJECT;
        response.data = responseText;
        socketResponse.Add(response);
    }

    void OnGetCardsOnPotLimit(Socket socket, Packet packet, params object[] args)
    {
        string responseText = JsonMapper.ToJson(args);

#if DEBUG

#if UNITY_EDITOR
        //if (GlobalGameManager.instance.CanDebugThis(SocketEvetns.ON_PlayerStandUp))
        //{
        //    //Debug.Log("OnStartGameTimerFound = " + responseText + "  Time = " + System.DateTime.Now);
        //}
#else
        Debug.Log("OnStartGameTimerFound = " + responseText + "  Time = " + System.DateTime.Now);
#endif
#endif

        SocketResponseTeenPatti response = new SocketResponseTeenPatti();
        response.eventType = SocketEvetnsTeenPatti.ON_GET_CARDS_ON_POT_LIMIT;
        response.data = responseText;
        socketResponse.Add(response);
    }

    void OnChaalNotification(Socket socket, Packet packet, params object[] args)
    {
        string responseText = JsonMapper.ToJson(args);

#if DEBUG

#if UNITY_EDITOR
        //if (GlobalGameManager.instance.CanDebugThis(SocketEvetns.ON_PlayerStandUp))
        //{
        //    //Debug.Log("OnStartGameTimerFound = " + responseText + "  Time = " + System.DateTime.Now);
        //}
#else
        Debug.Log("OnStartGameTimerFound = " + responseText + "  Time = " + System.DateTime.Now);
#endif
#endif

        SocketResponseTeenPatti response = new SocketResponseTeenPatti();
        response.eventType = SocketEvetnsTeenPatti.ON_CHAAL_NOTIFICATION;
        response.data = responseText;
        socketResponse.Add(response);
    }

    void OnShowNotification(Socket socket, Packet packet, params object[] args)
    {
        string responseText = JsonMapper.ToJson(args);

#if DEBUG

#if UNITY_EDITOR
        //if (GlobalGameManager.instance.CanDebugThis(SocketEvetns.ON_PlayerStandUp))
        //{
        //    //Debug.Log("OnStartGameTimerFound = " + responseText + "  Time = " + System.DateTime.Now);
        //}
#else
        Debug.Log("OnStartGameTimerFound = " + responseText + "  Time = " + System.DateTime.Now);
#endif
#endif

        SocketResponseTeenPatti response = new SocketResponseTeenPatti();
        response.eventType = SocketEvetnsTeenPatti.ON_SHOW_NOTIFICATION;
        response.data = responseText;
        socketResponse.Add(response);
    }

    void OnFoldNotification(Socket socket, Packet packet, params object[] args)
    {
        string responseText = JsonMapper.ToJson(args);

#if DEBUG

#if UNITY_EDITOR
        //if (GlobalGameManager.instance.CanDebugThis(SocketEvetns.ON_PlayerStandUp))
        //{
        //    //Debug.Log("OnStartGameTimerFound = " + responseText + "  Time = " + System.DateTime.Now);
        //}
#else
        Debug.Log("OnStartGameTimerFound = " + responseText + "  Time = " + System.DateTime.Now);
#endif
#endif

        SocketResponseTeenPatti response = new SocketResponseTeenPatti();
        response.eventType = SocketEvetnsTeenPatti.ON_FOLD_NOTIFICATION;
        response.data = responseText;
        socketResponse.Add(response);
    }


    #endregion





    // EMIT_METHODS ---------------------------------------------------------------------------------------------------------------------------------------------------------


    #region EMIT_METHODS
    public void SendStandUpdata()
    {
        StandUpdata requestData = new StandUpdata();
        requestData.tableId = TABLE_ID;
        requestData.userId = "" + PlayerManager.instance.GetPlayerGameData().userId;
        requestData.isStatndOut = 1;

        //Debug.LogError(requestData.userId + "  <Stand up User ID table id >" + TABLE_ID);

        string requestStringData = JsonMapper.ToJson(requestData);
        object requestObjectData = Json.Decode(requestStringData);

        SocketRequestTeenPatti request = new SocketRequestTeenPatti();
        request.emitEvent = "playerStatndOut";
        request.plainDataToBeSend = null;
        request.jsonDataToBeSend = requestObjectData;
        request.requestDataStructure = requestStringData;
        socketRequest.Add(request);

    }


    public void UserSeenCard()
    {
        


        string requestStringData = "{\"userId\":\"" + PlayerManager.instance.GetPlayerGameData().userId + "\"," +
     "\"tableId\":\"" + TABLE_ID + "\"}";


        object requestObjectData = Json.Decode(requestStringData);

        SocketRequestTeenPatti request = new SocketRequestTeenPatti();
        request.emitEvent = "cardSeen";

        request.plainDataToBeSend = null;
        request.jsonDataToBeSend = requestObjectData;
        request.requestDataStructure = requestStringData;
        socketRequest.Add(request);
    }


    public void UserSitAgain()
    {



        string requestStringData = "{\"userId\":\"" + PlayerManager.instance.GetPlayerGameData().userId + "\"," +
     "\"tableId\":\"" + TABLE_ID + "\"}";


        object requestObjectData = Json.Decode(requestStringData);

        SocketRequestTeenPatti request = new SocketRequestTeenPatti();
        request.emitEvent = "playerReSeat";

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

        SocketRequestTeenPatti request = new SocketRequestTeenPatti();
        request.emitEvent = "topUp";

        request.plainDataToBeSend = null;
        request.jsonDataToBeSend = requestObjectData;
        request.requestDataStructure = requestStringData;
        socketRequest.Add(request);
    }

    public void TipToDealer()
    {
        string requestStringData = "{\"userId\":\"" +( (int.Parse(PlayerManager.instance.GetPlayerGameData().userId)).ToString() + "\"," +
     "\"tableId\":\"" + int.Parse(TABLE_ID)).ToString() + "\"}";

        Debug.Log("i am herejhdsf   "+ requestStringData);
        object requestObjectData = Json.Decode(requestStringData);

        SocketRequestTeenPatti request = new SocketRequestTeenPatti();
        request.emitEvent = "allTipData";

        request.plainDataToBeSend = null;
        request.jsonDataToBeSend = requestObjectData;
        request.requestDataStructure = requestStringData;
        socketRequest.Add(request);
    }

    public void SentEmoji(int otherUserId,int emojiIndex)
    {
   
        string requestStringData = "{\"sentBy\":\"" + ((int.Parse(PlayerManager.instance.GetPlayerGameData().userId)).ToString() + "\"," +
            "\"sentTo\":\"" + otherUserId + "\"," +
            "\"deductionValue\":\"" + 2 + "\"," +
            "\"emojiIndex\":\"" + emojiIndex + "\"," +
            "\"tableId\":\"" + int.Parse(TABLE_ID)).ToString() + "\"}";

        //Debug.LogError("i am SentEmoji   " + requestStringData);
        object requestObjectData = Json.Decode(requestStringData);


        SocketRequestTeenPatti request = new SocketRequestTeenPatti();
        request.emitEvent = "sendEmoji";
        request.plainDataToBeSend = null;
        request.jsonDataToBeSend = requestObjectData;
        request.requestDataStructure = requestStringData;
        socketRequest.Add(request);
    }

    public void SendGameJoinRequest()
    {
        //string requestStringData = "{\"userId\":\"" + PlayerManager.instance.GetPlayerGameData().userId + "\"," +
        //     "\"players\":\"" + GlobalGameManager.instance.GetRoomData().players + "\"," +
        //     "\"roomId\":\"" + GlobalGameManager.instance.GetRoomData().roomId + "\"," +
        //     "\"playerType\":\"Real\"," +
        //     "\"isPrivate\":\"No\"," +
        //     "\"isFree\":\"No\"}";

        string requestStringData = "{\"userId\":" + PlayerManager.instance.GetPlayerGameData().userId + "," +
            "\"roomId\":" + 1 + "," +
             "\"players\":" + 5 + "}";
        //     //"\"playerType\":\"Real\"," +
        //     //"\"isPrivate\":\"No\"," +
        //     //"\"isFree\":\"No\"}";

        object requestObjectData = Json.Decode(requestStringData);

        SocketRequestTeenPatti request = new SocketRequestTeenPatti();
        request.emitEvent = "joinRoom";

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
        requestData.userData.playerAction = InGameManagerTeenPatti.instance.GetLastPlayerAction();
        requestData.userData.roundNo = InGameManagerTeenPatti.instance.GetMatchRound();

        requestData.userId = "" + PlayerManager.instance.GetPlayerGameData().userId;
        requestData.tableId = TABLE_ID;

        string requestStringData = JsonMapper.ToJson(requestData);
        object requestObjectData = Json.Decode(requestStringData);

        SocketRequestTeenPatti request = new SocketRequestTeenPatti();
        request.emitEvent = "pack";
        request.plainDataToBeSend = null;
        request.jsonDataToBeSend = requestObjectData;
        request.requestDataStructure = requestStringData;
        socketRequest.Add(request);
    }




    public void SendShowMatch()
    {
        BetData requestData = new BetData();
        //requestData.userData = new UserBetData();
        //requestData.userAction = userAction;
        //requestData.userAction = userAction;
        //requestData.userData.betData = totalBetInRound;

        //requestData.userData.playerAction = userAction;
        //requestData.userData.roundNo = roundNo;

        requestData.userId = "" + PlayerManager.instance.GetPlayerGameData().userId;
        requestData.tableId = TABLE_ID;
        //requestData.tableId = "1";
         requestData.bet = "" + GameConstants.playerbetAmount;

        string requestStringData = JsonMapper.ToJson(requestData);
        object requestObjectData = Json.Decode(requestStringData);

        SocketRequestTeenPatti request = new SocketRequestTeenPatti();
        request.emitEvent = "showMatch";
        request.plainDataToBeSend = null;
        request.jsonDataToBeSend = requestObjectData;
        request.requestDataStructure = requestStringData;
        socketRequest.Add(request);
    }


    public void OnSideShowUserCalled()
    {
        BetData requestData = new BetData();
        requestData.userData = new UserBetData();
        //requestData.userAction = userAction;
        //requestData.userAction = userAction;
        //requestData.userData.betData = totalBetInRound;

        //requestData.userData.playerAction = userAction;
        //requestData.userData.roundNo = roundNo;

        requestData.userId = "" + PlayerManager.instance.GetPlayerGameData().userId;
        requestData.tableId = TABLE_ID;
        requestData.bet = "" + GameConstants.playerbetAmount;

        string requestStringData = JsonMapper.ToJson(requestData);
        object requestObjectData = Json.Decode(requestStringData);

        SocketRequestTeenPatti request = new SocketRequestTeenPatti();
        request.emitEvent = "sideShowRequest";
        request.plainDataToBeSend = null;
        request.jsonDataToBeSend = requestObjectData;
        request.requestDataStructure = requestStringData;
        socketRequest.Add(request);
    }

    public void OnSideShowWinnerCalled()
    {
        SideShowData requestData = new SideShowData();
        //requestData.userData = new UserBetData();
        //requestData.userAction = userAction;
        //requestData.userAction = userAction;
        //requestData.userData.betData = totalBetInRound;

        //requestData.userData.playerAction = userAction;
        //requestData.userData.roundNo = roundNo;

        requestData.userId = "" + PlayerManager.instance.GetPlayerGameData().userId;
        requestData.tableId = TABLE_ID;
        requestData.requesterUserId = GameConstants.sideShowRequesterId;
       
        requestData.bet = "" + GameConstants.playerbetAmount;

        string requestStringData = JsonMapper.ToJson(requestData);
        object requestObjectData = Json.Decode(requestStringData);

        SocketRequestTeenPatti request = new SocketRequestTeenPatti();
        request.emitEvent = "sideShow";
        request.plainDataToBeSend = null;
        request.jsonDataToBeSend = requestObjectData;
        request.requestDataStructure = requestStringData;
        socketRequest.Add(request);
    }

    public void OnSideShowRejectCalled()
    {
        SideShowData requestData = new SideShowData();
        //requestData.userData = new UserBetData();
        //requestData.userAction = userAction;
        //requestData.userAction = userAction;
        //requestData.userData.betData = totalBetInRound;

        //requestData.userData.playerAction = userAction;
        //requestData.userData.roundNo = roundNo;

        //requestData.userId = "" + PlayerManager.instance.GetPlayerGameData().userId;
        requestData.tableId = TABLE_ID;
        requestData.requesterUserId = GameConstants.sideShowRequesterId;

        // requestData.bet = "" + GameConstants.playerbetAmount;

        string requestStringData = JsonMapper.ToJson(requestData);
        object requestObjectData = Json.Decode(requestStringData);

        SocketRequestTeenPatti request = new SocketRequestTeenPatti();
        request.emitEvent = "rejectRequest";
        request.plainDataToBeSend = null;
        request.jsonDataToBeSend = requestObjectData;
        request.requestDataStructure = requestStringData;
        socketRequest.Add(request);
    }


    public void SendBetData(int betAmount, int totalBetInRound, string userAction, int roundNo)
    {
        BetData requestData = new BetData();
        requestData.userData = new UserBetData();
        requestData.userAction = userAction;
        requestData.userAction = userAction;
        requestData.userData.betData = totalBetInRound;

        requestData.userData.playerAction = userAction;
        requestData.userData.roundNo = roundNo;

        requestData.userId = "" + PlayerManager.instance.GetPlayerGameData().userId;
        requestData.tableId = TABLE_ID;
        requestData.bet = "" + GameConstants.playerbetAmount;
        //Debug.LogError("Amount in bet is :" + GameConstants.playerbetAmount);

        string requestStringData = JsonMapper.ToJson(requestData);
        object requestObjectData = Json.Decode(requestStringData);

        SocketRequestTeenPatti request = new SocketRequestTeenPatti();
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

        SocketRequestTeenPatti request = new SocketRequestTeenPatti();
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

        SocketRequestTeenPatti request = new SocketRequestTeenPatti();
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

        SocketRequestTeenPatti request = new SocketRequestTeenPatti();
        request.emitEvent = "left";
        request.plainDataToBeSend = null;
        request.jsonDataToBeSend = requestObjectData;
        request.requestDataStructure = requestStringData;
        socketRequest.Add(request);

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
        SocketRequestTeenPatti request = new SocketRequestTeenPatti();

        request.emitEvent = "sendMessage";

        request.plainDataToBeSend = null;
        request.jsonDataToBeSend = requestObject;
        request.requestDataStructure = requestData;
        socketRequest.Add(request);
    }






    #endregion















    // OTHER_METHODS ---------------------------------------------------------------------------------------------------------------------------------------------------------

    public SocketStateTeenPatti GetSocketState()
    {
        return socketState;
    }

    public void SetSocketState(SocketStateTeenPatti state)
    {
        socketState = state;
    }

    public void SetTableId(string tableIdToAssign)
    {
        GlobalGameManager.instance.GetRoomData().socketTableId = tableIdToAssign;
        TABLE_ID = tableIdToAssign;

        PrefsManager.SetData(PrefsKey.RoomData, JsonUtility.ToJson(GlobalGameManager.instance.GetRoomData()));
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

            SetSocketState(SocketStateTeenPatti.NULL);
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
        InGameUiManagerTeenPatti.instance.ShowScreen(InGameScreensTeenPatti.Reconnecting);
        SetSocketState(SocketStateTeenPatti.ReConnecting);
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
                   // Debug.LogError("Functionality written");
                }
            }

            ++counter;
        }

        if (!socketManager.Socket.IsOpen)
        {
            InGameUiManagerTeenPatti.instance.DestroyScreen(InGameScreensTeenPatti.Reconnecting);
            InGameUiManagerTeenPatti.instance.ShowMessage("ReConnect Attempt failed, please check your network connection", () =>
            {
                StartCoroutine(WaitForReconnect());
            },

             () =>
             {
                 InGameManagerTeenPatti.instance.LoadMainMenu();
             }, "Retry", "Cancel"
            );
        }

        isPreocedureRunning = false;
    }

    private void RequestForMatchStatus()
    {
        //InGameManagerScript.instance.isUpdateTableCards = true;
        //InGameManagerScript.instance.MATCH_ROUND = -1;


        ReconnectData data = new ReconnectData();
        data.userId = "" + PlayerManager.instance.GetPlayerGameData().userId;
        data.tableId = TABLE_ID;
        data.isYesOrNo = "Yes";

        string requestdeta = JsonMapper.ToJson(data);
        object requestfordeta = Json.Decode(requestdeta);

        SocketRequestTeenPatti request = new SocketRequestTeenPatti();
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

            if (GetSocketState() == SocketStateTeenPatti.Game_Running)
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




public enum SocketEvetnsTeenPatti
{
    CONNECT,
    DISCONNECT,
    RECONNECT_ATTEMPT,
    PLAYER_OBJECT,
    ON_OPEN_CARD_DATA_FOUND,
    ON_OPEN_CARD_TIMER_FOUND,
    ON_NEXT_ROUND_TIMER_FOUND,
    ON_GAME_OVER_TIMER_FOUND,
    ON_CALL_TIMER_FOUND,
    ON_GAME_START_TIMER_FOUND,
    ON_ROUND_NO_FOUND,
    ON_POT_DATA_FOUND,
    ON_RESULT_FOUND,
    ON_RESULT_TIMER_FOUND,
    ON_BET_DATA_FOUND,
    ON_CARD_DISTRIBUTE_TIMER_FOUND,
    ON_MESSAGE_FOUND,
    ON_RECONNECTED,
    ON_MATCH_HISTORY_FOUND,
    ON_PlayerStandUp,
    ON_TipToDealer,
    ON_SendEmoji,
    ON_CARD_SEEN,
    ON_SHOW_MATCH,
    ON_SIDE_SHOW,
    ON_SIDE_SHOW_WINNER,
    ON_SEAT_OBJECT,
    ON_PLAYER_SEAT_AGAIN,
    ON_SIDE_SHOW_REJECT,
    ON_GET_CARDS_ON_POT_LIMIT,
    ON_CHAAL_NOTIFICATION,
    ON_SHOW_NOTIFICATION,
    ON_FOLD_NOTIFICATION,
    NULL
}


[System.Serializable]
public class StandUpdataTeenPatti
{
    public string tableId;
    public string userId;
    public int isStatndOut;
}

[System.Serializable]
public class SocketRequestTeenPatti
{
    public object jsonDataToBeSend;
    public string plainDataToBeSend;
    public string emitEvent;
    public string requestDataStructure;
}

[System.Serializable]
public class SocketResponseTeenPatti
{
    public SocketEvetnsTeenPatti eventType;
    public string data;
}




[System.Serializable]
public class TournamentJoiningRequestDataTeenPatti
{
    public string userId;
    public string tournamentId;
    public string players;
    public string currentRound;
    public string playerType;
    public string remainingTime;
}


[System.Serializable]
public class FoldDataTeenPatti
{
    public string tableId;
    public string userId;
    public UserBetData userData;
}

[System.Serializable]
public class BetDataTeenPatti
{
    public string tableId;
    public string userId;
    public string bet;
    public UserBetData userData;
    public string userAction;


}


[System.Serializable]
public class SideShowData
{
    public string tableId;
    public string userId;
    public string requesterUserId;
    public string bet;


}











[System.Serializable]
public class ReconnectDataTeenPatti
{
    public string tableId;
    public string userId;
    public string isYesOrNo;
}


[System.Serializable]
public class SendMessageDataTeenPatti
{
    public string userId;
    public string tableId;

    public string from;
    public string to;
    public string title;
    public string desc;
}

[System.Serializable]
public class CoinUpdateDataTeenPatti
{
    public string email;
    public string coins;
}

[System.Serializable]
public class UserBetDataTeenPatti
{
    public int betData, roundNo;
    public string playerAction;
}

[System.Serializable]
public class RematchDataTeenPatti
{
    public string isBuyIn, userId, tableId, coins;
}

[System.Serializable]
public class RoomCreateDataTeenPatti
{
    public string roomId;
    public string players;
    public string isPrivate;
    public string isFree;
}


[System.Serializable]
public class PrivateRoomJoinDataTeenPatti
{
    public string userId;
    public string roomId;
    public string players;
    public string playerType;

    public string isPrivate;
    public string isFree;
    public string tableId;
}



[System.Serializable]
public enum SocketStateTeenPatti
{
    Connecting,
    WaitingForOpponent,
    Game_Running,
    InitializingCards,
    ReConnecting,
    NULL
}


/*

 OnPlayerObjectFound = [[{"userData":{"betData":0.0,"roundNo":1.0,"playerAction":"Check"},"isNextWinner":false,"isTurn":false,"isWin":false,"isDeductCoins":false,"resultWaiting":false,"isRematch":"No","isAlreadyCheck":"No","isBet":false,"totalBet":20.0,"totalBet2":0.0,"isBlocked":false,"userId":51.0,"socketId":"aTuZzHp4jUxADK7-AAAC","tableId":14.0,"roomId":"9","userName":"jay","coins":1752.657,"playerType":"Real","isStart":true,"isGameBlock":false,"isDisconnet":false,"isLeft":false,"turnCount":0.0,"isDealer":true,"isCheck":true,"smallBlind":false,"bigBlind":true,"nextDealer":false,"isAllInAmt":false,"currentBet":0.0,"totalCoins":980.0,"isBuyIn":"No","mergeCards":["As","2s","8c","Tc","3h","4s","Ks"],"cards":["4s","Ks"]},{"userData":{"betData":0.0,"roundNo":1.0,"playerAction":"Check"},"isNextWinner":false,"isTurn":true,"isWin":false,"isDeductCoins":false,"resultWaiting":false,"isRematch":"No","isAlreadyCheck":"No","isBet":false,"totalBet":20.0,"totalBet2":10.0,"isBlocked":false,"userId":52.0,"socketId":"akiWmRuHBu3OGelpAAAD","tableId":14.0,"roomId":"9","userName":"jaydevice","coins":247.343,"playerType":"Real","isStart":true,"isGameBlock":false,"isDisconnet":false,"isLeft":false,"turnCount":1.0,"isDealer":false,"isCheck":true,"smallBlind":true,"bigBlind":false,"nextDealer":true,"isAllInAmt":false,"currentBet":0.0,"totalCoins":980.0,"isBuyIn":"No","mergeCards":["As","2s","8c","Tc","3h","Js","9s"],"cards":["Js","9s"]}]] 

    OnOpenCardDataFound = [["As","2s","8c","Tc","3h"]]

     [{"userId":52.0,"bet":10.0,"lastBet":20.0,"pot":40.0}]

    OnRoundNoFound = [{"currentSubRounds":2.0,"currentRounds":0.0}] 


    OnResultDataFound = [[[{"userId":51.0,"totalBet":20.0,"totalCoins":1020.0,"name":"High Card","discription":"A High","isWin":true,"winNumber":1.0,"posibleCards":["As","Ks","Tc","8c","4s"],"cards":["4s","Ks"],"mergeCards":["As","2s","8c","Tc","3h","4s","Ks"],"winAmount":40.0,"winBy":"winner"},{"userId":52.0,"totalBet":20.0,"totalCoins":980.0,"name":"High Card","discription":"A High","isWin":false,"winNumber":false,"posibleCards":["As","Js","Tc","9s","8c"],"cards":["Js","9s"],"mergeCards":["As","2s","8c","Tc","3h","Js","9s"],"winAmount":0.0,"winBy":"winner"}]]]  
    */


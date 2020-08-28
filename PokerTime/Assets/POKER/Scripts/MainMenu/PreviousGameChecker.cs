using System.Collections.Generic;
using UnityEngine;
using System;
using BestHTTP.SocketIO;
using System.Collections;
using BestHTTP.JSON;
using LitJson;

public class PreviousGameChecker: MonoBehaviour
{
    private SocketManager socketManager;

    [SerializeField]
    private GameObject loadingScreen;
    private bool isCheckConnectionStatus = true, isLeaveMatchRequestSending = false;
    private SocketState socketState = SocketState.NULL;
    RoomData roomData = null;



    private void Start()
    {
        roomData = PrefsManager.GetRoomData();

        Debug.Log("is lobby in script = "+roomData.isLobbyRoom);

        if (!string.IsNullOrEmpty(roomData.socketTableId))
        {
            ToggleLoadingScreen(true);
            Connect();
            StartCoroutine(WaitAndCheckConnectionStatus());
        }
        else
        {
            Destroy(gameObject);
        }
    }


    private int reconnectAttempt = 0;
    private IEnumerator WaitAndCheckConnectionStatus()
    {
        while (isCheckConnectionStatus)
        {
            yield return new WaitForSeconds(3f);

            if (socketState == SocketState.Connecting)
            {
                if (socketManager != null)
                {
                    socketManager.Close();
                    yield return new WaitForSeconds(1f);
                }

                Connect();
            }
            else if (socketState == SocketState.ReConnecting)
            {
                if (WebServices.instance.IsInternetAvailable() && socketManager != null && socketManager.Socket.IsOpen)
                {
                    ++reconnectAttempt;

                    if (reconnectAttempt <= 2)
                    {
                        SendReconectRequest();
                    }
                    else
                    {
                        Connect();
                    }
                }
                else
                {
                    Connect();
                }
            }

        }


        if (socketManager != null && socketManager.Socket.IsOpen)
        {
            socketManager.Close();
        }


        yield return new WaitForSeconds(2f);

        socketManager = null;
        ToggleLoadingScreen(false);

        Destroy(gameObject);
    }


    private void Connect()
    {
        reconnectAttempt = 0;
        socketState = SocketState.Connecting;
        socketManager = null;
        SocketOptions socketOptions = new SocketOptions();
        socketOptions.Timeout = new TimeSpan(0, 0, 4);
        socketOptions.Reconnection = false;
        socketOptions.AutoConnect = false;
        socketOptions.ReconnectionDelayMax = new TimeSpan(0, 0, 4);

        socketManager = new SocketManager(new Uri(GameConstants.SOCKET_URL + "/socket.io/"), socketOptions);

        socketManager.Socket.On(SocketIOEventTypes.Connect, OnServerConnect);
        socketManager.Socket.On(SocketIOEventTypes.Disconnect, OnServerDisconnect);
        socketManager.Socket.On(SocketIOEventTypes.Error, OnError);
        socketManager.Socket.On("userReconnect", OnReconnected);

        socketManager.Open();
    }




    void OnServerConnect(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log("Enter in OnServerConnect Time = " + System.DateTime.Now);
        SendReconectRequest();
    }

    void OnReconnected(Socket socket, Packet packet, params object[] args)
    {
        socketState = SocketState.NULL;

        string responseText = JsonMapper.ToJson(args);
        JsonData data = JsonMapper.ToObject(responseText);

        Debug.Log("OnReconnected to socket main menu responseText = " + responseText);

        ToggleLoadingScreen(false);


        if (isLeaveMatchRequestSending)
        {
            SendLeaveMatchRequest();
        }
        else
        {
            if (data[0]["message"].ToString().Contains("Reconnect Success"))
            {
                MainMenuController.instance.ShowMessage("Previous match is not ended yet, Do you want to join that table?", () =>
                {
                    GlobalGameManager.instance.SetRoomData(roomData);
                    GlobalGameManager.IsJoiningPreviousGame = true;
                    GlobalGameManager.instance.LoadScene(Scenes.InGame);
                }, () =>
                {
                    SendLeaveMatchRequest();
                });
            }
            else
            {
                DisconnectSocket();
            }
        }
    }

    void OnServerDisconnect(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log("Enter in OnServerDisconnect Time = " + System.DateTime.Now);
    }


    void OnError(Socket socket, Packet packet, params object[] args)
    {

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





    private void SendReconectRequest()
    {
        ReconnectData requestData = new ReconnectData();

        requestData.userId = "" + PlayerManager.instance.GetPlayerGameData().userId;
        requestData.tableId = roomData.socketTableId;
        requestData.isYesOrNo = "Yes";

        string requestStringData = JsonMapper.ToJson(requestData);
        object requestObjectData = Json.Decode(requestStringData);
        socketState = SocketState.ReConnecting;
        socketManager.Socket.Emit("userReconnect", requestObjectData);
    }


    public void SendLeaveMatchRequest()
    {
        if (WebServices.instance.IsInternetAvailable() && socketManager != null && socketManager.Socket.IsOpen)
        {
            FoldData requestData = new FoldData();

            requestData.userId = "" + PlayerManager.instance.GetPlayerGameData().userId;
            requestData.tableId = roomData.socketTableId;

            string requestStringData = JsonMapper.ToJson(requestData);
            object requestObjectData = Json.Decode(requestStringData);
            socketManager.Socket.Emit("leaveMatch", requestObjectData);

            Debug.Log("Sending leaveMatch Request Main Menu =" + requestStringData);
            Invoke("DisconnectSocket", GameConstants.BUFFER_TIME);
        }
        else
        {
            ToggleLoadingScreen(true);
            isLeaveMatchRequestSending = true;
            Connect();
        }
    }

    private void DisconnectSocket()
    {
        isCheckConnectionStatus = false;
    }


    private void ToggleLoadingScreen(bool isShow)
    {
        loadingScreen.SetActive(isShow);
    }



}

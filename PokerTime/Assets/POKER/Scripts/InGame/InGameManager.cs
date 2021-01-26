using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using DG.Tweening;
using UnityEditor;
using System.IO;
using UnityEngine.Networking;
using System;
using BestHTTP.SocketIO;
using System.Security.Cryptography;
using System.Linq;

public class InGameManager : MonoBehaviour
{
    public static InGameManager instance;

    [SerializeField]
    public PlayerScript[] allPlayersObject;

    [SerializeField]
    private Transform[] allPlayerPos;


    [SerializeField]
    private GameObject cardAnimationPrefab,betAnimationPrefab;
    [SerializeField]
    private Transform animationLayer;

    public List<GameObject> AllPots = new List<GameObject>();
    public List<float> PotValues = new List<float>();

    public GameObject Pot;
    
    [SerializeField]
    private Text potText;

    [SerializeField]
    private GameObject winningPrefab,chipscoine;

    [SerializeField]
    public Image[] communityCards;

    public bool isGameStart;

    private PlayerScript[] onlinePlayersScript = null;
    private PlayerScript myPlayerObject = null,currentPlayer = null;
    private int MATCH_ROUND = 0, LAST_BET_AMOUNT = 0;
    private CardData[] openCards = null;
    
    private string lastPlayerAction = "";
    private List<GameObject> winnersObject = new List<GameObject>();
    private int communityCardsAniamtionShowedUpToRound = 0;
    private int currentRoundTotalBets = 0;
    private float pot1Amount = 0;

    private bool isRematchRequestSent = false,isTopUpDone = false;
    private float availableBalance = 0;

    public GameObject WinnAnimationpos;

    //DEV_CODE (Created By Nitin)
    public string currentClickedSeatNum = "";

    //DEV_CODE

    //Variables to store values regarding match winning cards and to highlight them
    public bool isHighlightCard;
    public CardData[] highlightCards;
    public string[] highlightCardString;

    //For Recording Game Play
    Texture2D screenshot;
    public int videoWidth /* = 1280*/;
    public int videoHeight /*= 720*/;
    public bool isRecording = false;
    

    //To Store Player Data
    public string cardValue = "";          //To Store Card Number with Card Icon
    string tableValue = "";         //To Store table blinds values
    string userID = "";

    //To Store Date and Time
    string date = "";
    string time = "";

    //To Store total player bet value
    string balance = "";

    bool isCardValueSet = false;
    bool isScreenshotCaptured = false;
    string myPlayerSeat;

    public Image thunderPointBar;
    [HideInInspector]
    public bool userWinner = false;

    private void Awake()
    {
        instance = this;
        //Debug.Log("Time: " + System.DateTime.Now.Hour + System.DateTime.Now.Minute);
    }

    public Text TableName;
    public GameObject RabbitButton;

    private void Start()
    {
        //DEV_CODE
        highlightCardString = new string[5];
        highlightCards = new CardData[5];

        //InGameUiManager.instance.ShowTableMessage("Select a seat");
        RabbitButton.SetActive(false);
        gameExitCalled = false;
        //DEV_CODE
        videoHeight = (int)InGameUiManager.instance.height;
        videoWidth = (int)InGameUiManager.instance.width;

        for (int i = 0; i < communityCards.Length; i++)
        {
            communityCards[i].gameObject.SetActive(false);
        }

        UpdatePot("");
        Pot.SetActive(false);
        DeactivateAllPots();
        onlinePlayersScript = new PlayerScript[0];

        for (int i = 0; i < allPlayersObject.Length; i++)
        {
            allPlayersObject[i].TogglePlayerUI(false);
            allPlayersObject[i].ResetAllData();
        }
        Debug.Log("table id is :" + GlobalGameManager.instance.GetRoomData().socketTableId);
        TableName.text = "";// GlobalGameManager.instance.GetRoomData().title;
        //AdjustAllPlayersOnTable(GlobalGameManager.instance.GetRoomData().players);
    }

    public void DeactivateAllPots()
    {
        foreach(GameObject g in AllPots)
        {
            g.SetActive(false);
        }
    }

    public void GetAvailableSeats()
    {
        string req = "{\"tableId\":\"" + GlobalGameManager.instance.GetRoomData().socketTableId + "\"}";
        //Debug.LogError("Sending get available seats :" + req);
        WebServices.instance.SendRequest(RequestType.GetSeatObject, req, true, OnServerResponseFound);
    }

    public List<GameObject> AllSeatButtons = new List<GameObject>();
    TableSeats AllSeats;

    public GameObject GetSeatObject(string seatNo)
    {
        int seat = 0;
        int.TryParse(seatNo, out seat);

        seat = seat - 1;
        if(seat < 0) { seat = 0; }

        return AllSeatButtons[seat];
    }

    public void OnServerResponseFound(RequestType requestType, string serverResponse, bool isShowErrorMessage, string errorMessage)
    {
        //Debug.LogError("Seats available 0:" + serverResponse);

        if (errorMessage.Length > 0)
        {
            if (isShowErrorMessage)
            {
                //InGameUiManager.instance.ShowMessage(errorMessage);
            }
            return;
        }

        if (requestType == RequestType.GetSeatObject)
        {
            //Debug.LogError("Seats available 1:" + serverResponse);
            AllSeats = JsonUtility.FromJson<TableSeats>(serverResponse);
            UpdateSeatClickSettingsAndView();
        }
    }

    private void UpdateSeatClickSettingsAndView()
    {
        Vector3 position1 = GetSeatObject(myPlayerSeat).transform.position;
        Vector3 position2 = GetSeatObject("1").transform.position;

        //GetSeatObject(myPlayerSeat).transform.position = position2;
        //GetSeatObject("1").transform.position = position1;
        foreach (GameObject g in AllSeatButtons)
        {
            //g.SetActive(false);            
        }
            for (int i = 0; i < AllSeatButtons.Count; i++)
            {
                AllSeatButtons[i].SetActive(true);
                AllSeatButtons[i].GetComponent<PlayerSeat>().UpdateState();
            }
        
        //GetSeatObject(myPlayerSeat).SetActive(false);
    }
    private bool DontShowCommunityCardAnimation = false;
    public void OnRabbitDataFound(string responseText)
    {
        //Debug.LogError("vip catd is :" + GetMyPlayerObject().GetPlayerData().userVIPCard);
        //Debug.LogError("isFold :" + GetMyPlayerObject().GetPlayerData().isFold);
        InGameUiManager.instance.ToggleSuggestionButton(false);
        InGameUiManager.instance.ToggleActionButton(false);
        int vipCard = 0;
        int.TryParse(GetMyPlayerObject().GetPlayerData().userVIPCard, out vipCard);
        
        if (vipCard > 0)
        {
            RabbitButton.SetActive(true);
            StartCoroutine(DisableRabbitButton());
            OnOpenCardsDataFound(responseText);
            DontShowCommunityCardAnimation = true;
        }
    }

    private IEnumerator DisableRabbitButton()
    {
        yield return new WaitForSeconds(2.5f);
        RabbitButton.SetActive(false);
    }

    private void Init(List<MatchMakingPlayerData> matchMakingPlayerData)
    {
        Debug.Log("Total Users " + matchMakingPlayerData.Count);
        isRematchRequestSent = false;
        List<MatchMakingPlayerData> newMatchMakingPlayerData = new List<MatchMakingPlayerData>();
        if (matchMakingPlayerData.Count > 1)
            newMatchMakingPlayerData = ReArrangePlayersList(matchMakingPlayerData);
        else
            newMatchMakingPlayerData = matchMakingPlayerData;
        onlinePlayersScript = new PlayerScript[newMatchMakingPlayerData.Count];
        PlayerScript playerScriptWhosTurn = null;
        //Debug.Log("Arranged Users " + newMatchMakingPlayerData.Count);
        for (int i = 0; i < allPlayersObject.Length; i++)
        {
            allPlayersObject[i].ResetAllData();
            if (i < newMatchMakingPlayerData.Count)
            {
                Debug.Log(newMatchMakingPlayerData[i].playerData.userName + " " + newMatchMakingPlayerData[i].isTurn);
                //Debug.Log("Url " + newMatchMakingPlayerData[i].playerData.avatarurl);
                allPlayersObject[i].seat = (i + 1).ToString();
                allPlayersObject[i].TogglePlayerUI(true, newMatchMakingPlayerData[i].playerData.avatarurl);
                onlinePlayersScript[i] = allPlayersObject[i];
                onlinePlayersScript[i].Init(newMatchMakingPlayerData[i]);

                if (newMatchMakingPlayerData[i].isTurn)
                {
                    playerScriptWhosTurn = onlinePlayersScript[i];
                }
            }
            else
            {
                allPlayersObject[i].TogglePlayerUI(false);
            }   
        }

        if (playerScriptWhosTurn != null)
        {
            StartCoroutine(WaitAndShowCardAnimation(onlinePlayersScript, playerScriptWhosTurn));
        }
        /*else
        {
#if ERROR_LOG
            Debug.LogError("Null Reference exception found playerId whos turn is not found");
#endif
        }*/
    }

    private IEnumerator WaitAndShowCardAnimation(PlayerScript[] players, PlayerScript playerScriptWhosTurn)
    {
        Debug.Log("WaitAndShowCardAnimation............");
        if (!GlobalGameManager.IsJoiningPreviousGame)
        {
            GlobalGameManager.IsJoiningPreviousGame = isGameStart;
            List<GameObject> animatedCards = new List<GameObject>();
            for (int i = 0; i < players.Length; i++)
            {
                Image[] playerCards = players[i].GetCardsImage();

                /*                Debug.Log("Player Cards: " + playerCards[i].name);*/

                if (players[i].playerData.isStart)
                {
                    for (int j = 0; j < playerCards.Length; j++)
                    {
                        GameObject gm = Instantiate(cardAnimationPrefab, animationLayer) as GameObject;
                        gm.transform.DOMove(playerCards[j].transform.position, GameConstants.CARD_ANIMATION_DURATION);
                        gm.transform.DOScale(Vector3.one/*playerCards[j].transform.localScale*/, GameConstants.CARD_ANIMATION_DURATION);
                        gm.transform.DORotateQuaternion(playerCards[j].transform.rotation, GameConstants.CARD_ANIMATION_DURATION);
                        animatedCards.Add(gm);
                        SoundManager.instance.PlaySound(SoundType.CardMove);
                        yield return new WaitForSeconds(0.1f);
                    }


                    yield return new WaitForSeconds(0.1f);
                }
            }

            yield return new WaitForSeconds(GameConstants.CARD_ANIMATION_DURATION);

            for (int i = 0; i < animatedCards.Count; i++)
            {
                Destroy(animatedCards[i]);
            }

            animatedCards.Clear();
        }


        for (int i = 0; i < players.Length; i++)
        {
            Debug.Log(players[i].playerData.userName + " Name " + players[i].IsMe() + ", " + players[i].playerData.isStart);
            if (!players[i].playerData.isStart)
                players[i].ToggleCards(false, players[i].IsMe());
            else
                players[i].ToggleCards(true, players[i].IsMe());
        }

        if (isGameStart)
            SocketController.instance.SetSocketState(SocketState.Game_Running);
        SwitchTurn(playerScriptWhosTurn,false);
    }

    public IEnumerator SwitchTables()
    {
        //set new table id
        InGameUiManager.instance.ShowScreen(InGameScreens.Loading);
        yield return new WaitForSeconds(2f);
        RoomData data = GetRandomRoom();
        if (PlayerManager.instance.GetPlayerGameData().coins < data.minBuyIn)
        {
            StartCoroutine(SwitchTables());
            yield break;
        }
        
        data.isLobbyRoom = true;
        GlobalGameManager.instance.SetRoomData(data);

        StartCoroutine(SwitchToAnotherTableReset());

        resetGame = true;
        StartCoroutine(StartWaitingCountdown(0));
    }

    public RoomData GetRandomRoom()
    {
        RoomData data = null;

        List<List<RoomData>> allRoomData = GlobalGameManager.instance.GetLobbyRoomData();
        int gameMode = GlobalGameManager.instance.GetGameMode();

        System.Random rnd = new System.Random();
        int randomVal = rnd.Next(0, allRoomData[gameMode].Count);

        data = allRoomData[gameMode][randomVal];

        return data;
    }

    public int GetLastBetAmount()
    {
        return LAST_BET_AMOUNT;
    }

    internal void OnGameOverCountDownFound(string data)
    {
        string t = data.Substring(1, data.Length - 2);
        Debug.LogError("Game Over - " + t);
        if (float.Parse(t) < 1)
            ResetMatchData();
    }

    public void UpdateAvailableBalance(float balance)
    {
        availableBalance = balance;
    }
    public void PlayerTimerReset()
    {
        for (int i = 0; i < onlinePlayersScript.Length; i++)
        {
            onlinePlayersScript[i].ResetTurn();
        }
    }

    public void ResetAllDataForPlayers()
    {
        for (int i = 0; i < onlinePlayersScript.Length; i++)
        {
            onlinePlayersScript[i].ResetAllData();
            onlinePlayersScript[i].ResetTurn();
        }
    }

    private void SwitchTurn(PlayerScript playerScript,bool isCheckAvailable)
    {
        //SoundManager.instance.PlaySound(SoundType.TurnSwitch);

        for (int i = 0; i < onlinePlayersScript.Length; i++)
        {
            onlinePlayersScript[i].ResetTurn();
        }
        
        currentPlayer = playerScript;
        if (currentPlayer.IsMe())
        {
            InGameUiManager.instance.ToggleSuggestionButton(false);

            SuggestionActions selectedSuggestionAction = InGameUiManager.instance.GetSelectedSuggestionAction();
            InGameUiManager.instance.ResetSuggetionAction();

            if (selectedSuggestionAction != SuggestionActions.Null)
            {
                switch (selectedSuggestionAction)
                {
                    case SuggestionActions.Call:
                    case SuggestionActions.Call_Any:
                        {
                            int callAmount = GetLastBetAmount() - (int)GetMyPlayerObject().GetPlayerData().totalBet;

                            if (callAmount < GetMyPlayerObject().GetPlayerData().balance)
                            {
                                OnPlayerActionCompleted(PlayerAction.Call, callAmount, "Call");
                            }
                            else
                            {
                                Debug.LogWarning("LAST BET AMOUNT " + LAST_BET_AMOUNT);
                                InGameUiManager.instance.ToggleActionButton(true, currentPlayer, isCheckAvailable, LAST_BET_AMOUNT);
                            }
                        }
                        break;

                    case SuggestionActions.Check:
                        {
                            OnPlayerActionCompleted(PlayerAction.Check, 0, "Check");
                        }
                        break;

                    case SuggestionActions.Fold:
                        {
                            OnPlayerActionCompleted(PlayerAction.Fold, 0, "Fold");
                        }
                        break;

                    default:
                        {
                            Debug.LogError("Unhandled suggetion type found = "+selectedSuggestionAction);
                        }
                    break;
                }
            }
            else if(!userWinner)
            {
                //Debug.LogWarning("LAST BET AMOUNT 1" + LAST_BET_AMOUNT);
                //InGameUiManager.instance.ToggleActionButton(true, currentPlayer, isCheckAvailable, LAST_BET_AMOUNT, GetMyPlayerObject().GetPlayerData().balance);
            }
        }
        else
        {
            InGameUiManager.instance.ToggleActionButton(false);

            if (GetMyPlayerObject().GetPlayerData() != null && !GetMyPlayerObject().GetPlayerData().isFold)
            {
                int callAmount = GetLastBetAmount() - (int)GetMyPlayerObject().GetPlayerData().totalBet;
                InGameUiManager.instance.ToggleSuggestionButton(true, isCheckAvailable, callAmount, GetMyPlayerObject().GetPlayerData().balance);
            }
        }        
    }

    private List<MatchMakingPlayerData> ReArrangePlayersList(List<MatchMakingPlayerData> matchMakingPlayerData)
    {
        List<MatchMakingPlayerData> updatedList = new List<MatchMakingPlayerData>();
        /*for (int i = 0; i < matchMakingPlayerData.Count; i++)
        {
            Debug.Log(matchMakingPlayerData[i].playerData.userId + ", " + matchMakingPlayerData[i].playerData.userName + " <color=green>=</color> " + PlayerManager.instance.GetPlayerGameData().userId + ", " + PlayerManager.instance.GetPlayerGameData().userName);
            if (matchMakingPlayerData[i].playerData.userId == PlayerManager.instance.GetPlayerGameData().userId)
            {
                MatchMakingPlayerData m = matchMakingPlayerData[i];
                matchMakingPlayerData[i] = matchMakingPlayerData[0];
                matchMakingPlayerData[0] = m;
                //updatedList[0] = matchMakingPlayerData[i];
            }
        }
        for (int k = 0; k < matchMakingPlayerData.Count; k++)
        {
            Debug.Log("User " + matchMakingPlayerData[k].playerData.userName);
        }*/

        int mySeatIndex = 0;
        for (int i = 0; i < matchMakingPlayerData.Count; i++)
        {
            //Debug.Log(matchMakingPlayerData[i].playerData.userId + ", " + matchMakingPlayerData[i].playerData.userName + " <color=green>=</color> " + PlayerManager.instance.GetPlayerGameData().userId + ", " + PlayerManager.instance.GetPlayerGameData().userName);
            if (matchMakingPlayerData[i].playerData.userId == PlayerManager.instance.GetPlayerGameData().userId)
            {
                mySeatIndex = i;
                break;
            }
        }

        for (int i = mySeatIndex; i < matchMakingPlayerData.Count; i++)
        {
            updatedList.Add(matchMakingPlayerData[i]);
        }
        for (int k = 0; k < mySeatIndex; k++)
        {
            updatedList.Add(matchMakingPlayerData[k]);
        }
        /*for (int j = 0; j < updatedList.Count; j++)
        {
            Debug.Log(updatedList[j].playerData.userName);
        }*/
        return updatedList;
    }


    public bool gameExitCalled = false;

    public void LoadMainMenu()
    {
        gameExitCalled = true;
        InGameUiManager.instance.ShowScreen(InGameScreens.Loading);
        StartCoroutine(WaitAndSendLeaveRequest());
    }



    private IEnumerator WaitAndSendLeaveRequest()
    {
        Debug.LogError("WaitAndSendLeaveRequest");
        yield return new WaitForEndOfFrame();
        SocketController.instance.SendLeaveMatchRequest();
        yield return new WaitForSeconds(7f);
        InGameUiManager.instance.ShowScreen(InGameScreens.Menu);
        /*yield return new WaitForSeconds(GameConstants.BUFFER_TIME);
        SocketController.instance.ResetConnection();
        gameExitCalled = true;
        GlobalGameManager.instance.LoadScene(Scenes.MainMenu);*/
    }

    public IEnumerator SwitchToAnotherTableReset()
    {
        gameExitCalled = true;
        InGameUiManager.instance.ShowScreen(InGameScreens.Loading);

        yield return new WaitForEndOfFrame();
        SocketController.instance.SendLeaveMatchRequest();
        yield return new WaitForSeconds(GameConstants.BUFFER_TIME);
        SocketController.instance.ResetConnection();
    }

    public PlayerScript GetMyPlayerObject()
    {
        if (myPlayerObject == null)
        {
            myPlayerObject = GetPlayerObject(PlayerManager.instance.GetPlayerGameData().userId);
        }

        return myPlayerObject;
    }


    public PlayerScript GetPlayerObject(string userId)
    {
        if (onlinePlayersScript == null)
        {
            return null;
        }

        for (int i = 0; i < onlinePlayersScript.Length; i++)
        {
            if (onlinePlayersScript[i].GetPlayerData().userId == userId)
            {
                return onlinePlayersScript[i];
            }
        }

        return null;
    }

    public PlayerScript[] GetAllPlayers()
    {
        return onlinePlayersScript;
    }

    public bool AmISpectator = true;

    private void ShowNewPlayersOnTable(JsonData data, bool isMatchStarted)
    {
        for (int i = 0; i < allPlayersObject.Length; i++)
        {
            //allPlayersObject[i].ResetAllData();
            allPlayersObject[i].TogglePlayerUI(false);
        }

        for (int i = 0; i < data[0].Count; i++)
        {
            //allPlayersObject[i].ResetAllData();
            allPlayersObject[i].TogglePlayerUI(true);
        }


        List<PlayerData> playerData = new List<PlayerData>();

        for (int i = 0; i < data[0].Count; i++)
        {
            //allPlayersObject[i].transform.GetChild(0).GetComponent<DownloadAvatar>().avatarUrl = data[0][i]["profileImage"].ToString();
            if (GetPlayerObject(data[0][i]["userId"].ToString()) == null) // player not in our list
            {
                PlayerData playerDataObject = new PlayerData();

                playerDataObject.userId = data[0][i]["userId"].ToString();
                playerDataObject.userName = data[0][i]["userName"].ToString();
                playerDataObject.tableId = data[0][i]["tableId"].ToString();
                playerDataObject.balance = float.Parse(data[0][i]["totalCoins"].ToString());
                playerDataObject.avatarurl = data[0][i]["profileImage"].ToString();
                playerDataObject.isFold = bool.Parse(data[0][i]["isFold"].ToString());
                playerDataObject.isBlock = bool.Parse(data[0][i]["isBlocked"].ToString());
                playerDataObject.isStart = bool.Parse(data[0][i]["isStart"].ToString());
                //Debug.LogError("URL     new 2222222 " + playerDataObject.avatarurl);
                /*if (isMatchStarted)
                {
                    playerDataObject.isFold = data[0][i]["isBlocked"].Equals(true);
                }
                else
                {
                    playerDataObject.isFold = false;
                }*/

                playerData.Add(playerDataObject);
            }
        }
        Debug.Log("Active Users " + onlinePlayersScript.Length);
        
        /*for (int i = onlinePlayersScript.Length - 1; i < allPlayersObject.Length; i++)
        {
            allPlayersObject[i].ResetAllData();
            allPlayersObject[i].TogglePlayerUI(false);
        }*/
        
        /*if (isMatchStarted)
        {
            if (playerData.Count > 0)
            {
                int startIndex = onlinePlayersScript.Length;
                int maxIndex = startIndex + playerData.Count;
                int index = 0;

                for (int i = startIndex; i < maxIndex && i < allPlayersObject.Length; i++)
                {
                    //allPlayersObject[i].TogglePlayerUI(true);
                    //allPlayersObject[i].ShowDetailsAsNewPlayer(playerData[index]);
                    //allPlayersObject[i].ResetRealtimeResult();
                    //++index;
                    if (playerData[i].userId == PlayerManager.instance.GetPlayerGameData().userId)
                    {
                        allPlayersObject[0].TogglePlayerUI(true);
                        allPlayersObject[0].ShowDetailsAsNewPlayer(playerData[i]);
                        allPlayersObject[0].ResetRealtimeResult();
                        ++index;
                    }
                    //else
                    //{
                      //  Debug.LogError("index showing : " + index);

                        //allPlayersObject[index].TogglePlayerUI(true);
                        //allPlayersObject[index].ShowDetailsAsNewPlayer(playerData[i]);
                        //allPlayersObject[index].ResetRealtimeResult();
                        //++index;
                    //}
                }
            }
        }
        else
        {
            int index = 1;

            for (int i = 0; i < playerData.Count && i < allPlayersObject.Length; i++)
            {
                if (playerData[i].userId == PlayerManager.instance.GetPlayerGameData().userId)
                {
                    allPlayersObject[0].TogglePlayerUI(true);
                    allPlayersObject[0].ShowDetailsAsNewPlayer(playerData[i]);
                    allPlayersObject[0].ResetRealtimeResult();
                }
                else
                {
                    //Debug.LogError("index showing : " + index);

                    allPlayersObject[index].TogglePlayerUI(true);
                    allPlayersObject[index].ShowDetailsAsNewPlayer(playerData[i]);
                    allPlayersObject[index].ResetRealtimeResult();
                    ++index;
                }
            }
        }*/
        
        /*if (isMatchStarted && onlinePlayersScript != null && onlinePlayersScript.Length > 0)
        {
            List<PlayerScript> leftPlayers = new List<PlayerScript>();

            for (int i = 0; i < onlinePlayersScript.Length; i++)
            {
                bool isMatchFound = false;

                for (int j = 0; j < data[0].Count; j++)
                {
                    if (data[0][j]["userId"].ToString() == onlinePlayersScript[i].GetPlayerData().userId)
                    {
                        isMatchFound = true;
                        j = 100;
                    }
                }

                if (!isMatchFound)
                {
                    leftPlayers.Add(onlinePlayersScript[i]);
                }
            }

            for (int i = 0; i < leftPlayers.Count; i++)
            {
                leftPlayers[i].TogglePlayerUI(false);
            }
        }*/

        //int maxPlayerOnTable = GlobalGameManager.instance.GetRoomData().players;
    }
    

    private void AdjustAllPlayersOnTable(int totalPlayerCount)
    {
        if (totalPlayerCount <= 4)
        {
            int index = 0;
            for (int i = 0; i < totalPlayerCount; i++)
            {
                allPlayersObject[i].transform.position = allPlayerPos[index].position;
                index += 2;
            }
        }
        else if (totalPlayerCount <= 7)
        {
            int index = 0;

            for (int i = 0; i < totalPlayerCount; i++)
            {
                if (i == 2 || i == 7)
                {
                    ++index;
                }

                allPlayersObject[i].transform.position = allPlayerPos[index].position;
                ++index;
            }
        }
        else
        {
            for (int i = 0; i < totalPlayerCount; i++)
            {
                allPlayersObject[i].gameObject.transform.position = allPlayerPos[i].position;
            }
        }
    }



    private IEnumerator WaitAndShowBetAnimation(PlayerScript playerScript, string betAmount)
    {
        //Debug.Log("Last All in Bet: " + /*playerScript.GetLocalBetAmount()*/betAmount);
        if (!playerScript.localBg().activeSelf)
        {
            GameObject gm = Instantiate(betAnimationPrefab, animationLayer) as GameObject;
            gm.transform.GetChild(0).GetComponent<Text>().text = /*playerScript.GetLocalBetAmount().ToString()*/betAmount;
            gm.transform.position = playerScript.transform.position;
            Vector3 initialScale = gm.transform.localScale;
            gm.transform.localScale = Vector3.zero;

            gm.transform.DOMove(playerScript.localBg().transform.position, GameConstants.BET_PLACE_ANIMATION_DURATION).SetEase(Ease.OutBack);
            gm.transform.DOScale(initialScale, GameConstants.BET_PLACE_ANIMATION_DURATION).SetEase(Ease.OutBack).OnComplete(() => { playerScript.localBg().SetActive(true); });
            SoundManager.instance.PlaySound(SoundType.Bet);
            yield return new WaitForSeconds(GameConstants.BET_PLACE_ANIMATION_DURATION);
            Destroy(gm);
        }
        else
        {
            playerScript.GetLocaPot().text = betAmount;
        }
    }

    private bool winnerAnimationFound = false;

    private IEnumerator WaitAndShowWinnersAnimation(PlayerScript playerScript, string betAmount,GameObject amount)
    {
        winnerAnimationFound = true;
        yield return new WaitForSeconds(.6f);

        SoundManager.instance.PlaySound(SoundType.IncomingPot);

        GameObject gm = Instantiate(chipscoine,WinnAnimationpos.transform) as GameObject;
    //    gm.GetComponent<Text>().text = betAmount;
        gm.transform.position = WinnAnimationpos.transform.position;
        /*        Vector3 initialScale = gm.transform.localScale;
                gm.transform.localScale = Vector3.zero;*/

        //gm.transform.DOMove(playerScript.transform.position, 1.0f).SetEase(Ease.Linear);
        gm.transform.DOMove(playerScript.transform.position, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
        {
            SoundManager.instance.PlaySound(SoundType.Bet);
            Destroy(gm);
            amount.transform.DOScale(Vector3.one, GameConstants.BET_PLACE_ANIMATION_DURATION).SetEase(Ease.OutBack);
        });
        // gm.transform.DOScale(initialScale, GameConstants.BET_PLACE_ANIMATION_DURATION).SetEase(Ease.OutBack);
        /*SoundManager.instance.PlaySound(SoundType.Bet);
        yield return new WaitForSeconds(1.1f);
        Destroy(gm);
        amount.transform.DOScale(Vector3.one, GameConstants.BET_PLACE_ANIMATION_DURATION).SetEase(Ease.OutBack);*/
        yield return new WaitForSeconds(3f);
        winnerAnimationFound = false;
        if (resetGame)
        {
            resetGame = false;
            GlobalGameManager.instance.LoadScene(Scenes.InGame);
        }
    }
    public float GetPotAmount()
    {
        return pot1Amount;
    }

    private void UpdatePot(string textToShow)
    {
        //if (string.IsNullOrEmpty(textToShow))
        //{
        //    Pot.SetActive(false);
        //}
        //else
        //{
        //    Pot.SetActive(true);
        //}
        potText.text = textToShow;
        foreach(GameObject g in AllPots)
        {
            g.SetActive(false);
        }


        for(int i = 0; i < PotValues.Count; i++)
        {
            string s = PotValues[i].ToString();
            if (!string.IsNullOrEmpty(s))
            {
                AllPots[i].SetActive(true);
                AllPots[i].transform.Find("Text").GetComponent<Text>().text = s;
            }
        }

    }

    public int GetMatchRound()
    {
        return MATCH_ROUND;
    }

    public void UpdateLastPlayerAction(string dataToAssign)
    {
        lastPlayerAction = dataToAssign;
    }

    public string GetLastPlayerAction()
    {
        return lastPlayerAction;
    }



    private void ShowCommunityCardsAnimation()
    {
        if (MATCH_ROUND <= communityCardsAniamtionShowedUpToRound || openCards == null)
        {
            return;
        }
        if (DontShowCommunityCardAnimation) { return; }

        StartCoroutine(WaitAndShowCommunityCardsAnimation());
    }

    public IEnumerator WaitAndShowRabbit()
    {
        bool allcardsEmpty = false;
        for (int i = 0; i < communityCards.Length; i++)
        {
            if (openCards[i].cardIcon == CardIcon.NONE) {
                //communityCards[i].gameObject.SetActive(false);
                allcardsEmpty = true;
            }
            else
            {
                allcardsEmpty = false;
            }
            communityCards[i].sprite = openCards[i].cardsSprite;
        }
        if (allcardsEmpty) { yield break; }
        yield return new WaitForSeconds(1f);

        SoundManager.instance.PlaySound(SoundType.CardMove);

        for (int i = 0; i < communityCards.Length; i++)
        {
            GameObject gm = Instantiate(cardAnimationPrefab, animationLayer) as GameObject;
            gm.SetActive(true);
            gm.transform.localScale = communityCards[i].transform.localScale;
            gm.GetComponent<Image>().sprite = openCards[i].cardsSprite;
            gm.transform.Rotate(0, -90, 0);
            gm.transform.position = communityCards[i].transform.position;
            gm.transform.DORotate(new Vector3(0, 90, 0), GameConstants.CARD_ANIMATION_DURATION, RotateMode.LocalAxisAdd);
            gm.transform.DOMove(communityCards[i].transform.position, GameConstants.CARD_ANIMATION_DURATION);            
            yield return new WaitForSeconds(GameConstants.CARD_ANIMATION_DURATION * 0.3f);
            Destroy(gm, GameConstants.CARD_ANIMATION_DURATION * 1);
        }

        for (int i = 0; i < communityCards.Length; i++)
        {
            communityCards[i].gameObject.SetActive(true);
            communityCards[i].sprite = openCards[i].cardsSprite;
        }

    }

    private IEnumerator WaitAndShowCommunityCardsAnimation()
    {
        Debug.Log("<color=yellow>MATCH_ROUND " + MATCH_ROUND + "</color>");
        communityCardsAniamtionShowedUpToRound = MATCH_ROUND;
        bool isBetFound = false;
        for (int i = 0; i < onlinePlayersScript.Length; i++)
        {
            if (MATCH_ROUND != 0)
            {
                Debug.LogError("HT @ " + handtype);
                onlinePlayersScript[i].UpdateRealTimeResult(handtype);
            }
            Text text = onlinePlayersScript[i].GetLocaPot();

            if (text.gameObject.activeInHierarchy && !string.IsNullOrEmpty(text.text))
            {
                isBetFound = true;
                text.transform.parent.gameObject.SetActive(false);
                GameObject gm = Instantiate(betAnimationPrefab, animationLayer) as GameObject;
                gm.transform.GetChild(0).GetComponent<Text>().text = text.text;
                //Debug.LogError(onlinePlayersScript[i].localBg().transform.localPosition+", " +onlinePlayersScript[i].localBg().transform.position+ " Pos..... " + text.GetComponent<RectTransform>().anchoredPosition+", "+ text.transform.position+", "+ text.transform.localPosition);
                gm.transform.position = onlinePlayersScript[i].localBg().transform.position;
                gm.transform.GetChild(0).GetComponent<Text>().text = text.text;
                gm.transform.DOMove(Pot.transform.position, 0.3f).OnComplete(() => { Destroy(gm); });
                //Destroy(gm,GameConstants.LOCAL_BET_ANIMATION_DURATION + 0.1f);
            }

            onlinePlayersScript[i].UpdateRoundNo(GetMatchRound());
        }

        if (isBetFound)
        {
            SoundManager.instance.PlaySound(SoundType.ChipsCollect);
        }

        UpdatePot("Pot : " + (int)pot1Amount);

        switch (MATCH_ROUND)
        {
            case 1:
                {
                    WinnersNameText.text = "";

                    for (int i = 0; i < 3; i++)
                    {
                        if (openCards[i].cardIcon == CardIcon.NONE) { break; }
                        communityCards[i].sprite = openCards[i].cardsSprite;
                    }
                    yield return new WaitForSeconds(1f);
                    SoundManager.instance.PlaySound(SoundType.CardMove);

                    for (int i = 0; i < 3; i++)
                    {
                        GameObject gm = Instantiate(cardAnimationPrefab,animationLayer) as GameObject;
                        gm.transform.localScale = communityCards[0].transform.localScale;
                        gm.GetComponent<RectTransform>().DOSizeDelta(new Vector2(56.875f, 80f), 0f);
                        gm.GetComponent<Image>().sprite = openCards[i].cardsSprite;
                        gm.transform.Rotate(0,-90,0);
                        gm.transform.position = communityCards[0].transform.position;

                        gm.transform.DORotate(new Vector3(0,90,0), GameConstants.CARD_ANIMATION_DURATION,RotateMode.LocalAxisAdd);
                        gm.transform.DOMove(communityCards[i].transform.position,GameConstants.CARD_ANIMATION_DURATION);
                        //gm.transform.DOScale(communityCards[i].transform.localScale, GameConstants.CARD_ANIMATION_DURATION).SetEase(Ease.OutBack);

                        yield return new WaitForSeconds(GameConstants.CARD_ANIMATION_DURATION * 0.3f);

                        Destroy(gm, GameConstants.CARD_ANIMATION_DURATION * 3);
                    }

                    yield return new WaitForSeconds(GameConstants.CARD_ANIMATION_DURATION);

                    for (int i = 0; i < 3; i++)
                    {
                        if(openCards[i].cardIcon == CardIcon.NONE) { break; }
                        communityCards[i].sprite = openCards[i].cardsSprite;
                        communityCards[i].gameObject.SetActive(true);
                    }
                }
            break;

            case 2:
                {
                    for (int i = 0; i < 3; i++)
                    {
                        if (openCards[i].cardIcon == CardIcon.NONE) { break; }
                        communityCards[i].sprite = openCards[i].cardsSprite;
                        communityCards[i].gameObject.SetActive(true);
                    }
                    yield return new WaitForSeconds(1f);

                    SoundManager.instance.PlaySound(SoundType.CardMove);

                    for (int i = 3; i < 4; i++)
                    {
                        GameObject gm = Instantiate(cardAnimationPrefab, animationLayer) as GameObject;

                        gm.GetComponent<RectTransform>().DOSizeDelta(new Vector2(56.875f, 80f), 0f);
                        gm.transform.localScale = communityCards[i].transform.localScale;
                        gm.GetComponent<Image>().sprite = openCards[i].cardsSprite;
                        gm.transform.Rotate(0, -90, 0);
                        gm.transform.position = communityCards[i].transform.position;

                        gm.transform.DORotate(new Vector3(0, 90, 0), GameConstants.CARD_ANIMATION_DURATION, RotateMode.LocalAxisAdd);
                        gm.transform.DOMove(communityCards[i].transform.position, GameConstants.CARD_ANIMATION_DURATION);

                        yield return new WaitForSeconds(GameConstants.CARD_ANIMATION_DURATION * 0.3f);

                        Destroy(gm, GameConstants.CARD_ANIMATION_DURATION * 1);
                    }

                    yield return new WaitForSeconds(GameConstants.CARD_ANIMATION_DURATION);

                    for (int i = 0; i < 4; i++)
                    {
                        if (openCards[i].cardIcon == CardIcon.NONE) { break; }
                        communityCards[i].sprite = openCards[i].cardsSprite;
                        communityCards[i].gameObject.SetActive(true);
                    }
                }
            break;

            case 3:
                {
                    for (int i = 0; i < 4; i++)
                    {
                        if (openCards[i].cardIcon == CardIcon.NONE) { break; }
                        communityCards[i].sprite = openCards[i].cardsSprite;
                    }
                    yield return new WaitForSeconds(1f);

                    if (DontShowCommunityCardAnimation) { yield break; }

                    SoundManager.instance.PlaySound(SoundType.CardMove);

                    for (int i = 4; i < 5; i++)
                    {
                        GameObject gm = Instantiate(cardAnimationPrefab, animationLayer) as GameObject;
                        gm.GetComponent<RectTransform>().DOSizeDelta(new Vector2(56.875f, 80f), 0f);
                        gm.transform.localScale = communityCards[i].transform.localScale;
                        gm.GetComponent<Image>().sprite = openCards[i].cardsSprite;
                        gm.transform.Rotate(0, -90, 0);
                        gm.transform.position = communityCards[i].transform.position;

                        gm.transform.DORotate(new Vector3(0, 90, 0), GameConstants.CARD_ANIMATION_DURATION, RotateMode.LocalAxisAdd);
                        gm.transform.DOMove(communityCards[i].transform.position, GameConstants.CARD_ANIMATION_DURATION);

                        yield return new WaitForSeconds(GameConstants.CARD_ANIMATION_DURATION * 0.3f);
                        
                        Destroy(gm, GameConstants.CARD_ANIMATION_DURATION * 1);
                    }

                    yield return new WaitForSeconds(GameConstants.CARD_ANIMATION_DURATION);

                    for (int i = 0; i < communityCards.Length; i++)
                    {
                        if (openCards[i].cardIcon == CardIcon.NONE) { break; }
                        communityCards[i].sprite = openCards[i].cardsSprite;
                        communityCards[i].gameObject.SetActive(true);
                    }
                }
            break;

            default:
                {
                    //for (int i = 0; i < communityCards.Length; i++)
                    //{
                    //    GameObject gm = Instantiate(cardAnimationPrefab, animationLayer) as GameObject;

                    //gm.transform.localScale = communityCards[i].transform.localScale;
                    //gm.GetComponent<Image>().sprite = openCards[i].cardsSprite;
                    //gm.transform.Rotate(0, -90, 0);
                    //gm.transform.position = communityCards[i].transform.position;

                    //gm.transform.DORotate(new Vector3(0, 90, 0), GameConstants.CARD_ANIMATION_DURATION, RotateMode.LocalAxisAdd);
                    //    gm.transform.DOMove(communityCards[i].transform.position, GameConstants.CARD_ANIMATION_DURATION);

                    //    yield return new WaitForSeconds(GameConstants.CARD_ANIMATION_DURATION * 0.3f);

                    //    Destroy(gm, GameConstants.CARD_ANIMATION_DURATION * 1);
                    //}

                    for (int i = 0; i < communityCards.Length; i++)
                    {
                        if (openCards[i].cardIcon == CardIcon.NONE) { break; }
                        communityCards[i].sprite = openCards[i].cardsSprite;
                        communityCards[i].gameObject.SetActive(true);

                        communityCards[i].transform.DOMove(communityCards[i].transform.position, GameConstants.CARD_ANIMATION_DURATION);

                        yield return new WaitForSeconds(GameConstants.CARD_ANIMATION_DURATION * 0.3f);
                    }


                    //DEV_CODE
                    if (isHighlightCard)
                    {
                        for (int n = 0; n < communityCards.Length; n++)
                        {
                            //communityCards[n].color = Color.white;
                            communityCards[n].transform.GetChild(0).gameObject.SetActive(false);
                        }
                    }
                    for (int num = 0; num < communityCards.Length; num++)
                    {
                        for (int num2 = 0; num2 < highlightCards.Length; num2++)
                        {
                            if (isHighlightCard && highlightCards[num2] != null && communityCards[num].sprite.name == highlightCards[num2].cardsSprite.name)
                            {
                                //communityCards[num].color = Color.yellow;
                                communityCards[num].transform.GetChild(0).gameObject.SetActive(true);

                                //Debug.LogError("Community Card: " + communityCards[num].sprite.name);
                            }
                        }
                    }
                }
            break;
        }
        isHighlightCard = false;
        yield return new WaitForSeconds(0.1f);
    }

    public void SendEmoji(string serverResponse)
    {
         InGameUiManager.instance.OnGetEmoji(serverResponse);
       
    }
    public void TipToDealer(string serverResponse)
    {
        Debug.LogError("TipToDealer serverResponse ---*****----> " + serverResponse);

        //JsonData data = JsonMapper.ToObject(serverResponse);
        //string s = data["data"]["allPlayers"].ToString();
    }

    public int PointEarnedCounter = 0;

    public void PointUpdated(string serverResponse)
    {
        Debug.LogError("PointUpdated serverResponse ---*****----> " + serverResponse);
        PointEarnedCounter++;
    }

    public void StandUpPlayer(string serverResponse)
    {
        Debug.LogError("standUp serverResponse  " + serverResponse);
        GetMyPlayerObject().StandUp();
    }

    public void OnClickStandupBtn()
    {
        AmISpectator = true;
        SocketController.instance.SendStandUpdata();
    }

    public void OnPlayerActionCompleted(PlayerAction actionType, int betAmount, string playerAction)
    {
        // GetMyPlayerObject().ResetTurn();
        PlayerTimerReset();

        //UpdateLastPlayerAction(playerAction);
        InGameUiManager.instance.ToggleActionButton(false);

        if (actionType == PlayerAction.Fold)
        {
            SoundManager.instance.PlaySound(SoundType.Fold);
            SocketController.instance.SendFoldRequest(GetMyPlayerObject().GetLocalBetAmount());
        }
        else
        {
            if (actionType == PlayerAction.Check)
            {
                SoundManager.instance.PlaySound(SoundType.Check);
            }

            GetMyPlayerObject().AddIntoLocalBetAmount(betAmount, GetMatchRound());
            SocketController.instance.SendBetData(betAmount, GetMyPlayerObject().GetLocalBetAmount(), playerAction, GetMatchRound());
        }
    }

    public void ToggleTopUpDone(bool isDone)
    {
        isTopUpDone = isDone;
    }
    #region SocketCallBacks

    private void ResultProcess(string serverResponse)
    {
        DeactivateAllPots();

        string s = serverResponse.Remove(serverResponse.Length - 1, 1);
        s = s.Remove(0, 1);
        Debug.LogWarning("s " + s);


        AllShowdownSidePots showdownSidePot = JsonUtility.FromJson<AllShowdownSidePots>(s);
        //Debug.LogWarning("side pot count : " + showdownSidePot.sidePot.Count);
        //Debug.LogWarning("side pot count amount: " + showdownSidePot.sidePot[0].amount);

        /*for (int i = 0; i < showdownSidePot.sidePot.Count; i++)
        {
            for (int j = 0; j < showdownSidePot.sidePot[i].winners.Count; j++)
            {
                Debug.LogWarning(showdownSidePot.sidePot[i].winners[j].isWin);
                //if winner count is greater than 0 then it is a split pot.
                //if (showdownSidePot.sidePot[i].winners[j].isWin)
                {
                    InstantiateWin(showdownSidePot.sidePot[i].winners[j].userId.ToString(),
                        showdownSidePot.sidePot[i].winners[j].name,
                        showdownSidePot.sidePot[i].winners[j].winAmount.ToString(), showdownSidePot.sidePot[i].winners[j].isWin);
                }
            }
        }*/

        Dictionary<int, WinnerObject> winnersData = new Dictionary<int, WinnerObject>();
        for (int i = 0; i < showdownSidePot.sidePot.Count; i++)
        {
            for (int j = 0; j < showdownSidePot.sidePot[i].winners.Count; j++)
            {
                Debug.LogWarning(showdownSidePot.sidePot[i].winners[j].isWin);
                if (winnersData.ContainsKey(showdownSidePot.sidePot[i].winners[j].userId))
                {
                    int winAmt = showdownSidePot.sidePot[i].winners[j].winAmount;
                    winnersData[showdownSidePot.sidePot[i].winners[j].userId].winAmount += winAmt; 
                }
                else
                {
                    WinnerObject wd = new WinnerObject();
                    wd.userId = showdownSidePot.sidePot[i].winners[j].userId;
                    wd.name = showdownSidePot.sidePot[i].winners[j].name;
                    wd.userName = showdownSidePot.sidePot[i].winners[j].userName;
                    wd.winAmount = showdownSidePot.sidePot[i].winners[j].winAmount;
                    wd.isWin = showdownSidePot.sidePot[i].winners[j].isWin;
                    Debug.LogWarning(wd.userId + " " + wd.name + " " + wd.winAmount + " " + wd.isWin);
                    winnersData.Add(showdownSidePot.sidePot[i].winners[j].userId, wd);
                }
            }
        }
        Debug.LogWarning("Total winners " + winnersData.Count);
        foreach (var res in winnersData)
        {
            Debug.LogWarning(res.Value.userId + " " + res.Value.name + " " + res.Value.winAmount + " " + res.Value.isWin);
            InstantiateWin(res.Value.userId.ToString(), res.Value.name, res.Value.winAmount.ToString(), res.Value.isWin);
        }
    }

    public Text WinnersNameText;

    private void InstantiateWin(string userId, string name, string winAmount, bool isWin)
    {
        PlayerScript winnerPlayer = GetPlayerObject(userId);

        if (winnerPlayer != null)
        {
            WinnersNameText.text += "[username=" + winnerPlayer.playerData.userName + 
                                    ",userId=" + winnerPlayer.playerData.userId +"] ";

            for (int i = 0; i < animationLayer.childCount; i++)
            {
                Destroy(animationLayer.GetChild(i).gameObject);
            }

            GameObject gm = Instantiate(winningPrefab, animationLayer) as GameObject;
            if (isWin)
            {
                gm.transform.Find("WinBy").GetComponent<Text>().text = name;
                gm.transform.Find("winAmount").GetComponent<Text>().text = "+" + winAmount;
                if (string.IsNullOrEmpty(name))
                {
                    gm.transform.Find("WinBy").gameObject.SetActive(false);
                    gm.transform.Find("Image").gameObject.SetActive(false);
                }
                else
                {
                    gm.transform.Find("WinBy").gameObject.SetActive(true);
                    gm.transform.Find("Image").gameObject.SetActive(true);
                }
                if (winAmount.ToCharArray().Length > 5)
                {
                    SoundManager.instance.PlaySound(SoundType.bigWin);
                }
                gm.transform.position = winnerPlayer.gameObject.transform.position;
                gm.transform.SetParent(winnerPlayer.gameObject.transform.GetChild(0).transform);
                gm.transform.SetSiblingIndex(0);
                Vector3 inititalScale = gm.transform.localScale;
                gm.transform.localScale = Vector3.zero;
            }
            else
            {
                gm.SetActive(false);
            }
            StartCoroutine(WaitAndShowWinnersAnimation(winnerPlayer, winAmount, gm));
            // gm.transform.DOScale(inititalScale, GameConstants.BET_PLACE_ANIMATION_DURATION).SetEase(Ease.OutBack);
            winnersObject.Add(gm);
        }
    }

    public void OnResultResponseFound(string serverResponse)
    {
        //Debug.LogWarning("RESULT RESPONSE :" + serverResponse);
        userWinner = true;
        GlobalGameManager.IsJoiningPreviousGame = false;
        InGameUiManager.instance.ToggleSuggestionButton(false);
        InGameUiManager.instance.ToggleActionButton(false);

        if (winnersObject.Count > 0)
        {
            return;
        }


        Debug.Log("OnResultREsponse: " + serverResponse);
        JsonData jsonData = JsonMapper.ToObject(serverResponse);

        //Debug.Log("isMyPlayerWin:" + jsonData[0]["sidePot"][0]["users"].Count.ToString());

        if (jsonData[0]["sidePot"][0]["users"].Count > 0)
        {
            for (int i = 0; i < jsonData[0]["sidePot"][0]["users"].Count; i++)
            {
                if (!jsonData[0]["sidePot"][0]["users"][i]["isWin"].Equals(true))
                {
                    continue;
                }
                
                PlayerScript playerObject = GetPlayerObject(jsonData[0]["sidePot"][0]["users"][i]["userId"].ToString());

                if (playerObject != null)
                {
                    Image[] playerCards = playerObject.GetCardsImage();

                    if (jsonData[0]["sidePot"][0]["users"][i]["winningCards"].Count > 2)
                    {
                        for (int j = 0; j < jsonData[0]["sidePot"][0]["users"][i]["winningCards"].Count; j++)
                        {
                            highlightCardString[j] = jsonData[0]["sidePot"][0]["users"][i]["winningCards"][j].ToString();
                            highlightCards[j] = CardsManager.instance.GetCardData(highlightCardString[j]);
                            //Debug.Log(highlightCards[j].cardIcon);
                            for (int k = 0; k < playerCards.Length; k++)
                            {
                                if (playerCards[k].sprite.name == highlightCards[j].cardsSprite.name)
                                {
                                    //Debug.Log("Player Card Matched  :: ");
                                    //playerCards[k].color = Color.yellow;
                                    playerCards[k].transform.GetChild(0).gameObject.SetActive(true);
                                }
                            }
                        }
                        isHighlightCard = true;
                    }
                    else
                    {
                        isHighlightCard = false;
                    }
                }
            }
        }

        MATCH_ROUND = 10; // ToShow all cards
        ShowCommunityCardsAnimation();
        ResultProcess(serverResponse);

        for (int i = 0; i < onlinePlayersScript.Length; i++)
        {
            Debug.Log(onlinePlayersScript[i].playerData.userName + " " + onlinePlayersScript[i].playerData.isFold);
            if (onlinePlayersScript[i].playerData.isStart)
                onlinePlayersScript[i].ToggleCards(!onlinePlayersScript[i].playerData.isFold, true);
            onlinePlayersScript[i].DisablePot();
        }   
    }

    const float EPSILON = 0.5f;

    public void OnNextMatchCountDownFound(string serverResponse)
    {
        
        //DEV_CODE
        if (isRecording)
        {
            //StopRecording();
        }

        for (int i = 0; i < communityCards.Length; i++)
        {
            //communityCards[i].color = Color.white;
            communityCards[i].transform.GetChild(0).gameObject.SetActive(false);
            highlightCards[i] = null;
            isHighlightCard = false;
        }

        for (int i = 0; i < onlinePlayersScript.Length; i++)
        {
            onlinePlayersScript[i].ResetRealtimeResult();

            //DEV_CODE 
            //Logic to reset all players highlighted cards to original one.
            Image[] playerCards = onlinePlayersScript[i].GetCardsImage();

            for (int j = 0; j < onlinePlayersScript[i].playerData.cards.Length; j++)
            {
                //playerCards[j].color = Color.white;
                playerCards[j].transform.GetChild(0).gameObject.SetActive(false);
            }
        }

        JsonData data = JsonMapper.ToObject(serverResponse);
        int remainingTime = (int)float.Parse(data[0].ToString());
        //Debug.LogWarning("NEXT ROUND SERVER :" + serverResponse);
        //Debug.LogWarning("NEXT ROUND In: " + remainingTime);
        if (remainingTime > 1)
        {
           //InGameUiManager.instance.ShowTableMessage("Next Round Will Start In : " + remainingTime);
           // InGameUiManager.instance.LoadingImage.SetActive(true);
            if (!isRematchRequestSent)
            {
                if (remainingTime > GameConstants.BUFFER_TIME)
                {
                    //DEV_CODE
                    InGameUiManager.instance.isSelectedWinningBooster = false;

                    if (isTopUpDone || availableBalance >= GlobalGameManager.instance.GetRoomData().minBuyIn)
                    {
                        ToggleTopUpDone(false);
                        //Debug.Log("<color=pink>" + "MinBuyIn: " + GlobalGameManager.instance.GetRoomData().minBuyIn + " and Available Balance: " + availableBalance + "</color>");
                        SocketController.instance.SendReMatchRequest("Yes", "0");
                    }
                    else
                    {
                        //int balanceToAdd = (int)GlobalGameManager.instance.GetRoomData().minBuyIn - (int)availableBalance;
                        //float userMainBalance = PlayerManager.instance.GetPlayerGameData().coins;

                        //now we are adding balance if userbalance is 0.
                        int balanceToAdd = (int)GlobalGameManager.instance.GetRoomData().minBuyIn;
                        float userMainBalance = PlayerManager.instance.GetPlayerGameData().coins;
                        Debug.LogWarning("USER MAIN BALANCE IS : " + userMainBalance);
                        //if (userMainBalance >= balanceToAdd)
                        if (userMainBalance < EPSILON)
                        {
                            Debug.Log("<color=pink>" + "UserMainBalance: " + userMainBalance  + " </color>");
                            SocketController.instance.SendReMatchRequest("Yes", "0");
                            //send topup request with the below api.. for clarification contact Pradeep - Digital Crew
                            SocketController.instance.SendTopUpRequest(balanceToAdd);

                            //userMainBalance -= balanceToAdd;
                            PlayerGameDetails playerData = PlayerManager.instance.GetPlayerGameData();
                            //playerData.coins = userMainBalance;
                            playerData.coins = balanceToAdd;
                            PlayerManager.instance.SetPlayerGameData(playerData);
                        }
                        else
                        {
                            if (availableBalance > GlobalGameManager.instance.GetRoomData().smallBlind)
                            {
                                Debug.Log("<color=pink>" + "SmallBlind: " + GlobalGameManager.instance.GetRoomData().smallBlind + " and Available Balance: " + availableBalance + "</color>");
                                SocketController.instance.SendReMatchRequest("Yes", "0");
                            }
                            else
                            {
                                InGameUiManager.instance.ShowMessage("You don't have enough coins to play, please purchase some coins to continue");
                                // TODO call sit out
                                // TODO show coin purchase screen
                            }
                        }
                    }
                }
                else
                {
                    Debug.Log("<color=pink>Remaining Time and buffer time not matched...</color>");
                    //SocketController.instance.SendReMatchRequest("No", "0");
                }
            }
        }
        else
        {
           // InGameUiManager.instance.LoadingImage.SetActive(false);
            InGameUiManager.instance.ShowTableMessage("");
        }

        ResetMatchData();
    }
    


    public void OnTurnCountDownFound(string serverResponse)
    {
        //Debug.LogWarning("OnTurnCountDownFound" + serverResponse);
        //if (SocketController.instance.GetSocketState() == SocketState.Game_Running)
        //{
        //    JsonData data = JsonMapper.ToObject(serverResponse);

        //    if (currentPlayer != null)
        //    {
        //        int remainingTime = (int)float.Parse(data[0].ToString());
        //        int endTime = (int)(GameConstants.TURN_TIME * 0.25f);

        //        if (remainingTime < endTime)
        //        {
        //            SoundManager.instance.PlaySound(SoundType.TurnEnd);
        //        }
        //        if (!currentPlayer.CountDownTimerRunning)
        //        {
        //            currentPlayer.PlayedExtraTimeOnce = false;
        //            currentPlayer.ShowRemainingTime(GameConstants.TURN_TIME);
        //        }
        //    }
        //    else
        //    {
        //        Debug.LogError("Null reference exception found current player object is null");
        //    }
        //}

        //Debug.Log("State " + SocketController.instance.GetSocketState());
        if (SocketController.instance.GetSocketState() == SocketState.Game_Running)
        {
            JsonData data = JsonMapper.ToObject(serverResponse);
            //Debug.Log("Timer " + data[0]["userName"].ToString() + "" + PrefsManager.GetPlayerData().userName);
            int remainingTime = (int)float.Parse(data[0].ToString());
            /*if (remainingTime >= GameConstants.TURN_TIME - 1)
            {
                InGameUiManager.instance.ToggleActionButton(true, currentPlayer, isMyTurn, LAST_BET_AMOUNT, GetMyPlayerObject().GetPlayerData().balance);
            }*/
            Debug.Log("Me&Other " + currentPlayer.IsMe() + ", " + remainingTime + ", " + GameConstants.TURN_TIME);
            if (currentPlayer != null)
            {
                if (remainingTime == 0)
                {
                    PlayerTimerReset();
                }
                if (remainingTime == GameConstants.TURN_TIME)
                {
                    if (!currentPlayer.avtar.GetComponent<Animator>().GetBool("Play"))
                    {
                        currentPlayer.avtar.GetComponent<Animator>().SetBool("Play", true);
                    }
                    //InGameUiManager.instance.actionPanelAnimator.SetBool("isOpen", false);
                }

                if (currentPlayer.IsMe())
                {
                    int endTime = (int)(GameConstants.TURN_TIME * 0.25f);

                    if (remainingTime == endTime)
                    {
                        SoundManager.instance.PlaySound(SoundType.TurnEnd);
                    }
                    currentPlayer.ShowRemainingTime(remainingTime);
                    //currentPlayer.StartPlayerTimer(remainingTime);
                    if (remainingTime >= GameConstants.TURN_TIME - 1)
                    {
                        InGameUiManager.instance.ToggleActionButton(true, currentPlayer, isMyTurn, LAST_BET_AMOUNT, GetMyPlayerObject().GetPlayerData().balance);
                    }
                }
                else if (!currentPlayer.IsMe())
                {
                    InGameUiManager.instance.raisePopUp.SetActive(false);
                    currentPlayer.ShowRemainingTime(remainingTime);
                    //currentPlayer.StartPlayerTimer(remainingTime);
                }
            }
            else
            {
                Debug.LogError("Null reference exception found current player object is null");
            }
        }
    }

    public void OnBetDataFound(string serverResponse)
    {
        //Debug.LogWarning("serverResponse BETDATAFOUND " + serverResponse);
        JsonData data = JsonMapper.ToObject(serverResponse);
        LAST_BET_AMOUNT = (int)float.Parse(data[0]["lastBet"].ToString());
        string userId = data[0]["userId"].ToString();

        pot1Amount = float.Parse(data[0]["pot"].ToString());

        string s = serverResponse.Remove(serverResponse.Length - 1, 1);
        s = s.Remove(0, 1);
        //Debug.LogWarning("s" + s);

        MyBetData betData = JsonUtility.FromJson<MyBetData>(s);

        //Debug.LogWarning("side pot length; " + betData.sidePot.Count);
        PotValues.Clear();
        for (int i=0;i< betData.sidePot.Count; i++)
        {
            var value = float.Parse(betData.sidePot[i].amount.ToString());
            PotValues.Add(value);
        }

        //if (SocketController.instance.GetSocketState() == SocketState.Game_Running)
        {
            //DEV_CODE
            if (!isCardValueSet)
            {
                for (int i = 0; i < GetMyPlayerObject().GetPlayerData().cards.Length; i++)
                {
                    cardValue = cardValue + GetMyPlayerObject().GetPlayerData().cards[i].cardIcon.ToString() + "_" + GetMyPlayerObject().GetPlayerData().cards[i].cardNumber + "_";
                }
                userID = GetMyPlayerObject().GetPlayerData().userId;
            }
            isCardValueSet = true;

            int betAmount = (int)float.Parse(data[0]["bet"].ToString());
            Debug.Log(userId+" " + PlayerManager.instance.GetPlayerGameData().userId);
            if (betAmount > 0 /*&& userId != PlayerManager.instance.GetPlayerGameData().userId*/)
            {
                PlayerScript playerObject = GetPlayerObject(userId);

                if (playerObject != null)
                {
                    Debug.Log("Current Bet Amount : " + betAmount);
                    //StartCoroutine(WaitAndShowBetAnimation(playerObject, "" + playerObject.GetLocalBetAmount()));
                    StartCoroutine(WaitAndShowBetAnimation(playerObject, "" + betAmount));
                }
                else
                {
#if ERROR_LOG
                    Debug.LogError("Null Reference exception found playerScript is null in BetDatFound Method = " + userId);
#endif
                }
            }
        }
        
    }

    string handtype;
    public void OnRoundDataFound(string serverResponse)
    {
        //UnityEngine.Debug.LogWarning("Round Data :- " + serverResponse);
        JsonData data = JsonMapper.ToObject(serverResponse);
        MATCH_ROUND = (int)float.Parse(data[0]["currentSubRounds"].ToString());
        if (MATCH_ROUND == -1)
            MATCH_ROUND = 1;
        handtype = serverResponse;
        //   Debug.LogError("hand typessss" + handtype);

        //DEV_CODE
        if (!isRecording)
        {
            //StartRecording();
        }

        ShowCommunityCardsAnimation();
    }


    public void OnOpenCardsDataFound(string serverResponse)
    {
        Debug.LogError("Response => OpenCardDataFound : " + serverResponse);
        JsonData data = JsonMapper.ToObject(serverResponse);
        openCards = new CardData[data[0].Count];

        for (int i = 0; i < data[0].Count; i++)
        {
            if (string.IsNullOrEmpty(data[0][i].ToString()))
            {
                openCards[i] = CardsManager.instance.GetEmptyCardData();
            }
            else
            {
                openCards[i] = CardsManager.instance.GetCardData(data[0][i].ToString());
            }
            //openCards[i] = CardsManager.instance.GetCardData(data[0][i].ToString());
        }

        for (int i = 0; i < communityCards.Length; i++)
        {
            if (openCards[i].cardIcon == CardIcon.NONE) { break; }
            communityCards[i].sprite = openCards[i].cardsSprite;
        }
    }



    public void OnGameStartTimeFound(string serverResponse)
    {
        JsonData data = JsonMapper.ToObject(serverResponse);

        int remainingTime = (int)float.Parse(data[0].ToString());
        Debug.Log("Game Start in => " + remainingTime);
     /*   if (remainingTime < 30)
        {*/
            if (remainingTime <= 1)
            {
                InGameUiManager.instance.ShowTableMessage("");
         //   InGameUiManager.instance.LoadingImage.SetActive (false);
            }
            else
            {
//InGameUiManager.instance.LoadingImage.SetActive(true);
        //    InGameUiManager.instance.ShowTableMessage("Match will start in " + remainingTime + " sec");
            }
      /*  }
        else
        {
            InGameUiManager.instance.ShowTableMessage("Waiting for opponent");
        }*/
    }

    private bool resetGame = false;

    float currCountdownValue;

    public IEnumerator StartWaitingCountdown(int countdownValue = 4)
    {
        int counter = countdownValue;
        while (counter > 0)
        {
            yield return new WaitForSeconds(1);
            counter--;
        }
        resetGame = false;
        GlobalGameManager.instance.LoadScene(Scenes.InGame);
    }

    private bool CallOnce = true, isMyTurn;

    public void OnPlayerObjectFound(string serverResponse)
    {
        if(serverResponse == "[[]]")
        {
            //clear UI
            StartCoroutine(WaitAndSendLeaveRequest());
            return;
        }
        if (!InGameUiManager.instance.isSelectedWinningBooster)
        {
            //Debug.Log("<color=yellow>isSelectedWinningBooster</color>");
            //SocketController.instance.GetRandomCard();
            //InGameUiManager.instance.isSelectedWinningBooster = true;
        }

        //Debug.Log("IsExitCalled: " + gameExitCalled);
        if (gameExitCalled) 
        {
            //SocketController.instance.ResetConnection();
            return;
        }
        
        //Debug.Log("[OnPlayerObjectFound] " + serverResponse);

        if (serverResponse.Length < 20)
        {
            Debug.LogError("Invalid playerObject response found = " + serverResponse);
            return;
        }

        JsonData data = JsonMapper.ToObject(serverResponse);

        AmISpectator = true;

        if (data[0].Count > 0)
        {
            //AdjustAllPlayersOnTable(data[0].Count);
            //Debug.Log(data[0][0]["currentSessionPoint"].ToString() + " <color=yellow>playRound</color> " + float.Parse(data[0][0]["playRound"].ToString()) / 10);
            thunderPointBar.fillAmount = float.Parse(data[0][0]["playRound"].ToString()) / 10;
            PointEarnedCounter = int.Parse(data[0][0]["currentSessionPoint"].ToString());
            bool isMatchStarted = data[0][0]["isStart"].Equals(true);
            isGameStart = isMatchStarted;
            //Debug.Log("**[OnPlayerObjectFound]" + serverResponse);
            SocketController.instance.SetTableId(data[0][0]["tableId"].ToString());
            InGameUiManager.instance.tableId = data[0][0]["tableId"].ToString();
            
            //Debug.Log("<color=yellow>" + isMatchStarted + "</color>");
            if (data[0].Count == 1)
            {
                //Debug.LogWarning("ONE PLAYER-" + serverResponse);
                SocketController.instance.SetSocketState(SocketState.WaitingForOpponent);
                //if "userData": "" then game has not started
                /*if (data[0][0]["userData"].ToString().Length > 0)
                {
                    resetGame = true;
                    StartCoroutine(StartWaitingCountdown());
                    //request server for playerObject packet here//
                    return;
                }*/
            }
            //Debug.Log(data[0].Count + " <color=yellow>" + isMatchStarted + "</color>, " + SocketController.instance.GetSocketState());
            if (SocketController.instance.GetSocketState() == SocketState.WaitingForOpponent)
            {
                SocketController.instance.SetTableId(data[0][0]["tableId"].ToString());

                //if (isMatchStarted) // Match is started
                {
                    //Debug.Log("isMatchStarted" + isMatchStarted);

                    List<MatchMakingPlayerData> matchMakingPlayerData = new List<MatchMakingPlayerData>();

                    SocketController.instance.SetTableId(data[0][0]["tableId"].ToString());
                    for (int i = 0; i < data[0].Count; i++)
                    {
                        MatchMakingPlayerData playerData = new MatchMakingPlayerData();

                        PlayerManager.instance.GetPlayerGameData().coins = float.Parse(data[0][i]["coins"].ToString());

                        playerData.playerData = new PlayerData();
                        playerData.playerData.userId = data[0][i]["userId"].ToString();

                        playerData.playerData.userName = data[0][i]["userName"].ToString();
                        playerData.playerData.avatarurl = data[0][i]["profileImage"].ToString();
                        playerData.playerData.tableId = data[0][i]["tableId"].ToString();
                        InGameUiManager.instance.tableId = data[0][i]["tableId"].ToString();
                        playerData.playerData.isFold = bool.Parse(data[0][i]["isFold"].ToString());
                        playerData.playerData.isBlock = bool.Parse(data[0][i]["isBlocked"].ToString());
                        playerData.playerData.isStart = bool.Parse(data[0][i]["isStart"].ToString());

                        playerData.playerData.totalBet = float.Parse(data[0][i]["totalBet"].ToString());
                        playerData.playerData.balance = float.Parse(data[0][i]["totalCoins"].ToString());

                        playerData.playerType = data[0][i]["playerType"].ToString();

                        playerData.isTurn = data[0][i]["isTurn"].Equals(true);
                        playerData.playerData.isDealer = data[0][i]["isDealer"].Equals(true);
                        playerData.playerData.isSmallBlind = data[0][i]["smallBlind"].Equals(true);
                        playerData.playerData.isBigBlind = data[0][i]["bigBlind"].Equals(true);

                        playerData.playerData.userVIPCard = data[0][i]["userVIPCard"].ToString();
                        playerData.playerData.cardValidity = data[0][i]["cardValidity"].ToString();
                        playerData.playerData.bufferTime = data[0][i]["bufferTime"].ToString();
                        playerData.playerData.seatNo = data[0][i]["seatNo"].ToString();

                        //Debug.LogWarning("buffer Time 0" + data[0][i]["bufferTime"].ToString());

                        if (playerData.isTurn)
                        {
                            playerData.isCheckAvailable = data[0][i]["isCheck"].Equals(true);
                        }

                        playerData.playerData.cards = new CardData[data[0][i]["cards"].Count];

                        
                        for (int j = 0; j < playerData.playerData.cards.Length; j++)
                        {
                            if (playerData == null)
                            {
#if ERROR_LOG
                                Debug.LogError("matchmaking object is null");
#endif
                            }

                            if (playerData.playerData.cards == null)
                            {
#if ERROR_LOG
                                Debug.LogError("cards is null");
#endif
                            }

                            playerData.playerData.cards[j] = CardsManager.instance.GetCardData(data[0][i]["cards"][j].ToString());                            
                        }

                        matchMakingPlayerData.Add(playerData);
                    }

                    Init(matchMakingPlayerData);
                    for (int z = 0; z < AllSeatButtons.Count; z++)
                    {
                        AllSeatButtons[z].SetActive(true);
                        AllSeatButtons[z].GetComponent<PlayerSeat>().DisableButtonClick();
                    }
                }
                //else
                {

                }
            }
            else if (SocketController.instance.GetSocketState() == SocketState.Game_Running)
            {
                PlayerScript playerWhosTurn = null;
                bool isCheckAvailable = false;
                for (int i = 0; i < data[0].Count; i++)
                {
                    PlayerScript playerObject = GetPlayerObject(data[0][i]["userId"].ToString());

                    if (playerObject != null)
                    {
                        PlayerData playerData = new PlayerData();
                        //Debug.LogError("************************************************************");
                        playerData.isFold = bool.Parse(data[0][i]["isFold"].ToString());
                        playerData.isBlock = bool.Parse(data[0][i]["isBlocked"].ToString());
                        playerData.isStart = bool.Parse(data[0][i]["isStart"].ToString());
                        playerData.totalBet = float.Parse(data[0][i]["totalBet"].ToString());
                        playerData.balance = float.Parse(data[0][i]["totalCoins"].ToString());

                        playerData.userVIPCard = data[0][i]["userVIPCard"].ToString();
                        playerData.cardValidity = data[0][i]["cardValidity"].ToString();
                        playerData.bufferTime = data[0][i]["bufferTime"].ToString();
                        playerData.seatNo = data[0][i]["seatNo"].ToString();
                        
                        //Debug.LogWarning("buffer Time " + data[0][i]["bufferTime"].ToString());
                        if (data[0][i]["isTurn"].Equals(true))
                        {
                            Debug.LogWarning(data[0][i]["userName"].ToString() + " isTurn is true");
                            playerWhosTurn = playerObject;
                            isCheckAvailable = data[0][i]["isCheck"].Equals(true);
                            isMyTurn = isCheckAvailable;
                        }
                        else
                        {
                            isMyTurn = false;
                            InGameUiManager.instance.ToggleSuggestionButton(false);
                            InGameUiManager.instance.ToggleActionButton(false);
                        }

                        if (data[0][i]["userData"] != null && data[0][i]["userData"].ToString().Length > 0)
                        {
                            //Debug.Log("isBlock " + data[0][i]["isBlocked"]);
                            string playerAction = data[0][i]["userData"]["playerAction"].ToString();
                            int betAmount = (int)float.Parse(data[0][i]["userData"]["betData"].ToString());
                            int roundNo = (int)float.Parse(data[0][i]["userData"]["roundNo"].ToString());
                            playerObject.UpdateDetails(playerData, playerAction, betAmount, roundNo);
                        }
                        else
                        {
                            playerObject.UpdateDetails(playerData,"",0,-1);
                        }
                        //update balance from playerObject
                        if(playerObject.playerData.userId == PlayerManager.instance.GetPlayerGameData().userId)
                        {
                            PlayerManager.instance.GetPlayerGameData().coins = playerObject.playerData.balance;
                            AmISpectator = false;
                            for (int x = 0; x < AllSeatButtons.Count; x++)
                            {
                                AllSeatButtons[x].SetActive(true);
                                AllSeatButtons[x].GetComponent<PlayerSeat>().DisableButtonClick();
                            }
                        }
                    }
                }

                if (playerWhosTurn != null)
                {
                    //Debug.LogWarning("Switching turn");
                    SwitchTurn(playerWhosTurn, isCheckAvailable);
                }
                else
                {
                    InGameUiManager.instance.ToggleSuggestionButton(false);
                    InGameUiManager.instance.ToggleActionButton(false);
                    Debug.Log("Null reference exception found playerWhosTurn is not found");
                }
            }

            //ShowNewPlayersOnTable(data, isMatchStarted);
            for (int i = 0; i < data[0].Count; i++)
            {
                //update balance from playerObject
                if (PlayerManager.instance.GetPlayerGameData().userId == data[0][i]["userId"].ToString())
                {
                    AmISpectator = false;
                    break;
                    //myPlayerSeat = data[0][i]["seatNo"].ToString();

                    //Vector3 position1 = GetSeatObject(myPlayerSeat).transform.position;
                    //Vector3 position2 = GetSeatObject("1").transform.position;

                    //GetSeatObject(myPlayerSeat).transform.position = position2;
                    //GetSeatObject("1").transform.position = position1;
                }
            }
            UpdateSeatClickSettingsAndView();

            //GetAvailableSeats();
        }
    }

    #endregion



    private void ResetMatchData()
    {
        //DEV_CODE
        //Reset highlighted cards
        for (int i = 0; i < communityCards.Length; i++)
        {
            //communityCards[i].color = Color.white;
            communityCards[i].transform.GetChild(0).gameObject.SetActive(false);
            highlightCards[i] = null;
            isHighlightCard = false;
        }

        for (int i = 0; i < onlinePlayersScript.Length; i++)
        {
            onlinePlayersScript[i].ResetRealtimeResult();
            onlinePlayersScript[i].ResetAllData();

            //DEV_CODE 
            //Logic to reset all players highlighted cards to original one.
            Image[] playerCards = onlinePlayersScript[i].GetCardsImage();

            for (int j = 0; j < onlinePlayersScript[i].playerData.cards.Length; j++)
            {
                //playerCards[j].color = Color.white;
                playerCards[j].transform.GetChild(0).gameObject.SetActive(false);
            }
        }

        InGameUiManager.instance.raisePopUp.SetActive(false);
        userWinner = false;
        DontShowCommunityCardAnimation = false;
        //ClearPotAmount();
        //UpdatePot("");        
        isRematchRequestSent = true;

        SocketController.instance.SetSocketState(SocketState.WaitingForOpponent);

        for (int i = 0; i < communityCards.Length; i++)
        {
            communityCards[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < winnersObject.Count; i++)
        {
            Destroy(winnersObject[i]);
        }

        winnersObject.Clear();
        communityCardsAniamtionShowedUpToRound = 0;
        currentRoundTotalBets = 0;
        pot1Amount = 0;
        RabbitButton.SetActive(false);
        ClearPotAmount();
        UpdatePot("");
        //ClearPotAmount();
        lastPlayerAction = "";
        openCards = null;
        LAST_BET_AMOUNT = 0;

        for (int i = 0; i < allPlayersObject.Length; i++)
        {
            allPlayersObject[i].ResetAllData();
            allPlayersObject[i].ToggleCards(false);
        }

        myPlayerObject = null;

        //onlinePlayersScript = null;
        //onlinePlayersScript = new PlayerScript[0];
        for (int i = 0; i < animationLayer.childCount; i++)
        {
            Destroy(animationLayer.GetChild(i).gameObject);
        }
    }

    private void ClearPotAmount()
    {
        PotValues.Clear();
    }


    /*void OnApplicationFocus(bool focus)
      {
          if (!focus)
          {
              Debug.LogError("OnApplicationFocusOnApplicationFocusOnApplicationFocus");
              LoadMainMenu();

          }
      }
      void OnApplicationQuit()
      {

          Debug.LogError("OnApplicationQuitOnApplicationQuitOnApplicationQuit");
          // StartCoroutine(WaitAndSendLeaveRequest());
          LoadMainMenu();
          SocketController.instance.SendLeaveMatchRequest();

      }*/

    void OnApplicationQuit()
    {

        Debug.LogError("OnApplicationQuitOnApplicationQuitOnApplicationQuit");
        SocketController.instance.SendLeaveMatchRequest();
        // StartCoroutine(WaitAndSendLeaveRequest());
//        LoadMainMenu();  
    }

    private void OnDisable()
    {
        thunderPointBar.fillAmount = 0f;
    }

    void SaveScreenshot()
    {
        //Taking Screenshot
        if (!Directory.Exists(Path.Combine(Application.persistentDataPath, "Screenshots")))
            Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "Screenshots"));

        byte[] byteArray = screenshot.EncodeToPNG();
        File.WriteAllBytes(Path.Combine(Application.persistentDataPath , "Screenshots", "Image_" + tableValue + "_" + cardValue + date + "_" + time + ".png"), byteArray);

        Debug.Log("Saved Screenshot successfully...");
        isScreenshotCaptured = false;

        //Delete Extra Files
        DirectoryInfo dirInfo = new DirectoryInfo(Application.persistentDataPath);
        FileInfo[] fileInfo = dirInfo.GetFiles("*.png");
        for (int j = 0; j < fileInfo.Length; j++)
        {
            File.Delete(fileInfo[j].FullName);
        }
    }
}

public class MatchMakingPlayerData
{
    public PlayerData playerData;
    public bool isTurn;
    public bool isCheckAvailable;
    public string playerType;
}

[System.Serializable]
public class TableSeats
{
    public bool status;
    public string message;
    public Seat[] data;
}

[System.Serializable]
public class Seat
{
    public int userId;
    public string seatNo;
}

[System.Serializable]
public class MyBetData
{
    public int userId;
    public int bet;
    public int lastBet;
    public int pot;
    public List<Pot> sidePot = new List<Pot>();
}

[System.Serializable]
public class Pot
{
    public int amount;
}


[System.Serializable]
public class WinnerObject
{
    public int userId;
    public string userName;
    public int winAmount;
    public bool isWin;
    public int betAmount;
    public string name;
}

[System.Serializable]
public class ShowdownSidePot
{
    public int amount;
    public List<WinnerObject> users = new List<WinnerObject>();
    public List<WinnerObject> winners = new List<WinnerObject>();
}

[System.Serializable]
public class AllShowdownSidePots
{
    public List<ShowdownSidePot> sidePot = new List<ShowdownSidePot>();
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using DG.Tweening;
using NatCorder;
using NatCorder.Clocks;
using NatCorder.Inputs;
using UnityEditor;
using System.IO;
using UnityEngine.Networking;
using System;
using BestHTTP.SocketIO;

public class InGameManager : MonoBehaviour
{
    public static InGameManager instance;

    [SerializeField]
    private PlayerScript[] allPlayersObject;

    [SerializeField]
    private Transform[] allPlayerPos;


    [SerializeField]
    private GameObject cardAnimationPrefab,betAnimationPrefab;
    [SerializeField]
    private Transform animationLayer;

    
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
    private float potAmount = 0;

    private bool isRematchRequestSent = false,isTopUpDone = false;
    private float availableBalance = 0;

    public GameObject WinnAnimationpos;

    //DEV_CODE
    Texture2D screenshot;
    public int videoWidth /* = 1280*/;
    public int videoHeight /*= 720*/;
    public bool isRecording = false;
    
    private MP4Recorder recorder;
    private CameraInput cameraInput;

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

    private void Awake()
    {
        instance = this;
        //Debug.Log("Time: " + System.DateTime.Now.Hour + System.DateTime.Now.Minute);
    }

    public Text TableName;

    private void Start()
    {
        //InGameUiManager.instance.ShowTableMessage("Select a seat");

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
        onlinePlayersScript = new PlayerScript[0];

        for (int i = 0; i < allPlayersObject.Length; i++)
        {
            allPlayersObject[i].TogglePlayerUI(false);
            allPlayersObject[i].ResetAllData();
        }
        UnityEngine.Debug.Log("table id is :" + GlobalGameManager.instance.GetRoomData().socketTableId);
        TableName.text = GlobalGameManager.instance.GetRoomData().title;
        AdjustAllPlayersOnTable(GlobalGameManager.instance.GetRoomData().players);
    }

    public void GetAvailableSeats()
    {
        string req = "{\"tableId\":\"" + GlobalGameManager.instance.GetRoomData().socketTableId + "\"}";
        Debug.LogError("Sending get available seats :" + req);
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
        Debug.LogError("Seats available 0:" + serverResponse);

        if (errorMessage.Length > 0)
        {
            if (isShowErrorMessage)
            {
                InGameUiManager.instance.ShowMessage(errorMessage);
            }
            return;
        }

        if (requestType == RequestType.GetSeatObject)
        {
            Debug.LogError("Seats available 1:" + serverResponse);
            AllSeats = JsonUtility.FromJson<TableSeats>(serverResponse);
            UpdateSeatClickSettingsAndView();
        }
    }

    private void UpdateSeatClickSettingsAndView()
    {
        Vector3 position1 = GetSeatObject(myPlayerSeat).transform.position;
        Vector3 position2 = GetSeatObject("1").transform.position;

        GetSeatObject(myPlayerSeat).transform.position = position2;
        GetSeatObject("1").transform.position = position1;
        foreach (GameObject g in AllSeatButtons)
        {
            g.SetActive(false);            
        }
        for (int i = 0; i < AllSeats.data.Length; i++)
        {
            AllSeatButtons[i].SetActive(true);
            AllSeatButtons[i].GetComponent<PlayerSeat>().UpdateState();            
        }
        GetSeatObject(myPlayerSeat).SetActive(false);
    }

    private void Init(List<MatchMakingPlayerData> matchMakingPlayerData)
    {
        isRematchRequestSent = false;
        matchMakingPlayerData = ReArrangePlayersList(matchMakingPlayerData);
        onlinePlayersScript = new PlayerScript[matchMakingPlayerData.Count];
        PlayerScript playerScriptWhosTurn = null;

        for (int i = 0; i < allPlayersObject.Length; i++)
        {
            allPlayersObject[i].ResetAllData();

            if (i < matchMakingPlayerData.Count)
            {
                allPlayersObject[i].TogglePlayerUI(true);

                onlinePlayersScript[i] = allPlayersObject[i];
                onlinePlayersScript[i].Init(matchMakingPlayerData[i]);

                if (matchMakingPlayerData[i].isTurn)
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
        else
        {
#if ERROR_LOG
            Debug.LogError("Null Reference exception found playerId whos turn is not found");
#endif
        }
    }

    private IEnumerator WaitAndShowCardAnimation(PlayerScript[] players, PlayerScript playerScriptWhosTurn)
    {
        if (!GlobalGameManager.IsJoiningPreviousGame)
        {
            List<GameObject> animatedCards = new List<GameObject>();
            for (int i = 0; i < players.Length; i++)
            {
                Image[] playerCards = players[i].GetCardsImage();

/*                Debug.Log("Player Cards: " + playerCards[i].name);*/

                for (int j = 0; j < playerCards.Length; j++)
                {
                    GameObject gm = Instantiate(cardAnimationPrefab, animationLayer) as GameObject;
                    gm.transform.DOMove(playerCards[j].transform.position, GameConstants.CARD_ANIMATION_DURATION);
                    gm.transform.DOScale(playerCards[j].transform.localScale, GameConstants.CARD_ANIMATION_DURATION);
                    gm.transform.DORotateQuaternion(playerCards[j].transform.rotation, GameConstants.CARD_ANIMATION_DURATION);
                    animatedCards.Add(gm);
                    SoundManager.instance.PlaySound(SoundType.CardMove);
                    yield return new WaitForSeconds(0.1f);
                }

                yield return new WaitForSeconds(0.1f);
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
            players[i].ToggleCards(true, players[i].IsMe());
        }

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
        SoundManager.instance.PlaySound(SoundType.TurnSwitch);

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
            else
            {
                Debug.LogWarning("LAST BET AMOUNT 1" + LAST_BET_AMOUNT);
                InGameUiManager.instance.ToggleActionButton(true, currentPlayer, isCheckAvailable, LAST_BET_AMOUNT, GetMyPlayerObject().GetPlayerData().balance);
            }
        }
        else
        {
            InGameUiManager.instance.ToggleActionButton(false);

            if (!GetMyPlayerObject().GetPlayerData().isFold)
            {
                int callAmount = GetLastBetAmount() - (int)GetMyPlayerObject().GetPlayerData().totalBet;
                InGameUiManager.instance.ToggleSuggestionButton(true, isCheckAvailable, callAmount, GetMyPlayerObject().GetPlayerData().balance);
            }
        }

    }




    private List<MatchMakingPlayerData> ReArrangePlayersList(List<MatchMakingPlayerData> matchMakingPlayerData)
    {
        List<MatchMakingPlayerData> updatedList = new List<MatchMakingPlayerData>();

        for (int i = 0; i < matchMakingPlayerData.Count; i++)
        {
            if (matchMakingPlayerData[i].playerData.userId == PlayerManager.instance.GetPlayerGameData().userId)
            {
                int index = i;
                int counter = 0;

                while (counter < matchMakingPlayerData.Count)
                {
                    updatedList.Add(matchMakingPlayerData[index]);

                    ++index;

                    if (index >= matchMakingPlayerData.Count)
                    {
                        index = 0;
                    }

                    ++counter;
                }

                break;
            }
        }


        return updatedList;
    }


    private bool gameExitCalled = false;

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
        yield return new WaitForSeconds(GameConstants.BUFFER_TIME);
        SocketController.instance.ResetConnection();

        GlobalGameManager.instance.LoadScene(Scenes.MainMenu);
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
        List<PlayerData> playerData = new List<PlayerData>();

        for (int i = 0; i < data[0].Count; i++)
        {
            if (GetPlayerObject(data[0][i]["userId"].ToString()) == null) // player not in our list
            {
                PlayerData playerDataObject = new PlayerData();

                playerDataObject.userId = data[0][i]["userId"].ToString();
                playerDataObject.userName = data[0][i]["userName"].ToString();
                playerDataObject.tableId = data[0][i]["tableId"].ToString();
                playerDataObject.balance = float.Parse(data[0][i]["totalCoins"].ToString());
                playerDataObject.avatarurl = data[0][i]["profileImage"].ToString();
                //Debug.LogError("URL     new 2222222 " + playerDataObject.avatarurl);
                if (isMatchStarted)
                {
                    playerDataObject.isFold = data[0][i]["isBlocked"].Equals(true);
                }
                else
                {
                    playerDataObject.isFold = false;
                }

                playerData.Add(playerDataObject);
            }
        }
        
        for (int i = onlinePlayersScript.Length; i < allPlayersObject.Length; i++)
        {
            allPlayersObject[i].TogglePlayerUI(false);
        }
        if (isMatchStarted)
        {
            if (playerData.Count > 0)
            {
                int startIndex = onlinePlayersScript.Length;
                int maxIndex = startIndex + playerData.Count;
                int index = 0;

                for (int i = startIndex; i < maxIndex && i < allPlayersObject.Length; i++)
                {
                    allPlayersObject[i].TogglePlayerUI(true);
                    allPlayersObject[i].ShowDetailsAsNewPlayer(playerData[index]);
                    allPlayersObject[i].ResetRealtimeResult();
                    ++index;
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
                    allPlayersObject[index].TogglePlayerUI(true);
                    allPlayersObject[index].ShowDetailsAsNewPlayer(playerData[i]);
                    allPlayersObject[index].ResetRealtimeResult();
                }

                ++index;
            }
        }
        
        if (isMatchStarted && onlinePlayersScript != null && onlinePlayersScript.Length > 0)
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
        }

        int maxPlayerOnTable = GlobalGameManager.instance.GetRoomData().players;
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
        Debug.Log("Last All in Bet: " + playerScript.GetLocalBetAmount()/*betAmount*/);
        GameObject gm = Instantiate(betAnimationPrefab,animationLayer) as GameObject;
        gm.transform.GetChild(0).GetComponent<Text>().text = playerScript.GetLocalBetAmount().ToString()/*betAmount*/;
        gm.transform.position = playerScript.transform.position;
        Vector3 initialScale = gm.transform.localScale;
        gm.transform.localScale = Vector3.zero;

        gm.transform.DOMove(playerScript.localBg().transform.position,GameConstants.BET_PLACE_ANIMATION_DURATION).SetEase(Ease.OutBack);
        gm.transform.DOScale(initialScale,GameConstants.BET_PLACE_ANIMATION_DURATION).SetEase(Ease.OutBack);
        SoundManager.instance.PlaySound(SoundType.Bet);
        yield return new WaitForSeconds(GameConstants.BET_PLACE_ANIMATION_DURATION);
        Destroy(gm);
    }

    private bool winnerAnimationFound = false;

    private IEnumerator WaitAndShowWinnersAnimation(PlayerScript playerScript, string betAmount,GameObject amount)
    {
        winnerAnimationFound = true;
        yield return new WaitForSeconds(.6f);
        GameObject gm = Instantiate(chipscoine,WinnAnimationpos.transform) as GameObject;
    //    gm.GetComponent<Text>().text = betAmount;
        gm.transform.position = WinnAnimationpos.transform.position;
/*        Vector3 initialScale = gm.transform.localScale;
        gm.transform.localScale = Vector3.zero;*/

        gm.transform.DOMove(playerScript.transform.position, .5f).SetEase(Ease.Linear);
       // gm.transform.DOScale(initialScale, GameConstants.BET_PLACE_ANIMATION_DURATION).SetEase(Ease.OutBack);
        SoundManager.instance.PlaySound(SoundType.Bet);
        yield return new WaitForSeconds(.6f);
        Destroy(gm);
        amount.transform.DOScale(Vector3.one, GameConstants.BET_PLACE_ANIMATION_DURATION).SetEase(Ease.OutBack);
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
        return potAmount;
    }

    private void UpdatePot(string textToShow)
    {
        if (string.IsNullOrEmpty(textToShow))
        {
            Pot.SetActive(false);
        }
        else
        {
            Pot.SetActive(true);
        }
        potText.text = textToShow;
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

        StartCoroutine(WaitAndShowCommunityCardsAnimation());
    }

    private IEnumerator WaitAndShowCommunityCardsAnimation()
    {
        communityCardsAniamtionShowedUpToRound = MATCH_ROUND;
        bool isBetFound = false;
        for (int i = 0; i < onlinePlayersScript.Length; i++)
        {
            if (MATCH_ROUND != 0)
            {
                //Debug.LogError("HT @ " + handtype);
                onlinePlayersScript[i].UpdateRealTimeResult(handtype);
            }
            Text text = onlinePlayersScript[i].GetLocaPot();

            if (text.gameObject.activeInHierarchy && !string.IsNullOrEmpty(text.text))
            {
                isBetFound = true;
                GameObject gm = Instantiate(betAnimationPrefab, animationLayer) as GameObject;

                gm.transform.GetChild(0).GetComponent<Text>().text = text.text;
                gm.transform.DOMove(potText.transform.position, GameConstants.LOCAL_BET_ANIMATION_DURATION).SetEase(Ease.OutBack);
                Destroy(gm,GameConstants.LOCAL_BET_ANIMATION_DURATION + 0.1f);
            }

            onlinePlayersScript[i].UpdateRoundNo(GetMatchRound());
        }

        if (isBetFound)
        {
            SoundManager.instance.PlaySound(SoundType.ChipsCollect);
        }

        UpdatePot("" + (int)potAmount);

        switch (MATCH_ROUND)
        {
            case 1:
                {
                    SoundManager.instance.PlaySound(SoundType.CardMove);

                    for (int i = 0; i < 3; i++)
                    {
                        if (openCards[i].cardIcon == CardIcon.NONE) { break; }
                        communityCards[i].sprite = openCards[i].cardsSprite;
                    }
                    yield return new WaitForSeconds(1f);

                    for (int i = 0; i < 3; i++)
                    {
                        GameObject gm = Instantiate(cardAnimationPrefab,animationLayer) as GameObject;
                        gm.transform.localScale = communityCards[0].transform.localScale;
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

                    SoundManager.instance.PlaySound(SoundType.CardMove);

                    for (int i = 4; i < 5; i++)
                    {
                        GameObject gm = Instantiate(cardAnimationPrefab, animationLayer) as GameObject;

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

                    for (int i = 0; i < communityCards.Length; i++)
                    {
                        if (openCards[i].cardIcon == CardIcon.NONE) { break; }
                        communityCards[i].sprite = openCards[i].cardsSprite;
                        communityCards[i].gameObject.SetActive(true);
                    }
                }
            break;
        }

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


    public void OnPlayerActionCompleted(PlayerAction actionType,int betAmount,string playerAction)
    {
        // GetMyPlayerObject().ResetTurn();
        PlayerTimerReset();

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

    public void OnResultResponseFound(string serverResponse)
    {
        InGameUiManager.instance.ToggleSuggestionButton(false);
        InGameUiManager.instance.ToggleActionButton(false);

        if (winnersObject.Count > 0)
        {
            return;
        }

        Debug.LogError("OnResultSuccessFound :" + serverResponse);

        MATCH_ROUND = 10; // ToShow all cards
        ShowCommunityCardsAnimation();
        InGameUiManager.instance.ToggleActionButton(false);
        InGameUiManager.instance.ToggleSuggestionButton(false);

        //DEV_CODE
        if (!isScreenshotCaptured)
        {
            //Taking Screenshot
            RenderTexture renderTexture = new RenderTexture(videoWidth, videoHeight, 24);
            Camera.main.targetTexture = renderTexture;

            screenshot = new Texture2D(videoWidth, videoHeight, TextureFormat.RGB24, false);
            Camera.main.Render();
            RenderTexture.active = renderTexture;
            Rect rect = new Rect(0, 0, videoWidth, videoHeight);
            screenshot.ReadPixels(rect, 0, 0);

            Camera.main.targetTexture = null;
            RenderTexture.active = null;
            Destroy(renderTexture);
        }
        isScreenshotCaptured = true;

        JsonData data = JsonMapper.ToObject(serverResponse);
        
        if (data[0].Count > 0)
        {
            for (int i = 0; i < data[0][0].Count; i++)
            {
                if (data[0][0][i]["isWin"].Equals(true))
                {
                    PlayerScript winnerPlayer = GetPlayerObject(data[0][0][i]["userId"].ToString());

                    if (winnerPlayer != null)
                    {
                        GameObject gm = Instantiate(winningPrefab, animationLayer) as GameObject;
                        gm.transform.Find("WinBy").GetComponent<Text>().text = data[0][0][i]["name"].ToString();
                        gm.transform.Find("winAmount").GetComponent<Text>().text="+"+data[0][0][i]["winAmount"].ToString(); 
                        if(data[0][0][i]["winAmount"].ToString()=="50000")
                        {
                            SoundManager.instance.PlaySound(SoundType.bigWin);
                        }
                        gm.transform.position = winnerPlayer.gameObject.transform.position;
                        gm.transform.SetParent(winnerPlayer.gameObject.transform.GetChild(0).transform);
                        gm.transform.SetSiblingIndex(0);
                        Vector3 inititalScale = gm.transform.localScale;
                        gm.transform.localScale = Vector3.zero;
                        StartCoroutine(WaitAndShowWinnersAnimation(winnerPlayer,  data[0][0][i]["winAmount"].ToString(), gm));
                       // gm.transform.DOScale(inititalScale, GameConstants.BET_PLACE_ANIMATION_DURATION).SetEase(Ease.OutBack);
                        winnersObject.Add(gm);
                    }
                }
            }
        }
        for (int i = 0; i < onlinePlayersScript.Length; i++)
        {
            onlinePlayersScript[i].ToggleCards(true,true);
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

        for (int i = 0; i < onlinePlayersScript.Length; i++)
        {
            onlinePlayersScript[i].ResetRealtimeResult();
        }
        JsonData data = JsonMapper.ToObject(serverResponse);
        int remainingTime = (int)float.Parse(data[0].ToString());

        if (remainingTime > 1)
        {
           // InGameUiManager.instance.ShowTableMessage("Next Round Will Start In : " + remainingTime);
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
                    SocketController.instance.SendReMatchRequest("No", "0");
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
        //Debug.LogError("OnTurnCountDownFound" + serverResponse);
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


        if (SocketController.instance.GetSocketState() == SocketState.Game_Running)
        {
            JsonData data = JsonMapper.ToObject(serverResponse);

            if (currentPlayer != null)
            {
                int remainingTime = (int)float.Parse(data[0].ToString());
                if (remainingTime == 1)
                {
                    PlayerTimerReset();
                }

                if (currentPlayer.IsMe())
                {
                    int endTime = (int)(GameConstants.TURN_TIME * 0.25f);

                    if (remainingTime == endTime)
                    {
                        SoundManager.instance.PlaySound(SoundType.TurnEnd);
                    }
                    currentPlayer.ShowRemainingTime(remainingTime);
                }
                else if (!currentPlayer.IsMe())
                {
                    currentPlayer.ShowRemainingTime(remainingTime);
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
        Debug.LogError("serverResponse BETDATAFOUND " + serverResponse);
        JsonData data = JsonMapper.ToObject(serverResponse);
        LAST_BET_AMOUNT = (int)float.Parse(data[0]["lastBet"].ToString());
        string userId = data[0]["userId"].ToString();
        potAmount = float.Parse(data[0]["pot"].ToString());


        if (SocketController.instance.GetSocketState() == SocketState.Game_Running)
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

            if (betAmount > 0 && userId != PlayerManager.instance.GetPlayerGameData().userId)
            {
                PlayerScript playerObject = GetPlayerObject(userId);

                if (playerObject != null)
                {
                    Debug.Log("Current Bet Amount : " + betAmount);
                    StartCoroutine(WaitAndShowBetAnimation(playerObject, "" + playerObject.GetLocalBetAmount()));
                    /*StartCoroutine(WaitAndShowBetAnimation(playerObject, "" + betAmount));*/
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
        UnityEngine.Debug.LogWarning("Round Data :- " + serverResponse);
        JsonData data = JsonMapper.ToObject(serverResponse);
        MATCH_ROUND = (int)float.Parse(data[0]["currentSubRounds"].ToString());
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
        Debug.LogWarning("OpenCardDataFound : " + serverResponse);
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

    private bool CallOnce = true;

    public void OnPlayerObjectFound(string serverResponse)
    {
        if (!InGameUiManager.instance.isSelectedWinningBooster)
        {
            SocketController.instance.GetRandomCard();
            //InGameUiManager.instance.isSelectedWinningBooster = true;
        }

        if (gameExitCalled) { return; }
        Debug.Log("**[OnPlayerObjectFound] _ 0" + serverResponse);

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
            bool isMatchStarted = data[0][0]["isStart"].Equals(true);
            //Debug.Log("**[OnPlayerObjectFound]" + serverResponse);
            SocketController.instance.SetTableId(data[0][0]["tableId"].ToString());
            InGameUiManager.instance.tableId = data[0][0]["tableId"].ToString();
            ShowNewPlayersOnTable(data, isMatchStarted);

            if (data[0].Count == 1)
            {
                Debug.LogWarning("ONE PLAYER-" + serverResponse);
                //if "userData": "" then game has not started
                if (data[0][0]["userData"].ToString().Length > 0)
                {
                    resetGame = true;
                    StartCoroutine(StartWaitingCountdown());
                    return;
                }
            }

            if (SocketController.instance.GetSocketState() == SocketState.WaitingForOpponent)
            {
                SocketController.instance.SetTableId(data[0][0]["tableId"].ToString());

                if (isMatchStarted) // Match is started
                {
                    Debug.Log("isMatchStarted" + isMatchStarted);

                    List<MatchMakingPlayerData> matchMakingPlayerData = new List<MatchMakingPlayerData>();

                    SocketController.instance.SetTableId(data[0][0]["tableId"].ToString());
                    for (int i = 0; i < data[0].Count; i++)
                    {
                        MatchMakingPlayerData playerData = new MatchMakingPlayerData();

                        PlayerManager.instance.GetPlayerGameData().coins = float.Parse(data[0][i]["coins"].ToString());

                        playerData.playerData = new PlayerData();
                        playerData.playerData.userId = data[0][i]["userId"].ToString();

                        playerData.playerData.userName = data[0][i]["userName"].ToString();
                        playerData.playerData.tableId = data[0][i]["tableId"].ToString();
                        InGameUiManager.instance.tableId = data[0][i]["tableId"].ToString();
                         playerData.playerData.isFold = data[0][i]["isBlocked"].Equals(true);

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

                        Debug.LogWarning("buffer Time 0" + data[0][i]["bufferTime"].ToString());

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
                }
            }
            else if (SocketController.instance.GetSocketState() == SocketState.Game_Running)
            {
                Debug.Log("Game not started" + isMatchStarted);
                
                PlayerScript playerWhosTurn = null;
                bool isCheckAvailable = false;
                for (int i = 0; i < data[0].Count; i++)
                {
                    PlayerScript playerObject = GetPlayerObject(data[0][i]["userId"].ToString());

                    if (playerObject != null)
                    {
                        PlayerData playerData = new PlayerData();
                        //Debug.LogError("************************************************************");
                        playerData.isFold = data[0][i]["isBlocked"].Equals(true);
                        playerData.totalBet = float.Parse(data[0][i]["totalBet"].ToString());
                        playerData.balance = float.Parse(data[0][i]["totalCoins"].ToString());

                        playerData.userVIPCard = data[0][i]["userVIPCard"].ToString();
                        playerData.cardValidity = data[0][i]["cardValidity"].ToString();
                        playerData.bufferTime = data[0][i]["bufferTime"].ToString();
                        playerData.seatNo = data[0][i]["seatNo"].ToString();
                        
                        Debug.LogWarning("buffer Time " + data[0][i]["bufferTime"].ToString());
                        if (data[0][i]["isTurn"].Equals(true))
                        {
                            Debug.LogWarning("isTurn is true");
                            playerWhosTurn = playerObject;
                            isCheckAvailable = data[0][i]["isCheck"].Equals(true);
                        }
                        else
                        {
                            InGameUiManager.instance.ToggleSuggestionButton(false);
                            InGameUiManager.instance.ToggleActionButton(false);
                        }

                        if (data[0][i]["userData"] != null && data[0][i]["userData"].ToString().Length > 0)
                        {
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
                        }
                    }
                }

                if (playerWhosTurn != null)
                {
                    Debug.LogWarning("Switching turn");
                    SwitchTurn(playerWhosTurn, isCheckAvailable);
                }
                else
                {
                    InGameUiManager.instance.ToggleSuggestionButton(false);
                    InGameUiManager.instance.ToggleActionButton(false);
                    Debug.LogError("Null reference exception found playerWhosTurn is not found");
                }
            }


            for (int i = 0; i < data[0].Count; i++)
            {
                //update balance from playerObject
                if (PlayerManager.instance.GetPlayerGameData().userId == data[0][i]["userId"].ToString())
                {
                    AmISpectator = false;
                    myPlayerSeat = data[0][i]["seatNo"].ToString();

                    Vector3 position1 = GetSeatObject(myPlayerSeat).transform.position;
                    Vector3 position2 = GetSeatObject("1").transform.position;

                    GetSeatObject(myPlayerSeat).transform.position = position2;
                    GetSeatObject("1").transform.position = position1;
                }
            }

            GetAvailableSeats();
        }        
    }

    #endregion



    private void ResetMatchData()
    {
        UpdatePot("");
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
        potAmount = 0;
        lastPlayerAction = "";
        openCards = null;
        LAST_BET_AMOUNT = 0;

        for (int i = 0; i < allPlayersObject.Length; i++)
        {
            //allPlayersObject[i].ResetAllData();
            allPlayersObject[i].ToggleCards(false);
        }

        myPlayerObject = null;

        onlinePlayersScript = null;
        onlinePlayersScript = new PlayerScript[0];
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

    //DEV_CODE
    public void StartRecording()
    {
        isRecording = true;
        var frameRate = 30;

        // Create a recorder
        recorder = new MP4Recorder(videoWidth, videoHeight, frameRate);
        var clock = new RealtimeClock();
        // And use a `CameraInput` to record the main game camera
        cameraInput = new CameraInput(recorder, clock, /*InGameUiManager.instance.cameraObj*/Camera.main);

        tableValue = GlobalGameManager.instance.GetRoomData().smallBlind.ToString() + "_" + GlobalGameManager.instance.GetRoomData().bigBlind.ToString();
        date = System.DateTime.Now.ToString("dd-MM-yyyy");
        time = System.DateTime.Now.Hour + "_" + System.DateTime.Now.Minute + "_";

        Debug.Log("Recording Started !!!");
    }

    public async void StopRecording()
    {
        Debug.Log("Inside Stopped Recording");
        isRecording = false;
        cameraInput.Dispose();
        var path = await recorder.FinishWriting();

        /*balance = GetMyPlayerObject().GetPlayerData().totalBet.ToString();*/

        if (!Directory.Exists(Path.Combine(Application.persistentDataPath, "Videos")))
            Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "Videos"));

        //For PC to move file
#if UNITY_EDITOR
        Debug.Log("Moving video to folder...");
        FileUtil.MoveFileOrDirectory(path, Path.Combine(Application.persistentDataPath, "Videos", "Video_" + tableValue + "_" + cardValue + date + "_" + time + ".mp4"));
        SaveScreenshot();
#elif UNITY_ANDROID
        File.Move(path, Path.Combine(Application.persistentDataPath, "Videos", "Video_" + tableValue + "_" + cardValue + date + "_" + time + ".mp4"));
        SaveScreenshot();
#endif
        Debug.Log("Recording Stopped ...");

        cardValue = "";
        isCardValueSet = false;

        //Delete Extra files
        DirectoryInfo dirInfo = new DirectoryInfo(Application.persistentDataPath);
        FileInfo[] fileInfo = dirInfo.GetFiles("*.mp4");
        for (int j = 0; j < fileInfo.Length; j++)
        {
            File.Delete(fileInfo[j].FullName);
        }
        Debug.Log("Deleted Extra files at persistent Data Path..");
        
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
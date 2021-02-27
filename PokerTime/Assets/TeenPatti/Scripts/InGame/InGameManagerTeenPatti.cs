using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using DG.Tweening;
//using NatCorder;
//using NatCorder.Clocks;
//using NatCorder.Inputs;
using UnityEditor;
using System.IO;
using UnityEngine.Networking;

public class InGameManagerTeenPatti : MonoBehaviour
{
    public static InGameManagerTeenPatti instance;

    [SerializeField]
    private PlayerScriptTeenPatti[] allPlayersObject;

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

    private PlayerScriptTeenPatti[] onlinePlayersScript = null;
    private PlayerScriptTeenPatti myPlayerObject = null,currentPlayer = null;
    private int MATCH_ROUND = 0, LAST_BET_AMOUNT = 0;
    private CardData[] openCards = null;
    private string lastPlayerAction = "";
    private List<GameObject> winnersObject = new List<GameObject>();
    private int communityCardsAniamtionShowedUpToRound = 0;
    private int currentRoundTotalBets = 0;
    public float potAmount = 0;

    private bool isRematchRequestSent = false,isTopUpDone = false;
    private float availableBalance = 0;

    public GameObject WinnAnimationpos;

    //DEV_CODE
    Texture2D screenshot;
    public int videoWidth /* = 1280*/;
    public int videoHeight /*= 720*/;
    public bool isRecording = false;
    
    //private MP4Recorder recorder;
    //private CameraInput cameraInput;

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


    public Text matchWinner;
    public GameObject notifyUser;

    private void Awake()
    {
        instance = this;
        //Debug.Log("Time: " + System.DateTime.Now.Hour + System.DateTime.Now.Minute);
    }


    private void Start()
    {
        gameExitCalled = false;

        //DEV_CODE
        videoHeight = (int)InGameUiManagerTeenPatti.instance.height;
        videoWidth = (int)InGameUiManagerTeenPatti.instance.width;

        for (int i = 0; i < communityCards.Length; i++)
        {
            communityCards[i].gameObject.SetActive(false);
        }

        UpdatePot("");
        Debug.Log("i am here!!!!!!!!!!!!!!!!!");
        Pot.SetActive(false);
        onlinePlayersScript = new PlayerScriptTeenPatti[0];

        for (int i = 0; i < allPlayersObject.Length; i++)
        {
            allPlayersObject[i].TogglePlayerUI(false);
            allPlayersObject[i].ResetAllData();
        }

        AdjustAllPlayersOnTable(GlobalGameManager.instance.GetRoomData().players);
    }


    private void Init(List<MatchMakingPlayerDataTeenPatti> matchMakingPlayerData)
    {


        Debug.Log("Total Users " + matchMakingPlayerData.Count);
        isRematchRequestSent = false;
        List<MatchMakingPlayerDataTeenPatti> newMatchMakingPlayerData = new List<MatchMakingPlayerDataTeenPatti>();
        if (matchMakingPlayerData.Count > 1)
            newMatchMakingPlayerData = ReArrangePlayersList(matchMakingPlayerData);
        else
            newMatchMakingPlayerData = matchMakingPlayerData;
        onlinePlayersScript = new PlayerScriptTeenPatti[newMatchMakingPlayerData.Count];
        PlayerScriptTeenPatti playerScriptWhosTurn = null;
        //Debug.Log("Arranged Users " + newMatchMakingPlayerData.Count);
        for (int i = 0; i < allPlayersObject.Length; i++)
        {
            allPlayersObject[i].ResetAllData();
            if (i < newMatchMakingPlayerData.Count)
            {
                Debug.Log(newMatchMakingPlayerData[i].playerData.userName + " " + newMatchMakingPlayerData[i].isTurn);
                //Debug.Log("Url " + newMatchMakingPlayerData[i].playerData.avatarurl);
                //allPlayersObject[i].seat = (i + 1).ToString();
                allPlayersObject[i].TogglePlayerUI(true, newMatchMakingPlayerData[i].playerData.avatarurl);
                allPlayersObject[i].cardSeenButton.SetActive(true);
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

            if (playerScriptWhosTurn != null)
            {
                StartCoroutine(WaitAndShowCardAnimation(onlinePlayersScript, playerScriptWhosTurn));
            }
            else
            {
#if ERROR_LOG
            //Debug.LogError("Null Reference exception found playerId whos turn is not found");
#endif
            }
        }
    }




   

    private IEnumerator WaitAndShowCardAnimation(PlayerScriptTeenPatti[] players, PlayerScriptTeenPatti playerScriptWhosTurn)
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
                    if (j < playerCards.Length / 3)
                    {
                        SoundManager.instance.PlaySound(SoundType.CardMove);
                    }
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

        SocketControllerTeenPatti.instance.SetSocketState(SocketStateTeenPatti.Game_Running);
        SwitchTurn(playerScriptWhosTurn,false);
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

    private void SwitchTurn(PlayerScriptTeenPatti playerScript,bool isCheckAvailable)
    {
        SoundManager.instance.PlaySound(SoundType.TurnSwitch);

        for (int i = 0; i < onlinePlayersScript.Length; i++)
        {
            onlinePlayersScript[i].ResetTurn();
        }


        currentPlayer = playerScript;
        if (currentPlayer.IsMe())
        {
            //InGameUiManager.instance.ToggleSuggestionButton(false);

            //SuggestionActions selectedSuggestionAction = InGameUiManager.instance.GetSelectedSuggestionAction();
            //InGameUiManager.instance.ResetSuggetionAction();
            InGameUiManagerTeenPatti.instance.ToggleActionButton(true, currentPlayer, isCheckAvailable, LAST_BET_AMOUNT);
            //if (selectedSuggestionAction != SuggestionActions.Null)
            //{
            //    //switch (selectedSuggestionAction)
            //    //{
            //    //    case SuggestionActions.Call:
            //    //    case SuggestionActions.Call_Any:
            //    //        {
            //    //            int callAmount = GetLastBetAmount() - (int)GetMyPlayerObject().GetPlayerData().totalBet;

            //    //            if (callAmount < GetMyPlayerObject().GetPlayerData().balance)
            //    //            {
            //    //                OnPlayerActionCompleted(PlayerAction.Call, callAmount, "Call");
            //    //            }
            //    //            else
            //    //            {
            //    //                InGameUiManager.instance.ToggleActionButton(true, currentPlayer, isCheckAvailable, LAST_BET_AMOUNT);
            //    //            }
            //    //        }
            //    //        break;

            //    //    case SuggestionActions.Check:
            //    //        {
            //    //            OnPlayerActionCompleted(PlayerAction.Check, 0, "Check");
            //    //        }
            //    //        break;

            //    //    case SuggestionActions.Fold:
            //    //        {
            //    //            OnPlayerActionCompleted(PlayerAction.Fold, 0, "Fold");
            //    //        }
            //    //        break;

            //    //    default:
            //    //        {
            //    //            Debug.LogError("Unhandled suggetion type found = "+selectedSuggestionAction);
            //    //        }
            //    //    break;
            //    //}
            //}
            //else
            //{
                
            //}
        }
        else
        {
            InGameUiManagerTeenPatti.instance.ToggleActionButton(false , null , false , 0);

            if (!GetMyPlayerObject().GetPlayerData().isFold)
            {
                int callAmount = GetLastBetAmount() - (int)GetMyPlayerObject().GetPlayerData().totalBet;
               // InGameUiManager.instance.ToggleSuggestionButton(true, isCheckAvailable, callAmount, GetMyPlayerObject().GetPlayerData().balance);
            }
        }
    }




    private List<MatchMakingPlayerDataTeenPatti> ReArrangePlayersList(List<MatchMakingPlayerDataTeenPatti> matchMakingPlayerData)
    {
        List<MatchMakingPlayerDataTeenPatti> updatedList = new List<MatchMakingPlayerDataTeenPatti>();

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
        InGameUiManagerTeenPatti.instance.ShowScreen(InGameScreensTeenPatti.Loading);
        StartCoroutine(WaitAndSendLeaveRequest());
    }



    private IEnumerator WaitAndSendLeaveRequest()
    {
        //Debug.LogError("WaitAndSendLeaveRequest");
        yield return new WaitForEndOfFrame();
        SocketControllerTeenPatti.instance.SendLeaveMatchRequest();
        yield return new WaitForSeconds(GameConstants.BUFFER_TIME);
        SocketControllerTeenPatti.instance.ResetConnection();
        if(GameConstants.poker)
            GlobalGameManager.instance.LoadScene(Scenes.MainMenu);
        else
            GlobalGameManager.instance.LoadScene(Scenes.MainMenuTeenPatti);
    }

    public PlayerScriptTeenPatti GetMyPlayerObject()
    {
        Debug.Log("My Player Object is: " + PlayerManager.instance.GetPlayerGameData().userId);
        if (myPlayerObject == null)
        {
            myPlayerObject = GetPlayerObject(PlayerManager.instance.GetPlayerGameData().userId);
        }

        return myPlayerObject;
    }


    public PlayerScriptTeenPatti GetPlayerObject(string userId)
    {
        //if (onlinePlayersScript == null)
        //{
        //    return null;
        //}

        for (int i = 0; i < onlinePlayersScript.Length; i++)
        {
            if (onlinePlayersScript[i].GetPlayerData().userId == userId)
            {
                return onlinePlayersScript[i];
            }
        }

        return null;
    }

    public PlayerScriptTeenPatti[] GetAllPlayers()
    {
        return onlinePlayersScript;
    }




    private void ShowNewPlayersOnTable(JsonData data, bool isMatchStarted)
    {
        List<PlayerDataTeenPatti> playerData = new List<PlayerDataTeenPatti>();

        for (int i = 0; i < data.Count; i++)
        {
            if (GetPlayerObject(data[i]["userId"].ToString()) == null) // player not in our list
            {
                PlayerDataTeenPatti playerDataObject = new PlayerDataTeenPatti();

                playerDataObject.userId = data[i]["userId"].ToString();
                playerDataObject.userName = data[i]["userName"].ToString();
                playerDataObject.tableId = data[i]["tableId"].ToString();
                playerDataObject.balance = float.Parse(data[i]["totalCoins"].ToString());
               // playerDataObject.avatarurl = data[0][i]["profileImage"].ToString();
                //Debug.LogError("URL     new 2222222 " + playerDataObject.avatarurl);
                if (isMatchStarted)
                {
                    playerDataObject.isFold = data[i]["isBlocked"].Equals(true);
                }
                else
                {
                    playerDataObject.isFold = false;
                }

                playerData.Add(playerDataObject);

                Debug.Log("Player added to list with user id: " + playerDataObject.userId);
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
            List<PlayerScriptTeenPatti> leftPlayers = new List<PlayerScriptTeenPatti>();

            for (int i = 0; i < onlinePlayersScript.Length; i++)
            {
                bool isMatchFound = false;

                for (int j = 0; j < data.Count; j++)
                {
                    if (data[j]["userId"].ToString() == onlinePlayersScript[i].GetPlayerData().userId)
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


        for (int i = 0; i < allPlayersObject.Length; i++)
        {
            allPlayersObject[i].ToggleEmptyObject(false);
        }

        int maxPlayerOnTable = GlobalGameManager.instance.GetRoomData().players;

        for (int i = 0; i < maxPlayerOnTable && i < allPlayersObject.Length; i++)
        {
            if (!allPlayersObject[i].IsPlayerObjectActive())
            {

                allPlayersObject[i].ToggleEmptyObject(true);
            }

        }
 /*       if (playerData.Count > 0)
        {
            int startIndex = onlinePlayersScript.Length;
            int maxIndex = startIndex + playerData.Count;
            int index = 0;

            for (int i = startIndex; i < maxIndex && i < allPlayersObject.Length; i++)
            {
                allPlayersObject[i].ShowAvtars_frame_flag(playerData[index].userId);
                ++index;
            }
        }*/
    }



    private void AdjustAllPlayersOnTable(int totalPlayerCount)
    {
        //if (totalPlayerCount <= 4)
        //{
        //    int index = 0;
        //    for (int i = 0; i < totalPlayerCount; i++)
        //    {
        //        allPlayersObject[i].transform.position = allPlayerPos[index].position;
        //        index += 2;
        //    }
        //}
        //else if (totalPlayerCount <= 7)
        //{
        //    int index = 0;

        //    for (int i = 0; i < totalPlayerCount; i++)
        //    {
        //        if (i == 2 || i == 7)
        //        {
        //            ++index;
        //        }

        //        allPlayersObject[i].transform.position = allPlayerPos[index].position;
        //        ++index;
        //    }
        //}


        for (int i = 0; i < totalPlayerCount; i++)
        {
            allPlayersObject[i].gameObject.transform.position = allPlayerPos[i].position;
        }
        //else
        //{
            
        //}
    }



    private IEnumerator WaitAndShowBetAnimation(PlayerScriptTeenPatti playerScript, string betAmount)
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


    private IEnumerator WaitAndShowBetAnimationPot(GameObject playerScript, string betAmount)
    {
        //Debug.Log("Last All in Bet: " + playerScript.GetLocalBetAmount()/*betAmount*/);
        GameObject gm = Instantiate(betAnimationPrefab, animationLayer) as GameObject;
        gm.transform.GetChild(0).GetComponent<Text>().text = betAmount/*betAmount*/;
        gm.transform.position = playerScript.transform.position;
        Vector3 initialScale = gm.transform.localScale;
        gm.transform.localScale = Vector3.zero;

        gm.transform.DOMove(playerScript.transform.position, GameConstants.BET_PLACE_ANIMATION_DURATION).SetEase(Ease.OutBack);
        gm.transform.DOScale(initialScale, GameConstants.BET_PLACE_ANIMATION_DURATION).SetEase(Ease.OutBack);
        SoundManager.instance.PlaySound(SoundType.Bet);
        yield return new WaitForSeconds(GameConstants.BET_PLACE_ANIMATION_DURATION);
        Destroy(gm);
    }

    private bool winnerAnimationFound = false;

    private IEnumerator WaitAndShowWinnersAnimation(PlayerScriptTeenPatti playerScript, string betAmount,GameObject amount)
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

        // Changes in script
        //if (resetGame)
        //{
        //    resetGame = false;
        //    if (GameConstants.poker)
        //    {
        //        //GlobalGameManager.instance.LoadScene(Scenes.InGame);
        //    }
        //    else
        //    {
        //        GlobalGameManager.instance.LoadScene(Scenes.InGameTeenPatti);
        //    }
        //}
       
    }
    public float GetPotAmount()
    {
        return potAmount;
    }

    public void UpdatePot(string textToShow)
    {
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
            UpdatePot("" + (int)potAmount);
            Pot.SetActive(true);

            if (text.gameObject.activeInHierarchy && !string.IsNullOrEmpty(text.text))
            {
                isBetFound = true;
                //GameObject gm = Instantiate(betAnimationPrefab, animationLayer) as GameObject;

                //gm.transform.GetChild(0).GetComponent<Text>().text = text.text;
                //gm.transform.DOMove(potText.transform.position, GameConstants.LOCAL_BET_ANIMATION_DURATION).SetEase(Ease.OutBack);
                //Destroy(gm,GameConstants.LOCAL_BET_ANIMATION_DURATION + 0.1f);

               // StartCoroutine(WaitAndShowBetAnimation(onlinePlayersScript[i], "" + onlinePlayersScript[i].GetLocalBetAmount()));
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
                        communityCards[i].gameObject.SetActive(true);
                        communityCards[i].sprite = openCards[i].cardsSprite;
                    }
                }
            break;

            case 2:
                {
                    for (int i = 0; i < 3; i++)
                    {
                        communityCards[i].gameObject.SetActive(true);
                        communityCards[i].sprite = openCards[i].cardsSprite;
                    }

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
                        communityCards[i].gameObject.SetActive(true);
                        communityCards[i].sprite = openCards[i].cardsSprite;
                    }
                }
            break;

            case 3:
                {
                    for (int i = 0; i < 4; i++)
                    {
                        communityCards[i].gameObject.SetActive(true);
                        communityCards[i].sprite = openCards[i].cardsSprite;
                    }

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
                        communityCards[i].gameObject.SetActive(true);
                        communityCards[i].sprite = openCards[i].cardsSprite;
                    }
                }
            break;

            default:
                {
                    for (int i = 0; i < communityCards.Length; i++)
                    {
                        communityCards[i].gameObject.SetActive(true);
                        communityCards[i].sprite = openCards[i].cardsSprite;
                    }
                }
            break;
        }

        yield return new WaitForSeconds(0.1f);
    }

    public void SendEmoji(string serverResponse)
    {
        InGameUiManagerTeenPatti.instance.OnGetEmoji(serverResponse);
       
    }

    public void CardSeen(string serverResponse)
    {
        InGameUiManagerTeenPatti.instance.OnCardSeen(serverResponse);

    }

    public void PlayerReseat(string serverResponse)
    {
        InGameUiManagerTeenPatti.instance.OnPlayerReseat(serverResponse);

    }

    public void ShowMatch(string serverResponse)
    {
        InGameUiManagerTeenPatti.instance.OnShowMatch(serverResponse);

    }


    public void SideShow(string serverResponse)
    {
        InGameUiManagerTeenPatti.instance.OnSideShow(serverResponse);

    }

    public void SideShowReject(string serverResponse)
    {
        InGameUiManagerTeenPatti.instance.OnSideShowRequestReject(serverResponse);

    }

    public void OnChaalNotify(string serverResponse)
    {
        InGameUiManagerTeenPatti.instance.OnChaalNotification(serverResponse);

    }

    public void OnShowNotify(string serverResponse)
    {
        InGameUiManagerTeenPatti.instance.OnShowNotification(serverResponse);

    }

    public void OnFoldNotify(string serverResponse)
    {
        InGameUiManagerTeenPatti.instance.OnFoldNotification(serverResponse);

    }

    public void PotLimiReached(string serverResponse)
    {
        InGameUiManagerTeenPatti.instance.OnPotLimitReached(serverResponse);

    }

    public void OnSideShowWinner(string serverResponse)
    {

        InGameUiManagerTeenPatti.instance.OnSideShowWinner(serverResponse);

    }

    public void OnSeatObject(string serverResponse)
    {

        InGameUiManagerTeenPatti.instance.OnSeatObjectPlayer(serverResponse);

    }


    public void TipToDealer(string serverResponse)
    {
        Debug.LogError("TipToDealer serverResponse ---*****----> " + serverResponse);
       
    }


    public void StandUpPlayer(string serverResponse)
    {
       //Debug.LogError("standUp serverResponse  " + serverResponse);       
        GetMyPlayerObject().StandUp();
    }

    public void OnClickStandupBtn()
    {
        SocketControllerTeenPatti.instance.SendStandUpdata();
    }

    


    public void OnPlayerActionCompleted(PlayerAction actionType,int betAmount,string playerAction)
    {


        // GetMyPlayerObject().ResetTurn();
        // PlayerTimerReset();
       
        StopCoroutine("CountDownAnimation");

        InGameUiManagerTeenPatti.instance.ToggleActionButton(false , null , false , 0);
        
        if (actionType == PlayerAction.Fold)
        {
            SoundManager.instance.PlaySound(SoundType.Fold);
            SocketControllerTeenPatti.instance.SendFoldRequest(GetMyPlayerObject().GetLocalBetAmount());
        }
        else
        {
            if (actionType == PlayerAction.Check)
            {
                SoundManager.instance.PlaySound(SoundType.Check);
            }

            GetMyPlayerObject().AddIntoLocalBetAmount(betAmount, GetMatchRound());
            SocketControllerTeenPatti.instance.SendBetData(betAmount, GetMyPlayerObject().GetLocalBetAmount(), playerAction, GetMatchRound());
        }

        
    }



    public void ToggleTopUpDone(bool isDone)
    {
        isTopUpDone = isDone;
    }


    #region SocketCallBacks

    int runApi = 0;

    public void OnResultResponseFound(string serverResponse)
    {
        if (winnersObject.Count > 0)
        {
            return;
        }

       // Debug.LogError("OnResultSuccessFound :" + serverResponse);

        MATCH_ROUND = 10; // ToShow all cards
        ShowCommunityCardsAnimation();
        InGameUiManagerTeenPatti.instance.ToggleActionButton(false, null, false, 0);
        //InGameUiManager.instance.ToggleSuggestionButton(false);

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
            if (data[0][0][0]["isWin"].Equals(true))
            {
                for (int i = 0; i < onlinePlayersScript.Length; i++)
                {
                    onlinePlayersScript[i].ToggleLocalPot(false);
                }
                string id = data[0][0][0]["userId"].ToString();

                Debug.Log(id);
                PlayerScriptTeenPatti winnerPlayer = GetPlayerObject(data[0][0][0]["userId"].ToString());
                // show Winner notification
                //Debug.LogError("user Id :" + data[0][0][0]["userId"].ToString());
                matchWinner.text = winnerPlayer.playerData.userName + " wins the game.";
                notifyUser.SetActive(true);

                if (winnerPlayer != null)
                {
                    GameObject gm = Instantiate(winningPrefab, animationLayer) as GameObject;
                    
                    //gm.transform.Find("WinBy").GetComponent<Text>().text = data[0][0][0]["name"].ToString();
                    gm.transform.Find("winAmount").GetComponent<Text>().text = "+" + data[0][0][0]["winAmount"].ToString();
                    if (data[0][0][0]["winAmount"].ToString() == "50000")
                    {
                        SoundManager.instance.PlaySound(SoundType.bigWin);
                    }
                    gm.transform.position = winnerPlayer.gameObject.transform.position;
                    gm.transform.SetParent(winnerPlayer.gameObject.transform.GetChild(0).transform);
                    gm.transform.SetSiblingIndex(0);
                    Vector3 inititalScale = gm.transform.localScale;
                    gm.transform.localScale = Vector3.zero;
                    StartCoroutine(WaitAndShowWinnersAnimation(winnerPlayer, data[0][0][0]["winAmount"].ToString(), gm));
                    // gm.transform.DOScale(inititalScale, GameConstants.BET_PLACE_ANIMATION_DURATION).SetEase(Ease.OutBack);
                    winnersObject.Add(gm);
                }
            }
        }
        for (int i = 0; i < onlinePlayersScript.Length; i++)
        {
            onlinePlayersScript[i].ToggleCards(true, true);
        }
        

        //Debug.LogError("OnResultSuccessFound :" + serverResponse);

        MATCH_ROUND = 10; // ToShow all cards
        //ShowCommunityCardsAnimation();
        InGameUiManagerTeenPatti.instance.ToggleActionButton(false, null, false, 0);
        //InGameUiManager.instance.ToggleSuggestionButton(false);

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

                    if (isTopUpDone || availableBalance >= GlobalGameManager.instance.GetRoomData().minBuyIn)
                    {
                        ToggleTopUpDone(false);
                        SocketControllerTeenPatti.instance.SendReMatchRequest("Yes", "0");
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
                            SocketControllerTeenPatti.instance.SendReMatchRequest("Yes", "0");
                            //send topup request with the below api.. for clarification contact Pradeep - Digital Crew
                            SocketControllerTeenPatti.instance.SendTopUpRequest(balanceToAdd);

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
                                SocketControllerTeenPatti.instance.SendReMatchRequest("Yes", "0");
                            }
                            else
                            {
                                InGameUiManagerTeenPatti.instance.ShowMessage("You don't have enough coins to play, please purchase some coins to continue");
                                // TODO call sit out
                                // TODO show coin purchase screen
                            }
                        }
                    }
                }
                else
                {
                    SocketControllerTeenPatti.instance.SendReMatchRequest("No", "0");
                }
            }
        }
        else
        {
            // InGameUiManager.instance.LoadingImage.SetActive(false);
            InGameUiManagerTeenPatti.instance.ShowTableMessage("");
        }

        ResetMatchData();
    }




    public void OnTurnCountDownFound(string serverResponse)
    {
        if (SocketControllerTeenPatti.instance.GetSocketState() == SocketStateTeenPatti.Game_Running)
        {
            JsonData data = JsonMapper.ToObject(serverResponse);

            if (currentPlayer != null)
            {

                int remainingTime = (int)float.Parse(data[0].ToString());

               

                // Debug.Log("%%%%%%%%%%%%%%%%%%%%%%  remainingTime " + remainingTime);
                if (remainingTime == -9)
                {
                    PlayerTimerReset();
                }

                if (currentPlayer.IsMe())
                {

                    int endTime = (int)(GameConstants.TURN_TIME * 0.25f);

                    //if (remainingTime == endTime)
                    //{
                    //    SoundManager.instance.PlaySound(SoundType.TurnEnd);
                    //}
                    currentPlayer.ShowRemainingTime(remainingTime);
                }

                else if(!currentPlayer.IsMe())
                {
                    //Debug.LogError("Timer runs Here");
                    currentPlayer.ShowRemainingTime(remainingTime);
                }
                //   Debug.Log("^^^^^^^^^^^^^^^^^^^   end time " + remainingTime);

                

            }
            else
            {
                Debug.LogError("Null reference exception found current player object is null");
            }
        }
    }

    public void OnBetDataFound(string serverResponse)
    {
        JsonData data = JsonMapper.ToObject(serverResponse);
        LAST_BET_AMOUNT = (int)float.Parse(data[0]["lastBet"].ToString());
        string userId = data[0]["userId"].ToString();
        potAmount = float.Parse(data[0]["pot"].ToString());


        if (SocketControllerTeenPatti.instance.GetSocketState() == SocketStateTeenPatti.Game_Running)
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

            PlayerScriptTeenPatti playerObject = GetPlayerObject(userId);

            if (playerObject != null)
            {
                UpdatePot("" + (int)potAmount);
                Pot.SetActive(true);
                Debug.Log("Current Bet Amount : " + betAmount);
                //StartCoroutine(WaitAndShowBetAnimation(playerObject, "" + playerObject.GetLocalBetAmount()));
                StartCoroutine(WaitAndShowBetAnimationPot(Pot, "" + playerObject.GetLocalBetAmount()));
                ///StartCoroutine(WaitAndShowBetAnimation(playerObject, "" + playerObject.t));
                /*StartCoroutine(WaitAndShowBetAnimation(playerObject, "" + betAmount));*/
            }
            else
            {
#if ERROR_LOG
                Debug.LogError("Null Reference exception found playerScript is null in BetDatFound Method = " + userId);
#endif
            }
//            if (betAmount > 0 && userId != PlayerManager.instance.GetPlayerGameData().userId)
//            {
//                PlayerScript playerObject = GetPlayerObject(userId);

//                if (playerObject != null)
//                {
//                    Debug.Log("Current Bet Amount : " + betAmount);
//                    StartCoroutine(WaitAndShowBetAnimation(playerObject, "" + playerObject.GetLocalBetAmount()));
//                    /*StartCoroutine(WaitAndShowBetAnimation(playerObject, "" + betAmount));*/
//                }
//                else
//                {
//#if ERROR_LOG
//                    Debug.LogError("Null Reference exception found playerScript is null in BetDatFound Method = " + userId);
//#endif
//                }
//            }
        }
        
    }

    string handtype;
    public void OnRoundDataFound(string serverResponse)
    {
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


    public void OnOpenCardsDataFound(JsonData newData , PlayerScriptTeenPatti player)
    {
        JsonData data = newData;
        // openCards = new CardData[data.Count];

        GameObject cardHolder = player.transform.GetChild(0).transform.GetChild(9).transform.gameObject;

        for (int i = 0; i < data.Count; i++)
        {
            //  openCards[i] = CardsManagerTeenPatti.instance.GetCardData(data[i].ToString());

            cardHolder.transform.GetChild(i).gameObject.GetComponent<Image>().sprite = CardsManagerTeenPatti.instance.GetCardData(data[i].ToString()).cardsSprite;
        }
    }

    public void OnOpenCardsDataFoundShowCard(JsonData newData, PlayerScriptTeenPatti player)
    {
        JsonData data = newData;
        // openCards = new CardData[data.Count];
        GameObject cardPlace = player.transform.GetChild(0).transform.GetChild(9).transform.gameObject;
        cardPlace.SetActive(false);
        //for(int i =0; i < cardPlace.transform.childCount; i++)
        //{
        //    cardPlace.transform.GetChild(i).gameObject.SetActive(false);
        //}

        GameObject cardHolder = player.transform.GetChild(0).transform.GetChild(8).transform.gameObject;
        

        for (int i = 0; i < data.Count; i++)
        {
            //  openCards[i] = CardsManagerTeenPatti.instance.GetCardData(data[i].ToString());
            cardHolder.transform.GetChild(i).gameObject.SetActive(true);
            cardHolder.transform.GetChild(i).gameObject.GetComponent<Image>().sprite = CardsManagerTeenPatti.instance.GetCardData(data[i].ToString()).cardsSprite;
        }
    }





    public void OnGameStartTimeFound(string serverResponse)
    {
        JsonData data = JsonMapper.ToObject(serverResponse);

        int remainingTime = (int)float.Parse(data[0].ToString());
        Debug.Log("Game Start serverResponse => " + serverResponse);
        Debug.Log("Game Start in => " + remainingTime);
        if(remainingTime == 0)
        PlayerScriptTeenPatti.instance.cardSeenButton.SetActive(true);
     /*   if (remainingTime < 30)
        {*/
            if (remainingTime <= 1)
            {
            InGameUiManagerTeenPatti.instance.ShowTableMessage("");
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
        if (GameConstants.poker)
        {
            //GlobalGameManager.instance.LoadScene(Scenes.InGame);
        }
        else
        {
            //GlobalGameManager.instance.LoadScene(Scenes.InGameTeenPatti);
        }
    }


    public void OnPlayerObjectFound(string serverResponse)
    {
        if (gameExitCalled) { return; }
        Debug.Log("**[OnPlayerObjectFound] _ 0" + serverResponse);

        if (serverResponse.Length < 20)
        {
            Debug.LogError("Invalid playerObject response found = " + serverResponse);
            return;
        }

        JsonData data = JsonMapper.ToObject(serverResponse);
        
        if (data[0].Count > 0)
        {
            //Debug.LogError("Data is :" + data[0].ToJson());
            JsonData newData = data[0]["players"];
            //AdjustAllPlayersOnTable(data[0].Count);
            bool isMatchStarted = data[0]["isGameStart"].Equals(true);
            bool isMatchOver = data[0]["isGameOver"].Equals(true);
            potAmount = float.Parse(data[0]["pot"].ToString());
            GameConstants.maxChaal = float.Parse(data[0]["maxChal"].ToString());
            //bool isMatchStarted = true;
            Debug.Log("**[OnPlayerObjectFound]" + serverResponse);

           // ShowNewPlayersOnTable(data, isMatchStarted);

            ShowNewPlayersOnTable(newData, isMatchStarted);

            if (isMatchOver)
            {
                Debug.LogWarning("ONE PLAYER- userData exists");
                ResetMatchData();
                InGameManagerTeenPatti.instance.Pot.SetActive(false);
                ResetAllDataForPlayers();
                matchWinner.text = "";
                notifyUser.SetActive(false);
                InGameUiManagerTeenPatti.instance.ToggleActionButton(false, null, false, 0);
                ShowNewPlayersOnTable(newData, false);
                resetGame = true;
                StartCoroutine(StartWaitingCountdown());
                return;
                Debug.LogWarning("ONE PLAYER-" + serverResponse);
                //if "userData": "" then game has not started
                if (newData[0]["userData"].Keys.Count > 0)
                {
                    
                }
            }

            if (SocketControllerTeenPatti.instance.GetSocketState() == SocketStateTeenPatti.WaitingForOpponent)
            {

                SocketControllerTeenPatti.instance.SetTableId(newData[0]["tableId"].ToString());
                

                if (isMatchStarted) // Match is started
                {
                    Debug.Log("isMatchStarted" + isMatchStarted);
                    

                    List<MatchMakingPlayerDataTeenPatti> matchMakingPlayerData = new List<MatchMakingPlayerDataTeenPatti>();

                    SocketControllerTeenPatti.instance.SetTableId(newData[0]["tableId"].ToString());
                    for (int i = 0; i < newData.Count; i++)
                    {
                        MatchMakingPlayerDataTeenPatti playerData = new MatchMakingPlayerDataTeenPatti();

                        playerData.playerData = new PlayerDataTeenPatti();
                        playerData.playerData.userId = newData[i]["userId"].ToString();
                        playerData.playerData.isBlind = newData[i]["isBlind"].Equals(true);
                        playerData.playerData.isShow = newData[i]["isShow"].Equals(true);
                        playerData.playerData.isSideShow = newData[i]["isSideShow"].Equals(true);
                        playerData.playerData.userName = newData[i]["userName"].ToString();
                        playerData.playerData.tableId = newData[i]["tableId"].ToString();
                        InGameUiManagerTeenPatti.instance.tableId = newData[i]["tableId"].ToString();
                        playerData.playerData.isFold = newData[i]["isBlocked"].Equals(true);

                        playerData.playerData.totalBet = float.Parse(newData[i]["minBet"].ToString());
                        //Debug.LogError("Check User balance is :" + newData[i]["minBet"].ToString());
                        playerData.playerData.playerAllBet = float.Parse(newData[i]["totalBet"].ToString());
                        playerData.playerData.balance = float.Parse(newData[i]["totalCoins"].ToString());

                       // playerData.playerType = data[0][i]["playerType"].ToString();

                        playerData.isTurn = newData[i]["isTurn"].Equals(true);
                        playerData.playerData.isDealer = newData[i]["isDealer"].Equals(true);
                        playerData.playerData.isSmallBlind = newData[i]["smallBlind"].Equals(true);
                        playerData.playerData.isBigBlind = newData[i]["bigBlind"].Equals(true);
                    
                        if (playerData.isTurn)
                        {
                            playerData.isCheckAvailable = newData[i]["isCheck"].Equals(true);
                        }

                       


                        playerData.playerData.cards = new CardData[newData[i]["cards"].Count];

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

                            playerData.playerData.cards[j] = CardsManagerTeenPatti.instance.GetCardData(newData[i]["cards"][j].ToString());                            
                        }

                        matchMakingPlayerData.Add(playerData);
                    }

                    Init(matchMakingPlayerData);
                }
            }
            else if (SocketControllerTeenPatti.instance.GetSocketState() == SocketStateTeenPatti.Game_Running)
            {
                Debug.Log("Game not started" + isMatchStarted);


                PlayerScriptTeenPatti playerWhosTurn = null;
                bool isCheckAvailable = false;

                for (int i = 0; i < newData.Count; i++)
                {
                    PlayerScriptTeenPatti playerObject = GetPlayerObject(newData[i]["userId"].ToString());

                    if (playerObject != null)
                    {
                        Debug.Log("PlayerObject is :" + playerObject);
                        PlayerDataTeenPatti playerData = new PlayerDataTeenPatti();
                        //Debug.LogError("************************************************************");
                        playerObject.playerData.isFold = newData[i]["isBlocked"].Equals(true);
                        playerObject.playerData.totalBet = float.Parse(newData[i]["minBet"].ToString());
                        playerObject.playerData.playerAllBet = float.Parse(newData[i]["totalBet"].ToString());
                        playerObject.playerData.balance = float.Parse(newData[i]["totalCoins"].ToString());
                        playerObject.playerData.isBlind = newData[i]["isBlind"].Equals(true);
                        playerObject.playerData.isShow = newData[i]["isShow"].Equals(true);
                        playerObject.playerData.isSideShow = newData[i]["isSideShow"].Equals(true);

                        if (newData[i]["isTurn"].Equals(true))
                        {
                            playerWhosTurn = playerObject;
                            isCheckAvailable = newData[i]["isCheck"].Equals(true);
                        }


                        if (newData[i]["userData"] != null && newData[i]["userData"].ToString().Length > 0)
                        {
                            string playerAction = newData[i]["userData"]["playerAction"].ToString();
                            int betAmount = (int)float.Parse(newData[i]["userData"]["betData"].ToString());
                            //Debug.LogError("BetAmount od user is :" + betAmount);
                            int roundNo = (int)float.Parse(newData[i]["userData"]["roundNo"].ToString());
                            playerObject.UpdateDetails(playerObject.playerData, playerAction, betAmount, roundNo);
                        }
                        else
                        {
                            playerObject. UpdateDetails(playerObject.playerData, "",0,-1);
                        }
                        //update balance from playerObject
                        if(playerObject.playerData.userId == PlayerManager.instance.GetPlayerGameData().userId)
                        {
                            PlayerManager.instance.GetPlayerGameData().coins = playerObject.playerData.balance;
                        }
                    }
                }

                if (playerWhosTurn != null)
                {
                    Debug.Log("Switching turn");

                    SwitchTurn(playerWhosTurn, isCheckAvailable);
                }
                else
                {
#if ERROR_LOG
                    Debug.LogError("Null reference exception found playerWhosTurn is not found");
#endif
                }
            }
        }        
    }

    #endregion



    private void ResetMatchData()
    {
        UpdatePot("");
        isRematchRequestSent = true;

        SocketControllerTeenPatti.instance.SetSocketState(SocketStateTeenPatti.WaitingForOpponent);

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
        onlinePlayersScript = new PlayerScriptTeenPatti[0];
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

        //Debug.LogError("OnApplicationQuitOnApplicationQuitOnApplicationQuit");
        SocketControllerTeenPatti.instance.SendLeaveMatchRequest();
        // StartCoroutine(WaitAndSendLeaveRequest());
//        LoadMainMenu();  
    }

    //DEV_CODE
//    public void StartRecording()
//    {
//        isRecording = true;
//        var frameRate = 30;

//        // Create a recorder
//        recorder = new MP4Recorder(videoWidth, videoHeight, frameRate);
//        var clock = new RealtimeClock();
//        // And use a `CameraInput` to record the main game camera
//        cameraInput = new CameraInput(recorder, clock, /*InGameUiManager.instance.cameraObj*/Camera.main);

//        tableValue = GlobalGameManager.instance.GetRoomData().smallBlind.ToString() + "_" + GlobalGameManager.instance.GetRoomData().bigBlind.ToString();
//        date = System.DateTime.Now.ToString("dd-MM-yyyy");
//        time = System.DateTime.Now.Hour + "_" + System.DateTime.Now.Minute + "_";

//        Debug.Log("Recording Started !!!");
//    }

//    public async void StopRecording()
//    {
//        Debug.Log("Inside Stopped Recording");
//        isRecording = false;
//        cameraInput.Dispose();
//        var path = await recorder.FinishWriting();

//        /*balance = GetMyPlayerObject().GetPlayerData().totalBet.ToString();*/

//        if (!Directory.Exists(Path.Combine(Application.persistentDataPath, "Videos")))
//            Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "Videos"));

//        //For PC to move file
//#if UNITY_EDITOR
//        Debug.Log("Moving video to folder...");
//        FileUtil.MoveFileOrDirectory(path, Path.Combine(Application.persistentDataPath, "Videos", "Video_" + tableValue + "_" + cardValue + date + "_" + time + ".mp4"));
//        SaveScreenshot();
//#elif UNITY_ANDROID
//        File.Move(path, Path.Combine(Application.persistentDataPath, "Videos", "Video_" + tableValue + "_" + cardValue + date + "_" + time + ".mp4"));
//        SaveScreenshot();
//#endif
//        Debug.Log("Recording Stopped ...");

//        cardValue = "";
//        isCardValueSet = false;

//        //Delete Extra files
//        DirectoryInfo dirInfo = new DirectoryInfo(Application.persistentDataPath);
//        FileInfo[] fileInfo = dirInfo.GetFiles("*.mp4");
//        for (int j = 0; j < fileInfo.Length; j++)
//        {
//            File.Delete(fileInfo[j].FullName);
//        }
//        Debug.Log("Deleted Extra files at persistent Data Path..");
        
//    }

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

public class MatchMakingPlayerDataTeenPatti
{
    public PlayerDataTeenPatti playerData;
    public bool isTurn;
    public bool isCheckAvailable;
    public string playerType;
    
}
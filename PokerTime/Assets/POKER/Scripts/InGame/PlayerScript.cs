using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using LitJson;
using System.Net;
using UnityEngine.Networking;
using System;
using System.IO;

public class PlayerScript : MonoBehaviour
{
    private const string TIMER_ANIMATION_ID = "PlayerTime";

    public static PlayerScript instance;

    [SerializeField]
    public PlayerData playerData;
    public RectTransform fx_holder;
    private Image[] cardsImage;
    public Sprite[] EventSprite;
    public Sprite defultavtar;
    private Image timerBar;
    public Image avtar, frame, flag;
    public GameObject lastActionImage;
    private Text balanceText, lastActionText, userName, localBetPot, RealTimeResulttxt;
    private GameObject foldScreen, parentObject, emptyObject, RealTimeResult, localbetBG;
    private bool isItMe;
    public string otheruserId;
    public string seat;
   
    private int localBetAmount = 0;
    private int localBetRoundNo = 0;

    public void Start()
    {
        instance = this;   
    }

    public void OnServerResponseFound(RequestType requestType, string serverResponse, bool isShowErrorMessage, string errorMessage)
    {
        if (errorMessage.Length > 0)
        {
            if (isShowErrorMessage)
            {
                MainMenuController.instance.ShowMessage(errorMessage);
            }
            return;
        }
        if (requestType == RequestType.GetUserDetails)
        {
            Debug.Log("Response => GetUserDetails: " + serverResponse);
            JsonData data = JsonMapper.ToObject(serverResponse);

            if (data["success"].ToString() == "1")
            {
                //    Debug.Log("Success data send");
                for (int i = 0; i < data["getData"].Count; i++)
                {
                    string av_url = (data["getData"][i]["profileImage"].ToString());
                    string flag_url = (data["getData"][i]["countryFlag"].ToString());
                    //string frame_url = (data["getData"][i]["frameURL"].ToString());
                    //StartCoroutine(loadSpriteImageFromUrl(av_url, avtar));
                    //StartCoroutine(loadSpriteImageFromUrl(flag_url, flag));
                    //StartCoroutine(loadSpriteImageFromUrl(frame_url, frame));
                }
            }
            else
            {
                MainMenuController.instance.ShowMessage(data["message"].ToString());
            }
        }
    }
 
    IEnumerator loadSpriteImageFromUrl(string URL, Image image)
    {
        //Debug.Log("Going To Set User Profile and Flag");
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

            image.sprite = sprite;

        }
    }
    public void Init(MatchMakingPlayerData matchMakingPlayerData)
    {
        localBetRoundNo = 0;
        ToggleLocalPot(false);
        playerData = matchMakingPlayerData.playerData;
        if (playerData.userId == PlayerManager.instance.GetPlayerGameData().userId)
        {
            isItMe = true;
            if (InGameManager.instance != null)
            {
                InGameManager.instance.UpdateAvailableBalance(playerData.balance);
            }
            else if(ClubInGameManager.instance!=null)
            {
                ClubInGameManager.instance.UpdateAvailableBalance(playerData.balance);
            }
            RealTimeResult.SetActive(true);

        }
        else
        {
            RealTimeResult.SetActive(false);
            isItMe = false;
        }

        LoadUI();

        ToggleFoldScreen(playerData.isFold);

        balanceText.text = "" + (int)playerData.balance;
        lastActionImage.SetActive(false);
        lastActionText.text = "";
        timerBar.fillAmount = 0;
        fx_holder.gameObject.SetActive(false);
        //Debug.Log("OTHERE USERNAME  ___   " + playerData.userName);
        if (playerData.userName.Length > 3)
        {
            userName.text = playerData.userName.Substring(0, 4) + "...";
        }
        else
        {
            userName.text = playerData.userName;
        }


        transform.Find("Bg/Dealer").gameObject.SetActive(playerData.isDealer);
        localBetAmount = (int)playerData.totalBet;

        if (playerData.totalBet > 0)
        {
            if (InGameManager.instance != null)
            {
                UpdateLocalPot((int)playerData.totalBet, InGameManager.instance.GetMatchRound());
            }
            else if(ClubInGameManager.instance != null)
            {
                UpdateLocalPot((int)playerData.totalBet, ClubInGameManager.instance.GetMatchRound());
            }
        }
    }
    private void LoadUI()
    {
        if (balanceText == null)
        {
            balanceText = transform.Find("Bg/blance bg/Balance").GetComponent<Text>();
            lastActionText = lastActionImage.transform.GetChild(0).GetComponent<Text>();
            timerBar = transform.Find("Bg/TimerBar").GetComponent<Image>();
            foldScreen = transform.Find("Bg/Fold").gameObject;
            parentObject = transform.Find("Bg").gameObject;
            userName = transform.Find("Bg/NameBg/Name").GetComponent<Text>();
            localbetBG = transform.Find("Bg/Local bet bg").gameObject;
            localBetPot = transform.Find("Bg/Local bet bg/LocalBet").GetComponent<Text>();
            RealTimeResult = transform.Find("Bg/RealTime Result").gameObject;
            RealTimeResulttxt = RealTimeResult.GetComponent<Text>();
            lastActionImage.SetActive(false);

            if (InGameManager.instance != null)
            {
                emptyObject = InGameManager.instance.GetSeatObject(playerData.seatNo); // transform.Find("Empty").gameObject;

            }
            else if (ClubInGameManager.instance != null)
            {
                emptyObject = ClubInGameManager.instance.GetSeatObject(playerData.seatNo); // transform.Find("Empty").gameObject;
            }


            cardsImage = new Image[GameConstants.NUMBER_OF_CARDS_PLAYER_GET_IN_MATCH[(int)GlobalGameManager.instance.GetRoomData().gameMode]];

            fx_holder.gameObject.SetActive(false);
            string parentName = "" + cardsImage.Length + "_Cards";

            for (int i = 0; i < cardsImage.Length; i++)
            {
                cardsImage[i] = transform.Find("Bg/" + parentName + "/" + i).GetComponent<Image>();
            }

            if (cardsImage.Length > 2)
            {
                transform.Find("Bg/2_Cards").gameObject.SetActive(false);
                transform.Find("Bg/4_Cards").gameObject.SetActive(true);
            }
            else
            {
                transform.Find("Bg/2_Cards").gameObject.SetActive(true);
                transform.Find("Bg/4_Cards").gameObject.SetActive(false);
            }
        }
    }

    public void StandUp()
    {
        TogglePlayerUI(false);
    }

    public void TogglePlayerUI(bool isShow, string avatarUrl = null)
    {
        LoadUI();
        parentObject.SetActive(isShow);
        if (avatarUrl != null)
            LoadAvtars_Frame_Flag(avatarUrl);
    }

    public bool IsPlayerObjectActive()
    {
        if (parentObject == null)
        {
            return false;
        }

        return gameObject.activeInHierarchy;
    }


    public void ToggleEmptyObject(bool isShow)
    {
        if (emptyObject == null)
        {
            if (InGameManager.instance != null)
            {
                emptyObject = InGameManager.instance.GetSeatObject(playerData.seatNo); //transform.Find("Empty").gameObject;
            }
            else if (ClubInGameManager.instance != null)
            {
                emptyObject = ClubInGameManager.instance.GetSeatObject(playerData.seatNo); //transform.Find("Empty").gameObject;
            }
        }

        emptyObject.SetActive(isShow);
        if (isShow == true)
        {
            avtar.sprite = defultavtar;
        }
    }

    //public void ShowAvtars_frame_flag(string userId)
    //{
    //    //  Debug.LogError("*****=> user id " + userId);
    //    //      StartCoroutine("CountDownAnimation");
    //    WebServices.instance.SendRequest(RequestType.GetUserDetails, "{\"userId\":\"" + userId + "\"}", true, OnServerResponseFound);
    //}

    public void ShowDetailsAsNewPlayer(PlayerData playerData)
    {
        //    Debug.LogError("Player data "+playerData.userName);

        LoadUI();
        transform.Find("Bg/blance bg/Balance").GetComponent<Text>().text = "" + (int)playerData.balance;
        transform.Find("Bg/NameBg/Name").GetComponent<Text>().text = playerData.userName;
        transform.Find("Bg/Dealer").gameObject.SetActive(false);
        otheruserId = playerData.userId;
        //ShowAvtars_frame_flag(playerData.userId);
        //LoadAvtars_Frame_Flag(playerData.avatarurl);
        timerBar.fillAmount = 0;
        fx_holder.gameObject.SetActive(false);
        lastActionImage.SetActive(false);
        lastActionText.text = "";

        ToggleLocalPot(false);
        ToggleFoldScreen(playerData.isFold);
    }

    private void LoadAvtars_Frame_Flag(string avtar_Url)
    {
        StartCoroutine(loadSpriteImageFromUrl(avtar_Url, avtar));
    }

    public void ToggleFoldScreen(bool isShow)
    {
        LoadUI();
        foldScreen.SetActive(isShow);

        if (isShow)
        {
            //Debug.Log("Going To Fold Screen...");
            //UpdateLastAction("Fold");
            ToggleCards(false);
            ResetTurn();
        }
    }
    public void UpdateRealTimeResult(string result)
    {
        Debug.LogWarning("Success data send " + result);
        JsonData data = JsonMapper.ToObject(result);
        //  [{"currentSubRounds":1.0,"currentRounds":0.0,"handType":[{"userId":64.0,"handType":"Straight"},{"userId":65.0,"handType":"Pair"}]}]
        for (int i = 0; i < data[0]["handType"].Count; i++)
        {
            //Debug.Log("Success data send" + data[0]["handType"][i]["userId"].ToString());
            string userId = (data[0]["handType"][i]["userId"].ToString());
            string handType = (data[0]["handType"][i]["handType"].ToString());
            if (playerData.userId == userId)
            {
                RealTimeResulttxt.text = handType;
            }
        }
    }

    public PlayerData GetPlayerData()
    {
        return playerData;
    }

    public bool IsMe()
    {
        return isItMe;
    }

    public Image[] GetCardsImage()
    {
        return cardsImage;
    }

    public void ResetTurn()
    {
        //Debug.LogError("Stopping Turn");
        avtar.GetComponent<Animator>().SetBool("Play", false);
        fx_holder.gameObject.SetActive(false);
        timerBar.fillAmount = 0;
        if (lastRoutine != null)
        {
            StopCoroutine(lastRoutine);
        }
        else
        {
            //Debug.LogError("lastRoutine is null");
        }
        CountDownTimerRunning = false;
    }

    public void ToggleLocalPot(bool isShow)
    {
        localbetBG.SetActive(isShow);
//        localBetPot.gameObject.SetActive(isShow);
    }

    private void UpdateLocalPot(int amount, int roundNo)
    {
        if (InGameManager.instance != null)
        {
            if (roundNo != InGameManager.instance.GetMatchRound())
            {
                amount = 0;
            }
        }
        else if (ClubInGameManager.instance != null)
        {
            if (roundNo != ClubInGameManager.instance.GetMatchRound())
            {
                amount = 0;
            }
        }


        if (amount > 0)
        {
            ToggleLocalPot(true);
            localBetPot.text = "" + amount;
        }
        else
        {
            ToggleLocalPot(false);
        }
    }
    public GameObject localBg()
    {
        return localbetBG;
    }

    public Text GetLocaPot()
    {
        return localBetPot;
    }

    public float GetTotalBet()
    {
        return playerData.totalBet;
    }
    public void SendUserID()
    {
        if (InGameUiManager.instance != null)
        {
            InGameUiManager.instance.TempUserID = this.playerData.userId;
            InGameUiManager.instance.currentClickedSeatNum = this.playerData.userId;
        }
        else if(ClubInGameUIManager.instance !=null)
        {
            ClubInGameUIManager.instance.TempUserID = this.playerData.userId;
            ClubInGameUIManager.instance.currentClickedSeatNum = this.playerData.userId;
        }
        Debug.LogError("Onclick " + this.playerData.userId);
        
    }
    public void UpdateLastAction(string textToShow)
    {
        //Debug.Log(playerData.userName + "Last Action: " + textToShow + ", " + playerData.isFold + "" + InGameManager.instance.isGameStart);
        if (textToShow == "" || string.IsNullOrEmpty(textToShow))
        {
            lastActionImage.SetActive(false);
        }
        else
        {
            lastActionImage.SetActive(true);
        }
        switch (textToShow)
        {
            case "Call":
                lastActionImage.GetComponent<Image>().sprite = EventSprite[0];
                break;
            case "Check":
                lastActionImage.GetComponent<Image>().sprite = EventSprite[1];
                break;
            case "Bet":
                lastActionImage.GetComponent<Image>().sprite = EventSprite[2];
                break;
            case "Raise":
                lastActionImage.GetComponent<Image>().sprite = EventSprite[3];
                break;
            case "AllIn":
                lastActionImage.GetComponent<Image>().sprite = EventSprite[4];
                break;
            case "Fold":
            case "fold":
                lastActionImage.GetComponent<Image>().sprite = EventSprite[5];
                break;

            default:
                break;
        }
        lastActionImage.GetComponent<Image>().SetNativeSize();
        lastActionText.text = textToShow;
    }

    public bool CountDownTimerRunning = false;

    //public bool PlayedExtraTimeOnce = false;
    //IEnumerator CountDownAnimation(float time)
    //{
    //    Debug.LogError("Starting time : " + time);
    //    float t = 0;
    //    fx_holder.gameObject.SetActive(true);
    //    while (t < time)
    //    {
    //        t += Time.deltaTime;
    //        timerBar.fillAmount = t / time;
    //        fx_holder.rotation = Quaternion.Euler(new Vector3(0, 0, -(timerBar.fillAmount) * 360));
    //        CountDownTimerRunning = true;
    //        yield return null;
    //    }
    //    if (!PlayedExtraTimeOnce)
    //    {
    //        if (!string.IsNullOrEmpty(playerData.userVIPCard))
    //        {
    //            Debug.LogError("playerData.userVIPCard " + playerData.userVIPCard);

    //            int userVIPCard = 0;
    //            int.TryParse(playerData.userVIPCard, out userVIPCard);
    //            if(userVIPCard > 0)
    //            {
    //                int extraTime = 0;
    //                int.TryParse(playerData.bufferTime, out extraTime);
    //                time = extraTime;
    //                Debug.LogError("Starting extra time");
    //                ShowRemainingTime(time);
    //                PlayedExtraTimeOnce = true;
    //            }

    //        }
    //    }
    //    CountDownTimerRunning = false;
    //}

    public void StartPlayerTimer(float playerTimer)
    {
        float t = 0;
        fx_holder.gameObject.SetActive(true);
        while (t <= playerTimer)
        {
            t += Time.deltaTime;
            timerBar.fillAmount = t / playerTimer;
            fx_holder.rotation = Quaternion.Euler(new Vector3(0, 0, -(timerBar.fillAmount) * 360));
        }
    }

    IEnumerator CountDownAnimation(float time, bool isSound)
    {
        //UnityEngine.Debug.LogError("CountDownAnimation timer " + time);
        if (isSound)
            SoundManager.instance.PlaySound(SoundType.TurnSwitch);

        //   if (time == 0) yield break;
        float t = 0;
        fx_holder.gameObject.SetActive(true);
        while (t <= time)
        {
            t += Time.deltaTime;
            timerBar.fillAmount = t / time;
            fx_holder.rotation = Quaternion.Euler(new Vector3(0, 0, -(timerBar.fillAmount) * 360));
            CountDownTimerRunning = true;
            //Debug.Log("Value " + timerBar.fillAmount.ToString("F2"));
            if (isSound && timerBar.fillAmount.ToString("F2") == "0.50")
            {
                //Handheld.Vibrate();
#if UNITY_ANDROID && !UNITY_EDITOR
        Vibration.Vibrate(400);
#endif
            }
            yield return null;
        }
        //avtar.GetComponent<Animator>().SetBool("Play", false);
        CountDownTimerRunning = false;
    }

    //public void ShowRemainingTime(float time)
    //{
    //    StartCoroutine(CountDownAnimation(time));      
    //}

    public void ShowRemainingTime(int remainingTime)
    {
        //UnityEngine.Debug.LogError("ShowRemainingTime = " + remainingTime);
        int extraTime = 0;
        int.TryParse(playerData.bufferTime, out extraTime);
        //Debug.Log("playerData.bufferTime " + playerData.bufferTime);
        int totalTime = GameConstants.TURN_TIME + extraTime;
        //UnityEngine.Debug.LogError(remainingTime + " " + totalTime + " " + GameConstants.TURN_TIME + " " + extraTime);
        //Debug.LogError("extraTime = " + extraTime);
        //Debug.LogError("totalTime = " + totalTime);

        remainingTime = totalTime - remainingTime;      //10  - 30

        //Debug.LogError("Updated RemainingTime = " + remainingTime);
        //UnityEngine.Debug.LogError("Starting timer " + remainingTime + ", " + PrefsManager.GetPlayerData().userId + ", " + playerData.userId);
        if (remainingTime == 0)
        {
            bool playMyTurnSound = false;
            if (PrefsManager.GetPlayerData().userId == playerData.userId)
                playMyTurnSound = true;
    
            lastRoutine = StartCoroutine(CountDownAnimation(GameConstants.TURN_TIME, playMyTurnSound));
            //Time.timeScale = 0;
        }
        if (remainingTime == GameConstants.TURN_TIME)
        {
            timerBar.fillAmount = 0;
            if (lastRoutine != null)
            {
                StopCoroutine(lastRoutine);
            }
            CountDownTimerRunning = false;
            //UnityEngine.Debug.LogError("Starting timer " + extraTime);
            //if (PrefsManager.GetPlayerData().userId == playerData.userId)
            bool playMyTurnSound = true;
            if (PrefsManager.GetPlayerData().userId == playerData.userId)
                playMyTurnSound = false;
            lastRoutine = StartCoroutine(CountDownAnimation(extraTime, playMyTurnSound));
        }

        //if (totalTime > GameConstants.TURN_TIME)
        //{
        //    //vip user, show extra time
        //    lastRoutine = StartCoroutine(CountDownAnimation(GameConstants.TURN_TIME));
        //}
        //else
        //{
        //    //normal user, show standard time
        //    lastRoutine = StartCoroutine(CountDownAnimation(GameConstants.TURN_TIME));
        //}

    }

    Coroutine lastRoutine = null;
    
    /// <summary>
    /// Update details,  usefull when user reconnects
    /// </summary>
    /// <param name="dataToAssign"> updated data</param>
    /// <param name="localBetAmount"> total bet placed in round</param>
    /// <param name="lastPlayerAction">last action taken</param>
    /// <param name="lastActionRoundNo">last round number in which action is taken</param>
    public void UpdateDetails(PlayerData dataToAssign, string lastPlayerAction, int totalBetInThisRound, int lastActionRoundNo)
    {

        playerData.balance = dataToAssign.balance;
        playerData.totalBet = dataToAssign.totalBet;
        playerData.isFold = dataToAssign.isFold;

        playerData.bufferTime = dataToAssign.bufferTime;
        playerData.userVIPCard = dataToAssign.userVIPCard;
        playerData.cardValidity = dataToAssign.cardValidity;
        playerData.seatNo = dataToAssign.seatNo;

        if (IsMe())
        {
            if (InGameManager.instance != null)
            {
                InGameManager.instance.UpdateAvailableBalance(playerData.balance);
            }
            else if(ClubInGameManager.instance!=null)
            {
                if (playerData.balance < 1)
                {
                    ClubInGameManager.instance.ShowEVChopButtons();
                }
                ClubInGameManager.instance.UpdateAvailableBalance(playerData.balance);
            }
        }

        ToggleFoldScreen(playerData.isFold);
        balanceText.text = "" + (int)playerData.balance;

        if (lastActionRoundNo >= 0) // All details found
        {
            UpdateLocalPot(totalBetInThisRound, lastActionRoundNo);

            if (InGameManager.instance != null)
            {
                //Debug.Log(lastActionRoundNo + " " + InGameManager.instance.GetMatchRound());
                if (lastActionRoundNo == InGameManager.instance.GetMatchRound())
                {
                    UpdateLastAction(lastPlayerAction);
                }
                else
                {
                    if (playerData.isFold)
                        UpdateLastAction("fold");
                    else
                        UpdateLastAction("");
                }
            }   
            else if (ClubInGameManager.instance != null)
            {
                if (lastActionRoundNo == ClubInGameManager.instance.GetMatchRound())
                {
                    UpdateLastAction(lastPlayerAction);
                }
                else
                {
                    UpdateLastAction("");
                }
            }

        }
    }


    public void ToggleCards(bool isShow, bool isShowOriginalCards = false)
    {
        if (cardsImage == null || cardsImage.Length <= 0)
        {
            return;
        }

        for (int i = 0; i < cardsImage.Length; i++)
        {
            /*if (i == 0)
                cardsImage[i].transform.localPosition = new Vector3(29.2f, 16.8f, 0f);
            else
                cardsImage[i].transform.localPosition = new Vector3(57.8f, 16.8f, 0f);
            cardsImage[i].transform.localScale = new Vector3(0.63f, 0.63f);
            cardsImage[i].transform.localRotation = Quaternion.Euler(0, 0, 0);*/
            cardsImage[i].gameObject.SetActive(isShow);
        }
        
        if (isShow)
        {
            if (isShowOriginalCards)
            {
                if (GetPlayerData().cards != null && GetPlayerData().cards.Length > 0)
                {
                    for (int i = 0; i < cardsImage.Length; i++)
                    {
                        cardsImage[i].sprite = playerData.cards[i].cardsSprite;

                        //DEV_CODE
                        //InGameManager.instance.cardValue = InGameManager.instance.cardValue + GetPlayerData().cards[i].cardNumber + GetPlayerData().cards[i].cardIcon.ToString();

                        /*Debug.Log("Current Player Card Data is--->>> : " + InGameManager.instance.cardValue);*/

                        /*Debug.Log("Card Number" + ":  " + i + " is ->>>>>>>>>>  " + GetPlayerData().cards[i].cardNumber);
                        Debug.Log("Card Icon" + ":  " + i + " is ->>>>>>>>>>  " + GetPlayerData().cards[i].cardIcon.ToString());*/


                        /*
                        Sprite[] cardSprites = Resources.LoadAll<Sprite>("cards");

                        CardData data = new CardData();
                        data.cardNumber = playerData.cards[i].cardNumber;
                        data.cardIcon = playerData.cards[i].cardIcon;

                        int totalCardNumbers = Enum.GetNames(typeof(CardNumber)).Length - 1;
                        int totalCardIcons = Enum.GetNames(typeof(CardIcon)).Length - 1;


                        int cardNumber = totalCardNumbers - (int)data.cardNumber; // reverse order
                        int cardIcon = totalCardIcons - (int)data.cardIcon; // reverse order

                        data.cardsSprite = cardSprites[(cardIcon * 13) + cardNumber];*/

                        /*Sprite cardSprite = GetPlayerData().cards[i].cardsSprite;*/
                        /*Texture2D cardTex = data.cardsSprite.texture;
                        var bytes = cardTex.EncodeToPNG();
                        //byte[] cardByte = cardTex.EncodeToPNG();
                        File.WriteAllBytes(Application.persistentDataPath + "/Card" + i + ".png", bytes);
*/
                        //Debug.Log("Cards positions....." + isItMe);
                        if (!isItMe)
                        {
                            Debug.Log("Cards positions2222....." + isItMe);
                            if (i == 0)
                            {
                                cardsImage[i].transform.localPosition = new Vector3(-40, -5);
                                //cardsImage[i].transform.localScale = new Vector3(0.55f, 0.55f);
                                cardsImage[i].transform.localScale = new Vector3(2f, 2f);
                                cardsImage[i].transform.localRotation = Quaternion.Euler(0, 0, 0);
                            }
                            if (i == 1)
                            {
                                cardsImage[i].transform.localPosition = new Vector3(-25, -5);
                                //cardsImage[i].transform.localScale = new Vector3(0.55f, 0.55f);
                                cardsImage[i].transform.localScale = new Vector3(2f, 2f);
                                cardsImage[i].transform.localRotation = Quaternion.Euler(0, 0, 0);
                            }
                        }                        
                    }
                }
            }
            else
            {
                Debug.Log(GetPlayerData().userName + " " + isShow + " " + isItMe);
                for (int i = 0; i < cardsImage.Length; i++)
                {
                    if (!isItMe)
                    {
                        if (i == 0)
                        {
                            cardsImage[i].transform.localPosition = new Vector3(0, 0);
                            cardsImage[i].transform.localScale = new Vector3(1f, 1f);
                        }
                        if (i == 1)
                        {
                            cardsImage[i].transform.localPosition = new Vector3(5, 0);
                            cardsImage[i].transform.localScale = new Vector3(1f, 1f);

                            if (cardsImage[i].transform.localRotation.z == 0)
                                cardsImage[i].transform.localRotation = Quaternion.Euler(0, 0, -4f);
                        }
                    }
                    cardsImage[i].sprite = CardsManager.instance.GetCardBackSideSprite();
                }

            }
        }


       
    }


    public void AddIntoLocalBetAmount(int amountToAdd, int roundNo)
    {
        if (localBetRoundNo == roundNo) // same round is running
        {
            localBetAmount += amountToAdd;
        }
        else // new round is started
        {
            localBetAmount = amountToAdd;
        }

        localBetRoundNo = roundNo;
    }

    public int GetLocalBetAmount()
    {
        return localBetAmount;
    }

    public void ResetRealtimeResult()
    {
        RealTimeResulttxt.text = "";
    }

    public void DisablePot()
    {
        ToggleLocalPot(false);
    }

    public void ResetAllData()
    {
        RealTimeResulttxt.text = "";
        ToggleCards(false);
        ToggleLocalPot(false);
        UpdateLastAction("");
    }

    public void UpdateRoundNo(int roundNo)
    {
        localBetAmount = 0;
        localBetRoundNo = roundNo;
        UpdateLocalPot(0, roundNo);
        UpdateLastAction("");
    }
}

[System.Serializable]
public class PlayerData
{
    public string userId;
    public string userName;
    public string tableId;
    public bool isDealer, isSmallBlind, isBigBlind, isFold, isTurn, isCheckAvailable;
    public float balance, totalBet;
    public CardData[] cards;
    public string avatarurl;
    public string userVIPCard, cardValidity, bufferTime;
    public string seatNo;
}

public class GetData
{
    public string cc;
}
public enum PlayerType
{
    RealPlayer,
    BotPlayer
}
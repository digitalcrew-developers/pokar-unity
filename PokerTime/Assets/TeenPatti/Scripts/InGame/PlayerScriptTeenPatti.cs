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

public class PlayerScriptTeenPatti : MonoBehaviour
{
    private const string TIMER_ANIMATION_ID = "PlayerTime";

    public static PlayerScriptTeenPatti instance;

    [SerializeField]
    public PlayerDataTeenPatti playerData;
    public RectTransform fx_holder;
    private Image[] cardsImage;
    public Sprite[] EventSprite;
    public Sprite defultavtar;
    public Image timerBar;
    public Image avtar, frame, flag;
    public GameObject lastActionImage;
    private Text balanceText, lastActionText, userName, localBetPot, RealTimeResulttxt;
    private GameObject foldScreen, parentObject, emptyObject, RealTimeResult, localbetBG;
    private bool isItMe;

    public string otheruserId;

   
    private int localBetAmount = 0;
    private int localBetRoundNo = 0;

    public GameObject cardHolder;

    public GameObject seendata;

    public GameObject cardSeenButton;

    public GameObject sideShowWinnerAcceptBtn;

    public GameObject tempCardHolder;

    
    

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
            JsonData data = JsonMapper.ToObject(serverResponse);

            if (data["success"].ToString() == "1")
            {
                //    Debug.Log("Success data send");
                for (int i = 0; i < data["getData"].Count; i++)
                {
                    string av_url = (data["getData"][i]["profileImage"].ToString());
                    string flag_url = (data["getData"][i]["countryFlag"].ToString());
                    string frame_url = (data["getData"][i]["frameURL"].ToString());
                    StartCoroutine(loadSpriteImageFromUrl(av_url, avtar));
                    StartCoroutine(loadSpriteImageFromUrl(flag_url, flag));
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
    public void Init(MatchMakingPlayerDataTeenPatti matchMakingPlayerData)
    {
        localBetRoundNo = 0;
        ToggleLocalPot(false);
        playerData = matchMakingPlayerData.playerData;
        if (playerData.userId == PlayerManager.instance.GetPlayerGameData().userId)
        {
            isItMe = true;
            InGameManagerTeenPatti.instance.UpdateAvailableBalance(playerData.balance);
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
        timerBar.gameObject.SetActive(false);
        Debug.Log("OTHERE USERNAME  ___   " + playerData.userName);
        userName.text = playerData.userName.Substring(0, 4) + "...";
        
        

        // transform.Find("Bg/Dealer").gameObject.SetActive(playerData.isDealer);
        localBetAmount = (int)playerData.totalBet;

        if (playerData.totalBet > 0)
        {
            UpdateLocalPot((int)playerData.totalBet, InGameManagerTeenPatti.instance.GetMatchRound());
        }
    }
    private void LoadUI()
    {
        if (balanceText == null)
        {
            transform.Find("Bg/3_Cards").gameObject.SetActive(true);
            transform.Find("Bg/4_Cards").gameObject.SetActive(true);
            for (int i = 0; i < transform.Find("Bg/4_Cards").gameObject.transform.childCount; i++)
            {
                transform.Find("Bg/4_Cards").gameObject.transform.GetChild(i).gameObject.SetActive(false);
            }
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
            emptyObject = transform.Find("Empty").gameObject;
            cardsImage = new Image[GameConstants.NUMBER_OF_CARDS_PLAYER_GET_IN_MATCH_IN_TEEN_PATTI];
            fx_holder.gameObject.SetActive(false);
            cardSeenButton.SetActive(false);
            string parentName = ""+ cardsImage.Length + "_Cards";

            for (int i = 0; i < cardsImage.Length; i++)
            {
                cardsImage[i] = cardHolder.transform.GetChild(i).GetComponent<Image>();
            }
            
            
            
            //if (cardsImage.Length > 3)
            //{
            //    transform.Find("Bg/3_Cards").gameObject.SetActive(false);
            //    transform.Find("Bg/4_Cards").gameObject.SetActive(true);
            //}
            //else
            //{
            //    transform.Find("Bg/3_Cards").gameObject.SetActive(true);
            //    transform.Find("Bg/4_Cards").gameObject.SetActive(false);
            //}


        }
    }

    public void StandUp()
    {
        TogglePlayerUI(false);
    }

    public void OnClickCardSeen()
    {

        SocketControllerTeenPatti.instance.UserSeenCard();
    }

    public void PlayerSeat()
    {
        SocketControllerTeenPatti.instance.UserSitAgain();
    }

    

    public void TogglePlayerUI(bool isShow)
    {

        LoadUI();
        parentObject.SetActive(isShow);

        if (isShow)
        {
            ToggleEmptyObject(false);
        }
    }

    public bool IsPlayerObjectActive()
    {
        if (parentObject == null)
        {
            return false;
        }


        return parentObject.activeInHierarchy;
    }


    public void ToggleEmptyObject(bool isShow)
    {
        if (emptyObject == null)
        {
            emptyObject = transform.Find("Empty").gameObject;
        }

        emptyObject.SetActive(isShow);
        if (isShow == true)
        {
            avtar.sprite = defultavtar;

        }
    }

    public void ShowAvtars_frame_flag(string userId)
    {
        //  Debug.LogError("*****=> user id " + userId);
        //      StartCoroutine("CountDownAnimation");
        WebServices.instance.SendRequest(RequestType.GetUserDetails, "{\"userId\":\"" + userId + "\"}", true, OnServerResponseFound);
    }
    public void ShowDetailsAsNewPlayer(PlayerData playerData)
    {
        //    Debug.LogError("Player data "+playerData.userName);

        LoadUI();
        transform.Find("Bg/blance bg/Balance").GetComponent<Text>().text = "" + (int)playerData.balance;
        transform.Find("Bg/NameBg/Name").GetComponent<Text>().text = playerData.userName;
        transform.Find("Bg/Dealer").gameObject.SetActive(false);
        otheruserId = playerData.userId;
        ShowAvtars_frame_flag(playerData.userId);
        timerBar.fillAmount = 0;
        fx_holder.gameObject.SetActive(false);
        timerBar.gameObject.SetActive(false);
        lastActionImage.SetActive(false);
        lastActionText.text = "";

        ToggleLocalPot(false);
        ToggleFoldScreen(playerData.isFold);
    }


    public void ToggleFoldScreen(bool isShow)
    {

        LoadUI();
        foldScreen.SetActive(isShow);

        if (isShow)
        {
            ToggleCards(false);
            ResetTurn();
        }
    }
    public void UpdateRealTimeResult(string result)
    {
        JsonData data = JsonMapper.ToObject(result);

        Debug.Log("Success data send" + data);
        //  [{"currentSubRounds":1.0,"currentRounds":0.0,"handType":[{"userId":64.0,"handType":"Straight"},{"userId":65.0,"handType":"Pair"}]}]
        for (int i = 0; i < data[0]["handType"].Count; i++)
        {
            Debug.Log("Success data send" + data[0]["handType"][i]["userId"].ToString());
            string userId = (data[0]["handType"][i]["userId"].ToString());
            string handType = (data[0]["handType"][i]["handType"].ToString());
            if (playerData.userId == userId)
            {
                RealTimeResulttxt.text = handType;
            }
        }
    }

    public PlayerDataTeenPatti GetPlayerData()
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
        GameConstants.timerStart = 0;
        fx_holder.gameObject.SetActive(false);
        timerBar.gameObject.SetActive(false);
        timerBar.fillAmount = 0;
        StopCoroutine("CountDownAnimation");
    }

    public void ToggleLocalPot(bool isShow)
    {
        localbetBG.SetActive(isShow);
//        localBetPot.gameObject.SetActive(isShow);
    }

    private void UpdateLocalPot(int amount, int roundNo)
    {
        if (roundNo != InGameManagerTeenPatti.instance.GetMatchRound())
        {
            amount = 0;
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
        InGameUiManagerTeenPatti.instance.TempUserID = otheruserId;
         Debug.LogError("Onclick");
        
    }
    public void UpdateLastAction(string textToShow)
    {
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

            default:
                break;
        }

        lastActionText.text = textToShow;
    }
    IEnumerator CountDownAnimation()
    {
        float t = 0;
        float time = 13;
        fx_holder.gameObject.SetActive(true);
        timerBar.gameObject.SetActive(true);
        while (t < time)
        {
            t += Time.deltaTime;
            timerBar.fillAmount = t / time;
            fx_holder.rotation = Quaternion.Euler(new Vector3(0, 0, -(timerBar.fillAmount) * 360));
            yield return null;
        }
    }

    

    public void ShowRemainingTime(int remainingTime)
    { 
        if (GameConstants.timerStart == 0)
        {
            GameConstants.timerStart++;
            StartCoroutine(CountDownAnimation());
        }
       
    }


    /// <summary>
    /// Update details,  usefull when user reconnects
    /// </summary>
    /// <param name="dataToAssign"> updated data</param>
    /// <param name="localBetAmount"> total bet placed in round</param>
    /// <param name="lastPlayerAction">last action taken</param>
    /// <param name="lastActionRoundNo">last round number in which action is taken</param>
    public void UpdateDetails(PlayerDataTeenPatti dataToAssign, string lastPlayerAction, int totalBetInThisRound, int lastActionRoundNo)
    {

        playerData.balance = dataToAssign.balance;
        playerData.totalBet = dataToAssign.totalBet;
        playerData.isFold = dataToAssign.isFold;
        //playerData.isFold = false;
        if (IsMe())
        {
            InGameManagerTeenPatti.instance.UpdateAvailableBalance(playerData.balance);
        }

        ToggleFoldScreen(playerData.isFold);
        balanceText.text = "" + (int)playerData.balance;

        if (lastActionRoundNo >= 0) // All details found
        {
            UpdateLocalPot(totalBetInThisRound, lastActionRoundNo);

            if (lastActionRoundNo == InGameManagerTeenPatti.instance.GetMatchRound())
            {
                UpdateLastAction(lastPlayerAction);
            }
            else
            {
                UpdateLastAction("");
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
                        cardsImage[i].sprite = CardsManagerTeenPatti.instance.GetCardBackSideSprite();
                        

                        if (!isItMe)
                        {
                            if (i == 0)
                            {
                                cardsImage[i].transform.localPosition = new Vector3(-53, 8);
                                cardsImage[i].transform.localScale = new Vector3(0.55f, 0.55f);
                            }
                            if (i == 1)
                            {
                                cardsImage[i].transform.localPosition = new Vector3(-24, 8);
                                cardsImage[i].transform.localScale = new Vector3(0.55f, 0.55f);
                            }
                        }                        
                    }
                }
            }
            else
            {
                for (int i = 0; i < cardsImage.Length; i++)
                {
                    if (!isItMe)
                    {
                        if (i == 0)
                        {
                            cardsImage[i].transform.localPosition = new Vector3(0, 0);
                            cardsImage[i].transform.localScale = new Vector3(0.35f, 0.35f);
                        }
                        if (i == 1)
                        {
                            cardsImage[i].transform.localPosition = new Vector3(16, 0);
                            cardsImage[i].transform.localScale = new Vector3(0.35f, 0.35f);
                        }
                    }
                    cardsImage[i].sprite = CardsManagerTeenPatti.instance.GetCardBackSideSprite();
                    cardSeenButton.SetActive(true);
                }

            }
        }


        

        if(!isShow)
        {
            cardHolder.SetActive(true);
            for(int i = 0; i < tempCardHolder.transform.childCount; i++)
            {
                tempCardHolder.transform.GetChild(i).gameObject.SetActive(false);
            }
            for (int i = 0; i < cardsImage.Length; i++)
            {
                cardsImage[i].sprite = CardsManagerTeenPatti.instance.cardBackSprite;

                //playerData.cards[i].cardsSprite = CardsManagerTeenPatti.instance.cardBackSprite;

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

                //Sprite cardSprite = GetPlayerData().cards[i].cardsSprite;
                /*Texture2D cardTex = data.cardsSprite.texture;
                var bytes = cardTex.EncodeToPNG();
                //byte[] cardByte = cardTex.EncodeToPNG();
                File.WriteAllBytes(Application.persistentDataPath + "/Card" + i + ".png", bytes);
*/

                //if (!isItMe)
                //{
                //    if (i == 0)
                //    {
                //        cardsImage[i].transform.localPosition = new Vector3(-53, 8);
                //        cardsImage[i].transform.localScale = new Vector3(0.55f, 0.55f);
                //    }
                //    if (i == 1)
                //    {
                //        cardsImage[i].transform.localPosition = new Vector3(-24, 8);
                //        cardsImage[i].transform.localScale = new Vector3(0.55f, 0.55f);
                //    }
                //}
            }
            if (GetPlayerData().cards != null && GetPlayerData().cards.Length > 0)
            {
                
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
public class PlayerDataTeenPatti
{
    public string userId;
    public string userName;
    public string tableId;
    public bool isDealer, isSmallBlind, isBigBlind, isFold, isTurn, isCheckAvailable;
    public float balance, totalBet;
    public CardData[] cards;
    public string avatarurl;
    public bool isBlind;
    public GameObject cardSeen;
    public bool isShow, isSideShow;
}

public class GetDataTeenPatti
{
    public string cc;
}
public enum PlayerTypeTeenPatti
{
    RealPlayer,
    BotPlayer
}
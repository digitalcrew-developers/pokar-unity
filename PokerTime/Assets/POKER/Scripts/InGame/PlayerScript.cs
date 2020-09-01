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

public class PlayerScript : MonoBehaviour
{
    private const string TIMER_ANIMATION_ID = "PlayerTime";


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
    private GameObject foldScreen, parentObject, emptyObject, RealTimeResult;
    private bool isItMe;

    public int otheruserId;


    private int localBetAmount = 0;
    private int localBetRoundNo = 0;

    public void Start()
    {

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
                    StartCoroutine(loadSpriteImageFromUrl(frame_url, frame));
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
    public void Init(MatchMakingPlayerData matchMakingPlayerData)
    {
        localBetRoundNo = 0;
        ToggleLocalPot(false);
        playerData = matchMakingPlayerData.playerData;
        if (playerData.userId == PlayerManager.instance.GetPlayerGameData().userId)
        {
            isItMe = true;
            InGameManager.instance.UpdateAvailableBalance(playerData.balance);
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
        userName.text = playerData.userName.Substring(0, 4) + "...";
        //      Debug.Log("OTHERE USERNAME  ___   " + playerData.userName);
        otheruserId = int.Parse(playerData.userId);
        transform.Find("Bg/Dealer").gameObject.SetActive(playerData.isDealer);
        localBetAmount = (int)playerData.totalBet;

        if (playerData.totalBet > 0)
        {
            UpdateLocalPot((int)playerData.totalBet, InGameManager.instance.GetMatchRound());
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
            localBetPot = transform.Find("Bg/LocalBet").GetComponent<Text>();
            RealTimeResult = transform.Find("Bg/RealTime Result").gameObject;
            RealTimeResulttxt = RealTimeResult.GetComponent<Text>();
            lastActionImage.SetActive(false);
            emptyObject = transform.Find("Empty").gameObject;
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
        ShowAvtars_frame_flag(playerData.userId);
        timerBar.fillAmount = 0;
        fx_holder.gameObject.SetActive(false);
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
        fx_holder.gameObject.SetActive(false);
        timerBar.fillAmount = 0;
        StopCoroutine("CountDownAnimation");
    }

    public void ToggleLocalPot(bool isShow)
    {
        localBetPot.gameObject.SetActive(isShow);
    }

    private void UpdateLocalPot(int amount, int roundNo)
    {
        if (roundNo != InGameManager.instance.GetMatchRound())
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
        InGameUiManager.instance.TempUserID = playerData.userId;
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
        float time = 9;
        fx_holder.gameObject.SetActive(true);
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
        remainingTime = GameConstants.TURN_TIME - remainingTime;
        // Debug.Log("remainingTime     " + remainingTime);
        if (remainingTime == 0)
        {
            StartCoroutine("CountDownAnimation");
        }
    }


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
        if (IsMe())
        {
            InGameManager.instance.UpdateAvailableBalance(playerData.balance);
        }

        ToggleFoldScreen(playerData.isFold);
        balanceText.text = "" + (int)playerData.balance;

        if (lastActionRoundNo >= 0) // All details found
        {
            UpdateLocalPot(totalBetInThisRound, lastActionRoundNo);

            if (lastActionRoundNo == InGameManager.instance.GetMatchRound())
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
                        cardsImage[i].sprite = playerData.cards[i].cardsSprite;

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
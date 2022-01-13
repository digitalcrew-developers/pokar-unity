using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using LitJson;
using DG.Tweening;

public class ClubInGameUIManager : MonoBehaviour
{
    public static ClubInGameUIManager instance;

    [SerializeField]
    private GameObject[] screens; // All screens prefab

    [SerializeField]
    private Transform[] screenLayers; // screen spawn parent

    [SerializeField]
    private GameObject[] actionButtons, suggestionButtons, suggestionButtonsActiveImage; // screen spawn parent

    [SerializeField]
    public GameObject actionButtonParent, raisePopUp, suggestionButtonParent, clubTableOject, lobbyTableObject;

    [SerializeField]
    private Text tableText, tableInfoText, callAmountText, sliderText, suggestionCallText;

    [SerializeField]
    private Slider slider;

    [SerializeField]
    private GameObject chatButton;
    [SerializeField]
    public GameObject LoadingImage;

    private SuggestionActions selectedSuggestionButton = SuggestionActions.Null;


    private float availableCallAmount = 0, selectedRaiseAmount = 0;
    private List<InGameActiveScreens> inGameActiveScreens = new List<InGameActiveScreens>();
    private bool useRaisePotWise = false;
    private int suggestionCallAmount = 0;

    public GameObject ArrowPopUp;
    public Text arrowPopUpText;

    public int emojiContainerVal;
    public Transform EmojiShowTransform;
    public Transform fromEmojiShowTransform;
    public Transform sentTo, sentBy;
    public GameObject[] EmojiPrefabs;

    public GameObject GirlDealerEmoji;
    public GameObject spinWheel;
    public GameObject tipsCoins;
    public Transform spwantipsCoinsPos;
    public GameObject tipsKiss;
    public Transform spwantipsKissPos;
    public string TempUserID;

    public GameObject players;
    public string tableId;

    //DEV_CODE
    public int winnigBoosterAmount;

    public bool isSelectedWinningBooster = false;
    public string winningBoosterCardName = "";

    //DEV_CODE (Created By Nitin)
    public string currentClickedSeatNum = "";

    public Camera cameraObj;
    [HideInInspector]
    public float height, width;
    public GameObject inGamePopUp;

    public List<GameObject> TableImages = new List<GameObject>();
    public Animator actionPanelAnimator;
    public ClubInGameManager clubInGameManager;

    [HideInInspector]
    public Canvas canvas;

    private void Awake()
    {
        instance = this;
        //cameraObj.gameObject.SetActive(false);
        //DEV_CODE
        inGamePopUp.SetActive(false);
        //cameraObj = GameObject.Find("VideoRecordingCamera").GetComponent<Camera>();

        canvas = gameObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = Camera.main;

        height = gameObject.GetComponent<RectTransform>().rect.height;
        width = gameObject.GetComponent<RectTransform>().rect.width;
    }

    private void OnDestroy()
    {
        instance = null;
    }

    private void OnEnable()
    {
        instance = this;    
    }

    public GameObject ClubMainMenu = null;
    private void Start()
    {
        if (GlobalGameManager.instance.GetRoomData().isLobbyRoom)
        {
            clubTableOject.SetActive(false);
            lobbyTableObject.SetActive(true);
        }
        else
        {
            clubTableOject.SetActive(true);
            lobbyTableObject.SetActive(false);
        }

        //To disable main menu scene on club scene load
        if (!GlobalGameManager.instance.creatingNewTable && !GlobalGameManager.instance.GetRoomData().isLobbyRoom)
        {
            ClubMainMenu = GameObject.Find("MainMenuScene(Clone)");
            ClubMainMenu.SetActive(false);
        }


        tableInfoText.text = "Blinds " + GlobalGameManager.instance.GetRoomData().smallBlind + "/" + GlobalGameManager.instance.GetRoomData().bigBlind + " Ante";
        ToggleActionButton(false);
        ToggleSuggestionButton(false);

        int counter = PlayerPrefs.GetInt("TableCount");
        SwitchTables(counter);
    }

    private void SwitchTables(int counter)
    {
        //Debug.LogError("counter is " + counter);
        PlayerPrefs.SetInt("TableCount", counter);

        foreach (GameObject g in TableImages)
        {
            g.SetActive(false);
        }
        TableImages[counter].SetActive(true);
    }

    public void CallDectuct(int val)
    {
        //StartCoroutine(ShowPopUp("You're in!", 1.5f));
        DeductCoinPostServer(val);

        winnigBoosterAmount = val;

        //ClubSocketController.instance.GetRandomCard();
    }


    public void DeductCoinPostServer(int boosterAmount)
    {
        //winningBoosterCardName = cardName;

        string requestData = "{\"userId\":\"" + PlayerManager.instance.GetPlayerGameData().userId + "\"," +
                              "\"amount\":\"" + boosterAmount + "\"," +
                              "\"deductFrom\":\"" + "coins" + "\"," +
                               "\"narration\":\"" + "Wining Booster" + "\"}";
        WebServices.instance.SendRequest(RequestType.deductFromWallet, requestData, true, OnServerResponseFound);
    }
    public void OnServerResponseFound(RequestType requestType, string serverResponse, bool isShowErrorMessage, string errorMessage)
    {

        //Debug.Log("IIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIII");
        //MainMenuController.instance.DestroyScreen(MainMenuScreens.Loading);

        if (errorMessage.Length > 0)
        {
            if (isShowErrorMessage)
            {
                ShowMessage(errorMessage);
            }

            return;
        }
        if (requestType == RequestType.deductFromWallet)
        {
            JsonData data = JsonMapper.ToObject(serverResponse);
            if (data["success"].ToString() == "1")
            {
                isSelectedWinningBooster = true;
                //Debug.Log("You're in!");
                StartCoroutine(ShowPopUp("You're in!", 1.5f));

                //Debug.Log("Table ID:" + tableId);
                //Debug.Log("User ID:" + PlayerManager.instance.GetPlayerGameData().userId);
                //Debug.Log("Reward Amount: " + winnigBoosterAmount);
                //Debug.Log("Card Name: " + winningBoosterCardName);

                ClubSocketController.instance.SendWinningBooster(int.Parse(tableId), winnigBoosterAmount, winningBoosterCardName);
            }
            else
            {
                ShowMessage(data["message"].ToString());
            }
        }

    }

    IEnumerator ShowPopUp(string msg, float delay)
    {
        inGamePopUp.SetActive(true);
        inGamePopUp.transform.GetChild(0).GetComponent<Text>().text = msg;
        yield return new WaitForSeconds(delay);
        inGamePopUp.SetActive(false);
    }


    public void OnSpinWheelArrowBtnClick()
    {
        Debug.Log("IIII  SpinWheelArrowBtnClick");
        SoundManager.instance.PlaySound(SoundType.Click);

        ShowScreen(InGameScreens.SpinWheelScreen);
    }

    public void OnArrowBtnClick()
    {
        if (!isSelectedWinningBooster)
            ArrowPopUp.SetActive(true);
        else
            StartCoroutine(ShowPopUp("You're in! Please wait for the result", 1.5f));
    }
    public void CloseArrowBtnClick()
    {
        ArrowPopUp.SetActive(false);
    }

    public void OnClickEmoji(int val)
    {
        emojiContainerVal = val;
    }

    public void OnClickEmojiTransform(Transform val)
    {
        EmojiShowTransform = val;
        sentTo = val;   //DEV_CODE Added this line as per InGameUiManager
        Debug.Log("I am getting emoji transform   " + val.transform.parent.parent.name);
        if (val.transform.parent.parent.name.Equals("LobbyTable"))
        {
            otherId = 0;
        }
        else
        {
            Debug.Log("I am getting emoji transform 000000  " + val.transform.parent.parent.GetComponent<PlayerScript>().playerData.userId);
            //if ((val.transform.parent.parent.GetComponent<PlayerScript>().playerData.userId) != "")
            //{
            //    otherId = int.Parse(val.transform.parent.parent.GetComponent<PlayerScript>().playerData.userId);
            //}
            //if (InGameUiManager.instance.TempUserID != "")
            //{
            //    otherId = int.Parse(InGameUiManager.instance.TempUserID);
            //}
            otherId = int.Parse(currentClickedSeatNum);
        }

    }


    public void OnSpwanTipCoin()
    {
        SoundManager.instance.PlaySound(SoundType.Tip);

        GameObject g = Instantiate(tipsCoins, spwantipsCoinsPos) as GameObject;
        g.transform.SetParent(spwantipsCoinsPos);

        ClubSocketController.instance.TipToDealer();
        Invoke("OnSpwanKiss", 0.5f);
    }
    public void OnSpwanKiss()
    {
        SoundManager.instance.PlaySound(SoundType.Kiss);

        GameObject g = Instantiate(tipsKiss, spwantipsKissPos) as GameObject;
        g.transform.SetParent(spwantipsKissPos);


    }
    public void OnClickOnButton(string eventName)
    {
        SoundManager.instance.PlaySound(SoundType.Click);

        switch (eventName)
        {
            case "rabbit":
                {
                    //ClubInGameManager.instance.RabbitButton.SetActive(false);
                    clubInGameManager.RabbitButton.SetActive(false);
                    //StartCoroutine(ClubInGameManager.instance.WaitAndShowRabbit());
                    StartCoroutine(clubInGameManager.WaitAndShowRabbit());
                }
                break;
            case "store":
                {
                    ShowScreen(InGameScreens.InGameShop);
                }
                break;


            case "menu":
                {
                    
                    ShowScreen(InGameScreens.Menu);
                    
                }
                break;

            case "missions":
                {
                    ShowScreen(InGameScreens.Missions);
                }
                break;
            case "emojiScreen":
                {
                    //if (ClubInGameManager.instance.AmISpectator)
                    if (clubInGameManager.AmISpectator)
                        return;
                    ShowScreen(InGameScreens.EmojiScreen);
                }
                break;
            case "fold":
                {
                    //PlayerScript player = ClubInGameManager.instance.GetMyPlayerObject();
                    PlayerScript player = clubInGameManager.GetMyPlayerObject();
                    player.ResetTurn();
                    //ClubInGameManager.instance.OnPlayerActionCompleted(PlayerAction.Fold, 0, "Fold");
                    clubInGameManager.OnPlayerActionCompleted(PlayerAction.Fold, 0, "Fold");
                }
                break;

            case "call":
                {
                    //PlayerScript player = ClubInGameManager.instance.GetMyPlayerObject();
                    PlayerScript player = clubInGameManager.GetMyPlayerObject();
                    player.ResetTurn();
                    //ClubInGameManager.instance.OnPlayerActionCompleted(PlayerAction.Call, (int)availableCallAmount, "Call");
                    clubInGameManager.OnPlayerActionCompleted(PlayerAction.Call, (int)availableCallAmount, "Call");
                }
                break;
            case "allin":
                {
                    PlayerScript player = clubInGameManager.GetMyPlayerObject();
                    player.ResetTurn();
                    clubInGameManager.OnPlayerActionCompleted(PlayerAction.AllIn, (int)player.GetPlayerData().balance, "AllIn");
                }
                break;

            case "check":
                {
                    PlayerScript player = clubInGameManager.GetMyPlayerObject();
                    player.ResetTurn();
                    clubInGameManager.OnPlayerActionCompleted(PlayerAction.Check, 0, "Check");
                }
                break;


            case "raiseOpen":
                {
                    PlayerScript player = clubInGameManager.GetMyPlayerObject();

                    if (player != null)
                    {
                        Debug.Log("availableCallAmount..." + availableCallAmount + ", " + player.GetPlayerData().balance + ", " + GlobalGameManager.instance.GetRoomData().smallBlind);
                        if (availableCallAmount < GlobalGameManager.instance.GetRoomData().smallBlind)
                            availableCallAmount = GlobalGameManager.instance.GetRoomData().smallBlind;
                        ToggleRaisePopUp(true, availableCallAmount, player.GetPlayerData().balance, clubInGameManager.GetPotAmount());
                    }
                    else
                    {
#if ERROR_LOG
                        Debug.LogError("Null Reference exception found playerObject is null in InGameUiManager.RaiseOpen");
#endif
                    }
                }
                break;


            case "raiseClose":
                {
                    ToggleRaisePopUp(false);
                }
                break;


            case "raiseDone":
                {
                    if (sliderText.text == "All In")
                    {
                        PlayerScript player = clubInGameManager.GetMyPlayerObject();

                        if (player != null)
                        {
                            player.ResetTurn();
                            clubInGameManager.OnPlayerActionCompleted(PlayerAction.Raise, (int)player.GetPlayerData().balance, "AllIn");
                        }
                        else
                        {
#if ERROR_LOG
                            Debug.LogError("Null Reference exception found playerObject is null in InGameUiManager.RaiseOpen");
#endif
                        }
                    }
                    else
                    {
                        clubInGameManager.OnPlayerActionCompleted(PlayerAction.Raise, (int)selectedRaiseAmount, "Raise");
                    }
                }
                break;

            case "pot":
                {
                    if (useRaisePotWise) // PotWise Calculation
                    {
                        slider.value = clubInGameManager.GetPotAmount();
                        OnSliderValueChange();
                    }
                    else // Call Amount wise calculation
                    {
                        slider.value = availableCallAmount * 4;
                        OnSliderValueChange();
                    }
                }
                break;

            case "halfPot":
                {
                    if (useRaisePotWise) // PotWise Calculation
                    {
                        slider.value = (clubInGameManager.GetPotAmount() / 2f);
                        OnSliderValueChange();
                    }
                    else // Call Amount wise calculation
                    {
                        slider.value = availableCallAmount * 3f;
                        OnSliderValueChange();
                    }
                }
                break;

            case "thirdPot":
                {
                    if (useRaisePotWise) // PotWise Calculation
                    {
                        slider.value = ((clubInGameManager.GetPotAmount() * 2f) / 3f);
                        OnSliderValueChange();
                    }
                    else // Call Amount wise calculation
                    {
                        slider.value = availableCallAmount * 2f;
                        OnSliderValueChange();
                    }
                }
                break;


            case "sCall":
                {

                    if (selectedSuggestionButton == SuggestionActions.Call)
                    {
                        suggestionButtonsActiveImage[(int)selectedSuggestionButton].SetActive(false);
                        selectedSuggestionButton = SuggestionActions.Null;
                    }
                    else
                    {
                        for (int i = 0; i < suggestionButtonsActiveImage.Length; i++)
                        {
                            suggestionButtonsActiveImage[i].gameObject.SetActive(false);
                        }

                        suggestionCallAmount = (int)availableCallAmount;
                        selectedSuggestionButton = SuggestionActions.Call;

                        suggestionButtonsActiveImage[(int)selectedSuggestionButton].SetActive(true);
                    }
                }
                break;


            case "sCallAny":
                {
                    if (selectedSuggestionButton == SuggestionActions.Call_Any)
                    {
                        suggestionButtonsActiveImage[(int)selectedSuggestionButton].SetActive(false);
                        selectedSuggestionButton = SuggestionActions.Null;
                    }
                    else
                    {
                        for (int i = 0; i < suggestionButtonsActiveImage.Length; i++)
                        {
                            suggestionButtonsActiveImage[i].gameObject.SetActive(false);
                        }

                        selectedSuggestionButton = SuggestionActions.Call_Any;
                        suggestionButtonsActiveImage[(int)selectedSuggestionButton].SetActive(true);
                    }
                }
                break;

            case "sCheck":
                {
                    if (selectedSuggestionButton == SuggestionActions.Check)
                    {
                        suggestionButtonsActiveImage[(int)selectedSuggestionButton].SetActive(false);
                        selectedSuggestionButton = SuggestionActions.Null;
                    }
                    else
                    {
                        for (int i = 0; i < suggestionButtonsActiveImage.Length; i++)
                        {
                            suggestionButtonsActiveImage[i].gameObject.SetActive(false);
                        }

                        selectedSuggestionButton = SuggestionActions.Check;
                        suggestionButtonsActiveImage[(int)selectedSuggestionButton].SetActive(true);
                    }


                }
                break;

            case "sFold":
                {
                    if (selectedSuggestionButton == SuggestionActions.Fold)
                    {
                        suggestionButtonsActiveImage[(int)selectedSuggestionButton].SetActive(false);
                        selectedSuggestionButton = SuggestionActions.Null;
                    }
                    else
                    {
                        for (int i = 0; i < suggestionButtonsActiveImage.Length; i++)
                        {
                            suggestionButtonsActiveImage[i].gameObject.SetActive(false);
                        }

                        selectedSuggestionButton = SuggestionActions.Fold;
                        suggestionButtonsActiveImage[(int)selectedSuggestionButton].SetActive(true);
                    }


                }
                break;


            case "handHistory":
                {
                    ShowScreen(InGameScreens.HandHistory);
                }
                break;

            case "realTimeResult":
                {
                    ShowScreen(InGameScreens.RealTimeResult);
                }
                break;
            case "pointEarnopen":
                {
                    ShowScreen(InGameScreens.PointEarnMsg);
                }
                break;

            case "chat":
                {
                    if (ClubSocketController.instance.GetSocketState() == SocketState.Game_Running)
                    {
                        ShowScreen(InGameScreens.Chat);
                    }
                    else
                    {
                        // NativeFunctionalityIntegration.SharedInstance.showToastMessage("Please wait for game");
                    }
                }
                break;

            default:
#if ERROR_LOG
                Debug.LogError("unhdnled eventName found in InGameUiManager = " + eventName);
#endif
                break;
        }
    }

    float sliderVal;
    int calculatedAmount, myTableBalance;
    public void OnSliderValueChange()
    {
        /*if (slider.value >= slider.maxValue)
        {
            sliderText.text = "All In";
            calculatedAmount = myTableBalance;
        }
        else
        {
            Debug.Log(calculatedAmount + "  " + GlobalGameManager.instance.CalculateSliderValue(calculatedAmount));
            if (slider.value > sliderVal)
                calculatedAmount = calculatedAmount + GlobalGameManager.instance.CalculateSliderValue(calculatedAmount);
            else if (slider.value > 0)
                calculatedAmount = calculatedAmount - GlobalGameManager.instance.CalculateSliderValue(calculatedAmount);

            sliderText.text = calculatedAmount.ToString();
        }
        selectedRaiseAmount = calculatedAmount;
        sliderVal = slider.value;*/

        if (slider.value >= slider.maxValue)
        {
            sliderText.text = "All In";
        }
        else
        {
            sliderText.text = "" + (int)slider.value;
        }

        selectedRaiseAmount = slider.value;
    }


    private void ToggleRaisePopUp(bool isShow, float minBet = 0, float maxBet = 0, float potAmount = 0)
    {
        raisePopUp.SetActive(isShow);

        if (isShow)
        {
            slider.minValue = minBet;
            slider.maxValue = maxBet;
            slider.value = minBet;
            /*Debug.Log(minBet + " minBet " + GlobalGameManager.instance.GetRoomData().smallBlind);
            if (minBet >= GlobalGameManager.instance.GetRoomData().smallBlind)
                sliderText.text = GlobalGameManager.instance.GetRoomData().smallBlind.ToString();
            else
                sliderText.text = GlobalGameManager.instance.GetRoomData().smallBlind.ToString();
            slider.value = 0;
            calculatedAmount = int.Parse(sliderText.text);
            myTableBalance = (int)maxBet;
            Debug.Log(calculatedAmount + "  " + myTableBalance);*/
            if (potAmount <= 0)
            {
                useRaisePotWise = false;
            }

            if (useRaisePotWise) // Pot Wise Raise Amount
            {
                raisePopUp.transform.Find("Pot/Text").gameObject.GetComponent<Text>().text = "Pot";
                raisePopUp.transform.Find("HalfPot/Text").gameObject.GetComponent<Text>().text = "1/2 Pot";
                raisePopUp.transform.Find("ThirdPart/Text").gameObject.GetComponent<Text>().text = "2/3 Pot";

                if (maxBet >= potAmount)
                {
                    raisePopUp.transform.Find("Pot").gameObject.SetActive(true);
                    raisePopUp.transform.Find("HalfPot").gameObject.SetActive(true);
                    raisePopUp.transform.Find("ThirdPart").gameObject.SetActive(true);
                }
                else
                {
                    raisePopUp.transform.Find("Pot").gameObject.SetActive(false);

                    if (slider.maxValue >= (potAmount / 2))
                    {
                        raisePopUp.transform.Find("HalfPot").gameObject.SetActive(true);
                        raisePopUp.transform.Find("ThirdPart").gameObject.SetActive(true);
                    }
                    else
                    {
                        raisePopUp.transform.Find("HalfPot").gameObject.SetActive(false);

                        float twoThirdAmount = (potAmount * 2) / 3;

                        if (slider.maxValue >= twoThirdAmount)
                        {
                            raisePopUp.transform.Find("ThirdPart").gameObject.SetActive(true);
                        }
                        else
                        {
                            raisePopUp.transform.Find("ThirdPart").gameObject.SetActive(false);
                        }
                    }
                }
            }
            else // Call Wise Raise Amount
            {

                raisePopUp.transform.Find("Pot/Text").gameObject.GetComponent<Text>().text = "X4";
                raisePopUp.transform.Find("HalfPot/Text").gameObject.GetComponent<Text>().text = "X3";
                raisePopUp.transform.Find("ThirdPart/Text").gameObject.GetComponent<Text>().text = "X2";


                if (maxBet >= (availableCallAmount * 4))
                {
                    raisePopUp.transform.Find("Pot").gameObject.SetActive(true);
                    raisePopUp.transform.Find("HalfPot").gameObject.SetActive(true);
                    raisePopUp.transform.Find("ThirdPart").gameObject.SetActive(true);
                }
                else
                {
                    raisePopUp.transform.Find("Pot").gameObject.SetActive(false);

                    if (slider.maxValue >= (availableCallAmount * 4))
                    {
                        raisePopUp.transform.Find("HalfPot").gameObject.SetActive(true);
                        raisePopUp.transform.Find("ThirdPart").gameObject.SetActive(true);
                    }
                    else
                    {
                        raisePopUp.transform.Find("HalfPot").gameObject.SetActive(false);


                        if (slider.maxValue >= (availableCallAmount * 2))
                        {
                            raisePopUp.transform.Find("ThirdPart").gameObject.SetActive(true);
                        }
                        else
                        {
                            raisePopUp.transform.Find("ThirdPart").gameObject.SetActive(false);
                        }
                    }
                }
            }


            OnSliderValueChange();
        }
    }



    public SuggestionActions GetSelectedSuggestionAction()
    {
        return selectedSuggestionButton;
    }

    public void ResetSuggetionAction()
    {
        selectedSuggestionButton = SuggestionActions.Null;
    }


    public void ToggleSuggestionButton(bool isShow, bool isCheckAvailable = false, int callAmount = 0, float availableBalance = 0)
    {
        //DEV_CODE Added
        if (!clubInGameManager.userWinner)
            suggestionButtonParent.SetActive(isShow);
        else
            suggestionButtonParent.SetActive(false);
        //suggestionButtonParent.SetActive(isShow);

        if (isShow)
        {
            if (callAmount <= 0)
            {
                isCheckAvailable = true;
            }

            for (int i = 0; i < suggestionButtons.Length; i++)
            {
                suggestionButtons[i].SetActive(true);
                suggestionButtonsActiveImage[i].SetActive(false);
            }

            if (isCheckAvailable)
            {
                suggestionButtons[(int)SuggestionActions.Call].SetActive(false);
                suggestionButtons[(int)SuggestionActions.Check].SetActive(true);
            }
            else
            {
                availableCallAmount = callAmount;
                if (callAmount < availableBalance)
                {
                    suggestionButtons[(int)SuggestionActions.Call].SetActive(true);
                    suggestionButtons[(int)SuggestionActions.Check].SetActive(false);
                    if (callAmount != 0)
                    {
                        suggestionCallText.text = "" + callAmount;
                    }
                    else
                    {
                        suggestionCallText.text = "";
                    }
                }
                else
                {
                    for (int i = 0; i < suggestionButtons.Length; i++)
                    {
                        suggestionButtons[i].SetActive(false);
                        suggestionButtonsActiveImage[i].SetActive(false);
                    }

                    suggestionButtons[(int)SuggestionActions.Fold].SetActive(true);
                }
            }


            if (selectedSuggestionButton == SuggestionActions.Call && callAmount != suggestionCallAmount)
            {
                selectedSuggestionButton = SuggestionActions.Null;
            }
            else if (selectedSuggestionButton != SuggestionActions.Null && suggestionButtons[(int)selectedSuggestionButton].activeInHierarchy)
            {
                suggestionButtonsActiveImage[(int)selectedSuggestionButton].SetActive(true);
            }
            else
            {
                selectedSuggestionButton = SuggestionActions.Null;
            }
        }
    }



    public bool IsActionButtonsActive()
    {
        return actionButtonParent.activeInHierarchy;
    }

    public void ToggleActionButton(bool isShow, PlayerScript playerObject = null, bool isCheckAvailable = false, int lastBetAmount = 0, float availableBalance = 0)
    {
        //DEV_CODE Added 
        /*if (isShow)
        {
            ResetSuggetionAction();
            //raisePopUp.SetActive(false);
            int callAmount = lastBetAmount - (int)playerObject.GetPlayerData().totalBet;

            if (callAmount > 0)
            {
                isCheckAvailable = false;
            }

            useRaisePotWise = isCheckAvailable;

            Debug.LogError("isShow " + isShow + " isCheckAvailable " + isCheckAvailable + " call amount  " + callAmount + "  lba  " + lastBetAmount + " availableBalance " + availableBalance + " totalBet " + playerObject.GetPlayerData().totalBet);

            //if (!isCheckAvailable)
            {
                if (callAmount > 0) // amount available to bet
                {
                    if (lastBetAmount > availableBalance)
                    {
                        actionButtons[(int)PlayerAction.Check].SetActive(false);
                        actionButtons[(int)PlayerAction.Call].SetActive(false);
                        actionButtons[(int)PlayerAction.AllIn].SetActive(true);
                    }
                    else
                    {
                        callAmountText.text = "" + callAmount;
                        actionButtons[(int)PlayerAction.Check].SetActive(false);
                        actionButtons[(int)PlayerAction.AllIn].SetActive(false);
                        actionButtons[(int)PlayerAction.Call].SetActive(true);
                    }
                }
                else // dont have amount to bet hence show only fold and all-in
                {
                    actionButtons[(int)PlayerAction.Call].SetActive(false);
                    actionButtons[(int)PlayerAction.Raise].SetActive(true);
                    actionButtons[(int)PlayerAction.Check].SetActive(true);
                }

                if (callAmount == 0)
                {
                    callAmountText.text = "";
                    //actionButtons[(int)PlayerAction.Call].SetActive(false);
                    //actionButtons[(int)PlayerAction.Raise].SetActive(false);
                    //actionButtons[(int)PlayerAction.Check].SetActive(false);
                    //actionButtons[(int)PlayerAction.AllIn].SetActive(false);
                    //actionButtons[(int)PlayerAction.Fold].SetActive(false);
                }
            }

            availableCallAmount = callAmount;
        }
        actionButtonParent.SetActive(isShow);
        */




        //DEV_CODE Commented 

        //actionButtonParent.SetActive(isShow);
        //if (isShow)
        //{
        //    ResetSuggetionAction();

        //    for (int i = 0; i < actionButtons.Length; i++)
        //    {
        //        actionButtons[i].SetActive(true);
        //    }

        //    raisePopUp.SetActive(false);


        //    int callAmount = lastBetAmount - (int)playerObject.GetPlayerData().totalBet;

        //    if (callAmount > 0)
        //    {
        //        isCheckAvailable = false;
        //    }

        //    useRaisePotWise = isCheckAvailable;

        //    actionButtons[(int)PlayerAction.Check].SetActive(isCheckAvailable);
        //    actionButtons[(int)PlayerAction.AllIn].SetActive(false);

        //    if (!isCheckAvailable)
        //    {
        //        if (callAmount >= 0) // amount availabel to bet
        //        {
        //            if (lastBetAmount > availableBalance)
        //            {
        //                actionButtons[(int)PlayerAction.Call].SetActive(false);
        //                actionButtons[(int)PlayerAction.AllIn].SetActive(true);
        //            }
        //            else
        //            {
        //                callAmountText.text = "" + callAmount;

        //                actionButtons[(int)PlayerAction.AllIn].SetActive(false);
        //                actionButtons[(int)PlayerAction.Call].SetActive(true);
        //            }
        //        }
        //        else // dont have amount to bet hence show only fold and all-in
        //        {
        //            actionButtons[(int)PlayerAction.Call].SetActive(false);
        //            actionButtons[(int)PlayerAction.Raise].SetActive(false);
        //        }
        //        if (callAmount == 0)
        //        {
        //            callAmountText.text = "";

        //            //DEV_CODE Commented these lines as per InGameUiManager script
        //            //actionButtons[(int)PlayerAction.Call].SetActive(false);
        //            //actionButtons[(int)PlayerAction.Raise].SetActive(false);
        //            //actionButtons[(int)PlayerAction.Check].SetActive(false);
        //            //actionButtons[(int)PlayerAction.AllIn].SetActive(false);
        //            //actionButtons[(int)PlayerAction.Fold].SetActive(false);
        //        }
        //    }

        //    availableCallAmount = callAmount;
        //}
        Debug.LogError("AAAAA  " + transform.parent.name);
        if (isShow)
        {
            ResetSuggetionAction();
            //raisePopUp.SetActive(false);
            int callAmount = lastBetAmount - (int)playerObject.GetPlayerData().totalBet;

            if (callAmount > 0)
            {
                isCheckAvailable = false;
            }

            useRaisePotWise = isCheckAvailable;

            Debug.LogError("call amount  " + callAmount + "  lba  " + lastBetAmount + " availableBalance " + availableBalance + " totalBet " + playerObject.GetPlayerData().totalBet);

            //if (!isCheckAvailable)
            {
                if (callAmount > 0) // amount available to bet
                {
                    if (lastBetAmount > availableBalance)
                    {
                        actionButtons[(int)PlayerAction.Check].SetActive(false);
                        actionButtons[(int)PlayerAction.Call].SetActive(false);
                        actionButtons[(int)PlayerAction.AllIn].SetActive(true);
                    }
                    else
                    {
                        callAmountText.text = "" + GlobalGameManager.instance.ScoreShow(callAmount);
                        actionButtons[(int)PlayerAction.Check].SetActive(false);
                        actionButtons[(int)PlayerAction.AllIn].SetActive(false);
                        actionButtons[(int)PlayerAction.Call].SetActive(true);
                    }
                }
                else // dont have amount to bet hence show only fold and all-in
                {
                    actionButtons[(int)PlayerAction.Call].SetActive(false);
                    actionButtons[(int)PlayerAction.AllIn].SetActive(false);
                    actionButtons[(int)PlayerAction.Raise].SetActive(true);
                    actionButtons[(int)PlayerAction.Check].SetActive(true);
                    actionButtons[(int)PlayerAction.Fold].SetActive(true);
                }

                if (callAmount == 0)
                {
                    callAmountText.text = "";
                    //actionButtons[(int)PlayerAction.Call].SetActive(false);
                    //actionButtons[(int)PlayerAction.Raise].SetActive(false);
                    //actionButtons[(int)PlayerAction.Check].SetActive(false);
                    //actionButtons[(int)PlayerAction.AllIn].SetActive(false);
                    //actionButtons[(int)PlayerAction.Fold].SetActive(false);
                }
            }

            availableCallAmount = callAmount;
        }
        actionButtonParent.SetActive(isShow);
        actionPanelAnimator.SetBool("isOpen", true);
    }


    public void ShowScreen(InGameScreens screenName, object[] parameter = null)
    {
        int layer = (int)GetScreenLayer(screenName);
        for (int i = layer + 1; i < screenLayers.Length; i++)
        {
            DestroyScreen((ScreenLayer)i);
        }

        if (!IsScreenActive(screenName))
        {
            DestroyScreen(GetScreenLayer(screenName));

            InGameActiveScreens mainMenuScreen = new InGameActiveScreens();
            mainMenuScreen.screenName = screenName;
            mainMenuScreen.screenLayer = GetScreenLayer(screenName);

            GameObject gm = Instantiate(screens[(int)screenName], screenLayers[(int)mainMenuScreen.screenLayer]) as GameObject;
            mainMenuScreen.screenObject = gm;
            inGameActiveScreens.Add(mainMenuScreen);


            switch (screenName)
            {
                case InGameScreens.Menu:
                    {
                        gm.GetComponent<MenuScript>().OnOpen();
                        canvas.sortingOrder = 2;

                        if(GlobalGameManager.instance.AllTables.Count > 1)
                        {
                            int count = gm.transform.GetChild(0).childCount;
                            gm.transform.GetChild(0).GetChild(count - 2).gameObject.SetActive(false);
                            gm.transform.GetChild(0).GetChild(count - 1).gameObject.SetActive(true);
                        }
                    }
                    break;
                case InGameScreens.TopUp:
                    {
                        Debug.Log("Init topUp screen");
                        gm.GetComponent<TopUpScript>().Init((float)parameter[0]);
                    }
                    break;
                case InGameScreens.EmojiScreen:
                    {
                        gm.GetComponent<EmojiUIScreenManager>().containerVal = emojiContainerVal;
                    }
                    break;
                case InGameScreens.GameDisplay:
                    {
                        gm.GetComponent<SwitchTable>().TableImages = TableImages;
                    }
                    break;
                case InGameScreens.RealTimeResult:
                    {
                        gm.GetComponent<RealTimeResultUiManager>().OnOpen();
                        canvas.sortingOrder = 2;
                    }
                    break;
                case InGameScreens.HandHistory:
                    {
                        gm.GetComponent<HandHistoryManager>().Init();
                        canvas.sortingOrder = 2;
                    }
                    break;
                case InGameScreens.PointEarnMsg:
                    {
                        gm.GetComponent<PointEarnMsg>().OnOpen();
                    }
                    break;
                case InGameScreens.Counter:
                    {
                        if (!gm.activeSelf)
                        {
                            gm.SetActive(true);
                        }
                    }
                    break;
                case InGameScreens.CounterClub:
                    {
                        if (!gm.activeSelf)
                        {
                            gm.SetActive(true);
                        }
                    }
                    break;
                default:
                    break;
            }

        }
    }


    public void ShowMessage(string messageToShow, Action callBackMethod = null, string okButtonText = "Ok")
    {
        if (!IsScreenActive(InGameScreens.Message))
        {
            InGameActiveScreens mainMenuScreen = new InGameActiveScreens();
            mainMenuScreen.screenName = InGameScreens.Message;
            mainMenuScreen.screenLayer = GetScreenLayer(InGameScreens.Message);

            GameObject gm = Instantiate(screens[(int)InGameScreens.Message], screenLayers[(int)mainMenuScreen.screenLayer]) as GameObject;
            mainMenuScreen.screenObject = gm;

            inGameActiveScreens.Add(mainMenuScreen);

            gm.GetComponent<MessageScript>().ShowSingleButtonPopUp(messageToShow, callBackMethod, okButtonText);
        }
    }


    public void ShowMessage(string messageToShow, Action yesButtonCallBack, Action noButtonCallBack, string yesButtonText = "Yes", string noButtonText = "No")
    {
        if (!IsScreenActive(InGameScreens.Message))
        {
            InGameActiveScreens mainMenuScreen = new InGameActiveScreens();
            mainMenuScreen.screenName = InGameScreens.Message;
            mainMenuScreen.screenLayer = GetScreenLayer(InGameScreens.Message);

            GameObject gm = Instantiate(screens[(int)InGameScreens.Message], screenLayers[(int)mainMenuScreen.screenLayer]) as GameObject;
            mainMenuScreen.screenObject = gm;

            inGameActiveScreens.Add(mainMenuScreen);
            gm.GetComponent<MessageScript>().ShowDoubleButtonPopUp(messageToShow, yesButtonCallBack, noButtonCallBack, yesButtonText, noButtonText);
        }
    }



    public void DestroyScreen(InGameScreens screenName)
    {
        for (int i = 0; i < inGameActiveScreens.Count; i++)
        {
            if (inGameActiveScreens[i].screenName == screenName)
            {
                Destroy(inGameActiveScreens[i].screenObject);
                inGameActiveScreens.RemoveAt(i);
            }
        }
    }

    public void DestroyScreen(ScreenLayer layerName)
    {
        for (int i = 0; i < inGameActiveScreens.Count; i++)
        {
            if (inGameActiveScreens[i].screenLayer == layerName)
            {
                Destroy(inGameActiveScreens[i].screenObject);
                inGameActiveScreens.RemoveAt(i);
            }
        }
    }

    private bool IsScreenActive(InGameScreens screenName)
    {
        for (int i = 0; i < inGameActiveScreens.Count; i++)
        {
            if (inGameActiveScreens[i].screenName == screenName)
            {
                return true;
            }
        }

        return false;
    }


    private ScreenLayer GetScreenLayer(InGameScreens screenName)
    {
        switch (screenName)
        {
            case InGameScreens.Message:
                return ScreenLayer.LAYER2;

            case InGameScreens.Reconnecting:
                return ScreenLayer.LAYER3;

            case InGameScreens.Loading:
                return ScreenLayer.LAYER4;

            default:
                return ScreenLayer.LAYER1;
        }
    }

    public void ShowTableMessage(string messageToShow)
    {
        tableText.text = messageToShow;
    }


    public void OnClickOnBack()
    {
        clubInGameManager.LoadMainMenu();
    }

    /////// EMOJI
    /// <summary>
    /// 

    /// </summary>
    /// <param name="str"></param>
    /// 

    public int emojiIndex;
    public int otherId;
    public string sentToEmojiValue;

    public void CallEmojiSocket(int index)
    {
        emojiIndex = index;
        // Debug.LogError("i am here------------ call emoji index "+index+"   "+emojiIndex+"    "+otherId);
        ClubSocketController.instance.SentEmoji(otherId, ClubInGameUIManager.instance.emojiIndex);


    }

    public void ShowEmojiOnScreen(string str)
    {
        if (sentBy == null) { sentBy = sentTo; }    //DEV_CODE Added this line as per InGameUiManager script

        //Debug.LogError("Show Emoji From List on Screen *** " + str);
        GameObject g = null;
        Vector3 scaleValue = new Vector3(1f, 1f, 1f);   //DEV_CODE Added this line as per InGameUiManager script
        switch (str)
        {

            case "bluffing":
                g = Instantiate(EmojiPrefabs[(int)Emoji.Bluffing], EmojiShowTransform) as GameObject;
                scaleValue = EmojiPrefabs[(int)Emoji.Bluffing].transform.localScale;
                break;
            case "youRaPro":
                g = Instantiate(EmojiPrefabs[(int)Emoji.YouRaPro], EmojiShowTransform) as GameObject;
                scaleValue = EmojiPrefabs[(int)Emoji.YouRaPro].transform.localScale;
                break;
            case "beerCheers":
                g = Instantiate(EmojiPrefabs[(int)Emoji.BeerCheers], EmojiShowTransform) as GameObject;
                scaleValue = EmojiPrefabs[(int)Emoji.BeerCheers].transform.localScale;
                break;
            case "murgi":
                g = Instantiate(EmojiPrefabs[(int)Emoji.Murgi], EmojiShowTransform) as GameObject;
                scaleValue = EmojiPrefabs[(int)Emoji.Murgi].transform.localScale;
                break;
            case "rocket":
                g = Instantiate(EmojiPrefabs[(int)Emoji.Rocket], EmojiShowTransform) as GameObject;
                scaleValue = EmojiPrefabs[(int)Emoji.Rocket].transform.localScale;
                break;
            case "dung":
                g = Instantiate(EmojiPrefabs[(int)Emoji.Dung], EmojiShowTransform) as GameObject;
                scaleValue = EmojiPrefabs[(int)Emoji.Dung].transform.localScale;
                break;
            case "oscar":
                Debug.LogError("oscar Emoji From List on Screen 00000*** " + str);
                g = Instantiate(EmojiPrefabs[(int)Emoji.Oscar], EmojiShowTransform) as GameObject;
                scaleValue = EmojiPrefabs[(int)Emoji.Oscar].transform.localScale;
                break;
            case "donkey":
                Debug.LogError("donkey Emoji From List on Screen *** " + str);
                g = Instantiate(EmojiPrefabs[(int)Emoji.Donkey], EmojiShowTransform) as GameObject;
                scaleValue = EmojiPrefabs[(int)Emoji.Donkey].transform.localScale;
                break;
            case "thumbUp":
                g = Instantiate(EmojiPrefabs[(int)Emoji.ThumbsUp], EmojiShowTransform) as GameObject;
                scaleValue = EmojiPrefabs[(int)Emoji.ThumbsUp].transform.localScale;
                break;
            case "cherees":
                g = Instantiate(EmojiPrefabs[(int)Emoji.Cherees], EmojiShowTransform) as GameObject;
                scaleValue = EmojiPrefabs[(int)Emoji.Cherees].transform.localScale;
                break;
            case "kiss":
                g = Instantiate(EmojiPrefabs[(int)Emoji.Kiss], EmojiShowTransform) as GameObject;
                scaleValue = EmojiPrefabs[(int)Emoji.Kiss].transform.localScale;
                break;
            case "fish":
                g = Instantiate(EmojiPrefabs[(int)Emoji.Fish], EmojiShowTransform) as GameObject;
                scaleValue = EmojiPrefabs[(int)Emoji.Fish].transform.localScale;
                break;
            case "gun":
                g = Instantiate(EmojiPrefabs[(int)Emoji.Gun], EmojiShowTransform) as GameObject;
                scaleValue = EmojiPrefabs[(int)Emoji.Gun].transform.localScale;
                break;
            case "rose":
                g = Instantiate(EmojiPrefabs[(int)Emoji.Rose], EmojiShowTransform) as GameObject;
                scaleValue = EmojiPrefabs[(int)Emoji.Rose].transform.localScale;
                break;
            case "perfume":
                g = Instantiate(EmojiPrefabs[(int)Emoji.Perfume], EmojiShowTransform) as GameObject;
                scaleValue = EmojiPrefabs[(int)Emoji.Perfume].transform.localScale;
                break;
            case "ring":
                g = Instantiate(EmojiPrefabs[(int)Emoji.Ring], EmojiShowTransform) as GameObject;
                scaleValue = EmojiPrefabs[(int)Emoji.Ring].transform.localScale;
                break;
            case "car":
                g = Instantiate(EmojiPrefabs[(int)Emoji.Car], EmojiShowTransform) as GameObject;
                scaleValue = EmojiPrefabs[(int)Emoji.Car].transform.localScale;
                break;
        }
        g.transform.SetParent(EmojiShowTransform);
        g.transform.localScale = scaleValue;    //DEV_CODE Added this line as per InGameUiManager script
        //g.GetComponent<EmojiBehaviour>().target = fromEmojiShowTransform;     //DEV_CODE Commented this line as per InGameUiManager script
        g.transform.DOMove(sentTo.position, 1.5f);  //DEV_CODE Added this line as per InGameUiManager script
    }


    public void OnGetEmoji(string serverResponse)
    {
        Debug.LogError("OnSentEmoji:" + serverResponse);

        JsonData data = JsonMapper.ToObject(serverResponse);

        if (data[0]["Status"].ToString().Equals("1.0") || data[0]["Status"].ToString().Equals("1"))
        {
            for (int i = 0; i < players.transform.childCount; i++)
            {
                if (data[0]["sentTo"].ToString().Equals("0"))
                {
                    sentTo = GirlDealerEmoji.transform;     //DEV_CODE Added this line as per InGameUiManager script
                    //fromEmojiShowTransform = GirlDealerEmoji.transform;   //DEV_CODE Commented this line as per InGameUiManage script
                    sentToEmojiValue = "Dealer";
                }
                else
                {
                    if (players.transform.GetChild(i).GetComponent<PlayerScript>().playerData.userId == data[0]["sentTo"].ToString())
                    {
                        //DEV_CODE Added this line as per InGameUiManager script
                        sentTo = players.transform.GetChild(i).GetChild(0).Find("Emoji").transform;

                        //DEV_CODE Commented below lines as per InGameUiManager script
                        //fromEmojiShowTransform = players.transform.GetChild(i).GetChild(0).Find("Emoji").transform;
                        //break;
                    }

                    //DEV_CODE Added this condition as per InGameUiManager script
                    if (players.transform.GetChild(i).GetComponent<PlayerScript>().playerData.userId == data[0]["sentBy"].ToString())
                    {
                        sentBy = players.transform.GetChild(i).GetChild(0).Find("Emoji").transform;
                    }
                }
            }
            switch (data[0]["emojiIndex"].ToString())
            {
                case "0":
                    ShowEmojiOnScreen("bluffing");
                    break;
                case "1":
                    ShowEmojiOnScreen("youRaPro");
                    break;
                case "2":
                    ShowEmojiOnScreen("beerCheers");
                    break;
                case "3":
                    ShowEmojiOnScreen("murgi");
                    break;
                case "4":
                    ShowEmojiOnScreen("rocket");
                    break;
                case "5":
                    ShowEmojiOnScreen("dung");
                    break;
                case "6":
                    ShowEmojiOnScreen("oscar");
                    break;
                case "7":
                    ShowEmojiOnScreen("donkey");
                    break;
                case "8":
                    ShowEmojiOnScreen("gun");
                    break;
                case "9":
                    ShowEmojiOnScreen("cherees");
                    break;
                case "10":
                    ShowEmojiOnScreen("kiss");
                    break;
                case "11":
                    ShowEmojiOnScreen("fish");
                    break;
                case "12":
                    ShowEmojiOnScreen("thumbUp");
                    break;
                case "13":
                    ShowEmojiOnScreen("rose");
                    break;
                case "14":
                    ShowEmojiOnScreen("perfume");
                    break;
                case "15":
                    ShowEmojiOnScreen("ring");
                    break;
                case "16":
                    ShowEmojiOnScreen("car");
                    break;

            }
        }
    }
}


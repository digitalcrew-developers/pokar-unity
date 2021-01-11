using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using LitJson;

public class InGameUiManagerTeenPatti : MonoBehaviour
{
    public static InGameUiManagerTeenPatti instance;

    [SerializeField]
    private GameObject[] screens; // All screens prefab

    [SerializeField]
    private Transform[] screenLayers; // screen spawn parent

    [SerializeField]
    private GameObject[] actionButtons, suggestionButtons, suggestionButtonsActiveImage; // screen spawn parent

    [SerializeField]
    public GameObject actionButtonParent, raisePopUp, suggestionButtonParent, clubTableOject, lobbyTableObject;

    [SerializeField]
    private Text tableText, tableInfoText, callAmountText, sliderText, suggestionCallText , showCallAomuntText;

    [SerializeField]
    private Slider slider;

    [SerializeField]
    private GameObject chatButton;
    [SerializeField]
    public GameObject LoadingImage;

    public GameObject acceptSideShowButton, rejectsideShowButton;

    public GameObject betPlusButton, betMinusButton;



    

    

   // private SuggestionActions selectedSuggestionButton = SuggestionActions.Null;


    private float availableCallAmount = 0, selectedRaiseAmount = 0;
    private List<InGameActiveScreensTeenPatti> inGameActiveScreens = new List<InGameActiveScreensTeenPatti>();
    private bool useRaisePotWise = false;
    private int suggestionCallAmount = 0;

    public GameObject ArrowPopUp;

    public int emojiContainerVal;
    public Transform EmojiShowTransform;
    public Transform fromEmojiShowTransform;
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
    public Camera cameraObj;
    public float height, width;

    public List<GameObject> TableImages = new List<GameObject>();

    public GameObject showMatchButton;

    public Sprite blindSprite, callSprite;

    public Text sideShowRequesterPlayer;

    public GameObject rejectSideShowBtn;

    public Text actionText;

    public GameObject notifyUser;

    private void Awake()
    {
        instance = this;

        //DEV_CODE
        //cameraObj = GameObject.Find("VideoRecordingCamera").GetComponent<Camera>();

        Canvas canvas = gameObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = Camera.main;

        height = gameObject.GetComponent<RectTransform>().rect.height;
        width = gameObject.GetComponent<RectTransform>().rect.width;
    }


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


        tableInfoText.text = "Blinds " + GlobalGameManager.instance.GetRoomData().smallBlind + "/" + GlobalGameManager.instance.GetRoomData().bigBlind + " Ante";
        ToggleActionButton(false , null , false , 0);
        //ToggleSuggestionButton(false);

        int counter = PlayerPrefs.GetInt("TableCount");
        //SwitchTables(counter);
    }

    private void SwitchTables(int counter)
    {
        Debug.LogError("counter is " + counter);
        PlayerPrefs.SetInt("TableCount", counter);

        foreach (GameObject g in TableImages)
        {
            g.SetActive(false);
        }
        TableImages[counter].SetActive(true);
    }

    public void CallDectuct(int val) {
        DeductCoinPostServer(val);
    }


    void DeductCoinPostServer(int val)
    {

        int amount = val;

        string requestData = "{\"userId\":\"" + PlayerManager.instance.GetPlayerGameData().userId + "\"," +
                              "\"amount\":\"" + amount + "\"," +
                              "\"deductFrom\":\"" + "coins" + "\"," +
                               "\"narration\":\"" + "Wining Booster" + "\"}";
        WebServices.instance.SendRequest(RequestType.deductFromWallet, requestData, true, OnServerResponseFound);
    }
    public void OnServerResponseFound(RequestType requestType, string serverResponse, bool isShowErrorMessage, string errorMessage)
    {

        Debug.Log("IIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIII");
        MainMenuController.instance.DestroyScreen(MainMenuScreens.Loading);

        if (errorMessage.Length > 0)
        {
            if (isShowErrorMessage)
            {
                MainMenuController.instance.ShowMessage(errorMessage);
            }

            return;
        }
        if (requestType == RequestType.deductFromWallet)
        {
            JsonData data = JsonMapper.ToObject(serverResponse);
            if (data["success"].ToString() == "1")
            {

              
            }
            else
            {
                MainMenuController.instance.ShowMessage(data["message"].ToString());
            }
        }
       
    }

    public void OnSpinWheelArrowBtnClick()
    {
        Debug.Log("IIII  SpinWheelArrowBtnClick");
        SoundManager.instance.PlaySound(SoundType.Click);

        ShowScreen(InGameScreensTeenPatti.SpinWheelScreen);
    }

   

    public void OnArrowBtnClick()
    {
         ArrowPopUp.SetActive(true);
    }
    public void CloseArrowBtnClick()
    {
        ArrowPopUp.SetActive(false);
    }

    public void OnClickEmoji(int val)
    {
        emojiContainerVal = val;
        Debug.Log("I AM HERE______________  "+val);
    }
    public void OnClickEmojiTransform(Transform val)
    {
        EmojiShowTransform = val;
        Debug.Log("I am getting emoji transform   "+val.transform.parent.parent.name);
        if (val.transform.parent.parent.name.Equals("LobbyTable"))
        {
            otherId = 0;
        }
        else {
            Debug.Log("I am getting emoji transform 000000  " + val.transform.parent.parent.GetComponent<PlayerScriptTeenPatti>().playerData.userId);
            //if ((val.transform.parent.parent.GetComponent<PlayerScript>().playerData.userId) != "")
            //{
            //    otherId = int.Parse(val.transform.parent.parent.GetComponent<PlayerScript>().playerData.userId);
            //}
            if (InGameUiManagerTeenPatti.instance.TempUserID!= "")
            {
                otherId = int.Parse(InGameUiManagerTeenPatti.instance.TempUserID);
            }
        }
        
    }


    public void OnSpwanTipCoin()
    {
        SoundManager.instance.PlaySound(SoundType.Tip);

        GameObject g = Instantiate(tipsCoins, spwantipsCoinsPos) as GameObject;
        g.transform.SetParent(spwantipsCoinsPos);

        SocketController.instance.TipToDealer();
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
            case "store":
                {
                    ShowScreen(InGameScreensTeenPatti.InGameShop);
                }
                break;


            case "menu":
                {
                    
                    ShowScreen(InGameScreensTeenPatti.MenuTeenPatti);
                }
                break;

            case "missions":
                {
                    ShowScreen(InGameScreensTeenPatti.Missions);
                }
                break;
            case "emojiScreen":
                {
                    ShowScreen(InGameScreensTeenPatti.EmojiScreen);
                }
                break;
            case "fold":
                {
                    GameConstants.timerStart = 0;
                    PlayerScriptTeenPatti.instance.fx_holder.gameObject.SetActive(false);
                    PlayerScriptTeenPatti.instance.timerBar.fillAmount = 0;
                    InGameManagerTeenPatti.instance.OnPlayerActionCompleted(PlayerAction.Fold, 0, "Fold");
                    //actionText.text = PlayerScriptTeenPatti.instance.playerData.userName + " fold the cards.";
                }
                break;

            case "call":
                {
                    GameConstants.timerStart = 0;
                    
                    



                    PlayerScriptTeenPatti.instance.fx_holder.gameObject.SetActive(false);
                    PlayerScriptTeenPatti.instance.timerBar.fillAmount = 0;
                    PlayerScriptTeenPatti.instance.timerBar.gameObject.SetActive(false);
                    InGameManagerTeenPatti.instance.OnPlayerActionCompleted(PlayerAction.Call, (int)availableCallAmount, "Call");
                    //if(PlayerScriptTeenPatti.instance.playerData.isBlind)
                    //actionText.text = PlayerScriptTeenPatti.instance.playerData.userName + " calls the blind";
                    //else
                    //    actionText.text = PlayerScriptTeenPatti.instance.playerData.userName + " calls the chaal";
                }
                break;


            case "check":
                {
                    InGameManagerTeenPatti.instance.OnPlayerActionCompleted(PlayerAction.Check, 0, "Check");
                }
                break;


            case "raiseOpen":
                {
                    PlayerScriptTeenPatti player = InGameManagerTeenPatti.instance.GetMyPlayerObject();

                    if (player != null)
                    {
                        ToggleRaisePopUp(true, availableCallAmount + 1, player.GetPlayerData().balance, InGameManagerTeenPatti.instance.GetPotAmount());
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
                        PlayerScriptTeenPatti player = InGameManagerTeenPatti.instance.GetMyPlayerObject();

                        if (player != null)
                        {
                            InGameManagerTeenPatti.instance.OnPlayerActionCompleted(PlayerAction.Raise, (int)player.GetPlayerData().balance, "AllIn");
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
                        InGameManagerTeenPatti.instance.OnPlayerActionCompleted(PlayerAction.Raise, (int)selectedRaiseAmount, "Raise");
                    }
                }
                break;

            case "pot":
                {
                    if (useRaisePotWise) // PotWise Calculation
                    {
                        slider.value = InGameManagerTeenPatti.instance.GetPotAmount();
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
                        slider.value = (InGameManagerTeenPatti.instance.GetPotAmount() / 2f);
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
                        slider.value = ((InGameManagerTeenPatti.instance.GetPotAmount() * 2f) / 3f);
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

                    //if (selectedSuggestionButton == SuggestionActions.Call)
                    //{
                    //    suggestionButtonsActiveImage[(int)selectedSuggestionButton].SetActive(false);
                    //    selectedSuggestionButton = SuggestionActions.Null;
                    //}
                    //else
                    //{
                    //    for (int i = 0; i < suggestionButtonsActiveImage.Length; i++)
                    //    {
                    //        suggestionButtonsActiveImage[i].gameObject.SetActive(false);
                    //    }

                    //    suggestionCallAmount = (int)availableCallAmount;
                    //    selectedSuggestionButton = SuggestionActions.Call;

                    //    suggestionButtonsActiveImage[(int)selectedSuggestionButton].SetActive(true);
                    //}
                }
                break;


            case "sCallAny":
                {
                    //if (selectedSuggestionButton == SuggestionActions.Call_Any)
                    //{
                    //    suggestionButtonsActiveImage[(int)selectedSuggestionButton].SetActive(false);
                    //    selectedSuggestionButton = SuggestionActions.Null;
                    //}
                    //else
                    //{
                    //    for (int i = 0; i < suggestionButtonsActiveImage.Length; i++)
                    //    {
                    //        suggestionButtonsActiveImage[i].gameObject.SetActive(false);
                    //    }

                    //    selectedSuggestionButton = SuggestionActions.Call_Any;
                    //    suggestionButtonsActiveImage[(int)selectedSuggestionButton].SetActive(true);
                    //}
                }
                break;

            case "sCheck":
                {
                    //if (selectedSuggestionButton == SuggestionActions.Check)
                    //{
                    //    suggestionButtonsActiveImage[(int)selectedSuggestionButton].SetActive(false);
                    //    selectedSuggestionButton = SuggestionActions.Null;
                    //}
                    //else
                    //{
                    //    for (int i = 0; i < suggestionButtonsActiveImage.Length; i++)
                    //    {
                    //        suggestionButtonsActiveImage[i].gameObject.SetActive(false);
                    //    }

                    //    selectedSuggestionButton = SuggestionActions.Check;
                    //    suggestionButtonsActiveImage[(int)selectedSuggestionButton].SetActive(true);
                    //}


                }
                break;

            case "sFold":
                {
                    //if (selectedSuggestionButton == SuggestionActions.Fold)
                    //{
                    //    suggestionButtonsActiveImage[(int)selectedSuggestionButton].SetActive(false);
                    //    selectedSuggestionButton = SuggestionActions.Null;
                    //}
                    //else
                    //{
                    //    for (int i = 0; i < suggestionButtonsActiveImage.Length; i++)
                    //    {
                    //        suggestionButtonsActiveImage[i].gameObject.SetActive(false);
                    //    }

                    //    selectedSuggestionButton = SuggestionActions.Fold;
                    //    suggestionButtonsActiveImage[(int)selectedSuggestionButton].SetActive(true);
                    //}


                }
                break;


            case "handHistory":
                {
                    ShowScreen(InGameScreensTeenPatti.HandHistory);
                }
                break;

            case "realTimeResult":
                {
                    ShowScreen(InGameScreensTeenPatti.RealTimeResult);
                }
                break;
            case "pointEarnopen":
                {
                    ShowScreen(InGameScreensTeenPatti.PointEarnMsg);
                }
                break;

            case "chat":
                {
                    if (SocketControllerTeenPatti.instance.GetSocketState() == SocketStateTeenPatti.Game_Running)
                    {
                        ShowScreen(InGameScreensTeenPatti.Chat);
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




    public void OnSliderValueChange()
    {
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



    //public SuggestionActions GetSelectedSuggestionAction()
    //{
    //    return selectedSuggestionButton;
    //}

    //public void ResetSuggetionAction()
    //{
    //    selectedSuggestionButton = SuggestionActions.Null;
    //}


    public void ToggleSuggestionButton(bool isShow, bool isCheckAvailable = false, int callAmount = 0, float availableBalance = 0)
    {
        suggestionButtonParent.SetActive(isShow);

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


            //if (selectedSuggestionButton == SuggestionActions.Call && callAmount != suggestionCallAmount)
            //{
            //    selectedSuggestionButton = SuggestionActions.Null;
            //}
            //else if (selectedSuggestionButton != SuggestionActions.Null && suggestionButtons[(int)selectedSuggestionButton].activeInHierarchy)
            //{
            //    suggestionButtonsActiveImage[(int)selectedSuggestionButton].SetActive(true);
            //}
            //else
            //{
            //    selectedSuggestionButton = SuggestionActions.Null;
            //}
        }
    }



    public bool IsActionButtonsActive()
    {
        return actionButtonParent.activeInHierarchy;
    }

    bool clickPlusButton = false;
    public void OnClickBetPlusButton()
    {
        if (!clickPlusButton)
        {
            callAmountText.text = "" + (GameConstants.playerbetAmount * 2).ToString();
            GameConstants.playerbetAmount = GameConstants.playerbetAmount * 2;
            clickPlusButton = true;
        }
    }

    public void OnClickBetMinusButton()
    {
        if (clickPlusButton)
        {
            callAmountText.text = "" + (GameConstants.playerbetAmount / 2).ToString();
            GameConstants.playerbetAmount = GameConstants.playerbetAmount / 2;
            clickPlusButton = false;
        }

    }

    int i = 0;
    public void ToggleActionButton(bool isShow, PlayerScriptTeenPatti playerObject = null,
        bool isCheckAvailable = false, int lastBetAmount = 0)
    {
        actionButtonParent.SetActive(isShow);

        if (isShow)
        {
            //ResetSuggetionAction();

            for (int i = 0; i < actionButtons.Length; i++)
            {
                actionButtons[i].SetActive(true);
            }

            //raisePopUp.SetActive(false);
            //if (i == 0)
            //{
            //    lastBetAmount = 200;
            //    i++;
            //}

            clickPlusButton = false;
            acceptSideShowButton.SetActive(false);
            rejectSideShowBtn.SetActive(false);

            int callAmount = (int)playerObject.GetPlayerData().totalBet;
            //int callAmount = (int)playerObject.GetPlayerData().totalBet;
            
            GameConstants.playerbetAmount = callAmount;

            if (callAmount > 0)
            {
                isCheckAvailable = false;
            }
            

           

            // useRaisePotWise = isCheckAvailable;

            //actionButtons[(int)PlayerAction.Check].SetActive(isCheckAvailable);

            if (!isCheckAvailable)
            {
                Debug.LogError("Player is :" + playerObject.GetPlayerData().userId);

                if (playerObject.GetPlayerData().isShow)
                {
                    showMatchButton.SetActive(true);
                    actionButtons[5].SetActive(false);
                }
                
                else if(playerObject.GetPlayerData().isSideShow)
                {
                    showMatchButton.SetActive(false);
                    actionButtons[5].SetActive(true);
                }

                else if(!playerObject.GetPlayerData().isShow && !playerObject.GetPlayerData().isSideShow)
                {
                    showMatchButton.SetActive(false);
                    actionButtons[5].SetActive(false);
                }
                
                
                if (playerObject.GetPlayerData().isBlind)
                {

                    if (callAmount >= 0) // amount availabel to bet
                    {
                        callAmountText.transform.parent.transform.GetChild(0).gameObject.GetComponent<Text>().text = "Blind";
                        callAmountText.text = "" + callAmount;
                        showCallAomuntText.text = "" + callAmount;
                        actionButtons[(int)PlayerAction.Call].GetComponent<Image>().sprite = blindSprite;
                    }
                    else // dont have amount to bet hence show only fold and all-in
                    {
                        //actionButtons[(int)PlayerAction.Call].SetActive(false);
                        //actionButtons[(int)PlayerAction.Raise].SetActive(false);
                        callAmountText.transform.parent.transform.GetChild(0).gameObject.GetComponent<Text>().text = "Blind";
                        callAmountText.text = "" + Mathf.Abs (callAmount);
                        showCallAomuntText.text = "" + callAmount;
                        actionButtons[(int)PlayerAction.Call].GetComponent<Image>().sprite = blindSprite;
                    }
                    if (callAmount == 0)
                    {
                        callAmountText.transform.parent.transform.GetChild(0).gameObject.GetComponent<Text>().text = "Blind";
                        callAmountText.text = "";
                        showCallAomuntText.text = "" + callAmount;

                    }
                }
                else
                {
                    if (callAmount >= 0) // amount availabel to bet
                    {
                        callAmountText.transform.parent.transform.GetChild(0).gameObject.GetComponent<Text>().text = "Chaal";
                        callAmountText.text = "" + callAmount;
                        showCallAomuntText.text = "" + callAmount;
                        actionButtons[(int)PlayerAction.Call].GetComponent<Image>().sprite = callSprite;
                    }
                    else // dont have amount to bet hence show only fold and all-in
                    {
                        actionButtons[(int)PlayerAction.Call].SetActive(false);
                       
                        // actionButtons[(int)PlayerAction.Raise].SetActive(false);
                    }
                    if (callAmount == 0)
                    {
                        callAmountText.transform.parent.transform.GetChild(0).gameObject.GetComponent<Text>().text = "Chaal";
                        showCallAomuntText.text = "" + callAmount;
                        callAmountText.text = "";

                    }
                }

               

            }

            availableCallAmount = callAmount;
        }
    }



    public void ConvertActionButtons(bool isShow, PlayerScriptTeenPatti playerObject = null,
        bool isCheckAvailable = false, int lastBetAmount = 200)
    {
        //actionButtonParent.SetActive(isShow);

        int callAmount = (int)playerObject.GetPlayerData().totalBet;
        GameConstants.playerbetAmount = callAmount;

        if (callAmount > 0)
        {
            isCheckAvailable = false;
        }




        // useRaisePotWise = isCheckAvailable;

        //actionButtons[(int)PlayerAction.Check].SetActive(isCheckAvailable);

        if (!isCheckAvailable)
        {
            Debug.LogError("Player is :" + playerObject.GetPlayerData().userId);

            if (playerObject.GetPlayerData().isShow)
            {
                showMatchButton.SetActive(true);
                actionButtons[5].SetActive(false);
            }

            else if (playerObject.GetPlayerData().isSideShow)
            {
                showMatchButton.SetActive(false);
                actionButtons[5].SetActive(true);
            }

            else if (!playerObject.GetPlayerData().isShow && !playerObject.GetPlayerData().isSideShow)
            {
                showMatchButton.SetActive(false);
                actionButtons[5].SetActive(false);
            }


            if (playerObject.GetPlayerData().isBlind)
            {

                if (callAmount >= 0) // amount availabel to bet
                {
                    callAmountText.transform.parent.transform.GetChild(0).gameObject.GetComponent<Text>().text = "Blind";
                    callAmountText.text = "" + callAmount;
                    actionButtons[(int)PlayerAction.Call].GetComponent<Image>().sprite = blindSprite;
                    showCallAomuntText.text = "" + callAmount;
                }
                else // dont have amount to bet hence show only fold and all-in
                {
                    actionButtons[(int)PlayerAction.Call].SetActive(false);
                    //actionButtons[(int)PlayerAction.Raise].SetActive(false);
                }
                if (callAmount == 0)
                {
                    callAmountText.transform.parent.transform.GetChild(0).gameObject.GetComponent<Text>().text = "Blind";
                    callAmountText.text = "";
                    showCallAomuntText.text = "" + callAmount;

                }
            }
            else
            {
                if (callAmount >= 0) // amount availabel to bet
                {
                    callAmountText.transform.parent.transform.GetChild(0).gameObject.GetComponent<Text>().text = "Chaal";
                    callAmountText.text = "" + callAmount;
                    showCallAomuntText.text = "" + callAmount;
                    actionButtons[(int)PlayerAction.Call].GetComponent<Image>().sprite = callSprite;
                }
                else // dont have amount to bet hence show only fold and all-in
                {
                    actionButtons[(int)PlayerAction.Call].SetActive(false);
                    // actionButtons[(int)PlayerAction.Raise].SetActive(false);
                }
                if (callAmount == 0)
                {
                    callAmountText.transform.parent.transform.GetChild(0).gameObject.GetComponent<Text>().text = "Chaal";
                    callAmountText.text = "";
                    showCallAomuntText.text = "" + callAmount;

                }
            }

        //    if (isShow)
        //{
        //    //ResetSuggetionAction();

        //    for (int i = 0; i < actionButtons.Length; i++)
        //    {
        //        actionButtons[i].SetActive(true);
        //    }

        //    //raisePopUp.SetActive(false);
        //    if (i == 0)
        //    {
        //        lastBetAmount = 200;
        //        i++;
        //    }




            



        //    }

        //    availableCallAmount = callAmount;        //    if (isShow)
        //{
        //    //ResetSuggetionAction();

        //    for (int i = 0; i < actionButtons.Length; i++)
        //    {
        //        actionButtons[i].SetActive(true);
        //    }

        //    //raisePopUp.SetActive(false);
        //    if (i == 0)
        //    {
        //        lastBetAmount = 200;
        //        i++;
        //    }




            



        //    }

        //    availableCallAmount = callAmount;        //    if (isShow)
        //{
        //    //ResetSuggetionAction();

        //    for (int i = 0; i < actionButtons.Length; i++)
        //    {
        //        actionButtons[i].SetActive(true);
        //    }

        //    //raisePopUp.SetActive(false);
        //    if (i == 0)
        //    {
        //        lastBetAmount = 200;
        //        i++;
        //    }




            



        //    }

        //    availableCallAmount = callAmount;        //    if (isShow)
        //{
        //    //ResetSuggetionAction();

        //    for (int i = 0; i < actionButtons.Length; i++)
        //    {
        //        actionButtons[i].SetActive(true);
        //    }

        //    //raisePopUp.SetActive(false);
        //    if (i == 0)
        //    {
        //        lastBetAmount = 200;
        //        i++;
        //    }




            



        //    }

        //    availableCallAmount = callAmount;        //    if (isShow)
        //{
        //    //ResetSuggetionAction();

        //    for (int i = 0; i < actionButtons.Length; i++)
        //    {
        //        actionButtons[i].SetActive(true);
        //    }

        //    //raisePopUp.SetActive(false);
        //    if (i == 0)
        //    {
        //        lastBetAmount = 200;
        //        i++;
        //    }




            



        //    }

        //    availableCallAmount = callAmount;        //    if (isShow)
        //{
        //    //ResetSuggetionAction();

        //    for (int i = 0; i < actionButtons.Length; i++)
        //    {
        //        actionButtons[i].SetActive(true);
        //    }

        //    //raisePopUp.SetActive(false);
        //    if (i == 0)
        //    {
        //        lastBetAmount = 200;
        //        i++;
        //    }




            



        //    }

        //    availableCallAmount = callAmount;        //    if (isShow)
        //{
        //    //ResetSuggetionAction();

        //    for (int i = 0; i < actionButtons.Length; i++)
        //    {
        //        actionButtons[i].SetActive(true);
        //    }

        //    //raisePopUp.SetActive(false);
        //    if (i == 0)
        //    {
        //        lastBetAmount = 200;
        //        i++;
        //    }




            



        //    }

        //    availableCallAmount = callAmount;
        }
    }

    public void ShowScreen(InGameScreensTeenPatti screenName, object[] parameter = null)
    {
        int layer = (int)GetScreenLayer(screenName);
        for (int i = layer + 1; i < screenLayers.Length; i++)
        {
            DestroyScreen((ScreenLayer)i);
        }

        if (!IsScreenActive(screenName))
        {
            DestroyScreen(GetScreenLayer(screenName));

            InGameActiveScreensTeenPatti mainMenuScreen = new InGameActiveScreensTeenPatti();
            mainMenuScreen.screenName = screenName;
            mainMenuScreen.screenLayer = GetScreenLayer(screenName);

            GameObject gm = Instantiate(screens[(int)screenName], screenLayers[(int)mainMenuScreen.screenLayer]) as GameObject;
            mainMenuScreen.screenObject = gm;
            inGameActiveScreens.Add(mainMenuScreen);


            switch (screenName)
            {
                case InGameScreensTeenPatti.TopUp:
                    {
                        Debug.Log("Init topUp screen");
                        gm.GetComponent<TopUpScriptTeenPatti>().Init((float)parameter[0]);
                    }
                    break;
                case InGameScreensTeenPatti.EmojiScreen:
                    {
                        gm.GetComponent<EmojiUIScreenManager>().containerVal = emojiContainerVal;
                    }
                    break;
                case InGameScreensTeenPatti.SwitchTable:
                    {
                        gm.GetComponent<SwitchTableTeenPatti>().TableImages = TableImages;
                    }
                    break;
                default:
                    break;
            }

        }
    }


    public void ShowMessage(string messageToShow, Action callBackMethod = null, string okButtonText = "Ok")
    {
        if (!IsScreenActive(InGameScreensTeenPatti.Message))
        {
            InGameActiveScreensTeenPatti mainMenuScreen = new InGameActiveScreensTeenPatti();
            mainMenuScreen.screenName = InGameScreensTeenPatti.Message;
            mainMenuScreen.screenLayer = GetScreenLayer(InGameScreensTeenPatti.Message);

            GameObject gm = Instantiate(screens[(int)InGameScreensTeenPatti.Message], screenLayers[(int)mainMenuScreen.screenLayer]) as GameObject;
            mainMenuScreen.screenObject = gm;

            inGameActiveScreens.Add(mainMenuScreen);

            gm.GetComponent<MessageScript>().ShowSingleButtonPopUp(messageToShow, callBackMethod, okButtonText);
        }
    }


    public void ShowMessage(string messageToShow, Action yesButtonCallBack, Action noButtonCallBack, string yesButtonText = "Yes", string noButtonText = "No")
    {
        if (!IsScreenActive(InGameScreensTeenPatti.Message))
        {
            InGameActiveScreensTeenPatti mainMenuScreen = new InGameActiveScreensTeenPatti();
            mainMenuScreen.screenName = InGameScreensTeenPatti.Message;
            mainMenuScreen.screenLayer = GetScreenLayer(InGameScreensTeenPatti.Message);

            GameObject gm = Instantiate(screens[(int)InGameScreensTeenPatti.Message], screenLayers[(int)mainMenuScreen.screenLayer]) as GameObject;
            mainMenuScreen.screenObject = gm;

            inGameActiveScreens.Add(mainMenuScreen);
            gm.GetComponent<MessageScript>().ShowDoubleButtonPopUp(messageToShow, yesButtonCallBack, noButtonCallBack, yesButtonText, noButtonText);
        }
    }



    public void DestroyScreen(InGameScreensTeenPatti screenName)
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

    private bool IsScreenActive(InGameScreensTeenPatti screenName)
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


    private ScreenLayer GetScreenLayer(InGameScreensTeenPatti screenName)
    {
        switch (screenName)
        {
            case InGameScreensTeenPatti.Message:
                return ScreenLayer.LAYER2;

            case InGameScreensTeenPatti.Reconnecting:
                return ScreenLayer.LAYER3;

            case InGameScreensTeenPatti.Loading:
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
        InGameManagerTeenPatti.instance.LoadMainMenu();
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

    public void CallEmojiSocket(int index) {
        emojiIndex = index;
        // Debug.LogError("i am here------------ call emoji index "+index+"   "+emojiIndex+"    "+otherId);
        SocketControllerTeenPatti.instance.SentEmoji(otherId, InGameUiManagerTeenPatti.instance.emojiIndex);


    }

    public void ShowEmojiOnScreen(string str)
    {

        //Debug.LogError("Show Emoji From List on Screen *** " + str);
        GameObject g = null;
        switch (str)
        {

            case "bluffing":
                g = Instantiate(EmojiPrefabs[(int)Emoji.Bluffing], EmojiShowTransform) as GameObject;
                break;
            case "youRaPro":
                g = Instantiate(EmojiPrefabs[(int)Emoji.YouRaPro], EmojiShowTransform) as GameObject;
                break;
            case "beerCheers":
                g = Instantiate(EmojiPrefabs[(int)Emoji.BeerCheers], EmojiShowTransform) as GameObject;
                break;
            case "murgi":
                g = Instantiate(EmojiPrefabs[(int)Emoji.Murgi], EmojiShowTransform) as GameObject;
                break;
            case "rocket":
                g = Instantiate(EmojiPrefabs[(int)Emoji.Rocket], EmojiShowTransform) as GameObject;
                break;
            case "dung":
                g = Instantiate(EmojiPrefabs[(int)Emoji.Dung], EmojiShowTransform) as GameObject;
                break;
            case "oscar":
                Debug.LogError("oscar Emoji From List on Screen 00000*** " + str);
                g = Instantiate(EmojiPrefabs[(int)Emoji.Oscar], EmojiShowTransform) as GameObject;
                break;
            case "donkey":
                Debug.LogError("donkey Emoji From List on Screen *** " + str);
                g = Instantiate(EmojiPrefabs[(int)Emoji.Donkey], EmojiShowTransform) as GameObject;
                break;
            case "thumbUp":
                g = Instantiate(EmojiPrefabs[(int)Emoji.ThumbsUp], EmojiShowTransform) as GameObject;
                break;
            case "cherees":
                g = Instantiate(EmojiPrefabs[(int)Emoji.Cherees], EmojiShowTransform) as GameObject;
                break;
            case "kiss":
                g = Instantiate(EmojiPrefabs[(int)Emoji.Kiss], EmojiShowTransform) as GameObject;
                break;
            case "fish":
                g = Instantiate(EmojiPrefabs[(int)Emoji.Fish], EmojiShowTransform) as GameObject;
                break;
            case "gun":
                g = Instantiate(EmojiPrefabs[(int)Emoji.Gun], EmojiShowTransform) as GameObject;
                break;
            case "rose":
                g = Instantiate(EmojiPrefabs[(int)Emoji.Rose], EmojiShowTransform) as GameObject;
                break;
            case "perfume":
                g = Instantiate(EmojiPrefabs[(int)Emoji.Perfume], EmojiShowTransform) as GameObject;
                break;
            case "ring":
                g = Instantiate(EmojiPrefabs[(int)Emoji.Ring], EmojiShowTransform) as GameObject;
                break;
            case "car":
                g = Instantiate(EmojiPrefabs[(int)Emoji.Car], EmojiShowTransform) as GameObject;
                break;
        }
        g.transform.SetParent(EmojiShowTransform);
        g.GetComponent<EmojiBehaviour>().target = fromEmojiShowTransform;
    }

    
    public void OnGetEmoji(string serverResponse)
    {
        // [{ "Status":true,"message":"Success","sentBy":"52","sentTo":"0","emojiIndex":"2","balanceDiamond":208990.0}]
        JsonData data = JsonMapper.ToObject(serverResponse);
       
        if (data[0]["Status"].ToString().Equals("1.0")|| data[0]["Status"].ToString().Equals("1"))
        {
            for (int i = 0; i < players.transform.childCount; i++)
            {
                if (players.transform.GetChild(i).GetComponent<PlayerScriptTeenPatti>().playerData.userId == data[0]["sentBy"].ToString())
                {
                    EmojiShowTransform = players.transform.GetChild(i).GetChild(0).Find("Emoji").transform;
                   
                    break;
                }
            }
            for (int i = 0; i < players.transform.childCount; i++)
            {
                if (data[0]["sentTo"].ToString().Equals("0"))
                {
                    fromEmojiShowTransform = GirlDealerEmoji.transform;
                    sentToEmojiValue = "Dealer";
                }
                else
                {
                    if (players.transform.GetChild(i).GetComponent<PlayerScriptTeenPatti>().playerData.userId == data[0]["sentTo"].ToString())
                    {
                       fromEmojiShowTransform = players.transform.GetChild(i).GetChild(0).Find("Emoji").transform;
                        
                        break;
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


    public void OnCardSeen(string serverResponse)
    {
        // [{ "Status":true,"message":"Success","sentBy":"52","sentTo":"0","emojiIndex":"2","balanceDiamond":208990.0}]
        JsonData data = JsonMapper.ToObject(serverResponse);

        Debug.LogError("CardSeenData is :" + data.ToJson());

        //To Open Card

        //   InGameManagerTeenPatti.instance.OnOpenCards();

        PlayerScriptTeenPatti cardSeenPlayer = InGameManagerTeenPatti.instance.GetPlayerObject(data[0]["userId"].ToString());
        cardSeenPlayer.cardSeenButton.SetActive(false);

        if (cardSeenPlayer.IsMe())
        {

            cardSeenPlayer.playerData.isBlind = data[0]["isBlind"].Equals(true);

            if (!cardSeenPlayer.playerData.isBlind)
            {
                GameConstants.playerblind = false;
            }

            if (cardSeenPlayer.playerData.isShow)
            {
                GameConstants.showmatch = true;
            }

            cardSeenPlayer.playerData.totalBet = int.Parse(data[0]["minBet"].ToString());

            JsonData newData = data[0]["playerCards"];
           
            InGameUiManagerTeenPatti.instance.ConvertActionButtons(true, cardSeenPlayer, true, (int)cardSeenPlayer.playerData.totalBet);


            InGameManagerTeenPatti.instance.OnOpenCardsDataFound(newData, cardSeenPlayer);
        }

    }


    public void OnPlayerReseat(string serverResponse)
    {
        // [{ "Status":true,"message":"Success","sentBy":"52","sentTo":"0","emojiIndex":"2","balanceDiamond":208990.0}]
        JsonData data = JsonMapper.ToObject(serverResponse);

        Debug.LogError("ReseatData is :" + data.ToJson());

        //PlayerScriptTeenPatti cardSeenPlayer = InGameManagerTeenPatti.instance.GetPlayerObject(data[0]["userId"].ToString());

        //To Open Card

        //   InGameManagerTeenPatti.instance.OnOpenCards();





    }

    public void OnClickShowMatch()
    {
        StopCoroutine("CountDownAnimation");
        PlayerScriptTeenPatti.instance.fx_holder.gameObject.SetActive(false);
        PlayerScriptTeenPatti.instance.timerBar.gameObject.SetActive(false);
        PlayerScriptTeenPatti.instance.timerBar.fillAmount = 0;
        GameConstants.timerStart++;
        SocketControllerTeenPatti.instance.SendShowMatch();
        //InGameManagerTeenPatti.instance.OnPlayerActionCompleted(PlayerAction.Call, (int)availableCallAmount, "Call");
        //InGameUiManagerTeenPatti.instance.ToggleActionButton(false, null, false, 0);
    }

    public void OnClickSideShowRequest()
    {
        SocketControllerTeenPatti.instance.OnSideShowUserCalled();
        

    }

    public void OnClickSideShowWinner(GameObject btn)
    {
        btn.SetActive(false);
        rejectSideShowBtn.SetActive(false);
        SocketControllerTeenPatti.instance.OnSideShowWinnerCalled();
        PlayerScriptTeenPatti cardSeenPlayer = InGameManagerTeenPatti.instance.GetPlayerObject(GameConstants.sideShowRequesterId);
        if(cardSeenPlayer.IsMe())
        {
            sideShowRequesterPlayer.text = cardSeenPlayer.playerData.userName + " accept sideshow request";
            StartCoroutine(DisableNotification());
        }


    }


    public void OnClickRejectSideShowWinner(GameObject btn)
    {
        btn.SetActive(false);
        acceptSideShowButton.SetActive(false);
        SocketControllerTeenPatti.instance.OnSideShowRejectCalled();
        PlayerScriptTeenPatti cardSeenPlayer = InGameManagerTeenPatti.instance.GetPlayerObject(GameConstants.sideShowRequesterId);
        if (cardSeenPlayer.IsMe())
        {
            sideShowRequesterPlayer.text = cardSeenPlayer.playerData.userName + " reject sideshow request";
            StartCoroutine(DisableNotification());
        }


    }

    public void OnShowMatch(string serverResponse)
    {
        // [{ "Status":true,"message":"Success","sentBy":"52","sentTo":"0","emojiIndex":"2","balanceDiamond":208990.0}]
        JsonData data = JsonMapper.ToObject(serverResponse);

        Debug.LogError("ShowMatch is :" + data.ToJson());
        InGameManagerTeenPatti.instance.PlayerTimerReset();
        
        PlayerScriptTeenPatti cardSeenPlayer1 = InGameManagerTeenPatti.instance.GetPlayerObject(data[0]["p1"]["userId"].ToString());
        cardSeenPlayer1.DisableCardSeenBtn();
        JsonData newData1 = data[0]["p1"]["cards"];
        InGameManagerTeenPatti.instance.OnOpenCardsDataFoundShowCard(newData1, cardSeenPlayer1);

        PlayerScriptTeenPatti cardSeenPlayer2 = InGameManagerTeenPatti.instance.GetPlayerObject(data[0]["p2"]["userId"].ToString());
        cardSeenPlayer2.DisableCardSeenBtn();
        JsonData newData2 = data[0]["p2"]["cards"];
        InGameManagerTeenPatti.instance.OnOpenCardsDataFoundShowCard(newData2, cardSeenPlayer2);


    }

    bool requestData = false;

    public void OnSideShow(string serverResponse)
    {
        // [{ "Status":true,"message":"Success","sentBy":"52","sentTo":"0","emojiIndex":"2","balanceDiamond":208990.0}]
        JsonData data = JsonMapper.ToObject(serverResponse);

        Debug.LogError("SideShow is :" + data.ToJson());
        PlayerScriptTeenPatti cardSeenPlayer = InGameManagerTeenPatti.instance.GetPlayerObject(data[0]["from"].ToString());
        GameConstants.sideShowRequesterId = data[0]["from"].ToString();
        string requesterPlayer = cardSeenPlayer.playerData.userName;
        if (cardSeenPlayer.IsMe())
        {
            sideShowRequesterPlayer.text = requesterPlayer + " send you sideshow request";
            StartCoroutine(DisableNotification());
        }
        cardSeenPlayer.sideShowWinnerAcceptBtn.SetActive(true);
        cardSeenPlayer.sideShowRejectBtn.SetActive(true);
        
        // GameConstants.sideShowRequesterId = int.Parse(data["0"].ToString());








    }

    public void OnSideShowRequestReject(string serverResponse)
    {
        // [{ "Status":true,"message":"Success","sentBy":"52","sentTo":"0","emojiIndex":"2","balanceDiamond":208990.0}]
        JsonData data = JsonMapper.ToObject(serverResponse);

        Debug.LogError("SideShowReject is :" + data.ToJson());
        

        // GameConstants.sideShowRequesterId = int.Parse(data["0"].ToString());








    }


    IEnumerator DisableNotification()
    {
        yield return new WaitForSeconds(3f);
        sideShowRequesterPlayer.text = "";
    }


    public void OnSideShowWinner(string serverResponse)
    {

        // [{ "Status":true,"message":"Success","sentBy":"52","sentTo":"0","emojiIndex":"2","balanceDiamond":208990.0}]
        JsonData data = JsonMapper.ToObject(serverResponse);

        Debug.LogError("SideShowWinner is :" + data.ToJson());

        string status = data[0]["status"].ToString();

        if(status == "loser")
        {
            OnClickOnButton("fold");
        }
    }

    public void OnSeatObjectPlayer(string serverResponse)
    {

        // [{ "Status":true,"message":"Success","sentBy":"52","sentTo":"0","emojiIndex":"2","balanceDiamond":208990.0}]
        JsonData data = JsonMapper.ToObject(serverResponse);

        Debug.LogError("On Seat Object is :" + data.ToJson());





    }
}



public class InGameActiveScreensTeenPatti
{
    public GameObject screenObject;
    public InGameScreensTeenPatti screenName;
    public ScreenLayer screenLayer;
}

public enum InGameScreensTeenPatti
{
    Message,
    Loading,
    Reconnecting,
    TopUp,
    Menu,
    HandHistory,
    Chat,
    InGameShop,
    RealTimeResult,
    TableSettings,
    HandRanking,
    Missions,
    EmojiScreen,
    PointEarnMsg,
    Tips,
    SpinWheelScreen,
    DealerImageScreen,
    SwitchTable,
    GameDisplay,
    MenuTeenPatti
}

public enum PlayerActionTeenPatti
{
    Call,
    Raise,
    Fold,
    Check,
    Chaal,
    Blind
}

public enum SuggestionActionsTeenPatti
{
    Call,
    Call_Any,
    Fold,
    Check,
    Chaal,
    Blind,
    Null
}

public enum EmojiTeenPatti
{
    BeerCheers,
    Bluffing,
    Cherees,
    DangerBomb,
    Donkey,
    Dung,
    Fish,
    Gun,
    Kiss,
    Murgi,
    Oscar,
    Rocket,
    ThumbsUp,
    YouRaPro,
    Rose,
    Perfume,
    Ring,
    Car
}

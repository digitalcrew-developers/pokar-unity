using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class ClubTableController : MonoBehaviour
{
    public static ClubTableController instance;

    [Header("NLH")]
    public Button RingGameTabButton_NLH;
    public Button SNGGameTabButton_NLH;
    public Button MTTGameTabButton_NLH;
    public GameObject RingGamePanel_NLH, SNGamePanel_NLH, MTTGamePanel_NLH;
    public List<GameObject> NLH_RingGameSettings = new List<GameObject>();
    public List<GameObject> NLH_SNGGameSettings = new List<GameObject>();
    public List<GameObject> NLH_MTTGameSettings = new List<GameObject>();
    public List<TMP_Text> NLH_RingGameSliderTexts = new List<TMP_Text>();
    public List<TMP_Text> NLH_SNGSliderTexts = new List<TMP_Text>();
    public List<TMP_Text> NLH_MTTSliderTexts = new List<TMP_Text>();

    public Toggle EVChop;
    public GameObject EVChopValueField;

    [Header("PLO")]
    public Button RingGameTabButton_PLO;
    public Button SNGGameTabButton_PLO;
    public Button MTTGameTabButton_PLO;
    public GameObject RingGamePanel_PLO, SNGamePanel_PLO, MTTGamePanel_PLO;
    public List<GameObject> PLO_RingGameSettings = new List<GameObject>();
    public List<GameObject> PLO_SNGGameSettings = new List<GameObject>();
    public List<GameObject> PLO_MTTGameSettings = new List<GameObject>();

    public Toggle EVChop_PLO;
    public GameObject EVChopValueField_PLO;

    [Header("MIXED GAME")]
    public Button RingGameTabButton_MIXED;
    public Button SNGGameTabButton_MIXED;
    public Button MTTGameTabButton_MIXED;
    public GameObject RingGamePanel_MIXED, SNGamePanel_MIXED, MTTGamePanel_MIXED;
    public List<GameObject> MIXED_RingGameSettings = new List<GameObject>();
    public List<GameObject> MIXED_SNGGameSettings = new List<GameObject>();
    public List<GameObject> MIXED_MTTGameSettings = new List<GameObject>();

    public Toggle EVChop_MIXED;
    public GameObject EVChopValueField_MIXED;

    //public static readonly string Save_Folder = Application.persistentDataPath + "/ClubData";

    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
        }

        //if(!Directory.Exists(Save_Folder))
        //{
        //    Directory.CreateDirectory(Save_Folder);
        //}
    }

    private void Start()
    {
        Initialise();
    }

    private void Initialise()
    {
        RingGameTabButton_NLH.onClick.RemoveAllListeners();
        SNGGameTabButton_NLH.onClick.RemoveAllListeners();
        MTTGameTabButton_NLH.onClick.RemoveAllListeners();


        RingGameTabButton_NLH.onClick.AddListener(() => OpenScreen("Ring"));
        SNGGameTabButton_NLH.onClick.AddListener(() => OpenScreen("SNG"));
        MTTGameTabButton_NLH.onClick.AddListener(() => OpenScreen("MTT"));

        EVChop.onValueChanged.AddListener(delegate {
            ToggleValueChanged(EVChop);
        });
    }

    void ToggleValueChanged(Toggle change)
    {
        if (change.isOn)
        {
            EVChopValueField.SetActive(true);
        }
        else
        {
            EVChopValueField.SetActive(false);
        }
    }


    private void OpenScreen(string screenName)
    {
        Color c = new Color(1, 1, 1, 1);
        Color c1 = new Color(1, 1, 1, 0);

        switch (screenName)
        {
            case "Ring":
                RingGameTabButton_NLH.GetComponent<Image>().color = c;
                SNGGameTabButton_NLH.GetComponent<Image>().color = c1;
                MTTGameTabButton_NLH.GetComponent<Image>().color = c1;

                RingGamePanel_NLH.SetActive(true);
                SNGamePanel_NLH.SetActive(false);
                MTTGamePanel_NLH.SetActive(false);
                break;
            case "SNG":
                RingGameTabButton_NLH.GetComponent<Image>().color = c1;
                SNGGameTabButton_NLH.GetComponent<Image>().color = c;
                MTTGameTabButton_NLH.GetComponent<Image>().color = c1;

                RingGamePanel_NLH.SetActive(false);
                SNGamePanel_NLH.SetActive(true);
                MTTGamePanel_NLH.SetActive(false);
                break;
            case "MTT":
                RingGameTabButton_NLH.GetComponent<Image>().color = c1;
                SNGGameTabButton_NLH.GetComponent<Image>().color = c1;
                MTTGameTabButton_NLH.GetComponent<Image>().color = c;

                RingGamePanel_NLH.SetActive(false);
                SNGamePanel_NLH.SetActive(false);
                MTTGamePanel_NLH.SetActive(true);
                break;
            default:
                break;
        }
    }

    public void OnClickOnButton(string eventName)
    {
        SoundManager.instance.PlaySound(SoundType.Click);

        switch (eventName)
        {
            case "SaveRingGameNLH":
                {
                    OnClickOnSave("NLH","Ring Game");
                }
                break;
            case "SaveSNGNLH":
                {
                    OnClickOnSave("NLH", "SNG");
                }
                break;
            case "SaveMTTNLH":
                {
                    OnClickOnSave("NLH", "MTT");
                }
                break;
            case "CreateRingGameNLH":
                {
                    //OnClickOnSave("NLH", "Ring Game");
                }
                break;
            case "CreateSNGNLH":
                {
                    //OnClickOnSave("NLH", "SNG");
                }
                break;
            case "CreateMTTNLH":
                {
                    //OnClickOnSave("NLH", "MTT");
                }
                break;            
        }
    }

    public void OnSliderValueChange(string name)
    {
        switch(name)
        {
            case "ringGame_blinds":
                NLH_RingGameSliderTexts[0].text = "Blinds: " + NLH_RingGameSettings[4].transform.GetComponent<Slider>().value.ToString() + "/" + (NLH_RingGameSettings[4].transform.GetComponent<Slider>().value * 2).ToString();
                break;

            case "ringGame_buyIn":
                NLH_RingGameSliderTexts[1].text = "BuyIn: " + NLH_RingGameSettings[5].transform.GetComponent<RangeSlider>().LowValue.ToString();
                NLH_RingGameSliderTexts[2].text = "Max: " + NLH_RingGameSettings[5].transform.GetComponent<RangeSlider>().HighValue.ToString();
                break;

            case "ringGame_VPIP":
                NLH_RingGameSliderTexts[3].text = "Min. VPIP: " + NLH_RingGameSettings[6].transform.GetComponent<Slider>().value.ToString() + "%";
                break;

            case "ringGame_VPIPLevel":
                NLH_RingGameSliderTexts[4].text = "VPIP Level: " + NLH_RingGameSettings[7].transform.GetComponent<Slider>().value.ToString() + "%";
                break;

            case "ringGame_Time":
                NLH_RingGameSliderTexts[5].text = "Times: " + NLH_RingGameSettings[11].transform.GetComponent<Slider>().value.ToString();
                break;

            case "SNG_StartingChips":
                NLH_SNGSliderTexts[0].text = "Starting Chips: " + NLH_SNGGameSettings[5].transform.GetComponent<Slider>().value.ToString() + "BB";
                break;

            case "SNG_BlindsUp":
                NLH_SNGSliderTexts[1].text = "Blinds Up: " + NLH_SNGGameSettings[6].transform.GetComponent<Slider>().value.ToString() + "min";
                break;

            case "MTT_StartingChips":
                NLH_MTTSliderTexts[0].text = "Starting Chips: " + NLH_MTTGameSettings[9].transform.GetComponent<Slider>().value.ToString();
                break;

            case "MTT_BlindsUp":
                NLH_MTTSliderTexts[1].text = "Blinds Up: " + NLH_MTTGameSettings[10].transform.GetComponent<Slider>().value.ToString() + "min";
                break;

            case "MTT_LaterRegistrationLevel":
                NLH_MTTSliderTexts[2].text = "Later Registration: Level " + NLH_MTTGameSettings[11].transform.GetComponent<Slider>().value.ToString();
                break;

            case "MTT_Players":
                NLH_MTTSliderTexts[3].text = "Min Players: " + NLH_MTTGameSettings[12].transform.GetComponent<RangeSlider>().LowValue.ToString();
                NLH_MTTSliderTexts[4].text = "Max Players: " + NLH_MTTGameSettings[12].transform.GetComponent<RangeSlider>().HighValue.ToString();
                break;
        }        
    }

    private void OnClickOnSave(string gameType, string templateType)
    {
        // ---------------- Ring Game Toggle ----------------
        //Ring Game ActionTime Toggle Group
        string actionTime_RingGame = "";
        Toggle[] toggles_ActionTimeRingGame = NLH_RingGameSettings[2].transform.GetComponentsInChildren<Toggle>();
        foreach (var t in toggles_ActionTimeRingGame)
        {
            if (t.isOn)
            {
                actionTime_RingGame = t.name;
                Debug.Log("Toggle Name:" + actionTime_RingGame);
            }
        }

        //Ring Game AutoStartWith Toggle Group
        string autoStartWith = "";
        Toggle[] toggles_AutoStartWith = NLH_RingGameSettings[9].transform.GetComponentsInChildren<Toggle>();
        foreach (var t in toggles_AutoStartWith)
        {
            if (t.isOn)
            {
                autoStartWith = t.name;
                Debug.Log("Toggle Name:" + autoStartWith);
            }
        }
        //------------------------------------------------

        // ---------------- SNG Toggle ----------------
        //SNG ActionTime Toggle Group
        string actionTime_SNG = "";
        Toggle[] toggles_ActionTimeSNG = NLH_SNGGameSettings[2].transform.GetComponentsInChildren<Toggle>();
        foreach (var t in toggles_ActionTimeSNG)
        {
            if (t.isOn)
            {
                actionTime_SNG = t.name;
                Debug.Log("Toggle Name:" + actionTime_SNG);
            }
        }

        string maxMultiplier_SNG = "";
        Toggle[] toggles_maxMultiplierSNG = NLH_SNGGameSettings[7].transform.GetComponentsInChildren<Toggle>();
        foreach (var t in toggles_maxMultiplierSNG)
        {
            if (t.isOn)
            {
                maxMultiplier_SNG = t.name;
                Debug.Log("Toggle Name:" + maxMultiplier_SNG);
            }
        }

        string blindStructure_SNG = "";
        Toggle[] toggles_blindStructureSNG = NLH_SNGGameSettings[8].transform.GetComponentsInChildren<Toggle>();
        foreach (var t in toggles_blindStructureSNG)
        {
            if (t.isOn)
            {
                blindStructure_SNG = t.name;
                Debug.Log("Toggle Name:" + blindStructure_SNG);
            }
        }

        //--------------------------------------------

        // ---------------- MTT Toggle ----------------
        //MTT ActionTime Toggle Group
        string actionTime_MTT = "";
        Toggle[] toggles_ActionTimeMTT = NLH_MTTGameSettings[2].transform.GetComponentsInChildren<Toggle>();
        foreach (var t in toggles_ActionTimeMTT)
        {
            if (t.isOn)
            {
                actionTime_MTT = t.name;
                Debug.Log("Toggle Name:" + actionTime_MTT);
            }
        }

        string blindStructure_MTT = "";
        Toggle[] toggles_blindStructureMTT = NLH_MTTGameSettings[13].transform.GetComponentsInChildren<Toggle>();
        foreach (var t in toggles_blindStructureMTT)
        {
            if (t.isOn)
            {
                blindStructure_MTT = t.name;
                Debug.Log("Toggle Name:" + blindStructure_MTT);
            }
        }

        string prizePool_MTT = "";
        Toggle[] toggles_prizePoolMTT = NLH_MTTGameSettings[19].transform.GetComponentsInChildren<Toggle>();
        foreach (var t in toggles_prizePoolMTT)
        {
            if (t.isOn)
            {
                prizePool_MTT = t.name;
                Debug.Log("Toggle Name:" + prizePool_MTT);
            }
        }

        //---------------------------------------------

        //Request Data
        string requestData = "";

        if (templateType.Equals("Ring Game"))
        {
            requestData = "{\"clubId\":\"" + ClubDetailsUIManager.instance.GetClubId() + "\"," +
                "\"gameType\":\"" + gameType + "\"," +
                "\"templateType\":\"" + templateType + "\"," +
                "\"status\":\"" + "Saved" + "\"," +
                "\"tableId\":\"" + "" + "\"," +
                "\"settingData\":[{\"templateSubType\":\"" + NLH_RingGameSettings[0].transform.GetComponent<Text>().text.ToString() + "\"," +
                "\"memberCount\":\"" + NLH_RingGameSettings[1].transform.GetComponent<TMP_Text>().text.ToString() + "\"," +
                "\"actionTime\":\"" + actionTime_RingGame + "\"," +
                "\"exclusiveTable\":\"" + (NLH_RingGameSettings[3].transform.GetComponent<ToggleController>().isOn.ToString().Equals("True") ? "On" : "Off") + "\"," +
                "\"blinds\":\"" + NLH_RingGameSettings[4].transform.GetComponent<Slider>().value.ToString() + "\"," +
                "\"buyInMin\":\"" + NLH_RingGameSettings[5].transform.GetComponent<RangeSlider>().LowValue.ToString() + "\"," +
                "\"buyInMax\":\"" + NLH_RingGameSettings[5].transform.GetComponent<RangeSlider>().HighValue.ToString() + "\"," +
                "\"minVPIP\":\"" + NLH_RingGameSettings[6].transform.GetComponent<Slider>().value.ToString() + "\"," +
                "\"VPIPLevel\":\"" + NLH_RingGameSettings[7].transform.GetComponent<Slider>().value.ToString() + "\"," +
                "\"handsThreshold\":\"" + "" + "\"," +
                "\"autoStart\":\"" + (NLH_RingGameSettings[8].transform.GetComponent<ToggleController>().isOn.ToString().Equals("True") ? "On" : "Off") + "\"," +
                "\"autoStartWith\":\"" + autoStartWith + "\"," +
                "\"autoExtension\":\"" + (NLH_RingGameSettings[10].transform.GetComponent<ToggleController>().isOn.ToString().Equals("True") ? "On" : "Off") + "\"," +
                "\"autoExtensionTimes\":\"" + NLH_RingGameSettings[11].transform.GetComponent<Slider>().value.ToString() + "\"," +
                "\"autoOpen\":\"" + (NLH_RingGameSettings[12].transform.GetComponent<ToggleController>().isOn.ToString().Equals("True") ? "On" : "Off") + "\"," +
                "\"riskManagement\":\"" + "" + "\"," +
                "\"fee\":\"" + NLH_RingGameSettings[14].transform.GetComponent<TMP_Text>().text.ToString().Substring(0, NLH_RingGameSettings[14].transform.GetComponent<TMP_Text>().text.ToString().Length - 1) + "\"," +
                "\"cap\":\"" + NLH_RingGameSettings[15].transform.GetComponent<TMP_Text>().text.ToString().Substring(0, NLH_RingGameSettings[15].transform.GetComponent<TMP_Text>().text.ToString().Length - 1) + "\"," +
                "\"calltime\":\"" + (NLH_RingGameSettings[16].transform.GetComponent<ToggleController>().isOn.ToString().Equals("True") ? "On" : "Off") + "\"," +
                "\"authorizedBuyIn\":\"" + (NLH_RingGameSettings[17].transform.GetComponent<ToggleController>().isOn.ToString().Equals("True") ? "On" : "Off") + "\"," +
                "\"GPSRestriction\":\"" + (NLH_RingGameSettings[18].transform.GetComponent<ToggleController>().isOn.ToString().Equals("True") ? "On" : "Off") + "\"," +
                "\"IPRestriction\":\"" + (NLH_RingGameSettings[19].transform.GetComponent<ToggleController>().isOn.ToString().Equals("True") ? "On" : "Off") + "\"," +
                "\"banChatting\":\"" + (NLH_RingGameSettings[20].transform.GetComponent<ToggleController>().isOn.ToString().Equals("True") ? "On" : "Off") + "\"," +
                "\"hours\":\"" + NLH_RingGameSettings[21].transform.GetComponent<Slider>().value.ToString() + "\"}]}";
        }
        else if (templateType.Equals("SNG"))
        {
            requestData = "{\"clubId\":\"" + ClubDetailsUIManager.instance.GetClubId() + "\"," +
            "\"gameType\":\"" + gameType + "\"," +
            "\"templateType\":\"" + templateType + "\"," +
            "\"status\":\"" + "Saved" + "\"," +
            "\"tableId\":\"" + "" + "\"," +
            "\"settingData\":[{\"templateSubType\":\"" + NLH_SNGGameSettings[0].transform.GetComponent<Text>().text.ToString() + "\"," +
            "\"memberCount\":\"" + NLH_SNGGameSettings[1].transform.GetComponent<TMP_Text>().text.ToString() + "\"," +
            "\"actionTime\":\"" + actionTime_SNG + "\"," +
            "\"exclusiveTable\":\"" + (NLH_SNGGameSettings[3].transform.GetComponent<ToggleController>().isOn.ToString().Equals("True") ? "On" : "Off") + "\"," +
            "\"buyIn\":\"" + NLH_SNGGameSettings[4].transform.GetComponent<TMP_InputField>().text.ToString() + "\"," +
            "\"startingChips\":\"" + NLH_SNGGameSettings[5].transform.GetComponent<Slider>().value.ToString() + "\"," +
            "\"blindsUp\":\"" + NLH_SNGGameSettings[6].transform.GetComponent<Slider>().value.ToString() + "\"," +
            "\"maxMultiplier\":\"" + maxMultiplier_SNG + "\"," +
            "\"blindStructure\":\"" + blindStructure_SNG + "\"," +
            "\"AutoOpen\":\"" + (NLH_SNGGameSettings[9].transform.GetComponent<ToggleController>().isOn.ToString().Equals("True") ? "On" : "Off") + "\"," +
            "\"authorizedRegister\":\"" + (NLH_SNGGameSettings[10].transform.GetComponent<ToggleController>().isOn.ToString().Equals("True") ? "On" : "Off") + "\"," +
            "\"GPSRestriction\":\"" + (NLH_SNGGameSettings[11].transform.GetComponent<ToggleController>().isOn.ToString().Equals("True") ? "On" : "Off") + "\"," +
            "\"IPRestriction\":\"" + (NLH_SNGGameSettings[12].transform.GetComponent<ToggleController>().isOn.ToString().Equals("True") ? "On" : "Off") + "\"," +
            "\"banChatting\":\"" + (NLH_SNGGameSettings[13].transform.GetComponent<ToggleController>().isOn.ToString().Equals("True") ? "On" : "Off") + "\"}]}";
        }
        else if (templateType.Equals("MTT"))
        {
            requestData = "{\"clubId\":\"" + ClubDetailsUIManager.instance.GetClubId() + "\"," +
               "\"gameType\":\"" + gameType + "\"," +
               "\"templateType\":\"" + templateType + "\"," +
               "\"status\":\"" + "Saved" + "\"," +
               "\"tableId\":\"" + "" + "\"," +
               "\"settingData\":[{\"templateSubType\":\"" + NLH_MTTGameSettings[0].transform.GetComponent<Text>().text.ToString() + "\"," +
               "\"memberCount\":\"" + NLH_MTTGameSettings[1].transform.GetComponent<TMP_Text>().text.ToString() + "\"," +
               "\"actionTime\":\"" + actionTime_MTT + "\"," +
               "\"exclusiveTable\":\"" + (NLH_MTTGameSettings[3].transform.GetComponent<ToggleController>().isOn.ToString().Equals("True") ? "On" : "Off") + "\"," +
               "\"buyIn\":\"" + NLH_MTTGameSettings[4].transform.GetComponent<TMP_InputField>().text.ToString() + "\"," +
               "\"rebuy\":\"" + NLH_MTTGameSettings[5].transform.GetComponent<TMP_InputField>().text.ToString() + "\"," +
               "\"addOn\":\"" + NLH_MTTGameSettings[6].transform.GetComponent<TMP_InputField>().text.ToString() + "\"," +
               "\"rebuyTimes\":\"" + NLH_MTTGameSettings[7].transform.GetComponent<TMP_Text>().text.ToString() + "\"," +
               "\"addOnX\":\"" + NLH_MTTGameSettings[8].transform.GetComponent<TMP_Text>().text.ToString() + "\"," +
               "\"startingChips\":\"" + NLH_MTTGameSettings[9].transform.GetComponent<Slider>().value.ToString() + "\"," +
               "\"blindsUp\":\"" + NLH_MTTGameSettings[10].transform.GetComponent<Slider>().value.ToString() + "\"," +
               "\"lateRegistrationLevel\":\"" + NLH_MTTGameSettings[11].transform.GetComponent<Slider>().value.ToString() + "\"," +
               "\"minPlayers\":\"" + NLH_MTTGameSettings[12].transform.GetComponent<RangeSlider>().LowValue.ToString() + "\"," +
               "\"maxPlayers\":\"" + NLH_MTTGameSettings[12].transform.GetComponent<RangeSlider>().HighValue.ToString() + "\"," +
               "\"blindStructure\":\"" + blindStructure_SNG + "\"," +
               "\"earlyBird\":\"" + (NLH_MTTGameSettings[14].transform.GetComponent<ToggleController>().isOn.ToString().Equals("True") ? "On" : "Off") + "\"," +
               "\"KOBounty\":\"" + (NLH_MTTGameSettings[15].transform.GetComponent<ToggleController>().isOn.ToString().Equals("True") ? "On" : "Off") + "\"," +
               "\"authorizedRegister\":\"" + (NLH_MTTGameSettings[16].transform.GetComponent<ToggleController>().isOn.ToString().Equals("True") ? "On" : "Off") + "\"," +
               "\"banChatting\":\"" + (NLH_MTTGameSettings[17].transform.GetComponent<ToggleController>().isOn.ToString().Equals("True") ? "On" : "Off") + "\"," +
               "\"break\":\"" + (NLH_MTTGameSettings[18].transform.GetComponent<ToggleController>().isOn.ToString().Equals("True") ? "On" : "Off") + "\"," +
               "\"prizePool\":\"" + prizePool_MTT + "\"," +
               "\"GtdPrizePool\":\"" + (NLH_MTTGameSettings[20].transform.GetComponent<ToggleController>().isOn.ToString().Equals("True") ? "On" : "Off") + "\"," +
               "\"startTime\":\"" + NLH_MTTGameSettings[21].transform.GetComponent<TMP_Text>().text.ToString() + "\"}]}";
        }

        WebServices.instance.SendRequest(RequestType.CreateTemplate, requestData, true, OnServerResponseFound);
    }


    private void OnClickOnCreate(string gameType, string templateType)
    {
        //if (templateType.Equals("NLH"))
        //{
        //    string requestData = "{\"clubId\":\"" + ClubDetailsUIManager.instance.GetClubId() + "\"," +
        //        "\"gameType\":\"" + gameType + "\"," +
        //        "\"templateType\":\"" + templateType + "\"," +
        //        "\"status\":\"" + "Saved" + "\"," +
        //        "\"settingData\":[{\"templateSubType\":\"" + NLH_RingGameSettings[0].transform.GetComponent<Text>().text.ToString() + "\"," +
        //        "\"memberCount\":\"" + NLH_RingGameSettings[1].transform.GetComponent<TMP_Text>().text.ToString() + "\"," +
        //        "\"actionTime\":\"" + NLH_RingGameSettings[2].transform.GetComponent<Toggle>().isOn.ToString() + "\"," +
        //        "\"exclusiveTable\":\"" + NLH_RingGameSettings[3].transform.GetComponent<ToggleController>().isOn.ToString() + "\"," +
        //        "\"blinds\":\"" + NLH_RingGameSettings[4].transform.GetComponent<Slider>().value.ToString() + "\"," +
        //        "\"buyInMin\":\"" + NLH_RingGameSettings[5].transform.GetComponent<RangeSlider>().HighValue.ToString() + "\"," +
        //        "\"buyInMax\":\"" + NLH_RingGameSettings[5].transform.GetComponent<RangeSlider>().LowValue.ToString() + "\"," +
        //        "\"minVPIP\":\"" + NLH_RingGameSettings[6].transform.GetComponent<Slider>().value.ToString() + "\"," +
        //        "\"VPIPLevel\":\"" + NLH_RingGameSettings[7].transform.GetComponent<Slider>().value.ToString() + "\"," +
        //        "\"handsThreshold\":\"" + "" + "\"," +
        //        "\"autoStart\":\"" + NLH_RingGameSettings[8].transform.GetComponent<ToggleController>().isOn.ToString() + "\"," +
        //        "\"autoStartWith\":\"" + "" + "\"," +
        //        "\"autoExtension\":\"" + NLH_RingGameSettings[9].transform.GetComponent<ToggleController>().isOn.ToString() + "\"," +
        //        "\"autoExtensionTimes\":\"" + NLH_RingGameSettings[10].transform.GetComponent<Slider>().value.ToString() + "\"," +
        //        "\"autoOpen\":\"" + NLH_RingGameSettings[11].transform.GetComponent<ToggleController>().isOn.ToString() + "\"," +
        //        "\"riskManagement\":\"" + "" + "\"," +
        //        "\"fee\":\"" + NLH_RingGameSettings[13].transform.GetComponent<TMP_Dropdown>().value.ToString() + "\"," +
        //        "\"cap\":\"" + NLH_RingGameSettings[14].transform.GetComponent<TMP_Dropdown>().value.ToString() + "\"," +
        //        "\"calltime\":\"" + NLH_RingGameSettings[15].transform.GetComponent<ToggleController>().isOn.ToString() + "\"," +
        //        "\"authorizedBuyIn\":\"" + NLH_RingGameSettings[16].transform.GetComponent<ToggleController>().isOn.ToString() + "\"," +
        //        "\"GPSRestriction\":\"" + NLH_RingGameSettings[17].transform.GetComponent<ToggleController>().isOn.ToString() + "\"," +
        //        "\"IPRestriction\":\"" + NLH_RingGameSettings[18].transform.GetComponent<ToggleController>().isOn.ToString() + "\"," +
        //        "\"banChatting\":\"" + NLH_RingGameSettings[19].transform.GetComponent<ToggleController>().isOn.ToString() + "\"," +
        //        "\"hours\":\"" + NLH_RingGameSettings[20].transform.GetComponent<Slider>().value.ToString() + "\"}]}";


        //    WebServices.instance.SendRequest(RequestType.CreateTemplate, requestData, true, OnServerResponseFound);
        //}

    }


    public void OnServerResponseFound(RequestType requestType, string serverResponse, bool isShowErrorMessage, string errorMessage)
    {

        //MainMenuController.instance.DestroyScreen(MainMenuScreens.Loading);

        if (errorMessage.Length > 0)
        {
            if (isShowErrorMessage)
            {
                //MainMenuController.instance.ShowMessage(errorMessage);
                Debug.Log(errorMessage);
            }

            return;
        }



        switch (requestType)
        {
            case RequestType.UpdateTemplateStatus:
                {
                    JsonData data = JsonMapper.ToObject(serverResponse);

                    if (data["success"].ToString() == "1")
                    {
                        //createClubPopUp.SetActive(false);
                        //MainMenuController.instance.ShowMessage("Club created successfully");
                        //ClubListUiManager.instance.FetchList();
                    }
                    else
                    {
                        //MainMenuController.instance.ShowMessage(data["message"].ToString());
                    }
                }
                break;


            case RequestType.CreateTemplate:
                {
                    JsonData data = JsonMapper.ToObject(serverResponse);

                    if (data["success"].ToString() == "1")
                    {
                        Debug.Log(data["message"].ToString());
                        //joinClubPopUp.SetActive(false);
                        MainMenuController.instance.ShowMessage(data["message"].ToString());
                    }
                    else
                    {
                        Debug.Log(data["message"].ToString());
                        MainMenuController.instance.ShowMessage(data["message"].ToString());
                    }
                }
                break;
            
            default:
#if ERROR_LOG
                Debug.LogError("Unhandled requestType found in  MenuHandller = " + requestType);
#endif
                break;
        }
    }    
}

//[System.Serializable]
//public class RingGameData
//{
//    public string clubID, gameType, templateType, status;
//    public List<SettingDataRingGame> settingData;
//}

//[System.Serializable]
//public class SettingDataRingGame
//{
//    public string templateSubType;
//    public string memberCount;
//    public string actionTime;
//    public string exclusiveTable, blinds, buyInMin, buyInMax, minVPIP, VPIPLevel, handsThreshold, autoStart, autoStartWith, autoExtension,
//                  autoExtensionTimes, autoOpen, riskManagement, fee, cap, calltime, authorizedBuyIn, GPSRestriction, IPRestriction, banChatting, hours;
//}

//[System.Serializable]
//public class SNGData
//{
//    public string clubID, gameType, templateType, status;
//    public List<SettingDataSNG> settingData;
//}

//[System.Serializable]
//public class SettingDataSNG
//{
//    public string templateSubType, memberCount, actionTime, exclusiveTable, buyIn, startingChips, blindsUp, maxMultiplier, blindStructure, AutoOpen, authorizedRegister, GPSRestriction,
//                  IPRestriction, banChatting;
//}

//[System.Serializable]
//public class MTTData
//{
//    public string clubID, gameType, templateType, status;
//    public List<SettingDataMTT> settingData;
//}

//[System.Serializable]
//public class SettingDataMTT
//{
//    public string templateSubType, memberCount, actionTime, exclusiveTable, buyIn, rebuy, addOn, rebuyTimes, addOnX, startingChips, blindsUp, lateRegistrationLevel, minPlayers, maxPlayers,
//                  blindStructure, earlyBird, KOBounty, authorizedRegister, banChatting, breakData, prizePool, GtdPrizePool, startTime;
//}
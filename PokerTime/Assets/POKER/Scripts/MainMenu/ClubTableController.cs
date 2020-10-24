using DG.Tweening;
using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Permissions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class ClubTableController : MonoBehaviour
{
    public static ClubTableController instance;

    public GameObject templateObj;
    public Transform container;


    private int memberCount_NLH = 9;
    [Header("NLH")]
    public Button incrementMemberCount_NLH;
    public Button decreamentMemberCount_NLH;

    public TMP_Text handThresholdText;
    public Slider handThreshold;
    public Slider autoExtensionTime;
    public Slider callTime;

    public Button RingGame_RegularModeTabNLH;
    public Button RingGame_6PlusModeTabNLH;

    public Button RingGameTabButton_NLH;
    public Button SNGGameTabButton_NLH;
    public Button MTTGameTabButton_NLH;

    public GameObject BanChattingDiamondObj, RingGamePanel_NLH, SNGamePanel_NLH, MTTGamePanel_NLH;

    public List<GameObject> NLH_autoStartMemberList = new List<GameObject>();

    public List<GameObject> NLH_RingGameSettings = new List<GameObject>();
    public List<GameObject> NLH_SNGGameSettings = new List<GameObject>();
    public List<GameObject> NLH_MTTGameSettings = new List<GameObject>();

    [Space(5)]
    public List<TMP_Text> NLH_RingGameSliderTexts = new List<TMP_Text>();
    public List<TMP_Text> NLH_SNGSliderTexts = new List<TMP_Text>();
    public List<TMP_Text> NLH_MTTSliderTexts = new List<TMP_Text>();

    public Toggle EVChop;
    public GameObject EVChopValueField;


    private int memberCount_PLO = 9;
    [Header("PLO")]
    public Button incrementMemberCount_PLO;
    public Button decreamentMemberCount_PLO;
    public Button RingGameTabButton_PLO;
    public Button SNGGameTabButton_PLO;
    public Button MTTGameTabButton_PLO;
    public GameObject RingGamePanel_PLO, SNGamePanel_PLO, MTTGamePanel_PLO;
    public List<GameObject> PLO_RingGameSettings = new List<GameObject>();
    public List<GameObject> PLO_SNGGameSettings = new List<GameObject>();
    public List<GameObject> PLO_MTTGameSettings = new List<GameObject>();

    [Space(5)]
    public List<TMP_Text> PLO_RingGameSliderTexts = new List<TMP_Text>();
    public List<TMP_Text> PLO_SNGSliderTexts = new List<TMP_Text>();
    public List<TMP_Text> PLO_MTTSliderTexts = new List<TMP_Text>();

    public Toggle EVChop_PLO;
    public GameObject EVChopValueField_PLO;

    
    private int memberCount_MIXED = 9;
    [Header("MIXED GAME")]
    public Button incrementMemberCount_MIXED;
    public Button decreamentMemberCount_MIXED;
    public Button RingGameTabButton_MIXED;
    public Button SNGGameTabButton_MIXED;
    public Button MTTGameTabButton_MIXED;
    public GameObject RingGamePanel_MIXED, SNGamePanel_MIXED, MTTGamePanel_MIXED;
    public List<GameObject> MIXED_RingGameSettings = new List<GameObject>();
    public List<GameObject> MIXED_SNGGameSettings = new List<GameObject>();
    public List<GameObject> MIXED_MTTGameSettings = new List<GameObject>();

    [Space(5)]
    public List<TMP_Text> MIXED_RingGameSliderTexts = new List<TMP_Text>();
    public List<TMP_Text> MIXED_SNGSliderTexts = new List<TMP_Text>();
    public List<TMP_Text> MIXED_MTTSliderTexts = new List<TMP_Text>();

    public Toggle EVChop_MIXED;
    public GameObject EVChopValueField_MIXED;

    //public static readonly string Save_Folder = Application.persistentDataPath + "/ClubData";

    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
        }

        RequestTemplateData();
    }

    private void Start()
    {
        Initialise();
        InitializeNLHRingGameParameters("NLH_RegularMode");
    }

    private void InitializeNLHRingGameParameters(string templateSubType)
    {
        if(templateSubType.Equals("NLH_RegularMode"))
        {
            NLH_RingGameSettings.Add(RingGamePanel_NLH.transform.GetChild(0).Find("Content_RegularMode").Find("TemplateSubType").gameObject as GameObject);
            NLH_RingGameSettings.Add(RingGamePanel_NLH.transform.GetChild(0).Find("Content_RegularMode").Find("MemberCount").gameObject as GameObject);
            NLH_RingGameSettings.Add(RingGamePanel_NLH.transform.GetChild(0).Find("Content_RegularMode").Find("ActionTime").gameObject as GameObject);
            NLH_RingGameSettings.Add(RingGamePanel_NLH.transform.GetChild(0).Find("Content_RegularMode").Find("ExclusiveTableToggle").gameObject as GameObject);
            NLH_RingGameSettings.Add(RingGamePanel_NLH.transform.GetChild(0).Find("Content_RegularMode").Find("BlindsSlider").gameObject as GameObject);
            NLH_RingGameSettings.Add(RingGamePanel_NLH.transform.GetChild(0).Find("Content_RegularMode").Find("BuyInSlider").gameObject as GameObject);
            NLH_RingGameSettings.Add(RingGamePanel_NLH.transform.GetChild(0).Find("Content_RegularMode").Find("MinVPIP").gameObject as GameObject);
            NLH_RingGameSettings.Add(RingGamePanel_NLH.transform.GetChild(0).Find("Content_RegularMode").Find("VPIPLevel").gameObject as GameObject);
            NLH_RingGameSettings.Add(RingGamePanel_NLH.transform.GetChild(0).Find("Content_RegularMode").Find("AutoStartToggle").gameObject as GameObject);
            NLH_RingGameSettings.Add(RingGamePanel_NLH.transform.GetChild(0).Find("Content_RegularMode").Find("AutoStart").gameObject as GameObject);
            NLH_RingGameSettings.Add(RingGamePanel_NLH.transform.GetChild(0).Find("Content_RegularMode").Find("AutoStartExtensionToggle").gameObject as GameObject);
            NLH_RingGameSettings.Add(RingGamePanel_NLH.transform.GetChild(0).Find("Content_RegularMode").Find("TimesSlider").gameObject as GameObject);
            NLH_RingGameSettings.Add(RingGamePanel_NLH.transform.GetChild(0).Find("Content_RegularMode").Find("AutoOpenToggle").gameObject as GameObject);
            NLH_RingGameSettings.Add(RingGamePanel_NLH.transform.GetChild(0).Find("Content_RegularMode").Find("RiskManagement").gameObject as GameObject);
            NLH_RingGameSettings.Add(RingGamePanel_NLH.transform.GetChild(0).Find("Content_RegularMode").Find("Fee").gameObject as GameObject);
            NLH_RingGameSettings.Add(RingGamePanel_NLH.transform.GetChild(0).Find("Content_RegularMode").Find("Cap").gameObject as GameObject);
            NLH_RingGameSettings.Add(RingGamePanel_NLH.transform.GetChild(0).Find("Content_RegularMode").Find("CallTimeToggle").gameObject as GameObject);
            NLH_RingGameSettings.Add(RingGamePanel_NLH.transform.GetChild(0).Find("Content_RegularMode").Find("AuthorizedBuyInToggle").gameObject as GameObject);
            NLH_RingGameSettings.Add(RingGamePanel_NLH.transform.GetChild(0).Find("Content_RegularMode").Find("GPSRestrictionToggle").gameObject as GameObject);
            NLH_RingGameSettings.Add(RingGamePanel_NLH.transform.GetChild(0).Find("Content_RegularMode").Find("IPRestrictionToggle").gameObject as GameObject);
            NLH_RingGameSettings.Add(RingGamePanel_NLH.transform.GetChild(0).Find("Content_RegularMode").Find("BanChattingToggle").gameObject as GameObject);
            NLH_RingGameSettings.Add(RingGamePanel_NLH.transform.GetChild(0).Find("Content_RegularMode").Find("TimeMarkerSlider").gameObject as GameObject);

        }
        else if(templateSubType.Equals("NLH_6Plus"))
        {
            NLH_RingGameSettings.Add(RingGamePanel_NLH.transform.GetChild(0).Find("Content_6Plus").Find("TemplateSubType").gameObject as GameObject);
            NLH_RingGameSettings.Add(RingGamePanel_NLH.transform.GetChild(0).Find("Content_6Plus").Find("MemberCount").gameObject as GameObject);
            NLH_RingGameSettings.Add(RingGamePanel_NLH.transform.GetChild(0).Find("Content_6Plus").Find("ActionTime").gameObject as GameObject);
            NLH_RingGameSettings.Add(RingGamePanel_NLH.transform.GetChild(0).Find("Content_6Plus").Find("ExclusiveTableToggle").gameObject as GameObject);
            NLH_RingGameSettings.Add(RingGamePanel_NLH.transform.GetChild(0).Find("Content_6Plus").Find("AnteSlider").gameObject as GameObject);
            NLH_RingGameSettings.Add(RingGamePanel_NLH.transform.GetChild(0).Find("Content_6Plus").Find("BuyInSlider").gameObject as GameObject);
            NLH_RingGameSettings.Add(RingGamePanel_NLH.transform.GetChild(0).Find("Content_6Plus").Find("MinVPIP").gameObject as GameObject);
            NLH_RingGameSettings.Add(RingGamePanel_NLH.transform.GetChild(0).Find("Content_6Plus").Find("VPIPLevel").gameObject as GameObject);
            NLH_RingGameSettings.Add(RingGamePanel_NLH.transform.GetChild(0).Find("Content_6Plus").Find("AutoStartToggle").gameObject as GameObject);
            NLH_RingGameSettings.Add(RingGamePanel_NLH.transform.GetChild(0).Find("Content_6Plus").Find("AutoStart").gameObject as GameObject);
            NLH_RingGameSettings.Add(RingGamePanel_NLH.transform.GetChild(0).Find("Content_6Plus").Find("AutoStartExtensionToggle").gameObject as GameObject);
            NLH_RingGameSettings.Add(RingGamePanel_NLH.transform.GetChild(0).Find("Content_6Plus").Find("TimesSlider").gameObject as GameObject);
            NLH_RingGameSettings.Add(RingGamePanel_NLH.transform.GetChild(0).Find("Content_6Plus").Find("AutoOpenToggle").gameObject as GameObject);
            NLH_RingGameSettings.Add(RingGamePanel_NLH.transform.GetChild(0).Find("Content_6Plus").Find("RiskManagement").gameObject as GameObject);
            NLH_RingGameSettings.Add(RingGamePanel_NLH.transform.GetChild(0).Find("Content_6Plus").Find("Fee").gameObject as GameObject);
            NLH_RingGameSettings.Add(RingGamePanel_NLH.transform.GetChild(0).Find("Content_6Plus").Find("Cap").gameObject as GameObject);
            NLH_RingGameSettings.Add(RingGamePanel_NLH.transform.GetChild(0).Find("Content_6Plus").Find("ChipWIthdrawalToggle").gameObject as GameObject);
            NLH_RingGameSettings.Add(RingGamePanel_NLH.transform.GetChild(0).Find("Content_6Plus").Find("AuthorizedBuyInToggle").gameObject as GameObject);
            NLH_RingGameSettings.Add(RingGamePanel_NLH.transform.GetChild(0).Find("Content_6Plus").Find("GPSRestrictionToggle").gameObject as GameObject);
            NLH_RingGameSettings.Add(RingGamePanel_NLH.transform.GetChild(0).Find("Content_6Plus").Find("IPRestrictionToggle").gameObject as GameObject);
            NLH_RingGameSettings.Add(RingGamePanel_NLH.transform.GetChild(0).Find("Content_6Plus").Find("BanChattingToggle").gameObject as GameObject);
            NLH_RingGameSettings.Add(RingGamePanel_NLH.transform.GetChild(0).Find("Content_6Plus").Find("TimeMarkerSlider").gameObject as GameObject);
        }
    }


    private void Update()
    {
        if (memberCount_NLH < 9)
            incrementMemberCount_NLH.interactable = true;
        else
            incrementMemberCount_NLH.interactable = false;

        //if (memberCount_PLO < 9)
        //    incrementMemberCount_PLO.interactable = true;
        //else
        //    incrementMemberCount_PLO.interactable = false;

        //if (memberCount_MIXED < 9)
        //    incrementMemberCount_MIXED.interactable = true;
        //else
        //    incrementMemberCount_MIXED.interactable = false;

        if (memberCount_NLH > 2)
            decreamentMemberCount_NLH.interactable = true;
        else
            decreamentMemberCount_NLH.interactable = false;

        //if (memberCount_PLO > 2)
        //    decreamentMemberCount_PLO.interactable = true;
        //else
        //    decreamentMemberCount_PLO.interactable = false;

        //if (memberCount_MIXED > 2)
        //    decreamentMemberCount_MIXED.interactable = true;
        //else
        //    decreamentMemberCount_MIXED.interactable = false;

        //Members Toggle list Enable/Disable
        if(NLH_RingGameSettings[8].transform.GetComponent<ToggleController>().isOn)
        {
            NLH_RingGameSettings[9].transform.Find("Members").gameObject.SetActive(true);
            NLH_RingGameSettings[9].transform.GetComponent<RectTransform>().sizeDelta = new Vector2(608.5f, 420.48f);
        }
        else
        {
            NLH_RingGameSettings[9].transform.Find("Members").gameObject.SetActive(false);
            NLH_RingGameSettings[9].transform.GetComponent<RectTransform>().sizeDelta = new Vector2(608.5f, 344f);
        }

        //Hand Threshold Slider Enable/Disable
        if(NLH_RingGameSettings[6].transform.GetComponent<Slider>().value > 15)
        {
            //handThreshold.interactable = true;
            handThreshold.gameObject.SetActive(true);
            handThresholdText.gameObject.SetActive(true);
            handThreshold.transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(608, 510);
        }
        else
        {
            //handThreshold.interactable = false;
            handThreshold.gameObject.SetActive(false);
            handThresholdText.gameObject.SetActive(false);
            handThreshold.transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(608, 414);
        }
        
        //Auto Extension Time Slider Enable/Disable
        if (NLH_RingGameSettings[10].transform.GetComponent<ToggleController>().isOn)
        {
            autoExtensionTime.interactable = true;
        }
        else
        {
            autoExtensionTime.interactable = false;
        }

        //Call Time Slider Enable/Disable
        if (NLH_RingGameSettings[16].transform.GetComponent<ToggleController>().isOn)
        {
            callTime.gameObject.SetActive(true);
            callTime.transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(608, 135);
            //callTime.interactable = true;
        }
        else
        {
            //callTime.interactable = false;
            callTime.gameObject.SetActive(false);
            callTime.transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(608, 75);
        }

        //Ban Chatting Diamond Prefab Enable
        if(NLH_RingGameSettings[20].transform.GetComponent<ToggleController>().isOn)
        {
            BanChattingDiamondObj.SetActive(true);
        }
        else
        {
            BanChattingDiamondObj.SetActive(false);
        }
    }

    private void Initialise()
    {
        //NLH
        RingGameTabButton_NLH.onClick.RemoveAllListeners();
        SNGGameTabButton_NLH.onClick.RemoveAllListeners();
        MTTGameTabButton_NLH.onClick.RemoveAllListeners();


        RingGameTabButton_NLH.onClick.AddListener(() => OpenScreen("Ring_NLH"));
        SNGGameTabButton_NLH.onClick.AddListener(() => OpenScreen("SNG_NLH"));
        MTTGameTabButton_NLH.onClick.AddListener(() => OpenScreen("MTT_NLH"));

        RingGame_RegularModeTabNLH.onClick.RemoveAllListeners();
        RingGame_6PlusModeTabNLH.onClick.RemoveAllListeners();

        RingGame_RegularModeTabNLH.onClick.AddListener(() => OpenScreen("NLH_RegularMode"));
        RingGame_6PlusModeTabNLH.onClick.AddListener(() => OpenScreen("NLH_6Plus"));


        //PLO
        RingGameTabButton_PLO.onClick.RemoveAllListeners();
        SNGGameTabButton_PLO.onClick.RemoveAllListeners();
        MTTGameTabButton_PLO.onClick.RemoveAllListeners();


        RingGameTabButton_PLO.onClick.AddListener(() => OpenScreen("Ring_PLO"));
        SNGGameTabButton_PLO.onClick.AddListener(() => OpenScreen("SNG_PLO"));
        MTTGameTabButton_PLO.onClick.AddListener(() => OpenScreen("MTT_PLO"));


        //MIXED
        RingGameTabButton_MIXED.onClick.RemoveAllListeners();
        SNGGameTabButton_MIXED.onClick.RemoveAllListeners();
        MTTGameTabButton_MIXED.onClick.RemoveAllListeners();


        RingGameTabButton_MIXED.onClick.AddListener(() => OpenScreen("Ring_MIXED"));
        SNGGameTabButton_MIXED.onClick.AddListener(() => OpenScreen("SNG_MIXED"));
        MTTGameTabButton_MIXED.onClick.AddListener(() => OpenScreen("MTT_MIXED"));

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
            case "Ring_NLH":
                RingGameTabButton_NLH.GetComponent<Image>().color = c;
                SNGGameTabButton_NLH.GetComponent<Image>().color = c1;
                MTTGameTabButton_NLH.GetComponent<Image>().color = c1;

                RingGamePanel_NLH.SetActive(true);
                SNGamePanel_NLH.SetActive(false);
                MTTGamePanel_NLH.SetActive(false);
                break;
            case "SNG_NLH":
                RingGameTabButton_NLH.GetComponent<Image>().color = c1;
                SNGGameTabButton_NLH.GetComponent<Image>().color = c;
                MTTGameTabButton_NLH.GetComponent<Image>().color = c1;

                RingGamePanel_NLH.SetActive(false);
                SNGamePanel_NLH.SetActive(true);
                MTTGamePanel_NLH.SetActive(false);
                break;
            case "MTT_NLH":
                RingGameTabButton_NLH.GetComponent<Image>().color = c1;
                SNGGameTabButton_NLH.GetComponent<Image>().color = c1;
                MTTGameTabButton_NLH.GetComponent<Image>().color = c;

                RingGamePanel_NLH.SetActive(false);
                SNGamePanel_NLH.SetActive(false);
                MTTGamePanel_NLH.SetActive(true);
                break;

            case "Ring_PLO":
                RingGameTabButton_PLO.GetComponent<Image>().color = c;
                SNGGameTabButton_PLO.GetComponent<Image>().color = c1;
                MTTGameTabButton_PLO.GetComponent<Image>().color = c1;

                RingGamePanel_PLO.SetActive(true);
                SNGamePanel_PLO.SetActive(false);
                MTTGamePanel_PLO.SetActive(false);
                break;
            case "SNG_PLO":
                RingGameTabButton_PLO.GetComponent<Image>().color = c1;
                SNGGameTabButton_PLO.GetComponent<Image>().color = c;
                MTTGameTabButton_PLO.GetComponent<Image>().color = c1;

                RingGamePanel_PLO.SetActive(false);
                SNGamePanel_PLO.SetActive(true);
                MTTGamePanel_PLO.SetActive(false);
                break;
            case "MTT_PLO":
                RingGameTabButton_PLO.GetComponent<Image>().color = c1;
                SNGGameTabButton_PLO.GetComponent<Image>().color = c1;
                MTTGameTabButton_PLO.GetComponent<Image>().color = c;

                RingGamePanel_PLO.SetActive(false);
                SNGamePanel_PLO.SetActive(false);
                MTTGamePanel_PLO.SetActive(true);
                break;

            case "Ring_MIXED":
                RingGameTabButton_MIXED.GetComponent<Image>().color = c;
                SNGGameTabButton_MIXED.GetComponent<Image>().color = c1;
                MTTGameTabButton_MIXED.GetComponent<Image>().color = c1;

                RingGamePanel_MIXED.SetActive(true);
                SNGamePanel_MIXED.SetActive(false);
                MTTGamePanel_MIXED.SetActive(false);
                break;
            case "SNG_MIXED":
                RingGameTabButton_MIXED.GetComponent<Image>().color = c1;
                SNGGameTabButton_MIXED.GetComponent<Image>().color = c;
                MTTGameTabButton_MIXED.GetComponent<Image>().color = c1;

                RingGamePanel_MIXED.SetActive(false);
                SNGamePanel_MIXED.SetActive(true);
                MTTGamePanel_MIXED.SetActive(false);
                break;
            case "MTT_MIXED":
                RingGameTabButton_MIXED.GetComponent<Image>().color = c1;
                SNGGameTabButton_MIXED.GetComponent<Image>().color = c1;
                MTTGameTabButton_MIXED.GetComponent<Image>().color = c;

                RingGamePanel_MIXED.SetActive(false);
                SNGamePanel_MIXED.SetActive(false);
                MTTGamePanel_MIXED.SetActive(true);
                break;


            case "NLH_RegularMode":
                Debug.Log("OnClick Regular Mode");

                RingGamePanel_NLH.transform.GetChild(0).Find("Content_RegularMode").gameObject.SetActive(true);
                RingGamePanel_NLH.transform.GetChild(0).Find("Content_6Plus").gameObject.SetActive(false);

                RingGamePanel_NLH.transform.GetComponent<ScrollRect>().content = RingGamePanel_NLH.transform.GetChild(0).Find("Content_RegularMode").GetComponent<RectTransform>();

                InitializeNLHRingGameParameters("NLH_RegularMode");
                break;

            case "NLH_6Plus":
                Debug.Log("OnClick 6+ Mode");
                
                RingGamePanel_NLH.transform.GetChild(0).Find("Content_RegularMode").gameObject.SetActive(false);
                RingGamePanel_NLH.transform.GetChild(0).Find("Content_6Plus").gameObject.SetActive(true);

                RingGamePanel_NLH.transform.GetComponent<ScrollRect>().content = RingGamePanel_NLH.transform.GetChild(0).Find("Content_6Plus").GetComponent<RectTransform>();
                InitializeNLHRingGameParameters("NLH_6Plus");
                break;

            default:
                break;
        }
    }

    private void RequestTemplateData()
    {

        string requestData = "{\"clubId\":\"" + ClubDetailsUIManager.instance.GetClubId() + "\"," +
                                "\"tableId\":\"" + "" + "\"," +
                                "\"status\":\"" + "" + "\"," +
                                "\"settingData\":\"" + "Yes" + "\"}";

        WebServices.instance.SendRequest(RequestType.GetTemplates, requestData, true, OnServerResponseFound);
    }

    private void LoadAllTemplates(JsonData data)
    {
        for (int i = 0; i < container.childCount; i++)
        {
            Destroy(container.GetChild(i).gameObject);
        }

        Debug.Log("Total Templates: " + data["response"].Count);
        
        for (int i = 0; i < data["response"].Count; i++)
        {
            GameObject obj = Instantiate(templateObj, container) as GameObject;

            obj.GetComponent<ClubTemplateManager>().tableId = data["response"][i]["tableId"].ToString();
            obj.GetComponent<ClubTemplateManager>().templateType.text = data["response"][i]["templateType"].ToString();
            obj.GetComponent<ClubTemplateManager>().gameType.text = data["response"][i]["gameType"].ToString();
            
            if (data["response"][i]["settingData"].Count > 0 && data["response"][i]["templateType"].ToString().Equals("Ring Game"))
            {
                Debug.Log("Count Greater Zero: " + data["response"][i]["settingData"]["hours"].ToString());
                obj.GetComponent<ClubTemplateManager>().chipsData.text = data["response"][i]["tableId"].ToString();
                obj.GetComponent<ClubTemplateManager>().userData.text = data["response"][i]["settingData"]["memberCount"].ToString();
                obj.GetComponent<ClubTemplateManager>().timeData.text = data["response"][i]["settingData"]["hours"].ToString();
            }
            
            obj.GetComponent<ClubTemplateManager>().deleteButton.onClick.AddListener(() => OnClickOnDeleteButton(obj.GetComponent<ClubTemplateManager>().tableId));
            obj.GetComponent<ClubTemplateManager>().editButton.onClick.AddListener(() => OnClickOnEditButton());
        }
    }

    private void OnClickOnTemplate()
    {

    }

    private void OnClickOnDeleteButton(string tableId)
    {
        Debug.Log("Table to delete: " + tableId);
        string requestData = "{\"clubId\":\"" + ClubDetailsUIManager.instance.GetClubId().ToString() + "\"," +
                               "\"status\":\"" + "Deleted" + "\"," +
                               "\"tableId\":\"" + tableId + "\"}";

        WebServices.instance.SendRequest(RequestType.UpdateTemplateStatus, requestData, true, OnServerResponseFound);
    }

    private void OnClickOnEditButton()
    {

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


            case "SaveRingGamePLO":
                {
                    OnClickOnSave("PLO", "Ring Game");
                }
                break;
            case "SaveSNGPLO":
                {
                    OnClickOnSave("PLO", "SNG");
                }
                break;
            case "SaveMTTPLO":
                {
                    OnClickOnSave("PLO", "MTT");
                }
                break;
            case "CreateRingGamePLO":
                {
                    //OnClickOnSave("PLO", "Ring Game");
                }
                break;
            case "CreateSNGPLO":
                {
                    //OnClickOnSave("PLO", "SNG");
                }
                break;
            case "CreateMTTPLO":
                {
                    //OnClickOnSave("PLO", "MTT");
                }
                break;


            case "SaveRingGameMIXED":
                {
                    OnClickOnSave("MIXED", "Ring Game");
                }
                break;
            case "SaveSNGMIXED":
                {
                    OnClickOnSave("MIXED", "SNG");
                }
                break;
            case "SaveMTTMIXED":
                {
                    OnClickOnSave("MIXED", "MTT");
                }
                break;
            case "CreateRingGameMIXED":
                {
                    //OnClickOnSave("MIXED", "Ring Game");
                }
                break;
            case "CreateSNGMIXED":
                {
                    //OnClickOnSave("MIXED", "SNG");
                }
                break;
            case "CreateMTTMIXED":
                {
                    //OnClickOnSave("MIXED", "MTT");
                }
                break;
        }
    }

    public void OnSliderValueChange(string name)
    {
        switch(name)
        {
            case "ringGame_blinds_NLH":
                NLH_RingGameSliderTexts[0].text = "Blinds: " + NLH_RingGameSettings[4].transform.GetComponent<Slider>().value.ToString() + "/" + (NLH_RingGameSettings[4].transform.GetComponent<Slider>().value * 2).ToString();
                break;

            case "ringGame_buyIn_NLH":
                NLH_RingGameSliderTexts[1].text = "BuyIn: " + NLH_RingGameSettings[5].transform.GetComponent<RangeSlider>().LowValue.ToString();
                NLH_RingGameSliderTexts[2].text = "Max: " + NLH_RingGameSettings[5].transform.GetComponent<RangeSlider>().HighValue.ToString();
                break;

            case "ringGame_VPIP_NLH":
                NLH_RingGameSliderTexts[3].text = "Min. VPIP: " + NLH_RingGameSettings[6].transform.GetComponent<Slider>().value.ToString() + "%";
                break;

            case "ringGame_VPIPLevel_NLH":
                NLH_RingGameSliderTexts[4].text = "VPIP Level: " + NLH_RingGameSettings[7].transform.GetComponent<Slider>().value.ToString() + "%";
                break;

            case "ringGame_Time_NLH":
                NLH_RingGameSliderTexts[5].text = "Times: " + NLH_RingGameSettings[11].transform.GetComponent<Slider>().value.ToString();
                break;

            case "SNG_StartingChips_NLH":
                NLH_SNGSliderTexts[0].text = "Starting Chips: " + NLH_SNGGameSettings[5].transform.GetComponent<Slider>().value.ToString() + "BB";
                break;

            case "SNG_BlindsUp_NLH":
                NLH_SNGSliderTexts[1].text = "Blinds Up: " + NLH_SNGGameSettings[6].transform.GetComponent<Slider>().value.ToString() + "min";
                break;

            case "MTT_StartingChips_NLH":
                NLH_MTTSliderTexts[0].text = "Starting Chips: " + NLH_MTTGameSettings[9].transform.GetComponent<Slider>().value.ToString();
                break;

            case "MTT_BlindsUp_NLH":
                NLH_MTTSliderTexts[1].text = "Blinds Up: " + NLH_MTTGameSettings[10].transform.GetComponent<Slider>().value.ToString() + "min";
                break;

            case "MTT_LaterRegistrationLevel_NLH":
                NLH_MTTSliderTexts[2].text = "Later Registration: Level " + NLH_MTTGameSettings[11].transform.GetComponent<Slider>().value.ToString();
                break;

            case "MTT_Players_NLH":
                NLH_MTTSliderTexts[3].text = "Min Players: " + NLH_MTTGameSettings[12].transform.GetComponent<RangeSlider>().LowValue.ToString();
                NLH_MTTSliderTexts[4].text = "Max Players: " + NLH_MTTGameSettings[12].transform.GetComponent<RangeSlider>().HighValue.ToString();
                break;

            //PLO

            case "ringGame_blinds_PLO":
                PLO_RingGameSliderTexts[0].text = "Blinds: " + PLO_RingGameSettings[4].transform.GetComponent<Slider>().value.ToString() + "/" + (PLO_RingGameSettings[4].transform.GetComponent<Slider>().value * 2).ToString();
                break;

            case "ringGame_buyIn_PLO":
                PLO_RingGameSliderTexts[1].text = "BuyIn: " + PLO_RingGameSettings[5].transform.GetComponent<RangeSlider>().LowValue.ToString();
                PLO_RingGameSliderTexts[2].text = "Max: " + PLO_RingGameSettings[5].transform.GetComponent<RangeSlider>().HighValue.ToString();
                break;

            case "ringGame_VPIP_PLO":
                PLO_RingGameSliderTexts[3].text = "Min. VPIP: " + PLO_RingGameSettings[6].transform.GetComponent<Slider>().value.ToString() + "%";
                break;

            case "ringGame_VPIPLevel_PLO":
                PLO_RingGameSliderTexts[4].text = "VPIP Level: " + PLO_RingGameSettings[7].transform.GetComponent<Slider>().value.ToString() + "%";
                break;

            case "ringGame_Time_PLO":
                PLO_RingGameSliderTexts[5].text = "Times: " + PLO_RingGameSettings[11].transform.GetComponent<Slider>().value.ToString();
                break;

            case "SNG_StartingChips_PLO":
                PLO_SNGSliderTexts[0].text = "Starting Chips: " + PLO_SNGGameSettings[5].transform.GetComponent<Slider>().value.ToString() + "BB";
                break;

            case "SNG_BlindsUp_PLO":
                PLO_SNGSliderTexts[1].text = "Blinds Up: " + PLO_SNGGameSettings[6].transform.GetComponent<Slider>().value.ToString() + "min";
                break;

            case "MTT_StartingChips_PLO":
                PLO_MTTSliderTexts[0].text = "Starting Chips: " + PLO_MTTGameSettings[9].transform.GetComponent<Slider>().value.ToString();
                break;

            case "MTT_BlindsUp_PLO":
                PLO_MTTSliderTexts[1].text = "Blinds Up: " + PLO_MTTGameSettings[10].transform.GetComponent<Slider>().value.ToString() + "min";
                break;

            case "MTT_LaterRegistrationLevel_PLO":
                PLO_MTTSliderTexts[2].text = "Later Registration: Level " + PLO_MTTGameSettings[11].transform.GetComponent<Slider>().value.ToString();
                break;

            case "MTT_Players_PLO":
                PLO_MTTSliderTexts[3].text = "Min Players: " + PLO_MTTGameSettings[12].transform.GetComponent<RangeSlider>().LowValue.ToString();
                PLO_MTTSliderTexts[4].text = "Max Players: " + PLO_MTTGameSettings[12].transform.GetComponent<RangeSlider>().HighValue.ToString();
                break;


                //MIXED

            case "ringGame_blinds_MIXED":
                MIXED_RingGameSliderTexts[0].text = "Blinds: " + MIXED_RingGameSettings[4].transform.GetComponent<Slider>().value.ToString() + "/" + (MIXED_RingGameSettings[4].transform.GetComponent<Slider>().value * 2).ToString();
                break;

            case "ringGame_buyIn_MIXED":
                MIXED_RingGameSliderTexts[1].text = "BuyIn: " + MIXED_RingGameSettings[5].transform.GetComponent<RangeSlider>().LowValue.ToString();
                MIXED_RingGameSliderTexts[2].text = "Max: " + MIXED_RingGameSettings[5].transform.GetComponent<RangeSlider>().HighValue.ToString();
                break;

            case "ringGame_VPIP_MIXED":
                MIXED_RingGameSliderTexts[3].text = "Min. VPIP: " + MIXED_RingGameSettings[6].transform.GetComponent<Slider>().value.ToString() + "%";
                break;

            case "ringGame_VPIPLevel_MIXED":
                MIXED_RingGameSliderTexts[4].text = "VPIP Level: " + MIXED_RingGameSettings[7].transform.GetComponent<Slider>().value.ToString() + "%";
                break;

            case "ringGame_Time_MIXED":
                MIXED_RingGameSliderTexts[5].text = "Times: " + MIXED_RingGameSettings[11].transform.GetComponent<Slider>().value.ToString();
                break;

            case "SNG_StartingChips_MIXED":
                MIXED_SNGSliderTexts[0].text = "Starting Chips: " + MIXED_SNGGameSettings[5].transform.GetComponent<Slider>().value.ToString() + "BB";
                break;

            case "SNG_BlindsUp_MIXED":
                MIXED_SNGSliderTexts[1].text = "Blinds Up: " + MIXED_SNGGameSettings[6].transform.GetComponent<Slider>().value.ToString() + "min";
                break;

            case "MTT_StartingChips_MIXED":
                MIXED_MTTSliderTexts[0].text = "Starting Chips: " + MIXED_MTTGameSettings[9].transform.GetComponent<Slider>().value.ToString();
                break;

            case "MTT_BlindsUp_MIXED":
                MIXED_MTTSliderTexts[1].text = "Blinds Up: " + MIXED_MTTGameSettings[10].transform.GetComponent<Slider>().value.ToString() + "min";
                break;

            case "MTT_LaterRegistrationLevel_MIXED":
                MIXED_MTTSliderTexts[2].text = "Later Registration: Level " + MIXED_MTTGameSettings[11].transform.GetComponent<Slider>().value.ToString();
                break;

            case "MTT_Players_MIXED":
                MIXED_MTTSliderTexts[3].text = "Min Players: " + MIXED_MTTGameSettings[12].transform.GetComponent<RangeSlider>().LowValue.ToString();
                MIXED_MTTSliderTexts[4].text = "Max Players: " + MIXED_MTTGameSettings[12].transform.GetComponent<RangeSlider>().HighValue.ToString();
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
                "\"cap\":\"" + NLH_RingGameSettings[15].transform.GetComponent<TMP_Text>().text.ToString().Substring(0, NLH_RingGameSettings[15].transform.GetComponent<TMP_Text>().text.ToString().Length - 3) + "\"," +
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
            Debug.Log("Inside MTT: " + ClubDetailsUIManager.instance.GetClubId());
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
               "\"startTime\":\"" + /*NLH_MTTGameSettings[21].transform.GetComponent<TMP_Text>().text.ToString()*/"" + "\"}]}";
        }

        WebServices.instance.SendRequest(RequestType.CreateTemplate, requestData, true, OnServerResponseFound);
    }


    private void OnClickOnCreate(string gameType, string templateType)
    {
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
            case RequestType.GetTemplates:
                {
                    JsonData data = JsonMapper.ToObject(serverResponse);

                    if (data["success"].ToString() == "1")
                    {
                        LoadAllTemplates(data);
                    }
                    else
                    {
                        MainMenuController.instance.ShowMessage("Unable to get room data");
                    } 
                }
                break;

            case RequestType.UpdateTemplateStatus:
                {
                    JsonData data = JsonMapper.ToObject(serverResponse);

                    if (data["success"].ToString() == "1")
                    {
                        if (data["message"].ToString().Equals("Template Deleted"))
                        {
                            MainMenuController.instance.ShowMessage("Deleted successfully");
                            RequestTemplateData();
                        }
                        else if (data["message"].ToString().Equals("Template Published"))
                            MainMenuController.instance.ShowMessage("Published successfully");
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
    
    public void OnClickIncreaseMemberCount()
    {
        if(memberCount_NLH < 9)
        {
            memberCount_NLH++;
            NLH_RingGameSettings[1].transform.GetComponent<TMP_Text>().text = memberCount_NLH.ToString();
        }
        //else
        //{
        //    incrementMemberCount.interactable = false;
        //}

        switch(memberCount_NLH)
        {
            case 3:
                {
                    NLH_autoStartMemberList[0].SetActive(true);
                }
                break;
            case 4:
                {
                    for (int i = 0; i < 2; i++)
                    {
                        NLH_autoStartMemberList[i].SetActive(true);
                    }
                }
                break;
            case 5:
                {
                    for (int i = 0; i < 3; i++)
                    {
                        NLH_autoStartMemberList[i].SetActive(true);
                    }
                }
                break;
        }
    }

    public void OnClickDecreaseMemberCount()
    {
        if(memberCount_NLH > 2)
        {
            memberCount_NLH--;
            NLH_RingGameSettings[1].transform.GetComponent<TMP_Text>().text = memberCount_NLH.ToString();
        }
        //else
        //{
        //    decreamentMemberCount.interactable = false;
        //}

        switch (memberCount_NLH)
        {
            case 2:
                {
                    for (int i = 2; i >= 0; i--)
                    {
                        NLH_autoStartMemberList[i].SetActive(false);
                    }
                }
                break;

            case 3:
                {
                    for (int i = 2; i >= 1; i--)
                    {
                        NLH_autoStartMemberList[i].SetActive(false);
                    }
                }
                break;
            case 4:
                {
                    NLH_autoStartMemberList[2].SetActive(false);                    
                }
                break;
           
        }
    }
}
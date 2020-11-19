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
    public Text popUpText;

    [Header ("Table Template")]
    public GameObject templateObj;
    public Transform container;
    public Toggle selectAllToggle;
    public Text noTemplateText;
    public Button createBtn;


    [Header("NLH")]
    public Button RingGame_RegularModeTabNLH;
    public Button RingGame_6PlusModeTabNLH;

    public Button RingGameTabButton_NLH;
    public Button SNGGameTabButton_NLH;
    public Button MTTGameTabButton_NLH;

    public GameObject RingGamePanel_NLH, SNGamePanel_NLH, MTTGamePanel_NLH;


    [Header("PLO")]
    public Button RingGameTabButton_PLO;
    public Button SNGGameTabButton_PLO;
    public Button MTTGameTabButton_PLO;
    public GameObject RingGamePanel_PLO, SNGamePanel_PLO, MTTGamePanel_PLO;
    
    public Button RingGame_PLO4ModePLO;
    public Button RingGame_PLO5ModePLO;

    [Space(5)]
    [Header("MIXED GAME")]
    public Button RingGameTabButton_MIXED;
    public Button SNGGameTabButton_MIXED;
    public Button MTTGameTabButton_MIXED;
    public GameObject RingGamePanel_MIXED, SNGamePanel_MIXED, MTTGamePanel_MIXED;
    

    public Button RingGame_NLHPLO4ModePLO;
    public Button RingGame_NLHPLO5ModePLO;

    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
        }

        popUpText.gameObject.SetActive(false);
        RequestTemplateData();
    }

    private void Start()
    {
        Initialise();
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

        RingGame_PLO4ModePLO.onClick.AddListener(() => OpenScreen("PLO4Mode"));
        RingGame_PLO5ModePLO.onClick.AddListener(() => OpenScreen("PLO5Mode"));


        //MIXED
        RingGameTabButton_MIXED.onClick.RemoveAllListeners();
        SNGGameTabButton_MIXED.onClick.RemoveAllListeners();
        MTTGameTabButton_MIXED.onClick.RemoveAllListeners();


        RingGameTabButton_MIXED.onClick.AddListener(() => OpenScreen("Ring_MIXED"));
        SNGGameTabButton_MIXED.onClick.AddListener(() => OpenScreen("SNG_MIXED"));
        MTTGameTabButton_MIXED.onClick.AddListener(() => OpenScreen("MTT_MIXED"));

        RingGame_NLHPLO4ModePLO.onClick.AddListener(() => OpenScreen("NLHPLO4"));
        RingGame_NLHPLO5ModePLO.onClick.AddListener(() => OpenScreen("NLHPLO5"));
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

                break;

            case "NLH_6Plus":
                Debug.Log("OnClick 6+ Mode");
                
                RingGamePanel_NLH.transform.GetChild(0).Find("Content_RegularMode").gameObject.SetActive(false);
                RingGamePanel_NLH.transform.GetChild(0).Find("Content_6Plus").gameObject.SetActive(true);

                RingGamePanel_NLH.transform.GetComponent<ScrollRect>().content = RingGamePanel_NLH.transform.GetChild(0).Find("Content_6Plus").GetComponent<RectTransform>();
                break;

            case "PLO4Mode":
                Debug.Log("OnClick PLO4 Mode");

                RingGamePanel_PLO.transform.GetChild(0).Find("Content_PLO4").gameObject.SetActive(true);
                RingGamePanel_PLO.transform.GetChild(0).Find("Content_PLO5").gameObject.SetActive(false);

                RingGamePanel_PLO.transform.GetComponent<ScrollRect>().content = RingGamePanel_PLO.transform.GetChild(0).Find("Content_PLO4").GetComponent<RectTransform>();
                break;

            case "PLO5Mode":
                Debug.Log("OnClick PLO4 Mode");

                RingGamePanel_PLO.transform.GetChild(0).Find("Content_PLO4").gameObject.SetActive(false);
                RingGamePanel_PLO.transform.GetChild(0).Find("Content_PLO5").gameObject.SetActive(true);

                RingGamePanel_PLO.transform.GetComponent<ScrollRect>().content = RingGamePanel_PLO.transform.GetChild(0).Find("Content_PLO5").GetComponent<RectTransform>();
                break;

            case "NLHPLO4":
                Debug.Log("OnClick NLH & PLO4 Mode");

                RingGamePanel_MIXED.transform.GetChild(0).Find("Content_NLH_PLO4").gameObject.SetActive(true);
                RingGamePanel_MIXED.transform.GetChild(0).Find("Content_NLH_PLO5").gameObject.SetActive(false);

                RingGamePanel_MIXED.transform.GetComponent<ScrollRect>().content = RingGamePanel_MIXED.transform.GetChild(0).Find("Content_NLH_PLO4").GetComponent<RectTransform>();
                break;

            case "NLHPLO5":
                Debug.Log("OnClick NLH & PLO5 Mode");

                RingGamePanel_MIXED.transform.GetChild(0).Find("Content_NLH_PLO4").gameObject.SetActive(false);
                RingGamePanel_MIXED.transform.GetChild(0).Find("Content_NLH_PLO5").gameObject.SetActive(true);

                RingGamePanel_MIXED.transform.GetComponent<ScrollRect>().content = RingGamePanel_MIXED.transform.GetChild(0).Find("Content_NLH_PLO5").GetComponent<RectTransform>();
                break;

            default:
                break;
        }
    }

    public void RequestTemplateData()
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

        for (int i = 0; i < data["response"].Count; i++)
        {
            GameObject obj = Instantiate(templateObj, container) as GameObject;

            obj.GetComponent<ClubTemplateManager>().tableId = data["response"][i]["tableId"].ToString();
            
            if (data["response"][i]["templateName"] != null)
                obj.GetComponent<ClubTemplateManager>().templateName.text = data["response"][i]["templateName"].ToString();
            else
                obj.GetComponent<ClubTemplateManager>().templateName.text = "Unnamed Tab...";

            obj.GetComponent<ClubTemplateManager>().gameType.text = data["response"][i]["gameType"].ToString();

            if (data["response"][i]["settingData"].Count > 0 && data["response"][i]["templateType"].ToString().Equals("Ring Game"))
            {
                if (data["response"][i]["settingData"]["templateSubType"].ToString().Equals("6+"))
                {
                    obj.GetComponent<ClubTemplateManager>()._6PlusObj.SetActive(true);
                    obj.GetComponent<ClubTemplateManager>().blindsInfoObj.SetActive(false);
                    obj.GetComponent<ClubTemplateManager>().anteInfoObj.SetActive(true);
                    obj.GetComponent<ClubTemplateManager>().anteText.text = data["response"][i]["settingData"]["ante"].ToString();
                }
                else
                {
                    obj.GetComponent<ClubTemplateManager>()._6PlusObj.SetActive(false);
                    obj.GetComponent<ClubTemplateManager>().blindsInfoObj.SetActive(true);
                    obj.GetComponent<ClubTemplateManager>().anteInfoObj.SetActive(false);

                    if (data["response"][i]["settingData"]["blinds"] != null)
                        obj.GetComponent<ClubTemplateManager>().blindsText.text = data["response"][i]["settingData"]["blinds"].ToString();
                    else
                        obj.GetComponent<ClubTemplateManager>().blindsText.text = "";
                }

                obj.GetComponent<ClubTemplateManager>().playerText.text = data["response"][i]["settingData"]["memberCount"].ToString();
                obj.GetComponent<ClubTemplateManager>().timeText.text = data["response"][i]["settingData"]["hours"].ToString();
            }
            
            obj.GetComponent<ClubTemplateManager>().deleteButton.onClick.AddListener(() => OnClickOnDeleteButton(obj.GetComponent<ClubTemplateManager>().tableId));
            obj.GetComponent<ClubTemplateManager>().editButton.onClick.AddListener(() => OnClickOnEditButton());
        }      
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

    public void OnClickSelectAllToggle()
    {
        if(selectAllToggle.isOn)
        {
            for (int i = 0; i < container.childCount; i++)
            {
                container.GetChild(i).GetComponent<ClubTemplateManager>().selectToggle.isOn = true;
                container.GetChild(i).GetComponent<ClubTemplateManager>().OnSelectedCheckBox();
            }
        }
        else
        {
            for (int i = 0; i < container.childCount; i++)
            {
                container.GetChild(i).GetComponent<ClubTemplateManager>().selectToggle.isOn = false;
                container.GetChild(i).GetComponent<ClubTemplateManager>().OnSelectedCheckBox();
            }
        }
    }

    public void OnClickOnCreate()
    {
        int selectedTemplateCounter = 0;
        string tableIds = "";

        for (int i = 0; i < container.childCount; i++)
        {
            if (container.GetChild(i).GetComponent<ClubTemplateManager>().selectToggle.isOn)
                selectedTemplateCounter++;           
        }

        if(selectedTemplateCounter > 0)
        {
            for (int i = 0; i < container.childCount; i++)
            {
                if (container.GetChild(i).GetComponent<ClubTemplateManager>().selectToggle.isOn)
                {
                    if (tableIds == "")
                        tableIds += tableIds + container.GetChild(i).GetComponent<ClubTemplateManager>().tableId;
                    else
                        tableIds += "," + container.GetChild(i).GetComponent<ClubTemplateManager>().tableId;                   
                }                
            }

            string requestData = "{\"clubId\":\"" + ClubDetailsUIManager.instance.GetClubId() + "\"," +
                                "\"status\":\"" + "Published" + "\"," +
                                "\"tableIds\":[\"" + tableIds  + "\"]}";

            WebServices.instance.SendRequest(RequestType.UpdateTemplateStatus, requestData, true, OnServerResponseFound);
        }
        else
        {
            StartCoroutine(ShowPopUp("Please select a template", 1.29f));
        }        
    }

    IEnumerator ShowPopUp(string msg, float delay)
    {
        popUpText.gameObject.SetActive(true);
        popUpText.text = msg;
        yield return new WaitForSeconds(delay);
        popUpText.gameObject.SetActive(false);
    }


    public void OnServerResponseFound(RequestType requestType, string serverResponse, bool isShowErrorMessage, string errorMessage)
    {
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
                    Debug.Log("Response => GetTemplates : " + serverResponse);
                    JsonData data = JsonMapper.ToObject(serverResponse);
                    if (data["success"].ToString() == "1")
                    {
                        //if (data[ <= 0)
                        //{
                        //    Debug.Log("No Data Found");
                        //    noTemplateText.gameObject.SetActive(true);
                        //    createBtn.interactable = false;
                        //}
                        //else
                        //{ 
                            noTemplateText.gameObject.SetActive(false);
                            createBtn.interactable = true;
                            LoadAllTemplates(data);
                        //}
                    }
                    else
                    {
                        MainMenuController.instance.ShowMessage("Unable to get room data");
                    } 
                }
                break;

            case RequestType.UpdateTemplateStatus:
                {
                    Debug.Log("Response => UpdateTemplateStatus : " + serverResponse);
                    JsonData data = JsonMapper.ToObject(serverResponse);

                    if (data["success"].ToString() == "1")
                    {
                        if (data["message"].ToString().Equals("Template Deleted"))
                        {
                            MainMenuController.instance.ShowMessage("Deleted successfully");
                            for (int i = 0; i < container.childCount; i++)
                            {
                                Destroy(container.GetChild(i).gameObject);
                            }
                            RequestTemplateData();
                        }
                        else if (data["message"].ToString().Equals("Template Published"))
                        {
                            //StartCoroutine(ShowPopUp("Template Published ", 1.25f));
                            Debug.Log("Tamplate Published Successfully");                            
                        }
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
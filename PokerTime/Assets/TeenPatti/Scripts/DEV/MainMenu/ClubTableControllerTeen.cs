using DG.Tweening;
using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Permissions;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class ClubTableControllerTeen : MonoBehaviour
{
    public static ClubTableControllerTeen instance;
    public Text popUpText;

    [Header("Table Templates Data")]
    public Transform tableListContainer;
    public GameObject templateObj;
    public Toggle selectAllToggle;
    public GameObject availableTemplatePanel;
    public GameObject noTemplatePanel;
    public Button createBtn;

    [Header("Menu Buttons")]
    public Button createTable;
    public Button tableTemplates;

    [Header("Buttons")]
    public Button saveBtn;
    public Button startBtn;
    public Button editBtn;

    [Header("Panels")]
    public GameObject createTablePanel;
    public GameObject tableTemplatesPanel;

    [Header("Components")]
    public List<GameObject> components;

    [HideInInspector]
    public bool isPublishTemplateWithCreate = false;
    public static int tableIdStatic = 0;

    public static bool isEditingTemplate = false;
    TableData tableData;

    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
        }

        popUpText.gameObject.SetActive(false);
        RequestTemplateData(false);

        createTable.onClick.RemoveAllListeners();
        tableTemplates.onClick.RemoveAllListeners();

        createTable.onClick.AddListener(() => OpenScreen("CreateTemplate"));
        tableTemplates.onClick.AddListener(() => OpenScreen("TableTemplates"));        
    }

    //void Initialize()
    //{
    //    tableData.userId = PlayerManager.instance.GetPlayerGameData().userId;
    //}

    private void OnEnable()
    {
        OpenScreen("CreateTemplate");
    }

    private void OnDisable()
    {
        isEditingTemplate = false;
        tableIdStatic = 0;
        ResetTaleData();
    }

    private void OpenScreen(string screenName)
    {
        Color c = new Color(1, 1, 1, 1);
        Color c1 = new Color(1, 1, 1, 0);

        switch (screenName)
        {
            case "CreateTemplate":
                createTable.GetComponent<Image>().color = c;
                tableTemplates.GetComponent<Image>().color = c1;

                createTablePanel.SetActive(true);
                tableTemplatesPanel.SetActive(false);

                if(isEditingTemplate)
                {
                    saveBtn.gameObject.SetActive(false);
                    startBtn.gameObject.SetActive(false);
                    editBtn.gameObject.SetActive(true);
                }
                else
                {
                    saveBtn.gameObject.SetActive(true);
                    startBtn.gameObject.SetActive(true);
                    editBtn.gameObject.SetActive(false);
                }

                break;

            case "TableTemplates":
                ResetTaleData();

                createTable.GetComponent<Image>().color = c1;
                tableTemplates.GetComponent<Image>().color = c;

                createTablePanel.SetActive(false);
                tableTemplatesPanel.SetActive(true);
                break;
            
            default:
                break;
        }
    }

    public void RequestTemplateData(bool isPublish)
    {
        string requestData = "{\"clubId\":\"" + ClubDetailsUIManagerTeen.instance.GetClubId() + "\"," +
                                "\"tableId\":\"" + "" + "\"," +
                                "\"status\":\"" + (isPublish ? "Published" : "") + "\"," +
                                "\"settingData\":\"" + "Yes" + "\"}";

        WebServices.instance.SendRequestTP(RequestTypeTP.GetTemplates, requestData, true, OnServerResponseFound);
    }

    private void LoadAllTemplates(JsonData data)
    {
        for (int i = 0; i < tableListContainer.childCount; i++)
        {
            Destroy(tableListContainer.GetChild(i).gameObject);
        }

        //Debug.Log("After RESET Total Templates: " + container.childCount);

        for (int i = 0; i < data["response"].Count; i++)
        {
            GameObject obj = Instantiate(templateObj, tableListContainer) as GameObject;

            obj.GetComponent<ClubTemplateManagerTeen>().tableId = data["response"][i]["tableId"].ToString();

            if (data["response"][i]["tableName"] != null && !data["response"][i]["tableName"].ToString().Equals(""))
                obj.GetComponent<ClubTemplateManagerTeen>().templateName.text = data["response"][i]["tableName"].ToString();
            else
                obj.GetComponent<ClubTemplateManagerTeen>().templateName.text = "Unnamed Tab...";

            obj.GetComponent<ClubTemplateManagerTeen>().gameMode.text = data["response"][i]["gameMode"].ToString();
            obj.GetComponent<ClubTemplateManagerTeen>().bootText.text = data["response"][i]["bootAmount"].ToString();
            obj.GetComponent<ClubTemplateManagerTeen>().playerText.text = data["response"][i]["playerCount"].ToString();
            obj.GetComponent<ClubTemplateManagerTeen>().timeText.text = data["response"][i]["tableTime"].ToString();
            
            obj.GetComponent<ClubTemplateManagerTeen>().deleteButton.onClick.AddListener(() => OnClickOnDeleteButton(obj.GetComponent<ClubTemplateManagerTeen>().tableId));
            obj.GetComponent<ClubTemplateManagerTeen>().editButton.onClick.AddListener(() => OnClickOnEditButton(obj.GetComponent<ClubTemplateManagerTeen>().tableId, data));
        }
    }

    private void OnClickOnDeleteButton(string tableId)
    {
        Debug.Log("Table to delete: " + tableId);
        string requestData = "{\"clubId\":\"" + ClubDetailsUIManagerTeen.instance.GetClubId() + "\"," +
                               "\"status\":\"" + "Deleted" + "\"," +
                               "\"tableId\":\"" + tableId + "\"}";

        WebServices.instance.SendRequestTP(RequestTypeTP.UpdateTemplateStatus, requestData, true, OnServerResponseFound);
    }

    private void OnClickOnEditButton(string tableId, JsonData data)
    {
        for (int i = 0; i < data["response"].Count; i++)
        {
            if (data["response"][i]["tableId"].ToString().Equals(tableId))
            {
                Debug.Log("Response Table ID: " + data["response"][i]["tableId"].ToString());
                Debug.Log("Current Table ID: " + tableId);

                int.TryParse(data["response"][i]["tableId"].ToString(), out tableIdStatic);

                Debug.Log("Setting Up Data for edit..." + tableIdStatic);
                                
                components[0].transform.GetComponent<TMP_InputField>().text = data["response"][i]["tableName"].ToString();
                components[1].transform.GetComponent<TMP_Text>().text = data["response"][i]["playerCount"].ToString();
                components[2].GetComponent<TMP_Dropdown>().captionText.text = data["response"][i]["gameMode"].ToString();
                components[3].transform.Find(data["response"][i]["actionTime"].ToString()).GetComponent<Toggle>().isOn = true;
                components[5].transform.GetComponent<TMP_Text>().text = data["response"][i]["minBuyIn"].ToString();
                components[7].transform.GetComponent<TMP_Text>().text = data["response"][i]["fee"].ToString();
                components[8].transform.GetComponent<TMP_Text>().text = data["response"][i]["cap"].ToString();

                components[9].transform.GetComponent<ToggleController>().isOn = (data["response"][i]["hideRealTimeResult"].ToString().Equals("1") ? true : false);
                components[10].transform.GetComponent<ToggleController>().isOn = (data["response"][i]["gpsRestriction"].ToString().Equals("1") ? true : false);
                components[11].transform.GetComponent<ToggleController>().isOn = (data["response"][i]["ipRestriction"].ToString().Equals("1") ? true : false);

                for (int j = 0; j < components[4].transform.parent.parent.GetComponent<SliderChange>().sliderValues.Length; j++)
                {
                    if (components[4].transform.parent.parent.GetComponent<SliderChange>().sliderValues[j].Equals(data["response"][i]["bootAmount"].ToString()))
                    {
                        components[4].transform.parent.parent.Find("BootSlider").GetComponent<Slider>().value = j;
                    }
                }

                float sliderVal;
                float.TryParse(data["response"][i]["tableTime"].ToString(), out sliderVal);
                components[12].transform.parent.Find("TimeMarkerSlider").GetComponent<Slider>().value = sliderVal;
                components[12].transform.GetComponent<TMP_Text>().text = data["response"][i]["tableTime"].ToString();

                isEditingTemplate = true;
                OpenScreen("CreateTemplate");
            }
        }
    }
    
    public void OnClickSelectAllToggle()
    {
        if(selectAllToggle.isOn)
        {
            for (int i = 0; i < tableListContainer.childCount; i++)
            {
                tableListContainer.GetChild(i).GetComponent<ClubTemplateManagerTeen>().selectToggle.isOn = true;
                tableListContainer.GetChild(i).GetComponent<ClubTemplateManagerTeen>().OnSelectedCheckBox();
            }
        }
        else
        {
            for (int i = 0; i < tableListContainer.childCount; i++)
            {
                tableListContainer.GetChild(i).GetComponent<ClubTemplateManagerTeen>().selectToggle.isOn = false;
                tableListContainer.GetChild(i).GetComponent<ClubTemplateManagerTeen>().OnSelectedCheckBox();
            }
        }
    }

    public void OnClickOnCreate()
    {
        int selectedTemplateCounter = 0;
        string tableIds = "";

        for (int i = 0; i < tableListContainer.childCount; i++)
        {
            if (tableListContainer.GetChild(i).GetComponent<ClubTemplateManagerTeen>().selectToggle.isOn)
                selectedTemplateCounter++;           
        }

        if(selectedTemplateCounter > 0)
        {
            for (int i = 0; i < tableListContainer.childCount; i++)
            {
                if (tableListContainer.GetChild(i).GetComponent<ClubTemplateManagerTeen>().selectToggle.isOn)
                {
                    if (tableIds == "")
                        tableIds += tableIds + tableListContainer.GetChild(i).GetComponent<ClubTemplateManagerTeen>().tableId;
                    else
                        tableIds += "," + tableListContainer.GetChild(i).GetComponent<ClubTemplateManagerTeen>().tableId;                   
                }                
            }

            string requestData = "{\"clubId\":\"" + ClubDetailsUIManagerTeen.instance.GetClubId() + "\"," +
                                "\"status\":\"" + "Published" + "\"," +
                                "\"tableIds\":[\"" + tableIds + "\"]}";

            WebServices.instance.SendRequestTP(RequestTypeTP.UpdateTemplateStatus, requestData, true, OnServerResponseFound);
        }
        else
        {
            StartCoroutine(ShowPopUp("Please select a template", 1.29f));
        }        
    }

    public void ShowPopUp(string msg)
    {
        StartCoroutine(ShowPopUp(msg, 1.29f));
    }


    IEnumerator ShowPopUp(string msg, float delay)
    {
        popUpText.transform.parent.gameObject.SetActive(true);
        popUpText.transform.gameObject.SetActive(true);
        popUpText.text = msg;
        yield return new WaitForSeconds(delay);
        popUpText.transform.parent.gameObject.SetActive(false);
    }


    public void OnServerResponseFound(RequestTypeTP requestType, string serverResponse, bool isShowErrorMessage, string errorMessage)
    {
        tableIdStatic = 0;

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
            case RequestTypeTP.GetTemplates:
                {
                    Debug.Log("Response => GetTemplates (TP) : " + serverResponse);
                    JsonData data = JsonMapper.ToObject(serverResponse);
                    if (data["success"].ToString() == "1")
                    {
                        if (data["response"].Count <= 0)
                        {
                            Debug.Log("No Data Found");
                            noTemplatePanel.gameObject.SetActive(true);
                            availableTemplatePanel.SetActive(false);
                            createBtn.interactable = false;
                        }
                        else
                        {
                            noTemplatePanel.SetActive(false);
                            availableTemplatePanel.SetActive(true);
                            createBtn.interactable = true;

                            LoadAllTemplates(data);
                        }
                    }
                    else
                    {
                        noTemplatePanel.gameObject.SetActive(true);
                        availableTemplatePanel.SetActive(false);
                        createBtn.interactable = false;
                        //    MainMenuController.instance.ShowMessage("Unable to get room data");
                    }
                }
                break;

            case RequestTypeTP.UpdateTemplateStatus:
                {
                    Debug.Log("Response => UpdateTemplateStatus : " + serverResponse);
                    JsonData data = JsonMapper.ToObject(serverResponse);

                    if (data["success"].ToString() == "1")
                    {
                        if (data["message"].ToString().Equals("Template Deleted"))
                        {
                            MainMenuController.instance.ShowMessage("Deleted successfully");
                            for (int i = 0; i < tableListContainer.childCount; i++)
                            {
                                Destroy(tableListContainer.GetChild(i).gameObject);
                            }
                            RequestTemplateData(false);
                        }
                        else if (data["message"].ToString().Equals("Template Published"))
                        {
                            StartCoroutine(ShowPopUp("Template Published ", 1.25f));
                            //Debug.Log("Tamplate Published Successfully");

                            isPublishTemplateWithCreate = false;

                            ClubDetailsUIManagerTeen.instance.GetClubTemplates();
                            RequestTemplateData(false);                            
                        }
                    }
                    else
                    {
                        //MainMenuController.instance.ShowMessage(data["message"].ToString());
                    }
                }
                break;


            case RequestTypeTP.CreateTable:
                {
                    Debug.Log("Response => Create Table: " + serverResponse);
                    JsonData data = JsonMapper.ToObject(serverResponse);

                    if (data["success"].ToString() == "1")
                    {
                        if (isPublishTemplateWithCreate)
                        {
                            string requestData = "{\"clubId\":\"" + ClubDetailsUIManager.instance.GetClubId() + "\"," +
                                    "\"status\":\"" + "Published" + "\"," +
                                    "\"tableIds\":[\"" + data["tableId"].ToString() + "\"]}";

                            WebServices.instance.SendRequestTP(RequestTypeTP.UpdateTemplateStatus, requestData, true, OnServerResponseFound);
                        }
                        else
                        {
                            ShowPopUp("Template saved successfully.");
                        }
                        //Debug.Log(data["message"].ToString());
                        //joinClubPopUp.SetActive(false);
                        //MainMenuController.instance.ShowMessage(data["message"].ToString());
                        RequestTemplateData(false);

                        tableIdStatic = 0;
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


    //DEV_CODE
    //TEMPORARY METHODS
    public void OnClickOnButton(string name)
    {
        //ActionTime Toggle Group
        string actionTime = "";
        Toggle[] toggles_ActionTime = components[3].transform.GetComponentsInChildren<Toggle>();
        foreach (var t in toggles_ActionTime)
        {
            if (t.isOn)
            {
                actionTime = t.name;
                //Debug.Log("Toggle Name:" + actionTime);
            }
        }

        switch (name)
        {
            case "Start":
                {
                    isPublishTemplateWithCreate = true;
                    OnClickOnButton("Create");
                }
                break;
            case "Create":
            {
                    string requestData = "{\"userId\":\"" + PlayerManager.instance.GetPlayerGameData().userId + "\"," +
                                "\"tableName\":\"" + components[0].GetComponent<TMP_InputField>().text + "\"," +
                                "\"playerCount\":" + components[1].GetComponent<TMP_Text>().text + "," +
                                "\"gameMode\":\"" + components[2].GetComponent<TMP_Dropdown>().captionText.text + "\"," +
                                "\"actionTime\": " + actionTime + "," +
                                "\"minBuyIn\":" + components[5].GetComponent<TMP_Text>().text + "," +
                                //"\"maxBuyIn\":" + components[6].GetComponent<TMP_Text>().text + "," +
                                "\"fee\":" + components[7].transform.GetComponent<TMP_Text>().text.Replace("%", "") + "," +
                                "\"cap\":" + components[8].transform.GetComponent<TMP_Text>().text.Replace("BB", "") + "," +
                                "\"hideRealTimeResult\":" + (components[9].transform.GetComponent<ToggleController>().isOn.ToString().Equals("True") ? 1 : 0) + "," +
                                "\"gpsRestriction\":" + (components[10].transform.GetComponent<ToggleController>().isOn.ToString().Equals("True") ? 1 : 0) + "," +
                                "\"ipRestriction\":" + (components[11].transform.GetComponent<ToggleController>().isOn.ToString().Equals("True") ? 1 : 0) + "," +
                                "\"tableTime\":" + components[12].GetComponent<TMP_Text>().text + "," +
                                "\"entryCurrency\":" + 0 + "," +
                                "\"players\":" + 0 + "," +
                                "\"bootAmount\":\"" + components[4].GetComponent<TMP_Text>().text + "\"," +
                                "\"smallBlind\":" + 0 + "," +
                                "\"bigBlind\":" + 0 + "," +
                                "\"minBet\":" + 0 + "," +
                                "\"maxBet\":" + 0 + "," +
                                "\"potLimit\":" + 0 + "," +
                                "\"callTimmer\":" + 0 + "," +
                                "\"startGameTimmer\":" + 0 + "," +
                                "\"nextRoundTimmer\":" + 0 + "," +
                                "\"gameOverTimmer\":" + 0 + "," +
                                "\"gameIcon\":\"" + "" + "\"," +
                                "\"iconBaseUrl\":\"" + "" + "\"," +
                                "\"backgroundImg\":\"" + "" + "\"," +
                                "\"status\":\"" + "Saved" + "\"," +
                                "\"clubId\":\"" + ClubDetailsUIManagerTeen.instance.GetClubId() + "\"," +
                                "\"tableId\":\"" + ((tableIdStatic != 0) ? tableIdStatic.ToString() : "") + "\"}";

                    WebServices.instance.SendRequestTP(RequestTypeTP.CreateTable, requestData, true, OnServerResponseFound);
                }
                break;
        }
    }

    void ResetTaleData()
    {
        components[0].GetComponent<TMP_InputField>().text = "";
        components[1].GetComponent<TMP_Text>().text = "9";
        components[2].GetComponent<TMP_Dropdown>().value = 0;
        components[3].transform.Find("5").GetComponent<Toggle>().isOn = true;
        components[4].transform.parent.parent.Find("BootSlider").GetComponent<Slider>().value = 0;

        components[4].GetComponent<TMP_Text>().text = components[4].transform.parent.parent.GetComponent<SliderChange>().sliderValues[0];
        components[5].GetComponent<TMP_Text>().text = components[4].transform.parent.parent.GetComponent<SliderChange>().lowValueText.text;

        components[7].transform.GetComponent<TMP_Text>().text = "5";
        components[8].transform.GetComponent<TMP_Text>().text = "3 BB";
        components[9].transform.GetComponent<ToggleController>().isOn = false;
        components[10].transform.GetComponent<ToggleController>().isOn = false;
        components[11].transform.GetComponent<ToggleController>().isOn = false;
        components[12].transform.parent.Find("TimeMarkerSlider").GetComponent<Slider>().value = 0;

        saveBtn.gameObject.SetActive(true);
        startBtn.gameObject.SetActive(true);
        editBtn.gameObject.SetActive(false);

        isEditingTemplate = false;
        tableIdStatic = 0;

        Debug.Log("All Data Reset...");    
    }
}

public class TableData
{
    public string userId;
    public string tableName;
    public string playerCount;
    public string gameMode;
    public string actionTime;
    public string minBuyIn;
    public string fee;
    public string cap;
    public string hideRealTimeResult;
    public string gpsRestriction;
    public string ipRestriction;
    public string tableTime;
    public string entryCurrency;
    public string players;
    public string bootAmount;
    public string smallBlind;
    public string bigBlind;
    public string minBet;
    public string maxBet;
    public string potLimit;
    public string callTimmer;
    public string startGameTimmer;
    public string nextRoundTimmer;
    public string gameOverTimmer;
    public string gameIcon;
    public string iconBaseUrl;
    public string backgroundImg;
    public string tableId;
    public string clubId;
}
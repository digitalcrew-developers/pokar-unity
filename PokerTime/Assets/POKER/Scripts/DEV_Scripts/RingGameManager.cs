using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class RingGameManager : MonoBehaviour
{
    public static RingGameManager instance;

    private int memberCount = 9;

    public string gameType = "";
    public string templateType = "";
    public string templateSubType = "";

    [Space(7)]
    public List<GameObject> components = new List<GameObject>();

    [Space(7)]
    public Button incrementMemberCount;
    public Button decreamentMemberCount;

    [Space(7)]
    public Slider VPIPLevelSlider;
    public GameObject callTimeTextData;
    public GameObject banChattingDiamondObj;
    
    [Space(7)]
    public List<GameObject> autoStartMemberList = new List<GameObject>();

    [Space(7)]
    public List<GameObject> innerComponents = new List<GameObject>();

    [HideInInspector]
    public bool isPublishTemplateWithCreate = false;

    private void Awake()
    {
        instance = this;
    }


    private void Update()
    {
        //Enable/Disable Increment Button
        if (memberCount < 9)
            incrementMemberCount.interactable = true;
        else
            incrementMemberCount.interactable = false;

        //Enable/Disable Decrement Button
        if (memberCount > 2)
            decreamentMemberCount.interactable = true;
        else
            decreamentMemberCount.interactable = false;

        //Members Toggle list Enable/Disable
        if (components[12].transform.GetComponent<ToggleController>().isOn)
        {
            innerComponents[0].SetActive(true);
            innerComponents[0].transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(608.5f, 420.48f);
        }
        else
        {
            innerComponents[0].SetActive(false);
            innerComponents[0].transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(608.5f, 344f);
        }

        //Hand Threshold Slider Enable/ Disable
        if (VPIPLevelSlider.value > 0)
        {
            innerComponents[1].SetActive(true);
            innerComponents[1].transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(608, 520);
        }
        else
        {
            innerComponents[1].SetActive(false);
            innerComponents[1].transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(608, 414);
        }

        //Auto Extension Time Slider Enable/Disable
        if (components[14].transform.GetComponent<ToggleController>().isOn)
        {
            innerComponents[2].transform.GetComponent<Slider>().interactable = true;
        }
        else
        {
            innerComponents[2].transform.GetComponent<Slider>().interactable = false;
        }

        //Call Time Slider Enable/Disable
        if (components[21].transform.GetComponent<ToggleController>().isOn)
        {
            callTimeTextData.SetActive(true);
            innerComponents[3].SetActive(true);
            innerComponents[3].transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(608, 135);
            //callTime.interactable = true;
        }
        else
        {
            callTimeTextData.SetActive(false);
            //callTime.interactable = false;
            innerComponents[3].gameObject.SetActive(false);
            innerComponents[3].transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(608, 75);
        }

        //Ban Chatting Diamond Prefab Enable
        if (components[27].transform.GetComponent<ToggleController>().isOn)
        {
            innerComponents[4].SetActive(true);
        }
        else
        {
            innerComponents[4].SetActive(false);
        }
    }

    public void OnClickIncreaseMemberCount()
    {
        if (memberCount < 9)
        {
            memberCount++;
            components[1].transform.GetComponent<TMP_Text>().text = memberCount.ToString();
        }        

        switch (memberCount)
        {
            case 3:
                {
                    autoStartMemberList[0].SetActive(true);
                }
                break;
            case 4:
                {
                    for (int i = 0; i < 2; i++)
                    {
                        autoStartMemberList[i].SetActive(true);
                    }
                }
                break;
            case 5:
                {
                    for (int i = 0; i < 3; i++)
                    {
                       autoStartMemberList[i].SetActive(true);
                    }
                }
                break;
        }
    }

    public void OnClickDecreaseMemberCount()
    {
        if (memberCount > 2)
        {
            memberCount--;
            components[1].transform.GetComponent<TMP_Text>().text = memberCount.ToString();
        }       

        switch (memberCount)
        {
            case 2:
                {
                    for (int i = 2; i >= 0; i--)
                    {
                        autoStartMemberList[i].SetActive(false);
                    }
                }
                break;

            case 3:
                {
                    for (int i = 2; i >= 1; i--)
                    {
                        autoStartMemberList[i].SetActive(false);
                    }
                }
                break;
            case 4:
                {
                    autoStartMemberList[2].SetActive(false);
                }
                break;

        }
    }


    public void OnClickOnSave()
    {
        //ActionTime Toggle Group
        string actionTime = "";
        Toggle[] toggles_ActionTime = components[2].transform.GetComponentsInChildren<Toggle>();
        foreach (var t in toggles_ActionTime)
        {
            if (t.isOn)
            {
                actionTime = t.name;
                //Debug.Log("Toggle Name:" + actionTime);
            }
        }

        //Hi Lo Toggle Group
        string hi_Lo = "";
        Toggle[] toggles_Hi_Lo = components[6].transform.GetComponentsInChildren<Toggle>();
        foreach (var t in toggles_Hi_Lo)
        {
            if (t.isOn)
            {
                hi_Lo = t.name;
                //Debug.Log("Toggle Name:" + hi_Lo);
            }
        }

        //AutoStartWith Toggle Group
        string autoStartWith = "";
        Toggle[] toggles_AutoStartWith = components[13].transform.GetComponentsInChildren<Toggle>();
        foreach (var t in toggles_AutoStartWith)
        {
            if (t.isOn)
            {
                autoStartWith = t.name;
                //Debug.Log("Toggle Name:" + autoStartWith);
            }
        }

        //Risk Management Toggle Group
        //string riskManagement = "";
        //Toggle[] toggles_RiskManagement = components[15].transform.GetComponentsInChildren<Toggle>();
        //foreach (var t in toggles_RiskManagement)
        //{
        //    if (t.isOn)
        //    {
        //        riskManagement = t.name;
        //        Debug.Log("Toggle Name:" + riskManagement);
        //    }
        //}

        
        string blindString = "";
        string anteString = "";
        string callTime = "";
        string callTimeText = "";
        string chipWithdrawal = "";

        if (templateSubType.Equals("6 Plus"))
        {
            anteString = components[5].transform.GetComponent<TMP_Text>().text.ToString();
            chipWithdrawal = components[23].transform.GetComponent<ToggleController>().isOn.ToString().Equals("True") ? "On" : "Off";
            
            blindString = "";
            callTime = "";
            callTimeText = "";
        }
        else
        {
            anteString = "";
            chipWithdrawal = "";
            blindString = components[4].transform.GetComponent<TMP_Text>().text;
            callTime = components[21].transform.GetComponent<ToggleController>().isOn.ToString().Equals("True") ? "On" : "Off";
            callTimeText = components[22].transform.GetComponent<TMP_Text>().text;            
        }


        //Debug.Log("Blind String: " + blindString);
        //Debug.Log("Ante String" + anteString);
        //Debug.Log("Chip Withdrawal" + chipWithdrawal);
        //Debug.Log("Call Time" + callTime);
        //Debug.Log("Call Time Text" + callTimeText);


        //Debug.Log("ClubID: " + ClubDetailsUIManager.instance.GetClubId());
        //Debug.Log("GameType: " + gameType);
        //Debug.Log("TemplateName: " + components[0].transform.GetComponent<TMP_InputField>().text.ToString());
        //Debug.Log("templateType: " + templateType);
        //Debug.Log("Status: " + "Saved");
        //Debug.Log("TableId: " + "");
        //Debug.Log("Template Sub Type: " + templateSubType);
        //Debug.Log("Member Count: " + components[1].transform.GetComponent<TMP_Text>().text);
        //Debug.Log("ActionTime: " + actionTime);
        //Debug.Log("ExclusiveTable: " + (components[3].transform.GetComponent<ToggleController>().isOn.ToString().Equals("True") ? "On" : "Off"));
        //Debug.Log("Blinds Type: " + blindString);
        //Debug.Log("Ante Type: " + anteString);
        //Debug.Log("high_low: " + hi_Lo);
        //Debug.Log("MinBuy: " + components[7].transform.GetComponent<TMP_Text>().text);
        //Debug.Log("MaxBuy: " + components[8].transform.GetComponent<TMP_Text>().text);
        //Debug.Log("MinVPIP: " + components[9].transform.GetComponent<TMP_Text>().text.Substring(0, components[9].transform.GetComponent<TMP_Text>().text.ToString().Length - 1));
        //Debug.Log("VPIP Level: " + components[10].transform.GetComponent<TMP_Text>().text.Substring(0, components[10].transform.GetComponent<TMP_Text>().text.ToString().Length - 1));
        //Debug.Log("Hand Threshold: " + components[11].transform.GetComponent<TMP_Text>().text);
        //Debug.Log("AutoStart: " + (components[12].transform.GetComponent<ToggleController>().isOn.ToString().Equals("True") ? "On" : "Off"));
        //Debug.Log("AutoStartWith: " + autoStartWith);
        //Debug.Log("AutoExtension: " + (components[14].transform.GetComponent<ToggleController>().isOn.ToString().Equals("True") ? "On" : "Off"));
        //Debug.Log("AutoExtensionTimes: " + components[15].transform.GetComponent<TMP_Text>().text.ToString());
        //Debug.Log("AutoOpen: " + (components[16].transform.GetComponent<ToggleController>().isOn.ToString().Equals("True") ? "On" : "Off"));
        //Debug.Log("RunItMulti: " + (components[17].transform.GetComponent<Toggle>().isOn.ToString().Equals("True") ? "Yes" : "No"));
        //Debug.Log("EVChop: " + (components[18].transform.GetComponent<Toggle>().isOn.ToString().Equals("True") ? "Yes" : "No"));
        //Debug.Log("Fee : " + components[19].transform.GetComponent<TMP_Text>().text.ToString().Substring(0, components[19].transform.GetComponent<TMP_Text>().text.ToString().Length - 1));
        //Debug.Log("Cap: " + components[20].transform.GetComponent<TMP_Text>().text.ToString().Substring(0, components[20].transform.GetComponent<TMP_Text>().text.ToString().Length - 3));
        //Debug.Log("Call Time: " + callTime);
        //Debug.Log("Time Text: " + callTimeText);
        //Debug.Log("Chip Withdrawal: " +chipWithdrawal);
        //Debug.Log("Authorized: " + (components[24].transform.GetComponent<ToggleController>().isOn.ToString().Equals("True") ? "On" : "Off"));
        //Debug.Log("GPS: " + (components[25].transform.GetComponent<ToggleController>().isOn.ToString().Equals("True") ? "On" : "Off"));
        //Debug.Log("IP: " + (components[26].transform.GetComponent<ToggleController>().isOn.ToString().Equals("True") ? "On" : "Off"));
        //Debug.Log("Ban Chatting: " + (components[27].transform.GetComponent<ToggleController>().isOn.ToString().Equals("True") ? "On" : "Off"));
        //Debug.Log("Hours: " + components[28].transform.GetComponent<TMP_Text>().text.Substring(0, components[28].transform.GetComponent<TMP_Text>().text.Length - 2));


        //Request Data
        string requestData = "{\"clubId\":\"" + ClubDetailsUIManager.instance.GetClubId() + "\"," +
            "\"gameType\":\"" + gameType + "\"," +
            "\"templateName\":\"" + components[0].transform.GetComponent<TMP_InputField>().text.ToString() + "\"," +
            "\"templateType\":\"" + templateType + "\"," +
            "\"status\":\"" + "Saved" + "\"," +
            "\"tableId\":\"" + "" + "\"," +
            "\"settingData\":[{\"templateSubType\":\"" + templateSubType + "\"," +
            "\"memberCount\":\"" + components[1].transform.GetComponent<TMP_Text>().text.ToString() + "\"," +
            "\"actionTime\":\"" + actionTime + "\"," +
            "\"exclusiveTable\":\"" + (components[3].transform.GetComponent<ToggleController>().isOn.ToString().Equals("True") ? "On" : "Off") + "\"," +
            "\"blinds\":\"" + blindString + "\"," +
            "\"ante\":\"" + anteString + "\"," +
            "\"high_low\":\"" + hi_Lo + "\"," +
            "\"buyInMin\":\"" + components[7].transform.GetComponent<TMP_Text>().text + "\"," +
            "\"buyInMax\":\"" + components[8].transform.GetComponent<TMP_Text>().text + "\"," +
            "\"minVPIP\":\"" + components[9].transform.GetComponent<TMP_Text>().text.Substring(0, components[9].transform.GetComponent<TMP_Text>().text.ToString().Length - 1) + "\"," +
            "\"VPIPLevel\":\"" + components[10].transform.GetComponent<TMP_Text>().text.Substring(0, components[10].transform.GetComponent<TMP_Text>().text.ToString().Length - 1) + "\"," +
            "\"handsThreshold\":\"" + components[11].transform.GetComponent<TMP_Text>().text + "\"," +
            "\"autoStart\":\"" + (components[12].transform.GetComponent<ToggleController>().isOn.ToString().Equals("True") ? "On" : "Off") + "\"," +
            "\"autoStartWith\":\"" + autoStartWith + "\"," +
            "\"autoExtension\":\"" + (components[14].transform.GetComponent<ToggleController>().isOn.ToString().Equals("True") ? "On" : "Off") + "\"," +
            "\"autoExtensionTimes\":\"" + components[15].transform.GetComponent<TMP_Text>().text + "\"," +
            "\"autoOpen\":\"" + (components[16].transform.GetComponent<ToggleController>().isOn.ToString().Equals("True") ? "On" : "Off") + "\"," +
            //"\"riskManagement\":\"" + /*riskManagement*/"" + "\"," +
            "\"runItMulti\":\"" + (components[17].transform.GetComponent<Toggle>().isOn.ToString().Equals("True") ? "Yes" : "No") + "\"," +
            "\"evChop\":\"" + (components[18].transform.GetComponent<Toggle>().isOn.ToString().Equals("True") ? "Yes" : "No") + "\"," +
            "\"fee\":\"" + components[19].transform.GetComponent<TMP_Text>().text.ToString().Substring(0, components[19].transform.GetComponent<TMP_Text>().text.ToString().Length - 1) + "\"," +
            "\"cap\":\"" + components[20].transform.GetComponent<TMP_Text>().text.ToString().Substring(0, components[20].transform.GetComponent<TMP_Text>().text.ToString().Length - 3) + "\"," +
            "\"calltime\":\"" + callTime + "\"," +
            "\"time\":\"" + callTimeText + "\"," +
            "\"chipWithdrawal\":\"" + chipWithdrawal + "\"," +
            "\"authorizedBuyIn\":\"" + (components[24].transform.GetComponent<ToggleController>().isOn.ToString().Equals("True") ? "On" : "Off") + "\"," +
            "\"GPSRestriction\":\"" + (components[25].transform.GetComponent<ToggleController>().isOn.ToString().Equals("True") ? "On" : "Off") + "\"," +
            "\"IPRestriction\":\"" + (components[26].transform.GetComponent<ToggleController>().isOn.ToString().Equals("True") ? "On" : "Off") + "\"," +
            "\"banChatting\":\"" + (components[27].transform.GetComponent<ToggleController>().isOn.ToString().Equals("True") ? "On" : "Off") + "\"," +
            "\"hours\":\"" + components[28].transform.GetComponent<TMP_Text>().text.Substring(0, components[28].transform.GetComponent<TMP_Text>().text.Length - 2) + "\"}]}";

        WebServices.instance.SendRequest(RequestType.CreateTemplate, requestData, true, OnServerResponseFound);
    }

    public void OnClickOnCreate()
    {
        isPublishTemplateWithCreate = true;
        OnClickOnSave();
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
            case RequestType.CreateTemplate:
                {
                    JsonData data = JsonMapper.ToObject(serverResponse);

                    if (data["success"].ToString() == "1")
                    {
                        ClubTableController.instance.ShowPopUp("Template saved successfully.");
                        //Debug.Log(data["message"].ToString());
                        //joinClubPopUp.SetActive(false);
                        //MainMenuController.instance.ShowMessage(data["message"].ToString());
                        ClubTableController.instance.RequestTemplateData(false);
                    }
                    else
                    {
                        Debug.Log(data["message"].ToString());
                        MainMenuController.instance.ShowMessage(data["message"].ToString());
                    }

                    if(isPublishTemplateWithCreate)
                    {
                        string requestData = "{\"clubId\":\"" + ClubDetailsUIManager.instance.GetClubId() + "\"," +
                                "\"status\":\"" + "Published" + "\"," +
                                "\"tableIds\":[\"" + data["tableId"].ToString() + "\"]}";

                        WebServices.instance.SendRequest(RequestType.UpdateTemplateStatus, requestData, true, OnServerResponseFound);
                    }
                }
                break;

            case RequestType.UpdateTemplateStatus:
                {
                    JsonData data = JsonMapper.ToObject(serverResponse);

                    if (data["success"].ToString() == "1")
                    {
                        if (data["message"].ToString().Equals("Template Published"))
                        {
                            Debug.Log("Tamplate Published Successfully");
                            isPublishTemplateWithCreate = false;
                            //StartCoroutine(ShowPopUp("Template Published ", 1.25f));
                            ClubDetailsUIManager.instance.GetClubTemplates();
                            ClubTableController.instance.RequestTemplateData(true);

                            if (ClubTableController.instance.createTablePanel.activeSelf)
                                ClubTableController.instance.createTablePanel.SetActive(false);
                        }
                    }
                    else
                    {
                        //MainMenuController.instance.ShowMessage(data["message"].ToString());
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
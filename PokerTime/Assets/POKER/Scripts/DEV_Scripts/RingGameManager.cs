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
    public GameObject banChattingDiamondObj;

    [Space(7)]
    public List<GameObject> autoStartMemberList = new List<GameObject>();

    [Space(7)]
    public List<GameObject> innerComponents = new List<GameObject>();

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
        if (components[10].transform.GetComponent<ToggleController>().isOn)
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
        //if (components[9].transform.GetComponent<Slider>().value > 15)
        //{
        //    innerComponents[1].SetActive(true);
        //    innerComponents[1].transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(608, 510);
        //}
        //else
        //{
        //    innerComponents[1].SetActive(false);
        //    innerComponents[1].transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(608, 414);
        //}

        //Auto Extension Time Slider Enable/Disable
        if (components[12].transform.GetComponent<ToggleController>().isOn)
        {
            innerComponents[2].transform.GetComponent<Slider>().interactable = true;
        }
        else
        {
            innerComponents[2].transform.GetComponent<Slider>().interactable = false;
        }

        //Call Time Slider Enable/Disable
        if (components[18].transform.GetComponent<ToggleController>().isOn)
        {
            innerComponents[3].SetActive(true);
            innerComponents[3].transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(608, 135);
            //callTime.interactable = true;
        }
        else
        {
            //callTime.interactable = false;
            innerComponents[3].gameObject.SetActive(false);
            innerComponents[3].transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(608, 75);
        }

        //Ban Chatting Diamond Prefab Enable
        if (components[24].transform.GetComponent<ToggleController>().isOn)
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
            components[0].transform.GetComponent<TMP_Text>().text = memberCount.ToString();
        }
        //else
        //{
        //    incrementMemberCount.interactable = false;
        //}

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
            components[0].transform.GetComponent<TMP_Text>().text = memberCount.ToString();
        }
        //else
        //{
        //    decreamentMemberCount.interactable = false;
        //}

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
        Toggle[] toggles_ActionTime = components[1].transform.GetComponentsInChildren<Toggle>();
        foreach (var t in toggles_ActionTime)
        {
            if (t.isOn)
            {
                actionTime = t.name;
                Debug.Log("Toggle Name:" + actionTime);
            }
        }

        //AutoStartWith Toggle Group
        string autoStartWith = "";
        Toggle[] toggles_AutoStartWith = components[11].transform.GetComponentsInChildren<Toggle>();
        foreach (var t in toggles_AutoStartWith)
        {
            if (t.isOn)
            {
                autoStartWith = t.name;
                Debug.Log("Toggle Name:" + autoStartWith);
            }
        }

        //Risk Management Toggle Group
        string riskManagement = "";
        Toggle[] toggles_RiskManagement = components[15].transform.GetComponentsInChildren<Toggle>();
        foreach (var t in toggles_RiskManagement)
        {
            if (t.isOn)
            {
                riskManagement = t.name;
                Debug.Log("Toggle Name:" + riskManagement);
            }
        }

        //Hi Lo Toggle Group
        string hi_Lo = "";
        Toggle[] toggles_Hi_Lo = components[26].transform.GetComponentsInChildren<Toggle>();
        foreach (var t in toggles_Hi_Lo)
        {
            if (t.isOn)
            {
                hi_Lo = t.name;
                Debug.Log("Toggle Name:" + hi_Lo);
            }
        }

        
        string blindString = "";
        string anteString = "";
        string callTime = "";
        string callTimeText = "";
        string chipWithdrawal = "";

        if (templateSubType.Equals("6 Plus"))
        {
            anteString = components[4].transform.GetComponent<TMP_Text>().text.ToString();
            chipWithdrawal = components[20].transform.GetComponent<ToggleController>().isOn.ToString();
            
            blindString = "";
            callTime = "";
            callTimeText = "";
        }
        else
        {
            //string[] oldStr = components[3].transform.GetComponent<TMP_Text>().text.Split('/');
            //for (int i = 0; i < oldStr.Length; i++)
            //{
            //    Debug.Log("Blind Data:" + oldStr[i]);
            //    blindString = blindString + oldStr[i];
            //}

            anteString = "";
            chipWithdrawal = "";
            callTime = components[18].transform.GetComponent<ToggleController>().isOn.ToString().Equals("True") ? "On" : "Off";
            callTimeText = components[19].transform.GetComponent<TMP_Text>().text;            
        }


        //Debug.Log("Blind String: " + blindString);
        //Debug.Log("Ante String" + anteString);
        //Debug.Log("Chip Withdrawal" + chipWithdrawal);
        //Debug.Log("Call Time" + callTime);
        //Debug.Log("Call Time Text" + callTimeText);


        Debug.Log("ClubID: " + ClubDetailsUIManager.instance.GetClubId());
        Debug.Log("GameType: " + gameType);
        Debug.Log("TemplateName: " + components[27].transform.GetComponent<TMP_InputField>().text.ToString());
        Debug.Log("templateType: " + templateType);
        Debug.Log("Status: " + "Saved");
        Debug.Log("TableId: " + "");
        Debug.Log("Template Sub Type: " + templateSubType);
        Debug.Log("Member Count: " + components[0].transform.GetComponent<TMP_Text>().text);
        Debug.Log("ActionTime: " + actionTime);
        Debug.Log("ExclusiveTable: " + (components[2].transform.GetComponent<ToggleController>().isOn.ToString().Equals("True") ? "On" : "Off"));
        Debug.Log("Blinds Type: " + components[3].transform.GetComponent<TMP_Text>().text);
        Debug.Log("Ante Type: " + components[4].transform.GetComponent<TMP_Text>().text);
        Debug.Log("high_low: " + hi_Lo);
        Debug.Log("MinBuy: " + components[5].transform.GetComponent<TMP_Text>().text);
        Debug.Log("MaxBuy: " + components[6].transform.GetComponent<TMP_Text>().text);
        Debug.Log("MinVPIP: " + components[7].transform.GetComponent<TMP_Text>().text.Substring(0, components[7].transform.GetComponent<TMP_Text>().text.ToString().Length - 1));
        Debug.Log("VPIP Level: " + components[8].transform.GetComponent<TMP_Text>().text.Substring(0, components[8].transform.GetComponent<TMP_Text>().text.ToString().Length - 1));
        Debug.Log("Hand Threshold: " + components[9].transform.GetComponent<TMP_Text>().text.Substring(0, components[8].transform.GetComponent<TMP_Text>().text.ToString().Length - 1));
        Debug.Log("AutoStart: " + (components[10].transform.GetComponent<ToggleController>().isOn.ToString().Equals("True") ? "On" : "Off"));
        Debug.Log("AutoStartWith: " + autoStartWith);
        Debug.Log("AutoExtension: " + (components[12].transform.GetComponent<ToggleController>().isOn.ToString().Equals("True") ? "On" : "Off"));
        Debug.Log("AsutoExtensionTimes: " + components[13].transform.GetComponent<TMP_Text>().text.ToString());
        Debug.Log("AutoOpen: " + (components[14].transform.GetComponent<ToggleController>().isOn.ToString().Equals("True") ? "On" : "Off"));
        Debug.Log("RunItMulti: " + "");
        Debug.Log("EVChop: " + "");
        Debug.Log("Fee : " + components[16].transform.GetComponent<TMP_Text>().text.ToString().Substring(0, components[16].transform.GetComponent<TMP_Text>().text.ToString().Length - 1));
        Debug.Log("Cap: " + components[17].transform.GetComponent<TMP_Text>().text.ToString().Substring(0, components[17].transform.GetComponent<TMP_Text>().text.ToString().Length - 3));
        Debug.Log("Call Time: " + callTime);
        Debug.Log("Time Text: " + callTimeText);
        Debug.Log("Chip Withdrawal: " +chipWithdrawal);
        Debug.Log("Autorized: " + (components[21].transform.GetComponent<ToggleController>().isOn.ToString().Equals("True") ? "On" : "Off"));
        Debug.Log("GPS: " + (components[22].transform.GetComponent<ToggleController>().isOn.ToString().Equals("True") ? "On" : "Off"));
        Debug.Log("IP: " + (components[23].transform.GetComponent<ToggleController>().isOn.ToString().Equals("True") ? "On" : "Off"));
        Debug.Log("Ban Chatting: " + (components[24].transform.GetComponent<ToggleController>().isOn.ToString().Equals("True") ? "On" : "Off"));
        Debug.Log("Hours: " + components[25].transform.GetComponent<Slider>().value.ToString());
        

        //Request Data
        string requestData = "{\"clubId\":\"" + ClubDetailsUIManager.instance.GetClubId() + "\"," +
            "\"gameType\":\"" + gameType + "\"," +
            "\"templateName\":\"" + components[27].transform.GetComponent<TMP_InputField>().text.ToString() + "\"," +
            "\"templateType\":\"" + templateType + "\"," +
            "\"status\":\"" + "Saved" + "\"," +
            "\"tableId\":\"" + "" + "\"," +
            "\"settingData\":[{\"templateSubType\":\"" + templateSubType + "\"," +
            "\"memberCount\":\"" + components[0].transform.GetComponent<TMP_Text>().text.ToString() + "\"," +
            "\"actionTime\":\"" + actionTime + "\"," +
            "\"exclusiveTable\":\"" + (components[2].transform.GetComponent<ToggleController>().isOn.ToString().Equals("True") ? "On" : "Off") + "\"," +
            "\"blinds\":\"" + blindString.ToString() + "\"," +
            "\"ante\":\"" + anteString.ToString() + "\"," +
            "\"high_low\":\"" + "" + "\"," +
            "\"buyInMin\":\"" + components[5].transform.GetComponent<TMP_Text>().text + "\"," +
            "\"buyInMax\":\"" + components[6].transform.GetComponent<TMP_Text>().text + "\"," +
            "\"minVPIP\":\"" + components[7].transform.GetComponent<TMP_Text>().text.Substring(0, components[7].transform.GetComponent<TMP_Text>().text.ToString().Length - 1) + "\"," +
            "\"VPIPLevel\":\"" + components[8].transform.GetComponent<TMP_Text>().text.Substring(0, components[8].transform.GetComponent<TMP_Text>().text.ToString().Length - 1) + "\"," +
            "\"handsThreshold\":\"" + components[9].transform.GetComponent<TMP_Text>().text.Substring(0, components[8].transform.GetComponent<TMP_Text>().text.ToString().Length - 1) + "\"," +
            "\"autoStart\":\"" + (components[10].transform.GetComponent<ToggleController>().isOn.ToString().Equals("True") ? "On" : "Off") + "\"," +
            "\"autoStartWith\":\"" + autoStartWith + "\"," +
            "\"autoExtension\":\"" + (components[12].transform.GetComponent<ToggleController>().isOn.ToString().Equals("True") ? "On" : "Off") + "\"," +
            "\"autoExtensionTimes\":\"" + components[13].transform.GetComponent<TMP_Text>().text.ToString() + "\"," +
            "\"autoOpen\":\"" + (components[14].transform.GetComponent<ToggleController>().isOn.ToString().Equals("True") ? "On" : "Off") + "\"," +
            //"\"riskManagement\":\"" + /*riskManagement*/"" + "\"," +
            "\"runItMulti\":\"" + /*anteString.ToString()*/"" + "\"," +
            "\"evChop\":\"" + /*anteString.ToString()*/"" + "\"," +
            "\"fee\":\"" + components[16].transform.GetComponent<TMP_Text>().text.ToString().Substring(0, components[16].transform.GetComponent<TMP_Text>().text.ToString().Length - 1) + "\"," +
            "\"cap\":\"" + components[17].transform.GetComponent<TMP_Text>().text.ToString().Substring(0, components[17].transform.GetComponent<TMP_Text>().text.ToString().Length - 3) + "\"," +
            "\"calltime\":\"" + callTime + "\"," +
            "\"time\":\"" + callTimeText + "\"," +
            "\"chipWithdrawal\":\"" + chipWithdrawal.ToString() + "\"," +
            "\"authorizedBuyIn\":\"" + (components[21].transform.GetComponent<ToggleController>().isOn.ToString().Equals("True") ? "On" : "Off") + "\"," +
            "\"GPSRestriction\":\"" + (components[22].transform.GetComponent<ToggleController>().isOn.ToString().Equals("True") ? "On" : "Off") + "\"," +
            "\"IPRestriction\":\"" + (components[23].transform.GetComponent<ToggleController>().isOn.ToString().Equals("True") ? "On" : "Off") + "\"," +
            "\"banChatting\":\"" + (components[24].transform.GetComponent<ToggleController>().isOn.ToString().Equals("True") ? "On" : "Off") + "\"," +
            "\"hours\":\"" + components[25].transform.GetComponent<Slider>().value.ToString() + "\"}]}";

        WebServices.instance.SendRequest(RequestType.CreateTemplate, requestData, true, OnServerResponseFound);
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
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


        //Debug.Log("Length: " + components[12].transform.GetComponent<TMP_Text>().text.ToString().Substring(7, 2));

        Debug.Log("ClubID: " + ClubDetailsUIManager.instance.GetClubId());
        Debug.Log("GameType: " + gameType);
        Debug.Log("GameType: " + components[27].transform.GetComponent<TMP_InputField>().text.ToString());
        Debug.Log("templateType: " + templateType);
        Debug.Log("Template Sub Type: " + templateSubType);
        Debug.Log("ExclusiveTable: " + (components[2].transform.GetComponent<ToggleController>().isOn.ToString().Equals("True") ? "On" : "Off"));
        Debug.Log("Blinds Type: " + components[3].transform.GetComponent<TMP_Text>().text.Substring(8, components[3].transform.GetComponent<TMP_Text>().text.Length - 8));
        Debug.Log("MinBuy: " + components[5].transform.GetComponent<TMP_Text>().text.Substring(7, 2));
        Debug.Log("MaxBuy: " + components[6].transform.GetComponent<TMP_Text>().text.Substring(5, components[5].transform.GetComponent<TMP_Text>().text.Length - 5));
        Debug.Log("minVPIP: " + (components[7].transform.GetComponent<TMP_Text>().text.Length.ToString().Equals("13") ? components[6].transform.GetComponent<TMP_Text>().text.Substring(11, 1) : components[6].transform.GetComponent<TMP_Text>().text.Substring(11, 2)));
        Debug.Log("VPIP Level: " + (components[8].transform.GetComponent<TMP_Text>().text.Length.ToString().Equals("14") ? components[7].transform.GetComponent<TMP_Text>().text.Substring(12, 1) : components[6].transform.GetComponent<TMP_Text>().text.Substring(12, 2)));
        Debug.Log("autoStart: " + (components[10].transform.GetComponent<ToggleController>().isOn.ToString().Equals("True") ? "On" : "Off"));
        Debug.Log("AutoStartWith: " + autoStartWith);
        Debug.Log("AutoExtension: " + (components[12].transform.GetComponent<ToggleController>().isOn.ToString().Equals("True") ? "On" : "Off"));
        Debug.Log("AutoExtensionTimes: " + components[13].transform.GetComponent<TMP_Text>().text.ToString().Substring(7, 2));
        Debug.Log("AutoOpen: " + (components[14].transform.GetComponent<ToggleController>().isOn.ToString().Equals("True") ? "On" : "Off"));
        Debug.Log("AutoStartWith: " + riskManagement);
        Debug.Log("Fee : " + components[16].transform.GetComponent<TMP_Text>().text.ToString().Substring(0, components[15].transform.GetComponent<TMP_Text>().text.ToString().Length - 1));
        Debug.Log("Cap: " + components[17].transform.GetComponent<TMP_Text>().text.ToString().Substring(0, components[16].transform.GetComponent<TMP_Text>().text.ToString().Length - 3));
        Debug.Log("Call Time: " + (components[18].transform.GetComponent<ToggleController>().isOn.ToString().Equals("True") ? "On" : "Off"));
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
            "\"blinds\":\"" + /*(templateSubType.Equals("6+") ? "" :*/ components[3].transform.GetComponent<TMP_Text>().text.Substring(8, components[3].transform.GetComponent<TMP_Text>().text.Length - 8)/*)*/ + "\"," +
            "\"ante\":\"" + /*(templateSubType.Equals("6+") ? components[4].transform.GetComponent<TMP_Text>().text : "")*/"" + "\"," +
            "\"buyInMin\":\"" + components[5].transform.GetComponent<TMP_Text>().text.Substring(7, components[4].transform.GetComponent<TMP_Text>().text.Length - 7) + "\"," +
            "\"buyInMax\":\"" + components[6].transform.GetComponent<TMP_Text>().text.Substring(5, components[5].transform.GetComponent<TMP_Text>().text.Length - 5) + "\"," +
            "\"minVPIP\":\"" + (components[7].transform.GetComponent<TMP_Text>().text.Length.ToString().Equals("13") ? components[6].transform.GetComponent<TMP_Text>().text.Substring(11, 1) : components[6].transform.GetComponent<TMP_Text>().text.Substring(11, 2)) + "\"," +
            "\"VPIPLevel\":\"" + (components[8].transform.GetComponent<TMP_Text>().text.Length.ToString().Equals("14") ? components[7].transform.GetComponent<TMP_Text>().text.Substring(12, 1) : components[6].transform.GetComponent<TMP_Text>().text.Substring(12, 2)) + "\"," +
            "\"handsThreshold\":\"" + /*(components[9].transform.GetComponent<TMP_Text>().text.Length.ToString().Equals("19") ? components[8].transform.GetComponent<TMP_Text>().text.Substring(17, 1) : components[6].transform.GetComponent<TMP_Text>().text.Substring(17, 2))*/"" + "\"," +
            "\"autoStart\":\"" + (components[10].transform.GetComponent<ToggleController>().isOn.ToString().Equals("True") ? "On" : "Off") + "\"," +
            "\"autoStartWith\":\"" + autoStartWith + "\"," +
            "\"autoExtension\":\"" + (components[12].transform.GetComponent<ToggleController>().isOn.ToString().Equals("True") ? "On" : "Off") + "\"," +
            "\"autoExtensionTimes\":\"" + components[13].transform.GetComponent<TMP_Text>().text.ToString().Substring(7, 2) + "\"," +
            "\"autoOpen\":\"" + (components[14].transform.GetComponent<ToggleController>().isOn.ToString().Equals("True") ? "On" : "Off") + "\"," +
            "\"riskManagement\":\"" + riskManagement + "\"," +
            "\"fee\":\"" + components[16].transform.GetComponent<TMP_Text>().text.ToString().Substring(0, components[15].transform.GetComponent<TMP_Text>().text.ToString().Length - 1) + "\"," +
            "\"cap\":\"" + components[17].transform.GetComponent<TMP_Text>().text.ToString().Substring(0, components[16].transform.GetComponent<TMP_Text>().text.ToString().Length - 3) + "\"," +
            "\"calltime\":\"" + (components[18].transform.GetComponent<ToggleController>().isOn.ToString().Equals("True") ? "On" : "Off") + "\"," +
            "\"time\":\"" + "" + "\"," +
            "\"chipWithdrawal\":\"" + /*(templateSubType.Equals("6+") ? components[20].transform.GetComponent<TMP_Text>().text : "")*/"" + "\"," +
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
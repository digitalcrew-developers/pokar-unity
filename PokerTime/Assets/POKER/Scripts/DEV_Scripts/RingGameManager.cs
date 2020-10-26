using LitJson;
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
        if (components[8].transform.GetComponent<ToggleController>().isOn)
        {
            innerComponents[0].SetActive(true);
            innerComponents[0].transform.GetComponent<RectTransform>().sizeDelta = new Vector2(608.5f, 420.48f);
        }
        else
        {
            innerComponents[0].SetActive(false);
            innerComponents[0].transform.GetComponent<RectTransform>().sizeDelta = new Vector2(608.5f, 344f);
        }

        //Hand Threshold Slider Enable/Disable
        if (components[6].transform.GetComponent<Slider>().value > 15)
        {
            innerComponents[1].SetActive(true);
            innerComponents[1].transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(608, 510);
        }
        else
        {
            innerComponents[1].SetActive(false);
            innerComponents[1].transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(608, 414);
        }

        //Auto Extension Time Slider Enable/Disable
        if (components[10].transform.GetComponent<ToggleController>().isOn)
        {
            innerComponents[2].transform.GetComponent<Slider>().interactable = true;
        }
        else
        {
            innerComponents[2].transform.GetComponent<Slider>().interactable = false;
        }

        //Call Time Slider Enable/Disable
        if (components[16].transform.GetComponent<ToggleController>().isOn)
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
        if (components[21].transform.GetComponent<ToggleController>().isOn)
        {
            innerComponents[4].SetActive(true);
        }
        else
        {
            innerComponents[4].SetActive(false);
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
                //Debug.Log("Toggle Name:" + actionTime);
            }
        }

        //AutoStartWith Toggle Group
        string autoStartWith = "";
        Toggle[] toggles_AutoStartWith = components[9].transform.GetComponentsInChildren<Toggle>();
        foreach (var t in toggles_AutoStartWith)
        {
            if (t.isOn)
            {
                autoStartWith = t.name;
                //Debug.Log("Toggle Name:" + autoStartWith);
            }
        }

        //Debug.Log("Club ID: " + components[1].transform./*GetComponent<TMP_Text>().text.ToString()*/name);

        //Request Data
        string requestData = "{\"clubId\":\"" + ClubDetailsUIManager.instance.GetClubId() + "\"," +
            "\"gameType\":\"" + gameType + "\"," +
            "\"templateType\":\"" + templateType + "\"," +
            "\"status\":\"" + "Saved" + "\"," +
            "\"tableId\":\"" + "" + "\"," +
            "\"settingData\":[{\"templateSubType\":\"" + templateSubType + "\"," +
            "\"memberCount\":\"" + components[0].transform.GetComponent<TMP_Text>().text.ToString() + "\"," +
            "\"actionTime\":\"" + actionTime + "\"," +
            "\"exclusiveTable\":\"" + (components[2].transform.GetComponent<ToggleController>().isOn.ToString().Equals("True") ? "On" : "Off") + "\"," +
            "\"blinds\":\"" + components[3].transform.GetComponent<Slider>().value.ToString() + "\"," +
            "\"buyInMin\":\"" + components[4].transform.GetComponent<RangeSlider>().LowValue.ToString() + "\"," +
            "\"buyInMax\":\"" + components[4].transform.GetComponent<RangeSlider>().HighValue.ToString() + "\"," +
            "\"minVPIP\":\"" + components[5].transform.GetComponent<Slider>().value.ToString() + "\"," +
            "\"VPIPLevel\":\"" + components[6].transform.GetComponent<Slider>().value.ToString() + "\"," +
            "\"handsThreshold\":\"" + ""/*components[7].transform.GetComponent<Slider>().value.ToString()*/ + "\"," +
            "\"autoStart\":\"" + (components[8].transform.GetComponent<ToggleController>().isOn.ToString().Equals("True") ? "On" : "Off") + "\"," +
            "\"autoStartWith\":\"" + autoStartWith + "\"," +
            "\"autoExtension\":\"" + (components[10].transform.GetComponent<ToggleController>().isOn.ToString().Equals("True") ? "On" : "Off") + "\"," +
            "\"autoExtensionTimes\":\"" + components[11].transform.GetComponent<Slider>().value.ToString() + "\"," +
            "\"autoOpen\":\"" + (components[12].transform.GetComponent<ToggleController>().isOn.ToString().Equals("True") ? "On" : "Off") + "\"," +
            "\"riskManagement\":\"" + "" + "\"," +
            "\"fee\":\"" + components[14].transform.GetComponent<TMP_Text>().text.ToString().Substring(0, components[14].transform.GetComponent<TMP_Text>().text.ToString().Length - 1) + "\"," +
            "\"cap\":\"" + components[15].transform.GetComponent<TMP_Text>().text.ToString().Substring(0, components[15].transform.GetComponent<TMP_Text>().text.ToString().Length - 3) + "\"," +
            "\"calltime\":\"" + (components[16].transform.GetComponent<ToggleController>().isOn.ToString().Equals("True") ? "On" : "Off") + "\"," +
            "\"authorizedBuyIn\":\"" + (components[18].transform.GetComponent<ToggleController>().isOn.ToString().Equals("True") ? "On" : "Off") + "\"," +
            "\"GPSRestriction\":\"" + (components[19].transform.GetComponent<ToggleController>().isOn.ToString().Equals("True") ? "On" : "Off") + "\"," +
            "\"IPRestriction\":\"" + (components[20].transform.GetComponent<ToggleController>().isOn.ToString().Equals("True") ? "On" : "Off") + "\"," +
            "\"banChatting\":\"" + (components[21].transform.GetComponent<ToggleController>().isOn.ToString().Equals("True") ? "On" : "Off") + "\"," +
            "\"hours\":\"" + components[22].transform.GetComponent<Slider>().value.ToString() + "\"}]}";

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
using LitJson;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateTableManagerTeen : MonoBehaviour
{
    public static CreateTableManagerTeen instance;

    private int memberCount = 9;

    public string gameType = "";
    public string templateType = "";
    public string templateSubType = "";

    [Space(7)]
    public List<GameObject> components = new List<GameObject>();

    [Space(7)]
    public Button incrementMemberCount;
    public Button decreamentMemberCount;

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
        //if (components[12].transform.GetComponent<ToggleController>().isOn)
        //{
        //    innerComponents[0].SetActive(true);
        //    innerComponents[0].transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(608.5f, 420.48f);
        //}
        //else
        //{
        //    innerComponents[0].SetActive(false);
        //    innerComponents[0].transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(608.5f, 344f);
        //}

        //Hand Threshold Slider Enable/ Disable
        //if (VPIPLevelSlider.value > 0)
        //{
        //    innerComponents[1].SetActive(true);
        //    innerComponents[1].transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(608, 520);
        //}
        //else
        //{
        //    innerComponents[1].SetActive(false);
        //    innerComponents[1].transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(608, 414);
        //}

        //Auto Extension Time Slider Enable/Disable
        //if (components[14].transform.GetComponent<ToggleController>().isOn)
        //{
        //    innerComponents[2].transform.GetComponent<Slider>().interactable = true;
        //}
        //else
        //{
        //    innerComponents[2].transform.GetComponent<Slider>().interactable = false;
        //}

        //Call Time Slider Enable/Disable
        //if (components[21].transform.GetComponent<ToggleController>().isOn)
        //{
        //    callTimeTextData.SetActive(true);
        //    innerComponents[3].SetActive(true);
        //    innerComponents[3].transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(608, 135);
        //    //callTime.interactable = true;
        //}
        //else
        //{
        //    callTimeTextData.SetActive(false);
        //    //callTime.interactable = false;
        //    innerComponents[3].gameObject.SetActive(false);
        //    innerComponents[3].transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(608, 75);
        //}

        //Ban Chatting Diamond Prefab Enable
        //if (components[27].transform.GetComponent<ToggleController>().isOn)
        //{
        //    innerComponents[4].SetActive(true);
        //}
        //else
        //{
        //    innerComponents[4].SetActive(false);
        //}
    }

    public void OnClickIncreaseMemberCount()
    {
        if (memberCount < 9)
        {
            memberCount++;
            components[1].transform.GetComponent<TMP_Text>().text = memberCount.ToString();
        }

        //switch (memberCount)
        //{
        //    case 3:
        //        {
        //            autoStartMemberList[0].SetActive(true);
        //        }
        //        break;
        //    case 4:
        //        {
        //            for (int i = 0; i < 2; i++)
        //            {
        //                autoStartMemberList[i].SetActive(true);
        //            }
        //        }
        //        break;
        //    case 5:
        //        {
        //            for (int i = 0; i < 3; i++)
        //            {
        //                autoStartMemberList[i].SetActive(true);
        //            }
        //        }
        //        break;
        //}
    }

    public void OnClickDecreaseMemberCount()
    {
        if (memberCount > 2)
        {
            memberCount--;
            components[1].transform.GetComponent<TMP_Text>().text = memberCount.ToString();
        }

        //switch (memberCount)
        //{
        //    case 2:
        //        {
        //            for (int i = 2; i >= 0; i--)
        //            {
        //                autoStartMemberList[i].SetActive(false);
        //            }
        //        }
        //        break;

        //    case 3:
        //        {
        //            for (int i = 2; i >= 1; i--)
        //            {
        //                autoStartMemberList[i].SetActive(false);
        //            }
        //        }
        //        break;
        //    case 4:
        //        {
        //            autoStartMemberList[2].SetActive(false);
        //        }
        //        break;

        //}
    }


    public void OnClickOnSave()
    {
        //ActionTime Toggle Group
        //string actionTime = "";
        //Toggle[] toggles_ActionTime = components[2].transform.GetComponentsInChildren<Toggle>();
        //foreach (var t in toggles_ActionTime)
        //{
        //    if (t.isOn)
        //    {
        //        actionTime = t.name;
        //        //Debug.Log("Toggle Name:" + actionTime);
        //    }
        //}

        ////Hi Lo Toggle Group
        //string hi_Lo = "";
        //Toggle[] toggles_Hi_Lo = components[6].transform.GetComponentsInChildren<Toggle>();
        //foreach (var t in toggles_Hi_Lo)
        //{
        //    if (t.isOn)
        //    {
        //        hi_Lo = t.name;
        //        //Debug.Log("Toggle Name:" + hi_Lo);
        //    }
        //}

        //AutoStartWith Toggle Group
        //string autoStartWith = "";
        //Toggle[] toggles_AutoStartWith = components[13].transform.GetComponentsInChildren<Toggle>();
        //foreach (var t in toggles_AutoStartWith)
        //{
        //    if (t.isOn)
        //    {
        //        autoStartWith = t.name;
        //        //Debug.Log("Toggle Name:" + autoStartWith);
        //    }
        //}

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


        //string blindString = "";
        //string anteString = "";
        //string callTime = "";
        //string callTimeText = "";
        //string chipWithdrawal = "";

        //if (templateSubType.Equals("6 Plus"))
        //{
        //    anteString = components[5].transform.GetComponent<TMP_Text>().text.ToString();
        //    chipWithdrawal = components[23].transform.GetComponent<ToggleController>().isOn.ToString().Equals("True") ? "On" : "Off";

        //    blindString = "";
        //    callTime = "";
        //    callTimeText = "";
        //}
        //else
        //{
        //    anteString = "";
        //    chipWithdrawal = "";
        //    blindString = components[4].transform.GetComponent<TMP_Text>().text;
        //    callTime = components[21].transform.GetComponent<ToggleController>().isOn.ToString().Equals("True") ? "On" : "Off";
        //    callTimeText = components[22].transform.GetComponent<TMP_Text>().text;
        //}


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
        string requestData = "{\"userId\":\"" + PlayerManager.instance.GetPlayerGameData().userId + "\"," +
           "\"tableName\":\"" + "ak47" + "\"," +
            "\"playerCount\":\"" + 5 + "\"," +
            "\"gameMode\":\"" + "classic" + "\"," +
            "\"actionTime\":\"" + 5 + "\"," +
            //"\"bootValue\":\"" + 100 + "\"," +
            "\"minBuyIn\":\"" + 100 + "\"," +
            "\"fee\":\"" + 1 + "\"," +
            "\"cap\":\"" + 1 + "\"," +
            "\"hideRealTimeResult\":\"" + 1 + "\"," +
            "\"gpsRestriction\":\"" + 1 + "\"," +
            "\"ipRestriction\":\"" + 1 + "\"," +
            "\"tableTime\":\"" + 5 + "\"," +
            "\"entryCurrency\":\"" + 100 + "\"," +
            "\"players\":\"" + 5 + "\"," +
            "\"bootAmount\":\"" + 100 + "\"," +
            "\"smallBlind\":\"" + 100 + "\"," +
            "\"bigBlind\":\"" + 1200 + "\"," +
            "\"minBet\":\"" + 500 + "\"," +
            "\"maxBet\":\"" + 300 + "\"," +
            "\"potLimit\":\"" + 2800 + "\"," +
            "\"callTimmer\":\"" + 5 + "\"," +
            "\"startGameTimmer\":\"" + 5 + "\"," +
            "\"nextRoundTimmer\":\"" + 5 + "\"," +
            "\"gameOverTimmer\":\"" + 5 + "\"," +
            "\"gameIcon\":\"" + "test" + "\"," +
            "\"iconBaseUrl\":\"" + "test" + "\"," +
            "\"backgroundImg\":\"" + "test" + "\"," +
            "\"tableId\":\"" + "1" + "\"," +
            "\"clubId\":\"" + ClubDetailsUIManagerTeen.instance.GetClubId() + "\"}";

        WebServices.instance.SendRequestTP(RequestTypeTP.CreateTable, requestData, true, OnServerResponseFound);

        //components[28].transform.parent.Find("SaveBtn").GetComponent<Button>().interactable = false;
        //components[28].transform.parent.Find("CreateBtn").GetComponent<Button>().interactable = false;
    }

    public void OnClickOnCreate()
    {
        isPublishTemplateWithCreate = true;
        OnClickOnSave();
    }

    public void OnServerResponseFound(RequestTypeTP requestType, string serverResponse, bool isShowErrorMessage, string errorMessage)
    {
        //components[28].transform.parent.Find("SaveBtn").GetComponent<Button>().interactable = true;
        //components[28].transform.parent.Find("CreateBtn").GetComponent<Button>().interactable = true;

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
            case RequestTypeTP.CreateTable:
                {
                    Debug.Log("Response => TP_CreateTable: " + serverResponse);
                    JsonData data = JsonMapper.ToObject(serverResponse);

                    if (data["success"].ToString() == "1")
                    {
                        ClubTableControllerTeen.instance.ShowPopUp("Template saved successfully.");
                        //Debug.Log(data["message"].ToString());
                        //joinClubPopUp.SetActive(false);
                        //MainMenuController.instance.ShowMessage(data["message"].ToString());
                        //ClubTableControllerTeen.instance.RequestTemplateData(false);
                    }
                    else
                    {
                        Debug.Log(data["message"].ToString());
                        MainMenuControllerTeen.instance.ShowMessage(data["message"].ToString());
                    }

                    if (isPublishTemplateWithCreate)
                    {
                        //string requestData = "{\"clubId\":\"" + ClubDetailsUIManagerTeen.instance.GetClubId() + "\"," +
                        //        "\"status\":\"" + "Published" + "\"," +
                        //        "\"tableIds\":[\"" + data["tableId"].ToString() + "\"]}";

                        //WebServices.instance.SendRequest(RequestType.UpdateTemplateStatus, requestData, true, OnServerResponseFound);
                    }
                }
                break;

            //case RequestTypeTP.UpdateTemplateStatus:
            //    {
            //        JsonData data = JsonMapper.ToObject(serverResponse);

            //        if (data["success"].ToString() == "1")
            //        {
            //            if (data["message"].ToString().Equals("Template Published"))
            //            {
            //                Debug.Log("Tamplate Published Successfully");
            //                isPublishTemplateWithCreate = false;
            //                //StartCoroutine(ShowPopUp("Template Published ", 1.25f));
            //                ClubDetailsUIManager.instance.GetClubTemplates();
            //                ClubTableController.instance.RequestTemplateData(true);

            //                if (ClubTableController.instance.createTablePanel.activeSelf)
            //                    ClubTableController.instance.createTablePanel.SetActive(false);
            //            }
            //        }
            //        else
            //        {
            //            //MainMenuController.instance.ShowMessage(data["message"].ToString());
            //        }
            //    }
            //    break;

            default:
#if ERROR_LOG
                Debug.LogError("Unhandled requestType found in  MenuHandller = " + requestType);
#endif
                break;
        }
    }

    public void SetDataForEdit(JsonData data, int index)
    {
        for (int i = 0; i < data["response"].Count; i++)
        {
            if (i == index)
            {
                components[0].transform.GetComponent</*TMP_InputField*/InputField>().text = data["response"][i]["templateName"].ToString();
                components[1].transform.GetComponent<TMP_Text>().text = data["response"][i]["settingData"]["memberCount"].ToString();
                components[2].transform.Find(data["response"][i]["settingData"]["actionTime"].ToString()).GetComponent<Toggle>().isOn = true;
                components[3].transform.GetComponent<ToggleController>().isOn = (data["response"][i]/*["settingData"]*/["exclusiveTable"].ToString().Equals("On") ? true : false);
                components[4].transform.GetComponent<TMP_Text>().text = data["response"][i]["settingData"]["blinds"].ToString();

                //Debug.Log("Blinds Value:" + components[4].transform.GetComponent<TMP_Text>().text);

                for (int j = 0; j < transform.Find("BlindsData").GetComponent<SliderChange>().sliderValues.Length; j++)
                {
                    if (transform.Find("BlindsData").GetComponent<SliderChange>().sliderValues[j].Equals(data["response"][i]["settingData"]["blinds"].ToString()))
                    {
                        transform.Find("BlindsData/BlindsSlider").GetComponent<Slider>().value = j;
                    }
                }

                components[5].transform.GetComponent<TMP_Text>().text = data["response"][i]["settingData"]["ante"].ToString();
                components[6].transform.Find(data["response"][i]["settingData"]["high_low"].ToString()).GetComponent<Toggle>().isOn = true;
                components[7].transform.GetComponent<TMP_Text>().text = data["response"][i]["settingData"]["buyInMin"].ToString();
                components[8].transform.GetComponent<TMP_Text>().text = data["response"][i]["settingData"]["buyInMax"].ToString();

                //for (int j = 0; j < transform.Find("BlindsData").GetComponent<SliderChange>().sliderValues.Length; j++)
                //{
                //    if (transform.Find("BlindsData").GetComponent<SliderChange>().sliderValues[j].Equals(data["response"][i]["settingData"]["blinds"].ToString()))
                //    {
                //        transform.Find("BlindsData/BlindsSlider").GetComponent<Slider>().value = j;
                //    }
                //}

                components[9].transform.GetComponent<TMP_Text>().text = data["response"][i]["settingData"]["minVPIP"].ToString();

                for (int j = 0; j < transform.Find("BlindsData/MinVPIP").GetComponent<SliderManager>().values.Length; j++)
                {
                    //Debug.Log("Value of MinVPIP: " + transform.Find("BlindsData/MinVPIP").GetComponent<SliderManager>().values[j] + " AT INDEX: " + j);
                    if (transform.Find("BlindsData/MinVPIP").GetComponent<SliderManager>().values[j].ToString().Equals(data["response"][i]["settingData"]["minVPIP"].ToString()))
                    {
                        Debug.Log("Slider Value for MinVPIP " + j);
                        transform.Find("BlindsData/MinVPIP").GetComponent<Slider>().value = j;
                    }
                }

                components[10].transform.GetComponent<TMP_Text>().text = data["response"][i]["settingData"]["VPIPLevel"].ToString();

                for (int j = 0; j < transform.Find("BlindsData/VPIPLevel").GetComponent<SliderManager>().values.Length; j++)
                {
                    if (transform.Find("BlindsData/VPIPLevel").GetComponent<SliderManager>().values[j].ToString().Equals(data["response"][i]["settingData"]["VPIPLevel"].ToString()))
                    {
                        Debug.Log("Slider Value for VPIPLevel " + j);
                        transform.Find("BlindsData/VPIPLevel").GetComponent<Slider>().value = j;
                    }
                }

                components[11].transform.GetComponent<TMP_Text>().text = data["response"][i]["settingData"]["handsThreshold"].ToString();

                for (int j = 0; j < transform.Find("BlindsData/HandsThreshold/HandsThresholdSlider").GetComponent<SliderManager>().values.Length; j++)
                {
                    if (transform.Find("BlindsData/HandsThreshold/HandsThresholdSlider").GetComponent<SliderManager>().values[j].ToString().Equals(data["response"][i]["settingData"]["handsThreshold"].ToString()))
                    {
                        Debug.Log("Slider Value for HandsThreshold " + j);
                        transform.Find("BlindsData/HandsThreshold/HandsThresholdSlider").GetComponent<Slider>().value = j;
                    }
                }

                components[12].transform.GetComponent<ToggleController>().isOn = (data["response"][i]["settingData"]["autoStart"].ToString().Equals("On") ? true : false);
                components[13].transform.Find("Members/" + data["response"][i]["settingData"]["autoStartWith"].ToString()).GetComponent<Toggle>().isOn = true;
                components[14].transform.GetComponent<ToggleController>().isOn = (data["response"][i]["settingData"]["autoExtension"].ToString().Equals("On") ? true : false);
                components[15].transform.GetComponent<TMP_Text>().text = data["response"][i]["settingData"]["autoExtensionTimes"].ToString();
                components[16].transform.GetComponent<ToggleController>().isOn = (data["response"][i]["settingData"]["autoOpen"].ToString().Equals("On") ? true : false);
                components[17].transform.GetComponent<Toggle>().isOn = (data["response"][i]["settingData"]["runItMulti"].ToString().Equals("Yes") ? true : false);
                components[18].transform.GetComponent<Toggle>().isOn = (data["response"][i]["settingData"]["evChop"].ToString().Equals("Yes") ? true : false);
                components[19].transform.GetComponent<TMP_Text>().text = data["response"][i]["settingData"]["fee"].ToString();
                components[20].transform.GetComponent<TMP_Text>().text = data["response"][i]["settingData"]["cap"].ToString();
                components[21].transform.GetComponent<ToggleController>().isOn = (data["response"][i]["settingData"]["calltime"].ToString().Equals("On") ? true : false);
                components[22].transform.GetComponent<TMP_Text>().text = data["response"][i]["settingData"]["time"].ToString();
                components[23].transform.GetComponent<ToggleController>().isOn = (data["response"][i]["settingData"]["chipWithdrawal"].ToString().Equals("On") ? true : false);
                components[24].transform.GetComponent<ToggleController>().isOn = (data["response"][i]["settingData"]["authorizedBuyIn"].ToString().Equals("On") ? true : false);
                components[25].transform.GetComponent<ToggleController>().isOn = (data["response"][i]["settingData"]["GPSRestriction"].ToString().Equals("On") ? true : false);
                components[26].transform.GetComponent<ToggleController>().isOn = (data["response"][i]["settingData"]["IPRestriction"].ToString().Equals("On") ? true : false);
                components[27].transform.GetComponent<ToggleController>().isOn = (data["response"][i]["settingData"]["banChatting"].ToString().Equals("On") ? true : false);
                components[28].transform.GetComponent<TMP_Text>().text = data["response"][i]["settingData"]["hours"].ToString();

            }
        }
    }
}
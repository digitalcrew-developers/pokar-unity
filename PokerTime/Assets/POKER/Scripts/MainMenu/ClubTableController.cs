using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;

public class ClubTableController : MonoBehaviour
{
    public static ClubTableController instance;

    [Header("NLH")]
    public Button RingGameTabButton_NLH;
    public Button SNGGameTabButton_NLH;
    public Button MTTGameTabButton_NLH;
    public GameObject RingGamePanel_NLH, SNGamePanel_NLH, MTTGamePanel_NLH;

    public Toggle EVChop;
    public GameObject EVChopValueField;

    

    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
        }
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


    public void CreateTemplate()
    {

        string requestData = "{\"clubId\":\"" + "17" + "\"," +
                            "\"gameType\":\"" + "NLH" + "\"," +
                            "\"templateType\":\"" + "Ring Game" + "\"," +
                            "\"status\":\"" + "Saved" + "\"," +
                            "\"tableId\":\"" + "Me" + "\"," +
                            "\"settingData\":" + "[{" +
                            "\"templateSubType\":\"" + "Regular Mode" + "\"," +
                            "\"memberCount\":\"" + "6" + "\"," +
                            "\"actionTime\":\"" + "15" + "\"," +
                            "\"exclusiveTable\":\"" + "Off" + "\"," +
                            "\"blinds\":\"" + "1/2" + "\"," +
                            "\"buyInMin\":\"" + "10" + "\"," +
                            "\"buyInMax\":\"" + "100" + "\"," +
                            "\"minVPIP\":\"" + "0%" + "\"," +
                            "\"VPIPLevel\":\"" + "0%" + "\"," +
                            "\"handsThreshold\":\"" + "" + "\"," +
                            "\"autoStart\":\"" + "Yes" + "\"," +
                            "\"autoStartWith\":\"" + "1" + "\"," +
                            "\"autoExtension\":\"" + "5" + "\"," +
                            "\"autoExtensionTimes\":\"" + "10" + "\"," +
                            "\"autoOpen\":\"" + "Yes" + "\"," +
                            "\"riskManagement\":\"" + "No" + "\"," +
                            "\"fee\":\"" + "5" + "\"," +
                            "\"cap\":\"" + "10" + "\"," +

                            "\"calltime\":\"" + "2" + "\"," +
                            "\"authorizedBuyIn\":\"" + "On" + "\"," +
                            "\"GPSRestriction\":\"" + "On" + "\"," +
                            "\"IPRestriction\":\"" + "On" + "\"," +
                            "\"banChatting\":\"" + "On" + "\"," +
                            "\"hours\":\"" + "5"  +

                           "\"}]}";

        WebServices.instance.SendRequest(RequestType.CreateTemplate, requestData, true, OnServerResponseFound);
    }





    public void OnServerResponseFound(RequestType requestType, string serverResponse, bool isShowErrorMessage, string errorMessage)
    {
        if (errorMessage.Length > 0)
        {
            if (isShowErrorMessage)
            {
                //  Debug.LogError("111111111111111111111111111111");
                MainMenuController.instance.ShowMessage(errorMessage);
            }
            return;
        }
        if (requestType == RequestType.CreateTemplate)
        {
            JsonData data = JsonMapper.ToObject(serverResponse);

            Debug.LogError("Data is : " + data.ToJson());


            MainMenuController.instance.ShowMessage(data["message"].ToJson());

            //if (data["success"].ToString() == "1")
            //{
            //    for (int i = 0; i < data["getData"].Count; i++)
            //    {

            //        loadImages(data["getData"][i]["profileImage"].ToString(), data["getData"][i]["frameURL"].ToString(), data["getData"][i]["countryFlag"].ToString());
            //        userLevel.text = "Lvl. " + data["getData"][i]["userLevel"].ToString() + ">>";
            //        userName.text = data["getData"][i]["userName"].ToString();
            //        userId.text = "UserID:" + data["getData"][i]["userId"].ToString();
            //        countrycode = data["getData"][i]["countryCode"].ToString();
            //        countryname = data["getData"][i]["countryName"].ToString();
            //        avtarurl = data["getData"][i]["profileImage"].ToString();
            //        frameurl = data["getData"][i]["frameURL"].ToString();
            //        flagurl = data["getData"][i]["countryFlag"].ToString();
            //        avtarid = int.Parse(data["getData"][i]["avatarID"].ToString());
            //    }
            //    MainMenuController.instance.OnClickOnButton("profile");
            //}
            //else
            //{
            //    MainMenuController.instance.ShowMessage(data["message"].ToString());
            //}
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
}

using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionContentUIManager : MonoBehaviour
{
    public Text missionTypeTxt;
    public Text missionDiscriptionTxt;
    public Text missionValueTxt;
    public Button collectBtn;
    public GameObject collectedBtnImg;

    //DEV_CODE
    public Sprite collectBtnNewImage;


    public void OnCollectBtnClick(Transform btn) {

        if (btn.GetChild(0).GetComponent<Text>().text == "Active")
        {
            switch (btn.parent.Find("SubTitle").GetComponent<Text>().text)
            {
                case "Consecutive Login":
                    Debug.Log("I am Destroy       ");

                    FetchConsecutiveLogin(true);
                    // Destroy(this.gameObject);
                    MainMenuController.instance.ShowScreen(MainMenuScreens.ConsecutiveLoginReward);
                    MainMenuController.instance.DestroyScreen(MainMenuScreens.Missions);
                    break;
            }
           
        }
    
    }

    public void FetchConsecutiveLogin(bool isShowLoading)
    {
        string requestData = "{\"userId\":\"" + PlayerManager.instance.GetPlayerGameData().userId + "\"," +
                               "\"coins\":\"" + 500 + "\"}";
        if (isShowLoading)
        {
            MainMenuController.instance.ShowScreen(MainMenuScreens.Loading);
        }
        WebServices.instance.SendRequest(RequestType.redeemDailyMission, requestData, true, OnServerResponseFound);
    }

    public void OnServerResponseFound(RequestType requestType, string serverResponse, bool isShowErrorMessage, string errorMessage)
    {
        MainMenuController.instance.DestroyScreen(MainMenuScreens.Loading);

        if (errorMessage.Length > 0)
        {
            if (isShowErrorMessage)
            {
                MainMenuController.instance.ShowMessage(errorMessage);
            }

            return;
        }
        if (requestType == RequestType.redeemDailyMission)
        {
            JsonData data = JsonMapper.ToObject(serverResponse);

            if (data["success"].ToString() == "1")
            {
                //ShowMissionList(data);
            }
            else
            {
                //MainMenuController.instance.ShowMessage(data["message"].ToString());
            }
        }
    }
}

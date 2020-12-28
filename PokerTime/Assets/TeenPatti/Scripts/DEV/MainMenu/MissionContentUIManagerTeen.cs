using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionContentUIManagerTeen : MonoBehaviour
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
                    MainMenuControllerTeen.instance.ShowScreen(MainMenuScreensTeen.ConsecutiveLoginReward);
                    MainMenuControllerTeen.instance.DestroyScreen(MainMenuScreensTeen.Missions);
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
            MainMenuControllerTeen.instance.ShowScreen(MainMenuScreensTeen.Loading);
        }
        WebServices.instance.SendRequest(RequestType.redeemDailyMission, requestData, true, OnServerResponseFound);
    }

    public void OnServerResponseFound(RequestType requestType, string serverResponse, bool isShowErrorMessage, string errorMessage)
    {
        MainMenuControllerTeen.instance.DestroyScreen(MainMenuScreensTeen.Loading);

        if (errorMessage.Length > 0)
        {
            if (isShowErrorMessage)
            {
                MainMenuControllerTeen.instance.ShowMessage(errorMessage);
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
                //MainMenuControllerTeen.instance.ShowMessage(data["message"].ToString());
            }
        }
    }
}

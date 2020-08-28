using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionsUiManager : MonoBehaviour
{

    public GameObject missonContentPrefab;
    public Transform Container;

    public void Start()
    {
        FetchData(true);
    }


    public void FetchData(bool isShowLoading)
    {
        string requestData = "{\"userId\":\"" + PlayerManager.instance.GetPlayerGameData().userId + "\"}";

        if (isShowLoading)
        {
            MainMenuController.instance.ShowScreen(MainMenuScreens.Loading);
        }
        WebServices.instance.SendRequest(RequestType.GetMissions, requestData, true, OnServerResponseFound);
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
        if (requestType == RequestType.GetMissions)
        {
            JsonData data = JsonMapper.ToObject(serverResponse);

            if (data["success"].ToString() == "1")
            {
                ShowMissionList(data);
            }
            else
            {
                //MainMenuController.instance.ShowMessage(data["message"].ToString());
            }
        }
    }



    public void ShowMissionList(JsonData data)
    {

        for (int i = 0; i < Container.childCount; i++)
        {
            Destroy(Container.GetChild(i).gameObject);
        }

        //      "missionID": 12,
        //"missionType": "General",
        //"missionIcon": null,
        //"missionDesc": "Recieve 10 likes for a comment in Forum",
        //"missionValue": 200000,
        //"missionInterval": 1,
        //"Status": "Active"


        for (int i = 0; i < data["getData"].Count; i++)
        {

            GameObject gm1 = Instantiate(missonContentPrefab, Container) as GameObject;
            gm1.GetComponent<MissionContentUIManager>().missionTypeTxt.text = data["getData"][i]["missionType"].ToString();
            gm1.GetComponent<MissionContentUIManager>().missionDiscriptionTxt.text = data["getData"][i]["missionDesc"].ToString();
            gm1.GetComponent<MissionContentUIManager>().missionValueTxt.text = "X " + data["getData"][i]["missionValue"].ToString();

            gm1.GetComponent<MissionContentUIManager>().collectBtn.transform.GetChild(0).GetComponent<Text>().text = data["getData"][i]["Status"].ToString();
            // gm1.GetComponent<MissionContentUIManager>().collectBtn.GetComponent<Image>().color.a = 1f;
            Image image = gm1.GetComponent<MissionContentUIManager>().collectBtn.GetComponent<Image>();

            Color c = image.color;
            if (data["getData"][i]["Status"].ToString().Equals("Active"))
            {
                c.a = 0;
            }
            else
            {
                c.a = 1;
            }
            image.color = c;

        }
    }



    public void OnClickOnButton(string eventName)
    {
        SoundManager.instance.PlaySound(SoundType.Click);

        switch (eventName)
        {
            case "back":
                {
                    if (MainMenuController.instance != null)
                    {
                        MainMenuController.instance.DestroyScreen(MainMenuScreens.Missions);
                        MainMenuController.instance.ShowScreen(MainMenuScreens.Lobby);
                    }
                    if (InGameUiManager.instance != null)
                    {
                        InGameUiManager.instance.DestroyScreen(InGameScreens.Missions);
                    }


                }
                break;

            default:
                {
                    Debug.LogError("Unhandled eventName found in MissionsUiManager = " + eventName);
                }
                break;
        }
    }
}

using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClubDataManager : MonoBehaviour
{
    public static ClubDataManager instance;

    public Transform container;

    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        GetClubDataAnalytics();
        GetClubTableDataAnalytics();
    }

    void GetClubDataAnalytics()
    {
        string requestData = "{\"clubId\":" + 59/*ClubDetailsUIManager.instance.GetClubId()*/ + "," +
                             "\"startDate\":\"" + "2021-01-13" + "\"," +
                             "\"endDate\":\"" + "2021-01-13" + "\"}";
        WebServices.instance.SendRequest(RequestType.GetClubDataAnalytics, requestData, true, OnServerResponseFound);
    }

    void GetClubTableDataAnalytics()
    {
        string requestData = "{\"tableId\":" + 130 + "," +
                             "\"startDate\":\"" + "2021-01-13" + "\"," +
                             "\"endDate\":\"" + "2021-01-13" + "\"}";
        WebServices.instance.SendRequest(RequestType.GetClubTableDataAnalytics, requestData, true, OnServerResponseFound);
    }

    public void OnServerResponseFound(RequestType requestType, string serverResponse, bool isShowErrorMessage, string errorMessage)
    {
        //Debug.Log("server response club admin : " + serverResponse);
        MainMenuController.instance.DestroyScreen(MainMenuScreens.Loading);

        if (errorMessage.Length > 0)
        {
            if (isShowErrorMessage)
            {
                MainMenuController.instance.ShowMessage(errorMessage);
            }

            return;
        }

        switch (requestType)
        {
            case RequestType.GetClubDataAnalytics:
                {
                    Debug.Log("Response => GetClubDataAnalytics: " + serverResponse.ToString());
                    JsonData data = JsonMapper.ToObject(serverResponse);
                    //if (data["success"].ToString().Equals("1"))
                    //{
                    //    Debug.Log("Total Data: " + data["response"].Count);
                        //    if (DisbandClub.activeInHierarchy)//api response is for disband club
                        //    {
                        //        MainMenuController.instance.ShowMessage("Club has been disbanded", () =>
                        //        {
                        //            ClubDetailsUIManager.instance.OnClickOnButton("back");
                        //            for (int i = 0; i < ClubListUiManager.instance.container.childCount; i++)
                        //            {
                        //                Destroy(ClubListUiManager.instance.container.GetChild(i).gameObject);
                        //            }
                        //            ClubListUiManager.instance.FetchList(true);
                        //        });
                        //    }
                        //    if (PreferencesPanel.activeInHierarchy)//api response is for preferences
                        //    {

                        //    }
                        //    //if (JackpotPanel.activeInHierarchy)//api response is for jackpot
                        //    //{

                        //    //}

                        //}
                        //else
                        //{
                        //    MainMenuController.instance.ShowMessage(data["message"].ToString());
                        //}
                    //}
                }
                break;

            case RequestType.GetClubTableDataAnalytics:
                Debug.Log("Response => GetClubTableDataAnalytics: " + serverResponse.ToString());
                break;

            default:
#if ERROR_LOG
                Debug.LogError("Unhandled requestType found in  MenuHandller = " + requestType);
#endif
                break;
        }
    }

    public void OnClickOnData()
    {
        ClubAdminManager.instance.ShowScreen(ClubScreens.GameData);
    }
    public void OnClickExpertData()
    {
        ClubAdminManager.instance.ShowScreen(ClubScreens.ExpertData);
    }
}
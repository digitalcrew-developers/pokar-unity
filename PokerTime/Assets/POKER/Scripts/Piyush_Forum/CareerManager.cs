using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CareerManager : MonoBehaviour
{
    public static CareerManager instance;
    public int selectedIndex_CareerMenuScreen = 0;

    //DEV_CODE
    public GameObject requestObj;

    public Text headingTxt;
    public GameObject[] DMY_objList;
    public GameObject[] DMY_objfocus;

    private void Awake()
    {
        instance = this;
        requestObj.SetActive(false);
        GetRequestList();
    }
    public void OnMenuBtnClick()
    {
        MainMenuController.instance.ShowScreen(MainMenuScreens.CareerMenuScreen);
    }

    public void OnDataBtnClick()
    {
        MainMenuController.instance.ShowScreen(MainMenuScreens.CareerDataScreen);
    }

    public void OnClickVIP()
    {
        MainMenuController.instance.ShowScreen(MainMenuScreens.VIP_Privilege);
    }

    public void OnDMY_BtnClick(string val) {
        for (int i = 0; i < DMY_objList.Length; i++)
        {
            DMY_objList[i].SetActive(false);
            DMY_objfocus[i].SetActive(false);
        }
        switch (val)
        {
            case "day":
                DMY_objList[0].SetActive(true);
                DMY_objfocus[0].SetActive(true);

                break;
            case "month":
                DMY_objList[1].SetActive(true);
                DMY_objfocus[1].SetActive(true);
                break;
            case "year":
                DMY_objList[2].SetActive(true);
                DMY_objfocus[2].SetActive(true);
                break;
        }
    }

    public void OnClickOnRequestStatus(string status)
    {
        Debug.Log("My Player ID For Requests (Accept/Deny):" + PlayerManager.instance.GetPlayerGameData().userId);

        string requestData = "{\"id\":\"" + 1 +
                               "\"userId\":\"" + PlayerManager.instance.GetPlayerGameData().userId + "\"," +
                               "\"status\":\"" + status + "\"}";

        WebServices.instance.SendRequest(RequestType.UpdateMultiAccountRequestStatus, requestData, true, OnServerResponseFound);
    }

    private void GetRequestList()
    {
        Debug.Log("My Player ID for Requests:" + PlayerManager.instance.GetPlayerGameData().userId);
        string requestData = "{\"userId\":\"" + PlayerManager.instance.GetPlayerGameData().userId + "\"}";

        WebServices.instance.SendRequest(RequestType.GetMultiAccountPendingRequests, requestData, true, OnServerResponseFound);
    }

    public void OnServerResponseFound(RequestType requestType, string serverResponse, bool isShowErrorMessage, string errorMessage)
    {

        if (errorMessage.Length > 0)
        {
            if (isShowErrorMessage)
            {
                MainMenuController.instance.ShowMessage(errorMessage);
            }

            return;
        }

        if (requestType == RequestType.UpdateMultiAccountRequestStatus)
        {
            JsonData data = JsonMapper.ToObject(serverResponse);
            Debug.Log("Update Request Status: " + data.ToString());

            if (data["status"].Equals(true))
            {
                MainMenuController.instance.ShowMessage(data["response"].ToString());
                requestObj.SetActive(false);
            }
            else
            {
                MainMenuController.instance.ShowMessage("Unable to update request..");
            }
        }
        else if (requestType == RequestType.GetMultiAccountPendingRequests)
        {
            JsonData data = JsonMapper.ToObject(serverResponse);
            Debug.Log("Get Request List: " + data.ToString());

            if (data["status"].Equals(true))
            {
                MainMenuController.instance.ShowMessage(data["response"].ToString());
                for (int i = 0; i < data["getData"].Count; i++)
                {
                    requestObj.SetActive(true);
                    requestObj.transform.GetChild(0).GetComponent<Text>().text = "Player " + data["getData"][i]["byUserId"] + " is requesting your career data.";
                }
            }
            else
            {
                MainMenuController.instance.ShowMessage("Unable to send request");
            }            
        }
        else
        {

#if ERROR_LOG
            Debug.LogError("Unhadnled response found in  = " + requestType);
#endif
        }

    }
}
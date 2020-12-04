using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

public class CareerManager : MonoBehaviour
{
    public static CareerManager instance;
    public int selectedIndex_CareerMenuScreen = 0;

    //DEV_CODE
    public GameObject requestObj, dataObj;
    public List<RequestData> requestDataList = new List<RequestData>();
    public static int requestCounter = -1;

    public int currentDate, currentMonth, currentYear;

    public Text headingTxt;
    public GameObject[] DMY_objList;
    public GameObject[] DMY_objfocus;

    private void Awake()
    {
        instance = this;
        requestObj.SetActive(false);
        if (PlayerManager.instance.IsLogedIn())
        {
            GetRequestList();
        }

        currentDate = DateTime.Now.Day;
        currentMonth = DateTime.Now.Month;
        currentYear = DateTime.Now.Year;
    }

    //private void FindTotalDays()
    //{
        //int startYear = 2020;
        //int currentYear = DateTime.Now.Year;
        //int startMonth = 07;
        //int currentMonth = DateTime.Now.Month;
        //int startDay = 4;
        //int currentDay = DateTime.Now.Day;

        //int totalDays = 0;

        //DateTime today = DateTime.Now;
        //DateTime startDate = new DateTime(startYear,startMonth,startDay);    
        //TimeSpan elapsed = today.Subtract(startDate);
        //Debug.Log("Total Days: " +/* DateTime.DaysInMonth(2020,10)*/(int)elapsed.TotalDays);
    //}

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

    public void OnDMY_BtnClick(string val)
    {
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

    public void OnClickOnRequestStatus(string id, string status)
    {
        Debug.Log("My Player ID For Requests (Accept/Deny):" + PlayerManager.instance.GetPlayerGameData().userId);

        string requestData = "{\"id\":\"" + id + "\"," +
                               "\"userId\":\"" + PlayerManager.instance.GetPlayerGameData().userId + "\"," +
                               "\"status\":\"" + status + "\"}";

        WebServices.instance.SendRequest(RequestType.UpdateMultiAccountRequestStatus, requestData, true, OnServerResponseFound);
    }

    public void GetRequestList()
    {
        Debug.Log("My Player ID for Requests:" + PlayerManager.instance.GetPlayerGameData().userId);
        string requestData = "{\"userId\":\"" + PlayerManager.instance.GetPlayerGameData().userId + "\"}";

        WebServices.instance.SendRequest(RequestType.GetMultiAccountPendingRequests, requestData, true, OnServerResponseFound);
    }

    private void ShowRequestList()
    {
        /*for (int i = 0; i < data.Count; i++)
        {*/
        requestObj.transform.GetChild(0).GetComponent<Text>().text = "Player " + requestDataList[requestCounter].byUserId + " is requesting your career data.";
        requestObj.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => OnClickOnRequestStatus(requestDataList[requestCounter].requestId, "Approved"));
        requestObj.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(() => OnClickOnRequestStatus(requestDataList[requestCounter].requestId, "Rejected"));
        requestObj.SetActive(true);
        
        requestDataList.RemoveAt(requestCounter);
        requestCounter--;
        /*}*/
    }

    public void OnServerResponseFound(RequestType requestType, string serverResponse, bool isShowErrorMessage, string errorMessage)
    {
        Debug.Log(errorMessage);
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
                //requestObj.SetActive(false);

                //Debug.Log("Req Counter:" + requestCounter);
                if (requestDataList.Count > 0)
                {
                    dataObj.transform.localPosition = new Vector3(0, 0, 0);
                    ShowRequestList();
                }
                else
                {
                    requestObj.SetActive(false);
                    dataObj.transform.localPosition = new Vector3(0, 80, 0);
                }
            }
            else
            {
                MainMenuController.instance.ShowMessage("Unable to update request..");
            }
        }
        
        if (requestType == RequestType.GetMultiAccountPendingRequests)
        {
            JsonData data = JsonMapper.ToObject(serverResponse);
            Debug.Log("Get Request List: " + data.ToString());

            if (data["status"].Equals(true))
            {
                //MainMenuController.instance.ShowMessage(data["response"].ToString());
                for (int i = 0; i < data["getData"].Count; i++)
                {
                    Debug.Log("Request No:" + i);
                    RequestData reqData = new RequestData();
                    reqData.requestId = data["getData"][i]["requestId"].ToString();
                    reqData.byUserId = data["getData"][i]["byUserId"].ToString();
                    reqData.toUserId = data["getData"][i]["toUserId"].ToString();

                    requestDataList.Add(reqData);
                    requestCounter++;

                    /*Debug.Log("Request ID:" + reqData.requestId);
                    Debug.Log("By ID:" + reqData.byUserId);
                    Debug.Log("To ID:" + reqData.toUserId);*/

                    /*requestObj.SetActive(true);
                    requestObj.transform.GetChild(0).GetComponent<Text>().text = "Player " + data["getData"][i]["byUserId"] + " is requesting your career data.";*/
                }

                if(requestDataList.Count > 0)
                    ShowRequestList();
            }
            else
            {
                Debug.Log("Unable to send request GetMultiAccountPendingRequests");
                //MainMenuController.instance.ShowMessage("Unable to send request GetMultiAccountPendingRequests");
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

public class RequestData
{
    public string requestId, byUserId, toUserId;
}
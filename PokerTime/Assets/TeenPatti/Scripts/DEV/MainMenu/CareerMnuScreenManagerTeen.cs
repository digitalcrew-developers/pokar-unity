using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CareerMnuScreenManagerTeen : MonoBehaviour
{
    public List<GameObject> btnList;
    public GameObject multiAccountList, multiAccountBtn;

    public InputField requestedUserID;
    
    public void OnEnable()
    {
        Debug.Log("selectedIndex---------------   " + CareerManagerTeen.instance.selectedIndex_CareerMenuScreen);
        for (int i = 0; i < btnList.Count; i++)
        {
            //btnList[i].GetComponent<Image>().color = new Color32(42, 42, 42, 255);
            if (CareerManagerTeen.instance.selectedIndex_CareerMenuScreen == i)
            {
                /*btnList[i].GetComponent<Image>().color = new Color32(80, 180, 80, 255);*/
                /*btnList[i].GetComponent<Button>().pre*/
                CareerManagerTeen.instance.headingTxt.text = btnList[i].transform.GetChild(0).GetComponent<Text>().text;
            }
        }

        LoadPlayerAccounts();
    }

    private void LoadPlayerAccounts()
    {
        Debug.Log("Player ID for Load Account:" + PlayerManager.instance.GetPlayerGameData().userId);
        string requestData = "{\"userId\":\"" + PlayerManager.instance.GetPlayerGameData().userId + "\"}";

        WebServices.instance.SendRequest(RequestType.GetMyConnectedAccounts, requestData, true, OnServerResponseFound);
    }

    public void OnClickAddAccount()
    {
        Debug.Log("My Player ID to add account:" + PlayerManager.instance.GetPlayerGameData().userId);
        Debug.Log("My New ID to add account:" + requestedUserID.text);

        string requestData = "{\"userId\":\"" + PlayerManager.instance.GetPlayerGameData().userId + "\"," +
                               "\"ToUserId\":\"" + requestedUserID.text + "\"}";

        WebServices.instance.SendRequest(RequestType.AddMultiAccountConnectRequest, requestData, true, OnServerResponseFound);
    }

    public void OnServerResponseFound(RequestType requestType, string serverResponse, bool isShowErrorMessage, string errorMessage)
    {

        if (errorMessage.Length > 0)
        {
            if (isShowErrorMessage)
            {
                MainMenuControllerTeen.instance.ShowMessage(errorMessage);
            }

            return;
        }

        if (requestType == RequestType.AddMultiAccountConnectRequest)
        {
            Debug.Log("Response => AddMultiAccountConnectRequest : " + serverResponse);
            JsonData data = JsonMapper.ToObject(serverResponse);

            if (data["status"].Equals(true))
            {
                MainMenuControllerTeen.instance.ShowMessage(data["response"].ToString());
                requestedUserID.text = "";
                CareerManager.instance.GetRequestList();
            }
            else
            {
                MainMenuControllerTeen.instance.ShowMessage("Unable to send request");
            }
        }
        else if (requestType == RequestType.GetMyConnectedAccounts)
        {
            Debug.Log("Response => GetMyConnectedAccounts : " + serverResponse);
            JsonData data = JsonMapper.ToObject(serverResponse);

            if (data["status"].Equals(true))
            {
                for (int i = 0; i < data["getData"].Count; i++)
                {
                    GameObject gm = Instantiate(multiAccountBtn, multiAccountList.transform) as GameObject;
                    gm.transform.GetChild(0).GetComponent<Text>().text = "PLAYER ID: 1234567";
                }
            }
            else
            {
                MainMenuControllerTeen.instance.ShowMessage("Unable to get accounts");
            }
        }
        else
        {

#if ERROR_LOG
            Debug.LogError("Unhadnled response found in  = " + requestType);
#endif
        }

    }

    public void OnCloseBtnClick() {

        MainMenuControllerTeen.instance.DestroyScreen(MainMenuScreensTeen.CareerMenuScreen);
    }

    public void OnBtnClick(int val) {
        CareerManagerTeen.instance.selectedIndex_CareerMenuScreen = val;
        for (int i = 0; i < btnList.Count; i++)
        {
            btnList[i].GetComponent<Image>().color = new Color32(42, 42, 42, 255);
            if (CareerManagerTeen.instance.selectedIndex_CareerMenuScreen == i)
            {
                btnList[i].GetComponent<Image>().color = new Color32(80, 180, 80, 255);
                CareerManagerTeen.instance.headingTxt.text = btnList[i].transform.GetChild(0).GetComponent<Text>().text;
            }
        }
        Debug.Log("selectedIndex-----0000----------   " + CareerManagerTeen.instance.selectedIndex_CareerMenuScreen);

    }
}

﻿using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnlinkYourEmailManagerTeen : MonoBehaviour
{
    [Header("Unlink Change Email Pannel")]
    public GameObject unlinkChangePannel;
    public Text emailUnlinkTxt;

    [Header("mail Pwd Pannel")]
    public GameObject mailPwdPannel;
    public Text emailMailTxt;
    public InputField pwdInput;


    private void Start()
    {
        emailUnlinkTxt.text = emailMailTxt.text = PlayerPrefs.GetString("USER_EMAIL");
        unlinkChangePannel.SetActive(true);
        mailPwdPannel.SetActive(false);
    }

    public void OnClickUnlinkBtn() {
        SoundManager.instance.PlaySound(SoundType.Click);

        unlinkChangePannel.SetActive(false);
        mailPwdPannel.SetActive(true);
    }
    public void OnClickCloseBtn()
    {
        SoundManager.instance.PlaySound(SoundType.Click);

        gameObject.SetActive(false);
        //MainMenuController.instance.DestroyScreen(MainMenuScreens.UnlinkYourEmail);
    }
    public void OnClickNextBtn()
    {
        SoundManager.instance.PlaySound(SoundType.Click);

        FetchUserData();
    }

    private void FetchUserData()
    {
        string requestData = "{\"userId\":\"" + PlayerManager.instance.GetPlayerGameData().userId + "\"," +
             "\"email\":\"" + PlayerPrefs.GetString("USER_EMAIL") + "\"," +
              "\"password\":\"" + pwdInput.text + "\"}";
        //string requestData = "{\"userId\":\"" + PlayerManager.instance.GetPlayerGameData().userId + "\"," +
        //      "\"email\":\"" + PlayerPrefs.GetString("USER_EMAIL") + "\"}";

        //MainMenuController.instance.ShowScreen(MainMenuScreens.Loading);
        WebServices.instance.SendRequest(RequestType.unlinkEmail, requestData, true, OnServerResponseFound);

    }

    public void OnServerResponseFound(RequestType requestType, string serverResponse, bool isShowErrorMessage, string errorMessage)
    {
        MainMenuControllerTeen.instance.DestroyScreen(MainMenuScreensTeen.Loading);

        if (errorMessage.Length > 0)
        {
            if (isShowErrorMessage)
            {
                if (requestType == RequestType.unlinkEmail)
                {
                    MainMenuControllerTeen.instance.ShowMessage(errorMessage, () =>
                    {
                        FetchUserData();
                    });
                }
                else
                {
                    MainMenuControllerTeen.instance.ShowMessage(errorMessage);
                }
            }

            return;
        }
       
        if (requestType == RequestType.unlinkEmail)
        {
            JsonData data = JsonMapper.ToObject(serverResponse);

            if (data["success"].ToString() == "1")
            {
                PlayerPrefs.DeleteKey("USER_EMAIL");
                //MainMenuController.instance.DestroyScreen(MainMenuScreens.UnlinkYourEmail);
                MainMenuControllerTeen.instance.ShowScreen(MainMenuScreensTeen.LinkYourEmail);
                
            }
            else
            {
                MainMenuControllerTeen.instance.ShowMessage(data["response"].ToString());

            }
        }

    }
}

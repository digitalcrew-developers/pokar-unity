﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageScript : MonoBehaviour
{
    public GameObject yesButton, noButton, okButton;

    public Text okButtonText, yesButtonText, noButtonText,messageText;
    private Action defaultCallBackMethod;
    private Action okButtonCallBackMethod, noButtonCallBackMethod, yesButtonCallBackMethod;

    private void Start()
    {
        okButton.GetComponent<Button>().onClick.AddListener(ClosePopup);
        noButton.GetComponent<Button>().onClick.AddListener(ClosePopup);
        yesButton.GetComponent<Button>().onClick.AddListener(ClosePopup);

        okButton.GetComponent<Button>().onClick.AddListener(OnClickOnOk);
        noButton.GetComponent<Button>().onClick.AddListener(OnClickOnNo);
        yesButton.GetComponent<Button>().onClick.AddListener(OnClickOnYes);
    }

    private void ClosePopup()
    {
        gameObject.SetActive(false);
    }

    public void ShowSingleButtonPopUp(string messageToShow,Action callBack = null,string buttonText = "Ok")
    {
        Debug.Log("Show meesage :" + messageToShow);

        if (messageToShow.Equals("No record found"))
        {
            messageText.text = "";
            MainMenuController.instance.DestroyScreen(MainMenuScreens.Message);
        }
        else {
            messageText.text = messageToShow;
        }
       
        okButtonText.text = buttonText;
        okButtonCallBackMethod = callBack;

        defaultCallBackMethod = OnClickOnOk;


        yesButton.SetActive(false);
        noButton.SetActive(false);
        okButton.SetActive(true);
    }


    public void ShowDoubleButtonPopUp(string messageToShow,Action yesButtonCallBack = null,Action noButtonCallBack = null,string yesText = "Yes",string noText = "No")
    {
        messageText.text = messageToShow;
        yesButtonText.text = yesText;
        noButtonText.text = noText;

        yesButtonCallBackMethod = yesButtonCallBack;
        noButtonCallBackMethod = noButtonCallBack;
        defaultCallBackMethod = OnClickOnNo;



        yesButton.SetActive(true);
        noButton.SetActive(true);

        okButton.SetActive(false);
    }



    public void OnClickOnOk()
    {
        SoundManager.instance.PlaySound(SoundType.Click);
        okButtonCallBackMethod?.Invoke();


        if (MainMenuController.instance != null)
        {
            MainMenuController.instance.DestroyScreen(MainMenuScreens.Message);
        }
        else if(MainMenuControllerTeen.instance!=null)
        {
            MainMenuControllerTeen.instance.DestroyScreen(MainMenuScreensTeen.Message);
        }
        else
        {
            InGameUiManager.instance.DestroyScreen(InGameScreens.Message);
        }


        
    }

    public void OnClickOnNo()
    {
        SoundManager.instance.PlaySound(SoundType.Click);
        noButtonCallBackMethod?.Invoke();
        if (MainMenuController.instance != null)
        {
            MainMenuController.instance.DestroyScreen(MainMenuScreens.Message);
        }
        else
        {
            InGameUiManager.instance.DestroyScreen(InGameScreens.Message);
        }
    }

    public void OnClickOnYes()
    {
        SoundManager.instance.PlaySound(SoundType.Click);
        yesButtonCallBackMethod?.Invoke();
        if (MainMenuController.instance != null)
        {
            MainMenuController.instance.DestroyScreen(MainMenuScreens.Message);
        }
        else if(InGameManager.instance != null)
        {
            InGameUiManager.instance.DestroyScreen(InGameScreens.Message);
        }
        else if(MainMenuControllerTeen.instance!=null)
        {
            MainMenuControllerTeen.instance.DestroyScreen(MainMenuScreensTeen.Message);
        }
    }

    public void ExecuteDefaultMethod()
    {
        defaultCallBackMethod?.Invoke();
    }

}

using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LinkYourEmailManager : MonoBehaviour
{
    public static LinkYourEmailManager instance;

   
    [Header("FOR LINK EMAIL")]
    public GameObject forLinkEmail;
    public InputField emailInputField;
    public InputField verificationCodeInputField;
    public GameObject wrongEmail;
    public GameObject wrongCode;
    public GameObject btnVerificationCode;
    public GameObject btnLinkEmail;

    private bool isColorBtnLinkEmail;
    private Image colorImgBtnLinkEmail;

    [Header("Already LINK EMAIL")]
    public GameObject AlreaLinkEmail;
    public Text emailAlreadyTxt;

   
    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        if (!PlayerPrefs.HasKey("USER_EMAIL"))
        {
            forLinkEmail.SetActive(true);
            AlreaLinkEmail.SetActive(false);
        }
        else {
            forLinkEmail.SetActive(false);
            AlreaLinkEmail.SetActive(true);
            emailAlreadyTxt.text = PlayerPrefs.GetString("USER_EMAIL");
        }
       
    }

    private void Start()
    {
        colorImgBtnLinkEmail = btnLinkEmail.GetComponent<Image>();
        colorImgBtnLinkEmail.color = new Color32(140, 140, 140, 255);
        btnLinkEmail.GetComponent<Button>().enabled = false;
    }
    private void FixedUpdate()
    {
        if (verificationCodeInputField.text.Length > 0)
        {
             isColorBtnLinkEmail = true;
            colorImgBtnLinkEmail.color = new Color32(52, 140, 52, 255);
            btnLinkEmail.GetComponent<Button>().enabled = true;
        }
    }

    public void OnCloseLinkYourEmail()
    {
        SoundManager.instance.PlaySound(SoundType.Click);

        MainMenuController.instance.DestroyScreen(MainMenuScreens.LinkYourEmail);
    }


    public void OnClickVerifyCode() {
        SoundManager.instance.PlaySound(SoundType.Click);

        bool hasAt = emailInputField.text.IndexOf('@') > 0;
        if (hasAt)
        {
             wrongEmail.SetActive(false);
        }
        else
        {
            wrongEmail.SetActive(true);
        }

    }
    public void OnClickLinkEmail()
    {
        SoundManager.instance.PlaySound(SoundType.Click);

        bool hasAt = emailInputField.text.IndexOf('@') > 0;
        if (hasAt)
        {
             wrongEmail.SetActive(false);
            FetchUserData();
        }
        else
        {
            wrongEmail.SetActive(true);
        }

    }


    public void UnlinkBtnClick() {
        SoundManager.instance.PlaySound(SoundType.Click);

        MainMenuController.instance.ShowScreen(MainMenuScreens.UnlinkYourEmail);

    }



    private void FetchUserData()
    {
        string requestData = "{\"userId\":\"" + PlayerManager.instance.GetPlayerGameData().userId + "\"," +
             "\"email\":\"" + emailInputField.text + "\"," +
              "\"otp\":\"" + ""+ "\"}";
      


        MainMenuController.instance.ShowScreen(MainMenuScreens.Loading);
        WebServices.instance.SendRequest(RequestType.emailVerified, requestData, true, OnServerResponseFound);

       
    }

    public void OnServerResponseFound(RequestType requestType, string serverResponse, bool isShowErrorMessage, string errorMessage)
    {
        MainMenuController.instance.DestroyScreen(MainMenuScreens.Loading);

        if (errorMessage.Length > 0)
        {
            if (isShowErrorMessage)
            {
                if (requestType == RequestType.emailVerified)
                {
                    MainMenuController.instance.ShowMessage(errorMessage, () =>
                    {
                        FetchUserData();
                    });
                }
                else
                {
                    MainMenuController.instance.ShowMessage(errorMessage);
                }
            }

            return;
        }
        //{ "success":0,"status":false,"message":"Failed","response":"Invaild OTP"}
        //{"success":1,"status":true,"message":"Success","response":"Otp sent to your email successfully","otp":"906i2F"}

        if (requestType == RequestType.emailVerified)
        {
            JsonData data = JsonMapper.ToObject(serverResponse);

            if (data["success"].ToString()== "1")
            {
                Debug.Log("uhuidhuhfdabuuuuuuuu");
                MainMenuController.instance.DestroyScreen(MainMenuScreens.LinkYourEmail);
                MainMenuController.instance.ShowScreen(MainMenuScreens.LinkingSucessfull);
                PlayerPrefs.SetString("USER_EMAIL", emailInputField.text);
            }
            else
            {
                MainMenuController.instance.ShowMessage(data["response"].ToString());
                
            }
        }
       
    }
}

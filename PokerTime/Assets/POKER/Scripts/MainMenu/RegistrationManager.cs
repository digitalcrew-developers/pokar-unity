using System.Collections;
using System.Collections.Generic;
using LitJson;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RegistrationManager : MonoBehaviour
{
    public GameObject registrationScreen, loginScreen,signUpScreen, forgotPassword;
    public InputField registrationUserName, registrationPassword, registrationConfirmPassword;
    public InputField loginUserName, loginPassword;

    public Text popUpText, wrongPasswordText;

    //DEV_CODE
    public TMP_InputField tmp_registrationUserName, tmp_registrationPassword, tmp_registrationConfirmPassword;
    public TMP_InputField tmp_loginUserName, tmp_loginPassword;

    private void OnEnable()
    {
        popUpText.gameObject.SetActive(false);
        wrongPasswordText.gameObject.SetActive(false);
        forgotPassword.SetActive(false);

        if (GlobalGameManager.instance.isLoginShow)
        {
            registrationScreen.SetActive(false);
            loginScreen.SetActive(true);
            signUpScreen.SetActive(false);
        }
        else {
            registrationScreen.SetActive(false);
            loginScreen.SetActive(false);
            signUpScreen.SetActive(true);
        }
    }

    public void OnClickOnButton(string eventName)
    {
        SoundManager.instance.PlaySound(SoundType.Click);


        switch (eventName)
        {
            case "back":
                {
                    OnClickOnBack();
                }
            break;


            case "submit":
                {
                    if (loginScreen.activeInHierarchy)
                    {
                        string error;

                        if (!Utility.IsValidUserName(/*tmp_loginUserName*/loginUserName.text, out error))
                        {
                            /*wrongPasswordText.gameObject.SetActive(true);*/
                            StartCoroutine(MsgForVideo(error, 1.5f));
                            /*MainMenuController.instance.ShowMessage(error);*/
                            /*return;*/
                            break;
                        }
                        else if (!Utility.IsValidPassword(/*tmp_loginPassword*/loginPassword.text, out error))
                        {
                            /*wrongPasswordText.gameObject.SetActive(true);*/
                            StartCoroutine(MsgForVideo(error, 1.5f));
                            /*MainMenuController.instance.ShowMessage(error);*/
                            /*return;*/
                            break;
                        }
                        else
                        {
                            string requestData = "{\"userName\":\"" + /*tmp_loginUserName.text*/loginUserName.text + "\"," +
                               "\"userPassword\":\"" + /*tmp_loginPassword*/loginPassword.text + "\"," +
                               "\"registrationType\":\"Custom\"," +
                               "\"socialId\":\"\"}";

                            MainMenuController.instance.ShowScreen(MainMenuScreens.Loading);
                            WebServices.instance.SendRequest(RequestType.Login, requestData, true, OnServerResponseFound);
                        }
                    }
                    else if (registrationScreen.activeInHierarchy)
                    {
                        string error;

                        if (!Utility.IsValidUserName(/*tmp_registrationUserName*/registrationUserName.text, out error))
                        {
                            StartCoroutine(MsgForVideo(error, 1.5f));
                            /*MainMenuController.instance.ShowMessage(error);*/
                            /*return;*/
                            break;
                        }
                        else if (!Utility.IsValidPassword(/*tmp_registrationPassword*/registrationPassword.text, out error))
                        {
                            StartCoroutine(MsgForVideo(error, 1.5f));
                            /*MainMenuController.instance.ShowMessage(error);*/
                            /*return;*/
                            break;
                        }
                        else if (/*tmp_registrationConfirmPassword*/registrationConfirmPassword.text != /*tmp_registrationPassword*/registrationPassword.text)
                        {
                            StartCoroutine(MsgForVideo("password does not matched", 1.5f));
                            /*MainMenuController.instance.ShowMessage("password does not matched");*/
                            /*return;*/
                            break;
                        }
                        else
                        {
                            string requestData = "{\"userName\":\"" + /*tmp_registrationUserName*/registrationUserName.text + "\"," +
                           "\"userPassword\":\"" + /*tmp_registrationPassword*/registrationPassword.text + "\"," +
                           "\"registrationType\":\"Custom\"," +
                           "\"socialId\":\"\"}";

                            MainMenuController.instance.ShowScreen(MainMenuScreens.Loading);
                            WebServices.instance.SendRequest(RequestType.Registration, requestData, true, OnServerResponseFound);
                        }
                    }
                    else
                    {
#if ERROR_LOG
                        Debug.LogError("Unhandled case in submit in registrationManager");
#endif
                    }
                }
            break;


            case "openLogin":
                {
                    ResetLoginScreen();

                    loginScreen.SetActive(true);
                    registrationScreen.SetActive(false);
                    signUpScreen.SetActive(false);
                }
            break;


            case "openRegistration":
                {
                    ResetRegistrationScreen();

                    loginScreen.SetActive(false);
                    registrationScreen.SetActive(true);
                    signUpScreen.SetActive(false);
                }
                break;

            case "forgotpwd":
                {
                    forgotPassword.SetActive(true);

                    loginScreen.SetActive(false);
                    registrationScreen.SetActive(false);
                    signUpScreen.SetActive(false);
                }
                break;

            default:
            Debug.LogError("Unhandled eventName found = "+eventName);
            break;
        }
    }

    public void SubmitEmailForForgotPassword()
    {

        bool hasAt = forgotPassword.transform.Find("Email").GetComponent<InputField>().text.IndexOf('@') > 0;
        if (hasAt)
        {
            forgotPassword.transform.Find("WrongEmail").gameObject.SetActive(false);
            FetchUserData();
        }
        else
        {
            forgotPassword.transform.Find("WrongEmail").gameObject.SetActive(true);
        }
    }

    void FetchUserData()
    {
        string requestData = "{\"email\":\"" + forgotPassword.transform.Find("Email").GetComponent<InputField>().text + "\"}";

        MainMenuController.instance.ShowScreen(MainMenuScreens.Loading);
        WebServices.instance.SendRequest(RequestType.ForgotPassword, requestData, true, OnServerResponseFound);
    }

    private void ResetLoginScreen()
    {
        loginUserName.text = "";
        loginPassword.text = "";
        //tmp_loginUserName.text = "";
        /*tmp_loginPassword.text = "";*/
    }

    private void ResetRegistrationScreen()
    {
        /*tmp_registrationUserName*/registrationUserName.text = "";
        /*tmp_registrationPassword*/registrationPassword.text = "";
        /*tmp_registrationConfirmPassword*/registrationConfirmPassword.text = "";
    }

    public Sprite EyeOff, EyeOn;
    public Image RegisterPasswordEye, LoginPasswordEye;

    public void RegisterEyeClick() {
        if (this./*tmp_registrationPassword*/registrationPassword != null)
        {
            if (this./*tmp_registrationPassword*/registrationPassword.contentType== /*TMP_InputField.ContentType.Password*/InputField.ContentType.Password)
            {
                RegisterPasswordEye.sprite = EyeOn;
                this./*tmp_registrationPassword*/registrationPassword.contentType = /*TMP_InputField.ContentType.Standard*/InputField.ContentType.Standard;
            }
            else
            {
                RegisterPasswordEye.sprite = EyeOff;
                this./*tmp_registrationPassword*/registrationPassword.contentType = /*TMP_InputField.ContentType.Password*/InputField.ContentType.Password;
            }

            this.registrationPassword.ForceLabelUpdate();
        }
    }
    public void LoginEyeClick() {
        if (this./*tmp_loginPassword*/loginPassword != null)
        {
            if (this./*tmp_loginPassword*/loginPassword.contentType == /*TMP_InputField.ContentType.Password*/InputField.ContentType.Password)
            {
                LoginPasswordEye.sprite = EyeOn;
                this./*tmp_loginPassword*/loginPassword.contentType = /*TMP_InputField.ContentType.Standard*/InputField.ContentType.Standard;
            }
            else
            {
                LoginPasswordEye.sprite = EyeOff;
                this./*tmp_loginPassword*/loginPassword.contentType = /*TMP_InputField.ContentType.Password*/InputField.ContentType.Password;
            }

            this./*tmp_loginPassword*/loginPassword.ForceLabelUpdate();
        }
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

        if (requestType == RequestType.Registration)
        {
            JsonData data = JsonMapper.ToObject(serverResponse);

            Debug.Log("data " + JsonMapper.ToJson(data));

            //Debug.Log("User Success Status:" + data.Count);

            if (data["success"].ToString() == "1")
            {
                //MainMenuController.instance.ShowMessage(data["message"].ToString());

                //JsonData parsedObject = JsonMapper.ToObject(data["result"].ToString().Replace(@"\",""));

                PlayerGameDetails playerData = PlayerManager.instance.GetPlayerGameData();
                /*playerData.userId = parsedObject["userId"].ToString();*/
                playerData.userId = data["result"]["userId"].ToString();
                playerData.userName = /*tmp_registrationUserName*/registrationUserName.text;
                playerData.password = /*tmp_registrationPassword*/registrationPassword.text;


                //MainMenuController.instance.ShowMessage(data["message"].ToString());

                ResetLoginScreen();
                ResetRegistrationScreen();

                StartCoroutine(MsgForVideo("Registered Successfully", 1.5f));

                loginScreen.SetActive(true);
                registrationScreen.SetActive(false);

                /*string requestData = "{\"userName\":\"" + registrationUserName.text + "\"," +
                         "\"userPassword\":\"" + registrationPassword.text + "\"," +
                         "\"registrationType\":\"Custom\"," +
                         "\"socialId\":\"\"}";


                 MainMenuController.instance.ShowScreen(MainMenuScreens.Loading);
                 WebServices.instance.SendRequest(RequestType.Login, requestData, true, OnServerResponseFound);*/
            }
            else
            {
                //Debug.Log("Uesr already exist");
                //MainMenuController.instance.ShowMessage(data["message"].ToString());
                loginScreen.SetActive(true);
                registrationScreen.SetActive(false);
            }
        }
        else if (requestType == RequestType.Login)
        {
            JsonData data = JsonMapper.ToObject(serverResponse);

            if (data["success"].ToString() == "1")
            {
                PlayerGameDetails playerData = Utility.ParsePlayerGameData(data);

                playerData.userName = /*tmp_loginUserName*/loginUserName.text;
                playerData.password = /*tmp_loginPassword*/loginPassword.text;

                PlayerManager.instance.SetPlayerGameData(playerData);

                //Activate bottom panel
                if (!MainMenuController.instance.bottomPanel.activeSelf)
                    MainMenuController.instance.bottomPanel.SetActive(true);

                MainMenuController.instance._ShowScreen(MainMenuScreens.Shop);
                MainMenuController.instance._ShowScreen(MainMenuScreens.Profile);
                MainMenuController.instance._ShowScreen(MainMenuScreens.Lobby);
                MainMenuController.instance._ShowScreen(MainMenuScreens.MainMenu);
                //MainMenuController.instance.ShowMessage(data["message"].ToString());
                // GlobalGameManager.instance.SendFirebaseToken(FireBaseAnalyticsIntegration.TOKEN);
            }
            else
            {
                //MainMenuController.instance.ShowMessage(data["message"].ToString());
                wrongPasswordText.gameObject.SetActive(true);
                StartCoroutine(MsgForVideo("Incorrect password or username does not exist", 1.5f));
            }
        }
        else if (requestType == RequestType.ForgotPassword)
        {
            JsonData data = JsonMapper.ToObject(serverResponse);
            if (data["success"].ToString() == "1")
            {
                MainMenuController.instance.ShowMessage(data["response"].ToString());
                ResetLoginScreen();
                forgotPassword.SetActive(false);
            }
            else
            {
                MainMenuController.instance.ShowMessage(data["response"].ToString());
                ResetLoginScreen();
                forgotPassword.SetActive(false);
                Debug.Log("Unable to process");
            }
        }
        else
        {

#if ERROR_LOG
            Debug.LogError("Unhandled server requestType found  " + requestType);
#endif
        }

    }


    public void OnClickOnBack()
    {
        if (PlayerManager.instance.IsLogedIn())
        {
            MainMenuController.instance.ShowScreen(MainMenuScreens.MainMenu);
        }
        else
        {
            MainMenuController.instance.ShowMessage("Do you really want to quit?", () => {
                GlobalGameManager.instance.CloseApplication();
            }, () => { });
        }
    }

    IEnumerator MsgForVideo(string msg, float delay)
    {
        popUpText.gameObject.SetActive(true);
        popUpText.text = msg;
        yield return new WaitForSeconds(delay);
        popUpText.gameObject.SetActive(false);
        wrongPasswordText.gameObject.SetActive(false);
    }
}

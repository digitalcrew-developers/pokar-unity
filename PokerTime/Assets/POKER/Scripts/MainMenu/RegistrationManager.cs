using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LitJson;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RegistrationManager : MonoBehaviour
{
    public static RegistrationManager instance = null;

    public GameObject registrationScreen, loginScreen,signUpScreen, forgotPassword, resetPassword;
    public InputField registrationUserName, registrationPassword, registrationConfirmPassword;
    public InputField loginUserName, loginPassword;
    public InputField newPassword;

    public Text popUpText, wrongPasswordText;

    //DEV_CODE
    public TMP_InputField tmp_registrationUserName, tmp_registrationPassword, tmp_registrationConfirmPassword;
    public TMP_InputField tmp_loginUserName, tmp_loginPassword;

    [Header("Forgot Password")]
    public InputField verificationCodeInputField;
    public Button GetVerificationCode;
    
    private float timer = 1;
    private string verificationCode = string.Empty;

    private void Awake()
    {
        if (null== instance)
        {
            instance = this;
        }
    }

    private void OnEnable()
    {
        verificationCodeInputField.interactable = true;
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


    private void FixedUpdate()
    {
        if (timer > 1)
        {
            verificationCodeInputField.interactable = true;
            timer -= Time.deltaTime;
            GetVerificationCode.transform.GetChild(0).GetComponent<Text>().text = "Resend After " + timer.ToString("f0") + "s";
        }
        else if (timer < 1)
        {
            GetVerificationCode.interactable = true;
            verificationCodeInputField.transform.Find("WrongVeriCode").gameObject.SetActive(false);
            GetVerificationCode.transform.GetChild(0).GetComponent<Text>().text = "Resend";
            GetVerificationCode.transform.GetChild(0).GetComponent<Text>().color = new Color32(140, 224, 240, 255);
        }

        if (verificationCodeInputField.text.Length > 0 && forgotPassword.transform.Find("Email").GetComponent<InputField>().text.Length > 0)
        {
            forgotPassword.transform.Find("Submit").GetComponent<Button>().interactable = true;
        }
        else
        {
            forgotPassword.transform.Find("Submit").GetComponent<Button>().interactable = false;
        }
    }


    public void OnValueChangedEmail()
    {
        if (forgotPassword.transform.Find("WrongEmail").gameObject.activeSelf)
        {
            forgotPassword.transform.Find("WrongEmail").GetComponent<Text>().text = "";
            forgotPassword.transform.Find("WrongEmail").gameObject.SetActive(false);
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
                        //no confirm password in register anymore
                        //else if (/*tmp_registrationConfirmPassword*/registrationConfirmPassword.text != /*tmp_registrationPassword*/registrationPassword.text)
                        //{
                        //    StartCoroutine(MsgForVideo("password does not matched", 1.5f));
                        //    /*MainMenuController.instance.ShowMessage("password does not matched");*/
                        //    /*return;*/
                        //    break;
                        //}
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
                    else if(forgotPassword.activeInHierarchy)
                    {
                        if (verificationCodeInputField.text.Equals(verificationCode) && forgotPassword.transform.Find("Email").GetComponent<InputField>().text.Length > 0)
                        {
                            resetPassword.SetActive(true);
                            ResetForgotPwdScreen();
                            forgotPassword.SetActive(false);
                        }
                        else
                        {
                            verificationCodeInputField.transform.Find("WrongVeriCode").GetComponent<Text>().text = "Wrong verification code";
                            verificationCodeInputField.transform.Find("WrongVeriCode").GetComponent<Text>().color = Color.red;
                            verificationCodeInputField.transform.Find("WrongVeriCode").gameObject.SetActive(true);
                        }
                    }
                    else if(resetPassword.activeInHierarchy)
                    {
                        MainMenuController.instance.ShowMessage("Going To Reset Password...");
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
                    resetPassword.SetActive(false);
                    forgotPassword.SetActive(false);
                    loginScreen.SetActive(true);
                    registrationScreen.SetActive(false);
                    signUpScreen.SetActive(false);
                }
            break;


            case "openRegistration":
                {
                    ResetRegistrationScreen();
                    resetPassword.SetActive(false);
                    forgotPassword.SetActive(false);
                    loginScreen.SetActive(false);
                    registrationScreen.SetActive(true);
                    signUpScreen.SetActive(false);
                }
                break;

            case "forgotpwd":
                {
                    resetPassword.SetActive(false);
                    loginScreen.SetActive(false);
                    registrationScreen.SetActive(false);
                    signUpScreen.SetActive(false);
                    forgotPassword.SetActive(true);
                    forgotPassword.transform.Find("Submit").GetComponent<Button>().interactable = false;
                }
                break;

            case "closeForgotPwd":
                {
                    resetPassword.SetActive(false);
                    forgotPassword.SetActive(false);
                    ResetForgotPwdScreen();
                    loginScreen.SetActive(true);
                    registrationScreen.SetActive(false);
                    signUpScreen.SetActive(false);
                }
                break;

            case "closeLogin":
                {
                    forgotPassword.SetActive(false);
                    ResetLoginScreen();
                    loginScreen.SetActive(false);
                    registrationScreen.SetActive(false);
                    signUpScreen.SetActive(true);
                    resetPassword.SetActive(false);
                }
                break;

            case "closeRegistration":
                {
                    forgotPassword.SetActive(false);
                    ResetRegistrationScreen();
                    loginScreen.SetActive(false);
                    registrationScreen.SetActive(false);
                    signUpScreen.SetActive(true);
                    resetPassword.SetActive(false);
                }
                break;

            case "closeResetPassword":
                {
                    forgotPassword.SetActive(false);
                    ResetResetPwdScreen();
                    loginScreen.SetActive(true);
                    registrationScreen.SetActive(false);
                    signUpScreen.SetActive(false);
                    resetPassword.SetActive(false);
                }
                break;

            case "LoginWithGoogle":
                {
                    GoogleManager.instance.SignInWithGoogle();
                }
                break;

            default:
            Debug.LogError("Unhandled eventName found = "+eventName);
            break;
        }
    }

    public void GetVerificationCodeOnEmail()
    {
        if (forgotPassword.transform.Find("Email").GetComponent<InputField>().text.Length > 0)
        {
            forgotPassword.transform.Find("WrongEmail").gameObject.SetActive(false);
            FetchOTPOnEmail();
        }
        else
        {
            forgotPassword.transform.Find("WrongEmail").GetComponent<Text>().text = "*Content cannot be empty";
            forgotPassword.transform.Find("WrongEmail").gameObject.SetActive(true);
        }
    }

    void FetchOTPOnEmail()
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

    private void ResetResetPwdScreen()
    {
        newPassword.text = "";
    }

    private void ResetForgotPwdScreen()
    {
        timer = 1;

        verificationCodeInputField.text = "";
        verificationCodeInputField.transform.Find("WrongVeriCode").gameObject.SetActive(false);
        verificationCodeInputField.transform.Find("WrongVeriCode").GetComponent<Text>().text = "";

        GetVerificationCode.interactable = true;
        GetVerificationCode.transform.GetChild(0).GetComponent<Text>().text = "Get Verification Code";

        forgotPassword.transform.Find("Email").GetComponent<InputField>().text = "";
        forgotPassword.transform.Find("WrongEmail").GetComponent<Text>().text = "";
    }

    public Sprite EyeOff, EyeOn;
    public Image RegisterPasswordEye, LoginPasswordEye, NewPasswordEye;

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

    public void NewPwdEyeClick()
    {
        if(this.newPassword!=null)
        {
            if(this.newPassword.contentType==InputField.ContentType.Password)
            {
                NewPasswordEye.sprite = EyeOn;
                this.newPassword.contentType = InputField.ContentType.Standard;
            }
            else
            {
                NewPasswordEye.sprite = EyeOff;
                this.newPassword.contentType = InputField.ContentType.Password;
            }

            this.newPassword.ForceLabelUpdate();
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

            Debug.Log("Response => Registration: " + JsonMapper.ToJson(data));

            //Debug.Log("User Success Status:" + data.Count);

            if (data["success"].ToString() == "1")
            {
                //MainMenuController.instance.ShowMessage(data["message"].ToString());

                //JsonData parsedObject = JsonMapper.ToObject(data["result"].ToString().Replace(@"\",""));

                PlayerGameDetails playerData = PlayerManager.instance.GetPlayerGameData();
                if (null == playerData) { playerData = new PlayerGameDetails(); }
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
            //Debug.Log("Response => Login: " + serverResponse);
            JsonData data = JsonMapper.ToObject(serverResponse);
            Debug.Log("Response => Login: " + JsonMapper.ToJson(data));

            if (data["success"].ToString() == "1")
            {
                PlayerGameDetails playerData = Utility.ParsePlayerGameData(data);

                playerData.userName = /*tmp_loginUserName*/loginUserName.text;
                playerData.password = /*tmp_loginPassword*/loginPassword.text;

                PlayerManager.instance.SetPlayerGameData(playerData);

                MainMenuController.instance.SwitchToMainMenu();
            }
            else
            {
                wrongPasswordText.gameObject.SetActive(true);
                StartCoroutine(MsgForVideo("Incorrect password or username does not exist", 1.5f));
            }
        }
        else if (requestType == RequestType.ForgotPassword)
        {
            Debug.Log("Response => ForgotPassword: " + serverResponse);
            JsonData data = JsonMapper.ToObject(serverResponse);
            //if (data["success"].ToString() == "1")
            //{
            //    MainMenuController.instance.ShowMessage(data["response"].ToString());
            //    ResetLoginScreen();
            //    forgotPassword.SetActive(false);
            //}
            //else
            //{
            //    MainMenuController.instance.ShowMessage(data["response"].ToString());
            //    ResetLoginScreen();
            //    forgotPassword.SetActive(false);
            //    Debug.Log("Unable to process");
            //}

            if (data["status"].Equals(true))
            {
                //MainMenuController.instance.ShowMessage(data["response"].ToString());
                //ResetForgotPwdScreen();
                //ResetLoginScreen();
                //forgotPassword.SetActive(false);
                //OnClickOnButton("openLogin");

                if(data["otp"]!=null)
                    verificationCode = data["otp"].ToString();

                timer = 60;
                GetVerificationCode.interactable = false;
                verificationCodeInputField.transform.Find("WrongVeriCode").GetComponent<Text>().text = "Verification code sent";
                verificationCodeInputField.transform.Find("WrongVeriCode").GetComponent<Text>().color = GetVerificationCode.transform.GetChild(0).GetComponent<Text>().color;
                verificationCodeInputField.transform.Find("WrongVeriCode").gameObject.SetActive(true);
            }
            else
            {
                //forgotPassword.SetActive(false);
                //MainMenuController.instance.ShowMessage(data["response"].ToString());
                ////ResetLoginScreen();
                //OnClickOnButton("forgotpwd");
                forgotPassword.transform.Find("WrongEmail").GetComponent<Text>().text = "Email address has yet to be linked";
                forgotPassword.transform.Find("WrongEmail").gameObject.SetActive(true);
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
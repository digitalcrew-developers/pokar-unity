using System.Collections;
using System.Collections.Generic;
using LitJson;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RegistrationManager : MonoBehaviour
{
    public GameObject registrationScreen, loginScreen,signUpScreen;
    public InputField registrationUserName, registrationPassword, registrationConfirmPassword;
    public InputField loginUserName, loginPassword;

    //DEV_CODE
    public TMP_InputField tmp_registrationUserName, tmp_registrationPassword, tmp_registrationConfirmPassword;
    public TMP_InputField tmp_loginUserName, tmp_loginPassword;

    private void OnEnable()
    {
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

                        if (!Utility.IsValidUserName(tmp_loginUserName.text, out error))
                        {
                            MainMenuController.instance.ShowMessage(error);
                            return;
                        }

                        if (!Utility.IsValidPassword(tmp_loginPassword.text, out error))
                        {
                            MainMenuController.instance.ShowMessage(error);
                            return;
                        }

                        string requestData = "{\"userName\":\"" + tmp_loginUserName.text + "\"," +
                           "\"userPassword\":\"" + tmp_loginPassword.text + "\"," +
                           "\"registrationType\":\"Custom\"," +
                           "\"socialId\":\"\"}";

                        MainMenuController.instance.ShowScreen(MainMenuScreens.Loading);
                        WebServices.instance.SendRequest(RequestType.Login, requestData, true, OnServerResponseFound);
                    }
                    else if (registrationScreen.activeInHierarchy)
                    {
                        string error;

                        if (!Utility.IsValidUserName(tmp_registrationUserName.text, out error))
                        {
                            MainMenuController.instance.ShowMessage(error);
                            return;
                        }

                        if (!Utility.IsValidPassword(tmp_registrationPassword.text, out error))
                        {
                            MainMenuController.instance.ShowMessage(error);
                            return;
                        }

                        if (tmp_registrationConfirmPassword.text != tmp_registrationPassword.text)
                        {
                            MainMenuController.instance.ShowMessage("password does not matched");
                            return;
                        }

                        string requestData = "{\"userName\":\"" + tmp_registrationUserName.text + "\"," +
                           "\"userPassword\":\"" + tmp_registrationPassword.text + "\"," +
                           "\"registrationType\":\"Custom\"," +
                           "\"socialId\":\"\"}";


                        MainMenuController.instance.ShowScreen(MainMenuScreens.Loading);
                        WebServices.instance.SendRequest(RequestType.Registration, requestData, true, OnServerResponseFound);
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


            default:
            Debug.LogError("Unhandled eventName found = "+eventName);
            break;
        }
    }

    private void ResetLoginScreen()
    {
        tmp_loginUserName.text = "";
        tmp_loginPassword.text = "";
    }

    private void ResetRegistrationScreen()
    {
        tmp_registrationUserName.text = "";
        tmp_registrationPassword.text = "";
        tmp_registrationConfirmPassword.text = "";
    }

    public void RegisterEyeClick() {
        if (this.tmp_registrationPassword != null)
        {
            if (this.tmp_registrationPassword.contentType== TMP_InputField.ContentType.Password)
            {
                this.tmp_registrationPassword.contentType = TMP_InputField.ContentType.Standard;
            }
            else
            {
                this.tmp_registrationPassword.contentType = TMP_InputField.ContentType.Password;
            }

            this.tmp_registrationPassword.ForceLabelUpdate();
        }
    }
    public void LoginEyeClick() {
        if (this.tmp_loginPassword != null)
        {
            if (this.tmp_loginPassword.contentType == TMP_InputField.ContentType.Password)
            {
                this.tmp_loginPassword.contentType = TMP_InputField.ContentType.Standard;
            }
            else
            {
                this.tmp_loginPassword.contentType = TMP_InputField.ContentType.Password;
            }

            this.tmp_loginPassword.ForceLabelUpdate();
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

            Debug.Log("data "+JsonMapper.ToJson(data));

            //Debug.Log("User Success Status:" + data.Count);

            if (data["success"].ToString() == "1")
            {
                MainMenuController.instance.ShowMessage(data["message"].ToString());
                
                //JsonData parsedObject = JsonMapper.ToObject(data["result"].ToString().Replace(@"\",""));

                PlayerGameDetails playerData = PlayerManager.instance.GetPlayerGameData();
                /*playerData.userId = parsedObject["userId"].ToString();*/
                playerData.userId = data["result"]["userId"].ToString();
                playerData.userName = tmp_registrationUserName.text;
                playerData.password = tmp_registrationPassword.text;

                
                MainMenuController.instance.ShowMessage(data["message"].ToString());

                ResetLoginScreen();
                ResetRegistrationScreen();

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
                Debug.Log("Uesr already exist");
                MainMenuController.instance.ShowMessage(data["message"].ToString());
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

                playerData.userName = tmp_loginUserName.text;
                playerData.password = tmp_loginPassword.text;

                PlayerManager.instance.SetPlayerGameData(playerData);

                //Activate bottom panel
                if (!MainMenuController.instance.bottomPanel.activeSelf)
                    MainMenuController.instance.bottomPanel.SetActive(true);

                MainMenuController.instance.ShowScreen(MainMenuScreens.MainMenu);
                MainMenuController.instance.ShowMessage(data["message"].ToString());
                // GlobalGameManager.instance.SendFirebaseToken(FireBaseAnalyticsIntegration.TOKEN);
            }
            else
            {
                MainMenuController.instance.ShowMessage(data["message"].ToString());
            }
        }
        else
        {

#if ERROR_LOG
            Debug.LogError("Unhandled server requestType found  "+requestType);
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

}

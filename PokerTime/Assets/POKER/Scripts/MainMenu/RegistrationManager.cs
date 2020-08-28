using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;
using UnityEngine.UI;

public class RegistrationManager : MonoBehaviour
{
    public GameObject registrationScreen, loginScreen;
    public InputField registrationUserName, registrationPassword, registrationConfirmPassword;
    public InputField loginUserName, loginPassword;


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

                        if (!Utility.IsValidUserName(loginUserName.text, out error))
                        {
                            MainMenuController.instance.ShowMessage(error);
                            return;
                        }

                        if (!Utility.IsValidPassword(loginPassword.text, out error))
                        {
                            MainMenuController.instance.ShowMessage(error);
                            return;
                        }

                        string requestData = "{\"userName\":\"" + loginUserName.text + "\"," +
                           "\"userPassword\":\"" + loginPassword.text + "\"," +
                           "\"registrationType\":\"Custom\"," +
                           "\"socialId\":\"\"}";


                        MainMenuController.instance.ShowScreen(MainMenuScreens.Loading);
                        WebServices.instance.SendRequest(RequestType.Login, requestData, true, OnServerResponseFound);
                    }
                    else if (registrationScreen.activeInHierarchy)
                    {
                        string error;

                        if (!Utility.IsValidUserName(registrationUserName.text, out error))
                        {
                            MainMenuController.instance.ShowMessage(error);
                            return;
                        }

                        if (!Utility.IsValidPassword(registrationPassword.text, out error))
                        {
                            MainMenuController.instance.ShowMessage(error);
                            return;
                        }

                        if (registrationConfirmPassword.text != registrationPassword.text)
                        {
                            MainMenuController.instance.ShowMessage("password does not matched");
                            return;
                        }

                        string requestData = "{\"userName\":\"" + registrationUserName.text + "\"," +
                           "\"userPassword\":\"" + registrationPassword.text + "\"," +
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
                    loginScreen.SetActive(true);
                    registrationScreen.SetActive(false);
                }
            break;


            case "openRegistration":
                {
                    loginScreen.SetActive(false);
                    registrationScreen.SetActive(true);
                }
                break;


            default:
            Debug.LogError("Unhandled eventName found = "+eventName);
            break;
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

            if (data["success"].ToString() == "1")
            {
        
                JsonData parsedObject = JsonMapper.ToObject(data["result"].ToString().Replace(@"\",""));


                PlayerGameDetails playerData = PlayerManager.instance.GetPlayerGameData();
                playerData.userId = parsedObject["userId"].ToString();
                playerData.userName = registrationUserName.text;
                playerData.password = registrationPassword.text;


                string requestData = "{\"userName\":\"" + registrationUserName.text + "\"," +
                        "\"userPassword\":\"" + registrationPassword.text + "\"," +
                        "\"registrationType\":\"Custom\"," +
                        "\"socialId\":\"\"}";


                MainMenuController.instance.ShowScreen(MainMenuScreens.Loading);
                WebServices.instance.SendRequest(RequestType.Login, requestData, true, OnServerResponseFound);
            }
            else
            {
                MainMenuController.instance.ShowMessage(data["message"].ToString());
            }
        }
        else if (requestType == RequestType.Login)
        {
            JsonData data = JsonMapper.ToObject(serverResponse);

            if (data["success"].ToString() == "1")
            {
                PlayerGameDetails playerData = Utility.ParsePlayerGameData(data);

                playerData.userName = loginUserName.text;
                playerData.password = loginPassword.text;

                PlayerManager.instance.SetPlayerGameData(playerData);

                MainMenuController.instance.ShowScreen(MainMenuScreens.MainMenu);
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

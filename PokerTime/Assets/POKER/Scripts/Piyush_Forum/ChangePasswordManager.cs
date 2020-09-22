using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangePasswordManager : MonoBehaviour
{
    public InputField currentPwd;
    public InputField newPwd;
    public InputField confrimPwd;


    public void OnCloseBtnClick()
    {
        SoundManager.instance.PlaySound(SoundType.Click);
        MainMenuController.instance.DestroyScreen(MainMenuScreens.ChangePassword);


    }

    public void OnConfirmBtnClick() {
        SoundManager.instance.PlaySound(SoundType.Click);
        FetchUserData();


    }




    private void FetchUserData()
    {
        string requestData = "{\"userId\":\"" + PlayerManager.instance.GetPlayerGameData().userId + "\"," +
             "\"currentPassword\":\"" + currentPwd.text + "\"," +
             "\"newPassword\":\"" + newPwd.text + "\"," +
              "\"confirmPassword\":\"" + confrimPwd.text + "\"}";
        
        MainMenuController.instance.ShowScreen(MainMenuScreens.Loading);
        WebServices.instance.SendRequest(RequestType.changePassword, requestData, true, OnServerResponseFound);

    }

    public void OnServerResponseFound(RequestType requestType, string serverResponse, bool isShowErrorMessage, string errorMessage)
    {
        MainMenuController.instance.DestroyScreen(MainMenuScreens.Loading);

        if (errorMessage.Length > 0)
        {
            if (isShowErrorMessage)
            {
                if (requestType == RequestType.changePassword)
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

        if (requestType == RequestType.changePassword)
        {
            JsonData data = JsonMapper.ToObject(serverResponse);

            if (data["success"].ToString() == "1")
            {
                if (data["message"].ToString() == "Failed")
                {
                    MainMenuController.instance.ShowMessage(data["response"].ToString());
                }
                else
                {
                    GlobalGameManager.instance.isLoginShow = true;
                    MainMenuController.instance.DestroyScreen(MainMenuScreens.ChangePassword);
                    MainMenuController.instance.DestroyScreen(MainMenuScreens.ProfileSetting);

                    //Deactivate bottom panel
                    if (MainMenuController.instance.bottomPanel.activeSelf)
                        MainMenuController.instance.bottomPanel.SetActive(false);

                    MainMenuController.instance.ShowScreen(MainMenuScreens.Registration);
                }


            }
            else
            {
                MainMenuController.instance.ShowMessage(data["response"].ToString());

            }
        }

    }
}

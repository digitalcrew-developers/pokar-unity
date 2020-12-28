using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangePasswordManagerTeen : MonoBehaviour
{
    public InputField currentPwd;
    public InputField newPwd;
    public InputField confirmPwd;

    public Button confirmBtn;
   
    private void OnEnable()
    {
        confirmBtn.interactable = false;
    }

    void Update()
    {
        //Check if the Input Field is in focus and able to alter
        if (newPwd.text.Length > 0 || confirmPwd.text.Length > 0 || currentPwd.text.Length > 0)
        {
            confirmBtn.interactable = true;
        }
        else
        {
            confirmBtn.interactable = false;
        }
    }


    public void OnCloseBtnClick()
    {
        SoundManager.instance.PlaySound(SoundType.Click);
        MainMenuControllerTeen.instance.DestroyScreen(MainMenuScreensTeen.ChangePassword);
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
              "\"confirmPassword\":\"" + confirmPwd.text + "\"}";

        MainMenuControllerTeen.instance.ShowScreen(MainMenuScreensTeen.Loading);
        WebServices.instance.SendRequest(RequestType.changePassword, requestData, true, OnServerResponseFound);

    }

    public void OnServerResponseFound(RequestType requestType, string serverResponse, bool isShowErrorMessage, string errorMessage)
    {
        MainMenuControllerTeen.instance.DestroyScreen(MainMenuScreensTeen.Loading);

        if (errorMessage.Length > 0)
        {
            if (isShowErrorMessage)
            {
                if (requestType == RequestType.changePassword)
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

        if (requestType == RequestType.changePassword)
        {
            JsonData data = JsonMapper.ToObject(serverResponse);

            if (data["success"].ToString() == "1")
            {
                if (data["message"].ToString() == "Failed")
                {
                    MainMenuControllerTeen.instance.ShowMessage(data["response"].ToString());
                }
                else
                {
                    GlobalGameManager.instance.isLoginShow = true;
                    MainMenuControllerTeen.instance.DestroyScreen(MainMenuScreensTeen.ChangePassword);
                    MainMenuControllerTeen.instance.DestroyScreen(MainMenuScreensTeen.ProfileSetting);

                    //Deactivate bottom panel
                    if (MainMenuControllerTeen.instance.bottomPanel.activeSelf /*&& GameConstants.poker*/)
                        MainMenuControllerTeen.instance.bottomPanel.SetActive(false);
                    //else if (MainMenuController.instance.bottomPanel.activeSelf && !GameConstants.poker)
                    //    MainMenuController.instance.bottomPanelTeen.SetActive(false);

                    MainMenuControllerTeen.instance.ShowScreen(MainMenuScreensTeen.Registration);
                }


            }
            else
            {
                MainMenuControllerTeen.instance.ShowMessage(data["response"].ToString());

            }
        }

    }
}

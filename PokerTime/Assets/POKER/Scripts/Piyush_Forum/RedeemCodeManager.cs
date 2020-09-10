using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RedeemCodeManager : MonoBehaviour
{
    public InputField redeemCodeInput;

    public void OnCloseBtnClick()
    {
        SoundManager.instance.PlaySound(SoundType.Click);
        MainMenuController.instance.DestroyScreen(MainMenuScreens.RedeemCode);


    }

    public void OnConfirmBtnClick()
    {
        SoundManager.instance.PlaySound(SoundType.Click);
        FetchUserData();


    }

    private void FetchUserData()
    {
        string requestData = "{\"userId\":\"" + PlayerManager.instance.GetPlayerGameData().userId + "\"," +
              "\"couponCode\":\"" + redeemCodeInput.text + "\"}";

        MainMenuController.instance.ShowScreen(MainMenuScreens.Loading);
        WebServices.instance.SendRequest(RequestType.redeemCoupon, requestData, true, OnServerResponseFound);

    }

    public void OnServerResponseFound(RequestType requestType, string serverResponse, bool isShowErrorMessage, string errorMessage)
    {
        MainMenuController.instance.DestroyScreen(MainMenuScreens.Loading);

        if (errorMessage.Length > 0)
        {
            if (isShowErrorMessage)
            {
                if (requestType == RequestType.redeemCoupon)
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

        if (requestType == RequestType.redeemCoupon)
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
                    MainMenuController.instance.DestroyScreen(MainMenuScreens.RedeemCode);
                   
                }


            }
            else
            {
                MainMenuController.instance.ShowMessage(data["response"].ToString());

            }
        }

    }
}

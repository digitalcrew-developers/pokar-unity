using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RedeemCodeManagerTeen : MonoBehaviour
{
    public InputField redeemCodeInput;

    public void OnCloseBtnClick()
    {
        SoundManager.instance.PlaySound(SoundType.Click);
        MainMenuControllerTeen.instance.DestroyScreen(MainMenuScreensTeen.RedeemCode);


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

        MainMenuControllerTeen.instance.ShowScreen(MainMenuScreensTeen.Loading);
        WebServices.instance.SendRequest(RequestType.redeemCoupon, requestData, true, OnServerResponseFound);

    }

    public void OnServerResponseFound(RequestType requestType, string serverResponse, bool isShowErrorMessage, string errorMessage)
    {
        MainMenuControllerTeen.instance.DestroyScreen(MainMenuScreensTeen.Loading);

        if (errorMessage.Length > 0)
        {
            if (isShowErrorMessage)
            {
                if (requestType == RequestType.redeemCoupon)
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

        if (requestType == RequestType.redeemCoupon)
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
                    MainMenuControllerTeen.instance.DestroyScreen(MainMenuScreensTeen.RedeemCode);
                   
                }


            }
            else
            {
                MainMenuControllerTeen.instance.ShowMessage(data["response"].ToString());

            }
        }

    }
}

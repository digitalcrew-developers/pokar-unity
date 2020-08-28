using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendFriendRequest : MonoBehaviour
{
    public string TouserId="64";
   
    public void Sendfriendrequest()
    {
        if (PlayerManager.instance.GetPlayerGameData().userId != InGameUiManager.instance.TempUserID)
        {
            string requestData = "{\"userId\":\"" + PlayerManager.instance.GetPlayerGameData().userId + "\"," +
                                   "\"ToUserId\":\"" + InGameUiManager.instance.TempUserID + "\"}";
            WebServices.instance.SendRequest(RequestType.SendFriendRequest, requestData, true, OnServerResponseFound);
        }
    }
    public void OnServerResponseFound(RequestType requestType, string serverResponse, bool isShowErrorMessage, string errorMessage)
    {
        if (errorMessage.Length > 0)
        {
            if (isShowErrorMessage)
            {
                MainMenuController.instance.ShowMessage(errorMessage);
            }
            return;
        }
        if (requestType == RequestType.SendFriendRequest)
        {
            JsonData data = JsonMapper.ToObject(serverResponse);
                InGameUiManager.instance.ShowMessage(data["response"].ToString());
            /*if (data["status"].Equals(true))
            {
            }   */         
        }
        else
        {

#if ERROR_LOG
            Debug.LogError("Unhadnled response found in  = " + requestType);
#endif
        }
    }
}

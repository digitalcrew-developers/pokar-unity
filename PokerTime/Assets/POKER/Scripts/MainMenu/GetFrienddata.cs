using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetFrienddata : MonoBehaviour
{
    public Image profile,frame;
    public Text name;
    public Text online_offline;
    public string id;
    public string UserId;
    public string status;

    public void AcceptFriendRequest()
    {
        string requestData = "{\"id\":\"" +id+ "\"," +
                                "\"Status\":\"" + "Accepted"+ "\"," +
                                  "\"userId\":\"" + PlayerManager.instance.GetPlayerGameData().userId + "\"}";

        WebServices.instance.SendRequest(RequestType.UpdateRequestStatus, requestData, true, OnServerResponseFound);

    }
    public void CancelledFriendRequest()
    {
        string requestData = "{\"id\":\"" + id + "\"," +
                                "\"Status\":\"" + "Cancelled" + "\"," +
                                  "\"userId\":\"" + PlayerManager.instance.GetPlayerGameData().userId + "\"}";

        WebServices.instance.SendRequest(RequestType.UpdateRequestStatus, requestData, true, OnServerResponseFound);
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
        if (requestType == RequestType.UpdateRequestStatus)
        {
            JsonData data = JsonMapper.ToObject(serverResponse);
            MainMenuController.instance.ShowMessage(data["response"].ToString());
          //  Debug.LogError("&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&");
            MainMenuController.instance.DestroyScreen(MainMenuScreens.FriendList);
            MainMenuController.instance.ShowScreen(MainMenuScreens.FriendList);
        }
        else
        {

#if ERROR_LOG
            Debug.LogError("Unhadnled response found in  = " + requestType);
#endif
        }
    }
}

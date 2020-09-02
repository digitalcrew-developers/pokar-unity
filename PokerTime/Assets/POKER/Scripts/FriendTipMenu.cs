using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendTipMenu : MonoBehaviour
{
    public Image profile, frame;
    public Text username, userid;
    public string  id;
    public void OnClosedbtn()
    {
        Destroy(this.gameObject);
    }
    public void Removefriend()
    {
        string requestData = "{\"id\":\"" + id + "\"," +
                              "\"Status\":\"" + "Cancelled" + "\"," +
                                "\"userId\":\"" + PlayerManager.instance.GetPlayerGameData().userId + "\"}";

        WebServices.instance.SendRequest(RequestType.UpdateRequestStatus, requestData, true, OnServerResponseFound);
    }
    void OnServerResponseFound(RequestType requestType, string serverResponse, bool isShowErrorMessage, string errorMessage)
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
            MainMenuController.instance.DestroyScreen(MainMenuScreens.FriendList);
            MainMenuController.instance.ShowScreen(MainMenuScreens.FriendList);
            OnClosedbtn();
        }
        else
        {

#if ERROR_LOG
            Debug.LogError("Unhadnled response found in  = " + requestType);
#endif
        }
    }
}

using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetFrienddata : MonoBehaviour
{
    public Image profile,frame;
    public Text userName;
    public Text online_offline;
    public string id;
    public string UserId;
    public string status;
    public GameObject tipsFrineds;
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
    public void onclickFriend()
    {
        GameObject Friendtips = Instantiate(tipsFrineds, MainMenuController.instance.screenLayers[3].transform);
        Friendtips.GetComponent<FriendTipMenu>().userid.text = UserId;
        Friendtips.GetComponent<FriendTipMenu>().username.text = userName.text;
        Friendtips.GetComponent<FriendTipMenu>().profile.sprite = profile.sprite;
        Friendtips.GetComponent<FriendTipMenu>().frame.sprite = frame.sprite;
        Friendtips.GetComponent<FriendTipMenu>().id = id;

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

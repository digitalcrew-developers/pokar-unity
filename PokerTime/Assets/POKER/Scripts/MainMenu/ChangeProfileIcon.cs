using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class GetAvatars
{
    public int avatarID;
    public string avatarURL, Status;
}

public class ChangeProfileIcon : MonoBehaviour
{
    public GameObject AvatarGO, avatarPrefebs;
    public int SelectedAvatarId;
    public void Start()
    {
        WebServices.instance.SendRequest(RequestType.GetAvatars, "", true, OnServerResponseFound);
    }

    public void OncloseChangeProfileIcon()
    {
        MainMenuController.instance.DestroyScreen(MainMenuScreens.ChangeProfileIcon);
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



        if (requestType == RequestType.GetAvatars)
        {
            JsonData data = JsonMapper.ToObject(serverResponse);

            if (data["status"].Equals(true))
            {
                ReadAllavatars(data);
            }
            else
            {
                MainMenuController.instance.ShowMessage("Unable to get Avatars");
            }

        }
        else
        {

#if ERROR_LOG
            Debug.LogError("Unhadnled response found in  = " + requestType);
#endif
        }

    }
    private void ReadAllavatars(JsonData data)
    {

        for (int i = 0; i < data["getData"].Count; i++)
        {
            GetAvatars getAvatars = new GetAvatars();

            getAvatars.avatarID =int.Parse( data["getData"][i]["avatarID"].ToString());
            getAvatars.avatarURL = data["getData"][i]["avatarURL"].ToString();
       //     getAvatars.Status = data["getData"][i]["Status"].ToString();
            GameObject av = Instantiate(AvatarGO, avatarPrefebs.transform);
            av.GetComponent<SelectAvatar>().urlImage = getAvatars.avatarURL;
            av.GetComponent<SelectAvatar>().avtarid = getAvatars.avatarID;
        }
    }
  
}

using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class GetAvatarsTeen
{
    public int avatarID;
    public string avatarURL, Status;
}

public class ChangeProfileIconTeen : MonoBehaviour
{
    public GameObject AvatarGO, avatarPrefebs;
    public int SelectedAvatarId;
    public void Start()
    {
        WebServices.instance.SendRequest(RequestType.GetAvatars, "", true, OnServerResponseFound);
    }

    public void OncloseChangeProfileIcon()
    {
        MainMenuControllerTeen.instance.DestroyScreen(MainMenuScreensTeen.ChangeProfileIcon);
    }
    public void OnConfirmChangeProfileIcon()
    {
        MainMenuControllerTeen.instance.DestroyScreen(MainMenuScreensTeen.ChangeProfileIcon);
        MainMenuControllerTeen.instance.DestroyScreen(MainMenuScreensTeen.SelectFrom);
    }

    public void OnServerResponseFound(RequestType requestType, string serverResponse, bool isShowErrorMessage, string errorMessage)
    {
     
        if (errorMessage.Length > 0)
        {
            if (isShowErrorMessage)
            {
                MainMenuControllerTeen.instance.ShowMessage(errorMessage);
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
                MainMenuControllerTeen.instance.ShowMessage("Unable to get Avatars");
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
            GetAvatarsTeen getAvatars = new GetAvatarsTeen();

            getAvatars.avatarID =int.Parse( data["getData"][i]["avatarID"].ToString());
            getAvatars.avatarURL = data["getData"][i]["avatarURL"].ToString();
       //     getAvatars.Status = data["getData"][i]["Status"].ToString();
            GameObject av = Instantiate(AvatarGO, avatarPrefebs.transform);
            av.GetComponent<SelectAvatar>().urlImage = getAvatars.avatarURL;
            av.GetComponent<SelectAvatar>().avtarid = getAvatars.avatarID;
        }
    }
  
}

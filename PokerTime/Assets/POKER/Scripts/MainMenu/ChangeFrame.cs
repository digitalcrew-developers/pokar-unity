using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeFrame : MonoBehaviour
{
    public GameObject frameObj, container;
    

    public void Start()
    {
        //string requestData = "{\"PRADEEP\":\"" + "VIVEK" + "\"}";
        WebServices.instance.SendRequest(RequestType.getFrames, /*requestData*/"{}", true, OnServerResponseFound);
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

        if (requestType == RequestType.getFrames)
        {
            JsonData data = JsonMapper.ToObject(serverResponse);

            if (data["status"].Equals(true))
            {
                ReadAllFrames(data);
            }
            else
            {
                MainMenuController.instance.ShowMessage("Unable to get Frames");
            }

        }
        else
        {

#if ERROR_LOG
            Debug.LogError("Unhadnled response found in  = " + requestType);
#endif
        }

    }

    private void ReadAllFrames(JsonData data)
    {
        for (int i = 0; i < data["getData"].Count; i++)
        {
            GetFrames getFrames = new GetFrames();

            getFrames.frameID = int.Parse(data["getData"][i]["frameID"].ToString());
            getFrames.frameURL = data["getData"][i]["frameURL"].ToString();
            //     getAvatars.Status = data["getData"][i]["Status"].ToString();
            GameObject frame = Instantiate(frameObj, container.transform);
            frame.GetComponent<SelectFrame>().urlImage = getFrames.frameURL;
            frame.GetComponent<SelectFrame>().frameId = getFrames.frameID;
        }
    }

    public void CloseBtnChanegFrame()
    {
        MainMenuController.instance.DestroyScreen(MainMenuScreens.ChangeFrame);
        //Destroy(this.gameObject);
    }
}

[System.Serializable]
public class GetFrames
{
    public int frameID;
    public string frameURL, Status;
}
using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeFrameTeen : MonoBehaviour
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
                MainMenuControllerTeen.instance.ShowMessage(errorMessage);
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
                MainMenuControllerTeen.instance.ShowMessage("Unable to get Frames");
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
            GetFramesTeen getFrames = new GetFramesTeen();

            getFrames.frameID = int.Parse(data["getData"][i]["frameID"].ToString());
            getFrames.frameURL = data["getData"][i]["frameURL"].ToString();
            //     getAvatars.Status = data["getData"][i]["Status"].ToString();
            GameObject frame = Instantiate(frameObj, container.transform);
            frame.GetComponent<SelectFrameTeen>().urlImage = getFrames.frameURL;
            frame.GetComponent<SelectFrameTeen>().frameId = getFrames.frameID;
        }
    }

    public void CloseBtnChanegFrame()
    {
        MainMenuControllerTeen.instance.DestroyScreen(MainMenuScreensTeen.ChangeFrame);
        //Destroy(this.gameObject);
    }
}

[System.Serializable]
public class GetFramesTeen
{
    public int frameID;
    public string frameURL, Status;
}
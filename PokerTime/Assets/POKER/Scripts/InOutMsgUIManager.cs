using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class InOutMsgUIManager : MonoBehaviour
{
    public string userId;
    public Image profileImage;

    // Start is called before the first frame update
    void Start()
    {
        WebServices.instance.SendRequest(RequestType.GetUserDetails, "{\"userId\":\"" + userId + "\"}", true, OnServerResponseFound);
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
        if (requestType == RequestType.GetUserDetails)
        {
            JsonData data = JsonMapper.ToObject(serverResponse);

            if (data["success"].ToString() == "1")
            {
                //    Debug.Log("Success data send");
                for (int i = 0; i < data["getData"].Count; i++)
                {
                    string av_url = (data["getData"][i]["profileImage"].ToString());

                    StartCoroutine(loadSpriteImageFromUrl(av_url));
                }
            }
            else
            {
                MainMenuController.instance.ShowMessage(data["message"].ToString());
            }
        }
    }


    IEnumerator loadSpriteImageFromUrl(string URL)
    {
        UnityWebRequest unityWebRequest = UnityWebRequestTexture.GetTexture(URL);
        yield return unityWebRequest.SendWebRequest();

        if (unityWebRequest.isNetworkError || unityWebRequest.isHttpError)
        {
            Debug.LogError("Download failed");
        }
        else
        {
            var Text = DownloadHandlerTexture.GetContent(unityWebRequest);
            Sprite sprite = Sprite.Create(Text, new Rect(0, 0, Text.width, Text.height), Vector2.zero);
            profileImage.sprite = sprite;

            Debug.Log("Successfully Set Player Profile");
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class DownloadAvatar : MonoBehaviour
{
    public string avatarUrl;
    public Image avatarSprite;

    private void OnEnable()
    {
        StartCoroutine(loadSpriteImageFromUrl());
    }

    IEnumerator loadSpriteImageFromUrl()
    {
        Debug.Log("Going To Set User Profile and Flag " + gameObject.name);
        UnityWebRequest unityWebRequest = UnityWebRequestTexture.GetTexture(avatarUrl);
        yield return unityWebRequest.SendWebRequest();

        if (unityWebRequest.isNetworkError || unityWebRequest.isHttpError)
        {
            Debug.LogError("Download failed");
        }
        else
        {
            //  image.sprite = null;
            var Text = DownloadHandlerTexture.GetContent(unityWebRequest);
            Sprite sprite = Sprite.Create(Text, new Rect(0, 0, Text.width, Text.height), Vector2.zero);

            avatarSprite.sprite = sprite;

        }
    }
}

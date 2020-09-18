using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class HandPostPanel : MonoBehaviour
{
    public static HandPostPanel instance;

    [SerializeField]
    public string userID;
    public string path;

    public Image videoImage;
    public InputField description;
    public Text hashTagText;

    private void Awake()
    {
        instance = this;

        userID = PlayerManager.instance.GetPlayerGameData().userId;
        
        //videoImage.mainTexture = AssetPreview.GetMiniThumbnail(path);
    }

    private void OnEnable()
    {
        hashTagText.text = "#hashtag";
    }

    public void OnClickBackButton()
    {
        gameObject.SetActive(false);
    }

    public void OnPostVideo()
    {
        //To Upload Video
        StartCoroutine(UploadVideo(path));
    }


    IEnumerator UploadVideo(string path)
    {
        Debug.Log("Started Uploading Vide..." + path);
        byte[] videoByte = File.ReadAllBytes(path);
        WWWForm formData = new WWWForm();
        formData.AddField("userId", userID);
        formData.AddField("description", description.text);
        formData.AddBinaryData("forumImage", videoByte, path, "video/mp4");

        UnityWebRequest www = UnityWebRequest.Post("http://3.17.201.78:3000/createForum", formData);

        Debug.Log("Uploading !!!!!!");
        yield return www.SendWebRequest();

        Debug.Log("Upload Success...");

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("Form upload complete! and Response: " + www.downloadHandler.text);
        }
    }    
}
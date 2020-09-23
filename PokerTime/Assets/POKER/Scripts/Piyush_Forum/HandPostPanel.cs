using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;

public class HandPostPanel : MonoBehaviour
{
    public static HandPostPanel instance;

    [SerializeField]
    public string userID;
    public string path;

    public RawImage bgImage, frontImage;
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
        //hashTagText.text = "#hashtag";
        path = HandUiManager.instance.videoPath;
        LoadImage();
    }

    public void OnClickBackButton()
    {
        gameObject.SetActive(false);
    }

    public void LoadImage()
    {
        string newPath = "Image";

        string[] x = path.Split('_');
        for (int i = 1; i < x.Length - 1; i++)
        {
            Debug.Log("Val " + i + " : " + x[i] + " Length: " + x[i].Length);
            newPath = newPath + "_" + x[i];
        }

        if (!Directory.Exists(Path.Combine(Application.persistentDataPath, "Screenshots")))
            Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "Screenshots"));

        byte[] byteArray = File.ReadAllBytes(Path.Combine(Application.persistentDataPath, "Screenshots", newPath + "_.png"));
        Texture2D sampleTexture = new Texture2D(2, 2);
        // the size of the texture will be replaced by image size
        bool isLoaded = sampleTexture.LoadImage(byteArray);
        
        // apply this texure as per requirement on image or material
        if (isLoaded)
        {
            bgImage.texture = sampleTexture;
            frontImage.texture = sampleTexture;
        }
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
        MainMenuController.instance.ShowMessage("Uploaded Successfully.");
        gameObject.SetActive(false);
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
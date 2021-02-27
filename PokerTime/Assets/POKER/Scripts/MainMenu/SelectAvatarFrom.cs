using DG.Tweening.Plugins.Core.PathCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using ImageAndVideoPicker;
using UnityEngine.UI;
using UnityEditor;

public class SelectAvatarFrom : MonoBehaviour
{
    public Text pathText;

    public Image profileImage;
    private string profileImagePath;

    public void OnCloseSelectFrom()
    {
        MainMenuController.instance.DestroyScreen(MainMenuScreens.SelectFrom);
    }
    public void OnClickDefaultbtn()
    {
        MainMenuController.instance.ShowScreen(MainMenuScreens.ChangeProfileIcon);
    }

    public void OpenGallery()
    {
#if UNITY_ANDROID
        AndroidPicker.BrowseImage(false);
#elif UNITY_EDITOR
        string path = EditorUtility.OpenFilePanel("Ovrewrite with jpg", "", "");
        if (path != null)
        {
            WWW www = new WWW("" + path);
            Sprite sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), Vector2.zero);
            ProfileScreenUiManager.instance.avtar.sprite = sprite;
            
            /*ProfileImage.texture = www.texture;*/
        }
#endif   
    }

    public void UploadProfileImage()
    {
        //pathText.text = "Inside Upload Image Method";
        StartCoroutine(UploadImage());
    }

    public IEnumerator UploadImage()
    {
        //pathText.text = "Start Coroutine To Upload Image";

        Texture2D newTexture = new Texture2D(profileImage.mainTexture.width, profileImage.mainTexture.height);
        newTexture.LoadRawTextureData(newTexture.GetRawTextureData());
        newTexture.Apply();

        byte[] bytes = newTexture.EncodeToJPG();
        Destroy(newTexture);

        //pathText.text = "Converted to byte data with Path" + profileImagePath + " -- " + PlayerManager.instance.GetPlayerGameData().userId;
        var form = new WWWForm();
        form.AddField("userId", PlayerManager.instance.GetPlayerGameData().userId);
        //form.AddField("userName", PlayerManager.instance.GetPlayerGameData().userName);
        //form.AddField("language", "hindi");
        form.AddBinaryData("profileImage", bytes, profileImagePath, "image/jpg");
        //form.AddField("nickName", PlayerManager.instance.GetPlayerGameData().nickname);
        //form.AddField("mobile", PlayerManager.instance.GetPlayerGameData().mobile);
        //form.AddField("emailId", PlayerManager.instance.GetPlayerGameData().emailId);

        UnityWebRequest www = UnityWebRequest.Post(GameConstants.API_URL + "/updateProfile", form);

        //pathText.text = "Uploading!!!";
        //Debug.Log("Uploading !!!!!!");
        yield return www.SendWebRequest();

        //pathText.text = "Upload Success....";
        //Debug.Log("Upload Success...");

        if (www.isNetworkError || www.isHttpError)
        {
            //pathText.text = www.error.ToString();
            //Debug.Log(www.error);
        }
        else
        {
            //pathText.text = www.downloadHandler.text;
            OnCloseSelectFrom();
            ////Debug.Log("Form upload complete! and Response: " + www.downloadHandler.text);
        }
    }

    #region Image Picking Methods
    void OnEnable()
    {
        PickerEventListener.onImageSelect += OnImageSelect;
        PickerEventListener.onImageLoad += OnImageLoad;
        PickerEventListener.onError += OnError;
        PickerEventListener.onCancel += OnCancel;

#if UNITY_ANDROID
        AndroidPicker.CheckPermissions();
#endif
    }

    void OnDisable()
    {
        PickerEventListener.onImageSelect -= OnImageSelect;
        PickerEventListener.onImageLoad -= OnImageLoad;
        PickerEventListener.onError -= OnError;
        PickerEventListener.onCancel -= OnCancel;
    }

    void OnImageSelect(string imgPath, ImageOrientation imgOrientation)
    {
        //Debug.Log("Image Location 0: " + imgPath);        
    }

    void OnImageLoad(string imgPath, Texture2D tex, ImageOrientation imgOrientation)
    {
        //Debug.Log("Image Location 1: " + imgPath);
        //pathText.text = imgPath;

        ProfileModification.instance.profileImageRaw.texture = tex;

        //if (null == tex)
        //{
        //    Debug.Log("tex is null");
        //}

        //pathText.text = "Creating Sprite";

        Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);

        ProfileModification.instance.profileImage.sprite = sprite;

        profileImage.sprite = sprite;
        profileImagePath = imgPath;

        //pathText.text = "Going To Upload profile from " + profileImagePath;
        UploadProfileImage();

        //if(null== ProfileModification.instance)
        //{
        //    Debug.Log("profile modification is null");
        //}
        //else
        //{
        //    ProfileModification.instance.profileImage.sprite = sprite;
        //    ProfileModification.instance.profileImagePath = imgPath;
        //}
        //ProfileScreenUiManager.instance.avtar.sprite = sprite;

        //OnCloseSelectFrom();
    }

    void OnError(string errorMsg)
    {
        //Debug.Log("Error : " + errorMsg);
    }

    void OnCancel()
    {
        //Debug.Log("Cancel by user");
    }
    #endregion
}
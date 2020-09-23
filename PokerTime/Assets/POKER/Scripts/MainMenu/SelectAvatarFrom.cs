using DG.Tweening.Plugins.Core.PathCore;
using ImageAndVideoPicker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SelectAvatarFrom : MonoBehaviour
{
    private string path;

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

    void OnImageSelect(string imgPath, ImageAndVideoPicker.ImageOrientation imgOrientation)
    {
        Debug.Log("Image Location : " + imgPath);        
    }

    void OnImageLoad(string imgPath, Texture2D tex, ImageAndVideoPicker.ImageOrientation imgOrientation)
    {
        Debug.Log("Image Location : " + imgPath);  
        ProfileScreenUiManager.instance.avtar.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
    }

    void OnError(string errorMsg)
    {
        Debug.Log("Error : " + errorMsg);       
    }

    void OnCancel()
    {
        Debug.Log("Cancel by user");       
    }
    #endregion

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
        AndroidPicker.BrowseImage(true);
#elif UNITY_EDITOR
        path = EditorUtility.OpenFilePanel("Ovrewrite with jpg", "", "");
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
        StartCoroutine(UploadImage());
    }

    private IEnumerator UploadImage()
    {
        Texture2D newTexture = new Texture2D(ProfileScreenUiManager.instance.avtar.mainTexture.width, ProfileScreenUiManager.instance.avtar.mainTexture.height);
        newTexture.LoadRawTextureData(newTexture.GetRawTextureData());
        newTexture.Apply();

        byte[] bytes = newTexture.EncodeToPNG();
        Destroy(newTexture);

        var form = new WWWForm();
        form.AddField("userId", PlayerManager.instance.GetPlayerGameData().userId);
        form.AddField("userName", PlayerManager.instance.GetPlayerGameData().userName);
        form.AddField("language", "hindi");
        form.AddBinaryData("profileImage", bytes, path, "image/jpg");

        UnityWebRequest www = UnityWebRequest.Post("http://3.17.201.78:3000/createForum", form);

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
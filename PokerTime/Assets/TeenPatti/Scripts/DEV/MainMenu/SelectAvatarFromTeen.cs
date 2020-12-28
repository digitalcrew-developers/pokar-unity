using DG.Tweening.Plugins.Core.PathCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using ImageAndVideoPicker;
using UnityEngine.UI;
using UnityEditor;

public class SelectAvatarFromTeen : MonoBehaviour
{
    public void OnCloseSelectFrom()
    {
        Destroy(gameObject);
    }
    public void OnClickDefaultbtn()
    {
        MainMenuControllerTeen.instance.ShowScreen(MainMenuScreensTeen.ChangeProfileIcon);
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
            ProfileScreenUiManagerTeen.instance.avtar.sprite = sprite;
            
            /*ProfileImage.texture = www.texture;*/
        }
#endif   
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

    void OnImageSelect(string imgPath, ImageAndVideoPicker.ImageOrientation imgOrientation)
    {
        Debug.Log("Image Location 0: " + imgPath);        
    }

    void OnImageLoad(string imgPath, Texture2D tex, ImageAndVideoPicker.ImageOrientation imgOrientation)
    {
        Debug.Log("Image Location 1: " + imgPath);
        ProfileModification.instance.profileImageRaw.texture = tex;

        if (null == tex)
        {
            Debug.Log("tex is null");
        }
        Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);

        if(null== ProfileModification.instance)
        {
            Debug.Log("profile modification is null");
        }
        else
        {
            ProfileModification.instance.profileImage.sprite = sprite;
            ProfileModification.instance.profileImagePath = imgPath;
        }
        ProfileScreenUiManagerTeen.instance.avtar.sprite = sprite;

        OnCloseSelectFrom();        
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
}
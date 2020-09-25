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
        //Debug.Log("Image Location : " + imgPath);        
    }

    void OnImageLoad(string imgPath, Texture2D tex, ImageAndVideoPicker.ImageOrientation imgOrientation)
    {
        //Debug.Log("Image Location : " + imgPath);

        ProfileModification.instance.profileImagePath = imgPath;
        ProfileModification.instance.pathText.text = imgPath;

        Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
        ProfileModification.instance.profileImage.sprite = sprite;

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
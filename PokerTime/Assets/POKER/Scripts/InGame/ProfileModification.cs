using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ProfileModification : MonoBehaviour
{
    public static ProfileModification instance = null;

    public RawImage profileImageRaw;
    public Image profileImage,countryimage,frameImage;
    
    public Text nickNameHeader, countryRegion;
    
    public InputField NickNameinputField;
    public string countrycode;
    public int frameId,avtarid;

    //DEV_CODE
    public string profileImagePath;

    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
        }
    }

    public void Start()
    {
        nickNameHeader.text = ProfileScreenUiManager.instance.userName.text;
        NickNameinputField.text = ProfileScreenUiManager.instance.userName.text;
        countryRegion.text = ProfileScreenUiManager.instance.countryname;
        countrycode = ProfileScreenUiManager.instance.countrycode;
        avtarid = ProfileScreenUiManager.instance.avtarid;
        StartCoroutine(loadSpriteImageFromUrl(ProfileScreenUiManager.instance.avtarurl, profileImage));
        StartCoroutine(loadSpriteImageFromUrl(ProfileScreenUiManager.instance.flagurl, countryimage));
        StartCoroutine(loadSpriteImageFromUrl(ProfileScreenUiManager.instance.frameurl, frameImage));
    }
    IEnumerator loadSpriteImageFromUrl(string URL,Image image)
    {
        WWW www = new WWW(URL);
        while (!www.isDone)
        {
            yield return null;
        }
        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.Log("Download failed"+image.gameObject.name);
        }
        else
        {
            Texture2D texture = new Texture2D(1, 1);
            www.LoadImageIntoTexture(texture);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            image.sprite = sprite;
        }
    }
    public void CloseBtnProfilemodi()
    {
        MainMenuController.instance.DestroyScreen(MainMenuScreens.ProfileModification);
        //Destroy(this.gameObject);
    }
    public void ChangeFrameBtn()
    {
        MainMenuController.instance.ShowScreen(MainMenuScreens.ChangeFrame);

    }
    public void OnClickEditBtn()
    {
        MainMenuController.instance.ShowScreen(MainMenuScreens.SelectRegion);

    }
    public void OnclikeProfileicon()
    {
        MainMenuController.instance.ShowScreen(MainMenuScreens.SelectFrom);

    }
    public void OnClickConfirmbtn()
    {
        string requestData = "{\"userId\":\"" + PlayerManager.instance.GetPlayerGameData().userId + "\"," +
                             "\"soundToggle\":\"" + "1" + "\"," +
                              "\"nickName\":\"" + NickNameinputField.text + "\"," +
                               "\"countryCode\":\"" + countrycode + "\"," +
                             "\"frameID\":\"" + "1" + "\"," +
                              "\"avatarID\":\"" + avtarid + "\"}";

        WebServices.instance.SendRequest(RequestType.UpdateUserSettings, requestData, true, OnServerResponseFound);

        StartCoroutine(UploadImage());
    }
    

    private IEnumerator UploadImage()
    {
        RenderTexture rTexture = new RenderTexture(profileImageRaw.texture.width, profileImageRaw.texture.height, 24, RenderTextureFormat.ARGB32);
        rTexture.Create();
        Graphics.Blit(profileImageRaw.texture, rTexture, new Vector2(1, 1), new Vector2(0, 0));

        Texture2D newTexture = new Texture2D(rTexture.width, rTexture.height);
        newTexture.ReadPixels(new Rect(0, 0, rTexture.width, rTexture.height), 0, 0);
        newTexture.Apply();

        byte[] bytes = newTexture.EncodeToPNG(); //Can also encode to jpg, just make sure to change the file extensions down below
                                                 //Destroy(tex);

        Debug.Log("byte array length : " + bytes.Length);
        var form = new WWWForm();
        form.AddField("userId", PlayerManager.instance.GetPlayerGameData().userId);
        form.AddField("userName", PlayerManager.instance.GetPlayerGameData().userName);
        form.AddField("language", "hindi");
        form.AddBinaryData("profileImage", bytes, profileImagePath, "image/jpg");

        UnityWebRequest www = UnityWebRequest.Post("http://3.17.201.78:3000/updateProfile", form);

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error.ToString());
        }
        else
        {
            Debug.Log("Form upload complete! and Response: " + www.downloadHandler.text);
        }
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
        if (requestType == RequestType.UpdateUserSettings)
        {
            JsonData data = JsonMapper.ToObject(serverResponse);

            if (data["success"].ToString() == "1")
            {
                if (requestType == RequestType.UpdateUserSettings)
                {
                    Debug.Log("Success data send");
                    //   ProfileScreenUiManager.instance.GetProfileURLs(PlayerManager.instance.GetPlayerGameData().userId);

                    MainMenuController.instance.DestroyScreen(MainMenuScreens.ProfileModification);
                    //MainMenuController.instance.DestroyScreen(MainMenuScreens.Profile);
                    //MainMenuController.instance.ShowScreen(MainMenuScreens.Profile);
                }
                MainMenuController.instance.OnClickOnButton("profile");
            }
            else
            {
                //MainMenuController.instance.ShowMessage(data["message"].ToString());
            }
        }
    }
}

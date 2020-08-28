using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProfileModification : MonoBehaviour
{
    public static ProfileModification instance;

    public Image profileImage,countryimage,frameImage;
    
    public Text nickNameHeader, countryRegion;
    
    public InputField NickNameinputField;
    public string countrycode;
    public int frameId,avtarid;
   
    public void Start()
    {
        instance = this;
        nickNameHeader.text = ProfileScreenUiManager.instance.userName.text;
        NickNameinputField.text = ProfileScreenUiManager.instance.userName.text;
        StartCoroutine(loadSpriteImageFromUrl(PlayerManager.instance.GetPlayerGameData().avatarURL, profileImage));
        StartCoroutine(loadSpriteImageFromUrl(PlayerManager.instance.GetPlayerGameData().CountryURL, countryimage));
        StartCoroutine(loadSpriteImageFromUrl(PlayerManager.instance.GetPlayerGameData().FrameUrl, frameImage));
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
                              "\"nickName\":\"" + NickNameinputField.text+ "\"," +
                               "\"countryCode\":\"" + countrycode + "\"," +
                             "\"frameID\":\"" + "1" + "\"," +
                              "\"avatarID\":\"" + avtarid + "\"}";
        
        WebServices.instance.SendRequest(RequestType.UpdateUserSettings, requestData, true,OnServerResponseFound);
        MainMenuController.instance.DestroyScreen(MainMenuScreens.ProfileModification);
        MainMenuController.instance.DestroyScreen(MainMenuScreens.Profile);
        MainMenuController.instance.ShowScreen(MainMenuScreens.Profile);
        //PlayerManager.instance.GetPlayerGameData().userName

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

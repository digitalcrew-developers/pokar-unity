using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProfileScreenUiManager : MonoBehaviour
{
    public static ProfileScreenUiManager instance;
    public Image avtar, frame;
    public string countrycode, countryname;
    public string avtarurl, flagurl, frameurl;
    public int avtarid;
    public void Awake()
    {
        instance = this;
    }
    public Text coinsText, diamondsText, pointsText, userName, userId, userLevel;

    void Start()
    {
        PlayerGameDetails playerData = PlayerManager.instance.GetPlayerGameData();
        coinsText.text = Utility.GetTrimmedAmount("" + playerData.coins);
        diamondsText.text = Utility.GetTrimmedAmount("" + playerData.diamonds);
        pointsText.text = Utility.GetTrimmedAmount("" + playerData.points);


        GetProfileURLs(playerData.userId);
    }
    public void GetProfileURLs(string playerid)
    {
        WebServices.instance.SendRequest(RequestType.GetUserDetails, "{\"userId\":\"" + playerid + "\"}", true, OnServerResponseFound);
    }

    public void OnServerResponseFound(RequestType requestType, string serverResponse, bool isShowErrorMessage, string errorMessage)
    {
        if (errorMessage.Length > 0)
        {
            if (isShowErrorMessage)
            {
                //  Debug.LogError("111111111111111111111111111111");
                MainMenuController.instance.ShowMessage(errorMessage);
            }
            return;
        }
        if (requestType == RequestType.GetUserDetails)
        {
            JsonData data = JsonMapper.ToObject(serverResponse);

            if (data["success"].ToString() == "1")
            {
                for (int i = 0; i < data["getData"].Count; i++)
                {

                    loadImages(data["getData"][i]["profileImage"].ToString(), data["getData"][i]["frameURL"].ToString());
                    userLevel.text = "Lvl. " + data["getData"][i]["userLevel"].ToString() + ">>";
                    userName.text = data["getData"][i]["userName"].ToString();
                    userId.text = "UserID:" + data["getData"][i]["userId"].ToString();
                    countrycode = data["getData"][i]["countryCode"].ToString();
                    countryname = data["getData"][i]["countryName"].ToString();
                    avtarurl = data["getData"][i]["profileImage"].ToString();
                    frameurl = data["getData"][i]["frameURL"].ToString();
                    flagurl = data["getData"][i]["countryFlag"].ToString();
                    avtarid = int.Parse(data["getData"][i]["avatarID"].ToString());
                }
                MainMenuController.instance.OnClickOnButton("profile");
            }
            else
            {
                MainMenuController.instance.ShowMessage(data["message"].ToString());
            }
        }
    }
    public void loadImages(string urlAvtar,string urlframe)
    {
     //   Debug.Log("Success data send");
        StartCoroutine(loadSpriteImageFromUrl(urlAvtar, avtar));
        StartCoroutine(loadSpriteImageFromUrl(urlframe, frame));
    }
    IEnumerator loadSpriteImageFromUrl(string URL, Image image)
    {
        WWW www = new WWW(URL);
        while (!www.isDone)
        {
            yield return null;
        }
        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.Log("Download failed" + image.gameObject.name);
        }
        else
        {
      //  Debug.Log("Success222222222 data send");
            Texture2D texture = new Texture2D(1, 1);
            www.LoadImageIntoTexture(texture);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            image.sprite = sprite;
        }
    }

    public void OnClickOnButton(string eventName)
    {
        SoundManager.instance.PlaySound(SoundType.Click);

        switch (eventName)
        {
            case "coinShop":
                {
                    MainMenuController.instance.ShowScreen(MainMenuScreens.Shop);
                }
                break;

            case "diamondShop":
                {
                    MainMenuController.instance.ShowScreen(MainMenuScreens.Shop, new object[] { "diamond" });
                }
                break;

            case "vip":
                {
                    //TODO Show VIP cards Screen
                }
                break;
            case "profilemodificataion":
                {
                    MainMenuController.instance.ShowScreen(MainMenuScreens.ProfileModification);

                }
                break;
            case "Settings":
                {
                    MainMenuController.instance.ShowScreen(MainMenuScreens.ProfileSetting);
                }
                break;
            case "AboutUS":
                {
                    MainMenuController.instance.ShowScreen(MainMenuScreens.AboutUs);
                }
                break;
            case "Feedback":
                {
                    Application.OpenURL("https://www.lipsum.com/");
                }
                break;
            case "Tutorial":
                {
                    Application.OpenURL("https://www.lipsum.com/");
                }
                break;

            default:
#if ERROR_LOG
            Debug.LogError("unhdnled eventName found in menuHandller = " + eventName);
#endif
            break;
        }
    }

}

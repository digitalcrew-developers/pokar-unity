using LitJson;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpinWheelUIManagerTeenPatti : MonoBehaviour
{
    public static SpinWheelUIManagerTeenPatti instance;
    public GameObject oneXBtn,fiveXBtn;

    public GameObject ImgGetContainer;
    public GameObject TextGetContainer;
    public GameObject spinWheel;
    public GameObject drawOutput;
    public Image draw1xOutputImg;
    public Text draw1xOutputText;
    public Image[] draw5xOutputImg;
    public Text[] draw5xOutputText;

    public Image spinWheelImage;
    public Sprite[] spinWheelSprites;
    public Sprite[] drawButtonSprites;

    public Text winnerListFirstTxt;
    public Text winnerListSecondTxt;
    public GameObject winnerListItemPrefabs;
    public GameObject bottomWinnerListContainer;

    public string eventValue;

    public void Awake()
    {
        instance = this;
    }
    void Start()
    {
        GetLuckyDrawAvatars(); 
        GetTextItemsList();

        FetchDataGetSpinWinnerList();
    }

    private void OnEnable()
    {
        drawOutput.SetActive(false);
    }


    public void FetchDataGetSpinWinnerList()
    {

        string requestData = null;
        //WebServices.instance.SendRequestTP(RequestTypeTP.getSpinWinnerList, requestData, true, OnServerResponseFound);

    }


    public void OnServerResponseFound(RequestTypeTP requestType, string serverResponse, bool isShowErrorMessage, string errorMessage)
    {

        Debug.Log("IIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIII");
        MainMenuControllerTeen.instance.DestroyScreen(MainMenuScreensTeen.Loading);

        if (errorMessage.Length > 0)
        {
            if (isShowErrorMessage)
            {
                MainMenuControllerTeen.instance.ShowMessage(errorMessage);
            }

            return;
        }
        //if (requestType == RequestTypeTP.getSpinWinnerList)
        //{
        //    JsonData data = JsonMapper.ToObject(serverResponse);
        //    if (data["success"].ToString() == "1")
        //    {

        //        Debug.Log("i OOOOOOOOOOOOOOOOOOO   jkjkjkjkjkjjk " + data["response"].Count);
        //        for (int i = 0; i < data["response"].Count; i++)
        //        {
        //            string str="";

        //            if (data["response"][i]["nickName"] != null)
        //            {
        //                str = str+ data["response"][i]["nickName"] + " has won Coins ";
                        
        //            }
        //            else
        //            {
        //                str = str + " -- " + " has won Coins ";
        //            }

        //            if (data["response"][i]["coins"] != null)
        //            {
        //                str = str + data["response"][i]["coins"] + " in ";
        //            }
        //            else
        //            {
        //                str = str + "--" + " in ";
        //            }

        //            if (data["response"][i]["gameType"] != null)
        //            {
        //                str = str + data["response"][i]["gameType"] ;
        //            }
        //            else {
        //                str = str + " -- " ;
        //            }

        //            if (data["response"][i]["smallBlind"] != null)
        //            {
        //                str = str + " " + data["response"][i]["smallBlind"]+"/";
        //            }
        //            else
        //            {
        //                str = str + " -- " + "/";
        //            }

        //            if (data["response"][i]["bigBlind"] != null)
        //            {
        //                str = str +" "+ data["response"][i]["bigBlind"] + " ";
        //            }
        //            else
        //            {
        //                str = str + " -- " ;
        //            }

        //            //Debug.Log("Now the STR of value ---  " + str);

        //            if (i == 0)
        //            {
        //                winnerListFirstTxt.text = str;
        //            }
        //            else if (i == 1)
        //            {
        //                winnerListSecondTxt.text = str;
        //            }
        //            else {
        //                GameObject g = Instantiate(winnerListItemPrefabs, bottomWinnerListContainer.transform) as GameObject;
        //                g.transform.SetParent(bottomWinnerListContainer.transform);
        //                g.transform.GetComponent<Text>().text = str;
        //            }
                   
        //        }
        //        //ShowSpinWheelContent(data);

        //    }
        //    else
        //    {
        //        MainMenuControllerTeen.instance.ShowMessage(data["message"].ToString());
        //    }
        //}
        //else if (requestType == RequestTypeTP.deductFromWallet)
        //{
        //    JsonData data = JsonMapper.ToObject(serverResponse);

        //    if (data["success"].ToString() == "1")
        //    {
        //        //ShowSpinWheelContent(data);
        //        Debug.Log("Successfully deduct amount");
        //    }
        //    else
        //    {
        //        MainMenuControllerTeen.instance.ShowMessage(data["message"].ToString());
        //    }
        //}
    }

    public void UpdateUserBalance(PlayerGameDetails updatedData)
    {
        string requestData = "{\"userId\":\"" + PlayerManager.instance.GetPlayerGameData().userId + "\"," +
            "\"silver\":\"0\"," +
            "\"coins\":\"" + (int)updatedData.coins + "\"," +
            "\"points\":\"" + (int)updatedData.points + "\"," +
            "\"diamond\":\"" + (int)updatedData.diamonds + "\"," +

            "\"rabbit\":\"0\"," +
            "\"emoji\":\"0\"," +
            "\"time\":\"0\"," +
            "\"day\":\"0\"," +
            "\"playerProgress\":\"\"}";

        //MainMenuControllerTeen.instance.ShowScreen(MainMenuScreensTeen.Loading);

        //WebServices.instance.SendRequestTP(RequestTypeTP.UpdateUserBalance, requestData, true, (requestType, serverResponse, isShowErrorMessage, errorMessage) =>
        //{
        //    //MainMenuControllerTeen.instance.DestroyScreen(MainMenuScreensTeen.Loading);

        //    if (errorMessage.Length > 0)
        //    {
        //        //MainMenuControllerTeen.instance.ShowMessage(errorMessage);
        //    }
        //    else
        //    {
        //        JsonData data = JsonMapper.ToObject(serverResponse);
        //        if (data["status"].Equals(true))
        //        {
        //            PlayerManager.instance.SetPlayerGameData(updatedData);
        //            //UpdateAlltext(updatedData);
        //        }
        //        else
        //        {
        //            //MainMenuControllerTeen.instance.ShowMessage(data["message"].ToString());
        //        }
        //    }
        //});
    }


    public void GetLuckyDrawAvatars() {
        for (int i = 0; i < ImgGetContainer.transform.childCount; i++)
        {
            StartCoroutine(loadSpriteImageFromUrl(SpinManagerTeenPatti.instance.spinItemList[i].itemIcon, ImgGetContainer.transform.GetChild(i).GetComponent<Image>()));
        }
        }
    public void GetTextItemsList()
    {
        for (int i = 0; i < ImgGetContainer.transform.childCount; i++)
        {
            TextGetContainer.transform.GetChild(i).GetComponent<TextMeshProUGUI>().text =" X "+ SpinManagerTeenPatti.instance.spinItemList[i].itemMultipler;
        }
    }
    IEnumerator loadSpriteImageFromUrl(string URL, Image image)
    {

        WWW www = new WWW(URL);
        while (!www.isDone)
        {
            //     Debug.Log("Download image on progress" + www.progress);
            yield return null;
        }

        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.LogError("Download failed");
        }
        else
        {
          //  Debug.Log("Image url is : " + URL + "            name  => " + image.gameObject.name);
            Texture2D texture = new Texture2D(1, 1);
            www.LoadImageIntoTexture(texture);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            image.sprite = sprite;
        }
    }

    public void OnClickOnButton(string eventName)
    {
        SoundManager.instance.PlaySound(SoundType.Click);
        drawOutput.SetActive(false);
        drawOutput.transform.GetChild(0).gameObject.SetActive(false);
        drawOutput.transform.GetChild(1).gameObject.SetActive(false);

        switch (eventName)
        {
            case "back":
                {

                    InGameUiManagerTeenPatti.instance.DestroyScreen(InGameScreensTeenPatti.SpinWheelScreen);                   

                }
                break;
            case "1x":
                {
                    eventValue = "1x";
                    Debug.Log("Table ID OF Player----------  ");
                     if (PlayerManager.instance.GetPlayerGameData().coins > 1500)
                    {
                        spinWheel.SetActive(true);
                    }
                    else 
                    {
                        InGameUiManagerTeenPatti.instance.DestroyScreen(InGameScreensTeenPatti.SpinWheelScreen);

                        InGameUiManagerTeenPatti.instance.ShowScreen(InGameScreensTeenPatti.InGameShop);
                    }
                    DeductCoinPostServer(1500);
                }
                break;
            case "5x":
                {
                    eventValue = "5x";
                    if (PlayerManager.instance.GetPlayerGameData().coins > 4800)
                    {
                        spinWheel.SetActive(true);
                    }
                    else
                    {
                        InGameUiManagerTeenPatti.instance.DestroyScreen(InGameScreensTeenPatti.SpinWheelScreen);

                        InGameUiManagerTeenPatti.instance.ShowScreen(InGameScreensTeenPatti.InGameShop);
                    }
                    DeductCoinPostServer(4800);
                }
                break;

            default:
                {
                    Debug.LogError("Unhandled eventName found in MissionsUiManager = " + eventName);
                }
                break;
        }
    }

    void DeductCoinPostServer(int val) {

        int amount = val;

        PlayerGameDetails playerData = PlayerManager.instance.GetPlayerGameData();
        playerData.coins -= amount;
        UpdateUserBalance(playerData);

        //string requestData = "{\"userId\":\"" + PlayerManager.instance.GetPlayerGameData().userId + "\"," +
        //                      "\"amount\":\"" + amount + "\"," +
        //                      "\"deductFrom\":\"" + "coins" + "\"," +
        //                       "\"narration\":\"" + "Spin Wheel"+ "\"}";
        //WebServices.instance.SendRequestTP(RequestTypeTP.deductFromWallet, requestData, true, OnServerResponseFound);        
    }
}

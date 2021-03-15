using LitJson;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpinManager : MonoBehaviour
{
    public static SpinManager instance;
    public Transform Container;
    public GameObject InactiveSpinRotation;
    [SerializeField]
    public List<SpinItemData> spinItemList;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        spinItemList = new List<SpinItemData>(); 
        FetchDataSpinWheel();
    }

    public void FetchDataSpinWheel()
    {

        //string requestData = "{\"PRADEEP\":\"" + "VIVEK" + "\"}";
        WebServices.instance.SendRequest(RequestType.GetSpinWheelItems, /*requestData*/"{}", true, OnServerResponseFound);

    }


    public void SpinBtnClick()
    {
        InGameUiManager.instance.spinWheel.SetActive(true);
        InactiveSpinRotation.SetActive(false);
        DeductCoinPostServer(10000);
    }

    void DeductCoinPostServer(int val)
    {

        int amount = val;

        string requestData = "{\"userId\":\"" + PlayerManager.instance.GetPlayerGameData().userId + "\"," +
                              "\"amount\":\"" + amount + "\"," +
                              "\"deductFrom\":\"" + "coins" + "\"," +
                               "\"narration\":\"" + "Spin Wheel" + "\"}";
        WebServices.instance.SendRequest(RequestType.deductFromWallet, requestData, true, OnServerResponseFound);
    }
    public void SetSpinWheelWinning(int index)
    {
        int itemid = int.Parse(spinItemList[index].itemID);
        Debug.Log("VALUE OF ITEM--->  "+itemid);

        string requestData = "{\"userId\":\"" + PlayerManager.instance.GetPlayerGameData().userId + "\"," +
                             "\"itemID\":\"" + itemid + "\"," +
                             "\"gameType\":\"" + GlobalGameManager.instance.currentRoomData.gameMode + "\"," +
                             "\"smallBlind\":\"" + GlobalGameManager.instance.currentRoomData.smallBlind + "\"," +
                              "\"bigBlind\":\"" + GlobalGameManager.instance.currentRoomData.smallBlind + "\"}";
        WebServices.instance.SendRequest(RequestType.SetSpinWheelWinning, requestData, true, OnServerResponseFound);
    }


    public void OnServerResponseFound(RequestType requestType, string serverResponse, bool isShowErrorMessage, string errorMessage)
    {

        Debug.Log("IIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIIII");
        MainMenuController.instance.DestroyScreen(MainMenuScreens.Loading);

        if (errorMessage.Length > 0)
        {
            if (isShowErrorMessage)
            {
                MainMenuController.instance.ShowMessage(errorMessage);
                Debug.Log("Error: " + errorMessage);
            }

            return;
        }
        if (requestType == RequestType.GetSpinWheelItems)
        {
            Debug.Log("Response => GetSpinWheelItems: " + serverResponse);
            JsonData data = JsonMapper.ToObject(serverResponse);

            if (data["success"].ToString() == "1")
            {
                ShowSpinWheelContent(data);
            }
            else
            {
                MainMenuController.instance.ShowMessage(data["message"].ToString());
            }
        }
        if (requestType == RequestType.SetSpinWheelWinning)
        {
            JsonData data = JsonMapper.ToObject(serverResponse);

            if (data["success"].ToString() == "1")
            {
                Debug.Log("********SET THE VALUE OF SPIN WHEEL" );
            }
            else
            {
                MainMenuController.instance.ShowMessage(data["message"].ToString());
            }
        }
        if (requestType == RequestType.deductFromWallet)
        {
            JsonData data = JsonMapper.ToObject(serverResponse);

            if (data["success"].ToString() == "1")
            {
                Debug.Log("*******000000000*SET THE VALUE OF SPIN WHEEL");
            }
            else
            {
                MainMenuController.instance.ShowMessage(data["message"].ToString());
            }
        }
    }

    public void ShowSpinWheelContent(JsonData data)
    {
        
        for (int i = 0; i < Container.childCount; i++)
        {
            Container.GetChild(i).GetComponent<TextMeshProUGUI>().text="";
        }

        
        for (int i = 0; i < Container.childCount; i++)
        {
           SpinItemData sp = new SpinItemData();
            sp.itemID = data["getData"][i]["itemID"].ToString();
            sp.itemName = data["getData"][i]["itemName"].ToString();
            sp.itemValue = data["getData"][i]["itemValue"].ToString();
            sp.itemMultipler = data["getData"][i]["itemMultipler"].ToString();
            sp.itemIcon = data["getData"][i]["itemIcon"].ToString();

            
            if (sp.itemIcon != null)
            {
               StartCoroutine(loadSpriteImageFromUrl(sp.itemIcon, Container.GetChild(i).GetChild(0).GetComponent<Image>()));
            }

            spinItemList.Add(sp);
            //Container.GetChild(i).GetComponent<TextMeshProUGUI>().text = data["getData"][i]["itemValue"].ToString()+" x "+ data["getData"][i]["itemMultipler"].ToString();
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
      //      Debug.Log("Image url is : " + URL + "            name  => " + image.gameObject.name);
            Texture2D texture = new Texture2D(1, 1);
            www.LoadImageIntoTexture(texture);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            image.sprite = sprite;
        }
    }
}


[System.Serializable]
public class SpinItemData
{    
    public string itemID;
    public string itemName;
    public string itemValue;
    public string itemMultipler;
    public string itemIcon;
    
}
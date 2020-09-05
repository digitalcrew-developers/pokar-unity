using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpinWheelUIManager : MonoBehaviour
{
    public static SpinWheelUIManager instance;
    public GameObject oneXBtn,fiveXBtn;

    public GameObject ImgGetContainer;
    public GameObject TextGetContainer;
    public GameObject spinWheel;
    public Image draw1xOutput;


    public void Awake()
    {
        instance = this;
    }
    void Start()
    {
        GetLuckyDrawAvatars(); 
        GetTextItemsList(); 
    }


    public void ShowSpinWheelOutPut(int val) {
        draw1xOutput.transform.parent.gameObject.SetActive(false);
        draw1xOutput = ImgGetContainer.transform.GetChild(val).GetComponent<Image>();
        

    }

    public void GetLuckyDrawAvatars() {
        for (int i = 0; i < ImgGetContainer.transform.childCount; i++)
        {
            StartCoroutine(loadSpriteImageFromUrl(SpinManager.instance.spinItemList[i].itemIcon, ImgGetContainer.transform.GetChild(i).GetComponent<Image>()));
        }
        }
    public void GetTextItemsList()
    {
        for (int i = 0; i < ImgGetContainer.transform.childCount; i++)
        {
            TextGetContainer.transform.GetChild(i).GetComponent<TextMeshProUGUI>().text =" X "+ SpinManager.instance.spinItemList[i].itemMultipler;
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

        switch (eventName)
        {
            case "back":
                {
                    
                        InGameUiManager.instance.DestroyScreen(InGameScreens.SpinWheelScreen);
                  

                }
                break;
            case "1x":
                {
                    Debug.Log("PLAYER GAMEDATA COIN_______   "+ (PlayerManager.instance.GetPlayerGameData().coins > 150));
                     if (PlayerManager.instance.GetPlayerGameData().coins > 150)
                    {
                        spinWheel.SetActive(true);
                    }
                    else {
                        InGameUiManager.instance.ShowScreen(InGameScreens.InGameShop);
                    }
                   // 
                }
                break;
            case "5x":
                {
                    
                    InGameUiManager.instance.DestroyScreen(InGameScreens.SpinWheelScreen);
                    if (PlayerManager.instance.GetPlayerGameData().coins < 480)
                    {
                        spinWheel.SetActive(true);
                    }
                    else
                    {
                        InGameUiManager.instance.ShowScreen(InGameScreens.InGameShop);
                    }
                }
                break;

            default:
                {
                    Debug.LogError("Unhandled eventName found in MissionsUiManager = " + eventName);
                }
                break;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmojiUIScreenManager : MonoBehaviour
{

    public static EmojiUIScreenManager instance;

    public GameObject[] containerAry;

    public int containerVal;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        ShowContainer(containerVal);
    }

    void ShowContainer(int val)
    {
        for (int i = 0; i < containerAry.Length; i++)
        {
            if (i == val)
            {
                containerAry[i].SetActive(true);
            }
            else
            {
                containerAry[i].SetActive(false);
            }
        }
    }
   
    public void SelectEmojiButton(string str)
    {
        SoundManager.instance.PlaySound(SoundType.Click);

        int emojiIndex = 0;
        Debug.Log("Here Get The emoji name which show ---  " + str);

        switch (str)
        {
            case "bluffing":
                emojiIndex = 0;
                break;
            case "youRaPro":
                emojiIndex = 1;
                break;
            case "beerCheers":
                emojiIndex = 2;
                break;
            case "murgi":
                emojiIndex = 3;
                break;
            case "rocket":
                emojiIndex = 4;
                break;
            case "dung":
                emojiIndex = 5;
                break;
            case "oscar":
                emojiIndex = 6;
                break;
            case "donkey":
                emojiIndex = 7;
                break;
            case "thumbUp":
                emojiIndex = 8;
                break;
            case "cherees":
                emojiIndex = 9;
                break;
            case "kiss":
                emojiIndex = 10;
                break;
            case "fish":
                emojiIndex = 11;
                break;
            case "gun":
                emojiIndex = 12;
                break;
        }
        //InGameUiManager.instance.emojiIndex = emojiIndex;
        // InGameUiManager.instance.ShowEmojiOnScreen(str);
        InGameUiManager.instance.CallEmojiSocket(emojiIndex);
        OnClickOnButton("back");
    }


    public void OnClickOnButton(string eventName)
    {
        SoundManager.instance.PlaySound(SoundType.Click);

        switch (eventName)
        {
            case "back":
                {
                    InGameUiManager.instance.DestroyScreen(InGameScreens.EmojiScreen);


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

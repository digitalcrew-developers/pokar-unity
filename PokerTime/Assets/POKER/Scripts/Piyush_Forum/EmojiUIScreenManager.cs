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

    void ShowContainer(int val) {
        for (int i = 0; i < containerAry.Length; i++)
        {
            if (i == val)
            {
                containerAry[i].SetActive(true);
            }
            else {
                containerAry[i].SetActive(false);
            }
        }    
    }

    public void SelectEmojiButton(string str) {
        SoundManager.instance.PlaySound(SoundType.Click);

       // Debug.Log("Here Get The emoji name which show ---  "+str);
        InGameUiManager.instance.ShowEmojiOnScreen(str);
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

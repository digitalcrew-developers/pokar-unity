using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DealerImgUIScreenManager : MonoBehaviour
{
    public static DealerImgUIScreenManager instance;
    public Text selectPageTxt;

    public void Awake()
    {
        instance = this;
    }
    public void BackBtn()
    {
        InGameUiManager.instance.DestroyScreen(InGameScreens.DealerImageScreen);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        selectPageTxt.text = ScrollSnapRect.instance._currentPage+1+"/2";
    }
}

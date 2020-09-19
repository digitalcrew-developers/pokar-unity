using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CareerManager : MonoBehaviour
{
    public static CareerManager instance;
    public int selectedIndex_CareerMenuScreen = 0;

    public Text headingTxt;
    public GameObject[] DMY_objList;
    public GameObject[] DMY_objfocus;

    private void Awake()
    {
        instance = this;
    }
    public void OnMenuBtnClick()
    {
        MainMenuController.instance.ShowScreen(MainMenuScreens.CareerMenuScreen);
    }

    public void OnDataBtnClick()
    {
        MainMenuController.instance.ShowScreen(MainMenuScreens.CareerDataScreen);
    }

    public void OnDMY_BtnClick(string val) {
        for (int i = 0; i < DMY_objList.Length; i++)
        {
            DMY_objList[i].SetActive(false);
            DMY_objfocus[i].SetActive(false);
        }
        switch (val)
        {
            case "day":
                DMY_objList[0].SetActive(true);
                DMY_objfocus[0].SetActive(true);

                break;
            case "month":
                DMY_objList[1].SetActive(true);
                DMY_objfocus[1].SetActive(true);
                break;
            case "year":
                DMY_objList[2].SetActive(true);
                DMY_objfocus[2].SetActive(true);
                break;
        }
    }
}

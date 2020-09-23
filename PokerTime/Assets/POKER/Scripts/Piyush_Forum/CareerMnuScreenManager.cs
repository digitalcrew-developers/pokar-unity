using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CareerMnuScreenManager : MonoBehaviour
{
    public List<GameObject> btnList;

    
    public void OnEnable()
    {
        Debug.Log("selectedIndex---------------   " + CareerManager.instance.selectedIndex_CareerMenuScreen);
        for (int i = 0; i < btnList.Count; i++)
        {
            btnList[i].GetComponent<Image>().color = new Color32(42, 42, 42, 255);
            if (CareerManager.instance.selectedIndex_CareerMenuScreen == i)
            {
                btnList[i].GetComponent<Image>().color = new Color32(80, 180, 80, 255);
                CareerManager.instance.headingTxt.text = btnList[i].transform.GetChild(0).GetComponent<Text>().text;
            }
        }
        
    }

    public void OnCloseBtnClick() {

        MainMenuController.instance.DestroyScreen(MainMenuScreens.CareerMenuScreen);
    }

    public void OnBtnClick(int val) {
        CareerManager.instance.selectedIndex_CareerMenuScreen = val;
        for (int i = 0; i < btnList.Count; i++)
        {
            btnList[i].GetComponent<Image>().color = new Color32(42, 42, 42, 255);
            if (CareerManager.instance.selectedIndex_CareerMenuScreen == i)
            {
                btnList[i].GetComponent<Image>().color = new Color32(80, 180, 80, 255);
                CareerManager.instance.headingTxt.text = btnList[i].transform.GetChild(0).GetComponent<Text>().text;
            }
        }
        Debug.Log("selectedIndex-----0000----------   " + CareerManager.instance.selectedIndex_CareerMenuScreen);

    }
}

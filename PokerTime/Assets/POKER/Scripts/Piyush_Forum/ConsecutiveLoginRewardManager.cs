using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsecutiveLoginRewardManager : MonoBehaviour
{
    public int dayFocusVal;

    public GameObject[] FocusImgs;

    private void Start()
    {
        dayFocusVal = 1;
        //ShowFocusVal(dayFocusVal);
    }

    public void ShowFocusVal(int val)
    {
        for (int i = 0; i < FocusImgs.Length; i++)
        {
            if (i == dayFocusVal-1)
            {
                FocusImgs[i].SetActive(true);
            }
            else {
                FocusImgs[i].SetActive(false);
            }
        }
    }

    public void OnCollectBtnClick()
    {
        SoundManager.instance.PlaySound(SoundType.Congratulation);

        MainMenuController.instance.ShowScreen(MainMenuScreens.Congratulation);
        MainMenuController.instance.DestroyScreen(MainMenuScreens.ConsecutiveLoginReward);
    }
    public void OnCloseBtnClick()
    {

        MainMenuController.instance.DestroyScreen(MainMenuScreens.BackPack);
    }
}
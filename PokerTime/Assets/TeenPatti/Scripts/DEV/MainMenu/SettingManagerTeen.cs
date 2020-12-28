using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingManagerTeen : MonoBehaviour
{
    public GameObject soundon, soundOff;
    public bool issound;
    public void Start()
    {
        if (SoundManager.instance.sound)
        {
            soundOff.SetActive(false);
            soundon.SetActive(true);
        }
        else
        {
            soundOff.SetActive(true);
            soundon.SetActive(false);
        }
        issound = SoundManager.instance.sound;
        //transform.GetChild(0).GetComponent<PopupBounce>().PlayMainMenuAnimations();
    }
    public void OnCloseSetting()
    {
        MainMenuControllerTeen.instance.DestroyScreen(MainMenuScreensTeen.ProfileSetting);
    }

    public void OnClickLanguageBTN()
    {
        MainMenuControllerTeen.instance.ShowScreen(MainMenuScreensTeen.Language);
    }

    public void OnClickLinkEmailBTN()
    {
        SoundManager.instance.PlaySound(SoundType.Click);

        MainMenuControllerTeen.instance.ShowScreen(MainMenuScreensTeen.LinkYourEmail);
    }


    public void OnClickChangePwdBTN()
    {
        SoundManager.instance.PlaySound(SoundType.Click);
        MainMenuControllerTeen.instance.ShowScreen(MainMenuScreensTeen.ChangePassword);
    }


    public void OnClickRedeemCodeBTN()
    {
        Debug.Log("**********You CLICK ON RADEEM CODE");
        SoundManager.instance.PlaySound(SoundType.Click);
        MainMenuControllerTeen.instance.ShowScreen(MainMenuScreensTeen.RedeemCode);
    }


    public void OnClickLogOutBTN()
    {
        Debug.Log("*********You CLICK ON LOG OUT");
        if(null!= ClubListUiManagerTeen.instance)
        {
            ClubListUiManagerTeen.instance.CleaClubList();
        }
        SoundManager.instance.PlaySound(SoundType.Click);
        PlayerPrefs.DeleteAll();
        PlayerManager.instance.DeletePlayerGameData();
        PrefsManager.DeletePlayerData();
        GlobalGameManager.instance.isLoginShow = false;
        MainMenuControllerTeen.instance.DestroyScreen(MainMenuScreensTeen.Profile);
        MainMenuControllerTeen.instance.ShowScreen(MainMenuScreensTeen.Registration);
    }


    public void OnClickSoundBTN()
    {
        if (issound)
        {
            issound = false;
            soundOff.SetActive(true);
            soundon.SetActive(false);
            PlayerPrefs.SetString("issound", "0");
            PlayerPrefs.Save();
        }
        else
        {
            issound = true;
            soundOff.SetActive(false);
            soundon.SetActive(true);
            PlayerPrefs.SetString("issound", "1");
            PlayerPrefs.Save();
        }
        SoundManager.instance.SoundCheck();
    }
}

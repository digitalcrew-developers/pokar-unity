using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingManager : MonoBehaviour
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
        MainMenuController.instance.DestroyScreen(MainMenuScreens.ProfileSetting);
    }

    public void OnClickLanguageBTN()
    {
        MainMenuController.instance.ShowScreen(MainMenuScreens.Language);
    }

    public void OnClickLinkEmailBTN()
    {
        SoundManager.instance.PlaySound(SoundType.Click);

        MainMenuController.instance.ShowScreen(MainMenuScreens.LinkYourEmail);
    }


    public void OnClickChangePwdBTN()
    {
        SoundManager.instance.PlaySound(SoundType.Click);
        MainMenuController.instance.ShowScreen(MainMenuScreens.ChangePassword);
    }


    public void OnClickRedeemCodeBTN()
    {
        Debug.Log("**********You CLICK ON RADEEM CODE");
        SoundManager.instance.PlaySound(SoundType.Click);
        MainMenuController.instance.ShowScreen(MainMenuScreens.RedeemCode);
    }


    public void OnClickLogOutBTN()
    {
        //Debug.Log("*********You CLICK ON LOG OUT");
        if (null != ClubListUiManager.instance)
        {
            ClubListUiManager.instance.CleaClubList();
        }

        //Code to logout from social accounts if logged in with social account.
        if (PlayerManager.instance.GetPlayerGameData().registrationType.Equals("google") || PlayerManager.instance.GetPlayerGameData().registrationType.Equals("Google"))
        {
            GoogleManager.instance.SignOutFromGoogle();
        }
        else if(PlayerManager.instance.GetPlayerGameData().registrationType.Equals("facebook") || PlayerManager.instance.GetPlayerGameData().registrationType.Equals("Facebook"))
        {
            //FacebookLogin.instance.logout();
            FacebookManager.instance.SignOutFromFB();
        }

        SoundManager.instance.PlaySound(SoundType.Click);
        PlayerPrefs.DeleteAll();
        PlayerManager.instance.DeletePlayerGameData();
        PrefsManager.DeletePlayerData();
        GlobalGameManager.instance.isLoginShow = false;
        MainMenuController.instance.DestroyScreen(MainMenuScreens.Profile);
        MainMenuController.instance.ShowScreen(MainMenuScreens.Registration);
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

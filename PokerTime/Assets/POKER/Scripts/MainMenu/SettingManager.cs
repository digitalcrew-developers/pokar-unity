using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingManager : MonoBehaviour
{
    public GameObject soundon, soundOff;
    public bool issound;
    public void Start()
    {
        if(SoundManager.instance.sound)
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
    }
    public void OnCloseSetting()
    {
        MainMenuController.instance.DestroyScreen(MainMenuScreens.ProfileSetting);
    }
   
public void OnClickLanguageBTN()
    {
        MainMenuController.instance.ShowScreen(MainMenuScreens.Language);
    }
   public void OnClickSoundBTN()
    {
        if(issound)
        {
            issound = false;
            soundOff.SetActive(true);
            soundon.SetActive(false);
            PlayerPrefs.SetString("SoundOnOf", "Off");
            PlayerPrefs.Save();
        }
        else
        {
            issound = true;
            soundOff.SetActive(false);
            soundon.SetActive(true);
            PlayerPrefs.SetString("SoundOnOf", "On");
            PlayerPrefs.Save();
        }
        SoundManager.instance.SoundCheck();
    }
}

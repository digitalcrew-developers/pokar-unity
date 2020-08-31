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

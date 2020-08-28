using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageManager : MonoBehaviour
{
    public void OnCloseLanguage()
    {
        MainMenuController.instance.DestroyScreen(MainMenuScreens.Language);
    }
}

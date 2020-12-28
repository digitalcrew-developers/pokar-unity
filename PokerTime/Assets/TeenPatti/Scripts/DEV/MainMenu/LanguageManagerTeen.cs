using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageManagerTeen : MonoBehaviour
{
    public void OnCloseLanguage()
    {
        MainMenuControllerTeen.instance.DestroyScreen(MainMenuScreensTeen.Language);
    }
}

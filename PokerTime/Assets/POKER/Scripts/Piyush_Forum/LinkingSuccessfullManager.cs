using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LinkingSuccessfullManager : MonoBehaviour
{
    public Text emailTxt;

    private void Start()
    {
        emailTxt.text = LinkYourEmailManager.instance.emailInputField.text;
    }
    public void OnClose()
    {
        MainMenuController.instance.DestroyScreen(MainMenuScreens.LinkingSucessfull);
    }

}

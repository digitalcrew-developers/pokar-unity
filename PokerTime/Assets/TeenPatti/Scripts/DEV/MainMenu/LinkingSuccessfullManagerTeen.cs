using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LinkingSuccessfullManagerTeen : MonoBehaviour
{
    public Text emailTxt;

    private void Start()
    {
        emailTxt.text = LinkYourEmailManagerTeen.instance.emailInputField.text;
    }
    public void OnClose()
    {
        gameObject.SetActive(false);
        //MainMenuController.instance.DestroyScreen(MainMenuScreens.LinkingSucessfull);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AboutUsManager : MonoBehaviour
{
    public void OnClickAboutUsMethod(string name)
    {
        switch (name)
        {
            case "Close":
                {

                    MainMenuController.instance.DestroyScreen(MainMenuScreens.AboutUs);
                }
                break;
            case "FairGaming":
                {
                MainMenuController.instance.ShowScreen(MainMenuScreens.FairGaming);
                }
                break;
            case "Compliance":
                {
                           MainMenuController.instance.ShowScreen(MainMenuScreens.Compliance);
                }
                break;
            case "Contact":
                {
                    MainMenuController.instance.ShowScreen(MainMenuScreens.Contact);
                }
                break;
            case "TermsOfService":
                {
                    Application.OpenURL("https://www.lipsum.com/");
                }
                break;
            case "PrivacyPolicy":
                {
                    Application.OpenURL("https://www.lipsum.com/");
                }
                break;


        }
    }
}

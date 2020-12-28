using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AboutUsManagerTeen : MonoBehaviour
{
    public void OnClickAboutUsMethod(string name)
    {
        switch (name)
        {
            case "Close":
                {

                    MainMenuControllerTeen.instance.DestroyScreen(MainMenuScreensTeen.AboutUs);
                }
                break;
            case "FairGaming":
                {
                    MainMenuControllerTeen.instance.ShowScreen(MainMenuScreensTeen.FairGaming);
                }
                break;
            case "Compliance":
                {
                    MainMenuControllerTeen.instance.ShowScreen(MainMenuScreensTeen.Compliance);
                }
                break;
            case "Contact":
                {
                    MainMenuControllerTeen.instance.ShowScreen(MainMenuScreensTeen.Contact);
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

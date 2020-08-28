using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GlobalTournamentListUiManager : MonoBehaviour
{
    public Button recommendedButton, spinUpButton, mttButton;
    public Text coinsText;


    public void ShowScreen(string screenName = "")
    {
        coinsText.text =Utility.GetTrimmedAmount(""+PlayerManager.instance.GetPlayerGameData().coins);

        if (!string.IsNullOrEmpty(screenName))
        {
            OnClickOnButton(screenName);
        }
        else
        {
            OnClickOnButton("recommended");
        }
    }


    public void OnClickOnButton(string eventName)
    {
        SoundManager.instance.PlaySound(SoundType.Click);

        switch (eventName)
        {
            case "back":
                {
                    MainMenuController.instance.ShowScreen(MainMenuScreens.MainMenu);
                }
                break;


            case "coinsShop":
                {
                    MainMenuController.instance.ShowScreen(MainMenuScreens.Shop);
                }
            break;

            case "recommended":
                {
                    recommendedButton.interactable = false;
                    spinUpButton.interactable = true;
                    mttButton.interactable = true;
                }
                break;

            case "spinUp":
                {
                    recommendedButton.interactable = true;
                    spinUpButton.interactable = false;
                    mttButton.interactable = true;
                }
                break;

            case "mtt":
                {
                    recommendedButton.interactable = true;
                    spinUpButton.interactable = true;
                    mttButton.interactable = false;
                }
                break;


            case "play":
                {
                    MainMenuController.instance.ShowMessage("Coming soon");
                }
                break;

            default:
#if ERROR_LOG
            Debug.LogError("unhdnled eventName found in LobbyUiManager = " + eventName);
#endif
                break;
        }
    }


}

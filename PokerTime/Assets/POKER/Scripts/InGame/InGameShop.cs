using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameShop : MonoBehaviour
{
    public GameObject itemScreen,diamondScreen,pointScreen;


    public void OnClickOnButton(string eventName)
    {
        SoundManager.instance.PlaySound(SoundType.Click);

        switch (eventName)
        {
            case "back":
                {
                    InGameUiManager.instance.DestroyScreen(InGameScreens.InGameShop);
                }
                break;

            case "item":
                {
                    itemScreen.SetActive(true);
                    diamondScreen.SetActive(false);
                    pointScreen.SetActive(false);
                }
            break;

            case "point":
                {
                    itemScreen.SetActive(false);
                    diamondScreen.SetActive(false);
                    pointScreen.SetActive(true);
                }
                break;


            case "diamond":
                {
                    itemScreen.SetActive(false);
                    diamondScreen.SetActive(true);
                    pointScreen.SetActive(false);
                }
                break;

                


            default:
                {
                    Debug.LogError("Unhandled eventName found in RealTimeResultUiManager = " + eventName);
                }
                break;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameShop : MonoBehaviour
{
    public GameObject itemScreen,diamondScreen,pointScreen;
    [SerializeField]
    private PlayerData playerData;

    public Text playerGold;
    public Text pointText, diamondText, coinsText;

    public void OnEnable()
    {
    
        OnClickOnButton("item");
        playerGold.text= "" + (int)playerData.balance;
        pointText.text = Utility.GetTrimmedAmount("" + PlayerManager.instance.GetPlayerGameData().points);
        diamondText.text = Utility.GetTrimmedAmount("" + PlayerManager.instance.GetPlayerGameData().diamonds);
        coinsText.text = Utility.GetTrimmedAmount("" + PlayerManager.instance.GetPlayerGameData().coins);

    }
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
                   // Debug.Log("I ammamm000000000amammammam");
                    itemScreen.SetActive(true);
                    diamondScreen.SetActive(false);
                    pointScreen.SetActive(false);
                }
            break;

            case "point":
                {
                   // Debug.Log("I ammamm111111111111amammammam");
                    itemScreen.SetActive(false);
                    diamondScreen.SetActive(false);
                    pointScreen.SetActive(true);
                }
                break;


            case "diamond":
                {
                   // Debug.Log("I ammamm33333333333333330amammammam");
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

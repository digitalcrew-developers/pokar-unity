using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{
    public void OnClickOnButton(string eventName)
    {
        SoundManager.instance.PlaySound(SoundType.Click);

        switch (eventName)
        {
            case "back":
                {
                    InGameUiManager.instance.DestroyScreen(InGameScreens.Menu);
                }
                break;
            case "StandUp":
                {
                    /*  InGameUiManager.instance.DestroyScreen(InGameScreens.Menu);
                      InGameUiManager.instance.ShowScreen(InGameScreens.Tips);*/
                    InGameUiManager.instance.DestroyScreen(InGameScreens.Menu);
                    InGameManager.instance.OnClickStandupBtn();
                }
                break;

            case "GameDisplay":
                {
                    InGameUiManager.instance.ShowScreen(InGameScreens.GameDisplay);
                }
                break;

            case "SwitchTable":
                {
                    StartCoroutine(InGameManager.instance.SwitchTables());
                }
                break;
            case "tableSettings":
                {
                    InGameUiManager.instance.ShowScreen(InGameScreens.TableSettings);

                }
                break;

            case "handRanking":
                {
                    InGameUiManager.instance.ShowScreen(InGameScreens.HandRanking);
                }
                break;

            case "exit":
                {
                    InGameManager.instance.LoadMainMenu();
                }
            break;

            case "topUp":
                {
                    if (SocketController.instance.GetSocketState() == SocketState.Game_Running)
                    {
                        if (PlayerManager.instance.GetPlayerGameData().coins > GlobalGameManager.instance.GetRoomData().minBuyIn)
                        {
                            if (InGameManager.instance.GetMyPlayerObject() != null)
                            {
                                InGameUiManager.instance.ShowScreen(InGameScreens.TopUp, new object[] { InGameManager.instance.GetMyPlayerObject().GetPlayerData().balance });
                            }
                        }
                        else
                        {
                            InGameUiManager.instance.ShowMessage("You don't have sufficient coins for TopUp");
                            //TODO show shop screen
                        }
                    }
                    else
                    {
                        InGameUiManager.instance.ShowMessage("Can only top up when game is running.");
                    }
                }
                break;

            default:
#if ERROR_LOG
            Debug.LogError("Unhandled eventName found = "+eventName);
#endif
            break;
        }

    }
}

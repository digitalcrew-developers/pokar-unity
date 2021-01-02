using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuScriptTeenPatti : MonoBehaviour
{
    public void OnClickOnButton(string eventName)
    {
        SoundManager.instance.PlaySound(SoundType.Click);

        switch (eventName)
        {
            case "back":
                {
                    InGameUiManagerTeenPatti.instance.DestroyScreen(InGameScreensTeenPatti.MenuTeenPatti);
                }
                break;
            case "StandUp":
                {
                    /*  InGameUiManager.instance.DestroyScreen(InGameScreens.Menu);
                      InGameUiManager.instance.ShowScreen(InGameScreens.Tips);*/
                    InGameUiManagerTeenPatti.instance.DestroyScreen(InGameScreensTeenPatti.MenuTeenPatti);
                    InGameManagerTeenPatti.instance.OnClickStandupBtn();
                }
                break;

            case "GameDisplay":
                {
                    InGameUiManagerTeenPatti.instance.ShowScreen(InGameScreensTeenPatti.GameDisplay);
                }
                break;

            case "SwitchTable":
                {
                }
                break;
            case "tableSettings":
                {
                    InGameUiManagerTeenPatti.instance.ShowScreen(InGameScreensTeenPatti.TableSettings);

                }
                break;

            case "handRanking":
                {
                    InGameUiManagerTeenPatti.instance.ShowScreen(InGameScreensTeenPatti.HandRanking);
                }
                break;

            case "exit":
                {
                    InGameManagerTeenPatti.instance.LoadMainMenu();
                }
            break;

            case "topUp":
                {
                    if (SocketController.instance.GetSocketState() == SocketState.Game_Running)
                    {
                        if (PlayerManager.instance.GetPlayerGameData().coins > GlobalGameManager.instance.GetRoomData().minBuyIn)
                        {
                            if (InGameManagerTeenPatti.instance.GetMyPlayerObject() != null)
                            {
                                InGameUiManagerTeenPatti.instance.ShowScreen(InGameScreensTeenPatti.TopUp,new object[] { InGameManagerTeenPatti.instance.GetMyPlayerObject().GetPlayerData().balance});
                            }
                        }
                        else
                        {
                            InGameUiManagerTeenPatti.instance.ShowMessage("You don't have sufficient coins for TopUp");
                            //TODO show shop screen
                        }
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

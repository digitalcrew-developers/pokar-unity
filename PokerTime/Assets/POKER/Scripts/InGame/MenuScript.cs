using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{
    //DEV_CODE
    private void Awake()
    {
        if (GameConstants.poker)
        {
            if (InGameUiManager.instance == null)
            {
                //Debug.Log("Current Room Data: " + GlobalGameManager.instance.currentRoomData.isEVChop.ToString());

                if (GlobalGameManager.instance.currentRoomData.isEVChop)
                    transform.Find("PopUp/EVChopRules").gameObject.SetActive(true);
                else
                    transform.Find("PopUp/EVChopRules").gameObject.SetActive(false);
            }
        }
    }
    //***********************************************************

    public void OnClickOnButton(string eventName)
    {
        SoundManager.instance.PlaySound(SoundType.Click);

        switch (eventName)
        {
            case "back":
                {
                    if (GameConstants.poker)
                    {
                        if (InGameUiManager.instance != null)
                        {
                            InGameUiManager.instance.DestroyScreen(InGameScreens.Menu);
                        }
                        else
                        {
                            ClubInGameUIManager.instance.DestroyScreen(InGameScreens.Menu);
                        }
                    }
                    else
                    {
                        if (InGameUiManagerTeenPatti.instance != null)
                        {
                            InGameUiManagerTeenPatti.instance.DestroyScreen(InGameScreensTeenPatti.MenuTeenPatti);
                        }
                        else
                        {
                            ClubInGameUIManager.instance.DestroyScreen(InGameScreens.Menu);
                        }
                    }

                    //DEV_CODE
                  //  InGameUiManager.instance.DestroyScreen(InGameScreens.Menu);
                }
                break;

            case "StandUp":
                {
                    if (GameConstants.poker)
                    {
                        if (InGameUiManager.instance != null)
                        {
                            InGameUiManager.instance.DestroyScreen(InGameScreens.Message);
                            if (InGameManager.instance.isGameStart && InGameManager.instance.GetMyPlayerObject().GetTotalBet() > 0)
                            {
                                InGameUiManager.instance.ShowMessage("You'll give up the pot if you choose to stand up before this hand ends",
                                () =>
                                {
                                    Debug.Log(InGameManager.instance.GetMyPlayerObject().GetTotalBet());
                                    InGameUiManager.instance.DestroyScreen(InGameScreens.Menu);
                                    InGameManager.instance.OnClickStandupBtn();
                                },
                                () =>
                                {
                                    InGameUiManager.instance.DestroyScreen(InGameScreens.Menu);
                                },
                                "Stand Up", "Cancel"
                                );
                            }
                            else
                            {
                                InGameManager.instance.OnClickStandupBtn();
                            }
                        }
                        else if(ClubInGameUIManager.instance != null)
                        {
                            ClubInGameUIManager.instance.DestroyScreen(InGameScreens.Menu);
                            ClubInGameManager.instance.OnClickStandupBtn();                            
                        }

                    }
                    else
                    {
                        if (InGameUiManagerTeenPatti.instance != null)
                        {
                            InGameUiManagerTeenPatti.instance.DestroyScreen(InGameScreensTeenPatti.MenuTeenPatti);
                            InGameManagerTeenPatti.instance.OnClickStandupBtn();
                        }
                    }     
                }
                break;

            case "GameDisplay":
                {
                    if (InGameUiManager.instance != null)
                    {
                        InGameUiManager.instance.ShowScreen(InGameScreens.GameDisplay);
                    }
                    else
                    {
                        ClubInGameUIManager.instance.ShowScreen(InGameScreens.GameDisplay);
                    }
                }
                break;

            case "SwitchTable":
                {
                    if (InGameUiManager.instance != null)
                    {
                        StartCoroutine(InGameManager.instance.SwitchTables());
                        //InGameUiManager.instance.DestroyScreen(InGameScreens.Menu);
                    }
                }
                break;

            case "tableSettings":
                {
                    if (InGameUiManager.instance != null) 
                    {
                        InGameUiManager.instance.ShowScreen(InGameScreens.TableSettings);

                    }
                    else
                    {
                        ClubInGameUIManager.instance.ShowScreen(InGameScreens.TableSettings);
                    }
                }
                break;

            case "handRanking":
                {
                    if (InGameUiManager.instance != null)
                    {
                        InGameUiManager.instance.ShowScreen(InGameScreens.HandRanking);
                    }
                    else
                    {
                        ClubInGameUIManager.instance.ShowScreen(InGameScreens.HandRanking);
                    }
                }
                break;

            case "hostPrivilege":
                {
                    if (InGameUiManager.instance != null)
                    {
                        InGameUiManager.instance.ShowScreen(InGameScreens.HostPrivilege);
                    }
                    else
                    {
                        ClubInGameUIManager.instance.ShowScreen(InGameScreens.HostPrivilege);
                    }
                }
                break;

            case "evChopRules":
                {
                    if (InGameUiManager.instance != null)
                    {
                        InGameUiManager.instance.ShowScreen(InGameScreens.EVChopRules);
                    }
                    else
                    {
                        ClubInGameUIManager.instance.ShowScreen(InGameScreens.EVChopRules);
                    }
                }
                break; 

            case "counter":
                {
                    if (InGameUiManager.instance != null)
                    {
                        InGameUiManager.instance.ShowScreen(InGameScreens.Counter);
                    }
                    else
                    {
                        ClubInGameUIManager.instance.ShowScreen(InGameScreens.Counter);
                    }
                }
                break;
            case "counterclub":
                {
                    if (InGameUiManager.instance != null)
                    {
                        InGameUiManager.instance.ShowScreen(InGameScreens.Counter);
                    }
                    else
                    {
                        ClubInGameUIManager.instance.ShowScreen(InGameScreens.CounterClub);
                    }
                }
                break;

            case "exit":
                {
                    if (GameConstants.poker)
                    {
                        if (InGameUiManager.instance != null)
                        {
                            InGameManager.instance.LoadMainMenu();
                        }
                        else
                        {
                            //exit for club menu
                            ClubInGameManager.instance.LoadMainMenu();
                        }
                    }
                    else
                    {
                        if (InGameUiManagerTeenPatti.instance != null)
                        {
                            InGameManagerTeenPatti.instance.LoadMainMenu();
                        }
                        else
                        {
                            //exit for club menu
                            ClubInGameManager.instance.LoadMainMenu();
                        }
                    }
                }
            break;

            case "topUp":
                {
                    //if (SocketController.instance.GetSocketState() == SocketState.Game_Running)
                    //{
                        if (PlayerManager.instance.GetPlayerGameData().coins > GlobalGameManager.instance.GetRoomData().minBuyIn)
                        {
                            if (InGameManager.instance != null)
                            {
                                if (InGameManager.instance.GetMyPlayerObject() != null)
                                {
                                    InGameUiManager.instance.ShowScreen(InGameScreens.TopUp, new object[] { InGameManager.instance.GetMyPlayerObject().GetPlayerData().balance });
                                }
                            }
                            else
                            {
                                if (ClubInGameManager.instance.GetMyPlayerObject() != null)
                                {
                                    ClubInGameUIManager.instance.ShowScreen(InGameScreens.TopUp, new object[] { ClubInGameManager.instance.GetMyPlayerObject().GetPlayerData().balance });
                                }
                            }
                            
                        }
                        else
                        {
                            if (InGameUiManager.instance != null)
                            {
                                InGameUiManager.instance.ShowMessage("You don't have sufficient coins for TopUp");
                            }
                            else
                            {
                                ClubInGameUIManager.instance.ShowMessage("You don't have sufficient coins for TopUp");
                            }
                        //TODO show shop screen
                    }
                    //}
                    //else
                    //{
                    //    InGameUiManager.instance.ShowMessage("Can only top up when game is running.");
                    //}
                }
                break;

            case "Share":
                {
                    Debug.Log("On Click On Share....");
#if UNITY_ANDROID
                    new NativeShare().SetText("https://www.google.com/").Share();
#endif
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

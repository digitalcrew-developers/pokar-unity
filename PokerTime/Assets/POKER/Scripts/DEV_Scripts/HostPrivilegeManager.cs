using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HostPrivilegeManager : MonoBehaviour
{
    public static HostPrivilegeManager instance;

    [Header("Authorized To Buy In")]
    public GameObject authorizedOn;
    public GameObject authorizedOff;

    [Header("Auto Open")]
    public GameObject autoOpenOn;
    public GameObject autoOpenOff;

    [Header("Auto Extension")]
    public GameObject autoExtensionOn;
    public GameObject autoExtensionOff;

    [Header("Extend Table Duration")]
    public Button extendTableDuration;

    [Header("Disband Table")]
    public Button disbandTable;

    private int isAuthorized = 1, isAutoOpen = 1, isAutoExtension = 1;

    private void Awake()
    {
        instance = this;        
    }

    public void OnClickOnButton(string eventName)
    {
        SoundManager.instance.PlaySound(SoundType.Click);

        switch (eventName)
        {
            case "back":
                {
                    if (InGameUiManager.instance != null)
                        InGameUiManager.instance.DestroyScreen(InGameScreens.HostPrivilege);
                    else if (ClubInGameUIManager.instance != null)
                        ClubInGameUIManager.instance.DestroyScreen(InGameScreens.HostPrivilege);
                }
                break;

            case "authorizedToBuyIn":
                {
                    if (isAuthorized == 1)
                    {
                        isAuthorized = 0;
                        authorizedOn.SetActive(false);
                        authorizedOff.SetActive(true);


                    }
                    else
                    {
                        isAuthorized = 1;
                        authorizedOn.SetActive(true);
                        authorizedOff.SetActive(false);

                    }

                }
                break;

            case "autoOpen":
                {
                    if (isAutoOpen == 1)
                    {
                        isAutoOpen = 0;
                        autoOpenOn.SetActive(false);
                        autoOpenOff.SetActive(true);


                    }
                    else
                    {
                        isAutoOpen = 1;
                        autoOpenOn.SetActive(true);
                        autoOpenOff.SetActive(false);

                    }

                }
                break;

            case "autoExtension":
                {
                    if (isAutoExtension == 1)
                    {
                        isAutoExtension = 0;
                        autoExtensionOn.SetActive(false);
                        autoExtensionOff.SetActive(true);


                    }
                    else
                    {
                        isAutoExtension = 1;
                        autoExtensionOn.SetActive(true);
                        autoExtensionOff.SetActive(false);

                    }

                }
                break;

            default:
                {
                    Debug.LogError("Unhandled eventName found in MissionsUiManager = " + eventName);
                }
                break;
        }
    }
}
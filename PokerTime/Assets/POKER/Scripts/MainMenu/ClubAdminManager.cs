using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClubAdminManager : MonoBehaviour
{
    public static ClubAdminManager instance = null;
    public List<GameObject> AllScreens = new List<GameObject>();

    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
        }
    }

    /// <summary>
    /// temp. implementation. change later
    /// </summary>
    /// <param name="eventName"></param>
    public void OnClickOnButton(string eventName)
    {
        SoundManager.instance.PlaySound(SoundType.Click);

        CloseAllScreens();

        //to-do add null checks

        switch (eventName)
        {
            case "Notifications":
                AllScreens[0].SetActive(true);
                break;
            case "ClubRating":
                AllScreens[1].SetActive(true);
                break;
            case "Backpack":
                AllScreens[2].SetActive(true);
                break;
            case "Jackpot":
                AllScreens[3].SetActive(true);
                break;
            case "Promotion":
                AllScreens[4].SetActive(true);
                break;
            case "Preferences":
                AllScreens[5].SetActive(true);
                break;
            case "DisbandTheClub":
                AllScreens[6].SetActive(true);
                break;
            default:
                break;
        }
    }

    private void CloseAllScreens()
    {
        foreach(GameObject g in AllScreens)
        {
            if(null!=g)
                g.SetActive(false);
        }
    }
}

public enum AdminSettins
{
    Notifications,
    ClubRating,
    Backpack,
    Jackpot,
    Promotion,
    Preferences,
    DisbandTheClub
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameRulesPanelManagerTournament : MonoBehaviour
{
    public List<Button> menus = new List<Button>();
    public List<GameObject> screens = new List<GameObject>();

    private void OnEnable()
    {
        OnClickedOnMenu(0);
    }

    public void OnClickOnButton(string eventName)
    {
        SoundManager.instance.PlaySound(SoundType.Click);

        switch (eventName)
        {
            case "back":
            {
                if (TournamentInGameUiManager.instance != null)
                    TournamentInGameUiManager.instance.DestroyScreen(TournamentInGameScreens.HandRanking);
            }
            break;

            default:
            {
                Debug.LogError("Unhandled eventName found in GameRulesPanelManagerTournament = " + eventName);
            }
            break;
        }
    }

    public void OnClickedOnMenu(int index)
    {
        SoundManager.instance.PlaySound(SoundType.Click);

        for (int i = 0; i < menus.Count; i++)
        {
            if (i == index)
            {
                menus[i].interactable = false;
                //menus[i].gameObject.GetComponent<Image>().enabled = true;
                menus[i].gameObject.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                screens[i].SetActive(true);
            }
            else
            {
                menus[i].interactable = true;
                //menus[i].gameObject.GetComponent<Image>().enabled = false;
                menus[i].gameObject.GetComponent<Image>().color = new Color32(255, 255, 255, 0);
                screens[i].SetActive(false);
            }
        }
    }
}
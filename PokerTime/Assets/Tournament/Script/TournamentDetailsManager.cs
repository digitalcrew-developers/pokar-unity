using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TournamentDetailsManager : MonoBehaviour
{
    public List<Button> menus = new List<Button>();
    public List<GameObject> screens = new List<GameObject>();
    public List<Sprite> btnSprites = new List<Sprite>();

    public Button button;

    [Header("Details Data")]
    public Text tourneyName;
    public Text timerText;

    private void OnEnable()
    {
        OnClickedOnMenu(0);
    }

    public void OnClickOnButton(int index)
    {
        switch (index)
        {
            case 0:
                if (TournamentInGameUiManager.instance != null)
                    TournamentInGameUiManager.instance.DestroyScreen(TournamentInGameScreens.TournamentDetails);
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

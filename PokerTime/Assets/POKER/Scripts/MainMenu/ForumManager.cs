using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForumManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnClickF_backBtn()
    {
        MainMenuController.instance.ShowScreen(MainMenuScreens.MainMenu);
    }
}

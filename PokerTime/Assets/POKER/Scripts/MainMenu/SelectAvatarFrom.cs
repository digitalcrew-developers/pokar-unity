using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectAvatarFrom : MonoBehaviour
{
    public void onCloseSelectFrom()
    {
        MainMenuController.instance.DestroyScreen(MainMenuScreens.SelectFrom);
    }
    public void onClickDefaultbtn()
    {
        MainMenuController.instance.ShowScreen(MainMenuScreens.ChangeProfileIcon);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClubDataManager : MonoBehaviour
{
    public static ClubDataManager instance;

    public Transform container;

    private void Awake()
    {
        instance = this;
    }

    public void OnClickOnData()
    {
        ClubAdminManager.instance.ShowScreen(ClubScreens.GameData);
    }
}
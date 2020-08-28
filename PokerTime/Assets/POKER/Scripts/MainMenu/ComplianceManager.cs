using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComplianceManager : MonoBehaviour
{
    public void OnCloseCompliance()
    {
        MainMenuController.instance.DestroyScreen(MainMenuScreens.Compliance);
    }
}

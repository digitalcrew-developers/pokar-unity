using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComplianceManagerTeen : MonoBehaviour
{
    public void OnCloseCompliance()
    {
        MainMenuControllerTeen.instance.DestroyScreen(MainMenuScreensTeen.Compliance);
    }
}

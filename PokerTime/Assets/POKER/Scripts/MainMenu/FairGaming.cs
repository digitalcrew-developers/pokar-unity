using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FairGaming : MonoBehaviour
{
    bool isGPS = false;
    bool isIP = false;
    public GameObject GPSOn, GPSOff, IPOn, IPOff;

    public void OnCloseFairGame()
    {
        MainMenuController.instance.DestroyScreen(MainMenuScreens.FairGaming);
    }

    public void Start()
    {
        if (isGPS)
        {
            GPSOff.SetActive(false);
            GPSOn.SetActive(true);
        }
        else
        {
            GPSOff.SetActive(true);
            GPSOn.SetActive(false);
        }

        if (isIP)
        {
            IPOff.SetActive(false);
            IPOn.SetActive(true);
        }
        else
        {
            IPOff.SetActive(true);
            IPOn.SetActive(false);
        }
    }

    public void OnClickGPSBTN()
    {
        if (isGPS)
        {
            isGPS = false;
            GPSOff.SetActive(false);
            GPSOn.SetActive(true);
        }
        else
        {
            isGPS = true;
            GPSOff.SetActive(true);
            GPSOn.SetActive(false);
        }
    }

    public void OnClickIPBTN()
    {
        if (isIP)
        {
            isIP = false;
            IPOff.SetActive(false);
            IPOn.SetActive(true);
        }
        else
        {
            isIP = true;
            IPOff.SetActive(true);
            IPOn.SetActive(false);
        }
    }
}

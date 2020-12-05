using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinInativeRotationTeenPatti : MonoBehaviour
{


    int spinStartIndex = 0;
    public GameObject spinVal;



    private void OnEnable()
    {
        StartSpin();

    }

    private void OnDisable()
    {
        //StartSpin();
        StopSpin();
    }


    public void StartSpin()
    {
        InvokeRepeating("RotateSpin", 0.25f, 0.25f);
       
        
    }


    void StopSpin()
    {
        CancelInvoke("RotateSpin");
        
    }


    public void DeActiveObj()
    {
        this.gameObject.SetActive(false);
    }


    void RotateSpin()
    {
        for (int i = 0; i < this.transform.Find("Focus").childCount; i++)
        {
            if (spinStartIndex == i)
            {
                this.transform.Find("Focus").GetChild(i).GetChild(0).gameObject.SetActive(true);
            }
            else
            {
                this.transform.Find("Focus").GetChild(i).GetChild(0).gameObject.SetActive(false);
            }
        }
        if (spinStartIndex < this.transform.Find("Focus").childCount)
        {
            spinStartIndex++;
            if (spinStartIndex == this.transform.Find("Focus").childCount)
            {
                spinStartIndex = 0;
            }
        }
    }
}

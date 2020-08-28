using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpinBehaviour : MonoBehaviour
{
    
    
    int spinStartIndex = 0;
    public GameObject spinVal;
    
  

    private void OnEnable()
    {
        spinVal.SetActive(false);
        StartSpin();

    }




    public void StartSpin()
    {
        InvokeRepeating("RotateSpin", 0.15f, 0.15f);
        float rand = Random.Range(3.0f, 5.5f);
        Invoke("StopSpin", rand);
    }


    void StopSpin()
    {
        CancelInvoke("RotateSpin");
        spinVal.SetActive(true);
        spinVal.GetComponent<Text>().text = "+ "+SpinManager.instance.spinItemList[spinStartIndex].itemValue ;
        SpinManager.instance.SetSpinWheelWinning(spinStartIndex);
        Invoke("DeActiveObj", 2);
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

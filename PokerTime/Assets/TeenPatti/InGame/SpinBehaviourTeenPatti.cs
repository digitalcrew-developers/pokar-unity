using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpinBehaviourTeenPatti : MonoBehaviour
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
        InvokeRepeating("RotateSpin", 0.10f, 0.10f);
        float rand = Random.Range(2.0f, 3.5f);
        Invoke("StopSpin", rand);
    }


    void StopSpin()
    {
        CancelInvoke("RotateSpin");
        spinVal.SetActive(true);
        spinVal.GetComponent<Text>().text = "+ "+SpinManagerTeenPatti.instance.spinItemList[spinStartIndex].itemValue ;

        SpinManagerTeenPatti.instance.SetSpinWheelWinning(spinStartIndex);
        Invoke("DeActiveObj", 2);
    }


    public void DeActiveObj()
    {
        this.gameObject.SetActive(false);
        SpinManagerTeenPatti.instance.InactiveSpinRotation.SetActive(true);
    }


    void RotateSpin()
    {
        for (int i = 0; i < this.transform.Find("Focus").childCount; i++)
        {
            if (spinStartIndex == i)
            {
                this.transform.Find("Focus").GetChild(i).GetChild(0).gameObject.SetActive(true);
                SoundManager.instance.PlaySound(SoundType.spinWheel);

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

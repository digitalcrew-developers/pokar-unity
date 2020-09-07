using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SipnBigBehaviour : MonoBehaviour
{
    int spinStartIndex = 0;
    public GameObject spinVal;
    private int counter = 0;


    private void OnEnable()
    {
        spinVal.SetActive(false);
        StartSpin();

    }




    public void StartSpin()
    {
        InvokeRepeating("RotateSpin", 0.15f, 0.15f);
        
        SpinWheelUIManager.instance.oneXBtn.GetComponent<Image>().color = new Color32(120, 120, 120, 255);
        SpinWheelUIManager.instance.oneXBtn.GetComponent<Button>().enabled=false;
        SpinWheelUIManager.instance.fiveXBtn.GetComponent<Image>().color = new Color32(120, 120, 120, 255);
        SpinWheelUIManager.instance.fiveXBtn.GetComponent<Button>().enabled = false;

        float rand=0;
        switch (SpinWheelUIManager.instance.eventValue)
        {
            case "1x":
                rand = Random.Range(3.0f, 5.5f);
                Invoke("StopSpin", rand);
                break;
            case "5x":
                 rand = Random.Range(3.0f, 5.5f);
                Invoke("StopSpin", rand);
                
                break;
        }
        
    }


    void StopSpin()
    {
        CancelInvoke("RotateSpin");
        spinVal.SetActive(true);
        spinVal.GetComponent<Text>().text = "+ " + SpinManager.instance.spinItemList[spinStartIndex].itemValue;
       

        switch (SpinWheelUIManager.instance.eventValue)
        {
            case "1x":

                SpinWheelUIManager.instance.drawOutput.SetActive(true);
                SpinWheelUIManager.instance.drawOutput.transform.GetChild(0).gameObject.SetActive(true);
                SpinWheelUIManager.instance.drawOutput.transform.GetChild(1).gameObject.SetActive(false);
                Image img = SpinWheelUIManager.instance.ImgGetContainer.transform.GetChild(spinStartIndex).GetComponent<Image>();
                SpinWheelUIManager.instance.draw1xOutputImg.sprite = img.sprite;
                SpinWheelUIManager.instance.draw1xOutputText.text = "+ " + SpinManager.instance.spinItemList[spinStartIndex].itemValue;

                break;
            case "5x":
                SpinWheelUIManager.instance.drawOutput.SetActive(true);
                SpinWheelUIManager.instance.drawOutput.transform.GetChild(0).gameObject.SetActive(false);
                SpinWheelUIManager.instance.drawOutput.transform.GetChild(1).gameObject.SetActive(true);
                break;
        }

        

        SpinManager.instance.SetSpinWheelWinning(spinStartIndex);
        Invoke("DeActiveObj", 2);
    }


    public void DeActiveObj()
    {
        switch (SpinWheelUIManager.instance.eventValue)
        {
            case "1x":
                this.gameObject.SetActive(false);
                SpinManager.instance.InactiveSpinRotation.SetActive(true);
                break;
            case "5x":
                if (counter < 5)
                {
                    StartSpin();
                    counter += 1;
                }
                else {
                    this.gameObject.SetActive(false);
                    SpinManager.instance.InactiveSpinRotation.SetActive(true);
                }
                
                break;
        }
        SpinWheelUIManager.instance.oneXBtn.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        SpinWheelUIManager.instance.oneXBtn.GetComponent<Button>().enabled = true;
        SpinWheelUIManager.instance.fiveXBtn.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        SpinWheelUIManager.instance.fiveXBtn.GetComponent<Button>().enabled = true;

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

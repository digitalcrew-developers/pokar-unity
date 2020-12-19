using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SipnBigBehaviourTeenPatti : MonoBehaviour
{
    int spinStartIndex = 0;
    public GameObject spinVal;
    public int counter = 0;
    public List<int> prevValue = new List<int>();

    private void OnEnable()
    {
        spinVal.SetActive(false);
        prevValue.Clear();
        counter = 0;
        StartSpin();

    }




    public void StartSpin()
    {
        InvokeRepeating("RotateSpin", 0.01f, 0.06f);

        SpinWheelUIManagerTeenPatti.instance.oneXBtn.GetComponent<Image>().color = new Color32(120, 120, 120, 255);
        SpinWheelUIManagerTeenPatti.instance.oneXBtn.GetComponent<Button>().enabled = false;
        SpinWheelUIManagerTeenPatti.instance.fiveXBtn.GetComponent<Image>().color = new Color32(120, 120, 120, 255);
        SpinWheelUIManagerTeenPatti.instance.fiveXBtn.GetComponent<Button>().enabled = false;

        float rand = 0;
        switch (SpinWheelUIManagerTeenPatti.instance.eventValue)
        {
            case "1x":
                rand = Random.Range(1.0f, 2.0f);
                Invoke("StopSpin", rand);
                break;
            case "5x":
                rand = Random.Range(1.0f, 2.0f);
                Invoke("StopSpin", rand);

                break;
        }

    }


    void StopSpin()
    {
        CancelInvoke("RotateSpin");
        spinVal.SetActive(true);
        spinVal.GetComponent<Text>().text = "" ;


        switch (SpinWheelUIManagerTeenPatti.instance.eventValue)
        {
            case "1x":

                SpinWheelUIManagerTeenPatti.instance.drawOutput.SetActive(true);
                SpinWheelUIManagerTeenPatti.instance.drawOutput.transform.GetChild(0).gameObject.SetActive(true);
                SpinWheelUIManagerTeenPatti.instance.drawOutput.transform.GetChild(1).gameObject.SetActive(false);
                Image img = SpinWheelUIManagerTeenPatti.instance.ImgGetContainer.transform.GetChild(spinStartIndex).GetComponent<Image>();
                SpinWheelUIManagerTeenPatti.instance.draw1xOutputImg.sprite = img.sprite;
                SpinWheelUIManagerTeenPatti.instance.draw1xOutputText.text = "+ " + SpinManagerTeenPatti.instance.spinItemList[spinStartIndex].itemValue;

                break;
            case "5x":
                SpinWheelUIManagerTeenPatti.instance.drawOutput.SetActive(true);
                SpinWheelUIManagerTeenPatti.instance.drawOutput.transform.GetChild(0).gameObject.SetActive(false);
                SpinWheelUIManagerTeenPatti.instance.drawOutput.transform.GetChild(1).gameObject.SetActive(true);


                prevValue.Add(spinStartIndex);
                for (int j = counter; j > -1; j--)
                {
                    SpinWheelUIManagerTeenPatti.instance.draw5xOutputImg[counter].gameObject.SetActive(true);

                    Image img2 = SpinWheelUIManagerTeenPatti.instance.ImgGetContainer.transform.GetChild(prevValue[j]).GetComponent<Image>();
                    SpinWheelUIManagerTeenPatti.instance.draw5xOutputImg[j].sprite = img2.sprite;
                    SpinWheelUIManagerTeenPatti.instance.draw5xOutputText[j].text = "+ " + SpinManagerTeenPatti.instance.spinItemList[prevValue[j]].itemValue;

                }

                for (int i = counter+1; i < SpinWheelUIManagerTeenPatti.instance.draw5xOutputImg.Length; i++)
                {
                    SpinWheelUIManagerTeenPatti.instance.draw5xOutputImg[i].gameObject.SetActive(false);

                }
                break;
        }



        SpinManagerTeenPatti.instance.SetSpinWheelWinning(spinStartIndex);
        Invoke("DeActiveObj", 2);
    }


    public void DeActiveObj()
    {
        switch (SpinWheelUIManagerTeenPatti.instance.eventValue)
        {
            case "1x":
                this.gameObject.SetActive(false);
                SpinManagerTeenPatti.instance.InactiveSpinRotation.SetActive(true);
                break;
            case "5x":
                if (counter < 4)
                {
                    StartSpin();

                }
                else
                {
                    this.gameObject.SetActive(false);
                    SpinManagerTeenPatti.instance.InactiveSpinRotation.SetActive(true);
                }
                counter += 1;
                break;
        }
        SpinWheelUIManagerTeenPatti.instance.oneXBtn.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        SpinWheelUIManagerTeenPatti.instance.oneXBtn.GetComponent<Button>().enabled = true;
        SpinWheelUIManagerTeenPatti.instance.fiveXBtn.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        SpinWheelUIManagerTeenPatti.instance.fiveXBtn.GetComponent<Button>().enabled = true;

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

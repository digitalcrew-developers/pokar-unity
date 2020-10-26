using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
public class SliderChange : MonoBehaviour
{
    public static SliderChange instance;
    public RangeSlider rg;
    public Slider slider;
    public TMP_Text valueText;
    public TMP_Text lowValueText;
    public TMP_Text highValueText;        
    public string[] sliderValues;
    public int[] SubValuearray;
    public int[] StoreSubValue;
    public GameObject high;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        valueText.text = "Blinds: " + sliderValues[0];
        lowValueText.text = "BuyIn: " + SubValuearray[0].ToString();
        highValueText.text = "Max : " + SubValuearray[15];
    }

    public void SliderValueChange()
    {
        for (int i = 0; i < SubValuearray.Length; i++)
        {
            if (slider.value <= 4)
                SubValuearray[i] = StoreSubValue[i] * (Mathf.RoundToInt(slider.value) + 1);


            if (slider.value == 5)
                SubValuearray[i] = StoreSubValue[i] * 10;

            if (slider.value == 6)
                SubValuearray[i] = StoreSubValue[i] * 15;

            if (slider.value == 7)
                SubValuearray[i] = StoreSubValue[i] * 25;

            if (slider.value == 8)
                SubValuearray[i] = StoreSubValue[i] * 50;

            if (slider.value == 9)
                SubValuearray[i] = StoreSubValue[i] * 100;

            if (slider.value == 10)
                SubValuearray[i] = StoreSubValue[i] * 250;

            if (slider.value == 11)
                SubValuearray[i] = StoreSubValue[i] * 500;


            for (int j = SubValuearray.Length; j >= 0; j--)
            {
                if (rg.HighValue == j)
                {
                    highValueText.text = "Max: " + SubValuearray[j].ToString();
                }
            }
            for (int k = 0; k < SubValuearray.Length; k++)
            {
                if (rg.LowValue == k)
                {
                    lowValueText.text = "BuyIn: " + SubValuearray[k].ToString();
                }
            }
        }


        for (int i = 0; i < sliderValues.Length; i++)
        {
            if (slider.value == i)
            {
                valueText.text = "Blinds: " + sliderValues[i];
            }
        }
    }

    public void TwoWaySlider()
    {
        for (int i = 0; i < SubValuearray.Length; i++)
        {
            if (rg.LowValue == i && rg.targetGraphic == rg.m_LowHandleRect.GetComponent<Graphic>())
            {
                lowValueText.text = "BuyIn: " + SubValuearray[i].ToString();
            }
        }

        for (int i = SubValuearray.Length; i >= 0; i--)
        {
            if (rg.HighValue == i && rg.targetGraphic == rg.m_HighHandleRect.GetComponent<Graphic>())
            {
                highValueText.text = "Max: " + SubValuearray[i].ToString();
            }
        }
    }
}

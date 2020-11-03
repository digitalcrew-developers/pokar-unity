using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderManager : MonoBehaviour
{
    [SerializeField]
    private bool isToShowPerSign = false;

    [SerializeField]
    private bool isTimeSlider = false;

    [SerializeField]
    private TMP_Text sliderValueText;

    [SerializeField]
    private TMP_Text[] timeValues;

    [SerializeField]
    private  Slider slider;

    [SerializeField]
    private int[] values;

    private void Awake()
    {
        SliderValueChange();        
    }

    public void SliderValueChange()
    {
        if (!isTimeSlider)
        {
            if (isToShowPerSign)
                sliderValueText.text = values[(int)slider.value].ToString() + "%";
            else
                sliderValueText.text = values[(int)slider.value].ToString();
        }
        else
        {
            for (int i = 0; i < timeValues.Length; i++)
            {
                if (i == slider.value)
                {
                    timeValues[i].color = new Color32(255, 146, 49, 255);
                    RingGameManager.instance.components[28].transform.GetComponent<TMP_Text>().text = timeValues[i].text;
                }
                else
                    timeValues[i].color = new Color32(255, 255, 255, 255);
            }
            
        }
    }
}
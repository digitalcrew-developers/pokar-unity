using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClubTemplateManager : MonoBehaviour
{
    public static ClubTemplateManager instance;

    public Toggle selectToggle;
    public Text templateName, gameType, blindsText, anteText, playerText, timeText, templateCounterText;
    public Button editButton, deleteButton, incrementBtn, decrementBtn;
    public GameObject editData, counterData, blindsInfoObj, anteInfoObj, _6PlusObj;
    public string tableId;


    private int templateCounter = 1;

    private void OnEnable()
    {
        editData.SetActive(true);
        counterData.SetActive(false);

        selectToggle.isOn = false;

        templateCounterText.text = templateCounter.ToString();
    }

    private void Awake()
    {
        instance = this;        
    }

    private void Update()
    {
        //Enable/Disable Increment Button
        if (templateCounter < 10)
            incrementBtn.interactable = true;
        else
            incrementBtn.interactable = false;

        //Enable/Disable Decrement Button
        if (templateCounter > 1)
            decrementBtn.interactable = true;
        else
            decrementBtn.interactable = false;
    }

    public void OnSelectedCheckBox()
    {
        if(selectToggle.isOn)
        {
            counterData.SetActive(true);
            editData.SetActive(false);
        }
        else
        {
            counterData.SetActive(false);
            editData.SetActive(true);
        }
    }

    public void OnClickIncrementButton()
    {
        if (templateCounter < 10)
        {
            templateCounter++;
            templateCounterText.text = templateCounter.ToString();
        }
    }

    public void OnClickDecrementButton()
    {
        if (templateCounter > 1)
        {
            templateCounter--;
            templateCounterText.text = templateCounter.ToString();
        }
    }
}
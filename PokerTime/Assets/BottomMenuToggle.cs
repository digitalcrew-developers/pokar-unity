using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BottomMenuToggle : MonoBehaviour
{
    public Text ToggleText;
    public GameObject SelectedBG;
    public string ScreenToShow = string.Empty;
    private Toggle myToggle;

    private Color textColor;

    private void Start()
    {
        textColor = new Color();
        ColorUtility.TryParseHtmlString("#0CBAD2", out textColor);

        myToggle = GetComponent<Toggle>();
        myToggle.onValueChanged.AddListener(delegate {
            ToggleValueChanged(myToggle);
        });

        UpdateColors();
    }

    private void UpdateColors()
    {
        if (myToggle.isOn)
        {
            //call MainMenuController here.
            textColor.a = 1.0f;
            SelectedBG.SetActive(true);
        }
        else
        {
            SelectedBG.SetActive(false);
            textColor.a = 0.5f;
        }
        ToggleText.color = textColor;
    }

    private void ToggleValueChanged(object m_Toggle)
    {
        UpdateColors();
    }
}

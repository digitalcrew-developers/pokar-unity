using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClubExportDataManagerTeen : MonoBehaviour
{
    public static ClubExportDataManagerTeen instance;

    public Button exportData, exportRecord;
    public TMP_Text dateText;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void OnEnable()
    {
        dateText.text = DateTime.Now.Year + "." + (DateTime.Now.Month.ToString().Length == 1 ? "0" + DateTime.Now.Month.ToString() : DateTime.Now.Month.ToString()) + "." +
                                                   (DateTime.Now.Day.ToString().Length == 1 ? "0" + DateTime.Now.Day.ToString() : DateTime.Now.Day.ToString()) + " - " +
                        DateTime.Now.Year + "." + (DateTime.Now.Month.ToString().Length == 1 ? "0" + DateTime.Now.Month.ToString() : DateTime.Now.Month.ToString()) + "." +
                                                   (DateTime.Now.Day.ToString().Length == 1 ? "0" + DateTime.Now.Day.ToString() : DateTime.Now.Day.ToString());
    }
}
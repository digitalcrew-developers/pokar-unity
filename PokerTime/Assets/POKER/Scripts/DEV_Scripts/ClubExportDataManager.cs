using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClubExportDataManager : MonoBehaviour
{
    public static ClubExportDataManager instance;

    public Button exportData, exportRecord;
    public Text dateText;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
}
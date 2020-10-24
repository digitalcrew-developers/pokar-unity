using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClubTemplateManager : MonoBehaviour
{
    public static ClubTemplateManager instance;

    public Toggle selectToggle;
    public Text templateType, gameType, chipsData, userData, timeData;
    public Button editButton, deleteButton;
    public string tableId;

    private void Awake()
    {
        instance = this;
    }
}
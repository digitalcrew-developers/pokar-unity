using LitJson;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class InOutMsgUIManager : MonoBehaviour
{
    public string userId;
    public Image profileImage;
    public Text userName, userNameFirstLetter;

    public RectTransform backgroundRectTransform;
    public TextMeshProUGUI textMeshPro;

    public void SetText(string tooltipText)
    {
        textMeshPro.SetText(tooltipText);
        textMeshPro.ForceMeshUpdate();

        Vector2 textSize = textMeshPro.GetRenderedValues(false);
        Vector2 paddingSize = new Vector2(36, 18);

        backgroundRectTransform.sizeDelta = textSize + paddingSize;
    }    
}
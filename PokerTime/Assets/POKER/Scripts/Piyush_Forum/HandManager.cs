using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandManager : MonoBehaviour
{
    public static HandManager instance;

    public Image[] cards;
    public Text dateAndTime, roomData, chipsData, betData;
    public Button shareButton, collectionButton, removeFromCollectionButton;

    private void Awake()
    {
        instance = this;
    }
}
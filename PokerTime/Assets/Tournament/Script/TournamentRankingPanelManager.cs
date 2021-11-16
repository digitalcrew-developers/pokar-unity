using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TournamentRankingPanelManager : MonoBehaviour
{
    public static TournamentRankingPanelManager instance;

    [Header("Rank Sprite")]
    public Sprite[] rankSprites;

    [Header("Winner Player Data")]
    public Text playerName;
    public Text playerID;
    public Image playerImage;

    public GameObject rankPrefab;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void OnClickOnButton(int index)
    {
        switch (index)
        {
            case 0:
                print("BackButton");
                break;
        }
    }
}
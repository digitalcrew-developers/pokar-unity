using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using System;

public class HandHistoryUiManager : MonoBehaviour
{
    public static HandHistoryUiManager instance;

    public Image[] communityCards;
    public GameObject playerCardsPrefab, roundHeadingPrefab,loadingScreen;
    public Transform container;
    public Text pageNoText;
    public LayoutManager layoutManager;
    public GameObject handdetailsBtn;
    public GameObject handsummaryBtn;


    [SerializeField]
    private List<MatcHistoryData> matchHistoryData = new List<MatcHistoryData>();
    private int currentPageNo = 0;



    private void Awake()
    {
        instance = this;
    }

    private void OnDestroy()
    {
        instance = null;
    }

    void Start()
    {
        pageNoText.text = "0/0";
        ToggleLoadingScreen(true);
        handDetailClik();
        SocketController.instance.RequestForMatchHistory();
    }

    public void handDetailClik() {
        handdetailsBtn.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        handsummaryBtn.GetComponent<Image>().color = new Color32(160, 160, 160, 255);
    }
    public void handDetailSummary() {
        handdetailsBtn.GetComponent<Image>().color = new Color32(160, 160, 160, 255);
        handsummaryBtn.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
    }



    public void OnClickonButton(string eventName)
    {
        SoundManager.instance.PlaySound(SoundType.Click);

        switch (eventName)
        {

            case "back":
                {
                    InGameUiManager.instance.DestroyScreen(InGameScreens.HandHistory);
                }
            break;


            case "next":
                {
                    if (currentPageNo < matchHistoryData.Count)
                    {
                        ++currentPageNo;
                        ShowHistory(currentPageNo);
                    }
                }
            break;

            case "prev":
                {
                    if (currentPageNo > 0)
                    {
                        --currentPageNo;
                        ShowHistory(currentPageNo);
                    }
                }
                break;


            default:
                {
#if ERROR_LOG
                    Debug.LogError("Unhandled event name found in HandHistoryUiManager = "+eventName);
#endif
                }
            break;
        }


    }

    private void ShowHistory(int pageNo)
    {
        for (int i = 0; i < container.childCount; i++)
        {
            Destroy(container.GetChild(i).gameObject);
        }

        for (int i = 0; i < communityCards.Length; i++)
        {
            communityCards[i].gameObject.SetActive(false);
        }


        switch (matchHistoryData[pageNo].maxRoundNo)
        {
            case 0:
                {
                    for (int i = 0; i < communityCards.Length; i++)
                    {
                        communityCards[i].gameObject.SetActive(false);
                    }
                }
            break;

            case 1:
                {
                    for (int i = 0; i < 3; i++)
                    {
                        communityCards[i].gameObject.SetActive(true);
                        communityCards[i].sprite = matchHistoryData[pageNo].communityCards[i].cardsSprite;
                    }
                }
                break;

            case 2:
                {
                    for (int i = 0; i < 4; i++)
                    {
                        communityCards[i].gameObject.SetActive(true);
                        communityCards[i].sprite = matchHistoryData[pageNo].communityCards[i].cardsSprite;
                    }
                }
                break;

            case 3:
                {
                    for (int i = 0; i < communityCards.Length; i++)
                    {
                        communityCards[i].gameObject.SetActive(true);
                        communityCards[i].sprite = matchHistoryData[pageNo].communityCards[i].cardsSprite;
                    }
                }
                break;

            default:
                {
                    for (int i = 0; i < communityCards.Length; i++)
                    {
                        communityCards[i].gameObject.SetActive(true);
                        communityCards[i].sprite = matchHistoryData[pageNo].communityCards[i].cardsSprite;
                    }
                }
            break;
        }




        for (int i = 0; i < matchHistoryData[pageNo].roundData.Count; i++)
        {
            if (matchHistoryData[pageNo].roundData[i].isSpawnHeading)
            {
                GameObject heading = Instantiate(roundHeadingPrefab,container) as GameObject;
                heading.transform.Find("Text").GetComponent<Text>().text = matchHistoryData[pageNo].roundData[i].roundName;
            }

            GameObject gm = Instantiate(playerCardsPrefab, container) as GameObject;

            if (matchHistoryData[pageNo].roundData[i].roundNo != ""+matchHistoryData[pageNo].maxRoundNo)
            {
                gm.transform.Find("Card_0").GetComponent<Image>().sprite = matchHistoryData[pageNo].roundData[i].playerCards[0].cardsSprite;
                gm.transform.Find("Card_1").GetComponent<Image>().sprite = matchHistoryData[pageNo].roundData[i].playerCards[1].cardsSprite;

                gm.transform.Find("Name").GetComponent<Text>().text = matchHistoryData[pageNo].roundData[i].userName;
                gm.transform.Find("Action").GetComponent<Text>().text = matchHistoryData[pageNo].roundData[i].betType;
            }
        }


        pageNoText.text = "" + currentPageNo + "/" + matchHistoryData.Count;
        layoutManager.UpdateLayout();
    }



    public void OnMatchHistoryFound(string serverResponse)
    {
        ToggleLoadingScreen(false);
        matchHistoryData.Clear();

        JsonData data = JsonMapper.ToObject(serverResponse);

        for (int i = 0; i < data[0].Count; i++)
        {
            MatcHistoryData matchData = new MatcHistoryData();
            string roundNo = "";

            for (int j = 0; j < data[0][i]["logs"].Count; j++)
            {
                if (matchData.communityCards == null)
                {
                    matchData.communityCards = new CardData[data[0][i]["logs"][j]["openCards"].Count];

                    for (int k = 0; k < matchData.communityCards.Length; k++)
                    {
                        matchData.communityCards[k] = CardsManager.instance.GetCardData(data[0][i]["logs"][j]["openCards"][k].ToString());
                    }
                }


                UserBetHistory betHisory = new UserBetHistory();

                betHisory.roundNo = data[0][i]["logs"][j]["currentSubRounds"].ToString();
                betHisory.roundName = data[0][i]["logs"][j]["roundName"].ToString();
                betHisory.userName = data[0][i]["logs"][j]["userName"].ToString();

                int betAmount = (int)float.Parse(data[0][i]["logs"][j]["amount"].ToString());

                if (betAmount > 0)
                {
                    betHisory.betType = data[0][i]["logs"][j]["betType"].ToString()+" "+betAmount;
                }
                else
                {
                    betHisory.betType = data[0][i]["logs"][j]["betType"].ToString();
                }

                betHisory.playerCards = new CardData[data[0][i]["logs"][j]["playerCards"].Count];

                for (int k = 0; k < betHisory.playerCards.Length; k++)
                {
                    betHisory.playerCards[k] = CardsManager.instance.GetCardData(data[0][i]["logs"][j]["playerCards"][k].ToString());
                }

                if (roundNo == betHisory.roundNo)
                {
                    betHisory.isSpawnHeading = false;
                }
                else
                {
                    betHisory.isSpawnHeading = true;
                    roundNo = betHisory.roundNo;
                }

                matchData.roundData.Add(betHisory);
            }

            if (data[0][i]["logs"].Count > 0)
            {
                matchData.maxRoundNo = (int)float.Parse(matchData.roundData[matchData.roundData.Count - 1].roundNo);
            }
            else
            {
                matchData.maxRoundNo = 0;
            }

            matchHistoryData.Add(matchData);
        }

        currentPageNo = matchHistoryData.Count;

        if (currentPageNo > 0)
        {
            ShowHistory(currentPageNo - 1);
        }
    }


    



    private void ToggleLoadingScreen(bool isShow)
    {
        loadingScreen.SetActive(isShow);
    }
    
}



[System.Serializable]
public class MatcHistoryData
{
    public List<UserBetHistory> roundData = new List<UserBetHistory>();
    public CardData[] communityCards;
    public int maxRoundNo;
}


[System.Serializable]
public class UserBetHistory
{
    public bool isSpawnHeading;
    public string roundNo;
    public string roundName;
    public string userName;
    public string betType;
    public CardData[] playerCards = new CardData[2];
}
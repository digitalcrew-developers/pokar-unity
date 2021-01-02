using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class HandHistoryManager : MonoBehaviour
{
    public GameObject LoadingText;
    public GameObject DetailsPanel, SummaryPanel;
    public GameObject DetailsButtonImage, SummaryButtonImage;
    public GameObject HandDetailRoundPrefab, PlayerDetailsPrefab, HandSummaryItemPrefab;
    public Transform HandDetailsContent, HandSummaryContent;

    private int pageNo = 0;
    private int totalPages;
    private AllHandHistroy histories;

    public Text PageNoText;
    public Button ButtonNext;
    public Button ButtonBack;
    public Button OpenSlider;

    public Button OpenHandDetailsButton;
    public Button OpenHandSummaryButton;

    public GameObject Slider;

    public Text TimeText;
    public Text SB_BB;
    public Text GameID;

    public Text DebugText;

    public void OnClickOnButton(string eventName)
    {
        SoundManager.instance.PlaySound(SoundType.Click);

        switch (eventName)
        {
            case "back":
                {
                    if (InGameUiManager.instance != null)
                        InGameUiManager.instance.DestroyScreen(InGameScreens.HandHistory);
                    else if (ClubInGameUIManager.instance != null)
                        ClubInGameUIManager.instance.DestroyScreen(InGameScreens.HandHistory);
                }
                break;

            default:
                {
                    Debug.LogError("Unhandled eventName found in RealTimeResultUiManager = " + eventName);
                }
                break;
        }
    }

    private void IncreasePage()
    {
        pageNo++;
        if(pageNo > totalPages - 1)
        {
            pageNo = totalPages - 1;
        }
        int val = totalPages - 1;
        PageNoText.text = pageNo + "/" + val;
        FillDetailsFromPageNo();
    }

    private void DecreasePage()
    {
        pageNo--;
        if (pageNo < 0)
        {
            pageNo = 0;
        }
        int val = totalPages - 1;
        PageNoText.text = pageNo + "/" + val;
        FillDetailsFromPageNo();
    }

    public void Init()
    {
        ButtonNext.onClick.RemoveAllListeners();
        ButtonBack.onClick.RemoveAllListeners();

        ButtonNext.onClick.AddListener(IncreasePage);
        ButtonBack.onClick.AddListener(DecreasePage);

        OpenHandDetailsButton.onClick.RemoveAllListeners();
        OpenHandSummaryButton.onClick.RemoveAllListeners();
        OpenHandDetailsButton.onClick.AddListener(() => OpenScreen("Details"));
        OpenHandSummaryButton.onClick.AddListener(() => OpenScreen("Summary"));

        OpenScreen("Summary");

        //StartCoroutine(LoadDemoCustomJSON("https://api.jsonbin.io/b/5fa11671a03d4a3bab0bd2ad"));
        Debug.Log("))))))))))))))))))))))))  ");
        Debug.LogError("tableId id is :" + GlobalGameManager.instance.GetRoomData().socketTableId);
        string requestData = "{\"tableId\":\"" + GlobalGameManager.instance.GetRoomData().socketTableId + "\"}";
        
        WebServices.instance.SendRequest(RequestType.GetTableHandHistory, requestData, true, OnServerResponseFound);
    }

    public void OnServerResponseFound(RequestType requestType, string serverResponse, bool isShowErrorMessage, string errorMessage)
    {
        Debug.Log("REspoNSE: " + serverResponse);
        MainMenuController.instance.DestroyScreen(MainMenuScreens.Loading);

        if (errorMessage.Length > 0)
        {
            if (isShowErrorMessage)
            {
                Debug.LogError("ERror hand histry.........................................");
                MainMenuController.instance.ShowMessage(errorMessage);
            }

            return;
        }

        if (requestType == RequestType.GetTableHandHistory)
        {
            Debug.Log("Response ========> GetTableHandHistory :" + serverResponse);

            if (!string.IsNullOrEmpty(serverResponse))
            {
                histories = JsonUtility.FromJson<AllHandHistroy>("{\"histories\":" + serverResponse + "}");
                Debug.Log("histories list count:" + histories.histories.Length);
                FillDetailsFromPageNo();
            }
        }
    }

    private void Start()
    {

    }

    private void OpenScreen(string screenName)
    {
        if(screenName == "Details")
        {
            DetailsPanel.SetActive(true);
            SummaryPanel.SetActive(false);
            DetailsButtonImage.SetActive(false);
            SummaryButtonImage.SetActive(true);
        }
        else
        {
            DetailsPanel.SetActive(false);
            SummaryPanel.SetActive(true);
            DetailsButtonImage.SetActive(true);
            SummaryButtonImage.SetActive(false);
        }
    }

    private IEnumerator LoadDemoCustomJSON(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            webRequest.SetRequestHeader("secret-key", "$2b$10$xXEJFkicTzXmf7wmrMCquuI/hP31tR8Wj/B1o0laX2Pf8tk20E9zq");
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
            {
                Debug.Log(": Error: " + webRequest.error);
            }
            else
            {
                Debug.Log(":\nReceived: " + webRequest.downloadHandler.text);
                histories = JsonUtility.FromJson<AllHandHistroy>("{\"histories\":" + webRequest.downloadHandler.text + "}");
                Debug.Log("histories list count:" + histories.histories.Length);
                FillDetailsFromPageNo();
            }
        }
    }

    private void ClearPage()
    {
        if(HandDetailsContent.childCount > 0)
        {
            for(int i =0; i< HandDetailsContent.childCount; i++)
            {
                Destroy(HandDetailsContent.GetChild(i).gameObject);
            }
        }
        if (HandSummaryContent.childCount > 0)
        {
            for (int i = 0; i < HandSummaryContent.childCount; i++)
            {
                Destroy(HandSummaryContent.GetChild(i).gameObject);
            }
        }
    }

    private void FillDetailsFromPageNo()
    {
        ClearPage();

        totalPages = histories.histories.Length;
        int val = totalPages - 1;
        PageNoText.text = pageNo + "/" + val;

        if (totalPages > 0)
        {
            LoadingText.SetActive(false);
            TimeText.text = histories.histories[pageNo].matchDetails.gameTime;
            SB_BB.text = histories.histories[pageNo].matchDetails.blind;
            //HandSummary
            for (int j = 0; j < histories.histories[pageNo].handSummary.Count; j++)
            {
                GameObject gm = Instantiate(HandSummaryItemPrefab, HandSummaryContent) as GameObject;
                gm.SetActive(true);
                gm.GetComponent<HandSummaryItemControl>().Init(histories.histories[pageNo].handSummary[j]);
            }

            //HandDetails
            //preflop
            if (histories.histories[pageNo].handDetails.PREFLOP.Count > 0)
            {
                GameObject gm = Instantiate(HandDetailRoundPrefab, HandDetailsContent) as GameObject;
                gm.SetActive(true);
                gm.GetComponent<RoundHeading>().Init("preflop", histories.histories[pageNo].handDetails);

                for (int h = 0; h < histories.histories[pageNo].handDetails.PREFLOP.Count; h++)
                {
                    GameObject details1 = Instantiate(PlayerDetailsPrefab, HandDetailsContent) as GameObject;
                    details1.SetActive(true);
                    details1.GetComponent<PlayerDeatilsControl>().Init("preflop", histories.histories[pageNo].handDetails,
                        h);
                }
            }
            //postflop
            if (histories.histories[pageNo].handDetails.POSTFLOP.Count > 0)
            {
                GameObject gm1 = Instantiate(HandDetailRoundPrefab, HandDetailsContent) as GameObject;
                gm1.SetActive(true);
                gm1.GetComponent<RoundHeading>().Init("postflop", histories.histories[pageNo].handDetails);

                for (int h = 0; h < histories.histories[pageNo].handDetails.POSTFLOP.Count; h++)
                {
                    GameObject details2 = Instantiate(PlayerDetailsPrefab, HandDetailsContent) as GameObject;
                    details2.SetActive(true);
                    details2.GetComponent<PlayerDeatilsControl>().Init("postflop", histories.histories[pageNo].handDetails,
                        h);
                }
            }
            //turn
            if (histories.histories[pageNo].handDetails.POSTTURN.Count > 0)
            {
                GameObject gm2 = Instantiate(HandDetailRoundPrefab, HandDetailsContent) as GameObject;
                gm2.SetActive(true);
                gm2.GetComponent<RoundHeading>().Init("turn", histories.histories[pageNo].handDetails);

                for (int h = 0; h < histories.histories[pageNo].handDetails.POSTTURN.Count; h++)
                {
                    GameObject details2 = Instantiate(PlayerDetailsPrefab, HandDetailsContent) as GameObject;
                    details2.SetActive(true);
                    details2.GetComponent<PlayerDeatilsControl>().Init("turn", histories.histories[pageNo].handDetails,
                        h);
                }
            }
            //river
            if (histories.histories[pageNo].handDetails.POSTRIVER.Count > 0)
            {
                GameObject gm2 = Instantiate(HandDetailRoundPrefab, HandDetailsContent) as GameObject;
                gm2.SetActive(true);
                gm2.GetComponent<RoundHeading>().Init("river", histories.histories[pageNo].handDetails);

                for (int h = 0; h < histories.histories[pageNo].handDetails.POSTRIVER.Count; h++)
                {
                    GameObject details2 = Instantiate(PlayerDetailsPrefab, HandDetailsContent) as GameObject;
                    details2.SetActive(true);
                    details2.GetComponent<PlayerDeatilsControl>().Init("turn", histories.histories[pageNo].handDetails,
                        h);
                }
            }
        }
    }
}



[Serializable]
public class AllHandHistroy
{
    public HandHistoryData[] histories;
}

[Serializable]
public class HandHistoryData
{
    public List<HandSummary> handSummary = new List<HandSummary>();
    public HandDetails handDetails;
    public MatchDetails matchDetails;
}

[Serializable]
public class PreFlop
{
    public string roundName;
    public int currentSubRounds;
    public string userName;
    public string betType;
    public double amount;
    public List<string> playerCards = new List<string>();
    public List<string> openCards = new List<string>();
    public double currentPot;
    public string seatName;
}

[Serializable]
public class PostFlop {
    public string roundName;
    public int currentSubRounds;
    public string userName;
    public string betType;
    public double amount;
    public List<string> playerCards = new List<string>();
    public List<string> openCards = new List<string>();
    public double currentPot;
    public string seatName;
}

[Serializable]
public class PostTurn
{
    public string roundName;
    public int currentSubRounds;
    public string userName;
    public string betType;
    public double amount;
    public List<string> playerCards = new List<string>();
    public List<string> openCards = new List<string>();
    public double currentPot;
    public string seatName;
}

[Serializable]
public class PostRiver
{
    public string roundName;
    public int currentSubRounds;
    public string userName;
    public string betType;
    public double amount;
    public List<string> playerCards = new List<string>();
    public List<string> openCards = new List<string>();
    public double currentPot;
    public string seatName;
}

[Serializable]
public class ShowDown
{
    public string roundName;
    public int currentSubRounds;
    public string userName;
    public string betType;
    public double amount;
    public List<string> playerCards = new List<string>();
    public List<string> openCards = new List<string>();
    public double currentPot;
    public string seatName;
}

[Serializable]
public class Details
{
    public string roundName;
    public int currentSubRounds;
    public string userName;
    public string betType;
    public double amount;
    public List<string> playerCards = new List<string>();
    public List<string> openCards = new List<string>();
    public double currentPot;
}



[Serializable]
public class HandSummary
{
    public List<string> communityCard = new List<string>();
    public double betAmount;
    public double winAmount;
    public int userId;
    public bool isWin;
    public string userName;
    public int totalBet;
    public int totalCoins;
    public string name;
    public string discription;
    public List<string> possibleCards = new List<string>();
    public List<string> cards = new List<string>();
    public List<string> mergeCards = new List<string>();
    public string winBy;
    public string handStrength;
}

[Serializable]
public class HandDetails
{
    public List<PreFlop> PREFLOP = new List<PreFlop>();
    public List<PostFlop> POSTFLOP = new List<PostFlop>();
    public List<PostTurn> POSTTURN = new List<PostTurn>();
    public List<PostRiver> POSTRIVER = new List<PostRiver>();
    //public List<PostRiver> PORTRIVER = new List<PostRiver>();

}

[Serializable]
public class MatchDetails
{
    public string blind;
    public int players;
    public string bet;
    public int activePlayers;
    public string gameTime;
}

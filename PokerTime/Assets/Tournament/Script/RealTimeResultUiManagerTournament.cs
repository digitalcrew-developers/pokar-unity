using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Globalization;

public class RealTimeResultUiManagerTournament : MonoBehaviour
{
    public GameObject loadingText;
    public Text TimeText;

    public List<Button> menus = new List<Button>();
    public List<GameObject> bottomScreens = new List<GameObject>();

    [Header("Ranking Data")]
    public GameObject resultPrefab_Ranking;
    public Transform rankingObjContainer;
    
    [Header("Prizes Data")]
    public GameObject resultPrefab_Prizes;
    public Transform prizesObjContainer;

    [Header("Tables Data")]
    public GameObject resultPrefab_Tables;
    public Transform tablesObjContainer;
    public GameObject spectatorPrefab;
    public Transform spectatorContainer;
    public Text totalSpectatorText;

    [Header("Blinds Data")]
    public GameObject resultPrefab_Blinds;
    public Transform blindsObjContainer;

    private string tableId;

    [Header("Top Part")]
    public Text myPositionTextValue;
    public Text entriesTextValue;
    public Text prizePoolTextValue;
    public Text levelTextValue;
    public Text lateRegTextValue;
    public Text runningTextValue;
    public Text currentLevelTextValue;
    public Text nextLevelTextValue;

    [Header("Middle Part")]
    public Text avgStackTextValue;
    public Text totalBuyInsTextValue;
    public Text largestStackTextValue;
    public Text rebuysTextValue;
    public Text smallestStackTextValue;
    public Text addonsTextValue;

    [Header("RankingScreen/MyTitle")]
    public Text myTitleRanking;
    public Text myTitleNickName;
    public Text myTitleStack;
    public Text myTitleRA;
    public Text myTitleValue;

    string tableIdGlobalVar;

    public void OnOpen()
    {
        OnClickedOnMenu(0);

        //Old code
        /*TimeText.text = System.DateTime.Now.ToLocalTime().ToShortTimeString();
        tableId = GlobalGameManager.instance.GetRoomData().socketTableId;
        UnityEngine.Debug.Log("Table ID ;" + tableId);

        string requestData = "{\"tableId\":\"" + tableId + "\"}";

        WebServices.instance.SendRequest(RequestType.GetTableHandHistory, requestData, true, (requestType, serverResponse, isShowErrorMessage, errorMessage) =>
        {
            UnityEngine.Debug.LogError(serverResponse);
            if (errorMessage.Length > 0)
            {
                //InGameUiManager.instance.ShowMessage(errorMessage);
            }
            else
            {
                JsonData data = JsonMapper.ToObject(serverResponse);
                if (data["status"].Equals(true))
                {
                    for (int i = 0; i < rankingObjContainer.childCount; i++)
                    {
                        Destroy(rankingObjContainer.GetChild(i).gameObject);
                    }

                    for (int i = 0; i < spectatorContainer.childCount; i++)
                    {
                        Destroy(spectatorContainer.GetChild(i).gameObject);
                    }

                    for (int i=0;i< data["data"]["realTimeArr"].Count; i++)
                    {
                        loadingText.SetActive(false);
                        GameObject gm = Instantiate(resultPrefab_Ranking, rankingObjContainer) as GameObject;
                        gm.SetActive(true);
                        gm.transform.Find("Name").GetComponent<TMPro.TextMeshProUGUI>().text = data["data"]["realTimeArr"][i]["userName"].ToString();
                        gm.transform.Find("BuyIn").GetComponent<TMPro.TextMeshProUGUI>().text = data["data"]["realTimeArr"][i]["totalCoinInGame"].ToString();
                        gm.transform.Find("Winnings").GetComponent<TMPro.TextMeshProUGUI>().text = data["data"]["realTimeArr"][i]["totalWinCoin"].ToString();

                        Debug.Log(PrefsManager.GetPlayerData().userName + " = " + data["data"]["realTimeArr"][i]["userName"].ToString());
                        if (PrefsManager.GetPlayerData().userName == data["data"]["realTimeArr"][i]["userName"].ToString())
                            gm.transform.Find("Winnings").GetComponent<TMPro.TextMeshProUGUI>().color = Color.yellow;
                        else
                        {
                            if (data["data"]["realTimeArr"][i]["totalWinCoin"].ToString().Contains("-"))
                            {
                                gm.transform.Find("Winnings").GetComponent<TMPro.TextMeshProUGUI>().color = Color.red;
                            }
                            else
                            {
                                gm.transform.Find("Winnings").GetComponent<TMPro.TextMeshProUGUI>().color = Color.green;
                                if(float.Parse(data["data"]["realTimeArr"][i]["totalWinCoin"].ToString()) > 0)
                                    gm.transform.Find("Winnings").GetComponent<TMPro.TextMeshProUGUI>().text = "+" + data["data"]["realTimeArr"][i]["totalWinCoin"].ToString();
                            }
                        }
                    }
                    if(data["data"]["standoutArr"].Count > 0)
                    {
                        for (int i = 0; i < data["data"]["standoutArr"].Count; i++)
                        {
                            GameObject gm = Instantiate(spectatorPrefab, spectatorContainer) as GameObject;
                            gm.GetComponent<DownloadAvatar>().avatarUrl = data["data"]["standoutArr"][i]["profileImage"].ToString();
                            gm.transform.Find("Text").GetComponent<Text>().text = data["data"]["standoutArr"][i]["userName"].ToString();
                            gm.SetActive(true);
                        }
                    }
                }
                else
                {
                    TournamentInGameUiManager.instance.ShowMessage(data["message"].ToString());
                }
            }
        });*/

        string tourneyId = TournamentSocketController.instance.TOURNEY_ID;
        string tableId = TournamentSocketController.instance.GetTableID();
        string userId = PlayerManager.instance.GetPlayerGameData().userId;
        GetTournamentDetails(tourneyId, userId, tableId);

        tableIdGlobalVar = tableId;
        StartCoroutine(SetTimer(360, "06:00"));
    }

    public void GetTournamentDetails(string tourneyId, string userId, string tableId)
    {
        if (!string.IsNullOrEmpty(tourneyId) && !string.IsNullOrEmpty(userId))
        { 
            string requestData = "{\"tourneyId\":\"" + tourneyId + "\", \"userId\":\"" + userId + "\", \"tableId\":\"" + tableId + "\"}";
            Debug.Log("from real time ui manager requestData=" + requestData);
            StartCoroutine(WebServices.instance.POSTRequestData("http://dcpoker.dcgames.co.in:3335/tournament/details", requestData, OnGetTournamentDetailsComplete)); //http://3.109.177.149
        }
    }

    private void OnGetTournamentDetailsComplete(string serverResponse, bool isErrorMessage, string errorMessage)
    {
        if (isErrorMessage)
        {
            Debug.Log("error=" + errorMessage);
        }
        else
        {
            Debug.Log("Response => OnGetTournamentDetailsComplete: " + serverResponse);

            JsonData jsonData = JsonMapper.ToObject(serverResponse);

            IDictionary dictionary = jsonData;      //DEV_CODE Converting to dictionary for checking key availability

            if (jsonData["status"].Equals(true))
            {
                IDictionary dictionaryData = jsonData["data"];

                myPositionTextValue.text = jsonData["data"]["position"].ToString();
                entriesTextValue.text = jsonData["data"]["entries"].ToString();
                prizePoolTextValue.text = jsonData["data"]["prize_pool"].ToString();
                levelTextValue.text = jsonData["data"]["level"].ToString();
                lateRegTextValue.text = jsonData["data"]["late_reg"].ToString();
                //runningTextValue.text = jsonData["data"][""].ToString();
                currentLevelTextValue.text = jsonData["data"]["current_level"].ToString();
                nextLevelTextValue.text = jsonData["data"]["next_level"].ToString();

                // avgStackTextValue.text = jsonData["data"]["avg_stack"].ToString();
                if (dictionaryData.Contains("tables")) {
                    for (int i = 0; i < jsonData["data"]["tables"].Count; i++)
                    {
                        if (jsonData["data"]["tables"][i]["tableId"].ToString().Equals(tableIdGlobalVar))
                        {
                            avgStackTextValue.text = Decimal.Truncate(Convert.ToDecimal(jsonData["data"]["tables"][i]["av_stack"].ToString())).ToString();
                            break;
                        }
                    }
                } else {
                    avgStackTextValue.text = "0";
                }
                totalBuyInsTextValue.text = jsonData["data"]["total_buyin"].ToString();
                largestStackTextValue.text = jsonData["data"]["larget_stack"].ToString();
                rebuysTextValue.text = jsonData["data"]["rebuy"].ToString();
                smallestStackTextValue.text = jsonData["data"]["smallest_stack"].ToString();
                addonsTextValue.text = jsonData["data"]["add_on"].ToString();

                try {
                    string str = jsonData["data"]["game_start"].ToString();
                    str = str.Substring(0, str.IndexOf('(') - 1);
                    DateTime strToDt = DateTime.ParseExact(str, "ddd MMM dd yyyy HH:mm:ss 'GMT'K", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
                    StartCoroutine(SetRunningTimerContinue(strToDt));
                } catch(Exception e) {
                    Debug.Log("game start timer error: "+e.Message);
                }

                for (int i = 0; i < rankingObjContainer.childCount; i++)
                {
                    Destroy(rankingObjContainer.GetChild(i).gameObject);
                }
                for (int i = 0; i < prizesObjContainer.childCount; i++)
                {
                    Destroy(prizesObjContainer.GetChild(i).gameObject);
                }
                for (int i = 0; i < tablesObjContainer.childCount; i++)
                {
                    Destroy(tablesObjContainer.GetChild(i).gameObject);
                }
                for (int i = 0; i < blindsObjContainer.childCount; i++)
                {
                    Destroy(blindsObjContainer.GetChild(i).gameObject);
                }

                int rankingDataCount = jsonData["data"]["ranking"].Count;

                if (rankingDataCount > 0)
                {
                    for (int i = 0; i < rankingDataCount; i++)
                    {
                        GameObject rankingObj = Instantiate(resultPrefab_Ranking, rankingObjContainer);
                        rankingObj.name = rankingObj.name + "_" + i;
                        rankingObj.transform.Find("Ranking").GetComponent<Text>().text = jsonData["data"]["ranking"][i]["rank"].ToString();
                        rankingObj.transform.Find("NickName").GetComponent<Text>().text = jsonData["data"]["ranking"][i]["nickName"].ToString();

                        rankingObj.transform.Find("Stack").GetComponent<Text>().text = jsonData["data"]["ranking"][i]["currentSessionPoint"].ToString();
                        //rankingObj.transform.Find("R+A").GetComponent<Text>().text = "";
                        //rankingObj.transform.Find("Value").GetComponent<Text>().text = "";

                        if(jsonData["data"]["ranking"][i]["userId"].ToString()==PlayerManager.instance.GetPlayerGameData().userId)
                        {
                            myTitleRanking.text = jsonData["data"]["ranking"][i]["rank"].ToString();
                            myTitleNickName.text = jsonData["data"]["ranking"][i]["nickName"].ToString();
                            myTitleStack.text = jsonData["data"]["ranking"][i]["currentSessionPoint"].ToString();
                            myTitleRA.text = "";
                            myTitleValue.text = "";
                        }
                    }
                }

                int prizeStructureDataCount = jsonData["data"]["prize_structure"].Count;

                if (prizeStructureDataCount > 0)
                {
                    for (int i = 0; i < prizeStructureDataCount; i++)
                    {
                        GameObject prizeObj = Instantiate(resultPrefab_Prizes, prizesObjContainer);
                        prizeObj.name = prizeObj.name + "_" + i;
                        //prizeObj.transform.Find("Ranking").GetComponent<Text>().text = "";
                        //prizeObj.transform.Find("Score").GetComponent<Text>().text = "";
                        //prizeObj.transform.Find("Hand/Text").GetComponent<Text>().text = "";
                        //prizeObj.transform.Find("Timer/Text").GetComponent<Text>().text = "";
                        //prizeObj.transform.Find("Info/Text").GetComponent<Text>().text = "";
                        //prizeObj.transform.Find("Dollar/Text").GetComponent<Text>().text = "";

                    }
                }

                int tableStructureDataCount = jsonData["data"]["tables"].Count;
                
                if (tableStructureDataCount > 0)
                {
                    for (int i = 0; i < tableStructureDataCount; i++)
                    {
                        GameObject tableObj = Instantiate(resultPrefab_Tables, tablesObjContainer);
                        tableObj.name = tableObj.name + "_" + i;
                        tableObj.transform.Find("Number/Text").GetComponent<Text>().text = "No." + (i + 1);
                        tableObj.transform.Find("UserIcon/Text").GetComponent<Text>().text = jsonData["data"]["tables"][i]["players_count"].ToString();
                        tableObj.transform.Find("Coin/Text").GetComponent<Text>().text = jsonData["data"]["tables"][i]["total_stack"].ToString() + "/" + jsonData["data"]["tables"][i]["av_stack"].ToString();
                    }
                }

                int blindStructureDataCount = jsonData["data"]["blind_structure"].Count;

                if (blindStructureDataCount > 0)
                {
                    for (int i = 0; i < blindStructureDataCount; i++)
                    {
                        GameObject blindObj = Instantiate(resultPrefab_Blinds, blindsObjContainer);
                        blindObj.name = blindObj.name + "_" + i;
                        blindObj.transform.Find("Level").GetComponent<Text>().text = jsonData["data"]["blind_structure"][i]["level"].ToString();
                        blindObj.transform.Find("Blinds").GetComponent<Text>().text = Utility.GetTrimmedAmount(jsonData["data"]["blind_structure"][i]["sb"].ToString()) + "/" + Utility.GetTrimmedAmount(jsonData["data"]["blind_structure"][i]["bb"].ToString());
                        blindObj.transform.Find("Ante").GetComponent<Text>().text = jsonData["data"]["blind_structure"][i]["ante"].ToString();
                    }
                }

                OnClickedOnMenu(0);
            }
        }
    }

    IEnumerator SetTimer(int totalSeconds, string startingTimerString)
    {
        DateTime afterMin = DateTime.Now.AddSeconds((double) totalSeconds);
        TimeText.text = startingTimerString;

        for (int i = totalSeconds; i >= 0; i--)
        {
            TimeSpan difference1 = DateTime.Now.Subtract(afterMin);
            if (i == 0)
            {
                StartCoroutine(SetTimer(totalSeconds, startingTimerString));
                yield return null;
                break;
            }
            else
            {
                yield return new WaitForSecondsRealtime(1f);
                try
                {
                    TimeText.text = (difference1.Minutes * -1).ToString("D2") + ":" + (difference1.Seconds * -1).ToString("D2");
                }
                catch(Exception e){
                    Debug.Log("error in timerText: "+e.Message);
                }
            }
        }
    }

    IEnumerator SetRunningTimerContinue(DateTime gameStartTime)
    {
        DateTime givenTime = gameStartTime.ToLocalTime();

        TimeSpan differenceGameStart = DateTime.Now.Subtract(givenTime);
        int totalSecondsGameStart = (int)differenceGameStart.TotalSeconds;

        // if (totalSecondsGameStart < 0) { Debug.Log("game will be starts"); }
        if (totalSecondsGameStart == 0 || totalSecondsGameStart > 0) {
            //Debug.Log("game already started");
            try {
                runningTextValue.text = (differenceGameStart.Hours).ToString("D2") + ":" + (differenceGameStart.Minutes).ToString("D2") + ":" + (differenceGameStart.Seconds).ToString("D2");
            }
            catch(Exception e) {
                Debug.Log("error in RunningTimer: "+e.Message);
            }

            TimeSpan timerTs = differenceGameStart;
            while(true)
            {
                yield return new WaitForSecondsRealtime(1f);
                try {
                    timerTs += TimeSpan.FromSeconds(1.0f);
                    runningTextValue.text = (timerTs.Hours).ToString("D2") + ":" + (timerTs.Minutes).ToString("D2") + ":" + (timerTs.Seconds).ToString("D2");
                }
                catch(Exception e) {
                    Debug.Log("error in RunningTimer: "+e.Message);
                }
            }
        }
    }

    public void OnClickOnButton(string eventName)
    {
        SoundManager.instance.PlaySound(SoundType.Click);

        switch (eventName)
        {
            case "back":
                {
                    if (InGameUiManager.instance != null)
                        InGameUiManager.instance.DestroyScreen(InGameScreens.RealTimeResult);
                    else if (ClubInGameUIManager.instance != null)
                        ClubInGameUIManager.instance.DestroyScreen(InGameScreens.RealTimeResult);
                    else if (TournamentInGameUiManager.instance != null)
                        TournamentInGameUiManager.instance.DestroyScreen(TournamentInGameScreens.RealTimeResult);
                }
                break;
          
            default:
                {
                    Debug.LogError("Unhandled eventName found in RealTimeResultUiManager = " + eventName);
                }
                break;
        }
    }

    public void OnClickedOnMenu(int index)
    {
        SoundManager.instance.PlaySound(SoundType.Click);

        for (int i = 0; i < menus.Count; i++)
        {
            if (i == index)
            {
                menus[i].interactable = false;
                menus[i].gameObject.GetComponent<Image>().enabled = true;
                bottomScreens[i].SetActive(true);
            }
            else
            {
                menus[i].interactable = true;
                menus[i].gameObject.GetComponent<Image>().enabled = false;
                bottomScreens[i].SetActive(false);
            }
        }
    }

    private void OnDestroy() {
        StopCoroutine("SetRunningTimerContinue");
        StartCoroutine("SetTimer");
    }
}

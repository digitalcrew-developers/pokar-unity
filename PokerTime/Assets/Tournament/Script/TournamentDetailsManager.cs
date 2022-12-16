using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class TournamentDetailsManager : MonoBehaviour
{
    public static TournamentDetailsManager instance;
    public List<Button> menus = new List<Button>();
    public List<GameObject> screens = new List<GameObject>();
    public List<Sprite> btnSprites = new List<Sprite>();

    public Button button;

    [Header("Details Panel")]
    public Text tourneyName;
    public Text timerText, startTime, blindUpData, lateRegistrationData, currentLevel;
    public TMP_Text prizePoolData, totalBuyIn, entryValue, entriyRange, startingChips, avgStack, rebuy, addOn, breakVal;

    [Header("Entries Panel")]
    public Transform entriesContainer;
    public GameObject entryObject;
    public Text enrolledValue;

    [Header("Table Panel")]
    public Transform tablesContainer;
    public GameObject tableObject;
    public Text tablesLeftValue;

    [Header("Prizes Panel")]
    public Transform prizeContainer;
    public GameObject prizeObject, infoObject;

    [Header("Prizes Panel")]
    public GameObject registrationBtn;
    public TMP_Text registrationBtnText;
    public TMP_Text statusText;

    int forLoopIndex = -1;

    private bool isTimerStart = false;
    private float timeRemaining = 0.0f;

    private void OnEnable()
    {
        OnClickedOnMenu(0);
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;

        //Debug.Log("User Name:" + "ajdnchdueydhngjeik".Remove(13));
    }

    private void Update()
    {
        if(isTimerStart)
        {
            if(timeRemaining > 1)
            {
                timeRemaining -= Time.deltaTime;
                DisplayTime(timeRemaining);
            }
            else
            {
                Debug.Log("Time has run out!");
                timeRemaining = 0;
                isTimerStart = false;
            }
        }
    }

    private void DisplayTime(float timeToDisplay)
    {
        TimeSpan t = TimeSpan.FromSeconds(timeToDisplay);
        timerText.text = t.ToString(@"hh\:mm\:ss");
    }

    public void Initialize(int id, int forLoopIndex)
    {
        this.forLoopIndex = forLoopIndex;
        string requestData = "{\"tourneyId\":\"" + id + "\"}";
        StartCoroutine(WebServices.instance.POSTRequestData("http://dcpoker.dcgames.co.in:3335/tournament/details", requestData, OnGetTourneyDetailsComplete)); //http://3.109.177.149
    }

    private void OnGetTourneyDetailsComplete(string serverResponse, bool isErrorMessage, string errorMessage)
    {
        if (isErrorMessage)
        {
            Debug.Log("error=" + errorMessage);
        }
        else
        {
            Debug.Log("Response => OnGetStreamDetails: " + serverResponse);

            JsonData jsonData = JsonMapper.ToObject(serverResponse);

            IDictionary dictionary = jsonData["data"];      //DEV_CODE Converting to dictionary for checking key availability

            string status = "Already Started..";
            try {
                if (forLoopIndex > -1) {
                    Transform row = TournamentLobbyUiManager.instance.container.GetChild(forLoopIndex);
                    GameObject registerBtn = row.GetChild(11).gameObject;
                    GameObject joinBtn = row.GetChild(12).gameObject;
                    GameObject lateRegisterBtn = row.GetChild(13).gameObject;
                    GameObject observeBtn = row.GetChild(14).gameObject;
                    GameObject enrolledBtn = row.GetChild(15).gameObject;

                    if(registerBtn.activeSelf) {
                        status = "Register";
                        registrationBtn.GetComponent<Button>().onClick.AddListener(() => {
                            registerBtn.GetComponent<Button>().onClick.Invoke();
                            if (TournamentInGameUiManager.instance != null) {
                                TournamentInGameUiManager.instance.DestroyScreen(TournamentInGameScreens.TournamentDetails);
                            }
                        });
                    } else if(joinBtn.activeSelf) {
                        registrationBtnText.text = "Join";
                        status = "Join";
                        registrationBtn.GetComponent<Button>().onClick.AddListener(() => {
                            joinBtn.GetComponent<Button>().onClick.Invoke();
                            if (TournamentInGameUiManager.instance != null) {
                                TournamentInGameUiManager.instance.DestroyScreen(TournamentInGameScreens.TournamentDetails);
                            }
                        });
                    } else if(lateRegisterBtn.activeSelf) {
                        registrationBtnText.text = "Late Reg.";
                        status = "Late Reg.";
                        registrationBtn.GetComponent<Button>().onClick.AddListener(() => {
                            lateRegisterBtn.GetComponent<Button>().onClick.Invoke();
                            if (TournamentInGameUiManager.instance != null) {
                                TournamentInGameUiManager.instance.DestroyScreen(TournamentInGameScreens.TournamentDetails);
                            }
                        });
                    } else if(observeBtn.activeSelf) {
                        registrationBtnText.text = "Observe";
                        status = "Observe";
                        registrationBtn.GetComponent<Button>().onClick.AddListener(() => {
                            observeBtn.GetComponent<Button>().onClick.Invoke();
                            if (TournamentInGameUiManager.instance != null) {
                                TournamentInGameUiManager.instance.DestroyScreen(TournamentInGameScreens.TournamentDetails);
                            }
                        });
                    } else if(enrolledBtn.activeSelf) {
                        status = enrolledBtn.transform.GetChild(0).GetComponent<Text>().text;
                        registrationBtn.SetActive(false);
                        statusText.text = status;
                        statusText.gameObject.SetActive(true);
                    }
                }
            } catch (Exception e) {
                Debug.Log("button status error: "+e.Message);
            }
            Debug.Log("status="+status);

            if (jsonData["status"].Equals(true))
            {
                tourneyName.text = jsonData["data"]["name"].ToString();
                timerText.text = "";
            
                string dateTime = jsonData["data"]["game_start"].ToString().Substring(4, 17);

                string[] array = dateTime.Split(' ');
                int month = DateTime.ParseExact(array[0], "MMM", CultureInfo.CurrentCulture).Month;

                DateTime dt = new DateTime(int.Parse(array[2]), month, int.Parse(array[1]), int.Parse(array[3].Substring(0, 2)), int.Parse(array[3].Substring(3, 2)), 0).ToLocalTime();

                TimeSpan t = dt - DateTime.Now;
                if(dt > DateTime.Now)
                {
                    if (t.Days > 0)
                    {
                        timerText.text = t.Days + " days to start...";
                    }
                    else
                    {
                        timeRemaining = (float)t.TotalSeconds;
                        isTimerStart = true;
                    }
                }
                else
                {
                    // timerText.text = "Already Started..";
                    timerText.text = status;
                }

                startTime.text = "Start Time: " + (dt.Day.ToString().Length < 2 ? "0" + dt.Day.ToString() : dt.Day.ToString()) + "/" +
                                                   (dt.Month.ToString().Length < 2 ? "0" + dt.Month.ToString() : dt.Month.ToString()) + " " +
                                                   (dt.Hour.ToString().Length < 2 ? "0" + dt.Hour.ToString() : dt.Hour.ToString()) + ":" +
                                                   (dt.Minute.ToString().Length < 2 ? "0" + dt.Minute.ToString() : dt.Minute.ToString());

                //startTime.text = "Start Time: " + month + "/" + array[2].Substring(2, 2) + " " + array[3].Substring(0, 2) + ":" + array[3].Substring(3, 2);
                blindUpData.text = "";
                lateRegistrationData.text = "Level " + jsonData["data"]["late_reg"].ToString();
                currentLevel.text = jsonData["data"]["level"].ToString();
                prizePoolData.text = jsonData["data"]["prize_pool"].ToString();
                totalBuyIn.text = "<sprite=0> " + jsonData["data"]["total_buyin"].ToString();
                entryValue.text = jsonData["data"]["default_stack"].ToString();
                entriyRange.text = jsonData["data"]["smallest_stack"].ToString() + "-" + jsonData["data"]["larget_stack"].ToString();
                startingChips.text = jsonData["data"]["entry_chip_amt"].ToString();
                if (dictionary.Contains("avg_stack"))
                    avgStack.text = jsonData["data"]["avg_stack"].ToString();
                else
                    avgStack.text = "";
                rebuy.text = jsonData["data"]["rebuy"].ToString();
                addOn.text = (int.Parse(jsonData["data"]["add_on"].ToString()) > 0 ? jsonData["data"]["add_on"].ToString() + "x starting chips" : "0");
                breakVal.text = "No";

                int tableCount = jsonData["data"]["tables"].Count;
                //Debug.Log("Tables: " + tableCount + " available..");
                if (tableCount > 0)
                {
                    for (int i = 1; i < tablesContainer.childCount; i++)
                    {
                        Destroy(tablesContainer.GetChild(i).gameObject);
                    }

                    for (int i = 0; i < tableCount; i++)
                    {
                        GameObject obj = Instantiate(tableObject, tablesContainer);
                        obj.SetActive(true);
                        obj.transform.Find("Number/Text").GetComponent<Text>().text = "No." + (i + 1);
                        obj.transform.Find("UserIcon/Text").GetComponent<Text>().text = jsonData["data"]["tables"][i]["players_count"].ToString();
                        obj.transform.Find("Coin/Text").GetComponent<Text>().text = jsonData["data"]["tables"][i]["total_stack"].ToString() + "/" + jsonData["data"]["tables"][i]["av_stack"].ToString();
                    }
                }

                int rankingCount = jsonData["data"]["ranking"].Count;
                //Debug.Log("Ranking: " + rankingCount + " available..");
                if (rankingCount > 0)
                {
                    screens[1].transform.Find("Titles/EnrolledLabel/EnrolledValue").GetComponent<Text>().text = rankingCount.ToString();

                    for (int i = 1; i < entriesContainer.childCount; i++)
                    {
                        Destroy(entriesContainer.GetChild(i).gameObject);
                    }

                    for (int i = 0; i < rankingCount; i++)
                    {
                        IDictionary ranking = jsonData["data"]["ranking"][i];

                        GameObject obj = Instantiate(entryObject, entriesContainer);
                        obj.SetActive(true);

                        //if(jsonData["data"]["ranking"][i]["profileImage"] != null || jsonData["data"]["ranking"][i]["profileImage"].ToString().Length > 0)

                        if (ranking.Contains("nickName")) {
                            string userName = jsonData["data"]["ranking"][i]["nickName"].ToString();
                            obj.transform.Find("Username").GetComponent<Text>().text = (userName.Length > 13 ? userName.Remove(13) + "..." : userName);
                        } else if (ranking.Contains("userName")) {
                            string userName = jsonData["data"]["ranking"][i]["userName"].ToString();
                            obj.transform.Find("Username").GetComponent<Text>().text = (userName.Length > 13 ? userName.Remove(13) + "..." : userName);
                        }
                        // try {
                        //     string userName = jsonData["data"]["ranking"][i]["nickName"].ToString();
                        //     obj.transform.Find("Username").GetComponent<Text>().text = (userName.Length > 13 ? userName.Remove(13) + "..." : userName);
                        // } catch (Exception e) {
                        //     Debug.Log("nickName not found");
                        // }

                        obj.transform.Find("UserID").GetComponent<Text>().text = jsonData["data"]["ranking"][i]["userId"].ToString();
                    }
                }

                int prizeCount = jsonData["data"]["prize_structure"].Count;
                //Debug.Log("Prizing: " + prizeCount + " available..");
                if (prizeCount > 0)
                {
                    for (int i = 1; i < prizeContainer.childCount; i++)
                    {
                        Destroy(prizeContainer.GetChild(i).gameObject);
                    }

                    for (int i = 0; i < prizeCount; i++)
                    {
                        GameObject obj = Instantiate(prizeObject, prizeContainer);
                        obj.SetActive(true);

                        //if(jsonData["data"]["ranking"][i]["profileImage"] != null || jsonData["data"]["ranking"][i]["profileImage"].ToString().Length > 0)
                    }
                    
                    GameObject gameObject = Instantiate(infoObject, prizeContainer);
                    gameObject.SetActive(true);
                }
            }
            else
            {
                
            }
        }
    }

    public void OnClickOnButton(int index)
    {
        switch (index)
        {
            case 0:
                if (TournamentInGameUiManager.instance != null)
                    TournamentInGameUiManager.instance.DestroyScreen(TournamentInGameScreens.TournamentDetails);
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
                menus[i].gameObject.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                screens[i].SetActive(true);
            }
            else
            {
                menus[i].interactable = true;
                //menus[i].gameObject.GetComponent<Image>().enabled = false;
                menus[i].gameObject.GetComponent<Image>().color = new Color32(255, 255, 255, 0);
                screens[i].SetActive(false);
            }
        }
    }
}

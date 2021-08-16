using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public void OnOpen()
    {
        OnClickedOnMenu(0);

        TimeText.text = System.DateTime.Now.ToLocalTime().ToShortTimeString();
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
        });
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
}

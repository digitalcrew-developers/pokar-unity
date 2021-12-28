using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RealTimeResultUiManager : MonoBehaviour
{
    public GameObject LoadingText1, LoadingText2;
    public GameObject resultPrefab;
    public GameObject spectatorPrefab;
    public Transform RealTimeContent;
    public Transform spectatorContent;
    public Text TimeText;

    private string tableId;

    public void OnOpen()
    {
        TimeText.text = System.DateTime.Now.ToLocalTime().ToShortTimeString();
        tableId = GlobalGameManager.instance.GetRoomData().socketTableId;
        UnityEngine.Debug.Log("Table ID ;" + tableId);

        string requestData = "{\"tableId\":\"" + tableId + "\"}";

        WebServices.instance.SendRequest(RequestType.RealtimeResult, requestData, true, (requestType, serverResponse, isShowErrorMessage, errorMessage) =>
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
                    for (int i = 0; i < RealTimeContent.childCount; i++)
                    {
                        Destroy(RealTimeContent.GetChild(i).gameObject);
                    }

                    for (int i = 0; i < spectatorContent.childCount; i++)
                    {
                        Destroy(spectatorContent.GetChild(i).gameObject);
                    }

                    for (int i = 0; i < data["data"]["realTimeArr"].Count; i++)
                    {
                        LoadingText1.SetActive(false);
                        LoadingText2.SetActive(false);
                        GameObject gm = Instantiate(resultPrefab, RealTimeContent) as GameObject;
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
                                if (float.Parse(data["data"]["realTimeArr"][i]["totalWinCoin"].ToString()) > 0)
                                    gm.transform.Find("Winnings").GetComponent<TMPro.TextMeshProUGUI>().text = "+" + data["data"]["realTimeArr"][i]["totalWinCoin"].ToString();
                            }
                        }
                    }
                    if (data["data"]["standoutArr"].Count > 0)
                    {
                        for (int i = 0; i < data["data"]["standoutArr"].Count; i++)
                        {
                            GameObject gm = Instantiate(spectatorPrefab, spectatorContent) as GameObject;
                            gm.GetComponent<DownloadAvatar>().avatarUrl = data["data"]["standoutArr"][i]["profileImage"].ToString();
                            gm.transform.Find("Text").GetComponent<Text>().text = data["data"]["standoutArr"][i]["userName"].ToString();
                            gm.SetActive(true);
                        }
                    }
                }
                else
                {
                    if(InGameUiManager.instance != null)
                        InGameUiManager.instance.ShowMessage(data["message"].ToString());
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
                {
                    ClubInGameUIManager.instance.canvas.sortingOrder = 0;
                    ClubInGameUIManager.instance.DestroyScreen(InGameScreens.RealTimeResult);
                    //Debug.Log("OnclickButton:..... BACK "+ InGameScreens.RealTimeResult + ", tableId:" + tableId);
                }    
            }
            break;

            default:
            {
                Debug.LogError("Unhandled eventName found in RealTimeResultUiManager = " + eventName);
            }
            break;
        }
    }
}

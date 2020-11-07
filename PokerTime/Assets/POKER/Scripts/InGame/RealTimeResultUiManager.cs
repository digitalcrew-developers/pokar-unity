using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RealTimeResultUiManager : MonoBehaviour
{
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
            UnityEngine.Debug.Log(serverResponse);
            if (errorMessage.Length > 0)
            {
                InGameUiManager.instance.ShowMessage(errorMessage);
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

                    for (int i=0;i< data["data"]["realTimeArr"].Count; i++)
                    {
                        GameObject gm = Instantiate(resultPrefab, RealTimeContent) as GameObject;
                        gm.SetActive(true);
                        gm.transform.Find("Name").GetComponent<TMPro.TextMeshProUGUI>().text = data["data"]["realTimeArr"][i]["userName"].ToString();
                        gm.transform.Find("BuyIn").GetComponent<TMPro.TextMeshProUGUI>().text = data["data"]["realTimeArr"][i]["totalCoinInGame"].ToString();
                        gm.transform.Find("Winnings").GetComponent<TMPro.TextMeshProUGUI>().text = data["data"]["realTimeArr"][i]["totalWinCoin"].ToString();

                        if (data["data"]["realTimeArr"][i]["totalWinCoin"].ToString().Contains("+"))
                        {
                            gm.transform.Find("Winnings").GetComponent<TMPro.TextMeshProUGUI>().color = Color.green;
                        }
                        if (data["data"]["realTimeArr"][i]["totalWinCoin"].ToString().Contains("-"))
                        {
                            gm.transform.Find("Winnings").GetComponent<TMPro.TextMeshProUGUI>().color = Color.red;
                        }else
                        {
                            gm.transform.Find("Winnings").GetComponent<TMPro.TextMeshProUGUI>().color = Color.yellow;
                        }                    
                    }

                    for (int i = 0; i < data["data"]["realTimeArr"].Count; i++)
                    {
                        GameObject gm = Instantiate(spectatorPrefab, spectatorContent) as GameObject;
                        gm.SetActive(true);

                        gm.transform.Find("Text").GetComponent<Text>().text = data["data"]["standoutArr"][i]["userName"].ToString();
                        
                    }
                }
                else
                {
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
                    InGameUiManager.instance.DestroyScreen(InGameScreens.RealTimeResult);
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClubCounter : MonoBehaviour
{
    public static ClubCounter instance;

    public Button StatsBtn;
    public Button TabTradeBtn, TabSleepModeBtn, TabVIPBtn, TabTicketBtn;
    public GameObject TradePanel, SleepPanel, VIPPanel, TicketPanel;

    public Button SendOutBtn, ClaimBackBtn, SetLimitBtn, RemoveLimitBtn, SendVIPBtn, AddTicketBtn;

    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
        }
    }

    private void Start()
    {
        Initialise();
    }

    private void Initialise()
    {
        TabTradeBtn.onClick.RemoveAllListeners();
        TabSleepModeBtn.onClick.RemoveAllListeners();
        TabVIPBtn.onClick.RemoveAllListeners();
        TabTicketBtn.onClick.RemoveAllListeners();

        TabTradeBtn.onClick.AddListener(() => OpenScreen("Trade"));
        TabSleepModeBtn.onClick.AddListener(() => OpenScreen("SleepMode"));
        TabVIPBtn.onClick.AddListener(() => OpenScreen("VIP"));
        TabTicketBtn.onClick.AddListener(() => OpenScreen("Ticket"));
    }

    private void OpenScreen(string screenName)
    {
        Color c = new Color(1, 1, 1, 1);
        Color c1 = new Color(1, 1, 1, 0);

        switch (screenName)
        {
            case "Trade":
                TabTradeBtn.GetComponent<Image>().color = c;
                TabSleepModeBtn.GetComponent<Image>().color = c1;
                TabVIPBtn.GetComponent<Image>().color = c1;
                TabTicketBtn.GetComponent<Image>().color = c1;

                TradePanel.SetActive(true);
                SleepPanel.SetActive(false);
                VIPPanel.SetActive(false);
                TicketPanel.SetActive(false);
                break;
            case "SleepMode":
                TabTradeBtn.GetComponent<Image>().color = c1;
                TabSleepModeBtn.GetComponent<Image>().color = c;
                TabVIPBtn.GetComponent<Image>().color = c1;
                TabTicketBtn.GetComponent<Image>().color = c1;

                TradePanel.SetActive(false);
                SleepPanel.SetActive(true);
                VIPPanel.SetActive(false);
                TicketPanel.SetActive(false);
                break;
            case "VIP":
                TabTradeBtn.GetComponent<Image>().color = c1;
                TabSleepModeBtn.GetComponent<Image>().color = c1;
                TabVIPBtn.GetComponent<Image>().color = c;
                TabTicketBtn.GetComponent<Image>().color = c1;

                TradePanel.SetActive(false);
                SleepPanel.SetActive(false);
                VIPPanel.SetActive(true);
                TicketPanel.SetActive(false);
                break;
            case "Ticket":
                TabTradeBtn.GetComponent<Image>().color = c1;
                TabSleepModeBtn.GetComponent<Image>().color = c1;
                TabVIPBtn.GetComponent<Image>().color = c1;
                TabTicketBtn.GetComponent<Image>().color = c;

                TradePanel.SetActive(false);
                SleepPanel.SetActive(false);
                VIPPanel.SetActive(false);
                TicketPanel.SetActive(true);
                break;
            default:
                break;
        }
    }

}

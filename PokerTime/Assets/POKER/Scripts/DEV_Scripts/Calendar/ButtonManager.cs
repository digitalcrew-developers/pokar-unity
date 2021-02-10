using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonManager : MonoBehaviour
{
    public static ButtonManager instance;
    
    #region Fields
    public TextMeshProUGUI label;

	private Button button;
	public UnityAction buttonAction;

	public static int currentDateIndex = 0;

    #endregion

    #region Private Methods
    private void Awake()
    {
        if (instance == null)
            instance = this;

        button = GetComponent<Button>();
    }

    private void OnDestroy()
    {
        //button.onClick.RemoveListener(buttonAction);
    }

    #endregion
    

	#region Public Methods

	public void Initialize(int day, int month, int year, int index, Action<(string, string)> clickEventHandler)
	{
		this.label.text = day.ToString();

        //To disable future dates
        if (day > DateTime.Now.Day && month == DateTime.Now.Month && year == DateTime.Now.Year)
        {
        }
        else
        {
            buttonAction += () => clickEventHandler((day.ToString(), day.ToString()));
			button.onClick.AddListener(buttonAction);
			button.onClick.AddListener(delegate
			{ HighlightMe(index, day, month, year); });
        }
    }

	public void HighlightMe(int index, int day, int month, int year)
	{
        //To Disselect all other dates
        for (int i = 0; i < BodyManager.totalCells.Count; i++)
        {
			if(i < currentDateIndex)
            {
                BodyManager.totalCells[i].transform.GetComponent<Image>().color = CalendarManager.instance.disselectedDateBGColor;
                BodyManager.totalCells[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.white;
            }

			if(i == currentDateIndex)
            {
                BodyManager.totalCells[i].transform.GetComponent<Image>().color = CalendarManager.instance.disselectedDateBGColor;
                BodyManager.totalCells[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.yellow;
			}
		}

        if (CalendarManager.previousClickIndex == 0)
        {
            CalendarManager.previousClickIndex = index;
            CalendarManager.nextClickIndex = index;

            CalendarManager.instance.startDate = year.ToString() + "." +
                                                 (month.ToString().Length == 1 ? "0" + month.ToString() : month.ToString()) + "." +
                                                 (day.ToString().Length == 1 ? "0" + day.ToString() : day.ToString());

            CalendarManager.instance.endDate = "";

            transform.GetComponent<Image>().color = CalendarManager.instance.selectedDateBGColor;
            this.label.color = CalendarManager.instance.selectedDateTextColor;
            return;
        }

        if (index < CalendarManager.previousClickIndex)
        {
            CalendarManager.previousClickIndex = index;

            CalendarManager.instance.startDate = year.ToString() + "." +
                                                 (month.ToString().Length == 1 ? "0" + month.ToString() : month.ToString()) + "." +
                                                 (day.ToString().Length == 1 ? "0" + day.ToString() : day.ToString());

            CalendarManager.instance.endDate = "";

            transform.GetComponent<Image>().color = CalendarManager.instance.selectedDateBGColor;
            this.label.color = CalendarManager.instance.selectedDateTextColor;
        }
        else if(index > CalendarManager.previousClickIndex)
        {
            CalendarManager.nextClickIndex = index;

            int distance = CalendarManager.nextClickIndex - CalendarManager.previousClickIndex;

            if (distance > CalendarManager.instance.daysToSelect)
            {
                BodyManager.totalCells[CalendarManager.previousClickIndex].transform.GetComponent<Image>().color = CalendarManager.instance.selectedDateBGColor;
                BodyManager.totalCells[CalendarManager.previousClickIndex].transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = CalendarManager.instance.selectedDateTextColor;

                if(CalendarManager.instance.popUpText != null)
                    StartCoroutine(CalendarManager.instance.ShowPopUp(1.29f));
            }
            else
            {
                for (int i = CalendarManager.previousClickIndex; i <= CalendarManager.nextClickIndex; i++)
                {
                    BodyManager.totalCells[i].transform.GetComponent<Image>().color = CalendarManager.instance.selectedDateBGColor;
                    BodyManager.totalCells[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = CalendarManager.instance.selectedDateTextColor;
                }

                CalendarManager.instance.endDate = year.ToString() + "." +
                                                   (month.ToString().Length == 1 ? "0" + month.ToString() : month.ToString()) + "." +
                                                   (day.ToString().Length == 1 ? "0" + day.ToString() : day.ToString());

                CalendarManager.previousClickIndex = 0;
                CalendarManager.nextClickIndex = 0;
            }
        }
        else
        {
            CalendarManager.previousClickIndex = 0;
            CalendarManager.nextClickIndex = 0;

            CalendarManager.instance.startDate = "";
            CalendarManager.instance.endDate = "";

            transform.GetComponent<Image>().color = CalendarManager.instance.selectedDateBGColor;
            this.label.color = CalendarManager.instance.selectedDateTextColor;
        }
    }

	#endregion	
}
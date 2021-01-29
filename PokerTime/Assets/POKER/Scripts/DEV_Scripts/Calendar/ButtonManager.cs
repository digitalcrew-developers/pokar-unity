using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonManager : MonoBehaviour
{
	#region Fields

	[SerializeField]
	private TextMeshProUGUI label;

	private Button button;
	public UnityAction buttonAction;

	private static int clickCounter = 0;

	DateTime prevDate;

	#endregion

	#region Public Methods

	public void Initialize(int day, int month, int year, Action<(string, string)> clickEventHandler)
	{
		this.label.text = day.ToString();

		int today;
		int.TryParse(day.ToString(), out today);

		//To disable future dates
		if (day > DateTime.Now.Day && month == DateTime.Now.Month && year == DateTime.Now.Year)
		{
			this.label.color = Color.gray;
		}
		else
		{
			if(day == DateTime.Now.Day && month == DateTime.Now.Month && year == DateTime.Now.Year)
            {
				prevDate = new DateTime(year, month, day);

				this.label.color = Color.yellow;

				Color myColor;
				ColorUtility.TryParseHtmlString("#004D1F", out myColor);
				transform.GetComponent<Image>().color = myColor;
			}

			buttonAction += () => clickEventHandler((day.ToString(), day.ToString()));
			button.onClick.AddListener(buttonAction);
			button.onClick.AddListener(delegate
			{ HighlightMe(day, month, year); });
		}
	}

	public void HighlightMe(int day, int month, int year)
	{
		//if (prevDate == null)
		//	Debug.Log("Empty Date...");

		//Debug.Log("Current Click Counte: " + clickCounter);
		//if (clickCounter == 0)
		//{
			
		//	clickCounter++;
		//}
		//else if (clickCounter == 1)
		//	clickCounter = 0;

		//Debug.Log("Day is: " + day);
		//Debug.Log("Month is: " + month);
		//Debug.Log("Year is: " + year);

        //Disselect dates from 1st month 
        for (int i = 0; i < CalendarManager.instance.bodyManager1.transform.childCount; i++)
        {
			if(CalendarManager.instance.bodyManager1.transform.GetChild(i).GetComponent<Image>() != null)
            {
				CalendarManager.instance.bodyManager1.transform.GetChild(i).GetComponent<Image>().color = Color.black;
				CalendarManager.instance.bodyManager1.transform.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.white;
			}
        }

		//Disselect dates from 2nd month
		for (int i = 0; i < CalendarManager.instance.bodyManager2.transform.childCount; i++)
		{
			if (CalendarManager.instance.bodyManager2.transform.GetChild(i).GetComponent<Image>() != null)
			{
				CalendarManager.instance.bodyManager2.transform.GetChild(i).GetComponent<Image>().color = Color.black;
				CalendarManager.instance.bodyManager2.transform.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.white;
			}
		}

		//Disselect dates from 3rd month 
		for (int i = 0; i < CalendarManager.instance.bodyManager3.transform.childCount; i++)
		{
			if (CalendarManager.instance.bodyManager3.transform.GetChild(i).GetComponent<Image>() != null)
			{
				CalendarManager.instance.bodyManager3.transform.GetChild(i).GetComponent<Image>().color = Color.black;
				CalendarManager.instance.bodyManager3.transform.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.white;
			}
		}

		//Disselect dates from 4th month 
		for (int i = 0; i < CalendarManager.instance.bodyManager4.transform.childCount; i++)
		{
			if (CalendarManager.instance.bodyManager4.transform.GetChild(i).GetComponent<Image>() != null)
			{
				CalendarManager.instance.bodyManager4.transform.GetChild(i).GetComponent<Image>().color = Color.black;
				CalendarManager.instance.bodyManager4.transform.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.white;
			}
		}

		//Select current clicked button
		Color myColor;
		ColorUtility.TryParseHtmlString("#004D1F", out myColor);
		transform.GetComponent<Image>().color = myColor;
		this.label.color = Color.green;
	}

	#endregion

	#region Private Methods

	private void Awake()
	{
		button = GetComponent<Button>();
	}

	private void OnDestroy()
	{
		//button.onClick.RemoveListener(buttonAction);
	}

	#endregion
}

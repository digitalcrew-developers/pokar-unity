using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BodyManager : MonoBehaviour
{
	#region Fields

	[SerializeField]
	private GameObject buttonPrefab;

	[SerializeField]
	private GameObject placeHolderPrefab;

	private List<GameObject> cells;

    public static List<GameObject> totalCells;

    #endregion

    #region Public Methods

    public void Initialize(int year, int month, Action<(string, string)> clickEventHandler)
	{
		//Debug.Log("Year:" + year + " And Month:" + month);

	    var dateTime = new DateTime(year, month, 1);
		var daysInMonth = DateTime.DaysInMonth(year, month);
		var dayOfWeek = (int)dateTime.DayOfWeek;
		var size = (dayOfWeek + daysInMonth) / 7;
		
		if ((dayOfWeek + daysInMonth) % 7 > 0)
			size++;
		
		var arr = new int[size * 7];
		
		for (var i = 0; i < daysInMonth; i++)
			arr[dayOfWeek + i] = i + 1;

		if (cells == null)
			cells = new List<GameObject>();

		//if (totalCells == null)
		//	totalCells = new List<GameObject>();


		foreach (var c in cells)
			Destroy(c);

		cells.Clear();

		foreach (var day in arr)
		{
			var instance = Instantiate(day == 0 ? placeHolderPrefab : buttonPrefab, transform);
			var buttonManager = instance.GetComponent<ButtonManager>();

			if (instance.transform.childCount > 0)
				totalCells.Add(instance);

			if (buttonManager != null)
			{
				buttonManager.Initialize(day, month, year, totalCells.Count - 1, clickEventHandler);
			}

			cells.Add(instance);

			if (day > DateTime.Now.Day && month == DateTime.Now.Month && year == DateTime.Now.Year)
			{
				//Debug.Log("This is future date");
				buttonManager.label.color = Color.gray;
            }
			else
			{
				if (day == DateTime.Now.Day && month == DateTime.Now.Month && year == DateTime.Now.Year)
				{
					//Debug.Log("This is today date with count: " + (totalCells.Count-1));

					ButtonManager.currentDateIndex = totalCells.Count - 1;

					CalendarManager.previousClickIndex = ButtonManager.currentDateIndex;
					CalendarManager.nextClickIndex = ButtonManager.currentDateIndex;

					CalendarManager.instance.startDate = year.ToString() + "." +
														 (month.ToString().Length == 1 ? "0" + month.ToString() : month.ToString()) + "." +
														 (day.ToString().Length == 1 ? "0" + day.ToString() : day.ToString());

					CalendarManager.instance.endDate = "";

					buttonManager.label.color = Color.green;
					buttonManager.GetComponent<Image>().color = CalendarManager.instance.selectedDateColor;
                }
			}
		}
    }

	#endregion
}

using System;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CalendarManager : MonoBehaviour
{
	public static CalendarManager instance;
	#region Fields

	[SerializeField]
	private HeaderManager headerManager;

	[Header("Selectable Days")]
	public int daysToSelect;

	[Space(15)]

	public BodyManager bodyManager4;
	public BodyManager bodyManager3;
	public BodyManager bodyManager2;
	public BodyManager bodyManager1;

	public Text month4;
	public Text month3;
	public Text month2;
	public Text month1;

	private DateTime targetDateTime;
	private CultureInfo cultureInfo;

	public static int previousClickIndex = 0;
	public static int nextClickIndex = 0;

	public string startDate, endDate;

	public Color selectedDateColor;

	#endregion

	#region Public Methods

	public void OnGoToPreviousOrNextMonthButtonClicked(string param)
	{
		targetDateTime = targetDateTime.AddMonths(param == "Prev" ? -1 : 1);
		Refresh(targetDateTime.Year, targetDateTime.Month);
	}

    #endregion

    #region Private Methods

    private void Awake()
    {
		instance = this;

		ColorUtility.TryParseHtmlString("#004D1F", out selectedDateColor);

		startDate = endDate = "";
	}

    private void OnEnable()
	{
		if (BodyManager.totalCells == null)
			BodyManager.totalCells = new List<GameObject>();

		foreach (var c in BodyManager.totalCells)
			Destroy(c);

		BodyManager.totalCells.Clear();

		targetDateTime = DateTime.Now;
		cultureInfo = new CultureInfo("en-US");
		//Refresh(targetDateTime.Year, targetDateTime.Month);

        targetDateTime = targetDateTime.AddMonths(-3);
        month4.text = ((targetDateTime.Month.ToString().Length == 1) ? "0" + targetDateTime.Month.ToString() + "/" + targetDateTime.Year.ToString() : targetDateTime.Month.ToString() + "/" + targetDateTime.Year.ToString());
        bodyManager4.Initialize(targetDateTime.Year, targetDateTime.Month, OnButtonClicked);

        targetDateTime = targetDateTime.AddMonths(1);
        month3.text = ((targetDateTime.Month.ToString().Length == 1) ? "0" + targetDateTime.Month.ToString() + "/" + targetDateTime.Year.ToString() : targetDateTime.Month.ToString() + "/" + targetDateTime.Year.ToString());
        bodyManager3.Initialize(targetDateTime.Year, targetDateTime.Month, OnButtonClicked);

        targetDateTime = targetDateTime.AddMonths(1);
        month2.text = ((targetDateTime.Month.ToString().Length == 1) ? "0" + targetDateTime.Month.ToString() + "/" + targetDateTime.Year.ToString() : targetDateTime.Month.ToString() + "/" + targetDateTime.Year.ToString());
        bodyManager2.Initialize(targetDateTime.Year, targetDateTime.Month, OnButtonClicked);

        targetDateTime = targetDateTime.AddMonths(1);
        month1.text = ((targetDateTime.Month.ToString().Length == 1) ? "0" + targetDateTime.Month.ToString() + "/" + targetDateTime.Year.ToString() : targetDateTime.Month.ToString() + "/" + targetDateTime.Year.ToString());
        bodyManager1.Initialize(targetDateTime.Year, targetDateTime.Month, OnButtonClicked);
    }

	#endregion

	#region Event Handlers

	private void Refresh(int year, int month)
	{
		headerManager.SetTitle($"{year} {cultureInfo.DateTimeFormat.GetMonthName(month)}");
		bodyManager1.Initialize(year, month, OnButtonClicked);
	}

	private void OnButtonClicked((string day, string legend) param)
	{
		//tailManager.SetLegend($"You have clicked day {param.day}.");
		//Debug.Log("You Clicked On Date" + param.);
	}

	public void OnClickConfirmButton(TMP_Text dateText)
    {
		if (endDate.Equals(""))
			dateText.text = startDate + " - " + startDate;
			//ClubExportDataManager.instance.dateText.text = startDate + " - " + startDate;
		else
			dateText.text = startDate + " - " + endDate;
			//ClubExportDataManager.instance.dateText.text = startDate + " - " + endDate;

		startDate = endDate = "";
    }

	public void OnClickOnClose()
    {

    }
	#endregion
}

using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class CalendarManager : MonoBehaviour
{
	public static CalendarManager instance;
	#region Fields

	[SerializeField]
	private HeaderManager headerManager;

	//[SerializeField]
	public BodyManager bodyManager4;
	
	//[SerializeField]
	public BodyManager bodyManager3;

	//[SerializeField]
	public BodyManager bodyManager2;

	//[SerializeField]
	public BodyManager bodyManager1;

	[SerializeField]
	//private TailManager tailManager;

	public Text month4;
	public Text month3;
	public Text month2;
	public Text month1;

	private DateTime targetDateTime;
	private CultureInfo cultureInfo;

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
    }

    private void Start()
	{
		targetDateTime = DateTime.Now;
		cultureInfo = new CultureInfo("en-US");
		//Refresh(targetDateTime.Year, targetDateTime.Month);

		month1.text = ((targetDateTime.Month.ToString().Length == 1) ? "0" + targetDateTime.Month.ToString() + "/" + targetDateTime.Year.ToString() : targetDateTime.Month.ToString() + "/" + targetDateTime.Year.ToString());
		bodyManager1.Initialize(targetDateTime.Year, targetDateTime.Month, OnButtonClicked);

		targetDateTime = targetDateTime.AddMonths(-1);
		month2.text = ((targetDateTime.Month.ToString().Length == 1) ? "0" + targetDateTime.Month.ToString() + "/" + targetDateTime.Year.ToString() : targetDateTime.Month.ToString() + "/" + targetDateTime.Year.ToString());
		bodyManager2.Initialize(targetDateTime.Year, targetDateTime.Month, OnButtonClicked);

		targetDateTime = targetDateTime.AddMonths(-1);
		month3.text = ((targetDateTime.Month.ToString().Length == 1) ? "0" + targetDateTime.Month.ToString() + "/" + targetDateTime.Year.ToString() : targetDateTime.Month.ToString() + "/" + targetDateTime.Year.ToString());
		bodyManager3.Initialize(targetDateTime.Year, targetDateTime.Month, OnButtonClicked);

		targetDateTime = targetDateTime.AddMonths(-1);
		month4.text = ((targetDateTime.Month.ToString().Length == 1) ? "0" + targetDateTime.Month.ToString() + "/" + targetDateTime.Year.ToString() : targetDateTime.Month.ToString() + "/" + targetDateTime.Year.ToString());
		bodyManager4.Initialize(targetDateTime.Year, targetDateTime.Month, OnButtonClicked);
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

	#endregion
}

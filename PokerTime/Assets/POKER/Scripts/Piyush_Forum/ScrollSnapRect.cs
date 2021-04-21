using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System;
using LitJson;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Mask))]
[RequireComponent(typeof(ScrollRect))]
public class ScrollSnapRect : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler {

    public static ScrollSnapRect instance;

    public Text changeIndexTxt;
    public Text allTimeText;
    public Text currentWiseWinOrLoseText;
    public Text last7DaysText;

    public string containerScroll_Name;

    [Tooltip("Set starting page index - starting from 0")]
    public int startingPage = 0;
    [Tooltip("Threshold time for fast swipe in seconds")]
    public float fastSwipeThresholdTime = 0.3f;
    [Tooltip("Threshold time for fast swipe in (unscaled) pixels")]
    public int fastSwipeThresholdDistance = 100;
    [Tooltip("How fast will page lerp to target position")]
    public float decelerationRate = 10f;
    [Tooltip("Button to go to the previous page (optional)")]
    public GameObject prevButton;
    [Tooltip("Button to go to the next page (optional)")]
    public GameObject nextButton;
    [Tooltip("Sprite for unselected page (optional)")]
    public Sprite unselectedPage;
    [Tooltip("Sprite for selected page (optional)")]
    public Sprite selectedPage;
    [Tooltip("Container with page images (optional)")]
    public Transform pageSelectionIcons;

    // fast swipes should be fast and short. If too long, then it is not fast swipe
    private int _fastSwipeThresholdMaxLimit;

    private ScrollRect _scrollRectComponent;
    private RectTransform _scrollRectRect;
    private RectTransform _container;

    private bool _horizontal;
    
    // number of pages in container
    private int _pageCount;
    public int _currentPage;

    // whether lerping is in progress and target lerp position
    private bool _lerp;
    private Vector2 _lerpTo;

    // target position of every page
    private List<Vector2> _pagePositions = new List<Vector2>();

    // in draggging, when dragging started and where it started
    private bool _dragging;
    private float _timeStamp;
    private Vector2 _startPosition;

    // for showing small page icons
    private bool _showPageSelection;
    private int _previousPageSelectionIndex;
    // container with Image components - one Image for each page
    private List<Image> _pageSelectionImages;
    public GameObject careerListPrefab, careerDayListContainer, careerMonthListContainer, careerYearListContainer;
    GameObject listObj; // Add By GP

    [Space(10)]
    public Transform dayContainer, monthContainer, yearContainer;

    [Space(10)]
    public GameObject noContentPanel;

    private Transform graphData;

    public void Awake()
    {
        instance = this;
        containerScroll_Name = "DayScroll";
    }

    public void OnEnable()
    {
        //ChangeTxtVal();//Comment By GP
    }

    //------------------------------------------------------------------------
    void Start() {
        _scrollRectComponent = GetComponent<ScrollRect>();
        _scrollRectRect = GetComponent<RectTransform>();
        _container = _scrollRectComponent.content;
        _pageCount = _container.childCount;

        // is it horizontal or vertical scrollrect
        if (_scrollRectComponent.horizontal && !_scrollRectComponent.vertical) {
            _horizontal = true;
        } else if (!_scrollRectComponent.horizontal && _scrollRectComponent.vertical) {
            _horizontal = false;
        } else {
            Debug.LogWarning("Confusing setting of horizontal/vertical direction. Default set to horizontal.");
            _horizontal = true;
        }

        _lerp = false;

        // init
        UpdatePages();

        // prev and next buttons
        if (nextButton)
            nextButton.GetComponent<Button>().onClick.AddListener(() => { NextScreen(); });

        if (prevButton)
            prevButton.GetComponent<Button>().onClick.AddListener(() => { PreviousScreen(); });
	}

    public void ChangeTxtVal() 
    {
        string startDate = "", endDate = "";
        switch (this.gameObject.name)
        {
            case "YearScroll":
                {
                    changeIndexTxt.text = CareerManager.instance.currentYear.ToString();
                    yearContainer.GetChild(_currentPage).Find("Window_Graph").gameObject.SetActive(true);
                    startDate = CareerManager.instance.currentYear.ToString() + "-01-01";

                    if (CareerManager.instance.currentYear == DateTime.Now.Year)
                    {
                        endDate = CareerManager.instance.currentYear.ToString() + "-" +
                                    ((DateTime.Now.Month.ToString().Length == 1) ? "0" + DateTime.Now.Month.ToString() : DateTime.Now.Month.ToString()) + "-" +
                                    ((DateTime.Now.Day.ToString().Length == 1) ? "0" + DateTime.Now.Day.ToString() : DateTime.Now.Day.ToString());
                    }
                    else
                    {
                        endDate = CareerManager.instance.currentYear.ToString() + "-12-31";
                    }
                }
                break;
            case "MonthScroll":
                {
                    changeIndexTxt.text = CareerManager.instance.currentYear + "-" + ((CareerManager.instance.currentMonth.ToString().Length == 1) ? "0" + CareerManager.instance.currentMonth.ToString() : CareerManager.instance.currentMonth.ToString());
                    monthContainer.GetChild(_currentPage).Find("Window_Graph").gameObject.SetActive(true);

                    monthContainer.GetChild(_currentPage).Find("xAxis/M7").GetComponent<Text>().text = DateTime.DaysInMonth(CareerManager.instance.currentYear, CareerManager.instance.currentMonth).ToString();

                    startDate = CareerManager.instance.currentYear.ToString() + "-" +
                                ((CareerManager.instance.currentMonth.ToString().Length == 1) ? "0" + CareerManager.instance.currentMonth.ToString() : CareerManager.instance.currentMonth.ToString()) 
                                + "-01";
                    if (CareerManager.instance.currentMonth == DateTime.Now.Month)
                    {
                        endDate = CareerManager.instance.currentYear.ToString() + "-" +
                                    ((DateTime.Now.Month.ToString().Length == 1) ? "0" + DateTime.Now.Month.ToString() : DateTime.Now.Month.ToString()) + "-" +
                                    ((DateTime.Now.Day.ToString().Length == 1) ? "0" + DateTime.Now.Day.ToString() : DateTime.Now.Day.ToString());
                    }
                    else
                    {
                        int daysInMonth = DateTime.DaysInMonth(CareerManager.instance.currentYear, CareerManager.instance.currentMonth);
                        endDate = CareerManager.instance.currentYear.ToString() + "-" + 
                                  ((CareerManager.instance.currentMonth.ToString().Length == 1) ? "0" + CareerManager.instance.currentMonth.ToString() : CareerManager.instance.currentMonth.ToString()) + "-" + 
                                  daysInMonth;
                    }
                }
                break;
            case "DayScroll":
                {
                    changeIndexTxt.text = ((CareerManager.instance.currentMonth.ToString().Length == 1) ? "0" + CareerManager.instance.currentMonth.ToString() : CareerManager.instance.currentMonth.ToString()) + "/" +
                                          ((CareerManager.instance.currentDate.ToString().Length == 1) ? "0" + CareerManager.instance.currentDate.ToString() : CareerManager.instance.currentDate.ToString());
                    dayContainer.GetChild(_currentPage).Find("Window_Graph").gameObject.SetActive(true);

                    startDate = endDate = CareerManager.instance.currentYear + "-" +
                      ((CareerManager.instance.currentMonth.ToString().Length == 1) ? "0" + CareerManager.instance.currentMonth.ToString() : CareerManager.instance.currentMonth.ToString()) + "-" +
                      ((CareerManager.instance.currentDate.ToString().Length == 1) ? "0" + CareerManager.instance.currentDate.ToString() : CareerManager.instance.currentDate.ToString());
                }
                break;
        }
        //string date = CareerManager.instance.currentYear + "-" +
        //              ((CareerManager.instance.currentMonth.ToString().Length == 1) ? "0" + CareerManager.instance.currentMonth.ToString() : CareerManager.instance.currentMonth.ToString()) + "-" +
        //              ((CareerManager.instance.currentDate.ToString().Length == 1) ? "0" + CareerManager.instance.currentDate.ToString() : CareerManager.instance.currentDate.ToString());


        string requestData = "{\"userId\":" + int.Parse(PlayerManager.instance.GetPlayerGameData().userId) + "," +
                               "\"startDate\":\"" + startDate/*date*//*"2021-03-13"*/ + "\"," +
                               "\"endDate\":\"" + endDate/*date*//*"2021-03-13"*/ + "\"}";

        WebServices.instance.SendRequest(RequestType.CareerData, requestData, true, OnServerResponseFound);
    }

    public void OnServerResponseFound(RequestType requestType, string serverResponse, bool isShowErrorMessage, string errorMessage)
    {
        if (errorMessage.Length > 0)
        {
            if (isShowErrorMessage)
            {
                MainMenuController.instance.ShowMessage(errorMessage);
            }

            return;
        }
        
        if (requestType == RequestType.CareerData)
        {
            //Debug.Log("Response => CareerData : " + serverResponse + "   Total Data: " );
            
            JsonData data = JsonMapper.ToObject(serverResponse);
            Debug.Log("Response => CareerData : " + data.ToJson().ToString() + "   Total Data: " + data["data"].Count);

            float totalWinOrLose = 0.0f;

            if (containerScroll_Name.Equals("DayScroll"))
            {
                if (data["message"].Equals("Success"))
                {
                    for (int i = 0; i < careerDayListContainer.transform.childCount; i++)
                    {
                        Destroy(careerDayListContainer.transform.GetChild(i).gameObject);
                    }

                    if (data["data"].Count > 0)
                    {
                        //Clear old graph data
                        dayContainer.GetChild(_currentPage).Find("Window_Graph").GetComponent<GraphManager>().valueList.Clear();
                        dayContainer.GetChild(_currentPage).Find("Window_Graph").GetComponent<GraphManager>().xPosValueList.Clear();
                        noContentPanel.SetActive(false);

                        for (int i = 0; i < data["data"].Count; i++)
                        {
                            listObj = Instantiate(careerListPrefab, careerDayListContainer.transform);

                            listObj.transform.Find("bg Image/hand text").GetComponent<Text>().text = data["data"][i]["date"].ToString().Substring(5, 2) + "/" + data["data"][i]["date"].ToString().Substring(8, 2);

                            if (data["data"][i]["isClub"].Equals(false))
                            {
                                //Debug.Log("This is not club data...");
                                listObj.transform.Find("bg Image/win text").GetComponent<Text>().text = "Hand: " + data["data"][i]["total"].ToString();
                            }
                            else
                            {
                                //Debug.Log("This is club data...");
                                listObj.transform.Find("bg Image/win text").GetComponent<Text>().text = "Game: " + data["data"][i]["table"].ToString() + "\n" + "Hand: " + data["data"][i]["total"].ToString();
                            }

                            totalWinOrLose += float.Parse(data["data"][i]["profit"].ToString());

                            if (float.Parse(data["data"][i]["profit"].ToString()) > 0)
                            {
                                listObj.transform.Find("bg Image/loss text").GetComponent<Text>().color = Color.green;
                            }
                            else
                            {
                                listObj.transform.Find("bg Image/loss text").GetComponent<Text>().color = Color.red;
                            }

                            listObj.transform.Find("bg Image/loss text").GetComponent<Text>().text = data["data"][i]["profit"].ToString();

                            //Adding profit data to the value list
                            dayContainer.GetChild(_currentPage).Find("Window_Graph").GetComponent<GraphManager>().valueList.Add(int.Parse(data["data"][i]["profit"].ToString()));
                            //dayContainer.GetChild(_currentPage).Find("Window_Graph").GetComponent<GraphManager>().valueList.Add(int.Parse(data["data"][i]["betAmount"].ToString()));

                            //Adding dates from response to the xPosList
                            dayContainer.GetChild(_currentPage).Find("Window_Graph").GetComponent<GraphManager>().xPosValueList.Add(0);

                            //Calling Show Graph Method to generate graph
                            dayContainer.GetChild(_currentPage).Find("Window_Graph").GetComponent<GraphManager>().ShowGraph(dayContainer.GetChild(_currentPage).Find("Window_Graph").GetComponent<GraphManager>().valueList,
                                                                                                                            dayContainer.GetChild(_currentPage).Find("Window_Graph").GetComponent<GraphManager>().xPosValueList,
                                                                                                                            /*(int _i) => "" + i,*/ (float _f) => /*"$"*/ "" + Mathf.RoundToInt(_f),true);
                        }
                        if(totalWinOrLose >0)
                        {
                            allTimeText.color = currentWiseWinOrLoseText.color = last7DaysText.color = Color.green;
                            allTimeText.text = currentWiseWinOrLoseText.text = last7DaysText.text = totalWinOrLose.ToString();
                        }
                        else
                        {
                            allTimeText.color = currentWiseWinOrLoseText.color = last7DaysText.color = Color.red;
                            allTimeText.text = currentWiseWinOrLoseText.text = last7DaysText.text = totalWinOrLose.ToString();
                        }
                    }
                    else
                    {
                        noContentPanel.SetActive(true);
                        allTimeText.color = currentWiseWinOrLoseText.color = last7DaysText.color = Color.gray;
                        allTimeText.text = currentWiseWinOrLoseText.text = last7DaysText.text = "0";

                        //Generating YAxis on Graph
                        dayContainer.GetChild(_currentPage).Find("Window_Graph").GetComponent<GraphManager>().valueList.Clear();
                        dayContainer.GetChild(_currentPage).Find("Window_Graph").GetComponent<GraphManager>().valueList.Add(10);
                        dayContainer.GetChild(_currentPage).Find("Window_Graph").GetComponent<GraphManager>().ShowDefaultGraph(dayContainer.GetChild(_currentPage).Find("Window_Graph").GetComponent<GraphManager>().valueList, /*(int _i) => "Day " + (+_i+1),*/ (float _f) => /*"$"*/ "" + Mathf.RoundToInt(_f));
                    }
                }
                else
                {
                    MainMenuController.instance.ShowMessage("Unable to update request..");
                }
            }
            else if (containerScroll_Name.Equals("MonthScroll"))
            {
                if (data["message"].Equals("Success"))
                {
                    for (int i = 0; i < careerMonthListContainer.transform.childCount; i++)
                    {
                        Destroy(careerMonthListContainer.transform.GetChild(i).gameObject);
                    }

                    if (data["data"].Count > 0)
                    {
                        //Clear old graph data
                        monthContainer.GetChild(_currentPage).Find("Window_Graph").GetComponent<GraphManager>().valueList.Clear();
                        monthContainer.GetChild(_currentPage).Find("Window_Graph").GetComponent<GraphManager>().xPosValueList.Clear();
                        noContentPanel.SetActive(false);

                        for (int i = 0; i < data["data"].Count; i++)
                        {
                            listObj = Instantiate(careerListPrefab, careerMonthListContainer.transform);

                            listObj.transform.Find("bg Image/hand text").GetComponent<Text>().text = data["data"][i]["date"].ToString().Substring(5,2) + "/" + data["data"][i]["date"].ToString().Substring(8, 2);

                            if (data["data"][i]["isClub"].Equals(false))
                            {
                                //Debug.Log("This is not club data...");
                                listObj.transform.Find("bg Image/win text").GetComponent<Text>().text = "Hand: " + data["data"][i]["total"].ToString();
                            }
                            else
                            {
                                //Debug.Log("This is club data...");
                                listObj.transform.Find("bg Image/win text").GetComponent<Text>().text = "Game: " + data["data"][i]["table"].ToString() + "\n" + "Hand: " + data["data"][i]["total"].ToString();
                            }

                            totalWinOrLose += float.Parse(data["data"][i]["profit"].ToString());

                            if (float.Parse(data["data"][i]["profit"].ToString()) > 0)
                            {
                                listObj.transform.Find("bg Image/loss text").GetComponent<Text>().color = Color.green;
                            }
                            else
                            {
                                listObj.transform.Find("bg Image/loss text").GetComponent<Text>().color = Color.red;
                            }

                            listObj.transform.Find("bg Image/loss text").GetComponent<Text>().text = data["data"][i]["profit"].ToString();

                            //Adding profit data to the value list
                            monthContainer.GetChild(_currentPage).Find("Window_Graph").GetComponent<GraphManager>().valueList.Add(int.Parse(data["data"][i]["profit"].ToString()));
                            //monthContainer.GetChild(_currentPage).Find("Window_Graph").GetComponent<GraphManager>().valueList.Add(int.Parse(data["data"][i]["betAmount"].ToString()));

                            //Adding dates from response to the xPosList
                            float pos = monthContainer.GetChild(_currentPage).Find("xAxis/M1").GetComponent<RectTransform>().anchoredPosition.x + ((int.Parse(data["data"][i]["date"].ToString().Substring(8, 2))%30) * 14.5f);

                            monthContainer.GetChild(_currentPage).Find("Window_Graph").GetComponent<GraphManager>().xPosValueList.Add(pos);
                            
                            //Calling Show Graph Method to generate graph
                            monthContainer.GetChild(_currentPage).Find("Window_Graph").GetComponent<GraphManager>().ShowGraph(monthContainer.GetChild(_currentPage).Find("Window_Graph").GetComponent<GraphManager>().valueList,
                                                                                                                              monthContainer.GetChild(_currentPage).Find("Window_Graph").GetComponent<GraphManager>().xPosValueList,
                                                                                                                              /*(int _i) => "Day " + (+_i+1),*/ (float _f) => /*"$"*/ "" + Mathf.RoundToInt(_f),false,true);
                        }
                        if (totalWinOrLose > 0)
                        {
                            allTimeText.color = currentWiseWinOrLoseText.color = last7DaysText.color = Color.green;
                            allTimeText.text = currentWiseWinOrLoseText.text = last7DaysText.text = totalWinOrLose.ToString();
                        }
                        else
                        {
                            allTimeText.color = currentWiseWinOrLoseText.color = last7DaysText.color = Color.red;
                            allTimeText.text = currentWiseWinOrLoseText.text = last7DaysText.text = totalWinOrLose.ToString();
                        }
                    }
                    else
                    {
                        noContentPanel.SetActive(true);
                        allTimeText.color = currentWiseWinOrLoseText.color = last7DaysText.color = Color.gray;
                        allTimeText.text = currentWiseWinOrLoseText.text = last7DaysText.text = "0";

                        //Generating YAxis on Graph
                        monthContainer.GetChild(_currentPage).Find("Window_Graph").GetComponent<GraphManager>().valueList.Clear();
                        monthContainer.GetChild(_currentPage).Find("Window_Graph").GetComponent<GraphManager>().valueList.Add(10);
                        monthContainer.GetChild(_currentPage).Find("Window_Graph").GetComponent<GraphManager>().ShowDefaultGraph(monthContainer.GetChild(_currentPage).Find("Window_Graph").GetComponent<GraphManager>().valueList, /*(int _i) => "Day " + (+_i+1),*/ (float _f) => /*"$"*/ "" + Mathf.RoundToInt(_f));
                    }
                }
                else
                {
                    MainMenuController.instance.ShowMessage("Unable to update request..");
                }
            }
            else if (containerScroll_Name.Equals("YearScroll"))
            {
                if (data["message"].Equals("Success"))
                {
                    for (int i = 0; i < careerYearListContainer.transform.childCount; i++)
                    {
                        Destroy(careerYearListContainer.transform.GetChild(i).gameObject);
                    }

                    if (data["data"].Count > 0)
                    {
                        //Clear old graph data
                        yearContainer.GetChild(_currentPage).Find("Window_Graph").GetComponent<GraphManager>().valueList.Clear();
                        yearContainer.GetChild(_currentPage).Find("Window_Graph").GetComponent<GraphManager>().xPosValueList.Clear();
                        noContentPanel.SetActive(false);

                        for (int i = 0; i < data["data"].Count; i++)
                        {
                            listObj = Instantiate(careerListPrefab, careerYearListContainer.transform);

                            listObj.transform.Find("bg Image/hand text").GetComponent<Text>().text = data["data"][i]["date"].ToString().Substring(5, 2) + "/" + data["data"][i]["date"].ToString().Substring(8, 2);

                            if (data["data"][i]["isClub"].Equals(false))
                            {
                                //Debug.Log("This is not club data...");
                                listObj.transform.Find("bg Image/win text").GetComponent<Text>().text = "Hand: " + data["data"][i]["total"].ToString();
                            }
                            else
                            {
                                //Debug.Log("This is club data...");
                                listObj.transform.Find("bg Image/win text").GetComponent<Text>().text = "Game: " + data["data"][i]["table"].ToString() + "\n" + "Hand: " + data["data"][i]["total"].ToString();
                            }


                            totalWinOrLose += float.Parse(data["data"][i]["profit"].ToString());

                            if (float.Parse(data["data"][i]["profit"].ToString()) > 0)
                            {
                                listObj.transform.Find("bg Image/loss text").GetComponent<Text>().color = Color.green;
                            }
                            else
                            {
                                listObj.transform.Find("bg Image/loss text").GetComponent<Text>().color = Color.red;
                            }

                            listObj.transform.Find("bg Image/loss text").GetComponent<Text>().text = data["data"][i]["profit"].ToString();

                            //Adding profit data to the value list
                            yearContainer.GetChild(_currentPage).Find("Window_Graph").GetComponent<GraphManager>().valueList.Add(int.Parse(data["data"][i]["profit"].ToString()));
                            //yearContainer.GetChild(_currentPage).Find("Window_Graph").GetComponent<GraphManager>().valueList.Add(int.Parse(data["data"][i]["betAmount"].ToString()));

                            //Adding months from response to the xPosList
                            float pos = yearContainer.GetChild(_currentPage).Find("xAxis/Y1").GetComponent<RectTransform>().anchoredPosition.x + ((int.Parse(data["data"][i]["date"].ToString().Substring(5, 2)) % 12) * 14.5f);
                            //yearContainer.GetChild(_currentPage).Find("Window_Graph").GetComponent<GraphManager>().xPosValueList.Add(int.Parse(data["data"][i]["date"].ToString().Substring(5, 2)));
                            yearContainer.GetChild(_currentPage).Find("Window_Graph").GetComponent<GraphManager>().xPosValueList.Add(pos);

                            //Calling Show Graph Method to generate graph
                            yearContainer.GetChild(_currentPage).Find("Window_Graph").GetComponent<GraphManager>().ShowGraph(yearContainer.GetChild(_currentPage).Find("Window_Graph").GetComponent<GraphManager>().valueList,
                                                                                                                             yearContainer.GetChild(_currentPage).Find("Window_Graph").GetComponent<GraphManager>().xPosValueList,
                                                                                                                             /*(int _i) => "Day " + (+_i+1),*/ (float _f) => /*"$"*/ "" + Mathf.RoundToInt(_f),false,false,true);
                        }
                        if (totalWinOrLose > 0)
                        {
                            allTimeText.color = currentWiseWinOrLoseText.color = last7DaysText.color = Color.green;
                            allTimeText.text = currentWiseWinOrLoseText.text = last7DaysText.text = totalWinOrLose.ToString();
                        }
                        else
                        {
                            allTimeText.color = currentWiseWinOrLoseText.color = last7DaysText.color = Color.red;
                            allTimeText.text = currentWiseWinOrLoseText.text = last7DaysText.text = totalWinOrLose.ToString();
                        }
                    }
                    else
                    {
                        noContentPanel.SetActive(true);

                        allTimeText.color = currentWiseWinOrLoseText.color = last7DaysText.color = Color.gray;
                        allTimeText.text = currentWiseWinOrLoseText.text = last7DaysText.text = "0";

                        //Generating YAxis on Graph
                        yearContainer.GetChild(_currentPage).Find("Window_Graph").GetComponent<GraphManager>().valueList.Clear();
                        yearContainer.GetChild(_currentPage).Find("Window_Graph").GetComponent<GraphManager>().valueList.Add(10);
                        yearContainer.GetChild(_currentPage).Find("Window_Graph").GetComponent<GraphManager>().ShowDefaultGraph(yearContainer.GetChild(_currentPage).Find("Window_Graph").GetComponent<GraphManager>().valueList, /*(int _i) => "Day " + (+_i+1),*/ (float _f) => /*"$"*/ "" + Mathf.RoundToInt(_f));
                    }
                }
                else
                {
                    MainMenuController.instance.ShowMessage("Unable to update request..");
                }
            }
        }
        else
        {

#if ERROR_LOG
            Debug.LogError("Unhadnled response found in  = " + requestType);
#endif
        }

    }

    //------------------------------------------------------------------------
    void Update() {
        
        // if moving to target position
        if (_lerp) {
            // prevent overshooting with values greater than 1
            float decelerate = Mathf.Min(decelerationRate * Time.deltaTime, 1f);
            _container.anchoredPosition = Vector2.Lerp(_container.anchoredPosition, _lerpTo, decelerate);
            // time to stop lerping?
            if (Vector2.SqrMagnitude(_container.anchoredPosition - _lerpTo) < 0.25f) {
                // snap to target and stop lerping
                _container.anchoredPosition = _lerpTo;
                _lerp = false;
                // clear also any scrollrect move that may interfere with our lerping
                _scrollRectComponent.velocity = Vector2.zero;
            }

            // switches selection icon exactly to correct page
            if (_showPageSelection) {
                SetPageSelection(GetNearestPage());
            }
        }
    }

    //------------------------------------------------------------------------
    //DEV_CODE
    public void UpdatePages()
    {
        SetPagePositions();
        SetPage(startingPage);
        InitPageSelection();
        SetPageSelection(startingPage);
    }

    private void SetPagePositions() {
        int width = 0;
        int height = 0;
        int offsetX = 0;
        int offsetY = 0;
        int containerWidth = 0;
        int containerHeight = 0;
        if (_horizontal) {
            // screen width in pixels of scrollrect window
            width = (int)_scrollRectRect.rect.width;
            // center position of all pages
            offsetX = width / 2;
            // total width
            containerWidth = width * _pageCount;
            // limit fast swipe length - beyond this length it is fast swipe no more
            _fastSwipeThresholdMaxLimit = width;
        } else {
            height = (int)_scrollRectRect.rect.height;
            offsetY = height / 2;
            containerHeight = height * _pageCount;
            _fastSwipeThresholdMaxLimit = height;
        }

        // set width of container
        Vector2 newSize = new Vector2(containerWidth, containerHeight);
        _container.sizeDelta = newSize;
        Vector2 newPosition = new Vector2(containerWidth / 2, containerHeight / 2);
        _container.anchoredPosition = newPosition;

        // delete any previous settings
        _pagePositions.Clear();

        // iterate through all container childern and set their positions
        for (int i = 0; i < _pageCount; i++) {
            RectTransform child = _container.GetChild(i).GetComponent<RectTransform>();
            Vector2 childPosition;
            if (_horizontal) {
                childPosition = new Vector2(i * width - containerWidth / 2 + offsetX, 0f);
            } else {
                childPosition = new Vector2(0f, -(i * height - containerHeight / 2 + offsetY));
            }
            child.anchoredPosition = childPosition;
            _pagePositions.Add(-childPosition);
        }
    }

    //------------------------------------------------------------------------
    private void SetPage(int aPageIndex) {
        aPageIndex = Mathf.Clamp(aPageIndex, 0, _pageCount - 1);
        _container.anchoredPosition = _pagePositions[aPageIndex];
        _currentPage = aPageIndex;
        ChangeTxtVal();      
    }

    //------------------------------------------------------------------------
    private void LerpToPage(int aPageIndex) 
    {
        aPageIndex = Mathf.Clamp(aPageIndex, 0, _pageCount - 1);

        //Debug.Log("Current Page: " + _currentPage);
        //Debug.Log("APage: " + aPageIndex);
        
        if (aPageIndex < _currentPage)
        {
            if (containerScroll_Name.Equals("DayScroll"))
            {
                //Debug.Log("Previous Date : " + (CareerManager.instance.currentDate--));
                CareerManager.instance.currentDate--;
                if (CareerManager.instance.currentDate < 1)
                {
                    CareerManager.instance.currentMonth--;
                    CareerManager.instance.currentDate = DateTime.DaysInMonth(CareerManager.instance.currentYear, CareerManager.instance.currentMonth);
                }
            }
            else if (containerScroll_Name.Equals("MonthScroll"))
            {
                CareerManager.instance.currentMonth--;
                if(CareerManager.instance.currentMonth <1)
                {
                    CareerManager.instance.currentYear--;
                    CareerManager.instance.currentMonth = 12;
                }
            }
            else if (containerScroll_Name.Equals("YearScroll"))
            {
                CareerManager.instance.currentYear--;
            }
        }
        else if (aPageIndex > _currentPage)
        {
            if (containerScroll_Name.Equals("DayScroll"))
            {
                //Debug.Log("Next Date : " + (CareerManager.instance.currentDate++));
                CareerManager.instance.currentDate++;
                if (CareerManager.instance.currentDate > DateTime.DaysInMonth(CareerManager.instance.currentYear, CareerManager.instance.currentMonth))
                {
                    CareerManager.instance.currentMonth++;
                    CareerManager.instance.currentDate = 1;
                }
            }
            else if (containerScroll_Name.Equals("MonthScroll"))
            {
                CareerManager.instance.currentMonth++;
                if (CareerManager.instance.currentMonth > 12)
                {
                    CareerManager.instance.currentYear++;
                    CareerManager.instance.currentMonth = 1;
                }
            }
            else if (containerScroll_Name.Equals("YearScroll"))
            {
                CareerManager.instance.currentYear++;                
            }
        }     
        
        _lerpTo = _pagePositions[aPageIndex];
        _lerp = true;
        _currentPage = aPageIndex;

        ChangeTxtVal();
    }

    //------------------------------------------------------------------------
    private void InitPageSelection() {
        // page selection - only if defined sprites for selection icons
        _showPageSelection = unselectedPage != null && selectedPage != null;
        if (_showPageSelection) {
            // also container with selection images must be defined and must have exatly the same amount of items as pages container
            if (pageSelectionIcons == null || pageSelectionIcons.childCount != _pageCount) {
                Debug.LogWarning("Different count of pages and selection icons - will not show page selection");
                _showPageSelection = false;
            } else {
                _previousPageSelectionIndex = -1;
                _pageSelectionImages = new List<Image>();

                // cache all Image components into list
                for (int i = 0; i < pageSelectionIcons.childCount; i++) {
                    Image image = pageSelectionIcons.GetChild(i).GetComponent<Image>();
                    if (image == null) {
                        Debug.LogWarning("Page selection icon at position " + i + " is missing Image component");
                    }
                    _pageSelectionImages.Add(image);
                }
            }
        }
    }

    //------------------------------------------------------------------------
    private void SetPageSelection(int aPageIndex) {
        // nothing to change
        if (_previousPageSelectionIndex == aPageIndex) {
            return;
        }
        
        // unselect old
        if (_previousPageSelectionIndex >= 0) 
        {
            //_pageSelectionImages[_previousPageSelectionIndex].sprite = unselectedPage;
            //_pageSelectionImages[_previousPageSelectionIndex].SetNativeSize();
        }

        // select new
        //_pageSelectionImages[aPageIndex].sprite = selectedPage;
        //_pageSelectionImages[aPageIndex].SetNativeSize();

        _previousPageSelectionIndex = aPageIndex;
    }

    //------------------------------------------------------------------------
    private void NextScreen() {
        LerpToPage(_currentPage + 1);
    }

    //------------------------------------------------------------------------
    private void PreviousScreen() {
        LerpToPage(_currentPage - 1);
    }

    //------------------------------------------------------------------------
    private int GetNearestPage() {
        // based on distance from current position, find nearest page
        Vector2 currentPosition = _container.anchoredPosition;

        float distance = float.MaxValue;
        int nearestPage = _currentPage;

        for (int i = 0; i < _pagePositions.Count; i++) {
            float testDist = Vector2.SqrMagnitude(currentPosition - _pagePositions[i]);
            if (testDist < distance) {
                distance = testDist;
                nearestPage = i;
            }
        }

        return nearestPage;
    }

    //------------------------------------------------------------------------
    public void OnBeginDrag(PointerEventData aEventData) {
        // if currently lerping, then stop it as user is draging
        _lerp = false;
        // not dragging yet
        _dragging = false;
    }

    //------------------------------------------------------------------------
    public void OnEndDrag(PointerEventData aEventData) {
        // how much was container's content dragged
        float difference;
        if (_horizontal) {
            difference = _startPosition.x - _container.anchoredPosition.x;
        } else {
            difference = - (_startPosition.y - _container.anchoredPosition.y);
        }

        // test for fast swipe - swipe that moves only +/-1 item
        if (Time.unscaledTime - _timeStamp < fastSwipeThresholdTime &&
            Mathf.Abs(difference) > fastSwipeThresholdDistance &&
            Mathf.Abs(difference) < _fastSwipeThresholdMaxLimit) {
            if (difference > 0) {
                //Debug.Log("Going to next");
                NextScreen();
            } else {
                //Debug.Log("Going to previous");
                PreviousScreen();
            }
        } else {
            // if not fast time, look to which page we got to
            LerpToPage(GetNearestPage());
        }

        _dragging = false;
    }

    //------------------------------------------------------------------------
    public void OnDrag(PointerEventData aEventData) {
        if (!_dragging) {
            // dragging started
            _dragging = true;
            // save time - unscaled so pausing with Time.scale should not affect it
            _timeStamp = Time.unscaledTime;
            // save current position of cointainer
            _startPosition = _container.anchoredPosition;
        } else {
            if (_showPageSelection) {
                SetPageSelection(GetNearestPage());
            }
        }
    }
}

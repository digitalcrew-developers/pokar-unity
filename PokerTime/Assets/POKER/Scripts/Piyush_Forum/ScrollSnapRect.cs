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
    public Text dayWiseWinOrLoseText;
    public Text allTimeText;
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
        switch (this.gameObject.name)
        {
            case "YearScroll":
                //changeIndexTxt.text = "202" + _currentPage ;
                changeIndexTxt.text = CareerManager.instance.currentYear.ToString();
                break;
            case "MonthScroll":
                //changeIndexTxt.text = "2020 - "+_currentPage + 1 ;
                changeIndexTxt.text = CareerManager.instance.currentYear + "-" + ((CareerManager.instance.currentMonth.ToString().Length == 1) ? "0" + CareerManager.instance.currentMonth.ToString() : CareerManager.instance.currentMonth.ToString());
                break;
            case "DayScroll":
                //changeIndexTxt.text = _currentPage + 1 + "/25";
                //Debug.Log("Setting New Date..");
                changeIndexTxt.text = CareerManager.instance.currentMonth + "/" + ((CareerManager.instance.currentDate.ToString().Length == 1) ? "0" + CareerManager.instance.currentDate.ToString() : CareerManager.instance.currentDate.ToString());
                break;
        }
        //GameObject g = GameObject.Find("Date");//Add By Gp
        //Destroy(g);//Add By GP
        string date = CareerManager.instance.currentYear + "-" +
                      ((CareerManager.instance.currentMonth.ToString().Length == 1) ? "0" + CareerManager.instance.currentMonth.ToString() : CareerManager.instance.currentMonth.ToString()) + "-" +
                      ((CareerManager.instance.currentDate.ToString().Length == 1) ? "0" + CareerManager.instance.currentDate.ToString() : CareerManager.instance.currentDate.ToString());


        string requestData = "{\"userId\":" + int.Parse(PlayerManager.instance.GetPlayerGameData().userId) + "," +
                               "\"date\":\"" + date + "\"," +
                               "\"endDate\":\"" + date + "\"}";

        WebServices.instance.SendRequest(RequestType.GetGameHistory, requestData, true, OnServerResponseFound);
        //Debug.LogError("FD1");
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
        
        if (requestType == RequestType.GetGameHistory)
        {
            Debug.Log("Response => GetGameHistory : " + serverResponse);
            
            JsonData data = JsonMapper.ToObject(serverResponse);

            if (containerScroll_Name.Equals("DayScroll"))
            {
                //if (data["status"].Equals(true))
                //{
                    listObj = Instantiate(careerListPrefab, careerDayListContainer.transform);
                    listObj.name = "Date";
                    listObj.transform.Find("bg Image/hand text").GetComponent<Text>().text = "Hand: " + data["data"]["totalHand"].ToString();
                    listObj.transform.Find("bg Image/win text").GetComponent<Text>().text = "Win: " + data["data"]["totalWin"].ToString();
                    listObj.transform.Find("bg Image/loss text").GetComponent<Text>().text = "Loss: " + data["data"]["totalLoss"].ToString();

                    Debug.Log("data   ==>>>>" + data["data"]["totalHand"].ToString());
                //}
                //else
                //{
                //    MainMenuController.instance.ShowMessage("Unable to update request..");
                //}
            }
            else if(containerScroll_Name.Equals("MonthScroll"))
            {
                listObj = Instantiate(careerListPrefab, careerMonthListContainer.transform);
                //datelist.name = "Date";
                listObj.transform.Find("bg Image/hand text").GetComponent<Text>().text = "Hand: " + data["data"]["totalHand"].ToString();
                listObj.transform.Find("bg Image/win text").GetComponent<Text>().text = "Win: " + data["data"]["totalWin"].ToString();
                listObj.transform.Find("bg Image/loss text").GetComponent<Text>().text = "Loss: " + data["data"]["totalLoss"].ToString();

                Debug.Log("data   ==>>>>" + data["data"]["totalHand"].ToString());
            }
            else if (containerScroll_Name.Equals("YearScroll"))
            {
                listObj = Instantiate(careerListPrefab, careerYearListContainer.transform);
                //datelist.name = "Date";
                listObj.transform.Find("bg Image/hand text").GetComponent<Text>().text = "Hand: " + data["data"]["totalHand"].ToString();
                listObj.transform.Find("bg Image/win text").GetComponent<Text>().text = "Win: " + data["data"]["totalWin"].ToString();
                listObj.transform.Find("bg Image/loss text").GetComponent<Text>().text = "Loss: " + data["data"]["totalLoss"].ToString();

                Debug.Log("data   ==>>>>" + data["data"]["totalHand"].ToString());
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
        if (_previousPageSelectionIndex >= 0) {
            _pageSelectionImages[_previousPageSelectionIndex].sprite = unselectedPage;
            _pageSelectionImages[_previousPageSelectionIndex].SetNativeSize();
        }

        // select new
        _pageSelectionImages[aPageIndex].sprite = selectedPage;
        _pageSelectionImages[aPageIndex].SetNativeSize();

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

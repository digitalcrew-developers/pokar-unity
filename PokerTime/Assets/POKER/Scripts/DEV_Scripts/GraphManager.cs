using CodeMonkey.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphManager : MonoBehaviour
{
    public static GraphManager instance;
    [SerializeField]
    private Sprite circleSprite;
    private RectTransform graphContainer;
    private RectTransform labelTemplateX;
    private RectTransform labelTemplateY;
    private List<GameObject> gameObjectsList;

    public List<int> valueList;
    public List<float> xPosValueList;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        graphContainer = transform.Find("graphContainer").GetComponent<RectTransform>();
        //labelTemplateX = graphContainer.Find("labelTemplateX").GetComponent<RectTransform>();
        labelTemplateY = graphContainer.Find("labelTemplateY").GetComponent<RectTransform>();
        gameObjectsList = new List<GameObject>();

        valueList = new List<int>();
        xPosValueList = new List<float>();

        //List<int> valueList = new List<int>() { 5, 98, 56, 45, 33, 18, 15, 40, 36, 33, -15, -18, -44 };
        //ShowGraph(valueList, /*(int _i) => "Day " + (+_i+1),*/ (float _f) => /*"$"*/ "" + Mathf.RoundToInt(_f));

        //FunctionPeriodic.Create(() => 
        //{
        //    valueList.Clear();
        //    for (int i = 0; i < 15; i++)
        //    {
        //        valueList.Add(UnityEngine.Random.Range(0, 500));
        //    }
        //    ShowGraph(valueList, (int _i) => "Day " + (+_i + 1), (float _f) => "$" + Mathf.RoundToInt(_f));

        //}, .5f);
    }

    private GameObject CreateCircle(Vector2 anchoredPosition)
    {
        GameObject gameObject = new GameObject("circle", typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);
        gameObject.GetComponent<Image>().sprite = circleSprite;
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = new Vector2(11, 11);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        return gameObject;
    }

    public void ShowGraph(List<int> valueList, List<float> xPosValueList, /*Func<int, string> getAxisLabelX = null,*/ Func<float, string> getAxisLabelY = null, bool isDay = false, bool isMonth = false, bool isYear = false)
    {
        //if (getAxisLabelX == null)
        //{
        //    getAxisLabelX = delegate (int _i)
        //    {
        //        return _i.ToString();
        //    };
        //}

        if (getAxisLabelY == null)
        {
            getAxisLabelY = delegate (float _f)
            {
                return Mathf.RoundToInt(_f).ToString();
            };
        }

        foreach (GameObject gameObject in gameObjectsList)
        {
            Destroy(gameObject);
        }
        gameObjectsList.Clear();

        float graphHeight = graphContainer.sizeDelta.y;

        float graphWidth = graphContainer.sizeDelta.x;
        
        float yMaximum = valueList[0];
        float yMinimum = valueList[0];

        foreach(int value in valueList)
        {
            if (value > yMaximum)
                yMaximum = value;

            if (value < yMinimum)
                yMinimum = value;
        }

        yMaximum = yMaximum + ((yMaximum - yMinimum) * 0.25f);
        yMinimum = yMinimum - ((yMaximum - yMinimum) * 0.25f);


        //Calculating xMinimum and xMaximum values
        float xMaximum = xPosValueList[0];
        float xMinimum = xPosValueList[0];

        foreach (int value in xPosValueList)
        {
            if (value > xMaximum)
                xMaximum = value;

            if (value < xMinimum)
                xMinimum = value;
        }

        GameObject lastCircleGameObject = null;

        xPosValueList.Sort();

        for (int i = 0; i < valueList.Count; i++)
        {
            float xPosition = xPosValueList[i] + 14.5f;
            float yPosition = ((valueList[i] - yMinimum) / (yMaximum - yMinimum)) * graphHeight * 1.01f;
            GameObject circleGameObject = CreateCircle(new Vector2(xPosition, yPosition));
            gameObjectsList.Add(circleGameObject);

            if(lastCircleGameObject/*circleGameObject*/ != null)
            {
                //Debug.Log("Creating Line with dots");
                GameObject dotConnectionGameObject =  CreateDotConnection(lastCircleGameObject.GetComponent<RectTransform>().anchoredPosition, circleGameObject.GetComponent<RectTransform>().anchoredPosition);
                gameObjectsList.Add(dotConnectionGameObject);
            }
            //Enable follow code if you want to show dot when there is single data
            else if (lastCircleGameObject == null && valueList.Count == 1)
            {
                Debug.Log("Only One Data is there so do only dot no line is there..." + xPosition + " AND Y POsition: " + yPosition);
                gameObjectsList.Clear();
                GameObject dotObject = CreateCircle(new Vector2(xPosition, yMinimum * graphHeight * 1.01f));
                gameObjectsList.Add(dotObject);
            }
            lastCircleGameObject = circleGameObject;

            //RectTransform labelX = Instantiate(labelTemplateX);
            //labelX.SetParent(graphContainer);
            //labelX.gameObject.SetActive(true);
            //labelX.localScale = new Vector3(1, 1, 1);
            //labelX.anchoredPosition = new Vector2(xPosition, -20f);
            //labelX.GetComponent<Text>().text = getAxisLabelX(i);
            //gameObjectsList.Add(labelX.gameObject);
        }


        //float seperatorCount = -1f;
        //for (int i = 0; i < 5; i++)
        //{
        //    RectTransform labelY = Instantiate(labelTemplateY);
        //    labelY.SetParent(graphContainer);
        //    labelY.gameObject.SetActive(true);
        //    labelY.localScale = new Vector3(1, 1, 1);
        //    float normalizedValue = i * 1f / seperatorCount;
        //    seperatorCount += 0.5f;
        //    labelY.anchoredPosition = new Vector2(-23f, (i * 1.05f / 5) * graphHeight);
        //    if (yMaximum == yMinimum)
        //        labelY.GetComponent<Text>().text = Utility.GetTrimmedAmount(getAxisLabelY(yMinimum + (normalizedValue * yMinimum)));
        //    else
        //        labelY.GetComponent<Text>().text = Utility.GetTrimmedAmount(getAxisLabelY(yMinimum + (normalizedValue * (yMaximum - yMinimum))));
        //    gameObjectsList.Add(labelY.gameObject);
        //}



        //ORIGINAL Logic
        int seperatorCount = 5;
        for (int i = 0; i < seperatorCount; i++)
        {
            RectTransform labelY = Instantiate(labelTemplateY);
            labelY.SetParent(graphContainer);
            labelY.gameObject.SetActive(true);
            labelY.localScale = new Vector3(1, 1, 1);
            float normalizedValue = i * 1.05f / seperatorCount;
            labelY.anchoredPosition = new Vector2(-23f, normalizedValue * graphHeight);

            if (yMaximum == yMinimum)
                labelY.GetComponent<Text>().text = Utility.GetTrimmedAmount(getAxisLabelY(yMinimum + (normalizedValue * yMinimum)));
            else
                labelY.GetComponent<Text>().text = Utility.GetTrimmedAmount(getAxisLabelY(yMinimum + (normalizedValue * (yMaximum - yMinimum))));

            gameObjectsList.Add(labelY.gameObject);
        }
    }

    private GameObject CreateDotConnection(Vector2 dotPositionA, Vector2 dotPositionB)
    {
        GameObject gameObject = new GameObject("dotConnection", typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);
        gameObject.GetComponent<Image>().color = new Color(1, 1, 1, .5f);
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        Vector2 dir = (dotPositionB - dotPositionA).normalized;
        float distance = Vector2.Distance(dotPositionA, dotPositionB);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.sizeDelta = new Vector2(distance, 3f);
        rectTransform.anchoredPosition = dotPositionA + dir * distance * .5f; 
        rectTransform.localEulerAngles = new Vector3(0, 0, UtilsClass.GetAngleFromVectorFloat(dir));
        return gameObject;
    }

    public void ShowDefaultGraph(List<int> valueList, /*Func<int, string> getAxisLabelX = null,*/ Func<float, string> getAxisLabelY = null)
    {
        float graphHeight = graphContainer.sizeDelta.y;

        float yMinimum = valueList[0];

        float seperatorCount = -1f;
        for (int i = 0; i < 5; i++)
        {
            RectTransform labelY = Instantiate(labelTemplateY);
            labelY.SetParent(graphContainer);
            labelY.gameObject.SetActive(true);
            labelY.localScale = new Vector3(1, 1, 1);
            float normalizedValue = /*i * 1f /*/ seperatorCount;
            seperatorCount += 0.5f;
            labelY.anchoredPosition = new Vector2(-23f, (i* 1.05f/5) * graphHeight);
            labelY.GetComponent<Text>().text = ((normalizedValue * yMinimum > 0)?"+":"") + getAxisLabelY(/*yMinimum + */(normalizedValue * /*(yMaximum - yMinimum)*/ yMinimum));
            gameObjectsList.Add(labelY.gameObject);
        }
    }
}
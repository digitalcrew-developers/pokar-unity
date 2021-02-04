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

    private void Awake()
    {
        if (instance == null)
            instance = this;

        graphContainer = transform.Find("graphContainer").GetComponent<RectTransform>();
        //labelTemplateX = graphContainer.Find("labelTemplateX").GetComponent<RectTransform>();
        labelTemplateY = graphContainer.Find("labelTemplateY").GetComponent<RectTransform>();
        gameObjectsList = new List<GameObject>();

        valueList = new List<int>();

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

    public void ShowGraph(List<int> valueList, /*Func<int, string> getAxisLabelX = null,*/ Func<float, string> getAxisLabelY = null)
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
        
        float yMaximum = valueList[0];
        float yMinimum = valueList[0];

        foreach(int value in valueList)
        {
            if (value > yMaximum)
                yMaximum = value;

            if (value < yMinimum)
                yMinimum = value;
        }

        yMaximum = yMaximum + ((yMaximum - yMinimum) * 0.2f);
        yMinimum = yMinimum - ((yMaximum - yMinimum) * 0.2f);

        float xSize = 50f;

        GameObject lastCircleGameObject = null;
        for (int i = 0; i < valueList.Count; i++)
        {
            float xPosition = xSize + i * xSize;
            float yPosition = ((valueList[i] - yMinimum) / (yMaximum - yMinimum)) * graphHeight;
            GameObject circleGameObject = CreateCircle(new Vector2(xPosition, yPosition));
            gameObjectsList.Add(circleGameObject);
            if(lastCircleGameObject != null)
            {
                GameObject dotConnectionGameObject =  CreateDotConnection(lastCircleGameObject.GetComponent<RectTransform>().anchoredPosition, circleGameObject.GetComponent<RectTransform>().anchoredPosition);
                gameObjectsList.Add(dotConnectionGameObject);
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

        int seperatorCount = 5;
        for (int i = 0; i < seperatorCount; i++)
        {
            RectTransform labelY = Instantiate(labelTemplateY);
            labelY.SetParent(graphContainer);
            labelY.gameObject.SetActive(true);
            labelY.localScale = new Vector3(1, 1, 1);
            float normalizedValue = i * 1f / seperatorCount;
            labelY.anchoredPosition = new Vector2(-23f, normalizedValue * graphHeight);
            labelY.GetComponent<Text>().text = getAxisLabelY(yMinimum + (normalizedValue * (yMaximum - yMinimum)));
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
}
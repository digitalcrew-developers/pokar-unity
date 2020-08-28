using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LayoutManager: MonoBehaviour
{
    public LayoutType layoutType;
    public ChildAlignment elementAlignment;
    public Padding padding;
    public ScrollRect scrollRect;

    public float spacingX, spacingY;
    public int numberOfColumns;
    public bool IsAdjustWidth, IsAdjustHeight;
    public bool IsMaintainCurrentSize;

    private RectTransform container, childElement;
    private Vector2 defaultContainerSize = new Vector2(-1f,-1f);

    #if UNITY_EDITOR
        public bool Refresh;
    //public bool Refresh2;


        private void OnDrawGizmosSelected()
        {

        //if(Refresh2)
        //{
        //    Refresh2 = false;
        //    container.anchorMin = new Vector2(0.5f, 0.5f);
        //    container.anchorMax = new Vector2(0.5f, 0.5f);
        //}

            if (Refresh)
            {
                Refresh = false;
                UpdateLayout();
            }     
        }

#endif

    public void UpdateLayout()
    {
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(WaitAndUpdateLayout());
        }
    }


    private IEnumerator WaitAndUpdateLayout()
    {
        yield return new WaitForEndOfFrame();
        container = GetComponent<RectTransform>();

        if (defaultContainerSize.x < 0)
        {
            defaultContainerSize = GetRectSize(container);
        }

        if (container.childCount > 0)
        {
            childElement = container.GetChild(0).GetComponent<RectTransform>();
            AdjustContainerSize();

            Vector3 spawnPos = GetSpawnPos();
            Vector2 distanceBetweenElements = GetDistanceBetweenElements2();

            Vector2 initialSpawnPos = spawnPos;
            int counter = 0;

            for (int i = 0; i < container.childCount; i++)
            {
                container.GetChild(i).localPosition = spawnPos;

                if (layoutType == LayoutType.Grid)
                {
                    ++counter;

                    spawnPos.x += distanceBetweenElements.x;

                    if (counter >= numberOfColumns)
                    {
                        counter = 0;
                        spawnPos.y += distanceBetweenElements.y;
                        spawnPos.x = initialSpawnPos.x;
                    }
                }
                else
                {
                    spawnPos.y += distanceBetweenElements.y;
                    spawnPos.x += distanceBetweenElements.x;
                }
            }
        }
        else
        {
            //Debug.LogError("number of child should be greater than 0");
        }


        if(scrollRect != null)
        {
            yield return new WaitForEndOfFrame();

            switch (layoutType)
            {
                case LayoutType.Vertical:
                scrollRect.verticalNormalizedPosition = 1;
                break;

                case LayoutType.Horizontal:
                scrollRect.horizontalNormalizedPosition = 1;
                break;
                  
                default:
                scrollRect.verticalNormalizedPosition = 1;
                scrollRect.horizontalNormalizedPosition = 1;
                break;
            }

        }

    }




    private Vector2 GetDistanceBetweenElements2()
    {
        float childHeight = GetRectSize(childElement).y + spacingY;
        float childWidth = GetRectSize(childElement).x + spacingX;

        Vector2 distanceBetweenElements = Vector2.zero;

        distanceBetweenElements.x = GetRectSize(childElement).x + spacingX;
        distanceBetweenElements.y = GetRectSize(childElement).y + spacingY;


        switch (layoutType)
        {
            case LayoutType.Vertical:
            {
                switch (elementAlignment)
                {
                    case ChildAlignment.BottomCenter:
                    case ChildAlignment.BottomLeft:
                    case ChildAlignment.BottomRight:
                    {
                        distanceBetweenElements.x = 0;
                    }
                    break;

                    default:
                    {
                        distanceBetweenElements.x = 0;
                        distanceBetweenElements.y *= -1;
                    }
                    break;
                }
            }
            break;

            case LayoutType.Horizontal:
            {
                switch (elementAlignment)
                {
                    case ChildAlignment.TopRight:
                    case ChildAlignment.MiddleRight:
                    case ChildAlignment.BottomRight:
                    {
                        distanceBetweenElements.x *= -1;
                        distanceBetweenElements.y = 0;
                    }
                    break;

                    default:
                    distanceBetweenElements.y = 0;
                    break;
                }
            }
            break;

            default:
            {
                switch (elementAlignment)
                {
                    case ChildAlignment.TopLeft:
                    case ChildAlignment.TopCenter:
                    case ChildAlignment.MiddleLeft:
                    case ChildAlignment.MiddleCenter:
                    {
                        distanceBetweenElements.y *= -1;
                    }
                    break;

                    case ChildAlignment.TopRight:
                    case ChildAlignment.MiddleRight:
                    {
                        distanceBetweenElements.y *= -1;
                        distanceBetweenElements.x *= -1;
                    }
                    break;

                    case ChildAlignment.BottomRight:
                    {
                        distanceBetweenElements.x *= -1;
                    }
                    break;

                    //case ChildAlignment.BottomLeft:
                    //case ChildAlignment.BottomCenter:
                    //{
                    //}
                    //break;


                    default:
                    break;
                }
            }
            break;
        }



        return distanceBetweenElements;
    }


    private Vector3 GetSpawnPos()
    {
        int totalChildCount = container.childCount - 1;

        Vector2 parentSizeDelta = GetRectSize(container);
        Vector2 spawnPos = Vector2.zero;
        float childHeight = GetRectSize(childElement).y + spacingY;
        float childWidth = GetRectSize(childElement).x + spacingX;

        switch (elementAlignment)
        {
            case ChildAlignment.TopCenter:
            case ChildAlignment.TopLeft:
            case ChildAlignment.TopRight:
            {
                spawnPos.y = (parentSizeDelta.y / 2);
                spawnPos.y += -childHeight / 2;
                spawnPos.y += spacingY / 2;

                //if (padding.top > 0)
                //{
                //    spawnPos.y -= padding.top;
                //}
            }
            break;

            case ChildAlignment.MiddleCenter:
            case ChildAlignment.MiddleLeft:
            case ChildAlignment.MiddleRight:
            {
                if(layoutType == LayoutType.Horizontal)
                {
                    spawnPos.y = 0;
                }
                else
                {
                    float requireHeight = childHeight * totalChildCount;
                    spawnPos.y = (requireHeight / 2);
                }

                //if (padding.top > 0)
                //{
                //    spawnPos.y -= padding.top / 2;
                //}
            }
            break;


            default:
            spawnPos.y = (-parentSizeDelta.y / 2);
            spawnPos.y += childHeight / 2;
            spawnPos.y -= (spacingY / 2);

            //if (padding.top > 0)
            //{
            //    spawnPos.y -= padding.top / 2;
            //}

            break;
        }




        switch (elementAlignment)
        {
            case ChildAlignment.TopLeft:
            case ChildAlignment.MiddleLeft:
            case ChildAlignment.BottomLeft:
            {
                spawnPos.x = -parentSizeDelta.x / 2;
                spawnPos.x += (childWidth / 2);
                spawnPos.x -= spacingX / 2;

                //if (padding.left > 0)
                //{
                //    spawnPos.y -= padding.left / 2;
                //}
            }
            break;


            case ChildAlignment.TopCenter:
            case ChildAlignment.MiddleCenter:
            case ChildAlignment.BottomCenter:
            {
                if (layoutType == LayoutType.Vertical)
                {
                    spawnPos.x = 0;
                }
                else
                {
                    float requireWidth = childWidth * totalChildCount;
                    spawnPos.x = (-requireWidth / 2);
                }

                //if (padding.left > 0)
                //{
                //    spawnPos.y -= padding.left / 2;
                //}

            }
            break;


            default:
                spawnPos.x = parentSizeDelta.x / 2;
                spawnPos.x += (-childWidth / 2);
                spawnPos.x += spacingX / 2;

            //if (padding.left > 0)
            //{
            //    spawnPos.y -= padding.left / 2;
            //}
               
            break;
        }


        spawnPos.y += -padding.top; 
        spawnPos.y += padding.bottom; 

        spawnPos.x += padding.left;
        spawnPos.x += -padding.right;

        //if(layoutType == LayoutType.Vertical)
        //{
        //    spawnPos.x = 0;
        //}
        //else if (layoutType == LayoutType.Horizontal)
        //{
        //    spawnPos.y = 0;
        //}

        return spawnPos;
    }

    //----------------------------------------------------------------------------------------------------------------------------------------------------------------


    private Vector2 GetDistanceBetweenElements()
    {
        Vector2 distanceBetweenElements = Vector2.zero;


        switch (layoutType)
        {

            case LayoutType.Vertical:

            distanceBetweenElements.y += GetRectSize(childElement).y + spacingY;
            distanceBetweenElements.y *= -1;

            break;

            case LayoutType.Horizontal:

            distanceBetweenElements.x += GetRectSize(childElement).x + spacingX;
            break;


            default:

            distanceBetweenElements.x += GetRectSize(childElement).x + spacingX;
            distanceBetweenElements.y += GetRectSize(childElement).y + spacingY;
            distanceBetweenElements.y *= -1;
            break;
        }




        return distanceBetweenElements;
    }

    //private void AddPaddingSpace()
    //{
    //    Vector2 currentSize = GetRectSize(container);

    //    currentSize.x += padding.x;
    //    currentSize.y += padding.y;
    //    SetRectSize(container,currentSize);
    //}


    private void AdjustContainerSize()
    {
        SetRectSize(container,defaultContainerSize);

        Vector2 singleObjectSizeDelta = GetRectSize(childElement);
        singleObjectSizeDelta.x += spacingX;
        singleObjectSizeDelta.y += spacingY;
        Vector2 containerSizeDelta = GetRectSize(container);

        switch (layoutType)
        {
            case LayoutType.Vertical:
            {
                if (IsAdjustWidth)
                {
                    containerSizeDelta.x = singleObjectSizeDelta.x;
                }

                if (IsAdjustHeight)
                {
                    containerSizeDelta.y = singleObjectSizeDelta.y * container.childCount;
                }
            }
            break;
           
            case LayoutType.Horizontal:
            {
                if (IsAdjustWidth)
                {
                    containerSizeDelta.x = singleObjectSizeDelta.x * container.childCount;
                }

                if (IsAdjustHeight)
                {
                    containerSizeDelta.y = singleObjectSizeDelta.y;
                }
            }
          
            break;


            default:


            if (numberOfColumns > 0)
            {
                int totalChildCount = container.childCount;

                if(IsAdjustHeight)
                {
                    int numberOfRows = (int)((float)totalChildCount / numberOfColumns);

                    if (totalChildCount % numberOfColumns > 0)
                    {
                        ++numberOfRows;
                    }

                    containerSizeDelta.y = singleObjectSizeDelta.y * numberOfRows;
                }

                if(IsAdjustWidth)
                {
                    if(totalChildCount < numberOfColumns)
                    {
                        containerSizeDelta.x = singleObjectSizeDelta.x * totalChildCount;
                    }
                    else
                    {
                        containerSizeDelta.x = singleObjectSizeDelta.x * numberOfColumns;
                    }
                }
            }
            else
            {
                containerSizeDelta.y = singleObjectSizeDelta.y * container.childCount;
            }
            break;
        }

        containerSizeDelta.x -= spacingX;
        containerSizeDelta.y -= spacingY;

        //containerSizeDelta.x += padding.x;
        //containerSizeDelta.y += padding.y;

        containerSizeDelta.x += (padding.left + padding.right);
        containerSizeDelta.y += (padding.top + padding.bottom);

        if(IsMaintainCurrentSize)
        {
            if(containerSizeDelta.x < defaultContainerSize.x)
            {
                containerSizeDelta.x = defaultContainerSize.x;
            }

            if (containerSizeDelta.y < defaultContainerSize.y)
            {
                containerSizeDelta.y = defaultContainerSize.y;
            }
        }

        SetRectSize(container,containerSizeDelta);
    }


    private Vector2 GetRectSize(RectTransform rectTransform)
    {
        return new Vector2(rectTransform.rect.width,rectTransform.rect.height);
    }

    private void SetRectSize(RectTransform rectTransform,Vector2 sizeOfRect)
    {
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.sizeDelta = sizeOfRect;
    }





}



public enum LayoutType
{
    Horizontal,
    Vertical,
    Grid
}

public enum ChildAlignment
{
    TopCenter,
    TopLeft,
    TopRight,

    MiddleCenter,
    MiddleLeft,
    MiddleRight,

    BottomCenter,
    BottomLeft,
    BottomRight
}

[System.Serializable]
public class Padding
{
    public float left, right, bottom, top;
}

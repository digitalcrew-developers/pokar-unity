﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Swipe : MonoBehaviour
{
    //Scroll bar as per scrolling like horizontal or vertical.
    public GameObject scrollBar;    //Horizontal scroll bar reference
    float scroll_pos = 0;
    float[] pos;

    private void Start()
    {
        scroll_pos = 0.5f;/*scrollBar.GetComponent<Scrollbar>().value;*/
    }

    private void Update()
    {
        pos = new float[transform.childCount];
        float distance = 1f / (pos.Length - 1f);
        for (int i = 0; i < pos.Length; i++)
        {
            pos[i] = distance * i;
        }

        if (Input.GetMouseButton(0))
        {
            scroll_pos = scrollBar.GetComponent<Scrollbar>().value;
        }
        else
        {
            for (int i = 0; i < pos.Length; i++)
            {
                if (scroll_pos < pos[i] + (distance / 2) && scroll_pos > pos[i] - (distance / 2))
                {
                    scrollBar.GetComponent<Scrollbar>().value = Mathf.Lerp(scrollBar.GetComponent<Scrollbar>().value, pos[i], 0.1f);
                }
            }
        }

        for (int i = 0; i < pos.Length; i++)
        {
            if (scroll_pos < pos[i] + (distance / 2) && scroll_pos > pos[i] - (distance / 2))
            {
                //Debug.LogWarning("Current Selected Level" + i);
                transform.GetChild(i).localScale = Vector2.Lerp(transform.GetChild(i).localScale, new Vector2(1.8f, 1.8f), 0.1f);
                
                for (int j = 0; j < pos.Length; j++)
                {
                    if (j != i)
                    {
                        transform.GetChild(j).localScale = Vector2.Lerp(transform.GetChild(j).localScale, new Vector2(1f, 1f), 0.1f);
                        transform.GetChild(j).Find("Button2").gameObject.SetActive(false);
                        transform.GetChild(j).Find("Button1").gameObject.SetActive(true);
                    }
                    else
                    {
                        transform.GetChild(j).Find("Button2").gameObject.SetActive(true);
                        transform.GetChild(j).Find("Button1").gameObject.SetActive(false);
                    }
                }
            }
        }
    }
}
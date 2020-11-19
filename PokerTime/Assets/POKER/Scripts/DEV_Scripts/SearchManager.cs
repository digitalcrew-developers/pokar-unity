using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using VoxelBusters.Utility;

public class SearchManager : MonoBehaviour
{
    public static SearchManager instance;

    public Transform container, searchContainer;

    private void Awake()
    {
        instance = this;
    }

    public void OnValueChange()
    {
        for (int i = 0; i < searchContainer.childCount; i++)
        {
            Destroy(searchContainer.GetChild(i).gameObject);
        }

        for (int i = 0; i < container.childCount; i++)
        {
            if(container.GetChild(i).name.Contains(transform.GetComponent<TMP_InputField>().text))
            {
                GameObject obj = Instantiate(container.GetChild(i).gameObject, searchContainer);
            }
        }

        if(searchContainer.childCount>0)
        {
            container.gameObject.SetActive(false);
            searchContainer.gameObject.SetActive(true);
        }
        else
        {
            searchContainer.gameObject.SetActive(false);
            container.gameObject.SetActive(true);
        }
    }
}
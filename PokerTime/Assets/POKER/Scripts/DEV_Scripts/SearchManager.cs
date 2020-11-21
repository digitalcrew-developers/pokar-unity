using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VoxelBusters.Utility;

public class SearchManager : MonoBehaviour
{
    public static SearchManager instance;
    public ScrollRect scrollRect;
    public Transform container, searchContainer;

    //private List<GameObject> _searchable = new List<GameObject>();

    private void Awake()
    {
        instance = this;        
    }

    public void OnValueChange()
    {
        //DEV_CODE

        if (transform.GetComponent<TMP_InputField>().text.Length > 0)
        {
            for (int i = 0; i < searchContainer.childCount; i++)
            {
                Destroy(searchContainer.GetChild(i).gameObject);
            }

            for (int i = 0; i < container.childCount; i++)
            {
                if (container.GetChild(i).name.ToLower().Contains(transform.GetComponent<TMP_InputField>().text.ToLower()))
                {
                    GameObject obj = Instantiate(container.GetChild(i).gameObject, searchContainer);
                }
            }
            container.gameObject.SetActive(false);
            searchContainer.gameObject.SetActive(true);

            scrollRect.content = searchContainer.GetComponent<RectTransform>();
        }
        else
        {
            for (int i = 0; i < searchContainer.childCount; i++)
            {
                Destroy(searchContainer.GetChild(i).gameObject);
            }

            container.gameObject.SetActive(true);
            searchContainer.gameObject.SetActive(false);

            scrollRect.content = container.GetComponent<RectTransform>();
        }

        //-------------------------------------------------------------------------------------------------------------------

        //string _inputString = transform.GetComponent<TMP_InputField>().text.ToLower();
        //if (_inputString.Length > 0)
        //{
        //    var found = _searchable.Where(s => s.name.ToLower().Contains(_inputString)); //search query
        //    foreach (GameObject g in _searchable)
        //    {
        //        //deactive all
        //        g.SetActive(false);
        //    }
        //    foreach (GameObject g in found)
        //    {
        //        //activate only we need
        //        g.SetActive(true);
        //    }
        //}
        //else
        //{
        //    foreach (GameObject g in _searchable)
        //    {
        //        //if string is empty,show all
        //        g.SetActive(true);
        //    }
        //}
    }
}
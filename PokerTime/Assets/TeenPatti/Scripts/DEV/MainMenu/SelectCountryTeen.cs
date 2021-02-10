using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectCountryTeen : MonoBehaviour
{
    public Image countryicon, countryselected;

    public Text countryName;

    public string countryURL, countryID;
    // Start is called before the first frame update
    void Start()
    {
        countryselected.enabled = false;
        StartCoroutine(loadSpriteImageFromUrl(countryURL));
    }
    IEnumerator loadSpriteImageFromUrl(string URL)
    {

        WWW www = new WWW(URL);
        while (!www.isDone)
        {
            //     Debug.Log("Download image on progress" + www.progress);
            yield return null;
        }

        if (!string.IsNullOrEmpty(www.error))
        {
               Debug.Log("Download failed" + URL);
        }
        else
        {
            Texture2D texture = new Texture2D(1, 1);
            www.LoadImageIntoTexture(texture);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            countryicon.sprite = sprite;
        }
    }
    public void OnClickCountry()
    {
        countryselected.enabled = true;
        ProfileModificationTeen.instance.countrycode = countryID;
        ProfileModificationTeen.instance.countryRegion.text = countryName.text;
        ProfileModificationTeen.instance.countryimage.sprite = countryicon.sprite;

        for (int i = 0; i < SelectCountryRegionTeen.instance.prefebParent.transform.childCount; i++)
        {
            if(!SelectCountryRegionTeen.instance.prefebParent.transform.GetChild(i).transform.GetComponent<SelectCountryTeen>().countryID.Equals(countryID))
            {
                SelectCountryRegionTeen.instance.prefebParent.transform.GetChild(i).transform.GetComponent<SelectCountryTeen>().countryselected.enabled = false;
            }
        }
    }
}

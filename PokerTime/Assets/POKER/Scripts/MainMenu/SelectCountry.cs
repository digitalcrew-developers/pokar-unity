using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectCountry : MonoBehaviour
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
               Debug.Log("Download failed");
        }
        else
        {
            Texture2D texture = new Texture2D(1, 1);
            www.LoadImageIntoTexture(texture);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            countryicon.sprite = sprite;
        }
    }
    public void OnClickCuntry()
    {
        countryselected.enabled = true;
        ProfileModification.instance.countrycode = countryID;
        ProfileModification.instance.countryRegion.text = countryName.text;
        ProfileModification.instance.countryimage.sprite = countryicon.sprite;
    }
}

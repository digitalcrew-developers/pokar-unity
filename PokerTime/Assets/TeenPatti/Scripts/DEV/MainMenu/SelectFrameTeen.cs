using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectFrameTeen : MonoBehaviour
{
    public Image frameImage;

    public int frameId;

    public string urlImage;

    private void Start()
    {
        StartCoroutine(loadSpriteImageFromUrl(urlImage));
    }

    
    /*public void OnSelectFrame()
    {
        ProfileModification.instance.avtarid = frameId;
        ProfileModification.instance.profileImage.sprite = frameImage.sprite;
    }*/

    IEnumerator loadSpriteImageFromUrl(string URL)
    {
        WWW www = new WWW(URL);
        while (!www.isDone)
        {
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
            frameImage.sprite = sprite;
        }
    }

}

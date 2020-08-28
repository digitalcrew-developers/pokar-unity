using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectAvatar : MonoBehaviour
{
    public Image avtarimage,selectavatar;

    public int avtarid;

    public string urlImage;

    private void Start()
    {
        selectavatar.enabled = false;
        StartCoroutine(loadSpriteImageFromUrl(urlImage));
    }
    // Start is called before the first frame update
    public void OnSelectAvatar()
    {
        selectavatar.enabled = true;
        ProfileModification.instance.avtarid = avtarid;
        ProfileModification.instance.profileImage.sprite = avtarimage.sprite;
    }
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
            Sprite sprite = Sprite.Create(texture,new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            avtarimage.sprite = sprite;
        }
    }

}

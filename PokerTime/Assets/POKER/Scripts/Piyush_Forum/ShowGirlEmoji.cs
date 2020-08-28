using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowGirlEmoji : MonoBehaviour
{
    public Button girl;
    // Start is called before the first frame update
    void Start()
    {
        girl.onClick.AddListener(TaskOnClick);
    }

    
    void TaskOnClick()
    {
        Debug.Log("I Touch Girls");

        InGameUiManager.instance.OnClickEmoji(2);
        InGameUiManager.instance.OnClickOnButton("emojiScreen");
        InGameUiManager.instance.OnClickEmojiTransform(this.transform);
    }
}

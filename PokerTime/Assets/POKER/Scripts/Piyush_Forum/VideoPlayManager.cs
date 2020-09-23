using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VideoPlayManager : MonoBehaviour
{
    public static VideoPlayManager instance;

    public Text videoIndexText;
    public Button backButton, playButton, pauseButton, postButton, commentButton, likeButton, previousButton, nextButton;
    public Slider slider;
    public Text sliderValue;
    private void Awake()
    {
        instance = this;
    }

    public void OnClickBackBtn()
    {
        Destroy(gameObject);
    }
}
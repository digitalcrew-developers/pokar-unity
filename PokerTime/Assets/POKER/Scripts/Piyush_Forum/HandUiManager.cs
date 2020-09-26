using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using NatCorder;
using System;
using UnityEngine.Video;
using UnityEngine.EventSystems;
using LitJson;
using VoxelBusters.Utility;
using UnityEngine.XR;

public class HandUiManager : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public static HandUiManager instance;

    public Transform container;
    public Image[] onfocusImageAry;
    public Text handReviewText;
    public Text popUpText;
    public Image noRecordImage;
    public GameObject deletePopUp;
    public GameObject handPrefab, inviteCommentPrefab;
    public GameObject videoRawImage;
    public GameObject commentPanel, commentPanelCommentObj, videoPanel, newPostPanel, hashTagPanel, inviteCommentPanel;

    public List<string> videoList = new List<string>();

    private GameObject handObject, videoObject, inviteCommentObject;

    bool slide = false;

    public string videoPath;

    private Slider tracking;
    private VideoPlayer videoPlayer;
    private VideoSource videoSource;

    private AudioSource audioSource;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        DisableAllPanels();
        GetAllVideoList(true);
    }

    private void DisableAllPanels()
    {
        noRecordImage.gameObject.SetActive(false);
        popUpText.gameObject.SetActive(false);
        commentPanel.SetActive(false);
        deletePopUp.SetActive(false);
        newPostPanel.SetActive(false);
        hashTagPanel.SetActive(false);
        inviteCommentPanel.SetActive(false);
    }

    public void OnPointerUp(PointerEventData a)
    {
        float frame = (float)tracking.value * (float)videoPlayer.frameCount;
        videoPlayer.frame = (long)frame;
        slide = false;
    }

    public void OnPointerDown(PointerEventData a)
    {
        slide = true;

        if (videoPlayer.isPlaying && !videoObject.GetComponent<VideoPlayManager>().pauseButton.gameObject.activeSelf)
        {
            videoObject.GetComponent<VideoPlayManager>().pauseButton.gameObject.SetActive(true);
            videoObject.GetComponent<VideoPlayManager>().previousButton.gameObject.SetActive(true);
            videoObject.GetComponent<VideoPlayManager>().nextButton.gameObject.SetActive(true);
        }
        else if (videoPlayer.isPlaying && videoObject.GetComponent<VideoPlayManager>().pauseButton.gameObject.activeSelf)
        {
            videoObject.GetComponent<VideoPlayManager>().pauseButton.gameObject.SetActive(false);
            videoObject.GetComponent<VideoPlayManager>().previousButton.gameObject.SetActive(false);
            videoObject.GetComponent<VideoPlayManager>().nextButton.gameObject.SetActive(false);
        }
        else if (!videoPlayer.isPlaying && !videoObject.GetComponent<VideoPlayManager>().playButton.gameObject.activeSelf)
        {
            videoObject.GetComponent<VideoPlayManager>().playButton.gameObject.SetActive(true);
            videoObject.GetComponent<VideoPlayManager>().previousButton.gameObject.SetActive(true);
            videoObject.GetComponent<VideoPlayManager>().nextButton.gameObject.SetActive(true);
        }
        else if (!videoPlayer.isPlaying && videoObject.GetComponent<VideoPlayManager>().playButton.gameObject.activeSelf)
        {
            videoObject.GetComponent<VideoPlayManager>().playButton.gameObject.SetActive(false);
            videoObject.GetComponent<VideoPlayManager>().previousButton.gameObject.SetActive(false);
            videoObject.GetComponent<VideoPlayManager>().nextButton.gameObject.SetActive(false);
        }
    }

    public void OnClickPreviosuButton(int index)
    {
        if (!Directory.Exists(Path.Combine(Application.persistentDataPath, "Videos")))
            Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "Videos"));

        DirectoryInfo dir = new DirectoryInfo(Path.Combine(Application.persistentDataPath, "Videos"));
        FileInfo[] info = dir.GetFiles("*.mp4");

        for (int i = 0; i < info.Length; i++)
        {
            if (info[i].Name == videoList[index])
            {
                Debug.Log("File Available");
                OnClickOnPlayButton(index + info[i].Name);
            }
        }
    }

    public void OnClickNextButton(int index)
    {
        if (!Directory.Exists(Path.Combine(Application.persistentDataPath, "Videos")))
            Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "Videos"));

        DirectoryInfo dir = new DirectoryInfo(Path.Combine(Application.persistentDataPath, "Videos"));
        FileInfo[] info = dir.GetFiles("*.mp4");

        for (int i = 0; i < info.Length; i++)
        {
            if (info[i].Name == videoList[index])
            {
                Debug.Log("File Available");
                OnClickOnPlayButton(index + info[i].Name);
            }
        }
    }

    public void OnClickBackBtn()
    {
        Destroy(videoObject);
    }

    public void OnClickCommentBtn()
    {
        commentPanelCommentObj.SetActive(true);
        commentPanel.SetActive(true);
    }

    public void OnClickCommentBackButton()
    {
        commentPanelCommentObj.SetActive(false);
    }

    public void OnClickLikeBtn()
    {

    }

    public void OnClickHashTag()
    {
        hashTagPanel.SetActive(true);
    }

    public void OnClickInviteComments()
    {
        inviteCommentPanel.SetActive(true);
    }

    public void OnClickAddPostButton()
    {
        newPostPanel.SetActive(true);
        HandPostPanel.instance.path = videoPlayer.url;
    }

    public void OnClickPauseBtn()
    {
        videoPlayer.Pause();
        videoObject.GetComponent<VideoPlayManager>().playButton.gameObject.SetActive(true);
        videoObject.GetComponent<VideoPlayManager>().pauseButton.gameObject.SetActive(false);
    }

    public void OnClickPlayBtn()
    {
        videoPlayer.Play();
        videoObject.GetComponent<VideoPlayManager>().playButton.gameObject.SetActive(false);
        videoObject.GetComponent<VideoPlayManager>().pauseButton.gameObject.SetActive(false);

        StartCoroutine(ResumeVideo());
        //tracking.value = (float)videoPlayer.frame / (float)videoPlayer.frameCount;
    }

    IEnumerator ResumeVideo()
    {
        while (videoPlayer.isPlaying)
        {
            tracking.value = (float)videoPlayer.frame / (float)videoPlayer.frameCount;

            yield return null;
        }
    }

    public void OnClickBtnClose()
    {
        MainMenuController.instance.DestroyScreen(MainMenuScreens.HandScreen);        
    }

    public void GetAllVideoList(bool isShowLoading)
    {
        ChangeBtnFocus(0);
        LoadAllVideosFromDevice();
    }

    public void GetAllCollectionList(bool isShowLoading)
    {
        ChangeBtnFocus(1);
        LoadAllCollectionVideos();
    }

    private void LoadAllVideosFromDevice()
    {
        //Reset VideoList
        videoList.Clear();

        DirectoryInfo dir;
        FileInfo[] info;
        
        if (!Directory.Exists(Path.Combine(Application.persistentDataPath, "Videos")))
            Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "Videos"));

        for (int i = 0; i < container.childCount; i++)
        {
            Destroy(container.GetChild(i).gameObject);
        }

        //Set Hand Review Text
        handReviewText.text = "Latest 100 hands records";

        dir = new DirectoryInfo(Path.Combine(Application.persistentDataPath, "Videos"));
        info = dir.GetFiles("*.mp4");

        Sprite[] cardSprites = Resources.LoadAll<Sprite>("cards");

        for (int j = 0; j < info.Length; j++)
        {
            string[] x = info[j].Name.Split('_');
            
            //Removing currepted files.
            if (x.Length == 7)
            {
                File.Delete(info[j].FullName);
            }            
        }

        dir = new DirectoryInfo(Path.Combine(Application.persistentDataPath, "Videos"));
        info = dir.GetFiles("*.mp4");

        //Show No Record Image
        if (info.Length == 0)
            noRecordImage.gameObject.SetActive(true);
        else
            noRecordImage.gameObject.SetActive(false);

        for (int i = 0; i < info.Length; i++)
        {
            FileInfo f = new FileInfo(info[i].Name);

            videoList.Add(f.Name);

            FileInfo f1 = new FileInfo(i + info[i].Name);
            string[] x = info[i].Name.Split('_');

            handObject = Instantiate(handPrefab, container) as GameObject;
            
            if (x[9].Length == 1)
                x[9] = "0" + x[9];

            handObject.transform.GetComponent<Button>().onClick.AddListener(() => OnClickOnPlayButton(f1.Name));

            GetFirstCardDetail(x[4], x[3], handObject, cardSprites);
            GetSecondCardDetail(x[6], x[5], handObject, cardSprites);

            handObject.GetComponent<HandManager>().dateAndTime.text = x[7] + " " + x[8] + " : " + x[9];
            handObject.GetComponent<HandManager>().chipsData.text = x[1] + "/" + x[2];
            handObject.GetComponent<HandManager>().shareButton.onClick.AddListener(() => OnClickOnShareButton(f.Name));
            handObject.GetComponent<HandManager>().collectionButton.onClick.AddListener(() => OnClickOnCollectionButton(f.Name));
            handObject.GetComponent<HandManager>().removeFromCollectionButton.gameObject.SetActive(false);

            if (PlayerPrefs.HasKey(f.Name))
            {
                //handObject.GetComponent<HandManager>().collectionButton.image.color = Color.white;
                handObject.GetComponent<HandManager>().collectionButton.image.sprite = handObject.GetComponent<HandManager>().collectionSprite;
            }
        }        
    }

    private void LoadAllCollectionVideos()
    {
        //Reset VideoList
        videoList.Clear();

        for (int i = 0; i < container.childCount; i++)
        {
            Destroy(container.GetChild(i).gameObject);
        }

        //Set Hand review Text for collection
        handReviewText.text = "Hands saved " + PlayerPrefs.GetInt("CollectionVideoCount") + "/100";

        Sprite[] cardSprites = Resources.LoadAll<Sprite>("cards");

        //With PlayerPrefs
        //Add videos to list
        if (!Directory.Exists(Path.Combine(Application.persistentDataPath, "Videos")))
            Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "Videos"));

        DirectoryInfo dir = new DirectoryInfo(Path.Combine(Application.persistentDataPath, "Videos"));
        FileInfo[] info = dir.GetFiles("*.mp4");

        //Show No Record Image
        if (PlayerPrefs.GetInt("CollectionVideoCount") == 0 || info.Length==0)
            noRecordImage.gameObject.SetActive(true);
        else
            noRecordImage.gameObject.SetActive(false);

        for (int i = 0; i < info.Length; i++)
        {
            FileInfo f = new FileInfo(info[i].Name);
            
            if (PlayerPrefs.HasKey(f.Name))
            {
                FileInfo file = new FileInfo(f.Name);

                videoList.Add(f.Name);

                FileInfo f1 = new FileInfo(i + info[i].Name);
                string[] x = file.Name.Split('_');

                handObject = Instantiate(handPrefab, container) as GameObject;

                handObject.transform.GetComponent<Button>().onClick.AddListener(() => OnClickOnPlayButton(f1.Name));

                GetFirstCardDetail(x[4], x[3], handObject, cardSprites);
                GetSecondCardDetail(x[6], x[5], handObject, cardSprites);
                
                handObject.GetComponent<HandManager>().dateAndTime.text = x[7] + " " + x[8] + " : " + x[9];
                handObject.GetComponent<HandManager>().chipsData.text = x[1] + "/" + x[2];
                handObject.GetComponent<HandManager>().shareButton.onClick.AddListener(() => OnClickOnShareButton(f.Name));
                handObject.GetComponent<HandManager>().collectionButton.onClick.AddListener(() => OnClickOnCollectionButton(f.Name));
                handObject.GetComponent<HandManager>().collectionButton.gameObject.SetActive(false);                //Disable save to collection button
                handObject.GetComponent<HandManager>().removeFromCollectionButton.gameObject.SetActive(true);       //Enable delete from collection button
                handObject.GetComponent<HandManager>().removeFromCollectionButton.onClick.AddListener(() => OnClickOnRemoveVideoFromCollection(f.Name));
            }
        }
    }

    private void OnClickOnPlayButton(string name)
    {
        int index = int.Parse(name.Substring(0, 1));
        string newName = name.Substring(1, name.Length-1);
        
        commentPanelCommentObj.SetActive(false);
        commentPanel.SetActive(true);
        videoPanel.SetActive(true);
        StartCoroutine(PlayVideo(newName, index));
    }

    IEnumerator PlayVideo(string vn, int index)
    {
        if (!gameObject.GetComponent<VideoPlayer>() && !gameObject.GetComponent<AudioSource>())
        {
            videoPlayer = gameObject.AddComponent<VideoPlayer>();
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        videoPlayer.playOnAwake = false;
        audioSource.playOnAwake = false;
        //audioSource.Pause();

        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = Path.Combine(Application.persistentDataPath, "Videos", vn);

        videoPath = videoPlayer.url;

        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        //Assign the Audio from Video to AudioSource to be played
        videoPlayer.EnableAudioTrack(0, true);
        videoPlayer.SetTargetAudioSource(0, audioSource);

        //Set video To Play then prepare Audio to prevent Buffering
        //videoPlayer.clip = videoToPlay;
        videoPlayer.Prepare();

        //Wait until video is prepared
        WaitForSeconds waitTime = new WaitForSeconds(1);
        while (!videoPlayer.isPrepared)
        {
            /*Debug.Log("Preparing Video");*/
            //Prepare/Wait for 5 sceonds only
            yield return waitTime;
            //Break out of the while loop after 5 seconds wait
            break;
        }

        /*Debug.Log("Done Preparing Video");*/
        videoObject = Instantiate(videoRawImage, videoPanel.transform) as GameObject;
        
        //Assign the Texture from Video to RawImage to be displayed
        videoObject.GetComponent<RawImage>().texture = videoPlayer.texture;

        //Assign the Slider
        tracking = videoObject.GetComponent<VideoPlayManager>().slider;

        videoObject.GetComponent<VideoPlayManager>().backButton.onClick.AddListener(() => OnClickBackBtn());
        videoObject.GetComponent<VideoPlayManager>().playButton.gameObject.SetActive(false);
        videoObject.GetComponent<VideoPlayManager>().playButton.onClick.AddListener(() => OnClickPlayBtn());
        videoObject.GetComponent<VideoPlayManager>().pauseButton.gameObject.SetActive(false);
        videoObject.GetComponent<VideoPlayManager>().pauseButton.onClick.AddListener(() => OnClickPauseBtn());
        videoObject.GetComponent<VideoPlayManager>().postButton.onClick.AddListener(() => OnClickAddPostButton());
        videoObject.GetComponent<VideoPlayManager>().commentButton.onClick.AddListener(() => OnClickCommentBtn());
        videoObject.GetComponent<VideoPlayManager>().likeButton.onClick.AddListener(() => OnClickLikeBtn());
        videoObject.GetComponent<VideoPlayManager>().previousButton.gameObject.SetActive(false);
        videoObject.GetComponent<VideoPlayManager>().previousButton.onClick.AddListener(() => OnClickPreviosuButton(index - 1));
        videoObject.GetComponent<VideoPlayManager>().nextButton.gameObject.SetActive(false);
        videoObject.GetComponent<VideoPlayManager>().nextButton.onClick.AddListener(() => OnClickNextButton(index + 1));
        videoObject.GetComponent<VideoPlayManager>().videoIndexText.text = index.ToString();

        if (container.childCount == 1)
        {
            videoObject.GetComponent<VideoPlayManager>().previousButton.interactable = false;
            videoObject.GetComponent<VideoPlayManager>().nextButton.interactable = false;
        }
        else if (videoObject.GetComponent<VideoPlayManager>().videoIndexText.text == "0")
        {
            videoObject.GetComponent<VideoPlayManager>().previousButton.interactable = false;
            videoObject.GetComponent<VideoPlayManager>().nextButton.interactable = true;
        }
        else if (videoObject.GetComponent<VideoPlayManager>().videoIndexText.text == (container.childCount - 1).ToString())
        {
            videoObject.GetComponent<VideoPlayManager>().previousButton.interactable = true;
            videoObject.GetComponent<VideoPlayManager>().nextButton.interactable = false;
        }
        else
        {
            videoObject.GetComponent<VideoPlayManager>().previousButton.interactable = true;
            videoObject.GetComponent<VideoPlayManager>().nextButton.interactable = true;
        }

        //Play Video
        videoPlayer.Play();

        //Play Sound
        audioSource.Play();

        /*Debug.Log("Playing Video");*/
        while (videoPlayer.isPlaying)
        {
            //Debug.Log("Video Time: " + Mathf.FloorToInt((float)videoPlayer.time));

            tracking.value = (float)videoPlayer.frame / (float)videoPlayer.frameCount;

            yield return null;
        }
        /*Debug.Log("Done Playing Video");*/
    }



    private void OnClickOnShareButton(string name)
    {
        StartCoroutine(StartSharing(Path.Combine(Application.persistentDataPath, "Videos", name)));   
    }

    private IEnumerator StartSharing(string path)
    {
        yield return new WaitForEndOfFrame();

        Debug.Log("Path: " + path);
        new NativeShare().AddFile(path).Share();
    }

    private void OnClickOnCollectionButton(string name)
    {
        bool isVideoAvailable = false;

        //Use of PlayerPrefs
        if (PlayerPrefs.GetInt("CollectionVideoCount") == 0)
        {
            PlayerPrefs.SetInt("CollectionVideoCount", PlayerPrefs.GetInt("CollectionVideoCount") + 1);
            PlayerPrefs.SetString(name, name);
            StartCoroutine(MsgForVideo("Saved into collection", 2.0f));
            LoadAllVideosFromDevice();
        }
        else
        {
            for (int i = 1; i <= PlayerPrefs.GetInt("CollectionVideoCount"); i++)
            {
                if (PlayerPrefs.GetString(name) == name)
                {
                    //Debug.Log("Already Available video");
                    isVideoAvailable = true;

                    PlayerPrefs.DeleteKey(name);
                    PlayerPrefs.SetInt("CollectionVideoCount", PlayerPrefs.GetInt("CollectionVideoCount") - 1);
                    StartCoroutine(MsgForVideo("Delete from collection", 2.0f));
                    LoadAllVideosFromDevice();
                }
            }

            if (!isVideoAvailable)
            {
                PlayerPrefs.SetInt("CollectionVideoCount", PlayerPrefs.GetInt("CollectionVideoCount") + 1);
                PlayerPrefs.SetString(name, name);
                StartCoroutine(MsgForVideo("Saved into collection", 2.0f));
                LoadAllVideosFromDevice();
            }
        }        
    }

    IEnumerator MsgForVideo(string msg, float delay)
    {
        popUpText.gameObject.SetActive(true);
        popUpText.text = msg;        
        yield return new WaitForSeconds(delay);
        popUpText.gameObject.SetActive(false);
    }

    private void OnClickOnRemoveVideoFromCollection(string name)
    {
        deletePopUp.SetActive(true);
        deletePopUp.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(() => OnClickRemoveVideo(name));        
    }

    public void OnClickRemoveVideo(string name)
    {
        //With PlayerPrefs
        if (PlayerPrefs.HasKey(name))
        {
            PlayerPrefs.DeleteKey(name);
            PlayerPrefs.SetInt("CollectionVideoCount", PlayerPrefs.GetInt("CollectionVideoCount") - 1);
            StartCoroutine(MsgForVideo("Delete from collection", 2.0f));
            deletePopUp.SetActive(false);
        }
        LoadAllCollectionVideos();
    }

    public void OnCloseDeletePopUp()
    {
        deletePopUp.SetActive(false);
    }

    void ChangeBtnFocus(int focusVal)
    {
        for (int i = 0; i < onfocusImageAry.Length; i++)
        {
            if (i != focusVal)
            {
                Color temp = onfocusImageAry[i].color;
                temp.a = 0.01f;
                onfocusImageAry[i].color = temp;
            }
            else
            {
                Color temp = onfocusImageAry[i].color;
                temp.a = 1f;
                onfocusImageAry[i].color = temp;
            }
        }
    }
    private void GetFirstCardDetail(string cardVal, string cardType, GameObject g1, Sprite[] cardSprites)
    {
        CardData data = new CardData();

        switch (cardVal)
        {
            case "TEN":
                data.cardNumber = CardNumber.TEN;
                break;

            case "JACK":
                data.cardNumber = CardNumber.JACK;
                break;

            case "QUEEN":
                data.cardNumber = CardNumber.QUEEN;
                break;

            case "KING":
                data.cardNumber = CardNumber.KING;
                break;

            case "ACE":
                data.cardNumber = CardNumber.ACE;
                break;

            case "TWO":
                data.cardNumber = CardNumber.TWO;
                break;

            case "THREE":
                data.cardNumber = CardNumber.THREE;
                break;

            case "FOUR":
                data.cardNumber = CardNumber.FOUR;
                break;

            case "FIVE":
                data.cardNumber = CardNumber.FIVE;
                break;

            case "SIX":
                data.cardNumber = CardNumber.SIX;
                break;

            case "SEVEN":
                data.cardNumber = CardNumber.SEVEN;
                break;

            case "EIGHT":
                data.cardNumber = CardNumber.EIGHT;
                break;

            case "NINE":
                data.cardNumber = CardNumber.NINE;
                break;
        }
        
        switch (cardType)
        {
            case "CLUB":
                data.cardIcon = CardIcon.CLUB;
                break;

            case "DIAMOND":
                data.cardIcon = CardIcon.DIAMOND;
                break;

            case "HEART":
                data.cardIcon = CardIcon.HEART;
                break;

            case "SPADES":
                data.cardIcon = CardIcon.SPADES;
                break;                
        }
        
        int totalCardNumbers = Enum.GetNames(typeof(CardNumber)).Length - 1;
        int totalCardIcons = Enum.GetNames(typeof(CardIcon)).Length - 1;


        int cardNumber = totalCardNumbers - (int)data.cardNumber; // reverse order
        int cardIcon = totalCardIcons - (int)data.cardIcon; // reverse order

        g1.transform.GetChild(0).GetComponent<Image>().sprite = cardSprites[(cardIcon * 13) + cardNumber];
    }

    private void GetSecondCardDetail(string cardVal, string cardType, GameObject g1, Sprite[] cardSprites)
    {
        CardData data = new CardData();

        switch (cardVal)
        {
            case "TEN":
                data.cardNumber = CardNumber.TEN;
                break;

            case "JACK":
                data.cardNumber = CardNumber.JACK;
                break;

            case "QUEEN":
                data.cardNumber = CardNumber.QUEEN;
                break;

            case "KING":
                data.cardNumber = CardNumber.KING;
                break;

            case "ACE":
                data.cardNumber = CardNumber.ACE;
                break;

            case "TWO":
                data.cardNumber = CardNumber.TWO;
                break;

            case "THREE":
                data.cardNumber = CardNumber.THREE;
                break;

            case "FOUR":
                data.cardNumber = CardNumber.FOUR;
                break;

            case "FIVE":
                data.cardNumber = CardNumber.FIVE;
                break;

            case "SIX":
                data.cardNumber = CardNumber.SIX;
                break;

            case "SEVEN":
                data.cardNumber = CardNumber.SEVEN;
                break;

            case "EIGHT":
                data.cardNumber = CardNumber.EIGHT;
                break;

            case "NINE":
                data.cardNumber = CardNumber.NINE;
                break;
        }


        switch (cardType)
        {
            case "CLUB":
                data.cardIcon = CardIcon.CLUB;
                break;

            case "DIAMOND":
                data.cardIcon = CardIcon.DIAMOND;
                break;

            case "HEART":
                data.cardIcon = CardIcon.HEART;
                break;

            case "SPADES":
                data.cardIcon = CardIcon.SPADES;
                break;
        }

        int totalCardNumbers = Enum.GetNames(typeof(CardNumber)).Length - 1;
        int totalCardIcons = Enum.GetNames(typeof(CardIcon)).Length - 1;


        int cardNumber = totalCardNumbers - (int)data.cardNumber; // reverse order
        int cardIcon = totalCardIcons - (int)data.cardIcon; // reverse order

        g1.transform.GetChild(1).GetComponent<Image>().sprite = cardSprites[(cardIcon * 13) + cardNumber];
    }   
}
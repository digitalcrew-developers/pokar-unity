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

public class HandUiManager : MonoBehaviour
{
    public Transform container;
    public Image[] onfocusImageAry;
    public GameObject handPrefab;

    public GameObject image;

    private DirectoryInfo dir;
    private FileInfo[] info;

    private GameObject handObject, videoObject;

    private VideoPlayer videoPlayer;
    private VideoSource videoSource;

    private AudioSource audioSource;

    void Start()
    {
        for (int i = 0; i < container.childCount; i++)
        {
            Destroy(container.GetChild(i).gameObject);
        }


        dir = new DirectoryInfo(Path.Combine(Application.persistentDataPath, "Video"));
        info = dir.GetFiles("*.mp4");

        Sprite[] cardSprites = Resources.LoadAll<Sprite>("cards");

        //data.cardsSprite = cardSprites[(cardIcon * 13) + cardNumber];

        for (int j = 0; j < info.Length; j++)
        {
            string[] x = info[j].Name.Split('_');
            Debug.Log("File --> " + info[j].Name + "    Length : " + x.Length);

            if (x.Length == 7)
            {
                Debug.Log("Currepted FIle !!!!!!");
                File.Delete(info[j].FullName);
            }
            /*else
            {
                Debug.Log("No file to delete");
                if (x[9].Length == 1)
                    x[9] = "0" + x[9];

                handObject = Instantiate(handPrefab, container) as GameObject;

                handObject.transform.GetChild(2).GetComponent<Text>().text = x[7] + " " + x[8] + " : " + x[9];
                handObject.transform.GetChild(5).GetComponent<Text>().text = x[1] + "/" + x[2];

                GetFirstCardDetail(x[4], x[3], handObject, cardSprites);
                GetSecondCardDetail(x[6], x[5], handObject, cardSprites);

                //handObject.GetComponent<Button>().onClick.AddListener(() => OnClickOnPlayButton(info[j - 1].Name));
            }*/
        }

        dir = new DirectoryInfo(Path.Combine(Application.persistentDataPath, "Video"));
        info = dir.GetFiles("*.mp4");

        foreach (FileInfo f in info)
        {
            string[] x = f.Name.Split('_');

            Debug.Log("File --> " + f.Name + "    Length : " + x.Length);
            /*for (int i = 0; i < x.Length; i++)
            {
                Debug.Log("Val "+i+" : "+ x[i] + " Length: " + x[i].Length);                
            }*/
            
            Debug.Log("Instantiate Object");
            handObject = Instantiate(handPrefab, container) as GameObject;

            if (x[9].Length == 1)
                x[9] = "0" + x[9];

            handObject.transform.GetChild(2).GetComponent<Text>().text = x[7] + " " + x[8] + " : " + x[9];
            handObject.transform.GetChild(5).GetComponent<Text>().text = x[1] + "/" + x[2];

            GetFirstCardDetail(x[4], x[3], handObject, cardSprites);
            GetSecondCardDetail(x[6], x[5], handObject, cardSprites);

            handObject.GetComponent<Button>().onClick.AddListener(() => OnClickOnPlayButton(f.Name));            
        }

        

        /*Debug.Log("Container Count: " + container.childCount);*/
        /*for (int k = 0; k < container.childCount; k++)
        {
            container.GetChild(k).GetComponent<Button>().onClick.AddListener(() => OnClickOnPlayButton(info[k-1].Name));
        }*/

        /*foreach (FileInfo f in info)
        {
            handObject.GetComponent<Button>().onClick.AddListener(() => OnClickOnPlayButton(f.Name));
        }*/


            ChangeBtnFocus(0);
        //GetAllVideoList(true);        
    }

    private void OnClickOnPlayButton(string name)
    {
        Debug.Log("Name @@@@@@@@@@" + name);
        StartCoroutine(PlayVideo(name));
    }

    IEnumerator PlayVideo(string vn)
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
        videoPlayer.url = Path.Combine(Application.persistentDataPath, "Video", vn);/*Application.persistentDataPath + "/Video/" + vn;*/

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
        videoObject = Instantiate(image, this.transform) as GameObject;
        //Assign the Texture from Video to RawImage to be displayed
        videoObject.GetComponent<RawImage>().texture = videoPlayer.texture;

        //Assign back button click listner
        videoObject.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() => ClickBackBtn());

        //Play Video
        videoPlayer.Play();

        //Play Sound
        audioSource.Play();

        /*Debug.Log("Playing Video");*/
        while (videoPlayer.isPlaying)
        {
            //Debug.Log("Video Time: " + Mathf.FloorToInt((float)videoPlayer.time));
            yield return null;
        }
        /*Debug.Log("Done Playing Video");*/
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

                /*default:
                    int numberIndex = int.Parse(cardVal.ToString());
                    data.cardNumber = (CardNumber)(numberIndex - 2);
                    break; */
        }
        /*Debug.Log("Card Number: !!!! " + data.cardNumber);*/

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

            /*default:
                int numberIndex = int.Parse(cardVal);
                data.cardNumber = (CardNumber)(numberIndex - 2);
                break;*/
        }
        /*Debug.Log("Card Number: !!!! " + data.cardNumber);*/

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
             /*default:
                    int numberIndex = int.Parse(cardVal.ToString());
                    data.cardNumber = (CardNumber)(numberIndex - 2);
                    break;*/
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

            /*default:
                int numberIndex = int.Parse(cardVal.ToString());
                data.cardNumber = (CardNumber)(numberIndex - 2);
                break;*/
        }

        int totalCardNumbers = Enum.GetNames(typeof(CardNumber)).Length - 1;
        int totalCardIcons = Enum.GetNames(typeof(CardIcon)).Length - 1;


        int cardNumber = totalCardNumbers - (int)data.cardNumber; // reverse order
        int cardIcon = totalCardIcons - (int)data.cardIcon; // reverse order

        g1.transform.GetChild(1).GetComponent<Image>().sprite = cardSprites[(cardIcon * 13) + cardNumber];
    }

    public void ClickBackBtn()
    {
        Destroy(videoObject);
    }

    public void OnClickBtnClose()
    {
        //MainMenuController.instance.DestroyScreen(ScreenLayer.LAYER2);
        Destroy(gameObject);
    }

    public void GetAllVideoList(bool isShowLoading)
    {
        ChangeBtnFocus(0);
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
}
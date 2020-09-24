using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class ForumFeedUIManager : MonoBehaviour
{


    public int forumId;
    public int userId;
    public string likeStatus;
    public Image iconImage;
    public Text userNametxt;
    public Text titleDatetxt;
    public Text discriptiontxt;
    public Text viewTranslationtxt;
    public GameObject videoBtn;
    public Text videoCountertxt;
    public bool isLike;
    public GameObject likeBtn;
    public Text likeCounter;
    public GameObject commentBtn;
    public Text commentCounter;
    public GameObject shareBtn;

    public GameObject likeIconBtn;
    public GameObject unlikeIconbtn;

    //DEV_CODE
    public RawImage videoBGImage, videoFrontImage;

    public RawImage videoObject;

    public string videoPath;

    private Slider tracking;
    private VideoSource videoSource;

    private AudioSource audioSource;

    //    {
    //	"forumId":1,
    //    "userId":8,
    //    "likeStatus":1
    //}

    void Start()
    {
        if (isLike)
        {
            likeIconBtn.SetActive(true);
            unlikeIconbtn.SetActive(false);
        }

        videoBtn.GetComponent<Button>().onClick.AddListener(OnClickVideoBtn);
    }


    public void ClickLikeIcon()
    {

        likeIconBtn.SetActive(true);
        unlikeIconbtn.SetActive(false);

        if (!isLike)
        {
            PostLike();
        }

    }
    public void ClickCommentIcon(bool isShowLoading)
    {

        
        ForumListUIManager.instance.commentPannel.SetActive(true);
        ForumListUIManager.instance.commentPannelCommentObj.SetActive(true);
        ForumListUIManager.instance.commentPannel.GetComponent<ForumCommentPannel>().GetComment(true,forumId,userId);
        
    }

    public void OnClickVideoBtn()
    {
        ForumListUIManager.instance.commentPannel.SetActive(true);
        ForumListUIManager.instance.commentPannelVedioObj.SetActive(true);
        ForumListUIManager.instance.commentPannelCommentObj.SetActive(false);
        ForumListUIManager.instance.commentPannel.GetComponent<ForumCommentPannel>().GetComment(true, forumId, userId);

        if(ForumListUIManager.instance.isMinePanel)
        {
            StartCoroutine(PlayVideo(videoPath));
        }
    }

    IEnumerator PlayVideo(string vn)
    {
        if (!ForumListUIManager.instance.videoPlayer.GetComponent<VideoPlayer>() && !ForumListUIManager.instance.videoPlayer.GetComponent<AudioSource>())
        {
            ForumListUIManager.instance.videoPlayer = ForumListUIManager.instance.videoPlayer.gameObject.AddComponent<VideoPlayer>();
            //ForumListUIManager.instance.audioSource = ForumListUIManager.instance.videoPlayer.gameObject.AddComponent<AudioSource>();
        }

        ForumListUIManager.instance.videoPlayer.playOnAwake = false;
        //ForumListUIManager.instance.audioSource.playOnAwake = false;
        //audioSource.Pause();

        ForumListUIManager.instance.videoPlayer.source = VideoSource.Url;
        ForumListUIManager.instance.videoPlayer.url = Path.Combine(Application.persistentDataPath, "Videos", vn);

        ForumListUIManager.instance.videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        //Assign the Audio from Video to AudioSource to be played
        ForumListUIManager.instance.videoPlayer.EnableAudioTrack(0, true);
        ForumListUIManager.instance.videoPlayer.SetTargetAudioSource(0, audioSource);

        //Set video To Play then prepare Audio to prevent Buffering
        //videoPlayer.clip = videoToPlay;
        ForumListUIManager.instance.videoPlayer.Prepare();

        //Wait until video is prepared
        WaitForSeconds waitTime = new WaitForSeconds(1);
        while (!ForumListUIManager.instance.videoPlayer.isPrepared)
        {
            /*Debug.Log("Preparing Video");*/
            //Prepare/Wait for 5 sceonds only
            yield return waitTime;
            //Break out of the while loop after 5 seconds wait
            break;
        }

        /*Debug.Log("Done Preparing Video");*/

        //Assign the Texture from Video to RawImage to be displayed
        //videoObject.texture = videoPlayer.texture;

        //Play Video
        ForumListUIManager.instance.videoPlayer.Play();

        //Play Sound
        //audioSource.Play();

        /*Debug.Log("Playing Video");*/
        /*while (videoPlayer.isPlaying)
        {
            //Debug.Log("Video Time: " + Mathf.FloorToInt((float)videoPlayer.time));

            tracking.value = (float)videoPlayer.frame / (float)videoPlayer.frameCount;

            yield return null;
        }*/
        /*Debug.Log("Done Playing Video");*/
    }

    public void PostLike(bool isShowLoading = true)
    {

        string requestData = "{\"forumId\":\"" + forumId + "\"," +
                           "\"userId\":\"" + userId + "\"," +
                           "\"likeStatus\":\"1\"}";

        if (isShowLoading)
        {
            MainMenuController.instance.ShowScreen(MainMenuScreens.Loading);
        }
        WebServices.instance.SendRequest(RequestType.PostLike, requestData, true, OnServerResponseFound);
    }




    public void OnServerResponseFound(RequestType requestType, string serverResponse, bool isShowErrorMessage, string errorMessage)
    {

        MainMenuController.instance.DestroyScreen(MainMenuScreens.Loading);

        if (errorMessage.Length > 0)
        {
            if (isShowErrorMessage)
            {
                MainMenuController.instance.ShowMessage(errorMessage);
            }

            return;
        }

        if (requestType == RequestType.PostLike)
        {
            JsonData data = JsonMapper.ToObject(serverResponse);

            if (data["success"].ToString() == "0")
            {
                if (data["message"].ToString() == "Like data added successfully")
                {
                    ForumListUIManager.instance.GetAllForumList(false);
                }

            }
            else
            {
                MainMenuController.instance.ShowMessage(data["message"].ToString());
            }
        }
    }

}

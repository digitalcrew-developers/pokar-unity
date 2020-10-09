using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;
using UnityEngine.Video;

public class ForumListUIManager : MonoBehaviour, IPointerDownHandler
{
    public static ForumListUIManager instance;
    public LayoutManager layoutManager;
    public GameObject forumFeedPrefab;
    public GameObject forumFeatureFeedPrefab;
    public Transform container;

    public Image[] onfocusImageAry;

    public Button backButton;

    public GameObject forumListPannel;
    public GameObject commentPannel;
    public GameObject commentPannelCommentObj;
    public GameObject commentPannelVedioObj;
    public GameObject commentPrefab;

    //DEV_CODE
    public GameObject addVideoPanel;
    private bool isAddVideoPanelActive = false;
    public bool isMinePanel = false;

    public VideoPlayer videoPlayer;

    private float width;
    private float height;

    public List<GameObject> panels = new List<GameObject>();

    public void Awake()
    {
        instance = this;
        width = (float)Screen.width;
        height = (float)Screen.height;
    }

    // Start is called before the first frame update
    void Start()
    {
        backButton.onClick.AddListener(ClickBackBtn);
        GetAllForumList(true);
        commentPannel.SetActive(false);

        //DEV_CODE
        addVideoPanel.SetActive(false);
    }

    public void OnPointerDown(PointerEventData a)
    {
        if (addVideoPanel.activeSelf)
        {
            addVideoPanel.SetActive(false);
            isAddVideoPanelActive = false;
        }
    }

    public void ClickBackBtn() {
        MainMenuController.instance.OnClickOnButton("menu");
    }
    public void OnClickVedioOnShowCommentObj()
    {
        commentPannelVedioObj.SetActive(true);
        commentPannelCommentObj.SetActive(false);
    }
    public void OnClickCommentbtnOnVedioObj()
    {
        commentPannelVedioObj.SetActive(true);
        commentPannelCommentObj.SetActive(true);

        //DEV_CODE
        videoPlayer.source = VideoSource.VideoClip;

        //ForumListUIManager.instance.commentPannel.GetComponent<ForumCommentPannel>().GetComment(true, forumId, userId);
    }
    public void OnClickBackBtnOnVedioObj()
    {
        commentPannel.SetActive(false);
        commentPannelVedioObj.SetActive(false);
    }
    public void BackBtnCommentPannel()
    {

        commentPannel.SetActive(false);
        GetAllForumList(true);
    }

    public void GetAllForumList(bool isShowLoading)
    {
        ChangeBtnFocus(0);
        string requestData = "{\"userId\":\"" + PlayerManager.instance.GetPlayerGameData().userId + "\"," +
                             "\"FilterUserID\":\"" + "" + "\"}";

        if (isShowLoading)
        {
            MainMenuController.instance.ShowScreen(MainMenuScreens.Loading);
        }

        WebServices.instance.SendRequest(RequestType.GetForum, requestData, true, OnServerResponseFound);
    }
    public void GetHotForumList(bool isShowLoading)
    {

        ChangeBtnFocus(1);
        string requestData = "{\"userId\":\"" + PlayerManager.instance.GetPlayerGameData().userId + "\"," +
                              "\"FilterUserID\":\"" + "" + "\"}";
        if (isShowLoading)
        {
            MainMenuController.instance.ShowScreen(MainMenuScreens.Loading);
        }
        WebServices.instance.SendRequest(RequestType.GetForum, requestData, true, OnServerResponseFound);
    }
    public void GetLatestForumList(bool isShowLoading)
    {

        ChangeBtnFocus(2);
        string requestData = "{\"userId\":\"" + PlayerManager.instance.GetPlayerGameData().userId + "\"," +
                             "\"FilterUserID\":\"" + "" + "\"}";
        if (isShowLoading)
        {
            MainMenuController.instance.ShowScreen(MainMenuScreens.Loading);
        }
        WebServices.instance.SendRequest(RequestType.GetForum, requestData, true, OnServerResponseFound);
    }

    public void GetMineForumList(bool isShowLoading)
    {
        isMinePanel = true;
        ChangeBtnFocus(3);
        string requestData = "{\"userId\":\"" + PlayerManager.instance.GetPlayerGameData().userId + "\"," +
                             "\"FilterUserID\":\"" + PlayerManager.instance.GetPlayerGameData().userId + "\"}";
        if (isShowLoading)
        {
            MainMenuController.instance.ShowScreen(MainMenuScreens.Loading);
        }
        WebServices.instance.SendRequest(RequestType.GetForum, requestData, true, OnServerResponseFound);
    }


    void ChangeBtnFocus(int focusVal) {

        if (focusVal == 3)
            isMinePanel = true;
        else
            isMinePanel = false;


        for (int i = 0; i < onfocusImageAry.Length; i++)
        {
            if (i != focusVal)
            {
                Color temp = onfocusImageAry[i].color;
                temp.a = 0.01f;
                onfocusImageAry[i].color = temp;
            }
            else {
                Color temp = onfocusImageAry[i].color;
                temp.a = 1f;
                onfocusImageAry[i].color = temp;
            }
        }

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

        if (requestType == RequestType.GetForum)
        {
            JsonData data = JsonMapper.ToObject(serverResponse);

            if (data["success"].ToString() == "0")
            {
                ShowForumList(data);
            }
            else
            {
                //MainMenuController.instance.ShowMessage(data["message"].ToString());
            }
        }
    }

    private void ShowForumList(JsonData data)
    {
        for (int i = 0; i < container.childCount; i++)
        {
            Destroy(container.GetChild(i).gameObject);
        }


        for (int i = 0; i < data["data"].Count; i++)
        {

            GameObject gm1 = Instantiate(forumFeedPrefab, container) as GameObject;

            gm1.GetComponent<ForumFeedUIManager>().forumId = (int)data["data"][i]["forumId"];
            gm1.GetComponent<ForumFeedUIManager>().userId = (int)data["data"][i]["userId"];
            gm1.GetComponent<ForumFeedUIManager>().likeStatus = data["data"][i]["isLiked"].ToString();
            gm1.GetComponent<ForumFeedUIManager>().userNametxt.text = data["data"][i]["userName"].ToString();
            gm1.GetComponent<ForumFeedUIManager>().discriptiontxt.text = data["data"][i]["description"].ToString();
            gm1.GetComponent<ForumFeedUIManager>().likeCounter.text = data["data"][i]["TotalLikes"].ToString();
            gm1.GetComponent<ForumFeedUIManager>().commentCounter.text = data["data"][i]["TotalComments"].ToString();

            if (isMinePanel)
            {
                Debug.Log("Path: " + data["data"][i]["forumName"].ToString().Substring(53));
                LoadImage(gm1, data["data"][i]["forumName"].ToString().Substring(53));

                if (!Directory.Exists(Path.Combine(Application.persistentDataPath, "Videos")))
                    Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "Videos"));

                gm1.GetComponent<ForumFeedUIManager>().videoPath = data["data"][i]["forumName"].ToString().Substring(53);
            }

            if (data["data"][i]["isLiked"].ToString().Equals("No"))
            {
                gm1.GetComponent<ForumFeedUIManager>().isLike = false;
            }
            else if (data["data"][i]["isLiked"].ToString().Equals("Yes"))
            {
                gm1.GetComponent<ForumFeedUIManager>().isLike = true;
            }

            //if (data["data"][i]["assignRole"].ToString() != "Not Assign")
            //{
            //    GameObject gm = Instantiate(forumFeedPrefab, container) as GameObject;

            //    string uniqueClubId = data["data"][i]["uniqueClubId"].ToString();
            //    string clubName = data["data"][i]["clubName"].ToString();
            //    string clubId = data["data"][i]["clubId"].ToString();


            //    gm.transform.Find("ClubName").GetComponent<Text>().text = clubName;
            //    Transform stars = gm.transform.Find("Star");


            //    int activeStarCount = UnityEngine.Random.Range(2, stars.childCount);

            //    for (int k = 0; k < stars.childCount; k++)
            //    {
            //        if (k < activeStarCount)
            //        {
            //            stars.GetChild(k).gameObject.SetActive(true);
            //        }
            //        else
            //        {
            //            stars.GetChild(k).gameObject.SetActive(false);
            //        }
            //    }

            //    //gm.transform.Find("ClubId").GetComponent<Text>().text = "ClubId : " + uniqueClubId;
            //    //gm.GetComponent<Button>().onClick.AddListener(() => OnClickOnClub(clubName, uniqueClubId, clubId));
            //}
        }

        //layoutManager.UpdateLayout();
    }


    //DEV_CODE
    public void LoadImage(GameObject obj, string path)
    {
        string newPath = "Image";

        string[] x = path.Split('_');
        for (int i = 1; i < x.Length - 1; i++)
        {
            //Debug.Log("Val " + i + " : " + x[i] + " Length: " + x[i].Length);
            newPath = newPath + "_" + x[i];
        }

        if (!Directory.Exists(Path.Combine(Application.persistentDataPath, "Screenshots")))
            Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "Screenshots"));

        byte[] byteArray = File.ReadAllBytes(Path.Combine(Application.persistentDataPath, "Screenshots", newPath + "_.png"));
        Texture2D sampleTexture = new Texture2D(2, 2);
        // the size of the texture will be replaced by image size
        bool isLoaded = sampleTexture.LoadImage(byteArray);

        // apply this texure as per requirement on image or material
        if (isLoaded)
        {
            obj.GetComponent<ForumFeedUIManager>().videoBGImage.texture = sampleTexture;
            obj.GetComponent<ForumFeedUIManager>().videoFrontImage.texture = sampleTexture;
        }
    }

    public void OnClickVideoAddObject()
    {
        if (!isAddVideoPanelActive)
        {
            addVideoPanel.SetActive(true);
            isAddVideoPanelActive = true;
        }
        else
        {
            addVideoPanel.SetActive(false);
            isAddVideoPanelActive = false;
        }
    }

    public void OnClickOpenPanel(string panelName)
    {
        switch(panelName)
        {
            case "hand":
                MainMenuController.instance.ShowScreen(MainMenuScreens.HandScreen);
                break;

            case "text":
                //MainMenuController.instance.ShowScreen(MainMenuScreens.NewPost);
                //GameObject gm = Instantiate(panels[1], container) as GameObject;
                //HandPostPanel.instance.isFromForum = true;
                break;
        }
    }   
}
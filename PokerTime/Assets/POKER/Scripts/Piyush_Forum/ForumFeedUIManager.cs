using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        ForumListUIManager.instance.commentPannel.GetComponent<ForumCommentPannel>().GetComment(true,forumId,userId);
        //for(int i = 0;i < 10;i++)
        //    {
        //    Debug.Log("Comment is CLICK"); 
        //    GameObject g = Instantiate(ForumListUIManager.instance.commentPrefab, ForumListUIManager.instance.commentPannel.GetComponent<ForumCommentPannel>().Container) as GameObject;

        //}



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

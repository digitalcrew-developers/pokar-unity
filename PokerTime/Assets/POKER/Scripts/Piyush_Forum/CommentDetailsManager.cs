using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommentDetailsManager : MonoBehaviour
{
	public Image commentImage;
	public Image likedImage;
	public int commentId;
	public int forumId;
	public Text commentDesprictionTxt;
	public Text usernameTxt;
	public Text DateTxt;
	public Text likeCounterTxt;
	public int userId;
	public Text ChildComments;

	public void LikeComment() {

        PostLike(true);

    }


    public void PostLike(bool isShowLoading = true)
    {

        string requestData = "{\"forumId\":\"" + forumId + "\"," +
                          "\"userId\":\"" + PlayerManager.instance.GetPlayerGameData().userId + "\"," +
                          "\"commentId\":\"" + commentId + "\"," +
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
                    likedImage.gameObject.SetActive(true);
                }

            }
            else
            {
                MainMenuController.instance.ShowMessage(data["message"].ToString());
            }
        }
    }

}

using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ForumCommentPannel : MonoBehaviour
{
    public static ForumCommentPannel instance;


    public int forumId;
    public int userId;
    public Transform Container;
    public GameObject commentPrefabs;
    public  InputField iField;



    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Comment Pannel Start");
    }
    private void OnEnable()
    {
        Debug.Log("Comment Pannel OnEnable  2");
    }

    public void GetComment(bool isShowLoading, int forumIdval,int userIdval)
    {
        iField.text = "";
        for (int i = 0; i < Container.childCount; i++)
        {
            DestroyImmediate(Container.GetChild(i).gameObject);
        }
       
        forumId = forumIdval;
        userId = userIdval;

        string requestData = "{\"forumId\":\"" + forumId + "\"}";

        if (isShowLoading)
        {
            MainMenuController.instance.ShowScreen(MainMenuScreens.Loading);
        }
        WebServices.instance.SendRequest(RequestType.GetComment, requestData, true, OnServerResponseFound);
    }


    public void PostComment(bool isShowLoading)
    {

        
        string requestData = "{\"forumId\":\"" + forumId + "\"," +
                           "\"userId\":\"" + userId + "\"," +
                           "\"comment\":\""+iField.text+"\"}";
        if (isShowLoading)
        {
            MainMenuController.instance.ShowScreen(MainMenuScreens.Loading);
        }
        WebServices.instance.SendRequest(RequestType.PostComment, requestData, true, OnServerResponseFound);
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

        if (requestType == RequestType.GetComment)
        {
            JsonData data = JsonMapper.ToObject(serverResponse);


            if (data["success"].ToString() == "0")
            {
                                
                for (int i = 0; i < data["data"].Count; i++)
                {
                    GameObject gm1 = Instantiate(commentPrefabs, Container) as GameObject;
                   
                    gm1.GetComponent<CommentDetailsManager>().usernameTxt.text = data["data"][i]["userName"].ToString();
                    gm1.GetComponent<CommentDetailsManager>().commentDesprictionTxt.text =data["data"][i]["comment"].ToString();
                    gm1.GetComponent<CommentDetailsManager>().userId = (int)data["data"][i]["userId"];
                }
            }
            else
            {
                MainMenuController.instance.ShowMessage(data["message"].ToString());
            }
        }
        if (requestType == RequestType.PostComment)
        {
            Debug.Log("IAKKKKKKKKKKKKKKK");
            iField.text = "";
        }
        }
}

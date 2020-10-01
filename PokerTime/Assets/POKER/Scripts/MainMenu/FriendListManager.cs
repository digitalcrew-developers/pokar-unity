using BestHTTP.Extensions;
using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendListManager : MonoBehaviour
{
    public Button friends, FriendList;
    public Transform container;
    public GameObject FriendsPrefeb, RequestFriendsPrefab;
    public Text emptylistmsg;
    // Start is called before the first frame update
    void Start()
    {
        OnClickBtn("Friends");
    }

    public void OnClickBtn(string eventName)
    {
        SoundManager.instance.PlaySound(SoundType.Click);
        friends.interactable = true;
        FriendList.interactable = true;
        switch (eventName)
        {
            case "close":
                {
                    MainMenuController.instance.DestroyScreen(MainMenuScreens.FriendList);
                }
                break;

            case "Friends":
                {
                    friends.interactable = false;
                    friends.GetComponent<Image>().color = new Color(1, 1, 1, 1);
                    FriendList.GetComponent<Image>().color = new Color(1, 1, 1, 0);
                    //    friends.GetComponent<Image>().color.a = 0;
                    for (int i = 0; i < container.childCount; i++)
                    {
                        Destroy(container.GetChild(i).gameObject);
                    }
                    string requestData = "{\"userId\":\"" + PlayerManager.instance.GetPlayerGameData().userId + "\"}";

                    WebServices.instance.SendRequest(RequestType.GetFriendList, requestData, true, OnServerResponseFound);
                }
                break;

            case "FriendList":
                {
                    FriendList.interactable = false;
                    friends.GetComponent<Image>().color = new Color(1, 1, 1, 0);
                    FriendList.GetComponent<Image>().color = new Color(1, 1, 1, 1);
                    for (int i = 0; i < container.childCount; i++)
                    {
                        Destroy(container.GetChild(i).gameObject);
                    }
                    string requestData = "{\"userId\":\"" + PlayerManager.instance.GetPlayerGameData().userId + "\"," +
                                   "\"Status\":\"" + "Pending" + "\"}";

                    WebServices.instance.SendRequest(RequestType.GetAllFriendRequest, requestData, true, OnServerResponseFound);
                }
                break;
            default:
#if ERROR_LOG
                Debug.LogError("unhdnled eventName found in LobbyUiManager = " + eventName);
#endif
                break;
        }
    }
    #region Friends

    public void OnServerResponseFound(RequestType requestType, string serverResponse, bool isShowErrorMessage, string errorMessage)
    {
        if (errorMessage.Length > 0)
        {
            if (isShowErrorMessage)
            {
                Debug.LogError("Error API IS :=> " + errorMessage);
                MainMenuController.instance.ShowMessage(errorMessage);
            }
            return;
        }
        if (requestType == RequestType.GetFriendList)
        {
            JsonData data = JsonMapper.ToObject(serverResponse);
            if (data["status"].Equals(true))
            {
                if (data["getData"].Count != 0)
                {
                    for (int i = 0; i < data["getData"].Count; i++)
                    {
                        emptylistmsg.text = "";
                        GameObject fl = Instantiate(FriendsPrefeb, container);
                        fl.GetComponent<GetFrienddata>().userName.text = data["getData"][i]["nickName"].ToString();
                        fl.GetComponent<GetFrienddata>().id = data["getData"][i]["id"].ToString();
                        fl.GetComponent<GetFrienddata>().UserId = data["getData"][i]["userId"].ToString();
                  //      StopAllCoroutines();
                        StartCoroutine(loadSpriteImageFromUrl(data["getData"][i]["profileImage"].ToString(), fl.GetComponent<GetFrienddata>().profile));
                        StartCoroutine(loadSpriteImageFromUrl(data["getData"][i]["frameURL"].ToString(), fl.GetComponent<GetFrienddata>().frame));
                    }
                }
                else
                {
                    emptylistmsg.text = "Send friend requests to players in Lobby game or Global Tournament!";
                }
            }
        }
        else if (requestType == RequestType.GetAllFriendRequest)
        {
            JsonData data = JsonMapper.ToObject(serverResponse);
            if (data["status"].Equals(true))
            {
                if (data["getData"].Count >= 1)
                {
                    for (int i = 0; i < data["getData"].Count; i++)
                    {
                        emptylistmsg.text = "";
                        
                        GameObject fl = Instantiate(RequestFriendsPrefab, container);
                        fl.GetComponent<GetFrienddata>().userName.text = data["getData"][i]["nickName"].ToString();
                        fl.GetComponent<GetFrienddata>().id = data["getData"][i]["id"].ToString();
                        fl.GetComponent<GetFrienddata>().UserId = data["getData"][i]["userId"].ToString();
                        fl.GetComponent<GetFrienddata>().status = data["getData"][i]["Status"].ToString();
                      //  StopAllCoroutines();
                        StartCoroutine(loadSpriteImageFromUrl(data["getData"][i]["profileImage"].ToString(), fl.GetComponent<GetFrienddata>().profile));
                        //StartCoroutine(loadSpriteImageFromUrl(data["getData"][i]["frameURL"].ToString(), fl.GetComponent<GetFrienddata>().frame));
                    }
                }
                else
                {
                    emptylistmsg.text = "No Friend Request";
                }
            }
        }
        else
        {

#if ERROR_LOG
            Debug.LogError("Unhadnled response found in  = " + requestType);
#endif
        }
    }
    IEnumerator loadSpriteImageFromUrl(string URL, Image image)
    {
        WWW www = new WWW(URL);
        while (!www.isDone)
        {
            yield return null;
        }
        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.Log("Download failed" + image.gameObject.name);
        }
        else
        {
            Texture2D texture = new Texture2D(1, 1);
            www.LoadImageIntoTexture(texture);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            image.sprite = sprite;
        }
    }
    #endregion

    #region FriendsLISt
    #endregion
}
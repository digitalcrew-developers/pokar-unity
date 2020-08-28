using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

public class ChatManager : MonoBehaviour
{

    public static ChatManager instance;
    private List<ChatMessage> chatList = new List<ChatMessage>();


    private void Awake()
    {
        instance = this;
    }

    public void OnChatMessageReceived(string serverResponse)
    {
        JsonData jsonData = JsonMapper.ToObject(serverResponse);

        if (jsonData[0]["from"].ToString() != PlayerManager.instance.GetPlayerGameData().userId)
        {
            ChatMessage data = new ChatMessage();
            data.desc = jsonData[0]["desc"].ToString();
            data.title = jsonData[0]["title"].ToString();
            data.isMe = false;
            chatList.Add(data);
        }

        if (ChatUiManager.instance != null)
        {
            ChatUiManager.instance.UpdateChatList();
        }
    }

    public void SendChatMessage(string messageToSend)
    {
        ChatMessage chatMessage = new ChatMessage();
        chatMessage.desc = messageToSend;
        chatMessage.isMe = true;
        chatMessage.title = GetUserName();
        chatList.Add(chatMessage);
        SocketController.instance.SendChatMessage(chatMessage.title, messageToSend);
    }

    private string GetUserName()
    {
        string userName = PlayerManager.instance.GetPlayerGameData().userName;
        userName += System.DateTime.Now.Hour + ":" + System.DateTime.Now.Minute + ":" + System.DateTime.Now.Second;

        return userName;
    }

    public List<ChatMessage> GetChatList()
    {
        return chatList;
    }

}

public class ChatMessage
{
    public string userName;
    public bool isMe;
    public string desc,title;
}
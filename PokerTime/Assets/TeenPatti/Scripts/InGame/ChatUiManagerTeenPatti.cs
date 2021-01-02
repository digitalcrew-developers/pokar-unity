﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ChatUiManagerTeenPatti : MonoBehaviour
{
    public static ChatUiManagerTeenPatti instance;
    public InputField inputFild;
    public GameObject inComingPrefab, outGoingPrefab,suggestionScreen;
    public Transform container;
    public LayoutManager layoutManager;

    bool isSuggestionShow = false;
    public GameObject suggestionBtnArrow;


    private void Awake()
    {
        instance = this;
    }

    private void OnDestroy()
    {
        instance = null;
    }

    private void Start()
    {
        UpdateChatList();
        SuggestionArrowManager();
    }


    void SuggestionArrowManager() {
        if (isSuggestionShow)
        {
            suggestionScreen.SetActive(true);
            suggestionBtnArrow.transform.localScale = new Vector3(suggestionBtnArrow.transform.localScale.x, suggestionBtnArrow.transform.localScale.y * (-1), suggestionBtnArrow.transform.localScale.z);
        }
        else {
            {
                suggestionScreen.SetActive(false);
                if (suggestionBtnArrow.transform.localScale.y < 0)
                {
                    suggestionBtnArrow.transform.localScale = new Vector3(suggestionBtnArrow.transform.localScale.x, suggestionBtnArrow.transform.localScale.y * (-1), suggestionBtnArrow.transform.localScale.z);

                }
                else {
                    suggestionBtnArrow.transform.localScale = new Vector3(suggestionBtnArrow.transform.localScale.x, suggestionBtnArrow.transform.localScale.y , suggestionBtnArrow.transform.localScale.z);

                }
            }
        }
    }


    public void UpdateChatList()
    {
        for (int i = 0; i < container.childCount; i++)
        {
            Destroy(container.GetChild(i).gameObject);
        }

        List<ChatMessage> chatList = ChatManagerTeenPatti.instance.GetChatList();

        for (int i = 0; i < chatList.Count; i++)
        {
            GameObject gm = null;

            if (chatList[i].isMe)
            {
                gm = Instantiate(outGoingPrefab, container) as GameObject;
                gm.GetComponent<InOutMsgUIManager>().userId = PlayerManager.instance.GetPlayerGameData().userId;
            }
            else
            {
                gm = Instantiate(inComingPrefab,container) as GameObject;
                gm.GetComponent<InOutMsgUIManager>().userId = chatList[i].userId;
            }

            gm.transform.Find("Title").GetComponent<Text>().text = chatList[i].title;
           // Debug.Log("HIHIHIHII    "+ chatList[i].desc);
           // gm.transform.Find("Desc").GetComponent<Text>().text = chatList[i].desc;
            gm.transform.Find("ADesc").GetChild(0).GetComponent<Text>().text = chatList[i].desc;

          //  gm.transform.GetComponent<InOutMsgUIManager>().userId= chatList[i].us
        }


        layoutManager.UpdateLayout();

    }


    public void OnClickOnButton(string eventName)
    {
        SoundManager.instance.PlaySound(SoundType.Click);

        switch (eventName)
        {
            case "back":
                {
                    if (suggestionScreen.activeInHierarchy)
                    {
                        suggestionScreen.SetActive(false);    
                    }

                    InGameUiManagerTeenPatti.instance.DestroyScreen(InGameScreensTeenPatti.Chat);
                }
            break;

            case "showSuggestion":
                {
                    if (!isSuggestionShow)
                    {
                        isSuggestionShow = true;
                    }
                    else {
                        isSuggestionShow = false;
                    }
                    SuggestionArrowManager();
                }
                break;

            case "send":
                {
                    if (inputFild.text.Length > 0)
                    {
                        ChatManagerTeenPatti.instance.SendChatMessage(inputFild.text);
                        InGameUiManagerTeenPatti.instance.DestroyScreen(InGameScreensTeenPatti.Chat);
                    }
                }
                break;

            default:
                {
                    Debug.LogError("Unhandled eventName found in ChatuiManager = "+eventName);
                }
            break;
        }
    }

    public void OnClickOnSuggestions(Text suggestionText)
    {
        SoundManager.instance.PlaySound(SoundType.Click);

        ChatManagerTeenPatti.instance.SendChatMessage(suggestionText.text);
        InGameUiManagerTeenPatti.instance.DestroyScreen(InGameScreensTeenPatti.Chat);
    }



   





}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotificationUiManager : MonoBehaviour
{
    public Sprite readImage, unreadImage;
    public GameObject notificationListScreen,notificationDetailsScreen,notificationPrefab;
    public Transform container;
    public Text notificationDetailsText;
    public LayoutManager layoutManager;

    private void Start()
    {
        notificationDetailsScreen.SetActive(false);
        notificationListScreen.SetActive(true);

        ShowNotificationList(MainMenuController.instance.GetNotificationDetails());
    }


    private void ShowNotificationList(NotificationDetails notificationDetails)
    {
        
        for (int i = 0; i < container.childCount; i++)
        {
            Destroy(container.GetChild(i).gameObject);
        }

        for (int i = 0; i < notificationDetails.notifications.Count; i++)
        {
            Notification notificationData = notificationDetails.notifications[i];

            GameObject gm = Instantiate(notificationPrefab,container) as GameObject;
            gm.transform.Find("Text").GetComponent<Text>().text = notificationData.title;
            Image icon = gm.transform.Find("Icon").GetComponent<Image>();

            if (notificationData.isRead)
            {
                icon.sprite = readImage;
            }
            else
            {
                icon.sprite = unreadImage;
            }

            gm.GetComponent<Button>().onClick.AddListener(()=> ShowDetaildMessage(notificationData, icon));
        }

        layoutManager.UpdateLayout();
    }


    private void ShowDetaildMessage(Notification data,Image icon)
    {
        notificationDetailsScreen.SetActive(true);
        notificationDetailsText.text = data.title;
        notificationDetailsText.text += "\n\n"+data.desc;

        if (!data.isRead)
        {
            icon.sprite = readImage;
            string requestData = "{\"firebaseNotificationId\":\"" + data.id + "\"," +
                        "\"userId\":\"" + PlayerManager.instance.GetPlayerGameData().userId + "\"," +
                        "\"type\":\"\"}";


            WebServices.instance.SendRequest(RequestType.UpdateNotificationMessage, requestData, true);
            MainMenuController.instance.UpdateReadMessage(data.id);

            if (MenuHandller.instance != null)
            {
                MenuHandller.instance.UpdateNotificationData(MainMenuController.instance.GetNotificationDetails().unreadMessageCount);
            }
        }
    }


    public void OnClickOnBack()
    {
        if (notificationDetailsScreen.activeInHierarchy)
        {
            notificationDetailsScreen.SetActive(false);
        }
        else
        {
            MainMenuController.instance.DestroyScreen(MainMenuScreens.Notification);
        }
    }
}


public class NotificationDetails
{
    public List<Notification> notifications = new List<Notification>();
    public int readMessageCount,unreadMessageCount;
}

public class Notification
{
    public string id;
    public string title;
    public string desc;
    public bool isRead;
}

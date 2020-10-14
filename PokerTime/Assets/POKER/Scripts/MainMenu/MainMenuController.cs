using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.IO;
using UnityEngine.UI;
using System.Linq;

public class MainMenuController : MonoBehaviour
{
	public static MainMenuController instance;

	public GameObject bottomPanel;

	public GameObject[] screens; // All screens prefab
	public Transform[] screenLayers; // screen spawn parent

	//DEV_CODE
	public GameObject[] bottomMenus;

	public bool isVIPFromShop = false;
	public bool isVIPFromProfile = false;

	private List<MainMenuActiveScreen> mainMenuActiveScreens = new List<MainMenuActiveScreen>();
	private NotificationDetails notificationDetails = new NotificationDetails();

	private Dictionary<MainMenuScreens, GameObject> availableScreens = new Dictionary<MainMenuScreens, GameObject>();


	private void Awake()
	{
		instance = this;		
	}


	private void Start()
	{
		LoadVideoAndScreenshot();
		MenuSelection(4);

		GlobalGameManager.IsJoiningPreviousGame = false;

		if (PlayerManager.instance.IsLogedIn())
		{
			//Activate bottom panel
			if (!bottomPanel.activeSelf)
				bottomPanel.SetActive(true);

			FetchUserData();
			FetchUserLogs();			
		}
		else
		{
			//Deactivate bottom panel
			if (bottomPanel.activeSelf)
				bottomPanel.SetActive(false);
			
			ShowScreen(MainMenuScreens.Registration);			
		}
	}

	private void LoadVideoAndScreenshot()
	{
		DirectoryInfo dir;
		FileInfo[] info;

		//Create directories to store videos and screenshots
		if (!Directory.Exists(Path.Combine(Application.persistentDataPath, "Videos")))
			Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "Videos"));
		
		if (!Directory.Exists(Path.Combine(Application.persistentDataPath, "Screenshots")))
			Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "Screenshots"));

		//Remove Currepted Videos
		dir = new DirectoryInfo(Path.Combine(Application.persistentDataPath, "Videos"));
		info = dir.GetFiles("*.mp4");

		for (int j = 0; j < info.Length; j++)
		{
			string[] x = info[j].Name.Split('_');

			//Removing currepted files.
			if (x.Length == 7)
			{
				File.Delete(info[j].FullName);
			}
		}

		//Remove Currepted Screenshots
		dir = new DirectoryInfo(Path.Combine(Application.persistentDataPath, "Screenshots"));
		info = dir.GetFiles("*.png");

		for (int j = 0; j < info.Length; j++)
		{
			string[] x = info[j].Name.Split('_');

			//Removing currepted files.
			if (x.Length == 7)
			{
				File.Delete(info[j].FullName);
			}
		}
	}

	public void OnClickOnButton(string eventName)
	{
		SoundManager.instance.PlaySound(SoundType.Click);


		switch (eventName)
		{

			case "menu":
				{
					MenuSelection(4);
					_ShowScreen(MainMenuScreens.MainMenu);
				}
				break;


			case "profile":
				{
					MenuSelection(3);
					_ShowScreen(MainMenuScreens.Profile);
				}
				break;

			case "shop":
				{
					MenuSelection(0);
					_ShowScreen(MainMenuScreens.Shop);
				}
				break;
			case "Forum":
				{
					MenuSelection(1);
					ShowScreen(MainMenuScreens.Forum);
				}
				break;
			case "career":
				{
					MenuSelection(2);
					ShowScreen(MainMenuScreens.Career);
				}
				break;

			default:
#if ERROR_LOG
				Debug.LogError("unhdnled eventName found in MainMenuController = " + eventName);
#endif
				break;
		}

	}


	private void MenuSelection(int index)
	{
        for (int i = 0; i < bottomMenus.Length; i++)
        {
			if (i == index)
			{
				bottomMenus[i].GetComponent<Image>().enabled = true;
				bottomMenus[i].transform.Find("Buttons").gameObject.SetActive(true);
			}
			else
			{
				bottomMenus[i].GetComponent<Image>().enabled = false;
				bottomMenus[i].transform.Find("Buttons").gameObject.SetActive(false);
			}
        }
	}



	private void FetchUserData()
	{
		string requestData = "{\"userName\":\"" + PlayerManager.instance.GetPlayerGameData().userName + "\"," +
			"\"userPassword\":\"" + PlayerManager.instance.GetPlayerGameData().password + "\"," +
			  "\"registrationType\":\"" +"Custom"+ "\"," +
			  "\"socialId\":\"" + PlayerManager.instance.GetPlayerGameData().password + "\"}";


		ShowScreen(MainMenuScreens.Loading);
		WebServices.instance.SendRequest(RequestType.Login, requestData, true, OnServerResponseFound);

		DownloadNotificationMessage();
	}
	private void FetchUserLogs()
	{
		string requestData = "{\"userId\":\"" + PlayerManager.instance.GetPlayerGameData().userId +  "\"}";
		//{ userId: 2}

		//WebServices.instance.SendRequest(RequestType.userLoginLogs, requestData, true, OnServerResponseFound);

	
	}
	public void DownloadNotificationMessage()
	{
		string notificationRequestData = "{\"userId\":\"" + PlayerManager.instance.GetPlayerGameData().userId + "\"}";
		WebServices.instance.SendRequest(RequestType.GetNotificationMessage, notificationRequestData, true, OnServerResponseFound);
	}

	public void UpdateReadMessage(string notificationid)
	{
		for (int i = 0; i < notificationDetails.notifications.Count; i++)
		{
			if (notificationDetails.notifications[i].id == notificationid)
			{
				notificationDetails.notifications[i].isRead = true;

				++notificationDetails.readMessageCount;
				--notificationDetails.unreadMessageCount;

				break;
			}
		}
	}


	private void PraseNotificationMessage(JsonData data)
	{
		notificationDetails.notifications.Clear();

		for (int i = 0; i < data["data"].Count; i++)
		{
			Notification notification = new Notification();
			notification.id = data["data"][i]["firebaseNotificationId"].ToString();

			notification.title = data["data"][i]["title"].ToString();
			notification.desc = data["data"][i]["body"].ToString();
			notification.isRead = data["data"][i]["isRead"].ToString() == "Yes";

			if (notification.isRead)
			{
				++notificationDetails.readMessageCount;
			}
			else
			{
				++notificationDetails.unreadMessageCount;
			}

			notificationDetails.notifications.Add(notification);
		}

		if (MenuHandller.instance != null)
		{
			MenuHandller.instance.UpdateNotificationData(notificationDetails.unreadMessageCount);
		}

	}

	public NotificationDetails GetNotificationDetails()
	{
		return notificationDetails;
	}

	public void _ShowScreen(MainMenuScreens screenName, object[] parameter = null)
	{
		List<GameObject> screenList = availableScreens.Values.ToList();

		for (int i = 0; i < screenList.Count; i++)
		{
			//Debug.Log("Disabling Screen: " + screenList[i].name);
			screenList[i].SetActive(false);
		}

		if (availableScreens.ContainsKey(screenName))
		{
			GameObject screen = availableScreens[screenName];
			screen.SetActive(true);
		}
		else
		{
			MainMenuActiveScreen mainMenuScreen = new MainMenuActiveScreen();
			mainMenuScreen.screenName = screenName;
			mainMenuScreen.screenLayer = GetScreenLayer(screenName);

			GameObject gm = Instantiate(screens[(int)screenName], screenLayers[/*(int)mainMenuScreen.screenLayer*/0]) as GameObject;
			//mainMenuScreen.screenObject = gm;
			availableScreens.Add(screenName, gm);

			switch (screenName)
			{
				case MainMenuScreens.Shop:
					{
						if (parameter != null)
						{
							gm.GetComponent<ShopUiManager>().ShowScreen((string)parameter[0]);
						}
						else
						{
							gm.GetComponent<ShopUiManager>().ShowScreen();
						}
					}
					break;

				default:
					break;
			}
		}
		
	}

	public void ShowScreen(MainMenuScreens screenName, object[] parameter = null)
	{
		int layer = (int)GetScreenLayer(screenName);
		for (int i = layer + 1; i < screenLayers.Length; i++)
		{
			DestroyScreen((ScreenLayer)i);
		}

		if (!IsScreenActive(screenName))
		{
			DestroyScreen(GetScreenLayer(screenName));

			MainMenuActiveScreen mainMenuScreen = new MainMenuActiveScreen();
			mainMenuScreen.screenName = screenName;
			mainMenuScreen.screenLayer = GetScreenLayer(screenName);

			GameObject gm = Instantiate(screens[(int)screenName], screenLayers[(int)mainMenuScreen.screenLayer]) as GameObject;
			mainMenuScreen.screenObject = gm;
			mainMenuActiveScreens.Add(mainMenuScreen);

			switch (screenName)
			{
				case MainMenuScreens.ClubDetails:
					gm.GetComponent<ClubDetailsUIManager>().Initialize((string)parameter[0], (string)parameter[1], (string)parameter[2]);
					break;


				case MainMenuScreens.GlobalTournament:
					{
						if (parameter != null)
						{
							gm.GetComponent<GlobalTournamentListUiManager>().ShowScreen((string)parameter[0]);
						}
						else
						{
							gm.GetComponent<GlobalTournamentListUiManager>().ShowScreen();
						}
					}
					break;


				case MainMenuScreens.Shop:
					{
						if (parameter != null)
						{
							gm.GetComponent<ShopUiManager>().ShowScreen((string)parameter[0]);
						}
						else
						{
							gm.GetComponent<ShopUiManager>().ShowScreen();
						}
					}
					break;

				default:
					break;
			}

		}
	}


	public void ShowMessage(string messageToShow, Action callBackMethod = null, string okButtonText = "Ok")
	{
		if (!IsScreenActive(MainMenuScreens.Message))
		{
			MainMenuActiveScreen mainMenuScreen = new MainMenuActiveScreen();
			mainMenuScreen.screenName = MainMenuScreens.Message;
			mainMenuScreen.screenLayer = GetScreenLayer(MainMenuScreens.Message);

			GameObject gm = Instantiate(screens[(int)MainMenuScreens.Message], screenLayers[(int)mainMenuScreen.screenLayer]) as GameObject;
			mainMenuScreen.screenObject = gm;

			mainMenuActiveScreens.Add(mainMenuScreen);

			gm.GetComponent<MessageScript>().ShowSingleButtonPopUp(messageToShow, callBackMethod, okButtonText);
		}
	}


	public void ShowMessage(string messageToShow, Action yesButtonCallBack, Action noButtonCallBack, string yesButtonText = "Yes", string noButtonText = "No")
	{
		if (!IsScreenActive(MainMenuScreens.Message))
		{
			MainMenuActiveScreen mainMenuScreen = new MainMenuActiveScreen();
			mainMenuScreen.screenName = MainMenuScreens.Message;
			mainMenuScreen.screenLayer = GetScreenLayer(MainMenuScreens.Message);

			GameObject gm = Instantiate(screens[(int)MainMenuScreens.Message], screenLayers[(int)mainMenuScreen.screenLayer]) as GameObject;
			mainMenuScreen.screenObject = gm;

			mainMenuActiveScreens.Add(mainMenuScreen);
			gm.GetComponent<MessageScript>().ShowDoubleButtonPopUp(messageToShow, yesButtonCallBack, noButtonCallBack, yesButtonText, noButtonText);
		}
	}



	public void DestroyScreen(MainMenuScreens screenName)
	{
		for (int i = 0; i < mainMenuActiveScreens.Count; i++)
		{
			if (mainMenuActiveScreens[i].screenName == screenName)
			{
				Destroy(mainMenuActiveScreens[i].screenObject);
				mainMenuActiveScreens.RemoveAt(i);
			}
		}
	}

	public void DestroyScreen(ScreenLayer layerName)
	{
		for (int i = 0; i < mainMenuActiveScreens.Count; i++)
		{
			if (mainMenuActiveScreens[i].screenLayer == layerName)
			{
				Destroy(mainMenuActiveScreens[i].screenObject);
				mainMenuActiveScreens.RemoveAt(i);
			}
		}
	}

	private bool IsScreenActive(MainMenuScreens screenName)
	{
		for (int i = 0; i < mainMenuActiveScreens.Count; i++)
		{
			if (mainMenuActiveScreens[i].screenName == screenName)
			{
				return true;
			}
		}

		return false;
	}


	private ScreenLayer GetScreenLayer(MainMenuScreens screenName)
	{
		switch (screenName)
		{
			case MainMenuScreens.MainMenu:
			case MainMenuScreens.Shop:
			case MainMenuScreens.Profile:
			case MainMenuScreens.Forum:
			case MainMenuScreens.Message:
			case MainMenuScreens.SelectFrom:
			case MainMenuScreens.FairGaming:
			case MainMenuScreens.Compliance:
			case MainMenuScreens.Contact:
			case MainMenuScreens.FriendList:
			case MainMenuScreens.Language:
			case MainMenuScreens.LinkYourEmail:
			case MainMenuScreens.LinkingSucessfull:
			case MainMenuScreens.UnlinkYourEmail:
			case MainMenuScreens.ChangePassword:
			case MainMenuScreens.RedeemCode:
			case MainMenuScreens.InGameShop:
			case MainMenuScreens.Missions:
			case MainMenuScreens.ConsecutiveLoginReward:
			case MainMenuScreens.Congratulation:
			case MainMenuScreens.BackPack:
			case MainMenuScreens.CareerMenuScreen:
			case MainMenuScreens.CareerDataScreen:
			case MainMenuScreens.CareerDefinationScreen:
			case MainMenuScreens.ChangeFrame:
			case MainMenuScreens.SelectRegion:
			case MainMenuScreens.ChangeProfileIcon:
			case MainMenuScreens.Loading:
				return ScreenLayer.LAYER1;

			default:
				return ScreenLayer.LAYER1;


			//case MainMenuScreens.MainMenu:
			//case MainMenuScreens.Shop:
			//case MainMenuScreens.Profile:
			//case MainMenuScreens.Forum:

			//	return ScreenLayer.LAYER1;

			//case MainMenuScreens.Message:
			//case MainMenuScreens.SelectFrom:
			//case MainMenuScreens.FairGaming:
			//case MainMenuScreens.Compliance:
			//case MainMenuScreens.Contact:
			//case MainMenuScreens.FriendList:
			//case MainMenuScreens.Language:
			//case MainMenuScreens.LinkYourEmail:
			//case MainMenuScreens.LinkingSucessfull:
			//case MainMenuScreens.UnlinkYourEmail:
			//case MainMenuScreens.ChangePassword:
			//case MainMenuScreens.RedeemCode:
			//case MainMenuScreens.InGameShop:
			//case MainMenuScreens.Missions:
			//case MainMenuScreens.ConsecutiveLoginReward:
			//case MainMenuScreens.Congratulation:
			//case MainMenuScreens.BackPack:
			//case MainMenuScreens.CareerMenuScreen:
			//case MainMenuScreens.CareerDataScreen:
			//case MainMenuScreens.CareerDefinationScreen:

			//	return ScreenLayer.LAYER3;

			///*case MainMenuScreens.Loading:*/
			//case MainMenuScreens.ChangeFrame:
			//case MainMenuScreens.SelectRegion:
			//case MainMenuScreens.ChangeProfileIcon:

			//	return ScreenLayer.LAYER4;

			//case MainMenuScreens.Loading:
			//	return ScreenLayer.LAYER5;

			//default:
			//	return ScreenLayer.LAYER2;
		}
	}


	public void OnServerResponseFound(RequestType requestType, string serverResponse, bool isShowErrorMessage, string errorMessage)
	{
		DestroyScreen(MainMenuScreens.Loading);

		if (errorMessage.Length > 0)
		{
			if (isShowErrorMessage)
			{
				if (requestType == RequestType.Login)
				{
					ShowMessage(errorMessage, () =>
					{
						FetchUserData();
					});
				}
				else
				{
					ShowMessage(errorMessage);
				}
			}

			return;
		}


		if (requestType == RequestType.Login)
		{
			JsonData data = JsonMapper.ToObject(serverResponse);

			if (data["success"].ToString() == "1")
			{
				PlayerGameDetails playerData = Utility.ParsePlayerGameData(data);
				playerData.password = PlayerManager.instance.GetPlayerGameData().password;
				playerData.userName = PlayerManager.instance.GetPlayerGameData().userName;
				PlayerManager.instance.SetPlayerGameData(playerData);

				//Activate bottom panel
				if (!bottomPanel.activeSelf)
					bottomPanel.SetActive(true);

				ShowScreen(MainMenuScreens.MainMenu);
				/*ShowMessage(data["message"].ToString());*/
			}
			else
			{
				ShowMessage(data["message"].ToString());

				//Deactivate bottom panel
				if (!bottomPanel.activeSelf)
					bottomPanel.SetActive(false);

				ShowScreen(MainMenuScreens.Registration);
			}
		}
		else if (requestType == RequestType.GetNotificationMessage)
		{
			JsonData data = JsonMapper.ToObject(serverResponse);

			if (data["success"].ToString() == "1")
			{
				PraseNotificationMessage(data);
			}
			else
			{
				ShowMessage(data["message"].ToString());
			}
		}
		else if (requestType == RequestType.userLoginLogs)
		{
			JsonData data = JsonMapper.ToObject(serverResponse);

			Debug.Log("USER LOGIN LOGS____________  ");
			if (data["success"].ToString() == "1")
			{
				//PraseNotificationMessage(data);
			}
			else
			{
				ShowMessage(data["message"].ToString());
			}
		}
	}
}


public class MainMenuActiveScreen
{
	public GameObject screenObject;
	public MainMenuScreens screenName;
	public ScreenLayer screenLayer;
}

public enum MainMenuScreens
{
	Registration,
	MainMenu,
	ClubDetails,
	Message,
	Loading,
	ClubList,
	Profile,
	Lobby,
	GlobalTournament,
	Shop,
	VIP_Privilege,
	Notification,
	Missions,
	Forum,
	ProfileModification,
	ChangeFrame,
	SelectRegion,
	SelectFrom,
	ChangeProfileIcon,
	ProfileSetting,
	AboutUs,
	FairGaming,
	Compliance,
	Contact,
	Language,
	TopPlayer,
	FriendList,
	LinkYourEmail,
	LinkingSucessfull,
	UnlinkYourEmail,
	ChangePassword,
	RedeemCode,
	InGameShop,
	ConsecutiveLoginReward,
	Congratulation,
	BackPack,
	Career,
	CareerMenuScreen,
	CareerDataScreen,
	CareerDefinationScreen,
	HandScreen,
}


public enum ScreenLayer
{
	LAYER1,
	LAYER2,
	LAYER3,
	LAYER4,
	LAYER5
}
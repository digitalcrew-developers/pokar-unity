using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using TMPro;

public class MenuHandller : MonoBehaviour
{
	public static MenuHandller instance;

	public Text coinsText, diamondsText, pointsText,notificationText;
	public GameObject createClubPopUp,joinClubPopUp,notificationIcon;

    public TMPro.TMP_InputField TMP_CreateClubField, TMP_JoinClubField, TMP_AgentField;

    //temp. club item until we call club list api
    [SerializeField]
    private Button tempClubItem;

    private void Awake()
    {
		instance = this;

		/*Debug.Log("Current Date and time:" + System.DateTime.Now.ToString("dd/MM/yyyy HH:mm"));*/		
	}

    private void OnDestroy()
    {
		instance = null;
    }

    void Start()
	{
		createClubPopUp.SetActive(false);
		joinClubPopUp.SetActive(false);

		if (!MainMenuController.instance.bottomPanel.activeSelf/* && GameConstants.poker*/)
		{
			MainMenuController.instance.bottomPanel.SetActive(true);
			//MainMenuController.instance.bottomPanelTeen.SetActive(false);
		}
		//else if (!MainMenuController.instance.bottomPanelTeen.activeSelf && !GameConstants.poker)
		//{
		//	MainMenuController.instance.bottomPanel.SetActive(false);
		//	MainMenuController.instance.bottomPanelTeen.SetActive(true);
		//}

		UpdateAllText();
		UpdateNotificationData(MainMenuController.instance.GetNotificationDetails().unreadMessageCount);
    }

	public void UpdateNotificationData(int unreadMessageCount)
	{
		if (unreadMessageCount > 0)
		{
			notificationIcon.SetActive(true);
			notificationText.text = "" + unreadMessageCount;
		}
		else
		{
			notificationIcon.SetActive(false);
		}
	}


	public void OnMissoinBtnClick()
	{
		MainMenuController.instance.ShowScreen(MainMenuScreens.Missions);
	}

	public void UpdateAllText()
	{
		PlayerGameDetails playerData = PlayerManager.instance.GetPlayerGameData();
		coinsText.text = Utility.GetTrimmedAmount(""+playerData.coins);
		diamondsText.text = Utility.GetTrimmedAmount("" + playerData.diamonds);
		pointsText.text = Utility.GetTrimmedAmount("" + playerData.points);
	}

	public void OnClickOnButton(string eventName)
	{
		SoundManager.instance.PlaySound(SoundType.Click);

		switch (eventName)
		{
		case "back":
			{
				OnClickOnBack();
			}
			break;


		case "openCreateClub":
			{
				createClubPopUp.transform.Find("BG1/BG2/ClubName").GetComponent<TMP_InputField>().text = "";
				createClubPopUp.SetActive(true);
				joinClubPopUp.SetActive(false);
			}
			break;

		case "openJoinClub":
			{
				joinClubPopUp.transform.Find("BG1/BG2/ClubIDTMP").GetComponent<TMP_InputField>().text = "";
				joinClubPopUp.transform.Find("BG1/BG2/ReferralIDTMP").GetComponent<TMP_InputField>().text = "";
				createClubPopUp.SetActive(false);
				joinClubPopUp.SetActive(true);
			}
			break;


		case "openClubList":
			{
				MainMenuController.instance.ShowScreen(MainMenuScreens.ClubList);
			}
			break;


		case "submit":
			{
				if (createClubPopUp.activeInHierarchy)
				{
					string error = "";

					if (!Utility.IsValidClubName(TMP_CreateClubField.text, out error))
					{
						MainMenuController.instance.ShowMessage(error);
						return;
					}

					string requestData = "{\"userId\":\"" + PlayerManager.instance.GetPlayerGameData().userId + "\"," +
						"\"clubName\":\"" + TMP_CreateClubField.text + "\"}";

					MainMenuController.instance.ShowScreen(MainMenuScreens.Loading);
					WebServices.instance.SendRequest(RequestType.CreateClub, requestData, true, OnServerResponseFound);
				}
				else
				{
					if (TMP_JoinClubField.text.Length <= 0)
					{
						MainMenuController.instance.ShowMessage("Please enter clubId");
						return;
					}

					string requestData = "{\"userId\":\"" + PlayerManager.instance.GetPlayerGameData().userId + "\"," +
						"\"uniqueClubId\":\"" + TMP_JoinClubField.text + "\","+
						"\"agentId\":\"" + TMP_AgentField.text + "\"}";

					MainMenuController.instance.ShowScreen(MainMenuScreens.Loading);
					WebServices.instance.SendRequest(RequestType.SendClubJoinRequest, requestData, true, OnServerResponseFound);
				}
			}
			break;


		case "lobby":
			{
					//		Debug.Log("I am here---------");
					//if (GameConstants.poker)
					//{
					//	MainMenuController.instance.ShowScreen(MainMenuScreens.Lobby);
					//}
					//else
					//{
					//	MainMenuController.instance.ShowScreen(MainMenuScreens.LobbyTeenPatti);
					//}

					//DEV_CODE
					MainMenuController.instance.ShowScreen(MainMenuScreens.Lobby);
				}
			break;

		case "spinUp":
			{
					MainMenuController.instance.ShowScreen(MainMenuScreens.GlobalTournament, new object[] { "spinUp" });
				}
			break;

			case "consecutiveLoginReward":
				{
					MainMenuController.instance.ShowScreen(MainMenuScreens.ConsecutiveLoginReward);
				}
				break;

			case "globalTournament":
			{
				MainMenuController.instance.ShowScreen(MainMenuScreens.GlobalTournament);
			}
			break;

		case "coinShop":
			{
                
				MainMenuController.instance.OpenShopPage("item");
			}
			break;

		case "diamondShop":
			{
				MainMenuController.instance.OpenShopPage("diamond");
			}
			break;

		case "vip":
			{
				MainMenuController.instance.ShowScreen(MainMenuScreens.VIP_Privilege);
			}
			break;

		case "notification":
			{
				MainMenuController.instance.ShowScreen(MainMenuScreens.Notification);
			}
			break;

		case "missions":
			{
				MainMenuController.instance.ShowScreen(MainMenuScreens.Missions);
			}
			break;
                
			default:
			#if ERROR_LOG
			Debug.LogError("unhdnled eventName found in menuHandller = "+eventName);
			#endif
			break;
		}
	}


	public void OnClickOnBack()
	{
		if (createClubPopUp.activeInHierarchy)
		{
			createClubPopUp.SetActive(false);
		}
		else
		{
			joinClubPopUp.SetActive(false);
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



		switch (requestType)
		{

		case RequestType.CreateClub:
			{
					Debug.Log("Response => CreateClub: " + serverResponse);
				JsonData data = JsonMapper.ToObject(serverResponse);

				if (data["success"].ToString() == "1")
				{
					createClubPopUp.SetActive(false);
					MainMenuController.instance.ShowMessage("Club created successfully");
					ClubListUiManager.instance.FetchList();
				}
				else
				{
					MainMenuController.instance.ShowMessage(data["message"].ToString());
						if (createClubPopUp.activeInHierarchy)
							createClubPopUp.SetActive(false);
				}
			}
			break;


		case RequestType.SendClubJoinRequest:
			{
				JsonData data = JsonMapper.ToObject(serverResponse);

				if (data["success"].ToString() == "1")
				{
					joinClubPopUp.SetActive(false);
					MainMenuController.instance.ShowMessage("Club join request sent");
				}
				else
				{
					MainMenuController.instance.ShowMessage(data["message"].ToString());
				}
			}
			break;


		default:
			#if ERROR_LOG
			Debug.LogError("Unhandled requestType found in  MenuHandller = "+requestType);
			#endif
			break;
		}
	}
}

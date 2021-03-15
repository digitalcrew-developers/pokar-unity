using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using TMPro;

public class MenuHandllerTeen : MonoBehaviour
{
	public static MenuHandllerTeen instance;
	public Text coinsText, diamondsText, pointsText,notificationText;
	public GameObject createClubPopUp,joinClubPopUp,notificationIcon;

    public /*TMP_InputField*/InputField TMP_CreateClubField, TMP_JoinClubField, TMP_AgentField;

    //temp. club item until we call club list api
    [SerializeField]
    private Button tempClubItem;

    private void Awake()
    {
		instance = this;

		//Debug.LogError("Current Date and time:" + System.DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
	}

    private void OnDestroy()
    {
		instance = null;
    }

    void Start()
	{
		//createClubPopUp.SetActive(false);
		//joinClubPopUp.SetActive(false);

		//if (!MainMenuControllerTeen.instance.bottomPanel.activeSelf && GameConstants.poker)
		//{
		//	MainMenuControllerTeen.instance.bottomPanel.SetActive(true);
		//	MainMenuControllerTeen.instance.bottomPanelTeen.SetActive(false);
		//}
		//else if (!MainMenuControllerTeen.instance.bottomPanelTeen.activeSelf && !GameConstants.poker)
		//{
		//	MainMenuControllerTeen.instance.bottomPanel.SetActive(false);
		//	MainMenuControllerTeen.instance.bottomPanelTeen.SetActive(true);
		//}

		if (!MainMenuControllerTeen.instance.bottomPanel.activeSelf)
		{
			MainMenuControllerTeen.instance.bottomPanel.SetActive(true);
		}


		UpdateAllText();
		//UpdateNotificationData(MainMenuControllerTeen.instance.GetNotificationDetails().unreadMessageCount);
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
		MainMenuControllerTeen.instance.ShowScreen(MainMenuScreensTeen.Missions);
	}

	public void UpdateAllText()
	{
		PlayerGameDetails playerData = PlayerManager.instance.GetPlayerGameData();
		//Debug.LogError("Data =====" + playerData.coins);
		coinsText.text = Utility.GetTrimmedAmount(""+playerData.coins);
		diamondsText.text = Utility.GetTrimmedAmount("" + playerData.diamonds);
		pointsText.text = Utility.GetTrimmedAmount("" + playerData.points);
	}

	public void OnClickOnButton(string eventName)
	{
		SoundManager.instance.PlaySound(SoundType.Click);

		switch (eventName)
		{	
		case "openCreateClub":
			{
				createClubPopUp.transform.Find("BG1/BG2/ClubName").GetComponent</*TMP_InputField*/InputField>().text = "";
				createClubPopUp.SetActive(true);
				joinClubPopUp.SetActive(false);
			}
			break;

		case "openJoinClub":
			{
				joinClubPopUp.transform.Find("BG1/BG2/ClubId").GetComponent</*TMP_InputField*/InputField>().text = "";
				joinClubPopUp.transform.Find("BG1/BG2/ReferralId").GetComponent</*TMP_InputField*/InputField>().text = "";
				createClubPopUp.SetActive(false);
				joinClubPopUp.SetActive(true);
			}
			break;


		case "openClubList":
			{
				MainMenuControllerTeen.instance.ShowScreen(MainMenuScreensTeen.ClubList);
			}
			break;


		case "submit":
			{
				if (createClubPopUp.activeInHierarchy)
				{
					string error = "";

					if (!Utility.IsValidClubName(TMP_CreateClubField.text, out error))
					{
						MainMenuControllerTeen.instance.ShowMessage(error);
						return;
					}

					string requestData = "{\"userId\":\"" + PlayerManager.instance.GetPlayerGameData().userId + "\"," +
						"\"clubName\":\"" + TMP_CreateClubField.text + "\"," +
						"\"clubDescription\":\"" + "Testing..." + "\"}";

					MainMenuControllerTeen.instance.ShowScreen(MainMenuScreensTeen.Loading);
					WebServices.instance.SendRequestTP(RequestTypeTP.CreateClub, requestData, true, OnServerResponseFound);
				}
				else
				{
					if (TMP_JoinClubField.text.Length <= 0)
					{
						MainMenuControllerTeen.instance.ShowMessage("Please enter clubId");
						return;
					}

					string requestData = "{\"uniqueClubId\":\"" + TMP_JoinClubField.text + "\"," +
						"\"userId\":\"" + PlayerManager.instance.GetPlayerGameData().userId + "\","+
						"\"referralCode\":\"" + TMP_AgentField.text + "\"}";

					MainMenuControllerTeen.instance.ShowScreen(MainMenuScreensTeen.Loading);
					WebServices.instance.SendRequestTP(RequestTypeTP.SendClubJoinRequest, requestData, true, OnServerResponseFound);
				}
			}
			break;


		case "lobby":
			{
					//		Debug.Log("I am here---------");
					//if (GameConstants.poker)
					//{
					//	MainMenuControllerTeen.instance.ShowScreen(MainMenuScreensTeen.Lobby);
					//}
					//else
					//{
						MainMenuControllerTeen.instance.ShowScreen(MainMenuScreensTeen.Lobby);
					//}

					//DEV_CODE
					//MainMenuControllerTeen.instance.ShowScreen(MainMenuScreensTeen.Lobby);
				}
			break;

		case "spinUp":
			{
					MainMenuControllerTeen.instance.ShowScreen(MainMenuScreensTeen.GlobalTournament, new object[] { "spinUp" });
				}
			break;

			case "PlayPoker":
				GameConstants.poker = true;
				GlobalGameManager.instance.LoadScene(Scenes.MainMenu);
				break;

			case "consecutiveLoginReward":
				{
					MainMenuControllerTeen.instance.ShowScreen(MainMenuScreensTeen.ConsecutiveLoginReward);
				}
				break;

			case "globalTournament":
			{
				MainMenuControllerTeen.instance.ShowScreen(MainMenuScreensTeen.GlobalTournament);
			}
			break;

		case "coinShop":
			{
                
				MainMenuControllerTeen.instance.OpenShopPage("item");
			}
			break;

		case "diamondShop":
			{
				MainMenuControllerTeen.instance.OpenShopPage("diamond");
			}
			break;

		case "vip":
			{
				MainMenuControllerTeen.instance.ShowScreen(MainMenuScreensTeen.VIP_Privilege);
			}
			break;

		case "notification":
			{
				MainMenuControllerTeen.instance.ShowScreen(MainMenuScreensTeen.Notification);
			}
			break;

		case "missions":
			{
				MainMenuControllerTeen.instance.ShowScreen(MainMenuScreensTeen.Missions);
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


	public void OnServerResponseFound(RequestTypeTP requestType, string serverResponse, bool isShowErrorMessage, string errorMessage)
	{

		MainMenuControllerTeen.instance.DestroyScreen(MainMenuScreensTeen.Loading);

		if (errorMessage.Length > 0)
		{
			if (isShowErrorMessage)
			{
				MainMenuControllerTeen.instance.ShowMessage(errorMessage);
			}

			return;
		}



		switch (requestType)
		{

			case RequestTypeTP.CreateClub:
			{
				Debug.Log("Response => CreateClub TP: " + serverResponse);
				JsonData data = JsonMapper.ToObject(serverResponse);

				if (data["success"].ToString() == "1")
				{
					createClubPopUp.SetActive(false);
					MainMenuControllerTeen.instance.ShowMessage("Club created successfully");
					ClubListUiManagerTeen.instance.FetchList();
				}
				else
				{
					MainMenuControllerTeen.instance.ShowMessage(data["message"].ToString());
					if (createClubPopUp.activeInHierarchy)
						createClubPopUp.SetActive(false);
				}
			}
			break;


			case RequestTypeTP.SendClubJoinRequest:
				{
					Debug.Log("Response => JoinClubRequest TP: " + serverResponse);
                    JsonData data = JsonMapper.ToObject(serverResponse);

                    if (data["success"].ToString() == "1")
                    {
                        joinClubPopUp.SetActive(false);
                        MainMenuControllerTeen.instance.ShowMessage("Club join request sent");
                    }
                    else
                    {
                        MainMenuControllerTeen.instance.ShowMessage(data["message"].ToString());
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

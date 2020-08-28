using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ClubDetailsUIManager : MonoBehaviour
{
	public static ClubDetailsUIManager instance;


	public Text clubNameText, uniqueClubIdText;
	private string clubId = "", uniqueClubId = "";


	private void Awake()
	{
		instance = this;
	}

	public void Initialize(string nameOfClub,string clubUniqueId,string idOfClub)
	{
		clubNameText.text = "Club Name : "+nameOfClub;
		uniqueClubIdText.text = "Club Id : "+clubUniqueId;
		clubId = idOfClub;
		uniqueClubId = clubUniqueId;
	}


	public void OnClickOnButton(string eventName)
	{
		SoundManager.instance.PlaySound(SoundType.Click);

		switch (eventName)
		{
		case "back":
			{
				MainMenuController.instance.ShowScreen(MainMenuScreens.MainMenu);
			}
			break;

		case "members":
			{
				MemberListUIManager.instance.ToggleScreen(true);
			}
			break;

		case "play1":
			{
					if (PlayerManager.instance.GetPlayerGameData().coins < 1000)
					{
						MainMenuController.instance.ShowMessage("You dont have sufficient coins to play, please purchase coins to continue", () => {
							MainMenuController.instance.ShowScreen(MainMenuScreens.Shop);
						}, () => {
						}, "Shop", "Cancel");

						return;
					}


					GlobalGameManager.instance.GetRoomData().isLobbyRoom = false;
                    GlobalGameManager.instance.LoadScene(Scenes.InGame);
			}
			break;

		case "play2":
			{

					if (PlayerManager.instance.GetPlayerGameData().coins < 1000)
					{
						MainMenuController.instance.ShowMessage("You dont have sufficient coins to play, please purchase coins to continue", () => {
							MainMenuController.instance.ShowScreen(MainMenuScreens.Shop);
						}, () => {
						}, "Shop", "Cancel");

						return;
					}


					GlobalGameManager.instance.GetRoomData().isLobbyRoom = false;
					GlobalGameManager.instance.LoadScene(Scenes.InGame);
			}
			break;

		default:
			#if ERROR_LOG
			Debug.LogError("Unhandled eventName found in ClubDetailsUiManager = "+eventName);
			#endif
			break;
		}

	}

	public string GetClubId()
	{
		return clubId;
	}


	public string GetClubUniqueId()
	{
		return uniqueClubId;
	}
}

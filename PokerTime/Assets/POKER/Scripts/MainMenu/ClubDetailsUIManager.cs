using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ClubDetailsUIManager : MonoBehaviour
{
	public static ClubDetailsUIManager instance;    
	public Text clubNameText, uniqueClubIdText;
	private string clubId = "", uniqueClubId = "";
    public Text CLubChips;

	//DEV_CODE
	[Header("Gameobject")]
	public GameObject clubProfile;
	public GameObject editClubProfile;
	public GameObject selectFrom;

	[Header("Images")]
	public Image clubProfileImg;
	public Image editClubProfileImg;

	[Header("Text/InputField")]
	public Text clubName;
	public InputField editProfileClubName;
	public InputField editProfileClubNotice;

	private string layout = "Listed";
    private bool isJackpotOn = false;
	private GameObject bottom;

	private void Awake()
	{
		instance = this;

		//Deactivate Bottom Panel
		if (MainMenuController.instance.bottomPanel.activeSelf)
			MainMenuController.instance.bottomPanel.SetActive(false);

		clubProfile.SetActive(false);
		editClubProfile.SetActive(false);
		selectFrom.SetActive(false);
	}

	public void Initialize(string nameOfClub,string clubUniqueId,string idOfClub)
	{
		clubNameText.text = "Club Name : "+nameOfClub;
		uniqueClubIdText.text = "Club Id : "+clubUniqueId;
		clubId = idOfClub;
		uniqueClubId = clubUniqueId;

		//DEV_CODE
		//Debug.Log("Club name: " + nameOfClub);
		//clubName.text = nameOfClub;
		//editProfileClubName.text = nameOfClub;

		GetChips();
        //to-do... get layout from server for this club and update in local string
    }

    public void GetChips()
    {
        int id = 1;
        string userId = PlayerManager.instance.GetPlayerGameData().userId;
        int userIdInt = 0;

        int.TryParse(userId, out userIdInt);

        string clubID = ClubDetailsUIManager.instance.GetClubId();
        int clubIdInt = 0;

        int.TryParse(clubID, out clubIdInt);

        string request = "{\"userId\":\"" + userIdInt + "\"," +
                        "\"clubId\":\"" + clubIdInt + "\"," +
                        "\"uniqueClubId\":\"" + ClubDetailsUIManager.instance.GetClubUniqueId() + "\"," +
                        "\"clubStatus\":\"" + id + "\"}";

        WebServices.instance.SendRequest(RequestType.GetClubDetails, request, true, OnServerResponseFound);
    }

    public void OnServerResponseFound(RequestType requestType, string serverResponse, bool isShowErrorMessage, string errorMessage)
    {
        Debug.Log(serverResponse);
        //MainMenuController.instance.DestroyScreen(MainMenuScreens.Loading);

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
            case RequestType.GetClubDetails:
                {
                    JsonData data = JsonMapper.ToObject(serverResponse);
                    string chipsText = data["data"][0]["ptChips"].ToString();
                    CLubChips.text = chipsText;
                }
                break;
            default:
#if ERROR_LOG
			Debug.LogError("Unhandled requestType found in  MenuHandller = "+requestType);
#endif
                break;
        }
    }

    public void OnClickOnButton(string eventName)
	{
		SoundManager.instance.PlaySound(SoundType.Click);

		switch (eventName)
		{
			case "back":
			{
				//Activate Bottom Panel
				if (!MainMenuController.instance.bottomPanel.activeSelf)
					MainMenuController.instance.bottomPanel.SetActive(true);

				MainMenuController.instance.ShowScreen(MainMenuScreens.MainMenu);
			}
			break;

			case "close_clubProfile":
			{
				clubProfile.SetActive(false);
			}
			break;

			case "close_editClubProfile":
			{
				editClubProfile.SetActive(false);
				clubProfile.SetActive(true);
			}
			break;

			case "close_selectFrom":
			{
				selectFrom.SetActive(false);
			}
			break;

			case "profile":
			{
				clubProfile.SetActive(true);
			}
			break;

			case "editClubProfile":
			{
				clubProfile.SetActive(false);
				editClubProfile.SetActive(true);
				selectFrom.SetActive(false);
			}
			break;

			case "changeClubProfilePic":
			{
				selectFrom.SetActive(true);
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

	public void OnClickSaveBtn()
	{
		Debug.Log("Clicked on Save Button...");
	}

	public void OnClickAlbumBtn()
	{
		Debug.Log("Clicked on Album Button...");
	}

	public Sprite GetClubImage()
    {
        return null;
    }

    public string GetClubName()
    {
        return clubNameText.text;
    }

	public string GetClubId()
	{
		return clubId;
	}
    
	public string GetClubUniqueId()
	{
		return uniqueClubId;
	}

    public void SetLayout(ClubTableLayout _layout) { layout = _layout.ToString(); }

    public string GetLayout()
    {
        return layout;
    }

    public void SetJackpotStatus(bool val)
    {
        isJackpotOn = val;
    }

    public bool GetJackpotStatus()
    {
        return isJackpotOn;
    }

}

public enum ClubTableLayout
{
    Listed,
    Classic
}

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
    private string layout = "Listed";
    private bool isJackpotOn = false;

	private GameObject bottom;

	private void Awake()
	{
		instance = this;

		//Deactivate Bottom Panel
		if (MainMenuController.instance.bottomPanel.activeSelf)
			MainMenuController.instance.bottomPanel.SetActive(false);
	}

	public void Initialize(string nameOfClub,string clubUniqueId,string idOfClub)
	{
		clubNameText.text = "Club Name : "+nameOfClub;
		uniqueClubIdText.text = "Club Id : "+clubUniqueId;
		clubId = idOfClub;
		uniqueClubId = clubUniqueId;
        GetChips();
        //to-do... get layout from server for this club and update in local string
    }


    private void OnEnable()
    {
        
    }

    public void GetChips()
    {
        GetTemplate();
        AddUpdateTables();
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
            case RequestType.GetClubDetails:
                {
                    JsonData data = JsonMapper.ToObject(serverResponse);
                    string chipsText = data["data"][0]["ptChips"].ToString();
                    CLubChips.text = chipsText;
                }
                break;

            case RequestType.GetTemplates:
                {
                    JsonData data = JsonMapper.ToObject(serverResponse);
                    Debug.LogError("Data is : " + data.ToJson());
                }
                break;

            case RequestType.PublishTemplate:
                {
                    JsonData data = JsonMapper.ToObject(serverResponse);
                    Debug.LogError("Data is : " + data.ToJson());
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


    public void GetTemplate()
    {
        string requestData = "{\"clubId\":\"" + "17" + "\"," +
                             "\"tableId\":\"" + "Me" + "\"," +
                            "\"status\":\"" + "Saved" + "\"," +
                            "\"settingData\":" + "[{" +
                            "\"templateSubType\":\"" + "Regular Mode" + "\"," +
                            "\"memberCount\":\"" + "6" + "\"," +
                            "\"actionTime\":\"" + "15" + "\"," +
                            "\"exclusiveTable\":\"" + "Off" + "\"," +
                            "\"blinds\":\"" + "1/2" + "\"," +
                            "\"buyInMin\":\"" + "10" + "\"," +
                            "\"buyInMax\":\"" + "100" + "\"," +
                            "\"minVPIP\":\"" + "0%" + "\"," +
                            "\"VPIPLevel\":\"" + "0%" + "\"," +
                            "\"handsThreshold\":\"" + "" + "\"," +
                            "\"autoStart\":\"" + "Yes" + "\"," +
                            "\"autoStartWith\":\"" + "1" + "\"," +
                            "\"autoExtension\":\"" + "5" + "\"," +
                            "\"autoExtensionTimes\":\"" + "10" + "\"," +
                            "\"autoOpen\":\"" + "Yes" + "\"," +
                            "\"riskManagement\":\"" + "No" + "\"," +
                            "\"fee\":\"" + "5" + "\"," +
                            "\"cap\":\"" + "10" + "\"," +
                            "\"calltime\":\"" + "2" + "\"," +
                            "\"authorizedBuyIn\":\"" + "On" + "\"," +
                            "\"GPSRestriction\":\"" + "On" + "\"," +
                            "\"IPRestriction\":\"" + "On" + "\"," +
                            "\"banChatting\":\"" + "On" + "\"," +
                            "\"hours\":\"" + "5" +

                           "\"}]}";

        WebServices.instance.SendRequest(RequestType.GetTemplates, requestData, true, OnServerResponseFound);
    }


    public void AddUpdateTables()
    {
        string requestData = "{\"clubId\":\"" + "17" + "\"," +
                            "\"status\":\"" + "Published" + "\"," +
                            "\"tableIds\":" + "[" +
                            "\"1" + "\"," +
                            "\"2\"" +
                           

                           "]}";

        WebServices.instance.SendRequest(RequestType.PublishTemplate, requestData, true, OnServerResponseFound);
    }



}

public enum ClubTableLayout
{
    Listed,
    Classic
}

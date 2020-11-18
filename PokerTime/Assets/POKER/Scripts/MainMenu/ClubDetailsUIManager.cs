using ImageAndVideoPicker;
using LitJson;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


public class ClubDetailsUIManager : MonoBehaviour
{
	public static ClubDetailsUIManager instance;
	public Text clubNameText, uniqueClubIdText;
	private string clubId = "", uniqueClubId = "";
	public Text CLubChips;
	public Image clubProfileImage;
	public TMP_Text jackpotAmountText;
	public string playerTypeForClub = "";

	//DEV_CODE
	[Header("Gameobject")]
	public GameObject clubTablesContainer;
	public GameObject clubProfile;
	public GameObject editClubProfile;
	public GameObject selectFrom;
	public GameObject clubEmail;
	public GameObject clubNotice;

	[Header("Prefabs")]
	public GameObject tableType2;
	public GameObject tableType3;

	[Header("Images")]
	public Image clubProfileImg;
	public Image editClubProfileImg;
	public Image clubNoticeImg;

	[Header("Text/InputField")]
	public Text clubName;
	public InputField editProfileClubName;
	public InputField editProfileClubNotice;

	private string layout = "Listed";
	private bool isJackpotOn = false;
	private GameObject bottom;

	private string path;
	public Text pathText;

	private void Awake()
	{
		instance = this;
	}

	private void OnEnable()
	{
		//Deactivate Bottom Panel
		if (MainMenuController.instance.bottomPanel.activeSelf)
			MainMenuController.instance.bottomPanel.SetActive(false);

		DisableAllScreens();

		PickerEventListener.onImageSelect += OnImageSelect;
		PickerEventListener.onImageLoad += OnImageLoad;
		PickerEventListener.onError += OnError;
		PickerEventListener.onCancel += OnCancel;

#if UNITY_ANDROID
		AndroidPicker.CheckPermissions();
#endif
	}

	public void Initialize(string nameOfClub, string clubUniqueId, string idOfClub, string clubProfileImagePath, string playerType)
	{
		clubNameText.text = "Club Name : " + nameOfClub;
		uniqueClubIdText.text = "Club Id : " + clubUniqueId;
		clubId = idOfClub;
		uniqueClubId = clubUniqueId;
		playerTypeForClub = playerType;

		StartCoroutine(LoadSpriteImageFromUrl(clubProfileImagePath, clubProfileImage));
		FetchJackpotDetails();

		//DEV_CODE
		//Debug.Log("Club name: " + nameOfClub);
		clubName.text = nameOfClub;
		editProfileClubName.text = nameOfClub;

		GetChips();
		GetNotifications();
		GetClubTemplates();
		//to-do... get layout from server for this club and update in local string
	}

	private void FetchJackpotDetails()
	{
		string requestData = "{\"clubId\":\"" + ClubDetailsUIManager.instance.GetClubId() + "\"}";
		WebServices.instance.SendRequest(RequestType.GetJackpotDetailByClubId, requestData, true, (requestType, serverResponse, isShowErrorMessage, errorMessage) =>
		{
			if (errorMessage.Length > 0)
			{
				if (isShowErrorMessage)
				{
					MainMenuController.instance.ShowMessage(errorMessage);
				}
				return;
			}

			Debug.Log("Response => GetJackpotDetails(ClubPanel): " + serverResponse);

			JsonData data = JsonMapper.ToObject(serverResponse);

			if (data["status"].Equals(true))
			{
				int a = data["data"][0]["jackpotAmount"].ToString().Length;

				string str = "";
				for (int i = 0; i < (9 - a); i++)
				{
					if (i == 1)
					{
						str += ",";
						continue;
					}
					else if (i == 5)
					{
						str += ",";
						continue;
					}
					str += "0";
				}

				str += data["data"][0]["jackpotAmount"].ToString();

				jackpotAmountText.text = str;
			}
			else
			{
				Debug.Log("No Jackpot is available...");
			}
		});
	}

	IEnumerator LoadSpriteImageFromUrl(string URL, Image image)
	{
		UnityWebRequest unityWebRequest = UnityWebRequestTexture.GetTexture(URL);
		yield return unityWebRequest.SendWebRequest();

		if (unityWebRequest.isNetworkError || unityWebRequest.isHttpError)
		{
			Debug.LogError("Download failed");
		}
		else
		{
			var Text = DownloadHandlerTexture.GetContent(unityWebRequest);
			Sprite sprite = Sprite.Create(Text, new Rect(0, 0, Text.width, Text.height), Vector2.zero);

			if (image != null)
				image.sprite = sprite;

			Debug.Log("Successfully Set Player Profile");
		}
	}

	private void DisableAllScreens()
	{
		clubProfile.SetActive(false);
		editClubProfile.SetActive(false);
		selectFrom.SetActive(false);
		clubEmail.SetActive(false);
		clubNotice.SetActive(false);
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

	public void GetNotifications()
	{
		string request = "{\"userId\":\"" + PlayerManager.instance.GetPlayerGameData().userId + "\"," +
						   "\"clubId\":\"" + GetClubId() + "\"," +
						   "\"unionId\":\"" + "" + "\"}";

		WebServices.instance.SendRequest(RequestType.GetNotification, request, true, OnServerResponseFound);
	}

	public void GetClubTemplates()
	{
		string requestData = "{\"clubId\":\"" + ClubDetailsUIManager.instance.GetClubId() + "\"," +
								"\"tableId\":\"" + "" + "\"," +
								"\"status\":\"" + "Published" + "\"," +
								"\"settingData\":\"" + "Yes" + "\"}";

		WebServices.instance.SendRequest(RequestType.GetTemplates, requestData, true, OnServerResponseFound);
	}

	public void OnServerResponseFound(RequestType requestType, string serverResponse, bool isShowErrorMessage, string errorMessage)
    {
        Debug.Log("Response GetClubDetails: " + serverResponse);
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

			case RequestType.GetNotification:
				{
					Debug.Log("Response => GetNotification: " + serverResponse);
				}
				break;

			case RequestType.GetTemplates:
				{
					Debug.Log("Response => GetTemplates : " + serverResponse);
					JsonData data = JsonMapper.ToObject(serverResponse);
					LoadTemplates(data);
				}
				break;

            default:
#if ERROR_LOG
			Debug.LogError("Unhandled requestType found in  MenuHandller = "+requestType);
#endif
                break;
        }
    }

	private void LoadTemplates(JsonData data)
	{
		//Debug.Log("Total Templates: " + data["response"].Count);

		for (int i = 1; i < clubTablesContainer.transform.childCount; i++)
		{
			Destroy(clubTablesContainer.transform.GetChild(i).gameObject);
		}

		for (int i = 0; i < data["response"].Count; i++)
		{
			GameObject obj;

			if ((i+1) % 2 == 0)
			{
				obj = Instantiate(tableType2, clubTablesContainer.transform) as GameObject;				
			}
			else
			{
				obj = Instantiate(tableType3, clubTablesContainer.transform) as GameObject;
			}

			if(data["response"][i]["templateName"] != null)
				obj.transform.Find("Image/title").GetComponent<Text>().text = data["response"][i]["templateName"].ToString();

			obj.transform.Find("Image/VPIP").gameObject.SetActive(true);
			//obj.transform.Find("Image/UserImg/user").GetComponent<Text>().text = "";

			//if(data["response"][i]["settingData"].Count > 0)
			//{
			//	if (data["response"][i]["settingData"]["blinds"] != null)
			//		obj.transform.Find("Image/Blinds").GetComponent<Text>().text = "Blinds: " + data["response"][i]["settingData"]["blinds"].ToString();

			//	else if (data["response"][i]["settingData"]["ante"] != null)
			//		obj.transform.Find("Image/Blinds").GetComponent<Text>().text = "Ante: " + data["response"][i]["settingData"]["ante"].ToString();
			//}
			
			obj.transform.Find("Image/time").GetComponent<Text>().text = data["response"][i]["created"].ToString().Substring(11,8);
			obj.transform.Find("Image/status/tabletype").GetComponent<Text>().text = data["response"][i]["gameType"].ToString();
			//obj.transform.Find("Image/PlayersWaiting/Text").GetComponent<Text>().text = "";
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

                    MainMenuController.instance.SwitchToMainMenu(true);
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
                        MainMenuController.instance.SwitchToMainMenu(true,0);
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
                        MainMenuController.instance.SwitchToMainMenu(true, 0);
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
		UploadProfileImage();
	}

	public void UploadProfileImage()
	{
		StartCoroutine(UploadImage());
	}

	private IEnumerator UploadImage()
	{
		Texture2D newTexture = new Texture2D(editClubProfileImg.mainTexture.width, editClubProfileImg.mainTexture.height);
		newTexture.LoadRawTextureData(newTexture.GetRawTextureData());
		newTexture.Apply();

		byte[] bytes = newTexture.EncodeToJPG();
		Destroy(newTexture);

		var form = new WWWForm();
		form.AddField("uniqueClubId", GetClubUniqueId());
		form.AddField("clubName", GetClubName());
		form.AddField("clubStatus", "1");
		form.AddField("jackpotToggle", GetJackpotStatus().ToString());
		form.AddField("layout", GetLayout());
		form.AddBinaryData("clubImage", bytes, path, "image/jpg");

		UnityWebRequest www = UnityWebRequest.Post("http://3.17.201.78:3000/updateClub", form);

		pathText.text = "Uploading!!!";
		//Debug.Log("Uploading !!!!!!");
		yield return www.SendWebRequest();

		pathText.text = "Upload Success....";
		//Debug.Log("Upload Success...");

		if (www.isNetworkError || www.isHttpError)
		{
			pathText.text = www.error.ToString();
			Debug.Log(www.error);
		}
		else
		{
			pathText.text = www.downloadHandler.text;
			Debug.Log("Form upload complete! and Response: " + www.downloadHandler.text);
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


	public void OpenGallery()
	{
#if UNITY_ANDROID
		AndroidPicker.BrowseImage(false);
#endif
#if UNITY_EDITOR
        string path = EditorUtility.OpenFilePanel("Ovrewrite with jpg", "", "");
        if (path != null)
        {
            WWW www = new WWW("" + path);
            Sprite sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), Vector2.zero);
            editClubProfileImg.sprite = sprite;
            
            /*ProfileImage.texture = www.texture;*/
        }
#endif
    }

	#region Image Picking Methods
	void OnDisable()
	{
		PickerEventListener.onImageSelect -= OnImageSelect;
		PickerEventListener.onImageLoad -= OnImageLoad;
		PickerEventListener.onError -= OnError;
		PickerEventListener.onCancel -= OnCancel;
	}

	void OnImageSelect(string imgPath, ImageAndVideoPicker.ImageOrientation imgOrientation)
	{
		//Debug.Log("Image Location : " + imgPath);        
	}

	void OnImageLoad(string imgPath, Texture2D tex, ImageAndVideoPicker.ImageOrientation imgOrientation)
	{
		//Debug.Log("Image Location : " + imgPath);

		//ProfileModification.instance.profileImagePath = imgPath;
		//ProfileModification.instance.pathText.text = imgPath;
		pathText.text = imgPath;
		path = imgPath;

		Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
		editClubProfileImg.sprite = sprite;

		//OnCloseSelectFrom();
		//UploadProfileImage();
		selectFrom.SetActive(false);
	}

	void OnError(string errorMsg)
	{
		Debug.Log("Error : " + errorMsg);
	}

	void OnCancel()
	{
		Debug.Log("Cancel by user");
	}
	#endregion

}

public enum ClubTableLayout
{
    Listed,
    Classic
}

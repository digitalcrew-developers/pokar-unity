using ImageAndVideoPicker;
using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
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
	public GameObject clubEmail;
	public GameObject clubNotice;
	

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

		PickerEventListener.onImageSelect += OnImageSelect;
		PickerEventListener.onImageLoad += OnImageLoad;
		PickerEventListener.onError += OnError;
		PickerEventListener.onCancel += OnCancel;

#if UNITY_ANDROID
		AndroidPicker.CheckPermissions();
#endif
	}

	public void Initialize(string nameOfClub,string clubUniqueId,string idOfClub)
	{
		clubNameText.text = "Club Name : "+nameOfClub;
		uniqueClubIdText.text = "Club Id : "+clubUniqueId;
		clubId = idOfClub;
		uniqueClubId = clubUniqueId;

		//DEV_CODE
		//Debug.Log("Club name: " + nameOfClub);
		clubName.text = nameOfClub;
		editProfileClubName.text = nameOfClub;

		GetChips();
        //to-do... get layout from server for this club and update in local string
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

				MainMenuController.instance._ShowScreen(MainMenuScreens.MainMenu);
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
		//form.AddField("jackpotToggle", GetJackpotStatus().ToString());
		//form.AddField("layout", GetLayout());
		form.AddBinaryData("clubImage", bytes, path, "image/jpg");

		UnityWebRequest www = UnityWebRequest.Post("http://3.17.201.78:3000/updateClub", form);

		pathText.text = "Uploading!!!";
		Debug.Log("Uploading !!!!!!");
		yield return www.SendWebRequest();

		pathText.text = "Upload Success....";
		Debug.Log("Upload Success...");

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
#elif UNITY_EDITOR
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

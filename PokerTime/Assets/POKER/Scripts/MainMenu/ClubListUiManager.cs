using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ClubListUiManager : MonoBehaviour
{
	public static ClubListUiManager instance;

	public LayoutManager layoutManager;
	public GameObject clubListPrefab;
	public Transform container;

    public GameObject MiddleButtons;
    public GameObject TopButtonJoin, TopButtonCreate;

	private Sprite[] clubProfiles;

	void Awake()
	{
		instance = this;
	}

	void Start()
    {
        if (PlayerManager.instance.IsLogedIn())
        {
            FetchList();			
		}
	}

	public void FetchList(bool isShowLoading = true)
	{
        Debug.LogError("user id is :" + PlayerManager.instance.GetPlayerGameData().userId);
		string requestData = "{\"userId\":\"" + PlayerManager.instance.GetPlayerGameData().userId + "\"}";

		if (isShowLoading)
		{
			//MainMenuController.instance.ShowScreen(MainMenuScreens.Loading);
		}
		
		WebServices.instance.SendRequest(RequestType.GetClubList, requestData, true, OnServerResponseFound);
	}	

	public void OnClickOnBack()
	{
		SoundManager.instance.PlaySound(SoundType.Click);

		MainMenuController.instance.ShowScreen(MainMenuScreens.MainMenu);
	}

    public Text DebugText;

    public void CleaClubList()
    {
        if (container.childCount > 0)
        {
            for (int i = 0; i < container.childCount; i++)
            {
                Destroy(container.GetChild(i).gameObject);
            }
        }
    }

	private void ShowClubList(JsonData data)
	{
        CleaClubList();

        for (int i = 0; i < data["data"].Count; i++)
		{
			if (data["data"][i]["assignRole"].ToString() != "Not Assign")
			{
				GameObject gm = Instantiate(clubListPrefab, container) as GameObject;

				string uniqueClubId = data["data"][i]["uniqueClubId"].ToString();
				string clubName = data["data"][i]["clubName"].ToString();
				string clubId = data["data"][i]["clubId"].ToString();
				string clubProfileImagePath = data["data"][i]["clubImage"].ToString();
				string playerType = data["data"][i]["playerType"].ToString();
				string playerRole = data["data"][i]["assignRole"].ToString();

				//Load Club Profile Image
				StartCoroutine(LoadSpriteImageFromUrl(clubProfileImagePath, gm.transform.Find("PhotoBg/Photo").GetComponent<Image>()));
				gm.transform.Find("ClubName").GetComponent<Text>().text = clubName;

				Transform stars = gm.transform.Find("Star");

				int activeStarCount = UnityEngine.Random.Range(2,stars.childCount);

                for (int k = 0; k < stars.childCount; k++)
                {
					if (k < activeStarCount)
					{
						stars.GetChild(k).gameObject.SetActive(true);
					}
					else
					{
						stars.GetChild(k).gameObject.SetActive(false);
					}
                }

				//gm.transform.Find("ClubId").GetComponent<Text>().text = "ClubId : " + uniqueClubId;
				gm.GetComponent<Button>().onClick.AddListener(() => OnClickOnClub(clubName, uniqueClubId, clubId, clubProfileImagePath, playerType, playerRole));
			}			
		}

		//layoutManager.UpdateLayout();
	}

	private void OnClickOnClub(string clubName,string uniqueClubId,string clubId, string clubProfileImagePath, string playerType, string playerRole)
	{
		SoundManager.instance.PlaySound(SoundType.Click);

		object[] parameters = new object[6];
		parameters[0] = clubName;
		parameters[1] = uniqueClubId;
		parameters[2] = clubId;
		parameters[3] = clubProfileImagePath;
		parameters[4] = playerType;
		parameters[5] = playerRole;
		
		MainMenuController.instance.ShowScreen(MainMenuScreens.ClubDetails, parameters);
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

		if (requestType == RequestType.GetClubList)
		{
			Debug.Log("Response => GetClubList: " + serverResponse);

			JsonData data = JsonMapper.ToObject(serverResponse);

			if (data["success"].ToString() == "1")
            {
				MiddleButtons.SetActive(false);
                TopButtonJoin.SetActive(true);
                TopButtonCreate.SetActive(true);
				ShowClubList(data);
			}
			else
			{
                MiddleButtons.SetActive(true);
                TopButtonJoin.SetActive(false);
                TopButtonCreate.SetActive(false);
                //MainMenuController.instance.ShowMessage(data["message"].ToString());
            }
		}
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

			//Debug.Log("Successfully Set Player Profile");
		}
	}
}
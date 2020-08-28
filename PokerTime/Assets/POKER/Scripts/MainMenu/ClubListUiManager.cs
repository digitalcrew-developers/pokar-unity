using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;
using UnityEngine.UI;

public class ClubListUiManager : MonoBehaviour
{
	public static ClubListUiManager instance;

	public LayoutManager layoutManager;
	public GameObject clubListPrefab;
	public Transform container;

	void Awake()
	{
		instance = this;
	}

	void Start()
	{
		FetchList();
	}

	public void FetchList(bool isShowLoading = true)
	{
		string requestData = "{\"userId\":\"" + PlayerManager.instance.GetPlayerGameData().userId + "\"}";

		if (isShowLoading)
		{
			MainMenuController.instance.ShowScreen(MainMenuScreens.Loading);
		}
		
		WebServices.instance.SendRequest(RequestType.GetClubList, requestData, true, OnServerResponseFound);
	}

	public void OnClickOnBack()
	{
		SoundManager.instance.PlaySound(SoundType.Click);

		MainMenuController.instance.ShowScreen(MainMenuScreens.MainMenu);
	}

	private void ShowClubList(JsonData data)
	{
		for (int i = 0; i < container.childCount; i++)
		{
			Destroy(container.GetChild(i).gameObject);
		}


		for (int i = 0; i < data["data"].Count; i++)
		{
			if (data["data"][i]["assignRole"].ToString() != "Not Assign")
			{
				GameObject gm = Instantiate(clubListPrefab, container) as GameObject;

				string uniqueClubId = data["data"][i]["uniqueClubId"].ToString();
				string clubName = data["data"][i]["clubName"].ToString();
				string clubId = data["data"][i]["clubId"].ToString();


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
				gm.GetComponent<Button>().onClick.AddListener(() => OnClickOnClub(clubName, uniqueClubId, clubId));
			}			
		}

		layoutManager.UpdateLayout();
	}

	private void OnClickOnClub(string clubName,string uniqueClubId,string clubId)
	{
		SoundManager.instance.PlaySound(SoundType.Click);

		object[] parameters = new object[3];
		parameters[0] = clubName;
		parameters[1] = uniqueClubId;
		parameters[2] = clubId;


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
			JsonData data = JsonMapper.ToObject(serverResponse);

			if (data["success"].ToString() == "1")
			{
				ShowClubList(data);
			}
			else
			{
				//MainMenuController.instance.ShowMessage(data["message"].ToString());
			}
		}
	}



}

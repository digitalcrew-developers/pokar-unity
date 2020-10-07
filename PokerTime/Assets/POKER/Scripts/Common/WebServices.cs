using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebServices : MonoBehaviour
{
	public static WebServices instance;

	void Awake()
	{
		//PlayerPrefs.DeleteAll();
		
		instance = this;
	}

	private void SendRequestToServer(APIRequests request)
	{
		if (request.retryCount >= GameConstants.API_RETRY_LIMIT)
		{
			Debug.Log("API retry count exceeded api code = " + request.requestCode);

			if (request.callbackMethod != null)
			{
				request.callbackMethod(request.requestCode, "", request.isShowErrorMessage, "Retry count exceeded.");
			}
			else
			{
				Debug.LogError("Call back method is null ");
			}
			return;
		}

		++request.retryCount;
		request.requestMethod = WaitForServerResponse(request);
		StartCoroutine(request.requestMethod);
	}


	public void SendRequest(RequestType requestCode, string requestData, bool isShowErrorMessage, System.Action<RequestType, string, bool, string> callbackMethod = null, WWWForm formData = null, bool isGetMethod = false)
	{
		APIRequests request = new APIRequests();

		if (formData != null)
		{
			request.pdata = formData.data;
		}
		else
		{
			if (!string.IsNullOrEmpty(requestData))
			{
				request.pdata = System.Text.Encoding.ASCII.GetBytes(requestData.ToCharArray());
			}
			else
			{
				request.pdata = null;
			}
		}

		request.url = GameConstants.GAME_URLS[(int)requestCode];
		request.requestCode = requestCode;
		request.callbackMethod = callbackMethod;
		request.isShowErrorMessage = isShowErrorMessage;

		request.retryCount = 0;

#if DEBUG_LOG
		//Debug.Log("Calling API sending data requestCode = " + request.requestCode + "  data = " + requestData);
		Debug.Log("Request API  URL ===> " + request.url+" requestCode ===> " + request.requestCode + "  data ===> " + requestData);
#endif

		SendRequestToServer(request);
	}


	private IEnumerator WaitForServerResponse(APIRequests request)
	{
		if (!IsInternetAvailable())
		{
			if (request.isShowErrorMessage)
			{
				request.callbackMethod(request.requestCode, "", request.isShowErrorMessage, "Internet Connection not available.");
			}

			yield break;
		}

		request.timerMethod = WaitForAPIResponse(request);
		Dictionary<string, string> headers = new Dictionary<string, string>();
		headers.Add("Content-Type", "application/json");
		//Debug.LogError(request.url);
		WWW www = new WWW(request.url, request.pdata, headers);

		StartCoroutine(request.timerMethod);

		yield return www;

		StopCoroutine(request.timerMethod);

#if DEBUG_LOG
		Debug.Log("Response requestCode ---> " + request.requestCode + "    data ---> " + www.text);
#endif


		if (request.callbackMethod != null)
		{
			string errorMessage = "";

			if (www.error != null)
			{
				errorMessage = www.error;
			}
			else if (string.IsNullOrEmpty(www.text))
			{
				errorMessage = "Server response not found, please check your internet connection is working properly";
			}
			Debug.LogWarning("Request was:" + request.url + " " + request.requestCode +" Response   " + www.text);
			request.callbackMethod(request.requestCode, www.text, request.isShowErrorMessage, errorMessage);
		}

		www.Dispose();
		www = null;
	}

	 
	private IEnumerator WaitForAPIResponse(APIRequests request)
	{
		yield return new WaitForSeconds(GameConstants.API_TIME_OUT_LIMIT);

		StopCoroutine(request.requestMethod);
		SendRequestToServer(request);
	}


	public bool IsInternetAvailable()
	{
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			return false;
		}

		return true;
	}
}


[System.Serializable]
public class APIRequests
{
	public byte[] pdata;
	public string url;
	public bool isShowErrorMessage;
	public RequestType requestCode;
	public IEnumerator requestMethod, timerMethod;

	public int retryCount;
	public System.Action<RequestType, string, bool, string> callbackMethod;
}




[System.Serializable]
public enum RequestMethod
{
	Get,
	Put,
	PostWithFormData
}

[System.Serializable]
public class LoginData
{
	public string userName;
	public string password;
}

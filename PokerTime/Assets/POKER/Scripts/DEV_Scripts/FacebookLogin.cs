using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;
using UnityEngine.UI;
using System;
using System.IO;

public static class SaveSystem
{
	private static readonly string SAVE_FOLDER = Application.dataPath + "/Saves/";
	public static void Init()
	{
		// Test if Save Folder exists
		if (!Directory.Exists(SAVE_FOLDER))
		{
			// Create Save Folder
			Directory.CreateDirectory(SAVE_FOLDER);
		}
	}
}
public class FacebookLogin : MonoBehaviour
{
	public static FacebookLogin instance;
	//public List<string> perms = new List<string>() { "public_profile", "email"};

	//public Button fbLoginBtn;

	//public Button fbLogoutBtn;

	//public Image playerProfilePic;

	//public Text /*player_Profile_Name,*/ STATUStEXT;

	//FirebaseAuth auth;

	private void Awake()
	{
		instance = this;
		//STATUStEXT.text = "Started...";
		//STATUStEXT.text = "Checking Firebase Dependencies..";
		//CheckFirebaseDependencies();
		//SaveSystem.Init();

		if (!FB.IsInitialized)
		{
			FB.Init(SetInit, OnHideUnity);
		}
		else
		{
			FB.ActivateApp();
		}

		//	Debug.LogError(PlayerPrefs.GetInt("fblogin"));
		//if (PlayerPrefs.GetInt("fblogin") == 1)
		//{
		//	//LoadImageData();
		//	fbLogoutBtn.gameObject.SetActive(true);
		//	fbLoginBtn.gameObject.SetActive(false);
		//}
		//else
		//{
		//	fbLogoutBtn.gameObject.SetActive(false);
		//	fbLoginBtn.gameObject.SetActive(true);
		//}

	}
	//public void Start()
	//{
	//STATUStEXT.text = "Checking Firebase Dependencies..";
	//CheckFirebaseDependencies();
	//}
	//public void SavedImageData(Texture2D texture)
	//{
	//	if (!Directory.Exists(Application.dataPath + "/Saves/"))
	//	{
	//		Directory.CreateDirectory(Application.dataPath + "/Saves/");
	//	}
	//	byte[] bytes = texture.EncodeToPNG();
	//	File.WriteAllBytes(Application.dataPath + "/Saves/" + PlayerPrefs.GetInt("FBImage").ToString() + ".png", bytes);
	//	//LoadImageData();
	//}
	//public void LoadImageData()
	//{
	//	player_Profile_Name.text = PlayerPrefs.GetString("first_name").ToString();
	//	playerProfilePic.GetComponent<Image>().sprite = LoadNewSprite(Application.dataPath + "/Saves/"
	//										+ PlayerPrefs.GetInt("FBImage").ToString() + ".png", 100f);
	//}
	//public Sprite LoadNewSprite(string FilePath, float PixelsPerUnit = 100.0f)
	//{
	//	Texture2D SpriteTexture = LoadTexture(FilePath);
	//	Sprite NewSprite = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height), new Vector2(0, 0), PixelsPerUnit);
	//	return NewSprite;
	//}
	//public Texture2D LoadTexture(string FilePath)
	//{
	//	Texture2D Tex2D;
	//	byte[] FileData;
	//	if (File.Exists(FilePath))
	//	{
	//		FileData = File.ReadAllBytes(FilePath);
	//		Tex2D = new Texture2D(2, 2, TextureFormat.RGBA32, false);           // Create new "empty" texture
	//		if (Tex2D.LoadImage(FileData))           // Load the imagedata into the texture (size is set automatically)
	//			return Tex2D;                 // If data = readable -> return texture
	//	}
	//	return null;                     // Return null if load failed
	//}

	private void SetInit()
	{
		//if (FB.IsLoggedIn)
		//{
		//	Debug.Log("FB is logged in");
		//	fbLoginBtn.gameObject.SetActive(false);
		//	fbLogoutBtn.gameObject.SetActive(true);
		//	PlayerPrefs.SetInt("fblogin", 1);
		//	DealWithFBMenus(FB.IsLoggedIn);
		//}

		if (FB.IsInitialized)
		{
			FB.ActivateApp();
		}
		else
		{
			AddToInformation("Failed to Initialize the Facebook SDK");
		}
	}
	private void OnHideUnity(bool isGameShown)
	{
	}

	public void FBlogin()
	{
		//STATUStEXT.text = "Clicked Login..";
		//fbLoginBtn.gameObject.SetActive(value: false);
		AddToInformation("Clicked Login..");
		FB.LogInWithReadPermissions(new List<string>() { "public_profile", "email" },
									AuthCallBack);
	}

	private void AuthCallBack(IResult result)
	{
		//STATUStEXT.text = "Inside AuthCallBack";
		AddToInformation("Inside AuthCallBack");
		if (result.Error != null)
		{
			//STATUStEXT.text = result.Error;
			AddToInformation(result.Error);
			Debug.Log(result.Error);
			return;
		}
		if (FB.IsLoggedIn)
		{
			var aToken = AccessToken.CurrentAccessToken;
			Debug.Log(aToken.TokenString);
			//STATUStEXT.text = "FB is logged in..";
			AddToInformation("FB is logged in..");

			Debug.Log("FB is logged in");
			//PlayerPrefs.SetInt("fblogin", 1);
			//firebasedFbLogin(aToken.TokenString);
		}
		else
		{
			//STATUStEXT.text = "FB is not logged in..";

			Debug.Log("FB is not logged in");
			//PlayerPrefs.SetInt("fblogin", 0);
		}
		//DealWithFBMenus(FB.IsLoggedIn);
	}

	//private void DealWithFBMenus(bool isLoggedIn)
	//{
	//	if (isLoggedIn)
	//	{
	//		PlayerPrefs.SetInt("fblogin", 1);
	//		//fbLoginBtn.gameObject.SetActive(value: false);
	//		//fbLogoutBtn.gameObject.SetActive(value: true);
	//		//FB.API("/me?fields=name", HttpMethod.GET, DisplayUsername);
	//		//FB.API("/me/picture?type=square&height=128&width=128", HttpMethod.GET, DisplayProfilePic);
	//	}
	//	else
	//	{
	//		//fbLoginBtn.gameObject.SetActive(value: true);
	//		//fbLogoutBtn.gameObject.SetActive(value: false);
	//		PlayerPrefs.SetInt("fblogin", 0);
	//	}
	//}

	//private void DisplayUsername(IResult result)
	//{
	//	if (result.Error == null)
	//	{
	//		Debug.LogError(result.ResultDictionary["name"].ToString());
	//		PlayerPrefs.SetString("first_name", result.ResultDictionary["name"].ToString());
	//	}
	//	else
	//	{
	//		Debug.Log(result.Error);
	//	}
	//}

	//private void DisplayProfilePic(IGraphResult result)
	//{
	//	if (result.Texture != null)
	//	{
	//		SavedImageData(result.Texture);
	//	}
	//}
	//private void CheckFirebaseDependencies()
	//{
	//	//STATUStEXT.text = "Inside Firebase Dependencies";
	//	FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
	//	{
	//		//STATUStEXT.text = "Inside Task";
	//		if (task.IsCompleted)
	//		{
	//			//STATUStEXT.text = "Task Completed..";
	//			if (task.Result == DependencyStatus.Available)
	//			{
	//				FirebaseManager.instance.auth = FirebaseAuth.DefaultInstance;
	//				//STATUStEXT.text = "Auth Data => " + FirebaseManager.instance.auth;
	//			}
	//			//else
	//				//STATUStEXT.text = ("Could not resolve all Firebase dependencies: " + task.Result.ToString());
	//		}
	//		else
	//		{
	//			//STATUStEXT.text = ("Dependency check was not completed. Error : " + task.Exception.Message);
	//		}
	//	});
	//}
	public void logout()
	{
		//STATUStEXT.text = "Clicked Logout..";
		//playerProfilePic.GetComponent<Image>().sprite = null;
		FB.LogOut();
		//fbLogoutBtn.gameObject.SetActive(value: false);
		//fbLoginBtn.gameObject.SetActive(value: true);
		//      PlayerPrefs.SetInt("fblogin", 0);
		//PlayerPrefs.SetInt("FBImage", PlayerPrefs.GetInt("FBImage") + 1);
	}

	//void firebasedFbLogin(string atoken)
	//{
	//	//if (MainMenuController.instance != null)
	//	//	MainMenuController.instance.ShowScreen(MainMenuScreens.Loading);

	//	//	auth = FirebaseAuth.DefaultInstance;
	//	AddToInformation("Registering Facebook with Firebase..");
	//       Credential credential = FacebookAuthProvider.GetCredential(atoken);
	//       AddToInformation("Credential Got with ==> " + atoken);
	//       FirebaseManager.instance.auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
	//	{
	//           AddToInformation("Checking For Exception..");
	//           if (task.IsCanceled)
	//		{
	//               AddToInformation("SignInWithCredentialAsync was canceled.");
	//               return;
	//		}
	//		if (task.IsFaulted)
	//		{
	//               AddToInformation("SignInWithCredentialAsync encountered an error: " + task.Exception);
	//               return;
	//		}


	//           AddToInformation("User Sign In Successfull...");

	//           FirebaseUser newUser = task.Result;
	//		Debug.LogFormat("User signed in successfully: {0} ({1})", newUser.DisplayName, newUser.UserId);

	//		string name = newUser.DisplayName;
	//		string email = newUser.Email;
	//		System.Uri photo_url = newUser.PhotoUrl;
	//		// The user's Id, unique to the Firebase project.
	//		// Do NOT use this value to authenticate with your backend server, if you
	//		// have one; use User.TokenAsync() instead.
	//		string uid = newUser.UserId;
	//		//STATUStEXT.text += "name " + name + "   email " + email + " photo url " + photo_url + " uID " + uid + " Token: " + atoken;

	//		RegistrationManager.instance.LoginWithSocialID(email, atoken, "facebook");
	//	});
	//}

	private void AddToInformation(string str)
	{
		//Debug.Log(str);
		//statusTxt.text += str;
		if (RegistrationManager.instance != null)
		{
			RegistrationManager.instance.statusText.text = str;
		}
	}
}






























//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Facebook.Unity;
//using UnityEngine.UI;
//using System;
//using System.IO;
////using Firebase.Auth;
//using Firebase;

//public static class SaveSystem
//{
//	private static readonly string SAVE_FOLDER = Application.dataPath + "/Saves/";
//	public static void Init()
//	{
//		// Test if Save Folder exists
//		if (!Directory.Exists(SAVE_FOLDER))
//		{
//			// Create Save Folder
//			Directory.CreateDirectory(SAVE_FOLDER);
//		}
//	}
//}
//public class FacebookLogin : MonoBehaviour
//{
//	public static FacebookLogin instance;
//	//public List<string> perms = new List<string>() { "public_profile", "email"};

//	//public Button fbLoginBtn;

//	//public Button fbLogoutBtn;

//    //public Image playerProfilePic;

//    //public Text /*player_Profile_Name,*/ STATUStEXT;

//    //FirebaseAuth auth;

//    private void Awake()
//	{
//		instance = this;
//		//STATUStEXT.text = "Started...";
//		//STATUStEXT.text = "Checking Firebase Dependencies..";
//		//CheckFirebaseDependencies();
//		//SaveSystem.Init();

//		if (!FB.IsInitialized)
//		{
//			FB.Init(SetInit, OnHideUnity);
//		}
//		else
//        {
//			FB.ActivateApp();
//        }

//		//	Debug.LogError(PlayerPrefs.GetInt("fblogin"));
//        //if (PlayerPrefs.GetInt("fblogin") == 1)
//        //{
//        //	//LoadImageData();
//        //	fbLogoutBtn.gameObject.SetActive(true);
//        //	fbLoginBtn.gameObject.SetActive(false);
//        //}
//        //else
//        //{
//        //	fbLogoutBtn.gameObject.SetActive(false);
//        //	fbLoginBtn.gameObject.SetActive(true);
//        //}

//    }
//	//public void Start()
//	//{
//		//STATUStEXT.text = "Checking Firebase Dependencies..";
//		//CheckFirebaseDependencies();
//	//}
//	//public void SavedImageData(Texture2D texture)
//	//{
//	//	if (!Directory.Exists(Application.dataPath + "/Saves/"))
//	//	{
//	//		Directory.CreateDirectory(Application.dataPath + "/Saves/");
//	//	}
//	//	byte[] bytes = texture.EncodeToPNG();
//	//	File.WriteAllBytes(Application.dataPath + "/Saves/" + PlayerPrefs.GetInt("FBImage").ToString() + ".png", bytes);
//	//	//LoadImageData();
//	//}
//	//public void LoadImageData()
//	//{
//	//	player_Profile_Name.text = PlayerPrefs.GetString("first_name").ToString();
//	//	playerProfilePic.GetComponent<Image>().sprite = LoadNewSprite(Application.dataPath + "/Saves/"
//	//										+ PlayerPrefs.GetInt("FBImage").ToString() + ".png", 100f);
//	//}
//	//public Sprite LoadNewSprite(string FilePath, float PixelsPerUnit = 100.0f)
//	//{
//	//	Texture2D SpriteTexture = LoadTexture(FilePath);
//	//	Sprite NewSprite = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height), new Vector2(0, 0), PixelsPerUnit);
//	//	return NewSprite;
//	//}
//	//public Texture2D LoadTexture(string FilePath)
//	//{
//	//	Texture2D Tex2D;
//	//	byte[] FileData;
//	//	if (File.Exists(FilePath))
//	//	{
//	//		FileData = File.ReadAllBytes(FilePath);
//	//		Tex2D = new Texture2D(2, 2, TextureFormat.RGBA32, false);           // Create new "empty" texture
//	//		if (Tex2D.LoadImage(FileData))           // Load the imagedata into the texture (size is set automatically)
//	//			return Tex2D;                 // If data = readable -> return texture
//	//	}
//	//	return null;                     // Return null if load failed
//	//}

//	private void SetInit()
//	{
//		//if (FB.IsLoggedIn)
//		//{
//		//	Debug.Log("FB is logged in");
//		//	fbLoginBtn.gameObject.SetActive(false);
//		//	fbLogoutBtn.gameObject.SetActive(true);
//		//	PlayerPrefs.SetInt("fblogin", 1);
//		//	DealWithFBMenus(FB.IsLoggedIn);
//		//}

//		if(FB.IsInitialized)
//        {
//			FB.ActivateApp();
//        }
//		else
//        {
//			AddToInformation("Failed to Initialize the Facebook SDK");
//        }
//	}
//	private void OnHideUnity(bool isGameShown)
//	{
//	}

//	public void FBlogin()
//	{
//		//STATUStEXT.text = "Clicked Login..";
//		//fbLoginBtn.gameObject.SetActive(value: false);
//		AddToInformation("Clicked Login..");
//        FB.LogInWithReadPermissions(new List<string>() { "public_profile", "email" },
//                                    AuthCallBack);
//    }

//	private void AuthCallBack(IResult result)
//	{
//		//STATUStEXT.text = "Inside AuthCallBack";
//		AddToInformation("Inside AuthCallBack");
//		if (result.Error != null)
//		{
//			//STATUStEXT.text = result.Error;
//			AddToInformation(result.Error);
//			Debug.Log(result.Error);
//			return;
//		}
//		if (FB.IsLoggedIn)
//		{
//			var aToken = AccessToken.CurrentAccessToken;
//			Debug.Log(aToken.TokenString);
//			//STATUStEXT.text = "FB is logged in..";
//			AddToInformation("FB is logged in..");

//            Debug.Log("FB is logged in");
//			//PlayerPrefs.SetInt("fblogin", 1);
//			firebasedFbLogin(aToken.TokenString);
//		}
//		else
//		{
//			//STATUStEXT.text = "FB is not logged in..";

//			Debug.Log("FB is not logged in");
//			//PlayerPrefs.SetInt("fblogin", 0);
//		}
//		//DealWithFBMenus(FB.IsLoggedIn);
//	}

//	//private void DealWithFBMenus(bool isLoggedIn)
//	//{
//	//	if (isLoggedIn)
//	//	{
//	//		PlayerPrefs.SetInt("fblogin", 1);
//	//		//fbLoginBtn.gameObject.SetActive(value: false);
//	//		//fbLogoutBtn.gameObject.SetActive(value: true);
//	//		//FB.API("/me?fields=name", HttpMethod.GET, DisplayUsername);
//	//		//FB.API("/me/picture?type=square&height=128&width=128", HttpMethod.GET, DisplayProfilePic);
//	//	}
//	//	else
//	//	{
//	//		//fbLoginBtn.gameObject.SetActive(value: true);
//	//		//fbLogoutBtn.gameObject.SetActive(value: false);
//	//		PlayerPrefs.SetInt("fblogin", 0);
//	//	}
//	//}

//	//private void DisplayUsername(IResult result)
//	//{
//	//	if (result.Error == null)
//	//	{
//	//		Debug.LogError(result.ResultDictionary["name"].ToString());
//	//		PlayerPrefs.SetString("first_name", result.ResultDictionary["name"].ToString());
//	//	}
//	//	else
//	//	{
//	//		Debug.Log(result.Error);
//	//	}
//	//}

//	//private void DisplayProfilePic(IGraphResult result)
//	//{
//	//	if (result.Texture != null)
//	//	{
//	//		SavedImageData(result.Texture);
//	//	}
//	//}
//	private void CheckFirebaseDependencies()
//	{
//		//STATUStEXT.text = "Inside Firebase Dependencies";
//		FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
//		{
//			//STATUStEXT.text = "Inside Task";
//			if (task.IsCompleted)
//			{
//				//STATUStEXT.text = "Task Completed..";
//				if (task.Result == DependencyStatus.Available)
//				{
//					FirebaseManager.instance.auth = FirebaseAuth.DefaultInstance;
//					//STATUStEXT.text = "Auth Data => " + FirebaseManager.instance.auth;
//				}
//				//else
//					//STATUStEXT.text = ("Could not resolve all Firebase dependencies: " + task.Result.ToString());
//			}
//			else
//			{
//				//STATUStEXT.text = ("Dependency check was not completed. Error : " + task.Exception.Message);
//			}
//		});
//	}
//	public void logout()
//	{
//		//STATUStEXT.text = "Clicked Logout..";
//		//playerProfilePic.GetComponent<Image>().sprite = null;
//		FB.LogOut();
//        //fbLogoutBtn.gameObject.SetActive(value: false);
//        //fbLoginBtn.gameObject.SetActive(value: true);
//  //      PlayerPrefs.SetInt("fblogin", 0);
//		//PlayerPrefs.SetInt("FBImage", PlayerPrefs.GetInt("FBImage") + 1);
//	}

//	void firebasedFbLogin(string atoken)
//	{
//		//if (MainMenuController.instance != null)
//		//	MainMenuController.instance.ShowScreen(MainMenuScreens.Loading);

//		//	auth = FirebaseAuth.DefaultInstance;
//		AddToInformation("Registering Facebook with Firebase..");
//        Credential credential = FacebookAuthProvider.GetCredential(atoken);
//        AddToInformation("Credential Got with ==> " + atoken);
//        FirebaseManager.instance.auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
//		{
//            AddToInformation("Checking For Exception..");
//            if (task.IsCanceled)
//			{
//                AddToInformation("SignInWithCredentialAsync was canceled.");
//                return;
//			}
//			if (task.IsFaulted)
//			{
//                AddToInformation("SignInWithCredentialAsync encountered an error: " + task.Exception);
//                return;
//			}


//            AddToInformation("User Sign In Successfull...");

//            FirebaseUser newUser = task.Result;
//			Debug.LogFormat("User signed in successfully: {0} ({1})", newUser.DisplayName, newUser.UserId);

//			string name = newUser.DisplayName;
//			string email = newUser.Email;
//			System.Uri photo_url = newUser.PhotoUrl;
//			// The user's Id, unique to the Firebase project.
//			// Do NOT use this value to authenticate with your backend server, if you
//			// have one; use User.TokenAsync() instead.
//			string uid = newUser.UserId;
//			//STATUStEXT.text += "name " + name + "   email " + email + " photo url " + photo_url + " uID " + uid + " Token: " + atoken;

//			RegistrationManager.instance.LoginWithSocialID(email, atoken, "facebook");
//		});
//	}

//	private void AddToInformation(string str)
//	{
//		//Debug.Log(str);
//		//statusTxt.text += str;
//		if (RegistrationManager.instance != null)
//		{
//			RegistrationManager.instance.statusText.text = str;
//		}
//	}
//}
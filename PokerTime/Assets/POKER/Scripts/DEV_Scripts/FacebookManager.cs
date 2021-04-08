using Facebook.Unity;
using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class FacebookManager : MonoBehaviour
{
    public static FacebookManager instance;
    private List<string> perms = new List<string>() { "public_profile", "email" };

    //public Text debugText;

    private void Awake()
    {
        instance = this;

        if (!FB.IsInitialized)
        {
            FB.Init(SetInit);
        }
        else
        {
            FB.ActivateApp();
        }
    }

    void SetInit()
    {
        if (FB.IsInitialized)
        {
            FB.ActivateApp();
        }
        else
        {
            DisplayMessage("Failed to Initialize the Facebook SDK");
        }
    }

    public void SignInWithFB()
    {
        //DisplayMessage("Clicked On FB Login");
        FB.LogInWithReadPermissions(new List<string>() { "public_profile", "email" },
                                    AuthCallBack);
    }

    public void SignOutFromFB()
    {
        FB.LogOut();
    }

    void AuthCallBack(IResult result)
    {
        DisplayMessage("Inside AuthCallBack");
        if (result.Error != null)
        {
            //DisplayMessage(result.Error);
            Debug.Log(result.Error);
            return;
        }
        if (FB.IsLoggedIn)
        {
            var aToken = AccessToken.CurrentAccessToken;

            DisplayMessage("FB is logged in with Token => " + aToken.TokenString);

            foreach (var args in aToken.Permissions)
            {
                DisplayMessage(args);
            }

            StartCoroutine(RequestUserData(aToken.UserId, aToken.TokenString));

            //FB.API("/me/picture?type=square&height=128&width=128", HttpMethod.GET, DisplayProfilePic);
            //FB.API("me?fields=name", HttpMethod.GET, RequestedData);


            //LoginWithFBOnFirebase(aToken.TokenString);
        }
        else
        {
            //DisplayMessage("FB is not logged in..");
        }
    }

    private void DisplayProfilePic(IGraphResult result)
    {
        if (result.Texture != null)
        {
            //SavedImageData(result.Texture);
            DisplayMessage("Texture Loaded...");
        }
    }

    IEnumerator RequestUserData(string userId, string aToken)
    {
        DisplayMessage("Requesting User Data..");

        UnityWebRequest uwr = UnityWebRequest.Get("https://graph.facebook.com/" + userId + "?fields=id,email&access_token=" + aToken);
        yield return uwr.SendWebRequest();

        if (uwr.result == UnityWebRequest.Result.ConnectionError)
        {
            DisplayMessage("Error While Sending: " + uwr.error);
        }
        else
        {
            JsonData data = JsonMapper.ToObject(uwr.downloadHandler.text);

            //DisplayMessage("Result is: " + data["email"].ToString());

            RegistrationManager.instance.LoginWithSocialID(data["email"].ToString(), aToken, "facebook");
        }
    }



    //void RequestedData(IGraphResult result)
    //{
    //    DisplayMessage("API Called and now checking for errors..");
    //    if (result.Error == null)
    //    {
    //        DisplayMessage(result.ResultDictionary["name"].ToString());
    //    }
    //    else
    //    {
    //        DisplayMessage(result.Error);
    //    }
    //}

    //void LoginWithFBOnFirebase(string atoken)
    //{
    //    if (MainMenuController.instance != null)
    //        MainMenuController.instance.ShowScreen(MainMenuScreens.Loading);

    //    DisplayMessage("Registering Facebook with Firebase..");
    //    Credential credential = FacebookAuthProvider.GetCredential(atoken);
    //    DisplayMessage("Credential Got with ==> " + atoken);
    //    FirebaseManager.instance.auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
    //    {
    //        DisplayMessage("Checking For Exception..");
    //        if (task.IsCanceled)
    //        {
    //            DisplayMessage("SignInWithCredentialAsync was canceled.");
    //            return;
    //        }
    //        if (task.IsFaulted)
    //        {
    //            DisplayMessage("SignInWithCredentialAsync encountered an error: " + task.Exception);
    //            return;
    //        }


    //        DisplayMessage("User Signed In Successfull...");

    //        FirebaseUser newUser = task.Result;
    //        Debug.LogFormat("User signed in successfully: {0} ({1})", newUser.DisplayName, newUser.UserId);

    //        //string name = newUser.DisplayName;
    //        string email = newUser.Email;
    //        //System.Uri photo_url = newUser.PhotoUrl;
    //        // The user's Id, unique to the Firebase project.
    //        // Do NOT use this value to authenticate with your backend server, if you
    //        // have one; use User.TokenAsync() instead.
    //        //string uid = newUser.UserId;
    //        //STATUStEXT.text += "name " + name + "   email " + email + " photo url " + photo_url + " uID " + uid + " Token: " + atoken;

    //        RegistrationManager.instance.LoginWithSocialID(email, atoken, "facebook");
    //    });
    //}

    void DisplayMessage(string msg)
    {
        //debugText.text = msg;

        if (RegistrationManager.instance != null)
        {
            RegistrationManager.instance.statusText.text = msg;
        }
    }
}

































//using Facebook.Unity;
////using Firebase.Auth;
//using LitJson;
//using System.Collections;
//using System.Collections.Generic;
//using System.IO;
//using UnityEngine;
//using UnityEngine.Networking;
//using UnityEngine.UI;

//public class FacebookManager : MonoBehaviour
//{
//    public static FacebookManager instance;
//    private List<string> perms = new List<string>() { "public_profile", "email" };

//    //public Text debugText;

//    private void Awake()
//    {
//        instance = this;

//        if(!FB.IsInitialized)
//        {
//            FB.Init(SetInit);
//        }
//        else
//        {
//            FB.ActivateApp();
//        }
//    }

//    void SetInit()
//    {
//        if(FB.IsInitialized)
//        {
//            FB.ActivateApp();
//        }
//        else
//        {
//            DisplayMessage("Failed to Initialize the Facebook SDK");
//        }
//    }

//    public void SignInWithFB()
//    {
//        //DisplayMessage("Clicked On FB Login");
//        FB.LogInWithReadPermissions(new List<string>() { "public_profile", "email" },
//                                    AuthCallBack);
//    }

//    public void SignOutFromFB()
//    {
//        FB.LogOut();
//    }

//    void AuthCallBack(IResult result)
//    {
//        DisplayMessage("Inside AuthCallBack");
//        if (result.Error != null)
//        {
//            //DisplayMessage(result.Error);
//            Debug.Log(result.Error);
//            return;
//        }
//        if (FB.IsLoggedIn)
//        {
//            var aToken = AccessToken.CurrentAccessToken;

//            DisplayMessage("FB is logged in with Token => " + aToken.TokenString);

//            foreach(var args in aToken.Permissions)
//            {
//                DisplayMessage(args);
//            }

//            StartCoroutine(RequestUserData(aToken.UserId, aToken.TokenString));

//            //FB.API("/me/picture?type=square&height=128&width=128", HttpMethod.GET, DisplayProfilePic);
//            //FB.API("me?fields=name", HttpMethod.GET, RequestedData);


//            //LoginWithFBOnFirebase(aToken.TokenString);
//        }
//        else
//        {
//            //DisplayMessage("FB is not logged in..");
//        }
//    }

//    private void DisplayProfilePic(IGraphResult result)
//    {
//        if (result.Texture != null)
//        {
//            //SavedImageData(result.Texture);
//            DisplayMessage("Texture Loaded...");
//        }
//    }

//    IEnumerator RequestUserData(string userId, string aToken)
//    {
//        DisplayMessage("Requesting User Data..");

//        UnityWebRequest uwr = UnityWebRequest.Get("https://graph.facebook.com/" + userId + "?fields=id,email&access_token=" + aToken);
//        yield return uwr.SendWebRequest();

//        if (uwr.result == UnityWebRequest.Result.ConnectionError)
//        {
//            DisplayMessage("Error While Sending: " + uwr.error);
//        }
//        else
//        {
//            JsonData data = JsonMapper.ToObject(uwr.downloadHandler.text);

//            //DisplayMessage("Result is: " + data["email"].ToString());

//            RegistrationManager.instance.LoginWithSocialID(data["email"].ToString(), aToken, "facebook");
//        }
//    }



//    //void RequestedData(IGraphResult result)
//    //{
//    //    DisplayMessage("API Called and now checking for errors..");
//    //    if (result.Error == null)
//    //    {
//    //        DisplayMessage(result.ResultDictionary["name"].ToString());
//    //    }
//    //    else
//    //    {
//    //        DisplayMessage(result.Error);
//    //    }
//    //}

//    //void LoginWithFBOnFirebase(string atoken)
//    //{
//    //    if (MainMenuController.instance != null)
//    //        MainMenuController.instance.ShowScreen(MainMenuScreens.Loading);

//    //    DisplayMessage("Registering Facebook with Firebase..");
//    //    Credential credential = FacebookAuthProvider.GetCredential(atoken);
//    //    DisplayMessage("Credential Got with ==> " + atoken);
//    //    FirebaseManager.instance.auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
//    //    {
//    //        DisplayMessage("Checking For Exception..");
//    //        if (task.IsCanceled)
//    //        {
//    //            DisplayMessage("SignInWithCredentialAsync was canceled.");
//    //            return;
//    //        }
//    //        if (task.IsFaulted)
//    //        {
//    //            DisplayMessage("SignInWithCredentialAsync encountered an error: " + task.Exception);
//    //            return;
//    //        }


//    //        DisplayMessage("User Signed In Successfull...");

//    //        FirebaseUser newUser = task.Result;
//    //        Debug.LogFormat("User signed in successfully: {0} ({1})", newUser.DisplayName, newUser.UserId);

//    //        //string name = newUser.DisplayName;
//    //        string email = newUser.Email;
//    //        //System.Uri photo_url = newUser.PhotoUrl;
//    //        // The user's Id, unique to the Firebase project.
//    //        // Do NOT use this value to authenticate with your backend server, if you
//    //        // have one; use User.TokenAsync() instead.
//    //        //string uid = newUser.UserId;
//    //        //STATUStEXT.text += "name " + name + "   email " + email + " photo url " + photo_url + " uID " + uid + " Token: " + atoken;

//    //        RegistrationManager.instance.LoginWithSocialID(email, atoken, "facebook");
//    //    });
//    //}

//    void DisplayMessage(string msg)
//    {
//        //debugText.text = msg;

//        if(RegistrationManager.instance != null)
//        {
//            RegistrationManager.instance.statusText.text = msg;
//        }
//    }
//}
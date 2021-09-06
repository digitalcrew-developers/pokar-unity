using Facebook.Unity;
using Firebase;
#if !UNITY_WEBGL
using Firebase.Auth;
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FacebookManager : MonoBehaviour
{
    public static FacebookManager instance;
    //private List<string> perms = new List<string>() { "public_profile", "email" };

    //public Text debugText;

    string userData = string.Empty;

    private void Awake()
    {
        instance = this;

        if(!FB.IsInitialized)
        {
            FB.Init(SetInit);
        }
        else
        {
            FB.ActivateApp();
        }
    }

    private void Start()
    {
        CheckFirebaseDependencies();
    }

    void SetInit()
    {
        if(FB.IsInitialized)
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
        FB.LogInWithReadPermissions(new List<string>() { "public_profile", "email"},
                                    AuthCallBack);
    }

    public void SignOutFromFB()
    {
        FB.LogOut();
    }

    void AuthCallBack(ILoginResult result)
    {
        //DisplayMessage("Inside AuthCallBack");
        if (result.Error != null)
        {
            //DisplayMessage(result.Error);
            Debug.Log(result.Error);
            return;
        }
        if (FB.IsLoggedIn)
        {
            var aToken = AccessToken.CurrentAccessToken;
            Debug.Log(aToken.TokenString);
            DisplayMessage("FB is logged in..");

            //DealWithFBMenus();
            //LoginWithFBOnFirebase(aToken.TokenString);
            LoginviaFirebaseFacebook2(aToken.TokenString);
        }
        else
        {
            //DisplayMessage("FB is not logged in..");
        }
    }
    Firebase.Auth.FirebaseAuth auth;
    private void CheckFirebaseDependencies()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                if (task.Result == DependencyStatus.Available)
                    auth = FirebaseAuth.DefaultInstance;
                else
                    DisplayMessage("Could not resolve all Firebase dependencies: " + task.Result.ToString());
            }
            else
            {
                DisplayMessage("Dependency check was not completed. Error : " + task.Exception.Message);
            }
        });
    }
    private void LoginviaFirebaseFacebook2(string accesstoken)
    {
        DisplayMessage("Registering Facebook with Firebase..");
        //auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        Firebase.Auth.Credential credential =
            Firebase.Auth.FacebookAuthProvider.GetCredential(accesstoken);
        auth.SignInWithCredentialAsync(credential).ContinueWith(task => {
            DisplayMessage("Checking For Exception..");
            if (task.IsCanceled)
            {
                DisplayMessage("SignInWithCredentialAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                DisplayMessage("SignInWithCredentialAsync encountered an error: " + task.Exception);
                return;
            }
            DisplayMessage("User Signed In Successfull...");

            FirebaseUser newUser = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})", newUser.DisplayName, newUser.UserId);

            //string name = newUser.DisplayName;
            string email = newUser.Email;
            //System.Uri photo_url = newUser.PhotoUrl;
            // The user's Id, unique to the Firebase project.
            // Do NOT use this value to authenticate with your backend server, if you
            // have one; use User.TokenAsync() instead.
            //string uid = newUser.UserId;
            //STATUStEXT.text += "name " + name + "   email " + email + " photo url " + photo_url + " uID " + uid + " Token: " + atoken;

            RegistrationManager.instance.LoginWithSocialID(email, accesstoken, "facebook");
      
    });
    }
    void LoginWithFBOnFirebase(string atoken)
    {
        //if (MainMenuController.instance != null)
        //    MainMenuController.instance.ShowScreen(MainMenuScreens.Loading);


        //DealWithFBMenus(FB.IsLoggedIn);




        //DEV_CODE Code with firebase


        DisplayMessage("Registering Facebook with Firebase..");
#if !UNITY_WEBGL
        Credential credential = FacebookAuthProvider.GetCredential(atoken);
        DisplayMessage("Credential Got with ==> " + atoken);
        FirebaseManager.instance.auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
        {
            DisplayMessage("Checking For Exception..");
            if (task.IsCanceled)
            {
                DisplayMessage("SignInWithCredentialAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                DisplayMessage("SignInWithCredentialAsync encountered an error: " + task.Exception);
                return;
            }


            DisplayMessage("User Signed In Successfull...");

            FirebaseUser newUser = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})", newUser.DisplayName, newUser.UserId);

                    //string name = newUser.DisplayName;
                    string email = newUser.Email;
                    //System.Uri photo_url = newUser.PhotoUrl;
                    // The user's Id, unique to the Firebase project.
                    // Do NOT use this value to authenticate with your backend server, if you
                    // have one; use User.TokenAsync() instead.
                    //string uid = newUser.UserId;
                    //STATUStEXT.text += "name " + name + "   email " + email + " photo url " + photo_url + " uID " + uid + " Token: " + atoken;

                    RegistrationManager.instance.LoginWithSocialID(email, atoken, "facebook");
        });
#endif
    }

    private void DealWithFBMenus(/*bool isLoggedIn*/)
    {
        //if (isLoggedIn)
        //{
            //FB.API("/me?fields=id", HttpMethod.GET, DisplayUserId);
            FB.API("/me?fields=name", HttpMethod.GET, DisplayUsername);
            //FB.API("/me/fields=email", HttpMethod.GET, DisplayEmail);
            //FB.API("/me/picture?type=square&height=128&width=128", HttpMethod.GET, DisplayProfilePic);

            //DisplayMessage("User's Data: " + userData);
        //}
        //else
        //{
        //    DisplayMessage("Not logged in...");
        //}
    }

    private void DisplayUserId(IResult result)
    {
        if (result.Error == null)
        {
            DisplayMessage(result.ResultDictionary["id"].ToString());
            userData += result.ResultDictionary["id"].ToString();
        }
        else
        {
            Debug.Log(result.Error);
        }
    }

    private void DisplayUsername(IResult result)
    {
        if (result.Error == null)
        {
            DisplayMessage(result.ResultDictionary["name"].ToString());
            //userData = userData + "" + result.ResultDictionary["name"].ToString();
        }
        else
        {
            Debug.Log(result.Error);
        }
    }

    private void DisplayEmail(IResult result)
    {
        if (result.Error == null)
        {
            DisplayMessage(result.ResultDictionary["email"].ToString());
            userData += userData + "" + result.ResultDictionary["email"].ToString();
        }
        else
        {
            Debug.Log(result.Error);
        }
    }

    void DisplayMessage(string msg)
    {
        //debugText.text = msg;

        if(RegistrationManager.instance != null)
        {
            RegistrationManager.instance.statusText.text = msg;
        }
    }
}
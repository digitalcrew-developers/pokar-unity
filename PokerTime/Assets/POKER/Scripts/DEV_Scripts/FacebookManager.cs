using Facebook.Unity;
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
    private List<string> perms = new List<string>() { "public_profile", "email" };

    //public Text debugText;

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
        FB.LogInWithReadPermissions(new List<string>() { "public_profile", "email" },
                                    AuthCallBack);
    }

    public void SignOutFromFB()
    {
        FB.LogOut();
    }

    void AuthCallBack(IResult result)
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
            //DisplayMessage("FB is logged in..");

            LoginWithFBOnFirebase(aToken.TokenString);
        }
        else
        {
            //DisplayMessage("FB is not logged in..");
        }
    }

    void LoginWithFBOnFirebase(string atoken)
    {
        if (MainMenuController.instance != null)
            MainMenuController.instance.ShowScreen(MainMenuScreens.Loading);

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

    void DisplayMessage(string msg)
    {
        //debugText.text = msg;

        if(RegistrationManager.instance != null)
        {
            RegistrationManager.instance.statusText.text = msg;
        }
    }
}
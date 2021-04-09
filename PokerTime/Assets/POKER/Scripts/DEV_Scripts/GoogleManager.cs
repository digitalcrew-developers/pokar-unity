#if !UNITY_WEBGL
using Firebase;
using Firebase.Auth;
#endif
using Google;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class GoogleManager : MonoBehaviour
{
    public static GoogleManager instance;

    //public Text statusTxt;

    //private string webClientId = "960874965249-13tfg83naj3hrieknkk2ic5s96r5g73p.apps.googleusercontent.com";
    private string webClientId = "814110012075-h3k1nsq4asvjoflddg6kei95lqetiq33.apps.googleusercontent.com";

    public string userName, userId, email;

    //private FirebaseAuth auth;
    private GoogleSignInConfiguration configuration;
   
    private void Awake()
    {
        instance = this;

        configuration = new GoogleSignInConfiguration { WebClientId = webClientId, RequestEmail = true, RequestIdToken = true };
        //CheckFirebaseDependencies();
    }

    //private void CheckFirebaseDependencies()
    //{
    //    FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
    //    {
    //        if (task.IsCompleted)
    //        {
    //            if (task.Result == DependencyStatus.Available)
    //                auth = FirebaseAuth.DefaultInstance;
    //            else
    //                AddToInformation("Could not resolve all Firebase dependencies: " + task.Result.ToString());
    //        }
    //        else
    //        {
    //            AddToInformation("Dependency check was not completed. Error : " + task.Exception.Message);
    //        }
    //    });
    //}

    public void SignInWithGoogle() { OnSignIn(); }
    public void SignOutFromGoogle() { OnSignOut(); }

    private void OnSignIn()
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;
        AddToInformation("Calling SignIn");
        
        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnAuthenticationFinished);
    }

    private void OnSignOut()
    {
        AddToInformation("Calling SignOut");
        GoogleSignIn.DefaultInstance.SignOut();
    }

    public void OnDisconnect()
    {
        AddToInformation("Calling Disconnect");
        GoogleSignIn.DefaultInstance.Disconnect();
    }

    internal void OnAuthenticationFinished(Task<GoogleSignInUser> task)
    {
        AddToInformation("Inside Autentication Finished.");
        if (task.IsFaulted)
        {
            AddToInformation("Faulted Task..");
            using (IEnumerator<Exception> enumerator = task.Exception.InnerExceptions.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    GoogleSignIn.SignInException error = (GoogleSignIn.SignInException)enumerator.Current;
                    AddToInformation("Got Error: " + error.Status + " " + error.Message);
                }
                else
                {
                    AddToInformation("Got Unexpected Exception?!?" + task.Exception);
                }
            }
        }
        else if (task.IsCanceled)
        {
            AddToInformation("Canceled");
        }
        else
        {
            AddToInformation("Welcome: " + task.Result.DisplayName + "!");
            //AddToInformation("Email = " + task.Result.Email);
            AddToInformation("Google ID Token = " + task.Result.IdToken);
            AddToInformation("Email = " + task.Result.Email);

            userName = task.Result.DisplayName;
            userId = task.Result.UserId;
            email = task.Result.Email;

            RegistrationManager.instance.LoginWithSocialID(task.Result.Email, task.Result.IdToken, "google");

            //SignInWithGoogleOnFirebase(task.Result.IdToken);
        }
    }

    //private void SignInWithGoogleOnFirebase(string idToken)
    //{
    //    if (MainMenuController.instance != null)
    //        MainMenuController.instance.ShowScreen(MainMenuScreens.Loading);

    //    AddToInformation("Registering With Firebase..");
    //    Credential credential = GoogleAuthProvider.GetCredential(idToken, null);
    //    AddToInformation("Credential Got With IDToken => " + idToken);
    //    FirebaseManager.instance.auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
    //    {
    //        AddToInformation("Checking For Exception..");
    //        AggregateException ex = task.Exception;
    //        if (ex != null)
    //        {
    //            if (ex.InnerExceptions[0] is FirebaseException inner && (inner.ErrorCode != 0))
    //                AddToInformation("\nError code = " + inner.ErrorCode + " Message = " + inner.Message);
    //        }
    //        else
    //        {
    //            AddToInformation("Sign In Successful.");
    //            RegistrationManager.instance.LoginWithSocialID(task.Result.Email, idToken, "google");
    //        }
    //    });
    //}

    public void OnSignInSilently()
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;
        AddToInformation("Calling SignIn Silently");

        GoogleSignIn.DefaultInstance.SignInSilently().ContinueWith(OnAuthenticationFinished);
    }

    public void OnGamesSignIn()
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = true;
        GoogleSignIn.Configuration.RequestIdToken = false;

        AddToInformation("Calling Games SignIn");

        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnAuthenticationFinished);
    }

    private void AddToInformation(string str) 
    { 
        //Debug.Log(str);
        //statusTxt.text += str;
        if(RegistrationManager.instance != null)
        {
            RegistrationManager.instance.statusText.text = str;
        }
    }
}
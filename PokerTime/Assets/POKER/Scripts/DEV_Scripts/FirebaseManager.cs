//using Firebase;
//using Firebase.Auth;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager instance;

    //Firebase variables
    //[Header("Firebase")]
    //public DependencyStatus dependencyStatus;
    //public FirebaseAuth auth;
    //public FirebaseUser User;

    //void Awake()
    //{
    //    if (instance == null)
    //        instance = this;

    //    //Check that all of the necessary dependencies for Firebase are present on the system
    //    FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
    //    {
    //        if (task.IsCompleted)
    //        {
    //            if (task.Result == DependencyStatus.Available)
    //                auth = FirebaseAuth.DefaultInstance;
    //            else
    //                Debug.Log("Could not resolve all Firebase dependencies: " + task.Result.ToString());
    //        }
    //        else
    //        {
    //            Debug.Log("Dependency check was not completed. Error : " + task.Exception.Message);
    //        }
    //    });
    //}
}
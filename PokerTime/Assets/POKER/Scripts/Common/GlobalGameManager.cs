﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GlobalGameManager : MonoBehaviour
{

#if UNITY_EDITOR
    public List<SocketEvetns> socketEvents = new List<SocketEvetns>();
    public bool CanDebugThis(SocketEvetns eventName)
    {
        for (int i = 0; i < socketEvents.Count; i++)
        {
            if (socketEvents[i] == eventName)
            {
                return true;
            }
        }

        return false;
    }
#endif


    public static GlobalGameManager instance;

    public bool isLoginShow = false;
    public GameObject[] gameScens; // prefab of all parent screens in game
    private GameObject previousScene = null; // contains current loaded sceneObject

    [SerializeField]
    public RoomData currentRoomData = new RoomData();

    public static bool IsJoiningPreviousGame = false;
    public bool isTokenSent = false;



    private void Awake()
    {
        instance = this;
   //   PlayerPrefs.DeleteAll();
    }
    

    private void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        LoadScene(Scenes.MainMenu);
       // FireBaseAnalyticsIntegration.SharedInstance.LogEvent(FireBaseEvents.Game_Launch);
    }



    public void LoadScene(Scenes gameScene)
    {
        if (previousScene != null)
        {
            StartCoroutine(WaitAndDestroyOldScreen(previousScene));
        }

        previousScene = Instantiate(gameScens[(int)gameScene], Vector3.zero, Quaternion.identity) as GameObject;
        if (MenuHandller.instance != null)
        {
            MenuHandller.instance.UpdateAllText();
        }
    }


    private IEnumerator WaitAndDestroyOldScreen(GameObject gm)
    {
        yield return new WaitForSeconds(1);

        if (gm != null)
        {
            Destroy(gm);
        }
        else
        {

#if ERROR_LOG
            Debug.LogError("Null reference exception found gm is null in GlobalGameManager.WaitAndDestroyOldScreen ");
#endif
        }

        yield return new WaitForSeconds(0.2f);
        System.GC.Collect();
    }

    private void OnApplicationQuit()
    {
        SaveAllData();
    }

    public void CloseApplication()
    {
        SaveAllData();
        Application.Quit();
    }

    private void SaveAllData()
    {
        if (null != PlayerManager.instance)
        {
            PlayerGameDetails playerGame = PlayerManager.instance.GetPlayerGameData();
            PrefsManager.SetPlayerGameData(PlayerManager.instance.GetPlayerGameData());
        }
        //FireBaseAnalyticsIntegration.SharedInstance.LogEvent(FireBaseEvents.Game_Close);        
    }

    private List<List<RoomData>> allRoomData = new List<List<RoomData>>();
    private int gameMode;

    public void StoreLastLobbyData(List<List<RoomData>> _allRoomData, int _gameMode)
    {
        allRoomData = _allRoomData;
        gameMode = _gameMode;
    }

    public List<List<RoomData>> GetLobbyRoomData()
    {
        return allRoomData;
    }

    public int GetGameMode()
    {
        return gameMode;
    }

    public void SetRoomData(RoomData data)
    {
        currentRoomData = data;
    }

    public RoomData GetRoomData()
    {
        return currentRoomData;
    }

    public void SendFirebaseToken(string token)
    {
        if (!isTokenSent && token.Length > 0 && PlayerManager.instance.IsLogedIn())
        {
            isTokenSent = true;

            string requestData = "{\"userId\":\"" + PlayerManager.instance.GetPlayerGameData().userId + "\"," +
          "\"token\":\"" + token + "\"}";

            WebServices.instance.SendRequest(RequestType.SendNotificationToken, requestData, true);
        }
        

       
    }


}


public enum Scenes
{
    MainMenu,
    InGame,
    MainMenuTeenPatti,
    InGameTeenPatti
}

public enum GameMode
{
    NLH,
    PLO,
    OFC
}

public enum GameModeTeen
{
    CLASSIC,
    MUFLIS,
    JOKER,
    _999
}


[System.Serializable]
public class RoomData
{
    public string roomId;
    public string socketTableId;
    public string title;
    public int players;
    public float commision;

    //DEV_CODE
    public string roomIconUrl;
    public string roomBG;

    public float smallBlind;
    public float bigBlind;
    public float minBuyIn;
    public float maxBuyIn;
    public int callTimer;
    public GameMode gameMode;
    public bool isLobbyRoom;

    public int totalActivePlayers;

    //DEV_CODE
    public bool isEVChop = false;
    public bool isRunMulti = false;
}



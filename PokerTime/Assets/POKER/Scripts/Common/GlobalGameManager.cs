using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public GameObject[] gameScens; // prefab of all parent screens in game
    private GameObject previousScene = null; // contains current loaded sceneObject

    [SerializeField]
    private RoomData currentRoomData = new RoomData();

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
        PrefsManager.SetPlayerGameData(PlayerManager.instance.GetPlayerGameData());
        //FireBaseAnalyticsIntegration.SharedInstance.LogEvent(FireBaseEvents.Game_Close);
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
    InGame
}

public enum GameMode
{
    NLH,
    PLO,
    OFC
}


[System.Serializable]
public class RoomData
{
    public string roomId;
    public string socketTableId;
    public string title;
    public int players;
    public float commision;

    public float smallBlind;
    public float bigBlind;
    public float minBuyIn;
    public float maxBuyIn;
    public int callTimer;
    public GameMode gameMode;
    public bool isLobbyRoom;
}



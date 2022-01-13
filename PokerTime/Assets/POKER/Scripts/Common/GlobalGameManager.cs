using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

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
    public bool isTokenSent = false, creatingNewTable = false;

    //DEV_CODE
    public static int RunItMultiTimes = 0, currentTableInd;
    [HideInInspector]
    public string currentClubName, currentUniqueClubId, currentClubId, currentClubProfileImagePath, currentPlayerType, currentPlayerRole;
    public GameObject newClubTable;
    public Dictionary<string, GameObject> AllTables = new Dictionary<string, GameObject>();
    public List<GameObject> table = new List<GameObject>();

    public List<RoomData> tableData = new List<RoomData>();
    
    private void Awake()
    {
        instance = this;
        //PlayerPrefs.DeleteAll();
    }


    private void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        LoadScene(Scenes.MainMenu);
        // FireBaseAnalyticsIntegration.SharedInstance.LogEvent(FireBaseEvents.Game_Launch);

        //Debug.Log("Score " + CalculateSliderValue(100000));
    }



    public void LoadScene(Scenes gameScene)
    {


        Debug.Log("************SCENE******************" + (GameObject.Find("MainMenuScene(Clone)") ? "Find Success ==> " + gameScene.ToString() : "No Scene Available" + gameScene.ToString()));

        //if (gameScene.ToString().Equals("MainMenu") && GameObject.Find("MainMenuScene(Clone)"))
        //{
        //    GameObject.Find("MainMenuScene(Clone)").SetActive(true);
        //}
        //else if (gameScene.ToString().Equals("InGame") && GameObject.Find("InGame(Clone)"))
        //{
        //    GameObject.Find("InGame(Clone)").SetActive(true);
        //}
        //else
        //{
        Debug.Log(previousScene);
        if (previousScene != null)
        {
            StartCoroutine(WaitAndDestroyOldScreen(previousScene));
        }
        previousScene = Instantiate(gameScens[(int)gameScene], Vector3.zero, Quaternion.identity) as GameObject;
        Debug.Log(previousScene + " - " + gameScens[(int)gameScene].name);
        /*if (previousScene.name != "MainMenuScene(Clone)" && previousScene.name == gameScens[(int)gameScene].name + "(Clone)")
        {
            previousScene.name = GetRoomData().roomId;
            AllTables.Add(GetRoomData().roomId, previousScene);
            table.Add(previousScene);
            ClubSocketController.instance.tableButton[0].name = GetRoomData().roomId;
        }*/

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
            if (gm.name == "MainMenuScene(Clone)" || gm.name == "InGame(Clone)" || gm.name == "ClubSocketController(Clone)" || gm.name == "InGameTournament(Clone)")
            {
                Destroy(gm);
            }
            else
            {
                //Destroy(gm);          //DEV_CODE This line is commented
                gm.SetActive(false);    //DEV_CODE This line is added
            }
        }
        else
        {

#if ERROR_LOG
            Debug.LogError("Null reference exception found gm is null in GlobalGameManager.WaitAndDestroyOldScreen ");
#endif
        }

        yield return new WaitForSeconds(0.2f);
        System.GC.Collect();

        /*yield return new WaitForSeconds(20);
        GameObject g = Instantiate(gameScens[(int)Scenes.InGame], Vector3.zero, Quaternion.identity) as GameObject;
        g.name = "Second";*/
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

    public string ScoreShow(int Score)
    {
        float Scor = Score;
        string result;
        string[] ScoreNames = new string[] { "", "K", "M", "B", "T", "aa", "ab", "ac", "ad", "ae", "af", "ag", "ah", "ai", "aj", "ak", "al", "am", "an", "ao", "ap", "aq", "ar", "as", "at", "au", "av", "aw", "ax", "ay", "az", "ba", "bb", "bc", "bd", "be", "bf", "bg", "bh", "bi", "bj", "bk", "bl", "bm", "bn", "bo", "bp", "bq", "br", "bs", "bt", "bu", "bv", "bw", "bx", "by", "bz", };
        int i;

        for (i = 0; i < ScoreNames.Length; i++)
            if (Scor < 10000)
                break;
            else Scor = Mathf.Floor(Scor / 100f) / 10f;

        if (Scor == Mathf.Floor(Scor))
            result = Scor.ToString() + ScoreNames[i];
        else result = Scor.ToString("F1") + ScoreNames[i];
        return result;
    }

    public int CalculateSliderValue(int amount)
    {
        int digit = GetDigitOfANumber(amount, 0);
        if (digit == 1)
            return amount;
        digit = digit - 2;
        Debug.Log("Raise Amount " + (2 * Mathf.Pow(10, digit)));
        return (int)(2 * Mathf.Pow(10, digit));
    }

    public int GetDigitOfANumber(int num, int totalDigit)
    {
        if (num == 0)
            return totalDigit;

        return GetDigitOfANumber(num / 10, ++totalDigit);
    }
}


public enum Scenes
{
    MainMenu,
    InGame,
    MainMenuTeenPatti,
    InGameTeenPatti,
    ClubInGame,  //DEV_CODE Added for referencing Club Prefab
    TournamentInGame
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

    public int passCode;
    public string exclusiveTable;
    public string assignRole;
}

public class RoomDataTeen
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
    public GameModeTeen gameMode;
    public bool isLobbyRoom;

    public int totalActivePlayers;
}
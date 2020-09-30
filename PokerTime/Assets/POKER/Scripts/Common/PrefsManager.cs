using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;


public class PrefsManager: MonoBehaviour
{
    public static void SetData(PrefsKey prefsKey,string dataToAssign)
    {
        PlayerPrefs.SetString("" + prefsKey, dataToAssign);
    }

    public static RoomData GetRoomData()
    {
        RoomData roomData = new RoomData();

        string stringData = PlayerPrefs.GetString(""+PrefsKey.RoomData);

        Debug.Log("parsing room = " + stringData);

        

        if (!string.IsNullOrEmpty(stringData))
        {
            JsonData data = JsonMapper.ToObject(stringData);

            Debug.Log("is contains key = " + data.Keys.Contains("isLobbyRoom"));

            if (data["roomId"] != null)
            {
                roomData.roomId = data["roomId"].ToString();
            }

            if (data["socketTableId"] != null)
            {
                roomData.socketTableId = data["socketTableId"].ToString();
            }

            if (data["title"] != null)
            {
                roomData.title = data["title"].ToString();
            }

            if (data["players"] != null && data["players"].ToString().Length > 0)
            {
                roomData.players = int.Parse(data["players"].ToString());
            }
            else
            {
                roomData.players = 4;
            }



            if (data["callTimer"] != null && data["callTimer"].ToString().Length > 0)
            {
                roomData.callTimer = int.Parse(data["callTimer"].ToString());
            }
            else
            {
                roomData.callTimer = 30;
            }

            if (data["commision"] != null && data["commision"].ToString().Length > 0)
            {
                roomData.commision = float.Parse(data["commision"].ToString());
            }
            else
            {
                roomData.commision = 1;
            }


            if (data["smallBlind"] != null && data["smallBlind"].ToString().Length > 0)
            {
                roomData.smallBlind = float.Parse(data["smallBlind"].ToString());
            }
            else
            {
                roomData.smallBlind = 10;
            }


            if (data["bigBlind"] != null && data["bigBlind"].ToString().Length > 0)
            {
                roomData.bigBlind = float.Parse(data["bigBlind"].ToString());
            }
            else
            {
                roomData.bigBlind = 20;
            }


            if (data["minBuyIn"] != null && data["minBuyIn"].ToString().Length > 0)
            {
                roomData.minBuyIn = float.Parse(data["minBuyIn"].ToString());
            }
            else
            {
                roomData.minBuyIn = 100;
            }

            if (data["maxBuyIn"] != null && data["maxBuyIn"].ToString().Length > 0)
            {
                roomData.maxBuyIn = float.Parse(data["maxBuyIn"].ToString());
            }
            else
            {
                roomData.maxBuyIn = 1000;
            }

            if (data.Keys.Contains("isLobbyRoom") && data["isLobbyRoom"] != null && data["isLobbyRoom"].ToString().Length > 0)
            {
                roomData.isLobbyRoom = data["isLobbyRoom"].ToString() == "True";
            }
            else
            {
                roomData.isLobbyRoom = true;
            }


            if (data["gameMode"] != null && data["gameMode"].ToString().Length > 0)
            {
                string[] gameModes = System.Enum.GetNames(typeof(GameMode));
                string currentName = data["gameMode"].ToString();
                roomData.gameMode = GameMode.NLH;

                for (int i = 0; i < gameModes.Length; i++)
                {
                    if (currentName == gameModes[i])
                    {
                        roomData.gameMode = (GameMode)i;
                        break;
                    }
                }
            }
            else
            {
                roomData.gameMode = GameMode.NLH;
            }
        }
        else
        {
            roomData.socketTableId = "";
        }

        return roomData;
    }


    public static void SetPlayerGameData(PlayerGameDetails dataToAssign)
    {
        PlayerPrefs.SetString("" + PrefsKey.PlayerGameData, JsonUtility.ToJson(dataToAssign));
    }


    public static PlayerGameDetails GetPlayerData()
    {
        PlayerGameDetails playerData = new PlayerGameDetails();
       
        string prefsData = PlayerPrefs.GetString(""+PrefsKey.PlayerGameData,"");

        if (!string.IsNullOrEmpty(prefsData))
        {
            JsonData data = JsonMapper.ToObject(prefsData);

            playerData.userId = data["userId"].ToString();
            playerData.password = data["password"].ToString();
            playerData.userName = data["userName"].ToString();

            if (data.Keys.Contains("coins") && data["coins"].ToString() != null && data["coins"].ToString().Length > 0)
            {
                float.TryParse(data["coins"].ToString(), out playerData.coins);
                // playerData.coins = float.Parse(data["coins"].ToString());
            }
            else
            {
                playerData.coins = 0f;
            }


            if (data.Keys.Contains("diamonds") && data["diamonds"].ToString() != null && data["diamonds"].ToString().Length > 0)
            {
                playerData.diamonds = float.Parse(data["diamonds"].ToString());
            }
            else
            {
                playerData.diamonds = 0;
            }

            if (data.Keys.Contains("points") && data["points"].ToString() != null && data["points"].ToString().Length > 0)
            {
                playerData.points = float.Parse(data["points"].ToString());
            }
            else
            {
                playerData.points = 0;
            }


            if (data.Keys.Contains("rabit") && data["rabit"].ToString() != null && data["rabit"].ToString().Length > 0)
            {
                playerData.rabit = int.Parse(data["rabit"].ToString());
            }
            else
            {
                playerData.rabit = 0;
            }
            if (data.Keys.Contains("emoji") && data["emoji"].ToString() != null && data["emoji"].ToString().Length > 0)
            {
                playerData.emoji = int.Parse(data["emoji"].ToString());
            }
            else
            {
                playerData.emoji = 0;
            }

            if (data.Keys.Contains("time") && data["time"].ToString() != null && data["time"].ToString().Length > 0)
            {
                playerData.time = int.Parse(data["time"].ToString());
            }
            else
            {
                playerData.time = 0;
            }

        }
        else
        {
            playerData.userName = playerData.password = playerData.userId = "";

            playerData.coins = playerData.diamonds = playerData.points = 0f;
            playerData.rabit = playerData.emoji = playerData.time = 0;
        }

        return playerData;
    }
}



public enum PrefsKey
{
    PlayerGameData,
    RoomData
}






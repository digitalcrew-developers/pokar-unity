using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{

    public static PlayerManager instance;

    [SerializeField]
    private PlayerGameDetails playerGameData = null; // contains user details


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        playerGameData = PrefsManager.GetPlayerData();
    }


    public PlayerGameDetails GetPlayerGameData()
    {
        return playerGameData;
    }

    public void SetPlayerGameData(PlayerGameDetails dataToAssign)
    {
        playerGameData = dataToAssign;
        PrefsManager.SetPlayerGameData(playerGameData);
    }

    public bool IsLogedIn()
    {
        if (playerGameData.userId.Length > 0)
        {
            return true;
        }

        return false;
    }

}

[System.Serializable]
public class PlayerGameDetails
{
    public string userId;
    public string userName;
    public string password;
    public string avatarURL,FrameUrl,CountryURL;
    public string referralCode;
    public string userLevel;
    public string countryName;
    public float coins, diamonds, points;
    public int rabit, emoji, time;
}

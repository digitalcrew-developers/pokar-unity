using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.Text.RegularExpressions;



public class Utility : MonoBehaviour
{
    public const string MatchEmailPattern =
        @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
        + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
        + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
        + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";

    public static bool IsValidUserName(string dataToValidate, out string error)
    {
        error = "";

        if (dataToValidate.Length < 3)
        {
            if (dataToValidate.Length <= 0)
            {
                error = "Please enter username";
            }
            else
            {
                error = "username must contains atleast 3 characters";
            }

            return false;
        }

        return true;
    }

    public static bool IsValidPassword(string dataToValidate, out string error)
    {
        error = "";

        if (dataToValidate.Length < 3)
        {
            if (dataToValidate.Length <= 0)
            {
                error = "Please enter password";
            }
            else
            {
                error = "Password must contains atleast 3 characters";
            }

            return false;
        }

        return true;
    }

    public static bool IsValidEmail(string dataToValidate, out string error)
    {
        error = "";

        if (dataToValidate.Length <= 0)
        {
            error = "Please enter email";
            return false;
        }
        else if (dataToValidate != null && dataToValidate.Length > 3)
        {
            if (Regex.IsMatch(dataToValidate, MatchEmailPattern))
                return true;
            else
            {
                error = "Please enter valid email";
                return false;
            }
        }
        else
        {
            error = "Please enter valid email";
            return false;
        }
    }

    public static bool IsValidClubName(string dataToValidate, out string error)
    {
        error = "";

        if (dataToValidate.Length < 3)
        {
            if (dataToValidate.Length <= 0)
            {
                error = "Please enter ClubName";
            }
            else
            {
                error = "Club name must contains atleast 3 characters";
            }

            return false;
        }

        return true;
    }

    public static PlayerGameDetails ParseUserDetails(JsonData jsonObject)
    {
        PlayerGameDetails playerData = new PlayerGameDetails();

        JsonData data = jsonObject["getData"][0];

        playerData.userId = data["userId"].ToString();
        playerData.userName = data["userName"].ToString();
        playerData.userLevel = data["userLevel"].ToString();
        if (data["countryName"] != null)
            playerData.countryName = data["countryName"].ToString();
        if (data["profileImage"] != null)
            playerData.avatarURL = data["profileImage"].ToString();
        if (data["countryFlag"] != null)
            playerData.CountryURL = data["countryFlag"].ToString();
        if (data["frameURL"] != null)
            playerData.FrameUrl = data["frameURL"].ToString();
        if (data["countryCode"] != null)
            playerData.countryCode = data["countryCode"].ToString();

        if (data["registrationType"] != null)
            playerData.registrationType = data["registrationType"].ToString();

        if (data["referalCode"] != null && data["referalCode"].ToString().Length > 0)
        {
            playerData.referralCode = data["referalCode"].ToString();
        }
        else
        {
            playerData.referralCode = "";
        }

        if (data["coins"] != null && data["coins"].ToString().Length > 0)
        {
            playerData.coins = float.Parse(data["coins"].ToString());
        }
        else
        {
            playerData.coins = 0;
        }

        if (data["diamond"] != null && data["diamond"].ToString().Length > 0)
        {
            playerData.diamonds = float.Parse(data["diamond"].ToString());
        }
        else
        {
            playerData.diamonds = 0;
        }

        if (data["points"] != null && data["points"].ToString().Length > 0)
        {
            playerData.points = float.Parse(data["points"].ToString());
        }
        else
        {
            playerData.points = 0;
        }


        if (data["rabbit"] != null && data["rabbit"].ToString().Length > 0)
        {
            playerData.rabit = int.Parse(data["rabbit"].ToString());
        }
        else
        {
            playerData.rabit = 0;
        }

        if (data["emoji"] != null && data["emoji"].ToString().Length > 0)
        {
            playerData.emoji = int.Parse(data["emoji"].ToString());
        }
        else
        {
            playerData.emoji = 0;
        }

        if (data["time"] != null && data["time"].ToString().Length > 0)
        {
            playerData.time = int.Parse(data["time"].ToString());
        }
        else
        {
            playerData.time = 0;
        }


        return playerData;
    }


    public static PlayerGameDetails ParsePlayerGameData(JsonData jsonObject)
    {
        PlayerGameDetails playerData = PlayerManager.instance.GetPlayerGameData();
        if(null== playerData)
        {
            playerData = new PlayerGameDetails();
        }

        JsonData data = jsonObject["data"][0];

        playerData.userId = data["userId"].ToString();
        playerData.userName = data["userName"].ToString();
        playerData.userLevel = data["userLevel"].ToString();
        if (data["countryName"] != null)
            playerData.countryName = data["countryName"].ToString();
        if (data["profileImage"] != null)
            playerData.avatarURL = data["profileImage"].ToString();
        if (data["countryFlag"] != null)
            playerData.CountryURL = data["countryFlag"].ToString();
        if (data["frameURL"] != null)
            playerData.FrameUrl = data["frameURL"].ToString();
        if (data["countryCode"] != null)
            playerData.countryCode = data["countryCode"].ToString();

        if(data["registrationType"] != null)
            playerData.registrationType = data["registrationType"].ToString();

        //if (data["profileImage"] != null && data["profileImage"].ToString().Length > 0)
        //{
        //}
        //else
        //{
        //    playerData.avatarURL = "";
        //}

        if (data["referalCode"] != null && data["referalCode"].ToString().Length > 0)
            {
                playerData.referralCode = data["referalCode"].ToString();
            }
            else
            {
                playerData.referralCode = "";
            }



        if (data["coins"] != null && data["coins"].ToString().Length > 0)
        {
            playerData.coins = float.Parse(data["coins"].ToString());
        }
        else
        {
            playerData.coins = 0;
        }

        if (data["diamond"] != null && data["diamond"].ToString().Length > 0)
        {
            playerData.diamonds = float.Parse(data["diamond"].ToString());
        }
        else
        {
            playerData.diamonds = 0;
        }

        if (data["points"] != null && data["points"].ToString().Length > 0)
        {
            playerData.points = float.Parse(data["points"].ToString());
        }
        else
        {
            playerData.points = 0;
        }


        if (data["rabbit"] != null && data["rabbit"].ToString().Length > 0)
        {
            playerData.rabit = int.Parse(data["rabbit"].ToString());
        }
        else
        {
            playerData.rabit = 0;
        }

        if (data["emoji"] != null && data["emoji"].ToString().Length > 0)
        {
            playerData.emoji = int.Parse(data["emoji"].ToString());
        }
        else
        {
            playerData.emoji = 0;
        }

        if (data["time"] != null && data["time"].ToString().Length > 0)
        {
            playerData.time = int.Parse(data["time"].ToString());
        }
        else
        {
            playerData.time = 0;
        }


        return playerData;
    }



    public static string GetTrimmedAmount(string currentValueInString)
    {
        double currentValue = double.Parse(currentValueInString);
        string updatedAmount = "";


        if (currentValue > 1000000000) // One Billion
        {
            updatedAmount = "" + (currentValue / 1000000000).ToString("0.00") + "B";
        }
        else if (currentValue > 1000000) // One Million
        {
            updatedAmount = "" + (currentValue / 1000000).ToString("0.00") + "M";
        }
        else if (currentValue > 1000) // One Thousand
        {
            updatedAmount = "" + (currentValue / 1000).ToString("0.00") + "K";
        }
        else if(currentValue <1000)
        {
            updatedAmount = "" + (currentValue / 1000).ToString("0.00") + "K";
        }
        else if(currentValue < 1000000) // One Million
        {
            updatedAmount = "" + (currentValue / 1000000).ToString("0.00") + "M";
        }
        else if (currentValue < 1000000000) // One Billion
        {
            updatedAmount = "" + (currentValue / 1000000000).ToString("0.00") + "B";
        }
        else
        {
            updatedAmount = currentValueInString;
        }

        if (updatedAmount.Contains(".00"))
        {
            updatedAmount = updatedAmount.Replace(".00", "");
        }

        return updatedAmount;
    }

}

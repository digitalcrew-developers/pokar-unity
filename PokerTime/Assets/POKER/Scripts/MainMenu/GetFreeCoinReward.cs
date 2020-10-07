using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class GetFreeCoinReward : MonoBehaviour
{
    public Text RewardTime;
    public Button RewardBtn;
    public DateTime lastRewardTime, now;     // The last time the user clicked in a reward
    public TimeSpan timer;
    public float maxTime = 900f; // How many seconds until the player can claim the reward
    public bool canClaim;              // Checks if the user can claim the reward 
    // Needed Constants
    private const string TIMED_REWARDS_TIME = "TimedRewardsTime";
    private const string FMT = "O";
    public bool isInitialized = false, isStartGame;
    private void Awake()
    {
        //   PlayerPrefs.DeleteKey(TIMED_REWARDS_TIME);
    }
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(InitializeTimer());
    }

    public void OnServerResponseFound(RequestType requestType, string serverResponse, bool isShowErrorMessage, string errorMessage)
    {
        if (errorMessage.Length > 0)
        {
            if (isShowErrorMessage)
            {
                Debug.LogError("Error API IS :=> " + errorMessage);
                MainMenuController.instance.ShowMessage(errorMessage);
            }
            return;
        }
        if (requestType == RequestType.GetRewardCoins)
        {
            JsonData data = JsonMapper.ToObject(serverResponse);
            if (data["status"].Equals(true))
            {
                PlayerPrefs.SetString(TIMED_REWARDS_TIME, now.Add(timer - TimeSpan.FromSeconds(maxTime)).ToString(FMT));
                timer = TimeSpan.FromSeconds(maxTime);
                canClaim = false;
                isStartGame = false;
            }
            else
            {
                Debug.LogError("Error API IS :=> " + errorMessage);

            }
        }
        else
        {

#if ERROR_LOG
            Debug.LogError("Unhadnled response found in  = " + requestType);
#endif
        }
    }
    private IEnumerator InitializeTimer()
    {
        now = DateTime.Now;
        isInitialized = true;
        if(!PlayerPrefs.HasKey(TIMED_REWARDS_TIME))
        {
            PlayerPrefs.SetString(TIMED_REWARDS_TIME, "00:00:00");
        }

        string lastRewardTimeStr = PlayerPrefs.GetString(TIMED_REWARDS_TIME);
        Debug.Log("lastRewardTimeStr" + lastRewardTimeStr);
        if (!string.IsNullOrEmpty(lastRewardTimeStr))
        {
            lastRewardTime = DateTime.ParseExact(lastRewardTimeStr, FMT, CultureInfo.InvariantCulture);
            timer = (lastRewardTime - now).Add(TimeSpan.FromSeconds(maxTime));
            //      Debug.LogError("timer***********************"+ timer);
        }
        else
        {
            //    Debug.LogError("timer ///////////////////" + timer);
            timer = TimeSpan.FromSeconds(maxTime);
        }

        //  Debug.LogError("timer ======"+timer);
        yield return new WaitForSeconds(0.1f);
    }
    void Update()
    {
        if (!isInitialized)
        {
            return;
        }
        now = now.AddSeconds(Time.unscaledDeltaTime);
        // Keeps ticking until the player claims
        if (!canClaim)
        {
            timer = timer.Subtract(TimeSpan.FromSeconds(Time.unscaledDeltaTime));
            if (timer.TotalSeconds <= 0)
            {
                //    Debug.LogError("!!!!!!!!!!!!!!");
                canClaim = true;
                RewardBtn.interactable = true;
                isStartGame = true;
                RewardTime.text = " ";// + string.Format("{0:D2}:{1:D2}:{2:D2}", timer.Hours, timer.Minutes, timer.Seconds);
            }
            else
            {
                // We need to save the player time every tick. If the player exits the game the information keeps logged
                // For perfomance issues you can save this information when the player switches scenes or quits the application
                PlayerPrefs.SetString(TIMED_REWARDS_TIME, now.Add(timer - TimeSpan.FromSeconds(maxTime)).ToString(FMT));
                //   Debug.LogError("2222222222222222222");
                TimeSpan timers = timer;
                //  Debug.LogError(timer);
                if (timers.TotalSeconds > 0)
                {
                    RewardBtn.interactable = false;
                    RewardTime.text = string.Format("{0:D2}:{1:D2}:{2:D2}", timer.Hours, timer.Minutes, timer.Seconds);
                    isStartGame = false;
                }
            }
        }
    }
    // The player claimed the prize. We need to reset to restart the timer
    public void ClaimReward()
    {
        string requestData = "{\"userId\":\"" + PlayerManager.instance.GetPlayerGameData().userId + "\"," +
                               "\"type\":\"" + "COINREWARD" + "\"}";
        WebServices.instance.SendRequest(RequestType.GetRewardCoins, requestData, true, OnServerResponseFound);


    }
    public void Reset()
    {
        PlayerPrefs.DeleteKey(TIMED_REWARDS_TIME);
        canClaim = true;
        timer = TimeSpan.FromSeconds(0);

    }
}

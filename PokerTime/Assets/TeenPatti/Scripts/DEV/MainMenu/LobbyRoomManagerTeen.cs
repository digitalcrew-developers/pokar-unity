using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyRoomManagerTeen : MonoBehaviour
{
    public GameObject insufficentPopUp;

    public Sprite[] roomTypeImages; 
    public void CallInsufficientCoin(RoomData data) {
        if (PlayerManager.instance.GetPlayerGameData().coins < data.minBuyIn)
        {
            //MainMenuController.instance.ShowMessage("You dont have sufficient coins to play, please purchase coins to continue",()=>{
            //    MainMenuController.instance.ShowScreen(MainMenuScreens.Shop);
            //},()=> {
            //},"Shop","Cancel");
            insufficentPopUp.SetActive(true);
            return;
            // InsufficientCoinLobbyRoomManager.in
        }
        else
        {
            PlayerGameDetails playerData = PlayerManager.instance.GetPlayerGameData();
            playerData.coins -= data.minBuyIn;
            if(playerData.coins < 0) { playerData.coins = 0f; }
            LobbyUiManagerTeen.instance.coinsText.text = Utility.GetTrimmedAmount("" + PlayerManager.instance.GetPlayerGameData().coins);

            UpdateUserBalance(playerData);
        }
    }

    public void UpdateUserBalance(PlayerGameDetails updatedData)
    {
        string requestData = "{\"userId\":\"" + PlayerManager.instance.GetPlayerGameData().userId + "\"," +
            "\"silver\":\"0\"," +
            "\"coins\":\"" + (int)updatedData.coins + "\"," +
            "\"points\":\"" + (int)updatedData.points + "\"," +
            "\"diamond\":\"" + (int)updatedData.diamonds + "\"," +

            "\"rabbit\":\"0\"," +
            "\"emoji\":\"0\"," +
            "\"time\":\"0\"," +
            "\"day\":\"0\"," +
            "\"playerProgress\":\"\"}";

        MainMenuControllerTeen.instance.ShowScreen(MainMenuScreensTeen.Loading);

        WebServices.instance.SendRequest(RequestType.UpdateUserBalance, requestData, true, (requestType, serverResponse, isShowErrorMessage, errorMessage) =>
        {
            MainMenuControllerTeen.instance.DestroyScreen(MainMenuScreensTeen.Loading);

            if (errorMessage.Length > 0)
            {
                MainMenuControllerTeen.instance.ShowMessage(errorMessage);
            }
            else
            {
                JsonData data = JsonMapper.ToObject(serverResponse);
                if (data["status"].Equals(true))
                {
                    PlayerManager.instance.SetPlayerGameData(updatedData);
                    UpdateAlltext(updatedData);
                }
                else
                {
                    MainMenuControllerTeen.instance.ShowMessage(data["message"].ToString());
                }
            }
        });
    }

    private void UpdateAlltext(PlayerGameDetails playerData)
    {
        LobbyUiManagerTeen.instance.coinsText.text = Utility.GetTrimmedAmount("" + playerData.coins);        
    }

    public void CallInGameShopScreen(RoomData data)
    {
        if (PlayerManager.instance.GetPlayerGameData().coins < data.minBuyIn)
        {
            //MainMenuController.instance.ShowMessage("You dont have sufficient coins to play, please purchase coins to continue",()=>{
            //    MainMenuController.instance.ShowScreen(MainMenuScreens.Shop);
            //},()=> {
            //},"Shop","Cancel");
            insufficentPopUp.SetActive(true);
            return;
            // InsufficientCoinLobbyRoomManager.in
        }
    }

    public void OnShopBtnClick()
    {
        MainMenuControllerTeen.instance.ShowScreen(MainMenuScreensTeen.Shop);
         
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyRoomManager : MonoBehaviour
{
    public GameObject insufficentPopUp;
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
          MainMenuController.instance.ShowScreen(MainMenuScreens.InGameShop);
         
    }
}

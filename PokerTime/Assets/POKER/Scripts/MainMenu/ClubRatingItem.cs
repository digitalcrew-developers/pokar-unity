using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ClubRatingItem : MonoBehaviour
{
    public Button ClickButton;
    public TextMeshProUGUI Title1, Title2, Manager, Member, Diamonds;
    private Text TipText;
    private GameObject TipPopup;
    private Button TipPopConfirmButton;
    private string title1, title2, manager, member, diamonds;
    public Image LevelImage;

    public void Initialise(string _title1, string _title2, string _manager, string _member, string _diamonds, string levelImagePath = "")
    {
        title1 = _title1;
        title2 = _title2;
        manager = _manager;
        member = _member;
        diamonds = _diamonds;

        Title1.text = title1;
        Title2.text = title2;
        Manager.text = manager;
        Member.text = member;
        Diamonds.text = diamonds;

        LevelImage.sprite = Resources.Load(levelImagePath, typeof(Sprite)) as Sprite;
        ClickButton.onClick.RemoveAllListeners();
        ClickButton.onClick.AddListener(() => OnClick(title1, diamonds));

        //TipPopup = GameObject.FindObjectOfType<ClubAdminManager>().gameObject.transform.Find("ClubTips").gameObject;
        //TipText = TipPopup.transform.Find("Heading/Panel/Panel/Text").GetComponent<Text>();
        //TipPopConfirmButton = TipPopup.transform.Find("Heading/Panel/BtnConfirm").GetComponent<Button>();
    }

    private void OnClick(string title, string diamonds)
    {
        diamonds = diamonds.Replace(",", "");
        int _diamonds = 0;
        int.TryParse(diamonds, out _diamonds);
        //TipPopup.SetActive(true);
        //TipText.text = "You are purchasing a " + title + "for 30 days";

        //TipPopConfirmButton.onClick.RemoveAllListeners();
        //TipPopConfirmButton.onClick.AddListener(() => OnConfirm(_diamonds));

        MainMenuController.instance.ShowMessage("You are purchasing a " + title + " for 30 days", () =>
        {
            OnConfirm(_diamonds);
        }, () =>
        {
        }, "Confirm", "Cancel");
    }

    private void OnConfirm(int diamonds)
    {
        Debug.Log("Buying for diamonds :" + diamonds);

        string clubRatingRequest = "{\"userId\":\"" + PlayerManager.instance.GetPlayerGameData().userId + "\"," +
                "\"clubId\":\"" + ClubDetailsUIManager.instance.GetClubId()
                + "\"}";

        WebServices.instance.SendRequest(RequestType.RateClub, clubRatingRequest, true, OnServerResponseFound);
    }

    public void OnServerResponseFound(RequestType requestType, string serverResponse, bool isShowErrorMessage, string errorMessage)
    {
        Debug.Log("server response club rating : " + serverResponse);
        MainMenuController.instance.DestroyScreen(MainMenuScreens.Loading);

        if (errorMessage.Length > 0)
        {
            if (isShowErrorMessage)
            {
                MainMenuController.instance.ShowMessage(errorMessage);
            }

            return;
        }

        switch (requestType)
        {
            case RequestType.RateClub:
                {

                }
                break;

            default:
#if ERROR_LOG
                Debug.LogError("Unhandled requestType found in  MenuHandller = " + requestType);
#endif
                break;
        }

    }
}

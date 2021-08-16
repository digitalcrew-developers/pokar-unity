using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSeat : MonoBehaviour
{
    public string seatNo;
    public Button myButton;
    public Sprite EmptyImage, PlusImage;

    private void Start()
    {
        GetComponent<Button>().onClick.RemoveAllListeners();
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public void UpdateState()
    {
        //DEV_CODE Changed code
        if (InGameManager.instance != null)
        {
            if (InGameManager.instance.AmISpectator)
            {
                GetComponent<Image>().sprite = PlusImage;
                myButton.interactable = true;
                GetComponent<Transform>().localScale = new Vector3(0.7f, 0.7f, 0.7f);
            }
            else
            {
                GetComponent<Image>().sprite = EmptyImage;
                myButton.interactable = false;
                GetComponent<Transform>().localScale = new Vector3(1f, 1f, 1f);
            }
        }
        else if (ClubInGameManager.instance != null)
        {
            if (ClubInGameManager.instance.AmISpectator)
            {
                GetComponent<Image>().sprite = PlusImage;
                myButton.interactable = true;
                GetComponent<Transform>().localScale = new Vector3(0.7f, 0.7f, 0.7f);
            }
            else
            {
                GetComponent<Image>().sprite = EmptyImage;
                myButton.interactable = false;
                GetComponent<Transform>().localScale = new Vector3(1f, 1f, 1f);
            }
        }
        else
        {
            if (TournamentInGameManager.instance.AmISpectator)
            {
                GetComponent<Image>().sprite = PlusImage;
                myButton.interactable = true;
                GetComponent<Transform>().localScale = new Vector3(0.7f, 0.7f, 0.7f);
            }
            else
            {
                GetComponent<Image>().sprite = EmptyImage;
                myButton.interactable = false;
                GetComponent<Transform>().localScale = new Vector3(1f, 1f, 1f);
            }
        }

        //if (InGameManager.instance.AmISpectator)
        //{
        //    GetComponent<Image>().sprite = PlusImage;
        //    myButton.interactable = true;
        //    GetComponent<Transform>().localScale = new Vector3(0.7f, 0.7f, 0.7f);
        //}
        //else
        //{
        //    GetComponent<Image>().sprite = EmptyImage;
        //    myButton.interactable = false;
        //    GetComponent<Transform>().localScale = new Vector3(1f, 1f, 1f);
        //}
    }

    public void DisableButtonClick()
    {
        myButton.interactable = false;
    }

    public void OnClick()
    {
        //if (InGameManager.instance.AmISpectator)
        //{
        //    SocketController.instance.SendGameJoinRequest(seatNo);
        //}
        InGameManager.instance.isSeatRotation = false;
        if (InGameManager.instance != null)
        {
            if (InGameManager.instance.AmISpectator)
            {
                SocketController.instance.SendGameJoinRequest(int.Parse(seatNo));
            }
        }
        else if(ClubInGameManager.instance != null)
        {
            if (ClubInGameManager.instance.AmISpectator)
            {
                ClubSocketController.instance.SendClubGameJoinRequest();
            }
        }
        //else
        //{
        //    if (TournamentInGameManager.instance.AmISpectator)
        //    {
        //        TournamentInGameManager.instance.Send();
        //    }
        //}
    }
}

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
        if (InGameManager.instance != null)
        {
            if (InGameManager.instance.AmISpectator)
            {
                SocketController.instance.SendGameJoinRequest();
            }
        }
        else
        {
            if (ClubInGameManager.instance.AmISpectator)
            {
                ClubSocketController.instance.SendClubGameJoinRequest();
            }
        }
    }
}

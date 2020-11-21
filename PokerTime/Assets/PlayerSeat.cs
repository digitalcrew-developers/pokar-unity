using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSeat : MonoBehaviour
{
    public string seatNo;
    public Button myButton;

    private void Start()
    {
        GetComponent<Button>().onClick.RemoveAllListeners();
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public void UpdateState()
    {
        if (InGameManager.instance.AmISpectator)
        {
            myButton.interactable = true;
        }
        else
        {
            myButton.interactable = false;
        }
    }

    public void OnClick()
    {
        SocketController.instance.SendGameJoinRequestWithSeat(seatNo);
    }
}

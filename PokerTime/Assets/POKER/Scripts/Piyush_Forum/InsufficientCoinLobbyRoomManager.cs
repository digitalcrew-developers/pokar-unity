using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InsufficientCoinLobbyRoomManager : MonoBehaviour
{
    public static InsufficientCoinLobbyRoomManager instance;

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void OnEnable()
    {
        Invoke("DeactiveObj",3);
    }


     void DeactiveObj()
    {
        this.gameObject.SetActive(false);
    }
}

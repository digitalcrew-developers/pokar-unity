using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeletePrefs : MonoBehaviour
{
    void Start()
    {



      //  PlayerPrefs.DeleteAll();
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }
}

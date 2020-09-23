using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackPackManager : MonoBehaviour
{

    public GameObject backPackContentPrefabs;
    public Transform container;

    private void Start()
    {
        SpawnBackPack();
    }

    public void SpawnBackPack() {
        for (int i = 0; i < 3; i++)
        {
            GameObject g = Instantiate(backPackContentPrefabs, container) as GameObject;
            g.transform.SetParent(container);
        }
        
    }

    public void OnCloseBtnClick() {

        MainMenuController.instance.DestroyScreen(MainMenuScreens.BackPack);
    }
}

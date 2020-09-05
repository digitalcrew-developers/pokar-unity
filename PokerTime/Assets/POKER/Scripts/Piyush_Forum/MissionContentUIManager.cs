using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionContentUIManager : MonoBehaviour
{
    public Text missionTypeTxt;
    public Text missionDiscriptionTxt;
    public Text missionValueTxt;
    public Button collectBtn;



    public void OnCollectBtnClick(Transform btn) {

        if (btn.GetChild(0).GetComponent<Text>().text == "Collect")
        {
            Destroy(this.gameObject);
        }
    
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandHashTagPanel : MonoBehaviour
{
    public InputField handInputText;
    public List<GameObject> popularHashTagList = new List<GameObject>();

    private void Awake()
    {
        handInputText.text = "#";
    }

    public void OnClickHashTagButton(int btnIndex)
    {
        handInputText.text = popularHashTagList[btnIndex].transform.GetChild(0).GetComponent<Text>().text;
    }

    public void OnConfirmHashTag()
    {
        HandPostPanel.instance.hashTagText.text = handInputText.text;
        gameObject.SetActive(false);
    }

    public void OnClickBackButton()
    {
        gameObject.SetActive(false);
    }
}
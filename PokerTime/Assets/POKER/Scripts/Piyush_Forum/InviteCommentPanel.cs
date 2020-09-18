using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InviteCommentPanel : MonoBehaviour
{
    public Transform container;
    public GameObject inviteCommentPrefab;

    private GameObject inviteCommentObject;

    private void OnEnable()
    {
        for (int i = 1; i < container.childCount; i++)
        {
            Destroy(container.GetChild(i).gameObject);
        }   
    }
    public void OnClickBackButton()
    {
        gameObject.SetActive(false);
    }
}
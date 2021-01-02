using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmojiBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("**********  GAME OBJECT NAME***************   " + 
                    (InGameUiManager.instance != null ? InGameUiManager.instance.emojiContainerVal: ClubInGameUIManager.instance.emojiContainerVal));

        if (InGameUiManager.instance!=null && (InGameUiManager.instance.emojiContainerVal == 2 || InGameUiManager.instance.sentToEmojiValue.Equals("Dealer")))
        {

            this.transform.SetParent(InGameUiManager.instance.spwantipsKissPos);


        }
        else if(ClubInGameUIManager.instance != null && (ClubInGameUIManager.instance.emojiContainerVal == 2 || ClubInGameUIManager.instance.sentToEmojiValue.Equals("Dealer")))
        {
            this.transform.SetParent(ClubInGameUIManager.instance.spwantipsKissPos);
        }
        else
        {
            if (target != null)
                this.transform.SetParent(target.transform);

        }
        Destroy(this.gameObject, 3);
    }

    public Transform target;
    public float speed;
    void Update()
    {
        if (target != null)
        {
            float step = speed * Time.deltaTime * 400;
            // Debug.LogError("$$$$$$$$      UPDATE i S CALLL  "+ step+"       "+ transform.position+"       "+ target.position);
            transform.position = Vector3.MoveTowards(transform.position, target.position, step);
        }
    }

}

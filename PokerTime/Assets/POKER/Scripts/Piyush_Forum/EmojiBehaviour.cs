using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmojiBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("**********  GAME OBJECT NAME***************   "+ this.transform.name);
        //if (this.transform.parent.parent.parent.name.Equals("0")||
        //this.transform.parent.parent.parent.name.Equals("1")||
        //this.transform.parent.parent.parent.name.Equals("2")||
        //this.transform.parent.parent.parent.name.Equals("3")||
        //this.transform.parent.parent.parent.name.Equals("4")||
        //this.transform.parent.parent.parent.name.Equals("5")||
        //this.transform.parent.parent.parent.name.Equals("6")||
        //this.transform.parent.parent.parent.name.Equals("7")||
        //this.transform.parent.parent.parent.name.Equals("8")||
        //this.transform.parent.parent.parent.name.Equals("9"))
        //{
        //    Debug.Log("EMOJI userID-------------   ");
        //    InGameUiManager.instance.otherId = this.transform.parent.parent.parent.GetComponent<PlayerScript>().otheruserId;
        //   // SocketController.instance.SentEmoji(this.transform.parent.parent.parent.GetComponent<PlayerScript>().otheruserId,InGameUiManager.instance.emojiIndex);

        //}
        
        
        Destroy(this.gameObject, 3);
    }

    public Transform target;
    public float speed;
    void Update()
    {
        float step = speed * Time.deltaTime*300;
        Debug.LogError("$$$$$$$$      UPDATE i S CALLL  "+ step+"       "+ transform.position+"       "+ target.position);
        transform.position = Vector3.MoveTowards(transform.position, target.position, step);
    }

}

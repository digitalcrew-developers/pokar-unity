using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmojiBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("this.transform.parent.parent.parent &&&&&&&&&&&&&   "+ this.transform.parent.parent.parent.name);
        if (this.transform.parent.parent.parent.name.Equals("0")||
        this.transform.parent.parent.parent.name.Equals("1")||
        this.transform.parent.parent.parent.name.Equals("2")||
        this.transform.parent.parent.parent.name.Equals("3")||
        this.transform.parent.parent.parent.name.Equals("4")||
        this.transform.parent.parent.parent.name.Equals("5")||
        this.transform.parent.parent.parent.name.Equals("6")||
        this.transform.parent.parent.parent.name.Equals("7")||
        this.transform.parent.parent.parent.name.Equals("8")||
        this.transform.parent.parent.parent.name.Equals("9"))
        {
            SocketController.instance.SentEmoji(this.transform.parent.parent.parent.GetComponent<PlayerScript>().otheruserId);

        }
        
        
        Destroy(this.gameObject, 3);
    }

   
    
}

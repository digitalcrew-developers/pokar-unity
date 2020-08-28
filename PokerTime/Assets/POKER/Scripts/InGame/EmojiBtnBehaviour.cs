using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmojiBtnBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.transform.GetComponent<Button>().onClick.AddListener(ShowContainer);
    }

   
    void ShowContainer()
    {
        Debug.Log("I CCCCCCCCCCCCCCCCCCCCCCCC   "+this.transform.parent.parent.name) ;
        if (this.transform.parent.parent.name.Equals("0"))
        {

        }
        else if (this.transform.parent.parent.name.Equals("1") ||
       this.transform.parent.parent.name.Equals("2") ||
       this.transform.parent.parent.name.Equals("3") ||
       this.transform.parent.parent.name.Equals("4") ||
       this.transform.parent.parent.name.Equals("5") ||
       this.transform.parent.parent.name.Equals("6") ||
       this.transform.parent.parent.name.Equals("7") ||
       this.transform.parent.parent.name.Equals("8") ||
       this.transform.parent.parent.name.Equals("9"))
        {

        }
        else { 
        
        }
    }
}

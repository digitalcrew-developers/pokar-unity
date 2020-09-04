using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class callTxtAmountManager : MonoBehaviour
{
    public Text callAmount;

    private Vector3 orignalTransfrom;
    private RectTransform txtTransform;
   
    // Start is called before the first frame update
    void Start()
    {
        orignalTransfrom = this.transform.GetComponent<RectTransform>().position;
        txtTransform = this.transform.GetComponent<RectTransform>();
    }

   
    void FixedUpdate()
    {
        if (callAmount.text.Equals(""))
        {
             txtTransform.position = new Vector3(orignalTransfrom.x, orignalTransfrom.y - 20.0f, orignalTransfrom.z);
        }
        else {
            txtTransform.position = new Vector3(orignalTransfrom.x, orignalTransfrom.y, orignalTransfrom.z);
        }
        
    }
}

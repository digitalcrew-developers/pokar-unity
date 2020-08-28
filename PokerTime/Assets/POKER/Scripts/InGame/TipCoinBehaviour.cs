using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TipCoinBehaviour : MonoBehaviour
{
    RectTransform faceButton;
   
    Vector3 newPos = new Vector3(0, 1100, 0);
    
    private Vector3 buttonVelocity = Vector3.zero;
   
    private float smoothTime = 0.3f;

   
    void Start()
    {
       
        faceButton = this.transform.GetComponent<RectTransform>();
    }

    void Update()
    {
        
        if (faceButton.localPosition.y < 1090.0f)
        {
            faceButton.localPosition = Vector3.SmoothDamp(faceButton.localPosition, newPos, ref buttonVelocity, smoothTime);
        }
        else {
            Destroy(this.gameObject);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TipKissBehaviour : MonoBehaviour
{
    RectTransform faceButton;

    Vector3 newPos = new Vector3(0, -1100, 0);

    private Vector3 buttonVelocity = Vector3.zero;

    private float smoothTime = 0.3f;

    public float scalingFactor = 0.1f;

    Image img;

    void Start()
    {
        img= this.transform.GetChild(0).GetComponent<Image>();
        //Get the RectTransform component
        faceButton = this.transform.GetComponent<RectTransform>();
    }

    bool isfade=false;
    void Update()
    {
        if (faceButton.localPosition.y > -1090.0f)
        {
           faceButton.localPosition = Vector3.SmoothDamp(faceButton.localPosition, newPos, ref buttonVelocity, smoothTime);
        }
        else
        {
            transform.localScale = new Vector3(transform.localScale.x + transform.localScale.x * scalingFactor * Time.deltaTime, transform.localScale.y + transform.localScale.y * scalingFactor * Time.deltaTime, transform.localScale.z + transform.localScale.z * scalingFactor * Time.deltaTime);
            //

            isfade = true;
            if (transform.localScale.x > 5.0f)
            {
                Destroy(this.gameObject);
            }
        }
    }

    private void FixedUpdate()
    {
        if (isfade)
        {

            float val = img.color.a;
                // set color with i as alpha
                img.color = new Color(1, 1, 1, img.color.a-0.02f);
                //yield return null;
          
            }
        }
}

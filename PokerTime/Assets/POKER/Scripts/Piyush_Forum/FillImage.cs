using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FillImage : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (this.GetComponent<Image>().fillAmount == 0.9f)
        {
            this.GetComponent<Image>().fillAmount += 1.0f;
        }
    }
}

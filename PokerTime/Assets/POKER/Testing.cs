using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        //Debug.Log("Enable "+transform.parent.parent.parent.name);
    }

    private void OnDisable()
    {
        //Debug.Log("Disable "+transform.parent.parent.parent.name);
    }
}

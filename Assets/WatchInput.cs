using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WatchInput : MonoBehaviour
{
    public GameObject outputOb;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        outputOb.GetComponent<TextMesh>().text = $"{Input.GetAxisRaw("Horizontal")}" +", "+  $"{Input.GetAxisRaw("Vertical")}";
    }
}

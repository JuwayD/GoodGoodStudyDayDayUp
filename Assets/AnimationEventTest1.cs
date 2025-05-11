using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventTest1 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
public void PrintEvent(string s)
    {
        Debug.Log("PrintEvent: " + s + " called at: " + Time.time);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}

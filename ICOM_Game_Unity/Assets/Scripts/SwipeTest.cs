using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeTest : MonoBehaviour
{
    InputManager inputManager = InputManager.Instance;


    // Start is called before the first frame update
    void Start()
    {
        //inputManager.swipePerformed += context => { Test(); };
    }

    private void Test()
    {
        //Debug.Log("Swipe Recived");
    }
}

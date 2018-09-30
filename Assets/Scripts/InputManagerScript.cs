using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManagerScript : MonoBehaviour {
    public InitScript init;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {    
        if (Input.GetKeyDown(KeyCode.UpArrow))
		{
            init.OnGuessUpPressed();
        } 
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            init.OnGuessDownPressed();
        }
        else if(Input.GetKeyDown(KeyCode.Space))
        {
            init.NextPrice();
        }
	}
}

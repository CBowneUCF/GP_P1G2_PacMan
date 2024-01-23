using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerInputScript : Singleton<ControllerInputScript>
{
    public Vector2 controlDirection;
    
    
    void Update()
    {
        bool upPressed = false;
        bool downPressed = false;
        bool leftPressed = false;
        bool rightPressed = false;
        
        upPressed = Input.GetKey(KeyCode.UpArrow);
        downPressed = Input.GetKey(KeyCode.DownArrow);
        rightPressed = Input.GetKey(KeyCode.RightArrow);
        leftPressed = Input.GetKey(KeyCode.LeftArrow);
        
        controlDirection.y = upPressed ? 1 : downPressed ? -1: 0;
        controlDirection.x = rightPressed ? 1 : leftPressed ? -1: 0;
    }
    

}

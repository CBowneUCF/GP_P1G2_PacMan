using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerInputScript : Singleton<ControllerInputScript>
{
    public Vector2 controlDirection;
    [SerializeField] GameplayManagerScript manager;
    InputSystem input;

    protected override void OnAwake()
    {
        input = new InputSystem();
        input.Enable();
        //manager = GameplayManagerScript.instance;
    }

    void Update()
    {
        NewInput();
    }
    
    void NewInput()
    {
        controlDirection = input.Main.Movement.ReadValue<Vector2>();

        if(controlDirection.x != 0) controlDirection.x = Mathf.Sign(controlDirection.x);
        if(controlDirection.y != 0) controlDirection.y = Mathf.Sign(controlDirection.y);

        if (input.Main.Pause.WasPressedThisFrame()) manager.PauseMenuToggle();
        if (input.Main.InstaQuit.WasPressedThisFrame()) manager.EndGame();
    }


    void OldInput()
    {
        bool upPressed = false;
        bool downPressed = false;
        bool leftPressed = false;
        bool rightPressed = false;

        upPressed = Input.GetKey(KeyCode.UpArrow);
        downPressed = Input.GetKey(KeyCode.DownArrow);
        rightPressed = Input.GetKey(KeyCode.RightArrow);
        leftPressed = Input.GetKey(KeyCode.LeftArrow);

        controlDirection.y = upPressed ? 1 : downPressed ? -1 : 0;
        controlDirection.x = rightPressed ? 1 : leftPressed ? -1 : 0;

        if (Input.GetKeyDown(KeyCode.Escape)) manager.PauseMenuToggle();
        if (Input.GetKey(KeyCode.Alpha3)) manager.EndGame();
    }

}

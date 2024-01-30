using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterScript : MonoBehaviour
{
    
    ControllerInputScript input;
    Rigidbody2D rb;
    new Transform transform;
    [SerializeField] float speed;

    [SerializeField] private Vector2 levelSize;

    
    
    void Awake()
    {
        input = ControllerInputScript.instance;    
        rb = GetComponent<Rigidbody2D>();
        transform = base.transform;
    }
    
    

    
    void FixedUpdate()
    {
        if(!isPaused) rb.MovePosition((Vector2)transform.position + input.controlDirection*speed*Time.deltaTime);
        
        if(Mathf.Abs(transform.position.x) > levelSize.x) transform.position = new Vector3(Mathf.Clamp(-transform.position.x, -levelSize.x, levelSize.x), transform.position.y);
        if(Mathf.Abs(transform.position.y) > levelSize.y) transform.position = new Vector3(transform.position.x, Mathf.Clamp(-transform.position.y, -levelSize.y, levelSize.y));
    
    }

    public void SetLevelSize(Vector2 size) => levelSize = size;

    bool isPaused;
    public void SetPause(bool pause = true)
    {
        isPaused = pause;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterScript : MonoBehaviour
{
    
    ControllerInputScript input;
    Rigidbody2D rb;
    new Transform transform;
    [SerializeField] float speed;
    
    void Awake()
    {
        input = ControllerInputScript.instance;    
        rb = GetComponent<Rigidbody2D>();
        transform = base.transform;
    }
    
    
    void Update()
    {
        
    }
    
    void FixedUpdate()
    {
        rb.MovePosition((Vector2)transform.position + input.controlDirection*speed*Time.deltaTime);
    }
}

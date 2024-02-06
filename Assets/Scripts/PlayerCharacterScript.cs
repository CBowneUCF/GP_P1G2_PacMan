using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterScript : MonoBehaviour
{
    
    ControllerInputScript input;
    Rigidbody2D rb;
    new Transform transform;
    [SerializeField] float speed;
    Collider2D coll;

    [SerializeField] private Vector2 levelSize;
    public Vector2 currentDirection;

    Animator anim;
    public bool animationCallback;
     
    
    void Awake()
    {
        input = ControllerInputScript.instance;    
        rb = GetComponent<Rigidbody2D>();
        transform = base.transform;
        anim= GetComponentInChildren<Animator>(); 
        coll= GetComponent<Collider2D>();
        anim.Play("Base.Walking");
    }
    
    

    
    void FixedUpdate()
    {
        if(!isPaused) rb.MovePosition((Vector2)transform.position + input.controlDirection*speed*Time.deltaTime);
        currentDirection = input.controlDirection;

        anim.SetFloat("Speed", (currentDirection != new Vector2(0,0)? 1 : 0));
        if(currentDirection != new Vector2(0, 0))
        {
            anim.SetFloat("X", currentDirection.x);
            anim.SetFloat("Y", currentDirection.y);
        }

        if(Mathf.Abs(transform.position.x) > levelSize.x) transform.position = new Vector3(Mathf.Clamp(-transform.position.x, -levelSize.x, levelSize.x), transform.position.y);
        if(Mathf.Abs(transform.position.y) > levelSize.y) transform.position = new Vector3(transform.position.x, Mathf.Clamp(-transform.position.y, -levelSize.y, levelSize.y));
    
    }

    public void SetLevelSize(Vector2 size) => levelSize = size;

    bool isPaused;
    public void SetPause(bool pause = true)
    {
        isPaused = pause;
    }


    public void DoDeathAnimation() { anim.Play("Major.Death"); }
    public void DoWinAnimation() { anim.Play("Major.Win"); }
    public void EndMajorAnimation() { anim.Play("Major.Neutral");}

    public void AnimationCallback()=> animationCallback = true;









}

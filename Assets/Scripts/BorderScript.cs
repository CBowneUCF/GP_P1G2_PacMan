using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BorderScript : MonoBehaviour
{

public Vector2 size; 

public bool vertical;

void OnTriggerEnter2D(Collider2D other)
{
    PlayerCharacterScript player = other.gameObject.GetComponent<PlayerCharacterScript>();
    if(player) player.transform.position = new Vector3(player.transform.position.x * (vertical? 1:-1), player.transform.position.y * (vertical? -1:1));
}


}

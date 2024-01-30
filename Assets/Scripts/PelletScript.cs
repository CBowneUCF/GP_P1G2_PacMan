using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PelletScript : MonoBehaviour
{

    public bool isPower;
    void Awake()
    {
        LevelManagerScript.instance.PelletAdd();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.GetComponent<PlayerCharacterScript>()) {
        LevelManagerScript.instance.PelletCollect();
        LevelManagerScript.instance.PowerPelletCollect();
        gameObject.SetActive(false);
        }
    }

}

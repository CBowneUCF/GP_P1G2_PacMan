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
        if(isPower)LevelManagerScript.instance.PowerPelletCollect();
        LevelManagerScript.instance.PelletCollect();
        gameObject.SetActive(false);
        }
    }

}

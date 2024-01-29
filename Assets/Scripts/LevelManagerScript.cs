using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManagerScript : Singleton<LevelManagerScript>
{

    public int levelID;

    public int pelletsLeft;

    public PlayerCharacterScript player;
    public EnemyEntityScript[] enemies;


    protected override void OnAwake() {
    LevelInit();

    }


    void LevelInit(){
        GameObject pellets = GameObject.FindGameObjectsWithTag("Pellets")[0];
        pellets.transform.GetChild(0).gameObject.SetActive(true);
    }

    public void PelletAdd(){  
        pelletsLeft++;

    }

    public void PelletCollect(){  
        pelletsLeft--;
        if(pelletsLeft == 0)Win();

    }

    void Win(){  
    Debug.Log("WIN");

    }

    public void PauseLevel(bool pause = true)
    {
        player.enabled = pause;
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].enabled = pause;
        }
    }

}

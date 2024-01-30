using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManagerScript : Singleton<LevelManagerScript>
{

    public int levelID;

    public int pelletsLeft;

    public PlayerCharacterScript player;
    public EnemyEntityScript[] enemies;
    GameplayManagerScript gameplayManager;
    public int points;
    public int pointsPerPellet;


    Vector2 playerStartPos;
    List<Vector2> enemyStartPos = new List<Vector2>();

    public float roundTimer;


    protected override void OnAwake() {
        gameplayManager = GameplayManagerScript.instance;
        gameplayManager.levelMan = this;
        GameObject pellets = GameObject.FindGameObjectsWithTag("Pellets")[0];
        pellets.transform.GetChild(0).gameObject.SetActive(true);
        playerStartPos = player.transform.position;
        for (int i = 0; i < enemies.Length; i++) enemyStartPos.Add(enemies[i].transform.position);
    }


    public void PelletAdd(){  
        pelletsLeft++;

    }

    public void PelletCollect(){  
        pelletsLeft--;
        points += pointsPerPellet;
        if (pelletsLeft == 0)Win();

    }
    public void PowerPelletCollect()
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].MakeVulnerable();
        }
    }

    void Win(){  
    Debug.Log("WIN");

    }

    public void PauseLevel(bool pause = true)
    {
        player.SetPause(pause);
        for (int i = 0; i < enemies.Length; i++) enemies[i].SetPause(pause);
    }
    public void PauseGame(bool pause = true)
    {
        gameplayManager.isPaused = pause;
        PauseLevel(pause);
    }


    public void Death()
    {
        if (--gameplayManager.currentLifeCount == 0)
        {
            gameplayManager.GameOver();
        }
        else
        {
            player.transform.position = playerStartPos;
            for (int i = 0; i < enemies.Length; i++)
            {
                enemies[i].transform.position = enemyStartPos[i];
                enemies[i].stateGenerating.EnterState();
            }
        }
    }


}

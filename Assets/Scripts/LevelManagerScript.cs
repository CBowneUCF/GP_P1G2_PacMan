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
    public int pointsPerPellet;
    public GameObject pellets;

    Vector2 playerStartPos;
    List<Vector2> enemyStartPos = new List<Vector2>();

    public float roundTimer;
    public float scatterChaseSwitcher = 4;
    bool makeChase = true;

    AudioSource audioSource;
    public AudioClip pelletClip;
    public AudioClip starClip;


    protected override void OnAwake() {
        gameplayManager = GameplayManagerScript.instance;
        gameplayManager.levelMan = this;
        pellets.SetActive(true);
        playerStartPos = player.transform.position;
        for (int i = 0; i < enemies.Length; i++)
        {
            enemyStartPos.Add(enemies[i].transform.position);
            
        }
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();
    }

    private void Update()
    {
        if (!gameplayManager.isPaused) roundTimer += Time.deltaTime;
        if(roundTimer >= scatterChaseSwitcher * (makeChase ? 1 : 4))
        {
            roundTimer = 0;
            for (int i = 0; i < enemies.Length; i++) enemies[i].ScatterChaseSwitch(makeChase);
            makeChase = !makeChase;
        }
    }


    public void PelletAdd(){  
        pelletsLeft++;

    }

    public void PelletCollect(){  
        pelletsLeft--;
        gameplayManager.points += pointsPerPellet;
        audioSource.PlayOneShot(pelletClip);
        if (pelletsLeft == 0)Win();

    }
    public void PowerPelletCollect()
    {
        audioSource.PlayOneShot(starClip);
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].MakeVulnerable();
        }
    }


    public void PauseLevel(bool pause = true)
    {
        player.SetPause(pause);
        for (int i = 0; i < enemies.Length; i++) enemies[i].SetPause(pause);
        if (pause) audioSource.Pause();
        else audioSource.UnPause();
    }
    public void PauseGame(bool pause = true)
    {
        gameplayManager.isPaused = pause;
        PauseLevel(pause);
        if (pause) audioSource.Pause();
        else audioSource.UnPause();
    }


    public void Death() => new Coroutine(DeathCO(), this);
    IEnumerator DeathCO()
    {
        gameplayManager.PauseGame(true);
        yield return WaitFor.Seconds(0.6f);

        player.DoDeathAnimation();

        while (!player.animationCallback) yield return null;
        player.animationCallback = false;


        if (--gameplayManager.currentLifeCount == 0)
        {
            player.gameObject.SetActive(false);
            gameplayManager.GameOver();
        }
        else
        {
            player.transform.position = playerStartPos;
            for (int i = 0; i < enemies.Length; i++)
            {
                enemies[i].transform.position = enemyStartPos[i];
                enemies[i].Begin();
                roundTimer = 0;
                gameplayManager.PauseGame(false);
                player.EndMajorAnimation();
            }
        }
        yield return null;

    }

    void Win() => new Coroutine(WinCO(),  this);
    IEnumerator WinCO()
    {
        gameplayManager.PauseGame(true);
        yield return WaitFor.Seconds(0.6f);

        player.DoWinAnimation();

        while (!player.animationCallback) yield return null;
        player.animationCallback = false;

        player.EndMajorAnimation();
        gameplayManager.NextLevel();

        yield return null;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayManagerScript : Singleton<GameplayManagerScript>
{


    public bool debug;
    public int points;



    protected override void OnAwake()
    {
        MainMenuToggle(true);     

    }

    private void Update()
    {
        if (!debug) return;
        //Also for Test Purposes
        //if (Input.GetKeyDown(KeyCode.P) && !inMainMenu && !isGameOver && !isInPauseMenu) PauseMenuToggle();
        if (inMainMenu)
        {
            if (Input.GetKeyDown(KeyCode.Q)) BeginGame();
            if (Input.GetKeyDown(KeyCode.E)) EndGame();
        }
        else if (isInPauseMenu)
        {
            if (Input.GetKeyDown(KeyCode.Q)) ReturnToMenu();
            if (Input.GetKeyDown(KeyCode.R)) EndGame();
        }
        else if (isGameOver)
        {
            if (Input.GetKeyDown(KeyCode.R)) BeginGame();
            if (Input.GetKeyDown(KeyCode.E)) ReturnToMenu();
        }
        if (Input.GetKeyDown(KeyCode.L)) GameOver();




    }


    public bool inMainMenu;
    public GameObject mainMenuObject;
    public void MainMenuToggle(bool On)
    {
        inMainMenu = On;
        mainMenuObject.SetActive(On);
    }

    public bool inLevel;
    public int currentLevelID = -1;
    public string[] levelIDs;
    public LevelManagerScript levelMan;

    void LoadUnloadLevel(int levelID)
    {
        if(currentLevelID > -1)
        {
            SceneManager.UnloadSceneAsync(levelIDs[currentLevelID]);
            levelMan = null;
            currentLevelID = -1;
        }
        inLevel = false;
        if (levelID == -1) return;

        SceneManager.LoadScene(levelIDs[levelID], LoadSceneMode.Additive);
        //levelMan = LevelManagerScript.instance;
        //levelMan = FindObjectOfType<LevelManagerScript>();
        //if (levelMan == null) Debug.LogWarning("What");
        currentLevelID = levelID;
    }

    public void BeginGame()
    {
        currentLifeCount = startingLifeCount;
        LoadUnloadLevel(0);
        inMainMenu = false;
        isGameOver = false;
        mainMenuObject.SetActive(false);
        gameOverObject.SetActive(false);
    }



    public bool isPaused;
    public bool isInPauseMenu;
    public GameObject pauseMenuObject;

    public void PauseMenuToggle()
    {
        if (inMainMenu || isGameOver) return;

        if (!inLevel || isGameOver || inMainMenu)
        if (!isInPauseMenu)
        {
            isInPauseMenu = true;
            PauseGame(true);
            pauseMenuObject.SetActive(true);
        }
        else
        {
            isInPauseMenu = false;
            PauseGame(false); 
            pauseMenuObject.SetActive(false);
        }
    }


    void PauseGame(bool pausing = true)
    {
        levelMan.PauseLevel(pausing);
        isPaused = pausing;
    }





    public bool isGameOver;
    public int startingLifeCount = 3;
    public int currentLifeCount;
    public GameObject gameOverObject;

    public void GameOver()
    {
        isGameOver = true;
        gameOverObject.SetActive(true);
        PauseGame(true);
    }



    public void ReturnToMenu()
    {
        isInPauseMenu = false;
        PauseGame(false);
        pauseMenuObject.SetActive(false);
        isGameOver = false;
        gameOverObject.SetActive(false);

        MainMenuToggle(true);

        LoadUnloadLevel(-1);

    }
    public void EndGame()
    {
        //K I L L
        Application.Quit();
        EditorApplication.isPlaying = false;
    }


}

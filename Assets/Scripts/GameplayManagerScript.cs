using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayManagerScript : Singleton<GameplayManagerScript>
{
    public bool inMainMenu;
    public bool inLevel;
    public bool isPaused;
    public bool isInPauseMenu;
    public bool isLoading;
    public bool isGameOver;
    public int startingLifeCount;
    public int currentLifeCount;
    public string[] levelIDs;
    public int currentLevelID;

    protected override void OnAwake()
    {
        //For Test Purposes.
        currentLifeCount = startingLifeCount;
        currentLevelID = 0;
        LoadUnloadLevel();

    }












    void LoadUnloadLevel(bool load = true)
    {
        if (inLevel && load)
        {
            Debug.LogError("You're already in a level. Unload first.");
            return;
        }
        string levelToLoad = levelIDs[currentLevelID];
        if (load)
        {
            SceneManager.LoadScene(levelToLoad, LoadSceneMode.Additive);
            level = FindObjectOfType<LevelManagerScript>();
        }
        else
        {
            SceneManager.UnloadSceneAsync(levelToLoad);
            level = null;
        }
    }
    LevelManagerScript level;


    public void PauseMenuToggle()
    {
        if(!inLevel || isLoading || isGameOver || inMainMenu)
        if (!isInPauseMenu)
        {
            isPaused = true;
            PauseGame(true); 
            //Enable Pause Menu
        }
        else
        {
            isPaused = false;
            //Disable Pause Menu
            PauseGame(false); 
        }
    }


    void PauseGame(bool pausing = true)
    {
        level.PauseLevel(pausing);
    }




    public void PlayerDie()
    {
        PauseGame(true);
        if (--currentLifeCount == 0)
        {
            //Enable Gameover Screen
        }
    }





    public void Retry()
    {
        //Disable Game Over menu.
        //Unload current level and reload first level.
    }
    public void ReturnToMenu()
    {
        //Disable Pause and Game Over Menus.
        //Enable Main Menu
        //Unload Level
    }
    public void EndGame()
    {
        //K I L L
        Application.Quit();
    }


}

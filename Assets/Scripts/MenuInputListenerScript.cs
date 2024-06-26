using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuInputListenerScript : MonoBehaviour
{
    public GameplayManagerScript g;

    public void BeginGame() => g.BeginGame();
    public void EndGame() => g.EndGame();
    public void ReturnMenu() => g.ReturnToMenu();

    public void UnPause() => g.PauseMenuToggle();
}

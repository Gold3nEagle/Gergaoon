using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{

    bool pauseGame; 

    public void PauseGame()
    {
        pauseGame = !pauseGame;
        if (pauseGame)
        {
            Time.timeScale = 0;
        } else
        {
            Time.timeScale = 1;
        }
    }

}

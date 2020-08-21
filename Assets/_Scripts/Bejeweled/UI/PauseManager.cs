using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject pausePanel;
    private Board board;
    public bool paused = false;

    // Start is called before the first frame update
    void Start()
    {
        board = GameObject.FindGameObjectWithTag("Board").GetComponent<Board>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if(paused && !pausePanel.activeInHierarchy)
        {
            pausePanel.SetActive(true);
            Debug.Log("PauseManager Pause");
            board.currentState = GameStatus.pause;
        }

        if(!paused && pausePanel.activeInHierarchy)
        {
            pausePanel.SetActive(false);
            board.currentState = GameStatus.move;
        }

    }

    public void PauseGame()
    {
        paused = !paused;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GameType
{
    Moves,
    Time
}

[System.Serializable]
public class EndGameRequirements
{
    public GameType gameType;
    public int counterValue;
}

public class EndGameManager : MonoBehaviour
{
    public GameObject movesLabel, timeLabel, youWinPanel, tryAgainPanel;
    public Text counter;
    public EndGameRequirements requirements;
    public int currentCounterValue;
    private float timerSeconds; 
    private Board board;

    Overworld overWorld;


    // Start is called before the first frame update
    void Start()
    { 
        board = FindObjectOfType<Board>();
        SetGameType();
        Setup();
    }

    void SetGameType()
    {
        if (board.world != null)
        {
            if (board.level < board.world.levels.Length)
            {
                if (board.world.levels[board.level] != null)
                {
                    requirements = board.world.levels[board.level].endGameRequirements;
                }
            }
        }
    }


    void Setup()
    {
        currentCounterValue = requirements.counterValue;
        if(requirements.gameType == GameType.Moves)
        {
            movesLabel.SetActive(true);
            timeLabel.SetActive(false);
        }
        else
        {
            timerSeconds = 1;
            movesLabel.SetActive(false);
            timeLabel.SetActive(true);
        }
        counter.text = currentCounterValue.ToString();
    }

    public void DecreaseCounterValue()
    {
        if (board.currentState != GameState.pause)
        {
            currentCounterValue--;
            counter.text = currentCounterValue.ToString();
            if (currentCounterValue <= 0)
            {
                LoseGame();
            }
        }
    } 

    public void WinGame()
    {
        youWinPanel.SetActive(true);
        board.currentState = GameState.win;
        currentCounterValue = 0;
        counter.text = currentCounterValue.ToString();
        AnimationController animationController = FindObjectOfType<AnimationController>();
        animationController.GameOver();
    }

    public void LoseGame()
    {
        tryAgainPanel.SetActive(true);
        board.currentState = GameState.lose;
        Debug.Log("Time is up LOSO!");
        currentCounterValue = 0;
        counter.text = currentCounterValue.ToString();
        AnimationController animationController = FindObjectOfType<AnimationController>();
        animationController.GameOver();
        overWorld = FindObjectOfType<Overworld>();
    }

    public void IncreaseCounter()
    {
        if (overWorld.GetTotalCandy() >= 100)
        {
            int tempTotalCandy = overWorld.GetTotalCandy();
            tempTotalCandy -= 100;
            PlayerPrefs.SetInt("totalCandy", tempTotalCandy);
            overWorld.DisplayTotalCandy();

            currentCounterValue = 10;
            counter.text = currentCounterValue.ToString();
            AnimationController animationController = FindObjectOfType<AnimationController>();
            animationController.Restart();
            tryAgainPanel.SetActive(false);
        } else
        {
            Debug.Log("Not Enough Candy!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(requirements.gameType == GameType.Time && currentCounterValue > 0)
        {
            timerSeconds -= Time.deltaTime;
            if (timerSeconds <= 0)
            {
                //Debug.Log("Time is up LOSER!");
                DecreaseCounterValue();
                timerSeconds = 1;
            }
            
        }
    }
}

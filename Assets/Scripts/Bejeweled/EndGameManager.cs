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
    public GameObject movesLabel, timeLabel, youWinPanel, tryAgainPanel, sparksEffect, movesFlash;
    public Text counter;
    public EndGameRequirements requirements;
    public int currentCounterValue;
    private float timerSeconds; 
    private Board board;
    public MusicController musicController;
    public SfxController sfx;
    public bool doubleRewards = false;
    public AdsScript ads;

    int matchesCounter;
    Overworld overWorld;
    ScoreManager scoreManager;


    // Start is called before the first frame update
    void Start()
    {
        scoreManager = FindObjectOfType<ScoreManager>();
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

            if(currentCounterValue == 5)
            {
                movesFlash.SetActive(true);
            }

            if (currentCounterValue <= 0 && board.currentState != GameState.win)
            {
                LoseGame();
            }
        }
    } 

    public void WinGame()
    {
        sparksEffect.SetActive(true);
        musicController.PlayCheeringSound();

        StartCoroutine(CheckForEnd());
    }

    IEnumerator CheckForEnd()
    {
        yield return new WaitForSeconds(1f);

        if (board.CheckForMatches())
        {
            StartCoroutine(CheckForEnd());
            matchesCounter = 0;
        } else
        {
            matchesCounter++;
            if(matchesCounter >= 5)
            {
                StartCoroutine(ShowWinPanel());
            } else
            {
                StartCoroutine(CheckForEnd());
            }
        }

    }

    IEnumerator  ShowWinPanel()
    {
        StartCoroutine(CalculateScore());
         
        yield return new WaitForSeconds(3f);
        youWinPanel.SetActive(true);
        board.currentState = GameState.win;
        doubleRewards = true;
    }

    IEnumerator CalculateScore()
    { 
        for (int i = 0; i < currentCounterValue; i++)
        { 
            yield return new WaitForSeconds(0.4f);
            if (currentCounterValue > 0)
            {
                scoreManager.IncreaseScore(50);
                sfx.PlayScoreSound();
                currentCounterValue--;
                counter.text = currentCounterValue.ToString();
            }
        }
        counter.text = "0";
        int finalScore = scoreManager.GetScore();
        int savedScore = PlayerPrefs.GetInt("totalCandy");
        savedScore = savedScore + finalScore;
        PlayerPrefs.SetInt("totalCandy", savedScore);

        yield return null;
    }

    public void LoseGame()
    {
        tryAgainPanel.SetActive(true);
        board.currentState = GameState.lose;
        overWorld = FindObjectOfType<Overworld>();
        Debug.Log("Time is up LOSO!");
        currentCounterValue = 0;
        counter.text = currentCounterValue.ToString();
        AnimationController animationController = GameObject.Find("FadePanel").GetComponent<AnimationController>();
        animationController.GameOver();

    }

    public void IncreaseCounter()
    {
        if (overWorld.GetTotalCandy() >= 10000)
        {
            int tempTotalCandy = overWorld.GetTotalCandy();
            tempTotalCandy -= 10000;
            PlayerPrefs.SetInt("totalCandy", tempTotalCandy);
            overWorld.DisplayTotalCandy();

            currentCounterValue = 10;
            counter.text = currentCounterValue.ToString();
            AnimationController animationController = GameObject.Find("FadePanel").GetComponent<AnimationController>();
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

    public void WatchedRewarded()
    {
        GameObject.Find("DoubleRewardsButton").SetActive(false);
    }

}

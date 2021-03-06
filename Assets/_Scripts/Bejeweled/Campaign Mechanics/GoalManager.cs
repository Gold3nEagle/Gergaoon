﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BlankGoal
{
    public Sprite goalSprite;
    public GoalKind goalKind;

    [Header("For Candy Goals")]
    public int numberNeeded;
    public int numberCollected;
    public string matchValue;

    [Header("For Score Goals")]
    public int scoreNeeded;
}

public enum GoalKind
{
    candyGoal,
    scoreGoal
}

public class GoalManager : MonoBehaviour
{
    public BlankGoal[] levelGoals;
    public List<GoalPanel> currentGoals = new List<GoalPanel>();
    public GameObject goalPrefab, goalIntroParent, goalGameParent, scoreGoalPrefab;
    private EndGameManager endGame;
    private Board board;

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        endGame = FindObjectOfType<EndGameManager>();
        GetGoals();
        SetupGoals();
    }

    void GetGoals()
    {
        if (board != null)
        {
            if (board.world != null)
            {
                if (board.level < board.world.levels.Length)
                {
                    if (board.world.levels[board.level] != null)
                    {
                        levelGoals = board.world.levels[board.level].levelGoals;

                        if (levelGoals[0].goalKind == GoalKind.candyGoal)
                        {
                            for (int i = 0; i < levelGoals.Length; i++)
                            {
                                levelGoals[i].numberCollected = 0;
                            }
                        }
                        else
                        {
                            levelGoals = board.world.levels[board.level].levelGoals;
                        }
                    }
                }
            }
        }
    }

    void SetupGoals()
    {
        if (levelGoals[0].goalKind == GoalKind.candyGoal)
        {
            for (int i = 0; i < levelGoals.Length; i++)
            {
                //Create new goal panel at the goalIntroParent position
                GameObject goal = Instantiate(goalPrefab, goalIntroParent.transform.position, Quaternion.identity);
                goal.transform.SetParent(goalIntroParent.transform);
                //Set the image and text of the goal
                GoalPanel panel = goal.GetComponent<GoalPanel>();
                panel.thisSprite = levelGoals[i].goalSprite;
                panel.thisString = levelGoals[i].numberNeeded.ToString();

                //Create a new Goal Panel at the goalGame position
                GameObject gameGoal = Instantiate(goalPrefab, goalGameParent.transform.position, Quaternion.identity);
                gameGoal.transform.SetParent(goalGameParent.transform);
                panel = gameGoal.GetComponent<GoalPanel>();
                currentGoals.Add(panel);
                panel.thisSprite = levelGoals[i].goalSprite;
                panel.thisString = levelGoals[i].numberNeeded.ToString();
            }
        }
        else
        {
            GameObject scoreGoal = Instantiate(scoreGoalPrefab, goalIntroParent.transform.position, Quaternion.identity);
            scoreGoal.transform.SetParent(goalIntroParent.transform);

            GoalPanel panel = scoreGoal.GetComponent<GoalPanel>();
            panel.thisString = levelGoals[0].scoreNeeded.ToString();

            GameObject gameGoal = Instantiate(scoreGoalPrefab, goalGameParent.transform.position, Quaternion.identity);
            gameGoal.transform.SetParent(goalGameParent.transform);
            panel = gameGoal.GetComponent<GoalPanel>();
            currentGoals.Add(panel);
            panel.thisString = levelGoals[0].scoreNeeded.ToString();
        }


    }

    public void UpdateCandyGoals()
    {
        int goalsCompleted = 0;

        for (int i = 0; i < levelGoals.Length; i++)
        {
            //currentGoals[i].thisText.text = levelGoals[i].numberCollected + " / " + levelGoals[i].numberNeeded;
            currentGoals[i].thisText.text = (levelGoals[i].numberNeeded - levelGoals[i].numberCollected).ToString();
            if (levelGoals[i].numberCollected >= levelGoals[i].numberNeeded)
            {
                goalsCompleted++;
                //currentGoals[i].thisText.text = levelGoals[i].numberNeeded + " / " + levelGoals[i].numberNeeded;
                currentGoals[i].thisText.enabled = false;
                currentGoals[i].finishedSprite.SetActive(true);
            }
        }
        if (goalsCompleted >= levelGoals.Length)
        {
            endGame.WinGame();
        }
    }

    public void CompareGoal(string goalToCompare)
    {
        for (int i = 0; i < levelGoals.Length; i++)
        {
            if (goalToCompare == levelGoals[i].matchValue)
            {
                levelGoals[i].numberCollected++;
            }
        }
    }

    public void UpdateScoreGoal()
    {
        endGame.WinGame();
    }

}

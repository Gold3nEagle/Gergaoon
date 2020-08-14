using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessDamageDestroy : MonoBehaviour
{

    public EndlessBoard board;

    //Destroys matches and shows particles.
    private void DestroyMatchesAt(int column, int row)
    {
        if (board.allDots[column, row].GetComponent<EndlessDot>().isMatched)
        {
 
            if (board.goalManager != null)
            {
                if (board.world.levels[board.level].levelGoals[0].goalKind == GoalKind.candyGoal)
                {
                    board.goalManager.CompareGoal(board.allDots[column, row].tag.ToString());
                    board.goalManager.UpdateCandyGoals();
                }
                else
                {
                    if (board.scoreManager.GetScore() >= board.world.levels[board.level].levelGoals[0].scoreNeeded)
                    {
                        //Debug.Log(goalManager.levelGoals[0].scoreNeeded.ToString());
                        board.goalManager.UpdateScoreGoal();
                    }
                }
            }

            GameObject particle = Instantiate(board.destroyEffect, board.allDots[column, row].transform.position, Quaternion.identity);
            board.musicController.PlayRandomDestroyNoise();

            Destroy(particle, .4f);
            Destroy(board.allDots[column, row]);
            board.scoreManager.IncreaseScore(board.basePieceValue * board.streakValue);
            board.allDots[column, row] = null;
        }
    }
  
    public void DestroyMatches()
    {
        //How many elements are in the matched pieces list from findmatches?
        if (board.findMatches.currentMatches.Count >= 4)
        {
            board.CheckToMakeBombs();
        }
        board.findMatches.currentMatches.Clear();

        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                if (board.allDots[i, j] != null)
                {
                    DestroyMatchesAt(i, j);
                }
            }
        }
        StartCoroutine(DecreaseRowCo());
    }

    public IEnumerator DecreaseRowCo()
    {
        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                // if the current spot isnt blank and is empty
                if (board.allDots[i, j] == null)
                {
                    //loop from the space above to the top of the column
                    for (int k = j + 1; k < board.height; k++)
                    {
                        //if a dot is found
                        if (board.allDots[i, k] != null)
                        {
                            //move that dot to this empty space
                            board.allDots[i, k].GetComponent<EndlessDot>().row = j;
                            //set that spot to be null
                            board.allDots[i, k] = null;
                            break;
                        }
                    }
                }
            }
        }
        yield return new WaitForSeconds(board.refillDelay * 0.5f);
        board.FillBoard();
    }
}

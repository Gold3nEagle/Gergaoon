using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDestroy : MonoBehaviour
{

    public Board board;

    //Destroys matches and shows particles.
    private void DestroyMatchesAt(int column, int row)
    {
        if (board.allDots[column, row].GetComponent<Dot>().isMatched)
        {
            //Does a breakable tile need to break?
            if (board.breakableTiles[column, row] != null)
            {
                board.breakableTiles[column, row].TakeDamage(1);
                if (board.breakableTiles[column, row].hitPoints <= 0)
                {
                    board.breakableTiles[column, row] = null;
                }
            }

            //Does a lock tile need to break?
            if (board.lockTiles[column, row] != null)
            {
                board.lockTiles[column, row].TakeDamage(1);
                if (board.lockTiles[column, row].hitPoints <= 0)
                {
                    board.lockTiles[column, row] = null;
                }
            }

            DamageConcrete(column, row);
            DamageSlime(column, row);
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

    private void DamageConcrete(int column, int row)
    {
        if (column > 0)
        {
            if (board.concreteTiles[column - 1, row])
            {
                board.concreteTiles[column - 1, row].TakeDamage(1);
                if (board.concreteTiles[column - 1, row].hitPoints <= 0)
                {
                    board.concreteTiles[column - 1, row] = null;
                }
            }
        }

        if (column < board.width - 1)
        {
            if (board.concreteTiles[column + 1, row])
            {
                board.concreteTiles[column + 1, row].TakeDamage(1);
                if (board.concreteTiles[column + 1, row].hitPoints <= 0)
                {
                    board.concreteTiles[column + 1, row] = null;
                }
            }
        }

        if (row > 0)
        {
            if (board.concreteTiles[column, row - 1])
            {
                board.concreteTiles[column, row - 1].TakeDamage(1);
                if (board.concreteTiles[column, row - 1].hitPoints <= 0)
                {
                    board.concreteTiles[column, row - 1] = null;
                }
            }
        }

        if (row < board.height - 1)
        {
            if (board.concreteTiles[column, row + 1])
            {
                board.concreteTiles[column, row + 1].TakeDamage(1);
                if (board.concreteTiles[column, row + 1].hitPoints <= 0)
                {
                    board.concreteTiles[column, row + 1] = null;
                }
            }
        }
    }

    private void DamageSlime(int column, int row)
    {
        if (column > 0)
        {
            if (board.slimeTiles[column - 1, row])
            {
                board.slimeTiles[column - 1, row].TakeDamage(1);
                if (board.slimeTiles[column - 1, row].hitPoints <= 0)
                {
                    board.slimeTiles[column - 1, row] = null;
                }
                board.makeSlime = false;
            }
        }

        if (column < board.width - 1)
        {
            if (board.slimeTiles[column + 1, row])
            {
                board.slimeTiles[column + 1, row].TakeDamage(1);
                if (board.slimeTiles[column + 1, row].hitPoints <= 0)
                {
                    board.slimeTiles[column + 1, row] = null;
                }
                board.makeSlime = false;
            }
        }

        if (row > 0)
        {
            if (board.slimeTiles[column, row - 1])
            {
                board.slimeTiles[column, row - 1].TakeDamage(1);
                if (board.slimeTiles[column, row - 1].hitPoints <= 0)
                {
                    board.slimeTiles[column, row - 1] = null;
                }
                board.makeSlime = false;
            }
        }

        if (row < board.height - 1)
        {
            if (board.slimeTiles[column, row + 1])
            {
                board.slimeTiles[column, row + 1].TakeDamage(1);
                if (board.slimeTiles[column, row + 1].hitPoints <= 0)
                {
                    board.slimeTiles[column, row + 1] = null;
                }
                board.makeSlime = false;
            }
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
                if (!board.blankSpaces[i, j] && board.allDots[i, j] == null && !board.concreteTiles[i, j] && !board.slimeTiles[i, j])
                {
                    //loop from the space above to the top of the column
                    for (int k = j + 1; k < board.height; k++)
                    {
                        //if a dot is found
                        if (board.allDots[i, k] != null)
                        {
                            //move that dot to this empty space
                            board.allDots[i, k].GetComponent<Dot>().row = j;
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

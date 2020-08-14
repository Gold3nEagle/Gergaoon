using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessHint : MonoBehaviour
{ 
    public GameObject hintParticle, currentHint;

    private float hintDelaySeconds;
    private EndlessBoard board;


    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<EndlessBoard>(); 
    }
     

    public void RequestHint()
    { 
        //Decrease Points/Time
            MarkHint(); 
    }

    List<GameObject> FindAllMatches()
    {
        List<GameObject> possibleMoves = new List<GameObject>();
        for (int i = 0; i < board.width; i++)
        {
            for (int j = 0; j < board.height; j++)
            {
                if (board.allDots[i, j] != null)
                {
                    if (i < board.width - 1)
                    {
                        if (board.SwitchAndCheck(i, j, Vector2.right))
                        {
                            possibleMoves.Add(board.allDots[i, j]);
                        }
                    }
                    if (j < board.height - 1)
                    {
                        if (board.SwitchAndCheck(i, j, Vector2.up))
                        {
                            possibleMoves.Add(board.allDots[i, j]);
                        }
                    }
                }
            }
        }
        return possibleMoves;
    }

    //Pick a match randomly
    GameObject PickOneRandomly()
    {
        List<GameObject> possibleMoves = new List<GameObject>();
        possibleMoves = FindAllMatches();
        if (possibleMoves.Count > 0)
        {
            int pieceToUsee = Random.Range(0, possibleMoves.Count);
            return possibleMoves[pieceToUsee];
        }

        return null;
    }

    //Show the hint on the scene
    private void MarkHint()
    {
        GameObject move = PickOneRandomly();
        if (move != null)
        {
            currentHint = Instantiate(hintParticle, move.transform.position, Quaternion.identity);
        }
    }

    public void DestroyHint()
    {
        if (currentHint != null)
        {
            Destroy(currentHint);
            currentHint = null; 
        }
    }

}

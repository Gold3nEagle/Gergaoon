using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 

public class EndlessBoard : MonoBehaviour
{

    [Header("Scriptable Objects")]
    public World world;
    public int level;


    public GameStatus currentState = GameStatus.move;
    public TileType[] boardLayout;
    public int height, width, offSet;

    [Header("Prefabs")]
    public GameObject tilePrefab, destroyEffect;
    public GameObject[] dots;
    public GameObject[,] allDots;

    public MatchType matchType;
    public EndlessDot currentDot;
    public int basePieceValue = 1;
    public int streakValue = 1;
    public float refillDelay = 0.5f;
    public int[] scoreGoals; 
 
    public EndlessMatches findMatches;
    public EndlessScore scoreManager;
    public MusicController musicController;
    public GoalManager goalManager;
    private BoardSetup boardSetup;
    private EndlessDamageDestroy damageDestroy;


    private void Awake()
    {
        level = 0;

        if (world.name == "Endless World")
        { 
                if (world.levels[level] != null)
                {
                    width = world.levels[level].width;
                    height = world.levels[level].height;
                    dots = world.levels[level].dots;
                    scoreGoals = world.levels[level].scoreGoals;
                    boardLayout = world.levels[level].boardLayout;
                } 
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        damageDestroy = FindObjectOfType<EndlessDamageDestroy>();
        boardSetup = FindObjectOfType<BoardSetup>();
        allDots = new GameObject[width, height];
        SetUp();
        currentState = GameStatus.pause;
    }


    public void SetUp()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector2 tempPosition = new Vector2(i, j + offSet);
                Vector2 tilePosition = new Vector2(i, j);
                GameObject backgroundTile = Instantiate(tilePrefab, tilePosition, Quaternion.identity) as GameObject;
                backgroundTile.transform.parent = this.transform;
                backgroundTile.name = "( " + i + ", " + j + " )";

                //Initializaing
                int dotToUse = UnityEngine.Random.Range(0, dots.Length);
                int maxIterations = 0;

                while (MatchesAt(i, j, dots[dotToUse]) && maxIterations < 100)
                {
                    dotToUse = UnityEngine.Random.Range(0, dots.Length);
                    maxIterations++;
                }
                maxIterations = 0;

                GameObject dot = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
                dot.GetComponent<EndlessDot>().row = j;
                dot.GetComponent<EndlessDot>().column = i;
                dot.transform.parent = this.transform;
                dot.name = "( " + i + ", " + j + " )";
            }
        }
    }

    public bool MatchesAt(int column, int row, GameObject piece)
    {
        if (column > 1 && row > 1)
        {
            if (allDots[column - 1, row] != null && allDots[column - 2, row] != null)
            {
                if (allDots[column - 1, row].tag == piece.tag && allDots[column - 2, row].tag == piece.tag)
                {
                    return true;
                }
            }

            if (allDots[column, row - 1] != null && allDots[column, row - 2] != null)
            {
                if (allDots[column, row - 1].tag == piece.tag && allDots[column, row - 2].tag == piece.tag)
                {
                    return true;
                }
            }
        }
        else if (column <= 1 || row <= 1)
        {
            if (row > 1)
            {
                if (allDots[column, row - 1] != null && allDots[column, row - 2] != null)
                {
                    if (allDots[column, row - 1].tag == piece.tag && allDots[column, row - 2].tag == piece.tag)
                    {
                        return true;
                    }
                }
            }

            if (column > 1)
            {
                if (allDots[column - 1, row] != null && allDots[column - 2, row] != null)
                {
                    if (allDots[column - 1, row].tag == piece.tag && allDots[column - 2, row].tag == piece.tag)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private MatchType ColumnOrRow()
    {
        //Make a copy of the current matches
        List<GameObject> matchCopy = findMatches.currentMatches as List<GameObject>;

        matchType.type = 0;
        matchType.color = "";

        //Cycle through all of match copy and decide if a bomb needs to be made
        for (int i = 0; i < matchCopy.Count; i++)
        {
            //Store this dot
            EndlessDot thisDot = matchCopy[i].GetComponent<EndlessDot>();
            string color = matchCopy[i].tag;

            int column = thisDot.column;
            int row = thisDot.row;
            int columnMatch = 0;
            int rowMatch = 0;
            for (int j = 0; j < matchCopy.Count; j++)
            {
                //Store next dot
                EndlessDot nextDot = matchCopy[j].GetComponent<EndlessDot>();
                if (nextDot == thisDot)
                {
                    continue;
                }
                if (nextDot.column == thisDot.column && nextDot.tag == color)
                {
                    columnMatch++;

                }

                if (nextDot.row == thisDot.row && nextDot.tag == color)
                {
                    rowMatch++;
                }
            }
            //return 3 if column or row match
            //return 2 if adjacent
            //return 1 if color bomb
            if (columnMatch == 4 || rowMatch == 4)
            {
                matchType.type = 1;
                matchType.color = color;
                return matchType;
            }
            else if (columnMatch == 2 && rowMatch == 2)
            {
                matchType.type = 2;
                matchType.color = color;
                return matchType;
            }
            else if (columnMatch == 3 || rowMatch == 3)
            {
                matchType.type = 3;
                matchType.color = color;
                return matchType;
            }
        }

        matchType.type = 0;
        matchType.color = "";
        return matchType;

    }

    public void CheckToMakeBombs()
    {
        //How many objects are in findMatches currentMatches?
        if (findMatches.currentMatches.Count > 3)
        {
            //What type of matches were done?
            MatchType typeOfMatch = ColumnOrRow();

            if (typeOfMatch.type == 1)
            {
                //Make a color bomb
                //is the current dot matched?
                if (currentDot != null && currentDot.isMatched && currentDot.tag == typeOfMatch.color)
                {
                    currentDot.isMatched = false;
                    currentDot.MakeColorBomb();
                }
                else
                {
                    if (currentDot.otherDot != null)
                    {
                        EndlessDot otherDot = currentDot.otherDot.GetComponent<EndlessDot>();
                        if (otherDot.isMatched && otherDot.tag == typeOfMatch.color)
                        {
                            otherDot.isMatched = false;
                            otherDot.MakeColorBomb();
                        }
                    }
                }
            }
            else if (typeOfMatch.type == 2)
            {
                //Make a adjacent bomb
                //is the current dot matched?
                if (currentDot != null && currentDot.isMatched && currentDot.tag == typeOfMatch.color)
                {

                    currentDot.isMatched = false;
                    currentDot.MakeAdjacentBomb();
                }
                else if (currentDot.otherDot != null)
                {
                    EndlessDot otherDot = currentDot.otherDot.GetComponent<EndlessDot>();
                    if (otherDot.isMatched && otherDot.tag == typeOfMatch.color)
                    {
                        otherDot.isMatched = false;
                        otherDot.MakeAdjacentBomb();
                    }
                }
            }
            else if (typeOfMatch.type == 3)
            {
                findMatches.CheckBombs(typeOfMatch);
            }
        }
    }


    public void BombRow(int row)
    {
 
    }

    public void BombColumn(int column)
    {
        for (int i = 0; i < width; i++)
        { 

        }
    } 

    private void RefillBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] == null)
                {
                    Vector2 tempPosition = new Vector2(i, j + offSet);
                    int dotToUse = UnityEngine.Random.Range(0, dots.Length);
                    int maxIterations = 0;
                    while (MatchesAt(i, j, dots[dotToUse]) && maxIterations < 100)
                    {
                        maxIterations++;
                        dotToUse = UnityEngine.Random.Range(0, dots.Length);
                    }
                    maxIterations = 0;

                    GameObject piece = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
                    allDots[i, j] = piece;
                    piece.GetComponent<EndlessDot>().row = j;
                    piece.GetComponent<EndlessDot>().column = i;
                }
            }
        }
    }


    private bool MatchesOnBoard()
    {
        findMatches.FindAllMatches();
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    if (allDots[i, j].GetComponent<EndlessDot>().isMatched)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public void FillBoard()
    {
        StartCoroutine(FillBoardCo());
    }

    private IEnumerator FillBoardCo()
    {

        yield return new WaitForSeconds(refillDelay);
        RefillBoard();

        while (MatchesOnBoard())
        {
            streakValue++;
            damageDestroy.DestroyMatches();
            yield break;
            //yield return new WaitForSeconds(refillDelay * 2);
        }
        currentDot = null;
        //yield return new WaitForSeconds(refillDelay ); 
        if (IsDeadlocked())
        {
            Debug.Log("DeadLocked!!! PLAYER LOSES!");
        }
        if (currentState != GameStatus.pause)
            currentState = GameStatus.move; 
        //Textual Gratification
        streakValue = 1;
    }


    private Vector2 CheckForAdjacent(int column, int row)
    {
        try
        {
            if (allDots[column + 1, row] && column < width - 1)
            {
                return Vector2.right;
            }
            if (allDots[column - 1, row] && column > 0)
            {
                return Vector2.left;
            }
            if (allDots[column, row + 1] && row < height - 1)
            {
                return Vector2.up;
            }
            if (allDots[column, row - 1] && row > 0)
            {
                return Vector2.down;
            }
        }
        catch
        {
            Debug.Log("Error Happened, Let's redo it.");
        }

        return Vector2.zero;
    }


    private void SwitchPieces(int column, int row, Vector2 direction)
    {
        if (allDots[column + (int)direction.x, row + (int)direction.y] != null)
        {
            //Take the second piece and save it in a holder
            GameObject holder = allDots[column + (int)direction.x, row + (int)direction.y] as GameObject;
            //Switching the first dot to be the second position
            allDots[column + (int)direction.x, row + (int)direction.y] = allDots[column, row];
            //Set the first dot to be the second dot
            allDots[column, row] = holder;
        }
    }

    public bool CheckForMatches()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    //Make sure that one and two to the right are in the board
                    if (i < width - 2)
                    {
                        //Check if the dots to the right and two to the right exists
                        if (allDots[i + 1, j] != null && allDots[i + 2, j] != null)
                        {
                            if (allDots[i + 1, j].tag == allDots[i, j].tag
                                && allDots[i + 2, j].tag == allDots[i, j].tag)
                            {
                                return true;
                            }
                        }
                    }
                    if (j < height - 2)
                    {
                        //Check if dots above exist
                        if (allDots[i, j + 1] != null && allDots[i, j + 2] != null)
                        {
                            if (allDots[i, j + 1].tag == allDots[i, j].tag
                                && allDots[i, j + 2].tag == allDots[i, j].tag)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
        }
        return false;
    }

    public bool SwitchAndCheck(int column, int row, Vector2 direction)
    {
        SwitchPieces(column, row, direction);
        if (CheckForMatches())
        {
            SwitchPieces(column, row, direction);
            return true;
        }
        SwitchPieces(column, row, direction);
        return false;
    }

    private bool IsDeadlocked()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    if (i < width - 1)
                    {
                        if (SwitchAndCheck(i, j, Vector2.right))
                        {
                            return false;
                        }
                    }
                    if (j < height - 1)
                    {
                        if (SwitchAndCheck(i, j, Vector2.up))
                        {
                            return false;
                        }
                    }
                }
            }
        }
        return true;
    }

}

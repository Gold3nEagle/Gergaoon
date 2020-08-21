using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessDot : MonoBehaviour
{
    [Header("Board Variables")]
    public int column, row, targetX, targetY, previousColumn, previousRow;
    public bool isMatched;
    public GameObject otherDot;

    [Header("Swipe Sensitivity")]
    public float swipeAngle;
    public float swipeResist = 1f;

    [Header("Powerups")]
    public bool isColumnBomb, isRowBomb, isColorBomb, isAdjacentBomb;
    public GameObject rowArrow, columnArrow, colorBomb, adjacentMarker, colorBombParticle, bombsParticle;

    private EndlessHint hintManager;
    private EndlessMatches findMatches;
    private EndlessManager endGameManager;
    private EndlessDamageDestroy damageDestroy;
    private EndlessBoard board;
    private Vector2 firstTouchPosition, finalTouchPosition, tempPosition;

    PowerUps powerUp;


    // Start is called before the first frame update
    void Start()
    {
        endGameManager = FindObjectOfType<EndlessManager>();
        powerUp = FindObjectOfType<PowerUps>();
        damageDestroy = FindObjectOfType<EndlessDamageDestroy>();

        board = GameObject.FindWithTag("Board").GetComponent<EndlessBoard>();

        hintManager = FindObjectOfType<EndlessHint>();
        findMatches = FindObjectOfType<EndlessMatches>();
    }

    //For testing and debug purposes
    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            isAdjacentBomb = true;
            GameObject marker = Instantiate(adjacentMarker, transform.position, Quaternion.identity);
            marker.transform.parent = this.transform;
        }
    }


    // Update is called once per frame
    void Update()
    {
        targetX = column;
        targetY = row;
        //For the X Axis
        if (Mathf.Abs(targetX - transform.position.x) > .1)
        {
            //Move Towards the target
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .4f);
            if (board.allDots[column, row] != this.gameObject)
            {
                board.allDots[column, row] = this.gameObject;
                findMatches.FindAllMatches();
            }
        }
        else
        {
            //Directly set the position
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = tempPosition;
        }

        //For the Y Axis
        if (Mathf.Abs(targetY - transform.position.y) > .1)
        {
            //Move Towards the target
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .4f);
            if (board.allDots[column, row] != this.gameObject)
            {
                board.allDots[column, row] = this.gameObject;
                findMatches.FindAllMatches();
            }

        }
        else
        {
            //Directly set the position
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = tempPosition;
        }
    }

    //Return dots to original place or destory them if they match.
    public IEnumerator CheckMoveCo()
    {
        if (isColorBomb)
        {
            findMatches.MatchPiecesOfColor(otherDot.tag);
            isMatched = true;
        }
        else if (otherDot.GetComponent<EndlessDot>().isColorBomb)
        {
            findMatches.MatchPiecesOfColor(gameObject.tag);
            otherDot.GetComponent<EndlessDot>().isMatched = true;
        }
        yield return new WaitForSeconds(.3f);
        if (otherDot != null)
        {
            if (!isMatched && !otherDot.GetComponent<EndlessDot>().isMatched)
            {
                otherDot.GetComponent<EndlessDot>().row = row;
                otherDot.GetComponent<EndlessDot>().column = column;
                row = previousRow;
                column = previousColumn;
                yield return new WaitForSeconds(.5f);
                board.currentDot = null;
                board.currentState = GameStatus.move;
            }
            else
            {
                if (endGameManager.requirements.gameType == GameType.Moves)
                {
                    endGameManager.DecreaseCounterValue();
                }
                damageDestroy.DestroyMatches();
            }
        }
    }

    private void OnMouseDown()
    {
        //DestroyHint
        if (hintManager != null)
        {
            hintManager.DestroyHint();
        }

        if (powerUp.toggleBoost)
        {
            //Check which powerup to select and do 
            if (powerUp.colorBombBoost)
            {
                BoostColorBomb();
            }
            else if (powerUp.destroyBoost)
            {
                BoostDestroyDot();
            }
            else if (powerUp.adjacentBombBoost)
            {
                BoostAdjacentBomb();
            }
        }

        if (board.currentState == GameStatus.move)
        {
            firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //  Debug.Log(firstTouchPosition);
        }
    }

    void BoostDestroyDot()
    {
        powerUp.DecreaseDestroyNum();
        Destroy(gameObject);
        board.FillBoard();

        powerUp.destroyBoost = false;
        powerUp.toggleBoost = false;
    }

    void BoostColorBomb()
    {
        powerUp.DecreaseColorNum();
        MakeColorBomb();

        powerUp.colorBombBoost = false;
        powerUp.toggleBoost = false;
    }

    void BoostAdjacentBomb()
    {
        powerUp.DecreaseAdjacentNum();
        MakeAdjacentBomb();

        powerUp.adjacentBombBoost = false;
        powerUp.toggleBoost = false;
    }

    private void OnMouseUp()
    {
        if (board.currentState == GameStatus.move)
        {
            finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalculateAngle();
        }
    }

    void CalculateAngle()
    {
        if (Mathf.Abs(finalTouchPosition.y - firstTouchPosition.y) > swipeResist || Mathf.Abs(finalTouchPosition.x - firstTouchPosition.x) > swipeResist)
        {
            board.currentState = GameStatus.wait;
            swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x) * 180 / Mathf.PI;
            // Debug.Log(swipeAngle);
            MovePieces();
            board.currentDot = this;
        }
        else
        {
            board.currentState = GameStatus.move;
        }
    }

    void MovePiecesActual(Vector2 direction)
    {
        otherDot = board.allDots[column + (int)direction.x, row + (int)direction.y];
        previousRow = row;
        previousColumn = column; 
            if (otherDot != null)
            {
                otherDot.GetComponent<EndlessDot>().column += -1 * (int)direction.x;
                otherDot.GetComponent<EndlessDot>().row += -1 * (int)direction.y;
                column += (int)direction.x;
                row += (int)direction.y;
                StartCoroutine(CheckMoveCo());
            }
            else
            {
                board.currentState = GameStatus.move;
            } 
    }

    void MovePieces()
    {
        if (swipeAngle > -45 && swipeAngle <= 45 && column < board.width - 1)
        {
            MovePiecesActual(Vector2.right);
        }
        else if (swipeAngle > -45 && swipeAngle <= 135 && row < board.height - 1)
        {
            MovePiecesActual(Vector2.up);
        }
        else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0)
        {
            MovePiecesActual(Vector2.left);
        }
        else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0)
        {
            MovePiecesActual(Vector2.down);
        }
        else
        {
            board.currentState = GameStatus.move;
        }
    }

    void FindMatches()
    {
        if (column > 0 && column < board.width - 1)
        {
            GameObject leftDot1 = board.allDots[column - 1, row];
            GameObject rightDot1 = board.allDots[column + 1, row];
            if (leftDot1 != null && rightDot1 != null)
            {
                if (leftDot1.tag == this.gameObject.tag && rightDot1.tag == this.gameObject.tag)
                {
                    leftDot1.GetComponent<EndlessDot>().isMatched = true;
                    rightDot1.GetComponent<EndlessDot>().isMatched = true;
                    isMatched = true;
                }
            }
        }

        if (row > 0 && row < board.height - 1)
        {
            GameObject upDot1 = board.allDots[column, row + 1];
            GameObject downDot1 = board.allDots[column, row - 1];
            if (upDot1 != null && downDot1 != null)
            {
                if (upDot1.tag == this.gameObject.tag && downDot1.tag == this.gameObject.tag)
                {
                    upDot1.GetComponent<EndlessDot>().isMatched = true;
                    downDot1.GetComponent<EndlessDot>().isMatched = true;
                    isMatched = true;
                }
            }
        }
    }

    public void MakeRowBomb()
    {
        if (!isColumnBomb && !isColorBomb && !isAdjacentBomb)
        {
            isRowBomb = true;
            GameObject arrow = Instantiate(rowArrow, transform.position, Quaternion.identity);
            GameObject particle = Instantiate(bombsParticle, transform.position, Quaternion.identity);
            particle.transform.parent = gameObject.transform;
            arrow.transform.parent = gameObject.transform;
        }
    }

    public void MakeColumnBomb()
    {
        if (!isRowBomb && !isColorBomb && !isAdjacentBomb)
        {
            isColumnBomb = true;
            GameObject arrow = Instantiate(columnArrow, transform.position, Quaternion.identity);
            GameObject particle = Instantiate(bombsParticle, transform.position, Quaternion.identity);
            particle.transform.parent = gameObject.transform;
            arrow.transform.parent = gameObject.transform;
        }
    }

    public void MakeColorBomb()
    {
        if (!isColumnBomb && !isRowBomb && !isAdjacentBomb)
        {
            isColorBomb = true;
            GameObject color = Instantiate(colorBomb, transform.position, Quaternion.identity);
            GameObject particle = Instantiate(colorBombParticle, transform.position, Quaternion.identity);
            particle.transform.parent = gameObject.transform;
            color.transform.parent = gameObject.transform;
            gameObject.tag = "Color";
        }
    }

    public void MakeAdjacentBomb()
    {
        if (!isColumnBomb && !isColorBomb && !isRowBomb)
        {
            isAdjacentBomb = true;
            GameObject marker = Instantiate(adjacentMarker, transform.position, Quaternion.identity);
            GameObject particle = Instantiate(bombsParticle, transform.position, Quaternion.identity);
            particle.transform.parent = gameObject.transform;
            marker.transform.parent = gameObject.transform;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AnimationController : MonoBehaviour
{

    public Animator animator, secondAnimator;
    public GameObject m_Object;
    string scene;
    int timesPlayed, IsRated;


    // Start is called before the first frame update
    void Start()
    {
        scene = SceneManager.GetActiveScene().name;
        timesPlayed = PlayerPrefs.GetInt("TP");
        IsRated = PlayerPrefs.GetInt("BeenRated");
        if (scene == "GameScene")
        {
            RateGame();
            //LBPointer();
        }
    }
      

    public void RateGame()
    {
        timesPlayed++;
        PlayerPrefs.SetInt("TP", timesPlayed);
        if (timesPlayed >= 1 && IsRated == 0) {  
        animator.SetBool("IsOpen", true);
        PlayerPrefs.SetInt("BeenRated", 1);
        }
    }

    public void ShowAdPanel()
    {
        animator.SetBool("IsOpen", true);
    }

    public void HideAdPanel()
    {
        animator.SetBool("IsOpen", false);
    }

    public void HideRateGame()
    {
        animator.SetBool("IsOpen", false);
        PlayerPrefs.SetInt("BeenRated", 1);

    }

    public void ShowOptions()
    {
        animator.SetBool("IsShown", true); 
        if(scene == "Bejeweled")
        {
            secondAnimator.SetBool("IsOpen", false);
            StartCoroutine(GameStartCo());
        }
    }

    IEnumerator GameStartCo()
    {
        Board board = FindObjectOfType<Board>();
        board.currentState = GameState.move;
        yield return new WaitForSeconds(0.5f); 
        secondAnimator.SetBool("IsGameOver", false);
    }

    public void GameOver()
    {
        animator.SetBool("IsOpen", true);
        secondAnimator.SetBool("IsGameOver", true);
    }

    public void Restart()
    {  
        StartCoroutine(GameStartCo()); 
    }
   
    public void HideOptions()
    {
        animator.SetBool("IsShown", false);
    }

    public void LBPointer()
    {
        if(timesPlayed == 1) {
        m_Object.SetActive(true);
        } 
    }
}

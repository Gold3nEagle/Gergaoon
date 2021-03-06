﻿using System.Collections;
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
    }
      

    public void RateGame()
    { 
        timesPlayed++;
        PlayerPrefs.SetInt("TP", timesPlayed);
        Debug.Log(timesPlayed +" Rated or not?   " +  IsRated);
  
        if(timesPlayed >= 100)
        {
            GPlayServices gPlay = FindObjectOfType<GPlayServices>();
            gPlay.UnlockAchievement(4);
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
        if (scene == "Bejeweled" || scene == "Endless")
        {
            secondAnimator.SetBool("IsOpen", false);
            StartCoroutine(GameStartCo());
        }
    }

    IEnumerator GameStartCo()
    {
        if(scene == "Bejeweled")
        {
            Board board = FindObjectOfType<Board>();
            board.currentState = GameStatus.move;
        } else
        {
            EndlessBoard board = FindObjectOfType<EndlessBoard>();
            board.currentState = GameStatus.move;
        } 
        yield return new WaitForSeconds(0.5f); 
        secondAnimator.SetBool("IsGameOver", false);
    } 

    public void GameOver()
    {
        //secondAnimator.SetBool("IsGameOver", true);
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{

    public Animator animator;
    public GameObject m_Object;
    int timesPlayed, IsRated;

    // Start is called before the first frame update
    void Start()
    {
        timesPlayed = PlayerPrefs.GetInt("TP");
        IsRated = PlayerPrefs.GetInt("BeenRated");
        RateGame();
        LBPointer();
    }
      

    public void RateGame()
    {
        if (timesPlayed >= 2 && IsRated == 0) {  
        animator.SetBool("IsOpen", true);
        }
    }

    public void HideRateGame()
    {
        animator.SetBool("IsOpen", false);
    }

    public void ShowOptions()
    {
        animator.SetBool("IsShown", true);
         
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

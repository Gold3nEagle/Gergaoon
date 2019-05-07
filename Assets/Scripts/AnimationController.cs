using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{

    public Animator animator;
    int timesPlayed, IsRated;

    // Start is called before the first frame update
    void Start()
    {
        timesPlayed = PlayerPrefs.GetInt("TP");
        IsRated = PlayerPrefs.GetInt("BeenRated");
        RateGame();
    }
      

    public void RateGame()
    {
        if (timesPlayed >= 1 && IsRated == 0) {  
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
}

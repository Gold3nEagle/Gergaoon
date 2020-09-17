﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterAnimation : MonoBehaviour
{

    public Animator boyAnim, girlAnim;

    private void Awake()
    {

        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            IdleAnimation();
        } else if(SceneManager.GetActiveScene().name == "GameScene")
        {
            HappyAnimation();
        }
         
    } 

    public void SadAnimation()
    {
        boyAnim.SetBool("IsSad", true);
        girlAnim.SetBool("IsSad", true);
    }

    public void HappyAnimation()
    {
        boyAnim.SetBool("IsHappy", true);
        girlAnim.SetBool("IsHappy", true);
    }

    public void IdleAnimation()
    {

        boyAnim.SetBool("IsHappy", false);
        girlAnim.SetBool("IsHappy", false); 
        boyAnim.SetBool("IsSad", false);
        girlAnim.SetBool("IsSad", false);
    }

}

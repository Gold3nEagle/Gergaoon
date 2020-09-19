using System.Collections;
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
        foreach (AnimatorControllerParameter parameter in boyAnim.parameters)
        {
            boyAnim.SetBool(parameter.name, false);
        }

        boyAnim.SetBool("IsSad", true);
        girlAnim.SetBool("IsSad", true); 
    }

    public void HappyAnimation()
    {
        foreach (AnimatorControllerParameter parameter in boyAnim.parameters)
        {
            boyAnim.SetBool(parameter.name, false);
        }

        boyAnim.SetBool("IsHappy", true);
        girlAnim.SetBool("IsHappy", true); 
    }

    public void IdleAnimation()
    {

        foreach (AnimatorControllerParameter parameter in boyAnim.parameters)
        {
            boyAnim.SetBool(parameter.name, false);
        }

        boyAnim.SetBool("IsIdle", true);
        girlAnim.SetBool("IsIdle", true); 
    }

    public void WavingAnimation()
    {
        foreach (AnimatorControllerParameter parameter in boyAnim.parameters)
        {
            boyAnim.SetBool(parameter.name, false);
        }

        boyAnim.SetBool("IsWaving", true);
        girlAnim.SetBool("IsWaving", true); 
    }

}

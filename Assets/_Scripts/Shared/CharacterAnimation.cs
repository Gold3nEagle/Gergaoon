using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterAnimation : MonoBehaviour
{

    public Animator boyAnim, girlAnim;
    Character character;
    CharacterState characterState;

    private void Awake()
    {

        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            SetAnimation(boyAnim, "IsIdle");
            SetAnimation(girlAnim, "IsWaving");
        } else if(SceneManager.GetActiveScene().name == "GameScene")
        {
            SetAnimation(boyAnim, "IsHappy");
            SetAnimation(girlAnim, "IsHappy");
        }
         
    }  

    public void SetAnimation(Animator animator, string animationString)
    {
        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            animator.SetBool(parameter.name, false);
        }

        animator.SetBool(animationString, true);

    }

}

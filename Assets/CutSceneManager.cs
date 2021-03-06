﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum CharacterState
{
    IsIdle,
    IsWaving,
    IsSad,
    IsHappy
}

public enum FaceState
{
   Smile,
   Speaking,
   Sad,
   Happy
}

public enum Character
{
    Boy,
    Girl
}

public class CutSceneManager : MonoBehaviour
{

    public Animator boyAnim, girlAnim;
    public SpriteRenderer boyFace, girlFace, backgroundRenderer;
    public Sprite[] boyFaceSprites, girlFaceSprites, backgroundSprites; 
    public GameObject[] cutScenes;
    int cutScene;

    // Start is called before the first frame update
    void Awake()
    {
        cutScene = PlayerPrefs.GetInt("cutScene");
        SetBackground(cutScene); 
        cutScenes[cutScene].SetActive(true);
        InvokeRepeating("CheckSceneLoaded", 3, 3);

    }
     

    void CheckSceneLoaded()
    { 
        if (cutScenes[cutScene].active == false)
        {
            cutScenes[cutScene].SetActive(true);
        }
    }


    public void AnimateCharacter(Character character, FaceState faceState, CharacterState animationState )
    {

        Animator selectedAnimator = null;
        SpriteRenderer selectedFace = null;
        Sprite[] selectedFaceSprites = null;

        if (character == Character.Boy)
        {
            selectedAnimator = boyAnim;
            selectedFace = boyFace;
            selectedFaceSprites = boyFaceSprites;

        } else if(character == Character.Girl)
        {
            selectedAnimator = girlAnim;
            selectedFace = girlFace;
            selectedFaceSprites = girlFaceSprites;
        } 


            switch (faceState)
            {
                case FaceState.Smile:
                selectedFace.sprite = selectedFaceSprites[0];
                    break;
                case FaceState.Speaking:
                selectedFace.sprite = selectedFaceSprites[1];
                break;
                case FaceState.Happy:
                selectedFace.sprite = selectedFaceSprites[2];
                break;
                case FaceState.Sad:
                selectedFace.sprite = selectedFaceSprites[3];
                break; 
            }

            switch (animationState)
            {
                case CharacterState.IsIdle:
                SetAnimation(selectedAnimator, "IsIdle");
                    break;
                case CharacterState.IsWaving:
                SetAnimation(selectedAnimator, "IsWaving");
                break;
                case CharacterState.IsSad:
                SetAnimation(selectedAnimator, "IsSad");
                break;
                case CharacterState.IsHappy:
                SetAnimation(selectedAnimator, "IsHappy");
                break; 
        } 
    }


    void SetAnimation(Animator animator, string animationString)
    { 
        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            animator.SetBool(parameter.name, false);
        }

        animator.SetBool(animationString, true);

    }

    public int CheckLocale()
    {
        SystemLanguage systemLanguage = Application.systemLanguage;

        if(systemLanguage == SystemLanguage.Arabic)
        {
            return 1;
        } else
        {
            return 2;
        }


    }

    void SetBackground(int cutScene)
    {
        switch (cutScene)
        {
            case 0:
                backgroundRenderer.sprite = backgroundSprites[2];
                break;

            case 1:
                backgroundRenderer.sprite = backgroundSprites[3];
                break;

            case 2:
                backgroundRenderer.sprite = backgroundSprites[1];
                break;
        } 
    }
}

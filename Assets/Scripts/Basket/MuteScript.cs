﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MuteScript : MonoBehaviour
{

    private bool toggle = false;
    public Sprite sound, mute;
    public Image buttonImage;

    // Start is called before the first frame update
    void Start()
    {
        buttonImage = GetComponent<Image>();
    }

    public void MuteAll()
    {

        if (toggle == false)
        {
            AudioListener.volume = 0f;
            buttonImage.sprite = mute;
            toggle = true;
        }
        else
        {
            AudioListener.volume = 1f;
            buttonImage.sprite = sound;
            toggle = false;
        }
    }


    public void PPAccess()
    {
        Application.OpenURL("http://www.geagle.tech/gergaoon-privacy-policy");
    } 

    public void RateGame()
    {
        Application.OpenURL("market://details?id=com.goldeneagle.gergaoon");
        PlayerPrefs.SetInt("BeenRated", 1);

        int totalCandy = PlayerPrefs.GetInt("totalCandy");
        totalCandy += 10;
        PlayerPrefs.SetInt("totalCandy", totalCandy);

    }
     

}